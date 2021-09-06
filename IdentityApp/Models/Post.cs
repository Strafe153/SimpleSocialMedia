using System;
using System.Collections.Generic;

namespace IdentityApp.Models
{
    public class Post
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public DateTime PostedTime { get; set; }
        public int Likes { get; set; }
        public bool IsLiked { get; set; }
        public bool IsEdited { get; set; }
        public virtual List<PostPicture> PostPictures { get; set; } = new List<PostPicture>();

        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}
