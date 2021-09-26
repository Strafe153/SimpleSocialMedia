using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityApp.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }

        [Required]
        [Display(Name = "Username")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = 
            "The username must be between 1 and 20 symbols long")]
        public string UserName { get; set; }

        [Display(Name = "Year of birth")]
        public int? Year { get; set; }

        [MaxLength(30, ErrorMessage = 
            "The country name must up to 30 symbols long")]
        public string Country { get; set; }

        [MaxLength(30, ErrorMessage = 
            "The city name must be up to 20 symbols long")]
        public string City { get; set; }

        [MaxLength(40, ErrorMessage = 
            "The company name must be up to 40 symbols long")]
        public string Company { get; set; }

        [MaxLength(100, ErrorMessage =
            "The status length must be up to 100 symbols long")]
        public string Status { get; set; }

        [Display(Name = "Profile Picture")]
        public IFormFile ProfilePicture { get; set; }

        public IList<string> Roles { get; set; }
        public string CalledFromAction { get; set; }
    }
}
