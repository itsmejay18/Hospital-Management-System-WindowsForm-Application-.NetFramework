using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HospitalManagementSystem.DAL.Repositories;
using HospitalManagementSystem.Helpers;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.BLL.Services
{
    /// <summary>
    /// Provides doctor business logic.
    /// </summary>
    public sealed class DoctorService
    {
        private readonly DoctorRepository _repository = new DoctorRepository();

        /// <summary>
        /// Gets all doctors.
        /// </summary>
        public Task<List<Doctor>> GetAllAsync()
        {
            return _repository.GetAllDoctorsAsync();
        }

        /// <summary>
        /// Adds a doctor with validation.
        /// </summary>
        public async Task<int> AddAsync(Doctor doctor)
        {
            Validate(doctor);
            doctor.IsAvailable = true;
            return await _repository.AddDoctorAsync(doctor).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates a doctor with validation.
        /// </summary>
        public Task<bool> UpdateAsync(Doctor doctor)
        {
            Validate(doctor);
            return _repository.UpdateDoctorAsync(doctor);
        }

        /// <summary>
        /// Deletes a doctor.
        /// </summary>
        public Task<bool> DeleteAsync(int doctorId)
        {
            return _repository.DeleteDoctorAsync(doctorId);
        }

        private static void Validate(Doctor doctor)
        {
            if (doctor == null)
            {
                throw new ArgumentNullException(nameof(doctor));
            }

            if (!ValidationHelper.IsRequired(doctor.DoctorCode))
            {
                throw new ArgumentException("Doctor code is required.");
            }
        }
    }
}
