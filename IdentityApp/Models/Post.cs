using System;

namespace IdentityApp.Models
{
    public class Post
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string PostedTime { get; set; }
        public int Likes { get; set; }
        public bool IsLiked { get; set; }
        public bool IsEdited { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
    }
}
