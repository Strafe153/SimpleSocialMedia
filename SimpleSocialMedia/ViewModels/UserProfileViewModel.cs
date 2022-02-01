using System.Collections.Generic;
using SimpleSocialMedia.Models;

namespace SimpleSocialMedia.ViewModels
{
    public class UserProfileViewModel
    {
        public User User { get; set; }
        public IEnumerable<Post> Posts { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public User AuthenticatedUser { get; set; }
        public IList<string> AuthenticatedUserRoles { get; set; }
    }
}
