using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
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

        public async Task<IActionResult> Create(string postId, string commentAuthorName, 
            string postContent, int page)
        {
            if (!string.IsNullOrEmpty(postId))
            {
                Post post = await _repository.FirstOrDefaultAsync(
                    _repository.GetAllPosts(), post => post.Id == postId);

                if (post != null)
                {
                    if (post.Content == null)
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
                            Author = commentAuthorName,
                            Content = postContent,
                            CommentedTime = DateTime.Now,
                            IsEdited = false,
                            PostId = post.Id
                        };

                        post.PostComments.Add(postComment);
                        await _repository.SaveChangesAsync();
                        _repository.LogInformation($"User {commentAuthorName} created a comment");

                        return RedirectToAction("Index", "Home", new { page = page });
                    }
                }
            }

            _repository.LogError("Post not found");
            return NotFound();
        }

        public async Task<IActionResult> Delete(string postCommentId, int page)
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

                comment.Post.PostComments.Remove(comment);
                await _repository.SaveChangesAsync();
                _repository.LogInformation($"User {comment.Author}'s post was deleted");

                return RedirectToAction("Index", "Home", new { page = page });
            }

            _repository.LogError("Comment id is not passed");
            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string postCommentId, string returnUrl, int page)
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

                EditPostCommentViewModel model = new EditPostCommentViewModel()
                {
                    CommentId = postCommentId,
                    Content = comment.Content,
                    Author = comment.Author,
                    Page = page,
                    ReturnUrl = returnUrl
                };

                return View(model);
            }

            _repository.LogError("Comment id is not passed");
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditPostCommentViewModel model)
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

            return RedirectToAction("Index", "Home", new { page = model.Page });
        }
    }
}
