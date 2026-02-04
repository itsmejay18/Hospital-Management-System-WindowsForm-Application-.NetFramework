using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Represents a system user.
    /// </summary>
    [Table("Users")]
    public sealed class User : BindableBase
    {
        private int _userId;
        private string _username;
        private string _passwordHash;
        private string _email;
        private int _roleId;
        private bool _isActive;
        private DateTime? _lastLogin;
        private DateTime? _createdDate;

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID
        {
            get => _userId;
            set => SetProperty(ref _userId, value);
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        /// <summary>
        /// Gets or sets the password hash.
        /// </summary>
        [Required]
        [StringLength(255)]
        public string PasswordHash
        {
            get => _passwordHash;
            set => SetProperty(ref _passwordHash, value);
        }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [StringLength(100)]
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        /// <summary>
        /// Gets or sets the role identifier.
        /// </summary>
        [Required]
        public int RoleID
        {
            get => _roleId;
            set => SetProperty(ref _roleId, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user is active.
        /// </summary>
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        /// <summary>
        /// Gets or sets the last login timestamp.
        /// </summary>
        public DateTime? LastLogin
        {
            get => _lastLogin;
            set => SetProperty(ref _lastLogin, value);
        }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        public DateTime? CreatedDate
        {
            get => _createdDate;
            set => SetProperty(ref _createdDate, value);
        }
    }
}
