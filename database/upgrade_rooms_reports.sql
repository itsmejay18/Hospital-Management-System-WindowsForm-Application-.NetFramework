-- Upgrade script: room module + reporting views
-- Run against HospitalManagementSystem database.

USE HospitalManagementSystem;

CREATE TABLE IF NOT EXISTS Wards (
    WardID INT PRIMARY KEY AUTO_INCREMENT,
    WardName VARCHAR(100) NOT NULL,
    WardType VARCHAR(50),
    Description VARCHAR(255),
    TotalBeds INT DEFAULT 0,
    AvailableBeds INT DEFAULT 0,
    ChargePerDay DECIMAL(10,2),
    IsActive BOOLEAN DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS Rooms (
    RoomID INT PRIMARY KEY AUTO_INCREMENT,
    RoomNumber VARCHAR(20) UNIQUE NOT NULL,
    WardID INT NULL,
    RoomType VARCHAR(50),
    TotalBeds INT DEFAULT 1,
    AvailableBeds INT DEFAULT 0,
    Facilities TEXT,
    RatePerDay DECIMAL(10,2),
    Status VARCHAR(20) DEFAULT 'Available',
    CONSTRAINT fk_rooms_ward
        FOREIGN KEY (WardID)
        REFERENCES Wards(WardID)
        ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS Admissions (
    AdmissionID INT PRIMARY KEY AUTO_INCREMENT,
    AdmissionNumber VARCHAR(30) UNIQUE NOT NULL,
    PatientID INT NOT NULL,
    DoctorID INT NOT NULL,
    RoomID INT NULL,
    AdmissionDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    ExpectedDischargeDate DATE,
    ActualDischargeDate DATETIME,
    AdmissionReason TEXT,
    Diagnosis TEXT,
    Status ENUM('Admitted', 'Discharged', 'Transferred') DEFAULT 'Admitted',
    DischargeSummary TEXT,
    CONSTRAINT fk_admission_patient
        FOREIGN KEY (PatientID)
        REFERENCES Patients(PatientID)
        ON DELETE CASCADE,
    CONSTRAINT fk_admission_doctor
        FOREIGN KEY (DoctorID)
        REFERENCES Doctors(DoctorID)
        ON DELETE CASCADE,
    CONSTRAINT fk_admission_room
        FOREIGN KEY (RoomID)
        REFERENCES Rooms(RoomID)
        ON DELETE SET NULL
);

CREATE OR REPLACE VIEW vw_room_occupancy AS
SELECT
    r.RoomID,
    r.RoomNumber,
    COALESCE(w.WardName, 'Unassigned') AS WardName,
    COALESCE(r.RoomType, 'General') AS RoomType,
    r.TotalBeds,
    r.AvailableBeds,
    (r.TotalBeds - r.AvailableBeds) AS OccupiedBeds,
    r.Status
FROM Rooms r
LEFT JOIN Wards w ON w.WardID = r.WardID;

CREATE OR REPLACE VIEW vw_doctor_schedule AS
SELECT
    d.DoctorID,
    d.DoctorCode,
    TRIM(CONCAT(COALESCE(ud.FirstName, ''), ' ', COALESCE(ud.LastName, ''))) AS DoctorName,
    ds.DayOfWeek,
    ds.StartTime,
    ds.EndTime,
    ds.MaxAppointments,
    ds.IsActive
FROM DoctorSchedules ds
INNER JOIN Doctors d ON d.DoctorID = ds.DoctorID
LEFT JOIN Users u ON u.UserID = d.UserID
LEFT JOIN UserDetails ud ON ud.UserID = u.UserID;

CREATE OR REPLACE VIEW vw_patient_payments AS
SELECT
    p.PatientID,
    p.PatientCode,
    CONCAT(p.FirstName, ' ', p.LastName) AS PatientName,
    COALESCE(inv.InvoiceCount, 0) AS TotalInvoices,
    COALESCE(inv.TotalInvoiced, 0) AS InvoicedAmount,
    COALESCE(pay.TotalPaid, 0) AS TotalPaid,
    (COALESCE(inv.TotalInvoiced, 0) - COALESCE(pay.TotalPaid, 0)) AS Balance
FROM Patients p
LEFT JOIN (
    SELECT PatientID, COUNT(*) AS InvoiceCount, SUM(GrandTotal) AS TotalInvoiced
    FROM Invoices
    GROUP BY PatientID
) inv ON inv.PatientID = p.PatientID
LEFT JOIN (
    SELECT i.PatientID, SUM(pm.Amount) AS TotalPaid
    FROM Payments pm
    INNER JOIN Invoices i ON i.InvoiceID = pm.InvoiceID
    GROUP BY i.PatientID
) pay ON pay.PatientID = p.PatientID;
