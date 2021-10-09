using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using IdentityApp.Models;
using IdentityApp.ViewModels;

namespace IdentityApp.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public PostController(UserManager<User> userManager,
            ApplicationDbContext context, ILogger<PostController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Create(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                User user = await _userManager.Users
                    .FirstOrDefaultAsync(user => user.Id == userId);

                if (user != null)
                {
                    CreatePostViewModel model = new CreatePostViewModel()
                    {
                        User = user
                    };

                    return View(model);
                }
            }

            _logger.LogError("User not found");
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePostViewModel model)
        {
            if (model.PostPictures != null)
            {
                if (model.PostPictures.Count > 5)
                {
                    ModelState.AddModelError("",
                        "A post can contain up to 5 pictures");
                    _logger.LogWarning("A post can contain up to 5 pictures");
                }
            }

            if (ModelState.IsValid)
            {
                User user = await _userManager.Users
                    .FirstOrDefaultAsync(user => user.Id == model.User.Id);

                Post post = new Post()
                {
                    Id = model.Id,
                    Content = model.Content,
                    PostedTime = model.PostedTime,
                    UserId = user.Id
                };

                if (model.PostPictures != null)
                {
                    foreach (IFormFile postPic in model.PostPictures)
                    {
                        byte[] pictureData = null;

                        using (BinaryReader binaryReader = new BinaryReader(
                            postPic.OpenReadStream()))
                        {
                            pictureData = binaryReader.ReadBytes(
                                (int)postPic.Length);
                        }

                        PostPicture postPicture = new PostPicture()
                        {
                            Id = Guid.NewGuid().ToString(),
                            PictureData = pictureData,
                            UploadedTime = DateTime.Now
                        };

                        post.PostPictures.Add(postPicture);
                    }
                }

                if (user != null)
                {
                    if (post.Content == null)
                    {
                        ModelState.AddModelError("", "The length of your " +
                            "post must be between 1 and 350 symbols");
                        _logger.LogWarning("The length of a post must be " +
                            "between 1 and 350 symbols");
                    }
                    else
                    {
                        user.Posts.Add(post);
                        await _userManager.UpdateAsync(user);
                        _logger.LogInformation($"User {user.UserName} " +
                            "created a post");

                        return RedirectToAction("Index", "Account",
                            new { userName = user.UserName });
                    }
                }
            }

            _logger.LogWarning("CreatePostViewModel is not valid");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string postId, string returnUrl)
        {
            Post post = await _context.Posts
                .FirstOrDefaultAsync(post => post.Id == postId);

            if (post == null)
            {
                _logger.LogError("Post not found");
                return NotFound();
            }

            EditPostViewModel model = new EditPostViewModel()
            {
                Id = post.Id,
                Content = post.Content,
                PostedTime = post.PostedTime,
                UserId = post.UserId,
                UserName = post.User.UserName,
                ReturnUrl = returnUrl,
                PostPictures = post.PostPictures
                    .OrderByDescending(postPic => postPic.UploadedTime)
                    .AsEnumerable()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditPostViewModel model)
        {
            if (model.AppendedPostPictures != null)
            {
                if (model.CopiedPostPictures != null)
                {
                    if (model.AppendedPostPictures.Count() > 5
                        || model.PostPictures.Count()
                        + model.AppendedPostPictures.Count() > 5)
                    {
                        ModelState.AddModelError("",
                            "A post can contain up to 5 pictures");
                        _logger.LogWarning("A post can contain up to " +
                            "5 pictures");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                Post post = await _context.Posts
                    .FirstOrDefaultAsync(post => post.Id == model.Id);

                if (post != null)
                {
                    if (model.Content != null)
                    {
                        User user = await _userManager
                            .FindByIdAsync(model.UserId);

                        if (model.AppendedPostPictures != null)
                        {
                            foreach (var postPic in model.AppendedPostPictures)
                            {
                                byte[] pictureData = null;

                                using (BinaryReader binaryReader =
                                    new BinaryReader(postPic.OpenReadStream()))
                                {
                                    pictureData = binaryReader.ReadBytes(
                                        (int)postPic.Length);
                                }

                                PostPicture postPicture = new PostPicture()
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    PictureData = pictureData,
                                    UploadedTime = DateTime.Now
                                };

                                post.PostPictures.Add(postPicture);
                            }
                        }

                        post.Content = model.Content;
                        post.PostedTime = model.PostedTime;
                        post.IsEdited = true;

                        _context.Update(post);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation($"User {user.UserName}'s post " +
                            "was edited");

                        if (model.ReturnUrl.Contains("Account"))
                        {
                            return RedirectToAction("Index", "Account",
                                new { userName = user.UserName });
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        model.PostPictures = post.PostPictures;
                        ModelState.AddModelError("", "The length of your " +
                            "post must be between 1 and 350 symbols");
                    }
                }
                else
                {
                    _logger.LogError("Post not found");
                    return NotFound();
                }
            }

            _logger.LogWarning("EditPostViewModel is not valid");
            return View(model);
        }

        public async Task<IActionResult> Delete(string postId, string returnUrl)
        {
            Post post = await _context.Posts
                .FirstOrDefaultAsync(post => post.Id == postId);
            User user = await _userManager.FindByIdAsync(post.UserId);

            if (post != null)
            {
                IEnumerable<LikedPost> likedPosts = _context.LikedPosts
                    .Where(likedPost => likedPost.PostId == post.Id)
                    .AsEnumerable();

                if (likedPosts != null)
                {
                    _context.LikedPosts.RemoveRange(likedPosts);
                }

                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"User {user.UserName}'s post " +
                    "was deleted");
            }
            else
            {
                _logger.LogError("Post not found");
                return NotFound();
            }

            if (returnUrl.Contains("Account"))
            {
                return RedirectToAction("Index", "Account",
                    new { userName = user.UserName });
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Like(PostLikeViewModel model)
        {
            User user = await _userManager.FindByIdAsync(model.UserId);

            if (user != null)
            {
                Post post = await _context.Posts
                    .FirstOrDefaultAsync(post => post.Id == model.PostId);

                if (post != null)
                {
                    LikedPost postToCheck = user.LikedPosts
                        .FirstOrDefault(post => post.UserId == model.UserId
                                        && post.PostId == model.PostId);

                    if (postToCheck != null)
                    {
                        user.LikedPosts.Remove(postToCheck);
                        post.Likes--;
                        _logger.LogInformation($"User {user.UserName} removed " +
                            "a like from a post");
                    }
                    else
                    {
                        user.LikedPosts.Add(
                            new LikedPost()
                            {
                                UserId = user.Id,
                                User = user,
                                PostId = post.Id,
                                Post = post
                            });
                        post.Likes++;
                        _logger.LogInformation($"User {user.UserName} " +
                            "liked a post");
                    }

                    await _context.SaveChangesAsync();
                }
                else
                {
                    _logger.LogError("Post not found");
                    return NotFound();
                }

                if (model.ReturnAction.Contains("Account"))
                {
                    return RedirectToAction("Index", "Account",
                        new
                        {
                            userName = model.LikedPostUserName,
                            page = model.Page
                        });
                }

                return RedirectToAction("Index", "Home",
                    new { page = model.Page });
            }
            else
            {
                _logger.LogError("User not found");
                return NotFound();
            }
        }
    }
}