using Microsoft.AspNetCore.Mvc.Rendering;
using CKM_ManagementSystem.Models.Entities;

namespace CKM_ManagementSystem.Models.ViewModels
{
    public class TasksViewModel
    {
        public string ProjectCode { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string PersonInCharge { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime? IssueDateStart { get; set; }
        public DateTime? IssueDateEnd { get; set; }
        public DateTime? DueDateStart { get; set; }
        public DateTime? DueDateEnd { get; set; }
        public string Assignee { get; set; } = string.Empty;
        public string PriorityCode { get; set; } = string.Empty;
        public string StatusCode { get; set; } = string.Empty;

        public int PageNumber { get; set; } 
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public List<SelectListItem> ProjectList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> PersonInChargeList { get;set; } = new List<SelectListItem>();
        public List<SelectListItem> AssigneeList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> PriorityList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> StatusList { get; set; } = new List<SelectListItem>();
        public List<Tasks> TaskListData { get; set; } = new List<Tasks>();

    }
}
