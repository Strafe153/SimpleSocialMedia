using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Collections.Generic;

namespace IdentityApp.Models
{
    public class User : IdentityUser
    {
        public int? Year { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Company { get; set; }
        public string Status { get; set; }
        public byte[] ProfilePicture { get; set; }
        public virtual List<Post> Posts { get; set; } = new List<Post>();
        public virtual List<LikedPosts> LikedPosts { get; set; } = new List<LikedPosts>();
    }
}