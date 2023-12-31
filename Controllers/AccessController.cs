using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers
{
    public class AccessController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
