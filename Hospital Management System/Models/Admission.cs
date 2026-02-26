using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Represents a patient admission transaction.
    /// </summary>
    [Table("Admissions")]
    public sealed class Admission : BindableBase
    {
        private int _admissionId;
        private string _admissionNumber;
        private int _patientId;
        private int _doctorId;
        private int? _roomId;
        private DateTime? _admissionDate;
        private DateTime? _expectedDischargeDate;
        private DateTime? _actualDischargeDate;
        private string _admissionReason;
        private string _diagnosis;
        private string _status;
        private string _dischargeSummary;
        private string _patientName;
        private string _doctorName;
        private string _roomNumber;

        /// <summary>
        /// Gets or sets the admission identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AdmissionID
        {
            get => _admissionId;
            set => SetProperty(ref _admissionId, value);
        }

        /// <summary>
        /// Gets or sets admission number.
        /// </summary>
        [Required]
        [StringLength(30)]
        public string AdmissionNumber
        {
            get => _admissionNumber;
            set => SetProperty(ref _admissionNumber, value);
        }

        /// <summary>
        /// Gets or sets patient identifier.
        /// </summary>
        [Required]
        public int PatientID
        {
            get => _patientId;
            set => SetProperty(ref _patientId, value);
        }

        /// <summary>
        /// Gets or sets doctor identifier.
        /// </summary>
        [Required]
        public int DoctorID
        {
            get => _doctorId;
            set => SetProperty(ref _doctorId, value);
        }

        /// <summary>
        /// Gets or sets room identifier.
        /// </summary>
        public int? RoomID
        {
            get => _roomId;
            set => SetProperty(ref _roomId, value);
        }

        /// <summary>
        /// Gets or sets admission date.
        /// </summary>
        public DateTime? AdmissionDate
        {
            get => _admissionDate;
            set => SetProperty(ref _admissionDate, value);
        }

        /// <summary>
        /// Gets or sets expected discharge date.
        /// </summary>
        public DateTime? ExpectedDischargeDate
        {
            get => _expectedDischargeDate;
            set => SetProperty(ref _expectedDischargeDate, value);
        }

        /// <summary>
        /// Gets or sets actual discharge date.
        /// </summary>
        public DateTime? ActualDischargeDate
        {
            get => _actualDischargeDate;
            set => SetProperty(ref _actualDischargeDate, value);
        }

        /// <summary>
        /// Gets or sets admission reason.
        /// </summary>
        public string AdmissionReason
        {
            get => _admissionReason;
            set => SetProperty(ref _admissionReason, value);
        }

        /// <summary>
        /// Gets or sets diagnosis.
        /// </summary>
        public string Diagnosis
        {
            get => _diagnosis;
            set => SetProperty(ref _diagnosis, value);
        }

        /// <summary>
        /// Gets or sets admission status.
        /// </summary>
        [StringLength(20)]
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        /// <summary>
        /// Gets or sets discharge summary.
        /// </summary>
        public string DischargeSummary
        {
            get => _dischargeSummary;
            set => SetProperty(ref _dischargeSummary, value);
        }

        /// <summary>
        /// Gets or sets patient display name.
        /// </summary>
        [NotMapped]
        public string PatientName
        {
            get => _patientName;
            set => SetProperty(ref _patientName, value);
        }

        /// <summary>
        /// Gets or sets doctor display name.
        /// </summary>
        [NotMapped]
        public string DoctorName
        {
            get => _doctorName;
            set => SetProperty(ref _doctorName, value);
        }

        /// <summary>
        /// Gets or sets room display number.
        /// </summary>
        [NotMapped]
        public string RoomNumber
        {
            get => _roomNumber;
            set => SetProperty(ref _roomNumber, value);
        }
    }
}
