using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebSIMS.Data;
using WebSIMS.Models.Entities;
using WebSIMS.Services.Interfaces;

namespace WebSIMS.Controllers;

  [Authorize(Roles = "Admin")]
    public class FacultyController : Controller
    {
        private readonly IFacultyService _facultyService;
        private readonly SIMSdbContext _context;

        public FacultyController(IFacultyService facultyService, SIMSdbContext context)
        {
            _facultyService = facultyService;
            _context = context; 
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,HireDate")] Faculty faculty, string password)
        {
            if (ModelState.IsValid)
            {
                try
                {
               
                    var existingUser = await _context.UsersDb.FirstOrDefaultAsync(u => u.Username == faculty.Email);
                    if (existingUser != null)
                    {
                        ModelState.AddModelError("Email", "Email already exists as a username.");
                        return View(faculty);
                    }

                    var user = new Users
                    {
                        Role = "Faculty",
                        Username = faculty.Email,
                        PasswordHash = password
                    };
                    
                    _context.UsersDb.Add(user);
                    await _context.SaveChangesAsync();
                    
                    faculty.UserID = user.UserID;
                    
                    await _facultyService.AddAsync(faculty);

                    TempData["SuccessMessage"] = "Faculty created successfully! Username: " + faculty.Email + ", Password: " + password;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error creating faculty: {ex.Message}";
                    return View(faculty);
                }
            }
            return View(faculty);
        }

        public async Task<IActionResult> Index()
        {
            var faculties = await _facultyService.GetAllAsync();
            return View(faculties);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var faculty = await _facultyService.GetByIdAsync(id.Value);
            if (faculty == null) return NotFound();
            return View(faculty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FacultyID,FirstName,LastName,Email,HireDate")] Faculty faculty)
        {
            if (id != faculty.FacultyID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var facultyToUpdate = await _facultyService.GetByIdAsync(id);
                    if (facultyToUpdate == null) return NotFound();

                    facultyToUpdate.FirstName = faculty.FirstName;
                    facultyToUpdate.LastName = faculty.LastName;
                    facultyToUpdate.HireDate = faculty.HireDate;

                    if (facultyToUpdate.Email != faculty.Email)
                    {
                        facultyToUpdate.Email = faculty.Email;
                        if (facultyToUpdate.User != null)
                        {
                            facultyToUpdate.User.Username = faculty.Email;
                        }
                    }

                    await _facultyService.UpdateAsync(facultyToUpdate);
                    TempData["SuccessMessage"] = "Update Complete";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"{ex.Message}";
                    return View(faculty);
                }
            }
            return View(faculty);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var faculty = await _facultyService.GetByIdAsync(id.Value);
            if (faculty == null) return NotFound();
            return View(faculty);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _facultyService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Delete Complete";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Delete Error, {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }