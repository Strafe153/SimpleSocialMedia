using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using SimpleSocialMedia.Models;
using SimpleSocialMedia.ViewModels;
using SimpleSocialMedia.Repositories.Interfaces;

namespace SimpleSocialMedia.Controllers
{
    [Authorize(Roles = "admin")]
    public class RolesController : Controller
    {
        private readonly IRolesControllable _repo;

        public RolesController(IRolesControllable repo)
        {
            _repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            List<IdentityRole> roles = await _repo.GetAllRolesAsync();
            return View(roles);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                IdentityResult result = await _repo.CreateAsync(new IdentityRole(roleName));

                if (result.Succeeded)
                {
                    _repo.LogInformation($"Created role {roleName}");
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    _repo.LogInformation($"Failed to create {roleName} role");
                }
            }

            return View(roleName);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string roleId)
        {
            IdentityRole role = await _repo.FindRoleByIdAsync(roleId);

            if (role != null)
            {
                await _repo.DeleteAsync(role);
                _repo.LogInformation($"Deleted role {role.Name}");
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string userId, string returnUrl)
        {
            User user = await _repo.FindUserByIdAsync(userId);

            if (user != null)
            {
                var model = new ChangeRoleViewModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    UserRoles = await _repo.GetRolesAsync(user),
                    AllRoles = await _repo.GetAllRolesAsync(),
                    ReturnUrl = returnUrl
                };

                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ChangeRoleViewModel model)
        {
            User user = await _repo.FindUserByIdAsync(model.UserId);

            if (user != null)
            {
                var userRoles = await _repo.GetRolesAsync(user);
                IEnumerable<string> addedRoles = model.NewRoles.Except(userRoles);
                IEnumerable<string> removedRoles = userRoles.Except(model.NewRoles);

                await _repo.AddToRolesAsync(user, addedRoles);
                await _repo.RemoveFromRolesAsync(user, removedRoles);
                _repo.LogInformation($"Changed user {user.UserName} roles");

                if (user.UserName == User.Identity.Name)
                {
                    await _repo.SignOutAsync();
                    await _repo.SignInAsync(user, false);
                }

                if (model.ReturnUrl.Contains("Accounts"))
                {
                    return RedirectToAction("Index", "Accounts", new { userName = user.UserName });
                }
                else
                {
                    userRoles = await _repo.GetRolesAsync(user);

                    if (userRoles.Contains("admin"))
                    {
                        return RedirectToAction("Index", "Users");
                    }

                    return RedirectToAction("Index", "Home");
                }
            }

            return NotFound();
        }
    }
}
