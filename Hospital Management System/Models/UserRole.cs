using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Represents a user role.
    /// </summary>
    [Table("UserRoles")]
    public sealed class UserRole : BindableBase
    {
        private int _roleId;
        private string _roleName;
        private string _description;
        private DateTime? _createdDate;

        /// <summary>
        /// Gets or sets the role identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleID
        {
            get => _roleId;
            set => SetProperty(ref _roleId, value);
        }

        /// <summary>
        /// Gets or sets the role name.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string RoleName
        {
            get => _roleName;
            set => SetProperty(ref _roleName, value);
        }

        /// <summary>
        /// Gets or sets the role description.
        /// </summary>
        [StringLength(255)]
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
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
