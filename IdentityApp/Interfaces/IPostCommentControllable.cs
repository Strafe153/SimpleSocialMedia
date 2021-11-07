using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using IdentityApp.Models;

namespace IdentityApp.Interfaces
{
    public interface IPostCommentControllable
    {
        Task<T> FirstOrDefaultAsync<T>(IQueryable<T> collection, Expression<Func<T, bool>> predicate);
        Task<int> SaveChangesAsync();
        DbSet<Post> GetAllPosts();
        DbSet<PostComment> GetAllComments();
        void LogError(string message);
        void LogWarning(string message);
        void LogInformation(string message);
    }
}
