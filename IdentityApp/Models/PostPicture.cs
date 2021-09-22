using System;

namespace IdentityApp.Models
{
    public class PostPicture
    {
        public string Id { get; set; }
        public byte[] PictureData { get; set; }
        public DateTime UploadedTime { get; set; }

        public string PostId { get; set; }
        public virtual Post Post { get; set; }
    }
}
