using System.ComponentModel.DataAnnotations;
using CKM_ManagementSystem.Models.ViewModels.Common;
namespace CKM_ManagementSystem.Models.ViewModels.TaskStatuses
{
    public class TaskStatusesListViewModel
    {
        public string Status_Code { get; set; } = string.Empty;
        public string Status_Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int SortOrder { get; set; }
        public DateTime? Created_Date { get; set; }
        public DateTime? Updated_Date { get; set; }
    }
    public class TaskStatusesPagesResultViewModel
    {
        public PagedResponse<TaskStatusesListViewModel> PagedData { get; set; } = new();
        public string? Search { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
