using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HospitalManagementSystem.DAL.Repositories;
using HospitalManagementSystem.Helpers;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.BLL.Services
{
    /// <summary>
    /// Provides appointment business logic.
    /// </summary>
    public sealed class AppointmentService
    {
        private readonly AppointmentRepository _repository = new AppointmentRepository();

        /// <summary>
        /// Gets all appointments.
        /// </summary>
        public Task<List<Appointment>> GetAllAsync()
        {
            return _repository.GetAllAppointmentsAsync();
        }

        /// <summary>
        /// Creates an appointment using the stored procedure.
        /// </summary>
        public async Task<int> CreateAsync(Appointment appointment)
        {
            AuthorizationHelper.EnsureRole("Administrator", "Receptionist", "Doctor", "Nurse");
            if (appointment == null)
            {
                throw new ArgumentNullException(nameof(appointment));
            }

            return await _repository.AddAppointmentAsync(appointment).ConfigureAwait(false);
        }

        /// <summary>
        /// Schedules an appointment with availability check.
        /// </summary>
        public async Task<int> ScheduleAsync(Appointment appointment, int createdBy)
        {
            AuthorizationHelper.EnsureRole("Administrator", "Receptionist", "Doctor", "Nurse");
            if (appointment == null)
            {
                throw new ArgumentNullException(nameof(appointment));
            }

            var result = await _repository.CreateAppointmentAsync(
                appointment.PatientID,
                appointment.DoctorID,
                appointment.AppointmentDate,
                appointment.AppointmentTime,
                appointment.AppointmentType,
                appointment.Reason,
                createdBy).ConfigureAwait(false);

            if (result.AppointmentID <= 0)
            {
                throw new InvalidOperationException(result.ErrorMessage ?? "Unable to schedule appointment.");
            }

            return result.AppointmentID;
        }

        /// <summary>
        /// Updates an appointment.
        /// </summary>
        public Task<bool> UpdateAsync(Appointment appointment)
        {
            AuthorizationHelper.EnsureRole("Administrator", "Receptionist", "Doctor", "Nurse");
            if (appointment == null)
            {
                throw new ArgumentNullException(nameof(appointment));
            }

            return _repository.UpdateAppointmentAsync(appointment);
        }

        /// <summary>
        /// Deletes an appointment.
        /// </summary>
        public Task<bool> DeleteAsync(int appointmentId)
        {
            AuthorizationHelper.EnsureRole("Administrator", "Receptionist");
            return _repository.DeleteAppointmentAsync(appointmentId);
        }
    }
}
