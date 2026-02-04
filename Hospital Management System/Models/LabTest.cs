using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Represents a lab test catalog entry.
    /// </summary>
    [Table("LabTests")]
    public sealed class LabTest : BindableBase
    {
        private int _testId;
        private string _testCode;
        private string _testName;
        private string _category;
        private string _normalRange;
        private string _unit;
        private decimal? _price;

        /// <summary>
        /// Gets or sets the test identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TestID
        {
            get => _testId;
            set => SetProperty(ref _testId, value);
        }

        /// <summary>
        /// Gets or sets the test code.
        /// </summary>
        [Required]
        [StringLength(20)]
        public string TestCode
        {
            get => _testCode;
            set => SetProperty(ref _testCode, value);
        }

        /// <summary>
        /// Gets or sets the test name.
        /// </summary>
        [Required]
        [StringLength(200)]
        public string TestName
        {
            get => _testName;
            set => SetProperty(ref _testName, value);
        }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        [StringLength(100)]
        public string Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
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
        /// Gets or sets the unit.
        /// </summary>
        [StringLength(50)]
        public string Unit
        {
            get => _unit;
            set => SetProperty(ref _unit, value);
        }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        public decimal? Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }
    }
}
