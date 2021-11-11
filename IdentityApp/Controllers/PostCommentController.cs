using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using IdentityApp.Models;
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
                    _repository.GetAllPosts(), post => post.Id == model.PostId);

                if (post != null)
                {
                    if (string.IsNullOrEmpty(model.PostContent))
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
                            Content = model.PostContent,
                            CommentedTime = DateTime.Now,
                            IsEdited = false,
                            PostId = post.Id
                        };

                        post.PostComments.Add(postComment);
                        await _repository.SaveChangesAsync();
                        _repository.LogInformation($"User {model.CommentAuthorName} created a comment");

                        if (model.ReturnUrl.Contains("Account"))
                        {
                            return RedirectToAction("Index", "Account", new { 
                                userName = post.User.UserName, page = model.PostContent });
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
                _repository.GetAllComments(), comment => comment.Id == model.CommentId);
            Post post = await _repository.FirstOrDefaultAsync(
                _repository.GetAllPosts(), post => post.Id == comment.PostId);

            if (comment != null)
            {
                IEnumerable<LikedComment> likedComments = _repository.GetAllLikedComments()
                    .Where(likedComment => likedComment.CommentLikedId == comment.Id).AsEnumerable();

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
                    _repository.GetAllComments(), comment => comment.Id == postCommentId);

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
                    Page = page,
                    ReturnUrl = calledFromAction
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
                _repository.GetAllComments(), comment => comment.Id == model.CommentId);

            if (comment == null)
            {
                _repository.LogError("Comment not found");
                return NotFound();
            }

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

        public async Task<IActionResult> Like(CommentLikeViewModel model)
        {
            User user = await _repository.FindByIdAsync(model.UserId);

            if (user != null)
            {
                PostComment comment = await _repository.FirstOrDefaultAsync(
                    _repository.GetAllComments(), comment => comment.Id == model.CommentId);

                if (comment != null)
                {
                    LikedComment commentToCheck = user.LikedComments.FirstOrDefault(comment =>
                        comment.UserWhoLikedId == model.UserId && comment.CommentLikedId == model.CommentId);

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
    }
}
