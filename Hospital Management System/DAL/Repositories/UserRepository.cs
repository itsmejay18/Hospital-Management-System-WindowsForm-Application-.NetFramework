using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using HospitalManagementSystem.DAL.DTOs;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.DAL.Repositories
{
    /// <summary>
    /// Provides CRUD operations for users, user details, and roles.
    /// </summary>
    public sealed class UserRepository : RepositoryBase
    {
        /// <summary>
        /// Gets a user with role name by username.
        /// </summary>
        /// <param name="username">Username.</param>
        public Task<AuthenticatedUserDto> GetAuthUserByUsernameAsync(string username)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"SELECT u.UserID,
                                            u.Username,
                                            u.PasswordHash,
                                            u.RoleID,
                                            ur.RoleName,
                                            u.IsActive
                                     FROM Users u
                                     INNER JOIN UserRoles ur ON ur.RoleID = u.RoleID
                                     WHERE u.Username = @Username";

                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.QuerySingleOrDefaultAsync<AuthenticatedUserDto>(sql, new { Username = username }).ConfigureAwait(false);
                }
            }, "GetAuthUserByUsernameAsync");
        }

        /// <summary>
        /// Updates last login time.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        public Task UpdateLastLoginAsync(int userId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "UPDATE Users SET LastLogin = NOW() WHERE UserID = @UserID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    await connection.ExecuteAsync(sql, new { UserID = userId }).ConfigureAwait(false);
                    return true;
                }
            }, "UpdateLastLoginAsync");
        }

        /// <summary>
        /// Updates password hash.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="passwordHash">New password hash.</param>
        public Task UpdatePasswordHashAsync(int userId, string passwordHash)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "UPDATE Users SET PasswordHash = @PasswordHash WHERE UserID = @UserID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    await connection.ExecuteAsync(sql, new { UserID = userId, PasswordHash = passwordHash }).ConfigureAwait(false);
                    return true;
                }
            }, "UpdatePasswordHashAsync");
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        public IEnumerable<User> GetAllUsers()
        {
            return ExecuteSafe(() =>
            {
                const string sql = "SELECT * FROM Users ORDER BY Username";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Query<User>(sql).ToList();
                }
            }, "GetAllUsers");
        }

        /// <summary>
        /// Gets all users asynchronously.
        /// </summary>
        public Task<List<User>> GetAllUsersAsync()
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT * FROM Users ORDER BY Username";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var results = await connection.QueryAsync<User>(sql).ConfigureAwait(false);
                    return results.ToList();
                }
            }, "GetAllUsersAsync");
        }

        /// <summary>
        /// Gets a user by identifier.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        public User GetUserById(int userId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "SELECT * FROM Users WHERE UserID = @UserID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.QuerySingleOrDefault<User>(sql, new { UserID = userId });
                }
            }, "GetUserById");
        }

        /// <summary>
        /// Gets a user by identifier asynchronously.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        public Task<User> GetUserByIdAsync(int userId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT * FROM Users WHERE UserID = @UserID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.QuerySingleOrDefaultAsync<User>(sql, new { UserID = userId }).ConfigureAwait(false);
                }
            }, "GetUserByIdAsync");
        }

        /// <summary>
        /// Gets a user by username.
        /// </summary>
        /// <param name="username">Username.</param>
        public User GetUserByUsername(string username)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "SELECT * FROM Users WHERE Username = @Username";
                using (var connection = Db.OpenConnection())
                {
                    return connection.QuerySingleOrDefault<User>(sql, new { Username = username });
                }
            }, "GetUserByUsername");
        }

        /// <summary>
        /// Gets a user by username asynchronously.
        /// </summary>
        /// <param name="username">Username.</param>
        public Task<User> GetUserByUsernameAsync(string username)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT * FROM Users WHERE Username = @Username";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Username = username }).ConfigureAwait(false);
                }
            }, "GetUserByUsernameAsync");
        }

        /// <summary>
        /// Adds a new user and returns the new identifier.
        /// </summary>
        /// <param name="user">User entity.</param>
        public int AddUser(User user)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"INSERT INTO Users
                                    (Username, PasswordHash, Email, RoleID, IsActive, LastLogin)
                                    VALUES (@Username, @PasswordHash, @Email, @RoleID, @IsActive, @LastLogin);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = Db.OpenConnection())
                {
                    return connection.ExecuteScalar<int>(sql, user);
                }
            }, "AddUser");
        }

        /// <summary>
        /// Adds a new user asynchronously and returns the new identifier.
        /// </summary>
        /// <param name="user">User entity.</param>
        public Task<int> AddUserAsync(User user)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"INSERT INTO Users
                                    (Username, PasswordHash, Email, RoleID, IsActive, LastLogin)
                                    VALUES (@Username, @PasswordHash, @Email, @RoleID, @IsActive, @LastLogin);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteScalarAsync<int>(sql, user).ConfigureAwait(false);
                }
            }, "AddUserAsync");
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="user">User entity.</param>
        public bool UpdateUser(User user)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"UPDATE Users SET
                                    Username = @Username,
                                    PasswordHash = @PasswordHash,
                                    Email = @Email,
                                    RoleID = @RoleID,
                                    IsActive = @IsActive,
                                    LastLogin = @LastLogin
                                    WHERE UserID = @UserID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, user) > 0;
                }
            }, "UpdateUser");
        }

        /// <summary>
        /// Updates an existing user asynchronously.
        /// </summary>
        /// <param name="user">User entity.</param>
        public Task<bool> UpdateUserAsync(User user)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"UPDATE Users SET
                                    Username = @Username,
                                    PasswordHash = @PasswordHash,
                                    Email = @Email,
                                    RoleID = @RoleID,
                                    IsActive = @IsActive,
                                    LastLogin = @LastLogin
                                    WHERE UserID = @UserID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, user).ConfigureAwait(false) > 0;
                }
            }, "UpdateUserAsync");
        }

        /// <summary>
        /// Deletes a user by identifier.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        public bool DeleteUser(int userId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "DELETE FROM Users WHERE UserID = @UserID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, new { UserID = userId }) > 0;
                }
            }, "DeleteUser");
        }

        /// <summary>
        /// Deletes a user by identifier asynchronously.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        public Task<bool> DeleteUserAsync(int userId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "DELETE FROM Users WHERE UserID = @UserID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, new { UserID = userId }).ConfigureAwait(false) > 0;
                }
            }, "DeleteUserAsync");
        }

        /// <summary>
        /// Searches users with filters and pagination.
        /// </summary>
        /// <param name="username">Username filter.</param>
        /// <param name="email">Email filter.</param>
        /// <param name="roleId">Role filter.</param>
        /// <param name="isActive">Active filter.</param>
        /// <param name="pageNumber">Page number (1-based).</param>
        /// <param name="pageSize">Page size.</param>
        public Task<PagedResult<User>> SearchUsersAsync(
            string username,
            string email,
            int? roleId,
            bool? isActive,
            int pageNumber,
            int pageSize)
        {
            return ExecuteSafeAsync(async () =>
            {
                var sql = new StringBuilder("SELECT * FROM Users WHERE 1=1 ");
                var countSql = new StringBuilder("SELECT COUNT(*) FROM Users WHERE 1=1 ");
                var parameters = new DynamicParameters();

                if (!string.IsNullOrWhiteSpace(username))
                {
                    sql.Append(" AND Username LIKE @Username ");
                    countSql.Append(" AND Username LIKE @Username ");
                    parameters.Add("@Username", $"%{username}%");
                }

                if (!string.IsNullOrWhiteSpace(email))
                {
                    sql.Append(" AND Email LIKE @Email ");
                    countSql.Append(" AND Email LIKE @Email ");
                    parameters.Add("@Email", $"%{email}%");
                }

                if (roleId.HasValue)
                {
                    sql.Append(" AND RoleID = @RoleID ");
                    countSql.Append(" AND RoleID = @RoleID ");
                    parameters.Add("@RoleID", roleId.Value);
                }

                if (isActive.HasValue)
                {
                    sql.Append(" AND IsActive = @IsActive ");
                    countSql.Append(" AND IsActive = @IsActive ");
                    parameters.Add("@IsActive", isActive.Value);
                }

                sql.Append(" ORDER BY Username ");
                AddPagination(sql, parameters, pageNumber, pageSize);

                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var totalCount = await connection.ExecuteScalarAsync<int>(countSql.ToString(), parameters).ConfigureAwait(false);
                    var items = await connection.QueryAsync<User>(sql.ToString(), parameters).ConfigureAwait(false);

                    return new PagedResult<User>
                    {
                        Items = items.ToList(),
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    };
                }
            }, "SearchUsersAsync");
        }

        /// <summary>
        /// Gets user details by user identifier.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        public UserDetail GetUserDetailByUserId(int userId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "SELECT * FROM UserDetails WHERE UserID = @UserID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.QuerySingleOrDefault<UserDetail>(sql, new { UserID = userId });
                }
            }, "GetUserDetailByUserId");
        }

        /// <summary>
        /// Gets user details by user identifier asynchronously.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        public Task<UserDetail> GetUserDetailByUserIdAsync(int userId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT * FROM UserDetails WHERE UserID = @UserID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.QuerySingleOrDefaultAsync<UserDetail>(sql, new { UserID = userId }).ConfigureAwait(false);
                }
            }, "GetUserDetailByUserIdAsync");
        }

        /// <summary>
        /// Adds a user detail record and returns the new identifier.
        /// </summary>
        /// <param name="detail">User detail entity.</param>
        public int AddUserDetail(UserDetail detail)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"INSERT INTO UserDetails
                                    (UserID, FirstName, LastName, DateOfBirth, Gender, ContactNumber, Address, EmergencyContact, ProfileImage)
                                    VALUES (@UserID, @FirstName, @LastName, @DateOfBirth, @Gender, @ContactNumber, @Address, @EmergencyContact, @ProfileImage);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = Db.OpenConnection())
                {
                    return connection.ExecuteScalar<int>(sql, detail);
                }
            }, "AddUserDetail");
        }

        /// <summary>
        /// Adds a user detail record asynchronously and returns the new identifier.
        /// </summary>
        /// <param name="detail">User detail entity.</param>
        public Task<int> AddUserDetailAsync(UserDetail detail)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"INSERT INTO UserDetails
                                    (UserID, FirstName, LastName, DateOfBirth, Gender, ContactNumber, Address, EmergencyContact, ProfileImage)
                                    VALUES (@UserID, @FirstName, @LastName, @DateOfBirth, @Gender, @ContactNumber, @Address, @EmergencyContact, @ProfileImage);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteScalarAsync<int>(sql, detail).ConfigureAwait(false);
                }
            }, "AddUserDetailAsync");
        }

        /// <summary>
        /// Updates a user detail record.
        /// </summary>
        /// <param name="detail">User detail entity.</param>
        public bool UpdateUserDetail(UserDetail detail)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"UPDATE UserDetails SET
                                    FirstName = @FirstName,
                                    LastName = @LastName,
                                    DateOfBirth = @DateOfBirth,
                                    Gender = @Gender,
                                    ContactNumber = @ContactNumber,
                                    Address = @Address,
                                    EmergencyContact = @EmergencyContact,
                                    ProfileImage = @ProfileImage
                                    WHERE UserDetailID = @UserDetailID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, detail) > 0;
                }
            }, "UpdateUserDetail");
        }

        /// <summary>
        /// Updates a user detail record asynchronously.
        /// </summary>
        /// <param name="detail">User detail entity.</param>
        public Task<bool> UpdateUserDetailAsync(UserDetail detail)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"UPDATE UserDetails SET
                                    FirstName = @FirstName,
                                    LastName = @LastName,
                                    DateOfBirth = @DateOfBirth,
                                    Gender = @Gender,
                                    ContactNumber = @ContactNumber,
                                    Address = @Address,
                                    EmergencyContact = @EmergencyContact,
                                    ProfileImage = @ProfileImage
                                    WHERE UserDetailID = @UserDetailID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, detail).ConfigureAwait(false) > 0;
                }
            }, "UpdateUserDetailAsync");
        }

        /// <summary>
        /// Deletes a user detail record.
        /// </summary>
        /// <param name="userDetailId">User detail identifier.</param>
        public bool DeleteUserDetail(int userDetailId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "DELETE FROM UserDetails WHERE UserDetailID = @UserDetailID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, new { UserDetailID = userDetailId }) > 0;
                }
            }, "DeleteUserDetail");
        }

        /// <summary>
        /// Deletes a user detail record asynchronously.
        /// </summary>
        /// <param name="userDetailId">User detail identifier.</param>
        public Task<bool> DeleteUserDetailAsync(int userDetailId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "DELETE FROM UserDetails WHERE UserDetailID = @UserDetailID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, new { UserDetailID = userDetailId }).ConfigureAwait(false) > 0;
                }
            }, "DeleteUserDetailAsync");
        }

        /// <summary>
        /// Gets all user roles.
        /// </summary>
        public IEnumerable<UserRole> GetAllRoles()
        {
            return ExecuteSafe(() =>
            {
                const string sql = "SELECT * FROM UserRoles ORDER BY RoleName";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Query<UserRole>(sql).ToList();
                }
            }, "GetAllRoles");
        }

        /// <summary>
        /// Gets all user roles asynchronously.
        /// </summary>
        public Task<List<UserRole>> GetAllRolesAsync()
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT * FROM UserRoles ORDER BY RoleName";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var results = await connection.QueryAsync<UserRole>(sql).ConfigureAwait(false);
                    return results.ToList();
                }
            }, "GetAllRolesAsync");
        }

        /// <summary>
        /// Gets a role by identifier.
        /// </summary>
        /// <param name="roleId">Role identifier.</param>
        public UserRole GetRoleById(int roleId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "SELECT * FROM UserRoles WHERE RoleID = @RoleID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.QuerySingleOrDefault<UserRole>(sql, new { RoleID = roleId });
                }
            }, "GetRoleById");
        }

        /// <summary>
        /// Gets a role by identifier asynchronously.
        /// </summary>
        /// <param name="roleId">Role identifier.</param>
        public Task<UserRole> GetRoleByIdAsync(int roleId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT * FROM UserRoles WHERE RoleID = @RoleID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.QuerySingleOrDefaultAsync<UserRole>(sql, new { RoleID = roleId }).ConfigureAwait(false);
                }
            }, "GetRoleByIdAsync");
        }

        /// <summary>
        /// Adds a new role and returns the new identifier.
        /// </summary>
        /// <param name="role">Role entity.</param>
        public int AddRole(UserRole role)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"INSERT INTO UserRoles (RoleName, Description)
                                    VALUES (@RoleName, @Description);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = Db.OpenConnection())
                {
                    return connection.ExecuteScalar<int>(sql, role);
                }
            }, "AddRole");
        }

        /// <summary>
        /// Adds a new role asynchronously and returns the new identifier.
        /// </summary>
        /// <param name="role">Role entity.</param>
        public Task<int> AddRoleAsync(UserRole role)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"INSERT INTO UserRoles (RoleName, Description)
                                    VALUES (@RoleName, @Description);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteScalarAsync<int>(sql, role).ConfigureAwait(false);
                }
            }, "AddRoleAsync");
        }

        /// <summary>
        /// Updates an existing role.
        /// </summary>
        /// <param name="role">Role entity.</param>
        public bool UpdateRole(UserRole role)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"UPDATE UserRoles SET
                                    RoleName = @RoleName,
                                    Description = @Description
                                    WHERE RoleID = @RoleID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, role) > 0;
                }
            }, "UpdateRole");
        }

        /// <summary>
        /// Updates an existing role asynchronously.
        /// </summary>
        /// <param name="role">Role entity.</param>
        public Task<bool> UpdateRoleAsync(UserRole role)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"UPDATE UserRoles SET
                                    RoleName = @RoleName,
                                    Description = @Description
                                    WHERE RoleID = @RoleID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, role).ConfigureAwait(false) > 0;
                }
            }, "UpdateRoleAsync");
        }

        /// <summary>
        /// Deletes a role by identifier.
        /// </summary>
        /// <param name="roleId">Role identifier.</param>
        public bool DeleteRole(int roleId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "DELETE FROM UserRoles WHERE RoleID = @RoleID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, new { RoleID = roleId }) > 0;
                }
            }, "DeleteRole");
        }

        /// <summary>
        /// Deletes a role by identifier asynchronously.
        /// </summary>
        /// <param name="roleId">Role identifier.</param>
        public Task<bool> DeleteRoleAsync(int roleId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "DELETE FROM UserRoles WHERE RoleID = @RoleID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, new { RoleID = roleId }).ConfigureAwait(false) > 0;
                }
            }, "DeleteRoleAsync");
        }

        /// <summary>
        /// Executes a stored procedure and returns typed results.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="procedureName">Procedure name.</param>
        /// <param name="parameters">Procedure parameters.</param>
        public IEnumerable<T> ExecuteStoredProcedure<T>(string procedureName, object parameters = null)
        {
            return ExecuteSafe(() =>
            {
                using (var connection = Db.OpenConnection())
                {
                    return connection.Query<T>(procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure).ToList();
                }
            }, "ExecuteStoredProcedure");
        }

        /// <summary>
        /// Executes a stored procedure asynchronously and returns typed results.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="procedureName">Procedure name.</param>
        /// <param name="parameters">Procedure parameters.</param>
        public Task<List<T>> ExecuteStoredProcedureAsync<T>(string procedureName, object parameters = null)
        {
            return ExecuteSafeAsync(async () =>
            {
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var results = await connection.QueryAsync<T>(procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure).ConfigureAwait(false);
                    return results.ToList();
                }
            }, "ExecuteStoredProcedureAsync");
        }
    }
}
