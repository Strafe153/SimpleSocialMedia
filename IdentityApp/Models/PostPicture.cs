using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityApp.Models
{
    public class PostPicture
    {
        public string Id { get; set; }
        public byte[] PictureData { get; set; }

        public string PostId { get; set; }
        public virtual Post Post { get; set; }
    }
}
