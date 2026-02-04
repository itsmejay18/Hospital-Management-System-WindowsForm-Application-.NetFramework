using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Represents a payment record.
    /// </summary>
    [Table("Payments")]
    public sealed class Payment : BindableBase
    {
        private int _paymentId;
        private string _paymentNumber;
        private int _invoiceId;
        private DateTime? _paymentDate;
        private string _paymentMethod;
        private decimal _amount;
        private string _referenceNumber;
        private int? _receivedBy;
        private string _notes;

        /// <summary>
        /// Gets or sets the payment identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentID
        {
            get => _paymentId;
            set => SetProperty(ref _paymentId, value);
        }

        /// <summary>
        /// Gets or sets the payment number.
        /// </summary>
        [Required]
        [StringLength(30)]
        public string PaymentNumber
        {
            get => _paymentNumber;
            set => SetProperty(ref _paymentNumber, value);
        }

        /// <summary>
        /// Gets or sets the invoice identifier.
        /// </summary>
        [Required]
        public int InvoiceID
        {
            get => _invoiceId;
            set => SetProperty(ref _invoiceId, value);
        }

        /// <summary>
        /// Gets or sets the payment date.
        /// </summary>
        public DateTime? PaymentDate
        {
            get => _paymentDate;
            set => SetProperty(ref _paymentDate, value);
        }

        /// <summary>
        /// Gets or sets the payment method.
        /// </summary>
        [StringLength(20)]
        public string PaymentMethod
        {
            get => _paymentMethod;
            set => SetProperty(ref _paymentMethod, value);
        }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        [Required]
        public decimal Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        /// <summary>
        /// Gets or sets the reference number.
        /// </summary>
        [StringLength(100)]
        public string ReferenceNumber
        {
            get => _referenceNumber;
            set => SetProperty(ref _referenceNumber, value);
        }

        /// <summary>
        /// Gets or sets the received by user identifier.
        /// </summary>
        public int? ReceivedBy
        {
            get => _receivedBy;
            set => SetProperty(ref _receivedBy, value);
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
