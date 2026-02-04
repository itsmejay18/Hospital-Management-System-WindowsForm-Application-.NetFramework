using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Represents an appointment history entry.
    /// </summary>
    [Table("AppointmentHistory")]
    public sealed class AppointmentHistory : BindableBase
    {
        private int _historyId;
        private int _appointmentId;
        private string _status;
        private int? _changedBy;
        private DateTime? _changedDate;
        private string _notes;

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
        /// Gets or sets the appointment identifier.
        /// </summary>
        [Required]
        public int AppointmentID
        {
            get => _appointmentId;
            set => SetProperty(ref _appointmentId, value);
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
        /// Gets or sets the user who changed the status.
        /// </summary>
        public int? ChangedBy
        {
            get => _changedBy;
            set => SetProperty(ref _changedBy, value);
        }

        /// <summary>
        /// Gets or sets the change date.
        /// </summary>
        public DateTime? ChangedDate
        {
            get => _changedDate;
            set => SetProperty(ref _changedDate, value);
        }

        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        [StringLength(500)]
        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }
    }
}
