using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using IdentityApp.Interfaces;

namespace IdentityApp.Models
{
    public class PostPictureRepository : IPostPictureControllable
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public PostPictureRepository(ApplicationDbContext context, ILogger<PostPictureRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PostPicture> FirstOrDefaultAsync(Expression<Func<PostPicture, bool>> predicate)
        {
            return await _context.PostPictures.FirstOrDefaultAsync(predicate);
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        public EntityEntry<PostPicture> Remove(PostPicture postPicture)
        {
            return _context.PostPictures.Remove(postPicture);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
