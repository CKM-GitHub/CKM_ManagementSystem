using System;
using System.Collections.Generic;

namespace CKM_ManagementSystem.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string StaffCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public string RoleCode { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public string DepartmentCode { get; set; } = null!;

    public string TimeZone { get; set; } = null!;

    public bool Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual Department DepartmentCodeNavigation { get; set; } = null!;

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    public virtual UserRole RoleCodeNavigation { get; set; } = null!;
}
