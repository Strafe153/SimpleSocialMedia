using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using IdentityApp.Models;
using IdentityApp.ViewModels;

namespace IdentityApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IWebHostEnvironment _appEnvironment;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,
            IWebHostEnvironment appeEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appEnvironment = appeEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string userName, int page = 1)
        {
            const int PAGE_SIZE = 5;
            User user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName);

            if (user != null)
            {
                user.Posts = (from post in user.Posts
                              orderby post.PostedTime descending
                              select post).ToList();

                IEnumerable<Post> allPosts = user.Posts;
                int postsNumber = allPosts.Count();
                IEnumerable<Post> currentUserPosts = allPosts.Skip((page - 1) * PAGE_SIZE).Take(PAGE_SIZE);

                UserProfileViewModel viewModel = new UserProfileViewModel()
                {
                    User = user,
                    UserManager = _userManager,
                    Posts = currentUserPosts,
                    PageViewModel = new PageViewModel(page, postsNumber, PAGE_SIZE)
                };

                return View(viewModel);
            }

            return NotFound();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            string defaultProfilePicPath = $"{_appEnvironment.WebRootPath}/Files/default_profile_pic.jpg";

            if (ModelState.IsValid)
            {
                User user = new User() {
                    Email = model.Email,
                    UserName = model.UserName
                };

                using (FileStream fileStream = new FileStream(defaultProfilePicPath,
                    FileMode.Open, FileAccess.Read))
                {
                    user.ProfilePicture = System.IO.File.ReadAllBytes(defaultProfilePicPath);
                    fileStream.Read(user.ProfilePicture, 0, System.Convert.ToInt32(fileStream.Length));
                }

                IdentityResult result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "user");
                    await _signInManager.SignInAsync(user, false);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByEmailAsync(model.Email);

                var result = await _signInManager.PasswordSignInAsync(user.UserName, 
                    model.Password,  model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect login and/or password");
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            EditUserViewModel model = new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Year = user.Year,
                Status = user.Status,
                Country = user.Country,
                City = user.City,
                Company = user.Company
            };

            Stream stream = new MemoryStream(user.ProfilePicture);
            model.ProfilePicture = new FormFile(stream, 0, user.ProfilePicture.Length, "name", "filename");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);

                if (user != null)
                {
                    user.Email = model.Email;
                    user.UserName = model.UserName;
                    user.Year = model.Year;
                    user.Status = model.Status;
                    user.Country = model.Country;
                    user.City = model.City;
                    user.Company = model.Company;

                    if (model.ProfilePicture != null)
                    {
                        byte[] imageData = null;

                        using (BinaryReader binaryReader = new BinaryReader(model.ProfilePicture.OpenReadStream()))
                        {
                            imageData = binaryReader.ReadBytes((int)model.ProfilePicture.Length);
                        }

                        user.ProfilePicture = imageData;
                    }

                    IdentityResult result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", new { userName = user.UserName });
                    }
                    else
                    {
                        foreach (IdentityError error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }

            return View(model);
        }
    }
}
