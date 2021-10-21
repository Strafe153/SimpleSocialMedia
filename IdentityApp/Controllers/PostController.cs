using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using IdentityApp.Models;
using IdentityApp.ViewModels;
using IdentityApp.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;

namespace IdentityApp.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly IPostControllable _repository;

        public PostController(IPostControllable repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Create(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                User user = await _repository.FirstOrDefaultAsync(
                    _repository.GetAllUsers(), user => user.Id == userId);

                if (user != null)
                {
                    CreatePostViewModel model = new CreatePostViewModel() { User = user };
                    return View(model);
                }
            }

            _repository.LogError("User not found");
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePostViewModel model)
        {
            CheckPostPicturesCount(model.PostPictures);

            if (ModelState.IsValid)
            {
                User user = await _repository.FirstOrDefaultAsync(
                    _repository.GetAllUsers(), user => user.Id == model.User.Id);

                Post post = new Post()
                {
                    Id = model.Id,
                    Content = model.Content,
                    PostedTime = model.PostedTime
                };

                AddPostPicturesToPost(model.PostPictures, post);

                if (user != null)
                {
                    post.UserId = user.Id;

                    if (post.Content == null)
                    {
                        ModelState.AddModelError("", "The length of your " +
                            "post must be between 1 and 350 symbols");
                        _repository.LogWarning("The length of a post must be between 1 and 350 symbols");
                    }
                    else
                    {
                        user.Posts.Add(post);
                        await _repository.UpdateAsync(user);
                        _repository.LogInformation($"User {user.UserName} created a post");

                        return RedirectToAction("Index", "Account", new { userName = user.UserName });
                    }
                }
                else
                {
                    _repository.LogError($"User not found");
                    return NotFound();
                }
            }

            _repository.LogWarning("CreatePostViewModel is not valid");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string postId, string returnUrl)
        {
            Post post = await _repository.FirstOrDefaultAsync(
                _repository.GetAllPosts(), post => post.Id == postId);

            if (post == null)
            {
                _repository.LogError("Post not found");
                return NotFound();
            }

            EditPostViewModel model = new EditPostViewModel()
            {
                Id = post.Id,
                Content = post.Content,
                PostedTime = post.PostedTime,
                UserId = post.UserId,
                UserName = post.User.UserName,
                CalledFromAction = returnUrl,
                PostPictures = post.PostPictures
                    .OrderByDescending(postPic => postPic.UploadedTime).AsEnumerable()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditPostViewModel model)
        {
            Post post = await _repository.FirstOrDefaultAsync(
                _repository.GetAllPosts(), post => post.Id == model.Id);

            if (post != null)
            {
                CheckPostPicturesCount(model.AppendedPostPictures, post.PostPictures);

                if (ModelState.IsValid)
                {
                    if (model.Content != null)
                    {
                        User user = await _repository.FindByIdAsync(model.UserId);

                        AddPostPicturesToPost(model.AppendedPostPictures, post);
                        post.Content = model.Content;
                        post.PostedTime = model.PostedTime;
                        post.IsEdited = true;

                        _repository.Update(post);
                        await _repository.SaveChangesAsync();
                        _repository.LogInformation($"User {user.UserName}'s post was edited");

                        if (model.CalledFromAction.Contains("Account"))
                        {
                            return RedirectToAction("Index", "Account", new { userName = user.UserName });
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "The length of your post must " +
                            "be between 1 and 350 symbols");
                    }
                }

                model.PostPictures = post.PostPictures.OrderByDescending(postPic => postPic.UploadedTime);

                _repository.LogWarning("EditPostViewModel is not valid");
                return View(model);
            }

            _repository.LogError("Post not found");
            return NotFound();
        }

        public async Task<IActionResult> Delete(string postId, string returnUrl)
        {
            Post post = await _repository.FirstOrDefaultAsync(
                _repository.GetAllPosts(), post => post.Id == postId);
            User user = await _repository.FindByIdAsync(post.UserId);

            if (post != null)
            {
                IEnumerable<LikedPost> likedPosts = _repository.GetAllLikedPosts()
                    .Where(likedPost => likedPost.PostId == post.Id).AsEnumerable();

                if (likedPosts != null)
                {
                    _repository.RemoveRange(likedPosts);
                }

                _repository.Remove(post);
                await _repository.SaveChangesAsync();
                _repository.LogInformation($"User {user.UserName}'s post was deleted");
            }
            else
            {
                _repository.LogError("Post not found");
                return NotFound();
            }

            if (returnUrl.Contains("Account"))
            {
                return RedirectToAction("Index", "Account", new { userName = user.UserName });
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Like(PostLikeViewModel model)
        {
            User user = await _repository.FindByIdAsync(model.UserId);

            if (user != null)
            {
                Post post = await _repository.FirstOrDefaultAsync(
                    _repository.GetAllPosts(), post => post.Id == model.PostId);

                if (post != null)
                {
                    LikedPost postToCheck = user.LikedPosts.FirstOrDefault(post =>
                        post.UserId == model.UserId && post.PostId == model.PostId);

                    LikeDislikePost(post, postToCheck, user);
                    await _repository.SaveChangesAsync();
                }
                else
                {
                    _repository.LogError("Post not found");
                    return NotFound();
                }

                if (model.ReturnAction.Contains("Account"))
                {
                    return RedirectToAction("Index", "Account", 
                        new { userName = model.LikedPostUserName, page = model.Page });
                }

                return RedirectToAction("Index", "Home", new { page = model.Page });
            }
            else
            {
                _repository.LogError("User not found");
                return NotFound();
            }
        }

        /// <summary>
        /// Checks if the number of a post's pictures exceeds 5
        /// </summary>
        /// <param name="appendedPostPictures"></param>
        /// <param name="postPictures"></param>
        private void CheckPostPicturesCount(IFormFileCollection appendedPostPictures,
            IEnumerable<PostPicture> postPictures = null)
        {
            if (appendedPostPictures != null)
            {
                if (appendedPostPictures.Count() > 5)
                {
                    ModelState.AddModelError("", "A post can contain up to 5 pictures");
                    _repository.LogWarning("A post can contain up to 5 pictures");
                    return;
                }

                if (postPictures != null)
                {
                    if (postPictures.Count() + appendedPostPictures.Count() > 5)
                    {
                        ModelState.AddModelError("", "A post can contain up to 5 pictures");
                        _repository.LogWarning("A post can contain up to 5 pictures");
                    }
                }
            }
        }

        /// <summary>
        /// Adds pictures to a post
        /// </summary>
        /// <param name="picturesToAdd"></param>
        /// <param name="post"></param>
        private void AddPostPicturesToPost(IFormFileCollection picturesToAdd, Post post)
        {
            if (picturesToAdd != null)
            {
                foreach (IFormFile postPic in picturesToAdd)
                {
                    byte[] pictureData = null;

                    using (BinaryReader binaryReader = new BinaryReader(postPic.OpenReadStream()))
                    {
                        pictureData = binaryReader.ReadBytes((int)postPic.Length);
                    }

                    pictureData = ResizeImage(pictureData);

                    PostPicture postPicture = new PostPicture()
                    {
                        Id = Guid.NewGuid().ToString(),
                        PictureData = pictureData,
                        UploadedTime = DateTime.Now
                    };

                    post.PostPictures.Add(postPicture);
                }
            }
        }

        /// <summary>
        /// Adds or removes a like according to the post's state
        /// </summary>
        /// <param name="postToLike"></param>
        /// <param name="postToCheck"></param>
        /// <param name="user"></param>
        private void LikeDislikePost(Post postToLike, LikedPost postToCheck, User user)
        {
            if (postToCheck != null)
            {
                user.LikedPosts.Remove(postToCheck);
                postToLike.Likes--;
                _repository.LogInformation($"User {user.UserName} removed a like from a post");
            }
            else
            {
                user.LikedPosts.Add(new LikedPost() { 
                    UserId = user.Id, User = user, PostId = postToLike.Id, Post = postToLike });
                postToLike.Likes++;
                _repository.LogInformation($"User {user.UserName} liked a post");
            }
        }

        /// <summary>
        /// Proportionally resizes an image to the height of 200px
        /// </summary>
        /// <param name="imageToResize"></param>
        /// <returns></returns>
        private byte[] ResizeImage(byte[] imageToResize)
        {
            byte[] resizedImage = null;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (Image image = Image.Load(imageToResize))
                {
                    int height = 200;
                    double coefficient = (double)image.Height / height;
                    double width = image.Width / coefficient;

                    image.Mutate(img => img.Resize((int)width, height));
                    image.Save(memoryStream, new PngEncoder());
                }

                resizedImage = memoryStream.ToArray();
            }

            return resizedImage;
        }
    }
}