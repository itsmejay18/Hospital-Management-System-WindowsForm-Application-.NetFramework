using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HospitalManagementSystem.DAL.Repositories;
using HospitalManagementSystem.Helpers;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.BLL.Services
{
    /// <summary>
    /// Provides user management logic.
    /// </summary>
    public sealed class UserService
    {
        private readonly UserRepository _repository = new UserRepository();

        /// <summary>
        /// Gets all users.
        /// </summary>
        public Task<List<User>> GetAllAsync()
        {
            return _repository.GetAllUsersAsync();
        }

        /// <summary>
        /// Searches users by username or email.
        /// </summary>
        /// <param name="query">Search text.</param>
        public async Task<List<User>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return await GetAllAsync().ConfigureAwait(false);
            }

            var term = query.Trim();
            var users = await GetAllAsync().ConfigureAwait(false);
            return users
                .Where(user =>
                    ContainsInsensitive(user.Username, term)
                    || ContainsInsensitive(user.Email, term))
                .ToList();
        }

        /// <summary>
        /// Adds a user.
        /// </summary>
        public async Task<int> AddAsync(User user)
        {
            AuthorizationHelper.EnsureRole("Administrator");
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (!PasswordHasher.IsHashFormat(user.PasswordHash))
            {
                user.PasswordHash = PasswordHasher.Hash(user.PasswordHash);
            }

            return await _repository.AddUserAsync(user).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates a user.
        /// </summary>
        public Task<bool> UpdateAsync(User user)
        {
            AuthorizationHelper.EnsureRole("Administrator");
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (!string.IsNullOrWhiteSpace(user.PasswordHash) && !PasswordHasher.IsHashFormat(user.PasswordHash))
            {
                user.PasswordHash = PasswordHasher.Hash(user.PasswordHash);
            }

            return _repository.UpdateUserAsync(user);
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        public Task<bool> DeleteAsync(int userId)
        {
            AuthorizationHelper.EnsureRole("Administrator");
            return _repository.DeleteUserAsync(userId);
        }

        /// <summary>
        /// Gets available roles.
        /// </summary>
        public Task<List<UserRole>> GetRolesAsync()
        {
            return _repository.GetAllRolesAsync();
        }

        public Task<UserDetail> GetUserDetailAsync(int userId)
        {
            return _repository.GetUserDetailByUserIdAsync(userId);
        }

        public Task<int> AddUserDetailAsync(UserDetail detail)
        {
            return _repository.AddUserDetailAsync(detail);
        }

        public Task<bool> UpdateUserDetailAsync(UserDetail detail)
        {
            return _repository.UpdateUserDetailAsync(detail);
        }

        private static bool ContainsInsensitive(string value, string searchText)
        {
            return !string.IsNullOrWhiteSpace(value)
                   && value.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
