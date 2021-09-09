﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Models;
using IdentityApp.ViewModels;
using System.IO;

namespace IdentityApp.Controllers
{
    public class PostController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public PostController(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Create(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                User user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

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
                User user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == model.Post.User.Id);

                if (model.PostPictures != null)
                {
                    foreach (IFormFile postPic in model.PostPictures)
                    {
                        byte[] pictureData = null;

                        using (BinaryReader binaryReader = new BinaryReader(postPic.OpenReadStream()))
                        {
                            pictureData = binaryReader.ReadBytes((int)postPic.Length);
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

                    return RedirectToAction("Index", "Account", new { userName = user.UserName });
                }
            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(string postId)
        {
            Post post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);

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
                PostPictures = (from postPic in post.PostPictures
                                orderby postPic.UploadedTime descending
                                select postPic).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(EditPostViewModel model)
        {
            if (ModelState.IsValid)
            {
                Post post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == model.Id);

                if (post != null)
                {
                    User user = await _userManager.FindByIdAsync(model.UserId);

                    if (model.AppendedPostPictures != null)
                    {
                        foreach (IFormFile postPic in model.AppendedPostPictures)
                        {
                            byte[] pictureData = null;

                            using (BinaryReader binaryReader = new BinaryReader(postPic.OpenReadStream()))
                            {
                                pictureData = binaryReader.ReadBytes((int)postPic.Length);
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

                    return RedirectToAction("Index", "Account", new { userName = user.UserName });
                }
                else
                {
                    return BadRequest();
                }
            }

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Delete(string postId)
        {
            Post post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            User user = await _userManager.FindByIdAsync(post.UserId);

            if (post != null)
            {
                _context.Posts.Remove(post);
                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Account", new { userName = user.UserName });
        }

        public async Task<IActionResult> Like(string postId)
        {
            Post post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);

            if (post != null)
            {
                if (post.IsLiked)
                {
                    post.IsLiked = false;
                    post.Likes -= 1;
                    _context.SaveChanges();
                }
                else
                {
                    post.IsLiked = true;
                    post.Likes += 1;
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("Index", "Account", new { userName = post.UserId });
        }
    }
}
