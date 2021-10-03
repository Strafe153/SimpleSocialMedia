using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using IdentityApp.Models;

namespace IdentityApp.ViewModels
{
    public class CreatePostViewModel
    {
        public string Id { get; set; }

        [MaxLength(350)]
        public string Content { get; set; }

        public DateTime PostedTime { get; set; }

        // [MaxLength(5)]
        public IFormFileCollection PostPictures { get; set; }

        public User User { get; set; }
    }
}
