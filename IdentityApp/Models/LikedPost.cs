namespace IdentityApp.Models
{
    public class LikedPost
    {
        public string UserId { get; set; }
        public virtual User User { get; set; }

        public string PostId { get; set; }
        public virtual Post Post { get; set; }
    }
}
