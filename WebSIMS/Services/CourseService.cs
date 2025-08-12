using Microsoft.EntityFrameworkCore;
using WebSIMS.Data;
using WebSIMS.Models.Entities;
using WebSIMS.Services.Interfaces;

namespace WebSIMS.Services;

public class CourseService(SIMSdbContext context) : ICourseService
{
    public async Task<List<Courses>> GetAllCoursesAsync()
        {
            return await context.CoursesDb.ToListAsync();
        }

        public async Task<Courses?> GetCourseByIdAsync(int id)
        {
            return await context.CoursesDb.FindAsync(id);
        }

        public async Task<bool> AddCourseAsync(Courses course)
        {
            try
            {
                course.CreatedAt = DateTime.Now;
                context.CoursesDb.Add(course);
                await context.SaveChangesAsync();
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateCourseAsync(Courses course)
        {
            try
            {
                var existingCourse = await context.CoursesDb.FindAsync(course.CourseID);
                if (existingCourse == null)
                    return false;
                existingCourse.CourseCode = course.CourseCode;
                existingCourse.CourseName = course.CourseName;
                existingCourse.Description = course.Description;
                existingCourse.Credits = course.Credits;
                existingCourse.Department = course.Department;
                existingCourse.CreatedAt = DateTime.Now;
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            try
            {
                var course = await context.CoursesDb.FindAsync(id);
                if (course == null)
                {
                    return false;
                }
                context.CoursesDb.Remove(course);
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
    