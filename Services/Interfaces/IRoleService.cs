using CKM_ManagementSystem.Models.ViewModels;

namespace CKM_ManagementSystem.Services.Interfaces
{
    public interface IRoleService
    {
        Task<bool> CheckDuplicateRoleCodeAsync(string roleCode);

        Task SaveRoleWithPermissionsAsync(RoleEntryViewModel model);

        Task<RoleListPagedViewModel> GetRoleListAsync(string searchKeyword, int? status, int pageNumber = 1, int pageSize = 10);

        
        Task<RoleEntryViewModel> GetRoleByCodeAsync(string roleCode);

        Task<bool> DeleteRoleAsync(string roleCode);
    }
}