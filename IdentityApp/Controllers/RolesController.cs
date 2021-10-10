using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger _logger;

        public RolesController(UserManager<User> userManager, 
            RoleManager<IdentityRole> roleManager, 
            ILogger<RolesController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("On Roles page");
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
                    _logger.LogInformation($"Created role {name}");
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    _logger.LogWarning($"Failed to create {name} role");
                }
            }

            _logger.LogWarning("Role name is not profived");
            return View(name);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string roleId)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(roleId);

            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
                _logger.LogInformation($"Deleted role {role.Name}");
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

            _logger.LogError($"User {user.UserName} not found");
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string userId, 
            List<string> roles, string returnUrl)
        {
            User user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                IEnumerable<string> addedRoles = roles.Except(userRoles);
                IEnumerable<string> removedRoles = userRoles.Except(roles);

                await _userManager.AddToRolesAsync(user, addedRoles);
                await _userManager.RemoveFromRolesAsync(user, removedRoles);
                _logger.LogInformation($"Changed ser {user.UserName} roles");

                if (!string.IsNullOrEmpty(returnUrl)
                    && Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }
            }

            _logger.LogError($"User {user.UserName} not found");
            return NotFound();
        }
    }
}
