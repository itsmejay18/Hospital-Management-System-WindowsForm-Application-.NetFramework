using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Represents an invoice header.
    /// </summary>
    [Table("Invoices")]
    public sealed class Invoice : BindableBase
    {
        private int _invoiceId;
        private string _invoiceNumber;
        private int _patientId;
        private int? _appointmentId;
        private DateTime? _invoiceDate;
        private DateTime? _dueDate;
        private decimal _totalAmount;
        private decimal _discount;
        private decimal _taxAmount;
        private decimal _grandTotal;
        private string _status;
        private int? _createdBy;
        private string _notes;

        /// <summary>
        /// Gets or sets the invoice identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvoiceID
        {
            get => _invoiceId;
            set => SetProperty(ref _invoiceId, value);
        }

        /// <summary>
        /// Gets or sets the invoice number.
        /// </summary>
        [Required]
        [StringLength(30)]
        public string InvoiceNumber
        {
            get => _invoiceNumber;
            set => SetProperty(ref _invoiceNumber, value);
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
        /// Gets or sets the appointment identifier.
        /// </summary>
        public int? AppointmentID
        {
            get => _appointmentId;
            set => SetProperty(ref _appointmentId, value);
        }

        /// <summary>
        /// Gets or sets the invoice date.
        /// </summary>
        public DateTime? InvoiceDate
        {
            get => _invoiceDate;
            set => SetProperty(ref _invoiceDate, value);
        }

        /// <summary>
        /// Gets or sets the due date.
        /// </summary>
        public DateTime? DueDate
        {
            get => _dueDate;
            set => SetProperty(ref _dueDate, value);
        }

        /// <summary>
        /// Gets or sets the total amount.
        /// </summary>
        public decimal TotalAmount
        {
            get => _totalAmount;
            set => SetProperty(ref _totalAmount, value);
        }

        /// <summary>
        /// Gets or sets the discount amount.
        /// </summary>
        public decimal Discount
        {
            get => _discount;
            set => SetProperty(ref _discount, value);
        }

        /// <summary>
        /// Gets or sets the tax amount.
        /// </summary>
        public decimal TaxAmount
        {
            get => _taxAmount;
            set => SetProperty(ref _taxAmount, value);
        }

        /// <summary>
        /// Gets or sets the grand total amount.
        /// </summary>
        public decimal GrandTotal
        {
            get => _grandTotal;
            set => SetProperty(ref _grandTotal, value);
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
        /// Gets or sets the creator user identifier.
        /// </summary>
        public int? CreatedBy
        {
            get => _createdBy;
            set => SetProperty(ref _createdBy, value);
        }

        /// <summary>
        /// Gets or sets notes.
        /// </summary>
        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }
    }
}
