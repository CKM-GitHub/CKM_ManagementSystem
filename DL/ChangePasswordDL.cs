using CKM_ManagementSystem.DL.Interface;
using CKM_ManagementSystem.Models.ViewModels.Password;
using Microsoft.Data.SqlClient;

namespace CKM_ManagementSystem.DL
{
    public class ChangePasswordDL : BaseDL, IChangePasswordDL
    {
        public ChangePasswordDL(IConfiguration configuration) : base(configuration) { }
        public async Task<string?> GetCurrentPasswordAsync(string StaffCode)
        {
            SqlParameter parameter = new SqlParameter("@StaffCode", StaffCode);

            return await ExecuteScalarAsync(
                "sp_GetCurrentPassword",
                parameter);
        }
        public async Task<int> ChangePasswordAsync(ChangePasswordViewModel model)
        {
            SqlParameter staffCode = new SqlParameter("@StaffCode", model.StaffCode);
            SqlParameter newpassword = new SqlParameter("@NewPassword", model.NewPassword);

            return await ExecuteNonQueryAsync("sp_ChangePassword",
                staffCode, 
                newpassword);
        }
    }
}
