namespace IdentityApp.Models
{
    public class LikedPost
    {
        public string UserWhoLikedId { get; set; }
        public virtual User UserWhoLiked { get; set; }

        public string PostLikedId { get; set; }
        public virtual Post PostLiked { get; set; }
    }
}
