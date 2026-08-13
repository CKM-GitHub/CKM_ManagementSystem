using System.ComponentModel.DataAnnotations;
using CKM_ManagementSystem.Models.Validation;

namespace CKM_ManagementSystem.Models.ViewModels
{
    public class UserUpdateViewModel
    {
        [Display(Name = "Staff Code")]                    
        public string StaffCode { get; set; } = string.Empty;   

        [Required(ErrorMessage = "Full Name is required")]
        [Display(Name = "Full Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email Address is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Gender")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Department is required")]
        [Display(Name = "Department")]
        public string DepartmentCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        [Display(Name = "Assign Roles")]
        public string RoleCode { get; set; } = string.Empty;

        [Display(Name = "Status")]
        public bool Status { get; set; } = true;

        [Display(Name = "Profile Photo")]
        [AllowedExtensionsAttribute(new string[] { ".jpg", ".jpeg", ".png", ".gif" }, ErrorMessage = "Only image files (.jpg, .jpeg, .png, .gif) are allowed")]  //Drr ga image format check tr pop nyi lay yrr
        [MaxFileSizeAttribute(5 * 1024 * 1024, ErrorMessage = "Maximum file size is 5MB")]   // dr ka file size pop nyi lay yrr  
        public IFormFile? ImageFile { get; set; }
        public string? ImageUrl { get; set; } 
    }
}
