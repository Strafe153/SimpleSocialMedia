using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using IdentityApp.Models;
using IdentityApp.Utilities;
using IdentityApp.ViewModels;
using IdentityApp.Interfaces;

namespace IdentityApp.Controllers
{
    public class PostCommentController : Controller
    {
        private readonly IPostCommentControllable _repository;

        public PostCommentController(IPostCommentControllable repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Create(CreatePostCommentViewModel model)
        {
            if (!string.IsNullOrEmpty(model.PostId))
            {
                Post post = await _repository.FirstOrDefaultAsync(
                    _repository.GetAllPosts(), p => p.Id == model.PostId);

                if (post != null)
                {
                    if (string.IsNullOrEmpty(model.CommentContent))
                    {
                        ModelState.AddModelError("", "The length of your " +
                            "comment must be between 1 and 200 symbols");
                        _repository.LogWarning("The length of a comment must be between 1 and 200 symbols");
                    }
                    else
                    {
                        PostComment postComment = new PostComment()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Author = model.CommentAuthorName,
                            Content = model.CommentContent,
                            CommentedTime = DateTime.Now,
                            PostId = post.Id
                        };

                        AddPicturesToComment(model.CommentPictures, postComment);
                        post.PostComments.Add(postComment);
                        await _repository.SaveChangesAsync();
                        _repository.LogInformation($"User {model.CommentAuthorName} created a comment");

                        if (model.ReturnUrl.Contains("Account"))
                        {
                            return RedirectToAction("Index", "Account", new { 
                                userName = post.User.UserName, page = model.Page });
                        }

                        return RedirectToAction(model.ReturnUrl.Contains("Feed") 
                            ? "Feed" : "Index", "Home", new { page = model.Page });
                    }
                }
            }

            _repository.LogError("Post not found");
            return NotFound();
        }

        public async Task<IActionResult> Delete(ManagePostCommentViewModel model)
        {
            PostComment comment = await _repository.FirstOrDefaultAsync(
                _repository.GetAllComments(), c => c.Id == model.CommentId);
            Post post = await _repository.FirstOrDefaultAsync(
                _repository.GetAllPosts(), p => p.Id == comment.PostId);

            if (comment != null)
            {
                IEnumerable<LikedComment> likedComments = _repository.GetAllLikedComments()
                    .Where(lc => lc.CommentLikedId == comment.Id).AsEnumerable();

                if (likedComments != null)
                {
                    _repository.RemoveRange(likedComments);
                }

                _repository.Remove(comment);
                await _repository.SaveChangesAsync();
                _repository.LogInformation($"User {comment.Author}'s comment was deleted");
            }
            else
            {
                _repository.LogError("Comment not found");
                return NotFound();
            }

            if (model.ReturnUrl.Contains("Account"))
            {
                return RedirectToAction("Index", "Account", new {
                    userName = post.User.UserName, page = model.Page });
            }

            return RedirectToAction(model.ReturnUrl.Contains("Feed")
                ? "Feed" : "Index", "Home", new { page = model.Page });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string postCommentId, string calledFromAction, int page)
        {
            if (!string.IsNullOrEmpty(postCommentId))
            {
                PostComment comment = await _repository.FirstOrDefaultAsync(
                    _repository.GetAllComments(), c => c.Id == postCommentId);

                if (comment == null)
                {
                    _repository.LogError("Comment not found");
                    return NotFound();
                }

                ManagePostCommentViewModel model = new ManagePostCommentViewModel()
                {
                    CommentId = postCommentId,
                    Content = comment.Content,
                    Author = comment.Author,
                    CommentedPostUser = comment.Post.User.UserName,
                    CommentPictures = comment.CommentPictures.OrderByDescending(
                        cp => cp.UploadedTime).AsEnumerable(),
                    ReturnUrl = calledFromAction,
                    Page = page
                };

                return View(model);
            }

            _repository.LogError("Comment id is not passed");
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ManagePostCommentViewModel model)
        {
            PostComment comment = await _repository.FirstOrDefaultAsync(
                _repository.GetAllComments(), c => c.Id == model.CommentId);

            if (comment == null)
            {
                _repository.LogError("Comment not found");
                return NotFound();
            }

            CheckCommentPicturesCount(model.AppendedCommentPictures, comment.CommentPictures);

            if (ModelState.IsValid)
            {
                if (model.Content != null)
                {
                    AddPicturesToComment(model.AppendedCommentPictures, comment);
                    comment.Content = model.Content;
                    comment.IsEdited = true;

                    _repository.GetAllComments().Update(comment);
                    await _repository.SaveChangesAsync();
                    _repository.LogInformation($"User {model.Author}'s post was edited");

                    if (model.ReturnUrl.Contains("Account"))
                    {
                        return RedirectToAction("Index", "Account", new {
                            userName = model.CommentedPostUser, page = model.Page });
                    }

                    return RedirectToAction(model.ReturnUrl.Contains("Feed")
                        ? "Feed" : "Index", "Home", new { page = model.Page });
                }
                else
                {
                    ModelState.AddModelError("", "The length of your comment must be between 1 and 200 symbols");
                }
            }

            model.CommentPictures = comment.CommentPictures.OrderByDescending(cp => cp.UploadedTime);
            _repository.LogWarning("ManagePostCommentViewModel is not valid");
            return View(model);
        }

        public async Task<IActionResult> Like(LikeViewModel model)
        {
            User user = await _repository.FindByIdAsync(model.UserId);

            if (user != null)
            {
                PostComment comment = await _repository.FirstOrDefaultAsync(
                    _repository.GetAllComments(), c => c.Id == model.Id);

                if (comment != null)
                {
                    LikedComment commentToCheck = user.LikedComments.FirstOrDefault(c =>
                        c.UserWhoLikedId == model.UserId && c.CommentLikedId == model.Id);

                    LikeDislikeComment(comment, commentToCheck, user);
                    await _repository.SaveChangesAsync();
                }
                else
                {
                    _repository.LogError("Comment not found");
                    return NotFound();
                }

                if (model.ReturnAction.Contains("Account"))
                {
                    return RedirectToAction("Index", "Account",
                        new { userName = comment.Post.User.UserName, page = model.Page });
                }

                return RedirectToAction(model.ReturnAction.Contains("Feed")
                    ? "Feed" : "Index", "Home", new { page = model.Page });
            }
            else
            {
                _repository.LogError("User not found");
                return BadRequest();
            }
        }

        private void LikeDislikeComment(PostComment commentToLike, LikedComment commentToCheck, User user)
        {
            if (commentToCheck != null)
            {
                user.LikedComments.Remove(commentToCheck);
                commentToLike.Likes--;
                _repository.LogInformation($"User {user.UserName} removed a like from a comment");
            }
            else
            {
                user.LikedComments.Add(new LikedComment()
                {
                    UserWhoLikedId = user.Id,
                    UserWhoLiked = user,
                    CommentLikedId = commentToLike.Id,
                    CommentLiked = commentToLike
                });
                commentToLike.Likes++;
                _repository.LogInformation($"User {user.UserName} liked a comment");
            }
        }

        private void AddPicturesToComment(IFormFileCollection picturesToAdd, PostComment comment)
        {
            if (picturesToAdd != null)
            {
                foreach (IFormFile picture in picturesToAdd)
                {
                    byte[] pictureData = null;

                    using (BinaryReader binaryReader = new BinaryReader(picture.OpenReadStream()))
                    {
                        pictureData = binaryReader.ReadBytes((int)picture.Length);
                    }

                    pictureData = PictureUtility.ResizeImage(pictureData, 200);

                    CommentPicture commentPicture = new CommentPicture()
                    {
                        Id = Guid.NewGuid().ToString(),
                        PictureData = pictureData,
                        UploadedTime = DateTime.Now
                    };

                    comment.CommentPictures.Add(commentPicture);
                }
            }
        }

        private void CheckCommentPicturesCount(IFormFileCollection appendedCommentPictures,
            IEnumerable<CommentPicture> commentPictures = null)
        {
            string errorMessage = "A comment can contain up to 5 pictures";

            if (appendedCommentPictures != null)
            {
                if (appendedCommentPictures.Count() > 5)
                {
                    ModelState.AddModelError("", errorMessage);
                    _repository.LogWarning(errorMessage);
                    return;
                }

                if (commentPictures != null)
                {
                    if (commentPictures.Count() + appendedCommentPictures.Count() > 5)
                    {
                        ModelState.AddModelError("", errorMessage);
                        _repository.LogWarning(errorMessage);
                    }
                }
            }
        }
    }
}
