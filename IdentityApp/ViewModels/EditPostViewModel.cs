using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using IdentityApp.Models;

namespace IdentityApp.ViewModels
{
    public class EditPostViewModel
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime PostedTime { get; set; }
        public virtual List<PostPicture> PostPictures { get; set; } 
        public IFormFileCollection AppendedPostPictures { get; set; }
        public string ReturnUrl { get; set; }

        public EditPostViewModel()
        {
            PostPictures = new List<PostPicture>();
        }
    }
}
