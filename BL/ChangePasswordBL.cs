using CKM_ManagementSystem.BL.Interface;
using CKM_ManagementSystem.DL;
using CKM_ManagementSystem.DL.Interface;
using CKM_ManagementSystem.Models.ViewModels.Password;
using CKM_ManagementSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace CKM_ManagementSystem.BL
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

          //  if (model.NewPassword != model.ConfirmPassword)
          //  {
            //    return 4;
        //    }

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
}
