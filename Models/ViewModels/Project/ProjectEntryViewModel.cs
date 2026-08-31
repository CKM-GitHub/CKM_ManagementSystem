using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CKM_ManagementSystem.Models.ViewModels.Projects
{
    public class ProjectEntryViewModel
    {
        [RegularExpression(@"^[a-zA-Z0-9\-_]+$", ErrorMessage = "Special characters are not allowed. Only letters, numbers, hyphens, and underscores.")]
        public string ProjectCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Project Name is required.")]
        [RegularExpression(@"^[a-zA-Z0-9\s\-_]+$", ErrorMessage = "Special characters are not allowed in Project Name.")]
        public string ProjectName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Project Manager is required.")]
        public string ProjectManagerId { get; set; } = string.Empty;

        public string? GitRepositoryUrl { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Start Date is required.")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "Target End Date is required.")]
        public DateTime? EndDate { get; set; }

        public string Status { get; set; } = "Active";

        public bool IsEdit { get; set; } = false;

        public List<ProjectMemberViewModel> ProjectMembers { get; set; } = new List<ProjectMemberViewModel>();
    }

    public class ProjectMemberViewModel
    {
        public string Staff_Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Image_URL { get; set; }
    }

    public class ProjectMemberSearchViewModel
    {
        public string Staff_Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Image_URL { get; set; }
        public string? Department_Name { get; set; }
    }
}