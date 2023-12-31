using System.Security.Claims;

namespace IdentityApp.ViewModels;

public class UserClaimsViewModel
{
    public string UserID { get; set; }
    public List<UserClaim> Claims { get; set; }

    public UserClaimsViewModel()
    {
        Claims = new List<UserClaim>();
    }
}

public class UserClaim
{
    public string ClaimType { get; set; }
    public bool IsSelected { get; set;}
}