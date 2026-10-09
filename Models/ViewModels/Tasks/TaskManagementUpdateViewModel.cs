using System.ComponentModel.DataAnnotations;

namespace CKM_ManagementSystem.Models.ViewModels.Tasks
{
    public class TaskManagementUpdateViewModel
    {
        [Required]
        public int ID { get; set; }

        [Required(ErrorMessage = "Task Title / Program Name is required.")]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Assignee is required.")]
        public string Assignee { get; set; } = string.Empty;

        public DateTime? DueDate { get; set; }

        [Required(ErrorMessage = "Priority is required.")]
        public string PriorityCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status is required.")]
        public string StatusCode { get; set; } = string.Empty;

        public string? Attachments { get; set; }

        public IFormFile? AttachmentFile { get; set; }
    }
}