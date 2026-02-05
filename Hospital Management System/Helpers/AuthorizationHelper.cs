using System;

namespace HospitalManagementSystem.Helpers
{
    /// <summary>
    /// Provides role-based authorization checks.
    /// </summary>
    public static class AuthorizationHelper
    {
        public static bool IsInRole(params string[] roles)
        {
            var currentRole = UserSession.CurrentUser?.RoleName;
            if (string.IsNullOrWhiteSpace(currentRole) || roles == null)
            {
                return false;
            }

            foreach (var role in roles)
            {
                if (string.Equals(currentRole, role, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public static void EnsureRole(params string[] roles)
        {
            if (!IsInRole(roles))
            {
                throw new UnauthorizedAccessException("You do not have permission to perform this action.");
            }
        }
    }
}
