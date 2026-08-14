using System.Collections.Generic;
using System.Threading.Tasks;
using CKM_ManagementSystem.Models.ViewModels;
using CKM_ManagementSystem.Models.ViewModels.Roles;

namespace CKM_ManagementSystem.Services.Interfaces
{
    public interface IRoleService
    {
        Task<RoleListPagedViewModel> GetRoleListPagedAsync(int pageNumber, int pageSize, string searchKeyword, int? status);

        Task<List<MenuPermissionViewModel>> GetMenuPermissionsAsync(string roleCode = null);

        Task<RoleEntryViewModel> GetRoleByCodeSPAsync(string roleCode);

        Task<bool> CheckDuplicateRoleCodeAsync(string roleCode);

        Task SaveRoleWithPermissionsAsync(RoleEntryViewModel model);

        Task<(bool Success, string Message)> DeleteRoleAsync(string roleCode);
    }
}