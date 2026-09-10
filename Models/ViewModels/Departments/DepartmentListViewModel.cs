namespace CKM_ManagementSystem.Models.ViewModels.Departments
{
    public class DepartmentListViewModel
    {
        public string? SearchText { get; set; }

        public bool? Status { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int TotalRecords { get; set; }

        public List<DepartmentListItemViewModel> Departments { get; set; }
            = new();

        public int TotalPages =>
            (int)Math.Ceiling((double)TotalRecords / PageSize);
    }

    public class DepartmentListItemViewModel
    {
        public string DepartmentCode { get; set; } = string.Empty;

        public string DepartmentName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool Status { get; set; }
    }
}