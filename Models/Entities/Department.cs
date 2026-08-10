using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CKM_ManagementSystem.Models.Entities
{
    [Table("Departments")]
    public class Department
    {
        

        [Required]
        [StringLength(30)]
        [Column("Department_Code")]
        public string DepartmentCode { get; set; } = string.Empty;

        [NotMapped]
        public string OriginalDepartmentCode { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        [Column("Department_Name")]
        public string DepartmentName { get; set; } = string.Empty;

        [Column("manager_user_id")]
        public Guid? ManagerUserId { get; set; }

        [StringLength(500)]
        [Column("Description")]
        public string? Description { get; set; }

        [Required]
        [Column("Status")]
        public bool Status { get; set; } = true;

        [Column("Created_Date")]
        public DateTime? CreatedDate { get; set; }

        [Column("Updated_Date")]
        public DateTime? UpdatedDate { get; set; }

        [Column("Deleted_Date")]
        public DateTime? DeletedDate { get; set; }
    }
}