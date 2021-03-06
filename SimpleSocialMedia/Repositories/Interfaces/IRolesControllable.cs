using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Collections.Generic;
using SimpleSocialMedia.Models;

namespace SimpleSocialMedia.Repositories.Interfaces
{
    public interface IRolesControllable
    {
        Task<User> FindUserByIdAsync(string userId);
        Task<IdentityRole> FindRoleByIdAsync(string roleId);
        Task<List<IdentityRole>> GetAllRolesAsync();
        Task<IdentityResult> CreateAsync(IdentityRole roleName);
        Task<IdentityResult> DeleteAsync(IdentityRole role);
        Task<IdentityResult> AddToRolesAsync(User user, IEnumerable<string> roles);
        Task<IdentityResult> RemoveFromRolesAsync(User user, IEnumerable<string> roles);
        Task<IList<string>> GetRolesAsync(User user);
        Task SignInAsync(User user, bool isPersistent);
        Task SignOutAsync();
        void LogInformation(string message);
    }
}
