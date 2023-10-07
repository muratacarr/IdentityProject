using IdentityProject.Entities;
using IdentityProject.Entities.Context;
using IdentityProject.Extensions;
using IdentityProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using System.Collections.Immutable;

namespace IdentityProject.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileProvider _fileProvider;

        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IFileProvider fileProvider)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _fileProvider = fileProvider;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

            var userViewModel = new UserViewModel
            {
                Email = currentUser!.Email!,
                UserName = currentUser.UserName!,
                PhoneNumber = currentUser.PhoneNumber,
            };

            return View(userViewModel);
        }

        public async Task LogOut()
        {
            await _signInManager.SignOutAsync();
        }

        public IActionResult PasswordChange()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeViewModel passwordChangeViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var currentUser=await _userManager.FindByNameAsync(User.Identity!.Name!);

            var checkOldPassword = await _userManager.CheckPasswordAsync(currentUser!, passwordChangeViewModel.PasswordOld!);

            if (!checkOldPassword)
            {
                ModelState.AddModelError(string.Empty, "Eski şifreniz yanlış");
                return View();
            }

            var resultChangePassword = await _userManager.ChangePasswordAsync(currentUser!,passwordChangeViewModel.PasswordOld!,passwordChangeViewModel.PasswordNew!);

            if (!resultChangePassword.Succeeded)
            {
                ModelState.AddModelErrorList(resultChangePassword.Errors.Select(x=>x.Description).ToList());
                return View();
            }

            await _userManager.UpdateSecurityStampAsync(currentUser!);
            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(currentUser!, passwordChangeViewModel.PasswordNew!, true, false);

            ViewData["SuccessMessage"]="Şifreniz başarıyla değiştirilmiştir";

            return View();
        }

        public async Task<IActionResult> UserEdit()
        {
            ViewBag.genderList = new SelectList(Enum.GetNames(typeof(Gender)));
            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

            var userEditViewModel = new UserEditViewModel()
            {
                Username = currentUser!.UserName!,
                Email = currentUser.Email,
                Phone = currentUser.PhoneNumber,
                BirthDate = currentUser.BirthDate,
                City = currentUser.City,
                ModelGender = currentUser.Gender,
            };

            return View(userEditViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> UserEdit(UserEditViewModel userEditViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

            currentUser.UserName = userEditViewModel.Username;
            currentUser.Email = userEditViewModel.Email;
            currentUser.PhoneNumber = userEditViewModel.Phone;
            currentUser.BirthDate = userEditViewModel.BirthDate;
            currentUser.City = userEditViewModel.City;
            currentUser.Gender = userEditViewModel.ModelGender;

            if (userEditViewModel.Picture!=null && userEditViewModel.Picture.Length>0)
            {
                var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot");

                string randomFileName=$"{Guid.NewGuid().ToString()}{Path.GetExtension(userEditViewModel.Picture.FileName)}";

                var newPicturePath=Path.Combine(wwwrootFolder!.First(x=>x.Name=="userpictures").PhysicalPath!, randomFileName);

                using var stream = new FileStream(newPicturePath, FileMode.Create);

                await userEditViewModel.Picture.CopyToAsync(stream);

                currentUser.Picture = randomFileName;
            }

            var updateToUserResult= await _userManager.UpdateAsync(currentUser);

            if (!updateToUserResult.Succeeded)
            {
                ModelState.AddModelErrorList(updateToUserResult.Errors);
                return View();
            }

            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(currentUser, true);

            TempData["SuccessMessage"] = "Üye bilgileri başarıyla değiştirilmiştir.";

            var userEditViewModelreturn = new UserEditViewModel()
            {
                Username = currentUser!.UserName!,
                Email = currentUser.Email,
                Phone = currentUser.PhoneNumber,
                BirthDate = currentUser.BirthDate,
                City = currentUser.City,
                ModelGender = currentUser.Gender,
            };

            ViewBag.genderList = new SelectList(Enum.GetNames(typeof(Gender)));
            return View(userEditViewModelreturn);
        }

        public IActionResult ClaimList()
        {
            var userClaimList = User.Claims.Select(x => new ClaimViewModel
            {
                Issuer = x.Issuer,
                Type = x.Type,
                Value = x.Value,
            }).ToList();

            return View(userClaimList);
        }
        [Authorize(Policy = "DenizliPolicy")]
        public IActionResult DenizliPage()
        {
            return View();
        }
        [Authorize(Policy = "ExchangePolicy")]
        public IActionResult ExchangePage()
        {
            return View();
        }
    }
}

