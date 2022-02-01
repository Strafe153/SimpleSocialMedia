using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using SimpleSocialMedia.Models;

namespace SimpleSocialMedia.Repositories.Interfaces
{
    public interface IHomeControllable
    {
        Task<User> FindByNameAsync(string userName);
        Task<IList<string>> GetRolesAsync(User user);
        IQueryable<User> GetAllUsers();
        IQueryable<Post> GetAllPosts();
        IQueryable<Following> GetAllFollowings();
        void LogInformation(string message);
    }
}
