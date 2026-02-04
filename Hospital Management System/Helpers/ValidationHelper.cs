using System;
using System.Text.RegularExpressions;

namespace HospitalManagementSystem.Helpers
{
    /// <summary>
    /// Provides common validation helpers.
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Ensures the input is not null or whitespace.
        /// </summary>
        public static bool IsRequired(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Validates a simple email format.
        /// </summary>
        public static bool IsEmail(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            return Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        /// <summary>
        /// Ensures a date is not in the future.
        /// </summary>
        public static bool IsPastOrToday(DateTime date)
        {
            return date.Date <= DateTime.Today;
        }
    }
}
