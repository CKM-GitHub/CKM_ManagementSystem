using CKM_ManagementSystem.DL;
using CKM_ManagementSystem.Models.ViewModels.Password;
using CKM_ManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CKM_ManagementSystem.BL 
{
     public class ChangePasswordBL
     {
        private readonly BaseDL bdl;
        private readonly PasswordService _passwordService;

        public ChangePasswordBL(BaseDL baseDL, PasswordService passwordService)
        {
            bdl = baseDL;
            _passwordService = passwordService;
        }

        public async Task<int> ChangePasswordAsync(ChangePasswordViewModel model)
        {
            if(string.IsNullOrWhiteSpace(model.StaffCode)||
               string.IsNullOrWhiteSpace(model.CurrentPassword)||
               string.IsNullOrWhiteSpace(model.NewPassword)||
               string.IsNullOrWhiteSpace(model.ConfirmPassword))
            {
                return 3;
            }

            SqlParameter staffCodeParam = new SqlParameter("@StaffCode",model.StaffCode);

            object? result = bdl.ExecuteScalarObject(
            "sp_GetCurrentPassword",
            staffCodeParam);

            string? currentPassword = result?.ToString();

            if (string.IsNullOrEmpty(currentPassword))
            {
                return 1;
            }

            bool isValid = _passwordService.VerifyPassword(
            currentPassword,
            model.CurrentPassword);

            if (!isValid)
            {
                return 2;
            }

            if (model.CurrentPassword == model.NewPassword)
            {
                return 4;
            }
            
            model.NewPassword = _passwordService.HashPassword(model.NewPassword);
            
            SqlParameter staffCode = new SqlParameter("@StaffCode", model.StaffCode);
            SqlParameter newPassword = new SqlParameter("@NewPassword", model.NewPassword);
            SqlParameter returnValue = new SqlParameter("@ReturnValue", SqlDbType.Int)
            {
                Direction = ParameterDirection.ReturnValue
            };
            
            await bdl.ExecuteNonQueryAsync("sp_ChangePassword",staffCode, newPassword, returnValue);
            
            return (int) returnValue.Value;   
        }
     }    
}