using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Represents a hospital room.
    /// </summary>
    [Table("Rooms")]
    public sealed class Room : BindableBase
    {
        private int _roomId;
        private string _roomNumber;
        private int? _wardId;
        private string _wardName;
        private string _roomType;
        private int _totalBeds;
        private int _availableBeds;
        private string _facilities;
        private decimal? _ratePerDay;
        private string _status;

        /// <summary>
        /// Gets or sets the room identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoomID
        {
            get => _roomId;
            set => SetProperty(ref _roomId, value);
        }

        /// <summary>
        /// Gets or sets the room number.
        /// </summary>
        [Required]
        [StringLength(20)]
        public string RoomNumber
        {
            get => _roomNumber;
            set => SetProperty(ref _roomNumber, value);
        }

        /// <summary>
        /// Gets or sets the ward identifier.
        /// </summary>
        public int? WardID
        {
            get => _wardId;
            set => SetProperty(ref _wardId, value);
        }

        /// <summary>
        /// Gets or sets ward display name.
        /// </summary>
        [NotMapped]
        public string WardName
        {
            get => _wardName;
            set => SetProperty(ref _wardName, value);
        }

        /// <summary>
        /// Gets or sets room type.
        /// </summary>
        [StringLength(50)]
        public string RoomType
        {
            get => _roomType;
            set => SetProperty(ref _roomType, value);
        }

        /// <summary>
        /// Gets or sets total beds.
        /// </summary>
        public int TotalBeds
        {
            get => _totalBeds;
            set => SetProperty(ref _totalBeds, value);
        }

        /// <summary>
        /// Gets or sets available beds.
        /// </summary>
        public int AvailableBeds
        {
            get => _availableBeds;
            set => SetProperty(ref _availableBeds, value);
        }

        /// <summary>
        /// Gets or sets facilities text.
        /// </summary>
        public string Facilities
        {
            get => _facilities;
            set => SetProperty(ref _facilities, value);
        }

        /// <summary>
        /// Gets or sets room rate per day.
        /// </summary>
        public decimal? RatePerDay
        {
            get => _ratePerDay;
            set => SetProperty(ref _ratePerDay, value);
        }

        /// <summary>
        /// Gets or sets room status.
        /// </summary>
        [StringLength(20)]
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        /// <summary>
        /// Gets occupied bed count.
        /// </summary>
        [NotMapped]
        public int OccupiedBeds => TotalBeds - AvailableBeds;
    }
}
