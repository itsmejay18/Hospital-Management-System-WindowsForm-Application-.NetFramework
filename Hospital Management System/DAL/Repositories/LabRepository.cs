using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.DAL.Repositories
{
    /// <summary>
    /// Provides CRUD operations for lab tests and lab orders.
    /// </summary>
    public sealed class LabRepository : RepositoryBase
    {
        /// <summary>
        /// Gets all lab tests.
        /// </summary>
        public IEnumerable<LabTest> GetAllLabTests()
        {
            return ExecuteSafe(() =>
            {
                const string sql = "SELECT * FROM LabTests ORDER BY TestName";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Query<LabTest>(sql).ToList();
                }
            }, "GetAllLabTests");
        }

        /// <summary>
        /// Gets all lab tests asynchronously.
        /// </summary>
        public Task<List<LabTest>> GetAllLabTestsAsync()
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT * FROM LabTests ORDER BY TestName";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var results = await connection.QueryAsync<LabTest>(sql).ConfigureAwait(false);
                    return results.ToList();
                }
            }, "GetAllLabTestsAsync");
        }

        /// <summary>
        /// Gets a lab test by identifier.
        /// </summary>
        /// <param name="testId">Test identifier.</param>
        public LabTest GetLabTestById(int testId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "SELECT * FROM LabTests WHERE TestID = @TestID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.QuerySingleOrDefault<LabTest>(sql, new { TestID = testId });
                }
            }, "GetLabTestById");
        }

        /// <summary>
        /// Gets a lab test by identifier asynchronously.
        /// </summary>
        /// <param name="testId">Test identifier.</param>
        public Task<LabTest> GetLabTestByIdAsync(int testId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT * FROM LabTests WHERE TestID = @TestID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.QuerySingleOrDefaultAsync<LabTest>(sql, new { TestID = testId }).ConfigureAwait(false);
                }
            }, "GetLabTestByIdAsync");
        }

        /// <summary>
        /// Adds a lab test and returns the new identifier.
        /// </summary>
        public int AddLabTest(LabTest test)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"INSERT INTO LabTests
                                    (TestCode, TestName, Category, NormalRange, Unit, Price)
                                    VALUES (@TestCode, @TestName, @Category, @NormalRange, @Unit, @Price);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = Db.OpenConnection())
                {
                    return connection.ExecuteScalar<int>(sql, test);
                }
            }, "AddLabTest");
        }

        /// <summary>
        /// Adds a lab test asynchronously and returns the new identifier.
        /// </summary>
        public Task<int> AddLabTestAsync(LabTest test)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"INSERT INTO LabTests
                                    (TestCode, TestName, Category, NormalRange, Unit, Price)
                                    VALUES (@TestCode, @TestName, @Category, @NormalRange, @Unit, @Price);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteScalarAsync<int>(sql, test).ConfigureAwait(false);
                }
            }, "AddLabTestAsync");
        }

        /// <summary>
        /// Updates a lab test.
        /// </summary>
        public bool UpdateLabTest(LabTest test)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"UPDATE LabTests SET
                                    TestCode = @TestCode,
                                    TestName = @TestName,
                                    Category = @Category,
                                    NormalRange = @NormalRange,
                                    Unit = @Unit,
                                    Price = @Price
                                    WHERE TestID = @TestID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, test) > 0;
                }
            }, "UpdateLabTest");
        }

        /// <summary>
        /// Updates a lab test asynchronously.
        /// </summary>
        public Task<bool> UpdateLabTestAsync(LabTest test)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"UPDATE LabTests SET
                                    TestCode = @TestCode,
                                    TestName = @TestName,
                                    Category = @Category,
                                    NormalRange = @NormalRange,
                                    Unit = @Unit,
                                    Price = @Price
                                    WHERE TestID = @TestID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, test).ConfigureAwait(false) > 0;
                }
            }, "UpdateLabTestAsync");
        }

        /// <summary>
        /// Deletes a lab test by identifier.
        /// </summary>
        public bool DeleteLabTest(int testId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "DELETE FROM LabTests WHERE TestID = @TestID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, new { TestID = testId }) > 0;
                }
            }, "DeleteLabTest");
        }

        /// <summary>
        /// Deletes a lab test by identifier asynchronously.
        /// </summary>
        public Task<bool> DeleteLabTestAsync(int testId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "DELETE FROM LabTests WHERE TestID = @TestID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, new { TestID = testId }).ConfigureAwait(false) > 0;
                }
            }, "DeleteLabTestAsync");
        }

        /// <summary>
        /// Searches lab tests with filters and pagination.
        /// </summary>
        public Task<PagedResult<LabTest>> SearchLabTestsAsync(
            string testCode,
            string testName,
            string category,
            int pageNumber,
            int pageSize)
        {
            return ExecuteSafeAsync(async () =>
            {
                var sql = new StringBuilder("SELECT * FROM LabTests WHERE 1=1 ");
                var countSql = new StringBuilder("SELECT COUNT(*) FROM LabTests WHERE 1=1 ");
                var parameters = new DynamicParameters();

                if (!string.IsNullOrWhiteSpace(testCode))
                {
                    sql.Append(" AND TestCode LIKE @TestCode ");
                    countSql.Append(" AND TestCode LIKE @TestCode ");
                    parameters.Add("@TestCode", $"%{testCode}%");
                }

                if (!string.IsNullOrWhiteSpace(testName))
                {
                    sql.Append(" AND TestName LIKE @TestName ");
                    countSql.Append(" AND TestName LIKE @TestName ");
                    parameters.Add("@TestName", $"%{testName}%");
                }

                if (!string.IsNullOrWhiteSpace(category))
                {
                    sql.Append(" AND Category LIKE @Category ");
                    countSql.Append(" AND Category LIKE @Category ");
                    parameters.Add("@Category", $"%{category}%");
                }

                sql.Append(" ORDER BY TestName ");
                AddPagination(sql, parameters, pageNumber, pageSize);

                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var totalCount = await connection.ExecuteScalarAsync<int>(countSql.ToString(), parameters).ConfigureAwait(false);
                    var items = await connection.QueryAsync<LabTest>(sql.ToString(), parameters).ConfigureAwait(false);

                    return new PagedResult<LabTest>
                    {
                        Items = items.ToList(),
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    };
                }
            }, "SearchLabTestsAsync");
        }

        /// <summary>
        /// Gets lab orders.
        /// </summary>
        public IEnumerable<LabOrder> GetLabOrders()
        {
            return ExecuteSafe(() =>
            {
                const string sql = "SELECT * FROM LabOrders ORDER BY OrderDate DESC";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Query<LabOrder>(sql).ToList();
                }
            }, "GetLabOrders");
        }

        /// <summary>
        /// Gets lab orders asynchronously.
        /// </summary>
        public Task<List<LabOrder>> GetLabOrdersAsync()
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT * FROM LabOrders ORDER BY OrderDate DESC";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var results = await connection.QueryAsync<LabOrder>(sql).ConfigureAwait(false);
                    return results.ToList();
                }
            }, "GetLabOrdersAsync");
        }

        /// <summary>
        /// Adds a lab order and returns the new identifier.
        /// </summary>
        public int AddLabOrder(LabOrder order)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"INSERT INTO LabOrders
                                    (OrderCode, VisitID, PatientID, DoctorID, OrderDate, Status, ResultDate, Notes)
                                    VALUES (@OrderCode, @VisitID, @PatientID, @DoctorID, @OrderDate, @Status, @ResultDate, @Notes);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = Db.OpenConnection())
                {
                    return connection.ExecuteScalar<int>(sql, order);
                }
            }, "AddLabOrder");
        }

        /// <summary>
        /// Adds a lab order asynchronously and returns the new identifier.
        /// </summary>
        public Task<int> AddLabOrderAsync(LabOrder order)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"INSERT INTO LabOrders
                                    (OrderCode, VisitID, PatientID, DoctorID, OrderDate, Status, ResultDate, Notes)
                                    VALUES (@OrderCode, @VisitID, @PatientID, @DoctorID, @OrderDate, @Status, @ResultDate, @Notes);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteScalarAsync<int>(sql, order).ConfigureAwait(false);
                }
            }, "AddLabOrderAsync");
        }

        /// <summary>
        /// Updates a lab order.
        /// </summary>
        public bool UpdateLabOrder(LabOrder order)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"UPDATE LabOrders SET
                                    OrderCode = @OrderCode,
                                    VisitID = @VisitID,
                                    PatientID = @PatientID,
                                    DoctorID = @DoctorID,
                                    OrderDate = @OrderDate,
                                    Status = @Status,
                                    ResultDate = @ResultDate,
                                    Notes = @Notes
                                    WHERE OrderID = @OrderID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, order) > 0;
                }
            }, "UpdateLabOrder");
        }

        /// <summary>
        /// Updates a lab order asynchronously.
        /// </summary>
        public Task<bool> UpdateLabOrderAsync(LabOrder order)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"UPDATE LabOrders SET
                                    OrderCode = @OrderCode,
                                    VisitID = @VisitID,
                                    PatientID = @PatientID,
                                    DoctorID = @DoctorID,
                                    OrderDate = @OrderDate,
                                    Status = @Status,
                                    ResultDate = @ResultDate,
                                    Notes = @Notes
                                    WHERE OrderID = @OrderID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, order).ConfigureAwait(false) > 0;
                }
            }, "UpdateLabOrderAsync");
        }

        /// <summary>
        /// Deletes a lab order by identifier.
        /// </summary>
        public bool DeleteLabOrder(int orderId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "DELETE FROM LabOrders WHERE OrderID = @OrderID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, new { OrderID = orderId }) > 0;
                }
            }, "DeleteLabOrder");
        }

        /// <summary>
        /// Deletes a lab order by identifier asynchronously.
        /// </summary>
        public Task<bool> DeleteLabOrderAsync(int orderId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "DELETE FROM LabOrders WHERE OrderID = @OrderID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, new { OrderID = orderId }).ConfigureAwait(false) > 0;
                }
            }, "DeleteLabOrderAsync");
        }

        /// <summary>
        /// Gets lab order details.
        /// </summary>
        public IEnumerable<LabOrderDetail> GetLabOrderDetails(int orderId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "SELECT * FROM LabOrderDetails WHERE OrderID = @OrderID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Query<LabOrderDetail>(sql, new { OrderID = orderId }).ToList();
                }
            }, "GetLabOrderDetails");
        }

        /// <summary>
        /// Gets lab order details asynchronously.
        /// </summary>
        public Task<List<LabOrderDetail>> GetLabOrderDetailsAsync(int orderId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT * FROM LabOrderDetails WHERE OrderID = @OrderID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var results = await connection.QueryAsync<LabOrderDetail>(sql, new { OrderID = orderId }).ConfigureAwait(false);
                    return results.ToList();
                }
            }, "GetLabOrderDetailsAsync");
        }

        /// <summary>
        /// Adds a lab order detail and returns the new identifier.
        /// </summary>
        public int AddLabOrderDetail(LabOrderDetail detail)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"INSERT INTO LabOrderDetails
                                    (OrderID, TestID, ResultValue, ResultUnit, NormalRange, IsNormal, Notes, TechnicianID, CompletedDate)
                                    VALUES (@OrderID, @TestID, @ResultValue, @ResultUnit, @NormalRange, @IsNormal, @Notes, @TechnicianID, @CompletedDate);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = Db.OpenConnection())
                {
                    return connection.ExecuteScalar<int>(sql, detail);
                }
            }, "AddLabOrderDetail");
        }

        /// <summary>
        /// Adds a lab order detail asynchronously and returns the new identifier.
        /// </summary>
        public Task<int> AddLabOrderDetailAsync(LabOrderDetail detail)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"INSERT INTO LabOrderDetails
                                    (OrderID, TestID, ResultValue, ResultUnit, NormalRange, IsNormal, Notes, TechnicianID, CompletedDate)
                                    VALUES (@OrderID, @TestID, @ResultValue, @ResultUnit, @NormalRange, @IsNormal, @Notes, @TechnicianID, @CompletedDate);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteScalarAsync<int>(sql, detail).ConfigureAwait(false);
                }
            }, "AddLabOrderDetailAsync");
        }

        /// <summary>
        /// Deletes a lab order detail.
        /// </summary>
        public bool DeleteLabOrderDetail(int detailId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "DELETE FROM LabOrderDetails WHERE OrderDetailID = @OrderDetailID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, new { OrderDetailID = detailId }) > 0;
                }
            }, "DeleteLabOrderDetail");
        }

        /// <summary>
        /// Deletes a lab order detail asynchronously.
        /// </summary>
        public Task<bool> DeleteLabOrderDetailAsync(int detailId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "DELETE FROM LabOrderDetails WHERE OrderDetailID = @OrderDetailID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, new { OrderDetailID = detailId }).ConfigureAwait(false) > 0;
                }
            }, "DeleteLabOrderDetailAsync");
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
