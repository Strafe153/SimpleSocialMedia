using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleSocialMedia.Models;

namespace SimpleSocialMedia.Repositories.Interfaces
{
    public interface IPostsControllable
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
        IQueryable<PostComment> GetAllPostComments();
        IQueryable<LikedComment> GetAllLikedPostComments();
        void RemoveLikedPostsRange(IEnumerable<LikedPost> likedPosts);
        void RemoveCommentsRange(IEnumerable<PostComment> comments);
        void RemoveLikedCommentsRange(IEnumerable<LikedComment> comments);
        void LogInformation(string message);
    }
}
