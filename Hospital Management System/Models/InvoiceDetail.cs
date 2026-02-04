using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Represents an invoice line item.
    /// </summary>
    [Table("InvoiceDetails")]
    public sealed class InvoiceDetail : BindableBase
    {
        private int _detailId;
        private int _invoiceId;
        private int _serviceId;
        private int _quantity;
        private decimal? _unitPrice;
        private decimal? _totalPrice;

        /// <summary>
        /// Gets or sets the detail identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DetailID
        {
            get => _detailId;
            set => SetProperty(ref _detailId, value);
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
        /// Gets or sets the service identifier.
        /// </summary>
        [Required]
        public int ServiceID
        {
            get => _serviceId;
            set => SetProperty(ref _serviceId, value);
        }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        public int Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value);
        }

        /// <summary>
        /// Gets or sets the unit price.
        /// </summary>
        public decimal? UnitPrice
        {
            get => _unitPrice;
            set => SetProperty(ref _unitPrice, value);
        }

        /// <summary>
        /// Gets or sets the total price.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal? TotalPrice
        {
            get => _totalPrice;
            set => SetProperty(ref _totalPrice, value);
        }
    }
}
