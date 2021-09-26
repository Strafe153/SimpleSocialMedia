namespace IdentityApp.ViewModels
{
    public class PostLikeViewModel
    {
        public string PostId { get; set; }
        public string UserId { get; set; }
        public string ReturnAction { get; set; }
        public string LikedPostUserName { get; set; }
        public int Page { get; set; }
    }
}
