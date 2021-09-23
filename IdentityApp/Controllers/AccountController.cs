using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using IdentityApp.Models;
using IdentityApp.ViewModels;

namespace IdentityApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _appEnvironment;

        public AccountController(UserManager<User> userManager, 
            SignInManager<User> signInManager, ApplicationDbContext context,
            IWebHostEnvironment appeEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _appEnvironment = appeEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string userName, int page = 1)
        {
            const int PAGE_SIZE = 5;
            User user = await _userManager.Users
                .FirstOrDefaultAsync(user => user.UserName == userName);

            if (user != null)
            {
                IEnumerable<Post> allPosts = user.Posts
                    .OrderByDescending(post => post.PostedTime);
                IEnumerable<Post> currentUserPosts = allPosts
                    .Skip((page - 1) * PAGE_SIZE).Take(PAGE_SIZE);

                UserProfileViewModel viewModel = new UserProfileViewModel()
                {
                    User = user,
                    UserManager = _userManager,
                    Posts = currentUserPosts,
                    PageViewModel = new PageViewModel(
                        page, allPosts.Count(), PAGE_SIZE)
                };

                return View(viewModel);
            }

            return NotFound();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            string defaultProfilePicPath = $"{_appEnvironment.WebRootPath}" +
                "/Files/default_profile_pic.jpg";

            if (ModelState.IsValid)
            {
                User existingUser = await _context.Users
                    .FirstOrDefaultAsync(user => user.UserName == model.UserName
                                         || user.Email == model.Email);

                if (existingUser == null)
                {
                    User user = new User()
                    {
                        Email = model.Email,
                        UserName = model.UserName
                    };

                    using (FileStream fileStream = new FileStream(
                        defaultProfilePicPath, FileMode.Open, FileAccess.Read))
                    {
                        user.ProfilePicture = System.IO.File
                            .ReadAllBytes(defaultProfilePicPath);
                        fileStream.Read(user.ProfilePicture, 0,
                            System.Convert.ToInt32(fileStream.Length));
                    }

                    IdentityResult result = await _userManager
                        .CreateAsync(user, model.Password);

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
                else
                {
                    ModelState.AddModelError("", "User with such an email " +
                        "and/or username already exists");
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(
                        user.UserName, model.Password, model.RememberMe, false);

                    if (result.Succeeded)
                    {
                        await _signInManager.PasswordSignInAsync(user.UserName,
                            model.Password, model.RememberMe, false);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Incorrect password");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect email");
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
        public async Task<IActionResult> Edit(string userId, string returnUrl)
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
                Company = user.Company,
                ReturnAction = "Index",
                ReturnController = !string.IsNullOrEmpty(returnUrl)
                    ? "Users" : "Account",
                Roles = await _userManager.GetRolesAsync(user)
            };

            Stream stream = new MemoryStream(user.ProfilePicture);
            model.ProfilePicture = new FormFile(stream, 0, 
                user.ProfilePicture.Length, "name", "filename");

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

                        using (BinaryReader binaryReader = new BinaryReader(
                            model.ProfilePicture.OpenReadStream()))
                        {
                            imageData = binaryReader.ReadBytes(
                                (int)model.ProfilePicture.Length);
                        }

                        user.ProfilePicture = imageData;
                    }

                    IdentityResult result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        await _userManager.UpdateAsync(user);
                        return RedirectToAction("Index", 
                            new { userName = user.UserName });
                    }
                    else
                    {
                        foreach (IdentityError error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }

                /*User existingUser = await _userManager.FindByNameAsync(model.UserName);

                if (existingUser == null)
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

                            using (BinaryReader binaryReader = new BinaryReader(
                                model.ProfilePicture.OpenReadStream()))
                            {
                                imageData = binaryReader.ReadBytes((int)model.ProfilePicture.Length);
                            }

                            user.ProfilePicture = imageData;
                        }

                        IdentityResult result = await _userManager.UpdateAsync(user);

                        if (result.Succeeded)
                        {
                            await _userManager.UpdateAsync(user);
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
                else
                {
                    ModelState.AddModelError("", "This username is already taken");
                }*/
            }

            return View(model);
        }
    }
}
