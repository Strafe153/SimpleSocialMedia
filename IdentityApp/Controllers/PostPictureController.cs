using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
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

        [HttpGet]
        public async Task<IActionResult> Delete(string postPictureId, string postId)
        {
            PostPicture postPicture = await _context.PostPictures.FirstOrDefaultAsync(p => p.Id == postPictureId);

            if (postPicture != null)
            {
                _context.PostPictures.Remove(postPicture);
                await _context.SaveChangesAsync();

                return RedirectToAction("Edit", "Post", new { postId = postId });
            }

            return NotFound();
        }
    }
}
