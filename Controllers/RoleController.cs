using IdentityApp.Data;
using IdentityApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers
{
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(ApplicationDbContext dbContext, UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var roles = _dbContext.Roles.ToList();
            return View(roles);
        }

        [HttpGet]
        public IActionResult Upsert(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View();
            }
            else
            {
                var user = _dbContext.Roles.FirstOrDefault(u => u.Id == id);
                return View(user);
            }
        }
    }
}
