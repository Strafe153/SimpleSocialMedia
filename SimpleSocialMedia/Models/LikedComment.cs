namespace SimpleSocialMedia.Models
{
    public class LikedComment
    {
        public string UserWhoLikedId { get; set; }
        public virtual User UserWhoLiked { get; set; }

        public string CommentLikedId { get; set; }
        public virtual PostComment CommentLiked { get; set; }
    }
}
