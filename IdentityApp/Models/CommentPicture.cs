using IdentityApp.Models.AbstractModels;

namespace IdentityApp.Models
{
    public class CommentPicture : Picture
    {
        public string CommentId { get; set; }
        public virtual PostComment Comment { get; set; }
    }
}
