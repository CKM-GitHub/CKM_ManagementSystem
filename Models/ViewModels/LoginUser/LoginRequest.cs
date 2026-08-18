using System.ComponentModel.DataAnnotations;

namespace CKM_ManagementSystem.Models.ViewModels.LoginUser
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email Address is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        public string? Staff_Code { get; set; }
    }
}
