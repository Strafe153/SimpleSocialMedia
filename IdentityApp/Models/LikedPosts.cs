using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityApp.Models
{
    public class LikedPosts
    {
        public string UserId { get; set; }
        public virtual User User { get; set; }

        public string PostId { get; set; }
        public virtual Post Post { get; set; }
    }
}
