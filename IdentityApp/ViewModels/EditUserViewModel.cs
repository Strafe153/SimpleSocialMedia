using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IdentityApp.Models;

namespace IdentityApp.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }

        [Required]
        [Display(Name = "Username")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = 
            "The username must be between 6 and 20 symbols long")]
        public string UserName { get; set; }

        [Display(Name = "Year of birth")]
        public int? Year { get; set; }

        [StringLength(20, MinimumLength = 1, ErrorMessage = 
            "The country name must be between 1 and 20 symbols long")]
        public string Country { get; set; }

        [StringLength(20, MinimumLength = 1, ErrorMessage = 
            "The city name must be between 1 and 20 symbols long")]
        public string City { get; set; }

        [StringLength(40, MinimumLength = 1, ErrorMessage = 
            "The company name must be between 1 and 40 symbols long")]
        public string Company { get; set; }

        public string Status { get; set; }

        [Display(Name = "Profile Picture")]
        public IFormFile ProfilePicture { get; set; }

        public IList<string> Roles { get; set; }

        public string CalledFromAction { get; set; }
    }
}
