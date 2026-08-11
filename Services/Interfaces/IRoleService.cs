using System.Collections.Generic;
using System.Threading.Tasks;
using CKM_ManagementSystem.Models.ViewModels.Roles;

namespace CKM_ManagementSystem.Services.Interfaces
{
    public interface IRoleService
    {
        Task<List<MenuPermissionViewModel>> GetMenuPermissionsAsync();

        Task<bool> CheckDuplicateRoleCodeAsync(string roleCode);
        Task SaveRoleWithPermissionsAsync(RoleEntryViewModel model);
    }
}