using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System;
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
    [Authorize]
    public class PostsController : Controller
    {
        private readonly IPostsControllable _repo;

        public PostsController(IPostsControllable repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> Create(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                User user = await _repo.FirstOrDefaultAsync(_repo.GetAllUsers(), u => u.Id == userId);

                if (user != null)
                {
                    var model = new CreatePostViewModel() 
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
            CheckPostPicturesCount(model.PostPictures);

            if (ModelState.IsValid)
            {
                User user = await _repo.FirstOrDefaultAsync(
                    _repo.GetAllUsers(), u => u.Id == model.User.Id);

                var post = new Post()
                {
                    Id = model.Id,
                    Content = model.Content,
                    PostedTime = DateTime.Now
                };

                AddPostPicturesToPost(model.PostPictures, post);

                if (user != null)
                {
                    post.UserId = user.Id;

                    if (post.Content == null)
                    {
                        ModelState.AddModelError("", "The length of a post " +
                            "must be between 1 and 350 symbols");
                    }
                    else
                    {
                        user.Posts.Add(post);
                        await _repo.UpdateAsync(user);
                        _repo.LogInformation($"User {user.UserName} created a post");

                        return RedirectToAction("Index", "Accounts", new { userName = user.UserName });
                    }
                }
                else
                {
                    return NotFound();
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string postId, string returnUrl, int page)
        {
            Post post = await _repo.FirstOrDefaultAsync(
                _repo.GetAllPosts(), p => p.Id == postId);

            if (post == null)
            {
                return NotFound();
            }

            var model = new EditPostViewModel()
            {
                Id = post.Id,
                Content = post.Content,
                UserId = post.UserId,
                UserName = post.User.UserName,
                CalledFromAction = returnUrl,
                PostPictures = post.PostPictures
                    .OrderByDescending(postPic => postPic.UploadedTime)
                    .AsEnumerable(),
                Page = page
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditPostViewModel model)
        {
            Post post = await _repo.FirstOrDefaultAsync(
                _repo.GetAllPosts(), p => p.Id == model.Id);

            if (post != null)
            {
                CheckPostPicturesCount(model.AppendedPostPictures, post.PostPictures);

                if (ModelState.IsValid)
                {
                    if (model.Content != null)
                    {
                        User user = await _repo.FindByIdAsync(model.UserId);

                        AddPostPicturesToPost(model.AppendedPostPictures, post);
                        post.Content = model.Content;
                        post.IsEdited = true;

                        _repo.Update(post);
                        await _repo.SaveChangesAsync();
                        _repo.LogInformation($"User {user.UserName}'s post was edited");

                        if (model.CalledFromAction.Contains("Accounts"))
                        {
                            return RedirectToAction("Index", "Accounts", new { 
                                userName = user.UserName, page = model.Page });
                        }

                        return RedirectToAction(model.CalledFromAction.Contains("Feed")
                            ? "Feed" : "Index", "Home", new { page = model.Page });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The length of your post must " +
                            "be between 1 and 350 symbols");
                    }
                }

                model.PostPictures = post.PostPictures.OrderByDescending(p => p.UploadedTime);

                return View(model);
            }

            return NotFound();
        }

        public async Task<IActionResult> Delete(string postId, string returnUrl, int page)
        {
            Post post = await _repo.FirstOrDefaultAsync(
                _repo.GetAllPosts(), p => p.Id == postId);
            User user = await _repo.FindByIdAsync(post.UserId);

            if (post != null)
            {
                IEnumerable<LikedPost> likedPosts = _repo.GetAllLikedPosts()
                    .Where(lp => lp.PostLikedId == post.Id).AsEnumerable();
                IEnumerable<PostComment> comments = _repo.GetAllPostComments()
                    .Where(c => c.PostId == post.Id).AsEnumerable();
                IEnumerable<LikedComment> likedComments = _repo.GetAllLikedPostComments();

                if (likedPosts != null)
                {
                    _repo.RemoveLikedCommentsRange(likedComments);
                    _repo.RemoveCommentsRange(comments);
                    _repo.RemoveLikedPostsRange(likedPosts);
                }

                _repo.Remove(post);
                await _repo.SaveChangesAsync();
                _repo.LogInformation($"User {user.UserName}'s post was deleted");
            }
            else
            {
                return NotFound();
            }

            if (returnUrl.Contains("Accounts"))
            {
                return RedirectToAction("Index", "Accounts",
                    new { userName = user.UserName, page = page });
            }

            return RedirectToAction(returnUrl.Contains("Feed")
                ? "Feed" : "Index", "Home", new { page = page });
        }

        public async Task<IActionResult> Like(LikeViewModel model)
        {
            User user = await _repo.FindByIdAsync(model.UserId);

            if (user != null)
            {
                Post post = await _repo.FirstOrDefaultAsync(
                    _repo.GetAllPosts(), p => p.Id == model.Id);

                if (post != null)
                {
                    LikedPost postToCheck = user.LikedPosts.FirstOrDefault(lp =>
                        lp.UserWhoLikedId == model.UserId && lp.PostLikedId == model.Id);

                    LikeOrDislikePost(post, postToCheck, user);
                    await _repo.SaveChangesAsync();
                }
                else
                {
                    return NotFound();
                }

                if (model.ReturnAction.Contains("Accounts"))
                {
                    return RedirectToAction("Index", "Accounts", 
                        new { userName = post.User.UserName, page = model.Page });
                }

                return RedirectToAction(model.ReturnAction.Contains("Feed")
                    ? "Feed" : "Index", "Home", new { page = model.Page });
            }
            else
            {
                return NotFound();
            }
        }

        private void CheckPostPicturesCount(IFormFileCollection appendedPostPictures,
            IEnumerable<PostPicture> postPictures = null)
        {
            if (appendedPostPictures != null)
            {
                if (appendedPostPictures.Count() > 5)
                {
                    ModelState.AddModelError("", "A post can contain up to 5 pictures");
                    return;
                }

                if (postPictures != null)
                {
                    if (postPictures.Count() + appendedPostPictures.Count() > 5)
                    {
                        ModelState.AddModelError("", "A post can contain up to 5 pictures");
                    }
                }
            }
        }

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

                    pictureData = PictureUtility.ResizeImage(pictureData, 200);

                    var postPicture = new PostPicture()
                    {
                        Id = Guid.NewGuid().ToString(),
                        PictureData = pictureData,
                        UploadedTime = DateTime.Now
                    };

                    post.PostPictures.Add(postPicture);
                }
            }
        }

        private void LikeOrDislikePost(Post postToLike, LikedPost postToCheck, User user)
        {
            if (postToCheck != null)
            {
                user.LikedPosts.Remove(postToCheck);
                postToLike.Likes--;
                _repo.LogInformation($"User {user.UserName} removed a like from a post");
            }
            else
            {
                user.LikedPosts.Add(new LikedPost() 
                { 
                    UserWhoLikedId = user.Id, 
                    UserWhoLiked = user, 
                    PostLikedId = postToLike.Id, 
                    PostLiked = postToLike 
                });

                postToLike.Likes++;
                _repo.LogInformation($"User {user.UserName} liked a post");
            }
        }
    }
}