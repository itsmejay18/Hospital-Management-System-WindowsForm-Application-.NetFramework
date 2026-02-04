using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Represents user profile details.
    /// </summary>
    [Table("UserDetails")]
    public sealed class UserDetail : BindableBase
    {
        private int _userDetailId;
        private int _userId;
        private string _firstName;
        private string _lastName;
        private DateTime? _dateOfBirth;
        private string _gender;
        private string _contactNumber;
        private string _address;
        private string _emergencyContact;
        private byte[] _profileImage;

        /// <summary>
        /// Gets or sets the user detail identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserDetailID
        {
            get => _userDetailId;
            set => SetProperty(ref _userDetailId, value);
        }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        [Required]
        public int UserID
        {
            get => _userId;
            set => SetProperty(ref _userId, value);
        }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        public DateTime? DateOfBirth
        {
            get => _dateOfBirth;
            set => SetProperty(ref _dateOfBirth, value);
        }

        /// <summary>
        /// Gets or sets the gender code.
        /// </summary>
        [StringLength(1)]
        public string Gender
        {
            get => _gender;
            set => SetProperty(ref _gender, value);
        }

        /// <summary>
        /// Gets or sets the contact number.
        /// </summary>
        [StringLength(20)]
        public string ContactNumber
        {
            get => _contactNumber;
            set => SetProperty(ref _contactNumber, value);
        }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        [StringLength(255)]
        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        /// <summary>
        /// Gets or sets the emergency contact number.
        /// </summary>
        [StringLength(20)]
        public string EmergencyContact
        {
            get => _emergencyContact;
            set => SetProperty(ref _emergencyContact, value);
        }

        /// <summary>
        /// Gets or sets the profile image binary.
        /// </summary>
        public byte[] ProfileImage
        {
            get => _profileImage;
            set => SetProperty(ref _profileImage, value);
        }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}
