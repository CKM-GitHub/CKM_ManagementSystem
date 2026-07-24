using CKM_ManagementSystem.Models.ViewModels;

namespace CKM_ManagementSystem.Services.Interfaces
{
    public interface IRoleService
    {
        Task<bool> CheckDuplicateRoleCodeAsync(string roleCode);
        Task SaveRoleWithPermissionsAsync(RoleEntryViewModel model);
    }
}