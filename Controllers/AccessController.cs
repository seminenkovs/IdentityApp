using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers
{
    [Authorize]
    public class AccessController : Controller
    {
        //Authorize from cookie or JWT
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Blogger, Pokemon")]
        public IActionResult PokemonAndBlogerAccess()
        {
            return View();
        }

        [Authorize(Policy = "OnlyBloggerChecker")]
        public IActionResult OnlyBloggerChecker()
        {
            return View();
        }

        [Authorize(Policy = "CheckNickNameBill")]
        public IActionResult CheckNickNameBill()
        {
            return View();
        }

    }
}
