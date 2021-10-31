using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using IdentityApp.Models;
using IdentityApp.ViewModels;
using IdentityApp.Interfaces;
using IdentityApp.ControllerRepositories;

namespace IdentityApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountControllable _repository;

        public AccountController(IAccountControllable repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string userName, int page = 1)
        {
            const int PAGE_SIZE = 5;
            User user = await _repository.FindByNameAsync(userName);
            User authenticatedUser = null;

            if (User.Identity.IsAuthenticated)
            {
                authenticatedUser = await _repository.FindByNameAsync(User.Identity.Name);
            }

            if (user != null)
            {
                IEnumerable<Post> allPosts = user.Posts.OrderByDescending(post => post.PostedTime);
                IEnumerable<Post> currentUserPosts = allPosts.Skip((page - 1) * PAGE_SIZE).Take(PAGE_SIZE);

                UserProfileViewModel viewModel = new UserProfileViewModel()
                {
                    User = user,
                    Posts = currentUserPosts,
                    PageViewModel = new PageViewModel(page, allPosts.Count(), PAGE_SIZE),
                    AuthenticatedUser = authenticatedUser,
                    AuthenticatedUserRoles = authenticatedUser != null 
                        ? await _repository.GetRolesAsync(authenticatedUser) 
                        : new List<string> { "user" }
                };

                _repository.LogInformation($"On user {user.UserName} profile");
                return View(viewModel);
            }

            _repository.LogError($"User not found");
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
            if (ModelState.IsValid)
            {
                User existingUser = await _repository.FirstOrDefaultAsync(user => 
                    user.UserName == model.UserName || user.Email == model.Email);

                if (existingUser == null)
                {
                    User user = new User()
                    {
                        Email = model.Email,
                        UserName = model.UserName
                    };

                    await SetDefaultProfilePicture(user);
                    IdentityResult result = await _repository.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        await _repository.AddToRoleAsync(user, "user");
                        await _repository.SignInAsync(user, false);
                        _repository.LogInformation($"Created user {user.UserName}");
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        foreach (IdentityError error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        _repository.LogWarning($"Failed to create user {user.UserName}");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User with such an email and/or username already exists");
                    _repository.LogWarning($"User with such email {model.Email} " +
                        $"and/or username {model.UserName} already exists");
                }
            }

            _repository.LogWarning("RegisterViewModel is not valid");
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
                User user = await _repository.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var result = await _repository.PasswordSignInAsync(
                        user.UserName, model.Password, model.RememberMe, false);

                    if (result.Succeeded)
                    {
                        _repository.LogInformation($"User {user.UserName} logged in");
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Incorrect password");
                        _repository.LogWarning($"User {user.UserName} failed to log in");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User with such an email doesn't exist");
                    _repository.LogWarning($"User with email {model.Email} doesn't exist");
                }
            }

            _repository.LogWarning("LoginViewModel is not valid");
            return View(model);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Logout(string userName)
        {
            if (await _repository.FindByNameAsync(userName) == null)
            {
                return NotFound();
            }

            await _repository.SignOutAsync();
            _repository.LogInformation($"User {userName} logged out");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string userId, string returnUrl)
        {
            User user = await _repository.FindByIdAsync(userId);
            User authenticatedUser = await _repository.FindByNameAsync(User.Identity.Name);

            if (user == null)
            {
                _repository.LogError($"User not found");
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
                ProfilePicture = ConvertByteArrayToIFormFile(user.ProfilePicture),
                CalledFromAction = returnUrl,
                AuthenticatedUserRoles = authenticatedUser != null
                    ? await _repository.GetRolesAsync(authenticatedUser)
                    : new List<string> { "user" }
            };

            _repository.LogInformation($"Editing user {user.UserName}");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            User user = await _repository.FindByIdAsync(model.Id);
            CheckIfUserNameIsTaken(model, user);

            if (ModelState.IsValid)
            {
                bool userNameChanged = model.UserName != user.UserName ? true : false;

                AssignEditUserViewModelToUser(model, user);
                IdentityResult result = await _repository.UpdateAsync(user);

                if (result.Succeeded)
                {
                    await _repository.SaveChangesAsync();
                    _repository.LogInformation($"Information updated for {user.UserName}");
                    await ReloginUserOnUserNameChanged(userNameChanged, user);

                    if (model.CalledFromAction.Contains("Account"))
                    {
                        return RedirectToAction("Index", new { userName = user.UserName });
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
                    _repository.LogWarning($"User {user.UserName} profile information hasn't been updated");
                }
            }

            model.UserName = user.UserName;
            model.ProfilePicture = ConvertByteArrayToIFormFile(user.ProfilePicture);

            _repository.LogWarning("EditUserViewModel is not valid");
            return View(model);
        }

        public async Task<IActionResult> Follow(string userToFollowName, 
            string authenticatedUserName, int page = 1)
        {
            User userToFollow = await _repository.FindByNameAsync(userToFollowName);
            User authenticatedUser = await _repository.FindByNameAsync(authenticatedUserName);

            if (userToFollow == null)
            {
                _repository.LogError("User not found");
                return NotFound();
            }

            Following followed = new Following()
            {
                FollowedUser = userToFollow,
                FollowedUserId = userToFollow.Id,
                Reader = authenticatedUser,
                ReaderId = authenticatedUser.Id
            };

            authenticatedUser.FollowingUsers.Add(followed);
            authenticatedUser.FollowsCount++;
            userToFollow.Followers.Add(followed);
            userToFollow.ReadersCount++;
            await _repository.SaveChangesAsync();

            _repository.LogInformation($"User {authenticatedUserName} is now following user {userToFollowName}");
            return RedirectToAction("Index", "Account", new { userName = userToFollowName, page = page });
        }

        public async Task<IActionResult> Unfollow(string userToUnfollowName, 
            string authenticatedUserName, int page = 1)
        {
            User userToUnfollow = await _repository.FindByNameAsync(userToUnfollowName);
            User authenticatedUser = await _repository.FindByNameAsync(authenticatedUserName);

            if (userToUnfollow == null || authenticatedUser == null)
            {
                _repository.LogError("User not found");
                return NotFound();
            }

            Following followed = authenticatedUser.FollowingUsers.FirstOrDefault(followed => 
                followed.FollowedUserId == userToUnfollow.Id && followed.ReaderId == authenticatedUser.Id);

            authenticatedUser.FollowingUsers.Remove(followed);
            authenticatedUser.FollowsCount--;
            userToUnfollow.Followers.Remove(followed);
            userToUnfollow.ReadersCount--;
            await _repository.SaveChangesAsync();

            _repository.LogInformation($"User {authenticatedUserName} unfollows user {userToUnfollowName}");
            return RedirectToAction("Index", "Account", new { userName = userToUnfollowName, page = page });
        }

        private async Task SetDefaultProfilePicture(User user)
        {
            string defaultProfilePicPath = $"{_repository.GetWebRootPath()}/Files/default_profile_pic.jpg";

            using (var fileStream = new FileStream(defaultProfilePicPath, FileMode.Open, FileAccess.Read))
            {
                user.ProfilePicture = await System.IO.File.ReadAllBytesAsync(defaultProfilePicPath);
                await fileStream.ReadAsync(user.ProfilePicture, 0, System.Convert.ToInt32(fileStream.Length));
            }
        }

        private IFormFile ConvertByteArrayToIFormFile(byte[] bytePicture)
        {
            Stream stream = new MemoryStream(bytePicture);
            IFormFile formFilePicture = new FormFile(stream, 0, bytePicture.Length, 
                "default_profile_picture", "default_profile_pic.jpg");

            return formFilePicture;
        }

        private byte[] ConvertIFormFileToByteArray(IFormFile formFilePicture, byte[] bytePicture)
        {
            if (formFilePicture != null)
            {
                using (BinaryReader binaryReader = new BinaryReader(formFilePicture.OpenReadStream()))
                {
                    bytePicture = binaryReader.ReadBytes((int)formFilePicture.Length);
                }
            }

            return bytePicture;
        }

        private void CheckIfUserNameIsTaken(EditUserViewModel model, User user)
        {
            if (user != null)
            {
                IEnumerable<string> userNames = _repository.GetAllUsers()
                    .Select(user => user.UserName).AsEnumerable();

                foreach (string userName in userNames)
                {
                    if (model.UserName == userName && model.UserName != user.UserName)
                    {
                        ModelState.AddModelError("", "The username is already taken");
                        _repository.LogWarning($"The username {model.UserName} is already taken");
                    }
                }
            }
        }

        private void AssignEditUserViewModelToUser(EditUserViewModel model, User user)
        {
            user.Email = model.Email;
            user.UserName = model.UserName;
            user.Year = model.Year;
            user.Status = model.Status;
            user.Country = model.Country;
            user.City = model.City;
            user.Company = model.Company;
            user.ProfilePicture = ConvertIFormFileToByteArray(model.ProfilePicture, user.ProfilePicture);
        }

        private async Task ReloginUserOnUserNameChanged(bool isUserNameChanged, User user)
        {
            if (isUserNameChanged)
            {
                await _repository.SignOutAsync();
                await _repository.SignInAsync(user, false);
                _repository.LogInformation($"User {user.UserName} relogged in");
            }
        }
    }
}