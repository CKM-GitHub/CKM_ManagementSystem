using CKM_ManagementSystem.BL.Interface;
using CKM_ManagementSystem.DL.Interface;
using CKM_ManagementSystem.Models.ViewModels;

namespace CKM_ManagementSystem.BL
{
    public class UserListBL : IUserListBL
    {
        private readonly IUserListDL _userDL;

        public UserListBL(IUserListDL userListDL)
        {
            _userDL = userListDL;
        }

        public async Task<PagedResponse<UserListViewModel>> GetUserListAsync(
            string? searchText,
            bool? status,
            string? departmentCode,
            string? roleCode,
            int pageNumber,
            int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var (rawUsers, rawDepartments, rawRoles, dbErrorCode) = await _userDL.GetUsersAsync(
                searchText,
                status,
                departmentCode,
                roleCode,
                pageNumber,
                pageSize
            );

            var response = new PagedResponse<UserListViewModel>
            {
                Data = rawUsers,
                Departments = rawDepartments, 
                Roles = rawRoles,            
                ErrorCode = dbErrorCode,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            if (rawUsers.Count > 0)
            {
                var targetRow = rawUsers[0];
                response.OverallTotalCount = targetRow.OverallTotalCount;
                response.OverallActiveCount = targetRow.OverallActiveCount;
                response.OverallInactiveCount = targetRow.OverallInactiveCount;
                response.TotalCount = targetRow.TotalCount;
                response.DepartmentCount = targetRow.DepartmentCount;
            }

            return response;
        }
        public async Task<(int ErrorCode, string? UserName)> DeleteUserAsync(string staffCode)
        {
            var result = await _userDL.DeleteUserAsync(staffCode);

            return result;
        }
    }
}
