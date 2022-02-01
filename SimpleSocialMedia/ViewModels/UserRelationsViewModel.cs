using System.Collections.Generic;
using SimpleSocialMedia.Models;

namespace SimpleSocialMedia.ViewModels
{
    public class UserRelationsViewModel
    {
        public string UserName { get; set; }
        public IEnumerable<User> RelatedUsers { get; set; }
    }
}
