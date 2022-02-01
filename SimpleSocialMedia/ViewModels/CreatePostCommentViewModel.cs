using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SimpleSocialMedia.ViewModels
{
    public class CreatePostCommentViewModel
    {
        public string PostId { get; set; }
        public string CommentAuthorName { get; set; }

        [MaxLength(200)]
        public string CommentContent { get; set; }

        public IFormFileCollection CommentPictures { get; set; }
        public string ReturnUrl { get; set; }
        public int Page { get; set; } = 1;
    }
}
