using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace HospitalManagementSystem.DAL
{
    /// <summary>
    /// Provides database connection and execution helpers.
    /// </summary>
    public sealed class DatabaseConnection
    {
        private static readonly Lazy<DatabaseConnection> InstanceValue =
            new Lazy<DatabaseConnection>(() => new DatabaseConnection());

        private readonly string _connectionString;

        private DatabaseConnection()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["HospitalDB"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new InvalidOperationException("HospitalDB connection string is missing in App.config.");
            }
        }

        /// <summary>
        /// Gets the singleton instance of <see cref="DatabaseConnection"/>.
        /// </summary>
        public static DatabaseConnection Instance => InstanceValue.Value;

        /// <summary>
        /// Opens and returns a new MySQL connection.
        /// </summary>
        /// <returns>An open <see cref="MySqlConnection"/>.</returns>
        public MySqlConnection OpenConnection()
        {
            try
            {
                var connection = new MySqlConnection(_connectionString);
                connection.Open();
                return connection;
            }
            catch (Exception ex)
            {
                Trace.TraceError("OpenConnection failed: {0}", ex);
                throw new DataAccessException("Failed to open database connection.", ex);
            }
        }

        /// <summary>
        /// Opens and returns a new MySQL connection asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An open <see cref="MySqlConnection"/>.</returns>
        public async Task<MySqlConnection> OpenConnectionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                return connection;
            }
            catch (Exception ex)
            {
                Trace.TraceError("OpenConnectionAsync failed: {0}", ex);
                throw new DataAccessException("Failed to open database connection.", ex);
            }
        }

        /// <summary>
        /// Closes and disposes the provided connection.
        /// </summary>
        /// <param name="connection">The connection to close.</param>
        public void CloseConnection(MySqlConnection connection)
        {
            if (connection == null)
            {
                return;
            }

            try
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("CloseConnection failed: {0}", ex);
                throw new DataAccessException("Failed to close database connection.", ex);
            }
            finally
            {
                connection.Dispose();
            }
        }

        /// <summary>
        /// Closes and disposes the provided connection asynchronously.
        /// </summary>
        /// <param name="connection">The connection to close.</param>
        public async Task CloseConnectionAsync(MySqlConnection connection)
        {
            if (connection == null)
            {
                return;
            }

            try
            {
                if (connection.State != ConnectionState.Closed)
                {
                    await connection.CloseAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("CloseConnectionAsync failed: {0}", ex);
                throw new DataAccessException("Failed to close database connection.", ex);
            }
            finally
            {
                connection.Dispose();
            }
        }

        /// <summary>
        /// Executes a query and returns the result as a <see cref="DataTable"/>.
        /// </summary>
        /// <param name="sql">SQL query.</param>
        /// <param name="parameters">Query parameters.</param>
        /// <param name="transaction">Optional transaction.</param>
        /// <returns>Query results.</returns>
        public DataTable ExecuteQuery(string sql, IDictionary<string, object> parameters = null, MySqlTransaction transaction = null)
        {
            MySqlConnection connection = null;
            var shouldClose = transaction == null;
            try
            {
                connection = shouldClose ? OpenConnection() : transaction.Connection;
                using (var command = new MySqlCommand(sql, connection, transaction))
                {
                    AddParameters(command, parameters);
                    using (var reader = command.ExecuteReader())
                    {
                        var table = new DataTable();
                        table.Load(reader);
                        return table;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("ExecuteQuery failed: {0}", ex);
                throw new DataAccessException("Failed to execute query.", ex);
            }
            finally
            {
                if (shouldClose)
                {
                    CloseConnection(connection);
                }
            }
        }

        /// <summary>
        /// Executes a query asynchronously and returns the result as a <see cref="DataTable"/>.
        /// </summary>
        /// <param name="sql">SQL query.</param>
        /// <param name="parameters">Query parameters.</param>
        /// <param name="transaction">Optional transaction.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Query results.</returns>
        public async Task<DataTable> ExecuteQueryAsync(
            string sql,
            IDictionary<string, object> parameters = null,
            MySqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            MySqlConnection connection = null;
            var shouldClose = transaction == null;
            try
            {
                connection = shouldClose
                    ? await OpenConnectionAsync(cancellationToken).ConfigureAwait(false)
                    : transaction.Connection;
                using (var command = new MySqlCommand(sql, connection, transaction))
                {
                    AddParameters(command, parameters);
                    using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                    {
                        var table = new DataTable();
                        table.Load(reader);
                        return table;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("ExecuteQueryAsync failed: {0}", ex);
                throw new DataAccessException("Failed to execute query.", ex);
            }
            finally
            {
                if (shouldClose)
                {
                    await CloseConnectionAsync(connection).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Executes a non-query command and returns the affected row count.
        /// </summary>
        /// <param name="sql">SQL command.</param>
        /// <param name="parameters">Command parameters.</param>
        /// <param name="transaction">Optional transaction.</param>
        /// <returns>Number of affected rows.</returns>
        public int ExecuteNonQuery(string sql, IDictionary<string, object> parameters = null, MySqlTransaction transaction = null)
        {
            MySqlConnection connection = null;
            var shouldClose = transaction == null;
            try
            {
                connection = shouldClose ? OpenConnection() : transaction.Connection;
                using (var command = new MySqlCommand(sql, connection, transaction))
                {
                    AddParameters(command, parameters);
                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("ExecuteNonQuery failed: {0}", ex);
                throw new DataAccessException("Failed to execute non-query.", ex);
            }
            finally
            {
                if (shouldClose)
                {
                    CloseConnection(connection);
                }
            }
        }

        /// <summary>
        /// Executes a non-query command asynchronously and returns the affected row count.
        /// </summary>
        /// <param name="sql">SQL command.</param>
        /// <param name="parameters">Command parameters.</param>
        /// <param name="transaction">Optional transaction.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Number of affected rows.</returns>
        public async Task<int> ExecuteNonQueryAsync(
            string sql,
            IDictionary<string, object> parameters = null,
            MySqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            MySqlConnection connection = null;
            var shouldClose = transaction == null;
            try
            {
                connection = shouldClose
                    ? await OpenConnectionAsync(cancellationToken).ConfigureAwait(false)
                    : transaction.Connection;
                using (var command = new MySqlCommand(sql, connection, transaction))
                {
                    AddParameters(command, parameters);
                    return await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("ExecuteNonQueryAsync failed: {0}", ex);
                throw new DataAccessException("Failed to execute non-query.", ex);
            }
            finally
            {
                if (shouldClose)
                {
                    await CloseConnectionAsync(connection).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Executes a scalar query and returns the first column of the first row.
        /// </summary>
        /// <param name="sql">SQL query.</param>
        /// <param name="parameters">Query parameters.</param>
        /// <param name="transaction">Optional transaction.</param>
        /// <returns>Scalar value.</returns>
        public object ExecuteScalar(string sql, IDictionary<string, object> parameters = null, MySqlTransaction transaction = null)
        {
            MySqlConnection connection = null;
            var shouldClose = transaction == null;
            try
            {
                connection = shouldClose ? OpenConnection() : transaction.Connection;
                using (var command = new MySqlCommand(sql, connection, transaction))
                {
                    AddParameters(command, parameters);
                    return command.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("ExecuteScalar failed: {0}", ex);
                throw new DataAccessException("Failed to execute scalar query.", ex);
            }
            finally
            {
                if (shouldClose)
                {
                    CloseConnection(connection);
                }
            }
        }

        /// <summary>
        /// Executes a scalar query asynchronously and returns the first column of the first row.
        /// </summary>
        /// <param name="sql">SQL query.</param>
        /// <param name="parameters">Query parameters.</param>
        /// <param name="transaction">Optional transaction.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Scalar value.</returns>
        public async Task<object> ExecuteScalarAsync(
            string sql,
            IDictionary<string, object> parameters = null,
            MySqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            MySqlConnection connection = null;
            var shouldClose = transaction == null;
            try
            {
                connection = shouldClose
                    ? await OpenConnectionAsync(cancellationToken).ConfigureAwait(false)
                    : transaction.Connection;
                using (var command = new MySqlCommand(sql, connection, transaction))
                {
                    AddParameters(command, parameters);
                    return await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("ExecuteScalarAsync failed: {0}", ex);
                throw new DataAccessException("Failed to execute scalar query.", ex);
            }
            finally
            {
                if (shouldClose)
                {
                    await CloseConnectionAsync(connection).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Begins a new transaction with its own connection.
        /// </summary>
        /// <param name="isolationLevel">Isolation level.</param>
        /// <returns>The transaction scope.</returns>
        public DatabaseTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            try
            {
                var connection = OpenConnection();
                var transaction = connection.BeginTransaction(isolationLevel);
                return new DatabaseTransaction(connection, transaction);
            }
            catch (Exception ex)
            {
                Trace.TraceError("BeginTransaction failed: {0}", ex);
                throw new DataAccessException("Failed to begin transaction.", ex);
            }
        }

        /// <summary>
        /// Begins a new transaction asynchronously with its own connection.
        /// </summary>
        /// <param name="isolationLevel">Isolation level.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The transaction scope.</returns>
        public async Task<DatabaseTransaction> BeginTransactionAsync(
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var connection = await OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                var transaction = await connection.BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(false);
                return new DatabaseTransaction(connection, (MySqlTransaction)transaction);
            }
            catch (Exception ex)
            {
                Trace.TraceError("BeginTransactionAsync failed: {0}", ex);
                throw new DataAccessException("Failed to begin transaction.", ex);
            }
        }

        private static void AddParameters(MySqlCommand command, IDictionary<string, object> parameters)
        {
            if (parameters == null)
            {
                return;
            }

            foreach (var pair in parameters)
            {
                var value = pair.Value ?? DBNull.Value;
                command.Parameters.AddWithValue(pair.Key, value);
            }
        }
    }
}
