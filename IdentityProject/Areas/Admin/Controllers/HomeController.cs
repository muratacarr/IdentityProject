using IdentityProject.Areas.Admin.Models;
using IdentityProject.Entities.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace IdentityProject.Areas.Admin.Controllers
{
    [Authorize(Roles ="Admin")]
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public HomeController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UserList()
        {
            var userViewModelList =await _userManager.Users.Select(x => new UserViewModel()
            {
                Id = x.Id,
                Name = x.UserName,
                Email = x.Email,
            }).ToListAsync();

            return View(userViewModelList);
        }
    }
}
