using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers
{
    public class RoleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
