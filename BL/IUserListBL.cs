using CKM_ManagementSystem.Models.ViewModels;

namespace CKM_ManagementSystem.BL
{
    public interface IUserListBL
    {
        Task<PagedResponse<UserListViewModel>> GetUserListAsync(
            string? searchText,
            bool? status,
            string? departmentCode,
            string? roleCode,
            int pageNumber,
            int pageSize);
    }
}
