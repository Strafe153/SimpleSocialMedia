using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using SimpleSocialMedia.Models.Abstract;

namespace SimpleSocialMedia.Repositories.Interfaces
{
    public interface IPicturesControllable<T> where T: Picture
    {
        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<int> SaveChangesAsync();
        EntityEntry<T> Remove(T commentPicture);
        void LogInformation(string message);
    }
}
