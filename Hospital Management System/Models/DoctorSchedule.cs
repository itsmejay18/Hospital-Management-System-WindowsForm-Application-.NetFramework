using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Represents a doctor's schedule entry.
    /// </summary>
    [Table("DoctorSchedules")]
    public sealed class DoctorSchedule : BindableBase
    {
        private int _scheduleId;
        private int _doctorId;
        private int _dayOfWeek;
        private TimeSpan _startTime;
        private TimeSpan _endTime;
        private int _maxAppointments;
        private bool _isActive;

        /// <summary>
        /// Gets or sets the schedule identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ScheduleID
        {
            get => _scheduleId;
            set => SetProperty(ref _scheduleId, value);
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
        /// Gets or sets the day of week (1-7).
        /// </summary>
        [Range(1, 7)]
        public int DayOfWeek
        {
            get => _dayOfWeek;
            set => SetProperty(ref _dayOfWeek, value);
        }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        [Required]
        public TimeSpan StartTime
        {
            get => _startTime;
            set => SetProperty(ref _startTime, value);
        }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        [Required]
        public TimeSpan EndTime
        {
            get => _endTime;
            set => SetProperty(ref _endTime, value);
        }

        /// <summary>
        /// Gets or sets the maximum appointments.
        /// </summary>
        public int MaxAppointments
        {
            get => _maxAppointments;
            set => SetProperty(ref _maxAppointments, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the schedule is active.
        /// </summary>
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }
    }
}
