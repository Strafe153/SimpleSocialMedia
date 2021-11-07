using System.ComponentModel.DataAnnotations;

namespace IdentityApp.ViewModels
{
    public class ManagePostCommentViewModel
    {
        public string CommentId { get; set; }
        [MaxLength(200)]
        public string Content { get; set; }
        public string Author { get; set; }
        public string CommentedPostUser { get; set; }
        public int Page { get; set; }
        public string ReturnUrl { get; set; }
    }
}
