using System.ComponentModel.DataAnnotations;

namespace WebSIMS.Models.Entities
{
    public class Enrollments
    {
        [Key]
        public int EnrollmentID { get; set; }

        [Required]
        public int StudentID { get; set; }

        [Required]
        public int CourseID { get; set; }

        public DateTime? EnrollmentDate { get; set; }

        public string? Grade { get; set; }
        
        public Student? Student { get; set; }
        public Courses? Course { get; set; }
    }
}
