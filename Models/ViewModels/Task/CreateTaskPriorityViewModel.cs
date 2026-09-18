using System;
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

        [Range(0,100, ErrorMessage = "Sort Order must be 0 or greater.")]
        [Display(Name = "Sort Order")]
        public int SortOrder { get; set; } = 0;

        [Display(Name = "Mode")]
        [Required(AllowEmptyStrings = true)]
        public string? Mode { get; set; } = "Entry";
    }
}