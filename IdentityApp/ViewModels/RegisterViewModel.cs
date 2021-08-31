using System.ComponentModel.DataAnnotations;

namespace IdentityApp.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "The username must be between 6 and 20 symbols long")]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The password must be between 6 and 20 symbols long")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The password must be between 6 and 20 symbols long")]
        [Compare("Password", ErrorMessage = "The passwords do not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
