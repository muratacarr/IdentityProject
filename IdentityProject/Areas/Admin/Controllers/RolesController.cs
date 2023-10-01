using Microsoft.AspNetCore.Mvc;

namespace IdentityProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RolesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
