namespace SimpleSocialMedia.Models
{
    public class Following
    {
        public string FollowedUserId { get; set; }
        public virtual User FollowedUser { get; set; }

        public string ReaderId { get; set; }
        public virtual User Reader { get; set; }
    }
}
