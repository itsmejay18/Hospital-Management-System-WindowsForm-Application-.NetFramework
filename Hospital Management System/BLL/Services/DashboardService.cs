using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
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

        public sealed class AppointmentTrendItem
        {
            public AppointmentTrendItem(string monthLabel, int totalAppointments, int completedAppointments)
            {
                MonthLabel = monthLabel;
                TotalAppointments = totalAppointments;
                CompletedAppointments = completedAppointments;
            }

            public string MonthLabel { get; }

            public int TotalAppointments { get; }

            public int CompletedAppointments { get; }
        }

        public sealed class RoleMetricItem
        {
            public RoleMetricItem(string title, decimal value, string subtitle, bool isCurrency = false)
            {
                Title = title;
                Value = value;
                Subtitle = subtitle;
                IsCurrency = isCurrency;
            }

            public string Title { get; }

            public decimal Value { get; }

            public string Subtitle { get; }

            public bool IsCurrency { get; }
        }

        /// <summary>
        /// Gets total patients count.
        /// </summary>
        public Task<int> GetTotalPatientsAsync()
        {
            return ExecuteIntSafeAsync("SELECT COUNT(*) FROM Patients");
        }

        /// <summary>
        /// Gets total doctors count.
        /// </summary>
        public Task<int> GetTotalDoctorsAsync()
        {
            return ExecuteIntSafeAsync("SELECT COUNT(*) FROM Doctors");
        }

        /// <summary>
        /// Gets total paid invoiced amount.
        /// </summary>
        public Task<decimal> GetTotalRevenueAsync()
        {
            return ExecuteDecimalSafeAsync("SELECT COALESCE(SUM(GrandTotal), 0) FROM Invoices WHERE UPPER(COALESCE(Status,'')) = 'PAID'");
        }

        /// <summary>
        /// Gets total appointments count.
        /// </summary>
        public Task<int> GetTotalAppointmentsAsync()
        {
            return ExecuteIntSafeAsync("SELECT COUNT(*) FROM Appointments");
        }

        /// <summary>
        /// Gets today's appointments count.
        /// </summary>
        public Task<int> GetTodayAppointmentsAsync()
        {
            return ExecuteIntSafeAsync("SELECT COUNT(*) FROM Appointments WHERE DATE(AppointmentDate) = CURDATE()");
        }

        /// <summary>
        /// Gets pending operational approvals/actions.
        /// </summary>
        public Task<int> GetPendingApprovalsAsync()
        {
            const string sql = @"
SELECT
    (SELECT COUNT(*) FROM Appointments WHERE UPPER(COALESCE(Status,'')) IN ('SCHEDULED', 'PENDING', 'CONFIRMED', 'RESCHEDULED'))
  + (SELECT COUNT(*) FROM LabOrders WHERE UPPER(COALESCE(Status,'')) IN ('PENDING', 'PROCESSING'))
  + (SELECT COUNT(*) FROM Admissions WHERE UPPER(COALESCE(Status,'')) = 'ADMITTED')
  + (SELECT COUNT(*) FROM Invoices WHERE UPPER(COALESCE(Status,'')) IN ('PENDING', 'UNPAID', 'PARTIAL')) AS TotalPending;";
            return ExecuteIntSafeAsync(sql);
        }

        /// <summary>
        /// Gets current month collected amount from payments.
        /// </summary>
        public Task<decimal> GetCurrentMonthCollectionsAsync()
        {
            const string sql = @"
SELECT COALESCE(SUM(Amount), 0)
FROM Payments
WHERE YEAR(PaymentDate) = YEAR(CURDATE())
  AND MONTH(PaymentDate) = MONTH(CURDATE());";
            return ExecuteDecimalSafeAsync(sql);
        }

        /// <summary>
        /// Gets room occupancy rate in percent.
        /// </summary>
        public Task<decimal> GetRoomOccupancyRateAsync()
        {
            const string sql = @"
SELECT
    CASE
        WHEN COALESCE(SUM(TotalBeds), 0) = 0 THEN 0
        ELSE ROUND(((COALESCE(SUM(TotalBeds), 0) - COALESCE(SUM(AvailableBeds), 0)) / COALESCE(SUM(TotalBeds), 1)) * 100, 2)
    END AS OccupancyRate
FROM Rooms;";
            return ExecuteDecimalSafeAsync(sql);
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

            var table = await QuerySafeAsync(sql).ConfigureAwait(false);
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
        /// Gets key entity distribution for the dashboard pie chart.
        /// </summary>
        public async Task<IList<GenderBreakdownItem>> GetEntityDistributionAsync()
        {
            const string sql = @"
SELECT 'Patients' AS Label, (SELECT COUNT(*) FROM Patients) AS Total
UNION ALL
SELECT 'Doctors', (SELECT COUNT(*) FROM Doctors)
UNION ALL
SELECT 'Appointments', (SELECT COUNT(*) FROM Appointments);";

            var table = await QuerySafeAsync(sql).ConfigureAwait(false);
            var result = new List<GenderBreakdownItem>();
            foreach (DataRow row in table.Rows)
            {
                result.Add(new GenderBreakdownItem(
                    Convert.ToString(row["Label"]) ?? "Unknown",
                    row["Total"] == DBNull.Value ? 0 : Convert.ToInt32(row["Total"])));
            }

            if (result.Count == 0)
            {
                result.Add(new GenderBreakdownItem("No Data", 1));
            }

            return result;
        }

        /// <summary>
        /// Gets paid revenue trend by month for the recent period.
        /// </summary>
        public async Task<IList<RevenueTrendItem>> GetMonthlyRevenueTrendAsync(int monthCount = 6)
        {
            var safeMonthCount = Math.Max(3, Math.Min(12, monthCount));
            const string sql = @"
SELECT DATE_FORMAT(PaymentDate, '%Y-%m') AS YearMonth,
       COALESCE(SUM(Amount), 0) AS TotalAmount
FROM Payments
WHERE PaymentDate >= DATE_SUB(DATE_FORMAT(CURRENT_DATE, '%Y-%m-01'), INTERVAL 11 MONTH)
GROUP BY DATE_FORMAT(PaymentDate, '%Y-%m')
ORDER BY YearMonth;";

            var table = await QuerySafeAsync(sql).ConfigureAwait(false);
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
                trend.Add(new RevenueTrendItem(monthCursor.ToString("yyyy-MM", CultureInfo.InvariantCulture), total));
                monthCursor = monthCursor.AddMonths(1);
            }

            return trend;
        }

        /// <summary>
        /// Gets monthly appointment trend for the dashboard.
        /// </summary>
        public async Task<IList<AppointmentTrendItem>> GetMonthlyAppointmentTrendAsync(int monthCount = 6)
        {
            var safeMonthCount = Math.Max(3, Math.Min(12, monthCount));
            const string sql = @"
SELECT DATE_FORMAT(AppointmentDate, '%Y-%m') AS YearMonth,
       COUNT(*) AS TotalAppointments,
       SUM(CASE WHEN UPPER(COALESCE(Status,'')) = 'COMPLETED' THEN 1 ELSE 0 END) AS CompletedAppointments
FROM Appointments
WHERE AppointmentDate >= DATE_SUB(DATE_FORMAT(CURRENT_DATE, '%Y-%m-01'), INTERVAL 11 MONTH)
GROUP BY DATE_FORMAT(AppointmentDate, '%Y-%m')
ORDER BY YearMonth;";

            var table = await QuerySafeAsync(sql).ConfigureAwait(false);
            var valuesByMonth = new Dictionary<string, AppointmentTrendItem>(StringComparer.OrdinalIgnoreCase);
            foreach (DataRow row in table.Rows)
            {
                var key = Convert.ToString(row["YearMonth"]) ?? string.Empty;
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }

                var total = row["TotalAppointments"] == DBNull.Value ? 0 : Convert.ToInt32(row["TotalAppointments"]);
                var completed = row["CompletedAppointments"] == DBNull.Value ? 0 : Convert.ToInt32(row["CompletedAppointments"]);
                valuesByMonth[key] = new AppointmentTrendItem(key, total, completed);
            }

            var trend = new List<AppointmentTrendItem>();
            var monthCursor = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-(safeMonthCount - 1));
            for (var i = 0; i < safeMonthCount; i++)
            {
                var key = monthCursor.ToString("yyyy-MM", CultureInfo.InvariantCulture);
                if (!valuesByMonth.TryGetValue(key, out var item))
                {
                    item = new AppointmentTrendItem(key, 0, 0);
                }

                trend.Add(item);
                monthCursor = monthCursor.AddMonths(1);
            }

            return trend;
        }

        /// <summary>
        /// Gets role-specific headline metrics for non-admin dashboards.
        /// </summary>
        public async Task<IList<RoleMetricItem>> GetRoleMetricsAsync(string roleName, int userId)
        {
            var role = (roleName ?? string.Empty).Trim().ToLowerInvariant();
            switch (role)
            {
                case "doctor":
                    return await BuildDoctorMetricsAsync(userId).ConfigureAwait(false);
                case "nurse":
                    return await BuildNurseMetricsAsync().ConfigureAwait(false);
                case "receptionist":
                    return await BuildReceptionMetricsAsync().ConfigureAwait(false);
                case "pharmacist":
                    return await BuildPharmacistMetricsAsync().ConfigureAwait(false);
                case "lab technician":
                    return await BuildLabTechnicianMetricsAsync().ConfigureAwait(false);
                case "accountant":
                    return await BuildAccountantMetricsAsync().ConfigureAwait(false);
                case "hr manager":
                    return await BuildHrMetricsAsync().ConfigureAwait(false);
                default:
                    return await BuildFallbackMetricsAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets primary role activity table.
        /// </summary>
        public Task<DataTable> GetRoleActivityAsync(string roleName, int userId)
        {
            var role = (roleName ?? string.Empty).Trim().ToLowerInvariant();
            switch (role)
            {
                case "doctor":
                    return QuerySafeAsync(
                        @"SELECT a.AppointmentCode AS `Code`,
                                 CONCAT(COALESCE(p.FirstName,''), ' ', COALESCE(p.LastName,'')) AS `Patient`,
                                 DATE_FORMAT(a.AppointmentDate, '%Y-%m-%d') AS `Date`,
                                 TIME_FORMAT(a.AppointmentTime, '%H:%i') AS `Time`,
                                 COALESCE(a.Status, 'Scheduled') AS `Status`
                          FROM Appointments a
                          INNER JOIN Doctors d ON d.DoctorID = a.DoctorID
                          LEFT JOIN Patients p ON p.PatientID = a.PatientID
                          WHERE d.UserID = @userId
                            AND a.AppointmentDate >= CURDATE()
                          ORDER BY a.AppointmentDate, a.AppointmentTime
                          LIMIT 12;",
                        WithUserId(userId));
                case "nurse":
                    return QuerySafeAsync(
                        @"SELECT ad.AdmissionNumber AS `Admission #`,
                                 CONCAT(COALESCE(p.FirstName,''), ' ', COALESCE(p.LastName,'')) AS `Patient`,
                                 COALESCE(r.RoomNumber, 'N/A') AS `Room`,
                                 DATE_FORMAT(ad.AdmissionDate, '%Y-%m-%d %H:%i') AS `Admitted At`,
                                 ad.Status AS `Status`
                          FROM Admissions ad
                          LEFT JOIN Patients p ON p.PatientID = ad.PatientID
                          LEFT JOIN Rooms r ON r.RoomID = ad.RoomID
                          WHERE UPPER(COALESCE(ad.Status,'')) = 'ADMITTED'
                          ORDER BY ad.AdmissionDate DESC
                          LIMIT 12;");
                case "receptionist":
                    return QuerySafeAsync(
                        @"SELECT a.AppointmentCode AS `Code`,
                                 CONCAT(COALESCE(p.FirstName,''), ' ', COALESCE(p.LastName,'')) AS `Patient`,
                                 CONCAT(COALESCE(ud.FirstName,''), ' ', COALESCE(ud.LastName,'')) AS `Doctor`,
                                 TIME_FORMAT(a.AppointmentTime, '%H:%i') AS `Time`,
                                 COALESCE(a.Status, 'Scheduled') AS `Status`
                          FROM Appointments a
                          LEFT JOIN Patients p ON p.PatientID = a.PatientID
                          LEFT JOIN Doctors d ON d.DoctorID = a.DoctorID
                          LEFT JOIN Users u ON u.UserID = d.UserID
                          LEFT JOIN UserDetails ud ON ud.UserID = u.UserID
                          WHERE DATE(a.AppointmentDate) = CURDATE()
                          ORDER BY a.AppointmentTime
                          LIMIT 12;");
                case "pharmacist":
                    return QuerySafeAsync(
                        @"SELECT ps.SaleNumber AS `Sale #`,
                                 CONCAT(COALESCE(p.FirstName,''), ' ', COALESCE(p.LastName,'')) AS `Patient`,
                                 DATE_FORMAT(ps.SaleDate, '%Y-%m-%d %H:%i') AS `Date`,
                                 ROUND(COALESCE(ps.NetAmount, 0), 2) AS `Net Amount`,
                                 COALESCE(ps.PaymentStatus, 'Pending') AS `Status`
                          FROM PharmacySales ps
                          LEFT JOIN Patients p ON p.PatientID = ps.PatientID
                          ORDER BY ps.SaleDate DESC
                          LIMIT 12;");
                case "lab technician":
                    return QuerySafeAsync(
                        @"SELECT lo.OrderCode AS `Order #`,
                                 CONCAT(COALESCE(p.FirstName,''), ' ', COALESCE(p.LastName,'')) AS `Patient`,
                                 COALESCE(lt.TestName, 'Lab Test') AS `Test`,
                                 DATE_FORMAT(lo.OrderDate, '%Y-%m-%d %H:%i') AS `Ordered`,
                                 COALESCE(lo.Status, 'Pending') AS `Status`
                          FROM LabOrders lo
                          LEFT JOIN Patients p ON p.PatientID = lo.PatientID
                          LEFT JOIN LabOrderDetails lod ON lod.OrderID = lo.OrderID
                          LEFT JOIN LabTests lt ON lt.TestID = lod.TestID
                          WHERE UPPER(COALESCE(lo.Status,'')) IN ('PENDING', 'IN PROGRESS')
                          ORDER BY lo.OrderDate DESC
                          LIMIT 12;");
                case "accountant":
                    return QuerySafeAsync(
                        @"SELECT i.InvoiceNumber AS `Invoice #`,
                                 CONCAT(COALESCE(p.FirstName,''), ' ', COALESCE(p.LastName,'')) AS `Patient`,
                                 DATE_FORMAT(i.InvoiceDate, '%Y-%m-%d') AS `Date`,
                                 ROUND(COALESCE(i.GrandTotal, 0), 2) AS `Total`,
                                 ROUND(COALESCE(pay.TotalPaid, 0), 2) AS `Paid`,
                                 COALESCE(i.Status, 'Pending') AS `Status`
                          FROM Invoices i
                          LEFT JOIN Patients p ON p.PatientID = i.PatientID
                          LEFT JOIN (
                              SELECT InvoiceID, SUM(Amount) AS TotalPaid
                              FROM Payments
                              GROUP BY InvoiceID
                          ) pay ON pay.InvoiceID = i.InvoiceID
                          ORDER BY i.InvoiceDate DESC
                          LIMIT 12;");
                case "hr manager":
                    return QuerySafeAsync(
                        @"SELECT s.StaffCode AS `Staff Code`,
                                 TRIM(CONCAT(COALESCE(ud.FirstName,''), ' ', COALESCE(ud.LastName,''))) AS `Staff`,
                                 COALESCE(s.Designation, '-') AS `Designation`,
                                 COALESCE(s.Department, '-') AS `Department`,
                                 COALESCE(s.Shift, '-') AS `Shift`,
                                 DATE_FORMAT(s.HireDate, '%Y-%m-%d') AS `Hire Date`
                          FROM Staff s
                          LEFT JOIN Users u ON u.UserID = s.UserID
                          LEFT JOIN UserDetails ud ON ud.UserID = u.UserID
                          ORDER BY s.HireDate DESC
                          LIMIT 12;");
                default:
                    return QuerySafeAsync(
                        @"SELECT AppointmentCode AS `Code`,
                                 DATE_FORMAT(AppointmentDate, '%Y-%m-%d') AS `Date`,
                                 COALESCE(Status, 'Scheduled') AS `Status`
                          FROM Appointments
                          ORDER BY AppointmentDate DESC, AppointmentTime DESC
                          LIMIT 12;");
            }
        }

        /// <summary>
        /// Gets secondary role workload table.
        /// </summary>
        public Task<DataTable> GetRoleQueueAsync(string roleName, int userId)
        {
            var role = (roleName ?? string.Empty).Trim().ToLowerInvariant();
            switch (role)
            {
                case "doctor":
                    return QuerySafeAsync(
                        @"SELECT a.AppointmentCode AS `Code`,
                                 CONCAT(COALESCE(p.FirstName,''), ' ', COALESCE(p.LastName,'')) AS `Patient`,
                                 DATE_FORMAT(a.AppointmentDate, '%Y-%m-%d') AS `Date`,
                                 TIME_FORMAT(a.AppointmentTime, '%H:%i') AS `Time`
                          FROM Appointments a
                          INNER JOIN Doctors d ON d.DoctorID = a.DoctorID
                          LEFT JOIN Patients p ON p.PatientID = a.PatientID
                          WHERE d.UserID = @userId
                            AND UPPER(COALESCE(a.Status,'')) = 'COMPLETED'
                          ORDER BY a.AppointmentDate DESC, a.AppointmentTime DESC
                          LIMIT 12;",
                        WithUserId(userId));
                case "nurse":
                    return QuerySafeAsync(
                        @"SELECT r.RoomNumber AS `Room`,
                                 COALESCE(w.WardName, 'Unassigned') AS `Ward`,
                                 r.TotalBeds AS `Total Beds`,
                                 r.AvailableBeds AS `Available`,
                                 (r.TotalBeds - r.AvailableBeds) AS `Occupied`,
                                 r.Status
                          FROM Rooms r
                          LEFT JOIN Wards w ON w.WardID = r.WardID
                          ORDER BY COALESCE(w.WardName, 'Unassigned'), r.RoomNumber
                          LIMIT 12;");
                case "receptionist":
                    return QuerySafeAsync(
                        @"SELECT i.InvoiceNumber AS `Invoice #`,
                                 CONCAT(COALESCE(p.FirstName,''), ' ', COALESCE(p.LastName,'')) AS `Patient`,
                                 DATE_FORMAT(i.DueDate, '%Y-%m-%d') AS `Due Date`,
                                 ROUND(i.GrandTotal, 2) AS `Total`,
                                 ROUND(COALESCE(pay.TotalPaid, 0), 2) AS `Paid`,
                                 ROUND(i.GrandTotal - COALESCE(pay.TotalPaid, 0), 2) AS `Balance`,
                                 i.Status
                          FROM Invoices i
                          LEFT JOIN Patients p ON p.PatientID = i.PatientID
                          LEFT JOIN (
                              SELECT InvoiceID, SUM(Amount) AS TotalPaid
                              FROM Payments
                              GROUP BY InvoiceID
                          ) pay ON pay.InvoiceID = i.InvoiceID
                          WHERE UPPER(COALESCE(i.Status,'')) IN ('PENDING', 'UNPAID', 'PARTIAL')
                          ORDER BY i.DueDate, i.InvoiceDate
                          LIMIT 12;");
                case "pharmacist":
                    return QuerySafeAsync(
                        @"SELECT m.MedicineCode AS `Code`,
                                 m.MedicineName AS `Medicine`,
                                 COALESCE(SUM(i.Quantity), 0) AS `Stock`,
                                 COALESCE(m.ReorderLevel, 10) AS `Reorder Level`,
                                 DATE_FORMAT(MIN(i.ExpiryDate), '%Y-%m-%d') AS `Nearest Expiry`
                          FROM Medicines m
                          LEFT JOIN Inventory i ON i.MedicineID = m.MedicineID
                          GROUP BY m.MedicineID, m.MedicineCode, m.MedicineName, m.ReorderLevel
                          HAVING COALESCE(SUM(i.Quantity), 0) <= COALESCE(m.ReorderLevel, 10)
                          ORDER BY `Stock` ASC, m.MedicineName
                          LIMIT 12;");
                case "lab technician":
                    return QuerySafeAsync(
                        @"SELECT lo.OrderCode AS `Order #`,
                                 CONCAT(COALESCE(p.FirstName,''), ' ', COALESCE(p.LastName,'')) AS `Patient`,
                                 DATE_FORMAT(COALESCE(lo.ResultDate, MAX(lod.CompletedDate), lo.OrderDate), '%Y-%m-%d %H:%i') AS `Completed`,
                                 COUNT(lod.OrderDetailID) AS `Tests`,
                                 COALESCE(lo.Status, 'Completed') AS `Status`
                          FROM LabOrders lo
                          LEFT JOIN Patients p ON p.PatientID = lo.PatientID
                          LEFT JOIN LabOrderDetails lod ON lod.OrderID = lo.OrderID
                          WHERE UPPER(COALESCE(lo.Status,'')) = 'COMPLETED'
                          GROUP BY lo.OrderID, lo.OrderCode, p.FirstName, p.LastName, lo.ResultDate, lo.OrderDate, lo.Status
                          ORDER BY COALESCE(lo.ResultDate, MAX(lod.CompletedDate), lo.OrderDate) DESC
                          LIMIT 12;");
                case "accountant":
                    return QuerySafeAsync(
                        @"SELECT p.PaymentNumber AS `Payment #`,
                                 COALESCE(i.InvoiceNumber, '-') AS `Invoice #`,
                                 DATE_FORMAT(p.PaymentDate, '%Y-%m-%d %H:%i') AS `Date`,
                                 COALESCE(p.PaymentMethod, 'Cash') AS `Method`,
                                 ROUND(COALESCE(p.Amount, 0), 2) AS `Amount`,
                                 TRIM(CONCAT(COALESCE(ud.FirstName,''), ' ', COALESCE(ud.LastName,''))) AS `Received By`
                          FROM Payments p
                          LEFT JOIN Invoices i ON i.InvoiceID = p.InvoiceID
                          LEFT JOIN Users u ON u.UserID = p.ReceivedBy
                          LEFT JOIN UserDetails ud ON ud.UserID = u.UserID
                          ORDER BY p.PaymentDate DESC
                          LIMIT 12;");
                case "hr manager":
                    return QuerySafeAsync(
                        @"SELECT u.Username AS `Username`,
                                 COALESCE(ur.RoleName, 'User') AS `Role`,
                                 CASE WHEN COALESCE(u.IsActive, 0) = 1 THEN 'Active' ELSE 'Inactive' END AS `Status`,
                                 DATE_FORMAT(u.LastLogin, '%Y-%m-%d %H:%i') AS `Last Login`,
                                 COALESCE(u.Email, '') AS `Email`
                          FROM Users u
                          LEFT JOIN UserRoles ur ON ur.RoleID = u.RoleID
                          ORDER BY u.IsActive DESC, u.LastLogin DESC
                          LIMIT 12;");
                default:
                    return QuerySafeAsync(
                        @"SELECT InvoiceNumber AS `Invoice #`,
                                 ROUND(GrandTotal, 2) AS `Total`,
                                 Status
                          FROM Invoices
                          ORDER BY InvoiceDate DESC
                          LIMIT 12;");
            }
        }

        private async Task<IList<RoleMetricItem>> BuildDoctorMetricsAsync(int userId)
        {
            var metrics = new List<RoleMetricItem>();
            metrics.Add(new RoleMetricItem(
                "Today's Appointments",
                await ExecuteIntSafeAsync(
                    @"SELECT COUNT(*) FROM Appointments a
                      INNER JOIN Doctors d ON d.DoctorID = a.DoctorID
                      WHERE d.UserID = @userId
                        AND DATE(a.AppointmentDate) = CURDATE();",
                    WithUserId(userId)).ConfigureAwait(false),
                "Scheduled for today"));

            metrics.Add(new RoleMetricItem(
                "Pending Follow-ups",
                await ExecuteIntSafeAsync(
                    @"SELECT COUNT(*) FROM Appointments a
                      INNER JOIN Doctors d ON d.DoctorID = a.DoctorID
                      WHERE d.UserID = @userId
                        AND UPPER(COALESCE(a.Status,'')) IN ('SCHEDULED', 'CONFIRMED', 'PENDING')
                        AND a.AppointmentDate >= CURDATE();",
                    WithUserId(userId)).ConfigureAwait(false),
                "Requires review"));

            metrics.Add(new RoleMetricItem(
                "Completed This Month",
                await ExecuteIntSafeAsync(
                    @"SELECT COUNT(*) FROM Appointments a
                      INNER JOIN Doctors d ON d.DoctorID = a.DoctorID
                      WHERE d.UserID = @userId
                        AND UPPER(COALESCE(a.Status,'')) = 'COMPLETED'
                        AND YEAR(a.AppointmentDate) = YEAR(CURDATE())
                        AND MONTH(a.AppointmentDate) = MONTH(CURDATE());",
                    WithUserId(userId)).ConfigureAwait(false),
                "Monthly accomplishment"));

            metrics.Add(new RoleMetricItem(
                "Patients Handled",
                await ExecuteIntSafeAsync(
                    @"SELECT COUNT(DISTINCT a.PatientID) FROM Appointments a
                      INNER JOIN Doctors d ON d.DoctorID = a.DoctorID
                      WHERE d.UserID = @userId;",
                    WithUserId(userId)).ConfigureAwait(false),
                "Unique patients"));
            return metrics;
        }

        private async Task<IList<RoleMetricItem>> BuildNurseMetricsAsync()
        {
            var metrics = new List<RoleMetricItem>();
            metrics.Add(new RoleMetricItem(
                "Admitted Patients",
                await ExecuteIntSafeAsync("SELECT COUNT(*) FROM Admissions WHERE UPPER(COALESCE(Status,'')) = 'ADMITTED'").ConfigureAwait(false),
                "Currently admitted"));
            metrics.Add(new RoleMetricItem(
                "Occupied Rooms",
                await ExecuteIntSafeAsync("SELECT COUNT(*) FROM Rooms WHERE (COALESCE(TotalBeds,0) - COALESCE(AvailableBeds,0)) > 0").ConfigureAwait(false),
                "In active use"));
            metrics.Add(new RoleMetricItem(
                "Discharge Due Today",
                await ExecuteIntSafeAsync("SELECT COUNT(*) FROM Admissions WHERE UPPER(COALESCE(Status,'')) = 'ADMITTED' AND DATE(ExpectedDischargeDate) = CURDATE()").ConfigureAwait(false),
                "Needs discharge prep"));
            metrics.Add(new RoleMetricItem(
                "Open Lab Orders",
                await ExecuteIntSafeAsync("SELECT COUNT(*) FROM LabOrders WHERE UPPER(COALESCE(Status,'')) IN ('PENDING', 'PROCESSING')").ConfigureAwait(false),
                "Awaiting completion"));
            return metrics;
        }

        private async Task<IList<RoleMetricItem>> BuildReceptionMetricsAsync()
        {
            var metrics = new List<RoleMetricItem>();
            metrics.Add(new RoleMetricItem(
                "Registrations Today",
                await ExecuteIntSafeAsync("SELECT COUNT(*) FROM Patients WHERE DATE(RegistrationDate) = CURDATE()").ConfigureAwait(false),
                "New patient records"));
            metrics.Add(new RoleMetricItem(
                "Appointments Today",
                await ExecuteIntSafeAsync("SELECT COUNT(*) FROM Appointments WHERE DATE(AppointmentDate) = CURDATE()").ConfigureAwait(false),
                "Front desk queue"));
            metrics.Add(new RoleMetricItem(
                "Pending Invoices",
                await ExecuteIntSafeAsync("SELECT COUNT(*) FROM Invoices WHERE UPPER(COALESCE(Status,'')) IN ('PENDING', 'UNPAID', 'PARTIAL')").ConfigureAwait(false),
                "Needs processing"));
            metrics.Add(new RoleMetricItem(
                "Payments Today",
                await ExecuteDecimalSafeAsync("SELECT COALESCE(SUM(Amount), 0) FROM Payments WHERE DATE(PaymentDate) = CURDATE()").ConfigureAwait(false),
                "Collected amount",
                isCurrency: true));
            return metrics;
        }

        private async Task<IList<RoleMetricItem>> BuildPharmacistMetricsAsync()
        {
            var metrics = new List<RoleMetricItem>();
            metrics.Add(new RoleMetricItem(
                "Sales Today",
                await ExecuteIntSafeAsync("SELECT COUNT(*) FROM PharmacySales WHERE DATE(SaleDate) = CURDATE()").ConfigureAwait(false),
                "Dispensing transactions"));
            metrics.Add(new RoleMetricItem(
                "Revenue Today",
                await ExecuteDecimalSafeAsync("SELECT COALESCE(SUM(NetAmount), 0) FROM PharmacySales WHERE DATE(SaleDate) = CURDATE()").ConfigureAwait(false),
                "Net pharmacy sales",
                isCurrency: true));
            metrics.Add(new RoleMetricItem(
                "Low Stock Items",
                await ExecuteIntSafeAsync(
                    @"SELECT COUNT(DISTINCT i.MedicineID)
                      FROM Inventory i
                      INNER JOIN Medicines m ON m.MedicineID = i.MedicineID
                      WHERE COALESCE(i.Quantity, 0) <= COALESCE(m.ReorderLevel, 10);").ConfigureAwait(false),
                "Needs replenishment"));
            metrics.Add(new RoleMetricItem(
                "Active Prescriptions",
                await ExecuteIntSafeAsync("SELECT COUNT(*) FROM Prescriptions WHERE UPPER(COALESCE(Status,'')) IN ('ACTIVE', 'PENDING')").ConfigureAwait(false),
                "Ready for dispensing"));
            return metrics;
        }

        private async Task<IList<RoleMetricItem>> BuildLabTechnicianMetricsAsync()
        {
            var metrics = new List<RoleMetricItem>();
            metrics.Add(new RoleMetricItem(
                "Orders Today",
                await ExecuteIntSafeAsync("SELECT COUNT(*) FROM LabOrders WHERE DATE(OrderDate) = CURDATE()").ConfigureAwait(false),
                "Incoming requests"));
            metrics.Add(new RoleMetricItem(
                "Pending Tests",
                await ExecuteIntSafeAsync("SELECT COUNT(*) FROM LabOrders WHERE UPPER(COALESCE(Status,'')) IN ('PENDING', 'IN PROGRESS')").ConfigureAwait(false),
                "Work in queue"));
            metrics.Add(new RoleMetricItem(
                "Completed Today",
                await ExecuteIntSafeAsync("SELECT COUNT(*) FROM LabOrders WHERE UPPER(COALESCE(Status,'')) = 'COMPLETED' AND DATE(COALESCE(ResultDate, OrderDate)) = CURDATE()").ConfigureAwait(false),
                "Released results"));
            metrics.Add(new RoleMetricItem(
                "Month Test Revenue",
                await ExecuteDecimalSafeAsync(
                    @"SELECT COALESCE(SUM(lt.Price), 0)
                      FROM LabOrderDetails lod
                      INNER JOIN LabOrders lo ON lo.OrderID = lod.OrderID
                      INNER JOIN LabTests lt ON lt.TestID = lod.TestID
                      WHERE YEAR(COALESCE(lod.CompletedDate, lo.ResultDate, lo.OrderDate)) = YEAR(CURDATE())
                        AND MONTH(COALESCE(lod.CompletedDate, lo.ResultDate, lo.OrderDate)) = MONTH(CURDATE());").ConfigureAwait(false),
                "Completed testing",
                isCurrency: true));
            return metrics;
        }

        private async Task<IList<RoleMetricItem>> BuildAccountantMetricsAsync()
        {
            var metrics = new List<RoleMetricItem>();
            metrics.Add(new RoleMetricItem(
                "Open Invoices",
                await ExecuteIntSafeAsync("SELECT COUNT(*) FROM Invoices WHERE UPPER(COALESCE(Status,'')) IN ('PENDING', 'PARTIAL', 'UNPAID')").ConfigureAwait(false),
                "Awaiting settlement"));
            metrics.Add(new RoleMetricItem(
                "Due Today",
                await ExecuteIntSafeAsync("SELECT COUNT(*) FROM Invoices WHERE DATE(DueDate) = CURDATE() AND UPPER(COALESCE(Status,'')) IN ('PENDING', 'PARTIAL', 'UNPAID')").ConfigureAwait(false),
                "Needs follow-up"));
            metrics.Add(new RoleMetricItem(
                "Collections Today",
                await ExecuteDecimalSafeAsync("SELECT COALESCE(SUM(Amount), 0) FROM Payments WHERE DATE(PaymentDate) = CURDATE()").ConfigureAwait(false),
                "Cash in",
                isCurrency: true));
            metrics.Add(new RoleMetricItem(
                "Outstanding Balance",
                await ExecuteDecimalSafeAsync(
                    @"SELECT COALESCE(SUM(i.GrandTotal - COALESCE(pay.TotalPaid, 0)), 0)
                      FROM Invoices i
                      LEFT JOIN (
                          SELECT InvoiceID, SUM(Amount) AS TotalPaid
                          FROM Payments
                          GROUP BY InvoiceID
                      ) pay ON pay.InvoiceID = i.InvoiceID
                      WHERE UPPER(COALESCE(i.Status,'')) IN ('PENDING', 'PARTIAL', 'UNPAID');").ConfigureAwait(false),
                "Uncollected",
                isCurrency: true));
            return metrics;
        }

        private async Task<IList<RoleMetricItem>> BuildHrMetricsAsync()
        {
            var metrics = new List<RoleMetricItem>();
            metrics.Add(new RoleMetricItem(
                "Active Staff",
                await ExecuteIntSafeAsync("SELECT COUNT(*) FROM Staff").ConfigureAwait(false),
                "Registered personnel"));
            metrics.Add(new RoleMetricItem(
                "New Hires (Month)",
                await ExecuteIntSafeAsync("SELECT COUNT(*) FROM Staff WHERE YEAR(HireDate) = YEAR(CURDATE()) AND MONTH(HireDate) = MONTH(CURDATE())").ConfigureAwait(false),
                "This month"));
            metrics.Add(new RoleMetricItem(
                "Active Accounts",
                await ExecuteIntSafeAsync("SELECT COUNT(*) FROM Users WHERE COALESCE(IsActive, 0) = 1").ConfigureAwait(false),
                "Can sign in"));
            metrics.Add(new RoleMetricItem(
                "Doctors on Staff",
                await ExecuteIntSafeAsync(
                    @"SELECT COUNT(*)
                      FROM Users u
                      INNER JOIN UserRoles ur ON ur.RoleID = u.RoleID
                      WHERE UPPER(REPLACE(COALESCE(ur.RoleName, ''), ' ', '')) = 'DOCTOR'
                        AND COALESCE(u.IsActive, 0) = 1;").ConfigureAwait(false),
                "Active physicians"));
            return metrics;
        }

        private async Task<IList<RoleMetricItem>> BuildFallbackMetricsAsync()
        {
            return new List<RoleMetricItem>
            {
                new RoleMetricItem("Total Patients", await GetTotalPatientsAsync().ConfigureAwait(false), "Active records"),
                new RoleMetricItem("Total Doctors", await GetTotalDoctorsAsync().ConfigureAwait(false), "Medical staff"),
                new RoleMetricItem("Appointments Today", await GetTodayAppointmentsAsync().ConfigureAwait(false), "Today's queue"),
                new RoleMetricItem("Current Month", await GetCurrentMonthCollectionsAsync().ConfigureAwait(false), "Collections", true)
            };
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

        private static IDictionary<string, object> WithUserId(int userId)
        {
            return new Dictionary<string, object> { { "@userId", userId } };
        }

        private async Task<DataTable> QuerySafeAsync(string sql, IDictionary<string, object> parameters = null)
        {
            try
            {
                return await DatabaseConnection.Instance.ExecuteQueryAsync(sql, parameters).ConfigureAwait(false);
            }
            catch
            {
                return new DataTable();
            }
        }

        private async Task<int> ExecuteIntSafeAsync(string sql, IDictionary<string, object> parameters = null)
        {
            try
            {
                var result = await DatabaseConnection.Instance.ExecuteScalarAsync(sql, parameters).ConfigureAwait(false);
                return result == null ? 0 : Convert.ToInt32(result);
            }
            catch
            {
                return 0;
            }
        }

        private async Task<decimal> ExecuteDecimalSafeAsync(string sql, IDictionary<string, object> parameters = null)
        {
            try
            {
                var result = await DatabaseConnection.Instance.ExecuteScalarAsync(sql, parameters).ConfigureAwait(false);
                return result == null ? 0m : Convert.ToDecimal(result);
            }
            catch
            {
                return 0m;
            }
        }
    }
}
