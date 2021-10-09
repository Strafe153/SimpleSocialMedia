using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using IdentityApp.Models;

namespace IdentityApp.Controllers
{
    public class PostPictureController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public PostPictureController (ApplicationDbContext context,
            ILogger<PostPictureController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Delete(string[] postPictureIds, 
            string postId)
        {
            foreach (string postPicId in postPictureIds)
            {
                PostPicture postPicture = await _context.PostPictures
                    .FirstOrDefaultAsync(picture => picture.Id == postPicId);

                if (postPicture != null)
                {
                    _context.PostPictures.Remove(postPicture);
                    _logger.LogInformation("User deleted a post picture");
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Edit", "Post",
                new { postId = postId });
        }
    }
}
