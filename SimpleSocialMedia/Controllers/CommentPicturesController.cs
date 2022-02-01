using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SimpleSocialMedia.Models;
using SimpleSocialMedia.Repositories.Interfaces;

namespace SimpleSocialMedia.Controllers
{
    public class CommentPicturesController : Controller
    {
        private readonly IPicturesControllable<CommentPicture> _repo;

        public CommentPicturesController(IPicturesControllable<CommentPicture> repo)
        {
            _repo = repo;
        }

        public async Task<IActionResult> Delete(string[] commentPictureIds, string commentId)
        {
            foreach (string pictureId in commentPictureIds)
            {
                CommentPicture picture = await _repo.SingleOrDefaultAsync(p => p.Id == pictureId);

                if (picture != null)
                {
                    _repo.Remove(picture);
                    _repo.LogInformation($"User {User.Identity.Name} deleted a comment picture");
                }
            }

            await _repo.SaveChangesAsync();
            return RedirectToAction("Edit", "PostComment", new { postCommentId = commentId });
        }
    }
}
