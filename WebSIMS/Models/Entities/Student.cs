using System.ComponentModel.DataAnnotations;

namespace WebSIMS.Models.Entities
{
    public class Student
    {
        [Key]
        public int StudentID { get; set; }

        [Required(ErrorMessage = "Student Code is required")]
        [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "Student Code must contain only uppercase letters and numbers")]
        [Display(Name = "Student Code")]
        public string StudentCode { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
        

        public DateTime? EnrollmentDate { get; set; }
        
        public int UserID { get; set; }
        
        public Users? User { get; set; }
        public ICollection<Enrollments>? Enrollments { get; set; }
    }
}