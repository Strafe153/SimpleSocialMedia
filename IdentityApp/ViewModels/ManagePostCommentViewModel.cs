using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IdentityApp.Models;

namespace IdentityApp.ViewModels
{
    public class ManagePostCommentViewModel
    {
        public string CommentId { get; set; }

        [MaxLength(200)]
        public string Content { get; set; }

        public string Author { get; set; }
        public string CommentedPostUser { get; set; }
        public virtual IEnumerable<CommentPicture> CommentPictures { get; set; }
        public IFormFileCollection AppendedCommentPictures { get; set; }
        public string ReturnUrl { get; set; }
        public int Page { get; set; }

        public ManagePostCommentViewModel()
        {
            CommentPictures = new List<CommentPicture>();
        }
    }
}
