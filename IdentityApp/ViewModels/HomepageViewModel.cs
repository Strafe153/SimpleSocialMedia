using System.Collections.Generic;
using IdentityApp.Models;

namespace IdentityApp.ViewModels
{
    public class HomepageViewModel : FeedPageViewModel
    {
        /*public User AuthenticatedUser { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public IEnumerable<Post> Posts { get; set; }*/
        public IList<string> AuthenticatedUserRoles { get; set; }
    }
}
