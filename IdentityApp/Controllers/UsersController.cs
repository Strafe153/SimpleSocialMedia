using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using IdentityApp.Models;
using IdentityApp.ViewModels;

namespace IdentityApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger _logger;

        public UsersController(UserManager<User> userManager,
            ApplicationDbContext context, SignInManager<User> signInManager,
            ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            _logger = logger;
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index(AdminPanelViewModel model)
        {
            const int PAGE_SIZE = 5;

            IQueryable<User> users = _userManager.Users;
            FilterUsers(ref users, model.UserName, model.Email,model.Year, 
                model.Country);
            users = ChooseSort(users, model.SortOrder);

            int usersNumber = await users.CountAsync();
            IEnumerable<User> currentPageUsers = users
                .Skip((model.Page - 1) * PAGE_SIZE)
                .Take(PAGE_SIZE)
                .AsEnumerable();

            var filterSortPageViewModel = new FilterSortPageViewModel()
            {
                Users = currentPageUsers,
                FilterViewModel = new FilterViewModel(
                    model.UserName, model.Email, model.Year, model.Country),
                SortViewModel = new SortViewModel(model.SortOrder),
                PageViewModel = new PageViewModel(
                    model.Page, usersNumber, PAGE_SIZE)
            };

            _logger.LogInformation("On Users page");
            return View(filterSortPageViewModel);
        }

        private void FilterUsers(ref IQueryable<User> users, string userName, 
            string email, int? year, string country)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                users = users.Where(user => user.UserName.Contains(userName));
            }

            if (!string.IsNullOrEmpty(email))
            {
                users = users.Where(user => user.Email.Contains(email));
            }

            if (year.HasValue)
            {
                users = users.Where(user => user.Year == year.Value);
            }

            if (!string.IsNullOrEmpty(country))
            {
                users = users.Where(user => user.Country.Contains(country));
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

        [Authorize]
        public async Task<IActionResult> Delete(string userId, string returnUrl)
        {
            User user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                IEnumerable<LikedPost> likedPosts = _context.LikedPosts.Where(
                    likedPost => likedPost.UserId == user.Id);
                List<LikedPost> ownedPosts = new List<LikedPost>();

                foreach (Post post in user.Posts)
                {
                    ownedPosts.AddRange(_context.LikedPosts
                        .Where(likedPost => likedPost.PostId == post.Id));
                }

                foreach (LikedPost likedPost in likedPosts)
                {
                    likedPost.Post.Likes--;
                }

                _context.LikedPosts.RemoveRange(ownedPosts);
                _context.LikedPosts.RemoveRange(likedPosts);

                if (user.UserName == User.Identity.Name)
                {
                    await _signInManager.SignOutAsync();
                }

                await _userManager.DeleteAsync(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Deleted user {user.UserName}");

                return RedirectToAction("Index", returnUrl.Contains("users",
                    StringComparison.OrdinalIgnoreCase) ? "Users" : "Home");
            }
            else
            {
                _logger.LogError($"User {user.UserName} not found");
                return NotFound();
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ChangePassword(string userId, 
            string returnUrl)
        {
            User user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                _logger.LogError($"User {user.UserName} not found");
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
                        _logger.LogInformation("Changed password for " +
                            $"user {user.UserName}");

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
                        _logger.LogWarning("Failed to change user " +
                            $"{user.UserName}'s password");
                    }
                }
                else
                {
                    _logger.LogError($"User {user.UserName} not found");
                    return NotFound();
                }
            }

            return View(model);
        }
    }
}
