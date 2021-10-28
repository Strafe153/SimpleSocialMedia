using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using IdentityApp.Models;
using IdentityApp.Interfaces;
using IdentityApp.ControllerRepositories;

namespace IdentityApp.Controllers
{
    public class PostPictureController : Controller
    {
        private readonly IPostPictureControllable _repository;

        public PostPictureController(IPostPictureControllable repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Delete(string[] postPictureIds, string postId)
        {
            foreach (string postPicId in postPictureIds)
            {
                PostPicture postPicture = await _repository.FirstOrDefaultAsync(
                    picture => picture.Id == postPicId);

                if (postPicture != null)
                {
                    _repository.Remove(postPicture);
                    _repository.LogInformation("User deleted a post picture");
                }
            }

            await _repository.SaveChangesAsync();
            return RedirectToAction("Edit", "Post", new { postId = postId });
        }
    }
}
