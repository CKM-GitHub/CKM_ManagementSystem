using CKM_ManagementSystem.BL.Interface;
using CKM_ManagementSystem.DL.Interface;
using CKM_ManagementSystem.Models;
using CKM_ManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace CKM_ManagementSystem.BL
{
    public class UserEntryBL : IUserEntryBL
    {
        private readonly IUserEntryDL _userEntryDL;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public UserEntryBL(IUserEntryDL userEntryDL)
        {
            _userEntryDL = userEntryDL;
        }

        public async Task<int> CreateUserAsync(UserCreateViewModel model)
        {
            var user = new User
            {
                StaffCode = model.StaffCode,
                Name = model.Name,
                Email = model.Email
            };

            model.Password = _passwordHasher.HashPassword(user,model.Password);

            return await _userEntryDL.CreateUserAsync(model);
        }

        public async Task<IEnumerable<Department>> GetDepartmentsAsync()
        {
            return await _userEntryDL.GetDepartmentsAsync();
        }

        public async Task<IEnumerable<UserRole>> GetUserRolesAsync()
        {
            return await _userEntryDL.GetUserRolesAsync();
        }
    }
}