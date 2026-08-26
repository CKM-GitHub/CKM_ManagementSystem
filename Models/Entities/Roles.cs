using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CKM_ManagementSystem.Models.Entities
{
    [Table("Roles")]
    public class Roles
    {
        [Key]
        [Required]
        [StringLength(20)]
        [Column("Role_Code")]
        public string RoleCode { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Column("Role_Name")]
        public string RoleName { get; set; } = string.Empty;

        [StringLength(250)]
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

    public class RolePermission
    {
        public string RoleCode { get; set; } = string.Empty;
        public int MenuId { get; set; }
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public bool CanDelete { get; set; }
    }
}