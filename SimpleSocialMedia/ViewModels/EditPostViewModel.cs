using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SimpleSocialMedia.Models;

namespace SimpleSocialMedia.ViewModels
{
    public class EditPostViewModel
    {
        public string Id { get; set; }

        [MaxLength(350)]
        public string Content { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public virtual IEnumerable<PostPicture> PostPictures { get; set; } 
        public IFormFileCollection AppendedPostPictures { get; set; }
        public string CalledFromAction { get; set; }
        public int Page { get; set; } = 1;

        public EditPostViewModel()
        {
            PostPictures = new List<PostPicture>();
        }
    }
}
