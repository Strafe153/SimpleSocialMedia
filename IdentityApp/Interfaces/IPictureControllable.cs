using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using IdentityApp.Models;
using IdentityApp.Models.AbstractModels;

namespace IdentityApp.Interfaces
{
    public interface IPictureControllable<T> where T: Picture
    {
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<int> SaveChangesAsync();
        EntityEntry<T> Remove(T commentPicture);
        void LogInformation(string message);
    }
}
