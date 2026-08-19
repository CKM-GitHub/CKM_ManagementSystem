using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CKM_ManagementSystem.Models.Entities
{
    [Table("Projects")]
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }

        [Required]
        [StringLength(50)]
        public string ProjectCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string ProjectName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string ProjectManagerId { get; set; } = string.Empty;

        [StringLength(500)]
        public string? GitRepositoryUrl { get; set; }

        public string? Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Active";

        public DateTime Created_Date { get; set; } = DateTime.Now;

        public DateTime? Updated_Date { get; set; }
    }
}