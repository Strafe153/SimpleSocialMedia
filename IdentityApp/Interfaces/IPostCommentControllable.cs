using System.Threading.Tasks;
using IdentityApp.Models;

namespace IdentityApp.Interfaces
{
    public interface IPostCommentControllable
    {
        Task<Post> FirstOrDefaultAsync(string commentId);
        void LogError(string message);
        void LogWarning(string message);
        void LogInformation(string message);
        Task<int> SaveChangesAsync();
    }
}
