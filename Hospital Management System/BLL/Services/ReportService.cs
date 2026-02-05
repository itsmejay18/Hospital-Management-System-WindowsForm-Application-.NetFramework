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
            var key = (reportKey ?? string.Empty).Trim().ToLowerInvariant();
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
    }
}
