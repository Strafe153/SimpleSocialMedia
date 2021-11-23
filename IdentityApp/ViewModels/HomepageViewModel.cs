using System.Collections.Generic;

namespace IdentityApp.ViewModels
{
    public class HomepageViewModel : FeedPageViewModel
    {
        public IList<string> AuthenticatedUserRoles { get; set; }
    }
}
