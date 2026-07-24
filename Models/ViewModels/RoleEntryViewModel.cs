using System.ComponentModel.DataAnnotations;

namespace CKM_ManagementSystem.Models.ViewModels
{
    public class RoleEntryViewModel
    {
        [Required(ErrorMessage = "Role Code ဖြည့်ရန် လိုအပ်ပါသည်။")]
        public string RoleCode { get; set; }

        [Required(ErrorMessage = "Display Name ဖြည့်ရန် လိုအပ်ပါသည်။")]
        public string DisplayName { get; set; }

        public string Description { get; set; }

        public bool Status { get; set; } = true;

        
        public List<MenuPermissionViewModel> MenuPermissions { get; set; } = new List<MenuPermissionViewModel>();
    }

    
    public class MenuPermissionViewModel
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public int? ParentId { get; set; }
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public bool CanDelete { get; set; }
    }
}