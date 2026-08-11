namespace CKM_ManagementSystem.Models.ViewModels
{
    public class UserListViewModel
    {
        public string StaffCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool Status { get; set; } = false;
        public int OverallTotalCount { get; set; }
        public int OverallActiveCount { get; set; }
        public int OverallInactiveCount { get; set; }
        public int TotalCount { get; set; }
        public int DepartmentCount { get; set; }
    }
    public class DepartmentDropdownViewModel
    {
        public string DepartmentCode { get; set; } = string.Empty;
        public string DepartmentName { get; set;} = string.Empty;
    }
    public class RoleDropdownViewModel
    {
        public string RoleCode { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }
}
