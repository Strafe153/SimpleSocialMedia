using System.ComponentModel.DataAnnotations;

namespace IdentityApp.ViewModels
{
    public class CreatePostCommentViewModel
    {
        public string PostId { get; set; }
        public string CommentAuthorName { get; set; }

        [MaxLength(200)]
        public string PostContent { get; set; }

        public string ReturnUrl { get; set; }
        public int Page { get; set; }
    }
}
