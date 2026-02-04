using System;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace HospitalManagementSystem.DAL
{
    /// <summary>
    /// Represents a database transaction scope with its own connection.
    /// </summary>
    public sealed class DatabaseTransaction : IDisposable
    {
        private bool _disposed;
        private bool _completed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseTransaction"/> class.
        /// </summary>
        /// <param name="connection">The open database connection.</param>
        /// <param name="transaction">The active transaction.</param>
        public DatabaseTransaction(MySqlConnection connection, MySqlTransaction transaction)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            Transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }

        /// <summary>
        /// Gets the open connection associated with this transaction.
        /// </summary>
        public MySqlConnection Connection { get; }

        /// <summary>
        /// Gets the transaction instance.
        /// </summary>
        public MySqlTransaction Transaction { get; }

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        public void Commit()
        {
            Transaction.Commit();
            _completed = true;
        }

        /// <summary>
        /// Commits the transaction asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await Transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            _completed = true;
        }

        /// <summary>
        /// Rolls back the transaction.
        /// </summary>
        public void Rollback()
        {
            Transaction.Rollback();
            _completed = true;
        }

        /// <summary>
        /// Rolls back the transaction asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            await Transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            _completed = true;
        }

        /// <summary>
        /// Disposes the transaction and its connection.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                if (!_completed)
                {
                    Transaction.Rollback();
                }
            }
            catch
            {
                // Swallow rollback exceptions on dispose.
            }
            finally
            {
                Transaction.Dispose();
                Connection.Dispose();
                _disposed = true;
            }
        }
    }
}
