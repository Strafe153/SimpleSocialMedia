using Microsoft.AspNetCore.Identity;
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
        public virtual List<Post> Posts { get; set; } 
        public virtual List<LikedPosts> LikedPosts { get; set; } 

        public User()
        {
            Posts = new List<Post>();
            LikedPosts = new List<LikedPosts>();
        }
    }
}