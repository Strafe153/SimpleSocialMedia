using System.ComponentModel.DataAnnotations;

namespace IdentityApp.ViewModels
{
    public class EditPostCommentViewModel
    {
        public string CommentId { get; set; }
        [MaxLength(200)]
        public string Content { get; set; }
        public string Author { get; set; }
        public int Page { get; set; }
        public string ReturnUrl { get; set; }
    }
}
