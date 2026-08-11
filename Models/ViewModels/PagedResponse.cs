namespace CKM_ManagementSystem.Models.ViewModels
{
    public class PagedResponse<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public int OverallTotalCount { get; set; }
        public int OverallActiveCount { get; set; }
        public int OverallInactiveCount { get; set; }
        public int TotalCount { get; set; }
        public int DepartmentCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages =>
            PageSize > 0
                ? (int)Math.Ceiling((double)TotalCount / PageSize)
                : 0;
        public List<DepartmentDropdownViewModel> Departments { get; set; } = new List<DepartmentDropdownViewModel>();
        public List<RoleDropdownViewModel> Roles { get; set; } = new List<RoleDropdownViewModel>();
        public int ErrorCode { get; set; }
        public bool HasError => ErrorCode != 0;
    }
}