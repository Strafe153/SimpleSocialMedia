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
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<User> _signInManager;

        public UsersController(UserManager<User> userManager,
            ApplicationDbContext context, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
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
                users = users.Where(user => user.UserName.Contains(userName));
            }

            if (!string.IsNullOrEmpty(email))
            {
                users = users.Where(user => user.Email.Contains(email));
            }

            if (year != null)
            {
                users = users.Where(user => user.Year == year);
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

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                List<LikedPost> ownedOrLikedPosts = new List<LikedPost>();

                foreach (Post post in user.Posts)
                {
                    ownedOrLikedPosts.AddRange(_context.LikedPosts.Where(
                        likedPost => likedPost.PostId == post.Id 
                        || likedPost.UserId == user.Id));
                }

                foreach (LikedPost post in ownedOrLikedPosts)
                {
                    if (post.UserId == user.Id)
                    {
                        post.Post.Likes--;
                    }
                }

                _context.LikedPosts.RemoveRange(ownedOrLikedPosts);

                if (user.UserName == User.Identity.Name)
                {
                    await _signInManager.SignOutAsync();
                }

                await _userManager.DeleteAsync(user);
                await _context.SaveChangesAsync();
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
