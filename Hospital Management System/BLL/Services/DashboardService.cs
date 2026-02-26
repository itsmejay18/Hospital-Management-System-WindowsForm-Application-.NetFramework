using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using HospitalManagementSystem.DAL;

namespace HospitalManagementSystem.BLL.Services
{
    /// <summary>
    /// Provides dashboard metrics.
    /// </summary>
    public sealed class DashboardService
    {
        public sealed class GenderBreakdownItem
        {
            public GenderBreakdownItem(string label, int total)
            {
                Label = label;
                Total = total;
            }

            public string Label { get; }

            public int Total { get; }
        }

        public sealed class RevenueTrendItem
        {
            public RevenueTrendItem(string monthLabel, decimal totalAmount)
            {
                MonthLabel = monthLabel;
                TotalAmount = totalAmount;
            }

            public string MonthLabel { get; }

            public decimal TotalAmount { get; }
        }

        /// <summary>
        /// Gets total patients count.
        /// </summary>
        public async Task<int> GetTotalPatientsAsync()
        {
            var sql = "SELECT COUNT(*) FROM Patients";
            var result = await DatabaseConnection.Instance.ExecuteScalarAsync(sql).ConfigureAwait(false);
            return result == null ? 0 : System.Convert.ToInt32(result);
        }

        /// <summary>
        /// Gets total doctors count.
        /// </summary>
        public async Task<int> GetTotalDoctorsAsync()
        {
            var sql = "SELECT COUNT(*) FROM Doctors";
            var result = await DatabaseConnection.Instance.ExecuteScalarAsync(sql).ConfigureAwait(false);
            return result == null ? 0 : System.Convert.ToInt32(result);
        }

        /// <summary>
        /// Gets total revenue.
        /// </summary>
        public async Task<decimal> GetTotalRevenueAsync()
        {
            var sql = "SELECT COALESCE(SUM(GrandTotal), 0) FROM Invoices WHERE Status = 'Paid'";
            var result = await DatabaseConnection.Instance.ExecuteScalarAsync(sql).ConfigureAwait(false);
            return result == null ? 0m : System.Convert.ToDecimal(result);
        }

        /// <summary>
        /// Gets total appointments count.
        /// </summary>
        public async Task<int> GetTotalAppointmentsAsync()
        {
            var sql = "SELECT COUNT(*) FROM Appointments";
            var result = await DatabaseConnection.Instance.ExecuteScalarAsync(sql).ConfigureAwait(false);
            return result == null ? 0 : Convert.ToInt32(result);
        }

        /// <summary>
        /// Gets patient distribution grouped by gender.
        /// </summary>
        public async Task<IList<GenderBreakdownItem>> GetPatientGenderDistributionAsync()
        {
            const string sql = @"
SELECT COALESCE(NULLIF(Gender, ''), 'U') AS GenderCode, COUNT(*) AS Total
FROM Patients
GROUP BY COALESCE(NULLIF(Gender, ''), 'U')
ORDER BY Total DESC;";

            var table = await DatabaseConnection.Instance.ExecuteQueryAsync(sql).ConfigureAwait(false);
            var distribution = new List<GenderBreakdownItem>();

            foreach (DataRow row in table.Rows)
            {
                var code = row["GenderCode"] == DBNull.Value ? string.Empty : Convert.ToString(row["GenderCode"]);
                var label = NormalizeGenderLabel(code);
                var total = row["Total"] == DBNull.Value ? 0 : Convert.ToInt32(row["Total"]);
                distribution.Add(new GenderBreakdownItem(label, total));
            }

            if (distribution.Count == 0)
            {
                distribution.Add(new GenderBreakdownItem("No Data", 1));
            }

            return distribution;
        }

        /// <summary>
        /// Gets paid revenue trend by month for the recent period.
        /// </summary>
        public async Task<IList<RevenueTrendItem>> GetMonthlyRevenueTrendAsync(int monthCount = 6)
        {
            var safeMonthCount = Math.Max(3, Math.Min(12, monthCount));
            const string sql = @"
SELECT DATE_FORMAT(InvoiceDate, '%Y-%m') AS YearMonth,
       COALESCE(SUM(GrandTotal), 0) AS TotalAmount
FROM Invoices
WHERE Status = 'Paid'
  AND InvoiceDate >= DATE_SUB(DATE_FORMAT(CURRENT_DATE, '%Y-%m-01'), INTERVAL 11 MONTH)
GROUP BY DATE_FORMAT(InvoiceDate, '%Y-%m')
ORDER BY YearMonth;";

            var table = await DatabaseConnection.Instance.ExecuteQueryAsync(sql).ConfigureAwait(false);
            var totalsByMonth = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

            foreach (DataRow row in table.Rows)
            {
                var monthKey = row["YearMonth"] == DBNull.Value
                    ? string.Empty
                    : Convert.ToString(row["YearMonth"]);

                if (string.IsNullOrWhiteSpace(monthKey))
                {
                    continue;
                }

                var amount = row["TotalAmount"] == DBNull.Value ? 0m : Convert.ToDecimal(row["TotalAmount"]);
                totalsByMonth[monthKey] = amount;
            }

            var trend = new List<RevenueTrendItem>();
            var monthCursor = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-(safeMonthCount - 1));
            for (var i = 0; i < safeMonthCount; i++)
            {
                var key = monthCursor.ToString("yyyy-MM", CultureInfo.InvariantCulture);
                totalsByMonth.TryGetValue(key, out var total);
                trend.Add(new RevenueTrendItem(monthCursor.ToString("MMM yyyy", CultureInfo.InvariantCulture), total));
                monthCursor = monthCursor.AddMonths(1);
            }

            return trend;
        }

        private static string NormalizeGenderLabel(string genderCode)
        {
            switch ((genderCode ?? string.Empty).Trim().ToUpperInvariant())
            {
                case "M":
                    return "Male";
                case "F":
                    return "Female";
                case "O":
                    return "Other";
                default:
                    return "Unknown";
            }
        }
    }
}
