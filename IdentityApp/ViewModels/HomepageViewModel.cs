using System.Collections.Generic;
using IdentityApp.Models;

namespace IdentityApp.ViewModels
{
    public class HomepageViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public IEnumerable<Post> Posts { get; set; }
        public PageViewModel PageViewModel { get; set; }
    }
}
