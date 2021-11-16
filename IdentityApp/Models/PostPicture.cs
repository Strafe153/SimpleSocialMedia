using IdentityApp.Models.AbstractModels;

namespace IdentityApp.Models
{
    public class PostPicture : Picture
    {
        public string PostId { get; set; }
        public virtual Post Post { get; set; }
    }
}
