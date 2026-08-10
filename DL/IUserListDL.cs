using CKM_ManagementSystem.Models.ViewModels;

namespace CKM_ManagementSystem.DL
{
    public interface IUserListDL
    {
        Task<(List<UserListViewModel>Users,int ErrorCode)> GetUsersAsync(
            string? searchText,
            bool? status,
            string? departmentCode,
            string? roleCode,
            int pageNumber,
            int pageSize);
        Task<bool> DeleteUserAsync(string saffCode);
    }

}
