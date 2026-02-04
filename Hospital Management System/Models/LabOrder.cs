using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Represents a lab order.
    /// </summary>
    [Table("LabOrders")]
    public sealed class LabOrder : BindableBase
    {
        private int _orderId;
        private string _orderCode;
        private int? _visitId;
        private int _patientId;
        private int _doctorId;
        private DateTime? _orderDate;
        private string _status;
        private DateTime? _resultDate;
        private string _notes;

        /// <summary>
        /// Gets or sets the order identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderID
        {
            get => _orderId;
            set => SetProperty(ref _orderId, value);
        }

        /// <summary>
        /// Gets or sets the order code.
        /// </summary>
        [Required]
        [StringLength(20)]
        public string OrderCode
        {
            get => _orderCode;
            set => SetProperty(ref _orderCode, value);
        }

        /// <summary>
        /// Gets or sets the visit identifier.
        /// </summary>
        public int? VisitID
        {
            get => _visitId;
            set => SetProperty(ref _visitId, value);
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
        /// Gets or sets the order date.
        /// </summary>
        public DateTime? OrderDate
        {
            get => _orderDate;
            set => SetProperty(ref _orderDate, value);
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
        /// Gets or sets the result date.
        /// </summary>
        public DateTime? ResultDate
        {
            get => _resultDate;
            set => SetProperty(ref _resultDate, value);
        }

        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }
    }
}
