using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Represents a service category.
    /// </summary>
    [Table("ServiceCategories")]
    public sealed class ServiceCategory : BindableBase
    {
        private int _categoryId;
        private string _categoryName;
        private string _description;

        /// <summary>
        /// Gets or sets the category identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryID
        {
            get => _categoryId;
            set => SetProperty(ref _categoryId, value);
        }

        /// <summary>
        /// Gets or sets the category name.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string CategoryName
        {
            get => _categoryName;
            set => SetProperty(ref _categoryName, value);
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [StringLength(255)]
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }
    }
}
