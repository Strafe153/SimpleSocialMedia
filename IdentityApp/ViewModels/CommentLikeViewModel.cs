namespace IdentityApp.ViewModels
{
    public class CommentLikeViewModel
    {
        public string CommentId { get; set; }
        public string UserId { get; set; }
        public string ReturnAction { get; set; }
        public string LikedCommentUserName { get; set; }
        public int Page { get; set; }
    }
}
