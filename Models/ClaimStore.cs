using System.Security.Claims;

namespace IdentityApp.Models;

public static class ClaimStore
{
    public static List<Claim> claimList = new List<Claim>()
    {
        new Claim("Create", "Create"),
        new Claim("Edit", "Edit"),
        new Claim("Delete", "Delete")
    };


}