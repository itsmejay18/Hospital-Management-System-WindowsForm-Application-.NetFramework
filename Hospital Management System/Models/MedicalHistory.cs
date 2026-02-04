using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Represents a patient medical history entry.
    /// </summary>
    [Table("MedicalHistories")]
    public sealed class MedicalHistory : BindableBase
    {
        private int _historyId;
        private int _patientId;
        private string _historyType;
        private string _description;
        private DateTime? _diagnosisDate;
        private string _severity;
        private string _status;
        private int? _recordedBy;
        private DateTime? _recordedDate;

        /// <summary>
        /// Gets or sets the history identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HistoryID
        {
            get => _historyId;
            set => SetProperty(ref _historyId, value);
        }

        /// <summary>
        /// Gets or sets the patient identifier.
        /// </summary>
        [Required]
        public int PatientID
        {
            get => _patientId;
            set => SetProperty(ref _patientId, value);
        }

        /// <summary>
        /// Gets or sets the history type.
        /// </summary>
        [StringLength(50)]
        public string HistoryType
        {
            get => _historyType;
            set => SetProperty(ref _historyType, value);
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [StringLength(500)]
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        /// <summary>
        /// Gets or sets the diagnosis date.
        /// </summary>
        public DateTime? DiagnosisDate
        {
            get => _diagnosisDate;
            set => SetProperty(ref _diagnosisDate, value);
        }

        /// <summary>
        /// Gets or sets the severity.
        /// </summary>
        [StringLength(20)]
        public string Severity
        {
            get => _severity;
            set => SetProperty(ref _severity, value);
        }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        [StringLength(20)]
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        /// <summary>
        /// Gets or sets the recorded by user identifier.
        /// </summary>
        public int? RecordedBy
        {
            get => _recordedBy;
            set => SetProperty(ref _recordedBy, value);
        }

        /// <summary>
        /// Gets or sets the recorded date.
        /// </summary>
        public DateTime? RecordedDate
        {
            get => _recordedDate;
            set => SetProperty(ref _recordedDate, value);
        }
    }
}
