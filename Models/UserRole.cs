using System;
using System.Collections.Generic;

namespace CKM_ManagementSystem.Models;

public partial class UserRole
{
    public Guid Id { get; set; }

    public string RoleCode { get; set; } = null!;

    public string RoleName { get; set; } = null!;

    public string? Description { get; set; }

    public bool Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual ICollection<UserRolePermission> UserRolePermissions { get; set; } = new List<UserRolePermission>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
