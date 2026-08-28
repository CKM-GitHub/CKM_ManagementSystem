using System.ComponentModel.DataAnnotations;

namespace CKM_ManagementSystem.Models.ViewModels.Departments
{
    public class DepartmentEntryViewModel
    {
        

        [Required(ErrorMessage = "Department Code is required.")]
        [StringLength( 30,ErrorMessage = "Department Code cannot exceed 30 characters.")]
        [RegularExpression("^[A-Za-z0-9-]+$", ErrorMessage = "Department Code can only contain letters, numbers, and hyphens.")]
        [Display(Name = "Department Code")]
        public string DepartmentCode { get; set; } = string.Empty;

        [Display(Name = "Department Code")]
        public string? OriginalDepartmentCode { get; set; }

        [Required(ErrorMessage = "Department Name is required.")]
        [StringLength(150, ErrorMessage = "Department Name cannot exceed 150 characters.")]
        [Display(Name = "Department Name")]
        public string DepartmentName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        public bool Status { get; set; } = true;
    }
}