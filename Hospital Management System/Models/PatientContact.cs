using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Represents a patient contact record.
    /// </summary>
    [Table("PatientContacts")]
    public sealed class PatientContact : BindableBase
    {
        private int _contactId;
        private int _patientId;
        private string _contactType;
        private string _contactValue;
        private bool _isPrimary;

        /// <summary>
        /// Gets or sets the contact identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContactID
        {
            get => _contactId;
            set => SetProperty(ref _contactId, value);
        }

        /// <summary>
        /// Gets or sets the patient identifier.
        /// </summary>
        [Required]
        public int PatientID
        {
            get => _patientId;
            set => SetProperty(ref _patientId, value);
        }

        /// <summary>
        /// Gets or sets the contact type.
        /// </summary>
        [StringLength(20)]
        public string ContactType
        {
            get => _contactType;
            set => SetProperty(ref _contactType, value);
        }

        /// <summary>
        /// Gets or sets the contact value.
        /// </summary>
        [Required]
        [StringLength(255)]
        public string ContactValue
        {
            get => _contactValue;
            set => SetProperty(ref _contactValue, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this contact is primary.
        /// </summary>
        public bool IsPrimary
        {
            get => _isPrimary;
            set => SetProperty(ref _isPrimary, value);
        }
    }
}
