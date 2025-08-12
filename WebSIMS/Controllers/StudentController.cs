using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebSIMS.Models.Entities;
using WebSIMS.Services.Interfaces;

namespace WebSIMS.Controllers;

  [Authorize(Roles = "Admin,Faculty,Student")]
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;

        public StudentController(IStudentService studentService, ICourseService courseService)
        {
            _studentService = studentService;
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var students = await _studentService.GetAllStudentsAsync();
            return View(students);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            ViewBag.Courses = new SelectList(courses, "CourseName", "CourseName");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            if (string.IsNullOrWhiteSpace(student.StudentCode))
            {
                ModelState.AddModelError(nameof(student.StudentCode), "Student Code is required.");
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(student.StudentCode, @"^[A-Z0-9]+$"))
            {
                ModelState.AddModelError(nameof(student.StudentCode), "Student Code must contain only uppercase letters and numbers.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (await _studentService.IsStudentCodeExistsAsync(student.StudentCode))
                    {
                        ModelState.AddModelError(nameof(student.StudentCode), "Student code already exists in the system.");
                        var courses = await _courseService.GetAllCoursesAsync();
                        ViewBag.Courses = new SelectList(courses, "CourseName", "CourseName");
                        return View(student);
                    }

                    var userIdClaim = User.FindFirst("UserID")?.Value;

                    if (!int.TryParse(userIdClaim, out int userId))
                    {
                        ModelState.AddModelError(string.Empty, "Error: Unable to identify the user. Please log in again.");
                        var courses = await _courseService.GetAllCoursesAsync();
                        ViewBag.Courses = new SelectList(courses, "CourseName", "CourseName");
                        return View(student);
                    }

                    student.UserID = userId;
                    var result = await _studentService.AddStudentAsync(student);
                    if (!result)
                    {
                        TempData["ErrorMessage"] = "Failed to add student.";
                        return RedirectToAction("Index");
                    }
                    TempData["SuccessMessage"] = "Successfully added a new student.";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException?.Message.Contains("duplicate key") == true)
                    {
                        ModelState.AddModelError(nameof(student.StudentCode), "Student code already exists in the system.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, $"System error: Could not add student. {ex.Message}");
                    }
                    var courses = await _courseService.GetAllCoursesAsync();
                    ViewBag.Courses = new SelectList(courses, "CourseName", "CourseName");
                    return View(student);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error creating student: {ex.Message}");
                    var courses = await _courseService.GetAllCoursesAsync();
                    ViewBag.Courses = new SelectList(courses, "CourseName", "CourseName");
                    return View(student);
                }
            }
            var failCourse = await _courseService.GetAllCoursesAsync();
            ViewBag.Courses = new SelectList(failCourse, "CourseName", "CourseName");
            return View(student);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
                return NotFound();

            var courses = await _courseService.GetAllCoursesAsync();
            ViewBag.Courses = new SelectList(courses, "CourseName", "CourseName");

            return View(student);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Student student)
        {
            if (string.IsNullOrWhiteSpace(student.StudentCode))
            {
                ModelState.AddModelError(nameof(student.StudentCode), "Student Code is required.");
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(student.StudentCode, @"^[A-Z0-9]+$"))
            {
                ModelState.AddModelError(nameof(student.StudentCode), "Student Code must contain only uppercase letters and numbers.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingStudent = await _studentService.GetStudentByIdAsync(student.StudentID);
                    if (existingStudent == null)
                    {
                        TempData["ErrorMessage"] = "Student to update not found.";
                        return RedirectToAction("Index");
                    }
                    
                    if (await _studentService.IsStudentCodeExistsAsync(student.StudentCode, student.StudentID))
                    {
                        ModelState.AddModelError(nameof(student.StudentCode), "Student code already exists in the system.");
                        var courses = await _courseService.GetAllCoursesAsync();
                        ViewBag.Courses = new SelectList(courses, "CourseName", "CourseName");
                        return View(student);
                    }

                    var result = await _studentService.UpdateStudentAsync(student);
                    if (!result)
                    {
                        TempData["ErrorMessage"] = "Failed to update student.";
                        return RedirectToAction("Index");
                    }
                    TempData["SuccessMessage"] = "Student information updated successfully.";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError(string.Empty, "Student data has been modified. Please try again.");
                    var courses = await _courseService.GetAllCoursesAsync();
                    ViewBag.Courses = new SelectList(courses, "CourseName", "CourseName");
                    return View(student);
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException?.Message.Contains("duplicate key") == true)
                    {
                        ModelState.AddModelError(nameof(student.StudentCode), "Student code already exists in the system.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, $"System error: Could not update student. {ex.Message}");
                    }
                    var courses = await _courseService.GetAllCoursesAsync();
                    ViewBag.Courses = new SelectList(courses, "CourseName", "CourseName");
                    return View(student);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error updating student: {ex.Message}");
                    var courses = await _courseService.GetAllCoursesAsync();
                    ViewBag.Courses = new SelectList(courses, "CourseName", "CourseName");
                    return View(student);
                }
            }
            var coursesList = await _courseService.GetAllCoursesAsync();
            ViewBag.Courses = new SelectList(coursesList, "CourseName", "CourseName");
            return View(student);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
                return NotFound();

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var student = await _studentService.GetStudentByIdAsync(id);
                if (student == null)
                {
                    TempData["ErrorMessage"] = "Student to delete not found.";
                    return RedirectToAction("Index");
                }

                var result = await _studentService.DeleteStudentAsync(id);
                if (!result)
                {
                    TempData["ErrorMessage"] = "Failed to delete student.";
                    return RedirectToAction("Index");
                }
                TempData["SuccessMessage"] = "Student deleted successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting student: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }