using CKM_ManagementSystem.DL.Interface;
using CKM_ManagementSystem.Models.ViewModels;
using CKM_ManagementSystem.BL.Interface;

namespace CKM_ManagementSystem.BL
{
    public class UserUpdateBL : IUserUpdateBL
    {
        private readonly IUserUpdateDL _userUpdateDL;

        public UserUpdateBL(IUserUpdateDL userUpdateDL)
        {
            _userUpdateDL = userUpdateDL;
        }
        public async Task<UserUpdateViewModel?> GetUserByStaffCodeAsync(string staffCode)
        {
            if (string.IsNullOrWhiteSpace(staffCode))
            {
                return null;
            }
            return await _userUpdateDL.GetUserByStaffCodeAsync(staffCode);
        }
        public async Task<List<DepartmentDropdownViewModel>>GetDepartmentsAsync()
        {
            return await _userUpdateDL.GetDepartmentsAsync();
        }

        public async Task<List<RoleDropdownViewModel>>GetRolesAsync()
        {
            return await _userUpdateDL.GetRolesAsync();
        }
        public async Task<int> UserUpdateAsync(UserUpdateViewModel model)
        {
            if (model == null)
            {
                return 3;
            }
            return await _userUpdateDL.UserUpdateAsync(model);
        }
    }
}
