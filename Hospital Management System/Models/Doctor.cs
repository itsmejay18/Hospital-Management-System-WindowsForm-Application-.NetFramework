using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Represents a doctor.
    /// </summary>
    [Table("Doctors")]
    public sealed class Doctor : BindableBase
    {
        private int _doctorId;
        private int _userId;
        private string _doctorCode;
        private int? _specializationId;
        private string _qualification;
        private string _licenseNumber;
        private int? _yearsOfExperience;
        private decimal? _consultationFee;
        private bool _isAvailable;
        private DateTime? _joiningDate;

        /// <summary>
        /// Gets or sets the doctor identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DoctorID
        {
            get => _doctorId;
            set => SetProperty(ref _doctorId, value);
        }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        [Required]
        public int UserID
        {
            get => _userId;
            set => SetProperty(ref _userId, value);
        }

        /// <summary>
        /// Gets or sets the doctor code.
        /// </summary>
        [Required]
        [StringLength(20)]
        public string DoctorCode
        {
            get => _doctorCode;
            set => SetProperty(ref _doctorCode, value);
        }

        /// <summary>
        /// Gets or sets the specialization identifier.
        /// </summary>
        public int? SpecializationID
        {
            get => _specializationId;
            set => SetProperty(ref _specializationId, value);
        }

        /// <summary>
        /// Gets or sets the qualification.
        /// </summary>
        [StringLength(255)]
        public string Qualification
        {
            get => _qualification;
            set => SetProperty(ref _qualification, value);
        }

        /// <summary>
        /// Gets or sets the license number.
        /// </summary>
        [StringLength(50)]
        public string LicenseNumber
        {
            get => _licenseNumber;
            set => SetProperty(ref _licenseNumber, value);
        }

        /// <summary>
        /// Gets or sets years of experience.
        /// </summary>
        public int? YearsOfExperience
        {
            get => _yearsOfExperience;
            set => SetProperty(ref _yearsOfExperience, value);
        }

        /// <summary>
        /// Gets or sets the consultation fee.
        /// </summary>
        public decimal? ConsultationFee
        {
            get => _consultationFee;
            set => SetProperty(ref _consultationFee, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the doctor is available.
        /// </summary>
        public bool IsAvailable
        {
            get => _isAvailable;
            set => SetProperty(ref _isAvailable, value);
        }

        /// <summary>
        /// Gets or sets the joining date.
        /// </summary>
        public DateTime? JoiningDate
        {
            get => _joiningDate;
            set => SetProperty(ref _joiningDate, value);
        }
    }
}
