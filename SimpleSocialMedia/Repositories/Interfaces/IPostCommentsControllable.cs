using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using SimpleSocialMedia.Models;

namespace SimpleSocialMedia.Repositories.Interfaces
{
    public interface IPostCommentsControllable
    {
        Task<T> FirstOrDefaultAsync<T>(IQueryable<T> collection, Expression<Func<T, bool>> predicate);
        Task<User> FindByIdAsync(string id);
        Task<int> SaveChangesAsync();
        DbSet<Post> GetAllPosts();
        DbSet<PostComment> GetAllComments();
        DbSet<LikedComment> GetAllLikedComments();
        EntityEntry<PostComment> Remove(PostComment comment);
        void RemoveRange(IEnumerable<LikedComment> likedComments);
        void LogInformation(string message);
    }
}
