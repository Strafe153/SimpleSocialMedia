using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Collections.Generic;
using IdentityApp.Models;
using IdentityApp.ViewModels;

namespace IdentityApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public HomeController(UserManager<User> userManager, 
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(int page = 1)
        {
            const int PAGE_SIZE = 10;
            IEnumerable<Post> allPosts = _context.Posts
                .OrderByDescending(post => post.PostedTime);
            IEnumerable<Post> currentPagePosts = allPosts
                .Skip((page - 1) * PAGE_SIZE).Take(PAGE_SIZE);
            IEnumerable<User> users = _userManager.Users;

            HomepageViewModel model = new HomepageViewModel()
            {
                Users = users,
                Posts = currentPagePosts,
                PageViewModel = new PageViewModel(
                    page, allPosts.Count(), PAGE_SIZE)
            };

            return View(model);
        }
    }
}
