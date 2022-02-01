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
    public class CommentPicturesRepository : IPicturesControllable<CommentPicture>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public CommentPicturesRepository(ApplicationDbContext context, 
            ILogger<CommentPicturesRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CommentPicture> SingleOrDefaultAsync(
            Expression<Func<CommentPicture, bool>> predicate)
        {
            return await _context.CommentPictures.SingleOrDefaultAsync(predicate);
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        public EntityEntry<CommentPicture> Remove(CommentPicture commentPicture)
        {
            return _context.CommentPictures.Remove(commentPicture);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
