using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using IdentityApp.Interfaces;

namespace IdentityApp.Models
{
    public class UsersControllerRepository : IUsersControllable
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger _logger;

        public UsersControllerRepository(UserManager<User> userManager, ApplicationDbContext context,
            SignInManager<User> signInManager, ILogger<UsersControllerRepository> logger)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        }

        public async Task<IdentityResult> DeleteAsync(User user)
        {
            return await _userManager.DeleteAsync(user);
        }

        public async Task<User> FindByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public DbSet<Following> GetAllFollowings()
        {
            return _context.Followings;
        }

        public DbSet<LikedPost> GetAllLikedPosts()
        {
            return _context.LikedPosts;
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
            _logger.LogWarning(message);
        }

        public void RemoveLikedPostsRange(IEnumerable<LikedPost> postsToRemove)
        {
            _context.LikedPosts.RemoveRange(postsToRemove);
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public Task SignOutAsync()
        {
            return _signInManager.SignOutAsync();
        }
    }
}
