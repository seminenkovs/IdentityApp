using System.ComponentModel.DataAnnotations;

namespace IdentityApp.ViewModels;

public class ResetPasswordViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name="Email")]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name="Password")]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name="Confirm Password")]
    [Compare("Password", ErrorMessage = "Passwords don't match!")]
    public string ConfirmPassword { get; set; }

    public string Code { get; set; }
}