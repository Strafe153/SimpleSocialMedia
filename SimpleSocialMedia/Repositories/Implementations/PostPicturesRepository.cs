using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using SimpleSocialMedia.Data;
using SimpleSocialMedia.Models;
using SimpleSocialMedia.Repositories.Interfaces;

namespace SimpleSocialMedia.Repositories.Implementations
{
    public class PostPicturesRepository : IPicturesControllable<PostPicture>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public PostPicturesRepository(ApplicationDbContext context, 
            ILogger<PostPicturesRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PostPicture> SingleOrDefaultAsync(Expression<Func<PostPicture, bool>> predicate)
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
