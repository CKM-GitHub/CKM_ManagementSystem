using System.Collections.Generic;
using System.Threading.Tasks;
using CKM_ManagementSystem.Models.ViewModels.Roles;
using CKM_ManagementSystem.Services.Interfaces;
using CKM_ManagementSystem.BL;

namespace CKM_ManagementSystem.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleBL _roleBL;

        public RoleService(RoleBL roleBL)
        {
            _roleBL = roleBL;
        }

        public async Task<List<MenuPermissionViewModel>> GetMenuPermissionsAsync()
        {
            return await _roleBL.GetMenuPermissionsAsync();
        }

        public async Task<bool> CheckDuplicateRoleCodeAsync(string roleCode)
        {
            return await _roleBL.CheckDuplicateRoleCodeAsync(roleCode);
        }

        public async Task SaveRoleWithPermissionsAsync(RoleEntryViewModel model)
        {
            await _roleBL.SaveRoleWithPermissionsAsync(model);
        }
    }
}