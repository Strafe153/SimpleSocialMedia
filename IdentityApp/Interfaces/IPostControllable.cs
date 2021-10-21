using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityApp.Models;

namespace IdentityApp.Interfaces
{
    public interface IPostControllable
    {
        Task<T> FirstOrDefaultAsync<T>(IQueryable<T> collection, Expression<Func<T, bool>> predicate);
        Task<User> FindByIdAsync(string id);
        Task<IdentityResult> UpdateAsync(User user);
        Task<int> SaveChangesAsync();
        EntityEntry<Post> Update(Post post);
        EntityEntry<Post> Remove(Post post);
        IQueryable<User> GetAllUsers();
        IQueryable<Post> GetAllPosts();
        IQueryable<LikedPost> GetAllLikedPosts();
        void RemoveRange(IEnumerable<LikedPost> likedPosts);
        void LogError(string message);
        void LogWarning(string message);
        void LogInformation(string message);
    }
}
