﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using IdentityApp.Models;
using IdentityApp.ViewModels;

namespace IdentityApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public HomeController(UserManager<User> userManager, 
            ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            const int PAGE_SIZE = 10;
            IEnumerable<Post> allPosts = _context.Posts
                .OrderByDescending(post => post.PostedTime);
            IEnumerable<Post> currentPagePosts = allPosts
                .Skip((page - 1) * PAGE_SIZE).Take(PAGE_SIZE);
            IEnumerable<User> users = _userManager.Users;
            User authenticatedUser = null;

            if (User.Identity.IsAuthenticated)
            {
                authenticatedUser = await _userManager
                    .FindByNameAsync(User.Identity.Name);
            }

            HomepageViewModel model = new HomepageViewModel()
            {
                AuthenticatedUser = authenticatedUser,
                PageViewModel = new PageViewModel(
                    page, allPosts.Count(), PAGE_SIZE),
                Posts = currentPagePosts,
                AuthenticatedUserRoles = authenticatedUser != null
                    ? await _userManager.GetRolesAsync(authenticatedUser)
                    : new List<string> { "user" },
            };

            _logger.LogInformation("On Home page");
            return View(model);
        }
    }
}
