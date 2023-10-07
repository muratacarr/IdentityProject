using IdentityProject.Entities.Context;
using IdentityProject.Extensions;
using IdentityProject.Models;
using IdentityProject.Services;
using IdentityProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace IdentityProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _UserManager;
        private readonly SignInManager<AppUser> _SignInManager;
        private readonly IEmailService _EmailService;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
        {
            _logger = logger;
            _UserManager = userManager;
            _SignInManager = signInManager;
            _EmailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
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
            }, signUpViewModel.PasswordConfirm!);




            if (!identityResult.Succeeded)
            {
                ModelState.AddModelErrorList(identityResult.Errors);
                return View();
            }

            var exchangeExpireClaim = new Claim("ExchangeExpireDate",DateTime.Now.AddDays(1).ToString());

            var user = await _UserManager.FindByNameAsync(signUpViewModel.Username!);

            var claimResult= await _UserManager.AddClaimAsync(user!, exchangeExpireClaim);

            if (!claimResult.Succeeded)
            {
                ModelState.AddModelErrorList(claimResult.Errors);
                return View();
            }

            ViewData["SuccessMessage"] = "Üyelik Başarılı";
            return RedirectToAction(nameof(HomeController.SignIn));


        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel signInViewModel,string? returnUrl=null)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            returnUrl = returnUrl ?? Url.Action("Index","Home");

            var hasUser=await _UserManager.FindByEmailAsync(signInViewModel.Email!);
            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Email veya şifre yanlış");
                return View();
            }

            var signInResult= await _SignInManager.PasswordSignInAsync(hasUser,signInViewModel.Password,signInViewModel.RememberMe,true);

            if (signInResult.Succeeded)
            {
                return Redirect(returnUrl);
            }

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelErrorList(new List<string>() { "3 dakika boyunca giriş yapamazsınız" });
                return View();
            }

            ModelState.AddModelErrorList(new List<string>() { "Email veya şifre yanlış" ,$"Başarısız giriş sayısı = {await _UserManager.GetAccessFailedCountAsync(hasUser)}"});


            return View();
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel forgetPasswordViewModel)
        {
            var hasUser = await _UserManager.FindByEmailAsync(forgetPasswordViewModel.Email!);

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Bu email adresine sahip kullanıcı bulunamamıştır.");
                return View();
            }

            string passwordResetToken = await _UserManager.GeneratePasswordResetTokenAsync(hasUser);

            var passwordRestLink = Url.Action("ResetPassword", "Home", new { userId=hasUser.Id,Token=passwordResetToken },HttpContext.Request.Scheme);

            await _EmailService.SendResetPasswordEmail(passwordRestLink!,hasUser.Email!);

            TempData["SuccessMessage"] = "Şifre yenileme linki, eposta adresine gönderilmiştir";

            return RedirectToAction(nameof(ForgetPassword));
        }

        public IActionResult ResetPassword(string userId,string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            var userId = TempData["userId"];
            var token = TempData["token"];

            if (userId==null || token==null)
            {
                throw new Exception("Bir hata meydana geldi");
            }

            var hasUser=await _UserManager.FindByIdAsync(userId.ToString()!);

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı bulunamamıştır.");
                return View();
            }

            IdentityResult result= await _UserManager.ResetPasswordAsync(hasUser,token.ToString()!,resetPasswordViewModel.Password!);

            if (result.Succeeded) 
            {
                await _UserManager.UpdateSecurityStampAsync(hasUser);
                ViewData["SuccessMessage"] = "Şifreniz başarıyla yenilenmiştir";
            }
            else
            {
                ModelState.AddModelErrorList(result.Errors.Select(result => result.Description).ToList());
            }


            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}