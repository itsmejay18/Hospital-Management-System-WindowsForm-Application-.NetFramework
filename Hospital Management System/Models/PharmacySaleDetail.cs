using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Represents a pharmacy sale detail.
    /// </summary>
    [Table("PharmacySaleDetails")]
    public sealed class PharmacySaleDetail : BindableBase
    {
        private int _saleDetailId;
        private int _saleId;
        private int _medicineId;
        private string _batchNumber;
        private int _quantity;
        private decimal? _unitPrice;
        private decimal? _totalPrice;

        /// <summary>
        /// Gets or sets the sale detail identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SaleDetailID
        {
            get => _saleDetailId;
            set => SetProperty(ref _saleDetailId, value);
        }

        /// <summary>
        /// Gets or sets the sale identifier.
        /// </summary>
        [Required]
        public int SaleID
        {
            get => _saleId;
            set => SetProperty(ref _saleId, value);
        }

        /// <summary>
        /// Gets or sets the medicine identifier.
        /// </summary>
        [Required]
        public int MedicineID
        {
            get => _medicineId;
            set => SetProperty(ref _medicineId, value);
        }

        /// <summary>
        /// Gets or sets the batch number.
        /// </summary>
        [StringLength(100)]
        public string BatchNumber
        {
            get => _batchNumber;
            set => SetProperty(ref _batchNumber, value);
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
