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
        public DateTime PostedTime { get; set; }
    }
}
