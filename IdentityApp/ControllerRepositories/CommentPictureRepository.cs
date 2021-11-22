using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using IdentityApp.Models;
using IdentityApp.Interfaces;

namespace IdentityApp.ControllerRepositories
{
    public class CommentPictureRepository : IPictureControllable<CommentPicture>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public CommentPictureRepository(ApplicationDbContext context, ILogger<CommentPictureRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CommentPicture> FirstOrDefaultAsync(Expression<Func<CommentPicture, bool>> predicate)
        {
            return await _context.CommentPictures.FirstOrDefaultAsync(predicate);
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
