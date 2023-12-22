using System.ComponentModel.DataAnnotations;

namespace IdentityApp.ViewModels;

public class ExternalLoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    public string Name { get; set; }
}