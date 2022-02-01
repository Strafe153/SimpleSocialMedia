using System;
using System.Collections.Generic;

namespace SimpleSocialMedia.Models
{
    public class PostComment
    {
        public string Id { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }
        public DateTime CommentedTime { get; set; }
        public int Likes { get; set; }
        public bool IsEdited { get; set; }
        public virtual List<LikedComment> LikedComments { get; set; }
        public virtual List<CommentPicture> CommentPictures { get; set; }

        public string PostId { get; set; }
        public virtual Post Post { get; set; }

        public PostComment()
        {
            LikedComments = new List<LikedComment>();
            CommentPictures = new List<CommentPicture>();
        }
    }
}
