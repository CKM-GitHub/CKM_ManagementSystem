using CKM_ManagementSystem.Models.ViewModels.Permissions;
using System.Text.Json;

namespace CKM_ManagementSystem.Authorization
{
    public class CurrentUserPermission
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private UserPermissionViewModel? _cached;

        public CurrentUserPermission(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool CanRead(int menuID) => Get().HasPermission(menuID, "read");
        public bool CanWrite(int menuID) => Get().HasPermission(menuID, "write");
        public bool CanDelete(int menuID) => Get().HasPermission(menuID, "delete");

        public UserPermissionViewModel Get()
        {
            if (_cached != null)
                return _cached;

            var json = _httpContextAccessor
                .HttpContext?
                .Session
                .GetString("UserPermissions");

            _cached = string.IsNullOrEmpty(json)
                ? new UserPermissionViewModel()
                : JsonSerializer.Deserialize<UserPermissionViewModel>(json)
                  ?? new UserPermissionViewModel();

            return _cached;
        }
    }
}