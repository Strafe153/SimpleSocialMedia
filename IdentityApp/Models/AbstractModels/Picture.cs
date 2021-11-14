using System;

namespace IdentityApp.Models.AbstractModels
{
    public abstract class Picture
    {
        public string Id { get; set; }
        public byte[] PictureData { get; set; }
        public DateTime UploadedTime { get; set; }
    }
}
