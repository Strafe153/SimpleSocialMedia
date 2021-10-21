using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using IdentityApp.Models;
using IdentityApp.ViewModels;
using IdentityApp.Interfaces;

namespace IdentityApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeControllable _repository;

        public HomeController(IHomeControllable repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            const int PAGE_SIZE = 5;
            IEnumerable<Post> allPosts = _repository.GetAllPosts().OrderByDescending(post => post.PostedTime);
            IEnumerable<Post> currentPagePosts = allPosts.Skip((page - 1) * PAGE_SIZE).Take(PAGE_SIZE);
            IEnumerable<User> users = _repository.GetAllUsers();
            User authenticatedUser = null;

            if (User.Identity.IsAuthenticated)
            {
                authenticatedUser = await _repository.FindByNameAsync(User.Identity.Name);
            }

            HomepageViewModel model = new HomepageViewModel()
            {
                AuthenticatedUser = authenticatedUser,
                PageViewModel = new PageViewModel(page, allPosts.Count(), PAGE_SIZE),
                Posts = currentPagePosts,
                AuthenticatedUserRoles = authenticatedUser != null
                    ? await _repository.GetRolesAsync(authenticatedUser)
                    : new List<string> { "user" },
            };

            _repository.LogInformation("On Home page");
            return View(model);
        }
    }
}
