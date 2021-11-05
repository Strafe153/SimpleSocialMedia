using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using IdentityApp.Interfaces;
using IdentityApp.Models;

namespace IdentityApp.ControllerRepositories
{
    public class PostCommentRepository : IPostCommentControllable
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public PostCommentRepository(UserManager<User> userManager, ApplicationDbContext context,
            ILogger<PostCommentRepository> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public async Task<Post> FirstOrDefaultAsync(string postId)
        {
            return await _context.Posts.FirstOrDefaultAsync(post => post.Id == postId);
        }

        public void LogError(string message)
        {
            _logger.LogError(message);
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        public void LogWarning(string message)
        {
            _logger.LogWarning(message);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
