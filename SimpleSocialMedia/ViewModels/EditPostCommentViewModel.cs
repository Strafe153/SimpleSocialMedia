using System.ComponentModel.DataAnnotations;
using SimpleSocialMedia.Models;

using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace SimpleSocialMedia.ViewModels
{
    public class EditPostCommentViewModel
    {
        public string CommentId { get; set; }

        [MaxLength(200)]
        public string Content { get; set; }

        public string Author { get; set; }
        public string CommentedPostUser { get; set; }
        public virtual IEnumerable<CommentPicture> CommentPictures { get; set; }
        public IFormFileCollection AppendedCommentPictures { get; set; }
        public string ReturnUrl { get; set; }
        public int Page { get; set; } = 1;

        public EditPostCommentViewModel()
        {
            CommentPictures = new List<CommentPicture>();
        }
    }
}
