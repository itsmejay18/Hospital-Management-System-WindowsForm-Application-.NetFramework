using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Represents a medical specialization.
    /// </summary>
    [Table("Specializations")]
    public sealed class Specialization : BindableBase
    {
        private int _specializationId;
        private string _specializationCode;
        private string _specializationName;
        private string _description;
        private string _department;

        /// <summary>
        /// Gets or sets the specialization identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SpecializationID
        {
            get => _specializationId;
            set => SetProperty(ref _specializationId, value);
        }

        /// <summary>
        /// Gets or sets the specialization code.
        /// </summary>
        [Required]
        [StringLength(20)]
        public string SpecializationCode
        {
            get => _specializationCode;
            set => SetProperty(ref _specializationCode, value);
        }

        /// <summary>
        /// Gets or sets the specialization name.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string SpecializationName
        {
            get => _specializationName;
            set => SetProperty(ref _specializationName, value);
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

        /// <summary>
        /// Gets or sets the department.
        /// </summary>
        [StringLength(100)]
        public string Department
        {
            get => _department;
            set => SetProperty(ref _department, value);
        }
    }
}
