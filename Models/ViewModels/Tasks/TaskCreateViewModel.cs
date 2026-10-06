using System.ComponentModel.DataAnnotations;

namespace CKM_ManagementSystem.Models.ViewModels.Tasks
{
    public class TaskCreateViewModel
    {
        [Required(ErrorMessage = "Project is required.")]
        public string ProjectCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Task Title is required.")]
        public string Title { get; set; } =string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage="Assignee is required.")]
        public string? Assignee { get; set; } = string.Empty ;

        public DateTime? DueDate { get; set; }

        [Required(ErrorMessage = "Priority is required.")]
        public string PriorityCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status is required.")]
        public string StatusCode { get; set; } = string.Empty;

        public string? Attachements { get; set; } 

    }
}