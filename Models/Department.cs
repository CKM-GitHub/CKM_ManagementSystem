using System;
using System.Collections.Generic;

namespace CKM_ManagementSystem.Models;

public partial class Department
{
    public string DepartmentCode { get; set; } = null!;

    public string DepartmentName { get; set; } = null!;

    public string? Description { get; set; }

    public bool Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
