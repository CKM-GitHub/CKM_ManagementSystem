using CKM_ManagementSystem.Models.ViewModels.User;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CKM_ManagementSystem.Models.ViewModels.TaskStatuses
{
    public class CreateTaskStatusesViewModel
    {
        [Required(ErrorMessage = "Status Code is required.")]
        [StringLength(50, ErrorMessage = "Status Code cannot exceed 50 characters.")]
        public string Status_Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status Name is required.")]
        [StringLength(100, ErrorMessage = "Status Name cannot exceed 100 characters.")]
        public string Status_Name { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "Description cannot exceed 300 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Sort Order is required.")]
        [Range(0, 10, ErrorMessage = "Sort Order must be between 0 and 10.")]
        public int? SortOrder { get; set; } 

        public string? Mode { get; set; }
        public DateTime? Created_Date { get; set; }
        public DateTime? Updated_Date { get; set; }
    }
}
