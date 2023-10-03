using IdentityProject.Areas.Admin.Models;
using IdentityProject.Entities.Context;
using IdentityProject.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace IdentityProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RolesController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public RolesController(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["SuccessMessage"] = TempData["SuccessMessage"];

            var roles= await _roleManager.Roles.Select(x=> new RoleViewModel
            {
                Id=x.Id,
                Name=x.Name!
            }).ToListAsync();

            return View(roles);
        }

        public IActionResult RoleCreate()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RoleCreate(RolCreateViewModel rolCreateViewModel)
        {
            var result= await _roleManager.CreateAsync(new AppRole() { Name = rolCreateViewModel.Name });

            if (!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors);
                return View();
            }

            TempData["SuccessMessage"] = "Role başarılı bir şekilde eklenmiştir.";

            return RedirectToAction(nameof(RolesController.Index));
        }

        public async Task<IActionResult> RoleUpdate(string id)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(id);
            if (roleToUpdate == null)
            {
                throw new Exception("Rol yok");
            }
            return View(new RoleUpdateViewModel
            {
                Id = roleToUpdate.Id,
                Name= roleToUpdate.Name!
            });
        }
        [HttpPost]
        public async Task<IActionResult> RoleUpdate(RoleUpdateViewModel roleUpdateViewModel)
        {
            var roleToUpdate= await _roleManager.FindByIdAsync(roleUpdateViewModel.Id);

            if (roleToUpdate == null)
            {
                throw new Exception("Rol Yok");
            }

            roleToUpdate.Name = roleUpdateViewModel.Name;

            await _roleManager.UpdateAsync(roleToUpdate);

            ViewData["SuccessMessage"] = "Rol bilgisi güncellenmiştir";

            return View();
        }

        public async Task<IActionResult> RoleDelete(string id)
        {
            var rolToDelete= await _roleManager.FindByIdAsync(id);
            if (rolToDelete == null)
            {
                throw new Exception("rol yok");
            }

            var result= await _roleManager.DeleteAsync(rolToDelete);

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.Select(x => x.Description).First());
            }

            TempData["SuccessMessage"] = "Role başarılı bir şekilde eklenmiştir.";


            return RedirectToAction(nameof(RolesController.Index));
        }

        public async Task<IActionResult> AssignRoleToUser(string id)
        {
            var currentUser = await _userManager.FindByIdAsync(id);
            var roles = await _roleManager.Roles.ToListAsync();
            var userRoles=await _userManager.GetRolesAsync(currentUser);
            var roleViewModelList=new List<AssignRoleToUserViewModel>();

            foreach (var role in roles)
            {
                var assignRoleToUserViewModel = new AssignRoleToUserViewModel
                {
                    Id = role.Id,
                    Name = role.Name!
                };

                if (userRoles.Contains(role.Name!))
                {
                    assignRoleToUserViewModel.Exist = true;
                }

                roleViewModelList.Add(assignRoleToUserViewModel);
            }

            return View(roleViewModelList);
        }
        [HttpPost]
        public IActionResult AssignRoleToUser(List<AssignRoleToUserViewModel> assignRoleToUserViewModels)
        {
            return View();
        }
    }
}
