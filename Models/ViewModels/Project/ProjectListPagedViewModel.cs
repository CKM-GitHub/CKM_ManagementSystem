using System;
using System.Collections.Generic;

namespace CKM_ManagementSystem.Models.ViewModels.Project
{
    public class ProjectListPagedViewModel
    {
        public List<ProjectListItemViewModel> Projects { get; set; }
            = new List<ProjectListItemViewModel>();

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public int TotalPages =>
            (int)Math.Ceiling((double)TotalRecords / PageSize);

        public string SearchKeyword { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool CanWrite { get; set; }
        public bool CanDelete { get; set; }
    }

    public class ProjectListItemViewModel
    {
        public long No { get; set; }
        public string ProjectCode { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string ProjectManagerName { get; set; } = string.Empty;
        public string? GitRepositoryUrl { get; set; }
        public int ProjectMemberCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ProjectType { get; set; } = string.Empty;
    }
}