using Microsoft.EntityFrameworkCore;
using WebSIMS.Data;
using WebSIMS.Models.Entities;
using WebSIMS.Services.Interfaces;

namespace WebSIMS.Services;

public class StudentService :  IStudentService
{
     private readonly SIMSdbContext _context;

        public StudentService(SIMSdbContext context)
        {
            _context = context;
        }

        public async Task<List<Student>> GetAllStudentsAsync()
        {
            return await _context.StudentsDb.ToListAsync();
        }

        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            return await _context.StudentsDb
                .FirstOrDefaultAsync(s => s.UserID == id);
            
        }

        public async Task<bool> AddStudentAsync(Student student)
        {
            try
            {
                student.EnrollmentDate ??= DateTime.Now;
                _context.StudentsDb.Add(student);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateStudentAsync(Student student)
        {
            try
            {
                var existingStudent = await _context.StudentsDb.FindAsync(student.StudentID);
                if (existingStudent == null)
                    return false;

                existingStudent.StudentCode = student.StudentCode;
                existingStudent.FirstName = student.FirstName;
                existingStudent.LastName = student.LastName;
                existingStudent.DateOfBirth = student.DateOfBirth;
                existingStudent.Gender = student.Gender;
                existingStudent.Email = student.Email;
                existingStudent.Phone = student.Phone;
                existingStudent.Address = student.Address;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            try
            {
                var student = await _context.StudentsDb.FindAsync(id);
                if (student == null)
                    return false;

                _context.StudentsDb.Remove(student);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsStudentCodeExistsAsync(string studentCode, int excludeStudentId = 0)
        {
            return await _context.StudentsDb
                .AnyAsync(s => s.StudentCode == studentCode && s.StudentID != excludeStudentId);
        }
        
        public async Task<Student?> GetStudentByUserIdAsync(string userId)
        {
            return await _context.StudentsDb
                .FirstOrDefaultAsync(s => s.StudentID.ToString().Equals(userId));
        }
    }
