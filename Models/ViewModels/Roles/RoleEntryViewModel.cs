using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CKM_ManagementSystem.Models.ViewModels.Roles
{
    public class RoleEntryViewModel
    {
        [Required(ErrorMessage = "Role Code is required.")]
        [StringLength(20, ErrorMessage = "Role Code cannot exceed 20 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_\-]+$", ErrorMessage = "Role Code can only contain letters, numbers, underscores, and hyphens.")]
        public string RoleCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Display Name is required.")]
        [StringLength(50, ErrorMessage = "Display Name cannot exceed 50 characters.")]
        public string DisplayName { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters.")]
        public string? Description { get; set; }

        public bool Status { get; set; } = true;

        public bool IsEdit { get; set; } = false;

        public List<MenuPermissionViewModel> MenuPermissions { get; set; } = new List<MenuPermissionViewModel>();
    }

    public class MenuPermissionViewModel
    {
        public int MenuId { get; set; }

        public string MenuName { get; set; } = string.Empty;

        public int? ParentId { get; set; }

        public int Level { get; set; } = 0;

        public bool CanRead { get; set; } = false;

        public bool CanWrite { get; set; } = false;

        public bool CanDelete { get; set; } = false;
    }
}