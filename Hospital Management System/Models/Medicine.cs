using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Represents a medicine.
    /// </summary>
    [Table("Medicines")]
    public sealed class Medicine : BindableBase
    {
        private int _medicineId;
        private string _medicineCode;
        private string _medicineName;
        private string _genericName;
        private int? _categoryId;
        private string _manufacturer;
        private string _unitOfMeasure;
        private decimal _unitPrice;
        private decimal _sellingPrice;
        private int _reorderLevel;
        private bool _isActive;

        /// <summary>
        /// Gets or sets the medicine identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MedicineID
        {
            get => _medicineId;
            set => SetProperty(ref _medicineId, value);
        }

        /// <summary>
        /// Gets or sets the medicine code.
        /// </summary>
        [Required]
        [StringLength(20)]
        public string MedicineCode
        {
            get => _medicineCode;
            set => SetProperty(ref _medicineCode, value);
        }

        /// <summary>
        /// Gets or sets the medicine name.
        /// </summary>
        [Required]
        [StringLength(200)]
        public string MedicineName
        {
            get => _medicineName;
            set => SetProperty(ref _medicineName, value);
        }

        /// <summary>
        /// Gets or sets the generic name.
        /// </summary>
        [StringLength(200)]
        public string GenericName
        {
            get => _genericName;
            set => SetProperty(ref _genericName, value);
        }

        /// <summary>
        /// Gets or sets the category identifier.
        /// </summary>
        public int? CategoryID
        {
            get => _categoryId;
            set => SetProperty(ref _categoryId, value);
        }

        /// <summary>
        /// Gets or sets the manufacturer.
        /// </summary>
        [StringLength(200)]
        public string Manufacturer
        {
            get => _manufacturer;
            set => SetProperty(ref _manufacturer, value);
        }

        /// <summary>
        /// Gets or sets the unit of measure.
        /// </summary>
        [StringLength(50)]
        public string UnitOfMeasure
        {
            get => _unitOfMeasure;
            set => SetProperty(ref _unitOfMeasure, value);
        }

        /// <summary>
        /// Gets or sets the unit price.
        /// </summary>
        [Required]
        public decimal UnitPrice
        {
            get => _unitPrice;
            set => SetProperty(ref _unitPrice, value);
        }

        /// <summary>
        /// Gets or sets the selling price.
        /// </summary>
        [Required]
        public decimal SellingPrice
        {
            get => _sellingPrice;
            set => SetProperty(ref _sellingPrice, value);
        }

        /// <summary>
        /// Gets or sets the reorder level.
        /// </summary>
        public int ReorderLevel
        {
            get => _reorderLevel;
            set => SetProperty(ref _reorderLevel, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the medicine is active.
        /// </summary>
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }
    }
}
