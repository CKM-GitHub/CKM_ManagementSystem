namespace CKM_ManagementSystem.Models.ViewModels
{
    public class DashboardViewModel
    {
        public string StaffCode { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? ImageURL { get; set; }
        public List<TaskStatusSummaryViewModel> TaskStatusSummary { get; set; } = new();
        public List<AnnouncementListViewModel> Announcements { get; set; } = new(); 
        public List<ProjectTaskSummaryViewModel> ProjectTaskSummary { get; set; } = new();
        //public AnnouncementDetailsViewModel? AnnouncementDetails { get; set; }
    }

    public class TaskStatusSummaryViewModel
    {
        public string StatusName { get; set; } = string.Empty;

        public int StatusCount { get; set; }
    }
    public class AnnouncementListViewModel
    {
        public int ID { get; set; }

        public string Title { get; set; } = string.Empty;
    }

    public class ProjectTaskSummaryViewModel
    {
        public string ProjectName { get; set; } = string.Empty;

        public int NewTasks { get; set; }

        public int OngoingTasks { get; set; }

        public string Status { get; set; } = string.Empty;
    }

    public class AnnouncementDetailsViewModel
    {
        public string Title { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string DepartmentName { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;
    }
}
