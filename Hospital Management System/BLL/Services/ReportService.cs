using System.Data;
using System.Linq;
using System.Threading.Tasks;
using HospitalManagementSystem.DAL;
using System.Collections.Generic;

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
            var key = NormalizeKey(reportKey);
            switch (key)
            {
                case "patients":
                    return DatabaseConnection.Instance.ExecuteQueryAsync(
                        @"SELECT PatientCode AS `Patient Code`,
                                 CONCAT(FirstName, ' ', LastName) AS `Patient Name`,
                                 DateOfBirth AS `Date Of Birth`,
                                 Gender,
                                 BloodGroup AS `Blood Group`,
                                 RegistrationDate AS `Registered At`,
                                 IsActive AS `Active`
                          FROM Patients
                          ORDER BY RegistrationDate DESC");
                case "appointments":
                    return DatabaseConnection.Instance.ExecuteQueryAsync(
                        @"SELECT a.AppointmentCode AS `Appointment Code`,
                                 CONCAT(p.FirstName, ' ', p.LastName) AS `Patient`,
                                 CONCAT(ud.FirstName, ' ', ud.LastName) AS `Doctor`,
                                 a.AppointmentDate AS `Date`,
                                 a.AppointmentTime AS `Time`,
                                 a.AppointmentType AS `Type`,
                                 a.Status,
                                 a.Reason
                          FROM Appointments a
                          LEFT JOIN Patients p ON p.PatientID = a.PatientID
                          LEFT JOIN Doctors d ON d.DoctorID = a.DoctorID
                          LEFT JOIN Users u ON u.UserID = d.UserID
                          LEFT JOIN UserDetails ud ON ud.UserID = u.UserID
                          ORDER BY a.AppointmentDate DESC, a.AppointmentTime DESC");
                case "billing":
                    return DatabaseConnection.Instance.ExecuteQueryAsync(
                        @"SELECT i.InvoiceNumber AS `Invoice #`,
                                 CONCAT(p.FirstName, ' ', p.LastName) AS `Patient`,
                                 i.InvoiceDate AS `Invoice Date`,
                                 i.DueDate AS `Due Date`,
                                 i.GrandTotal AS `Grand Total`,
                                 i.Status
                          FROM Invoices i
                          LEFT JOIN Patients p ON p.PatientID = i.PatientID
                          ORDER BY i.InvoiceDate DESC");
                case "pharmacy":
                    return DatabaseConnection.Instance.ExecuteQueryAsync(
                        @"SELECT m.MedicineCode AS `Medicine Code`,
                                 m.MedicineName AS `Medicine`,
                                 m.GenericName AS `Generic`,
                                 COALESCE(SUM(i.Quantity), 0) AS `Stock Qty`,
                                 m.ReorderLevel AS `Reorder Level`,
                                 ROUND(m.SellingPrice, 2) AS `Selling Price`,
                                 MIN(i.ExpiryDate) AS `Nearest Expiry`
                          FROM Medicines m
                          LEFT JOIN Inventory i ON i.MedicineID = m.MedicineID
                          GROUP BY m.MedicineID, m.MedicineCode, m.MedicineName, m.GenericName, m.ReorderLevel, m.SellingPrice
                          ORDER BY m.MedicineName");
                case "doctorperformance":
                    return DatabaseConnection.Instance.ExecuteQueryAsync(
                        @"SELECT d.DoctorCode AS `Doctor Code`,
                                 CONCAT(ud.FirstName, ' ', ud.LastName) AS `Doctor`,
                                 s.SpecializationName AS `Specialization`,
                                 d.ConsultationFee AS `Consultation Fee`,
                                 COUNT(a.AppointmentID) AS `Total Appointments`,
                                 SUM(CASE WHEN a.Status = 'Completed' THEN 1 ELSE 0 END) AS `Completed`
                          FROM Doctors d
                          LEFT JOIN Users u ON u.UserID = d.UserID
                          LEFT JOIN UserDetails ud ON ud.UserID = u.UserID
                          LEFT JOIN Specializations s ON s.SpecializationID = d.SpecializationID
                          LEFT JOIN Appointments a ON a.DoctorID = d.DoctorID
                          GROUP BY d.DoctorID, d.DoctorCode, ud.FirstName, ud.LastName, s.SpecializationName, d.ConsultationFee
                          ORDER BY `Total Appointments` DESC, `Doctor`");
                case "roomoccupancy":
                    return DatabaseConnection.Instance.ExecuteQueryAsync(
                        @"SELECT r.RoomNumber AS `Room`,
                                 COALESCE(w.WardName, 'Unassigned') AS `Ward`,
                                 COALESCE(r.RoomType, 'General') AS `Type`,
                                 r.TotalBeds AS `Total Beds`,
                                 r.AvailableBeds AS `Available Beds`,
                                 (r.TotalBeds - r.AvailableBeds) AS `Occupied Beds`,
                                 r.Status
                          FROM Rooms r
                          LEFT JOIN Wards w ON w.WardID = r.WardID
                          ORDER BY COALESCE(w.WardName, 'Unassigned'), r.RoomNumber");
                case "doctorschedules":
                    return DatabaseConnection.Instance.ExecuteQueryAsync(
                        @"SELECT d.DoctorCode AS `Doctor Code`,
                                 TRIM(CONCAT(COALESCE(ud.FirstName, ''), ' ', COALESCE(ud.LastName, ''))) AS `Doctor`,
                                 CASE ds.DayOfWeek
                                     WHEN 1 THEN 'Monday'
                                     WHEN 2 THEN 'Tuesday'
                                     WHEN 3 THEN 'Wednesday'
                                     WHEN 4 THEN 'Thursday'
                                     WHEN 5 THEN 'Friday'
                                     WHEN 6 THEN 'Saturday'
                                     WHEN 7 THEN 'Sunday'
                                     ELSE CONCAT('Day ', ds.DayOfWeek)
                                 END AS `Day`,
                                 ds.StartTime AS `Start Time`,
                                 ds.EndTime AS `End Time`,
                                 ds.MaxAppointments AS `Max Appointments`,
                                 ds.IsActive AS `Active`
                          FROM DoctorSchedules ds
                          INNER JOIN Doctors d ON d.DoctorID = ds.DoctorID
                          LEFT JOIN Users u ON u.UserID = d.UserID
                          LEFT JOIN UserDetails ud ON ud.UserID = u.UserID
                          ORDER BY `Doctor`, ds.DayOfWeek, ds.StartTime");
                case "paymentsperpatient":
                    return DatabaseConnection.Instance.ExecuteQueryAsync(
                        @"SELECT p.PatientCode AS `Patient Code`,
                                 CONCAT(p.FirstName, ' ', p.LastName) AS `Patient`,
                                 COALESCE(inv.InvoiceCount, 0) AS `Total Invoices`,
                                 COALESCE(inv.TotalInvoiced, 0) AS `Invoiced Amount`,
                                 COALESCE(pay.TotalPaid, 0) AS `Total Paid`,
                                 (COALESCE(inv.TotalInvoiced, 0) - COALESCE(pay.TotalPaid, 0)) AS `Balance`
                          FROM Patients p
                          LEFT JOIN (
                              SELECT i.PatientID,
                                     COUNT(*) AS InvoiceCount,
                                     SUM(i.GrandTotal) AS TotalInvoiced
                              FROM Invoices i
                              GROUP BY i.PatientID
                          ) inv ON inv.PatientID = p.PatientID
                          LEFT JOIN (
                              SELECT i.PatientID,
                                     SUM(pm.Amount) AS TotalPaid
                              FROM Payments pm
                              INNER JOIN Invoices i ON i.InvoiceID = pm.InvoiceID
                              GROUP BY i.PatientID
                          ) pay ON pay.PatientID = p.PatientID
                          ORDER BY `Balance` DESC, `Patient`");
                case "patientmedicines":
                    return DatabaseConnection.Instance.ExecuteQueryAsync(
                        @"SELECT ps.SaleNumber AS `Sale Number`,
                                 ps.SaleDate AS `Sale Date`,
                                 CONCAT(COALESCE(p.FirstName, ''), ' ', COALESCE(p.LastName, '')) AS `Patient`,
                                 m.MedicineCode AS `Medicine Code`,
                                 m.MedicineName AS `Medicine`,
                                 psd.Quantity AS `Qty`,
                                 psd.UnitPrice AS `Unit Price`,
                                 psd.TotalPrice AS `Line Total`,
                                 ps.PaymentStatus AS `Payment Status`
                          FROM PharmacySales ps
                          LEFT JOIN Patients p ON p.PatientID = ps.PatientID
                          LEFT JOIN PharmacySaleDetails psd ON psd.SaleID = ps.SaleID
                          LEFT JOIN Medicines m ON m.MedicineID = psd.MedicineID
                          ORDER BY ps.SaleDate DESC, ps.SaleID DESC");
                case "laboratorybilling":
                    return DatabaseConnection.Instance.ExecuteQueryAsync(
                        @"SELECT lo.OrderCode AS `Lab Order`,
                                 lo.OrderDate AS `Order Date`,
                                 CONCAT(COALESCE(p.FirstName, ''), ' ', COALESCE(p.LastName, '')) AS `Patient`,
                                 lt.TestCode AS `Test Code`,
                                 lt.TestName AS `Test`,
                                 lt.Price AS `Test Price`,
                                 lo.Status AS `Order Status`
                          FROM LabOrders lo
                          LEFT JOIN Patients p ON p.PatientID = lo.PatientID
                          LEFT JOIN LabOrderDetails lod ON lod.OrderID = lo.OrderID
                          LEFT JOIN LabTests lt ON lt.TestID = lod.TestID
                          ORDER BY lo.OrderDate DESC, lo.OrderID DESC");
                case "statisticalsummary":
                    return DatabaseConnection.Instance.ExecuteQueryAsync(
                        @"SELECT
                              (SELECT COUNT(*) FROM Patients WHERE IsActive = 1) AS `Active Patients`,
                              (SELECT COUNT(*) FROM Rooms) AS `Total Rooms`,
                              (SELECT COUNT(*) FROM Admissions WHERE Status = 'Admitted') AS `Current Room Occupants`,
                              (SELECT COUNT(*) FROM Appointments WHERE AppointmentDate = CURDATE()) AS `Today Appointments`,
                              (SELECT COALESCE(SUM(GrandTotal), 0) FROM Invoices WHERE DATE(InvoiceDate) = CURDATE()) AS `Today Invoiced`,
                              (SELECT COALESCE(SUM(Amount), 0) FROM Payments WHERE DATE(PaymentDate) = CURDATE()) AS `Today Payments`,
                              (SELECT COALESCE(SUM(NetAmount), 0) FROM PharmacySales WHERE DATE(SaleDate) = CURDATE()) AS `Today Medicine Sales`,
                              (SELECT COUNT(*) FROM LabOrders WHERE DATE(OrderDate) = CURDATE()) AS `Today Lab Orders`");
                default:
                    return DatabaseConnection.Instance.ExecuteQueryAsync(
                        @"SELECT PatientCode AS `Patient Code`,
                                 CONCAT(FirstName, ' ', LastName) AS `Patient Name`,
                                 DateOfBirth AS `Date Of Birth`,
                                 Gender,
                                 BloodGroup AS `Blood Group`
                          FROM Patients
                          ORDER BY RegistrationDate DESC");
            }
        }

        /// <summary>
        /// Gets recent appointment updates for dashboard presentation.
        /// </summary>
        public Task<DataTable> GetRecentAppointmentUpdatesAsync(int maxRows = 8)
        {
            var safeRows = maxRows <= 0 ? 8 : maxRows;
            return DatabaseConnection.Instance.ExecuteQueryAsync(
                @"SELECT
                      'Appointment' AS `Type`,
                      a.AppointmentCode AS `ReferenceNo`,
                      COALESCE(a.Status, 'Pending') AS `Status`,
                      DATE_FORMAT(CONCAT(a.AppointmentDate, ' ', COALESCE(a.AppointmentTime, '00:00:00')), '%Y-%m-%d %H:%i') AS `UpdatedAt`
                  FROM Appointments a
                  ORDER BY a.AppointmentDate DESC, a.AppointmentTime DESC
                  LIMIT @take;",
                new Dictionary<string, object> { { "@take", safeRows } });
        }

        /// <summary>
        /// Gets staff performance snapshot for dashboard presentation.
        /// </summary>
        public Task<DataTable> GetStaffPerformanceSnapshotAsync(int maxRows = 8)
        {
            var safeRows = maxRows <= 0 ? 8 : maxRows;
            return DatabaseConnection.Instance.ExecuteQueryAsync(
                @"SELECT
                      TRIM(CONCAT(COALESCE(ud.FirstName, ''), ' ', COALESCE(ud.LastName, ''))) AS `Staff`,
                      COALESCE(ur.RoleName, 'Staff') AS `Role`,
                      COUNT(a.AppointmentID) AS `Consultations`,
                      SUM(CASE WHEN COALESCE(a.Status, '') = 'Completed' THEN 1 ELSE 0 END) AS `Completed`,
                      SUM(CASE WHEN COALESCE(a.Status, '') = 'Cancelled' THEN 1 ELSE 0 END) AS `Overdue`,
                      ROUND(COALESCE(SUM(CASE WHEN COALESCE(a.Status, '') = 'Completed' THEN d.ConsultationFee ELSE 0 END), 0), 2) AS `Revenue`,
                      SUM(CASE WHEN COALESCE(a.Status, '') IN ('Pending', 'Scheduled', 'Rescheduled') THEN 1 ELSE 0 END) AS `Pending`
                  FROM Doctors d
                  LEFT JOIN Users u ON u.UserID = d.UserID
                  LEFT JOIN UserDetails ud ON ud.UserID = u.UserID
                  LEFT JOIN UserRoles ur ON ur.RoleID = u.RoleID
                  LEFT JOIN Appointments a ON a.DoctorID = d.DoctorID
                  GROUP BY d.DoctorID, ud.FirstName, ud.LastName, ur.RoleName
                  ORDER BY `Completed` DESC, `Revenue` DESC, `Staff`
                  LIMIT @take;",
                new Dictionary<string, object> { { "@take", safeRows } });
        }

        private static string NormalizeKey(string reportKey)
        {
            return new string((reportKey ?? string.Empty)
                .Trim()
                .ToLowerInvariant()
                .Where(char.IsLetterOrDigit)
                .ToArray());
        }
    }
}
