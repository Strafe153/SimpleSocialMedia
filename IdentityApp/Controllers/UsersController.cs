using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using IdentityApp.Models;
using IdentityApp.ViewModels;
using IdentityApp.Interfaces;

namespace IdentityApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUsersControllable _repository;

        public UsersController(IUsersControllable repository)
        {
            _repository = repository;
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index(AdminPanelViewModel model)
        {
            const int PAGE_SIZE = 5;

            IQueryable<User> users = _repository.GetAllUsers();
            FilterUsers(ref users, model.UserName, model.Email,model.Year, model.Country);
            users = ChooseSort(users, model.SortOrder);

            int usersNumber = await users.CountAsync();
            IEnumerable<User> currentPageUsers = users
                .Skip((model.Page - 1) * PAGE_SIZE).Take(PAGE_SIZE).AsEnumerable();

            var filterSortPageViewModel = new FilterSortPageViewModel()
            {
                Users = currentPageUsers,
                FilterViewModel = new FilterViewModel(model.UserName, model.Email, model.Year, model.Country),
                SortViewModel = new SortViewModel(model.SortOrder),
                PageViewModel = new PageViewModel(model.Page, usersNumber, PAGE_SIZE)
            };

            _repository.LogInformation("On Users page");
            return View(filterSortPageViewModel);
        }

        [Authorize]
        public async Task<IActionResult> Delete(string userId, string returnUrl)
        {
            User user = await _repository.FindByIdAsync(userId);

            if (user != null)
            {
                IEnumerable<LikedPost> likedPosts = _repository.GetAllLikedPosts()
                    .Where(likedPost => likedPost.UserId == user.Id);
                List<LikedPost> ownedPosts = new List<LikedPost>();

                foreach (Post post in user.Posts)
                {
                    ownedPosts.AddRange(_repository.GetAllLikedPosts()
                        .Where(likedPost => likedPost.PostId == post.Id));
                }

                foreach (LikedPost likedPost in likedPosts)
                {
                    likedPost.Post.Likes--;
                }

                _repository.GetAllLikedPosts().RemoveRange(ownedPosts);
                _repository.GetAllLikedPosts().RemoveRange(likedPosts);

                if (user.UserName == User.Identity.Name)
                {
                    await _repository.SignOutAsync();
                }

                await _repository.DeleteAsync(user);
                await _repository.SaveChangesAsync();
                _repository.LogInformation($"Deleted user {user.UserName}");

                return RedirectToAction("Index", returnUrl.Contains("users",
                    StringComparison.OrdinalIgnoreCase) ? "Users" : "Home");
            }
            else
            {
                _repository.LogError($"User not found");
                return NotFound();
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ChangePassword(string userId, 
            string returnUrl)
        {
            User user = await _repository.FindByIdAsync(userId);

            if (user == null)
            {
                _repository.LogError($"User not found");
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
                User user = await _repository.FindByIdAsync(model.Id);

                if (user != null)
                {
                    IdentityResult result = await _repository.ChangePasswordAsync(
                        user, model.CurrentPassword, model.NewPassword);

                    if (result.Succeeded)
                    {
                        _repository.LogInformation($"Changed password for user {user.UserName}");

                        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
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
                        _repository.LogWarning($"Failed to change user {user.UserName}'s password");
                    }
                }
                else
                {
                    _repository.LogError($"User not found");
                    return NotFound();
                }
            }

            return View(model);
        }

        /// <summary>
        /// Filters users according to the given parameters
        /// </summary>
        /// <param name="users"></param>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <param name="year"></param>
        /// <param name="country"></param>
        private void FilterUsers(ref IQueryable<User> users, string userName, string email,
            int? year, string country)
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

        /// <summary>
        /// Sorts users according to a given parameter
        /// </summary>
        /// <param name="users"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        private IQueryable<User> ChooseSort(IQueryable<User> users, SortState sortOrder)
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
    }
}
