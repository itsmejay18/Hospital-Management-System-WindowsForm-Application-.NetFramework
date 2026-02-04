using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Represents a staff member.
    /// </summary>
    [Table("Staff")]
    public sealed class Staff : BindableBase
    {
        private int _staffId;
        private int _userId;
        private string _staffCode;
        private string _designation;
        private string _department;
        private string _shift;
        private DateTime? _hireDate;
        private decimal? _salary;

        /// <summary>
        /// Gets or sets the staff identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StaffID
        {
            get => _staffId;
            set => SetProperty(ref _staffId, value);
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
        /// Gets or sets the staff code.
        /// </summary>
        [Required]
        [StringLength(20)]
        public string StaffCode
        {
            get => _staffCode;
            set => SetProperty(ref _staffCode, value);
        }

        /// <summary>
        /// Gets or sets the designation.
        /// </summary>
        [StringLength(100)]
        public string Designation
        {
            get => _designation;
            set => SetProperty(ref _designation, value);
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

        /// <summary>
        /// Gets or sets the shift.
        /// </summary>
        [StringLength(20)]
        public string Shift
        {
            get => _shift;
            set => SetProperty(ref _shift, value);
        }

        /// <summary>
        /// Gets or sets the hire date.
        /// </summary>
        public DateTime? HireDate
        {
            get => _hireDate;
            set => SetProperty(ref _hireDate, value);
        }

        /// <summary>
        /// Gets or sets the salary.
        /// </summary>
        public decimal? Salary
        {
            get => _salary;
            set => SetProperty(ref _salary, value);
        }
    }
}
