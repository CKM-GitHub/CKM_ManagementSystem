using System;
using System.Collections.Generic;

namespace CKM_ManagementSystem.Models.ViewModels.ProjectDetails
{
    public class ProjectDetailsDashboardViewModel
    {
        public string SearchQuery { get; set; } = string.Empty;

        // for task overview and project status
        public int TotalOpenTasks { get; set; }
        public int TotalInProgressTasks { get; set; }
        public int TotalReviewTasks { get; set; }
        public int TotalCompletedTasks { get; set; }
        public int TotalOnHoldTasks { get; set; }
        public int TotalCancelledTasks { get; set; }

        public int TotalActiveProjects { get; set; }
        public int TotalOnHoldProjects { get; set; }
        public int TotalCompletedProjects { get; set; }
        public int TotalInactiveProjects { get; set; }

        public List<ProjectListItem> Projects { get; set; } = new();

        public ProjectDetail? SelectedProjectInfo { get; set; }
        public List<ProjectMember> SelectedProjectMembers { get; set; } = new();
    }

    public class ProjectListItem
    {
        public string ProjectCode { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? Updated_Date { get; set; }
        public int OpenCount { get; set; }
        public int InProgressCount { get; set; }
        public int ReviewCount { get; set; }
        public int CompletedCount { get; set; }
        public int OnHoldCount { get; set; }
        public int CancelledCount { get; set; }
    }

    public class ProjectDetail
    {
        public string ProjectName { get; set; } = string.Empty;
        public string ProjectManagerName { get; set; } = string.Empty;
        public string ProjectManagerStaffCode { get; set; } = string.Empty;
        public string ProjectManagerImageURL { get; set; } = string.Empty;
        public string GitRepositoryUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class ProjectMember
    {
        public long No { get; set; }
        public string Staff_Code { get; set; } = string.Empty;
        public string Staff_Name { get; set; } = string.Empty;
        public string Image_URL { get; set; } = string.Empty;
    }
}