using Microsoft.AspNetCore.Mvc.Rendering;

namespace CKM_ManagementSystem.Models.ViewModels.Tasks
{
    public class TaskManagementFilterViewModel
    {
        public string? ProjectCode { get; set; }

        public string? PersonInCharge { get; set; }

        public string? Title { get; set; }

        public DateTime? IssueDateStart { get; set; }

        public DateTime? IssueDateEnd { get; set; }

        public DateTime? DueDateStart { get; set; }

        public DateTime? DueDateEnd { get; set; }

        public string? Assignee { get; set; }

        public string? PriorityCode { get; set; }

        public string? StatusCode { get; set; }

        public int CurrentPage { get; set; } = 1;

        public int PageSize { get; set; } = 5;

        public int TotalItems { get; set; }

        public int TotalPages =>
            (int)Math.Ceiling(
                (double)TotalItems / PageSize
            );

        public List<SelectListItem> ProjectList { get; set; }
            = new List<SelectListItem>();

        public List<SelectListItem> PersonInChargeList { get; set; }
            = new List<SelectListItem>();

        public List<SelectListItem> AssigneeList { get; set; }
            = new List<SelectListItem>();

        public List<SelectListItem> PriorityList { get; set; }
            = new List<SelectListItem>();

        public List<SelectListItem> StatusList { get; set; }
            = new List<SelectListItem>();

        public List<TaskManagementListItemViewModel> TaskListData { get; set; }
            = new List<TaskManagementListItemViewModel>();
    }
}