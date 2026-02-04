using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Represents a pharmacy inventory item.
    /// </summary>
    [Table("Inventory")]
    public sealed class InventoryItem : BindableBase
    {
        private int _inventoryId;
        private int _medicineId;
        private string _batchNumber;
        private DateTime? _expiryDate;
        private int _quantity;
        private decimal? _purchasePrice;
        private decimal? _sellingPrice;
        private string _supplier;
        private DateTime? _purchaseDate;
        private string _location;

        /// <summary>
        /// Gets or sets the inventory identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InventoryID
        {
            get => _inventoryId;
            set => SetProperty(ref _inventoryId, value);
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
        /// Gets or sets the expiry date.
        /// </summary>
        public DateTime? ExpiryDate
        {
            get => _expiryDate;
            set => SetProperty(ref _expiryDate, value);
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
        /// Gets or sets the purchase price.
        /// </summary>
        public decimal? PurchasePrice
        {
            get => _purchasePrice;
            set => SetProperty(ref _purchasePrice, value);
        }

        /// <summary>
        /// Gets or sets the selling price.
        /// </summary>
        public decimal? SellingPrice
        {
            get => _sellingPrice;
            set => SetProperty(ref _sellingPrice, value);
        }

        /// <summary>
        /// Gets or sets the supplier.
        /// </summary>
        [StringLength(200)]
        public string Supplier
        {
            get => _supplier;
            set => SetProperty(ref _supplier, value);
        }

        /// <summary>
        /// Gets or sets the purchase date.
        /// </summary>
        public DateTime? PurchaseDate
        {
            get => _purchaseDate;
            set => SetProperty(ref _purchaseDate, value);
        }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        [StringLength(100)]
        public string Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }
    }
}
