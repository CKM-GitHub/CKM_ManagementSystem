using System.ComponentModel.DataAnnotations;

namespace CKM_ManagementSystem.Models.Validation
{
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
}
