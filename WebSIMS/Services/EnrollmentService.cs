using Microsoft.EntityFrameworkCore;
using WebSIMS.Data;
using WebSIMS.Models;
using WebSIMS.Models.Entities;
using WebSIMS.Services.Interfaces;

namespace WebSIMS.Services;

    public class EnrollmentService : IEnrollmentService
    {
        private readonly SIMSdbContext _context;

        public EnrollmentService(SIMSdbContext context)
        {
            _context = context;
        }

        public async Task<List<Enrollments>> GetAllEnrollmentsAsync()
        {
            return await _context.EnrollmentsDb
                .Include(sc => sc.Student)
                .Include(sc => sc.Course)
                .ToListAsync();
        }

        public async Task<List<Enrollments>> GetEnrollmentsByStudentAsync(int studentId)
        {
            return await _context.EnrollmentsDb
                .Include(sc => sc.Course)
                .Where(sc => sc.StudentID == studentId)
                .ToListAsync();
        }

        public async Task<List<Enrollments>> GetEnrollmentsByCourseAsync(int courseId)
        {
            return await _context.EnrollmentsDb
                .Include(sc => sc.Student)
                .Where(sc => sc.CourseID == courseId)
                .ToListAsync();
        }

        public async Task<bool> EnrollStudentInCourseAsync(int studentId, int courseId)
        {
            var student = await _context.StudentsDb.FindAsync(studentId);
            var course = await _context.CoursesDb.FindAsync(courseId);

            if (student == null || course == null)
                return false;
            
            var existingEnrollment = await _context.EnrollmentsDb
                .FirstOrDefaultAsync(sc => sc.StudentID == studentId && sc.CourseID == courseId);

            if (existingEnrollment != null)
                return false;
            
            var enrollment = new Enrollments
            {
                StudentID = studentId,
                CourseID = courseId,
                EnrollmentDate = DateTime.Now
            };

            _context.EnrollmentsDb.Add(enrollment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveEnrollmentAsync(int studentId, int courseId)
        {
            var enrollment = await _context.EnrollmentsDb
                .FirstOrDefaultAsync(sc => sc.StudentID == studentId && sc.CourseID == courseId);

            if (enrollment == null)
                return false;

            _context.EnrollmentsDb.Remove(enrollment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateGradeAsync(int studentId, int courseId, string grade)
        {
            var enrollment = await _context.EnrollmentsDb
                .FirstOrDefaultAsync(sc => sc.StudentID == studentId && sc.CourseID == courseId);

            if (enrollment == null)
                return false;

            enrollment.Grade = grade;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Enrollments?> GetEnrollmentAsync(int studentId, int courseId)
        {
            return await _context.EnrollmentsDb
                .Include(sc => sc.Student)
                .Include(sc => sc.Course)
                .FirstOrDefaultAsync(sc => sc.StudentID == studentId && sc.CourseID == courseId);
        }
    }