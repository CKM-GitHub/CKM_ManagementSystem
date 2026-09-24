using CKM_ManagementSystem.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace CKM_ManagementSystem.Authorization
{
    public class PermissionPolicyProvider
        : IAuthorizationPolicyProvider
    {
        private readonly DefaultAuthorizationPolicyProvider
            _fallbackPolicyProvider;

        public PermissionPolicyProvider(
            IOptions<AuthorizationOptions> options)
        {
            _fallbackPolicyProvider =
                new DefaultAuthorizationPolicyProvider(
                    options);
        }

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            const string prefix = "Permission.";

            if (!policyName.StartsWith(
                    prefix,
                    StringComparison.OrdinalIgnoreCase))
            {
                return _fallbackPolicyProvider
                    .GetPolicyAsync(policyName);
            }

            var parts = policyName.Split('.');

            if (parts.Length != 3)
            {
                return Task.FromResult<AuthorizationPolicy?>(null);
            }

            string menuName = parts[1];
            string permission = parts[2];

            int? menuID = GetMenuID(menuName);

            if (menuID == null)
            {
                return Task.FromResult<AuthorizationPolicy?>(
                    null);
            }

            var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .AddRequirements( new PermissionRequirement(
                                menuID.Value,
                                permission))
                        .Build();

            return Task.FromResult<
                AuthorizationPolicy?>(policy);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return _fallbackPolicyProvider
                .GetDefaultPolicyAsync();
        }

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            return _fallbackPolicyProvider
                .GetFallbackPolicyAsync();
        }

        private static int? GetMenuID(string menuName)
        {
            var field = typeof(MenuIDs).GetField(
                menuName,
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.IgnoreCase);

            if (field == null)
                return null;

            return (int?)field.GetValue(null);
        }
    }
}