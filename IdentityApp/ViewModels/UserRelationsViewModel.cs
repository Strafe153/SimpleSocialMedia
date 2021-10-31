using System.Collections.Generic;
using IdentityApp.Models;

namespace IdentityApp.ViewModels
{
    public class UserRelationsViewModel
    {
        public string UserName { get; set; }
        public IEnumerable<User> RelatedUsers { get; set; }
    }
}
