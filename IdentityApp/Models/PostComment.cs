using System;

namespace IdentityApp.Models
{
    public class PostComment
    {
        public string Id { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }
        public DateTime CommentedTime { get; set; }
        public bool IsEdited { get; set; }

        public string PostId { get; set; }
        public virtual Post Post { get; set; }
    }
}
