using System;
using System.Collections.Generic;

namespace CKM_ManagementSystem.Models;

public partial class UserRolePermission
{
    public string RoleCode { get; set; } = null!;

    public int MenuId { get; set; }

    public bool CanRead { get; set; }

    public bool CanWrite { get; set; }

    public bool CanDelete { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual Menu Menu { get; set; } = null!;

    public virtual UserRole RoleCodeNavigation { get; set; } = null!;
}
