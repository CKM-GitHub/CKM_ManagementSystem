namespace CKM_ManagementSystem.Models.Entities
{
    public class Tasks
    {
        public long No { get; set; }
        public int ID { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string PersonInCharge { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Assignee { get; set; } = string.Empty;
        public DateTime? IssueDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Attachments { get; set; } = string.Empty;
        public int TotalCount { get; set; }
    }
}
