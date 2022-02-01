using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using SimpleSocialMedia.Models;
using SimpleSocialMedia.ViewModels;
using SimpleSocialMedia.Repositories.Interfaces;

namespace SimpleSocialMedia.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeControllable _repo;
        private const int PageSize = 5;

        public HomeController(IHomeControllable repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            var allPosts = _repo.GetAllPosts().OrderByDescending(p => p.PostedTime);
            var currentPagePosts = allPosts.Skip((page - 1) * PageSize).Take(PageSize);
            User authenticatedUser = null;

            if (User.Identity.IsAuthenticated)
            {
                authenticatedUser = await _repo.FindByNameAsync(User.Identity.Name);
            }

            var model = new HomepageViewModel()
            {
                AuthenticatedUser = authenticatedUser,
                PageViewModel = new PageViewModel(page, allPosts.Count(), PageSize),
                Posts = currentPagePosts,
                AuthenticatedUserRoles = authenticatedUser != null
                    ? await _repo.GetRolesAsync(authenticatedUser)
                    : new List<string> { "user" }
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Feed(int page = 1)
        {
            User authenticatedUser = null;

            if (User.Identity.IsAuthenticated)
            {
                authenticatedUser = await _repo.FindByNameAsync(User.Identity.Name);
            }
            else
            {
                return RedirectToAction("Login", "Accounts");
            }

            var followedUsersPosts = new List<Post>();

            foreach (Following following in _repo.GetAllFollowings())
            {
                if (following.ReaderId == authenticatedUser.Id)
                {
                    followedUsersPosts.AddRange(following.FollowedUser.Posts);
                }
            }

            var currentPagePosts = followedUsersPosts
                .OrderByDescending(post => post.PostedTime)
                .Skip((page - 1) * PageSize)
                .Take(PageSize);

            var model = new FeedPageViewModel()
            {
                AuthenticatedUser = authenticatedUser,
                PageViewModel = new PageViewModel(page, followedUsersPosts.Count(), PageSize),
                Posts = currentPagePosts
            };

            return View(model);
        }
    }
}
