using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using SimpleSocialMedia.Data;
using SimpleSocialMedia.Models;
using SimpleSocialMedia.Repositories.Interfaces;

namespace SimpleSocialMedia.Repositories.Implementations
{
    public class PostCommentsRepository : IPostCommentsControllable
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public PostCommentsRepository(UserManager<User> userManager,
            ApplicationDbContext context, ILogger<PostCommentsRepository> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public async Task<User> FindByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<T> FirstOrDefaultAsync<T>(IQueryable<T> collection, Expression<Func<T, bool>> predicate)
        {
            return await collection.FirstOrDefaultAsync(predicate);
        }

        public DbSet<PostComment> GetAllComments()
        {
            return _context.PostComments;
        }

        public DbSet<LikedComment> GetAllLikedComments()
        {
            return _context.LikedComments;
        }

        public DbSet<Post> GetAllPosts()
        {
            return _context.Posts;
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        public EntityEntry<PostComment> Remove(PostComment comment)
        {
            return _context.Remove(comment);
        }

        public void RemoveRange(IEnumerable<LikedComment> likedComments)
        {
            _context.RemoveRange(likedComments);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
