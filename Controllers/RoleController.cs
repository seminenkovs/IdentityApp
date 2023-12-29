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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(IdentityRole role)
        {
            if (await _roleManager.RoleExistsAsync(role.Name))
            {
                return RedirectToAction("Index");
            }

            if (string.IsNullOrEmpty(role.Id))
            {
                await _roleManager.CreateAsync(new IdentityRole {Name = role.Name});
            }
            else
            {
                var roleDb = _dbContext.Roles.FirstOrDefault(r => r.Id == role.Id);
                if (roleDb == null)
                {
                    return RedirectToAction(nameof(Index));
                }
                roleDb.Name = role.Name;
                roleDb.NormalizedName = role.Name.ToUpper();
                var result = await _roleManager.UpdateAsync(roleDb);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
