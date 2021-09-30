using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using IdentityApp.Models;

namespace IdentityApp.Controllers
{
    public class PostPictureController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostPictureController (ApplicationDbContext context)
        {
            _context = context;
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
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Edit", "Post",
                new { postId = postId });
        }
    }
}
