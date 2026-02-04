using System;
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
    /// Provides CRUD operations for invoices and payments.
    /// </summary>
    public sealed class BillingRepository : RepositoryBase
    {
        /// <summary>
        /// Gets all invoices.
        /// </summary>
        public IEnumerable<Invoice> GetAllInvoices()
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"SELECT i.*, 
                                            CONCAT(p.FirstName, ' ', p.LastName) AS PatientName
                                     FROM Invoices i
                                     LEFT JOIN Patients p ON i.PatientID = p.PatientID
                                     ORDER BY i.InvoiceDate DESC";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Query<Invoice>(sql).ToList();
                }
            }, "GetAllInvoices");
        }

        /// <summary>
        /// Gets all invoices asynchronously.
        /// </summary>
        public Task<List<Invoice>> GetAllInvoicesAsync()
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"SELECT i.*, 
                                            CONCAT(p.FirstName, ' ', p.LastName) AS PatientName
                                     FROM Invoices i
                                     LEFT JOIN Patients p ON i.PatientID = p.PatientID
                                     ORDER BY i.InvoiceDate DESC";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var results = await connection.QueryAsync<Invoice>(sql).ConfigureAwait(false);
                    return results.ToList();
                }
            }, "GetAllInvoicesAsync");
        }

        /// <summary>
        /// Gets an invoice by identifier.
        /// </summary>
        /// <param name="invoiceId">Invoice identifier.</param>
        public Invoice GetInvoiceById(int invoiceId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "SELECT * FROM Invoices WHERE InvoiceID = @InvoiceID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.QuerySingleOrDefault<Invoice>(sql, new { InvoiceID = invoiceId });
                }
            }, "GetInvoiceById");
        }

        /// <summary>
        /// Gets an invoice by identifier asynchronously.
        /// </summary>
        /// <param name="invoiceId">Invoice identifier.</param>
        public Task<Invoice> GetInvoiceByIdAsync(int invoiceId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT * FROM Invoices WHERE InvoiceID = @InvoiceID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.QuerySingleOrDefaultAsync<Invoice>(sql, new { InvoiceID = invoiceId }).ConfigureAwait(false);
                }
            }, "GetInvoiceByIdAsync");
        }

        /// <summary>
        /// Adds a new invoice and returns the new identifier.
        /// </summary>
        /// <param name="invoice">Invoice entity.</param>
        public int AddInvoice(Invoice invoice)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"INSERT INTO Invoices
                                    (InvoiceNumber, PatientID, AppointmentID, InvoiceDate, DueDate, TotalAmount, Discount,
                                     TaxAmount, GrandTotal, Status, CreatedBy, Notes)
                                    VALUES (@InvoiceNumber, @PatientID, @AppointmentID, @InvoiceDate, @DueDate, @TotalAmount,
                                            @Discount, @TaxAmount, @GrandTotal, @Status, @CreatedBy, @Notes);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = Db.OpenConnection())
                {
                    return connection.ExecuteScalar<int>(sql, invoice);
                }
            }, "AddInvoice");
        }

        /// <summary>
        /// Adds a new invoice asynchronously and returns the new identifier.
        /// </summary>
        /// <param name="invoice">Invoice entity.</param>
        public Task<int> AddInvoiceAsync(Invoice invoice)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"INSERT INTO Invoices
                                    (InvoiceNumber, PatientID, AppointmentID, InvoiceDate, DueDate, TotalAmount, Discount,
                                     TaxAmount, GrandTotal, Status, CreatedBy, Notes)
                                    VALUES (@InvoiceNumber, @PatientID, @AppointmentID, @InvoiceDate, @DueDate, @TotalAmount,
                                            @Discount, @TaxAmount, @GrandTotal, @Status, @CreatedBy, @Notes);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteScalarAsync<int>(sql, invoice).ConfigureAwait(false);
                }
            }, "AddInvoiceAsync");
        }

        /// <summary>
        /// Updates an existing invoice.
        /// </summary>
        /// <param name="invoice">Invoice entity.</param>
        public bool UpdateInvoice(Invoice invoice)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"UPDATE Invoices SET
                                    InvoiceNumber = @InvoiceNumber,
                                    PatientID = @PatientID,
                                    AppointmentID = @AppointmentID,
                                    InvoiceDate = @InvoiceDate,
                                    DueDate = @DueDate,
                                    TotalAmount = @TotalAmount,
                                    Discount = @Discount,
                                    TaxAmount = @TaxAmount,
                                    GrandTotal = @GrandTotal,
                                    Status = @Status,
                                    CreatedBy = @CreatedBy,
                                    Notes = @Notes
                                    WHERE InvoiceID = @InvoiceID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, invoice) > 0;
                }
            }, "UpdateInvoice");
        }

        /// <summary>
        /// Updates an existing invoice asynchronously.
        /// </summary>
        /// <param name="invoice">Invoice entity.</param>
        public Task<bool> UpdateInvoiceAsync(Invoice invoice)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"UPDATE Invoices SET
                                    InvoiceNumber = @InvoiceNumber,
                                    PatientID = @PatientID,
                                    AppointmentID = @AppointmentID,
                                    InvoiceDate = @InvoiceDate,
                                    DueDate = @DueDate,
                                    TotalAmount = @TotalAmount,
                                    Discount = @Discount,
                                    TaxAmount = @TaxAmount,
                                    GrandTotal = @GrandTotal,
                                    Status = @Status,
                                    CreatedBy = @CreatedBy,
                                    Notes = @Notes
                                    WHERE InvoiceID = @InvoiceID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, invoice).ConfigureAwait(false) > 0;
                }
            }, "UpdateInvoiceAsync");
        }

        /// <summary>
        /// Deletes an invoice by identifier.
        /// </summary>
        /// <param name="invoiceId">Invoice identifier.</param>
        public bool DeleteInvoice(int invoiceId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "DELETE FROM Invoices WHERE InvoiceID = @InvoiceID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, new { InvoiceID = invoiceId }) > 0;
                }
            }, "DeleteInvoice");
        }

        /// <summary>
        /// Deletes an invoice by identifier asynchronously.
        /// </summary>
        /// <param name="invoiceId">Invoice identifier.</param>
        public Task<bool> DeleteInvoiceAsync(int invoiceId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "DELETE FROM Invoices WHERE InvoiceID = @InvoiceID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, new { InvoiceID = invoiceId }).ConfigureAwait(false) > 0;
                }
            }, "DeleteInvoiceAsync");
        }

        /// <summary>
        /// Searches invoices with filters and pagination.
        /// </summary>
        public Task<PagedResult<Invoice>> SearchInvoicesAsync(
            int? patientId,
            string status,
            DateTime? dateFrom,
            DateTime? dateTo,
            int pageNumber,
            int pageSize)
        {
            return ExecuteSafeAsync(async () =>
            {
                var sql = new StringBuilder("SELECT * FROM Invoices WHERE 1=1 ");
                var countSql = new StringBuilder("SELECT COUNT(*) FROM Invoices WHERE 1=1 ");
                var parameters = new DynamicParameters();

                if (patientId.HasValue)
                {
                    sql.Append(" AND PatientID = @PatientID ");
                    countSql.Append(" AND PatientID = @PatientID ");
                    parameters.Add("@PatientID", patientId.Value);
                }

                if (!string.IsNullOrWhiteSpace(status))
                {
                    sql.Append(" AND Status = @Status ");
                    countSql.Append(" AND Status = @Status ");
                    parameters.Add("@Status", status);
                }

                if (dateFrom.HasValue)
                {
                    sql.Append(" AND InvoiceDate >= @DateFrom ");
                    countSql.Append(" AND InvoiceDate >= @DateFrom ");
                    parameters.Add("@DateFrom", dateFrom.Value);
                }

                if (dateTo.HasValue)
                {
                    sql.Append(" AND InvoiceDate <= @DateTo ");
                    countSql.Append(" AND InvoiceDate <= @DateTo ");
                    parameters.Add("@DateTo", dateTo.Value);
                }

                sql.Append(" ORDER BY InvoiceDate DESC ");
                AddPagination(sql, parameters, pageNumber, pageSize);

                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var totalCount = await connection.ExecuteScalarAsync<int>(countSql.ToString(), parameters).ConfigureAwait(false);
                    var items = await connection.QueryAsync<Invoice>(sql.ToString(), parameters).ConfigureAwait(false);

                    return new PagedResult<Invoice>
                    {
                        Items = items.ToList(),
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    };
                }
            }, "SearchInvoicesAsync");
        }

        /// <summary>
        /// Gets invoice details for an invoice.
        /// </summary>
        /// <param name="invoiceId">Invoice identifier.</param>
        public IEnumerable<InvoiceDetail> GetInvoiceDetails(int invoiceId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "SELECT * FROM InvoiceDetails WHERE InvoiceID = @InvoiceID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Query<InvoiceDetail>(sql, new { InvoiceID = invoiceId }).ToList();
                }
            }, "GetInvoiceDetails");
        }

        /// <summary>
        /// Gets invoice details asynchronously.
        /// </summary>
        /// <param name="invoiceId">Invoice identifier.</param>
        public Task<List<InvoiceDetail>> GetInvoiceDetailsAsync(int invoiceId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT * FROM InvoiceDetails WHERE InvoiceID = @InvoiceID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var results = await connection.QueryAsync<InvoiceDetail>(sql, new { InvoiceID = invoiceId }).ConfigureAwait(false);
                    return results.ToList();
                }
            }, "GetInvoiceDetailsAsync");
        }

        /// <summary>
        /// Adds an invoice detail and returns the new identifier.
        /// </summary>
        /// <param name="detail">Invoice detail entity.</param>
        public int AddInvoiceDetail(InvoiceDetail detail)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"INSERT INTO InvoiceDetails
                                    (InvoiceID, ServiceID, Quantity, UnitPrice)
                                    VALUES (@InvoiceID, @ServiceID, @Quantity, @UnitPrice);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = Db.OpenConnection())
                {
                    return connection.ExecuteScalar<int>(sql, detail);
                }
            }, "AddInvoiceDetail");
        }

        /// <summary>
        /// Adds an invoice detail asynchronously and returns the new identifier.
        /// </summary>
        /// <param name="detail">Invoice detail entity.</param>
        public Task<int> AddInvoiceDetailAsync(InvoiceDetail detail)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"INSERT INTO InvoiceDetails
                                    (InvoiceID, ServiceID, Quantity, UnitPrice)
                                    VALUES (@InvoiceID, @ServiceID, @Quantity, @UnitPrice);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteScalarAsync<int>(sql, detail).ConfigureAwait(false);
                }
            }, "AddInvoiceDetailAsync");
        }

        /// <summary>
        /// Deletes an invoice detail by identifier.
        /// </summary>
        /// <param name="detailId">Detail identifier.</param>
        public bool DeleteInvoiceDetail(int detailId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "DELETE FROM InvoiceDetails WHERE DetailID = @DetailID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, new { DetailID = detailId }) > 0;
                }
            }, "DeleteInvoiceDetail");
        }

        /// <summary>
        /// Deletes an invoice detail by identifier asynchronously.
        /// </summary>
        /// <param name="detailId">Detail identifier.</param>
        public Task<bool> DeleteInvoiceDetailAsync(int detailId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "DELETE FROM InvoiceDetails WHERE DetailID = @DetailID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, new { DetailID = detailId }).ConfigureAwait(false) > 0;
                }
            }, "DeleteInvoiceDetailAsync");
        }

        /// <summary>
        /// Gets all payments.
        /// </summary>
        public IEnumerable<Payment> GetAllPayments()
        {
            return ExecuteSafe(() =>
            {
                const string sql = "SELECT * FROM Payments ORDER BY PaymentDate DESC";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Query<Payment>(sql).ToList();
                }
            }, "GetAllPayments");
        }

        /// <summary>
        /// Gets all payments asynchronously.
        /// </summary>
        public Task<List<Payment>> GetAllPaymentsAsync()
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT * FROM Payments ORDER BY PaymentDate DESC";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var results = await connection.QueryAsync<Payment>(sql).ConfigureAwait(false);
                    return results.ToList();
                }
            }, "GetAllPaymentsAsync");
        }

        /// <summary>
        /// Gets a payment by identifier.
        /// </summary>
        /// <param name="paymentId">Payment identifier.</param>
        public Payment GetPaymentById(int paymentId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "SELECT * FROM Payments WHERE PaymentID = @PaymentID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.QuerySingleOrDefault<Payment>(sql, new { PaymentID = paymentId });
                }
            }, "GetPaymentById");
        }

        /// <summary>
        /// Gets a payment by identifier asynchronously.
        /// </summary>
        /// <param name="paymentId">Payment identifier.</param>
        public Task<Payment> GetPaymentByIdAsync(int paymentId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT * FROM Payments WHERE PaymentID = @PaymentID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.QuerySingleOrDefaultAsync<Payment>(sql, new { PaymentID = paymentId }).ConfigureAwait(false);
                }
            }, "GetPaymentByIdAsync");
        }

        /// <summary>
        /// Adds a new payment and returns the new identifier.
        /// </summary>
        /// <param name="payment">Payment entity.</param>
        public int AddPayment(Payment payment)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"INSERT INTO Payments
                                    (PaymentNumber, InvoiceID, PaymentDate, PaymentMethod, Amount, ReferenceNumber, ReceivedBy, Notes)
                                    VALUES (@PaymentNumber, @InvoiceID, @PaymentDate, @PaymentMethod, @Amount, @ReferenceNumber, @ReceivedBy, @Notes);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = Db.OpenConnection())
                {
                    return connection.ExecuteScalar<int>(sql, payment);
                }
            }, "AddPayment");
        }

        /// <summary>
        /// Adds a new payment asynchronously and returns the new identifier.
        /// </summary>
        /// <param name="payment">Payment entity.</param>
        public Task<int> AddPaymentAsync(Payment payment)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"INSERT INTO Payments
                                    (PaymentNumber, InvoiceID, PaymentDate, PaymentMethod, Amount, ReferenceNumber, ReceivedBy, Notes)
                                    VALUES (@PaymentNumber, @InvoiceID, @PaymentDate, @PaymentMethod, @Amount, @ReferenceNumber, @ReceivedBy, @Notes);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteScalarAsync<int>(sql, payment).ConfigureAwait(false);
                }
            }, "AddPaymentAsync");
        }

        /// <summary>
        /// Updates an existing payment.
        /// </summary>
        /// <param name="payment">Payment entity.</param>
        public bool UpdatePayment(Payment payment)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"UPDATE Payments SET
                                    PaymentNumber = @PaymentNumber,
                                    InvoiceID = @InvoiceID,
                                    PaymentDate = @PaymentDate,
                                    PaymentMethod = @PaymentMethod,
                                    Amount = @Amount,
                                    ReferenceNumber = @ReferenceNumber,
                                    ReceivedBy = @ReceivedBy,
                                    Notes = @Notes
                                    WHERE PaymentID = @PaymentID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, payment) > 0;
                }
            }, "UpdatePayment");
        }

        /// <summary>
        /// Updates an existing payment asynchronously.
        /// </summary>
        /// <param name="payment">Payment entity.</param>
        public Task<bool> UpdatePaymentAsync(Payment payment)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"UPDATE Payments SET
                                    PaymentNumber = @PaymentNumber,
                                    InvoiceID = @InvoiceID,
                                    PaymentDate = @PaymentDate,
                                    PaymentMethod = @PaymentMethod,
                                    Amount = @Amount,
                                    ReferenceNumber = @ReferenceNumber,
                                    ReceivedBy = @ReceivedBy,
                                    Notes = @Notes
                                    WHERE PaymentID = @PaymentID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, payment).ConfigureAwait(false) > 0;
                }
            }, "UpdatePaymentAsync");
        }

        /// <summary>
        /// Deletes a payment by identifier.
        /// </summary>
        /// <param name="paymentId">Payment identifier.</param>
        public bool DeletePayment(int paymentId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "DELETE FROM Payments WHERE PaymentID = @PaymentID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, new { PaymentID = paymentId }) > 0;
                }
            }, "DeletePayment");
        }

        /// <summary>
        /// Deletes a payment by identifier asynchronously.
        /// </summary>
        /// <param name="paymentId">Payment identifier.</param>
        public Task<bool> DeletePaymentAsync(int paymentId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "DELETE FROM Payments WHERE PaymentID = @PaymentID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, new { PaymentID = paymentId }).ConfigureAwait(false) > 0;
                }
            }, "DeletePaymentAsync");
        }

        /// <summary>
        /// Executes the GenerateInvoice stored procedure.
        /// </summary>
        public Task<GenerateInvoiceResult> GenerateInvoiceAsync(int patientId, int? appointmentId, int createdBy)
        {
            return ExecuteSafeAsync(async () =>
            {
                var parameters = new
                {
                    pPatientID = patientId,
                    pAppointmentID = appointmentId,
                    pCreatedBy = createdBy
                };

                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.QuerySingleAsync<GenerateInvoiceResult>(
                        "GenerateInvoice",
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure).ConfigureAwait(false);
                }
            }, "GenerateInvoiceAsync");
        }

        /// <summary>
        /// Executes the ProcessPayment stored procedure.
        /// </summary>
        public Task<ProcessPaymentResult> ProcessPaymentAsync(
            int invoiceId,
            string paymentMethod,
            decimal amount,
            string referenceNumber,
            int receivedBy)
        {
            return ExecuteSafeAsync(async () =>
            {
                var parameters = new
                {
                    pInvoiceID = invoiceId,
                    pPaymentMethod = paymentMethod,
                    pAmount = amount,
                    pReferenceNumber = referenceNumber,
                    pReceivedBy = receivedBy
                };

                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.QuerySingleAsync<ProcessPaymentResult>(
                        "ProcessPayment",
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure).ConfigureAwait(false);
                }
            }, "ProcessPaymentAsync");
        }

        /// <summary>
        /// Gets the outstanding balance for a patient using the database function.
        /// </summary>
        /// <param name="patientId">Patient identifier.</param>
        public Task<decimal> GetPatientBalanceAsync(int patientId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT GetPatientBalance(@PatientID)";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteScalarAsync<decimal>(sql, new { PatientID = patientId }).ConfigureAwait(false);
                }
            }, "GetPatientBalanceAsync");
        }

        /// <summary>
        /// Executes a stored procedure and returns typed results.
        /// </summary>
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
