using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger _logger;

        public AccountController(UserManager<User> userManager, 
            SignInManager<User> signInManager, ApplicationDbContext context,
            IWebHostEnvironment appeEnvironment, 
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _appEnvironment = appeEnvironment;
            _logger = logger;
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

                _logger.LogInformation($"On user {user.UserName} profile");
                return View(viewModel);
            }

            _logger.LogError($"User not found");
            return NotFound();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
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
                        _logger.LogInformation($"Created user {user.UserName}");
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        foreach (IdentityError error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        _logger.LogWarning("Failed to create user " +
                            $"{user.UserName}");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User with such an email " +
                        "and/or username already exists");
                    _logger.LogWarning($"User with such email {model.Email} " +
                        $"and/or username {model.UserName} already exists");
                }
            }

            _logger.LogWarning("RegisterViewModel is not valid");
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
                        _logger.LogInformation($"User {user.UserName} " +
                            "logged in");
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Incorrect password");
                        _logger.LogWarning($"User {user.UserName} failed " +
                            $"to log in");
                    }
                }
                else
                {
                    ModelState.AddModelError("", 
                        "User with such an email doesn't exist");
                    _logger.LogWarning($"User with email {model.Email} " +
                        "doesn't exist");
                }
            }

            _logger.LogWarning("LoginViewModel is not valid");
            return View(model);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Logout(string userName)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation($"User {userName} logged out");
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
                _logger.LogError($"User {user.UserName} not found");
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
                CalledFromAction = returnUrl,
                AuthenticatedUserRoles = authenticatedUser != null 
                    ? await _userManager.GetRolesAsync(authenticatedUser)
                    : new List<string> { "user" }
            };

            Stream stream = new MemoryStream(user.ProfilePicture);
            model.ProfilePicture = new FormFile(stream, 0, 
                user.ProfilePicture.Length, "default_profile_picture",
                "default_profile_pic.jpg");

            _logger.LogInformation($"Editing user {user.UserName}");
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
                        _logger.LogWarning($"The username {model.UserName} is " +
                            "already taken");
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
                    _logger.LogInformation("Information updated for " +
                        $"{user.UserName}");

                    if (userNameChanged)
                    {
                        await _signInManager.SignOutAsync();
                        await _signInManager.SignInAsync(user, false);
                        _logger.LogInformation($"User {user.UserName}" +
                            "relogged in");
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
                    _logger.LogWarning($"User {user.UserName} profile " +
                        "information hasn't been updated");
                }
            }

            model.UserName = user.UserName;

            Stream stream = new MemoryStream(user.ProfilePicture);
            model.ProfilePicture = new FormFile(stream, 0,
                user.ProfilePicture.Length, "profile_picture",
                "user_profile_picture");

            _logger.LogWarning("EditUserViewModel is not valid");
            return View(model);
        }
    }
}