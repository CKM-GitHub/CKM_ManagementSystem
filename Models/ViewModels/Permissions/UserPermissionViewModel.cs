namespace CKM_ManagementSystem.Models.ViewModels.Permissions
{
    public class UserPermissionViewModel
    {
        public Dictionary<int, MenuPermission> MenuPermissions { get; set; } = new();

        public bool HasPermission(int menuID, string permission)
        {
            if (!MenuPermissions.TryGetValue(menuID, out var p))
                return false;

            return permission.ToLower() switch
            {
                "read" => p.CanRead,
                "write" => p.CanWrite,
                "delete" => p.CanDelete,
                _ => false
            };
        }
    }

    public class MenuPermission
    {
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public bool CanDelete { get; set; }
    }
}