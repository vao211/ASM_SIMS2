namespace WebSIMS.Models.ViewModels.Dashboard;

public class DashboardViewModel
{
    public int TotalStudents { get; set; }
    public int TotalCourses { get; set; }

    public List<CourseInfo> Courses { get; set; }
    public string Role { get; set; }

    public class CourseInfo
    {
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
    }
}