using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebSIMS.Models.Entities;
using WebSIMS.Services.Interfaces;

namespace WebSIMS.Controllers;
[Authorize(Roles = "Admin,Faculty, Student")]
public class CourseController : Controller
{
     private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return View(courses);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction(nameof(Index));
            }

            var allCourses = await _courseService.GetAllCoursesAsync();
            var filteredCourses = allCourses.Where(c => 
                c.CourseCode.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                c.CourseName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                c.Department.Contains(query, StringComparison.OrdinalIgnoreCase)
            ).ToList();

            ViewBag.SearchQuery = query;
            return View("Index", filteredCourses);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseCode,CourseName,Description,Credits,Department")] Courses course)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _courseService.AddCourseAsync(course);
                    if (!result)
                    {
                        TempData["ErrorMessage"] = "Failed to add course.";
                        return View(course);
                    }
                    TempData["SuccessMessage"] = "Course added successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException?.Message.Contains("duplicate key") == true)
                    {
                        ModelState.AddModelError("CourseCode", "Course code already exists.");
                        TempData["ErrorMessage"] = "Course code already exists.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = $"An unexpected error occurred: {ex.Message}";
                    }
                    return View(course);
                }
            }
            TempData["ErrorMessage"] = "Please correct the errors in the form.";
            return View(course);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Course not found.";
                return RedirectToAction(nameof(Index));
            }

            var course = await _courseService.GetCourseByIdAsync(id.Value);
            if (course == null)
            {
                TempData["ErrorMessage"] = "Course not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CourseID,CourseCode,CourseName,Description,Credits,Department,CreatedAt")] Courses course)
        {
            if (id != course.CourseID)
            {
                TempData["ErrorMessage"] = "Data mismatch.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _courseService.UpdateCourseAsync(course);
                    if (!result)
                    {
                        TempData["ErrorMessage"] = "Failed to update course.";
                        return View(course);
                    }
                    TempData["SuccessMessage"] = "Course updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["ErrorMessage"] = "The course has been modified by another user. Please try again.";
                    return View(course);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"An unexpected error occurred: {ex.Message}";
                    return View(course);
                }
            }
            TempData["ErrorMessage"] = "Please correct the errors in the form.";
            return View(course);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Course not found.";
                return RedirectToAction(nameof(Index));
            }

            var course = await _courseService.GetCourseByIdAsync(id.Value);
            if (course == null)
            {
                TempData["ErrorMessage"] = "Course not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _courseService.DeleteCourseAsync(id);
                if (!result)
                {
                    TempData["ErrorMessage"] = "Failed to delete course.";
                    return RedirectToAction(nameof(Index));
                }
                TempData["SuccessMessage"] = "Course deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while deleting the course: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
}