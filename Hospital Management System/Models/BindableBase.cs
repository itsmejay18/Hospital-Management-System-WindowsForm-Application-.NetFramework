using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HospitalManagementSystem.Models
{
    /// <summary>
    /// Provides INotifyPropertyChanged support for data binding.
    /// </summary>
    public abstract class BindableBase : INotifyPropertyChanged
    {
        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Sets the field and raises PropertyChanged when needed.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="field">Backing field.</param>
        /// <param name="value">New value.</param>
        /// <param name="propertyName">Property name.</param>
        /// <returns>True if value changed.</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Raises PropertyChanged.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
