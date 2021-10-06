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
            User user = await _userManager.FindByNameAsync(userName);
            User authenticatedUser = null;

            if (User.Identity.IsAuthenticated)
            {
                authenticatedUser = await _userManager
                .FindByNameAsync(User.Identity.Name);
            }

            if (user != null)
            {
                IEnumerable<Post> allPosts = user.Posts
                    .OrderByDescending(post => post.PostedTime);
                IEnumerable<Post> currentUserPosts = allPosts
                    .Skip((page - 1) * PAGE_SIZE).Take(PAGE_SIZE);

                UserProfileViewModel viewModel = new UserProfileViewModel()
                {
                    User = user,
                    Posts = currentUserPosts,
                    PageViewModel = new PageViewModel(
                        page, allPosts.Count(), PAGE_SIZE),
                    AuthenticatedUser = authenticatedUser,
                    AuthenticatedUserRoles = authenticatedUser != null
                        ? await _userManager.GetRolesAsync(authenticatedUser)
                        : new List<string> { "user" }
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
                User existingUser = await _context.Users.FirstOrDefaultAsync(
                    user => user.UserName == model.UserName 
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
                        user.ProfilePicture = await System.IO.File
                            .ReadAllBytesAsync(defaultProfilePicPath);
                        await fileStream.ReadAsync(user.ProfilePicture, 0,
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
        [AutoValidateAntiforgeryToken]
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
                    ModelState.AddModelError("", 
                        "User with such an email doesn't exist");
                }
            }

            return View(model);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string userId, string returnUrl)
        {
            User user = await _userManager.FindByIdAsync(userId);
            User authenticatedUser = await _userManager
                .FindByNameAsync(User.Identity.Name);

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
                CalledFromAction = returnUrl[0..],
                AuthenticatedUserRoles = authenticatedUser != null 
                    ? await _userManager.GetRolesAsync(authenticatedUser)
                    : new List<string> { "user" }
            };

            Stream stream = new MemoryStream(user.ProfilePicture);
            model.ProfilePicture = new FormFile(stream, 0, 
                user.ProfilePicture.Length, "default_profile_picture",
                "default_profile_pic.jpg");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            User user = await _userManager.FindByIdAsync(model.Id);

            if (user != null)
            {
                IEnumerable<string> userNames = _context.Users
                    .Select(user => user.UserName).AsEnumerable();

                foreach (string userName in userNames)
                {
                    if (model.UserName == userName 
                        && model.UserName != user.UserName)
                    {
                        ModelState.AddModelError("", 
                            "The username is already taken");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                bool userNameChanged = model.UserName != user.UserName
                        ? true : false;

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
                    await _context.SaveChangesAsync();

                    if (userNameChanged)
                    {
                        await _signInManager.SignOutAsync();
                        await _signInManager.SignInAsync(user, false);
                    }

                    if (model.CalledFromAction.Contains("Account"))
                    {
                        return RedirectToAction("Index",
                            new { userName = user.UserName });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Users");
                    }
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            model.UserName = user.UserName;

            Stream stream = new MemoryStream(user.ProfilePicture);
            model.ProfilePicture = new FormFile(stream, 0,
                user.ProfilePicture.Length, "profile_picture",
                "user_profile_picture");

            return View(model);
        }
    }
}