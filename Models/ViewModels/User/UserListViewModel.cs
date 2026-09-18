using CKM_ManagementSystem.Models.ViewModels.Common;

namespace CKM_ManagementSystem.Models.ViewModels.User
{
    public class UserListViewModel
    {
        public PagedResponse<UserListItemViewModel> PagedData { get; set; } = new();

        public int OverallTotalCount { get; set; }

        public int OverallActiveCount { get; set; }

        public int OverallInactiveCount { get; set; }

        public int DepartmentCount { get; set; }

        public List<DepartmentDropdownViewModel> Departments { get; set; } = new();

        public List<RoleDropdownViewModel> Roles { get; set; } = new();

        public int ErrorCode { get; set; }

        public bool HasError => ErrorCode != 0;
    }
}