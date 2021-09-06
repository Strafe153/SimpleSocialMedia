using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using IdentityApp.Models;

namespace IdentityApp.ViewModels
{
    public class CreatePostViewModel
    {
        public User User { get; set; }
        public Post Post { get; set; }
        public IFormFileCollection PostPictures { get; set; }
    }
}
