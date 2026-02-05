-- Seed script for HospitalManagementSystem
-- Purpose: ensure every table has at least one row (idempotent inserts).
-- Run after schema creation:
--   mysql -u root -p HospitalManagementSystem < database/seed_all_tables.sql

USE HospitalManagementSystem;

-- 1) Roles
INSERT INTO UserRoles (RoleName, Description)
SELECT 'Administrator', 'Full system access'
WHERE NOT EXISTS (SELECT 1 FROM UserRoles WHERE RoleName = 'Administrator');

INSERT INTO UserRoles (RoleName, Description)
SELECT 'Doctor', 'Medical staff'
WHERE NOT EXISTS (SELECT 1 FROM UserRoles WHERE RoleName = 'Doctor');

INSERT INTO UserRoles (RoleName, Description)
SELECT 'Nurse', 'Nursing staff'
WHERE NOT EXISTS (SELECT 1 FROM UserRoles WHERE RoleName = 'Nurse');

INSERT INTO UserRoles (RoleName, Description)
SELECT 'Receptionist', 'Front desk'
WHERE NOT EXISTS (SELECT 1 FROM UserRoles WHERE RoleName = 'Receptionist');

-- 2) Users
INSERT INTO Users (Username, PasswordHash, Email, RoleID, IsActive, LastLogin)
SELECT
    'admin',
    'admin123',
    'admin@hospital.local',
    (SELECT RoleID FROM UserRoles WHERE RoleName = 'Administrator' LIMIT 1),
    1,
    NOW()
WHERE NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'admin');

INSERT INTO Users (Username, PasswordHash, Email, RoleID, IsActive)
SELECT
    'dr.smith',
    'admin123',
    'dr.smith@hospital.local',
    (SELECT RoleID FROM UserRoles WHERE RoleName = 'Doctor' LIMIT 1),
    1
WHERE NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'dr.smith');

INSERT INTO Users (Username, PasswordHash, Email, RoleID, IsActive)
SELECT
    'nurse.mary',
    'admin123',
    'nurse.mary@hospital.local',
    (SELECT RoleID FROM UserRoles WHERE RoleName = 'Nurse' LIMIT 1),
    1
WHERE NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'nurse.mary');

INSERT INTO Users (Username, PasswordHash, Email, RoleID, IsActive)
SELECT
    'reception.john',
    'admin123',
    'reception.john@hospital.local',
    (SELECT RoleID FROM UserRoles WHERE RoleName = 'Receptionist' LIMIT 1),
    1
WHERE NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'reception.john');

-- 3) UserDetails
INSERT INTO UserDetails (UserID, FirstName, LastName, DateOfBirth, Gender, ContactNumber, Address, EmergencyContact)
SELECT u.UserID, 'System', 'Admin', '1985-01-01', 'M', '+10000000001', 'HQ', '+10000000002'
FROM Users u
WHERE u.Username = 'admin'
  AND NOT EXISTS (SELECT 1 FROM UserDetails ud WHERE ud.UserID = u.UserID);

INSERT INTO UserDetails (UserID, FirstName, LastName, DateOfBirth, Gender, ContactNumber, Address, EmergencyContact)
SELECT u.UserID, 'John', 'Smith', '1980-02-10', 'M', '+10000000003', 'Doctor Street', '+10000000004'
FROM Users u
WHERE u.Username = 'dr.smith'
  AND NOT EXISTS (SELECT 1 FROM UserDetails ud WHERE ud.UserID = u.UserID);

INSERT INTO UserDetails (UserID, FirstName, LastName, DateOfBirth, Gender, ContactNumber, Address, EmergencyContact)
SELECT u.UserID, 'Mary', 'Nurse', '1990-03-20', 'F', '+10000000005', 'Nurse Street', '+10000000006'
FROM Users u
WHERE u.Username = 'nurse.mary'
  AND NOT EXISTS (SELECT 1 FROM UserDetails ud WHERE ud.UserID = u.UserID);

INSERT INTO UserDetails (UserID, FirstName, LastName, DateOfBirth, Gender, ContactNumber, Address, EmergencyContact)
SELECT u.UserID, 'John', 'Reception', '1992-07-15', 'M', '+10000000007', 'Reception Street', '+10000000008'
FROM Users u
WHERE u.Username = 'reception.john'
  AND NOT EXISTS (SELECT 1 FROM UserDetails ud WHERE ud.UserID = u.UserID);

-- 4) Patients
INSERT INTO Patients
(
    PatientCode, FirstName, LastName, DateOfBirth, Gender, BloodGroup, MaritalStatus,
    Nationality, IdentificationType, IdentificationNumber, RegistrationDate, IsActive
)
SELECT
    'P-SEED-001', 'Alice', 'Brown', '1995-05-05', 'F', 'O+', 'Single',
    'USA', 'NationalID', 'ALICE-001', NOW(), 1
WHERE NOT EXISTS (SELECT 1 FROM Patients WHERE PatientCode = 'P-SEED-001');

-- 5) PatientContacts
INSERT INTO PatientContacts (PatientID, ContactType, ContactValue, IsPrimary)
SELECT p.PatientID, 'Phone', '+10000001001', 1
FROM Patients p
WHERE p.PatientCode = 'P-SEED-001'
  AND NOT EXISTS (
      SELECT 1 FROM PatientContacts pc
      WHERE pc.PatientID = p.PatientID AND pc.ContactType = 'Phone' AND pc.ContactValue = '+10000001001'
  );

-- 6) MedicalHistories
INSERT INTO MedicalHistories
(PatientID, HistoryType, Description, DiagnosisDate, Severity, Status, RecordedBy, RecordedDate)
SELECT
    p.PatientID,
    'Allergy',
    'Seasonal allergy',
    CURDATE(),
    'Mild',
    'Active',
    (SELECT UserID FROM Users WHERE Username = 'admin' LIMIT 1),
    NOW()
FROM Patients p
WHERE p.PatientCode = 'P-SEED-001'
  AND NOT EXISTS (
      SELECT 1 FROM MedicalHistories mh
      WHERE mh.PatientID = p.PatientID AND mh.HistoryType = 'Allergy'
  );

-- 7) Specializations
INSERT INTO Specializations (SpecializationCode, SpecializationName, Description, Department)
SELECT 'GEN', 'General Medicine', 'General care', 'Medicine'
WHERE NOT EXISTS (SELECT 1 FROM Specializations WHERE SpecializationCode = 'GEN');

-- 8) Doctors
INSERT INTO Doctors
(UserID, DoctorCode, SpecializationID, Qualification, LicenseNumber, YearsOfExperience, ConsultationFee, IsAvailable, JoiningDate)
SELECT
    u.UserID,
    'D-SEED-001',
    (SELECT SpecializationID FROM Specializations WHERE SpecializationCode = 'GEN' LIMIT 1),
    'MD',
    'LIC-SEED-001',
    8,
    75.00,
    1,
    CURDATE()
FROM Users u
WHERE u.Username = 'dr.smith'
  AND NOT EXISTS (SELECT 1 FROM Doctors d WHERE d.UserID = u.UserID);

-- 9) DoctorSchedules
INSERT INTO DoctorSchedules (DoctorID, DayOfWeek, StartTime, EndTime, MaxAppointments, IsActive)
SELECT d.DoctorID, 2, '09:00:00', '13:00:00', 20, 1
FROM Doctors d
INNER JOIN Users u ON u.UserID = d.UserID
WHERE u.Username = 'dr.smith'
  AND NOT EXISTS (
      SELECT 1 FROM DoctorSchedules ds
      WHERE ds.DoctorID = d.DoctorID AND ds.DayOfWeek = 2 AND ds.StartTime = '09:00:00'
  );

-- 10) Staff
INSERT INTO Staff (UserID, StaffCode, Designation, Department, Shift, HireDate, Salary)
SELECT
    u.UserID,
    'S-SEED-001',
    'Receptionist',
    'Front Desk',
    'Day',
    CURDATE(),
    2500.00
FROM Users u
WHERE u.Username = 'reception.john'
  AND NOT EXISTS (SELECT 1 FROM Staff s WHERE s.UserID = u.UserID);

-- 11) Appointments
INSERT INTO Appointments
(
    AppointmentCode, PatientID, DoctorID, AppointmentDate, AppointmentTime, AppointmentType, Status,
    Reason, Duration, CreatedBy, CreatedDate, Notes
)
SELECT
    'APP-SEED-001',
    (SELECT PatientID FROM Patients WHERE PatientCode = 'P-SEED-001' LIMIT 1),
    (SELECT d.DoctorID FROM Doctors d INNER JOIN Users u ON u.UserID = d.UserID WHERE u.Username = 'dr.smith' LIMIT 1),
    CURDATE(),
    '10:00:00',
    'Consultation',
    'Scheduled',
    'Initial consultation',
    15,
    (SELECT UserID FROM Users WHERE Username = 'reception.john' LIMIT 1),
    NOW(),
    'Seed appointment'
WHERE NOT EXISTS (SELECT 1 FROM Appointments WHERE AppointmentCode = 'APP-SEED-001');

-- 12) AppointmentHistory
INSERT INTO AppointmentHistory (AppointmentID, Status, ChangedBy, ChangedDate, Notes)
SELECT
    a.AppointmentID,
    a.Status,
    (SELECT UserID FROM Users WHERE Username = 'admin' LIMIT 1),
    NOW(),
    'Seeded status'
FROM Appointments a
WHERE a.AppointmentCode = 'APP-SEED-001'
  AND NOT EXISTS (
      SELECT 1 FROM AppointmentHistory ah
      WHERE ah.AppointmentID = a.AppointmentID
  );

-- 13) ServiceCategories
INSERT INTO ServiceCategories (CategoryName, Description)
SELECT 'Consultation', 'Consultation fees'
WHERE NOT EXISTS (SELECT 1 FROM ServiceCategories WHERE CategoryName = 'Consultation');

-- 14) Services
INSERT INTO Services (ServiceCode, ServiceName, CategoryID, Price, TaxRate, IsActive)
SELECT
    'SRV-SEED-001',
    'General Consultation',
    (SELECT CategoryID FROM ServiceCategories WHERE CategoryName = 'Consultation' LIMIT 1),
    50.00,
    5.00,
    1
WHERE NOT EXISTS (SELECT 1 FROM Services WHERE ServiceCode = 'SRV-SEED-001');

-- 15) Invoices
INSERT INTO Invoices
(
    InvoiceNumber, PatientID, AppointmentID, InvoiceDate, DueDate, TotalAmount, Discount, TaxAmount,
    GrandTotal, Status, CreatedBy, Notes
)
SELECT
    'INV-SEED-001',
    (SELECT PatientID FROM Patients WHERE PatientCode = 'P-SEED-001' LIMIT 1),
    (SELECT AppointmentID FROM Appointments WHERE AppointmentCode = 'APP-SEED-001' LIMIT 1),
    NOW(),
    DATE_ADD(NOW(), INTERVAL 30 DAY),
    50.00,
    0.00,
    2.50,
    52.50,
    'Pending',
    (SELECT UserID FROM Users WHERE Username = 'admin' LIMIT 1),
    'Seed invoice'
WHERE NOT EXISTS (SELECT 1 FROM Invoices WHERE InvoiceNumber = 'INV-SEED-001');

-- 16) InvoiceDetails
INSERT INTO InvoiceDetails (InvoiceID, ServiceID, Quantity, UnitPrice)
SELECT
    (SELECT InvoiceID FROM Invoices WHERE InvoiceNumber = 'INV-SEED-001' LIMIT 1),
    (SELECT ServiceID FROM Services WHERE ServiceCode = 'SRV-SEED-001' LIMIT 1),
    1,
    50.00
WHERE NOT EXISTS (
    SELECT 1
    FROM InvoiceDetails id
    WHERE id.InvoiceID = (SELECT InvoiceID FROM Invoices WHERE InvoiceNumber = 'INV-SEED-001' LIMIT 1)
);

-- 17) Payments
INSERT INTO Payments
(PaymentNumber, InvoiceID, PaymentDate, PaymentMethod, Amount, ReferenceNumber, ReceivedBy, Notes)
SELECT
    'PAY-SEED-001',
    (SELECT InvoiceID FROM Invoices WHERE InvoiceNumber = 'INV-SEED-001' LIMIT 1),
    NOW(),
    'Cash',
    25.00,
    'REF-SEED-001',
    (SELECT UserID FROM Users WHERE Username = 'admin' LIMIT 1),
    'Partial seed payment'
WHERE NOT EXISTS (SELECT 1 FROM Payments WHERE PaymentNumber = 'PAY-SEED-001');

-- 18) Visits
INSERT INTO Visits
(
    VisitCode, PatientID, DoctorID, AppointmentID, VisitDate, Symptoms, Diagnosis, Treatment, FollowUpDate, VisitStatus, CreatedBy
)
SELECT
    'VIS-SEED-001',
    (SELECT PatientID FROM Patients WHERE PatientCode = 'P-SEED-001' LIMIT 1),
    (SELECT d.DoctorID FROM Doctors d INNER JOIN Users u ON u.UserID = d.UserID WHERE u.Username = 'dr.smith' LIMIT 1),
    (SELECT AppointmentID FROM Appointments WHERE AppointmentCode = 'APP-SEED-001' LIMIT 1),
    NOW(),
    'Headache',
    'Migraine',
    'Pain management',
    DATE_ADD(CURDATE(), INTERVAL 7 DAY),
    'Completed',
    (SELECT UserID FROM Users WHERE Username = 'dr.smith' LIMIT 1)
WHERE NOT EXISTS (SELECT 1 FROM Visits WHERE VisitCode = 'VIS-SEED-001');

-- 19) Prescriptions
INSERT INTO Prescriptions
(
    PrescriptionCode, VisitID, PatientID, DoctorID, PrescriptionDate, Instructions, Status
)
SELECT
    'RX-SEED-001',
    (SELECT VisitID FROM Visits WHERE VisitCode = 'VIS-SEED-001' LIMIT 1),
    (SELECT PatientID FROM Patients WHERE PatientCode = 'P-SEED-001' LIMIT 1),
    (SELECT d.DoctorID FROM Doctors d INNER JOIN Users u ON u.UserID = d.UserID WHERE u.Username = 'dr.smith' LIMIT 1),
    NOW(),
    'Take after meals',
    'Active'
WHERE NOT EXISTS (SELECT 1 FROM Prescriptions WHERE PrescriptionCode = 'RX-SEED-001');

-- 20) PrescriptionDetails
INSERT INTO PrescriptionDetails
(PrescriptionID, MedicineName, Dosage, Frequency, Duration, Instructions)
SELECT
    (SELECT PrescriptionID FROM Prescriptions WHERE PrescriptionCode = 'RX-SEED-001' LIMIT 1),
    'Paracetamol',
    '500mg',
    'Twice daily',
    '5 days',
    'With water'
WHERE NOT EXISTS (
    SELECT 1
    FROM PrescriptionDetails pd
    WHERE pd.PrescriptionID = (SELECT PrescriptionID FROM Prescriptions WHERE PrescriptionCode = 'RX-SEED-001' LIMIT 1)
);

-- 21) LabTests
INSERT INTO LabTests (TestCode, TestName, Category, NormalRange, Unit, Price)
SELECT 'LAB-SEED-001', 'Complete Blood Count', 'Hematology', 'Normal', 'N/A', 35.00
WHERE NOT EXISTS (SELECT 1 FROM LabTests WHERE TestCode = 'LAB-SEED-001');

-- 22) LabOrders
INSERT INTO LabOrders
(OrderCode, VisitID, PatientID, DoctorID, OrderDate, Status, ResultDate, Notes)
SELECT
    'LORD-SEED-001',
    (SELECT VisitID FROM Visits WHERE VisitCode = 'VIS-SEED-001' LIMIT 1),
    (SELECT PatientID FROM Patients WHERE PatientCode = 'P-SEED-001' LIMIT 1),
    (SELECT d.DoctorID FROM Doctors d INNER JOIN Users u ON u.UserID = d.UserID WHERE u.Username = 'dr.smith' LIMIT 1),
    NOW(),
    'Pending',
    NULL,
    'Seed lab order'
WHERE NOT EXISTS (SELECT 1 FROM LabOrders WHERE OrderCode = 'LORD-SEED-001');

-- 23) LabOrderDetails
INSERT INTO LabOrderDetails
(OrderID, TestID, ResultValue, ResultUnit, NormalRange, IsNormal, Notes, TechnicianID, CompletedDate)
SELECT
    (SELECT OrderID FROM LabOrders WHERE OrderCode = 'LORD-SEED-001' LIMIT 1),
    (SELECT TestID FROM LabTests WHERE TestCode = 'LAB-SEED-001' LIMIT 1),
    'Pending',
    'N/A',
    'N/A',
    NULL,
    'Awaiting result',
    (SELECT UserID FROM Users WHERE Username = 'admin' LIMIT 1),
    NULL
WHERE NOT EXISTS (
    SELECT 1
    FROM LabOrderDetails lod
    WHERE lod.OrderID = (SELECT OrderID FROM LabOrders WHERE OrderCode = 'LORD-SEED-001' LIMIT 1)
);

-- 24) MedicineCategories
INSERT INTO MedicineCategories (CategoryName, Description)
SELECT 'Analgesics', 'Pain relief medication'
WHERE NOT EXISTS (SELECT 1 FROM MedicineCategories WHERE CategoryName = 'Analgesics');

-- 25) Medicines
INSERT INTO Medicines
(MedicineCode, MedicineName, GenericName, CategoryID, Manufacturer, UnitOfMeasure, UnitPrice, SellingPrice, ReorderLevel, IsActive)
SELECT
    'MED-SEED-001',
    'Paracetamol 500mg',
    'Paracetamol',
    (SELECT CategoryID FROM MedicineCategories WHERE CategoryName = 'Analgesics' LIMIT 1),
    'Seed Pharma',
    'Tablet',
    0.10,
    0.50,
    100,
    1
WHERE NOT EXISTS (SELECT 1 FROM Medicines WHERE MedicineCode = 'MED-SEED-001');

-- 26) Inventory
INSERT INTO Inventory
(MedicineID, BatchNumber, ExpiryDate, Quantity, PurchasePrice, SellingPrice, Supplier, PurchaseDate, Location)
SELECT
    (SELECT MedicineID FROM Medicines WHERE MedicineCode = 'MED-SEED-001' LIMIT 1),
    'BATCH-SEED-001',
    DATE_ADD(CURDATE(), INTERVAL 365 DAY),
    1000,
    0.08,
    0.50,
    'Seed Supplier',
    CURDATE(),
    'Main Pharmacy'
WHERE NOT EXISTS (SELECT 1 FROM Inventory WHERE BatchNumber = 'BATCH-SEED-001');

-- 27) PharmacySales
INSERT INTO PharmacySales
(SaleNumber, PatientID, SaleDate, TotalAmount, Discount, NetAmount, PaymentStatus, SoldBy)
SELECT
    'PSALE-SEED-001',
    (SELECT PatientID FROM Patients WHERE PatientCode = 'P-SEED-001' LIMIT 1),
    NOW(),
    10.00,
    0.00,
    10.00,
    'Paid',
    (SELECT UserID FROM Users WHERE Username = 'admin' LIMIT 1)
WHERE NOT EXISTS (SELECT 1 FROM PharmacySales WHERE SaleNumber = 'PSALE-SEED-001');

-- 28) PharmacySaleDetails
INSERT INTO PharmacySaleDetails
(SaleID, MedicineID, BatchNumber, Quantity, UnitPrice)
SELECT
    (SELECT SaleID FROM PharmacySales WHERE SaleNumber = 'PSALE-SEED-001' LIMIT 1),
    (SELECT MedicineID FROM Medicines WHERE MedicineCode = 'MED-SEED-001' LIMIT 1),
    'BATCH-SEED-001',
    5,
    0.50
WHERE NOT EXISTS (
    SELECT 1
    FROM PharmacySaleDetails psd
    WHERE psd.SaleID = (SELECT SaleID FROM PharmacySales WHERE SaleNumber = 'PSALE-SEED-001' LIMIT 1)
);

-- 29) Wards
INSERT INTO Wards
(WardCode, WardName, WardType, Description, TotalBeds, AvailableBeds, ChargePerDay, IsActive)
SELECT 'WARD-SEED-001', 'General Ward Seed', 'General', 'Seed ward', 20, 20, 120.00, 1
WHERE NOT EXISTS (SELECT 1 FROM Wards WHERE WardCode = 'WARD-SEED-001');

-- 30) Rooms
INSERT INTO Rooms
(RoomNumber, WardID, RoomType, TotalBeds, AvailableBeds, Facilities, RatePerDay, Status)
SELECT
    'RM-SEED-001',
    (SELECT WardID FROM Wards WHERE WardCode = 'WARD-SEED-001' LIMIT 1),
    'General',
    2,
    2,
    'AC, Monitoring',
    150.00,
    'Available'
WHERE NOT EXISTS (SELECT 1 FROM Rooms WHERE RoomNumber = 'RM-SEED-001');

-- 31) Admissions
INSERT INTO Admissions
(
    AdmissionNumber, PatientID, DoctorID, RoomID, AdmissionDate, ExpectedDischargeDate,
    ActualDischargeDate, AdmissionReason, Diagnosis, Status, DischargeSummary
)
SELECT
    'ADM-SEED-001',
    (SELECT PatientID FROM Patients WHERE PatientCode = 'P-SEED-001' LIMIT 1),
    (SELECT d.DoctorID FROM Doctors d INNER JOIN Users u ON u.UserID = d.UserID WHERE u.Username = 'dr.smith' LIMIT 1),
    (SELECT RoomID FROM Rooms WHERE RoomNumber = 'RM-SEED-001' LIMIT 1),
    NOW(),
    DATE_ADD(CURDATE(), INTERVAL 3 DAY),
    NULL,
    'Observation',
    'Under evaluation',
    'Admitted',
    NULL
WHERE NOT EXISTS (SELECT 1 FROM Admissions WHERE AdmissionNumber = 'ADM-SEED-001');

-- 32) BedAllocations
INSERT INTO BedAllocations (AdmissionID, RoomID, BedNumber, AllocationDate, DischargeDate, Status)
SELECT
    (SELECT AdmissionID FROM Admissions WHERE AdmissionNumber = 'ADM-SEED-001' LIMIT 1),
    (SELECT RoomID FROM Rooms WHERE RoomNumber = 'RM-SEED-001' LIMIT 1),
    'B1',
    NOW(),
    NULL,
    'Occupied'
WHERE NOT EXISTS (
    SELECT 1
    FROM BedAllocations ba
    WHERE ba.AdmissionID = (SELECT AdmissionID FROM Admissions WHERE AdmissionNumber = 'ADM-SEED-001' LIMIT 1)
);

-- 33) SystemSettings
INSERT INTO SystemSettings (SettingKey, SettingValue, Description, Category)
SELECT 'SeedConfigured', 'true', 'Flag indicating seed script ran', 'System'
WHERE NOT EXISTS (SELECT 1 FROM SystemSettings WHERE SettingKey = 'SeedConfigured');

-- 34) AuditLogs
INSERT INTO AuditLogs (UserID, Action, TableName, RecordID, OldValue, NewValue, IPAddress, MachineName, LogDate)
SELECT
    (SELECT UserID FROM Users WHERE Username = 'admin' LIMIT 1),
    'SEED_INSERT',
    'System',
    1,
    NULL,
    JSON_OBJECT('seed', 'completed'),
    '127.0.0.1',
    'LOCALHOST',
    NOW()
WHERE NOT EXISTS (
    SELECT 1 FROM AuditLogs WHERE Action = 'SEED_INSERT' AND TableName = 'System'
);

-- 35) Notifications
INSERT INTO Notifications (UserID, Title, Message, NotificationType, IsRead, CreatedDate, ExpiryDate)
SELECT
    (SELECT UserID FROM Users WHERE Username = 'admin' LIMIT 1),
    'Seed Complete',
    'Database seed data inserted.',
    'System',
    0,
    NOW(),
    DATE_ADD(NOW(), INTERVAL 7 DAY)
WHERE NOT EXISTS (
    SELECT 1 FROM Notifications WHERE Title = 'Seed Complete' AND NotificationType = 'System'
);

SELECT 'Seed script finished.' AS Message;
