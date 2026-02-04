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
    /// Provides CRUD operations for patients and medical histories.
    /// </summary>
    public sealed class PatientRepository : RepositoryBase
    {
        /// <summary>
        /// Gets all patients.
        /// </summary>
        public IEnumerable<Patient> GetAllPatients()
        {
            return ExecuteSafe(() =>
            {
                const string sql = "SELECT * FROM Patients ORDER BY LastName, FirstName";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Query<Patient>(sql).ToList();
                }
            }, "GetAllPatients");
        }

        /// <summary>
        /// Gets all patients asynchronously.
        /// </summary>
        public Task<List<Patient>> GetAllPatientsAsync()
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT * FROM Patients ORDER BY LastName, FirstName";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var results = await connection.QueryAsync<Patient>(sql).ConfigureAwait(false);
                    return results.ToList();
                }
            }, "GetAllPatientsAsync");
        }

        /// <summary>
        /// Gets a patient by identifier.
        /// </summary>
        /// <param name="patientId">Patient identifier.</param>
        public Patient GetPatientById(int patientId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "SELECT * FROM Patients WHERE PatientID = @PatientID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.QuerySingleOrDefault<Patient>(sql, new { PatientID = patientId });
                }
            }, "GetPatientById");
        }

        /// <summary>
        /// Gets a patient by identifier asynchronously.
        /// </summary>
        /// <param name="patientId">Patient identifier.</param>
        public Task<Patient> GetPatientByIdAsync(int patientId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT * FROM Patients WHERE PatientID = @PatientID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.QuerySingleOrDefaultAsync<Patient>(sql, new { PatientID = patientId }).ConfigureAwait(false);
                }
            }, "GetPatientByIdAsync");
        }

        /// <summary>
        /// Adds a new patient and returns the new identifier.
        /// </summary>
        /// <param name="patient">Patient entity.</param>
        public int AddPatient(Patient patient)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"INSERT INTO Patients
                                    (PatientCode, FirstName, LastName, DateOfBirth, Gender, BloodGroup, MaritalStatus,
                                     Nationality, IdentificationType, IdentificationNumber, RegistrationDate, IsActive)
                                    VALUES (@PatientCode, @FirstName, @LastName, @DateOfBirth, @Gender, @BloodGroup, @MaritalStatus,
                                            @Nationality, @IdentificationType, @IdentificationNumber, @RegistrationDate, @IsActive);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = Db.OpenConnection())
                {
                    return connection.ExecuteScalar<int>(sql, patient);
                }
            }, "AddPatient");
        }

        /// <summary>
        /// Adds a new patient asynchronously and returns the new identifier.
        /// </summary>
        /// <param name="patient">Patient entity.</param>
        public Task<int> AddPatientAsync(Patient patient)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"INSERT INTO Patients
                                    (PatientCode, FirstName, LastName, DateOfBirth, Gender, BloodGroup, MaritalStatus,
                                     Nationality, IdentificationType, IdentificationNumber, RegistrationDate, IsActive)
                                    VALUES (@PatientCode, @FirstName, @LastName, @DateOfBirth, @Gender, @BloodGroup, @MaritalStatus,
                                            @Nationality, @IdentificationType, @IdentificationNumber, @RegistrationDate, @IsActive);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteScalarAsync<int>(sql, patient).ConfigureAwait(false);
                }
            }, "AddPatientAsync");
        }

        /// <summary>
        /// Updates an existing patient.
        /// </summary>
        /// <param name="patient">Patient entity.</param>
        public bool UpdatePatient(Patient patient)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"UPDATE Patients SET
                                    PatientCode = @PatientCode,
                                    FirstName = @FirstName,
                                    LastName = @LastName,
                                    DateOfBirth = @DateOfBirth,
                                    Gender = @Gender,
                                    BloodGroup = @BloodGroup,
                                    MaritalStatus = @MaritalStatus,
                                    Nationality = @Nationality,
                                    IdentificationType = @IdentificationType,
                                    IdentificationNumber = @IdentificationNumber,
                                    RegistrationDate = @RegistrationDate,
                                    IsActive = @IsActive
                                    WHERE PatientID = @PatientID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, patient) > 0;
                }
            }, "UpdatePatient");
        }

        /// <summary>
        /// Updates an existing patient asynchronously.
        /// </summary>
        /// <param name="patient">Patient entity.</param>
        public Task<bool> UpdatePatientAsync(Patient patient)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"UPDATE Patients SET
                                    PatientCode = @PatientCode,
                                    FirstName = @FirstName,
                                    LastName = @LastName,
                                    DateOfBirth = @DateOfBirth,
                                    Gender = @Gender,
                                    BloodGroup = @BloodGroup,
                                    MaritalStatus = @MaritalStatus,
                                    Nationality = @Nationality,
                                    IdentificationType = @IdentificationType,
                                    IdentificationNumber = @IdentificationNumber,
                                    RegistrationDate = @RegistrationDate,
                                    IsActive = @IsActive
                                    WHERE PatientID = @PatientID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, patient).ConfigureAwait(false) > 0;
                }
            }, "UpdatePatientAsync");
        }

        /// <summary>
        /// Deletes a patient by identifier.
        /// </summary>
        /// <param name="patientId">Patient identifier.</param>
        public bool DeletePatient(int patientId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "DELETE FROM Patients WHERE PatientID = @PatientID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, new { PatientID = patientId }) > 0;
                }
            }, "DeletePatient");
        }

        /// <summary>
        /// Deletes a patient by identifier asynchronously.
        /// </summary>
        /// <param name="patientId">Patient identifier.</param>
        public Task<bool> DeletePatientAsync(int patientId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "DELETE FROM Patients WHERE PatientID = @PatientID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, new { PatientID = patientId }).ConfigureAwait(false) > 0;
                }
            }, "DeletePatientAsync");
        }

        /// <summary>
        /// Searches patients with filters and pagination.
        /// </summary>
        public Task<PagedResult<Patient>> SearchPatientsAsync(
            string patientCode,
            string firstName,
            string lastName,
            DateTime? dateOfBirthFrom,
            DateTime? dateOfBirthTo,
            string gender,
            bool? isActive,
            int pageNumber,
            int pageSize)
        {
            return ExecuteSafeAsync(async () =>
            {
                var sql = new StringBuilder("SELECT * FROM Patients WHERE 1=1 ");
                var countSql = new StringBuilder("SELECT COUNT(*) FROM Patients WHERE 1=1 ");
                var parameters = new DynamicParameters();

                if (!string.IsNullOrWhiteSpace(patientCode))
                {
                    sql.Append(" AND PatientCode LIKE @PatientCode ");
                    countSql.Append(" AND PatientCode LIKE @PatientCode ");
                    parameters.Add("@PatientCode", $"%{patientCode}%");
                }

                if (!string.IsNullOrWhiteSpace(firstName))
                {
                    sql.Append(" AND FirstName LIKE @FirstName ");
                    countSql.Append(" AND FirstName LIKE @FirstName ");
                    parameters.Add("@FirstName", $"%{firstName}%");
                }

                if (!string.IsNullOrWhiteSpace(lastName))
                {
                    sql.Append(" AND LastName LIKE @LastName ");
                    countSql.Append(" AND LastName LIKE @LastName ");
                    parameters.Add("@LastName", $"%{lastName}%");
                }

                if (dateOfBirthFrom.HasValue)
                {
                    sql.Append(" AND DateOfBirth >= @DateOfBirthFrom ");
                    countSql.Append(" AND DateOfBirth >= @DateOfBirthFrom ");
                    parameters.Add("@DateOfBirthFrom", dateOfBirthFrom.Value);
                }

                if (dateOfBirthTo.HasValue)
                {
                    sql.Append(" AND DateOfBirth <= @DateOfBirthTo ");
                    countSql.Append(" AND DateOfBirth <= @DateOfBirthTo ");
                    parameters.Add("@DateOfBirthTo", dateOfBirthTo.Value);
                }

                if (!string.IsNullOrWhiteSpace(gender))
                {
                    sql.Append(" AND Gender = @Gender ");
                    countSql.Append(" AND Gender = @Gender ");
                    parameters.Add("@Gender", gender);
                }

                if (isActive.HasValue)
                {
                    sql.Append(" AND IsActive = @IsActive ");
                    countSql.Append(" AND IsActive = @IsActive ");
                    parameters.Add("@IsActive", isActive.Value);
                }

                sql.Append(" ORDER BY LastName, FirstName ");
                AddPagination(sql, parameters, pageNumber, pageSize);

                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var totalCount = await connection.ExecuteScalarAsync<int>(countSql.ToString(), parameters).ConfigureAwait(false);
                    var items = await connection.QueryAsync<Patient>(sql.ToString(), parameters).ConfigureAwait(false);

                    return new PagedResult<Patient>
                    {
                        Items = items.ToList(),
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    };
                }
            }, "SearchPatientsAsync");
        }

        /// <summary>
        /// Gets patient contacts by patient identifier.
        /// </summary>
        /// <param name="patientId">Patient identifier.</param>
        public IEnumerable<PatientContact> GetPatientContactsByPatientId(int patientId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "SELECT * FROM PatientContacts WHERE PatientID = @PatientID ORDER BY IsPrimary DESC, ContactType";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Query<PatientContact>(sql, new { PatientID = patientId }).ToList();
                }
            }, "GetPatientContactsByPatientId");
        }

        /// <summary>
        /// Gets patient contacts by patient identifier asynchronously.
        /// </summary>
        /// <param name="patientId">Patient identifier.</param>
        public Task<List<PatientContact>> GetPatientContactsByPatientIdAsync(int patientId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT * FROM PatientContacts WHERE PatientID = @PatientID ORDER BY IsPrimary DESC, ContactType";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var results = await connection.QueryAsync<PatientContact>(sql, new { PatientID = patientId }).ConfigureAwait(false);
                    return results.ToList();
                }
            }, "GetPatientContactsByPatientIdAsync");
        }

        /// <summary>
        /// Adds a patient contact and returns the new identifier.
        /// </summary>
        /// <param name="contact">Patient contact entity.</param>
        public int AddPatientContact(PatientContact contact)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"INSERT INTO PatientContacts (PatientID, ContactType, ContactValue, IsPrimary)
                                    VALUES (@PatientID, @ContactType, @ContactValue, @IsPrimary);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = Db.OpenConnection())
                {
                    return connection.ExecuteScalar<int>(sql, contact);
                }
            }, "AddPatientContact");
        }

        /// <summary>
        /// Adds a patient contact asynchronously and returns the new identifier.
        /// </summary>
        /// <param name="contact">Patient contact entity.</param>
        public Task<int> AddPatientContactAsync(PatientContact contact)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"INSERT INTO PatientContacts (PatientID, ContactType, ContactValue, IsPrimary)
                                    VALUES (@PatientID, @ContactType, @ContactValue, @IsPrimary);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteScalarAsync<int>(sql, contact).ConfigureAwait(false);
                }
            }, "AddPatientContactAsync");
        }

        /// <summary>
        /// Updates a patient contact.
        /// </summary>
        /// <param name="contact">Patient contact entity.</param>
        public bool UpdatePatientContact(PatientContact contact)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"UPDATE PatientContacts SET
                                    ContactType = @ContactType,
                                    ContactValue = @ContactValue,
                                    IsPrimary = @IsPrimary
                                    WHERE ContactID = @ContactID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, contact) > 0;
                }
            }, "UpdatePatientContact");
        }

        /// <summary>
        /// Updates a patient contact asynchronously.
        /// </summary>
        /// <param name="contact">Patient contact entity.</param>
        public Task<bool> UpdatePatientContactAsync(PatientContact contact)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"UPDATE PatientContacts SET
                                    ContactType = @ContactType,
                                    ContactValue = @ContactValue,
                                    IsPrimary = @IsPrimary
                                    WHERE ContactID = @ContactID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, contact).ConfigureAwait(false) > 0;
                }
            }, "UpdatePatientContactAsync");
        }

        /// <summary>
        /// Deletes a patient contact by identifier.
        /// </summary>
        /// <param name="contactId">Contact identifier.</param>
        public bool DeletePatientContact(int contactId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "DELETE FROM PatientContacts WHERE ContactID = @ContactID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, new { ContactID = contactId }) > 0;
                }
            }, "DeletePatientContact");
        }

        /// <summary>
        /// Deletes a patient contact by identifier asynchronously.
        /// </summary>
        /// <param name="contactId">Contact identifier.</param>
        public Task<bool> DeletePatientContactAsync(int contactId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "DELETE FROM PatientContacts WHERE ContactID = @ContactID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, new { ContactID = contactId }).ConfigureAwait(false) > 0;
                }
            }, "DeletePatientContactAsync");
        }

        /// <summary>
        /// Gets medical history entries for a patient.
        /// </summary>
        /// <param name="patientId">Patient identifier.</param>
        public IEnumerable<MedicalHistory> GetMedicalHistoriesByPatientId(int patientId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "SELECT * FROM MedicalHistories WHERE PatientID = @PatientID ORDER BY DiagnosisDate DESC";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Query<MedicalHistory>(sql, new { PatientID = patientId }).ToList();
                }
            }, "GetMedicalHistoriesByPatientId");
        }

        /// <summary>
        /// Gets medical history entries for a patient asynchronously.
        /// </summary>
        /// <param name="patientId">Patient identifier.</param>
        public Task<List<MedicalHistory>> GetMedicalHistoriesByPatientIdAsync(int patientId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT * FROM MedicalHistories WHERE PatientID = @PatientID ORDER BY DiagnosisDate DESC";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var results = await connection.QueryAsync<MedicalHistory>(sql, new { PatientID = patientId }).ConfigureAwait(false);
                    return results.ToList();
                }
            }, "GetMedicalHistoriesByPatientIdAsync");
        }

        /// <summary>
        /// Adds a medical history record and returns the new identifier.
        /// </summary>
        /// <param name="history">Medical history entity.</param>
        public int AddMedicalHistory(MedicalHistory history)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"INSERT INTO MedicalHistories
                                    (PatientID, HistoryType, Description, DiagnosisDate, Severity, Status, RecordedBy, RecordedDate)
                                    VALUES (@PatientID, @HistoryType, @Description, @DiagnosisDate, @Severity, @Status, @RecordedBy, @RecordedDate);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = Db.OpenConnection())
                {
                    return connection.ExecuteScalar<int>(sql, history);
                }
            }, "AddMedicalHistory");
        }

        /// <summary>
        /// Adds a medical history record asynchronously and returns the new identifier.
        /// </summary>
        /// <param name="history">Medical history entity.</param>
        public Task<int> AddMedicalHistoryAsync(MedicalHistory history)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"INSERT INTO MedicalHistories
                                    (PatientID, HistoryType, Description, DiagnosisDate, Severity, Status, RecordedBy, RecordedDate)
                                    VALUES (@PatientID, @HistoryType, @Description, @DiagnosisDate, @Severity, @Status, @RecordedBy, @RecordedDate);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteScalarAsync<int>(sql, history).ConfigureAwait(false);
                }
            }, "AddMedicalHistoryAsync");
        }

        /// <summary>
        /// Updates a medical history record.
        /// </summary>
        /// <param name="history">Medical history entity.</param>
        public bool UpdateMedicalHistory(MedicalHistory history)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"UPDATE MedicalHistories SET
                                    HistoryType = @HistoryType,
                                    Description = @Description,
                                    DiagnosisDate = @DiagnosisDate,
                                    Severity = @Severity,
                                    Status = @Status,
                                    RecordedBy = @RecordedBy,
                                    RecordedDate = @RecordedDate
                                    WHERE HistoryID = @HistoryID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, history) > 0;
                }
            }, "UpdateMedicalHistory");
        }

        /// <summary>
        /// Updates a medical history record asynchronously.
        /// </summary>
        /// <param name="history">Medical history entity.</param>
        public Task<bool> UpdateMedicalHistoryAsync(MedicalHistory history)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"UPDATE MedicalHistories SET
                                    HistoryType = @HistoryType,
                                    Description = @Description,
                                    DiagnosisDate = @DiagnosisDate,
                                    Severity = @Severity,
                                    Status = @Status,
                                    RecordedBy = @RecordedBy,
                                    RecordedDate = @RecordedDate
                                    WHERE HistoryID = @HistoryID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, history).ConfigureAwait(false) > 0;
                }
            }, "UpdateMedicalHistoryAsync");
        }

        /// <summary>
        /// Deletes a medical history record.
        /// </summary>
        /// <param name="historyId">History identifier.</param>
        public bool DeleteMedicalHistory(int historyId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "DELETE FROM MedicalHistories WHERE HistoryID = @HistoryID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, new { HistoryID = historyId }) > 0;
                }
            }, "DeleteMedicalHistory");
        }

        /// <summary>
        /// Deletes a medical history record asynchronously.
        /// </summary>
        /// <param name="historyId">History identifier.</param>
        public Task<bool> DeleteMedicalHistoryAsync(int historyId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "DELETE FROM MedicalHistories WHERE HistoryID = @HistoryID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, new { HistoryID = historyId }).ConfigureAwait(false) > 0;
                }
            }, "DeleteMedicalHistoryAsync");
        }

        /// <summary>
        /// Executes the RegisterPatient stored procedure.
        /// </summary>
        /// <param name="firstName">First name.</param>
        /// <param name="lastName">Last name.</param>
        /// <param name="dateOfBirth">Date of birth.</param>
        /// <param name="gender">Gender code.</param>
        /// <param name="bloodGroup">Blood group.</param>
        /// <param name="contactNumber">Contact number.</param>
        public Task<RegisterPatientResult> RegisterPatientAsync(
            string firstName,
            string lastName,
            DateTime dateOfBirth,
            string gender,
            string bloodGroup,
            string contactNumber)
        {
            return ExecuteSafeAsync(async () =>
            {
                var parameters = new
                {
                    pFirstName = firstName,
                    pLastName = lastName,
                    pDateOfBirth = dateOfBirth,
                    pGender = gender,
                    pBloodGroup = bloodGroup,
                    pContactNumber = contactNumber
                };

                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.QuerySingleAsync<RegisterPatientResult>(
                        "RegisterPatient",
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure).ConfigureAwait(false);
                }
            }, "RegisterPatientAsync");
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
