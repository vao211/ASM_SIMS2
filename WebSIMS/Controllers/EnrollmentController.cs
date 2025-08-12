using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebSIMS.Services.Interfaces;

namespace WebSIMS.Controllers;

    [Authorize(Roles = "Admin,Faculty,Student")]
    public class EnrollmentController : Controller
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;

        public EnrollmentController(
            IEnrollmentService enrollmentService,
            IStudentService studentService,
            ICourseService courseService)
        {
            _enrollmentService = enrollmentService;
            _studentService = studentService;
            _courseService = courseService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // if (User.IsInRole("Student"))
            // {
            //     var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
            //     if (string.IsNullOrEmpty(userId))
            //     {
            //         TempData["ErrorMessage"] = "Cannot find Id";
            //         return RedirectToAction("Index", "Dashboard");
            //     }
            //
            //     var student = await _studentService.GetStudentByIdAsync(int.Parse(userId));
            //     if (student == null)
            //     {
            //         TempData["ErrorMessage"] = "Cannot found student";
            //         return RedirectToAction("Index", "Dashboard");
            //     }
            //
            //     var enrollments = await _enrollmentService.GetEnrollmentsByStudentAsync(student.StudentID);
            //     ViewBag.Student = student;
            //     return View(enrollments);
            // }
                 var enrollments = await _enrollmentService.GetAllEnrollmentsAsync();
                var students = await _studentService.GetAllStudentsAsync();
                var courses = await _courseService.GetAllCoursesAsync();

                ViewBag.Students = students;
                ViewBag.Courses = courses;
                return View(enrollments);

        }
        
        public async Task<IActionResult> StudentEnrollments(int studentId)
        {
            var enrollments = await _enrollmentService.GetEnrollmentsByStudentAsync(studentId);
            var student = await _studentService.GetStudentByIdAsync(studentId);
            
            ViewBag.Student = student;
            return View(enrollments);
        }
        
        public async Task<IActionResult> CourseEnrollments(int courseId)
        {
            var enrollments = await _enrollmentService.GetEnrollmentsByCourseAsync(courseId);
            var course = await _courseService.GetCourseByIdAsync(courseId);
            
            ViewBag.Course = course;
            return View(enrollments);
        }
        
        [HttpPost]
        [Authorize(Roles = "Admin,Faculty")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int studentId, int courseId)
        {
            if (courseId == 0)
            {
                var courses = await _courseService.GetAllCoursesAsync();
                var successCount = 0;
                
                foreach (var course in courses)
                {
                    var result = await _enrollmentService.EnrollStudentInCourseAsync(studentId, course.CourseID);
                    if (result) successCount++;
                }
                
                if (successCount > 0)
                    TempData["SuccessMessage"] = $"Successfully enrolled student in {successCount} courses!";
                else
                    TempData["ErrorMessage"] = "Could not enroll student in any courses!";
            }
            else
            {
                var result = await _enrollmentService.EnrollStudentInCourseAsync(studentId, courseId);
                
                if (result)
                    TempData["SuccessMessage"] = "Enrollment successful!";
                else
                    TempData["ErrorMessage"] = "Enrollment failed!";
            }
            
            return RedirectToAction("Index");
        }
        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveEnrollment(int studentId, int courseId)
        {
            var result = await _enrollmentService.RemoveEnrollmentAsync(studentId, courseId);
            
            if (result)
                TempData["SuccessMessage"] = "Enrollment cancellation successful!";
            else
                TempData["ErrorMessage"] = "Enrollment cancellation failed!";
            
            return RedirectToAction("Index");
        }
        
        [HttpGet]
        [Authorize(Roles = "Faculty")]
        public async Task<IActionResult> UpdateGrade(int studentId, int courseId)
        {
            var enrollment = await _enrollmentService.GetEnrollmentAsync(studentId, courseId);
            if (enrollment == null)
            {
                TempData["ErrorMessage"] = "Enrollment not found!";
                return RedirectToAction("Index");
            }
            
            return View(enrollment);
        }

        [HttpPost]
        [Authorize(Roles = "Faculty")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateGrade(int studentId, int courseId, string grade)
        {
            var result = await _enrollmentService.UpdateGradeAsync(studentId, courseId, grade);
            
            if (result)
                TempData["SuccessMessage"] = "Grade updated successfully!";
            else
                TempData["ErrorMessage"] = "Grade update failed!";
            
            return RedirectToAction("Index");
        }
    }