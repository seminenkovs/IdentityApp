using System.ComponentModel.DataAnnotations;

namespace IdentityApp.ViewModels;

public class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}