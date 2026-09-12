using System.Collections.Generic;

namespace CKM_ManagementSystem.Models.ViewModels.Roles
{
    public class RoleListPagedViewModel
    {
        public List<RoleEntryViewModel> Roles { get; set; } = new List<RoleEntryViewModel>();
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
        public string SearchKeyword { get; set; }=string.Empty;
        public int? Status { get; set; }
    }
}