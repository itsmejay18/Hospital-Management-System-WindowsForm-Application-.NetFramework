using System;
using System.Collections.Generic;

namespace HospitalManagementSystem.Helpers
{
    /// <summary>
    /// Provides role-based authorization checks.
    /// </summary>
    public static class AuthorizationHelper
    {
        private static readonly Dictionary<string, string> RoleAliases =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["administrator"] = "administrator",
                ["admin"] = "administrator",
                ["systemadministrator"] = "administrator",
                ["systemadmin"] = "administrator",
                ["superadmin"] = "administrator",
                ["doctor"] = "doctor",
                ["physician"] = "doctor",
                ["nurse"] = "nurse",
                ["receptionist"] = "receptionist",
                ["frontdesk"] = "receptionist",
                ["frontdeskstaff"] = "receptionist",
                ["pharmacist"] = "pharmacist",
                ["labtechnician"] = "lab technician",
                ["accountant"] = "accountant",
                ["hrmanager"] = "hr manager"
            };

        private static readonly Dictionary<int, string> DefaultRoleById =
            new Dictionary<int, string>
            {
                [1] = "administrator",
                [2] = "doctor",
                [3] = "nurse",
                [4] = "receptionist",
                [5] = "pharmacist",
                [6] = "lab technician",
                [7] = "accountant",
                [8] = "hr manager"
            };

        public static bool IsInRole(params string[] roles)
        {
            var currentUser = UserSession.CurrentUser;
            if (currentUser == null || roles == null || roles.Length == 0)
            {
                return false;
            }

            var currentRole = NormalizeRole(currentUser.RoleName);
            string currentRoleById = null;
            if (DefaultRoleById.TryGetValue(currentUser.RoleID, out var mappedById))
            {
                currentRoleById = mappedById;
            }

            foreach (var role in roles)
            {
                var requiredRole = NormalizeRole(role);
                if (string.IsNullOrWhiteSpace(requiredRole))
                {
                    continue;
                }

                if (string.Equals(currentRole, requiredRole, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(currentRoleById, requiredRole, StringComparison.OrdinalIgnoreCase))
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

        private static string NormalizeRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return string.Empty;
            }

            var token = roleName.Trim().Replace(" ", string.Empty);
            if (RoleAliases.TryGetValue(token, out var canonicalRole))
            {
                return canonicalRole;
            }

            return roleName.Trim().ToLowerInvariant();
        }
    }
}
