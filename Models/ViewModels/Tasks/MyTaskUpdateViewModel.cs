using System.ComponentModel.DataAnnotations;

namespace CKM_ManagementSystem.Models.ViewModels.Tasks
{
    public class MyTaskUpdateViewModel
    {
        [Required]
        public int ID { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public string StatusCode { get; set; } = string.Empty;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}