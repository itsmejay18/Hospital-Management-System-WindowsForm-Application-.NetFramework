using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Represents a pharmacy sale.
    /// </summary>
    [Table("PharmacySales")]
    public sealed class PharmacySale : BindableBase
    {
        private int _saleId;
        private string _saleNumber;
        private int? _patientId;
        private DateTime? _saleDate;
        private decimal _totalAmount;
        private decimal _discount;
        private decimal _netAmount;
        private string _paymentStatus;
        private int? _soldBy;

        /// <summary>
        /// Gets or sets the sale identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SaleID
        {
            get => _saleId;
            set => SetProperty(ref _saleId, value);
        }

        /// <summary>
        /// Gets or sets the sale number.
        /// </summary>
        [Required]
        [StringLength(30)]
        public string SaleNumber
        {
            get => _saleNumber;
            set => SetProperty(ref _saleNumber, value);
        }

        /// <summary>
        /// Gets or sets the patient identifier.
        /// </summary>
        public int? PatientID
        {
            get => _patientId;
            set => SetProperty(ref _patientId, value);
        }

        /// <summary>
        /// Gets or sets the sale date.
        /// </summary>
        public DateTime? SaleDate
        {
            get => _saleDate;
            set => SetProperty(ref _saleDate, value);
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
        /// Gets or sets the net amount.
        /// </summary>
        public decimal NetAmount
        {
            get => _netAmount;
            set => SetProperty(ref _netAmount, value);
        }

        /// <summary>
        /// Gets or sets the payment status.
        /// </summary>
        [StringLength(20)]
        public string PaymentStatus
        {
            get => _paymentStatus;
            set => SetProperty(ref _paymentStatus, value);
        }

        /// <summary>
        /// Gets or sets the sold by user identifier.
        /// </summary>
        public int? SoldBy
        {
            get => _soldBy;
            set => SetProperty(ref _soldBy, value);
        }
    }
}
