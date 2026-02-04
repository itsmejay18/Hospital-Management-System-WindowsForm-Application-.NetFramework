using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Represents a billable service.
    /// </summary>
    [Table("Services")]
    public sealed class Service : BindableBase
    {
        private int _serviceId;
        private string _serviceCode;
        private string _serviceName;
        private int? _categoryId;
        private decimal _price;
        private decimal _taxRate;
        private bool _isActive;

        /// <summary>
        /// Gets or sets the service identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ServiceID
        {
            get => _serviceId;
            set => SetProperty(ref _serviceId, value);
        }

        /// <summary>
        /// Gets or sets the service code.
        /// </summary>
        [Required]
        [StringLength(20)]
        public string ServiceCode
        {
            get => _serviceCode;
            set => SetProperty(ref _serviceCode, value);
        }

        /// <summary>
        /// Gets or sets the service name.
        /// </summary>
        [Required]
        [StringLength(200)]
        public string ServiceName
        {
            get => _serviceName;
            set => SetProperty(ref _serviceName, value);
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
        /// Gets or sets the price.
        /// </summary>
        [Required]
        public decimal Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        /// <summary>
        /// Gets or sets the tax rate.
        /// </summary>
        public decimal TaxRate
        {
            get => _taxRate;
            set => SetProperty(ref _taxRate, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the service is active.
        /// </summary>
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }
    }
}
