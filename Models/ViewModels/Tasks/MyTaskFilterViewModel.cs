namespace CKM_ManagementSystem.Models.ViewModels.Tasks
{
    public class MyTaskFilterViewModel
    {
        public string? ProjectCode { get; set; }

        public string? Title { get; set; }

        public DateTime? DueDateStart { get; set; }

        public DateTime? DueDateEnd { get; set; }

        public string? PriorityCode { get; set; }

        public string? StatusCode { get; set; }
    }
}