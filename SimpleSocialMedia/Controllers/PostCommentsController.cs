using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
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
    public class PostCommentsController : Controller
    {
        private readonly IPostCommentsControllable _repo;

        public PostCommentsController(IPostCommentsControllable repo)
        {
            _repo = repo;
        }

        public async Task<IActionResult> Create(CreatePostCommentViewModel model)
        {
            if (!string.IsNullOrEmpty(model.PostId))
            {
                Post post = await _repo.FirstOrDefaultAsync(
                    _repo.GetAllPosts(), p => p.Id == model.PostId);

                if (post != null)
                {
                    if (string.IsNullOrEmpty(model.CommentContent))
                    {
                        ModelState.AddModelError("", "The length of a " +
                            "comment must be between 1 and 200 symbols");
                    }
                    else
                    {
                        var postComment = new PostComment()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Author = model.CommentAuthorName,
                            Content = model.CommentContent,
                            CommentedTime = DateTime.Now,
                            PostId = post.Id
                        };

                        AddPicturesToComment(model.CommentPictures, postComment);
                        post.PostComments.Add(postComment);
                        await _repo.SaveChangesAsync();
                        _repo.LogInformation($"User {model.CommentAuthorName} created a comment");

                        if (model.ReturnUrl.Contains("Accounts"))
                        {
                            return RedirectToAction("Index", "Accounts", new { 
                                userName = post.User.UserName, page = model.Page });
                        }

                        return RedirectToAction(model.ReturnUrl.Contains("Feed") 
                            ? "Feed" : "Index", "Home", new { page = model.Page });
                    }
                }
            }

            return NotFound();
        }

        public async Task<IActionResult> Delete(EditPostCommentViewModel model)
        {
            PostComment comment = await _repo.FirstOrDefaultAsync(
                _repo.GetAllComments(), c => c.Id == model.CommentId);
            Post post = await _repo.FirstOrDefaultAsync(
                _repo.GetAllPosts(), p => p.Id == comment.PostId);

            if (comment != null)
            {
                var likedComments = _repo.GetAllLikedComments()
                    .Where(lc => lc.CommentLikedId == comment.Id)
                    .AsEnumerable();

                if (likedComments != null)
                {
                    _repo.RemoveRange(likedComments);
                }

                _repo.Remove(comment);
                _repo.LogInformation($"User {comment.Author}'s comment was deleted");
                await _repo.SaveChangesAsync();
            }
            else
            {
                return NotFound();
            }

            if (model.ReturnUrl.Contains("Accounts"))
            {
                return RedirectToAction("Index", "Accounts", 
                    new { userName = post.User.UserName, page = model.Page });
            }

            return RedirectToAction(model.ReturnUrl.Contains("Feed")
                ? "Feed" : "Index", "Home", new { page = model.Page });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string postCommentId, string calledFromAction, int page)
        {
            if (!string.IsNullOrEmpty(postCommentId))
            {
                PostComment comment = await _repo.FirstOrDefaultAsync(
                    _repo.GetAllComments(), c => c.Id == postCommentId);

                if (comment == null)
                {
                    return NotFound();
                }

                var model = new EditPostCommentViewModel()
                {
                    CommentId = postCommentId,
                    Content = comment.Content,
                    Author = comment.Author,
                    CommentedPostUser = comment.Post.User.UserName,
                    CommentPictures = comment.CommentPictures
                        .OrderByDescending(cp => cp.UploadedTime)
                        .AsEnumerable(),
                    ReturnUrl = calledFromAction,
                    Page = page
                };

                return View(model);
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditPostCommentViewModel model)
        {
            PostComment comment = await _repo.FirstOrDefaultAsync(
                _repo.GetAllComments(), c => c.Id == model.CommentId);

            if (comment == null)
            {
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

                    _repo.GetAllComments().Update(comment);
                    _repo.LogInformation($"User {model.Author}'s comment was edited");
                    await _repo.SaveChangesAsync();

                    if (model.ReturnUrl.Contains("Accounts"))
                    {
                        return RedirectToAction("Index", "Accounts", new {
                            userName = model.CommentedPostUser, page = model.Page });
                    }

                    return RedirectToAction(model.ReturnUrl.Contains("Feed")
                        ? "Feed" : "Index", "Home", new { page = model.Page });
                }
                else
                {
                    ModelState.AddModelError("", "The length of a comment must be between 1 and 200 symbols");
                }
            }

            model.CommentPictures = comment.CommentPictures.OrderByDescending(cp => cp.UploadedTime);
            return View(model);
        }

        public async Task<IActionResult> Like(LikeViewModel model)
        {
            User user = await _repo.FindByIdAsync(model.UserId);

            if (user != null)
            {
                PostComment comment = await _repo.FirstOrDefaultAsync(
                    _repo.GetAllComments(), c => c.Id == model.Id);

                if (comment != null)
                {
                    LikedComment commentToCheck = user.LikedComments.FirstOrDefault(c =>
                        c.UserWhoLikedId == model.UserId && c.CommentLikedId == model.Id);

                    LikeOrDislikeComment(comment, commentToCheck, user);
                    await _repo.SaveChangesAsync();
                }
                else
                {
                    return NotFound();
                }

                if (model.ReturnAction.Contains("Accounts"))
                {
                    return RedirectToAction("Index", "Accounts",
                        new { userName = comment.Post.User.UserName, page = model.Page });
                }

                return RedirectToAction(model.ReturnAction.Contains("Feed")
                    ? "Feed" : "Index", "Home", new { page = model.Page });
            }
            else
            {
                return BadRequest();
            }
        }

        private void LikeOrDislikeComment(PostComment commentToLike, LikedComment commentToCheck, User user)
        {
            if (commentToCheck != null)
            {
                user.LikedComments.Remove(commentToCheck);
                commentToLike.Likes--;
                _repo.LogInformation($"User {user.UserName} removed a like from a comment");
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
                _repo.LogInformation($"User {user.UserName} liked a comment");
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

                    var commentPicture = new CommentPicture()
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
                    return;
                }

                if (commentPictures != null)
                {
                    if (commentPictures.Count() + appendedCommentPictures.Count() > 5)
                    {
                        ModelState.AddModelError("", errorMessage);
                    }
                }
            }
        }
    }
}
