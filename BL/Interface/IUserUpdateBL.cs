using CKM_ManagementSystem.Models.ViewModels;

namespace CKM_ManagementSystem.BL.Interface
{
    public interface IUserUpdateBL
    {
        Task<UserUpdateViewModel?> GetUserByStaffCodeAsync(string staffCode);
        Task<List<DepartmentDropdownViewModel>> GetDepartmentsAsync();
        Task<List<RoleDropdownViewModel>> GetRolesAsync();

        Task<int> UserUpdateAsync(UserUpdateViewModel model);
    }
}
