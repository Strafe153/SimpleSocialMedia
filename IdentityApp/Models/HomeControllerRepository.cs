using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using IdentityApp.Interfaces;
using IdentityApp.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace IdentityApp.Models
{
    public class HomeControllerRepository : IHomeControllable
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public HomeControllerRepository(UserManager<User> userManager,
            ApplicationDbContext context, ILogger<HomeControllerRepository> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public async Task<User> FindByNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public IQueryable<Post> GetAllPosts()
        {
            return _context.Posts;
        }

        public IQueryable<User> GetAllUsers()
        {
            return _context.Users;
        }

        public async Task<IList<string>> GetRolesAsync(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }
    }
}
