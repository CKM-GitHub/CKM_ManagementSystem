using System.ComponentModel.DataAnnotations;

namespace CKM_ManagementSystem.Models.ViewModels.Password
{
    public class ChangePasswordViewModel
    {
        public string? StaffCode { get; set; }

        [Required(ErrorMessage ="Current Password is required")]
        [Display(Name = "Current Password")]
        public string? CurrentPassword {  get; set; }

        [Required(ErrorMessage = "New Password is required")]
        [StringLength(15, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 15 characters")]  // Dr ka Password Count Check tr br
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one number")]   // Dr ka format check tr pop nyi lay yrr
        [Display(Name = "New Password")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("NewPassword", ErrorMessage = "Password and Confirm Password do not match")]
        [Display(Name = "Confirm Password")]
        public string? ConfirmPassword { get; set; }
    }
}
