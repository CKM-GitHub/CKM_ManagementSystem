using CKM_ManagementSystem.Models.ViewModels.Common;
using CKM_ManagementSystem.Models.ViewModels.Task;

namespace CKM_ManagementSystem.Models.ViewModels.TaskPriorities
{
    public class TaskPriorityListViewModel
    {
        public PagedResponse<TaskPriorityListItemViewModel> PagedData { get; set; } = new();
        public CreateTaskPriorityViewModel Entry { get; set; }= new();
        public string? Search { get; set; }
        public int ErrorCode { get; set; }
        public bool HasError => ErrorCode != 0;
    }
}