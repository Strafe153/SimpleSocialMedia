using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using IdentityApp.Models;
using IdentityApp.Interfaces;

namespace IdentityApp.Controllers
{
    public class CommentPictureController : Controller
    {
        private readonly IPictureControllable<CommentPicture> _repository;

        public CommentPictureController(IPictureControllable<CommentPicture> repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Delete(string[] commentPictureIds, string commentId)
        {
            foreach (string pictureId in commentPictureIds)
            {
                CommentPicture picture = await _repository.FirstOrDefaultAsync(p => p.Id == pictureId);

                if (picture != null)
                {
                    _repository.Remove(picture);
                    _repository.LogInformation("User deleted a comment picture");
                }
            }

            await _repository.SaveChangesAsync();
            return RedirectToAction("Edit", "PostComment", new { postCommentId = commentId });
        }
    }
}
