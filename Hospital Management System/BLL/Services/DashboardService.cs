using System.Threading.Tasks;
using HospitalManagementSystem.DAL;

namespace HospitalManagementSystem.BLL.Services
{
    /// <summary>
    /// Provides dashboard metrics.
    /// </summary>
    public sealed class DashboardService
    {
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
    }
}
