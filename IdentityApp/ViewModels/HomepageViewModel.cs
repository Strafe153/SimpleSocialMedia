using System.Collections.Generic;
using IdentityApp.Models;

namespace IdentityApp.ViewModels
{
    public class HomepageViewModel : FeedPageViewModel
    {
        public IList<string> AuthenticatedUserRoles { get; set; }
    }
}
