using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Represents a lab order detail.
    /// </summary>
    [Table("LabOrderDetails")]
    public sealed class LabOrderDetail : BindableBase
    {
        private int _orderDetailId;
        private int _orderId;
        private int _testId;
        private string _resultValue;
        private string _resultUnit;
        private string _normalRange;
        private bool? _isNormal;
        private string _notes;
        private int? _technicianId;
        private DateTime? _completedDate;

        /// <summary>
        /// Gets or sets the order detail identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderDetailID
        {
            get => _orderDetailId;
            set => SetProperty(ref _orderDetailId, value);
        }

        /// <summary>
        /// Gets or sets the order identifier.
        /// </summary>
        [Required]
        public int OrderID
        {
            get => _orderId;
            set => SetProperty(ref _orderId, value);
        }

        /// <summary>
        /// Gets or sets the test identifier.
        /// </summary>
        [Required]
        public int TestID
        {
            get => _testId;
            set => SetProperty(ref _testId, value);
        }

        /// <summary>
        /// Gets or sets the result value.
        /// </summary>
        [StringLength(200)]
        public string ResultValue
        {
            get => _resultValue;
            set => SetProperty(ref _resultValue, value);
        }

        /// <summary>
        /// Gets or sets the result unit.
        /// </summary>
        [StringLength(50)]
        public string ResultUnit
        {
            get => _resultUnit;
            set => SetProperty(ref _resultUnit, value);
        }

        /// <summary>
        /// Gets or sets the normal range.
        /// </summary>
        [StringLength(200)]
        public string NormalRange
        {
            get => _normalRange;
            set => SetProperty(ref _normalRange, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the result is normal.
        /// </summary>
        public bool? IsNormal
        {
            get => _isNormal;
            set => SetProperty(ref _isNormal, value);
        }

        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        /// <summary>
        /// Gets or sets the technician identifier.
        /// </summary>
        public int? TechnicianID
        {
            get => _technicianId;
            set => SetProperty(ref _technicianId, value);
        }

        /// <summary>
        /// Gets or sets the completed date.
        /// </summary>
        public DateTime? CompletedDate
        {
            get => _completedDate;
            set => SetProperty(ref _completedDate, value);
        }
    }
}
