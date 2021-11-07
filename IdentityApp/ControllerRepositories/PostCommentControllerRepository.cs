using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using IdentityApp.Interfaces;
using IdentityApp.Models;

namespace IdentityApp.ControllerRepositories
{
    public class PostCommentControllerRepository : IPostCommentControllable
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public PostCommentControllerRepository(ApplicationDbContext context, 
            ILogger<PostCommentControllerRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<T> FirstOrDefaultAsync<T>(IQueryable<T> collection, Expression<Func<T, bool>> predicate)
        {
            return await collection.FirstOrDefaultAsync(predicate);
        }

        public DbSet<PostComment> GetAllComments()
        {
            return _context.PostComments;
        }

        public DbSet<Post> GetAllPosts()
        {
            return _context.Posts;
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
