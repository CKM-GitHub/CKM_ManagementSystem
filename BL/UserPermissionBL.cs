using CKM_ManagementSystem.DL;
using CKM_ManagementSystem.Models.ViewModels.Permissions;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CKM_ManagementSystem.BL
{
    public class UserPermissionBL
    {
        private readonly BaseDL _baseDL;

        public UserPermissionBL(BaseDL baseDL)
        {
            _baseDL = baseDL;
        }
        public async Task<UserPermissionViewModel> GetPermissionsAsync(
            string staffCode)
        {
            var table = await _baseDL.SelectDataTableAsync(
                "sp_GetUserPermission",
                new SqlParameter("@StaffCode", staffCode),
                new SqlParameter("@MenuID", DBNull.Value));

            var result = new UserPermissionViewModel();

            foreach (DataRow row in table.Rows)
            {
                int menuID = Convert.ToInt32(row["MenuID"]);

                result.MenuPermissions[menuID] = new MenuPermission
                {
                    CanRead = row["CanRead"] != DBNull.Value && Convert.ToBoolean(row["CanRead"]),
                    CanWrite = row["CanWrite"] != DBNull.Value && Convert.ToBoolean(row["CanWrite"]),
                    CanDelete = row["CanDelete"] != DBNull.Value && Convert.ToBoolean(row["CanDelete"])
                };
            }

            return result;
        }
    }
}