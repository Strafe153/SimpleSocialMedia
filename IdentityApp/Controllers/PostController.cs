using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
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

        public PostController(UserManager<User> userManager, 
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
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

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePostViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.Users
                    .FirstOrDefaultAsync(user => user.Id == model.Post.User.Id);

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

                        model.Post.PostPictures.Add(postPicture);
                    }
                }

                if (user != null)
                {
                    user.Posts.Add(model.Post);
                    await _userManager.UpdateAsync(user);

                    return RedirectToAction("Index", "Account", 
                        new { userName = user.UserName });
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string postId, string returnUrl)
        {
            Post post = await _context.Posts
                .FirstOrDefaultAsync(post => post.Id == postId);

            if (post == null)
            {
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
            if (ModelState.IsValid)
            {
                Post post = await _context.Posts
                    .FirstOrDefaultAsync(post => post.Id == model.Id);

                if (post != null)
                {
                    User user = await _userManager.FindByIdAsync(model.UserId);

                    if (model.AppendedPostPictures != null)
                    {
                        foreach (IFormFile postPic in model.AppendedPostPictures)
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

                    post.Content = model.Content;
                    post.PostedTime = model.PostedTime;
                    post.IsEdited = true;
                    _context.Update(post);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "Account", 
                        new { userName = user.UserName });
                }
                else
                {
                    return BadRequest();
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(string postId, string returnUrl)
        {
            Post post = await _context.Posts
                .FirstOrDefaultAsync(post => post.Id == postId);
            User user = await _userManager.FindByIdAsync(post.UserId);

            if (post != null)
            {
                List<LikedPost> likedPosts = await _context.LikedPosts
                    .Where(likedPost => likedPost.PostId == post.Id)
                    .ToListAsync();

                if (likedPosts != null)
                {
                    _context.LikedPosts.RemoveRange(likedPosts);
                }

                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
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
                    }

                    await _context.SaveChangesAsync();
                }

                if (model.ReturnAction.Contains("Account"))
                {
                    return RedirectToAction("Index", "Account", 
                        new { userName = model.LikedPostUserName,
                              page = model.Page });
                }

                return RedirectToAction("Index", "Home", 
                    new { page = model.Page });
            }
            else
            {
                return NotFound();
            }
        }
    }
}