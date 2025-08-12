using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebSIMS.Data;
using WebSIMS.Models.ViewModels.Dashboard;

namespace WebSIMS.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly SIMSdbContext _context;

    public DashboardController(SIMSdbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Admin, Student, Faculty")]
    public IActionResult Index()
    {
        var totalStudents = _context.StudentsDb.Count();
        var totalCourses = _context.CoursesDb.Count();

        var courses = _context.CoursesDb.Select(c => new DashboardViewModel.CourseInfo
        {
            CourseCode = c.CourseCode,
            CourseName = c.CourseName
        }).ToList();


        var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "N/A";

        var model = new DashboardViewModel
        {
            TotalStudents = totalStudents,
            TotalCourses = totalCourses,
            Courses = courses,
            Role = userRole
        };

        return View(model);
    }
}