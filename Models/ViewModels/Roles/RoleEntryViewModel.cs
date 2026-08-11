using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CKM_ManagementSystem.Models.ViewModels.Roles
{
    public class RoleEntryViewModel
    {
        [Required(ErrorMessage = "Role Code ဖြည့်ရန် လိုအပ်ပါသည်။")]
        [StringLength(20, ErrorMessage = "Role Code သည် စာလုံးရေ 20 ထက် မပိုရပါ။")]
        public string RoleCode { get; set; }

        [Required(ErrorMessage = "Display Name ဖြည့်ရန် လိုအပ်ပါသည်။")]
        [StringLength(50, ErrorMessage = "Display Name သည် စာလုံးရေ 50 ထက် မပိုရပါ။")]
        public string DisplayName { get; set; }

        [StringLength(250, ErrorMessage = "Description သည် စာလုံးရေ 250 ထက် မပိုရပါ။")]
        public string? Description { get; set; }

        public bool Status { get; set; } = true;

        public List<MenuPermissionViewModel> MenuPermissions { get; set; } = new List<MenuPermissionViewModel>();
    }

    public class MenuPermissionViewModel
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public bool CanDelete { get; set; }
    }
}