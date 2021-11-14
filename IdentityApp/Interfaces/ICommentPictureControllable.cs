using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using IdentityApp.Models;

namespace IdentityApp.Interfaces
{
    public interface ICommentPictureControllable
    {
        Task<CommentPicture> FirstOrDefaultAsync(Expression<Func<CommentPicture, bool>> predicate);
        Task<int> SaveChangesAsync();
        EntityEntry<CommentPicture> Remove(CommentPicture commentPicture);
        void LogInformation(string message);
    }
}
