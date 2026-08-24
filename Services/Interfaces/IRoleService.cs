using System.Collections.Generic;
using System.Threading.Tasks;
using CKM_ManagementSystem.Models.ViewModels.Roles;

namespace CKM_ManagementSystem.Services.Interfaces
{
    public class IRoleService
    {
        Task<List<MenuPermissionViewModel>> GetMenuPermissionsAsync(string? roleCode = null);
        Task<bool> CheckDuplicateRoleCodeAsync(string roleCode);
        Task SaveRoleWithPermissionsAsync(RoleEntryViewModel model);
        Task<RoleListPagedViewModel> GetRoleListPagedAsync(int pageNumber, int pageSize, string? searchKeyword, int? status);
    }
}