using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using IdentityApp.Models;
using IdentityApp.ViewModels;

namespace IdentityApp.Controllers
{
    [Authorize(Roles = "admin")]
    public class RolesController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(UserManager<User> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _roleManager.Roles.ToListAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                IdentityResult result = await _roleManager
                    .CreateAsync(new IdentityRole(name));

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View(name);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string roleId)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(roleId);

            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string userId, string returnUrl)
        {
            User user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                ChangeRoleViewModel model = new ChangeRoleViewModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    UserRoles = await _userManager.GetRolesAsync(user),
                    AllRoles = await _roleManager.Roles.ToListAsync(),
                    ReturnUrl = returnUrl
                };

                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string userId, 
            List<string> roles, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(userId);

                if (user != null)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    var addedRoles = roles.Except(userRoles);
                    var removedRoles = userRoles.Except(roles);

                    await _userManager.AddToRolesAsync(user, addedRoles);
                    await _userManager.RemoveFromRolesAsync(user, removedRoles);

                    if (!string.IsNullOrEmpty(returnUrl) 
                        && Url.IsLocalUrl(returnUrl))
                    {
                        return LocalRedirect(returnUrl);
                    }
                }
            }

            return NotFound();
        }
    }
}
