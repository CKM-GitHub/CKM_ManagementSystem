using System.ComponentModel.DataAnnotations;

namespace CKM_ManagementSystem.Models.Validation
{
    public class MustBeTrueAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            return value is bool b && b
                ? ValidationResult.Success
                : new ValidationResult(ErrorMessage ?? "This Field must be checked bro.");
        }
    }
}
