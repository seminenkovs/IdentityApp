using System.Security.Claims;
using IdentityApp.Data;
using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace IdentityApp.Authorization;

public class NicknameAuthorization : AuthorizationHandler<NicknameRequirement>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ApplicationDbContext _contextDb;

    public NicknameAuthorization(UserManager<AppUser> userManager, ApplicationDbContext _contextDb)
    {
        _userManager = userManager;
        this._contextDb = _contextDb;
    }
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
        NicknameRequirement requirement)
    {
        string userId = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = _contextDb.AppUser.FirstOrDefault(u => u.Id == userId);
        var claims = Task.Run(async () => await _userManager.GetClaimsAsync(user)).Result;
        var claim = claims.FirstOrDefault(c => c.Type == "Nickname");
        if (claim != null)
        {
            if (claim.Value.ToLower().Contains(requirement.Name.ToLower()))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }
        return Task.CompletedTask;
    }
}