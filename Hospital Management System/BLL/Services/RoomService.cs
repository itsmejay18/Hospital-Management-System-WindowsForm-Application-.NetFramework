using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HospitalManagementSystem.DAL.Repositories;
using HospitalManagementSystem.Helpers;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.BLL.Services
{
    /// <summary>
    /// Provides room and admission business logic.
    /// </summary>
    public sealed class RoomService
    {
        private readonly RoomRepository _repository = new RoomRepository();

        /// <summary>
        /// Gets rooms with optional search.
        /// </summary>
        public Task<List<Room>> GetRoomsAsync(string searchText = null)
        {
            return _repository.GetRoomsAsync(searchText);
        }

        /// <summary>
        /// Adds a new room.
        /// </summary>
        public Task<int> AddRoomAsync(Room room)
        {
            AuthorizationHelper.EnsureRole("Administrator", "Receptionist");
            ValidateRoom(room);
            return _repository.AddRoomAsync(room);
        }

        /// <summary>
        /// Updates an existing room.
        /// </summary>
        public Task<bool> UpdateRoomAsync(Room room)
        {
            AuthorizationHelper.EnsureRole("Administrator", "Receptionist");
            if (room == null || room.RoomID <= 0)
            {
                throw new ArgumentException("Invalid room details.");
            }

            ValidateRoom(room);
            return _repository.UpdateRoomAsync(room);
        }

        /// <summary>
        /// Deletes a room.
        /// </summary>
        public Task<bool> DeleteRoomAsync(int roomId)
        {
            AuthorizationHelper.EnsureRole("Administrator");
            if (roomId <= 0)
            {
                throw new ArgumentException("Invalid room identifier.");
            }

            return _repository.DeleteRoomAsync(roomId);
        }

        /// <summary>
        /// Gets admission transactions.
        /// </summary>
        public Task<List<Admission>> GetAdmissionsAsync(string searchText = null, bool activeOnly = true)
        {
            return _repository.GetAdmissionsAsync(searchText, activeOnly);
        }

        /// <summary>
        /// Gets patients for admission selection.
        /// </summary>
        public Task<List<LookupItem>> GetPatientLookupAsync()
        {
            return _repository.GetPatientLookupAsync();
        }

        /// <summary>
        /// Gets doctors for admission selection.
        /// </summary>
        public Task<List<LookupItem>> GetDoctorLookupAsync()
        {
            return _repository.GetDoctorLookupAsync();
        }

        /// <summary>
        /// Gets available rooms for admission selection.
        /// </summary>
        public Task<List<LookupItem>> GetAvailableRoomLookupAsync()
        {
            return _repository.GetAvailableRoomLookupAsync();
        }

        /// <summary>
        /// Creates an admission transaction.
        /// </summary>
        public Task<int> AdmitPatientAsync(
            int patientId,
            int doctorId,
            int roomId,
            DateTime? expectedDischargeDate,
            string admissionReason,
            string diagnosis)
        {
            AuthorizationHelper.EnsureRole("Administrator", "Receptionist", "Nurse");
            if (patientId <= 0)
            {
                throw new ArgumentException("Select a patient.");
            }

            if (doctorId <= 0)
            {
                throw new ArgumentException("Select a doctor.");
            }

            if (roomId <= 0)
            {
                throw new ArgumentException("Select a room.");
            }

            if (expectedDischargeDate.HasValue && expectedDischargeDate.Value.Date < DateTime.Today)
            {
                throw new ArgumentException("Expected discharge date cannot be in the past.");
            }

            return _repository.AdmitPatientAsync(
                patientId,
                doctorId,
                roomId,
                expectedDischargeDate,
                admissionReason,
                diagnosis);
        }

        /// <summary>
        /// Discharges an admitted patient.
        /// </summary>
        public Task<bool> DischargeAdmissionAsync(int admissionId, string dischargeSummary)
        {
            AuthorizationHelper.EnsureRole("Administrator", "Receptionist", "Nurse", "Doctor");
            if (admissionId <= 0)
            {
                throw new ArgumentException("Invalid admission identifier.");
            }

            return _repository.DischargeAdmissionAsync(admissionId, dischargeSummary);
        }

        private static void ValidateRoom(Room room)
        {
            if (room == null)
            {
                throw new ArgumentNullException(nameof(room));
            }

            if (!ValidationHelper.IsRequired(room.RoomNumber))
            {
                throw new ArgumentException("Room number is required.");
            }

            if (room.TotalBeds <= 0)
            {
                throw new ArgumentException("Total beds must be at least 1.");
            }

            if (room.AvailableBeds < 0 || room.AvailableBeds > room.TotalBeds)
            {
                throw new ArgumentException("Available beds must be between 0 and total beds.");
            }

            if (string.IsNullOrWhiteSpace(room.Status))
            {
                room.Status = room.AvailableBeds > 0 ? "Available" : "Occupied";
            }
        }
    }
}
