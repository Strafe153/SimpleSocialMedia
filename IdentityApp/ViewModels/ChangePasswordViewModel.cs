using System.ComponentModel.DataAnnotations;

namespace IdentityApp.ViewModels
{
    public class ChangePasswordViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The password must be between 6 and 20 symbols long")]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The password must be between 6 and 20 symbols long")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The passwords do not match")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The password must be between 6 and 20 symbols long")]
        [Display(Name = "Confirm new password")]
        public string ConfirmNewPassword { get; set; }

        public string ReturnUrl { get; set; }
    }
}
