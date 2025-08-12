using WebSIMS.Models.Entities;

namespace WebSIMS.Services.Interfaces;

public interface IEnrollmentService
{
    Task<List<Enrollments>> GetAllEnrollmentsAsync();
    Task<List<Enrollments>> GetEnrollmentsByStudentAsync(int studentId);
    Task<List<Enrollments>> GetEnrollmentsByCourseAsync(int courseId);
    Task<bool> EnrollStudentInCourseAsync(int studentId, int courseId);
    Task<bool> RemoveEnrollmentAsync(int studentId, int courseId);
    Task<bool> UpdateGradeAsync(int studentId, int courseId, string grade);
    Task<Enrollments?> GetEnrollmentAsync(int studentId, int courseId);
}