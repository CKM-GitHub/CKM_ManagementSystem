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
    }
}