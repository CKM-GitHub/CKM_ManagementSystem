using System.ComponentModel.DataAnnotations;

namespace CKM_ManagementSystem.Models.Validation
{
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
}
