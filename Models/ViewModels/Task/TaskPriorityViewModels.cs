using CKM_ManagementSystem.Models.ViewModels.Common;
using CKM_ManagementSystem.Models.ViewModels.Task;
using System.ComponentModel.DataAnnotations;

namespace CKM_ManagementSystem.Models.ViewModels.Task
{
    public class CreateTaskPriorityViewModel
    {
        [Required(ErrorMessage = "Priority Code is required!")]
        [StringLength(20, ErrorMessage = "Priority Code cannot exceed 20 characters.")]
        [Display(Name = "Code")]
        public string Priority_Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Priority Name is required!")]
        [StringLength(100, ErrorMessage = "Priority Name cannot exceed 100 characters.")]
        [Display(Name = "Name")]
        public string Priority_Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters!")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Range(0, 100, ErrorMessage = "Sort Order must be 0 or greater.")]
        [Display(Name = "Sort Order")]
        public int SortOrder { get; set; } = 0;

        [Display(Name = "Mode")]
        [Required(AllowEmptyStrings = true)]
        public string? Mode { get; set; } = "Entry";
    }

    public class TaskPriorityListItemViewModel
    {
        public string Priority_Code { get; set; } = string.Empty;
        public string Priority_Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int SortOrder { get; set; }
    }
}

namespace CKM_ManagementSystem.Models.ViewModels.TaskPriorities
{
    public class TaskPriorityListViewModel
    {
        public PagedResponse<TaskPriorityListItemViewModel> PagedData { get; set; } = new();
        public CreateTaskPriorityViewModel Entry { get; set; } = new();
        public string? Search { get; set; }
        public int ErrorCode { get; set; }
        public bool HasError => ErrorCode != 0;
    }
}