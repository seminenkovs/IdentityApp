using IdentityApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var userList = _dbContext.Users.ToList();
            var roleList = _dbContext.UserRoles.ToList();
            // set user to "none" to make UI look better
            foreach (var user in userList)
            {
                var role = roleList.FirstOrDefault(r => r.UserId == user.Id);
                if (role == null)
                {
                    user.Role = "None";
                }
            }
        }
    }
}
