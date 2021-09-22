using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Models
{
    public class Post
    {
        public string Id { get; set; }

        // TODO: figure out the reason why these attributes don't work
        [MinLength(1, ErrorMessage = 
            "The length of your post must be at least 1 symbol")]
        [MaxLength(350, ErrorMessage = 
            "The length of your post must be less than 350 symbols")]
        public string Content { get; set; }

        public DateTime PostedTime { get; set; }
        public int Likes { get; set; }
        public bool IsEdited { get; set; }

        // [MaxLength(5)]
        public virtual List<PostPicture> PostPictures { get; set; } 
        public virtual List<LikedPosts> LikedPosts { get; set; } 

        public string UserId { get; set; }
        public virtual User User { get; set; }

        public Post()
        {
            PostPictures = new List<PostPicture>();
            LikedPosts = new List<LikedPosts>();
        }
    }
}
