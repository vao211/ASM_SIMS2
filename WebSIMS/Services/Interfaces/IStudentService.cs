using WebSIMS.Models.Entities;

namespace WebSIMS.Services.Interfaces;

public interface IStudentService
{
    Task<List<Student>> GetAllStudentsAsync();
    Task<Student?> GetStudentByIdAsync(int id);
    Task<bool> AddStudentAsync(Student student);
    Task<bool> UpdateStudentAsync(Student student);
    Task<bool> DeleteStudentAsync(int id);
    Task<bool> IsStudentCodeExistsAsync(string studentCode, int excludeStudentId = 0);
}