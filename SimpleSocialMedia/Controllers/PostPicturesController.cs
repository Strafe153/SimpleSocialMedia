using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SimpleSocialMedia.Models;
using SimpleSocialMedia.Repositories.Interfaces;

namespace SimpleSocialMedia.Controllers
{
    public class PostPicturesController : Controller
    {
        private readonly IPicturesControllable<PostPicture> _repo;

        public PostPicturesController(IPicturesControllable<PostPicture> repo)
        {
            _repo = repo;
        }

        public async Task<IActionResult> Delete(string[] postPictureIds, string postId)
        {
            foreach (string postPicId in postPictureIds)
            {
                PostPicture postPicture = await _repo.SingleOrDefaultAsync(p => p.Id == postPicId);

                if (postPicture != null)
                {
                    _repo.Remove(postPicture);
                    _repo.LogInformation($"User {User.Identity.Name} deleted a post picture");
                }
            }

            await _repo.SaveChangesAsync();
            return RedirectToAction("Edit", "Post", new { postId = postId });
        }
    }
}
