using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using IdentityApp.Models;
using IdentityApp.Interfaces;
using IdentityApp.ViewModels;

namespace IdentityApp.Controllers
{
    [Authorize(Roles = "admin")]
    public class RolesController : Controller
    {
        private readonly IRolesControllable _repository;

        public RolesController(IRolesControllable repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            _repository.LogInformation("On Roles page");
            return View(await _repository.GetAllRolesAsync());
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
                IdentityResult result = await _repository.CreateAsync(new IdentityRole(roleName));

                if (result.Succeeded)
                {
                    _repository.LogInformation($"Created role {roleName}");
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    _repository.LogWarning($"Failed to create {roleName} role");
                }
            }

            _repository.LogWarning("Role name is not profived");
            return View(roleName);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string roleId)
        {
            IdentityRole role = await _repository.FindRoleByIdAsync(roleId);

            if (role != null)
            {
                await _repository.DeleteAsync(role);
                _repository.LogInformation($"Deleted role {role.Name}");
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string userId, string returnUrl)
        {
            User user = await _repository.FindUserByIdAsync(userId);

            if (user != null)
            {
                ChangeRoleViewModel model = new ChangeRoleViewModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    UserRoles = await _repository.GetRolesAsync(user),
                    AllRoles = await _repository.GetAllRolesAsync(),
                    ReturnUrl = returnUrl
                };

                return View(model);
            }

            _repository.LogError($"User not found");
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ChangeRoleViewModel model)
        {
            User user = await _repository.FindUserByIdAsync(model.UserId);

            if (user != null)
            {
                var userRoles = await _repository.GetRolesAsync(user);
                IEnumerable<string> addedRoles = model.NewRoles.Except(userRoles);
                IEnumerable<string> removedRoles = userRoles.Except(model.NewRoles);

                await _repository.AddToRolesAsync(user, addedRoles);
                await _repository.RemoveFromRolesAsync(user, removedRoles);
                _repository.LogInformation($"Changed user {user.UserName} roles");

                if (user.UserName == User.Identity.Name)
                {
                    await _repository.SignOutAsync();
                    await _repository.SignInAsync(user, false);
                }

                if (model.ReturnUrl.Contains("Account"))
                {
                    return RedirectToAction("Index", "Account", new { userName = user.UserName });
                }
                else
                {
                    userRoles = await _repository.GetRolesAsync(user);

                    if (userRoles.Contains("admin"))
                    {
                        return RedirectToAction("Index", "Users");
                    }

                    return RedirectToAction("Index", "Home");
                }
            }

            _repository.LogError($"User not found");
            return NotFound();
        }
    }
}
