using IdentityApp.Models;

namespace IdentityApp.ViewModels
{
    public class EditPostViewModel
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string PostedTime { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
    }
}
