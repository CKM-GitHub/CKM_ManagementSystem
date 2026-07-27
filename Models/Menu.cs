using System;
using System.Collections.Generic;

namespace CKM_ManagementSystem.Models;

public partial class Menu
{
    public int MenuId { get; set; }

    public string MenuName { get; set; } = null!;

    public string ActionName { get; set; } = null!;

    public string ControllerName { get; set; } = null!;

    public string? MenuIcon { get; set; }

    public int DisplayOrder { get; set; }

    public int? ParentMenuId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual ICollection<Menu> InverseParentMenu { get; set; } = new List<Menu>();

    public virtual Menu? ParentMenu { get; set; }

    public virtual ICollection<UserRolePermission> UserRolePermissions { get; set; } = new List<UserRolePermission>();
}
