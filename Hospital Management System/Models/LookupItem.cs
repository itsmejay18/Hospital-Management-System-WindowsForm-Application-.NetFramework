namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Generic key/value item for combo-box lookups.
    /// </summary>
    public sealed class LookupItem : BindableBase
    {
        private int _id;
        private string _name;

        /// <summary>
        /// Gets or sets the numeric identifier.
        /// </summary>
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        /// <summary>
        /// Returns display text for list controls.
        /// </summary>
        public override string ToString()
        {
            return Name ?? string.Empty;
        }
    }
}
