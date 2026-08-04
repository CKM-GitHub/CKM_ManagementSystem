using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

public class UserCreateDto
{
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

    [Required(ErrorMessage = "Password is required")]
    [StringLength(15, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 15 characters")]  // Dr ka Password Count Check tr br
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one number")]   // Dr ka format check tr pop nyi lay yrr
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm Password is required")]
    [Compare("Password", ErrorMessage = "Password and Confirm Password do not match")]
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
}

public class MustBeTrueAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        return value is bool b && b
            ? ValidationResult.Success
            : new ValidationResult(ErrorMessage ?? "This Field must be checked bro.");
    }

}

public class AllowedExtensionsAttribute : ValidationAttribute
{
    private readonly string[] _extensions;
    public AllowedExtensionsAttribute(string[] extensions) => _extensions = extensions;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_extensions.Contains(extension))
            {
                return new ValidationResult(ErrorMessage ?? $"Only {string.Join(", ", _extensions)} files are allowed.");
            }
        }
        return ValidationResult.Success;
    }
}

public class MaxFileSizeAttribute : ValidationAttribute
{
    private readonly int _maxFileSize;
    public MaxFileSizeAttribute(int maxFileSize) => _maxFileSize = maxFileSize;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is IFormFile file && file.Length > _maxFileSize)
        {
            return new ValidationResult(ErrorMessage ?? $"Maximum allowed file size is {_maxFileSize} bytes.");
        }
        return ValidationResult.Success;
    }
}