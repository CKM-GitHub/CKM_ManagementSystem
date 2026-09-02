using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using CKM_ManagementSystem.Models.Validation;

namespace CKM_ManagementSystem.Models.ViewModels;
public class UserCreateViewModel
{
    public string Mode { get; set; } = "Entry";

    [Required(ErrorMessage = "Staff Code is required")]
    [RegularExpression(@"^CKM-\d{4}$", ErrorMessage = "Staff Code must be in format CKM-XXXX (e.g., CKM-0001)")]  // Dr ka Code format check htar drr pr
    [Display(Name = "Staff Code")]                     // Dr ka label mr show me sarr pr  .  Under Code tway lae D a Taing pr be
    public string StaffCode { get; set; } = string.Empty;   //.NET 8 mr ma pr ma phyit pr chin dl so "datatype.empty" enter pay ya pr dl , .NET 9 mr dop ma lo vu pop nyi lay yrr

    [Required(ErrorMessage = "Full Name is required")]
    [Display(Name = "Full Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email Address is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Mode == "Entry")
        {
            if (string.IsNullOrWhiteSpace(Password))
            {
                yield return new ValidationResult(
                    "Password is required",
                    new[] { nameof(Password) });
            }
            else
            {
                if (Password.Length < 8 || Password.Length > 15)
                {

                    yield return new ValidationResult(
                    "Password must be between 8 and 15 characters",
                    new[] { nameof(Password) });
                }
                if (!System.Text.RegularExpressions.Regex.IsMatch(Password,
                    @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$"))
                {
                    yield return new ValidationResult(
                        "Password must contain at least one uppercase letter, one lowercase letter, and one number",
                        new[] { nameof(Password) }
                    );
                }
                if (string.IsNullOrWhiteSpace(ConfirmPassword))
                {
                    yield return new ValidationResult(
                        "Confirm Password is required",
                        new[] { nameof(ConfirmPassword) }
                    );
                }
                else if (Password != ConfirmPassword)
                {
                    yield return new ValidationResult(
                        "Password and Confirm Password do not match",
                        new[] { nameof(ConfirmPassword) }
                    );
                }
            }
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
    [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".gif" }, ErrorMessage = "Only image files (.jpg, .jpeg, .png, .gif) are allowed")]  //Drr ga image format check tr pop nyi lay yrr
    [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "Maximum file size is 5MB")]   // dr ka file size pop nyi lay yrr  
    public IFormFile? ImageFile { get; set; }
    public string? TempImageName { get; set; }
    public string? ImageUrl { get; set; }
}





