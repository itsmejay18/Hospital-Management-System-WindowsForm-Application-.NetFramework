using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HospitalManagementSystem.DAL.Repositories;
using HospitalManagementSystem.Helpers;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.BLL.Services
{
    /// <summary>
    /// Provides patient business logic.
    /// </summary>
    public sealed class PatientService
    {
        private readonly PatientRepository _repository = new PatientRepository();

        /// <summary>
        /// Gets all patients.
        /// </summary>
        public Task<List<Patient>> GetAllAsync()
        {
            return _repository.GetAllPatientsAsync();
        }

        /// <summary>
        /// Adds a patient with validation.
        /// </summary>
        public async Task<int> AddAsync(Patient patient)
        {
            Validate(patient);
            if (!patient.RegistrationDate.HasValue)
            {
                patient.RegistrationDate = DateTime.Now;
            }
            patient.IsActive = true;
            return await _repository.AddPatientAsync(patient).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates a patient with validation.
        /// </summary>
        public async Task<bool> UpdateAsync(Patient patient)
        {
            Validate(patient);
            return await _repository.UpdatePatientAsync(patient).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes a patient.
        /// </summary>
        public Task<bool> DeleteAsync(int patientId)
        {
            return _repository.DeletePatientAsync(patientId);
        }

        private static void Validate(Patient patient)
        {
            if (patient == null)
            {
                throw new ArgumentNullException(nameof(patient));
            }

            if (!ValidationHelper.IsRequired(patient.FirstName))
            {
                throw new ArgumentException("First name is required.");
            }

            if (!ValidationHelper.IsRequired(patient.LastName))
            {
                throw new ArgumentException("Last name is required.");
            }

            if (!ValidationHelper.IsPastOrToday(patient.DateOfBirth))
            {
                throw new ArgumentException("Date of birth cannot be in the future.");
            }
        }
    }
}
