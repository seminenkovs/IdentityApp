using Microsoft.AspNetCore.Authorization;

namespace IdentityApp.Authorization;

public class NicknameRequirement : IAuthorizationRequirement
{
    public NicknameRequirement(string name)
    {
        Name = name;
    }
    public string Name { get; set; }
}