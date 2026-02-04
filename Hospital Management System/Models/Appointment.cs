using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Represents an appointment.
    /// </summary>
    [Table("Appointments")]
    public sealed class Appointment : BindableBase
    {
        private int _appointmentId;
        private string _appointmentCode;
        private int _patientId;
        private int _doctorId;
        private DateTime _appointmentDate;
        private TimeSpan _appointmentTime;
        private string _appointmentType;
        private string _status;
        private string _reason;
        private int _duration;
        private int? _createdBy;
        private DateTime? _createdDate;
        private string _notes;
        private string _patientName;
        private string _doctorName;

        /// <summary>
        /// Gets or sets the appointment identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AppointmentID
        {
            get => _appointmentId;
            set => SetProperty(ref _appointmentId, value);
        }

        /// <summary>
        /// Gets or sets the appointment code.
        /// </summary>
        [Required]
        [StringLength(20)]
        public string AppointmentCode
        {
            get => _appointmentCode;
            set => SetProperty(ref _appointmentCode, value);
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
        /// Gets or sets the doctor identifier.
        /// </summary>
        [Required]
        public int DoctorID
        {
            get => _doctorId;
            set => SetProperty(ref _doctorId, value);
        }

        /// <summary>
        /// Gets or sets the appointment date.
        /// </summary>
        [Required]
        public DateTime AppointmentDate
        {
            get => _appointmentDate;
            set => SetProperty(ref _appointmentDate, value);
        }

        /// <summary>
        /// Gets or sets the appointment time.
        /// </summary>
        [Required]
        public TimeSpan AppointmentTime
        {
            get => _appointmentTime;
            set => SetProperty(ref _appointmentTime, value);
        }

        /// <summary>
        /// Gets or sets the appointment type.
        /// </summary>
        [StringLength(20)]
        public string AppointmentType
        {
            get => _appointmentType;
            set => SetProperty(ref _appointmentType, value);
        }

        /// <summary>
        /// Gets or sets the appointment status.
        /// </summary>
        [StringLength(20)]
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        /// <summary>
        /// Gets or sets the reason.
        /// </summary>
        [StringLength(500)]
        public string Reason
        {
            get => _reason;
            set => SetProperty(ref _reason, value);
        }

        /// <summary>
        /// Gets or sets the duration in minutes.
        /// </summary>
        public int Duration
        {
            get => _duration;
            set => SetProperty(ref _duration, value);
        }

        /// <summary>
        /// Gets or sets the creator user identifier.
        /// </summary>
        public int? CreatedBy
        {
            get => _createdBy;
            set => SetProperty(ref _createdBy, value);
        }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        public DateTime? CreatedDate
        {
            get => _createdDate;
            set => SetProperty(ref _createdDate, value);
        }

        /// <summary>
        /// Gets or sets notes.
        /// </summary>
        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        /// <summary>
        /// Gets or sets the patient display name.
        /// </summary>
        [NotMapped]
        public string PatientName
        {
            get => _patientName;
            set => SetProperty(ref _patientName, value);
        }

        /// <summary>
        /// Gets or sets the doctor display name.
        /// </summary>
        [NotMapped]
        public string DoctorName
        {
            get => _doctorName;
            set => SetProperty(ref _doctorName, value);
        }
    }
}
