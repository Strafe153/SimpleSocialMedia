using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using SimpleSocialMedia.Data;
using SimpleSocialMedia.Models;
using SimpleSocialMedia.ViewModels;
using SimpleSocialMedia.Repositories.Interfaces;

namespace SimpleSocialMedia.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IAccountsControllable _repo;

        public AccountsController(IAccountsControllable repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string userName, int page = 1)
        {
            const int PAGE_SIZE = 5;
            User user = await _repo.FindByNameAsync(userName);
            User authenticatedUser = null;

            if (User.Identity.IsAuthenticated)
            {
                authenticatedUser = await _repo.FindByNameAsync(User.Identity.Name);
            }

            if (user != null)
            {
                var allPosts = user.Posts.OrderByDescending(p => p.PostedTime);
                var currentUserPosts = allPosts.Skip((page - 1) * PAGE_SIZE).Take(PAGE_SIZE);

                var viewModel = new UserProfileViewModel()
                {
                    User = user,
                    Posts = currentUserPosts,
                    PageViewModel = new PageViewModel(page, allPosts.Count(), PAGE_SIZE),
                    AuthenticatedUser = authenticatedUser,
                    AuthenticatedUserRoles = authenticatedUser != null 
                        ? await _repo.GetRolesAsync(authenticatedUser) 
                        : new List<string> { "user" }
                };

                return View(viewModel);
            }

            _repo.LogInformation($"User not found");
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
                User existingUser = await _repo.SingleOrDefaultAsync(u => 
                    u.UserName == model.UserName || u.Email == model.Email);

                if (existingUser == null)
                {
                    var newUser = new User()
                    {
                        Email = model.Email,
                        UserName = model.UserName
                    };

                    await SetDefaultProfilePicture(newUser);
                    IdentityResult result = await _repo.CreateAsync(newUser, model.Password);

                    if (result.Succeeded)
                    {
                        await _repo.AddToRoleAsync(newUser, "user");
                        await _repo.SignInAsync(newUser, false);
                        _repo.LogInformation($"User {newUser.UserName} registered");
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        foreach (IdentityError error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }

                        _repo.LogInformation($"Registration failed");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User with such email and/or username already exists");
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
                User user = await _repo.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var result = await _repo.PasswordSignInAsync(
                        user.UserName, model.Password, model.RememberMe, false);

                    if (result.Succeeded)
                    {
                        _repo.LogInformation($"User {user.UserName} logged in");
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Incorrect password");
                        _repo.LogInformation($"User {user.UserName} failed to log in");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User with such an email doesn't exist");
                }
            }

            return View(model);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Logout(string userName)
        {
            if (await _repo.FindByNameAsync(userName) == null)
            {
                return NotFound();
            }

            await _repo.SignOutAsync();
            _repo.LogInformation($"User {userName} logged out");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string userId, string returnUrl)
        {
            User user = await _repo.FindByIdAsync(userId);
            User authenticatedUser = await _repo.FindByNameAsync(User.Identity.Name);

            if (user == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Year = user.Year,
                Status = user.Status,
                Country = user.Country,
                City = user.City,
                Company = user.Company,
                ProfilePicture = PictureUtility.ConvertByteArrayToIFormFile(user.ProfilePicture),
                CalledFromAction = returnUrl,
                AuthenticatedUserRoles = authenticatedUser != null
                    ? await _repo.GetRolesAsync(authenticatedUser)
                    : new List<string> { "user" }
            };

            _repo.LogInformation($"User {user.UserName} is editing their profile");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            User user = await _repo.FindByIdAsync(model.Id);
            CheckIfUserNameIsTaken(model, user);

            if (ModelState.IsValid)
            {
                bool userNameChanged = model.UserName != user.UserName ? true : false;

                AssignEditUserViewModelToUser(model, user);
                IdentityResult result = await _repo.UpdateAsync(user);

                if (result.Succeeded)
                {
                    await _repo.SaveChangesAsync();
                    _repo.LogInformation($"Information updated for {user.UserName}");
                    await ReloginUserOnUserNameChanged(userNameChanged, user);

                    if (model.CalledFromAction.Contains("Accounts"))
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

                    _repo.LogInformation($"User {user.UserName} profile update failed");
                }
            }

            model.UserName = user.UserName;
            model.ProfilePicture = PictureUtility.ConvertByteArrayToIFormFile(user.ProfilePicture);

            return View(model);
        }

        public async Task<IActionResult> Follow(string userToFollowName, 
            string authenticatedUserName, int page = 1)
        {
            User userToFollow = await _repo.FindByNameAsync(userToFollowName);

            if (userToFollow == null)
            {
                return NotFound();
            }

            User authenticatedUser = await _repo.FindByNameAsync(authenticatedUserName);
            var following = new Following()
            {
                FollowedUser = userToFollow,
                FollowedUserId = userToFollow.Id,
                Reader = authenticatedUser,
                ReaderId = authenticatedUser.Id
            };

            authenticatedUser.FollowingUsers.Add(following);
            authenticatedUser.FollowsCount++;
            userToFollow.Followers.Add(following);
            userToFollow.ReadersCount++;
            await _repo.SaveChangesAsync();

            _repo.LogInformation($"User {authenticatedUserName} follows user {userToFollowName}");
            return RedirectToAction("Index", "Accounts", new { userName = userToFollowName, page = page });
        }

        public async Task<IActionResult> Unfollow(string userToUnfollowName, 
            string authenticatedUserName, int page = 1)
        {
            User userToUnfollow = await _repo.FindByNameAsync(userToUnfollowName);
            User authenticatedUser = await _repo.FindByNameAsync(authenticatedUserName);

            if (userToUnfollow == null || authenticatedUser == null)
            {
                return NotFound();
            }

            Following following = authenticatedUser.FollowingUsers.FirstOrDefault(f => 
                f.FollowedUserId == userToUnfollow.Id && f.ReaderId == authenticatedUser.Id);

            authenticatedUser.FollowingUsers.Remove(following);
            authenticatedUser.FollowsCount--;
            userToUnfollow.Followers.Remove(following);
            userToUnfollow.ReadersCount--;
            await _repo.SaveChangesAsync();

            _repo.LogInformation($"User {authenticatedUserName} unfollows user {userToUnfollowName}");
            return RedirectToAction("Index", "Accounts", new { userName = userToUnfollowName, page = page });
        }

        private async Task SetDefaultProfilePicture(User user)
        {
            string defaultProfilePicPath = $"{_repo.GetWebRootPath()}/Files/default_profile_pic.jpg";

            using (var fileStream = new FileStream(defaultProfilePicPath, FileMode.Open, FileAccess.Read))
            {
                user.ProfilePicture = await System.IO.File.ReadAllBytesAsync(defaultProfilePicPath);
                await fileStream.ReadAsync(user.ProfilePicture, 0, System.Convert.ToInt32(fileStream.Length));
            }
        }

        private void CheckIfUserNameIsTaken(EditUserViewModel model, User user)
        {
            if (user != null)
            {
                var userNames = _repo.GetAllUsers().Select(u => u.UserName).AsEnumerable();

                foreach (string userName in userNames)
                {
                    if (model.UserName == userName && model.UserName != user.UserName)
                    {
                        ModelState.AddModelError("", "The username is already taken");
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
            user.ProfilePicture = PictureUtility.ConvertIFormFileToByteArray(
                model.ProfilePicture, user.ProfilePicture);
        }

        private async Task ReloginUserOnUserNameChanged(bool isUserNameChanged, User user)
        {
            if (isUserNameChanged)
            {
                await _repo.SignOutAsync();
                await _repo.SignInAsync(user, false);
            }
        }
    }
}