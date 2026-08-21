using System.Collections.Generic;
using System.Threading.Tasks;
using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Models.ViewModels;
using CKM_ManagementSystem.Models.ViewModels.Roles;
using CKM_ManagementSystem.Services.Interfaces;

namespace CKM_ManagementSystem.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleBL _roleBL;

        public RoleService(RoleBL roleBL)
        {
            _roleBL = roleBL;
        }

        public async Task<RoleListPagedViewModel> GetRoleListPagedAsync(int pageNumber, int pageSize, string searchKeyword, int? status)
        {
            return await _roleBL.GetRoleListPagedAsync(pageNumber, pageSize, searchKeyword, status);
        }

        public async Task<List<MenuPermissionViewModel>> GetMenuPermissionsAsync(string roleCode = null)
        {
            return await _roleBL.GetMenuPermissionsAsync(roleCode);
        }

        public async Task<RoleEntryViewModel> GetRoleByCodeSPAsync(string roleCode)
        {
            return await _roleBL.GetRoleByCodeSPAsync(roleCode);
        }

        public async Task<bool> CheckDuplicateRoleCodeAsync(string roleCode)
        {
            return await _roleBL.CheckDuplicateRoleCodeAsync(roleCode);
        }

        public async Task SaveRoleWithPermissionsAsync(RoleEntryViewModel model)
        {
            await _roleBL.SaveRoleWithPermissionsAsync(model);
        }

        public async Task<(bool Success, string Message)> DeleteRoleAsync(string roleCode)
        {
            return await _roleBL.DeleteRoleAsync(roleCode);
        }
    }
}