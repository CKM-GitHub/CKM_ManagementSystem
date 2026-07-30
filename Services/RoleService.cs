using System.Data;
using CKM_ManagementSystem.DL;
using CKM_ManagementSystem.Models.ViewModels;
using CKM_ManagementSystem.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CKM_ManagementSystem.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleDL _roleDL;

        public RoleService(IConfiguration configuration)
        {
            _roleDL = new RoleDL(configuration);
        }

        
        public async Task<bool> CheckDuplicateRoleCodeAsync(string roleCode)
        {
            return await _roleDL.CheckDuplicateRoleCodeSPAsync(roleCode);
        }

       
        public async Task SaveRoleWithPermissionsAsync(RoleEntryViewModel model)
        {
            
            bool isRoleSaved = await _roleDL.SaveRoleInfoSPAsync(model);

            if (!isRoleSaved) return;

           
            if (model.MenuPermissions != null && model.MenuPermissions.Any())
            {
                foreach (var perm in model.MenuPermissions)
                {
                    bool isAllowed = perm.CanRead || perm.CanWrite || perm.CanDelete;
                    await _roleDL.SaveRolePermissionSPAsync(model.RoleCode, perm.MenuId, isAllowed);
                }
            }
        }
    }
}