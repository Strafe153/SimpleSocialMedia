using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using IdentityApp.Models;

namespace IdentityApp.Interfaces
{
    public interface IHomeControllable
    {
        Task<User> FindByNameAsync(string userName);
        Task<IList<string>> GetRolesAsync(User user);
        void LogInformation(string message);
        IQueryable<User> GetAllUsers();
        IQueryable<Post> GetAllPosts();
    }
}
