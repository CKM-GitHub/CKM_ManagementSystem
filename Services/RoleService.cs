using System.Data;
using CKM_ManagementSystem.DL;
using CKM_ManagementSystem.Models.ViewModels;
using CKM_ManagementSystem.Services.Interfaces;

namespace CKM_ManagementSystem.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleDL _roleDL;

        public RoleService(RoleDL roleDL)
        {
            _roleDL = roleDL;
        }

        
        public async Task<bool> CheckDuplicateRoleCodeAsync(string roleCode)
        {
            if (string.IsNullOrWhiteSpace(roleCode)) return false;

            return await _roleDL.CheckDuplicateRoleCodeAsync(roleCode);
        }

        
        public async Task SaveRoleWithPermissionsAsync(RoleEntryViewModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            await _roleDL.SaveRoleWithPermissionsAsync(model);
        }

        
        public async Task<RoleListPagedViewModel> GetRoleListAsync(string searchKeyword, int? status, int pageNumber = 1, int pageSize = 10)
        {
            return await _roleDL.GetRoleListAsync(searchKeyword, status, pageNumber, pageSize);
        }

        public async Task<RoleEntryViewModel> GetRoleByCodeAsync(string roleCode)
        {
            if (string.IsNullOrWhiteSpace(roleCode)) return null;

            DataTable dt = await _roleDL.GetRoleByCodeSPAsync(roleCode);

            if (dt == null || dt.Rows.Count == 0) return null;

            DataRow row = dt.Rows[0];

            var model = new RoleEntryViewModel
            {
                RoleCode = row["RoleCode"]?.ToString(),
                DisplayName = row.Table.Columns.Contains("DisplayName") ? row["DisplayName"]?.ToString() : row["Role_Name"]?.ToString(),
                Description = row.Table.Columns.Contains("Description") ? row["Description"]?.ToString() : string.Empty,
                Status = row.Table.Columns.Contains("Status") && row["Status"] != DBNull.Value && Convert.ToBoolean(row["Status"])
            };

            return model;
        }

        public async Task<bool> DeleteRoleAsync(string roleCode)
        {
            if (string.IsNullOrWhiteSpace(roleCode)) return false;

            return await _roleDL.DeleteRoleAsync(roleCode);
        }
    }
}