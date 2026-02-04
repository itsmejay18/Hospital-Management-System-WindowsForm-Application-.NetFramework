using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Represents a patient.
    /// </summary>
    [Table("Patients")]
    public sealed class Patient : BindableBase
    {
        private int _patientId;
        private string _patientCode;
        private string _firstName;
        private string _lastName;
        private DateTime _dateOfBirth;
        private string _gender;
        private string _bloodGroup;
        private string _maritalStatus;
        private string _nationality;
        private string _identificationType;
        private string _identificationNumber;
        private DateTime? _registrationDate;
        private bool _isActive;

        /// <summary>
        /// Gets or sets the patient identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatientID
        {
            get => _patientId;
            set => SetProperty(ref _patientId, value);
        }

        /// <summary>
        /// Gets or sets the patient code.
        /// </summary>
        [Required]
        [StringLength(20)]
        public string PatientCode
        {
            get => _patientCode;
            set => SetProperty(ref _patientCode, value);
        }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        [Required]
        public DateTime DateOfBirth
        {
            get => _dateOfBirth;
            set => SetProperty(ref _dateOfBirth, value);
        }

        /// <summary>
        /// Gets or sets the gender code.
        /// </summary>
        [StringLength(1)]
        public string Gender
        {
            get => _gender;
            set => SetProperty(ref _gender, value);
        }

        /// <summary>
        /// Gets or sets the blood group.
        /// </summary>
        [StringLength(5)]
        public string BloodGroup
        {
            get => _bloodGroup;
            set => SetProperty(ref _bloodGroup, value);
        }

        /// <summary>
        /// Gets or sets the marital status.
        /// </summary>
        [StringLength(20)]
        public string MaritalStatus
        {
            get => _maritalStatus;
            set => SetProperty(ref _maritalStatus, value);
        }

        /// <summary>
        /// Gets or sets the nationality.
        /// </summary>
        [StringLength(50)]
        public string Nationality
        {
            get => _nationality;
            set => SetProperty(ref _nationality, value);
        }

        /// <summary>
        /// Gets or sets the identification type.
        /// </summary>
        [StringLength(50)]
        public string IdentificationType
        {
            get => _identificationType;
            set => SetProperty(ref _identificationType, value);
        }

        /// <summary>
        /// Gets or sets the identification number.
        /// </summary>
        [StringLength(50)]
        public string IdentificationNumber
        {
            get => _identificationNumber;
            set => SetProperty(ref _identificationNumber, value);
        }

        /// <summary>
        /// Gets or sets the registration date.
        /// </summary>
        public DateTime? RegistrationDate
        {
            get => _registrationDate;
            set => SetProperty(ref _registrationDate, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the patient is active.
        /// </summary>
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}
