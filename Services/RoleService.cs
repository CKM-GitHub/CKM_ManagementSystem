using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CKM_ManagementSystem.BL;
using CKM_ManagementSystem.Models.Entities;
using CKM_ManagementSystem.Models.ViewModels;
using CKM_ManagementSystem.Models.ViewModels.Roles;
using CKM_ManagementSystem.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CKM_ManagementSystem.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleBL _roleBL;

        public RoleService(IConfiguration configuration)
        {
            _roleBL = new RoleBL(configuration);
        }

        public async Task<bool> CheckDuplicateRoleCodeAsync(string roleCode)
        {
            return await _roleBL.CheckDuplicateRoleCodeSPAsync(roleCode);
        }

        public async Task SaveRoleWithPermissionsAsync(RoleEntryViewModel model)
        {
            bool isRoleSaved = await _roleBL.SaveRoleInfoSPAsync(model);

            if (!isRoleSaved) return;

            if (model.MenuPermissions != null && model.MenuPermissions.Any())
            {
                foreach (var perm in model.MenuPermissions)
                {
                    
                    await _roleBL.SaveRolePermissionSPAsync(
                        model.RoleCode,
                        perm.MenuId,
                        perm.CanRead,
                        perm.CanWrite,
                        perm.CanDelete
                    );
                }
            }
        }

        public async Task<RoleListPagedViewModel> GetRoleListAsync(string search, int? status, int page, int pageSize)
        {
            return await _roleBL.GetRoleListAsync(search, status, page, pageSize);
        }

        public async Task<RoleEntryViewModel> GetRoleByCodeAsync(string roleCode)
        {
            return await _roleBL.GetRoleByCodeAsync(roleCode);
        }

        public async Task<bool> DeleteRoleAsync(string roleCode)
        {
            return await _roleBL.DeleteRoleAsync(roleCode);
        }
    }
}