using CKM_ManagementSystem.DL;
using CKM_ManagementSystem.Models.ViewModels.Password;
using CKM_ManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using System.Data;

/*namespace CKM_ManagementSystem.BL
{
    public class ChangePasswordBL : IChangePasswordBL
    {
        private readonly IChangePasswordDL _changePasswordDL;
        private readonly PasswordHasher<User> _passwordHasher = new();
        public ChangePasswordBL(IChangePasswordDL changePasswordDL)
        {
            _changePasswordDL = changePasswordDL;
        }

        public async Task<int> ChangePasswordAsync(ChangePasswordViewModel model)
        {

            if (string.IsNullOrWhiteSpace(model.StaffCode) ||
            string.IsNullOrWhiteSpace(model.CurrentPassword) ||
            string.IsNullOrWhiteSpace(model.NewPassword) ||
            string.IsNullOrWhiteSpace(model.ConfirmPassword))
            {
                return 3;
            }
            string? currentPassword = await _changePasswordDL.GetCurrentPasswordAsync(model.StaffCode);

            if (currentPassword == null)
            {
                return 1;
            }

            var user = new User
            {
                StaffCode = model.StaffCode
            };

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                currentPassword,
                model.CurrentPassword);

            if (result == PasswordVerificationResult.Failed)
            {
                return 2;
            }
            if (model.CurrentPassword == model.NewPassword)
            {
                return 4;
            }
            model.NewPassword = _passwordHasher.HashPassword(user, model.NewPassword);

            return await _changePasswordDL.ChangePasswordAsync(model);
        }
    } 
}*/

// Step 1: Check StaffCode, CurrentPassword, NewPassword, ConfirmPassword is null or empty → return 3 

// Step 2: Create @StaffCode parameter

// Step 3: Call bdl.ExecuteScalarAsync("sp_GetCurrentPassword")

// Step 4: If current password is null → return 1 (user not found)

// Step 5: Create User object with StaffCode

// Step 6: Verify current password using PasswordHasher

// Step 7: If verification failed → return 2

// Step 8: If CurrentPassword == NewPassword → return 4

// Step 9: Hash the NewPassword

// Step 10: Create @StaffCode, @NewPassword, @ReturnValue parameters

// Step 11: Call bdl.ExecuteNonQueryAsync("sp_ChangePassword")

// Step 12: Return the @ReturnValue from stored procedure

namespace CKM_ManagementSystem.BL 
{
     public class ChangePasswordBL
     {
        private readonly BaseDL bdl;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public ChangePasswordBL(BaseDL baseDL)
        {
            bdl = baseDL;
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

            string? currentPassword = await bdl.ExecuteScalarAsync("sp_GetCurrentPassword", staffCodeParam);
            
            if (currentPassword == null) 
            {
                return 1;
            }
            
            var user = new User
            {
                StaffCode = model.StaffCode
            };
            
            var verifyResult = _passwordHasher.VerifyHashedPassword(user, currentPassword,model.CurrentPassword);
            
            if(verifyResult == PasswordVerificationResult.Failed)
            {
                return 2;
            }
            
            if(model.CurrentPassword == model.NewPassword)
            {
                return 4;
            }
            
            model.NewPassword = _passwordHasher.HashPassword(user, model.NewPassword);
            
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