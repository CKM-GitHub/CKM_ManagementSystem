namespace CKM_ManagementSystem.Models.ViewModels.Tasks
{
    public class MyTaskStatusSummaryViewModel
    {
        public string StatusCode { get; set; } = string.Empty;

        public string StatusName { get; set; } = string.Empty;

        public int StatusCount { get; set; }

        public int TotalTasks { get; set; }
    }
}