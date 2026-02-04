using System.Collections.Generic;

namespace HospitalManagementSystem.DAL
{
    /// <summary>
    /// Represents a paged query result.
    /// </summary>
    /// <typeparam name="T">Type of items returned.</typeparam>
    public sealed class PagedResult<T>
    {
        /// <summary>
        /// Gets or sets the items for the current page.
        /// </summary>
        public IReadOnlyList<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// Gets or sets the total item count across all pages.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the current page number (1-based).
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        public int PageSize { get; set; }
    }
}
