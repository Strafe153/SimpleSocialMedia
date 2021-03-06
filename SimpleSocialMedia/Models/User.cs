using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace SimpleSocialMedia.Models
{
    public class User : IdentityUser
    {
        public int? Year { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Company { get; set; }
        public string Status { get; set; }
        public byte[] ProfilePicture { get; set; }
        public int ReadersCount { get; set; }
        public int FollowsCount { get; set; }
        public virtual List<Post> Posts { get; set; }
        public virtual List<LikedPost> LikedPosts { get; set; }
        public virtual List<PostComment> Comments { get; set; }
        public virtual List<LikedComment> LikedComments { get; set; }
        public virtual List<Following> FollowingUsers { get; set; }
        public virtual List<Following> Followers { get; set; }

        public User()
        {
            Posts = new List<Post>();
            LikedPosts = new List<LikedPost>();
            Comments = new List<PostComment>();
            LikedComments = new List<LikedComment>();
            FollowingUsers = new List<Following>();
            Followers = new List<Following>();
        }
    }
}