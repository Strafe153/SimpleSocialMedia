using System.Collections.Generic;

namespace SimpleSocialMedia.ViewModels
{
    public class HomepageViewModel : FeedPageViewModel
    {
        public IList<string> AuthenticatedUserRoles { get; set; }
    }
}
