using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using SimpleSocialMedia.Models;

namespace SimpleSocialMedia.ViewModels
{
    public class CreatePostViewModel
    {
        public string Id { get; set; }

        [MaxLength(350)]
        public string Content { get; set; }

        public IFormFileCollection PostPictures { get; set; }
        public User User { get; set; }
    }
}
