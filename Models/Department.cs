using System;
using System.Collections.Generic;

namespace CKM_ManagementSystem.Models;

public partial class Department
{
    public Guid Id { get; set; }

    public string DepartmentCode { get; set; } = null!;

    public string DepartmentName { get; set; } = null!;

    public Guid? ManagerUserId { get; set; }

    public string? Description { get; set; }

    public bool Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual User? ManagerUser { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
