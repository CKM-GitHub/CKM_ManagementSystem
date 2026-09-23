using CKM_ManagementSystem.Models.ViewModels.Permissions;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace CKM_ManagementSystem.Authorization
{
    public class PermissionAuthorizationHandler
        : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionAuthorizationHandler(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var session = _httpContextAccessor.HttpContext?.Session;

            if (session == null)
                return Task.CompletedTask;

            var json = session.GetString("UserPermissions");

            if (string.IsNullOrEmpty(json))
                return Task.CompletedTask;

            var permissions = JsonSerializer
                .Deserialize<UserPermissionViewModel>(json);

            if (permissions != null &&
                permissions.HasPermission(
                    requirement.MenuID,
                    requirement.Permission))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}