using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using IdentityApp.Models;

namespace IdentityApp.Interfaces
{
    public interface IAccountControllable
    {
        Task<User> FindByNameAsync(string userName);
        Task<User> FindByIdAsync(string id);
        Task<User> FindByEmailAsync(string email);
        Task<IList<string>> GetRolesAsync(User user);
        Task<User> FirstOrDefaultAsync(Expression<Func<User, bool>> predicate);
        Task<IdentityResult> CreateAsync(User user, string password);
        Task<IdentityResult> UpdateAsync(User user);
        Task<IdentityResult> AddToRoleAsync(User user, string roleName);
        Task SignInAsync(User user, bool isPersistent);
        Task<SignInResult> PasswordSignInAsync(string userName,
            string password, bool isPersistent, bool lockOutOnFailure);
        Task SignOutAsync();
        Task<int> SaveChangesAsync();
        void LogInformation(string message);
        void LogWarning(string message);
        void LogError(string message);
        IQueryable<User> GetAllUsers();
        string GetWebRootPath();
    }
}
