using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CKM_ManagementSystem.Models.ViewModels
{
    public class CreateMenuViewModel
    {
        public int? MenuID { get; set; }

        [Required(ErrorMessage = "Display Text is required.")]
        [StringLength(100, ErrorMessage ="Display Text cannot exceed 100 characters.")]
        public string DisplayText { get; set; } = string.Empty;

        [Required(ErrorMessage ="Action Name is required.")]
        [StringLength(100, ErrorMessage = "Action Name cannot exceed 100 characters")]
        public string ActionName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Controller Name is required.")]
        [StringLength(100, ErrorMessage = "Controller Name cannot exceed 100 characters")]
        public string ControllerName { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Icon Class cannot exceed 50 characters. ")]
        [RegularExpression(@"^[a-zA-Z\s\-]+$", ErrorMessage = "Icon class can only contain letters, spaces, and hyphens.")]
        public string? IconClass { get; set; }

        [Required(ErrorMessage = "Display Order is required.")]
        [Range(0, 999,  ErrorMessage = "Please enter numbers only between 000 and 999.")]
        public int? DisplayOrder { get; set; }

        public string MenuType { get; set; } = "Parent";

        public int? ParentMenuId { get; set; }
        public bool IsSubMenu { get; set; }
        public bool Status { get; set; } = true;
        public List<SelectListItem> ParentMenuList { get; set; } = new List<SelectListItem>();
    }
}
