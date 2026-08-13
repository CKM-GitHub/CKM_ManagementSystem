using CKM_ManagementSystem.Models.ViewModels;

namespace CKM_ManagementSystem.DL.Interface
{
    public interface IUserListDL
    {
        Task<(List<UserListViewModel>Users,List<DepartmentDropdownViewModel> Departments,List<RoleDropdownViewModel> Roles, int ErrorCode)> GetUsersAsync(
            string? searchText,
            bool? status,
            string? departmentCode,
            string? roleCode,
            int pageNumber,
            int pageSize);
        Task<(int ErrorCode, string? UserName)> DeleteUserAsync(string saffCode);
    }
}
