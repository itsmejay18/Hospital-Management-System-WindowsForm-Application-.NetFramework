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
    /// Provides CRUD operations for appointments.
    /// </summary>
    public sealed class AppointmentRepository : RepositoryBase
    {
        /// <summary>
        /// Gets all appointments.
        /// </summary>
        public IEnumerable<Appointment> GetAllAppointments()
        {
            return ExecuteSafe(() =>
            {
                const string sql = "SELECT * FROM Appointments ORDER BY AppointmentDate DESC, AppointmentTime DESC";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Query<Appointment>(sql).ToList();
                }
            }, "GetAllAppointments");
        }

        /// <summary>
        /// Gets all appointments asynchronously.
        /// </summary>
        public Task<List<Appointment>> GetAllAppointmentsAsync()
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT * FROM Appointments ORDER BY AppointmentDate DESC, AppointmentTime DESC";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var results = await connection.QueryAsync<Appointment>(sql).ConfigureAwait(false);
                    return results.ToList();
                }
            }, "GetAllAppointmentsAsync");
        }

        /// <summary>
        /// Gets an appointment by identifier.
        /// </summary>
        /// <param name="appointmentId">Appointment identifier.</param>
        public Appointment GetAppointmentById(int appointmentId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "SELECT * FROM Appointments WHERE AppointmentID = @AppointmentID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.QuerySingleOrDefault<Appointment>(sql, new { AppointmentID = appointmentId });
                }
            }, "GetAppointmentById");
        }

        /// <summary>
        /// Gets an appointment by identifier asynchronously.
        /// </summary>
        /// <param name="appointmentId">Appointment identifier.</param>
        public Task<Appointment> GetAppointmentByIdAsync(int appointmentId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT * FROM Appointments WHERE AppointmentID = @AppointmentID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.QuerySingleOrDefaultAsync<Appointment>(sql, new { AppointmentID = appointmentId }).ConfigureAwait(false);
                }
            }, "GetAppointmentByIdAsync");
        }

        /// <summary>
        /// Adds a new appointment and returns the new identifier.
        /// </summary>
        /// <param name="appointment">Appointment entity.</param>
        public int AddAppointment(Appointment appointment)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"INSERT INTO Appointments
                                    (AppointmentCode, PatientID, DoctorID, AppointmentDate, AppointmentTime, AppointmentType,
                                     Status, Reason, Duration, CreatedBy, CreatedDate, Notes)
                                    VALUES (@AppointmentCode, @PatientID, @DoctorID, @AppointmentDate, @AppointmentTime, @AppointmentType,
                                            @Status, @Reason, @Duration, @CreatedBy, @CreatedDate, @Notes);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = Db.OpenConnection())
                {
                    return connection.ExecuteScalar<int>(sql, appointment);
                }
            }, "AddAppointment");
        }

        /// <summary>
        /// Adds a new appointment asynchronously and returns the new identifier.
        /// </summary>
        /// <param name="appointment">Appointment entity.</param>
        public Task<int> AddAppointmentAsync(Appointment appointment)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"INSERT INTO Appointments
                                    (AppointmentCode, PatientID, DoctorID, AppointmentDate, AppointmentTime, AppointmentType,
                                     Status, Reason, Duration, CreatedBy, CreatedDate, Notes)
                                    VALUES (@AppointmentCode, @PatientID, @DoctorID, @AppointmentDate, @AppointmentTime, @AppointmentType,
                                            @Status, @Reason, @Duration, @CreatedBy, @CreatedDate, @Notes);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteScalarAsync<int>(sql, appointment).ConfigureAwait(false);
                }
            }, "AddAppointmentAsync");
        }

        /// <summary>
        /// Updates an existing appointment.
        /// </summary>
        /// <param name="appointment">Appointment entity.</param>
        public bool UpdateAppointment(Appointment appointment)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"UPDATE Appointments SET
                                    AppointmentCode = @AppointmentCode,
                                    PatientID = @PatientID,
                                    DoctorID = @DoctorID,
                                    AppointmentDate = @AppointmentDate,
                                    AppointmentTime = @AppointmentTime,
                                    AppointmentType = @AppointmentType,
                                    Status = @Status,
                                    Reason = @Reason,
                                    Duration = @Duration,
                                    CreatedBy = @CreatedBy,
                                    CreatedDate = @CreatedDate,
                                    Notes = @Notes
                                    WHERE AppointmentID = @AppointmentID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, appointment) > 0;
                }
            }, "UpdateAppointment");
        }

        /// <summary>
        /// Updates an existing appointment asynchronously.
        /// </summary>
        /// <param name="appointment">Appointment entity.</param>
        public Task<bool> UpdateAppointmentAsync(Appointment appointment)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"UPDATE Appointments SET
                                    AppointmentCode = @AppointmentCode,
                                    PatientID = @PatientID,
                                    DoctorID = @DoctorID,
                                    AppointmentDate = @AppointmentDate,
                                    AppointmentTime = @AppointmentTime,
                                    AppointmentType = @AppointmentType,
                                    Status = @Status,
                                    Reason = @Reason,
                                    Duration = @Duration,
                                    CreatedBy = @CreatedBy,
                                    CreatedDate = @CreatedDate,
                                    Notes = @Notes
                                    WHERE AppointmentID = @AppointmentID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, appointment).ConfigureAwait(false) > 0;
                }
            }, "UpdateAppointmentAsync");
        }

        /// <summary>
        /// Deletes an appointment by identifier.
        /// </summary>
        /// <param name="appointmentId">Appointment identifier.</param>
        public bool DeleteAppointment(int appointmentId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "DELETE FROM Appointments WHERE AppointmentID = @AppointmentID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, new { AppointmentID = appointmentId }) > 0;
                }
            }, "DeleteAppointment");
        }

        /// <summary>
        /// Deletes an appointment by identifier asynchronously.
        /// </summary>
        /// <param name="appointmentId">Appointment identifier.</param>
        public Task<bool> DeleteAppointmentAsync(int appointmentId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "DELETE FROM Appointments WHERE AppointmentID = @AppointmentID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, new { AppointmentID = appointmentId }).ConfigureAwait(false) > 0;
                }
            }, "DeleteAppointmentAsync");
        }

        /// <summary>
        /// Searches appointments with filters and pagination.
        /// </summary>
        public Task<PagedResult<Appointment>> SearchAppointmentsAsync(
            int? patientId,
            int? doctorId,
            DateTime? dateFrom,
            DateTime? dateTo,
            string status,
            int pageNumber,
            int pageSize)
        {
            return ExecuteSafeAsync(async () =>
            {
                var sql = new StringBuilder("SELECT * FROM Appointments WHERE 1=1 ");
                var countSql = new StringBuilder("SELECT COUNT(*) FROM Appointments WHERE 1=1 ");
                var parameters = new DynamicParameters();

                if (patientId.HasValue)
                {
                    sql.Append(" AND PatientID = @PatientID ");
                    countSql.Append(" AND PatientID = @PatientID ");
                    parameters.Add("@PatientID", patientId.Value);
                }

                if (doctorId.HasValue)
                {
                    sql.Append(" AND DoctorID = @DoctorID ");
                    countSql.Append(" AND DoctorID = @DoctorID ");
                    parameters.Add("@DoctorID", doctorId.Value);
                }

                if (dateFrom.HasValue)
                {
                    sql.Append(" AND AppointmentDate >= @DateFrom ");
                    countSql.Append(" AND AppointmentDate >= @DateFrom ");
                    parameters.Add("@DateFrom", dateFrom.Value.Date);
                }

                if (dateTo.HasValue)
                {
                    sql.Append(" AND AppointmentDate <= @DateTo ");
                    countSql.Append(" AND AppointmentDate <= @DateTo ");
                    parameters.Add("@DateTo", dateTo.Value.Date);
                }

                if (!string.IsNullOrWhiteSpace(status))
                {
                    sql.Append(" AND Status = @Status ");
                    countSql.Append(" AND Status = @Status ");
                    parameters.Add("@Status", status);
                }

                sql.Append(" ORDER BY AppointmentDate DESC, AppointmentTime DESC ");
                AddPagination(sql, parameters, pageNumber, pageSize);

                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var totalCount = await connection.ExecuteScalarAsync<int>(countSql.ToString(), parameters).ConfigureAwait(false);
                    var items = await connection.QueryAsync<Appointment>(sql.ToString(), parameters).ConfigureAwait(false);

                    return new PagedResult<Appointment>
                    {
                        Items = items.ToList(),
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    };
                }
            }, "SearchAppointmentsAsync");
        }

        /// <summary>
        /// Gets appointment history entries for an appointment.
        /// </summary>
        /// <param name="appointmentId">Appointment identifier.</param>
        public IEnumerable<AppointmentHistory> GetAppointmentHistory(int appointmentId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "SELECT * FROM AppointmentHistory WHERE AppointmentID = @AppointmentID ORDER BY ChangedDate DESC";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Query<AppointmentHistory>(sql, new { AppointmentID = appointmentId }).ToList();
                }
            }, "GetAppointmentHistory");
        }

        /// <summary>
        /// Gets appointment history entries asynchronously.
        /// </summary>
        /// <param name="appointmentId">Appointment identifier.</param>
        public Task<List<AppointmentHistory>> GetAppointmentHistoryAsync(int appointmentId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT * FROM AppointmentHistory WHERE AppointmentID = @AppointmentID ORDER BY ChangedDate DESC";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var results = await connection.QueryAsync<AppointmentHistory>(sql, new { AppointmentID = appointmentId }).ConfigureAwait(false);
                    return results.ToList();
                }
            }, "GetAppointmentHistoryAsync");
        }

        /// <summary>
        /// Checks doctor availability using the database function.
        /// </summary>
        /// <param name="doctorId">Doctor identifier.</param>
        /// <param name="date">Appointment date.</param>
        /// <param name="time">Appointment time.</param>
        public Task<bool> CheckDoctorAvailabilityAsync(int doctorId, DateTime date, TimeSpan time)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT CheckDoctorAvailability(@DoctorID, @AppointmentDate, @AppointmentTime)";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, new
                    {
                        DoctorID = doctorId,
                        AppointmentDate = date.Date,
                        AppointmentTime = time
                    }).ConfigureAwait(false);
                    return result == 1;
                }
            }, "CheckDoctorAvailabilityAsync");
        }

        /// <summary>
        /// Executes the CreateAppointment stored procedure.
        /// </summary>
        public Task<CreateAppointmentResult> CreateAppointmentAsync(
            int patientId,
            int doctorId,
            DateTime appointmentDate,
            TimeSpan appointmentTime,
            string appointmentType,
            string reason,
            int createdBy)
        {
            return ExecuteSafeAsync(async () =>
            {
                var parameters = new
                {
                    pPatientID = patientId,
                    pDoctorID = doctorId,
                    pAppointmentDate = appointmentDate.Date,
                    pAppointmentTime = appointmentTime,
                    pAppointmentType = appointmentType,
                    pReason = reason,
                    pCreatedBy = createdBy
                };

                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.QuerySingleAsync<CreateAppointmentResult>(
                        "CreateAppointment",
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure).ConfigureAwait(false);
                }
            }, "CreateAppointmentAsync");
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
