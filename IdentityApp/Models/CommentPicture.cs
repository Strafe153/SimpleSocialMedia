using IdentityApp.Models.AbstractModels;

namespace IdentityApp.Models
{
    public class CommentPicture : Picture
    {
        /*public string Id { get; set; }
        public byte[] PictureData { get; set; }
        public DateTime UploadedTime { get; set; }*/

        public string CommentId { get; set; }
        public virtual PostComment Comment { get; set; }
    }
}
