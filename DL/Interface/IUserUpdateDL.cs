using CKM_ManagementSystem.Models.ViewModels;

namespace CKM_ManagementSystem.DL.Interface
{
    public interface IUserUpdateDL
    {
        Task<UserUpdateViewModel?> GetUserByStaffCodeAsync(string staffCode);
        Task<List<DepartmentDropdownViewModel>> GetDepartmentsAsync();
        Task<List<RoleDropdownViewModel>> GetRolesAsync();
        Task<int> UserUpdateAsync(UserUpdateViewModel model);
    }
}
