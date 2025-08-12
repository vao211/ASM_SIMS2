using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebSIMS.Models.Entities;

namespace WebSIMS.Models.Entities
{
    [Table("Faculty")]
    public class Faculty
    {
        [Key]
        public int FacultyID { get; set; }
        
        public int? UserID { get; set; }

        [Required(ErrorMessage = "Name cannot be empty.")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name cannot be empty.")]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email cannot be empty.")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Email invalid")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hire Date cannot be empty")]
        [Display(Name = "Hire Date")]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }
        
        public Users? User { get; set; }
    }
}