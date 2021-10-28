using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using IdentityApp.Models;
using IdentityApp.Interfaces;

namespace IdentityApp.ControllerRepositories
{
    public class PostControllerRepository : IPostControllable
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public PostControllerRepository(UserManager<User> userManager,
            ApplicationDbContext context, ILogger<PostControllerRepository> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public async Task<User> FindByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<T> FirstOrDefaultAsync<T>(IQueryable<T> collection, 
            Expression<Func<T, bool>> predicate)
        {
            return await collection.FirstOrDefaultAsync(predicate);
        }

        public IQueryable<LikedPost> GetAllLikedPosts()
        {
            return _context.LikedPosts;
        }

        public IQueryable<Post> GetAllPosts()
        {
            return _context.Posts;
        }

        public IQueryable<User> GetAllUsers()
        {
            return _context.Users;
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
            _logger.LogInformation(message);
        }

        public EntityEntry<Post> Remove(Post post)
        {
            return _context.Remove(post);
        }

        public void RemoveRange(IEnumerable<LikedPost> likedPosts)
        {
            _context.LikedPosts.RemoveRange(likedPosts);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public EntityEntry<Post> Update(Post post)
        {
            return _context.Update(post);
        }

        public async Task<IdentityResult> UpdateAsync(User user)
        {
            return await _userManager.UpdateAsync(user);
        }
    }
}
