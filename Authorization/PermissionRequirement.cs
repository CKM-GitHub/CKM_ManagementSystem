using Microsoft.AspNetCore.Authorization;

namespace CKM_ManagementSystem.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public int MenuID { get; }

        public string Permission { get; }

        public PermissionRequirement(
            int menuID,
            string permission)
        {
            MenuID = menuID;
            Permission = permission;
        }
    }
}