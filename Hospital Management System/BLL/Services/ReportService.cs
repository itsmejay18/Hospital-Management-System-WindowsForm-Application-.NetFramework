using System.Data;
using System.Threading.Tasks;
using HospitalManagementSystem.DAL;

namespace HospitalManagementSystem.BLL.Services
{
    /// <summary>
    /// Provides reporting data.
    /// </summary>
    public sealed class ReportService
    {
        /// <summary>
        /// Gets report data by key.
        /// </summary>
        public Task<DataTable> GetReportAsync(string reportKey)
        {
            switch (reportKey)
            {
                case "Patients":
                    return DatabaseConnection.Instance.ExecuteQueryAsync("SELECT * FROM Patients");
                case "Appointments":
                    return DatabaseConnection.Instance.ExecuteQueryAsync("SELECT * FROM Appointments");
                case "Billing":
                    return DatabaseConnection.Instance.ExecuteQueryAsync("SELECT * FROM Invoices");
                case "Pharmacy":
                    return DatabaseConnection.Instance.ExecuteQueryAsync("SELECT * FROM PharmacyStockAlert");
                case "DoctorPerformance":
                    return DatabaseConnection.Instance.ExecuteQueryAsync("SELECT * FROM DoctorScheduleView");
                default:
                    return DatabaseConnection.Instance.ExecuteQueryAsync("SELECT * FROM Patients");
            }
        }
    }
}
