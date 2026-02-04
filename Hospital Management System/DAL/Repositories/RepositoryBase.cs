using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace HospitalManagementSystem.DAL.Repositories
{
    /// <summary>
    /// Provides shared repository helpers.
    /// </summary>
    public abstract class RepositoryBase
    {
        /// <summary>
        /// Gets the database connection helper.
        /// </summary>
        protected DatabaseConnection Db => DatabaseConnection.Instance;

        /// <summary>
        /// Executes an action with standardized error handling.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="action">Action to execute.</param>
        /// <param name="operation">Operation description.</param>
        /// <returns>Result of the action.</returns>
        protected T ExecuteSafe<T>(Func<T> action, string operation)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                Trace.TraceError("{0} failed: {1}", operation, ex);
                throw new DataAccessException($"{operation} failed.", ex);
            }
        }

        /// <summary>
        /// Executes an asynchronous action with standardized error handling.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="action">Action to execute.</param>
        /// <param name="operation">Operation description.</param>
        /// <returns>Result of the action.</returns>
        protected async Task<T> ExecuteSafeAsync<T>(Func<Task<T>> action, string operation)
        {
            try
            {
                return await action().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Trace.TraceError("{0} failed: {1}", operation, ex);
                throw new DataAccessException($"{operation} failed.", ex);
            }
        }

        /// <summary>
        /// Executes an asynchronous action with standardized error handling.
        /// </summary>
        /// <param name="action">Action to execute.</param>
        /// <param name="operation">Operation description.</param>
        protected async Task ExecuteSafeAsync(Func<Task> action, string operation)
        {
            try
            {
                await action().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Trace.TraceError("{0} failed: {1}", operation, ex);
                throw new DataAccessException($"{operation} failed.", ex);
            }
        }

        /// <summary>
        /// Adds pagination to a SQL query.
        /// </summary>
        /// <param name="sql">SQL builder.</param>
        /// <param name="parameters">Dapper parameters.</param>
        /// <param name="pageNumber">Page number (1-based).</param>
        /// <param name="pageSize">Page size.</param>
        protected static void AddPagination(StringBuilder sql, DynamicParameters parameters, int pageNumber, int pageSize)
        {
            var safePageNumber = pageNumber < 1 ? 1 : pageNumber;
            var safePageSize = pageSize < 1 ? 50 : pageSize;
            var offset = (safePageNumber - 1) * safePageSize;

            sql.Append(" LIMIT @Offset, @PageSize ");
            parameters.Add("@Offset", offset);
            parameters.Add("@PageSize", safePageSize);
        }
    }
}
