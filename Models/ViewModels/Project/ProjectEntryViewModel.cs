using System;
using System.ComponentModel.DataAnnotations;

namespace CKM_ManagementSystem.Models.ViewModels.Projects
{
    public class ProjectEntryViewModel
    {
        [Required(ErrorMessage = "Project Code is required.")]
        [StringLength(20, ErrorMessage = "Project Code cannot exceed 20 characters.")]
        public string ProjectCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Project Name is required.")]
        [StringLength(100, ErrorMessage = "Project Name cannot exceed 100 characters.")]
        public string ProjectName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a Project Manager.")]
        public string ProjectManagerId { get; set; } = string.Empty;

        [Url(ErrorMessage = "Invalid Git Repository URL format.")]
        public string? GitRepositoryUrl { get; set; }

        [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Start Date is required.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Target End Date is required.")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(1);

        public string Status { get; set; } = "Active";
    }
}