using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WebSIMS.Models.Entities
{
    public class Users
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        public required string Username { get; set; }

        [Required]
        public required string PasswordHash { get; set; }

        [Required]
        public required string Role { get; set; }

        [AllowNull]
        public DateTime? CreatedAt { get; set; }
    }
}