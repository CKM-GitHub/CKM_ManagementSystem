namespace CKM_ManagementSystem.Models.ViewModels.Tasks
{
    public class TaskManagementListItemViewModel
    {
        public int ID { get; set; }

        public string ProjectName { get; set; } = string.Empty;

        public string PersonInChargeName { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string AssigneeName { get; set; } = string.Empty;

        public DateTime IssueDate { get; set; }

        public DateTime? DueDate { get; set; }

        public string PriorityName { get; set; } = string.Empty;

        public string StatusName { get; set; } = string.Empty;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? Attachments { get; set; }

    }
}