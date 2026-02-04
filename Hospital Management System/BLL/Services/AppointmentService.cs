using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HospitalManagementSystem.DAL.Repositories;
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
        public Task<int> CreateAsync(Appointment appointment)
        {
            if (appointment == null)
            {
                throw new ArgumentNullException(nameof(appointment));
            }

            return _repository.AddAppointmentAsync(appointment);
        }

        /// <summary>
        /// Updates an appointment.
        /// </summary>
        public Task<bool> UpdateAsync(Appointment appointment)
        {
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
            return _repository.DeleteAppointmentAsync(appointmentId);
        }
    }
}
