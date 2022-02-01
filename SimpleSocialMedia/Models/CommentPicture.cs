using SimpleSocialMedia.Models.Abstract;

namespace SimpleSocialMedia.Models
{
    public class CommentPicture : Picture
    {
        public string CommentId { get; set; }
        public virtual PostComment Comment { get; set; }
    }
}
