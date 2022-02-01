using System.Collections.Generic;
using SimpleSocialMedia.Models;

namespace SimpleSocialMedia.ViewModels
{
    public class FeedPageViewModel
    {
        public User AuthenticatedUser { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public IEnumerable<Post> Posts { get; set; }
    }
}
