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
    public class PostsRepository : IPostsControllable
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public PostsRepository(UserManager<User> userManager,
            ApplicationDbContext context, ILogger<PostsRepository> logger)
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

        public IQueryable<PostComment> GetAllPostComments()
        {
            return _context.PostComments;
        }

        public IQueryable<Post> GetAllPosts()
        {
            return _context.Posts;
        }

        public IQueryable<User> GetAllUsers()
        {
            return _context.Users;
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        public EntityEntry<Post> Remove(Post post)
        {
            return _context.Remove(post);
        }

        public void RemoveLikedPostsRange(IEnumerable<LikedPost> likedPosts)
        {
            _context.LikedPosts.RemoveRange(likedPosts);
        }

        public void RemoveCommentsRange(IEnumerable<PostComment> comments)
        {
            _context.PostComments.RemoveRange(comments);
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

        public IQueryable<LikedComment> GetAllLikedPostComments()
        {
            return _context.LikedComments;
        }

        public void RemoveLikedCommentsRange(IEnumerable<LikedComment> comments)
        {
            _context.LikedComments.RemoveRange(comments);
        }
    }
}
