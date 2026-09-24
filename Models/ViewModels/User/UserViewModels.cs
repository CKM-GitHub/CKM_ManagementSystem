using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using CKM_ManagementSystem.Models.Validation;
using CKM_ManagementSystem.Models.ViewModels.Common;

namespace CKM_ManagementSystem.Models.ViewModels.User
{
    public class UserCreateViewModel : IValidatableObject
    {
        public string Mode { get; set; } = "Entry";

        [Required(ErrorMessage = "Staff Code is required")]
        [RegularExpression(@"^CKM-\d{4}$", ErrorMessage = "Staff Code must be in format CKM-XXXX (e.g., CKM-0001)")]
        [Display(Name = "Staff Code")]
        public string StaffCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email Address is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(200)]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.Equals(Mode, "Entry", StringComparison.OrdinalIgnoreCase))
            {
                yield break;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                yield return new ValidationResult(
                    "Password is required.",
                    new[] { nameof(Password) });
            }
            else
            {
                if (Password.Length < 8 || Password.Length > 15)
                {
                    yield return new ValidationResult(
                        "Password must be between 8 and 15 characters.",
                        new[] { nameof(Password) });
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(
                        Password,
                        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$"))
                {
                    yield return new ValidationResult(
                        "Password must contain at least one uppercase letter, one lowercase letter, and one number.",
                        new[] { nameof(Password) });
                }
            }

            if (string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                yield return new ValidationResult(
                    "Confirm Password is required.",
                    new[] { nameof(ConfirmPassword) });
            }
            else if (!string.Equals(Password, ConfirmPassword, StringComparison.Ordinal))
            {
                yield return new ValidationResult(
                    "Password and Confirm Password do not match.",
                    new[] { nameof(ConfirmPassword) });
            }
        }

        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

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

        [Required(ErrorMessage = "You must agree to the Terms of Service and Privacy Policy")]
        [MustBeTrue(ErrorMessage = "You must agree to the Terms of Service and Privacy Policy")]
        [Display(Name = "Terms Agreement")]
        public bool AcceptTerms { get; set; }

        [Display(Name = "Profile Photo")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".gif" }, ErrorMessage = "Only image files (.jpg, .jpeg, .png, .gif) are allowed")]
        [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "Maximum file size is 5MB")]
        public IFormFile? ImageFile { get; set; }

        public string? TempImageName { get; set; }
        public string? ImageUrl { get; set; }
        public bool CanWrite { get; set; }
    }

    public class DepartmentDropdownViewModel
    {
        public string DepartmentCode { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
    }

    public class RoleDropdownViewModel
    {
        public string RoleCode { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }

    public class UserListItemViewModel
    {
        public string StaffCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool Status { get; set; }
    }

    public class UserListViewModel
    {
        public PagedResponse<UserListItemViewModel> PagedData { get; set; } = new();

        public int OverallTotalCount { get; set; }
        public int OverallActiveCount { get; set; }
        public int OverallInactiveCount { get; set; }
        public int DepartmentCount { get; set; }

        public List<DepartmentDropdownViewModel> Departments { get; set; } = new();
        public List<RoleDropdownViewModel> Roles { get; set; } = new();

        public int ErrorCode { get; set; }
        public bool HasError => ErrorCode != 0;
        public bool CanWrite { get; set; }
        public bool CanDelete { get; set; }
    }
}