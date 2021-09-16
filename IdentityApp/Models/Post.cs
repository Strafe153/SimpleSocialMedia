using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityApp.Models
{
    public class Post
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public DateTime PostedTime { get; set; }
        public int Likes { get; set; }
        public bool IsEdited { get; set; }
        public virtual List<PostPicture> PostPictures { get; set; } = new List<PostPicture>();
        public virtual List<LikedPosts> LikedPosts { get; set; } = new List<LikedPosts>();

        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}
