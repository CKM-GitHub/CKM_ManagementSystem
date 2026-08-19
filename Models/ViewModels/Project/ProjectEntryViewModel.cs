using System;
using System.ComponentModel.DataAnnotations;

namespace CKM_ManagementSystem.Models.ViewModels.Projects
{
    public class ProjectEntryViewModel
    {
        [Required(ErrorMessage = "Project Code is required.")]
        public string ProjectCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Project Name is required.")]
        public string ProjectName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a Project Manager.")]
        public string ProjectManagerId { get; set; } = string.Empty;

        public string? GitRepositoryUrl { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Start Date is required.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Target End Date is required.")]
        public DateTime EndDate { get; set; }

        public string Status { get; set; } = "Active";
    }
}