using System.Threading.Tasks;
using CKM_ManagementSystem.Models.Entities;
using CKM_ManagementSystem.Models.ViewModels;
using CKM_ManagementSystem.Models.ViewModels.Roles;

namespace CKM_ManagementSystem.Services.Interfaces
{
    public interface IRoleService
    {
        
        Task<bool> CheckDuplicateRoleCodeAsync(string roleCode);
        Task SaveRoleWithPermissionsAsync(RoleEntryViewModel model);

       
        Task<RoleListPagedViewModel> GetRoleListAsync(string search, int? status, int page, int pageSize);
        Task<RoleEntryViewModel> GetRoleByCodeAsync(string roleCode);
        Task<bool> DeleteRoleAsync(string roleCode);
    }
}