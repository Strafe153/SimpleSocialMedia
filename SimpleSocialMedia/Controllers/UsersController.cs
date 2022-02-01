using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using SimpleSocialMedia.Data;
using SimpleSocialMedia.Models;
using SimpleSocialMedia.ViewModels;
using SimpleSocialMedia.Repositories.Interfaces;

namespace SimpleSocialMedia.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUsersControllable _repo;

        public UsersController(IUsersControllable repo)
        {
            _repo = repo;
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index(AdminPanelViewModel model)
        {
            const int PAGE_SIZE = 5;
            IQueryable<User> users = _repo.GetAllUsers();
            FilterUsers(ref users, model.UserName, model.Email,model.Year, model.Country);

            users = SortByParameter(users, model.SortOrder);

            int usersNumber = await users.CountAsync();
            var currentPageUsers = users
                .Skip((model.Page - 1) * PAGE_SIZE)
                .Take(PAGE_SIZE)
                .AsEnumerable();

            var filterSortPageViewModel = new FilterSortPageViewModel()
            {
                Users = currentPageUsers,
                FilterViewModel = new FilterViewModel(model.UserName, 
                    model.Email, model.Year, model.Country),
                SortViewModel = new SortViewModel(model.SortOrder),
                PageViewModel = new PageViewModel(model.Page, usersNumber, PAGE_SIZE)
            };

            return View(filterSortPageViewModel);
        }

        [Authorize]
        public async Task<IActionResult> Delete(string userId, string returnUrl)
        {
            User userToDelete = await _repo.FindByIdAsync(userId);

            if (userToDelete != null)
            {
                IEnumerable<Following> userFollowings = _repo.GetAllFollowings()
                    .Where(f => f.ReaderId == userToDelete.Id);
                IEnumerable<Following> userReaders = _repo.GetAllFollowings()
                    .Where(f => f.FollowedUserId == userToDelete.Id);
                IEnumerable<LikedPost> userLikes = _repo.GetAllLikedPosts()
                    .Where(p => p.UserWhoLikedId == userToDelete.Id);
                IEnumerable<LikedPost> userPosts = from p in userToDelete.Posts
                                                   from lp in _repo.GetAllLikedPosts()
                                                   where lp.PostLikedId == p.Id
                                                   select lp;

                userFollowings.ToList().ForEach(f => f.FollowedUser.ReadersCount--);
                userReaders.ToList().ForEach(f => f.Reader.FollowsCount--);
                userLikes.ToList().ForEach(lp => lp.PostLiked.Likes--);

                _repo.GetAllFollowings().RemoveRange(userFollowings);
                _repo.GetAllFollowings().RemoveRange(userReaders);
                _repo.GetAllLikedPosts().RemoveRange(userPosts);
                _repo.GetAllLikedPosts().RemoveRange(userLikes);

                if (userToDelete.UserName == User.Identity.Name)
                {
                    await _repo.SignOutAsync();
                }

                await _repo.DeleteAsync(userToDelete);
                await _repo.SaveChangesAsync();
                _repo.LogInformation($"Deleted user {userToDelete.UserName}");
                return RedirectToAction("Index", returnUrl.Contains("Users") ? "Users" : "Home");
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ChangePassword(string userId, string returnUrl)
        {
            User user = await _repo.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var model = new ChangePasswordViewModel()
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
                User user = await _repo.FindByIdAsync(model.Id);

                if (user != null)
                {
                    IdentityResult result = await _repo.ChangePasswordAsync(
                        user, model.CurrentPassword, model.NewPassword);

                    if (result.Succeeded)
                    {
                        _repo.LogInformation($"Changed password for user {user.UserName}");

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

                        _repo.LogInformation($"Failed to change user {user.UserName}'s password");
                    }
                }
                else
                {
                    return NotFound();
                }
            }

            return View(model);
        }

        public async Task<IActionResult> FindUser(string userName)
        {
            if (userName == null)
            {
                return View("UserNotFound", "User not provided");
            }

            User userToFind = await _repo.FindByNameAsync(userName);

            if (userToFind == null)
            {
                return View("UserNotFound", $"User with name {userName} not found");
            }

            return RedirectToAction("Index", "Accounts", new { userName = userName });
        }

        public async Task<IActionResult> UserReaders(string userId)
        {
            User user = await _repo.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            IEnumerable<User> readers = _repo.GetAllFollowings()
                .Where(f => f.FollowedUserId == user.Id)
                .Select(f => f.Reader)
                .Distinct();

            var model = new UserRelationsViewModel()
            {
                UserName = user.UserName,
                RelatedUsers = readers
            };

            return View(model);
        }

        public async Task<IActionResult> UserFollows(string userId)
        {
            User user = await _repo.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            IEnumerable<User> follows = _repo.GetAllFollowings()
                .Where(f => f.ReaderId == user.Id)
                .Select(f => f.FollowedUser)
                .Distinct();

            var model = new UserRelationsViewModel()
            {
                UserName = user.UserName,
                RelatedUsers = follows
            };

            return View(model);
        }

        private void FilterUsers(ref IQueryable<User> users, string userName, string email,
            int? year, string country)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                users = users.Where(u => u.UserName.Contains(userName));
            }

            if (!string.IsNullOrEmpty(email))
            {
                users = users.Where(u => u.Email.Contains(email));
            }

            if (year.HasValue)
            {
                users = users.Where(u => u.Year == year.Value);
            }

            if (!string.IsNullOrEmpty(country))
            {
                users = users.Where(u => u.Country.Contains(country));
            }
        }

        private IQueryable<User> SortByParameter(IQueryable<User> users, SortState sortOrder)
        {
            return sortOrder switch
            {
                SortState.NameDescending => users.OrderByDescending(u => u.UserName),
                SortState.EmailAscending => users.OrderBy(u => u.Email),
                SortState.EmailDescending => users.OrderByDescending(u => u.Email),
                SortState.YearAscending => users.OrderBy(u => u.Year),
                SortState.YearDescending => users.OrderByDescending(u => u.Year),
                SortState.CountryAscending => users.OrderBy(u => u.Country),
                SortState.CountryDescending => users.OrderByDescending(u => u.Country),
                _ => users.OrderBy(u => u.UserName)
            };
        }
    }
}
