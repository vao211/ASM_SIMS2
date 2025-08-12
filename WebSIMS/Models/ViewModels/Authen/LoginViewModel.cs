using System.ComponentModel.DataAnnotations;

namespace SIMS_test.Models.ViewModels.Authen;

public class LoginViewModel
{
    [Required(ErrorMessage = "Enter Username , please")]

    public string Username { get; set; } = null!;
    [Required(ErrorMessage = "Enter Password , please")]

    public string Password { get; set; } = null!;

}