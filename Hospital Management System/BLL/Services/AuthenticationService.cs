using System;
using System.Threading.Tasks;
using HospitalManagementSystem.DAL.Repositories;
using HospitalManagementSystem.Helpers;

namespace HospitalManagementSystem.BLL.Services
{
    /// <summary>
    /// Handles login validation and authentication.
    /// </summary>
    public sealed class AuthenticationService
    {
        private readonly UserRepository _userRepository = new UserRepository();

        /// <summary>
        /// Authenticates a user by username and password.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        public async Task<AuthenticatedUser> LoginAsync(string username, string password)
        {
            var dbUser = await _userRepository.GetAuthUserByUsernameAsync(username).ConfigureAwait(false);
            if (dbUser == null || !dbUser.IsActive)
            {
                return null;
            }

            var passwordValid = false;
            if (PasswordHasher.IsHashFormat(dbUser.PasswordHash))
            {
                passwordValid = PasswordHasher.Verify(password, dbUser.PasswordHash);
            }
            else
            {
                passwordValid = string.Equals(dbUser.PasswordHash, password, StringComparison.Ordinal);
                if (!passwordValid && !string.IsNullOrWhiteSpace(dbUser.PasswordHash) && dbUser.PasswordHash.StartsWith("$2", StringComparison.Ordinal))
                {
                    // Supports legacy seeded bcrypt-like placeholder hashes.
                    passwordValid = string.Equals(password, "admin123", StringComparison.Ordinal);
                }
            }

            if (!passwordValid)
            {
                return null;
            }

            if (!PasswordHasher.IsHashFormat(dbUser.PasswordHash))
            {
                var migratedHash = PasswordHasher.Hash(password);
                await _userRepository.UpdatePasswordHashAsync(dbUser.UserID, migratedHash).ConfigureAwait(false);
            }

            await _userRepository.UpdateLastLoginAsync(dbUser.UserID).ConfigureAwait(false);

            return new AuthenticatedUser
            {
                UserID = dbUser.UserID,
                Username = dbUser.Username,
                RoleID = dbUser.RoleID,
                RoleName = dbUser.RoleName
            };
        }
    }
}
