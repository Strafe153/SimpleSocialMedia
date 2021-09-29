using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Models;
using IdentityApp.ViewModels;

namespace IdentityApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index(AdminPanelViewModel model)
        {
            const int PAGE_SIZE = 5;
            int usersNumber;

            IQueryable<User> users = _userManager.Users;
            FilterUsers(ref users, model.UserName, model.Email,
                model.Year, model.Country);
            users = ChooseSort(users, model.SortOrder);

            usersNumber = await users.CountAsync();
            var currentPageUsers = await users
                .Skip((model.Page - 1) * PAGE_SIZE)
                .Take(PAGE_SIZE)
                .ToListAsync();

            var filterSortPageViewModel = new FilterSortPageViewModel()
            {
                Users = currentPageUsers,
                FilterViewModel = new FilterViewModel(
                    model.UserName, model.Email, model.Year, model.Country),
                SortViewModel = new SortViewModel(model.SortOrder),
                PageViewModel = new PageViewModel(
                    model.Page, usersNumber, PAGE_SIZE)
            };

            return View(filterSortPageViewModel);
        }

        private void FilterUsers(ref IQueryable<User> users, string userName, 
            string email, int? year, string country)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                users = from user in users
                        where user.UserName.Contains(userName)
                        select user;
            }

            if (!string.IsNullOrEmpty(email))
            {
                users = from user in users 
                        where user.Email.Contains(email) 
                        select user;
            }

            if (year != null)
            {
                users = from user in users 
                        where user.Year == year 
                        select user;
            }

            if (!string.IsNullOrEmpty(country))
            {
                users = from user in users 
                        where user.Country.Contains(country) 
                        select user;
            }
        }

        private IQueryable<User> ChooseSort(IQueryable<User> users,
            SortState sortOrder)
        {
            return sortOrder switch
            {
                SortState.NameDescending => 
                    users.OrderByDescending(user => user.UserName),
                SortState.EmailAscending => 
                    users.OrderBy(user => user.Email),
                SortState.EmailDescending => 
                    users.OrderByDescending(user => user.Email),
                SortState.YearAscending => 
                    users.OrderBy(user => user.Year),
                SortState.YearDescending => 
                    users.OrderByDescending(user => user.Year),
                SortState.CountryAscending => 
                    users.OrderBy(user => user.Country),
                SortState.CountryDescending => 
                    users.OrderByDescending(user => user.Country),
                _ => users.OrderBy(u => u.UserName)
            };
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ChangePassword(string userId, 
            string returnUrl)
        {
            User user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            ChangePasswordViewModel model = new ChangePasswordViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);

                if (user != null)
                {
                    IdentityResult result = await _userManager
                        .ChangePasswordAsync(user, model.CurrentPassword,
                            model.NewPassword);

                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(model.ReturnUrl) 
                            && Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return LocalRedirect(model.ReturnUrl);
                        }

                        return BadRequest();
                    }
                    else
                    {
                        foreach (IdentityError error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user has not been found");
                }
            }

            return View(model);
        }
    }
}
