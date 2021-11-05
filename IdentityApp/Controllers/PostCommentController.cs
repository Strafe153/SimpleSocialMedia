using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using IdentityApp.Models;
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

        public async Task<IActionResult> Create(string postId, string commentAuthorName, string postContent)
        {
            if (!string.IsNullOrEmpty(postId))
            {
                Post post = await _repository.FirstOrDefaultAsync(postId);

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

                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            _repository.LogError("Post not found");
            return NotFound();
        }
    }
}
