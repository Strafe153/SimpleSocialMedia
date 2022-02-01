using SimpleSocialMedia.Models.Abstract;

namespace SimpleSocialMedia.Models
{
    public class PostPicture : Picture
    {
        public string PostId { get; set; }
        public virtual Post Post { get; set; }
    }
}
