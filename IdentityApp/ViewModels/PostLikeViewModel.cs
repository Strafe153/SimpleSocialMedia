using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityApp.ViewModels
{
    public class PostLikeViewModel
    {
        public string PostId { get; set; }
        public string UserId { get; set; }
        public string ReturnAction { get; set; }
        public string LikedPostUserName { get; set; }
    }
}
