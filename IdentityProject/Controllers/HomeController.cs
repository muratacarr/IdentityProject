using IdentityProject.Entities.Context;
using IdentityProject.Extensions;
using IdentityProject.Models;
using IdentityProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace IdentityProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _UserManager;
        private readonly SignInManager<AppUser> _SignInManager;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _UserManager = userManager;
            _SignInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View(new SignUpViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel signUpViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var identityResult = await _UserManager.CreateAsync(new()
            {
                UserName = signUpViewModel.Username,
                Email = signUpViewModel.Email,
                PhoneNumber = signUpViewModel.Phone
            }, signUpViewModel.PasswordConfirm);

            if (identityResult.Succeeded)
            {
                ViewData["SuccessMessage"] = "Üyelik Başarılı";
                return View(nameof(HomeController.SignUp));
            }

            foreach (IdentityError item in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, item.Description);
            }
            ViewBag.ErrorMessage = "Gerekli Alanları doldurunuz";
            return View(signUpViewModel);
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel signInViewModel,string? returnUrl=null)
        {
            returnUrl = returnUrl ?? Url.Action("Index","Home");

            var hasUser=await _UserManager.FindByEmailAsync(signInViewModel.Email!);
            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Email veya şifre yanlış");
                return View();
            }

            var signInResult= await _SignInManager.PasswordSignInAsync(hasUser,signInViewModel.Password,signInViewModel.RememberMe,false);

            if (signInResult.Succeeded)
            {
                return Redirect(returnUrl);
            }

            ModelState.AddModelErrorList(new List<string>() { "Email veya şifre yanlış" });


            return View();
        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}