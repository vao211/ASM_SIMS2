using Microsoft.EntityFrameworkCore;
using WebSIMS.Models.Entities;

namespace WebSIMS.Data 
{
    public class SIMSdbContext : DbContext
    {
        public SIMSdbContext(DbContextOptions<SIMSdbContext> options) : base(options) { }

        public DbSet<Courses> CoursesDb { get; set; }
        public DbSet<Users> UsersDb { get; set; }
        public DbSet<Student> StudentsDb { get; set; }
        public DbSet<Faculty> FacultyDb { get; set; }
        public DbSet<Enrollments> EnrollmentsDb { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Student>().ToTable("Students");
            modelBuilder.Entity<Student>().HasKey(s => s.StudentID);
            
            modelBuilder.Entity<Faculty>().ToTable("Faculty");
            modelBuilder.Entity<Faculty>().HasKey(l => l.FacultyID);
            
            modelBuilder.Entity<Users>().ToTable("Users");
            modelBuilder.Entity<Users>().HasKey("UserID");
            modelBuilder.Entity<Users>().HasIndex("Username").IsUnique();
            modelBuilder.Entity<Users>().Property("Role").HasDefaultValue("Student");
            
            modelBuilder.Entity<Courses>().ToTable("Courses");
            modelBuilder.Entity<Courses>().HasKey("CourseID");
            modelBuilder.Entity<Courses>().HasIndex("CourseCode").IsUnique();
            
            modelBuilder.Entity<Student>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey("UserID");
            
            modelBuilder.Entity<Faculty>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserID);
            
            modelBuilder.Entity<Enrollments>().ToTable("Enrollments");
            modelBuilder.Entity<Enrollments>().HasKey(e => e.EnrollmentID);
            
            modelBuilder.Entity<Enrollments>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(sc => sc.StudentID);
            
            modelBuilder.Entity<Enrollments>()
                .HasOne(sc => sc.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(sc => sc.CourseID);
        }
    }
}