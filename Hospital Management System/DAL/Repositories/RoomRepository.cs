using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.DAL.Repositories
{
    /// <summary>
    /// Provides room CRUD and patient room transaction operations.
    /// </summary>
    public sealed class RoomRepository : RepositoryBase
    {
        /// <summary>
        /// Gets rooms with optional search.
        /// </summary>
        public Task<List<Room>> GetRoomsAsync(string searchText = null)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"SELECT
                                        r.RoomID,
                                        r.RoomNumber,
                                        r.WardID,
                                        COALESCE(w.WardName, 'Unassigned') AS WardName,
                                        r.RoomType,
                                        r.TotalBeds,
                                        r.AvailableBeds,
                                        r.Facilities,
                                        r.RatePerDay,
                                        r.Status
                                     FROM Rooms r
                                     LEFT JOIN Wards w ON w.WardID = r.WardID
                                     WHERE
                                        (@SearchText IS NULL OR @SearchText = '' OR
                                         r.RoomNumber LIKE @Pattern OR
                                         COALESCE(r.RoomType, '') LIKE @Pattern OR
                                         COALESCE(r.Status, '') LIKE @Pattern OR
                                         COALESCE(w.WardName, '') LIKE @Pattern)
                                     ORDER BY r.RoomNumber";

                var pattern = string.IsNullOrWhiteSpace(searchText) ? null : $"%{searchText.Trim()}%";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var results = await connection.QueryAsync<Room>(
                        sql,
                        new { SearchText = searchText, Pattern = pattern }).ConfigureAwait(false);
                    return results.ToList();
                }
            }, "GetRoomsAsync");
        }

        /// <summary>
        /// Adds a room and returns the new identifier.
        /// </summary>
        public Task<int> AddRoomAsync(Room room)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"INSERT INTO Rooms
                                    (RoomNumber, WardID, RoomType, TotalBeds, AvailableBeds, Facilities, RatePerDay, Status)
                                    VALUES (@RoomNumber, @WardID, @RoomType, @TotalBeds, @AvailableBeds, @Facilities, @RatePerDay, @Status);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteScalarAsync<int>(sql, room).ConfigureAwait(false);
                }
            }, "AddRoomAsync");
        }

        /// <summary>
        /// Updates a room.
        /// </summary>
        public Task<bool> UpdateRoomAsync(Room room)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"UPDATE Rooms SET
                                        RoomNumber = @RoomNumber,
                                        WardID = @WardID,
                                        RoomType = @RoomType,
                                        TotalBeds = @TotalBeds,
                                        AvailableBeds = @AvailableBeds,
                                        Facilities = @Facilities,
                                        RatePerDay = @RatePerDay,
                                        Status = @Status
                                     WHERE RoomID = @RoomID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, room).ConfigureAwait(false) > 0;
                }
            }, "UpdateRoomAsync");
        }

        /// <summary>
        /// Deletes a room by identifier.
        /// </summary>
        public Task<bool> DeleteRoomAsync(int roomId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "DELETE FROM Rooms WHERE RoomID = @RoomID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, new { RoomID = roomId }).ConfigureAwait(false) > 0;
                }
            }, "DeleteRoomAsync");
        }

        /// <summary>
        /// Gets active patients as lookup items.
        /// </summary>
        public Task<List<LookupItem>> GetPatientLookupAsync()
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"SELECT
                                        p.PatientID AS Id,
                                        CONCAT(p.PatientCode, ' - ', p.FirstName, ' ', p.LastName) AS Name
                                     FROM Patients p
                                     WHERE p.IsActive = 1
                                     ORDER BY p.LastName, p.FirstName";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var results = await connection.QueryAsync<LookupItem>(sql).ConfigureAwait(false);
                    return results.ToList();
                }
            }, "GetPatientLookupAsync");
        }

        /// <summary>
        /// Gets doctors as lookup items.
        /// </summary>
        public Task<List<LookupItem>> GetDoctorLookupAsync()
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"SELECT
                                        d.DoctorID AS Id,
                                        CONCAT(
                                            d.DoctorCode,
                                            ' - ',
                                            COALESCE(NULLIF(TRIM(CONCAT(ud.FirstName, ' ', ud.LastName)), ''), 'Doctor')
                                        ) AS Name
                                     FROM Doctors d
                                     LEFT JOIN Users u ON u.UserID = d.UserID
                                     LEFT JOIN UserDetails ud ON ud.UserID = u.UserID
                                     ORDER BY Name";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var results = await connection.QueryAsync<LookupItem>(sql).ConfigureAwait(false);
                    return results.ToList();
                }
            }, "GetDoctorLookupAsync");
        }

        /// <summary>
        /// Gets rooms with available beds as lookup items.
        /// </summary>
        public Task<List<LookupItem>> GetAvailableRoomLookupAsync()
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"SELECT
                                        r.RoomID AS Id,
                                        CONCAT(
                                            r.RoomNumber,
                                            ' - ',
                                            COALESCE(NULLIF(r.RoomType, ''), 'General'),
                                            ' (Beds: ',
                                            r.AvailableBeds,
                                            '/',
                                            r.TotalBeds,
                                            ')'
                                        ) AS Name
                                     FROM Rooms r
                                     WHERE r.AvailableBeds > 0
                                     ORDER BY r.RoomNumber";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var results = await connection.QueryAsync<LookupItem>(sql).ConfigureAwait(false);
                    return results.ToList();
                }
            }, "GetAvailableRoomLookupAsync");
        }

        /// <summary>
        /// Gets admissions with optional search.
        /// </summary>
        public Task<List<Admission>> GetAdmissionsAsync(string searchText = null, bool activeOnly = false)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"SELECT
                                        a.AdmissionID,
                                        a.AdmissionNumber,
                                        a.PatientID,
                                        a.DoctorID,
                                        a.RoomID,
                                        a.AdmissionDate,
                                        a.ExpectedDischargeDate,
                                        a.ActualDischargeDate,
                                        a.AdmissionReason,
                                        a.Diagnosis,
                                        a.Status,
                                        a.DischargeSummary,
                                        CONCAT(COALESCE(p.FirstName, ''), ' ', COALESCE(p.LastName, '')) AS PatientName,
                                        TRIM(CONCAT(COALESCE(ud.FirstName, ''), ' ', COALESCE(ud.LastName, ''))) AS DoctorName,
                                        r.RoomNumber
                                     FROM Admissions a
                                     LEFT JOIN Patients p ON p.PatientID = a.PatientID
                                     LEFT JOIN Doctors d ON d.DoctorID = a.DoctorID
                                     LEFT JOIN Users u ON u.UserID = d.UserID
                                     LEFT JOIN UserDetails ud ON ud.UserID = u.UserID
                                     LEFT JOIN Rooms r ON r.RoomID = a.RoomID
                                     WHERE
                                        (@ActiveOnly = 0 OR a.Status = 'Admitted')
                                        AND
                                        (@SearchText IS NULL OR @SearchText = '' OR
                                         a.AdmissionNumber LIKE @Pattern OR
                                         COALESCE(p.PatientCode, '') LIKE @Pattern OR
                                         COALESCE(p.FirstName, '') LIKE @Pattern OR
                                         COALESCE(p.LastName, '') LIKE @Pattern OR
                                         COALESCE(r.RoomNumber, '') LIKE @Pattern)
                                     ORDER BY a.AdmissionDate DESC";

                var pattern = string.IsNullOrWhiteSpace(searchText) ? null : $"%{searchText.Trim()}%";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var results = await connection.QueryAsync<Admission>(
                        sql,
                        new { ActiveOnly = activeOnly ? 1 : 0, SearchText = searchText, Pattern = pattern }).ConfigureAwait(false);
                    return results.ToList();
                }
            }, "GetAdmissionsAsync");
        }

        /// <summary>
        /// Creates an admission transaction and recalculates room occupancy.
        /// </summary>
        public Task<int> AdmitPatientAsync(
            int patientId,
            int doctorId,
            int roomId,
            DateTime? expectedDischargeDate,
            string admissionReason,
            string diagnosis)
        {
            return ExecuteSafeAsync(async () =>
            {
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        const string roomLockSql = @"SELECT RoomID, TotalBeds, AvailableBeds
                                                     FROM Rooms
                                                     WHERE RoomID = @RoomID
                                                     FOR UPDATE";
                        var room = await connection.QuerySingleOrDefaultAsync<RoomAvailabilityRow>(
                            roomLockSql,
                            new { RoomID = roomId },
                            transaction).ConfigureAwait(false);

                        if (room == null)
                        {
                            throw new InvalidOperationException("Selected room does not exist.");
                        }

                        if (room.AvailableBeds <= 0)
                        {
                            throw new InvalidOperationException("Selected room has no available beds.");
                        }

                        var now = DateTime.Now;
                        var nextId = await GetNextAdmissionSequenceAsync(connection, transaction, now.Year).ConfigureAwait(false);
                        var admissionNumber = BuildAdmissionNumber(now.Year, nextId);

                        const string insertSql = @"INSERT INTO Admissions
                                                  (AdmissionNumber, PatientID, DoctorID, RoomID, AdmissionDate, ExpectedDischargeDate,
                                                   AdmissionReason, Diagnosis, Status)
                                                  VALUES
                                                  (@AdmissionNumber, @PatientID, @DoctorID, @RoomID, @AdmissionDate, @ExpectedDischargeDate,
                                                   @AdmissionReason, @Diagnosis, 'Admitted');
                                                  SELECT LAST_INSERT_ID();";
                        var admissionId = await connection.ExecuteScalarAsync<int>(
                            insertSql,
                            new
                            {
                                AdmissionNumber = admissionNumber,
                                PatientID = patientId,
                                DoctorID = doctorId,
                                RoomID = roomId,
                                AdmissionDate = now,
                                ExpectedDischargeDate = expectedDischargeDate,
                                AdmissionReason = admissionReason,
                                Diagnosis = diagnosis
                            },
                            transaction).ConfigureAwait(false);

                        await RecalculateRoomAvailabilityAsync(connection, transaction, roomId).ConfigureAwait(false);
                        transaction.Commit();
                        return admissionId;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }, "AdmitPatientAsync");
        }

        /// <summary>
        /// Discharges an admission and updates room occupancy.
        /// </summary>
        public Task<bool> DischargeAdmissionAsync(int admissionId, string dischargeSummary)
        {
            return ExecuteSafeAsync(async () =>
            {
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        const string lockAdmissionSql = @"SELECT AdmissionID, RoomID, Status
                                                          FROM Admissions
                                                          WHERE AdmissionID = @AdmissionID
                                                          FOR UPDATE";
                        var row = await connection.QuerySingleOrDefaultAsync<AdmissionRoomRow>(
                            lockAdmissionSql,
                            new { AdmissionID = admissionId },
                            transaction).ConfigureAwait(false);

                        if (row == null || !string.Equals(row.Status, "Admitted", StringComparison.OrdinalIgnoreCase))
                        {
                            transaction.Rollback();
                            return false;
                        }

                        const string dischargeSql = @"UPDATE Admissions
                                                      SET Status = 'Discharged',
                                                          ActualDischargeDate = CURRENT_TIMESTAMP,
                                                          DischargeSummary = @DischargeSummary
                                                      WHERE AdmissionID = @AdmissionID
                                                        AND Status = 'Admitted'";
                        var affected = await connection.ExecuteAsync(
                            dischargeSql,
                            new { AdmissionID = admissionId, DischargeSummary = dischargeSummary },
                            transaction).ConfigureAwait(false);
                        if (affected <= 0)
                        {
                            transaction.Rollback();
                            return false;
                        }

                        if (row.RoomID.HasValue)
                        {
                            await RecalculateRoomAvailabilityAsync(connection, transaction, row.RoomID.Value).ConfigureAwait(false);
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }, "DischargeAdmissionAsync");
        }

        private static async Task<int> GetNextAdmissionSequenceAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            int year)
        {
            const string sql = @"SELECT COALESCE(MAX(CAST(SUBSTRING_INDEX(AdmissionNumber, '-', -1) AS UNSIGNED)), 0) + 1
                                 FROM Admissions
                                 WHERE AdmissionNumber LIKE @Pattern";
            var pattern = $"ADM-{year}-%";
            return await connection.ExecuteScalarAsync<int>(sql, new { Pattern = pattern }, transaction).ConfigureAwait(false);
        }

        private static string BuildAdmissionNumber(int year, int nextId)
        {
            return $"ADM-{year}-{nextId:D4}";
        }

        private static Task RecalculateRoomAvailabilityAsync(IDbConnection connection, IDbTransaction transaction, int roomId)
        {
            const string sql = @"UPDATE Rooms r
                                 LEFT JOIN (
                                     SELECT RoomID, COUNT(*) AS OccupiedCount
                                     FROM Admissions
                                     WHERE Status = 'Admitted'
                                       AND RoomID = @RoomID
                                     GROUP BY RoomID
                                 ) occupied ON occupied.RoomID = r.RoomID
                                 SET r.AvailableBeds = GREATEST(r.TotalBeds - COALESCE(occupied.OccupiedCount, 0), 0),
                                     r.Status = CASE
                                         WHEN GREATEST(r.TotalBeds - COALESCE(occupied.OccupiedCount, 0), 0) = 0 THEN 'Occupied'
                                         ELSE 'Available'
                                     END
                                 WHERE r.RoomID = @RoomID";
            return connection.ExecuteAsync(sql, new { RoomID = roomId }, transaction);
        }

        private sealed class RoomAvailabilityRow
        {
            public int RoomID { get; set; }
            public int TotalBeds { get; set; }
            public int AvailableBeds { get; set; }
        }

        private sealed class AdmissionRoomRow
        {
            public int AdmissionID { get; set; }
            public int? RoomID { get; set; }
            public string Status { get; set; }
        }
    }
}
