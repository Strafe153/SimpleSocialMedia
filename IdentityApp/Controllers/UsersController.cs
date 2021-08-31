using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using IdentityApp.Models;
using IdentityApp.ViewModels;

namespace IdentityApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index(string userName, string email, int? year, string country, 
            int page = 1, SortState sortOrder = SortState.NameAscending)
        {
            const int PAGE_SIZE = 5;
            int usersNumber;

            IQueryable<User> users = _userManager.Users;
            FilterUsers(ref users, userName, email, year, country);
            users = ChooseSort(sortOrder, users);

            usersNumber = await users.CountAsync();
            var currentPageUsers = await users.Skip((page - 1) * PAGE_SIZE).Take(PAGE_SIZE).ToListAsync();

            FilterSortPageViewModel model = new FilterSortPageViewModel()
            {
                Users = currentPageUsers,
                FilterViewModel = new FilterViewModel(userName, email, year, country),
                SortViewModel = new SortViewModel(sortOrder),
                PageViewModel = new PageViewModel(page, usersNumber, PAGE_SIZE)
            };

            return View(model);
        }

        private void FilterUsers(ref IQueryable<User> users, string userName, 
            string email, int? year, string country)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                users = from user in users where user.UserName.Contains(userName) select user;
            }

            if (!string.IsNullOrEmpty(email))
            {
                users = from user in users where user.Email.Contains(email) select user;
            }

            if (year != null)
            {
                users = from user in users where user.Year == year select user;
            }

            if (!string.IsNullOrEmpty(country))
            {
                users = from user in users where user.Country.Contains(country) select user;
            }
        }


        private IQueryable<User> ChooseSort(SortState sortOrder, IQueryable<User> users)
        {
            return sortOrder switch
            {
                SortState.NameDescending => users.OrderByDescending(u => u.UserName),
                SortState.EmailAscending => users.OrderBy(u => u.Email),
                SortState.EmailDescending => users.OrderByDescending(u => u.Email),
                SortState.YearAscending => users.OrderBy(u => u.Year),
                SortState.YearDescending => users.OrderByDescending(u => u.Year),
                SortState.CountryAscending => users.OrderBy(u => u.Country),
                SortState.CountryDescending => users.OrderByDescending(u => u.Country),
                _ => users.OrderBy(u => u.UserName)
            };
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
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
                Company = user.Company,
            };

            Stream stream = new MemoryStream(user.ProfilePicture);
            model.ProfilePicture = new FormFile(stream, 0, user.ProfilePicture.Length, "name", "filename");

            return View(model);
        }

        [HttpPost]
        [Authorize]
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
                        return RedirectToAction("Index");
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

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ChangePassword(string userId, string returnUrl)
        {
            User user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                return BadRequest();
            }

            ChangePasswordViewModel model = new ChangePasswordViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);

                if (user != null)
                {
                    IdentityResult result =
                        await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return LocalRedirect(model.ReturnUrl);
                        }

                        return BadRequest();
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
                    ModelState.AddModelError("", "The user has not been found");
                }
            }

            return View(model);
        }
    }
}
