using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using SimpleSocialMedia.Models;

namespace SimpleSocialMedia.Repositories.Interfaces
{
    public interface IUsersControllable
    {
        Task<User> FindByIdAsync(string id);
        Task<User> FindByNameAsync(string userName);
        Task<IdentityResult> DeleteAsync(User user);
        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);
        Task<int> SaveChangesAsync();
        Task SignOutAsync();
        IQueryable<User> GetAllUsers();
        DbSet<LikedPost> GetAllLikedPosts();
        DbSet<Following> GetAllFollowings();
        void RemoveLikedPostsRange(IEnumerable<LikedPost> postsToRemove);
        void LogInformation(string message);
    }
}
