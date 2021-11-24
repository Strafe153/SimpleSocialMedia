using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace IdentityApp.ViewModels
{
    public class ChangeRoleViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public IList<IdentityRole> AllRoles { get; set; }
        public IList<string> UserRoles { get; set; }
        public IList<string> NewRoles { get; set; }
        public string ReturnUrl { get; set; }

        public ChangeRoleViewModel()
        {
            AllRoles = new List<IdentityRole>();
            UserRoles = new List<string>();
        }
    }
}
