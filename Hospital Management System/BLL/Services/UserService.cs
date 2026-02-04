using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HospitalManagementSystem.DAL.Repositories;
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

            var result = await _repository.SearchUsersAsync(query, query, null, null, 1, 200).ConfigureAwait(false);
            return new List<User>(result.Items);
        }

        /// <summary>
        /// Adds a user.
        /// </summary>
        public async Task<int> AddAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return await _repository.AddUserAsync(user).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates a user.
        /// </summary>
        public Task<bool> UpdateAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return _repository.UpdateUserAsync(user);
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        public Task<bool> DeleteAsync(int userId)
        {
            return _repository.DeleteUserAsync(userId);
        }
    }
}
