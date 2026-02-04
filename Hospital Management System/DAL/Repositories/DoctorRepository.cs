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
    /// Provides CRUD operations for doctors and schedules.
    /// </summary>
    public sealed class DoctorRepository : RepositoryBase
    {
        /// <summary>
        /// Gets all doctors.
        /// </summary>
        public IEnumerable<Doctor> GetAllDoctors()
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"SELECT d.*, 
                                            CONCAT(ud.FirstName, ' ', ud.LastName) AS DoctorName,
                                            s.SpecializationName
                                     FROM Doctors d
                                     LEFT JOIN Users u ON d.UserID = u.UserID
                                     LEFT JOIN UserDetails ud ON u.UserID = ud.UserID
                                     LEFT JOIN Specializations s ON d.SpecializationID = s.SpecializationID
                                     ORDER BY d.DoctorCode";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Query<Doctor>(sql).ToList();
                }
            }, "GetAllDoctors");
        }

        /// <summary>
        /// Gets all doctors asynchronously.
        /// </summary>
        public Task<List<Doctor>> GetAllDoctorsAsync()
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"SELECT d.*, 
                                            CONCAT(ud.FirstName, ' ', ud.LastName) AS DoctorName,
                                            s.SpecializationName
                                     FROM Doctors d
                                     LEFT JOIN Users u ON d.UserID = u.UserID
                                     LEFT JOIN UserDetails ud ON u.UserID = ud.UserID
                                     LEFT JOIN Specializations s ON d.SpecializationID = s.SpecializationID
                                     ORDER BY d.DoctorCode";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var results = await connection.QueryAsync<Doctor>(sql).ConfigureAwait(false);
                    return results.ToList();
                }
            }, "GetAllDoctorsAsync");
        }

        /// <summary>
        /// Gets a doctor by identifier.
        /// </summary>
        /// <param name="doctorId">Doctor identifier.</param>
        public Doctor GetDoctorById(int doctorId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "SELECT * FROM Doctors WHERE DoctorID = @DoctorID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.QuerySingleOrDefault<Doctor>(sql, new { DoctorID = doctorId });
                }
            }, "GetDoctorById");
        }

        /// <summary>
        /// Gets a doctor by identifier asynchronously.
        /// </summary>
        /// <param name="doctorId">Doctor identifier.</param>
        public Task<Doctor> GetDoctorByIdAsync(int doctorId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT * FROM Doctors WHERE DoctorID = @DoctorID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.QuerySingleOrDefaultAsync<Doctor>(sql, new { DoctorID = doctorId }).ConfigureAwait(false);
                }
            }, "GetDoctorByIdAsync");
        }

        /// <summary>
        /// Adds a new doctor and returns the new identifier.
        /// </summary>
        /// <param name="doctor">Doctor entity.</param>
        public int AddDoctor(Doctor doctor)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"INSERT INTO Doctors
                                    (UserID, DoctorCode, SpecializationID, Qualification, LicenseNumber, YearsOfExperience,
                                     ConsultationFee, IsAvailable, JoiningDate)
                                    VALUES (@UserID, @DoctorCode, @SpecializationID, @Qualification, @LicenseNumber,
                                            @YearsOfExperience, @ConsultationFee, @IsAvailable, @JoiningDate);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = Db.OpenConnection())
                {
                    return connection.ExecuteScalar<int>(sql, doctor);
                }
            }, "AddDoctor");
        }

        /// <summary>
        /// Adds a new doctor asynchronously and returns the new identifier.
        /// </summary>
        /// <param name="doctor">Doctor entity.</param>
        public Task<int> AddDoctorAsync(Doctor doctor)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"INSERT INTO Doctors
                                    (UserID, DoctorCode, SpecializationID, Qualification, LicenseNumber, YearsOfExperience,
                                     ConsultationFee, IsAvailable, JoiningDate)
                                    VALUES (@UserID, @DoctorCode, @SpecializationID, @Qualification, @LicenseNumber,
                                            @YearsOfExperience, @ConsultationFee, @IsAvailable, @JoiningDate);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteScalarAsync<int>(sql, doctor).ConfigureAwait(false);
                }
            }, "AddDoctorAsync");
        }

        /// <summary>
        /// Updates an existing doctor.
        /// </summary>
        /// <param name="doctor">Doctor entity.</param>
        public bool UpdateDoctor(Doctor doctor)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"UPDATE Doctors SET
                                    UserID = @UserID,
                                    DoctorCode = @DoctorCode,
                                    SpecializationID = @SpecializationID,
                                    Qualification = @Qualification,
                                    LicenseNumber = @LicenseNumber,
                                    YearsOfExperience = @YearsOfExperience,
                                    ConsultationFee = @ConsultationFee,
                                    IsAvailable = @IsAvailable,
                                    JoiningDate = @JoiningDate
                                    WHERE DoctorID = @DoctorID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, doctor) > 0;
                }
            }, "UpdateDoctor");
        }

        /// <summary>
        /// Updates an existing doctor asynchronously.
        /// </summary>
        /// <param name="doctor">Doctor entity.</param>
        public Task<bool> UpdateDoctorAsync(Doctor doctor)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"UPDATE Doctors SET
                                    UserID = @UserID,
                                    DoctorCode = @DoctorCode,
                                    SpecializationID = @SpecializationID,
                                    Qualification = @Qualification,
                                    LicenseNumber = @LicenseNumber,
                                    YearsOfExperience = @YearsOfExperience,
                                    ConsultationFee = @ConsultationFee,
                                    IsAvailable = @IsAvailable,
                                    JoiningDate = @JoiningDate
                                    WHERE DoctorID = @DoctorID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, doctor).ConfigureAwait(false) > 0;
                }
            }, "UpdateDoctorAsync");
        }

        /// <summary>
        /// Deletes a doctor by identifier.
        /// </summary>
        /// <param name="doctorId">Doctor identifier.</param>
        public bool DeleteDoctor(int doctorId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "DELETE FROM Doctors WHERE DoctorID = @DoctorID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, new { DoctorID = doctorId }) > 0;
                }
            }, "DeleteDoctor");
        }

        /// <summary>
        /// Deletes a doctor by identifier asynchronously.
        /// </summary>
        /// <param name="doctorId">Doctor identifier.</param>
        public Task<bool> DeleteDoctorAsync(int doctorId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "DELETE FROM Doctors WHERE DoctorID = @DoctorID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, new { DoctorID = doctorId }).ConfigureAwait(false) > 0;
                }
            }, "DeleteDoctorAsync");
        }

        /// <summary>
        /// Searches doctors with filters and pagination.
        /// </summary>
        public Task<PagedResult<Doctor>> SearchDoctorsAsync(
            string doctorCode,
            int? specializationId,
            bool? isAvailable,
            int pageNumber,
            int pageSize)
        {
            return ExecuteSafeAsync(async () =>
            {
                var sql = new StringBuilder("SELECT * FROM Doctors WHERE 1=1 ");
                var countSql = new StringBuilder("SELECT COUNT(*) FROM Doctors WHERE 1=1 ");
                var parameters = new DynamicParameters();

                if (!string.IsNullOrWhiteSpace(doctorCode))
                {
                    sql.Append(" AND DoctorCode LIKE @DoctorCode ");
                    countSql.Append(" AND DoctorCode LIKE @DoctorCode ");
                    parameters.Add("@DoctorCode", $"%{doctorCode}%");
                }

                if (specializationId.HasValue)
                {
                    sql.Append(" AND SpecializationID = @SpecializationID ");
                    countSql.Append(" AND SpecializationID = @SpecializationID ");
                    parameters.Add("@SpecializationID", specializationId.Value);
                }

                if (isAvailable.HasValue)
                {
                    sql.Append(" AND IsAvailable = @IsAvailable ");
                    countSql.Append(" AND IsAvailable = @IsAvailable ");
                    parameters.Add("@IsAvailable", isAvailable.Value);
                }

                sql.Append(" ORDER BY DoctorCode ");
                AddPagination(sql, parameters, pageNumber, pageSize);

                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var totalCount = await connection.ExecuteScalarAsync<int>(countSql.ToString(), parameters).ConfigureAwait(false);
                    var items = await connection.QueryAsync<Doctor>(sql.ToString(), parameters).ConfigureAwait(false);

                    return new PagedResult<Doctor>
                    {
                        Items = items.ToList(),
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    };
                }
            }, "SearchDoctorsAsync");
        }

        /// <summary>
        /// Gets schedules for a doctor.
        /// </summary>
        /// <param name="doctorId">Doctor identifier.</param>
        public IEnumerable<DoctorSchedule> GetSchedulesByDoctorId(int doctorId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "SELECT * FROM DoctorSchedules WHERE DoctorID = @DoctorID ORDER BY DayOfWeek, StartTime";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Query<DoctorSchedule>(sql, new { DoctorID = doctorId }).ToList();
                }
            }, "GetSchedulesByDoctorId");
        }

        /// <summary>
        /// Gets schedules for a doctor asynchronously.
        /// </summary>
        /// <param name="doctorId">Doctor identifier.</param>
        public Task<List<DoctorSchedule>> GetSchedulesByDoctorIdAsync(int doctorId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT * FROM DoctorSchedules WHERE DoctorID = @DoctorID ORDER BY DayOfWeek, StartTime";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var results = await connection.QueryAsync<DoctorSchedule>(sql, new { DoctorID = doctorId }).ConfigureAwait(false);
                    return results.ToList();
                }
            }, "GetSchedulesByDoctorIdAsync");
        }

        /// <summary>
        /// Adds a doctor schedule and returns the new identifier.
        /// </summary>
        /// <param name="schedule">Schedule entity.</param>
        public int AddDoctorSchedule(DoctorSchedule schedule)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"INSERT INTO DoctorSchedules
                                    (DoctorID, DayOfWeek, StartTime, EndTime, MaxAppointments, IsActive)
                                    VALUES (@DoctorID, @DayOfWeek, @StartTime, @EndTime, @MaxAppointments, @IsActive);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = Db.OpenConnection())
                {
                    return connection.ExecuteScalar<int>(sql, schedule);
                }
            }, "AddDoctorSchedule");
        }

        /// <summary>
        /// Adds a doctor schedule asynchronously and returns the new identifier.
        /// </summary>
        /// <param name="schedule">Schedule entity.</param>
        public Task<int> AddDoctorScheduleAsync(DoctorSchedule schedule)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"INSERT INTO DoctorSchedules
                                    (DoctorID, DayOfWeek, StartTime, EndTime, MaxAppointments, IsActive)
                                    VALUES (@DoctorID, @DayOfWeek, @StartTime, @EndTime, @MaxAppointments, @IsActive);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteScalarAsync<int>(sql, schedule).ConfigureAwait(false);
                }
            }, "AddDoctorScheduleAsync");
        }

        /// <summary>
        /// Updates a doctor schedule.
        /// </summary>
        /// <param name="schedule">Schedule entity.</param>
        public bool UpdateDoctorSchedule(DoctorSchedule schedule)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"UPDATE DoctorSchedules SET
                                    DayOfWeek = @DayOfWeek,
                                    StartTime = @StartTime,
                                    EndTime = @EndTime,
                                    MaxAppointments = @MaxAppointments,
                                    IsActive = @IsActive
                                    WHERE ScheduleID = @ScheduleID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, schedule) > 0;
                }
            }, "UpdateDoctorSchedule");
        }

        /// <summary>
        /// Updates a doctor schedule asynchronously.
        /// </summary>
        /// <param name="schedule">Schedule entity.</param>
        public Task<bool> UpdateDoctorScheduleAsync(DoctorSchedule schedule)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"UPDATE DoctorSchedules SET
                                    DayOfWeek = @DayOfWeek,
                                    StartTime = @StartTime,
                                    EndTime = @EndTime,
                                    MaxAppointments = @MaxAppointments,
                                    IsActive = @IsActive
                                    WHERE ScheduleID = @ScheduleID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, schedule).ConfigureAwait(false) > 0;
                }
            }, "UpdateDoctorScheduleAsync");
        }

        /// <summary>
        /// Deletes a doctor schedule by identifier.
        /// </summary>
        /// <param name="scheduleId">Schedule identifier.</param>
        public bool DeleteDoctorSchedule(int scheduleId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "DELETE FROM DoctorSchedules WHERE ScheduleID = @ScheduleID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, new { ScheduleID = scheduleId }) > 0;
                }
            }, "DeleteDoctorSchedule");
        }

        /// <summary>
        /// Deletes a doctor schedule by identifier asynchronously.
        /// </summary>
        /// <param name="scheduleId">Schedule identifier.</param>
        public Task<bool> DeleteDoctorScheduleAsync(int scheduleId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "DELETE FROM DoctorSchedules WHERE ScheduleID = @ScheduleID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, new { ScheduleID = scheduleId }).ConfigureAwait(false) > 0;
                }
            }, "DeleteDoctorScheduleAsync");
        }

        /// <summary>
        /// Gets all specializations.
        /// </summary>
        public IEnumerable<Specialization> GetAllSpecializations()
        {
            return ExecuteSafe(() =>
            {
                const string sql = "SELECT * FROM Specializations ORDER BY SpecializationName";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Query<Specialization>(sql).ToList();
                }
            }, "GetAllSpecializations");
        }

        /// <summary>
        /// Gets all specializations asynchronously.
        /// </summary>
        public Task<List<Specialization>> GetAllSpecializationsAsync()
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT * FROM Specializations ORDER BY SpecializationName";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var results = await connection.QueryAsync<Specialization>(sql).ConfigureAwait(false);
                    return results.ToList();
                }
            }, "GetAllSpecializationsAsync");
        }

        /// <summary>
        /// Adds a specialization and returns the new identifier.
        /// </summary>
        /// <param name="specialization">Specialization entity.</param>
        public int AddSpecialization(Specialization specialization)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"INSERT INTO Specializations
                                    (SpecializationCode, SpecializationName, Description, Department)
                                    VALUES (@SpecializationCode, @SpecializationName, @Description, @Department);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = Db.OpenConnection())
                {
                    return connection.ExecuteScalar<int>(sql, specialization);
                }
            }, "AddSpecialization");
        }

        /// <summary>
        /// Updates a specialization.
        /// </summary>
        /// <param name="specialization">Specialization entity.</param>
        public bool UpdateSpecialization(Specialization specialization)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"UPDATE Specializations SET
                                    SpecializationCode = @SpecializationCode,
                                    SpecializationName = @SpecializationName,
                                    Description = @Description,
                                    Department = @Department
                                    WHERE SpecializationID = @SpecializationID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, specialization) > 0;
                }
            }, "UpdateSpecialization");
        }

        /// <summary>
        /// Deletes a specialization by identifier.
        /// </summary>
        /// <param name="specializationId">Specialization identifier.</param>
        public bool DeleteSpecialization(int specializationId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "DELETE FROM Specializations WHERE SpecializationID = @SpecializationID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, new { SpecializationID = specializationId }) > 0;
                }
            }, "DeleteSpecialization");
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
