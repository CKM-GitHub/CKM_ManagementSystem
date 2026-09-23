using System.ComponentModel.DataAnnotations;

namespace CKM_ManagementSystem.Models.Entities
{
    public class TaskStatuses
    {
        public string Status_Code { get; set; } = string.Empty;
        public string Status_Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int SortOrder { get; set; }
        public DateTime? Created_Date { get; set; }
        public DateTime? Updated_Date { get; set; }
    }
}
