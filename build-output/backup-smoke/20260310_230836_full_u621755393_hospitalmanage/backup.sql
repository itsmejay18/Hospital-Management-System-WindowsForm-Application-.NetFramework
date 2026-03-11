-- Hospital Management System Backup
-- Backup Type: Full
-- Created UTC: 2026-03-10 15:08:36

SET FOREIGN_KEY_CHECKS = 0;

DROP VIEW IF EXISTS `Admissions`;
DROP VIEW IF EXISTS `AppointmentHistory`;
DROP VIEW IF EXISTS `Appointments`;
DROP VIEW IF EXISTS `AuditLogs`;
DROP VIEW IF EXISTS `BedAllocations`;
DROP VIEW IF EXISTS `Doctors`;
DROP VIEW IF EXISTS `DoctorSchedules`;
DROP VIEW IF EXISTS `Inventory`;
DROP VIEW IF EXISTS `InvoiceDetails`;
DROP VIEW IF EXISTS `Invoices`;
DROP VIEW IF EXISTS `LabOrderDetails`;
DROP VIEW IF EXISTS `LabOrders`;
DROP VIEW IF EXISTS `LabTests`;
DROP VIEW IF EXISTS `MedicalHistories`;
DROP VIEW IF EXISTS `MedicineCategories`;
DROP VIEW IF EXISTS `Medicines`;
DROP VIEW IF EXISTS `Notifications`;
DROP VIEW IF EXISTS `PatientContacts`;
DROP VIEW IF EXISTS `Patients`;
DROP VIEW IF EXISTS `Payments`;
DROP VIEW IF EXISTS `PharmacySaleDetails`;
DROP VIEW IF EXISTS `PharmacySales`;
DROP VIEW IF EXISTS `PrescriptionDetails`;
DROP VIEW IF EXISTS `Prescriptions`;
DROP VIEW IF EXISTS `Rooms`;
DROP VIEW IF EXISTS `ServiceCategories`;
DROP VIEW IF EXISTS `Services`;
DROP VIEW IF EXISTS `Specializations`;
DROP VIEW IF EXISTS `Staff`;
DROP VIEW IF EXISTS `SystemSettings`;
DROP VIEW IF EXISTS `UserDetails`;
DROP VIEW IF EXISTS `UserRoles`;
DROP VIEW IF EXISTS `Users`;
DROP VIEW IF EXISTS `Visits`;
DROP VIEW IF EXISTS `Wards`;

DROP TABLE IF EXISTS `admissions`;
DROP TABLE IF EXISTS `appointmenthistory`;
DROP TABLE IF EXISTS `appointments`;
DROP TABLE IF EXISTS `auditlogs`;
DROP TABLE IF EXISTS `bedallocations`;
DROP TABLE IF EXISTS `doctors`;
DROP TABLE IF EXISTS `doctorschedules`;
DROP TABLE IF EXISTS `inventory`;
DROP TABLE IF EXISTS `invoicedetails`;
DROP TABLE IF EXISTS `invoices`;
DROP TABLE IF EXISTS `laborderdetails`;
DROP TABLE IF EXISTS `laborders`;
DROP TABLE IF EXISTS `labtests`;
DROP TABLE IF EXISTS `medicalhistories`;
DROP TABLE IF EXISTS `medicinecategories`;
DROP TABLE IF EXISTS `medicines`;
DROP TABLE IF EXISTS `notifications`;
DROP TABLE IF EXISTS `patientcontacts`;
DROP TABLE IF EXISTS `patients`;
DROP TABLE IF EXISTS `payments`;
DROP TABLE IF EXISTS `pharmacysaledetails`;
DROP TABLE IF EXISTS `pharmacysales`;
DROP TABLE IF EXISTS `prescriptiondetails`;
DROP TABLE IF EXISTS `prescriptions`;
DROP TABLE IF EXISTS `rooms`;
DROP TABLE IF EXISTS `servicecategories`;
DROP TABLE IF EXISTS `services`;
DROP TABLE IF EXISTS `specializations`;
DROP TABLE IF EXISTS `staff`;
DROP TABLE IF EXISTS `systemsettings`;
DROP TABLE IF EXISTS `userdetails`;
DROP TABLE IF EXISTS `userroles`;
DROP TABLE IF EXISTS `users`;
DROP TABLE IF EXISTS `visits`;
DROP TABLE IF EXISTS `wards`;

CREATE TABLE `admissions` (
  `AdmissionID` int(11) NOT NULL AUTO_INCREMENT,
  `AdmissionNumber` varchar(30) NOT NULL,
  `PatientID` int(11) NOT NULL,
  `DoctorID` int(11) NOT NULL,
  `RoomID` int(11) DEFAULT NULL,
  `AdmissionDate` datetime DEFAULT current_timestamp(),
  `ExpectedDischargeDate` date DEFAULT NULL,
  `ActualDischargeDate` datetime DEFAULT NULL,
  `AdmissionReason` text DEFAULT NULL,
  `Diagnosis` text DEFAULT NULL,
  `Status` enum('Admitted','Discharged','Transferred') DEFAULT 'Admitted',
  `DischargeSummary` text DEFAULT NULL,
  PRIMARY KEY (`AdmissionID`),
  UNIQUE KEY `AdmissionNumber` (`AdmissionNumber`),
  KEY `DoctorID` (`DoctorID`),
  KEY `RoomID` (`RoomID`),
  KEY `idx_admissions_patient` (`PatientID`),
  KEY `idx_admissions_status` (`Status`),
  KEY `idx_admissions_date` (`AdmissionDate`),
  CONSTRAINT `admissions_ibfk_1` FOREIGN KEY (`PatientID`) REFERENCES `patients` (`PatientID`) ON DELETE CASCADE,
  CONSTRAINT `admissions_ibfk_2` FOREIGN KEY (`DoctorID`) REFERENCES `doctors` (`DoctorID`) ON DELETE CASCADE,
  CONSTRAINT `admissions_ibfk_3` FOREIGN KEY (`RoomID`) REFERENCES `rooms` (`RoomID`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `appointmenthistory` (
  `HistoryID` int(11) NOT NULL AUTO_INCREMENT,
  `AppointmentID` int(11) NOT NULL,
  `Status` varchar(20) DEFAULT NULL,
  `ChangedBy` int(11) DEFAULT NULL,
  `ChangedDate` datetime DEFAULT current_timestamp(),
  `Notes` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`HistoryID`),
  KEY `AppointmentID` (`AppointmentID`),
  KEY `ChangedBy` (`ChangedBy`),
  CONSTRAINT `appointmenthistory_ibfk_1` FOREIGN KEY (`AppointmentID`) REFERENCES `appointments` (`AppointmentID`) ON DELETE CASCADE,
  CONSTRAINT `appointmenthistory_ibfk_2` FOREIGN KEY (`ChangedBy`) REFERENCES `users` (`UserID`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=47 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `appointments` (
  `AppointmentID` int(11) NOT NULL AUTO_INCREMENT,
  `AppointmentCode` varchar(20) NOT NULL,
  `PatientID` int(11) NOT NULL,
  `DoctorID` int(11) NOT NULL,
  `AppointmentDate` date NOT NULL,
  `AppointmentTime` time NOT NULL,
  `AppointmentType` enum('Consultation','Follow-up','Emergency','Check-up') DEFAULT NULL,
  `Status` enum('Scheduled','Confirmed','Completed','Cancelled','No-show') DEFAULT 'Scheduled',
  `Reason` varchar(500) DEFAULT NULL,
  `Duration` int(11) DEFAULT 15,
  `CreatedBy` int(11) DEFAULT NULL,
  `CreatedDate` datetime DEFAULT current_timestamp(),
  `Notes` text DEFAULT NULL,
  PRIMARY KEY (`AppointmentID`),
  UNIQUE KEY `AppointmentCode` (`AppointmentCode`),
  KEY `CreatedBy` (`CreatedBy`),
  KEY `idx_appointments_patient` (`PatientID`),
  KEY `idx_appointments_doctor` (`DoctorID`),
  KEY `idx_appointments_date_status` (`AppointmentDate`,`Status`),
  KEY `idx_appointments_doctor_date` (`DoctorID`,`AppointmentDate`,`AppointmentTime`),
  CONSTRAINT `appointments_ibfk_1` FOREIGN KEY (`PatientID`) REFERENCES `patients` (`PatientID`) ON DELETE CASCADE,
  CONSTRAINT `appointments_ibfk_2` FOREIGN KEY (`DoctorID`) REFERENCES `doctors` (`DoctorID`) ON DELETE CASCADE,
  CONSTRAINT `appointments_ibfk_3` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`UserID`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=47 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `auditlogs` (
  `LogID` int(11) NOT NULL AUTO_INCREMENT,
  `UserID` int(11) DEFAULT NULL,
  `Action` varchar(100) NOT NULL,
  `TableName` varchar(100) DEFAULT NULL,
  `RecordID` int(11) DEFAULT NULL,
  `OldValue` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`OldValue`)),
  `NewValue` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`NewValue`)),
  `IPAddress` varchar(50) DEFAULT NULL,
  `MachineName` varchar(100) DEFAULT NULL,
  `LogDate` datetime DEFAULT current_timestamp(),
  PRIMARY KEY (`LogID`),
  KEY `UserID` (`UserID`),
  CONSTRAINT `auditlogs_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `bedallocations` (
  `AllocationID` int(11) NOT NULL AUTO_INCREMENT,
  `AdmissionID` int(11) NOT NULL,
  `RoomID` int(11) NOT NULL,
  `BedNumber` varchar(10) DEFAULT NULL,
  `AllocationDate` datetime DEFAULT current_timestamp(),
  `DischargeDate` datetime DEFAULT NULL,
  `Status` varchar(20) DEFAULT 'Occupied',
  PRIMARY KEY (`AllocationID`),
  KEY `AdmissionID` (`AdmissionID`),
  KEY `RoomID` (`RoomID`),
  CONSTRAINT `bedallocations_ibfk_1` FOREIGN KEY (`AdmissionID`) REFERENCES `admissions` (`AdmissionID`) ON DELETE CASCADE,
  CONSTRAINT `bedallocations_ibfk_2` FOREIGN KEY (`RoomID`) REFERENCES `rooms` (`RoomID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `doctors` (
  `DoctorID` int(11) NOT NULL AUTO_INCREMENT,
  `UserID` int(11) NOT NULL,
  `DoctorCode` varchar(20) NOT NULL,
  `SpecializationID` int(11) DEFAULT NULL,
  `Qualification` varchar(255) DEFAULT NULL,
  `LicenseNumber` varchar(50) DEFAULT NULL,
  `YearsOfExperience` int(11) DEFAULT NULL,
  `ConsultationFee` decimal(10,2) DEFAULT NULL,
  `IsAvailable` tinyint(1) DEFAULT 1,
  `JoiningDate` date DEFAULT curdate(),
  PRIMARY KEY (`DoctorID`),
  UNIQUE KEY `UserID` (`UserID`),
  UNIQUE KEY `DoctorCode` (`DoctorCode`),
  KEY `idx_doctors_specialization` (`SpecializationID`),
  KEY `idx_doctors_available` (`IsAvailable`),
  CONSTRAINT `doctors_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`) ON DELETE CASCADE,
  CONSTRAINT `doctors_ibfk_2` FOREIGN KEY (`SpecializationID`) REFERENCES `specializations` (`SpecializationID`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=33 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `doctorschedules` (
  `ScheduleID` int(11) NOT NULL AUTO_INCREMENT,
  `DoctorID` int(11) NOT NULL,
  `DayOfWeek` int(11) DEFAULT NULL,
  `StartTime` time NOT NULL,
  `EndTime` time NOT NULL,
  `MaxAppointments` int(11) DEFAULT 20,
  `IsActive` tinyint(1) DEFAULT 1,
  PRIMARY KEY (`ScheduleID`),
  KEY `DoctorID` (`DoctorID`),
  CONSTRAINT `doctorschedules_ibfk_1` FOREIGN KEY (`DoctorID`) REFERENCES `doctors` (`DoctorID`) ON DELETE CASCADE,
  CONSTRAINT `doctorschedules_chk_1` CHECK (`DayOfWeek` between 1 and 7)
) ENGINE=InnoDB AUTO_INCREMENT=157 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `inventory` (
  `InventoryID` int(11) NOT NULL AUTO_INCREMENT,
  `MedicineID` int(11) NOT NULL,
  `BatchNumber` varchar(100) DEFAULT NULL,
  `ExpiryDate` date DEFAULT NULL,
  `Quantity` int(11) NOT NULL DEFAULT 0,
  `PurchasePrice` decimal(10,2) DEFAULT NULL,
  `SellingPrice` decimal(10,2) DEFAULT NULL,
  `Supplier` varchar(200) DEFAULT NULL,
  `PurchaseDate` date DEFAULT curdate(),
  `Location` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`InventoryID`),
  KEY `idx_inventory_expiry` (`ExpiryDate`),
  KEY `idx_inventory_medicine` (`MedicineID`),
  KEY `idx_inventory_quantity` (`Quantity`),
  CONSTRAINT `inventory_ibfk_1` FOREIGN KEY (`MedicineID`) REFERENCES `medicines` (`MedicineID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=40 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `invoicedetails` (
  `DetailID` int(11) NOT NULL AUTO_INCREMENT,
  `InvoiceID` int(11) NOT NULL,
  `ServiceID` int(11) NOT NULL,
  `Quantity` int(11) DEFAULT 1,
  `UnitPrice` decimal(10,2) DEFAULT NULL,
  `TotalPrice` decimal(10,2) GENERATED ALWAYS AS (`Quantity` * `UnitPrice`) STORED,
  PRIMARY KEY (`DetailID`),
  KEY `InvoiceID` (`InvoiceID`),
  KEY `ServiceID` (`ServiceID`),
  CONSTRAINT `invoicedetails_ibfk_1` FOREIGN KEY (`InvoiceID`) REFERENCES `invoices` (`InvoiceID`) ON DELETE CASCADE,
  CONSTRAINT `invoicedetails_ibfk_2` FOREIGN KEY (`ServiceID`) REFERENCES `services` (`ServiceID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=42 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `invoices` (
  `InvoiceID` int(11) NOT NULL AUTO_INCREMENT,
  `InvoiceNumber` varchar(30) NOT NULL,
  `PatientID` int(11) NOT NULL,
  `AppointmentID` int(11) DEFAULT NULL,
  `InvoiceDate` datetime DEFAULT current_timestamp(),
  `DueDate` datetime DEFAULT NULL,
  `TotalAmount` decimal(10,2) DEFAULT 0.00,
  `Discount` decimal(10,2) DEFAULT 0.00,
  `TaxAmount` decimal(10,2) DEFAULT 0.00,
  `GrandTotal` decimal(10,2) DEFAULT 0.00,
  `Status` enum('Pending','Paid','Partial','Cancelled') DEFAULT 'Pending',
  `CreatedBy` int(11) DEFAULT NULL,
  `Notes` text DEFAULT NULL,
  PRIMARY KEY (`InvoiceID`),
  UNIQUE KEY `InvoiceNumber` (`InvoiceNumber`),
  KEY `AppointmentID` (`AppointmentID`),
  KEY `CreatedBy` (`CreatedBy`),
  KEY `idx_invoices_patient` (`PatientID`),
  KEY `idx_invoices_status` (`Status`),
  KEY `idx_invoices_date` (`InvoiceDate`),
  CONSTRAINT `invoices_ibfk_1` FOREIGN KEY (`PatientID`) REFERENCES `patients` (`PatientID`) ON DELETE CASCADE,
  CONSTRAINT `invoices_ibfk_2` FOREIGN KEY (`AppointmentID`) REFERENCES `appointments` (`AppointmentID`) ON DELETE SET NULL,
  CONSTRAINT `invoices_ibfk_3` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`UserID`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=42 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `laborderdetails` (
  `OrderDetailID` int(11) NOT NULL AUTO_INCREMENT,
  `OrderID` int(11) NOT NULL,
  `TestID` int(11) NOT NULL,
  `ResultValue` varchar(200) DEFAULT NULL,
  `ResultUnit` varchar(50) DEFAULT NULL,
  `NormalRange` varchar(200) DEFAULT NULL,
  `IsNormal` tinyint(1) DEFAULT NULL,
  `Notes` text DEFAULT NULL,
  `TechnicianID` int(11) DEFAULT NULL,
  `CompletedDate` datetime DEFAULT NULL,
  PRIMARY KEY (`OrderDetailID`),
  KEY `OrderID` (`OrderID`),
  KEY `TestID` (`TestID`),
  KEY `TechnicianID` (`TechnicianID`),
  CONSTRAINT `laborderdetails_ibfk_1` FOREIGN KEY (`OrderID`) REFERENCES `laborders` (`OrderID`) ON DELETE CASCADE,
  CONSTRAINT `laborderdetails_ibfk_2` FOREIGN KEY (`TestID`) REFERENCES `labtests` (`TestID`) ON DELETE CASCADE,
  CONSTRAINT `laborderdetails_ibfk_3` FOREIGN KEY (`TechnicianID`) REFERENCES `users` (`UserID`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `laborders` (
  `OrderID` int(11) NOT NULL AUTO_INCREMENT,
  `OrderCode` varchar(20) NOT NULL,
  `VisitID` int(11) DEFAULT NULL,
  `PatientID` int(11) NOT NULL,
  `DoctorID` int(11) NOT NULL,
  `OrderDate` datetime DEFAULT current_timestamp(),
  `Status` enum('Pending','In Progress','Completed','Cancelled') DEFAULT 'Pending',
  `ResultDate` datetime DEFAULT NULL,
  `Notes` text DEFAULT NULL,
  PRIMARY KEY (`OrderID`),
  UNIQUE KEY `OrderCode` (`OrderCode`),
  KEY `VisitID` (`VisitID`),
  KEY `PatientID` (`PatientID`),
  KEY `DoctorID` (`DoctorID`),
  CONSTRAINT `laborders_ibfk_1` FOREIGN KEY (`VisitID`) REFERENCES `visits` (`VisitID`) ON DELETE SET NULL,
  CONSTRAINT `laborders_ibfk_2` FOREIGN KEY (`PatientID`) REFERENCES `patients` (`PatientID`) ON DELETE CASCADE,
  CONSTRAINT `laborders_ibfk_3` FOREIGN KEY (`DoctorID`) REFERENCES `doctors` (`DoctorID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `labtests` (
  `TestID` int(11) NOT NULL AUTO_INCREMENT,
  `TestCode` varchar(20) NOT NULL,
  `TestName` varchar(200) NOT NULL,
  `Category` varchar(100) DEFAULT NULL,
  `NormalRange` varchar(200) DEFAULT NULL,
  `Unit` varchar(50) DEFAULT NULL,
  `Price` decimal(10,2) DEFAULT NULL,
  PRIMARY KEY (`TestID`),
  UNIQUE KEY `TestCode` (`TestCode`)
) ENGINE=InnoDB AUTO_INCREMENT=42 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `medicalhistories` (
  `HistoryID` int(11) NOT NULL AUTO_INCREMENT,
  `PatientID` int(11) NOT NULL,
  `HistoryType` varchar(50) DEFAULT NULL,
  `Description` varchar(500) DEFAULT NULL,
  `DiagnosisDate` date DEFAULT NULL,
  `Severity` varchar(20) DEFAULT NULL,
  `Status` varchar(20) DEFAULT 'Active',
  `RecordedBy` int(11) DEFAULT NULL,
  `RecordedDate` datetime DEFAULT current_timestamp(),
  PRIMARY KEY (`HistoryID`),
  KEY `PatientID` (`PatientID`),
  KEY `RecordedBy` (`RecordedBy`),
  CONSTRAINT `medicalhistories_ibfk_1` FOREIGN KEY (`PatientID`) REFERENCES `patients` (`PatientID`) ON DELETE CASCADE,
  CONSTRAINT `medicalhistories_ibfk_2` FOREIGN KEY (`RecordedBy`) REFERENCES `users` (`UserID`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `medicinecategories` (
  `CategoryID` int(11) NOT NULL AUTO_INCREMENT,
  `CategoryName` varchar(100) NOT NULL,
  `Description` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`CategoryID`)
) ENGINE=InnoDB AUTO_INCREMENT=40 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `medicines` (
  `MedicineID` int(11) NOT NULL AUTO_INCREMENT,
  `MedicineCode` varchar(20) NOT NULL,
  `MedicineName` varchar(200) NOT NULL,
  `GenericName` varchar(200) DEFAULT NULL,
  `CategoryID` int(11) DEFAULT NULL,
  `Manufacturer` varchar(200) DEFAULT NULL,
  `UnitOfMeasure` varchar(50) DEFAULT NULL,
  `UnitPrice` decimal(10,2) NOT NULL,
  `SellingPrice` decimal(10,2) NOT NULL,
  `ReorderLevel` int(11) DEFAULT 10,
  `IsActive` tinyint(1) DEFAULT 1,
  PRIMARY KEY (`MedicineID`),
  UNIQUE KEY `MedicineCode` (`MedicineCode`),
  KEY `CategoryID` (`CategoryID`),
  CONSTRAINT `medicines_ibfk_1` FOREIGN KEY (`CategoryID`) REFERENCES `medicinecategories` (`CategoryID`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=40 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `notifications` (
  `NotificationID` int(11) NOT NULL AUTO_INCREMENT,
  `UserID` int(11) NOT NULL,
  `Title` varchar(200) NOT NULL,
  `Message` text NOT NULL,
  `NotificationType` varchar(50) DEFAULT NULL,
  `IsRead` tinyint(1) DEFAULT 0,
  `CreatedDate` datetime DEFAULT current_timestamp(),
  `ExpiryDate` datetime DEFAULT NULL,
  PRIMARY KEY (`NotificationID`),
  KEY `UserID` (`UserID`),
  CONSTRAINT `notifications_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `patientcontacts` (
  `ContactID` int(11) NOT NULL AUTO_INCREMENT,
  `PatientID` int(11) NOT NULL,
  `ContactType` enum('Phone','Email','Address') DEFAULT NULL,
  `ContactValue` varchar(255) NOT NULL,
  `IsPrimary` tinyint(1) DEFAULT 0,
  PRIMARY KEY (`ContactID`),
  KEY `PatientID` (`PatientID`),
  CONSTRAINT `patientcontacts_ibfk_1` FOREIGN KEY (`PatientID`) REFERENCES `patients` (`PatientID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=92 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `patients` (
  `PatientID` int(11) NOT NULL AUTO_INCREMENT,
  `PatientCode` varchar(20) NOT NULL,
  `FirstName` varchar(50) NOT NULL,
  `LastName` varchar(50) NOT NULL,
  `DateOfBirth` date NOT NULL,
  `Gender` enum('M','F','O') DEFAULT NULL,
  `BloodGroup` varchar(5) DEFAULT NULL,
  `MaritalStatus` varchar(20) DEFAULT NULL,
  `Nationality` varchar(50) DEFAULT NULL,
  `IdentificationType` varchar(50) DEFAULT NULL,
  `IdentificationNumber` varchar(50) DEFAULT NULL,
  `RegistrationDate` datetime DEFAULT current_timestamp(),
  `IsActive` tinyint(1) DEFAULT 1,
  `ProfileImage` longblob DEFAULT NULL,
  PRIMARY KEY (`PatientID`),
  UNIQUE KEY `PatientCode` (`PatientCode`),
  KEY `idx_patients_patientcode` (`PatientCode`),
  KEY `idx_patients_name` (`LastName`,`FirstName`),
  KEY `idx_patients_dob` (`DateOfBirth`)
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `payments` (
  `PaymentID` int(11) NOT NULL AUTO_INCREMENT,
  `PaymentNumber` varchar(30) NOT NULL,
  `InvoiceID` int(11) NOT NULL,
  `PaymentDate` datetime DEFAULT current_timestamp(),
  `PaymentMethod` enum('Cash','Credit Card','Debit Card','Insurance','Online') DEFAULT NULL,
  `Amount` decimal(10,2) NOT NULL,
  `ReferenceNumber` varchar(100) DEFAULT NULL,
  `ReceivedBy` int(11) DEFAULT NULL,
  `Notes` text DEFAULT NULL,
  PRIMARY KEY (`PaymentID`),
  UNIQUE KEY `PaymentNumber` (`PaymentNumber`),
  KEY `InvoiceID` (`InvoiceID`),
  KEY `ReceivedBy` (`ReceivedBy`),
  CONSTRAINT `payments_ibfk_1` FOREIGN KEY (`InvoiceID`) REFERENCES `invoices` (`InvoiceID`) ON DELETE CASCADE,
  CONSTRAINT `payments_ibfk_2` FOREIGN KEY (`ReceivedBy`) REFERENCES `users` (`UserID`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `pharmacysaledetails` (
  `SaleDetailID` int(11) NOT NULL AUTO_INCREMENT,
  `SaleID` int(11) NOT NULL,
  `MedicineID` int(11) NOT NULL,
  `BatchNumber` varchar(100) DEFAULT NULL,
  `Quantity` int(11) NOT NULL,
  `UnitPrice` decimal(10,2) DEFAULT NULL,
  `TotalPrice` decimal(10,2) GENERATED ALWAYS AS (`Quantity` * `UnitPrice`) STORED,
  PRIMARY KEY (`SaleDetailID`),
  KEY `SaleID` (`SaleID`),
  KEY `MedicineID` (`MedicineID`),
  CONSTRAINT `pharmacysaledetails_ibfk_1` FOREIGN KEY (`SaleID`) REFERENCES `pharmacysales` (`SaleID`) ON DELETE CASCADE,
  CONSTRAINT `pharmacysaledetails_ibfk_2` FOREIGN KEY (`MedicineID`) REFERENCES `medicines` (`MedicineID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=62 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `pharmacysales` (
  `SaleID` int(11) NOT NULL AUTO_INCREMENT,
  `SaleNumber` varchar(30) NOT NULL,
  `PatientID` int(11) DEFAULT NULL,
  `SaleDate` datetime DEFAULT current_timestamp(),
  `TotalAmount` decimal(10,2) DEFAULT 0.00,
  `Discount` decimal(10,2) DEFAULT 0.00,
  `NetAmount` decimal(10,2) DEFAULT 0.00,
  `PaymentStatus` enum('Pending','Paid','Partial') DEFAULT 'Pending',
  `SoldBy` int(11) DEFAULT NULL,
  PRIMARY KEY (`SaleID`),
  UNIQUE KEY `SaleNumber` (`SaleNumber`),
  KEY `PatientID` (`PatientID`),
  KEY `SoldBy` (`SoldBy`),
  CONSTRAINT `pharmacysales_ibfk_1` FOREIGN KEY (`PatientID`) REFERENCES `patients` (`PatientID`) ON DELETE SET NULL,
  CONSTRAINT `pharmacysales_ibfk_2` FOREIGN KEY (`SoldBy`) REFERENCES `users` (`UserID`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `prescriptiondetails` (
  `PrescriptionDetailID` int(11) NOT NULL AUTO_INCREMENT,
  `PrescriptionID` int(11) NOT NULL,
  `MedicineName` varchar(200) NOT NULL,
  `Dosage` varchar(100) DEFAULT NULL,
  `Frequency` varchar(100) DEFAULT NULL,
  `Duration` varchar(50) DEFAULT NULL,
  `Instructions` text DEFAULT NULL,
  PRIMARY KEY (`PrescriptionDetailID`),
  KEY `PrescriptionID` (`PrescriptionID`),
  CONSTRAINT `prescriptiondetails_ibfk_1` FOREIGN KEY (`PrescriptionID`) REFERENCES `prescriptions` (`PrescriptionID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `prescriptions` (
  `PrescriptionID` int(11) NOT NULL AUTO_INCREMENT,
  `PrescriptionCode` varchar(20) NOT NULL,
  `VisitID` int(11) NOT NULL,
  `PatientID` int(11) NOT NULL,
  `DoctorID` int(11) NOT NULL,
  `PrescriptionDate` datetime DEFAULT current_timestamp(),
  `Instructions` text DEFAULT NULL,
  `Status` varchar(20) DEFAULT 'Active',
  PRIMARY KEY (`PrescriptionID`),
  UNIQUE KEY `PrescriptionCode` (`PrescriptionCode`),
  KEY `VisitID` (`VisitID`),
  KEY `PatientID` (`PatientID`),
  KEY `DoctorID` (`DoctorID`),
  CONSTRAINT `prescriptions_ibfk_1` FOREIGN KEY (`VisitID`) REFERENCES `visits` (`VisitID`) ON DELETE CASCADE,
  CONSTRAINT `prescriptions_ibfk_2` FOREIGN KEY (`PatientID`) REFERENCES `patients` (`PatientID`) ON DELETE CASCADE,
  CONSTRAINT `prescriptions_ibfk_3` FOREIGN KEY (`DoctorID`) REFERENCES `doctors` (`DoctorID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `rooms` (
  `RoomID` int(11) NOT NULL AUTO_INCREMENT,
  `RoomNumber` varchar(20) NOT NULL,
  `WardID` int(11) DEFAULT NULL,
  `RoomType` varchar(50) DEFAULT NULL,
  `TotalBeds` int(11) DEFAULT 1,
  `AvailableBeds` int(11) DEFAULT 0,
  `Facilities` text DEFAULT NULL,
  `RatePerDay` decimal(10,2) DEFAULT NULL,
  `Status` varchar(20) DEFAULT 'Available',
  PRIMARY KEY (`RoomID`),
  UNIQUE KEY `RoomNumber` (`RoomNumber`),
  KEY `WardID` (`WardID`),
  CONSTRAINT `rooms_ibfk_1` FOREIGN KEY (`WardID`) REFERENCES `wards` (`WardID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=39 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `servicecategories` (
  `CategoryID` int(11) NOT NULL AUTO_INCREMENT,
  `CategoryName` varchar(100) NOT NULL,
  `Description` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`CategoryID`)
) ENGINE=InnoDB AUTO_INCREMENT=39 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `services` (
  `ServiceID` int(11) NOT NULL AUTO_INCREMENT,
  `ServiceCode` varchar(20) NOT NULL,
  `ServiceName` varchar(200) NOT NULL,
  `CategoryID` int(11) DEFAULT NULL,
  `Price` decimal(10,2) NOT NULL,
  `TaxRate` decimal(5,2) DEFAULT 0.00,
  `IsActive` tinyint(1) DEFAULT 1,
  PRIMARY KEY (`ServiceID`),
  UNIQUE KEY `ServiceCode` (`ServiceCode`),
  KEY `CategoryID` (`CategoryID`),
  CONSTRAINT `services_ibfk_1` FOREIGN KEY (`CategoryID`) REFERENCES `servicecategories` (`CategoryID`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=40 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `specializations` (
  `SpecializationID` int(11) NOT NULL AUTO_INCREMENT,
  `SpecializationCode` varchar(20) NOT NULL,
  `SpecializationName` varchar(100) NOT NULL,
  `Description` varchar(255) DEFAULT NULL,
  `Department` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`SpecializationID`),
  UNIQUE KEY `SpecializationCode` (`SpecializationCode`)
) ENGINE=InnoDB AUTO_INCREMENT=41 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `staff` (
  `StaffID` int(11) NOT NULL AUTO_INCREMENT,
  `UserID` int(11) NOT NULL,
  `StaffCode` varchar(20) NOT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `Department` varchar(100) DEFAULT NULL,
  `Shift` varchar(20) DEFAULT NULL,
  `HireDate` date DEFAULT curdate(),
  `Salary` decimal(10,2) DEFAULT NULL,
  PRIMARY KEY (`StaffID`),
  UNIQUE KEY `UserID` (`UserID`),
  UNIQUE KEY `StaffCode` (`StaffCode`),
  CONSTRAINT `staff_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=40 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `systemsettings` (
  `SettingID` int(11) NOT NULL AUTO_INCREMENT,
  `SettingKey` varchar(100) NOT NULL,
  `SettingValue` text DEFAULT NULL,
  `Description` varchar(255) DEFAULT NULL,
  `Category` varchar(50) DEFAULT NULL,
  `LastModified` datetime DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`SettingID`),
  UNIQUE KEY `SettingKey` (`SettingKey`)
) ENGINE=InnoDB AUTO_INCREMENT=57 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `userdetails` (
  `UserDetailID` int(11) NOT NULL AUTO_INCREMENT,
  `UserID` int(11) NOT NULL,
  `FirstName` varchar(50) NOT NULL,
  `LastName` varchar(50) NOT NULL,
  `DateOfBirth` date DEFAULT NULL,
  `Gender` enum('M','F','O') DEFAULT NULL,
  `ContactNumber` varchar(20) DEFAULT NULL,
  `Address` varchar(255) DEFAULT NULL,
  `EmergencyContact` varchar(20) DEFAULT NULL,
  `ProfileImage` longblob DEFAULT NULL,
  PRIMARY KEY (`UserDetailID`),
  UNIQUE KEY `UserID` (`UserID`),
  CONSTRAINT `userdetails_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=43 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `userroles` (
  `RoleID` int(11) NOT NULL AUTO_INCREMENT,
  `RoleName` varchar(50) NOT NULL,
  `Description` varchar(255) DEFAULT NULL,
  `CreatedDate` datetime DEFAULT current_timestamp(),
  PRIMARY KEY (`RoleID`),
  UNIQUE KEY `RoleName` (`RoleName`)
) ENGINE=InnoDB AUTO_INCREMENT=39 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `users` (
  `UserID` int(11) NOT NULL AUTO_INCREMENT,
  `Username` varchar(50) NOT NULL,
  `PasswordHash` varchar(255) NOT NULL,
  `Email` varchar(100) DEFAULT NULL,
  `RoleID` int(11) NOT NULL,
  `IsActive` tinyint(1) DEFAULT 1,
  `LastLogin` datetime DEFAULT NULL,
  `CreatedDate` datetime DEFAULT current_timestamp(),
  PRIMARY KEY (`UserID`),
  UNIQUE KEY `Username` (`Username`),
  UNIQUE KEY `Email` (`Email`),
  KEY `idx_users_username` (`Username`),
  KEY `idx_users_role` (`RoleID`,`IsActive`),
  CONSTRAINT `users_ibfk_1` FOREIGN KEY (`RoleID`) REFERENCES `userroles` (`RoleID`)
) ENGINE=InnoDB AUTO_INCREMENT=44 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `visits` (
  `VisitID` int(11) NOT NULL AUTO_INCREMENT,
  `VisitCode` varchar(20) NOT NULL,
  `PatientID` int(11) NOT NULL,
  `DoctorID` int(11) NOT NULL,
  `AppointmentID` int(11) DEFAULT NULL,
  `VisitDate` datetime DEFAULT current_timestamp(),
  `Symptoms` text DEFAULT NULL,
  `Diagnosis` text DEFAULT NULL,
  `Treatment` text DEFAULT NULL,
  `FollowUpDate` date DEFAULT NULL,
  `VisitStatus` varchar(20) DEFAULT 'Completed',
  `CreatedBy` int(11) DEFAULT NULL,
  PRIMARY KEY (`VisitID`),
  UNIQUE KEY `VisitCode` (`VisitCode`),
  KEY `AppointmentID` (`AppointmentID`),
  KEY `CreatedBy` (`CreatedBy`),
  KEY `idx_visits_patient` (`PatientID`),
  KEY `idx_visits_doctor` (`DoctorID`),
  KEY `idx_visits_date` (`VisitDate`),
  CONSTRAINT `visits_ibfk_1` FOREIGN KEY (`PatientID`) REFERENCES `patients` (`PatientID`) ON DELETE CASCADE,
  CONSTRAINT `visits_ibfk_2` FOREIGN KEY (`DoctorID`) REFERENCES `doctors` (`DoctorID`) ON DELETE CASCADE,
  CONSTRAINT `visits_ibfk_3` FOREIGN KEY (`AppointmentID`) REFERENCES `appointments` (`AppointmentID`) ON DELETE SET NULL,
  CONSTRAINT `visits_ibfk_4` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`UserID`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `wards` (
  `WardID` int(11) NOT NULL AUTO_INCREMENT,
  `WardCode` varchar(20) NOT NULL,
  `WardName` varchar(100) NOT NULL,
  `WardType` varchar(50) DEFAULT NULL,
  `Description` varchar(255) DEFAULT NULL,
  `TotalBeds` int(11) DEFAULT 0,
  `AvailableBeds` int(11) DEFAULT 0,
  `ChargePerDay` decimal(10,2) DEFAULT NULL,
  `IsActive` tinyint(1) DEFAULT 1,
  PRIMARY KEY (`WardID`),
  UNIQUE KEY `WardCode` (`WardCode`)
) ENGINE=InnoDB AUTO_INCREMENT=40 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Data for admissions
INSERT INTO `admissions` (`AdmissionID`, `AdmissionNumber`, `PatientID`, `DoctorID`, `RoomID`, `AdmissionDate`, `ExpectedDischargeDate`, `ActualDischargeDate`, `AdmissionReason`, `Diagnosis`, `Status`, `DischargeSummary`) VALUES (2, 'ADMNSD001', 2, 3, 9, '2026-02-14 14:00:00.000000', '2026-02-19 00:00:00.000000', '2026-02-19 17:00:00.000000', 'Seed inpatient monitoring after emergency consult.', 'Essential hypertension', 'Discharged', 'Patient stabilized and discharged with home care instructions.');
INSERT INTO `admissions` (`AdmissionID`, `AdmissionNumber`, `PatientID`, `DoctorID`, `RoomID`, `AdmissionDate`, `ExpectedDischargeDate`, `ActualDischargeDate`, `AdmissionReason`, `Diagnosis`, `Status`, `DischargeSummary`) VALUES (3, 'ADMNSD002', 3, 4, 10, '2026-02-15 14:00:00.000000', '2026-02-21 00:00:00.000000', '2026-02-21 17:00:00.000000', 'Seed inpatient monitoring after emergency consult.', 'Type 2 diabetes mellitus', 'Discharged', 'Patient stabilized and discharged with home care instructions.');
INSERT INTO `admissions` (`AdmissionID`, `AdmissionNumber`, `PatientID`, `DoctorID`, `RoomID`, `AdmissionDate`, `ExpectedDischargeDate`, `ActualDischargeDate`, `AdmissionReason`, `Diagnosis`, `Status`, `DischargeSummary`) VALUES (4, 'ADMNSD003', 4, 5, 11, '2026-02-16 14:00:00.000000', '2026-02-23 00:00:00.000000', '2026-02-23 17:00:00.000000', 'Seed inpatient monitoring after emergency consult.', 'Bronchial asthma', 'Discharged', 'Patient stabilized and discharged with home care instructions.');
INSERT INTO `admissions` (`AdmissionID`, `AdmissionNumber`, `PatientID`, `DoctorID`, `RoomID`, `AdmissionDate`, `ExpectedDischargeDate`, `ActualDischargeDate`, `AdmissionReason`, `Diagnosis`, `Status`, `DischargeSummary`) VALUES (5, 'ADMNSD004', 5, 6, 12, '2026-02-17 14:00:00.000000', '2026-02-25 00:00:00.000000', '2026-02-25 17:00:00.000000', 'Seed inpatient monitoring after emergency consult.', 'Hyperlipidemia', 'Discharged', 'Patient stabilized and discharged with home care instructions.');
INSERT INTO `admissions` (`AdmissionID`, `AdmissionNumber`, `PatientID`, `DoctorID`, `RoomID`, `AdmissionDate`, `ExpectedDischargeDate`, `ActualDischargeDate`, `AdmissionReason`, `Diagnosis`, `Status`, `DischargeSummary`) VALUES (6, 'ADMNSD005', 6, 7, 13, '2026-02-18 14:00:00.000000', '2026-02-22 00:00:00.000000', '2026-02-22 17:00:00.000000', 'Seed inpatient monitoring after emergency consult.', 'Migraine episodes', 'Discharged', 'Patient stabilized and discharged with home care instructions.');
INSERT INTO `admissions` (`AdmissionID`, `AdmissionNumber`, `PatientID`, `DoctorID`, `RoomID`, `AdmissionDate`, `ExpectedDischargeDate`, `ActualDischargeDate`, `AdmissionReason`, `Diagnosis`, `Status`, `DischargeSummary`) VALUES (7, 'ADMNSD006', 7, 8, 14, '2026-02-19 14:00:00.000000', '2026-02-24 00:00:00.000000', '2026-02-24 17:00:00.000000', 'Seed inpatient monitoring after emergency consult.', 'Allergic rhinitis', 'Discharged', 'Patient stabilized and discharged with home care instructions.');
INSERT INTO `admissions` (`AdmissionID`, `AdmissionNumber`, `PatientID`, `DoctorID`, `RoomID`, `AdmissionDate`, `ExpectedDischargeDate`, `ActualDischargeDate`, `AdmissionReason`, `Diagnosis`, `Status`, `DischargeSummary`) VALUES (8, 'ADMNSD007', 8, 9, 15, '2026-02-20 14:00:00.000000', '2026-02-26 00:00:00.000000', '2026-02-26 17:00:00.000000', 'Seed inpatient monitoring after emergency consult.', 'Lumbar strain', 'Discharged', 'Patient stabilized and discharged with home care instructions.');
INSERT INTO `admissions` (`AdmissionID`, `AdmissionNumber`, `PatientID`, `DoctorID`, `RoomID`, `AdmissionDate`, `ExpectedDischargeDate`, `ActualDischargeDate`, `AdmissionReason`, `Diagnosis`, `Status`, `DischargeSummary`) VALUES (9, 'ADMNSD008', 9, 10, 16, '2026-02-21 14:00:00.000000', '2026-02-28 00:00:00.000000', '2026-02-28 17:00:00.000000', 'Seed inpatient monitoring after emergency consult.', 'Osteoarthritis', 'Discharged', 'Patient stabilized and discharged with home care instructions.');
INSERT INTO `admissions` (`AdmissionID`, `AdmissionNumber`, `PatientID`, `DoctorID`, `RoomID`, `AdmissionDate`, `ExpectedDischargeDate`, `ActualDischargeDate`, `AdmissionReason`, `Diagnosis`, `Status`, `DischargeSummary`) VALUES (10, 'ADMNSD009', 10, 11, 17, '2026-02-22 14:00:00.000000', '2026-03-02 00:00:00.000000', '2026-03-02 17:00:00.000000', 'Seed inpatient monitoring after emergency consult.', 'Acute gastroenteritis', 'Discharged', 'Patient stabilized and discharged with home care instructions.');
INSERT INTO `admissions` (`AdmissionID`, `AdmissionNumber`, `PatientID`, `DoctorID`, `RoomID`, `AdmissionDate`, `ExpectedDischargeDate`, `ActualDischargeDate`, `AdmissionReason`, `Diagnosis`, `Status`, `DischargeSummary`) VALUES (11, 'ADMNSD010', 11, 12, 18, '2026-02-23 14:00:00.000000', '2026-02-27 00:00:00.000000', '2026-02-27 17:00:00.000000', 'Seed inpatient monitoring after emergency consult.', 'Urinary tract infection', 'Discharged', 'Patient stabilized and discharged with home care instructions.');
INSERT INTO `admissions` (`AdmissionID`, `AdmissionNumber`, `PatientID`, `DoctorID`, `RoomID`, `AdmissionDate`, `ExpectedDischargeDate`, `ActualDischargeDate`, `AdmissionReason`, `Diagnosis`, `Status`, `DischargeSummary`) VALUES (12, 'ADMNSD011', 12, 13, 19, '2026-02-24 14:00:00.000000', '2026-03-01 00:00:00.000000', '2026-03-01 17:00:00.000000', 'Seed inpatient monitoring after emergency consult.', 'Essential hypertension', 'Discharged', 'Patient stabilized and discharged with home care instructions.');
INSERT INTO `admissions` (`AdmissionID`, `AdmissionNumber`, `PatientID`, `DoctorID`, `RoomID`, `AdmissionDate`, `ExpectedDischargeDate`, `ActualDischargeDate`, `AdmissionReason`, `Diagnosis`, `Status`, `DischargeSummary`) VALUES (13, 'ADMNSD012', 13, 14, 20, '2026-02-25 14:00:00.000000', '2026-03-03 00:00:00.000000', '2026-03-03 17:00:00.000000', 'Seed inpatient monitoring after emergency consult.', 'Type 2 diabetes mellitus', 'Discharged', 'Patient stabilized and discharged with home care instructions.');
INSERT INTO `admissions` (`AdmissionID`, `AdmissionNumber`, `PatientID`, `DoctorID`, `RoomID`, `AdmissionDate`, `ExpectedDischargeDate`, `ActualDischargeDate`, `AdmissionReason`, `Diagnosis`, `Status`, `DischargeSummary`) VALUES (14, 'ADMNSD013', 14, 15, 21, '2026-02-26 14:00:00.000000', '2026-03-05 00:00:00.000000', '2026-03-05 17:00:00.000000', 'Seed inpatient monitoring after emergency consult.', 'Bronchial asthma', 'Discharged', 'Patient stabilized and discharged with home care instructions.');
INSERT INTO `admissions` (`AdmissionID`, `AdmissionNumber`, `PatientID`, `DoctorID`, `RoomID`, `AdmissionDate`, `ExpectedDischargeDate`, `ActualDischargeDate`, `AdmissionReason`, `Diagnosis`, `Status`, `DischargeSummary`) VALUES (15, 'ADMNSD014', 15, 16, 22, '2026-02-27 14:00:00.000000', '2026-03-07 00:00:00.000000', '2026-03-07 17:00:00.000000', 'Seed inpatient monitoring after emergency consult.', 'Hyperlipidemia', 'Discharged', 'Patient stabilized and discharged with home care instructions.');
INSERT INTO `admissions` (`AdmissionID`, `AdmissionNumber`, `PatientID`, `DoctorID`, `RoomID`, `AdmissionDate`, `ExpectedDischargeDate`, `ActualDischargeDate`, `AdmissionReason`, `Diagnosis`, `Status`, `DischargeSummary`) VALUES (16, 'ADMNSD015', 16, 17, 23, '2026-02-28 14:00:00.000000', '2026-03-04 00:00:00.000000', '2026-03-04 17:00:00.000000', 'Seed inpatient monitoring after emergency consult.', 'Migraine episodes', 'Discharged', 'Patient stabilized and discharged with home care instructions.');
INSERT INTO `admissions` (`AdmissionID`, `AdmissionNumber`, `PatientID`, `DoctorID`, `RoomID`, `AdmissionDate`, `ExpectedDischargeDate`, `ActualDischargeDate`, `AdmissionReason`, `Diagnosis`, `Status`, `DischargeSummary`) VALUES (17, 'ADMNSD016', 17, 18, 24, '2026-03-01 14:00:00.000000', '2026-03-06 00:00:00.000000', NULL, 'Seed inpatient monitoring after emergency consult.', 'Allergic rhinitis', 'Admitted', NULL);
INSERT INTO `admissions` (`AdmissionID`, `AdmissionNumber`, `PatientID`, `DoctorID`, `RoomID`, `AdmissionDate`, `ExpectedDischargeDate`, `ActualDischargeDate`, `AdmissionReason`, `Diagnosis`, `Status`, `DischargeSummary`) VALUES (18, 'ADMNSD017', 18, 19, 25, '2026-03-02 14:00:00.000000', '2026-03-08 00:00:00.000000', NULL, 'Seed inpatient monitoring after emergency consult.', 'Lumbar strain', 'Admitted', NULL);
INSERT INTO `admissions` (`AdmissionID`, `AdmissionNumber`, `PatientID`, `DoctorID`, `RoomID`, `AdmissionDate`, `ExpectedDischargeDate`, `ActualDischargeDate`, `AdmissionReason`, `Diagnosis`, `Status`, `DischargeSummary`) VALUES (19, 'ADMNSD018', 19, 20, 26, '2026-03-03 14:00:00.000000', '2026-03-10 00:00:00.000000', NULL, 'Seed inpatient monitoring after emergency consult.', 'Osteoarthritis', 'Admitted', NULL);
INSERT INTO `admissions` (`AdmissionID`, `AdmissionNumber`, `PatientID`, `DoctorID`, `RoomID`, `AdmissionDate`, `ExpectedDischargeDate`, `ActualDischargeDate`, `AdmissionReason`, `Diagnosis`, `Status`, `DischargeSummary`) VALUES (20, 'ADMNSD019', 20, 21, 27, '2026-03-04 14:00:00.000000', '2026-03-12 00:00:00.000000', NULL, 'Seed inpatient monitoring after emergency consult.', 'Acute gastroenteritis', 'Admitted', NULL);
INSERT INTO `admissions` (`AdmissionID`, `AdmissionNumber`, `PatientID`, `DoctorID`, `RoomID`, `AdmissionDate`, `ExpectedDischargeDate`, `ActualDischargeDate`, `AdmissionReason`, `Diagnosis`, `Status`, `DischargeSummary`) VALUES (21, 'ADMNSD020', 21, 22, 28, '2026-03-05 14:00:00.000000', '2026-03-09 00:00:00.000000', NULL, 'Seed inpatient monitoring after emergency consult.', 'Urinary tract infection', 'Admitted', NULL);
INSERT INTO `admissions` (`AdmissionID`, `AdmissionNumber`, `PatientID`, `DoctorID`, `RoomID`, `AdmissionDate`, `ExpectedDischargeDate`, `ActualDischargeDate`, `AdmissionReason`, `Diagnosis`, `Status`, `DischargeSummary`) VALUES (22, 'ADMNSD021', 22, 23, 29, '2026-03-06 14:00:00.000000', '2026-03-11 00:00:00.000000', NULL, 'Seed inpatient monitoring after emergency consult.', 'Essential hypertension', 'Admitted', NULL);
INSERT INTO `admissions` (`AdmissionID`, `AdmissionNumber`, `PatientID`, `DoctorID`, `RoomID`, `AdmissionDate`, `ExpectedDischargeDate`, `ActualDischargeDate`, `AdmissionReason`, `Diagnosis`, `Status`, `DischargeSummary`) VALUES (23, 'ADMNSD022', 23, 24, 30, '2026-03-07 14:00:00.000000', '2026-03-13 00:00:00.000000', NULL, 'Seed inpatient monitoring after emergency consult.', 'Type 2 diabetes mellitus', 'Admitted', NULL);
INSERT INTO `admissions` (`AdmissionID`, `AdmissionNumber`, `PatientID`, `DoctorID`, `RoomID`, `AdmissionDate`, `ExpectedDischargeDate`, `ActualDischargeDate`, `AdmissionReason`, `Diagnosis`, `Status`, `DischargeSummary`) VALUES (24, 'ADMNSD023', 24, 25, 31, '2026-03-08 14:00:00.000000', '2026-03-15 00:00:00.000000', NULL, 'Seed inpatient monitoring after emergency consult.', 'Bronchial asthma', 'Admitted', NULL);
INSERT INTO `admissions` (`AdmissionID`, `AdmissionNumber`, `PatientID`, `DoctorID`, `RoomID`, `AdmissionDate`, `ExpectedDischargeDate`, `ActualDischargeDate`, `AdmissionReason`, `Diagnosis`, `Status`, `DischargeSummary`) VALUES (25, 'ADMNSD024', 25, 26, 32, '2026-03-09 14:00:00.000000', '2026-03-17 00:00:00.000000', NULL, 'Seed inpatient monitoring after emergency consult.', 'Hyperlipidemia', 'Admitted', NULL);
INSERT INTO `admissions` (`AdmissionID`, `AdmissionNumber`, `PatientID`, `DoctorID`, `RoomID`, `AdmissionDate`, `ExpectedDischargeDate`, `ActualDischargeDate`, `AdmissionReason`, `Diagnosis`, `Status`, `DischargeSummary`) VALUES (26, 'ADMNSD025', 26, 27, 33, '2026-03-10 14:00:00.000000', '2026-03-14 00:00:00.000000', NULL, 'Seed inpatient monitoring after emergency consult.', 'Migraine episodes', 'Admitted', NULL);
INSERT INTO `admissions` (`AdmissionID`, `AdmissionNumber`, `PatientID`, `DoctorID`, `RoomID`, `AdmissionDate`, `ExpectedDischargeDate`, `ActualDischargeDate`, `AdmissionReason`, `Diagnosis`, `Status`, `DischargeSummary`) VALUES (27, 'ADMNSD026', 27, 28, 34, '2026-03-11 14:00:00.000000', '2026-03-16 00:00:00.000000', NULL, 'Seed inpatient monitoring after emergency consult.', 'Allergic rhinitis', 'Admitted', NULL);
INSERT INTO `admissions` (`AdmissionID`, `AdmissionNumber`, `PatientID`, `DoctorID`, `RoomID`, `AdmissionDate`, `ExpectedDischargeDate`, `ActualDischargeDate`, `AdmissionReason`, `Diagnosis`, `Status`, `DischargeSummary`) VALUES (28, 'ADMNSD027', 28, 29, 35, '2026-03-12 14:00:00.000000', '2026-03-18 00:00:00.000000', NULL, 'Seed inpatient monitoring after emergency consult.', 'Lumbar strain', 'Admitted', NULL);
INSERT INTO `admissions` (`AdmissionID`, `AdmissionNumber`, `PatientID`, `DoctorID`, `RoomID`, `AdmissionDate`, `ExpectedDischargeDate`, `ActualDischargeDate`, `AdmissionReason`, `Diagnosis`, `Status`, `DischargeSummary`) VALUES (29, 'ADMNSD028', 29, 30, 36, '2026-03-13 14:00:00.000000', '2026-03-20 00:00:00.000000', NULL, 'Seed inpatient monitoring after emergency consult.', 'Osteoarthritis', 'Admitted', NULL);
INSERT INTO `admissions` (`AdmissionID`, `AdmissionNumber`, `PatientID`, `DoctorID`, `RoomID`, `AdmissionDate`, `ExpectedDischargeDate`, `ActualDischargeDate`, `AdmissionReason`, `Diagnosis`, `Status`, `DischargeSummary`) VALUES (30, 'ADMNSD029', 30, 31, 37, '2026-03-14 14:00:00.000000', '2026-03-22 00:00:00.000000', NULL, 'Seed inpatient monitoring after emergency consult.', 'Acute gastroenteritis', 'Admitted', NULL);
INSERT INTO `admissions` (`AdmissionID`, `AdmissionNumber`, `PatientID`, `DoctorID`, `RoomID`, `AdmissionDate`, `ExpectedDischargeDate`, `ActualDischargeDate`, `AdmissionReason`, `Diagnosis`, `Status`, `DischargeSummary`) VALUES (31, 'ADMNSD030', 31, 32, 38, '2026-03-15 14:00:00.000000', '2026-03-19 00:00:00.000000', NULL, 'Seed inpatient monitoring after emergency consult.', 'Urinary tract infection', 'Admitted', NULL);

-- Data for appointmenthistory
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (2, 2, 'Completed', 9, '2026-03-08 23:56:49.000000', 'Seed history for appointment APTSD001.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (3, 3, 'Completed', 9, '2026-03-07 23:56:49.000000', 'Seed history for appointment APTSD002.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (4, 4, 'Completed', 9, '2026-03-06 23:56:49.000000', 'Seed history for appointment APTSD003.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (5, 5, 'Completed', 9, '2026-03-05 23:56:49.000000', 'Seed history for appointment APTSD004.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (6, 6, 'Completed', 9, '2026-03-04 23:56:50.000000', 'Seed history for appointment APTSD005.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (7, 7, 'Completed', 9, '2026-03-03 23:56:50.000000', 'Seed history for appointment APTSD006.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (8, 8, 'Completed', 9, '2026-03-02 23:56:50.000000', 'Seed history for appointment APTSD007.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (9, 9, 'Completed', 9, '2026-03-01 23:56:50.000000', 'Seed history for appointment APTSD008.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (10, 10, 'Completed', 9, '2026-02-28 23:56:50.000000', 'Seed history for appointment APTSD009.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (11, 11, 'Completed', 9, '2026-02-27 23:56:51.000000', 'Seed history for appointment APTSD010.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (12, 12, 'Completed', 9, '2026-02-26 23:56:51.000000', 'Seed history for appointment APTSD011.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (13, 13, 'Completed', 9, '2026-02-25 23:56:51.000000', 'Seed history for appointment APTSD012.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (14, 14, 'Completed', 9, '2026-02-24 23:56:51.000000', 'Seed history for appointment APTSD013.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (15, 15, 'Completed', 9, '2026-02-23 23:56:51.000000', 'Seed history for appointment APTSD014.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (16, 16, 'Completed', 9, '2026-02-22 23:56:51.000000', 'Seed history for appointment APTSD015.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (17, 17, 'Completed', 9, '2026-02-21 23:56:51.000000', 'Seed history for appointment APTSD016.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (18, 18, 'Completed', 9, '2026-02-20 23:56:55.000000', 'Seed history for appointment APTSD017.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (19, 19, 'Completed', 9, '2026-02-19 23:56:55.000000', 'Seed history for appointment APTSD018.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (20, 20, 'Completed', 9, '2026-02-18 23:56:56.000000', 'Seed history for appointment APTSD019.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (21, 21, 'Completed', 9, '2026-02-17 23:56:56.000000', 'Seed history for appointment APTSD020.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (22, 22, 'Completed', 9, '2026-02-16 23:56:56.000000', 'Seed history for appointment APTSD021.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (23, 23, 'Completed', 9, '2026-02-15 23:56:56.000000', 'Seed history for appointment APTSD022.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (24, 24, 'Completed', 9, '2026-02-14 23:56:56.000000', 'Seed history for appointment APTSD023.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (25, 25, 'Completed', 9, '2026-02-13 23:56:57.000000', 'Seed history for appointment APTSD024.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (26, 26, 'Completed', 9, '2026-02-12 23:56:57.000000', 'Seed history for appointment APTSD025.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (27, 27, 'Completed', 9, '2026-02-11 23:56:57.000000', 'Seed history for appointment APTSD026.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (28, 28, 'Completed', 9, '2026-02-10 23:56:57.000000', 'Seed history for appointment APTSD027.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (29, 29, 'Completed', 9, '2026-02-09 23:56:57.000000', 'Seed history for appointment APTSD028.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (30, 30, 'Completed', 9, '2026-02-08 23:56:57.000000', 'Seed history for appointment APTSD029.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (31, 31, 'Completed', 9, '2026-02-07 23:56:58.000000', 'Seed history for appointment APTSD030.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (32, 32, 'Scheduled', 9, '2026-02-06 23:56:59.000000', 'Seed history for appointment APTSD031.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (33, 33, 'Scheduled', 9, '2026-02-05 23:56:59.000000', 'Seed history for appointment APTSD032.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (34, 34, 'Scheduled', 9, '2026-02-04 23:56:59.000000', 'Seed history for appointment APTSD033.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (35, 35, 'Scheduled', 9, '2026-02-03 23:57:00.000000', 'Seed history for appointment APTSD034.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (36, 36, 'Scheduled', 9, '2026-02-02 23:57:00.000000', 'Seed history for appointment APTSD035.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (37, 37, 'Scheduled', 9, '2026-02-01 23:57:00.000000', 'Seed history for appointment APTSD036.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (38, 38, 'Scheduled', 9, '2026-01-31 23:57:01.000000', 'Seed history for appointment APTSD037.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (39, 39, 'Scheduled', 9, '2026-01-30 23:57:02.000000', 'Seed history for appointment APTSD038.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (40, 40, 'Scheduled', 9, '2026-01-29 23:57:03.000000', 'Seed history for appointment APTSD039.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (41, 41, 'Scheduled', 9, '2026-01-28 23:57:03.000000', 'Seed history for appointment APTSD040.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (42, 42, 'Cancelled', 9, '2026-01-27 23:57:03.000000', 'Seed history for appointment APTSD041.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (43, 43, 'Cancelled', 9, '2026-01-26 23:57:03.000000', 'Seed history for appointment APTSD042.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (44, 44, 'Cancelled', 9, '2026-01-25 23:57:03.000000', 'Seed history for appointment APTSD043.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (45, 45, 'Cancelled', 9, '2026-01-24 23:57:03.000000', 'Seed history for appointment APTSD044.');
INSERT INTO `appointmenthistory` (`HistoryID`, `AppointmentID`, `Status`, `ChangedBy`, `ChangedDate`, `Notes`) VALUES (46, 46, 'Cancelled', 9, '2026-01-23 23:57:04.000000', 'Seed history for appointment APTSD045.');

-- Data for appointments
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (2, 'APTSD001', 2, 3, '2026-02-03 00:00:00.000000', '09:00:00', 'Consultation', 'Completed', 'Seeded consultation note 01.', 30, 9, '2026-02-01 09:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (3, 'APTSD002', 3, 4, '2026-02-04 00:00:00.000000', '10:00:00', 'Follow-up', 'Completed', 'Seeded consultation note 02.', 45, 9, '2026-02-02 10:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (4, 'APTSD003', 4, 5, '2026-02-05 00:00:00.000000', '11:00:00', 'Emergency', 'Completed', 'Seeded consultation note 03.', 60, 9, '2026-02-03 11:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (5, 'APTSD004', 5, 6, '2026-02-06 00:00:00.000000', '12:00:00', 'Check-up', 'Completed', 'Seeded consultation note 04.', 15, 9, '2026-02-04 12:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (6, 'APTSD005', 6, 7, '2026-02-07 00:00:00.000000', '13:00:00', 'Consultation', 'Completed', 'Seeded consultation note 05.', 30, 9, '2026-02-05 13:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (7, 'APTSD006', 7, 8, '2026-02-08 00:00:00.000000', '14:00:00', 'Follow-up', 'Completed', 'Seeded consultation note 06.', 45, 9, '2026-02-06 14:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (8, 'APTSD007', 8, 9, '2026-02-09 00:00:00.000000', '15:00:00', 'Emergency', 'Completed', 'Seeded consultation note 07.', 60, 9, '2026-02-07 15:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (9, 'APTSD008', 9, 10, '2026-02-10 00:00:00.000000', '08:00:00', 'Check-up', 'Completed', 'Seeded consultation note 08.', 15, 9, '2026-02-08 08:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (10, 'APTSD009', 10, 11, '2026-02-11 00:00:00.000000', '09:00:00', 'Consultation', 'Completed', 'Seeded consultation note 09.', 30, 9, '2026-02-09 09:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (11, 'APTSD010', 11, 12, '2026-02-12 00:00:00.000000', '10:00:00', 'Follow-up', 'Completed', 'Seeded consultation note 10.', 45, 9, '2026-02-10 10:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (12, 'APTSD011', 12, 13, '2026-02-13 00:00:00.000000', '11:00:00', 'Emergency', 'Completed', 'Seeded consultation note 11.', 60, 9, '2026-02-11 11:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (13, 'APTSD012', 13, 14, '2026-02-14 00:00:00.000000', '12:00:00', 'Check-up', 'Completed', 'Seeded consultation note 12.', 15, 9, '2026-02-12 12:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (14, 'APTSD013', 14, 15, '2026-02-15 00:00:00.000000', '13:00:00', 'Consultation', 'Completed', 'Seeded consultation note 13.', 30, 9, '2026-02-13 13:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (15, 'APTSD014', 15, 16, '2026-02-16 00:00:00.000000', '14:00:00', 'Follow-up', 'Completed', 'Seeded consultation note 14.', 45, 9, '2026-02-14 14:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (16, 'APTSD015', 16, 17, '2026-02-17 00:00:00.000000', '15:00:00', 'Emergency', 'Completed', 'Seeded consultation note 15.', 60, 9, '2026-02-15 15:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (17, 'APTSD016', 17, 18, '2026-02-18 00:00:00.000000', '08:00:00', 'Check-up', 'Completed', 'Seeded consultation note 16.', 15, 9, '2026-02-16 08:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (18, 'APTSD017', 18, 19, '2026-02-19 00:00:00.000000', '09:00:00', 'Consultation', 'Completed', 'Seeded consultation note 17.', 30, 9, '2026-02-17 09:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (19, 'APTSD018', 19, 20, '2026-02-20 00:00:00.000000', '10:00:00', 'Follow-up', 'Completed', 'Seeded consultation note 18.', 45, 9, '2026-02-18 10:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (20, 'APTSD019', 20, 21, '2026-02-21 00:00:00.000000', '11:00:00', 'Emergency', 'Completed', 'Seeded consultation note 19.', 60, 9, '2026-02-19 11:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (21, 'APTSD020', 21, 22, '2026-02-22 00:00:00.000000', '12:00:00', 'Check-up', 'Completed', 'Seeded consultation note 20.', 15, 9, '2026-02-20 12:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (22, 'APTSD021', 22, 23, '2026-02-23 00:00:00.000000', '13:00:00', 'Consultation', 'Completed', 'Seeded consultation note 21.', 30, 9, '2026-02-21 13:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (23, 'APTSD022', 23, 24, '2026-02-24 00:00:00.000000', '14:00:00', 'Follow-up', 'Completed', 'Seeded consultation note 22.', 45, 9, '2026-02-22 14:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (24, 'APTSD023', 24, 25, '2026-02-25 00:00:00.000000', '15:00:00', 'Emergency', 'Completed', 'Seeded consultation note 23.', 60, 9, '2026-02-23 15:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (25, 'APTSD024', 25, 26, '2026-02-26 00:00:00.000000', '08:00:00', 'Check-up', 'Completed', 'Seeded consultation note 24.', 15, 9, '2026-02-24 08:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (26, 'APTSD025', 26, 27, '2026-02-27 00:00:00.000000', '09:00:00', 'Consultation', 'Completed', 'Seeded consultation note 25.', 30, 9, '2026-02-25 09:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (27, 'APTSD026', 27, 28, '2026-02-28 00:00:00.000000', '10:00:00', 'Follow-up', 'Completed', 'Seeded consultation note 26.', 45, 9, '2026-02-26 10:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (28, 'APTSD027', 28, 29, '2026-03-01 00:00:00.000000', '11:00:00', 'Emergency', 'Completed', 'Seeded consultation note 27.', 60, 9, '2026-02-27 11:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (29, 'APTSD028', 29, 30, '2026-03-02 00:00:00.000000', '12:00:00', 'Check-up', 'Completed', 'Seeded consultation note 28.', 15, 9, '2026-02-28 12:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (30, 'APTSD029', 30, 31, '2026-03-03 00:00:00.000000', '13:00:00', 'Consultation', 'Completed', 'Seeded consultation note 29.', 30, 9, '2026-03-01 13:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (31, 'APTSD030', 31, 32, '2026-03-04 00:00:00.000000', '14:00:00', 'Follow-up', 'Completed', 'Seeded consultation note 30.', 45, 9, '2026-03-02 14:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (32, 'APTSD031', 2, 3, '2026-03-10 00:00:00.000000', '15:00:00', 'Emergency', 'Scheduled', 'Seeded consultation note 31.', 60, 9, '2026-03-08 15:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (33, 'APTSD032', 3, 4, '2026-03-11 00:00:00.000000', '08:00:00', 'Check-up', 'Scheduled', 'Seeded consultation note 32.', 15, 9, '2026-03-09 08:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (34, 'APTSD033', 4, 5, '2026-03-12 00:00:00.000000', '09:00:00', 'Consultation', 'Scheduled', 'Seeded consultation note 33.', 30, 9, '2026-03-10 09:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (35, 'APTSD034', 5, 6, '2026-03-13 00:00:00.000000', '10:00:00', 'Follow-up', 'Scheduled', 'Seeded consultation note 34.', 45, 9, '2026-03-11 10:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (36, 'APTSD035', 6, 7, '2026-03-14 00:00:00.000000', '11:00:00', 'Emergency', 'Scheduled', 'Seeded consultation note 35.', 60, 9, '2026-03-12 11:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (37, 'APTSD036', 7, 8, '2026-03-15 00:00:00.000000', '12:00:00', 'Check-up', 'Scheduled', 'Seeded consultation note 36.', 15, 9, '2026-03-13 12:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (38, 'APTSD037', 8, 9, '2026-03-16 00:00:00.000000', '13:00:00', 'Consultation', 'Scheduled', 'Seeded consultation note 37.', 30, 9, '2026-03-14 13:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (39, 'APTSD038', 9, 10, '2026-03-17 00:00:00.000000', '14:00:00', 'Follow-up', 'Scheduled', 'Seeded consultation note 38.', 45, 9, '2026-03-15 14:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (40, 'APTSD039', 10, 11, '2026-03-18 00:00:00.000000', '15:00:00', 'Emergency', 'Scheduled', 'Seeded consultation note 39.', 60, 9, '2026-03-16 15:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (41, 'APTSD040', 11, 12, '2026-03-19 00:00:00.000000', '08:00:00', 'Check-up', 'Scheduled', 'Seeded consultation note 40.', 15, 9, '2026-03-17 08:00:00.000000', 'Patient instructed to arrive 15 minutes early.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (42, 'APTSD041', 12, 13, '2026-03-20 00:00:00.000000', '09:00:00', 'Consultation', 'Cancelled', 'Seeded consultation note 41.', 30, 9, '2026-03-18 09:00:00.000000', 'Cancelled by patient due to conflict.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (43, 'APTSD042', 13, 14, '2026-03-21 00:00:00.000000', '10:00:00', 'Follow-up', 'Cancelled', 'Seeded consultation note 42.', 45, 9, '2026-03-19 10:00:00.000000', 'Cancelled by patient due to conflict.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (44, 'APTSD043', 14, 15, '2026-03-22 00:00:00.000000', '11:00:00', 'Emergency', 'Cancelled', 'Seeded consultation note 43.', 60, 9, '2026-03-20 11:00:00.000000', 'Cancelled by patient due to conflict.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (45, 'APTSD044', 15, 16, '2026-03-23 00:00:00.000000', '12:00:00', 'Check-up', 'Cancelled', 'Seeded consultation note 44.', 15, 9, '2026-03-21 12:00:00.000000', 'Cancelled by patient due to conflict.');
INSERT INTO `appointments` (`AppointmentID`, `AppointmentCode`, `PatientID`, `DoctorID`, `AppointmentDate`, `AppointmentTime`, `AppointmentType`, `Status`, `Reason`, `Duration`, `CreatedBy`, `CreatedDate`, `Notes`) VALUES (46, 'APTSD045', 16, 17, '2026-03-24 00:00:00.000000', '13:00:00', 'Consultation', 'Cancelled', 'Seeded consultation note 45.', 30, 9, '2026-03-22 13:00:00.000000', 'Cancelled by patient due to conflict.');

-- Data for auditlogs
INSERT INTO `auditlogs` (`LogID`, `UserID`, `Action`, `TableName`, `RecordID`, `OldValue`, `NewValue`, `IPAddress`, `MachineName`, `LogDate`) VALUES (2, 6, 'CREATE', 'patients', 1, NULL, NULL, '192.168.10.21', 'SEED-STATION-1', '2026-02-18 08:00:00.000000');
INSERT INTO `auditlogs` (`LogID`, `UserID`, `Action`, `TableName`, `RecordID`, `OldValue`, `NewValue`, `IPAddress`, `MachineName`, `LogDate`) VALUES (3, 7, 'UPDATE', 'appointments', 2, NULL, NULL, '192.168.10.22', 'SEED-STATION-2', '2026-02-19 09:00:00.000000');
INSERT INTO `auditlogs` (`LogID`, `UserID`, `Action`, `TableName`, `RecordID`, `OldValue`, `NewValue`, `IPAddress`, `MachineName`, `LogDate`) VALUES (4, 8, 'VIEW', 'invoices', 3, NULL, NULL, '192.168.10.23', 'SEED-STATION-3', '2026-02-20 10:00:00.000000');
INSERT INTO `auditlogs` (`LogID`, `UserID`, `Action`, `TableName`, `RecordID`, `OldValue`, `NewValue`, `IPAddress`, `MachineName`, `LogDate`) VALUES (5, 9, 'EXPORT', 'laborders', 4, NULL, NULL, '192.168.10.24', 'SEED-STATION-4', '2026-02-21 11:00:00.000000');
INSERT INTO `auditlogs` (`LogID`, `UserID`, `Action`, `TableName`, `RecordID`, `OldValue`, `NewValue`, `IPAddress`, `MachineName`, `LogDate`) VALUES (6, 10, 'CREATE', 'admissions', 5, NULL, NULL, '192.168.10.25', 'SEED-STATION-5', '2026-02-22 12:00:00.000000');
INSERT INTO `auditlogs` (`LogID`, `UserID`, `Action`, `TableName`, `RecordID`, `OldValue`, `NewValue`, `IPAddress`, `MachineName`, `LogDate`) VALUES (7, 11, 'UPDATE', 'pharmacysales', 6, NULL, NULL, '192.168.10.26', 'SEED-STATION-6', '2026-02-23 13:00:00.000000');
INSERT INTO `auditlogs` (`LogID`, `UserID`, `Action`, `TableName`, `RecordID`, `OldValue`, `NewValue`, `IPAddress`, `MachineName`, `LogDate`) VALUES (8, 12, 'VIEW', 'patients', 7, NULL, NULL, '192.168.10.27', 'SEED-STATION-1', '2026-02-24 14:00:00.000000');
INSERT INTO `auditlogs` (`LogID`, `UserID`, `Action`, `TableName`, `RecordID`, `OldValue`, `NewValue`, `IPAddress`, `MachineName`, `LogDate`) VALUES (9, 13, 'EXPORT', 'appointments', 8, NULL, NULL, '192.168.10.28', 'SEED-STATION-2', '2026-02-25 07:00:00.000000');
INSERT INTO `auditlogs` (`LogID`, `UserID`, `Action`, `TableName`, `RecordID`, `OldValue`, `NewValue`, `IPAddress`, `MachineName`, `LogDate`) VALUES (10, 14, 'CREATE', 'invoices', 9, NULL, NULL, '192.168.10.29', 'SEED-STATION-3', '2026-02-26 08:00:00.000000');
INSERT INTO `auditlogs` (`LogID`, `UserID`, `Action`, `TableName`, `RecordID`, `OldValue`, `NewValue`, `IPAddress`, `MachineName`, `LogDate`) VALUES (11, 15, 'UPDATE', 'laborders', 10, NULL, NULL, '192.168.10.30', 'SEED-STATION-4', '2026-02-27 09:00:00.000000');
INSERT INTO `auditlogs` (`LogID`, `UserID`, `Action`, `TableName`, `RecordID`, `OldValue`, `NewValue`, `IPAddress`, `MachineName`, `LogDate`) VALUES (12, 16, 'VIEW', 'admissions', 11, NULL, NULL, '192.168.10.31', 'SEED-STATION-5', '2026-02-28 10:00:00.000000');
INSERT INTO `auditlogs` (`LogID`, `UserID`, `Action`, `TableName`, `RecordID`, `OldValue`, `NewValue`, `IPAddress`, `MachineName`, `LogDate`) VALUES (13, 17, 'EXPORT', 'pharmacysales', 12, NULL, NULL, '192.168.10.32', 'SEED-STATION-6', '2026-03-01 11:00:00.000000');
INSERT INTO `auditlogs` (`LogID`, `UserID`, `Action`, `TableName`, `RecordID`, `OldValue`, `NewValue`, `IPAddress`, `MachineName`, `LogDate`) VALUES (14, 18, 'CREATE', 'patients', 13, NULL, NULL, '192.168.10.33', 'SEED-STATION-1', '2026-03-02 12:00:00.000000');
INSERT INTO `auditlogs` (`LogID`, `UserID`, `Action`, `TableName`, `RecordID`, `OldValue`, `NewValue`, `IPAddress`, `MachineName`, `LogDate`) VALUES (15, 19, 'UPDATE', 'appointments', 14, NULL, NULL, '192.168.10.34', 'SEED-STATION-2', '2026-03-03 13:00:00.000000');
INSERT INTO `auditlogs` (`LogID`, `UserID`, `Action`, `TableName`, `RecordID`, `OldValue`, `NewValue`, `IPAddress`, `MachineName`, `LogDate`) VALUES (16, 20, 'VIEW', 'invoices', 15, NULL, NULL, '192.168.10.35', 'SEED-STATION-3', '2026-03-04 14:00:00.000000');
INSERT INTO `auditlogs` (`LogID`, `UserID`, `Action`, `TableName`, `RecordID`, `OldValue`, `NewValue`, `IPAddress`, `MachineName`, `LogDate`) VALUES (17, 21, 'EXPORT', 'laborders', 16, NULL, NULL, '192.168.10.36', 'SEED-STATION-4', '2026-03-05 07:00:00.000000');
INSERT INTO `auditlogs` (`LogID`, `UserID`, `Action`, `TableName`, `RecordID`, `OldValue`, `NewValue`, `IPAddress`, `MachineName`, `LogDate`) VALUES (18, 22, 'CREATE', 'admissions', 17, NULL, NULL, '192.168.10.37', 'SEED-STATION-5', '2026-03-06 08:00:00.000000');
INSERT INTO `auditlogs` (`LogID`, `UserID`, `Action`, `TableName`, `RecordID`, `OldValue`, `NewValue`, `IPAddress`, `MachineName`, `LogDate`) VALUES (19, 23, 'UPDATE', 'pharmacysales', 18, NULL, NULL, '192.168.10.38', 'SEED-STATION-6', '2026-03-07 09:00:00.000000');
INSERT INTO `auditlogs` (`LogID`, `UserID`, `Action`, `TableName`, `RecordID`, `OldValue`, `NewValue`, `IPAddress`, `MachineName`, `LogDate`) VALUES (20, 24, 'VIEW', 'patients', 19, NULL, NULL, '192.168.10.39', 'SEED-STATION-1', '2026-03-08 10:00:00.000000');
INSERT INTO `auditlogs` (`LogID`, `UserID`, `Action`, `TableName`, `RecordID`, `OldValue`, `NewValue`, `IPAddress`, `MachineName`, `LogDate`) VALUES (21, 25, 'EXPORT', 'appointments', 20, NULL, NULL, '192.168.10.40', 'SEED-STATION-2', '2026-03-09 11:00:00.000000');
INSERT INTO `auditlogs` (`LogID`, `UserID`, `Action`, `TableName`, `RecordID`, `OldValue`, `NewValue`, `IPAddress`, `MachineName`, `LogDate`) VALUES (22, 26, 'CREATE', 'invoices', 21, NULL, NULL, '192.168.10.41', 'SEED-STATION-3', '2026-03-10 12:00:00.000000');
INSERT INTO `auditlogs` (`LogID`, `UserID`, `Action`, `TableName`, `RecordID`, `OldValue`, `NewValue`, `IPAddress`, `MachineName`, `LogDate`) VALUES (23, 27, 'UPDATE', 'laborders', 22, NULL, NULL, '192.168.10.42', 'SEED-STATION-4', '2026-03-11 13:00:00.000000');
INSERT INTO `auditlogs` (`LogID`, `UserID`, `Action`, `TableName`, `RecordID`, `OldValue`, `NewValue`, `IPAddress`, `MachineName`, `LogDate`) VALUES (24, 28, 'VIEW', 'admissions', 23, NULL, NULL, '192.168.10.43', 'SEED-STATION-5', '2026-03-12 14:00:00.000000');
INSERT INTO `auditlogs` (`LogID`, `UserID`, `Action`, `TableName`, `RecordID`, `OldValue`, `NewValue`, `IPAddress`, `MachineName`, `LogDate`) VALUES (25, 29, 'EXPORT', 'pharmacysales', 24, NULL, NULL, '192.168.10.44', 'SEED-STATION-6', '2026-03-13 07:00:00.000000');
INSERT INTO `auditlogs` (`LogID`, `UserID`, `Action`, `TableName`, `RecordID`, `OldValue`, `NewValue`, `IPAddress`, `MachineName`, `LogDate`) VALUES (26, 30, 'CREATE', 'patients', 25, NULL, NULL, '192.168.10.45', 'SEED-STATION-1', '2026-03-14 08:00:00.000000');
INSERT INTO `auditlogs` (`LogID`, `UserID`, `Action`, `TableName`, `RecordID`, `OldValue`, `NewValue`, `IPAddress`, `MachineName`, `LogDate`) VALUES (27, 31, 'UPDATE', 'appointments', 26, NULL, NULL, '192.168.10.46', 'SEED-STATION-2', '2026-03-15 09:00:00.000000');
INSERT INTO `auditlogs` (`LogID`, `UserID`, `Action`, `TableName`, `RecordID`, `OldValue`, `NewValue`, `IPAddress`, `MachineName`, `LogDate`) VALUES (28, 32, 'VIEW', 'invoices', 27, NULL, NULL, '192.168.10.47', 'SEED-STATION-3', '2026-03-16 10:00:00.000000');
INSERT INTO `auditlogs` (`LogID`, `UserID`, `Action`, `TableName`, `RecordID`, `OldValue`, `NewValue`, `IPAddress`, `MachineName`, `LogDate`) VALUES (29, 33, 'EXPORT', 'laborders', 28, NULL, NULL, '192.168.10.48', 'SEED-STATION-4', '2026-03-17 11:00:00.000000');
INSERT INTO `auditlogs` (`LogID`, `UserID`, `Action`, `TableName`, `RecordID`, `OldValue`, `NewValue`, `IPAddress`, `MachineName`, `LogDate`) VALUES (30, 34, 'CREATE', 'admissions', 29, NULL, NULL, '192.168.10.49', 'SEED-STATION-5', '2026-03-18 12:00:00.000000');
INSERT INTO `auditlogs` (`LogID`, `UserID`, `Action`, `TableName`, `RecordID`, `OldValue`, `NewValue`, `IPAddress`, `MachineName`, `LogDate`) VALUES (31, 35, 'UPDATE', 'pharmacysales', 30, NULL, NULL, '192.168.10.50', 'SEED-STATION-6', '2026-03-19 13:00:00.000000');

-- Data for bedallocations
INSERT INTO `bedallocations` (`AllocationID`, `AdmissionID`, `RoomID`, `BedNumber`, `AllocationDate`, `DischargeDate`, `Status`) VALUES (2, 2, 9, 'B-01', '2026-02-14 15:00:00.000000', '2026-02-18 12:00:00.000000', 'Discharged');
INSERT INTO `bedallocations` (`AllocationID`, `AdmissionID`, `RoomID`, `BedNumber`, `AllocationDate`, `DischargeDate`, `Status`) VALUES (3, 3, 10, 'B-02', '2026-02-15 15:00:00.000000', '2026-02-19 12:00:00.000000', 'Discharged');
INSERT INTO `bedallocations` (`AllocationID`, `AdmissionID`, `RoomID`, `BedNumber`, `AllocationDate`, `DischargeDate`, `Status`) VALUES (4, 4, 11, 'B-01', '2026-02-16 15:00:00.000000', '2026-02-20 12:00:00.000000', 'Discharged');
INSERT INTO `bedallocations` (`AllocationID`, `AdmissionID`, `RoomID`, `BedNumber`, `AllocationDate`, `DischargeDate`, `Status`) VALUES (5, 5, 12, 'B-02', '2026-02-17 15:00:00.000000', '2026-02-21 12:00:00.000000', 'Discharged');
INSERT INTO `bedallocations` (`AllocationID`, `AdmissionID`, `RoomID`, `BedNumber`, `AllocationDate`, `DischargeDate`, `Status`) VALUES (6, 6, 13, 'B-01', '2026-02-18 15:00:00.000000', '2026-02-22 12:00:00.000000', 'Discharged');
INSERT INTO `bedallocations` (`AllocationID`, `AdmissionID`, `RoomID`, `BedNumber`, `AllocationDate`, `DischargeDate`, `Status`) VALUES (7, 7, 14, 'B-02', '2026-02-19 15:00:00.000000', '2026-02-23 12:00:00.000000', 'Discharged');
INSERT INTO `bedallocations` (`AllocationID`, `AdmissionID`, `RoomID`, `BedNumber`, `AllocationDate`, `DischargeDate`, `Status`) VALUES (8, 8, 15, 'B-01', '2026-02-20 15:00:00.000000', '2026-02-24 12:00:00.000000', 'Discharged');
INSERT INTO `bedallocations` (`AllocationID`, `AdmissionID`, `RoomID`, `BedNumber`, `AllocationDate`, `DischargeDate`, `Status`) VALUES (9, 9, 16, 'B-02', '2026-02-21 15:00:00.000000', '2026-02-25 12:00:00.000000', 'Discharged');
INSERT INTO `bedallocations` (`AllocationID`, `AdmissionID`, `RoomID`, `BedNumber`, `AllocationDate`, `DischargeDate`, `Status`) VALUES (10, 10, 17, 'B-01', '2026-02-22 15:00:00.000000', '2026-02-26 12:00:00.000000', 'Discharged');
INSERT INTO `bedallocations` (`AllocationID`, `AdmissionID`, `RoomID`, `BedNumber`, `AllocationDate`, `DischargeDate`, `Status`) VALUES (11, 11, 18, 'B-02', '2026-02-23 15:00:00.000000', '2026-02-27 12:00:00.000000', 'Discharged');
INSERT INTO `bedallocations` (`AllocationID`, `AdmissionID`, `RoomID`, `BedNumber`, `AllocationDate`, `DischargeDate`, `Status`) VALUES (12, 12, 19, 'B-01', '2026-02-24 15:00:00.000000', '2026-02-28 12:00:00.000000', 'Discharged');
INSERT INTO `bedallocations` (`AllocationID`, `AdmissionID`, `RoomID`, `BedNumber`, `AllocationDate`, `DischargeDate`, `Status`) VALUES (13, 13, 20, 'B-02', '2026-02-25 15:00:00.000000', '2026-03-01 12:00:00.000000', 'Discharged');
INSERT INTO `bedallocations` (`AllocationID`, `AdmissionID`, `RoomID`, `BedNumber`, `AllocationDate`, `DischargeDate`, `Status`) VALUES (14, 14, 21, 'B-01', '2026-02-26 15:00:00.000000', '2026-03-02 12:00:00.000000', 'Discharged');
INSERT INTO `bedallocations` (`AllocationID`, `AdmissionID`, `RoomID`, `BedNumber`, `AllocationDate`, `DischargeDate`, `Status`) VALUES (15, 15, 22, 'B-02', '2026-02-27 15:00:00.000000', '2026-03-03 12:00:00.000000', 'Discharged');
INSERT INTO `bedallocations` (`AllocationID`, `AdmissionID`, `RoomID`, `BedNumber`, `AllocationDate`, `DischargeDate`, `Status`) VALUES (16, 16, 23, 'B-01', '2026-02-28 15:00:00.000000', '2026-03-04 12:00:00.000000', 'Discharged');
INSERT INTO `bedallocations` (`AllocationID`, `AdmissionID`, `RoomID`, `BedNumber`, `AllocationDate`, `DischargeDate`, `Status`) VALUES (17, 17, 24, 'B-02', '2026-03-01 15:00:00.000000', NULL, 'Occupied');
INSERT INTO `bedallocations` (`AllocationID`, `AdmissionID`, `RoomID`, `BedNumber`, `AllocationDate`, `DischargeDate`, `Status`) VALUES (18, 18, 25, 'B-01', '2026-03-02 15:00:00.000000', NULL, 'Occupied');
INSERT INTO `bedallocations` (`AllocationID`, `AdmissionID`, `RoomID`, `BedNumber`, `AllocationDate`, `DischargeDate`, `Status`) VALUES (19, 19, 26, 'B-02', '2026-03-03 15:00:00.000000', NULL, 'Occupied');
INSERT INTO `bedallocations` (`AllocationID`, `AdmissionID`, `RoomID`, `BedNumber`, `AllocationDate`, `DischargeDate`, `Status`) VALUES (20, 20, 27, 'B-01', '2026-03-04 15:00:00.000000', NULL, 'Occupied');
INSERT INTO `bedallocations` (`AllocationID`, `AdmissionID`, `RoomID`, `BedNumber`, `AllocationDate`, `DischargeDate`, `Status`) VALUES (21, 21, 28, 'B-02', '2026-03-05 15:00:00.000000', NULL, 'Occupied');
INSERT INTO `bedallocations` (`AllocationID`, `AdmissionID`, `RoomID`, `BedNumber`, `AllocationDate`, `DischargeDate`, `Status`) VALUES (22, 22, 29, 'B-01', '2026-03-06 15:00:00.000000', NULL, 'Occupied');
INSERT INTO `bedallocations` (`AllocationID`, `AdmissionID`, `RoomID`, `BedNumber`, `AllocationDate`, `DischargeDate`, `Status`) VALUES (23, 23, 30, 'B-02', '2026-03-07 15:00:00.000000', NULL, 'Occupied');
INSERT INTO `bedallocations` (`AllocationID`, `AdmissionID`, `RoomID`, `BedNumber`, `AllocationDate`, `DischargeDate`, `Status`) VALUES (24, 24, 31, 'B-01', '2026-03-08 15:00:00.000000', NULL, 'Occupied');
INSERT INTO `bedallocations` (`AllocationID`, `AdmissionID`, `RoomID`, `BedNumber`, `AllocationDate`, `DischargeDate`, `Status`) VALUES (25, 25, 32, 'B-02', '2026-03-09 15:00:00.000000', NULL, 'Occupied');
INSERT INTO `bedallocations` (`AllocationID`, `AdmissionID`, `RoomID`, `BedNumber`, `AllocationDate`, `DischargeDate`, `Status`) VALUES (26, 26, 33, 'B-01', '2026-03-10 15:00:00.000000', NULL, 'Occupied');
INSERT INTO `bedallocations` (`AllocationID`, `AdmissionID`, `RoomID`, `BedNumber`, `AllocationDate`, `DischargeDate`, `Status`) VALUES (27, 27, 34, 'B-02', '2026-03-11 15:00:00.000000', NULL, 'Occupied');
INSERT INTO `bedallocations` (`AllocationID`, `AdmissionID`, `RoomID`, `BedNumber`, `AllocationDate`, `DischargeDate`, `Status`) VALUES (28, 28, 35, 'B-01', '2026-03-12 15:00:00.000000', NULL, 'Occupied');
INSERT INTO `bedallocations` (`AllocationID`, `AdmissionID`, `RoomID`, `BedNumber`, `AllocationDate`, `DischargeDate`, `Status`) VALUES (29, 29, 36, 'B-02', '2026-03-13 15:00:00.000000', NULL, 'Occupied');
INSERT INTO `bedallocations` (`AllocationID`, `AdmissionID`, `RoomID`, `BedNumber`, `AllocationDate`, `DischargeDate`, `Status`) VALUES (30, 30, 37, 'B-01', '2026-03-14 15:00:00.000000', NULL, 'Occupied');
INSERT INTO `bedallocations` (`AllocationID`, `AdmissionID`, `RoomID`, `BedNumber`, `AllocationDate`, `DischargeDate`, `Status`) VALUES (31, 31, 38, 'B-02', '2026-03-15 15:00:00.000000', NULL, 'Occupied');

-- Data for doctors
INSERT INTO `doctors` (`DoctorID`, `UserID`, `DoctorCode`, `SpecializationID`, `Qualification`, `LicenseNumber`, `YearsOfExperience`, `ConsultationFee`, `IsAvailable`, `JoiningDate`) VALUES (3, 14, 'DOCSD001', 11, 'MD, Specialty Board Certified', 'PRCSD001', 6, 1275.00, 1, '2024-12-31 00:00:00.000000');
INSERT INTO `doctors` (`DoctorID`, `UserID`, `DoctorCode`, `SpecializationID`, `Qualification`, `LicenseNumber`, `YearsOfExperience`, `ConsultationFee`, `IsAvailable`, `JoiningDate`) VALUES (4, 15, 'DOCSD002', 12, 'MD, Specialty Board Certified', 'PRCSD002', 7, 1350.00, 1, '2024-12-18 00:00:00.000000');
INSERT INTO `doctors` (`DoctorID`, `UserID`, `DoctorCode`, `SpecializationID`, `Qualification`, `LicenseNumber`, `YearsOfExperience`, `ConsultationFee`, `IsAvailable`, `JoiningDate`) VALUES (5, 16, 'DOCSD003', 13, 'MD, Specialty Board Certified', 'PRCSD003', 8, 1425.00, 1, '2024-12-05 00:00:00.000000');
INSERT INTO `doctors` (`DoctorID`, `UserID`, `DoctorCode`, `SpecializationID`, `Qualification`, `LicenseNumber`, `YearsOfExperience`, `ConsultationFee`, `IsAvailable`, `JoiningDate`) VALUES (6, 17, 'DOCSD004', 14, 'MD, Specialty Board Certified', 'PRCSD004', 9, 1500.00, 1, '2024-11-22 00:00:00.000000');
INSERT INTO `doctors` (`DoctorID`, `UserID`, `DoctorCode`, `SpecializationID`, `Qualification`, `LicenseNumber`, `YearsOfExperience`, `ConsultationFee`, `IsAvailable`, `JoiningDate`) VALUES (7, 18, 'DOCSD005', 15, 'MD, Specialty Board Certified', 'PRCSD005', 10, 1575.00, 1, '2024-11-09 00:00:00.000000');
INSERT INTO `doctors` (`DoctorID`, `UserID`, `DoctorCode`, `SpecializationID`, `Qualification`, `LicenseNumber`, `YearsOfExperience`, `ConsultationFee`, `IsAvailable`, `JoiningDate`) VALUES (8, 19, 'DOCSD006', 16, 'MD, Specialty Board Certified', 'PRCSD006', 11, 1650.00, 1, '2024-10-27 00:00:00.000000');
INSERT INTO `doctors` (`DoctorID`, `UserID`, `DoctorCode`, `SpecializationID`, `Qualification`, `LicenseNumber`, `YearsOfExperience`, `ConsultationFee`, `IsAvailable`, `JoiningDate`) VALUES (9, 20, 'DOCSD007', 17, 'MD, Specialty Board Certified', 'PRCSD007', 12, 1725.00, 1, '2024-10-14 00:00:00.000000');
INSERT INTO `doctors` (`DoctorID`, `UserID`, `DoctorCode`, `SpecializationID`, `Qualification`, `LicenseNumber`, `YearsOfExperience`, `ConsultationFee`, `IsAvailable`, `JoiningDate`) VALUES (10, 21, 'DOCSD008', 18, 'MD, Specialty Board Certified', 'PRCSD008', 13, 1800.00, 1, '2024-10-01 00:00:00.000000');
INSERT INTO `doctors` (`DoctorID`, `UserID`, `DoctorCode`, `SpecializationID`, `Qualification`, `LicenseNumber`, `YearsOfExperience`, `ConsultationFee`, `IsAvailable`, `JoiningDate`) VALUES (11, 22, 'DOCSD009', 19, 'MD, Specialty Board Certified', 'PRCSD009', 14, 1875.00, 1, '2024-09-18 00:00:00.000000');
INSERT INTO `doctors` (`DoctorID`, `UserID`, `DoctorCode`, `SpecializationID`, `Qualification`, `LicenseNumber`, `YearsOfExperience`, `ConsultationFee`, `IsAvailable`, `JoiningDate`) VALUES (12, 23, 'DOCSD010', 20, 'MD, Specialty Board Certified', 'PRCSD010', 15, 1950.00, 1, '2024-09-05 00:00:00.000000');
INSERT INTO `doctors` (`DoctorID`, `UserID`, `DoctorCode`, `SpecializationID`, `Qualification`, `LicenseNumber`, `YearsOfExperience`, `ConsultationFee`, `IsAvailable`, `JoiningDate`) VALUES (13, 24, 'DOCSD011', 21, 'MD, Specialty Board Certified', 'PRCSD011', 16, 2025.00, 1, '2024-08-23 00:00:00.000000');
INSERT INTO `doctors` (`DoctorID`, `UserID`, `DoctorCode`, `SpecializationID`, `Qualification`, `LicenseNumber`, `YearsOfExperience`, `ConsultationFee`, `IsAvailable`, `JoiningDate`) VALUES (14, 25, 'DOCSD012', 22, 'MD, Specialty Board Certified', 'PRCSD012', 17, 2100.00, 1, '2024-08-10 00:00:00.000000');
INSERT INTO `doctors` (`DoctorID`, `UserID`, `DoctorCode`, `SpecializationID`, `Qualification`, `LicenseNumber`, `YearsOfExperience`, `ConsultationFee`, `IsAvailable`, `JoiningDate`) VALUES (15, 26, 'DOCSD013', 23, 'MD, Specialty Board Certified', 'PRCSD013', 18, 2175.00, 1, '2024-07-28 00:00:00.000000');
INSERT INTO `doctors` (`DoctorID`, `UserID`, `DoctorCode`, `SpecializationID`, `Qualification`, `LicenseNumber`, `YearsOfExperience`, `ConsultationFee`, `IsAvailable`, `JoiningDate`) VALUES (16, 27, 'DOCSD014', 24, 'MD, Specialty Board Certified', 'PRCSD014', 19, 2250.00, 1, '2024-07-15 00:00:00.000000');
INSERT INTO `doctors` (`DoctorID`, `UserID`, `DoctorCode`, `SpecializationID`, `Qualification`, `LicenseNumber`, `YearsOfExperience`, `ConsultationFee`, `IsAvailable`, `JoiningDate`) VALUES (17, 28, 'DOCSD015', 25, 'MD, Specialty Board Certified', 'PRCSD015', 20, 2325.00, 1, '2024-07-02 00:00:00.000000');
INSERT INTO `doctors` (`DoctorID`, `UserID`, `DoctorCode`, `SpecializationID`, `Qualification`, `LicenseNumber`, `YearsOfExperience`, `ConsultationFee`, `IsAvailable`, `JoiningDate`) VALUES (18, 29, 'DOCSD016', 26, 'MD, Specialty Board Certified', 'PRCSD016', 21, 2400.00, 1, '2024-06-19 00:00:00.000000');
INSERT INTO `doctors` (`DoctorID`, `UserID`, `DoctorCode`, `SpecializationID`, `Qualification`, `LicenseNumber`, `YearsOfExperience`, `ConsultationFee`, `IsAvailable`, `JoiningDate`) VALUES (19, 30, 'DOCSD017', 27, 'MD, Specialty Board Certified', 'PRCSD017', 22, 2475.00, 1, '2024-06-06 00:00:00.000000');
INSERT INTO `doctors` (`DoctorID`, `UserID`, `DoctorCode`, `SpecializationID`, `Qualification`, `LicenseNumber`, `YearsOfExperience`, `ConsultationFee`, `IsAvailable`, `JoiningDate`) VALUES (20, 31, 'DOCSD018', 28, 'MD, Specialty Board Certified', 'PRCSD018', 23, 2550.00, 1, '2024-05-24 00:00:00.000000');
INSERT INTO `doctors` (`DoctorID`, `UserID`, `DoctorCode`, `SpecializationID`, `Qualification`, `LicenseNumber`, `YearsOfExperience`, `ConsultationFee`, `IsAvailable`, `JoiningDate`) VALUES (21, 32, 'DOCSD019', 29, 'MD, Specialty Board Certified', 'PRCSD019', 24, 2625.00, 1, '2024-05-11 00:00:00.000000');
INSERT INTO `doctors` (`DoctorID`, `UserID`, `DoctorCode`, `SpecializationID`, `Qualification`, `LicenseNumber`, `YearsOfExperience`, `ConsultationFee`, `IsAvailable`, `JoiningDate`) VALUES (22, 33, 'DOCSD020', 30, 'MD, Specialty Board Certified', 'PRCSD020', 25, 2700.00, 1, '2024-04-28 00:00:00.000000');
INSERT INTO `doctors` (`DoctorID`, `UserID`, `DoctorCode`, `SpecializationID`, `Qualification`, `LicenseNumber`, `YearsOfExperience`, `ConsultationFee`, `IsAvailable`, `JoiningDate`) VALUES (23, 34, 'DOCSD021', 31, 'MD, Specialty Board Certified', 'PRCSD021', 26, 2775.00, 1, '2024-04-15 00:00:00.000000');
INSERT INTO `doctors` (`DoctorID`, `UserID`, `DoctorCode`, `SpecializationID`, `Qualification`, `LicenseNumber`, `YearsOfExperience`, `ConsultationFee`, `IsAvailable`, `JoiningDate`) VALUES (24, 35, 'DOCSD022', 32, 'MD, Specialty Board Certified', 'PRCSD022', 27, 2850.00, 1, '2024-04-02 00:00:00.000000');
INSERT INTO `doctors` (`DoctorID`, `UserID`, `DoctorCode`, `SpecializationID`, `Qualification`, `LicenseNumber`, `YearsOfExperience`, `ConsultationFee`, `IsAvailable`, `JoiningDate`) VALUES (25, 36, 'DOCSD023', 33, 'MD, Specialty Board Certified', 'PRCSD023', 28, 2925.00, 1, '2024-03-20 00:00:00.000000');
INSERT INTO `doctors` (`DoctorID`, `UserID`, `DoctorCode`, `SpecializationID`, `Qualification`, `LicenseNumber`, `YearsOfExperience`, `ConsultationFee`, `IsAvailable`, `JoiningDate`) VALUES (26, 37, 'DOCSD024', 34, 'MD, Specialty Board Certified', 'PRCSD024', 29, 3000.00, 1, '2024-03-07 00:00:00.000000');
INSERT INTO `doctors` (`DoctorID`, `UserID`, `DoctorCode`, `SpecializationID`, `Qualification`, `LicenseNumber`, `YearsOfExperience`, `ConsultationFee`, `IsAvailable`, `JoiningDate`) VALUES (27, 38, 'DOCSD025', 35, 'MD, Specialty Board Certified', 'PRCSD025', 30, 3075.00, 1, '2024-02-23 00:00:00.000000');
INSERT INTO `doctors` (`DoctorID`, `UserID`, `DoctorCode`, `SpecializationID`, `Qualification`, `LicenseNumber`, `YearsOfExperience`, `ConsultationFee`, `IsAvailable`, `JoiningDate`) VALUES (28, 39, 'DOCSD026', 36, 'MD, Specialty Board Certified', 'PRCSD026', 31, 3150.00, 1, '2024-02-10 00:00:00.000000');
INSERT INTO `doctors` (`DoctorID`, `UserID`, `DoctorCode`, `SpecializationID`, `Qualification`, `LicenseNumber`, `YearsOfExperience`, `ConsultationFee`, `IsAvailable`, `JoiningDate`) VALUES (29, 40, 'DOCSD027', 37, 'MD, Specialty Board Certified', 'PRCSD027', 32, 3225.00, 1, '2024-01-28 00:00:00.000000');
INSERT INTO `doctors` (`DoctorID`, `UserID`, `DoctorCode`, `SpecializationID`, `Qualification`, `LicenseNumber`, `YearsOfExperience`, `ConsultationFee`, `IsAvailable`, `JoiningDate`) VALUES (30, 41, 'DOCSD028', 38, 'MD, Specialty Board Certified', 'PRCSD028', 33, 3300.00, 1, '2024-01-15 00:00:00.000000');
INSERT INTO `doctors` (`DoctorID`, `UserID`, `DoctorCode`, `SpecializationID`, `Qualification`, `LicenseNumber`, `YearsOfExperience`, `ConsultationFee`, `IsAvailable`, `JoiningDate`) VALUES (31, 42, 'DOCSD029', 39, 'MD, Specialty Board Certified', 'PRCSD029', 34, 3375.00, 1, '2024-01-02 00:00:00.000000');
INSERT INTO `doctors` (`DoctorID`, `UserID`, `DoctorCode`, `SpecializationID`, `Qualification`, `LicenseNumber`, `YearsOfExperience`, `ConsultationFee`, `IsAvailable`, `JoiningDate`) VALUES (32, 43, 'DOCSD030', 40, 'MD, Specialty Board Certified', 'PRCSD030', 35, 3450.00, 1, '2023-12-20 00:00:00.000000');

-- Data for doctorschedules
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (7, 3, 1, '08:00:00', '17:00:00', 19, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (8, 3, 2, '08:00:00', '16:00:00', 19, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (9, 3, 3, '08:00:00', '17:00:00', 19, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (10, 3, 4, '08:00:00', '16:00:00', 19, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (11, 3, 5, '08:00:00', '17:00:00', 19, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (12, 4, 1, '08:00:00', '17:00:00', 20, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (13, 4, 2, '08:00:00', '16:00:00', 20, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (14, 4, 3, '08:00:00', '17:00:00', 20, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (15, 4, 4, '08:00:00', '16:00:00', 20, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (16, 4, 5, '08:00:00', '17:00:00', 20, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (17, 5, 1, '08:00:00', '17:00:00', 21, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (18, 5, 2, '08:00:00', '16:00:00', 21, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (19, 5, 3, '08:00:00', '17:00:00', 21, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (20, 5, 4, '08:00:00', '16:00:00', 21, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (21, 5, 5, '08:00:00', '17:00:00', 21, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (22, 6, 1, '08:00:00', '17:00:00', 22, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (23, 6, 2, '08:00:00', '16:00:00', 22, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (24, 6, 3, '08:00:00', '17:00:00', 22, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (25, 6, 4, '08:00:00', '16:00:00', 22, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (26, 6, 5, '08:00:00', '17:00:00', 22, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (27, 7, 1, '08:00:00', '17:00:00', 18, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (28, 7, 2, '08:00:00', '16:00:00', 18, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (29, 7, 3, '08:00:00', '17:00:00', 18, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (30, 7, 4, '08:00:00', '16:00:00', 18, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (31, 7, 5, '08:00:00', '17:00:00', 18, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (32, 8, 1, '08:00:00', '17:00:00', 19, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (33, 8, 2, '08:00:00', '16:00:00', 19, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (34, 8, 3, '08:00:00', '17:00:00', 19, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (35, 8, 4, '08:00:00', '16:00:00', 19, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (36, 8, 5, '08:00:00', '17:00:00', 19, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (37, 9, 1, '08:00:00', '17:00:00', 20, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (38, 9, 2, '08:00:00', '16:00:00', 20, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (39, 9, 3, '08:00:00', '17:00:00', 20, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (40, 9, 4, '08:00:00', '16:00:00', 20, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (41, 9, 5, '08:00:00', '17:00:00', 20, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (42, 10, 1, '08:00:00', '17:00:00', 21, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (43, 10, 2, '08:00:00', '16:00:00', 21, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (44, 10, 3, '08:00:00', '17:00:00', 21, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (45, 10, 4, '08:00:00', '16:00:00', 21, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (46, 10, 5, '08:00:00', '17:00:00', 21, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (47, 11, 1, '08:00:00', '17:00:00', 22, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (48, 11, 2, '08:00:00', '16:00:00', 22, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (49, 11, 3, '08:00:00', '17:00:00', 22, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (50, 11, 4, '08:00:00', '16:00:00', 22, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (51, 11, 5, '08:00:00', '17:00:00', 22, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (52, 12, 1, '08:00:00', '17:00:00', 18, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (53, 12, 2, '08:00:00', '16:00:00', 18, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (54, 12, 3, '08:00:00', '17:00:00', 18, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (55, 12, 4, '08:00:00', '16:00:00', 18, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (56, 12, 5, '08:00:00', '17:00:00', 18, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (57, 13, 1, '08:00:00', '17:00:00', 19, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (58, 13, 2, '08:00:00', '16:00:00', 19, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (59, 13, 3, '08:00:00', '17:00:00', 19, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (60, 13, 4, '08:00:00', '16:00:00', 19, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (61, 13, 5, '08:00:00', '17:00:00', 19, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (62, 14, 1, '08:00:00', '17:00:00', 20, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (63, 14, 2, '08:00:00', '16:00:00', 20, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (64, 14, 3, '08:00:00', '17:00:00', 20, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (65, 14, 4, '08:00:00', '16:00:00', 20, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (66, 14, 5, '08:00:00', '17:00:00', 20, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (67, 15, 1, '08:00:00', '17:00:00', 21, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (68, 15, 2, '08:00:00', '16:00:00', 21, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (69, 15, 3, '08:00:00', '17:00:00', 21, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (70, 15, 4, '08:00:00', '16:00:00', 21, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (71, 15, 5, '08:00:00', '17:00:00', 21, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (72, 16, 1, '08:00:00', '17:00:00', 22, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (73, 16, 2, '08:00:00', '16:00:00', 22, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (74, 16, 3, '08:00:00', '17:00:00', 22, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (75, 16, 4, '08:00:00', '16:00:00', 22, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (76, 16, 5, '08:00:00', '17:00:00', 22, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (77, 17, 1, '08:00:00', '17:00:00', 18, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (78, 17, 2, '08:00:00', '16:00:00', 18, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (79, 17, 3, '08:00:00', '17:00:00', 18, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (80, 17, 4, '08:00:00', '16:00:00', 18, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (81, 17, 5, '08:00:00', '17:00:00', 18, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (82, 18, 1, '08:00:00', '17:00:00', 19, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (83, 18, 2, '08:00:00', '16:00:00', 19, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (84, 18, 3, '08:00:00', '17:00:00', 19, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (85, 18, 4, '08:00:00', '16:00:00', 19, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (86, 18, 5, '08:00:00', '17:00:00', 19, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (87, 19, 1, '08:00:00', '17:00:00', 20, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (88, 19, 2, '08:00:00', '16:00:00', 20, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (89, 19, 3, '08:00:00', '17:00:00', 20, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (90, 19, 4, '08:00:00', '16:00:00', 20, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (91, 19, 5, '08:00:00', '17:00:00', 20, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (92, 20, 1, '08:00:00', '17:00:00', 21, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (93, 20, 2, '08:00:00', '16:00:00', 21, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (94, 20, 3, '08:00:00', '17:00:00', 21, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (95, 20, 4, '08:00:00', '16:00:00', 21, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (96, 20, 5, '08:00:00', '17:00:00', 21, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (97, 21, 1, '08:00:00', '17:00:00', 22, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (98, 21, 2, '08:00:00', '16:00:00', 22, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (99, 21, 3, '08:00:00', '17:00:00', 22, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (100, 21, 4, '08:00:00', '16:00:00', 22, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (101, 21, 5, '08:00:00', '17:00:00', 22, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (102, 22, 1, '08:00:00', '17:00:00', 18, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (103, 22, 2, '08:00:00', '16:00:00', 18, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (104, 22, 3, '08:00:00', '17:00:00', 18, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (105, 22, 4, '08:00:00', '16:00:00', 18, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (106, 22, 5, '08:00:00', '17:00:00', 18, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (107, 23, 1, '08:00:00', '17:00:00', 19, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (108, 23, 2, '08:00:00', '16:00:00', 19, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (109, 23, 3, '08:00:00', '17:00:00', 19, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (110, 23, 4, '08:00:00', '16:00:00', 19, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (111, 23, 5, '08:00:00', '17:00:00', 19, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (112, 24, 1, '08:00:00', '17:00:00', 20, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (113, 24, 2, '08:00:00', '16:00:00', 20, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (114, 24, 3, '08:00:00', '17:00:00', 20, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (115, 24, 4, '08:00:00', '16:00:00', 20, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (116, 24, 5, '08:00:00', '17:00:00', 20, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (117, 25, 1, '08:00:00', '17:00:00', 21, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (118, 25, 2, '08:00:00', '16:00:00', 21, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (119, 25, 3, '08:00:00', '17:00:00', 21, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (120, 25, 4, '08:00:00', '16:00:00', 21, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (121, 25, 5, '08:00:00', '17:00:00', 21, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (122, 26, 1, '08:00:00', '17:00:00', 22, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (123, 26, 2, '08:00:00', '16:00:00', 22, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (124, 26, 3, '08:00:00', '17:00:00', 22, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (125, 26, 4, '08:00:00', '16:00:00', 22, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (126, 26, 5, '08:00:00', '17:00:00', 22, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (127, 27, 1, '08:00:00', '17:00:00', 18, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (128, 27, 2, '08:00:00', '16:00:00', 18, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (129, 27, 3, '08:00:00', '17:00:00', 18, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (130, 27, 4, '08:00:00', '16:00:00', 18, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (131, 27, 5, '08:00:00', '17:00:00', 18, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (132, 28, 1, '08:00:00', '17:00:00', 19, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (133, 28, 2, '08:00:00', '16:00:00', 19, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (134, 28, 3, '08:00:00', '17:00:00', 19, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (135, 28, 4, '08:00:00', '16:00:00', 19, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (136, 28, 5, '08:00:00', '17:00:00', 19, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (137, 29, 1, '08:00:00', '17:00:00', 20, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (138, 29, 2, '08:00:00', '16:00:00', 20, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (139, 29, 3, '08:00:00', '17:00:00', 20, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (140, 29, 4, '08:00:00', '16:00:00', 20, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (141, 29, 5, '08:00:00', '17:00:00', 20, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (142, 30, 1, '08:00:00', '17:00:00', 21, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (143, 30, 2, '08:00:00', '16:00:00', 21, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (144, 30, 3, '08:00:00', '17:00:00', 21, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (145, 30, 4, '08:00:00', '16:00:00', 21, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (146, 30, 5, '08:00:00', '17:00:00', 21, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (147, 31, 1, '08:00:00', '17:00:00', 22, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (148, 31, 2, '08:00:00', '16:00:00', 22, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (149, 31, 3, '08:00:00', '17:00:00', 22, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (150, 31, 4, '08:00:00', '16:00:00', 22, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (151, 31, 5, '08:00:00', '17:00:00', 22, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (152, 32, 1, '08:00:00', '17:00:00', 18, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (153, 32, 2, '08:00:00', '16:00:00', 18, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (154, 32, 3, '08:00:00', '17:00:00', 18, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (155, 32, 4, '08:00:00', '16:00:00', 18, 1);
INSERT INTO `doctorschedules` (`ScheduleID`, `DoctorID`, `DayOfWeek`, `StartTime`, `EndTime`, `MaxAppointments`, `IsActive`) VALUES (156, 32, 5, '08:00:00', '17:00:00', 18, 1);

-- Data for inventory
INSERT INTO `inventory` (`InventoryID`, `MedicineID`, `BatchNumber`, `ExpiryDate`, `Quantity`, `PurchasePrice`, `SellingPrice`, `Supplier`, `PurchaseDate`, `Location`) VALUES (10, 10, 'BATCH-SD-001', '2026-09-17 00:00:00.000000', 73, 16.50, 33.00, 'Supplier 1', '2026-01-22 00:00:00.000000', 'Rack-01');
INSERT INTO `inventory` (`InventoryID`, `MedicineID`, `BatchNumber`, `ExpiryDate`, `Quantity`, `PurchasePrice`, `SellingPrice`, `Supplier`, `PurchaseDate`, `Location`) VALUES (11, 11, 'BATCH-SD-002', '2026-09-29 00:00:00.000000', 76, 18.00, 36.00, 'Supplier 2', '2026-01-21 00:00:00.000000', 'Rack-02');
INSERT INTO `inventory` (`InventoryID`, `MedicineID`, `BatchNumber`, `ExpiryDate`, `Quantity`, `PurchasePrice`, `SellingPrice`, `Supplier`, `PurchaseDate`, `Location`) VALUES (12, 12, 'BATCH-SD-003', '2026-10-11 00:00:00.000000', 79, 19.50, 39.00, 'Supplier 3', '2026-01-20 00:00:00.000000', 'Rack-03');
INSERT INTO `inventory` (`InventoryID`, `MedicineID`, `BatchNumber`, `ExpiryDate`, `Quantity`, `PurchasePrice`, `SellingPrice`, `Supplier`, `PurchaseDate`, `Location`) VALUES (13, 13, 'BATCH-SD-004', '2026-10-23 00:00:00.000000', 82, 21.00, 42.00, 'Supplier 4', '2026-01-19 00:00:00.000000', 'Rack-04');
INSERT INTO `inventory` (`InventoryID`, `MedicineID`, `BatchNumber`, `ExpiryDate`, `Quantity`, `PurchasePrice`, `SellingPrice`, `Supplier`, `PurchaseDate`, `Location`) VALUES (14, 14, 'BATCH-SD-005', '2026-11-04 00:00:00.000000', 85, 22.50, 45.00, 'Supplier 5', '2026-01-18 00:00:00.000000', 'Rack-05');
INSERT INTO `inventory` (`InventoryID`, `MedicineID`, `BatchNumber`, `ExpiryDate`, `Quantity`, `PurchasePrice`, `SellingPrice`, `Supplier`, `PurchaseDate`, `Location`) VALUES (15, 15, 'BATCH-SD-006', '2026-11-16 00:00:00.000000', 88, 24.00, 48.00, 'Supplier 6', '2026-01-17 00:00:00.000000', 'Rack-06');
INSERT INTO `inventory` (`InventoryID`, `MedicineID`, `BatchNumber`, `ExpiryDate`, `Quantity`, `PurchasePrice`, `SellingPrice`, `Supplier`, `PurchaseDate`, `Location`) VALUES (16, 16, 'BATCH-SD-007', '2026-11-28 00:00:00.000000', 91, 25.50, 51.00, 'Supplier 7', '2026-01-16 00:00:00.000000', 'Rack-07');
INSERT INTO `inventory` (`InventoryID`, `MedicineID`, `BatchNumber`, `ExpiryDate`, `Quantity`, `PurchasePrice`, `SellingPrice`, `Supplier`, `PurchaseDate`, `Location`) VALUES (17, 17, 'BATCH-SD-008', '2026-12-10 00:00:00.000000', 94, 27.00, 54.00, 'Supplier 8', '2026-01-15 00:00:00.000000', 'Rack-08');
INSERT INTO `inventory` (`InventoryID`, `MedicineID`, `BatchNumber`, `ExpiryDate`, `Quantity`, `PurchasePrice`, `SellingPrice`, `Supplier`, `PurchaseDate`, `Location`) VALUES (18, 18, 'BATCH-SD-009', '2026-12-22 00:00:00.000000', 97, 28.50, 57.00, 'Supplier 1', '2026-01-14 00:00:00.000000', 'Rack-09');
INSERT INTO `inventory` (`InventoryID`, `MedicineID`, `BatchNumber`, `ExpiryDate`, `Quantity`, `PurchasePrice`, `SellingPrice`, `Supplier`, `PurchaseDate`, `Location`) VALUES (19, 19, 'BATCH-SD-010', '2027-01-03 00:00:00.000000', 100, 30.00, 60.00, 'Supplier 2', '2026-01-13 00:00:00.000000', 'Rack-10');
INSERT INTO `inventory` (`InventoryID`, `MedicineID`, `BatchNumber`, `ExpiryDate`, `Quantity`, `PurchasePrice`, `SellingPrice`, `Supplier`, `PurchaseDate`, `Location`) VALUES (20, 20, 'BATCH-SD-011', '2027-01-15 00:00:00.000000', 103, 31.50, 63.00, 'Supplier 3', '2026-01-12 00:00:00.000000', 'Rack-01');
INSERT INTO `inventory` (`InventoryID`, `MedicineID`, `BatchNumber`, `ExpiryDate`, `Quantity`, `PurchasePrice`, `SellingPrice`, `Supplier`, `PurchaseDate`, `Location`) VALUES (21, 21, 'BATCH-SD-012', '2027-01-27 00:00:00.000000', 106, 33.00, 66.00, 'Supplier 4', '2026-01-11 00:00:00.000000', 'Rack-02');
INSERT INTO `inventory` (`InventoryID`, `MedicineID`, `BatchNumber`, `ExpiryDate`, `Quantity`, `PurchasePrice`, `SellingPrice`, `Supplier`, `PurchaseDate`, `Location`) VALUES (22, 22, 'BATCH-SD-013', '2027-02-08 00:00:00.000000', 109, 34.50, 69.00, 'Supplier 5', '2026-01-10 00:00:00.000000', 'Rack-03');
INSERT INTO `inventory` (`InventoryID`, `MedicineID`, `BatchNumber`, `ExpiryDate`, `Quantity`, `PurchasePrice`, `SellingPrice`, `Supplier`, `PurchaseDate`, `Location`) VALUES (23, 23, 'BATCH-SD-014', '2027-02-20 00:00:00.000000', 112, 36.00, 72.00, 'Supplier 6', '2026-01-09 00:00:00.000000', 'Rack-04');
INSERT INTO `inventory` (`InventoryID`, `MedicineID`, `BatchNumber`, `ExpiryDate`, `Quantity`, `PurchasePrice`, `SellingPrice`, `Supplier`, `PurchaseDate`, `Location`) VALUES (24, 24, 'BATCH-SD-015', '2027-03-04 00:00:00.000000', 115, 37.50, 75.00, 'Supplier 7', '2026-01-08 00:00:00.000000', 'Rack-05');
INSERT INTO `inventory` (`InventoryID`, `MedicineID`, `BatchNumber`, `ExpiryDate`, `Quantity`, `PurchasePrice`, `SellingPrice`, `Supplier`, `PurchaseDate`, `Location`) VALUES (25, 25, 'BATCH-SD-016', '2027-03-16 00:00:00.000000', 118, 39.00, 78.00, 'Supplier 8', '2026-01-07 00:00:00.000000', 'Rack-06');
INSERT INTO `inventory` (`InventoryID`, `MedicineID`, `BatchNumber`, `ExpiryDate`, `Quantity`, `PurchasePrice`, `SellingPrice`, `Supplier`, `PurchaseDate`, `Location`) VALUES (26, 26, 'BATCH-SD-017', '2027-03-28 00:00:00.000000', 121, 40.50, 81.00, 'Supplier 1', '2026-01-06 00:00:00.000000', 'Rack-07');
INSERT INTO `inventory` (`InventoryID`, `MedicineID`, `BatchNumber`, `ExpiryDate`, `Quantity`, `PurchasePrice`, `SellingPrice`, `Supplier`, `PurchaseDate`, `Location`) VALUES (27, 27, 'BATCH-SD-018', '2027-04-09 00:00:00.000000', 124, 42.00, 84.00, 'Supplier 2', '2026-01-05 00:00:00.000000', 'Rack-08');
INSERT INTO `inventory` (`InventoryID`, `MedicineID`, `BatchNumber`, `ExpiryDate`, `Quantity`, `PurchasePrice`, `SellingPrice`, `Supplier`, `PurchaseDate`, `Location`) VALUES (28, 28, 'BATCH-SD-019', '2027-04-21 00:00:00.000000', 127, 43.50, 87.00, 'Supplier 3', '2026-01-04 00:00:00.000000', 'Rack-09');
INSERT INTO `inventory` (`InventoryID`, `MedicineID`, `BatchNumber`, `ExpiryDate`, `Quantity`, `PurchasePrice`, `SellingPrice`, `Supplier`, `PurchaseDate`, `Location`) VALUES (29, 29, 'BATCH-SD-020', '2027-05-03 00:00:00.000000', 130, 45.00, 90.00, 'Supplier 4', '2026-01-03 00:00:00.000000', 'Rack-10');
INSERT INTO `inventory` (`InventoryID`, `MedicineID`, `BatchNumber`, `ExpiryDate`, `Quantity`, `PurchasePrice`, `SellingPrice`, `Supplier`, `PurchaseDate`, `Location`) VALUES (30, 30, 'BATCH-SD-021', '2027-05-15 00:00:00.000000', 133, 46.50, 93.00, 'Supplier 5', '2026-01-02 00:00:00.000000', 'Rack-01');
INSERT INTO `inventory` (`InventoryID`, `MedicineID`, `BatchNumber`, `ExpiryDate`, `Quantity`, `PurchasePrice`, `SellingPrice`, `Supplier`, `PurchaseDate`, `Location`) VALUES (31, 31, 'BATCH-SD-022', '2027-05-27 00:00:00.000000', 136, 48.00, 96.00, 'Supplier 6', '2026-01-01 00:00:00.000000', 'Rack-02');
INSERT INTO `inventory` (`InventoryID`, `MedicineID`, `BatchNumber`, `ExpiryDate`, `Quantity`, `PurchasePrice`, `SellingPrice`, `Supplier`, `PurchaseDate`, `Location`) VALUES (32, 32, 'BATCH-SD-023', '2027-06-08 00:00:00.000000', 139, 49.50, 99.00, 'Supplier 7', '2025-12-31 00:00:00.000000', 'Rack-03');
INSERT INTO `inventory` (`InventoryID`, `MedicineID`, `BatchNumber`, `ExpiryDate`, `Quantity`, `PurchasePrice`, `SellingPrice`, `Supplier`, `PurchaseDate`, `Location`) VALUES (33, 33, 'BATCH-SD-024', '2027-06-20 00:00:00.000000', 142, 51.00, 102.00, 'Supplier 8', '2025-12-30 00:00:00.000000', 'Rack-04');
INSERT INTO `inventory` (`InventoryID`, `MedicineID`, `BatchNumber`, `ExpiryDate`, `Quantity`, `PurchasePrice`, `SellingPrice`, `Supplier`, `PurchaseDate`, `Location`) VALUES (34, 34, 'BATCH-SD-025', '2027-07-02 00:00:00.000000', 145, 52.50, 105.00, 'Supplier 1', '2025-12-29 00:00:00.000000', 'Rack-05');
INSERT INTO `inventory` (`InventoryID`, `MedicineID`, `BatchNumber`, `ExpiryDate`, `Quantity`, `PurchasePrice`, `SellingPrice`, `Supplier`, `PurchaseDate`, `Location`) VALUES (35, 35, 'BATCH-SD-026', '2027-07-14 00:00:00.000000', 148, 54.00, 108.00, 'Supplier 2', '2025-12-28 00:00:00.000000', 'Rack-06');
INSERT INTO `inventory` (`InventoryID`, `MedicineID`, `BatchNumber`, `ExpiryDate`, `Quantity`, `PurchasePrice`, `SellingPrice`, `Supplier`, `PurchaseDate`, `Location`) VALUES (36, 36, 'BATCH-SD-027', '2027-07-26 00:00:00.000000', 151, 55.50, 111.00, 'Supplier 3', '2025-12-27 00:00:00.000000', 'Rack-07');
INSERT INTO `inventory` (`InventoryID`, `MedicineID`, `BatchNumber`, `ExpiryDate`, `Quantity`, `PurchasePrice`, `SellingPrice`, `Supplier`, `PurchaseDate`, `Location`) VALUES (37, 37, 'BATCH-SD-028', '2027-08-07 00:00:00.000000', 154, 57.00, 114.00, 'Supplier 4', '2025-12-26 00:00:00.000000', 'Rack-08');
INSERT INTO `inventory` (`InventoryID`, `MedicineID`, `BatchNumber`, `ExpiryDate`, `Quantity`, `PurchasePrice`, `SellingPrice`, `Supplier`, `PurchaseDate`, `Location`) VALUES (38, 38, 'BATCH-SD-029', '2027-08-19 00:00:00.000000', 157, 58.50, 117.00, 'Supplier 5', '2025-12-25 00:00:00.000000', 'Rack-09');
INSERT INTO `inventory` (`InventoryID`, `MedicineID`, `BatchNumber`, `ExpiryDate`, `Quantity`, `PurchasePrice`, `SellingPrice`, `Supplier`, `PurchaseDate`, `Location`) VALUES (39, 39, 'BATCH-SD-030', '2027-08-31 00:00:00.000000', 160, 60.00, 120.00, 'Supplier 6', '2025-12-24 00:00:00.000000', 'Rack-10');

-- Data for invoicedetails
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (2, 2, 10, 2, 790.00, 1580.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (3, 3, 11, 3, 830.00, 2490.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (4, 4, 12, 1, 870.00, 870.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (5, 5, 13, 2, 910.00, 1820.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (6, 6, 14, 3, 950.00, 2850.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (7, 7, 15, 1, 990.00, 990.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (8, 8, 16, 2, 1030.00, 2060.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (9, 9, 17, 3, 1070.00, 3210.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (10, 10, 18, 1, 1110.00, 1110.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (11, 11, 19, 2, 1150.00, 2300.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (12, 12, 20, 3, 1190.00, 3570.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (13, 13, 21, 1, 1230.00, 1230.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (14, 14, 22, 2, 1270.00, 2540.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (15, 15, 23, 3, 1310.00, 3930.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (16, 16, 24, 1, 1350.00, 1350.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (17, 17, 25, 2, 1390.00, 2780.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (18, 18, 26, 3, 1430.00, 4290.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (19, 19, 27, 1, 1470.00, 1470.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (20, 20, 28, 2, 1510.00, 3020.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (21, 21, 29, 3, 1550.00, 4650.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (22, 22, 30, 1, 1590.00, 1590.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (23, 23, 31, 2, 1630.00, 3260.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (24, 24, 32, 3, 1670.00, 5010.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (25, 25, 33, 1, 1710.00, 1710.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (26, 26, 34, 2, 1750.00, 3500.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (27, 27, 35, 3, 1790.00, 5370.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (28, 28, 36, 1, 1830.00, 1830.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (29, 29, 37, 2, 1870.00, 3740.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (30, 30, 38, 3, 1910.00, 5730.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (31, 31, 39, 1, 1950.00, 1950.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (32, 32, 10, 2, 1990.00, 3980.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (33, 33, 11, 3, 2030.00, 6090.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (34, 34, 12, 1, 2070.00, 2070.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (35, 35, 13, 2, 2110.00, 4220.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (36, 36, 14, 3, 2150.00, 6450.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (37, 37, 15, 1, 2190.00, 2190.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (38, 38, 16, 2, 2230.00, 4460.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (39, 39, 17, 3, 2270.00, 6810.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (40, 40, 18, 1, 2310.00, 2310.00);
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (41, 41, 19, 2, 2350.00, 4700.00);

-- Data for invoices
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (2, 'INVSD001', 2, 2, '2026-02-08 16:00:00.000000', '2026-02-19 16:00:00.000000', 1910.00, 0.00, 229.20, 2139.20, 'Paid', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (3, 'INVSD002', 3, 3, '2026-02-09 16:00:00.000000', '2026-02-21 16:00:00.000000', 2020.00, 0.00, 242.40, 2262.40, 'Paid', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (4, 'INVSD003', 4, 4, '2026-02-10 16:00:00.000000', '2026-02-23 16:00:00.000000', 2130.00, 0.00, 255.60, 2385.60, 'Paid', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (5, 'INVSD004', 5, 5, '2026-02-11 16:00:00.000000', '2026-02-25 16:00:00.000000', 2240.00, 0.00, 268.80, 2508.80, 'Paid', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (6, 'INVSD005', 6, 6, '2026-02-12 16:00:00.000000', '2026-02-27 16:00:00.000000', 2350.00, 0.00, 282.00, 2632.00, 'Paid', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (7, 'INVSD006', 7, 7, '2026-02-13 16:00:00.000000', '2026-02-23 16:00:00.000000', 2460.00, 180.00, 273.60, 2553.60, 'Paid', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (8, 'INVSD007', 8, 8, '2026-02-14 16:00:00.000000', '2026-02-25 16:00:00.000000', 2570.00, 0.00, 308.40, 2878.40, 'Paid', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (9, 'INVSD008', 9, 9, '2026-02-15 16:00:00.000000', '2026-02-27 16:00:00.000000', 2680.00, 0.00, 321.60, 3001.60, 'Paid', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (10, 'INVSD009', 10, 10, '2026-02-16 16:00:00.000000', '2026-03-01 16:00:00.000000', 2790.00, 0.00, 334.80, 3124.80, 'Paid', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (11, 'INVSD010', 11, 11, '2026-02-17 16:00:00.000000', '2026-03-03 16:00:00.000000', 2900.00, 0.00, 348.00, 3248.00, 'Paid', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (12, 'INVSD011', 12, 12, '2026-02-18 16:00:00.000000', '2026-03-05 16:00:00.000000', 3010.00, 0.00, 361.20, 3371.20, 'Paid', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (13, 'INVSD012', 13, 13, '2026-02-19 16:00:00.000000', '2026-03-01 16:00:00.000000', 3120.00, 180.00, 352.80, 3292.80, 'Paid', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (14, 'INVSD013', 14, 14, '2026-02-20 16:00:00.000000', '2026-03-03 16:00:00.000000', 3230.00, 0.00, 387.60, 3617.60, 'Paid', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (15, 'INVSD014', 15, 15, '2026-02-21 16:00:00.000000', '2026-03-05 16:00:00.000000', 3340.00, 0.00, 400.80, 3740.80, 'Paid', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (16, 'INVSD015', 16, 16, '2026-02-22 16:00:00.000000', '2026-03-07 16:00:00.000000', 3450.00, 0.00, 414.00, 3864.00, 'Paid', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (17, 'INVSD016', 17, 17, '2026-02-23 16:00:00.000000', '2026-03-09 16:00:00.000000', 3560.00, 0.00, 427.20, 3987.20, 'Partial', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (18, 'INVSD017', 18, 18, '2026-02-24 16:00:00.000000', '2026-03-11 16:00:00.000000', 3670.00, 0.00, 440.40, 4110.40, 'Partial', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (19, 'INVSD018', 19, 19, '2026-02-25 16:00:00.000000', '2026-03-07 16:00:00.000000', 3780.00, 180.00, 432.00, 4032.00, 'Partial', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (20, 'INVSD019', 20, 20, '2026-02-26 16:00:00.000000', '2026-03-09 16:00:00.000000', 3890.00, 0.00, 466.80, 4356.80, 'Partial', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (21, 'INVSD020', 21, 21, '2026-02-27 16:00:00.000000', '2026-03-11 16:00:00.000000', 4000.00, 0.00, 480.00, 4480.00, 'Partial', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (22, 'INVSD021', 22, 22, '2026-02-28 16:00:00.000000', '2026-03-13 16:00:00.000000', 4110.00, 0.00, 493.20, 4603.20, 'Partial', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (23, 'INVSD022', 23, 23, '2026-03-01 16:00:00.000000', '2026-03-15 16:00:00.000000', 4220.00, 0.00, 506.40, 4726.40, 'Partial', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (24, 'INVSD023', 24, 24, '2026-03-02 16:00:00.000000', '2026-03-17 16:00:00.000000', 4330.00, 0.00, 519.60, 4849.60, 'Partial', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (25, 'INVSD024', 25, 25, '2026-03-03 16:00:00.000000', '2026-03-13 16:00:00.000000', 4440.00, 180.00, 511.20, 4771.20, 'Partial', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (26, 'INVSD025', 26, 26, '2026-03-04 16:00:00.000000', '2026-03-15 16:00:00.000000', 4550.00, 0.00, 546.00, 5096.00, 'Partial', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (27, 'INVSD026', 27, 27, '2026-03-05 16:00:00.000000', '2026-03-17 16:00:00.000000', 4660.00, 0.00, 559.20, 5219.20, 'Partial', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (28, 'INVSD027', 28, 28, '2026-03-06 16:00:00.000000', '2026-03-19 16:00:00.000000', 4770.00, 0.00, 572.40, 5342.40, 'Partial', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (29, 'INVSD028', 29, 29, '2026-03-07 16:00:00.000000', '2026-03-21 16:00:00.000000', 4880.00, 0.00, 585.60, 5465.60, 'Partial', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (30, 'INVSD029', 30, 30, '2026-03-08 16:00:00.000000', '2026-03-23 16:00:00.000000', 4990.00, 0.00, 598.80, 5588.80, 'Partial', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (31, 'INVSD030', 31, 31, '2026-03-09 16:00:00.000000', '2026-03-19 16:00:00.000000', 5100.00, 180.00, 590.40, 5510.40, 'Partial', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (32, 'INVSD031', 2, 32, '2026-03-10 16:00:00.000000', '2026-03-21 16:00:00.000000', 5210.00, 0.00, 625.20, 5835.20, 'Pending', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (33, 'INVSD032', 3, 33, '2026-03-11 16:00:00.000000', '2026-03-23 16:00:00.000000', 5320.00, 0.00, 638.40, 5958.40, 'Pending', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (34, 'INVSD033', 4, 34, '2026-03-12 16:00:00.000000', '2026-03-25 16:00:00.000000', 5430.00, 0.00, 651.60, 6081.60, 'Pending', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (35, 'INVSD034', 5, 35, '2026-03-13 16:00:00.000000', '2026-03-27 16:00:00.000000', 5540.00, 0.00, 664.80, 6204.80, 'Pending', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (36, 'INVSD035', 6, 36, '2026-03-14 16:00:00.000000', '2026-03-29 16:00:00.000000', 5650.00, 0.00, 678.00, 6328.00, 'Pending', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (37, 'INVSD036', 7, 37, '2026-03-15 16:00:00.000000', '2026-03-25 16:00:00.000000', 5760.00, 180.00, 669.60, 6249.60, 'Pending', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (38, 'INVSD037', 8, 38, '2026-03-16 16:00:00.000000', '2026-03-27 16:00:00.000000', 5870.00, 0.00, 704.40, 6574.40, 'Pending', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (39, 'INVSD038', 9, 39, '2026-03-17 16:00:00.000000', '2026-03-29 16:00:00.000000', 5980.00, 0.00, 717.60, 6697.60, 'Pending', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (40, 'INVSD039', 10, 40, '2026-03-18 16:00:00.000000', '2026-03-31 16:00:00.000000', 6090.00, 0.00, 730.80, 6820.80, 'Pending', 7, 'Seeded invoice linked to generated appointment.');
INSERT INTO `invoices` (`InvoiceID`, `InvoiceNumber`, `PatientID`, `AppointmentID`, `InvoiceDate`, `DueDate`, `TotalAmount`, `Discount`, `TaxAmount`, `GrandTotal`, `Status`, `CreatedBy`, `Notes`) VALUES (41, 'INVSD040', 11, 41, '2026-03-19 16:00:00.000000', '2026-04-02 16:00:00.000000', 6200.00, 0.00, 744.00, 6944.00, 'Pending', 7, 'Seeded invoice linked to generated appointment.');

-- Data for laborderdetails
INSERT INTO `laborderdetails` (`OrderDetailID`, `OrderID`, `TestID`, `ResultValue`, `ResultUnit`, `NormalRange`, `IsNormal`, `Notes`, `TechnicianID`, `CompletedDate`) VALUES (2, 2, 12, '83', 'mg/dL', '70-140', 1, 'Seeded laboratory detail result.', 11, '2026-02-23 17:00:00.000000');
INSERT INTO `laborderdetails` (`OrderDetailID`, `OrderID`, `TestID`, `ResultValue`, `ResultUnit`, `NormalRange`, `IsNormal`, `Notes`, `TechnicianID`, `CompletedDate`) VALUES (3, 3, 13, '86', 'mg/dL', '70-140', 1, 'Seeded laboratory detail result.', 11, '2026-02-24 17:00:00.000000');
INSERT INTO `laborderdetails` (`OrderDetailID`, `OrderID`, `TestID`, `ResultValue`, `ResultUnit`, `NormalRange`, `IsNormal`, `Notes`, `TechnicianID`, `CompletedDate`) VALUES (4, 4, 14, '89', 'mg/dL', '70-140', 1, 'Seeded laboratory detail result.', 11, '2026-02-25 17:00:00.000000');
INSERT INTO `laborderdetails` (`OrderDetailID`, `OrderID`, `TestID`, `ResultValue`, `ResultUnit`, `NormalRange`, `IsNormal`, `Notes`, `TechnicianID`, `CompletedDate`) VALUES (5, 5, 15, '92', 'mg/dL', '70-140', 0, 'Seeded laboratory detail result.', 11, '2026-02-26 17:00:00.000000');
INSERT INTO `laborderdetails` (`OrderDetailID`, `OrderID`, `TestID`, `ResultValue`, `ResultUnit`, `NormalRange`, `IsNormal`, `Notes`, `TechnicianID`, `CompletedDate`) VALUES (6, 6, 16, '95', 'mg/dL', '70-140', 1, 'Seeded laboratory detail result.', 11, '2026-02-27 17:00:00.000000');
INSERT INTO `laborderdetails` (`OrderDetailID`, `OrderID`, `TestID`, `ResultValue`, `ResultUnit`, `NormalRange`, `IsNormal`, `Notes`, `TechnicianID`, `CompletedDate`) VALUES (7, 7, 17, '98', 'mg/dL', '70-140', 1, 'Seeded laboratory detail result.', 11, '2026-02-28 17:00:00.000000');
INSERT INTO `laborderdetails` (`OrderDetailID`, `OrderID`, `TestID`, `ResultValue`, `ResultUnit`, `NormalRange`, `IsNormal`, `Notes`, `TechnicianID`, `CompletedDate`) VALUES (8, 8, 18, '101', 'mg/dL', '70-140', 1, 'Seeded laboratory detail result.', 11, '2026-03-01 17:00:00.000000');
INSERT INTO `laborderdetails` (`OrderDetailID`, `OrderID`, `TestID`, `ResultValue`, `ResultUnit`, `NormalRange`, `IsNormal`, `Notes`, `TechnicianID`, `CompletedDate`) VALUES (9, 9, 19, '104', 'mg/dL', '70-140', 0, 'Seeded laboratory detail result.', 11, '2026-03-02 17:00:00.000000');
INSERT INTO `laborderdetails` (`OrderDetailID`, `OrderID`, `TestID`, `ResultValue`, `ResultUnit`, `NormalRange`, `IsNormal`, `Notes`, `TechnicianID`, `CompletedDate`) VALUES (10, 10, 20, '107', 'mg/dL', '70-140', 1, 'Seeded laboratory detail result.', 11, '2026-03-03 17:00:00.000000');
INSERT INTO `laborderdetails` (`OrderDetailID`, `OrderID`, `TestID`, `ResultValue`, `ResultUnit`, `NormalRange`, `IsNormal`, `Notes`, `TechnicianID`, `CompletedDate`) VALUES (11, 11, 21, '110', 'mg/dL', '70-140', 1, 'Seeded laboratory detail result.', 11, '2026-03-04 17:00:00.000000');
INSERT INTO `laborderdetails` (`OrderDetailID`, `OrderID`, `TestID`, `ResultValue`, `ResultUnit`, `NormalRange`, `IsNormal`, `Notes`, `TechnicianID`, `CompletedDate`) VALUES (12, 12, 22, '113', 'mg/dL', '70-140', 1, 'Seeded laboratory detail result.', 11, '2026-03-05 17:00:00.000000');
INSERT INTO `laborderdetails` (`OrderDetailID`, `OrderID`, `TestID`, `ResultValue`, `ResultUnit`, `NormalRange`, `IsNormal`, `Notes`, `TechnicianID`, `CompletedDate`) VALUES (13, 13, 23, '116', 'mg/dL', '70-140', 0, 'Seeded laboratory detail result.', 11, '2026-03-06 17:00:00.000000');
INSERT INTO `laborderdetails` (`OrderDetailID`, `OrderID`, `TestID`, `ResultValue`, `ResultUnit`, `NormalRange`, `IsNormal`, `Notes`, `TechnicianID`, `CompletedDate`) VALUES (14, 14, 24, NULL, NULL, NULL, NULL, 'Seeded laboratory detail result.', 11, NULL);
INSERT INTO `laborderdetails` (`OrderDetailID`, `OrderID`, `TestID`, `ResultValue`, `ResultUnit`, `NormalRange`, `IsNormal`, `Notes`, `TechnicianID`, `CompletedDate`) VALUES (15, 15, 25, NULL, NULL, NULL, NULL, 'Seeded laboratory detail result.', 11, NULL);
INSERT INTO `laborderdetails` (`OrderDetailID`, `OrderID`, `TestID`, `ResultValue`, `ResultUnit`, `NormalRange`, `IsNormal`, `Notes`, `TechnicianID`, `CompletedDate`) VALUES (16, 16, 26, NULL, NULL, NULL, NULL, 'Seeded laboratory detail result.', 11, NULL);
INSERT INTO `laborderdetails` (`OrderDetailID`, `OrderID`, `TestID`, `ResultValue`, `ResultUnit`, `NormalRange`, `IsNormal`, `Notes`, `TechnicianID`, `CompletedDate`) VALUES (17, 17, 27, NULL, NULL, NULL, NULL, 'Seeded laboratory detail result.', 11, NULL);
INSERT INTO `laborderdetails` (`OrderDetailID`, `OrderID`, `TestID`, `ResultValue`, `ResultUnit`, `NormalRange`, `IsNormal`, `Notes`, `TechnicianID`, `CompletedDate`) VALUES (18, 18, 28, NULL, NULL, NULL, NULL, 'Seeded laboratory detail result.', 11, NULL);
INSERT INTO `laborderdetails` (`OrderDetailID`, `OrderID`, `TestID`, `ResultValue`, `ResultUnit`, `NormalRange`, `IsNormal`, `Notes`, `TechnicianID`, `CompletedDate`) VALUES (19, 19, 29, NULL, NULL, NULL, NULL, 'Seeded laboratory detail result.', 11, NULL);
INSERT INTO `laborderdetails` (`OrderDetailID`, `OrderID`, `TestID`, `ResultValue`, `ResultUnit`, `NormalRange`, `IsNormal`, `Notes`, `TechnicianID`, `CompletedDate`) VALUES (20, 20, 30, NULL, NULL, NULL, NULL, 'Seeded laboratory detail result.', 11, NULL);
INSERT INTO `laborderdetails` (`OrderDetailID`, `OrderID`, `TestID`, `ResultValue`, `ResultUnit`, `NormalRange`, `IsNormal`, `Notes`, `TechnicianID`, `CompletedDate`) VALUES (21, 21, 31, NULL, NULL, NULL, NULL, 'Seeded laboratory detail result.', 11, NULL);
INSERT INTO `laborderdetails` (`OrderDetailID`, `OrderID`, `TestID`, `ResultValue`, `ResultUnit`, `NormalRange`, `IsNormal`, `Notes`, `TechnicianID`, `CompletedDate`) VALUES (22, 22, 32, NULL, NULL, NULL, NULL, 'Seeded laboratory detail result.', 11, NULL);
INSERT INTO `laborderdetails` (`OrderDetailID`, `OrderID`, `TestID`, `ResultValue`, `ResultUnit`, `NormalRange`, `IsNormal`, `Notes`, `TechnicianID`, `CompletedDate`) VALUES (23, 23, 33, NULL, NULL, NULL, NULL, 'Seeded laboratory detail result.', 11, NULL);
INSERT INTO `laborderdetails` (`OrderDetailID`, `OrderID`, `TestID`, `ResultValue`, `ResultUnit`, `NormalRange`, `IsNormal`, `Notes`, `TechnicianID`, `CompletedDate`) VALUES (24, 24, 34, NULL, NULL, NULL, NULL, 'Seeded laboratory detail result.', 11, NULL);
INSERT INTO `laborderdetails` (`OrderDetailID`, `OrderID`, `TestID`, `ResultValue`, `ResultUnit`, `NormalRange`, `IsNormal`, `Notes`, `TechnicianID`, `CompletedDate`) VALUES (25, 25, 35, NULL, NULL, NULL, NULL, 'Seeded laboratory detail result.', 11, NULL);
INSERT INTO `laborderdetails` (`OrderDetailID`, `OrderID`, `TestID`, `ResultValue`, `ResultUnit`, `NormalRange`, `IsNormal`, `Notes`, `TechnicianID`, `CompletedDate`) VALUES (26, 26, 36, NULL, NULL, NULL, NULL, 'Seeded laboratory detail result.', 11, NULL);
INSERT INTO `laborderdetails` (`OrderDetailID`, `OrderID`, `TestID`, `ResultValue`, `ResultUnit`, `NormalRange`, `IsNormal`, `Notes`, `TechnicianID`, `CompletedDate`) VALUES (27, 27, 37, NULL, NULL, NULL, NULL, 'Seeded laboratory detail result.', 11, NULL);
INSERT INTO `laborderdetails` (`OrderDetailID`, `OrderID`, `TestID`, `ResultValue`, `ResultUnit`, `NormalRange`, `IsNormal`, `Notes`, `TechnicianID`, `CompletedDate`) VALUES (28, 28, 38, NULL, NULL, NULL, NULL, 'Seeded laboratory detail result.', 11, NULL);
INSERT INTO `laborderdetails` (`OrderDetailID`, `OrderID`, `TestID`, `ResultValue`, `ResultUnit`, `NormalRange`, `IsNormal`, `Notes`, `TechnicianID`, `CompletedDate`) VALUES (29, 29, 39, NULL, NULL, NULL, NULL, 'Seeded laboratory detail result.', 11, NULL);
INSERT INTO `laborderdetails` (`OrderDetailID`, `OrderID`, `TestID`, `ResultValue`, `ResultUnit`, `NormalRange`, `IsNormal`, `Notes`, `TechnicianID`, `CompletedDate`) VALUES (30, 30, 40, NULL, NULL, NULL, NULL, 'Seeded laboratory detail result.', 11, NULL);
INSERT INTO `laborderdetails` (`OrderDetailID`, `OrderID`, `TestID`, `ResultValue`, `ResultUnit`, `NormalRange`, `IsNormal`, `Notes`, `TechnicianID`, `CompletedDate`) VALUES (31, 31, 41, NULL, NULL, NULL, NULL, 'Seeded laboratory detail result.', 11, NULL);

-- Data for laborders
INSERT INTO `laborders` (`OrderID`, `OrderCode`, `VisitID`, `PatientID`, `DoctorID`, `OrderDate`, `Status`, `ResultDate`, `Notes`) VALUES (2, 'LABSD001', 2, 2, 3, '2026-02-22 09:00:00.000000', 'Completed', '2026-02-23 17:00:00.000000', 'Seeded laboratory order.');
INSERT INTO `laborders` (`OrderID`, `OrderCode`, `VisitID`, `PatientID`, `DoctorID`, `OrderDate`, `Status`, `ResultDate`, `Notes`) VALUES (3, 'LABSD002', 3, 3, 4, '2026-02-23 09:00:00.000000', 'Completed', '2026-02-24 17:00:00.000000', 'Seeded laboratory order.');
INSERT INTO `laborders` (`OrderID`, `OrderCode`, `VisitID`, `PatientID`, `DoctorID`, `OrderDate`, `Status`, `ResultDate`, `Notes`) VALUES (4, 'LABSD003', 4, 4, 5, '2026-02-24 09:00:00.000000', 'Completed', '2026-02-25 17:00:00.000000', 'Seeded laboratory order.');
INSERT INTO `laborders` (`OrderID`, `OrderCode`, `VisitID`, `PatientID`, `DoctorID`, `OrderDate`, `Status`, `ResultDate`, `Notes`) VALUES (5, 'LABSD004', 5, 5, 6, '2026-02-25 09:00:00.000000', 'Completed', '2026-02-26 17:00:00.000000', 'Seeded laboratory order.');
INSERT INTO `laborders` (`OrderID`, `OrderCode`, `VisitID`, `PatientID`, `DoctorID`, `OrderDate`, `Status`, `ResultDate`, `Notes`) VALUES (6, 'LABSD005', 6, 6, 7, '2026-02-26 09:00:00.000000', 'Completed', '2026-02-27 17:00:00.000000', 'Seeded laboratory order.');
INSERT INTO `laborders` (`OrderID`, `OrderCode`, `VisitID`, `PatientID`, `DoctorID`, `OrderDate`, `Status`, `ResultDate`, `Notes`) VALUES (7, 'LABSD006', 7, 7, 8, '2026-02-27 09:00:00.000000', 'Completed', '2026-02-28 17:00:00.000000', 'Seeded laboratory order.');
INSERT INTO `laborders` (`OrderID`, `OrderCode`, `VisitID`, `PatientID`, `DoctorID`, `OrderDate`, `Status`, `ResultDate`, `Notes`) VALUES (8, 'LABSD007', 8, 8, 9, '2026-02-28 09:00:00.000000', 'Completed', '2026-03-01 17:00:00.000000', 'Seeded laboratory order.');
INSERT INTO `laborders` (`OrderID`, `OrderCode`, `VisitID`, `PatientID`, `DoctorID`, `OrderDate`, `Status`, `ResultDate`, `Notes`) VALUES (9, 'LABSD008', 9, 9, 10, '2026-03-01 09:00:00.000000', 'Completed', '2026-03-02 17:00:00.000000', 'Seeded laboratory order.');
INSERT INTO `laborders` (`OrderID`, `OrderCode`, `VisitID`, `PatientID`, `DoctorID`, `OrderDate`, `Status`, `ResultDate`, `Notes`) VALUES (10, 'LABSD009', 10, 10, 11, '2026-03-02 09:00:00.000000', 'Completed', '2026-03-03 17:00:00.000000', 'Seeded laboratory order.');
INSERT INTO `laborders` (`OrderID`, `OrderCode`, `VisitID`, `PatientID`, `DoctorID`, `OrderDate`, `Status`, `ResultDate`, `Notes`) VALUES (11, 'LABSD010', 11, 11, 12, '2026-03-03 09:00:00.000000', 'Completed', '2026-03-04 17:00:00.000000', 'Seeded laboratory order.');
INSERT INTO `laborders` (`OrderID`, `OrderCode`, `VisitID`, `PatientID`, `DoctorID`, `OrderDate`, `Status`, `ResultDate`, `Notes`) VALUES (12, 'LABSD011', 12, 12, 13, '2026-03-04 09:00:00.000000', 'Completed', '2026-03-05 17:00:00.000000', 'Seeded laboratory order.');
INSERT INTO `laborders` (`OrderID`, `OrderCode`, `VisitID`, `PatientID`, `DoctorID`, `OrderDate`, `Status`, `ResultDate`, `Notes`) VALUES (13, 'LABSD012', 13, 13, 14, '2026-03-05 09:00:00.000000', 'Completed', '2026-03-06 17:00:00.000000', 'Seeded laboratory order.');
INSERT INTO `laborders` (`OrderID`, `OrderCode`, `VisitID`, `PatientID`, `DoctorID`, `OrderDate`, `Status`, `ResultDate`, `Notes`) VALUES (14, 'LABSD013', 14, 14, 15, '2026-03-06 09:00:00.000000', 'In Progress', NULL, 'Seeded laboratory order.');
INSERT INTO `laborders` (`OrderID`, `OrderCode`, `VisitID`, `PatientID`, `DoctorID`, `OrderDate`, `Status`, `ResultDate`, `Notes`) VALUES (15, 'LABSD014', 15, 15, 16, '2026-03-07 09:00:00.000000', 'In Progress', NULL, 'Seeded laboratory order.');
INSERT INTO `laborders` (`OrderID`, `OrderCode`, `VisitID`, `PatientID`, `DoctorID`, `OrderDate`, `Status`, `ResultDate`, `Notes`) VALUES (16, 'LABSD015', 16, 16, 17, '2026-03-08 09:00:00.000000', 'In Progress', NULL, 'Seeded laboratory order.');
INSERT INTO `laborders` (`OrderID`, `OrderCode`, `VisitID`, `PatientID`, `DoctorID`, `OrderDate`, `Status`, `ResultDate`, `Notes`) VALUES (17, 'LABSD016', 17, 17, 18, '2026-03-09 09:00:00.000000', 'In Progress', NULL, 'Seeded laboratory order.');
INSERT INTO `laborders` (`OrderID`, `OrderCode`, `VisitID`, `PatientID`, `DoctorID`, `OrderDate`, `Status`, `ResultDate`, `Notes`) VALUES (18, 'LABSD017', 18, 18, 19, '2026-03-10 09:00:00.000000', 'In Progress', NULL, 'Seeded laboratory order.');
INSERT INTO `laborders` (`OrderID`, `OrderCode`, `VisitID`, `PatientID`, `DoctorID`, `OrderDate`, `Status`, `ResultDate`, `Notes`) VALUES (19, 'LABSD018', 19, 19, 20, '2026-03-11 09:00:00.000000', 'In Progress', NULL, 'Seeded laboratory order.');
INSERT INTO `laborders` (`OrderID`, `OrderCode`, `VisitID`, `PatientID`, `DoctorID`, `OrderDate`, `Status`, `ResultDate`, `Notes`) VALUES (20, 'LABSD019', 20, 20, 21, '2026-03-12 09:00:00.000000', 'In Progress', NULL, 'Seeded laboratory order.');
INSERT INTO `laborders` (`OrderID`, `OrderCode`, `VisitID`, `PatientID`, `DoctorID`, `OrderDate`, `Status`, `ResultDate`, `Notes`) VALUES (21, 'LABSD020', 21, 21, 22, '2026-03-13 09:00:00.000000', 'In Progress', NULL, 'Seeded laboratory order.');
INSERT INTO `laborders` (`OrderID`, `OrderCode`, `VisitID`, `PatientID`, `DoctorID`, `OrderDate`, `Status`, `ResultDate`, `Notes`) VALUES (22, 'LABSD021', 22, 22, 23, '2026-03-14 09:00:00.000000', 'In Progress', NULL, 'Seeded laboratory order.');
INSERT INTO `laborders` (`OrderID`, `OrderCode`, `VisitID`, `PatientID`, `DoctorID`, `OrderDate`, `Status`, `ResultDate`, `Notes`) VALUES (23, 'LABSD022', 23, 23, 24, '2026-03-15 09:00:00.000000', 'In Progress', NULL, 'Seeded laboratory order.');
INSERT INTO `laborders` (`OrderID`, `OrderCode`, `VisitID`, `PatientID`, `DoctorID`, `OrderDate`, `Status`, `ResultDate`, `Notes`) VALUES (24, 'LABSD023', 24, 24, 25, '2026-03-16 09:00:00.000000', 'Pending', NULL, 'Seeded laboratory order.');
INSERT INTO `laborders` (`OrderID`, `OrderCode`, `VisitID`, `PatientID`, `DoctorID`, `OrderDate`, `Status`, `ResultDate`, `Notes`) VALUES (25, 'LABSD024', 25, 25, 26, '2026-03-17 09:00:00.000000', 'Pending', NULL, 'Seeded laboratory order.');
INSERT INTO `laborders` (`OrderID`, `OrderCode`, `VisitID`, `PatientID`, `DoctorID`, `OrderDate`, `Status`, `ResultDate`, `Notes`) VALUES (26, 'LABSD025', 26, 26, 27, '2026-03-18 09:00:00.000000', 'Pending', NULL, 'Seeded laboratory order.');
INSERT INTO `laborders` (`OrderID`, `OrderCode`, `VisitID`, `PatientID`, `DoctorID`, `OrderDate`, `Status`, `ResultDate`, `Notes`) VALUES (27, 'LABSD026', 27, 27, 28, '2026-03-19 09:00:00.000000', 'Pending', NULL, 'Seeded laboratory order.');
INSERT INTO `laborders` (`OrderID`, `OrderCode`, `VisitID`, `PatientID`, `DoctorID`, `OrderDate`, `Status`, `ResultDate`, `Notes`) VALUES (28, 'LABSD027', 28, 28, 29, '2026-03-20 09:00:00.000000', 'Pending', NULL, 'Seeded laboratory order.');
INSERT INTO `laborders` (`OrderID`, `OrderCode`, `VisitID`, `PatientID`, `DoctorID`, `OrderDate`, `Status`, `ResultDate`, `Notes`) VALUES (29, 'LABSD028', 29, 29, 30, '2026-03-21 09:00:00.000000', 'Pending', NULL, 'Seeded laboratory order.');
INSERT INTO `laborders` (`OrderID`, `OrderCode`, `VisitID`, `PatientID`, `DoctorID`, `OrderDate`, `Status`, `ResultDate`, `Notes`) VALUES (30, 'LABSD029', 30, 30, 31, '2026-03-22 09:00:00.000000', 'Pending', NULL, 'Seeded laboratory order.');
INSERT INTO `laborders` (`OrderID`, `OrderCode`, `VisitID`, `PatientID`, `DoctorID`, `OrderDate`, `Status`, `ResultDate`, `Notes`) VALUES (31, 'LABSD030', 31, 31, 32, '2026-03-23 09:00:00.000000', 'Pending', NULL, 'Seeded laboratory order.');

-- Data for labtests
INSERT INTO `labtests` (`TestID`, `TestCode`, `TestName`, `Category`, `NormalRange`, `Unit`, `Price`) VALUES (12, 'LABTSD001', 'Seed Laboratory Test 01', 'Immunology', '4.0-10.0', 'x10^9/L', 445.00);
INSERT INTO `labtests` (`TestID`, `TestCode`, `TestName`, `Category`, `NormalRange`, `Unit`, `Price`) VALUES (13, 'LABTSD002', 'Seed Laboratory Test 02', 'Hematology', '70-140', 'mg/dL', 490.00);
INSERT INTO `labtests` (`TestID`, `TestCode`, `TestName`, `Category`, `NormalRange`, `Unit`, `Price`) VALUES (14, 'LABTSD003', 'Seed Laboratory Test 03', 'Chemistry', '4.0-10.0', 'x10^9/L', 535.00);
INSERT INTO `labtests` (`TestID`, `TestCode`, `TestName`, `Category`, `NormalRange`, `Unit`, `Price`) VALUES (15, 'LABTSD004', 'Seed Laboratory Test 04', 'Hematology', '70-140', 'mg/dL', 580.00);
INSERT INTO `labtests` (`TestID`, `TestCode`, `TestName`, `Category`, `NormalRange`, `Unit`, `Price`) VALUES (16, 'LABTSD005', 'Seed Laboratory Test 05', 'Immunology', '4.0-10.0', 'x10^9/L', 625.00);
INSERT INTO `labtests` (`TestID`, `TestCode`, `TestName`, `Category`, `NormalRange`, `Unit`, `Price`) VALUES (17, 'LABTSD006', 'Seed Laboratory Test 06', 'Chemistry', '70-140', 'mg/dL', 670.00);
INSERT INTO `labtests` (`TestID`, `TestCode`, `TestName`, `Category`, `NormalRange`, `Unit`, `Price`) VALUES (18, 'LABTSD007', 'Seed Laboratory Test 07', 'Immunology', '4.0-10.0', 'x10^9/L', 715.00);
INSERT INTO `labtests` (`TestID`, `TestCode`, `TestName`, `Category`, `NormalRange`, `Unit`, `Price`) VALUES (19, 'LABTSD008', 'Seed Laboratory Test 08', 'Hematology', '70-140', 'mg/dL', 760.00);
INSERT INTO `labtests` (`TestID`, `TestCode`, `TestName`, `Category`, `NormalRange`, `Unit`, `Price`) VALUES (20, 'LABTSD009', 'Seed Laboratory Test 09', 'Chemistry', '4.0-10.0', 'x10^9/L', 805.00);
INSERT INTO `labtests` (`TestID`, `TestCode`, `TestName`, `Category`, `NormalRange`, `Unit`, `Price`) VALUES (21, 'LABTSD010', 'Seed Laboratory Test 10', 'Hematology', '70-140', 'mg/dL', 850.00);
INSERT INTO `labtests` (`TestID`, `TestCode`, `TestName`, `Category`, `NormalRange`, `Unit`, `Price`) VALUES (22, 'LABTSD011', 'Seed Laboratory Test 11', 'Immunology', '4.0-10.0', 'x10^9/L', 895.00);
INSERT INTO `labtests` (`TestID`, `TestCode`, `TestName`, `Category`, `NormalRange`, `Unit`, `Price`) VALUES (23, 'LABTSD012', 'Seed Laboratory Test 12', 'Chemistry', '70-140', 'mg/dL', 940.00);
INSERT INTO `labtests` (`TestID`, `TestCode`, `TestName`, `Category`, `NormalRange`, `Unit`, `Price`) VALUES (24, 'LABTSD013', 'Seed Laboratory Test 13', 'Immunology', '4.0-10.0', 'x10^9/L', 985.00);
INSERT INTO `labtests` (`TestID`, `TestCode`, `TestName`, `Category`, `NormalRange`, `Unit`, `Price`) VALUES (25, 'LABTSD014', 'Seed Laboratory Test 14', 'Hematology', '70-140', 'mg/dL', 1030.00);
INSERT INTO `labtests` (`TestID`, `TestCode`, `TestName`, `Category`, `NormalRange`, `Unit`, `Price`) VALUES (26, 'LABTSD015', 'Seed Laboratory Test 15', 'Chemistry', '4.0-10.0', 'x10^9/L', 1075.00);
INSERT INTO `labtests` (`TestID`, `TestCode`, `TestName`, `Category`, `NormalRange`, `Unit`, `Price`) VALUES (27, 'LABTSD016', 'Seed Laboratory Test 16', 'Hematology', '70-140', 'mg/dL', 1120.00);
INSERT INTO `labtests` (`TestID`, `TestCode`, `TestName`, `Category`, `NormalRange`, `Unit`, `Price`) VALUES (28, 'LABTSD017', 'Seed Laboratory Test 17', 'Immunology', '4.0-10.0', 'x10^9/L', 1165.00);
INSERT INTO `labtests` (`TestID`, `TestCode`, `TestName`, `Category`, `NormalRange`, `Unit`, `Price`) VALUES (29, 'LABTSD018', 'Seed Laboratory Test 18', 'Chemistry', '70-140', 'mg/dL', 1210.00);
INSERT INTO `labtests` (`TestID`, `TestCode`, `TestName`, `Category`, `NormalRange`, `Unit`, `Price`) VALUES (30, 'LABTSD019', 'Seed Laboratory Test 19', 'Immunology', '4.0-10.0', 'x10^9/L', 1255.00);
INSERT INTO `labtests` (`TestID`, `TestCode`, `TestName`, `Category`, `NormalRange`, `Unit`, `Price`) VALUES (31, 'LABTSD020', 'Seed Laboratory Test 20', 'Hematology', '70-140', 'mg/dL', 1300.00);
INSERT INTO `labtests` (`TestID`, `TestCode`, `TestName`, `Category`, `NormalRange`, `Unit`, `Price`) VALUES (32, 'LABTSD021', 'Seed Laboratory Test 21', 'Chemistry', '4.0-10.0', 'x10^9/L', 1345.00);
INSERT INTO `labtests` (`TestID`, `TestCode`, `TestName`, `Category`, `NormalRange`, `Unit`, `Price`) VALUES (33, 'LABTSD022', 'Seed Laboratory Test 22', 'Hematology', '70-140', 'mg/dL', 1390.00);
INSERT INTO `labtests` (`TestID`, `TestCode`, `TestName`, `Category`, `NormalRange`, `Unit`, `Price`) VALUES (34, 'LABTSD023', 'Seed Laboratory Test 23', 'Immunology', '4.0-10.0', 'x10^9/L', 1435.00);
INSERT INTO `labtests` (`TestID`, `TestCode`, `TestName`, `Category`, `NormalRange`, `Unit`, `Price`) VALUES (35, 'LABTSD024', 'Seed Laboratory Test 24', 'Chemistry', '70-140', 'mg/dL', 1480.00);
INSERT INTO `labtests` (`TestID`, `TestCode`, `TestName`, `Category`, `NormalRange`, `Unit`, `Price`) VALUES (36, 'LABTSD025', 'Seed Laboratory Test 25', 'Immunology', '4.0-10.0', 'x10^9/L', 1525.00);
INSERT INTO `labtests` (`TestID`, `TestCode`, `TestName`, `Category`, `NormalRange`, `Unit`, `Price`) VALUES (37, 'LABTSD026', 'Seed Laboratory Test 26', 'Hematology', '70-140', 'mg/dL', 1570.00);
INSERT INTO `labtests` (`TestID`, `TestCode`, `TestName`, `Category`, `NormalRange`, `Unit`, `Price`) VALUES (38, 'LABTSD027', 'Seed Laboratory Test 27', 'Chemistry', '4.0-10.0', 'x10^9/L', 1615.00);
INSERT INTO `labtests` (`TestID`, `TestCode`, `TestName`, `Category`, `NormalRange`, `Unit`, `Price`) VALUES (39, 'LABTSD028', 'Seed Laboratory Test 28', 'Hematology', '70-140', 'mg/dL', 1660.00);
INSERT INTO `labtests` (`TestID`, `TestCode`, `TestName`, `Category`, `NormalRange`, `Unit`, `Price`) VALUES (40, 'LABTSD029', 'Seed Laboratory Test 29', 'Immunology', '4.0-10.0', 'x10^9/L', 1705.00);
INSERT INTO `labtests` (`TestID`, `TestCode`, `TestName`, `Category`, `NormalRange`, `Unit`, `Price`) VALUES (41, 'LABTSD030', 'Seed Laboratory Test 30', 'Chemistry', '70-140', 'mg/dL', 1750.00);

-- Data for medicalhistories
INSERT INTO `medicalhistories` (`HistoryID`, `PatientID`, `HistoryType`, `Description`, `DiagnosisDate`, `Severity`, `Status`, `RecordedBy`, `RecordedDate`) VALUES (2, 2, 'Chronic Condition', 'Essential hypertension', '2025-12-30 00:00:00.000000', 'Mild', 'Active', 7, '2026-02-06 00:00:00.000000');
INSERT INTO `medicalhistories` (`HistoryID`, `PatientID`, `HistoryType`, `Description`, `DiagnosisDate`, `Severity`, `Status`, `RecordedBy`, `RecordedDate`) VALUES (3, 3, 'Chronic Condition', 'Type 2 diabetes mellitus', '2025-12-21 00:00:00.000000', 'Moderate', 'Active', 7, '2026-02-05 00:00:00.000000');
INSERT INTO `medicalhistories` (`HistoryID`, `PatientID`, `HistoryType`, `Description`, `DiagnosisDate`, `Severity`, `Status`, `RecordedBy`, `RecordedDate`) VALUES (4, 4, 'Chronic Condition', 'Bronchial asthma', '2025-12-12 00:00:00.000000', 'Severe', 'Active', 7, '2026-02-04 00:00:00.000000');
INSERT INTO `medicalhistories` (`HistoryID`, `PatientID`, `HistoryType`, `Description`, `DiagnosisDate`, `Severity`, `Status`, `RecordedBy`, `RecordedDate`) VALUES (5, 5, 'Chronic Condition', 'Hyperlipidemia', '2025-12-03 00:00:00.000000', 'Moderate', 'Active', 7, '2026-02-03 00:00:00.000000');
INSERT INTO `medicalhistories` (`HistoryID`, `PatientID`, `HistoryType`, `Description`, `DiagnosisDate`, `Severity`, `Status`, `RecordedBy`, `RecordedDate`) VALUES (6, 6, 'Chronic Condition', 'Migraine episodes', '2025-11-24 00:00:00.000000', 'Mild', 'Active', 7, '2026-02-02 00:00:00.000000');
INSERT INTO `medicalhistories` (`HistoryID`, `PatientID`, `HistoryType`, `Description`, `DiagnosisDate`, `Severity`, `Status`, `RecordedBy`, `RecordedDate`) VALUES (7, 7, 'Chronic Condition', 'Allergic rhinitis', '2025-11-15 00:00:00.000000', 'Severe', 'Active', 7, '2026-02-01 00:00:00.000000');
INSERT INTO `medicalhistories` (`HistoryID`, `PatientID`, `HistoryType`, `Description`, `DiagnosisDate`, `Severity`, `Status`, `RecordedBy`, `RecordedDate`) VALUES (8, 8, 'Chronic Condition', 'Lumbar strain', '2025-11-06 00:00:00.000000', 'Mild', 'Active', 7, '2026-01-31 00:00:00.000000');
INSERT INTO `medicalhistories` (`HistoryID`, `PatientID`, `HistoryType`, `Description`, `DiagnosisDate`, `Severity`, `Status`, `RecordedBy`, `RecordedDate`) VALUES (9, 9, 'Chronic Condition', 'Osteoarthritis', '2025-10-28 00:00:00.000000', 'Moderate', 'Active', 7, '2026-01-30 00:00:00.000000');
INSERT INTO `medicalhistories` (`HistoryID`, `PatientID`, `HistoryType`, `Description`, `DiagnosisDate`, `Severity`, `Status`, `RecordedBy`, `RecordedDate`) VALUES (10, 10, 'Chronic Condition', 'Acute gastroenteritis', '2025-10-19 00:00:00.000000', 'Severe', 'Active', 7, '2026-01-29 00:00:00.000000');
INSERT INTO `medicalhistories` (`HistoryID`, `PatientID`, `HistoryType`, `Description`, `DiagnosisDate`, `Severity`, `Status`, `RecordedBy`, `RecordedDate`) VALUES (11, 11, 'Chronic Condition', 'Urinary tract infection', '2025-10-10 00:00:00.000000', 'Moderate', 'Active', 7, '2026-01-28 00:00:00.000000');
INSERT INTO `medicalhistories` (`HistoryID`, `PatientID`, `HistoryType`, `Description`, `DiagnosisDate`, `Severity`, `Status`, `RecordedBy`, `RecordedDate`) VALUES (12, 12, 'Chronic Condition', 'Essential hypertension', '2025-10-01 00:00:00.000000', 'Mild', 'Active', 7, '2026-01-27 00:00:00.000000');
INSERT INTO `medicalhistories` (`HistoryID`, `PatientID`, `HistoryType`, `Description`, `DiagnosisDate`, `Severity`, `Status`, `RecordedBy`, `RecordedDate`) VALUES (13, 13, 'Chronic Condition', 'Type 2 diabetes mellitus', '2025-09-22 00:00:00.000000', 'Severe', 'Active', 7, '2026-01-26 00:00:00.000000');
INSERT INTO `medicalhistories` (`HistoryID`, `PatientID`, `HistoryType`, `Description`, `DiagnosisDate`, `Severity`, `Status`, `RecordedBy`, `RecordedDate`) VALUES (14, 14, 'Chronic Condition', 'Bronchial asthma', '2025-09-13 00:00:00.000000', 'Mild', 'Active', 7, '2026-01-25 00:00:00.000000');
INSERT INTO `medicalhistories` (`HistoryID`, `PatientID`, `HistoryType`, `Description`, `DiagnosisDate`, `Severity`, `Status`, `RecordedBy`, `RecordedDate`) VALUES (15, 15, 'Chronic Condition', 'Hyperlipidemia', '2025-09-04 00:00:00.000000', 'Moderate', 'Active', 7, '2026-01-24 00:00:00.000000');
INSERT INTO `medicalhistories` (`HistoryID`, `PatientID`, `HistoryType`, `Description`, `DiagnosisDate`, `Severity`, `Status`, `RecordedBy`, `RecordedDate`) VALUES (16, 16, 'Chronic Condition', 'Migraine episodes', '2025-08-26 00:00:00.000000', 'Severe', 'Active', 7, '2026-01-23 00:00:00.000000');
INSERT INTO `medicalhistories` (`HistoryID`, `PatientID`, `HistoryType`, `Description`, `DiagnosisDate`, `Severity`, `Status`, `RecordedBy`, `RecordedDate`) VALUES (17, 17, 'Chronic Condition', 'Allergic rhinitis', '2025-08-17 00:00:00.000000', 'Moderate', 'Active', 7, '2026-01-22 00:00:00.000000');
INSERT INTO `medicalhistories` (`HistoryID`, `PatientID`, `HistoryType`, `Description`, `DiagnosisDate`, `Severity`, `Status`, `RecordedBy`, `RecordedDate`) VALUES (18, 18, 'Chronic Condition', 'Lumbar strain', '2025-08-08 00:00:00.000000', 'Mild', 'Active', 7, '2026-01-21 00:00:00.000000');
INSERT INTO `medicalhistories` (`HistoryID`, `PatientID`, `HistoryType`, `Description`, `DiagnosisDate`, `Severity`, `Status`, `RecordedBy`, `RecordedDate`) VALUES (19, 19, 'Chronic Condition', 'Osteoarthritis', '2025-07-30 00:00:00.000000', 'Severe', 'Active', 7, '2026-01-20 00:00:00.000000');
INSERT INTO `medicalhistories` (`HistoryID`, `PatientID`, `HistoryType`, `Description`, `DiagnosisDate`, `Severity`, `Status`, `RecordedBy`, `RecordedDate`) VALUES (20, 20, 'Chronic Condition', 'Acute gastroenteritis', '2025-07-21 00:00:00.000000', 'Mild', 'Active', 7, '2026-01-19 00:00:00.000000');
INSERT INTO `medicalhistories` (`HistoryID`, `PatientID`, `HistoryType`, `Description`, `DiagnosisDate`, `Severity`, `Status`, `RecordedBy`, `RecordedDate`) VALUES (21, 21, 'Chronic Condition', 'Urinary tract infection', '2025-07-12 00:00:00.000000', 'Moderate', 'Active', 7, '2026-01-18 00:00:00.000000');
INSERT INTO `medicalhistories` (`HistoryID`, `PatientID`, `HistoryType`, `Description`, `DiagnosisDate`, `Severity`, `Status`, `RecordedBy`, `RecordedDate`) VALUES (22, 22, 'Chronic Condition', 'Essential hypertension', '2025-07-03 00:00:00.000000', 'Severe', 'Active', 7, '2026-01-17 00:00:00.000000');
INSERT INTO `medicalhistories` (`HistoryID`, `PatientID`, `HistoryType`, `Description`, `DiagnosisDate`, `Severity`, `Status`, `RecordedBy`, `RecordedDate`) VALUES (23, 23, 'Chronic Condition', 'Type 2 diabetes mellitus', '2025-06-24 00:00:00.000000', 'Moderate', 'Active', 7, '2026-01-16 00:00:00.000000');
INSERT INTO `medicalhistories` (`HistoryID`, `PatientID`, `HistoryType`, `Description`, `DiagnosisDate`, `Severity`, `Status`, `RecordedBy`, `RecordedDate`) VALUES (24, 24, 'Chronic Condition', 'Bronchial asthma', '2025-06-15 00:00:00.000000', 'Mild', 'Active', 7, '2026-01-15 00:00:00.000000');
INSERT INTO `medicalhistories` (`HistoryID`, `PatientID`, `HistoryType`, `Description`, `DiagnosisDate`, `Severity`, `Status`, `RecordedBy`, `RecordedDate`) VALUES (25, 25, 'Chronic Condition', 'Hyperlipidemia', '2025-06-06 00:00:00.000000', 'Severe', 'Active', 7, '2026-01-14 00:00:00.000000');
INSERT INTO `medicalhistories` (`HistoryID`, `PatientID`, `HistoryType`, `Description`, `DiagnosisDate`, `Severity`, `Status`, `RecordedBy`, `RecordedDate`) VALUES (26, 26, 'Chronic Condition', 'Migraine episodes', '2025-05-28 00:00:00.000000', 'Mild', 'Active', 7, '2026-01-13 00:00:00.000000');
INSERT INTO `medicalhistories` (`HistoryID`, `PatientID`, `HistoryType`, `Description`, `DiagnosisDate`, `Severity`, `Status`, `RecordedBy`, `RecordedDate`) VALUES (27, 27, 'Chronic Condition', 'Allergic rhinitis', '2025-05-19 00:00:00.000000', 'Moderate', 'Active', 7, '2026-01-12 00:00:00.000000');
INSERT INTO `medicalhistories` (`HistoryID`, `PatientID`, `HistoryType`, `Description`, `DiagnosisDate`, `Severity`, `Status`, `RecordedBy`, `RecordedDate`) VALUES (28, 28, 'Chronic Condition', 'Lumbar strain', '2025-05-10 00:00:00.000000', 'Severe', 'Active', 7, '2026-01-11 00:00:00.000000');
INSERT INTO `medicalhistories` (`HistoryID`, `PatientID`, `HistoryType`, `Description`, `DiagnosisDate`, `Severity`, `Status`, `RecordedBy`, `RecordedDate`) VALUES (29, 29, 'Chronic Condition', 'Osteoarthritis', '2025-05-01 00:00:00.000000', 'Moderate', 'Active', 7, '2026-01-10 00:00:00.000000');
INSERT INTO `medicalhistories` (`HistoryID`, `PatientID`, `HistoryType`, `Description`, `DiagnosisDate`, `Severity`, `Status`, `RecordedBy`, `RecordedDate`) VALUES (30, 30, 'Chronic Condition', 'Acute gastroenteritis', '2025-04-22 00:00:00.000000', 'Mild', 'Active', 7, '2026-01-09 00:00:00.000000');
INSERT INTO `medicalhistories` (`HistoryID`, `PatientID`, `HistoryType`, `Description`, `DiagnosisDate`, `Severity`, `Status`, `RecordedBy`, `RecordedDate`) VALUES (31, 31, 'Chronic Condition', 'Urinary tract infection', '2025-04-13 00:00:00.000000', 'Severe', 'Active', 7, '2026-01-08 00:00:00.000000');

-- Data for medicinecategories
INSERT INTO `medicinecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (10, 'Seed Medicine Category 01', 'Sample medicine category 01.');
INSERT INTO `medicinecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (11, 'Seed Medicine Category 02', 'Sample medicine category 02.');
INSERT INTO `medicinecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (12, 'Seed Medicine Category 03', 'Sample medicine category 03.');
INSERT INTO `medicinecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (13, 'Seed Medicine Category 04', 'Sample medicine category 04.');
INSERT INTO `medicinecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (14, 'Seed Medicine Category 05', 'Sample medicine category 05.');
INSERT INTO `medicinecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (15, 'Seed Medicine Category 06', 'Sample medicine category 06.');
INSERT INTO `medicinecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (16, 'Seed Medicine Category 07', 'Sample medicine category 07.');
INSERT INTO `medicinecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (17, 'Seed Medicine Category 08', 'Sample medicine category 08.');
INSERT INTO `medicinecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (18, 'Seed Medicine Category 09', 'Sample medicine category 09.');
INSERT INTO `medicinecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (19, 'Seed Medicine Category 10', 'Sample medicine category 10.');
INSERT INTO `medicinecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (20, 'Seed Medicine Category 11', 'Sample medicine category 11.');
INSERT INTO `medicinecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (21, 'Seed Medicine Category 12', 'Sample medicine category 12.');
INSERT INTO `medicinecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (22, 'Seed Medicine Category 13', 'Sample medicine category 13.');
INSERT INTO `medicinecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (23, 'Seed Medicine Category 14', 'Sample medicine category 14.');
INSERT INTO `medicinecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (24, 'Seed Medicine Category 15', 'Sample medicine category 15.');
INSERT INTO `medicinecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (25, 'Seed Medicine Category 16', 'Sample medicine category 16.');
INSERT INTO `medicinecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (26, 'Seed Medicine Category 17', 'Sample medicine category 17.');
INSERT INTO `medicinecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (27, 'Seed Medicine Category 18', 'Sample medicine category 18.');
INSERT INTO `medicinecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (28, 'Seed Medicine Category 19', 'Sample medicine category 19.');
INSERT INTO `medicinecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (29, 'Seed Medicine Category 20', 'Sample medicine category 20.');
INSERT INTO `medicinecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (30, 'Seed Medicine Category 21', 'Sample medicine category 21.');
INSERT INTO `medicinecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (31, 'Seed Medicine Category 22', 'Sample medicine category 22.');
INSERT INTO `medicinecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (32, 'Seed Medicine Category 23', 'Sample medicine category 23.');
INSERT INTO `medicinecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (33, 'Seed Medicine Category 24', 'Sample medicine category 24.');
INSERT INTO `medicinecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (34, 'Seed Medicine Category 25', 'Sample medicine category 25.');
INSERT INTO `medicinecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (35, 'Seed Medicine Category 26', 'Sample medicine category 26.');
INSERT INTO `medicinecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (36, 'Seed Medicine Category 27', 'Sample medicine category 27.');
INSERT INTO `medicinecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (37, 'Seed Medicine Category 28', 'Sample medicine category 28.');
INSERT INTO `medicinecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (38, 'Seed Medicine Category 29', 'Sample medicine category 29.');
INSERT INTO `medicinecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (39, 'Seed Medicine Category 30', 'Sample medicine category 30.');

-- Data for medicines
INSERT INTO `medicines` (`MedicineID`, `MedicineCode`, `MedicineName`, `GenericName`, `CategoryID`, `Manufacturer`, `UnitOfMeasure`, `UnitPrice`, `SellingPrice`, `ReorderLevel`, `IsActive`) VALUES (10, 'MEDSD001', 'Seed Medicine 01', 'Generic Compound 01', 10, 'Seed Pharma 1', 'Tablet', 20.00, 33.00, 11, 1);
INSERT INTO `medicines` (`MedicineID`, `MedicineCode`, `MedicineName`, `GenericName`, `CategoryID`, `Manufacturer`, `UnitOfMeasure`, `UnitPrice`, `SellingPrice`, `ReorderLevel`, `IsActive`) VALUES (11, 'MEDSD002', 'Seed Medicine 02', 'Generic Compound 02', 11, 'Seed Pharma 2', 'Capsule', 22.00, 36.00, 12, 1);
INSERT INTO `medicines` (`MedicineID`, `MedicineCode`, `MedicineName`, `GenericName`, `CategoryID`, `Manufacturer`, `UnitOfMeasure`, `UnitPrice`, `SellingPrice`, `ReorderLevel`, `IsActive`) VALUES (12, 'MEDSD003', 'Seed Medicine 03', 'Generic Compound 03', 12, 'Seed Pharma 3', 'Vial', 24.00, 39.00, 13, 1);
INSERT INTO `medicines` (`MedicineID`, `MedicineCode`, `MedicineName`, `GenericName`, `CategoryID`, `Manufacturer`, `UnitOfMeasure`, `UnitPrice`, `SellingPrice`, `ReorderLevel`, `IsActive`) VALUES (13, 'MEDSD004', 'Seed Medicine 04', 'Generic Compound 04', 13, 'Seed Pharma 4', 'Capsule', 26.00, 42.00, 14, 1);
INSERT INTO `medicines` (`MedicineID`, `MedicineCode`, `MedicineName`, `GenericName`, `CategoryID`, `Manufacturer`, `UnitOfMeasure`, `UnitPrice`, `SellingPrice`, `ReorderLevel`, `IsActive`) VALUES (14, 'MEDSD005', 'Seed Medicine 05', 'Generic Compound 05', 14, 'Seed Pharma 5', 'Tablet', 28.00, 45.00, 15, 1);
INSERT INTO `medicines` (`MedicineID`, `MedicineCode`, `MedicineName`, `GenericName`, `CategoryID`, `Manufacturer`, `UnitOfMeasure`, `UnitPrice`, `SellingPrice`, `ReorderLevel`, `IsActive`) VALUES (15, 'MEDSD006', 'Seed Medicine 06', 'Generic Compound 06', 15, 'Seed Pharma 6', 'Vial', 30.00, 48.00, 16, 1);
INSERT INTO `medicines` (`MedicineID`, `MedicineCode`, `MedicineName`, `GenericName`, `CategoryID`, `Manufacturer`, `UnitOfMeasure`, `UnitPrice`, `SellingPrice`, `ReorderLevel`, `IsActive`) VALUES (16, 'MEDSD007', 'Seed Medicine 07', 'Generic Compound 07', 16, 'Seed Pharma 1', 'Tablet', 32.00, 51.00, 17, 1);
INSERT INTO `medicines` (`MedicineID`, `MedicineCode`, `MedicineName`, `GenericName`, `CategoryID`, `Manufacturer`, `UnitOfMeasure`, `UnitPrice`, `SellingPrice`, `ReorderLevel`, `IsActive`) VALUES (17, 'MEDSD008', 'Seed Medicine 08', 'Generic Compound 08', 17, 'Seed Pharma 2', 'Capsule', 34.00, 54.00, 18, 1);
INSERT INTO `medicines` (`MedicineID`, `MedicineCode`, `MedicineName`, `GenericName`, `CategoryID`, `Manufacturer`, `UnitOfMeasure`, `UnitPrice`, `SellingPrice`, `ReorderLevel`, `IsActive`) VALUES (18, 'MEDSD009', 'Seed Medicine 09', 'Generic Compound 09', 18, 'Seed Pharma 3', 'Vial', 36.00, 57.00, 19, 1);
INSERT INTO `medicines` (`MedicineID`, `MedicineCode`, `MedicineName`, `GenericName`, `CategoryID`, `Manufacturer`, `UnitOfMeasure`, `UnitPrice`, `SellingPrice`, `ReorderLevel`, `IsActive`) VALUES (19, 'MEDSD010', 'Seed Medicine 10', 'Generic Compound 10', 19, 'Seed Pharma 4', 'Capsule', 38.00, 60.00, 20, 1);
INSERT INTO `medicines` (`MedicineID`, `MedicineCode`, `MedicineName`, `GenericName`, `CategoryID`, `Manufacturer`, `UnitOfMeasure`, `UnitPrice`, `SellingPrice`, `ReorderLevel`, `IsActive`) VALUES (20, 'MEDSD011', 'Seed Medicine 11', 'Generic Compound 11', 20, 'Seed Pharma 5', 'Tablet', 40.00, 63.00, 21, 1);
INSERT INTO `medicines` (`MedicineID`, `MedicineCode`, `MedicineName`, `GenericName`, `CategoryID`, `Manufacturer`, `UnitOfMeasure`, `UnitPrice`, `SellingPrice`, `ReorderLevel`, `IsActive`) VALUES (21, 'MEDSD012', 'Seed Medicine 12', 'Generic Compound 12', 21, 'Seed Pharma 6', 'Vial', 42.00, 66.00, 22, 1);
INSERT INTO `medicines` (`MedicineID`, `MedicineCode`, `MedicineName`, `GenericName`, `CategoryID`, `Manufacturer`, `UnitOfMeasure`, `UnitPrice`, `SellingPrice`, `ReorderLevel`, `IsActive`) VALUES (22, 'MEDSD013', 'Seed Medicine 13', 'Generic Compound 13', 22, 'Seed Pharma 1', 'Tablet', 44.00, 69.00, 23, 1);
INSERT INTO `medicines` (`MedicineID`, `MedicineCode`, `MedicineName`, `GenericName`, `CategoryID`, `Manufacturer`, `UnitOfMeasure`, `UnitPrice`, `SellingPrice`, `ReorderLevel`, `IsActive`) VALUES (23, 'MEDSD014', 'Seed Medicine 14', 'Generic Compound 14', 23, 'Seed Pharma 2', 'Capsule', 46.00, 72.00, 24, 1);
INSERT INTO `medicines` (`MedicineID`, `MedicineCode`, `MedicineName`, `GenericName`, `CategoryID`, `Manufacturer`, `UnitOfMeasure`, `UnitPrice`, `SellingPrice`, `ReorderLevel`, `IsActive`) VALUES (24, 'MEDSD015', 'Seed Medicine 15', 'Generic Compound 15', 24, 'Seed Pharma 3', 'Vial', 48.00, 75.00, 25, 1);
INSERT INTO `medicines` (`MedicineID`, `MedicineCode`, `MedicineName`, `GenericName`, `CategoryID`, `Manufacturer`, `UnitOfMeasure`, `UnitPrice`, `SellingPrice`, `ReorderLevel`, `IsActive`) VALUES (25, 'MEDSD016', 'Seed Medicine 16', 'Generic Compound 16', 25, 'Seed Pharma 4', 'Capsule', 50.00, 78.00, 26, 1);
INSERT INTO `medicines` (`MedicineID`, `MedicineCode`, `MedicineName`, `GenericName`, `CategoryID`, `Manufacturer`, `UnitOfMeasure`, `UnitPrice`, `SellingPrice`, `ReorderLevel`, `IsActive`) VALUES (26, 'MEDSD017', 'Seed Medicine 17', 'Generic Compound 17', 26, 'Seed Pharma 5', 'Tablet', 52.00, 81.00, 27, 1);
INSERT INTO `medicines` (`MedicineID`, `MedicineCode`, `MedicineName`, `GenericName`, `CategoryID`, `Manufacturer`, `UnitOfMeasure`, `UnitPrice`, `SellingPrice`, `ReorderLevel`, `IsActive`) VALUES (27, 'MEDSD018', 'Seed Medicine 18', 'Generic Compound 18', 27, 'Seed Pharma 6', 'Vial', 54.00, 84.00, 28, 1);
INSERT INTO `medicines` (`MedicineID`, `MedicineCode`, `MedicineName`, `GenericName`, `CategoryID`, `Manufacturer`, `UnitOfMeasure`, `UnitPrice`, `SellingPrice`, `ReorderLevel`, `IsActive`) VALUES (28, 'MEDSD019', 'Seed Medicine 19', 'Generic Compound 19', 28, 'Seed Pharma 1', 'Tablet', 56.00, 87.00, 29, 1);
INSERT INTO `medicines` (`MedicineID`, `MedicineCode`, `MedicineName`, `GenericName`, `CategoryID`, `Manufacturer`, `UnitOfMeasure`, `UnitPrice`, `SellingPrice`, `ReorderLevel`, `IsActive`) VALUES (29, 'MEDSD020', 'Seed Medicine 20', 'Generic Compound 20', 29, 'Seed Pharma 2', 'Capsule', 58.00, 90.00, 30, 1);
INSERT INTO `medicines` (`MedicineID`, `MedicineCode`, `MedicineName`, `GenericName`, `CategoryID`, `Manufacturer`, `UnitOfMeasure`, `UnitPrice`, `SellingPrice`, `ReorderLevel`, `IsActive`) VALUES (30, 'MEDSD021', 'Seed Medicine 21', 'Generic Compound 21', 30, 'Seed Pharma 3', 'Vial', 60.00, 93.00, 31, 1);
INSERT INTO `medicines` (`MedicineID`, `MedicineCode`, `MedicineName`, `GenericName`, `CategoryID`, `Manufacturer`, `UnitOfMeasure`, `UnitPrice`, `SellingPrice`, `ReorderLevel`, `IsActive`) VALUES (31, 'MEDSD022', 'Seed Medicine 22', 'Generic Compound 22', 31, 'Seed Pharma 4', 'Capsule', 62.00, 96.00, 32, 1);
INSERT INTO `medicines` (`MedicineID`, `MedicineCode`, `MedicineName`, `GenericName`, `CategoryID`, `Manufacturer`, `UnitOfMeasure`, `UnitPrice`, `SellingPrice`, `ReorderLevel`, `IsActive`) VALUES (32, 'MEDSD023', 'Seed Medicine 23', 'Generic Compound 23', 32, 'Seed Pharma 5', 'Tablet', 64.00, 99.00, 33, 1);
INSERT INTO `medicines` (`MedicineID`, `MedicineCode`, `MedicineName`, `GenericName`, `CategoryID`, `Manufacturer`, `UnitOfMeasure`, `UnitPrice`, `SellingPrice`, `ReorderLevel`, `IsActive`) VALUES (33, 'MEDSD024', 'Seed Medicine 24', 'Generic Compound 24', 33, 'Seed Pharma 6', 'Vial', 66.00, 102.00, 34, 1);
INSERT INTO `medicines` (`MedicineID`, `MedicineCode`, `MedicineName`, `GenericName`, `CategoryID`, `Manufacturer`, `UnitOfMeasure`, `UnitPrice`, `SellingPrice`, `ReorderLevel`, `IsActive`) VALUES (34, 'MEDSD025', 'Seed Medicine 25', 'Generic Compound 25', 34, 'Seed Pharma 1', 'Tablet', 68.00, 105.00, 35, 1);
INSERT INTO `medicines` (`MedicineID`, `MedicineCode`, `MedicineName`, `GenericName`, `CategoryID`, `Manufacturer`, `UnitOfMeasure`, `UnitPrice`, `SellingPrice`, `ReorderLevel`, `IsActive`) VALUES (35, 'MEDSD026', 'Seed Medicine 26', 'Generic Compound 26', 35, 'Seed Pharma 2', 'Capsule', 70.00, 108.00, 36, 1);
INSERT INTO `medicines` (`MedicineID`, `MedicineCode`, `MedicineName`, `GenericName`, `CategoryID`, `Manufacturer`, `UnitOfMeasure`, `UnitPrice`, `SellingPrice`, `ReorderLevel`, `IsActive`) VALUES (36, 'MEDSD027', 'Seed Medicine 27', 'Generic Compound 27', 36, 'Seed Pharma 3', 'Vial', 72.00, 111.00, 37, 1);
INSERT INTO `medicines` (`MedicineID`, `MedicineCode`, `MedicineName`, `GenericName`, `CategoryID`, `Manufacturer`, `UnitOfMeasure`, `UnitPrice`, `SellingPrice`, `ReorderLevel`, `IsActive`) VALUES (37, 'MEDSD028', 'Seed Medicine 28', 'Generic Compound 28', 37, 'Seed Pharma 4', 'Capsule', 74.00, 114.00, 38, 1);
INSERT INTO `medicines` (`MedicineID`, `MedicineCode`, `MedicineName`, `GenericName`, `CategoryID`, `Manufacturer`, `UnitOfMeasure`, `UnitPrice`, `SellingPrice`, `ReorderLevel`, `IsActive`) VALUES (38, 'MEDSD029', 'Seed Medicine 29', 'Generic Compound 29', 38, 'Seed Pharma 5', 'Tablet', 76.00, 117.00, 39, 1);
INSERT INTO `medicines` (`MedicineID`, `MedicineCode`, `MedicineName`, `GenericName`, `CategoryID`, `Manufacturer`, `UnitOfMeasure`, `UnitPrice`, `SellingPrice`, `ReorderLevel`, `IsActive`) VALUES (39, 'MEDSD030', 'Seed Medicine 30', 'Generic Compound 30', 39, 'Seed Pharma 6', 'Vial', 78.00, 120.00, 40, 1);

-- Data for notifications
INSERT INTO `notifications` (`NotificationID`, `UserID`, `Title`, `Message`, `NotificationType`, `IsRead`, `CreatedDate`, `ExpiryDate`) VALUES (2, 6, 'Seed Notification 01', 'Seeded system notification message 01.', 'System', 0, '2026-02-28 09:00:00.000000', '2026-04-09 00:00:00.000000');
INSERT INTO `notifications` (`NotificationID`, `UserID`, `Title`, `Message`, `NotificationType`, `IsRead`, `CreatedDate`, `ExpiryDate`) VALUES (3, 7, 'Seed Notification 02', 'Seeded system notification message 02.', 'System', 1, '2026-03-01 10:00:00.000000', '2026-04-10 00:00:00.000000');
INSERT INTO `notifications` (`NotificationID`, `UserID`, `Title`, `Message`, `NotificationType`, `IsRead`, `CreatedDate`, `ExpiryDate`) VALUES (4, 8, 'Seed Notification 03', 'Seeded system notification message 03.', 'System', 0, '2026-03-02 11:00:00.000000', '2026-04-11 00:00:00.000000');
INSERT INTO `notifications` (`NotificationID`, `UserID`, `Title`, `Message`, `NotificationType`, `IsRead`, `CreatedDate`, `ExpiryDate`) VALUES (5, 9, 'Seed Notification 04', 'Seeded system notification message 04.', 'System', 1, '2026-03-03 12:00:00.000000', '2026-04-12 00:00:00.000000');
INSERT INTO `notifications` (`NotificationID`, `UserID`, `Title`, `Message`, `NotificationType`, `IsRead`, `CreatedDate`, `ExpiryDate`) VALUES (6, 10, 'Seed Notification 05', 'Seeded system notification message 05.', 'System', 0, '2026-03-04 13:00:00.000000', '2026-04-13 00:00:00.000000');
INSERT INTO `notifications` (`NotificationID`, `UserID`, `Title`, `Message`, `NotificationType`, `IsRead`, `CreatedDate`, `ExpiryDate`) VALUES (7, 11, 'Seed Notification 06', 'Seeded system notification message 06.', 'System', 1, '2026-03-05 08:00:00.000000', '2026-04-14 00:00:00.000000');
INSERT INTO `notifications` (`NotificationID`, `UserID`, `Title`, `Message`, `NotificationType`, `IsRead`, `CreatedDate`, `ExpiryDate`) VALUES (8, 12, 'Seed Notification 07', 'Seeded system notification message 07.', 'System', 0, '2026-03-06 09:00:00.000000', '2026-04-15 00:00:00.000000');
INSERT INTO `notifications` (`NotificationID`, `UserID`, `Title`, `Message`, `NotificationType`, `IsRead`, `CreatedDate`, `ExpiryDate`) VALUES (9, 13, 'Seed Notification 08', 'Seeded system notification message 08.', 'System', 1, '2026-03-07 10:00:00.000000', '2026-04-16 00:00:00.000000');
INSERT INTO `notifications` (`NotificationID`, `UserID`, `Title`, `Message`, `NotificationType`, `IsRead`, `CreatedDate`, `ExpiryDate`) VALUES (10, 14, 'Seed Notification 09', 'Seeded system notification message 09.', 'System', 0, '2026-03-08 11:00:00.000000', '2026-04-17 00:00:00.000000');
INSERT INTO `notifications` (`NotificationID`, `UserID`, `Title`, `Message`, `NotificationType`, `IsRead`, `CreatedDate`, `ExpiryDate`) VALUES (11, 15, 'Seed Notification 10', 'Seeded system notification message 10.', 'System', 1, '2026-02-27 12:00:00.000000', '2026-04-18 00:00:00.000000');
INSERT INTO `notifications` (`NotificationID`, `UserID`, `Title`, `Message`, `NotificationType`, `IsRead`, `CreatedDate`, `ExpiryDate`) VALUES (12, 16, 'Seed Notification 11', 'Seeded system notification message 11.', 'System', 0, '2026-02-28 13:00:00.000000', '2026-04-19 00:00:00.000000');
INSERT INTO `notifications` (`NotificationID`, `UserID`, `Title`, `Message`, `NotificationType`, `IsRead`, `CreatedDate`, `ExpiryDate`) VALUES (13, 17, 'Seed Notification 12', 'Seeded system notification message 12.', 'System', 1, '2026-03-01 08:00:00.000000', '2026-04-20 00:00:00.000000');
INSERT INTO `notifications` (`NotificationID`, `UserID`, `Title`, `Message`, `NotificationType`, `IsRead`, `CreatedDate`, `ExpiryDate`) VALUES (14, 18, 'Seed Notification 13', 'Seeded system notification message 13.', 'System', 0, '2026-03-02 09:00:00.000000', '2026-04-21 00:00:00.000000');
INSERT INTO `notifications` (`NotificationID`, `UserID`, `Title`, `Message`, `NotificationType`, `IsRead`, `CreatedDate`, `ExpiryDate`) VALUES (15, 19, 'Seed Notification 14', 'Seeded system notification message 14.', 'System', 1, '2026-03-03 10:00:00.000000', '2026-04-22 00:00:00.000000');
INSERT INTO `notifications` (`NotificationID`, `UserID`, `Title`, `Message`, `NotificationType`, `IsRead`, `CreatedDate`, `ExpiryDate`) VALUES (16, 20, 'Seed Notification 15', 'Seeded system notification message 15.', 'System', 0, '2026-03-04 11:00:00.000000', '2026-04-23 00:00:00.000000');
INSERT INTO `notifications` (`NotificationID`, `UserID`, `Title`, `Message`, `NotificationType`, `IsRead`, `CreatedDate`, `ExpiryDate`) VALUES (17, 21, 'Seed Notification 16', 'Seeded system notification message 16.', 'System', 1, '2026-03-05 12:00:00.000000', '2026-04-24 00:00:00.000000');
INSERT INTO `notifications` (`NotificationID`, `UserID`, `Title`, `Message`, `NotificationType`, `IsRead`, `CreatedDate`, `ExpiryDate`) VALUES (18, 22, 'Seed Notification 17', 'Seeded system notification message 17.', 'System', 0, '2026-03-06 13:00:00.000000', '2026-04-25 00:00:00.000000');
INSERT INTO `notifications` (`NotificationID`, `UserID`, `Title`, `Message`, `NotificationType`, `IsRead`, `CreatedDate`, `ExpiryDate`) VALUES (19, 23, 'Seed Notification 18', 'Seeded system notification message 18.', 'System', 1, '2026-03-07 08:00:00.000000', '2026-04-26 00:00:00.000000');
INSERT INTO `notifications` (`NotificationID`, `UserID`, `Title`, `Message`, `NotificationType`, `IsRead`, `CreatedDate`, `ExpiryDate`) VALUES (20, 24, 'Seed Notification 19', 'Seeded system notification message 19.', 'System', 0, '2026-03-08 09:00:00.000000', '2026-04-27 00:00:00.000000');
INSERT INTO `notifications` (`NotificationID`, `UserID`, `Title`, `Message`, `NotificationType`, `IsRead`, `CreatedDate`, `ExpiryDate`) VALUES (21, 25, 'Seed Notification 20', 'Seeded system notification message 20.', 'System', 1, '2026-02-27 10:00:00.000000', '2026-04-28 00:00:00.000000');
INSERT INTO `notifications` (`NotificationID`, `UserID`, `Title`, `Message`, `NotificationType`, `IsRead`, `CreatedDate`, `ExpiryDate`) VALUES (22, 26, 'Seed Notification 21', 'Seeded system notification message 21.', 'System', 0, '2026-02-28 11:00:00.000000', '2026-04-29 00:00:00.000000');
INSERT INTO `notifications` (`NotificationID`, `UserID`, `Title`, `Message`, `NotificationType`, `IsRead`, `CreatedDate`, `ExpiryDate`) VALUES (23, 27, 'Seed Notification 22', 'Seeded system notification message 22.', 'System', 1, '2026-03-01 12:00:00.000000', '2026-04-30 00:00:00.000000');
INSERT INTO `notifications` (`NotificationID`, `UserID`, `Title`, `Message`, `NotificationType`, `IsRead`, `CreatedDate`, `ExpiryDate`) VALUES (24, 28, 'Seed Notification 23', 'Seeded system notification message 23.', 'System', 0, '2026-03-02 13:00:00.000000', '2026-05-01 00:00:00.000000');
INSERT INTO `notifications` (`NotificationID`, `UserID`, `Title`, `Message`, `NotificationType`, `IsRead`, `CreatedDate`, `ExpiryDate`) VALUES (25, 29, 'Seed Notification 24', 'Seeded system notification message 24.', 'System', 1, '2026-03-03 08:00:00.000000', '2026-05-02 00:00:00.000000');
INSERT INTO `notifications` (`NotificationID`, `UserID`, `Title`, `Message`, `NotificationType`, `IsRead`, `CreatedDate`, `ExpiryDate`) VALUES (26, 30, 'Seed Notification 25', 'Seeded system notification message 25.', 'System', 0, '2026-03-04 09:00:00.000000', '2026-05-03 00:00:00.000000');
INSERT INTO `notifications` (`NotificationID`, `UserID`, `Title`, `Message`, `NotificationType`, `IsRead`, `CreatedDate`, `ExpiryDate`) VALUES (27, 31, 'Seed Notification 26', 'Seeded system notification message 26.', 'System', 1, '2026-03-05 10:00:00.000000', '2026-05-04 00:00:00.000000');
INSERT INTO `notifications` (`NotificationID`, `UserID`, `Title`, `Message`, `NotificationType`, `IsRead`, `CreatedDate`, `ExpiryDate`) VALUES (28, 32, 'Seed Notification 27', 'Seeded system notification message 27.', 'System', 0, '2026-03-06 11:00:00.000000', '2026-05-05 00:00:00.000000');
INSERT INTO `notifications` (`NotificationID`, `UserID`, `Title`, `Message`, `NotificationType`, `IsRead`, `CreatedDate`, `ExpiryDate`) VALUES (29, 33, 'Seed Notification 28', 'Seeded system notification message 28.', 'System', 1, '2026-03-07 12:00:00.000000', '2026-05-06 00:00:00.000000');
INSERT INTO `notifications` (`NotificationID`, `UserID`, `Title`, `Message`, `NotificationType`, `IsRead`, `CreatedDate`, `ExpiryDate`) VALUES (30, 34, 'Seed Notification 29', 'Seeded system notification message 29.', 'System', 0, '2026-03-08 13:00:00.000000', '2026-05-07 00:00:00.000000');
INSERT INTO `notifications` (`NotificationID`, `UserID`, `Title`, `Message`, `NotificationType`, `IsRead`, `CreatedDate`, `ExpiryDate`) VALUES (31, 35, 'Seed Notification 30', 'Seeded system notification message 30.', 'System', 1, '2026-02-27 08:00:00.000000', '2026-05-08 00:00:00.000000');

-- Data for patientcontacts
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (2, 2, 'Phone', '09500154321', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (3, 2, 'Email', 'patient001@mail.local', 0);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (4, 2, 'Address', 'Lot 1, Seed Street, Davao City', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (5, 3, 'Phone', '09500308642', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (6, 3, 'Email', 'patient002@mail.local', 0);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (7, 3, 'Address', 'Lot 2, Seed Street, Davao City', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (8, 4, 'Phone', '09500462963', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (9, 4, 'Email', 'patient003@mail.local', 0);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (10, 4, 'Address', 'Lot 3, Seed Street, Davao City', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (11, 5, 'Phone', '09500617284', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (12, 5, 'Email', 'patient004@mail.local', 0);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (13, 5, 'Address', 'Lot 4, Seed Street, Davao City', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (14, 6, 'Phone', '09500771605', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (15, 6, 'Email', 'patient005@mail.local', 0);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (16, 6, 'Address', 'Lot 5, Seed Street, Davao City', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (17, 7, 'Phone', '09500925926', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (18, 7, 'Email', 'patient006@mail.local', 0);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (19, 7, 'Address', 'Lot 6, Seed Street, Davao City', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (20, 8, 'Phone', '09501080247', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (21, 8, 'Email', 'patient007@mail.local', 0);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (22, 8, 'Address', 'Lot 7, Seed Street, Davao City', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (23, 9, 'Phone', '09501234568', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (24, 9, 'Email', 'patient008@mail.local', 0);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (25, 9, 'Address', 'Lot 8, Seed Street, Davao City', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (26, 10, 'Phone', '09501388889', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (27, 10, 'Email', 'patient009@mail.local', 0);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (28, 10, 'Address', 'Lot 9, Seed Street, Davao City', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (29, 11, 'Phone', '09501543210', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (30, 11, 'Email', 'patient010@mail.local', 0);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (31, 11, 'Address', 'Lot 10, Seed Street, Davao City', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (32, 12, 'Phone', '09501697531', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (33, 12, 'Email', 'patient011@mail.local', 0);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (34, 12, 'Address', 'Lot 11, Seed Street, Davao City', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (35, 13, 'Phone', '09501851852', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (36, 13, 'Email', 'patient012@mail.local', 0);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (37, 13, 'Address', 'Lot 12, Seed Street, Davao City', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (38, 14, 'Phone', '09502006173', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (39, 14, 'Email', 'patient013@mail.local', 0);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (40, 14, 'Address', 'Lot 13, Seed Street, Davao City', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (41, 15, 'Phone', '09502160494', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (42, 15, 'Email', 'patient014@mail.local', 0);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (43, 15, 'Address', 'Lot 14, Seed Street, Davao City', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (44, 16, 'Phone', '09502314815', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (45, 16, 'Email', 'patient015@mail.local', 0);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (46, 16, 'Address', 'Lot 15, Seed Street, Davao City', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (47, 17, 'Phone', '09502469136', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (48, 17, 'Email', 'patient016@mail.local', 0);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (49, 17, 'Address', 'Lot 16, Seed Street, Davao City', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (50, 18, 'Phone', '09502623457', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (51, 18, 'Email', 'patient017@mail.local', 0);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (52, 18, 'Address', 'Lot 17, Seed Street, Davao City', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (53, 19, 'Phone', '09502777778', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (54, 19, 'Email', 'patient018@mail.local', 0);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (55, 19, 'Address', 'Lot 18, Seed Street, Davao City', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (56, 20, 'Phone', '09502932099', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (57, 20, 'Email', 'patient019@mail.local', 0);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (58, 20, 'Address', 'Lot 19, Seed Street, Davao City', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (59, 21, 'Phone', '09503086420', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (60, 21, 'Email', 'patient020@mail.local', 0);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (61, 21, 'Address', 'Lot 20, Seed Street, Davao City', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (62, 22, 'Phone', '09503240741', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (63, 22, 'Email', 'patient021@mail.local', 0);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (64, 22, 'Address', 'Lot 21, Seed Street, Davao City', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (65, 23, 'Phone', '09503395062', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (66, 23, 'Email', 'patient022@mail.local', 0);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (67, 23, 'Address', 'Lot 22, Seed Street, Davao City', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (68, 24, 'Phone', '09503549383', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (69, 24, 'Email', 'patient023@mail.local', 0);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (70, 24, 'Address', 'Lot 23, Seed Street, Davao City', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (71, 25, 'Phone', '09503703704', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (72, 25, 'Email', 'patient024@mail.local', 0);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (73, 25, 'Address', 'Lot 24, Seed Street, Davao City', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (74, 26, 'Phone', '09503858025', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (75, 26, 'Email', 'patient025@mail.local', 0);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (76, 26, 'Address', 'Lot 25, Seed Street, Davao City', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (77, 27, 'Phone', '09504012346', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (78, 27, 'Email', 'patient026@mail.local', 0);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (79, 27, 'Address', 'Lot 26, Seed Street, Davao City', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (80, 28, 'Phone', '09504166667', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (81, 28, 'Email', 'patient027@mail.local', 0);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (82, 28, 'Address', 'Lot 27, Seed Street, Davao City', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (83, 29, 'Phone', '09504320988', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (84, 29, 'Email', 'patient028@mail.local', 0);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (85, 29, 'Address', 'Lot 28, Seed Street, Davao City', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (86, 30, 'Phone', '09504475309', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (87, 30, 'Email', 'patient029@mail.local', 0);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (88, 30, 'Address', 'Lot 29, Seed Street, Davao City', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (89, 31, 'Phone', '09504629630', 1);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (90, 31, 'Email', 'patient030@mail.local', 0);
INSERT INTO `patientcontacts` (`ContactID`, `PatientID`, `ContactType`, `ContactValue`, `IsPrimary`) VALUES (91, 31, 'Address', 'Lot 30, Seed Street, Davao City', 1);

-- Data for patients
INSERT INTO `patients` (`PatientID`, `PatientCode`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `BloodGroup`, `MaritalStatus`, `Nationality`, `IdentificationType`, `IdentificationNumber`, `RegistrationDate`, `IsActive`, `ProfileImage`) VALUES (2, 'PATSD001', 'Andrei', 'Dela Cruz', '2007-03-29 00:00:00.000000', 'M', 'A+', 'Single', 'Filipino', 'Passport', 'ID-SD-0001', '2025-11-10 09:00:00.000000', 1, NULL);
INSERT INTO `patients` (`PatientID`, `PatientCode`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `BloodGroup`, `MaritalStatus`, `Nationality`, `IdentificationType`, `IdentificationNumber`, `RegistrationDate`, `IsActive`, `ProfileImage`) VALUES (3, 'PATSD002', 'Paolo', 'Santos', '2006-04-18 00:00:00.000000', 'F', 'A-', 'Single', 'Filipino', 'PhilSys', 'ID-SD-0002', '2025-11-11 10:00:00.000000', 1, NULL);
INSERT INTO `patients` (`PatientID`, `PatientCode`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `BloodGroup`, `MaritalStatus`, `Nationality`, `IdentificationType`, `IdentificationNumber`, `RegistrationDate`, `IsActive`, `ProfileImage`) VALUES (4, 'PATSD003', 'Miguel', 'Reyes', '2005-05-08 00:00:00.000000', 'M', 'B+', 'Married', 'Filipino', 'Passport', 'ID-SD-0003', '2025-11-12 11:00:00.000000', 1, NULL);
INSERT INTO `patients` (`PatientID`, `PatientCode`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `BloodGroup`, `MaritalStatus`, `Nationality`, `IdentificationType`, `IdentificationNumber`, `RegistrationDate`, `IsActive`, `ProfileImage`) VALUES (5, 'PATSD004', 'Jericho', 'Bautista', '2004-05-28 00:00:00.000000', 'F', 'B-', 'Single', 'Filipino', 'PhilSys', 'ID-SD-0004', '2025-11-13 12:00:00.000000', 1, NULL);
INSERT INTO `patients` (`PatientID`, `PatientCode`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `BloodGroup`, `MaritalStatus`, `Nationality`, `IdentificationType`, `IdentificationNumber`, `RegistrationDate`, `IsActive`, `ProfileImage`) VALUES (6, 'PATSD005', 'Rafael', 'Garcia', '2003-06-17 00:00:00.000000', 'M', 'AB+', 'Widowed', 'Filipino', 'Passport', 'ID-SD-0005', '2025-11-14 13:00:00.000000', 1, NULL);
INSERT INTO `patients` (`PatientID`, `PatientCode`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `BloodGroup`, `MaritalStatus`, `Nationality`, `IdentificationType`, `IdentificationNumber`, `RegistrationDate`, `IsActive`, `ProfileImage`) VALUES (7, 'PATSD006', 'Bryan', 'Mendoza', '2002-07-07 00:00:00.000000', 'F', 'AB-', 'Married', 'Filipino', 'PhilSys', 'ID-SD-0006', '2025-11-15 14:00:00.000000', 1, NULL);
INSERT INTO `patients` (`PatientID`, `PatientCode`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `BloodGroup`, `MaritalStatus`, `Nationality`, `IdentificationType`, `IdentificationNumber`, `RegistrationDate`, `IsActive`, `ProfileImage`) VALUES (8, 'PATSD007', 'Carlo', 'Torres', '2001-07-27 00:00:00.000000', 'M', 'O+', 'Single', 'Filipino', 'Passport', 'ID-SD-0007', '2025-11-16 08:00:00.000000', 1, NULL);
INSERT INTO `patients` (`PatientID`, `PatientCode`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `BloodGroup`, `MaritalStatus`, `Nationality`, `IdentificationType`, `IdentificationNumber`, `RegistrationDate`, `IsActive`, `ProfileImage`) VALUES (9, 'PATSD008', 'Nathaniel', 'Ramos', '2000-08-16 00:00:00.000000', 'F', 'O-', 'Single', 'Filipino', 'PhilSys', 'ID-SD-0008', '2025-11-17 09:00:00.000000', 1, NULL);
INSERT INTO `patients` (`PatientID`, `PatientCode`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `BloodGroup`, `MaritalStatus`, `Nationality`, `IdentificationType`, `IdentificationNumber`, `RegistrationDate`, `IsActive`, `ProfileImage`) VALUES (10, 'PATSD009', 'Joshua', 'Flores', '1999-09-05 00:00:00.000000', 'M', 'A+', 'Married', 'Filipino', 'Passport', 'ID-SD-0009', '2025-11-18 10:00:00.000000', 1, NULL);
INSERT INTO `patients` (`PatientID`, `PatientCode`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `BloodGroup`, `MaritalStatus`, `Nationality`, `IdentificationType`, `IdentificationNumber`, `RegistrationDate`, `IsActive`, `ProfileImage`) VALUES (11, 'PATSD010', 'Kevin', 'Gonzales', '1998-09-25 00:00:00.000000', 'F', 'A-', 'Widowed', 'Filipino', 'PhilSys', 'ID-SD-0010', '2025-11-19 11:00:00.000000', 1, NULL);
INSERT INTO `patients` (`PatientID`, `PatientCode`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `BloodGroup`, `MaritalStatus`, `Nationality`, `IdentificationType`, `IdentificationNumber`, `RegistrationDate`, `IsActive`, `ProfileImage`) VALUES (12, 'PATSD011', 'Alyssa', 'Fernandez', '1997-10-15 00:00:00.000000', 'M', 'B+', 'Single', 'Filipino', 'Passport', 'ID-SD-0011', '2025-11-20 12:00:00.000000', 1, NULL);
INSERT INTO `patients` (`PatientID`, `PatientCode`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `BloodGroup`, `MaritalStatus`, `Nationality`, `IdentificationType`, `IdentificationNumber`, `RegistrationDate`, `IsActive`, `ProfileImage`) VALUES (13, 'PATSD012', 'Katrina', 'Navarro', '1996-11-04 00:00:00.000000', 'F', 'B-', 'Married', 'Filipino', 'PhilSys', 'ID-SD-0012', '2025-11-21 13:00:00.000000', 1, NULL);
INSERT INTO `patients` (`PatientID`, `PatientCode`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `BloodGroup`, `MaritalStatus`, `Nationality`, `IdentificationType`, `IdentificationNumber`, `RegistrationDate`, `IsActive`, `ProfileImage`) VALUES (14, 'PATSD013', 'Janelle', 'Villanueva', '1995-11-24 00:00:00.000000', 'M', 'AB+', 'Single', 'Filipino', 'Passport', 'ID-SD-0013', '2025-11-22 14:00:00.000000', 1, NULL);
INSERT INTO `patients` (`PatientID`, `PatientCode`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `BloodGroup`, `MaritalStatus`, `Nationality`, `IdentificationType`, `IdentificationNumber`, `RegistrationDate`, `IsActive`, `ProfileImage`) VALUES (15, 'PATSD014', 'Camille', 'Aguilar', '1994-12-14 00:00:00.000000', 'F', 'AB-', 'Single', 'Filipino', 'PhilSys', 'ID-SD-0014', '2025-11-23 08:00:00.000000', 1, NULL);
INSERT INTO `patients` (`PatientID`, `PatientCode`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `BloodGroup`, `MaritalStatus`, `Nationality`, `IdentificationType`, `IdentificationNumber`, `RegistrationDate`, `IsActive`, `ProfileImage`) VALUES (16, 'PATSD015', 'Patricia', 'Castillo', '1994-01-03 00:00:00.000000', 'M', 'O+', 'Married', 'Filipino', 'Passport', 'ID-SD-0015', '2025-11-24 09:00:00.000000', 1, NULL);
INSERT INTO `patients` (`PatientID`, `PatientCode`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `BloodGroup`, `MaritalStatus`, `Nationality`, `IdentificationType`, `IdentificationNumber`, `RegistrationDate`, `IsActive`, `ProfileImage`) VALUES (17, 'PATSD016', 'Bea', 'Soriano', '1993-01-23 00:00:00.000000', 'F', 'O-', 'Single', 'Filipino', 'PhilSys', 'ID-SD-0016', '2025-11-25 10:00:00.000000', 1, NULL);
INSERT INTO `patients` (`PatientID`, `PatientCode`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `BloodGroup`, `MaritalStatus`, `Nationality`, `IdentificationType`, `IdentificationNumber`, `RegistrationDate`, `IsActive`, `ProfileImage`) VALUES (18, 'PATSD017', 'Angelica', 'Domingo', '1992-02-12 00:00:00.000000', 'M', 'A+', 'Single', 'Filipino', 'Passport', 'ID-SD-0017', '2025-11-26 11:00:00.000000', 1, NULL);
INSERT INTO `patients` (`PatientID`, `PatientCode`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `BloodGroup`, `MaritalStatus`, `Nationality`, `IdentificationType`, `IdentificationNumber`, `RegistrationDate`, `IsActive`, `ProfileImage`) VALUES (19, 'PATSD018', 'Clarisse', 'Aquino', '1991-03-04 00:00:00.000000', 'F', 'A-', 'Married', 'Filipino', 'PhilSys', 'ID-SD-0018', '2025-11-27 12:00:00.000000', 1, NULL);
INSERT INTO `patients` (`PatientID`, `PatientCode`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `BloodGroup`, `MaritalStatus`, `Nationality`, `IdentificationType`, `IdentificationNumber`, `RegistrationDate`, `IsActive`, `ProfileImage`) VALUES (20, 'PATSD019', 'Mica', 'Mercado', '1990-03-24 00:00:00.000000', 'M', 'B+', 'Single', 'Filipino', 'Passport', 'ID-SD-0019', '2025-11-28 13:00:00.000000', 1, NULL);
INSERT INTO `patients` (`PatientID`, `PatientCode`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `BloodGroup`, `MaritalStatus`, `Nationality`, `IdentificationType`, `IdentificationNumber`, `RegistrationDate`, `IsActive`, `ProfileImage`) VALUES (21, 'PATSD020', 'Joyce', 'Salazar', '1989-04-13 00:00:00.000000', 'F', 'B-', 'Widowed', 'Filipino', 'PhilSys', 'ID-SD-0020', '2025-11-29 14:00:00.000000', 1, NULL);
INSERT INTO `patients` (`PatientID`, `PatientCode`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `BloodGroup`, `MaritalStatus`, `Nationality`, `IdentificationType`, `IdentificationNumber`, `RegistrationDate`, `IsActive`, `ProfileImage`) VALUES (22, 'PATSD021', 'Hazel', 'Pascual', '1988-05-02 00:00:00.000000', 'M', 'AB+', 'Married', 'Filipino', 'Passport', 'ID-SD-0021', '2025-11-30 08:00:00.000000', 1, NULL);
INSERT INTO `patients` (`PatientID`, `PatientCode`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `BloodGroup`, `MaritalStatus`, `Nationality`, `IdentificationType`, `IdentificationNumber`, `RegistrationDate`, `IsActive`, `ProfileImage`) VALUES (23, 'PATSD022', 'Janine', 'Valdez', '1987-05-23 00:00:00.000000', 'F', 'AB-', 'Single', 'Filipino', 'PhilSys', 'ID-SD-0022', '2025-12-01 09:00:00.000000', 1, NULL);
INSERT INTO `patients` (`PatientID`, `PatientCode`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `BloodGroup`, `MaritalStatus`, `Nationality`, `IdentificationType`, `IdentificationNumber`, `RegistrationDate`, `IsActive`, `ProfileImage`) VALUES (24, 'PATSD023', 'Bianca', 'Cabrera', '1986-06-12 00:00:00.000000', 'M', 'O+', 'Single', 'Filipino', 'Passport', 'ID-SD-0023', '2025-12-02 10:00:00.000000', 1, NULL);
INSERT INTO `patients` (`PatientID`, `PatientCode`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `BloodGroup`, `MaritalStatus`, `Nationality`, `IdentificationType`, `IdentificationNumber`, `RegistrationDate`, `IsActive`, `ProfileImage`) VALUES (25, 'PATSD024', 'Trisha', 'Padilla', '1985-07-02 00:00:00.000000', 'F', 'O-', 'Married', 'Filipino', 'PhilSys', 'ID-SD-0024', '2025-12-03 11:00:00.000000', 1, NULL);
INSERT INTO `patients` (`PatientID`, `PatientCode`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `BloodGroup`, `MaritalStatus`, `Nationality`, `IdentificationType`, `IdentificationNumber`, `RegistrationDate`, `IsActive`, `ProfileImage`) VALUES (26, 'PATSD025', 'Kaye', 'Lim', '1984-07-21 00:00:00.000000', 'M', 'A+', 'Widowed', 'Filipino', 'Passport', 'ID-SD-0025', '2025-12-04 12:00:00.000000', 1, NULL);
INSERT INTO `patients` (`PatientID`, `PatientCode`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `BloodGroup`, `MaritalStatus`, `Nationality`, `IdentificationType`, `IdentificationNumber`, `RegistrationDate`, `IsActive`, `ProfileImage`) VALUES (27, 'PATSD026', 'Erika', 'Tan', '1983-08-11 00:00:00.000000', 'F', 'A-', 'Single', 'Filipino', 'PhilSys', 'ID-SD-0026', '2025-12-05 13:00:00.000000', 1, NULL);
INSERT INTO `patients` (`PatientID`, `PatientCode`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `BloodGroup`, `MaritalStatus`, `Nationality`, `IdentificationType`, `IdentificationNumber`, `RegistrationDate`, `IsActive`, `ProfileImage`) VALUES (28, 'PATSD027', 'Dianne', 'Abad', '1982-08-31 00:00:00.000000', 'M', 'B+', 'Married', 'Filipino', 'Passport', 'ID-SD-0027', '2025-12-06 14:00:00.000000', 1, NULL);
INSERT INTO `patients` (`PatientID`, `PatientCode`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `BloodGroup`, `MaritalStatus`, `Nationality`, `IdentificationType`, `IdentificationNumber`, `RegistrationDate`, `IsActive`, `ProfileImage`) VALUES (29, 'PATSD028', 'Shaina', 'Rosales', '1981-09-20 00:00:00.000000', 'F', 'B-', 'Single', 'Filipino', 'PhilSys', 'ID-SD-0028', '2025-12-07 08:00:00.000000', 1, NULL);
INSERT INTO `patients` (`PatientID`, `PatientCode`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `BloodGroup`, `MaritalStatus`, `Nationality`, `IdentificationType`, `IdentificationNumber`, `RegistrationDate`, `IsActive`, `ProfileImage`) VALUES (30, 'PATSD029', 'Nica', 'Malabanan', '1980-10-09 00:00:00.000000', 'M', 'AB+', 'Single', 'Filipino', 'Passport', 'ID-SD-0029', '2025-12-08 09:00:00.000000', 1, NULL);
INSERT INTO `patients` (`PatientID`, `PatientCode`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `BloodGroup`, `MaritalStatus`, `Nationality`, `IdentificationType`, `IdentificationNumber`, `RegistrationDate`, `IsActive`, `ProfileImage`) VALUES (31, 'PATSD030', 'Rica', 'Lopez', '1979-10-30 00:00:00.000000', 'F', 'AB-', 'Married', 'Filipino', 'PhilSys', 'ID-SD-0030', '2025-12-09 10:00:00.000000', 1, NULL);

-- Data for payments
INSERT INTO `payments` (`PaymentID`, `PaymentNumber`, `InvoiceID`, `PaymentDate`, `PaymentMethod`, `Amount`, `ReferenceNumber`, `ReceivedBy`, `Notes`) VALUES (2, 'PAYSD001', 2, '2026-02-20 13:00:00.000000', 'Cash', 2139.20, 'REFSD001', 12, 'Seeded payment entry.');
INSERT INTO `payments` (`PaymentID`, `PaymentNumber`, `InvoiceID`, `PaymentDate`, `PaymentMethod`, `Amount`, `ReferenceNumber`, `ReceivedBy`, `Notes`) VALUES (3, 'PAYSD002', 3, '2026-02-21 13:00:00.000000', 'Online', 2262.40, 'REFSD002', 12, 'Seeded payment entry.');
INSERT INTO `payments` (`PaymentID`, `PaymentNumber`, `InvoiceID`, `PaymentDate`, `PaymentMethod`, `Amount`, `ReferenceNumber`, `ReceivedBy`, `Notes`) VALUES (4, 'PAYSD003', 4, '2026-02-22 13:00:00.000000', 'Cash', 2385.60, 'REFSD003', 12, 'Seeded payment entry.');
INSERT INTO `payments` (`PaymentID`, `PaymentNumber`, `InvoiceID`, `PaymentDate`, `PaymentMethod`, `Amount`, `ReferenceNumber`, `ReceivedBy`, `Notes`) VALUES (5, 'PAYSD004', 5, '2026-02-23 13:00:00.000000', 'Online', 2508.80, 'REFSD004', 12, 'Seeded payment entry.');
INSERT INTO `payments` (`PaymentID`, `PaymentNumber`, `InvoiceID`, `PaymentDate`, `PaymentMethod`, `Amount`, `ReferenceNumber`, `ReceivedBy`, `Notes`) VALUES (6, 'PAYSD005', 6, '2026-02-24 13:00:00.000000', 'Cash', 2632.00, 'REFSD005', 12, 'Seeded payment entry.');
INSERT INTO `payments` (`PaymentID`, `PaymentNumber`, `InvoiceID`, `PaymentDate`, `PaymentMethod`, `Amount`, `ReferenceNumber`, `ReceivedBy`, `Notes`) VALUES (7, 'PAYSD006', 7, '2026-02-25 13:00:00.000000', 'Online', 2553.60, 'REFSD006', 12, 'Seeded payment entry.');
INSERT INTO `payments` (`PaymentID`, `PaymentNumber`, `InvoiceID`, `PaymentDate`, `PaymentMethod`, `Amount`, `ReferenceNumber`, `ReceivedBy`, `Notes`) VALUES (8, 'PAYSD007', 8, '2026-02-26 13:00:00.000000', 'Cash', 2878.40, 'REFSD007', 12, 'Seeded payment entry.');
INSERT INTO `payments` (`PaymentID`, `PaymentNumber`, `InvoiceID`, `PaymentDate`, `PaymentMethod`, `Amount`, `ReferenceNumber`, `ReceivedBy`, `Notes`) VALUES (9, 'PAYSD008', 9, '2026-02-27 13:00:00.000000', 'Online', 3001.60, 'REFSD008', 12, 'Seeded payment entry.');
INSERT INTO `payments` (`PaymentID`, `PaymentNumber`, `InvoiceID`, `PaymentDate`, `PaymentMethod`, `Amount`, `ReferenceNumber`, `ReceivedBy`, `Notes`) VALUES (10, 'PAYSD009', 10, '2026-02-28 13:00:00.000000', 'Cash', 3124.80, 'REFSD009', 12, 'Seeded payment entry.');
INSERT INTO `payments` (`PaymentID`, `PaymentNumber`, `InvoiceID`, `PaymentDate`, `PaymentMethod`, `Amount`, `ReferenceNumber`, `ReceivedBy`, `Notes`) VALUES (11, 'PAYSD010', 11, '2026-03-01 13:00:00.000000', 'Online', 3248.00, 'REFSD010', 12, 'Seeded payment entry.');
INSERT INTO `payments` (`PaymentID`, `PaymentNumber`, `InvoiceID`, `PaymentDate`, `PaymentMethod`, `Amount`, `ReferenceNumber`, `ReceivedBy`, `Notes`) VALUES (12, 'PAYSD011', 12, '2026-03-02 13:00:00.000000', 'Cash', 3371.20, 'REFSD011', 12, 'Seeded payment entry.');
INSERT INTO `payments` (`PaymentID`, `PaymentNumber`, `InvoiceID`, `PaymentDate`, `PaymentMethod`, `Amount`, `ReferenceNumber`, `ReceivedBy`, `Notes`) VALUES (13, 'PAYSD012', 13, '2026-03-03 13:00:00.000000', 'Online', 3292.80, 'REFSD012', 12, 'Seeded payment entry.');
INSERT INTO `payments` (`PaymentID`, `PaymentNumber`, `InvoiceID`, `PaymentDate`, `PaymentMethod`, `Amount`, `ReferenceNumber`, `ReceivedBy`, `Notes`) VALUES (14, 'PAYSD013', 14, '2026-03-04 13:00:00.000000', 'Cash', 3617.60, 'REFSD013', 12, 'Seeded payment entry.');
INSERT INTO `payments` (`PaymentID`, `PaymentNumber`, `InvoiceID`, `PaymentDate`, `PaymentMethod`, `Amount`, `ReferenceNumber`, `ReceivedBy`, `Notes`) VALUES (15, 'PAYSD014', 15, '2026-03-05 13:00:00.000000', 'Online', 3740.80, 'REFSD014', 12, 'Seeded payment entry.');
INSERT INTO `payments` (`PaymentID`, `PaymentNumber`, `InvoiceID`, `PaymentDate`, `PaymentMethod`, `Amount`, `ReferenceNumber`, `ReceivedBy`, `Notes`) VALUES (16, 'PAYSD015', 16, '2026-03-06 13:00:00.000000', 'Cash', 3864.00, 'REFSD015', 12, 'Seeded payment entry.');
INSERT INTO `payments` (`PaymentID`, `PaymentNumber`, `InvoiceID`, `PaymentDate`, `PaymentMethod`, `Amount`, `ReferenceNumber`, `ReceivedBy`, `Notes`) VALUES (17, 'PAYSD016', 17, '2026-03-07 13:00:00.000000', 'Online', 2192.96, 'REFSD016', 12, 'Seeded payment entry.');
INSERT INTO `payments` (`PaymentID`, `PaymentNumber`, `InvoiceID`, `PaymentDate`, `PaymentMethod`, `Amount`, `ReferenceNumber`, `ReceivedBy`, `Notes`) VALUES (18, 'PAYSD017', 18, '2026-03-08 13:00:00.000000', 'Cash', 2260.72, 'REFSD017', 12, 'Seeded payment entry.');
INSERT INTO `payments` (`PaymentID`, `PaymentNumber`, `InvoiceID`, `PaymentDate`, `PaymentMethod`, `Amount`, `ReferenceNumber`, `ReceivedBy`, `Notes`) VALUES (19, 'PAYSD018', 19, '2026-03-09 13:00:00.000000', 'Online', 2217.60, 'REFSD018', 12, 'Seeded payment entry.');
INSERT INTO `payments` (`PaymentID`, `PaymentNumber`, `InvoiceID`, `PaymentDate`, `PaymentMethod`, `Amount`, `ReferenceNumber`, `ReceivedBy`, `Notes`) VALUES (20, 'PAYSD019', 20, '2026-03-10 13:00:00.000000', 'Cash', 2396.24, 'REFSD019', 12, 'Seeded payment entry.');
INSERT INTO `payments` (`PaymentID`, `PaymentNumber`, `InvoiceID`, `PaymentDate`, `PaymentMethod`, `Amount`, `ReferenceNumber`, `ReceivedBy`, `Notes`) VALUES (21, 'PAYSD020', 21, '2026-03-11 13:00:00.000000', 'Online', 2464.00, 'REFSD020', 12, 'Seeded payment entry.');
INSERT INTO `payments` (`PaymentID`, `PaymentNumber`, `InvoiceID`, `PaymentDate`, `PaymentMethod`, `Amount`, `ReferenceNumber`, `ReceivedBy`, `Notes`) VALUES (22, 'PAYSD021', 22, '2026-03-12 13:00:00.000000', 'Cash', 2531.76, 'REFSD021', 12, 'Seeded payment entry.');
INSERT INTO `payments` (`PaymentID`, `PaymentNumber`, `InvoiceID`, `PaymentDate`, `PaymentMethod`, `Amount`, `ReferenceNumber`, `ReceivedBy`, `Notes`) VALUES (23, 'PAYSD022', 23, '2026-03-13 13:00:00.000000', 'Online', 2599.52, 'REFSD022', 12, 'Seeded payment entry.');
INSERT INTO `payments` (`PaymentID`, `PaymentNumber`, `InvoiceID`, `PaymentDate`, `PaymentMethod`, `Amount`, `ReferenceNumber`, `ReceivedBy`, `Notes`) VALUES (24, 'PAYSD023', 24, '2026-03-14 13:00:00.000000', 'Cash', 2667.28, 'REFSD023', 12, 'Seeded payment entry.');
INSERT INTO `payments` (`PaymentID`, `PaymentNumber`, `InvoiceID`, `PaymentDate`, `PaymentMethod`, `Amount`, `ReferenceNumber`, `ReceivedBy`, `Notes`) VALUES (25, 'PAYSD024', 25, '2026-03-15 13:00:00.000000', 'Online', 2624.16, 'REFSD024', 12, 'Seeded payment entry.');
INSERT INTO `payments` (`PaymentID`, `PaymentNumber`, `InvoiceID`, `PaymentDate`, `PaymentMethod`, `Amount`, `ReferenceNumber`, `ReceivedBy`, `Notes`) VALUES (26, 'PAYSD025', 26, '2026-03-16 13:00:00.000000', 'Cash', 2802.80, 'REFSD025', 12, 'Seeded payment entry.');
INSERT INTO `payments` (`PaymentID`, `PaymentNumber`, `InvoiceID`, `PaymentDate`, `PaymentMethod`, `Amount`, `ReferenceNumber`, `ReceivedBy`, `Notes`) VALUES (27, 'PAYSD026', 27, '2026-03-17 13:00:00.000000', 'Online', 2870.56, 'REFSD026', 12, 'Seeded payment entry.');
INSERT INTO `payments` (`PaymentID`, `PaymentNumber`, `InvoiceID`, `PaymentDate`, `PaymentMethod`, `Amount`, `ReferenceNumber`, `ReceivedBy`, `Notes`) VALUES (28, 'PAYSD027', 28, '2026-03-18 13:00:00.000000', 'Cash', 2938.32, 'REFSD027', 12, 'Seeded payment entry.');
INSERT INTO `payments` (`PaymentID`, `PaymentNumber`, `InvoiceID`, `PaymentDate`, `PaymentMethod`, `Amount`, `ReferenceNumber`, `ReceivedBy`, `Notes`) VALUES (29, 'PAYSD028', 29, '2026-03-19 13:00:00.000000', 'Online', 3006.08, 'REFSD028', 12, 'Seeded payment entry.');
INSERT INTO `payments` (`PaymentID`, `PaymentNumber`, `InvoiceID`, `PaymentDate`, `PaymentMethod`, `Amount`, `ReferenceNumber`, `ReceivedBy`, `Notes`) VALUES (30, 'PAYSD029', 30, '2026-03-20 13:00:00.000000', 'Cash', 3073.84, 'REFSD029', 12, 'Seeded payment entry.');
INSERT INTO `payments` (`PaymentID`, `PaymentNumber`, `InvoiceID`, `PaymentDate`, `PaymentMethod`, `Amount`, `ReferenceNumber`, `ReceivedBy`, `Notes`) VALUES (31, 'PAYSD030', 31, '2026-03-21 13:00:00.000000', 'Online', 3030.72, 'REFSD030', 12, 'Seeded payment entry.');

-- Data for pharmacysaledetails
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (2, 2, 10, 'PHA-SD-001-A', 3, 47.00, 141.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (3, 2, 15, 'PHA-SD-001-B', 2, 33.50, 67.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (4, 3, 11, 'PHA-SD-002-A', 4, 49.00, 196.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (5, 3, 16, 'PHA-SD-002-B', 3, 35.00, 105.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (6, 4, 12, 'PHA-SD-003-A', 5, 51.00, 255.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (7, 4, 17, 'PHA-SD-003-B', 1, 36.50, 36.50);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (8, 5, 13, 'PHA-SD-004-A', 2, 53.00, 106.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (9, 5, 18, 'PHA-SD-004-B', 2, 38.00, 76.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (10, 6, 14, 'PHA-SD-005-A', 3, 55.00, 165.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (11, 6, 19, 'PHA-SD-005-B', 3, 39.50, 118.50);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (12, 7, 15, 'PHA-SD-006-A', 4, 57.00, 228.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (13, 7, 20, 'PHA-SD-006-B', 1, 41.00, 41.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (14, 8, 16, 'PHA-SD-007-A', 5, 59.00, 295.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (15, 8, 21, 'PHA-SD-007-B', 2, 42.50, 85.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (16, 9, 17, 'PHA-SD-008-A', 2, 61.00, 122.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (17, 9, 22, 'PHA-SD-008-B', 3, 44.00, 132.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (18, 10, 18, 'PHA-SD-009-A', 3, 63.00, 189.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (19, 10, 23, 'PHA-SD-009-B', 1, 45.50, 45.50);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (20, 11, 19, 'PHA-SD-010-A', 4, 65.00, 260.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (21, 11, 24, 'PHA-SD-010-B', 2, 47.00, 94.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (22, 12, 20, 'PHA-SD-011-A', 5, 67.00, 335.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (23, 12, 25, 'PHA-SD-011-B', 3, 48.50, 145.50);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (24, 13, 21, 'PHA-SD-012-A', 2, 69.00, 138.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (25, 13, 26, 'PHA-SD-012-B', 1, 50.00, 50.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (26, 14, 22, 'PHA-SD-013-A', 3, 71.00, 213.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (27, 14, 27, 'PHA-SD-013-B', 2, 51.50, 103.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (28, 15, 23, 'PHA-SD-014-A', 4, 73.00, 292.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (29, 15, 28, 'PHA-SD-014-B', 3, 53.00, 159.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (30, 16, 24, 'PHA-SD-015-A', 5, 75.00, 375.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (31, 16, 29, 'PHA-SD-015-B', 1, 54.50, 54.50);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (32, 17, 25, 'PHA-SD-016-A', 2, 77.00, 154.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (33, 17, 30, 'PHA-SD-016-B', 2, 56.00, 112.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (34, 18, 26, 'PHA-SD-017-A', 3, 79.00, 237.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (35, 18, 31, 'PHA-SD-017-B', 3, 57.50, 172.50);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (36, 19, 27, 'PHA-SD-018-A', 4, 81.00, 324.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (37, 19, 32, 'PHA-SD-018-B', 1, 59.00, 59.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (38, 20, 28, 'PHA-SD-019-A', 5, 83.00, 415.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (39, 20, 33, 'PHA-SD-019-B', 2, 60.50, 121.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (40, 21, 29, 'PHA-SD-020-A', 2, 85.00, 170.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (41, 21, 34, 'PHA-SD-020-B', 3, 62.00, 186.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (42, 22, 30, 'PHA-SD-021-A', 3, 87.00, 261.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (43, 22, 35, 'PHA-SD-021-B', 1, 63.50, 63.50);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (44, 23, 31, 'PHA-SD-022-A', 4, 89.00, 356.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (45, 23, 36, 'PHA-SD-022-B', 2, 65.00, 130.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (46, 24, 32, 'PHA-SD-023-A', 5, 91.00, 455.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (47, 24, 37, 'PHA-SD-023-B', 3, 66.50, 199.50);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (48, 25, 33, 'PHA-SD-024-A', 2, 93.00, 186.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (49, 25, 38, 'PHA-SD-024-B', 1, 68.00, 68.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (50, 26, 34, 'PHA-SD-025-A', 3, 95.00, 285.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (51, 26, 39, 'PHA-SD-025-B', 2, 69.50, 139.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (52, 27, 35, 'PHA-SD-026-A', 4, 97.00, 388.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (53, 27, 10, 'PHA-SD-026-B', 3, 71.00, 213.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (54, 28, 36, 'PHA-SD-027-A', 5, 99.00, 495.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (55, 28, 11, 'PHA-SD-027-B', 1, 72.50, 72.50);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (56, 29, 37, 'PHA-SD-028-A', 2, 101.00, 202.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (57, 29, 12, 'PHA-SD-028-B', 2, 74.00, 148.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (58, 30, 38, 'PHA-SD-029-A', 3, 103.00, 309.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (59, 30, 13, 'PHA-SD-029-B', 3, 75.50, 226.50);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (60, 31, 39, 'PHA-SD-030-A', 4, 105.00, 420.00);
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`, `TotalPrice`) VALUES (61, 31, 14, 'PHA-SD-030-B', 1, 77.00, 77.00);

-- Data for pharmacysales
INSERT INTO `pharmacysales` (`SaleID`, `SaleNumber`, `PatientID`, `SaleDate`, `TotalAmount`, `Discount`, `NetAmount`, `PaymentStatus`, `SoldBy`) VALUES (2, 'SALSD001', 2, '2026-02-26 15:00:00.000000', 208.00, 20.00, 188.00, 'Paid', 10);
INSERT INTO `pharmacysales` (`SaleID`, `SaleNumber`, `PatientID`, `SaleDate`, `TotalAmount`, `Discount`, `NetAmount`, `PaymentStatus`, `SoldBy`) VALUES (3, 'SALSD002', 3, '2026-02-27 15:00:00.000000', 301.00, 20.00, 281.00, 'Paid', 10);
INSERT INTO `pharmacysales` (`SaleID`, `SaleNumber`, `PatientID`, `SaleDate`, `TotalAmount`, `Discount`, `NetAmount`, `PaymentStatus`, `SoldBy`) VALUES (4, 'SALSD003', 4, '2026-02-28 15:00:00.000000', 291.50, 20.00, 271.50, 'Paid', 10);
INSERT INTO `pharmacysales` (`SaleID`, `SaleNumber`, `PatientID`, `SaleDate`, `TotalAmount`, `Discount`, `NetAmount`, `PaymentStatus`, `SoldBy`) VALUES (5, 'SALSD004', 5, '2026-03-01 15:00:00.000000', 182.00, 20.00, 162.00, 'Paid', 10);
INSERT INTO `pharmacysales` (`SaleID`, `SaleNumber`, `PatientID`, `SaleDate`, `TotalAmount`, `Discount`, `NetAmount`, `PaymentStatus`, `SoldBy`) VALUES (6, 'SALSD005', 6, '2026-03-02 15:00:00.000000', 283.50, 20.00, 263.50, 'Paid', 10);
INSERT INTO `pharmacysales` (`SaleID`, `SaleNumber`, `PatientID`, `SaleDate`, `TotalAmount`, `Discount`, `NetAmount`, `PaymentStatus`, `SoldBy`) VALUES (7, 'SALSD006', 7, '2026-03-03 15:00:00.000000', 269.00, 20.00, 249.00, 'Paid', 10);
INSERT INTO `pharmacysales` (`SaleID`, `SaleNumber`, `PatientID`, `SaleDate`, `TotalAmount`, `Discount`, `NetAmount`, `PaymentStatus`, `SoldBy`) VALUES (8, 'SALSD007', 8, '2026-03-04 15:00:00.000000', 380.00, 20.00, 360.00, 'Paid', 10);
INSERT INTO `pharmacysales` (`SaleID`, `SaleNumber`, `PatientID`, `SaleDate`, `TotalAmount`, `Discount`, `NetAmount`, `PaymentStatus`, `SoldBy`) VALUES (9, 'SALSD008', 9, '2026-03-05 15:00:00.000000', 254.00, 20.00, 234.00, 'Paid', 10);
INSERT INTO `pharmacysales` (`SaleID`, `SaleNumber`, `PatientID`, `SaleDate`, `TotalAmount`, `Discount`, `NetAmount`, `PaymentStatus`, `SoldBy`) VALUES (10, 'SALSD009', 10, '2026-03-06 15:00:00.000000', 234.50, 20.00, 214.50, 'Paid', 10);
INSERT INTO `pharmacysales` (`SaleID`, `SaleNumber`, `PatientID`, `SaleDate`, `TotalAmount`, `Discount`, `NetAmount`, `PaymentStatus`, `SoldBy`) VALUES (11, 'SALSD010', 11, '2026-03-07 15:00:00.000000', 354.00, 20.00, 334.00, 'Paid', 10);
INSERT INTO `pharmacysales` (`SaleID`, `SaleNumber`, `PatientID`, `SaleDate`, `TotalAmount`, `Discount`, `NetAmount`, `PaymentStatus`, `SoldBy`) VALUES (12, 'SALSD011', 12, '2026-03-08 15:00:00.000000', 480.50, 20.00, 460.50, 'Paid', 10);
INSERT INTO `pharmacysales` (`SaleID`, `SaleNumber`, `PatientID`, `SaleDate`, `TotalAmount`, `Discount`, `NetAmount`, `PaymentStatus`, `SoldBy`) VALUES (13, 'SALSD012', 13, '2026-03-09 15:00:00.000000', 188.00, 20.00, 168.00, 'Paid', 10);
INSERT INTO `pharmacysales` (`SaleID`, `SaleNumber`, `PatientID`, `SaleDate`, `TotalAmount`, `Discount`, `NetAmount`, `PaymentStatus`, `SoldBy`) VALUES (14, 'SALSD013', 14, '2026-03-10 15:00:00.000000', 316.00, 20.00, 296.00, 'Partial', 10);
INSERT INTO `pharmacysales` (`SaleID`, `SaleNumber`, `PatientID`, `SaleDate`, `TotalAmount`, `Discount`, `NetAmount`, `PaymentStatus`, `SoldBy`) VALUES (15, 'SALSD014', 15, '2026-03-11 15:00:00.000000', 451.00, 20.00, 431.00, 'Partial', 10);
INSERT INTO `pharmacysales` (`SaleID`, `SaleNumber`, `PatientID`, `SaleDate`, `TotalAmount`, `Discount`, `NetAmount`, `PaymentStatus`, `SoldBy`) VALUES (16, 'SALSD015', 16, '2026-03-12 15:00:00.000000', 429.50, 20.00, 409.50, 'Partial', 10);
INSERT INTO `pharmacysales` (`SaleID`, `SaleNumber`, `PatientID`, `SaleDate`, `TotalAmount`, `Discount`, `NetAmount`, `PaymentStatus`, `SoldBy`) VALUES (17, 'SALSD016', 17, '2026-03-13 15:00:00.000000', 266.00, 20.00, 246.00, 'Partial', 10);
INSERT INTO `pharmacysales` (`SaleID`, `SaleNumber`, `PatientID`, `SaleDate`, `TotalAmount`, `Discount`, `NetAmount`, `PaymentStatus`, `SoldBy`) VALUES (18, 'SALSD017', 18, '2026-03-14 15:00:00.000000', 409.50, 20.00, 389.50, 'Partial', 10);
INSERT INTO `pharmacysales` (`SaleID`, `SaleNumber`, `PatientID`, `SaleDate`, `TotalAmount`, `Discount`, `NetAmount`, `PaymentStatus`, `SoldBy`) VALUES (19, 'SALSD018', 19, '2026-03-15 15:00:00.000000', 383.00, 20.00, 363.00, 'Partial', 10);
INSERT INTO `pharmacysales` (`SaleID`, `SaleNumber`, `PatientID`, `SaleDate`, `TotalAmount`, `Discount`, `NetAmount`, `PaymentStatus`, `SoldBy`) VALUES (20, 'SALSD019', 20, '2026-03-16 15:00:00.000000', 536.00, 20.00, 516.00, 'Partial', 10);
INSERT INTO `pharmacysales` (`SaleID`, `SaleNumber`, `PatientID`, `SaleDate`, `TotalAmount`, `Discount`, `NetAmount`, `PaymentStatus`, `SoldBy`) VALUES (21, 'SALSD020', 21, '2026-03-17 15:00:00.000000', 356.00, 20.00, 336.00, 'Partial', 10);
INSERT INTO `pharmacysales` (`SaleID`, `SaleNumber`, `PatientID`, `SaleDate`, `TotalAmount`, `Discount`, `NetAmount`, `PaymentStatus`, `SoldBy`) VALUES (22, 'SALSD021', 22, '2026-03-18 15:00:00.000000', 324.50, 20.00, 304.50, 'Partial', 10);
INSERT INTO `pharmacysales` (`SaleID`, `SaleNumber`, `PatientID`, `SaleDate`, `TotalAmount`, `Discount`, `NetAmount`, `PaymentStatus`, `SoldBy`) VALUES (23, 'SALSD022', 23, '2026-03-19 15:00:00.000000', 486.00, 20.00, 466.00, 'Partial', 10);
INSERT INTO `pharmacysales` (`SaleID`, `SaleNumber`, `PatientID`, `SaleDate`, `TotalAmount`, `Discount`, `NetAmount`, `PaymentStatus`, `SoldBy`) VALUES (24, 'SALSD023', 24, '2026-03-20 15:00:00.000000', 654.50, 50.00, 604.50, 'Pending', 10);
INSERT INTO `pharmacysales` (`SaleID`, `SaleNumber`, `PatientID`, `SaleDate`, `TotalAmount`, `Discount`, `NetAmount`, `PaymentStatus`, `SoldBy`) VALUES (25, 'SALSD024', 25, '2026-03-21 15:00:00.000000', 254.00, 20.00, 234.00, 'Pending', 10);
INSERT INTO `pharmacysales` (`SaleID`, `SaleNumber`, `PatientID`, `SaleDate`, `TotalAmount`, `Discount`, `NetAmount`, `PaymentStatus`, `SoldBy`) VALUES (26, 'SALSD025', 26, '2026-03-22 15:00:00.000000', 424.00, 20.00, 404.00, 'Pending', 10);
INSERT INTO `pharmacysales` (`SaleID`, `SaleNumber`, `PatientID`, `SaleDate`, `TotalAmount`, `Discount`, `NetAmount`, `PaymentStatus`, `SoldBy`) VALUES (27, 'SALSD026', 27, '2026-03-23 15:00:00.000000', 601.00, 50.00, 551.00, 'Pending', 10);
INSERT INTO `pharmacysales` (`SaleID`, `SaleNumber`, `PatientID`, `SaleDate`, `TotalAmount`, `Discount`, `NetAmount`, `PaymentStatus`, `SoldBy`) VALUES (28, 'SALSD027', 28, '2026-03-24 15:00:00.000000', 567.50, 20.00, 547.50, 'Pending', 10);
INSERT INTO `pharmacysales` (`SaleID`, `SaleNumber`, `PatientID`, `SaleDate`, `TotalAmount`, `Discount`, `NetAmount`, `PaymentStatus`, `SoldBy`) VALUES (29, 'SALSD028', 29, '2026-03-25 15:00:00.000000', 350.00, 20.00, 330.00, 'Pending', 10);
INSERT INTO `pharmacysales` (`SaleID`, `SaleNumber`, `PatientID`, `SaleDate`, `TotalAmount`, `Discount`, `NetAmount`, `PaymentStatus`, `SoldBy`) VALUES (30, 'SALSD029', 30, '2026-03-26 15:00:00.000000', 535.50, 20.00, 515.50, 'Pending', 10);
INSERT INTO `pharmacysales` (`SaleID`, `SaleNumber`, `PatientID`, `SaleDate`, `TotalAmount`, `Discount`, `NetAmount`, `PaymentStatus`, `SoldBy`) VALUES (31, 'SALSD030', 31, '2026-03-27 15:00:00.000000', 497.00, 20.00, 477.00, 'Pending', 10);

-- Data for prescriptiondetails
INSERT INTO `prescriptiondetails` (`PrescriptionDetailID`, `PrescriptionID`, `MedicineName`, `Dosage`, `Frequency`, `Duration`, `Instructions`) VALUES (2, 2, 'Seed Medicine 01', '1 tablet', 'Once daily', '6 days', 'Take after meals unless otherwise advised.');
INSERT INTO `prescriptiondetails` (`PrescriptionDetailID`, `PrescriptionID`, `MedicineName`, `Dosage`, `Frequency`, `Duration`, `Instructions`) VALUES (3, 3, 'Seed Medicine 02', '1 tablet', 'Twice daily', '7 days', 'Take after meals unless otherwise advised.');
INSERT INTO `prescriptiondetails` (`PrescriptionDetailID`, `PrescriptionID`, `MedicineName`, `Dosage`, `Frequency`, `Duration`, `Instructions`) VALUES (4, 4, 'Seed Medicine 03', '1 capsule', 'Once daily', '8 days', 'Take after meals unless otherwise advised.');
INSERT INTO `prescriptiondetails` (`PrescriptionDetailID`, `PrescriptionID`, `MedicineName`, `Dosage`, `Frequency`, `Duration`, `Instructions`) VALUES (5, 5, 'Seed Medicine 04', '1 tablet', 'Twice daily', '9 days', 'Take after meals unless otherwise advised.');
INSERT INTO `prescriptiondetails` (`PrescriptionDetailID`, `PrescriptionID`, `MedicineName`, `Dosage`, `Frequency`, `Duration`, `Instructions`) VALUES (6, 6, 'Seed Medicine 05', '1 tablet', 'Once daily', '10 days', 'Take after meals unless otherwise advised.');
INSERT INTO `prescriptiondetails` (`PrescriptionDetailID`, `PrescriptionID`, `MedicineName`, `Dosage`, `Frequency`, `Duration`, `Instructions`) VALUES (7, 7, 'Seed Medicine 06', '1 capsule', 'Twice daily', '11 days', 'Take after meals unless otherwise advised.');
INSERT INTO `prescriptiondetails` (`PrescriptionDetailID`, `PrescriptionID`, `MedicineName`, `Dosage`, `Frequency`, `Duration`, `Instructions`) VALUES (8, 8, 'Seed Medicine 07', '1 tablet', 'Once daily', '5 days', 'Take after meals unless otherwise advised.');
INSERT INTO `prescriptiondetails` (`PrescriptionDetailID`, `PrescriptionID`, `MedicineName`, `Dosage`, `Frequency`, `Duration`, `Instructions`) VALUES (9, 9, 'Seed Medicine 08', '1 tablet', 'Twice daily', '6 days', 'Take after meals unless otherwise advised.');
INSERT INTO `prescriptiondetails` (`PrescriptionDetailID`, `PrescriptionID`, `MedicineName`, `Dosage`, `Frequency`, `Duration`, `Instructions`) VALUES (10, 10, 'Seed Medicine 09', '1 capsule', 'Once daily', '7 days', 'Take after meals unless otherwise advised.');
INSERT INTO `prescriptiondetails` (`PrescriptionDetailID`, `PrescriptionID`, `MedicineName`, `Dosage`, `Frequency`, `Duration`, `Instructions`) VALUES (11, 11, 'Seed Medicine 10', '1 tablet', 'Twice daily', '8 days', 'Take after meals unless otherwise advised.');
INSERT INTO `prescriptiondetails` (`PrescriptionDetailID`, `PrescriptionID`, `MedicineName`, `Dosage`, `Frequency`, `Duration`, `Instructions`) VALUES (12, 12, 'Seed Medicine 11', '1 tablet', 'Once daily', '9 days', 'Take after meals unless otherwise advised.');
INSERT INTO `prescriptiondetails` (`PrescriptionDetailID`, `PrescriptionID`, `MedicineName`, `Dosage`, `Frequency`, `Duration`, `Instructions`) VALUES (13, 13, 'Seed Medicine 12', '1 capsule', 'Twice daily', '10 days', 'Take after meals unless otherwise advised.');
INSERT INTO `prescriptiondetails` (`PrescriptionDetailID`, `PrescriptionID`, `MedicineName`, `Dosage`, `Frequency`, `Duration`, `Instructions`) VALUES (14, 14, 'Seed Medicine 13', '1 tablet', 'Once daily', '11 days', 'Take after meals unless otherwise advised.');
INSERT INTO `prescriptiondetails` (`PrescriptionDetailID`, `PrescriptionID`, `MedicineName`, `Dosage`, `Frequency`, `Duration`, `Instructions`) VALUES (15, 15, 'Seed Medicine 14', '1 tablet', 'Twice daily', '5 days', 'Take after meals unless otherwise advised.');
INSERT INTO `prescriptiondetails` (`PrescriptionDetailID`, `PrescriptionID`, `MedicineName`, `Dosage`, `Frequency`, `Duration`, `Instructions`) VALUES (16, 16, 'Seed Medicine 15', '1 capsule', 'Once daily', '6 days', 'Take after meals unless otherwise advised.');
INSERT INTO `prescriptiondetails` (`PrescriptionDetailID`, `PrescriptionID`, `MedicineName`, `Dosage`, `Frequency`, `Duration`, `Instructions`) VALUES (17, 17, 'Seed Medicine 16', '1 tablet', 'Twice daily', '7 days', 'Take after meals unless otherwise advised.');
INSERT INTO `prescriptiondetails` (`PrescriptionDetailID`, `PrescriptionID`, `MedicineName`, `Dosage`, `Frequency`, `Duration`, `Instructions`) VALUES (18, 18, 'Seed Medicine 17', '1 tablet', 'Once daily', '8 days', 'Take after meals unless otherwise advised.');
INSERT INTO `prescriptiondetails` (`PrescriptionDetailID`, `PrescriptionID`, `MedicineName`, `Dosage`, `Frequency`, `Duration`, `Instructions`) VALUES (19, 19, 'Seed Medicine 18', '1 capsule', 'Twice daily', '9 days', 'Take after meals unless otherwise advised.');
INSERT INTO `prescriptiondetails` (`PrescriptionDetailID`, `PrescriptionID`, `MedicineName`, `Dosage`, `Frequency`, `Duration`, `Instructions`) VALUES (20, 20, 'Seed Medicine 19', '1 tablet', 'Once daily', '10 days', 'Take after meals unless otherwise advised.');
INSERT INTO `prescriptiondetails` (`PrescriptionDetailID`, `PrescriptionID`, `MedicineName`, `Dosage`, `Frequency`, `Duration`, `Instructions`) VALUES (21, 21, 'Seed Medicine 20', '1 tablet', 'Twice daily', '11 days', 'Take after meals unless otherwise advised.');
INSERT INTO `prescriptiondetails` (`PrescriptionDetailID`, `PrescriptionID`, `MedicineName`, `Dosage`, `Frequency`, `Duration`, `Instructions`) VALUES (22, 22, 'Seed Medicine 21', '1 capsule', 'Once daily', '5 days', 'Take after meals unless otherwise advised.');
INSERT INTO `prescriptiondetails` (`PrescriptionDetailID`, `PrescriptionID`, `MedicineName`, `Dosage`, `Frequency`, `Duration`, `Instructions`) VALUES (23, 23, 'Seed Medicine 22', '1 tablet', 'Twice daily', '6 days', 'Take after meals unless otherwise advised.');
INSERT INTO `prescriptiondetails` (`PrescriptionDetailID`, `PrescriptionID`, `MedicineName`, `Dosage`, `Frequency`, `Duration`, `Instructions`) VALUES (24, 24, 'Seed Medicine 23', '1 tablet', 'Once daily', '7 days', 'Take after meals unless otherwise advised.');
INSERT INTO `prescriptiondetails` (`PrescriptionDetailID`, `PrescriptionID`, `MedicineName`, `Dosage`, `Frequency`, `Duration`, `Instructions`) VALUES (25, 25, 'Seed Medicine 24', '1 capsule', 'Twice daily', '8 days', 'Take after meals unless otherwise advised.');
INSERT INTO `prescriptiondetails` (`PrescriptionDetailID`, `PrescriptionID`, `MedicineName`, `Dosage`, `Frequency`, `Duration`, `Instructions`) VALUES (26, 26, 'Seed Medicine 25', '1 tablet', 'Once daily', '9 days', 'Take after meals unless otherwise advised.');
INSERT INTO `prescriptiondetails` (`PrescriptionDetailID`, `PrescriptionID`, `MedicineName`, `Dosage`, `Frequency`, `Duration`, `Instructions`) VALUES (27, 27, 'Seed Medicine 26', '1 tablet', 'Twice daily', '10 days', 'Take after meals unless otherwise advised.');
INSERT INTO `prescriptiondetails` (`PrescriptionDetailID`, `PrescriptionID`, `MedicineName`, `Dosage`, `Frequency`, `Duration`, `Instructions`) VALUES (28, 28, 'Seed Medicine 27', '1 capsule', 'Once daily', '11 days', 'Take after meals unless otherwise advised.');
INSERT INTO `prescriptiondetails` (`PrescriptionDetailID`, `PrescriptionID`, `MedicineName`, `Dosage`, `Frequency`, `Duration`, `Instructions`) VALUES (29, 29, 'Seed Medicine 28', '1 tablet', 'Twice daily', '5 days', 'Take after meals unless otherwise advised.');
INSERT INTO `prescriptiondetails` (`PrescriptionDetailID`, `PrescriptionID`, `MedicineName`, `Dosage`, `Frequency`, `Duration`, `Instructions`) VALUES (30, 30, 'Seed Medicine 29', '1 tablet', 'Once daily', '6 days', 'Take after meals unless otherwise advised.');
INSERT INTO `prescriptiondetails` (`PrescriptionDetailID`, `PrescriptionID`, `MedicineName`, `Dosage`, `Frequency`, `Duration`, `Instructions`) VALUES (31, 31, 'Seed Medicine 30', '1 capsule', 'Twice daily', '7 days', 'Take after meals unless otherwise advised.');

-- Data for prescriptions
INSERT INTO `prescriptions` (`PrescriptionID`, `PrescriptionCode`, `VisitID`, `PatientID`, `DoctorID`, `PrescriptionDate`, `Instructions`, `Status`) VALUES (2, 'RXSD001', 2, 2, 3, '2026-02-10 11:00:00.000000', 'Take medication exactly as prescribed and return for follow-up.', 'Active');
INSERT INTO `prescriptions` (`PrescriptionID`, `PrescriptionCode`, `VisitID`, `PatientID`, `DoctorID`, `PrescriptionDate`, `Instructions`, `Status`) VALUES (3, 'RXSD002', 3, 3, 4, '2026-02-11 11:00:00.000000', 'Take medication exactly as prescribed and return for follow-up.', 'Active');
INSERT INTO `prescriptions` (`PrescriptionID`, `PrescriptionCode`, `VisitID`, `PatientID`, `DoctorID`, `PrescriptionDate`, `Instructions`, `Status`) VALUES (4, 'RXSD003', 4, 4, 5, '2026-02-12 11:00:00.000000', 'Take medication exactly as prescribed and return for follow-up.', 'Active');
INSERT INTO `prescriptions` (`PrescriptionID`, `PrescriptionCode`, `VisitID`, `PatientID`, `DoctorID`, `PrescriptionDate`, `Instructions`, `Status`) VALUES (5, 'RXSD004', 5, 5, 6, '2026-02-13 11:00:00.000000', 'Take medication exactly as prescribed and return for follow-up.', 'Active');
INSERT INTO `prescriptions` (`PrescriptionID`, `PrescriptionCode`, `VisitID`, `PatientID`, `DoctorID`, `PrescriptionDate`, `Instructions`, `Status`) VALUES (6, 'RXSD005', 6, 6, 7, '2026-02-14 11:00:00.000000', 'Take medication exactly as prescribed and return for follow-up.', 'Completed');
INSERT INTO `prescriptions` (`PrescriptionID`, `PrescriptionCode`, `VisitID`, `PatientID`, `DoctorID`, `PrescriptionDate`, `Instructions`, `Status`) VALUES (7, 'RXSD006', 7, 7, 8, '2026-02-15 11:00:00.000000', 'Take medication exactly as prescribed and return for follow-up.', 'Active');
INSERT INTO `prescriptions` (`PrescriptionID`, `PrescriptionCode`, `VisitID`, `PatientID`, `DoctorID`, `PrescriptionDate`, `Instructions`, `Status`) VALUES (8, 'RXSD007', 8, 8, 9, '2026-02-16 11:00:00.000000', 'Take medication exactly as prescribed and return for follow-up.', 'Active');
INSERT INTO `prescriptions` (`PrescriptionID`, `PrescriptionCode`, `VisitID`, `PatientID`, `DoctorID`, `PrescriptionDate`, `Instructions`, `Status`) VALUES (9, 'RXSD008', 9, 9, 10, '2026-02-17 11:00:00.000000', 'Take medication exactly as prescribed and return for follow-up.', 'Active');
INSERT INTO `prescriptions` (`PrescriptionID`, `PrescriptionCode`, `VisitID`, `PatientID`, `DoctorID`, `PrescriptionDate`, `Instructions`, `Status`) VALUES (10, 'RXSD009', 10, 10, 11, '2026-02-18 11:00:00.000000', 'Take medication exactly as prescribed and return for follow-up.', 'Active');
INSERT INTO `prescriptions` (`PrescriptionID`, `PrescriptionCode`, `VisitID`, `PatientID`, `DoctorID`, `PrescriptionDate`, `Instructions`, `Status`) VALUES (11, 'RXSD010', 11, 11, 12, '2026-02-19 11:00:00.000000', 'Take medication exactly as prescribed and return for follow-up.', 'Completed');
INSERT INTO `prescriptions` (`PrescriptionID`, `PrescriptionCode`, `VisitID`, `PatientID`, `DoctorID`, `PrescriptionDate`, `Instructions`, `Status`) VALUES (12, 'RXSD011', 12, 12, 13, '2026-02-20 11:00:00.000000', 'Take medication exactly as prescribed and return for follow-up.', 'Active');
INSERT INTO `prescriptions` (`PrescriptionID`, `PrescriptionCode`, `VisitID`, `PatientID`, `DoctorID`, `PrescriptionDate`, `Instructions`, `Status`) VALUES (13, 'RXSD012', 13, 13, 14, '2026-02-21 11:00:00.000000', 'Take medication exactly as prescribed and return for follow-up.', 'Active');
INSERT INTO `prescriptions` (`PrescriptionID`, `PrescriptionCode`, `VisitID`, `PatientID`, `DoctorID`, `PrescriptionDate`, `Instructions`, `Status`) VALUES (14, 'RXSD013', 14, 14, 15, '2026-02-22 11:00:00.000000', 'Take medication exactly as prescribed and return for follow-up.', 'Active');
INSERT INTO `prescriptions` (`PrescriptionID`, `PrescriptionCode`, `VisitID`, `PatientID`, `DoctorID`, `PrescriptionDate`, `Instructions`, `Status`) VALUES (15, 'RXSD014', 15, 15, 16, '2026-02-23 11:00:00.000000', 'Take medication exactly as prescribed and return for follow-up.', 'Active');
INSERT INTO `prescriptions` (`PrescriptionID`, `PrescriptionCode`, `VisitID`, `PatientID`, `DoctorID`, `PrescriptionDate`, `Instructions`, `Status`) VALUES (16, 'RXSD015', 16, 16, 17, '2026-02-24 11:00:00.000000', 'Take medication exactly as prescribed and return for follow-up.', 'Completed');
INSERT INTO `prescriptions` (`PrescriptionID`, `PrescriptionCode`, `VisitID`, `PatientID`, `DoctorID`, `PrescriptionDate`, `Instructions`, `Status`) VALUES (17, 'RXSD016', 17, 17, 18, '2026-02-25 11:00:00.000000', 'Take medication exactly as prescribed and return for follow-up.', 'Active');
INSERT INTO `prescriptions` (`PrescriptionID`, `PrescriptionCode`, `VisitID`, `PatientID`, `DoctorID`, `PrescriptionDate`, `Instructions`, `Status`) VALUES (18, 'RXSD017', 18, 18, 19, '2026-02-26 11:00:00.000000', 'Take medication exactly as prescribed and return for follow-up.', 'Active');
INSERT INTO `prescriptions` (`PrescriptionID`, `PrescriptionCode`, `VisitID`, `PatientID`, `DoctorID`, `PrescriptionDate`, `Instructions`, `Status`) VALUES (19, 'RXSD018', 19, 19, 20, '2026-02-27 11:00:00.000000', 'Take medication exactly as prescribed and return for follow-up.', 'Active');
INSERT INTO `prescriptions` (`PrescriptionID`, `PrescriptionCode`, `VisitID`, `PatientID`, `DoctorID`, `PrescriptionDate`, `Instructions`, `Status`) VALUES (20, 'RXSD019', 20, 20, 21, '2026-02-28 11:00:00.000000', 'Take medication exactly as prescribed and return for follow-up.', 'Active');
INSERT INTO `prescriptions` (`PrescriptionID`, `PrescriptionCode`, `VisitID`, `PatientID`, `DoctorID`, `PrescriptionDate`, `Instructions`, `Status`) VALUES (21, 'RXSD020', 21, 21, 22, '2026-03-01 11:00:00.000000', 'Take medication exactly as prescribed and return for follow-up.', 'Completed');
INSERT INTO `prescriptions` (`PrescriptionID`, `PrescriptionCode`, `VisitID`, `PatientID`, `DoctorID`, `PrescriptionDate`, `Instructions`, `Status`) VALUES (22, 'RXSD021', 22, 22, 23, '2026-03-02 11:00:00.000000', 'Take medication exactly as prescribed and return for follow-up.', 'Active');
INSERT INTO `prescriptions` (`PrescriptionID`, `PrescriptionCode`, `VisitID`, `PatientID`, `DoctorID`, `PrescriptionDate`, `Instructions`, `Status`) VALUES (23, 'RXSD022', 23, 23, 24, '2026-03-03 11:00:00.000000', 'Take medication exactly as prescribed and return for follow-up.', 'Active');
INSERT INTO `prescriptions` (`PrescriptionID`, `PrescriptionCode`, `VisitID`, `PatientID`, `DoctorID`, `PrescriptionDate`, `Instructions`, `Status`) VALUES (24, 'RXSD023', 24, 24, 25, '2026-03-04 11:00:00.000000', 'Take medication exactly as prescribed and return for follow-up.', 'Active');
INSERT INTO `prescriptions` (`PrescriptionID`, `PrescriptionCode`, `VisitID`, `PatientID`, `DoctorID`, `PrescriptionDate`, `Instructions`, `Status`) VALUES (25, 'RXSD024', 25, 25, 26, '2026-03-05 11:00:00.000000', 'Take medication exactly as prescribed and return for follow-up.', 'Active');
INSERT INTO `prescriptions` (`PrescriptionID`, `PrescriptionCode`, `VisitID`, `PatientID`, `DoctorID`, `PrescriptionDate`, `Instructions`, `Status`) VALUES (26, 'RXSD025', 26, 26, 27, '2026-03-06 11:00:00.000000', 'Take medication exactly as prescribed and return for follow-up.', 'Completed');
INSERT INTO `prescriptions` (`PrescriptionID`, `PrescriptionCode`, `VisitID`, `PatientID`, `DoctorID`, `PrescriptionDate`, `Instructions`, `Status`) VALUES (27, 'RXSD026', 27, 27, 28, '2026-03-07 11:00:00.000000', 'Take medication exactly as prescribed and return for follow-up.', 'Active');
INSERT INTO `prescriptions` (`PrescriptionID`, `PrescriptionCode`, `VisitID`, `PatientID`, `DoctorID`, `PrescriptionDate`, `Instructions`, `Status`) VALUES (28, 'RXSD027', 28, 28, 29, '2026-03-08 11:00:00.000000', 'Take medication exactly as prescribed and return for follow-up.', 'Active');
INSERT INTO `prescriptions` (`PrescriptionID`, `PrescriptionCode`, `VisitID`, `PatientID`, `DoctorID`, `PrescriptionDate`, `Instructions`, `Status`) VALUES (29, 'RXSD028', 29, 29, 30, '2026-03-09 11:00:00.000000', 'Take medication exactly as prescribed and return for follow-up.', 'Active');
INSERT INTO `prescriptions` (`PrescriptionID`, `PrescriptionCode`, `VisitID`, `PatientID`, `DoctorID`, `PrescriptionDate`, `Instructions`, `Status`) VALUES (30, 'RXSD029', 30, 30, 31, '2026-03-10 11:00:00.000000', 'Take medication exactly as prescribed and return for follow-up.', 'Active');
INSERT INTO `prescriptions` (`PrescriptionID`, `PrescriptionCode`, `VisitID`, `PatientID`, `DoctorID`, `PrescriptionDate`, `Instructions`, `Status`) VALUES (31, 'RXSD030', 31, 31, 32, '2026-03-11 11:00:00.000000', 'Take medication exactly as prescribed and return for follow-up.', 'Completed');

-- Data for rooms
INSERT INTO `rooms` (`RoomID`, `RoomNumber`, `WardID`, `RoomType`, `TotalBeds`, `AvailableBeds`, `Facilities`, `RatePerDay`, `Status`) VALUES (9, 'RMSD001', 10, 'Standard', 2, 2, 'Air conditioning, oxygen port, bedside monitor', 1910.00, 'Available');
INSERT INTO `rooms` (`RoomID`, `RoomNumber`, `WardID`, `RoomType`, `TotalBeds`, `AvailableBeds`, `Facilities`, `RatePerDay`, `Status`) VALUES (10, 'RMSD002', 11, 'Standard', 2, 2, 'Air conditioning, oxygen port, bedside monitor', 2020.00, 'Available');
INSERT INTO `rooms` (`RoomID`, `RoomNumber`, `WardID`, `RoomType`, `TotalBeds`, `AvailableBeds`, `Facilities`, `RatePerDay`, `Status`) VALUES (11, 'RMSD003', 12, 'Private', 2, 2, 'Air conditioning, oxygen port, bedside monitor', 2130.00, 'Available');
INSERT INTO `rooms` (`RoomID`, `RoomNumber`, `WardID`, `RoomType`, `TotalBeds`, `AvailableBeds`, `Facilities`, `RatePerDay`, `Status`) VALUES (12, 'RMSD004', 13, 'ICU', 2, 2, 'Air conditioning, oxygen port, bedside monitor', 2240.00, 'Available');
INSERT INTO `rooms` (`RoomID`, `RoomNumber`, `WardID`, `RoomType`, `TotalBeds`, `AvailableBeds`, `Facilities`, `RatePerDay`, `Status`) VALUES (13, 'RMSD005', 14, 'Standard', 2, 2, 'Air conditioning, oxygen port, bedside monitor', 2350.00, 'Available');
INSERT INTO `rooms` (`RoomID`, `RoomNumber`, `WardID`, `RoomType`, `TotalBeds`, `AvailableBeds`, `Facilities`, `RatePerDay`, `Status`) VALUES (14, 'RMSD006', 15, 'Private', 2, 2, 'Air conditioning, oxygen port, bedside monitor', 2460.00, 'Available');
INSERT INTO `rooms` (`RoomID`, `RoomNumber`, `WardID`, `RoomType`, `TotalBeds`, `AvailableBeds`, `Facilities`, `RatePerDay`, `Status`) VALUES (15, 'RMSD007', 16, 'Standard', 2, 2, 'Air conditioning, oxygen port, bedside monitor', 2570.00, 'Available');
INSERT INTO `rooms` (`RoomID`, `RoomNumber`, `WardID`, `RoomType`, `TotalBeds`, `AvailableBeds`, `Facilities`, `RatePerDay`, `Status`) VALUES (16, 'RMSD008', 17, 'ICU', 2, 2, 'Air conditioning, oxygen port, bedside monitor', 2680.00, 'Available');
INSERT INTO `rooms` (`RoomID`, `RoomNumber`, `WardID`, `RoomType`, `TotalBeds`, `AvailableBeds`, `Facilities`, `RatePerDay`, `Status`) VALUES (17, 'RMSD009', 18, 'Private', 2, 2, 'Air conditioning, oxygen port, bedside monitor', 2790.00, 'Available');
INSERT INTO `rooms` (`RoomID`, `RoomNumber`, `WardID`, `RoomType`, `TotalBeds`, `AvailableBeds`, `Facilities`, `RatePerDay`, `Status`) VALUES (18, 'RMSD010', 19, 'Standard', 2, 2, 'Air conditioning, oxygen port, bedside monitor', 2900.00, 'Available');
INSERT INTO `rooms` (`RoomID`, `RoomNumber`, `WardID`, `RoomType`, `TotalBeds`, `AvailableBeds`, `Facilities`, `RatePerDay`, `Status`) VALUES (19, 'RMSD011', 20, 'Standard', 2, 2, 'Air conditioning, oxygen port, bedside monitor', 3010.00, 'Available');
INSERT INTO `rooms` (`RoomID`, `RoomNumber`, `WardID`, `RoomType`, `TotalBeds`, `AvailableBeds`, `Facilities`, `RatePerDay`, `Status`) VALUES (20, 'RMSD012', 21, 'ICU', 2, 2, 'Air conditioning, oxygen port, bedside monitor', 3120.00, 'Available');
INSERT INTO `rooms` (`RoomID`, `RoomNumber`, `WardID`, `RoomType`, `TotalBeds`, `AvailableBeds`, `Facilities`, `RatePerDay`, `Status`) VALUES (21, 'RMSD013', 22, 'Standard', 2, 2, 'Air conditioning, oxygen port, bedside monitor', 3230.00, 'Available');
INSERT INTO `rooms` (`RoomID`, `RoomNumber`, `WardID`, `RoomType`, `TotalBeds`, `AvailableBeds`, `Facilities`, `RatePerDay`, `Status`) VALUES (22, 'RMSD014', 23, 'Standard', 2, 2, 'Air conditioning, oxygen port, bedside monitor', 3340.00, 'Available');
INSERT INTO `rooms` (`RoomID`, `RoomNumber`, `WardID`, `RoomType`, `TotalBeds`, `AvailableBeds`, `Facilities`, `RatePerDay`, `Status`) VALUES (23, 'RMSD015', 24, 'Private', 2, 2, 'Air conditioning, oxygen port, bedside monitor', 3450.00, 'Available');
INSERT INTO `rooms` (`RoomID`, `RoomNumber`, `WardID`, `RoomType`, `TotalBeds`, `AvailableBeds`, `Facilities`, `RatePerDay`, `Status`) VALUES (24, 'RMSD016', 25, 'ICU', 2, 1, 'Air conditioning, oxygen port, bedside monitor', 3560.00, 'Available');
INSERT INTO `rooms` (`RoomID`, `RoomNumber`, `WardID`, `RoomType`, `TotalBeds`, `AvailableBeds`, `Facilities`, `RatePerDay`, `Status`) VALUES (25, 'RMSD017', 26, 'Standard', 2, 1, 'Air conditioning, oxygen port, bedside monitor', 3670.00, 'Available');
INSERT INTO `rooms` (`RoomID`, `RoomNumber`, `WardID`, `RoomType`, `TotalBeds`, `AvailableBeds`, `Facilities`, `RatePerDay`, `Status`) VALUES (26, 'RMSD018', 27, 'Private', 2, 1, 'Air conditioning, oxygen port, bedside monitor', 3780.00, 'Available');
INSERT INTO `rooms` (`RoomID`, `RoomNumber`, `WardID`, `RoomType`, `TotalBeds`, `AvailableBeds`, `Facilities`, `RatePerDay`, `Status`) VALUES (27, 'RMSD019', 28, 'Standard', 2, 1, 'Air conditioning, oxygen port, bedside monitor', 3890.00, 'Available');
INSERT INTO `rooms` (`RoomID`, `RoomNumber`, `WardID`, `RoomType`, `TotalBeds`, `AvailableBeds`, `Facilities`, `RatePerDay`, `Status`) VALUES (28, 'RMSD020', 29, 'ICU', 2, 1, 'Air conditioning, oxygen port, bedside monitor', 4000.00, 'Available');
INSERT INTO `rooms` (`RoomID`, `RoomNumber`, `WardID`, `RoomType`, `TotalBeds`, `AvailableBeds`, `Facilities`, `RatePerDay`, `Status`) VALUES (29, 'RMSD021', 30, 'Private', 2, 1, 'Air conditioning, oxygen port, bedside monitor', 4110.00, 'Available');
INSERT INTO `rooms` (`RoomID`, `RoomNumber`, `WardID`, `RoomType`, `TotalBeds`, `AvailableBeds`, `Facilities`, `RatePerDay`, `Status`) VALUES (30, 'RMSD022', 31, 'Standard', 2, 1, 'Air conditioning, oxygen port, bedside monitor', 4220.00, 'Available');
INSERT INTO `rooms` (`RoomID`, `RoomNumber`, `WardID`, `RoomType`, `TotalBeds`, `AvailableBeds`, `Facilities`, `RatePerDay`, `Status`) VALUES (31, 'RMSD023', 32, 'Standard', 2, 1, 'Air conditioning, oxygen port, bedside monitor', 4330.00, 'Available');
INSERT INTO `rooms` (`RoomID`, `RoomNumber`, `WardID`, `RoomType`, `TotalBeds`, `AvailableBeds`, `Facilities`, `RatePerDay`, `Status`) VALUES (32, 'RMSD024', 33, 'ICU', 2, 1, 'Air conditioning, oxygen port, bedside monitor', 4440.00, 'Available');
INSERT INTO `rooms` (`RoomID`, `RoomNumber`, `WardID`, `RoomType`, `TotalBeds`, `AvailableBeds`, `Facilities`, `RatePerDay`, `Status`) VALUES (33, 'RMSD025', 34, 'Standard', 2, 1, 'Air conditioning, oxygen port, bedside monitor', 4550.00, 'Available');
INSERT INTO `rooms` (`RoomID`, `RoomNumber`, `WardID`, `RoomType`, `TotalBeds`, `AvailableBeds`, `Facilities`, `RatePerDay`, `Status`) VALUES (34, 'RMSD026', 35, 'Standard', 2, 1, 'Air conditioning, oxygen port, bedside monitor', 4660.00, 'Available');
INSERT INTO `rooms` (`RoomID`, `RoomNumber`, `WardID`, `RoomType`, `TotalBeds`, `AvailableBeds`, `Facilities`, `RatePerDay`, `Status`) VALUES (35, 'RMSD027', 36, 'Private', 2, 1, 'Air conditioning, oxygen port, bedside monitor', 4770.00, 'Available');
INSERT INTO `rooms` (`RoomID`, `RoomNumber`, `WardID`, `RoomType`, `TotalBeds`, `AvailableBeds`, `Facilities`, `RatePerDay`, `Status`) VALUES (36, 'RMSD028', 37, 'ICU', 2, 1, 'Air conditioning, oxygen port, bedside monitor', 4880.00, 'Available');
INSERT INTO `rooms` (`RoomID`, `RoomNumber`, `WardID`, `RoomType`, `TotalBeds`, `AvailableBeds`, `Facilities`, `RatePerDay`, `Status`) VALUES (37, 'RMSD029', 38, 'Standard', 2, 1, 'Air conditioning, oxygen port, bedside monitor', 4990.00, 'Available');
INSERT INTO `rooms` (`RoomID`, `RoomNumber`, `WardID`, `RoomType`, `TotalBeds`, `AvailableBeds`, `Facilities`, `RatePerDay`, `Status`) VALUES (38, 'RMSD030', 39, 'Private', 2, 1, 'Air conditioning, oxygen port, bedside monitor', 5100.00, 'Available');

-- Data for servicecategories
INSERT INTO `servicecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (9, 'Seed Service Category 01', 'Sample category 01 for seeded billing items.');
INSERT INTO `servicecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (10, 'Seed Service Category 02', 'Sample category 02 for seeded billing items.');
INSERT INTO `servicecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (11, 'Seed Service Category 03', 'Sample category 03 for seeded billing items.');
INSERT INTO `servicecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (12, 'Seed Service Category 04', 'Sample category 04 for seeded billing items.');
INSERT INTO `servicecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (13, 'Seed Service Category 05', 'Sample category 05 for seeded billing items.');
INSERT INTO `servicecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (14, 'Seed Service Category 06', 'Sample category 06 for seeded billing items.');
INSERT INTO `servicecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (15, 'Seed Service Category 07', 'Sample category 07 for seeded billing items.');
INSERT INTO `servicecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (16, 'Seed Service Category 08', 'Sample category 08 for seeded billing items.');
INSERT INTO `servicecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (17, 'Seed Service Category 09', 'Sample category 09 for seeded billing items.');
INSERT INTO `servicecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (18, 'Seed Service Category 10', 'Sample category 10 for seeded billing items.');
INSERT INTO `servicecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (19, 'Seed Service Category 11', 'Sample category 11 for seeded billing items.');
INSERT INTO `servicecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (20, 'Seed Service Category 12', 'Sample category 12 for seeded billing items.');
INSERT INTO `servicecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (21, 'Seed Service Category 13', 'Sample category 13 for seeded billing items.');
INSERT INTO `servicecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (22, 'Seed Service Category 14', 'Sample category 14 for seeded billing items.');
INSERT INTO `servicecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (23, 'Seed Service Category 15', 'Sample category 15 for seeded billing items.');
INSERT INTO `servicecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (24, 'Seed Service Category 16', 'Sample category 16 for seeded billing items.');
INSERT INTO `servicecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (25, 'Seed Service Category 17', 'Sample category 17 for seeded billing items.');
INSERT INTO `servicecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (26, 'Seed Service Category 18', 'Sample category 18 for seeded billing items.');
INSERT INTO `servicecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (27, 'Seed Service Category 19', 'Sample category 19 for seeded billing items.');
INSERT INTO `servicecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (28, 'Seed Service Category 20', 'Sample category 20 for seeded billing items.');
INSERT INTO `servicecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (29, 'Seed Service Category 21', 'Sample category 21 for seeded billing items.');
INSERT INTO `servicecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (30, 'Seed Service Category 22', 'Sample category 22 for seeded billing items.');
INSERT INTO `servicecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (31, 'Seed Service Category 23', 'Sample category 23 for seeded billing items.');
INSERT INTO `servicecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (32, 'Seed Service Category 24', 'Sample category 24 for seeded billing items.');
INSERT INTO `servicecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (33, 'Seed Service Category 25', 'Sample category 25 for seeded billing items.');
INSERT INTO `servicecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (34, 'Seed Service Category 26', 'Sample category 26 for seeded billing items.');
INSERT INTO `servicecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (35, 'Seed Service Category 27', 'Sample category 27 for seeded billing items.');
INSERT INTO `servicecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (36, 'Seed Service Category 28', 'Sample category 28 for seeded billing items.');
INSERT INTO `servicecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (37, 'Seed Service Category 29', 'Sample category 29 for seeded billing items.');
INSERT INTO `servicecategories` (`CategoryID`, `CategoryName`, `Description`) VALUES (38, 'Seed Service Category 30', 'Sample category 30 for seeded billing items.');

-- Data for services
INSERT INTO `services` (`ServiceID`, `ServiceCode`, `ServiceName`, `CategoryID`, `Price`, `TaxRate`, `IsActive`) VALUES (10, 'SRVSD001', 'Seed Clinical Service 01', 9, 905.00, 12.00, 1);
INSERT INTO `services` (`ServiceID`, `ServiceCode`, `ServiceName`, `CategoryID`, `Price`, `TaxRate`, `IsActive`) VALUES (11, 'SRVSD002', 'Seed Clinical Service 02', 10, 960.00, 12.00, 1);
INSERT INTO `services` (`ServiceID`, `ServiceCode`, `ServiceName`, `CategoryID`, `Price`, `TaxRate`, `IsActive`) VALUES (12, 'SRVSD003', 'Seed Clinical Service 03', 11, 1015.00, 12.00, 1);
INSERT INTO `services` (`ServiceID`, `ServiceCode`, `ServiceName`, `CategoryID`, `Price`, `TaxRate`, `IsActive`) VALUES (13, 'SRVSD004', 'Seed Clinical Service 04', 12, 1070.00, 12.00, 1);
INSERT INTO `services` (`ServiceID`, `ServiceCode`, `ServiceName`, `CategoryID`, `Price`, `TaxRate`, `IsActive`) VALUES (14, 'SRVSD005', 'Seed Clinical Service 05', 13, 1125.00, 12.00, 1);
INSERT INTO `services` (`ServiceID`, `ServiceCode`, `ServiceName`, `CategoryID`, `Price`, `TaxRate`, `IsActive`) VALUES (15, 'SRVSD006', 'Seed Clinical Service 06', 14, 1180.00, 12.00, 1);
INSERT INTO `services` (`ServiceID`, `ServiceCode`, `ServiceName`, `CategoryID`, `Price`, `TaxRate`, `IsActive`) VALUES (16, 'SRVSD007', 'Seed Clinical Service 07', 15, 1235.00, 12.00, 1);
INSERT INTO `services` (`ServiceID`, `ServiceCode`, `ServiceName`, `CategoryID`, `Price`, `TaxRate`, `IsActive`) VALUES (17, 'SRVSD008', 'Seed Clinical Service 08', 16, 1290.00, 12.00, 1);
INSERT INTO `services` (`ServiceID`, `ServiceCode`, `ServiceName`, `CategoryID`, `Price`, `TaxRate`, `IsActive`) VALUES (18, 'SRVSD009', 'Seed Clinical Service 09', 17, 1345.00, 12.00, 1);
INSERT INTO `services` (`ServiceID`, `ServiceCode`, `ServiceName`, `CategoryID`, `Price`, `TaxRate`, `IsActive`) VALUES (19, 'SRVSD010', 'Seed Clinical Service 10', 18, 1400.00, 12.00, 1);
INSERT INTO `services` (`ServiceID`, `ServiceCode`, `ServiceName`, `CategoryID`, `Price`, `TaxRate`, `IsActive`) VALUES (20, 'SRVSD011', 'Seed Clinical Service 11', 19, 1455.00, 12.00, 1);
INSERT INTO `services` (`ServiceID`, `ServiceCode`, `ServiceName`, `CategoryID`, `Price`, `TaxRate`, `IsActive`) VALUES (21, 'SRVSD012', 'Seed Clinical Service 12', 20, 1510.00, 12.00, 1);
INSERT INTO `services` (`ServiceID`, `ServiceCode`, `ServiceName`, `CategoryID`, `Price`, `TaxRate`, `IsActive`) VALUES (22, 'SRVSD013', 'Seed Clinical Service 13', 21, 1565.00, 12.00, 1);
INSERT INTO `services` (`ServiceID`, `ServiceCode`, `ServiceName`, `CategoryID`, `Price`, `TaxRate`, `IsActive`) VALUES (23, 'SRVSD014', 'Seed Clinical Service 14', 22, 1620.00, 12.00, 1);
INSERT INTO `services` (`ServiceID`, `ServiceCode`, `ServiceName`, `CategoryID`, `Price`, `TaxRate`, `IsActive`) VALUES (24, 'SRVSD015', 'Seed Clinical Service 15', 23, 1675.00, 12.00, 1);
INSERT INTO `services` (`ServiceID`, `ServiceCode`, `ServiceName`, `CategoryID`, `Price`, `TaxRate`, `IsActive`) VALUES (25, 'SRVSD016', 'Seed Clinical Service 16', 24, 1730.00, 12.00, 1);
INSERT INTO `services` (`ServiceID`, `ServiceCode`, `ServiceName`, `CategoryID`, `Price`, `TaxRate`, `IsActive`) VALUES (26, 'SRVSD017', 'Seed Clinical Service 17', 25, 1785.00, 12.00, 1);
INSERT INTO `services` (`ServiceID`, `ServiceCode`, `ServiceName`, `CategoryID`, `Price`, `TaxRate`, `IsActive`) VALUES (27, 'SRVSD018', 'Seed Clinical Service 18', 26, 1840.00, 12.00, 1);
INSERT INTO `services` (`ServiceID`, `ServiceCode`, `ServiceName`, `CategoryID`, `Price`, `TaxRate`, `IsActive`) VALUES (28, 'SRVSD019', 'Seed Clinical Service 19', 27, 1895.00, 12.00, 1);
INSERT INTO `services` (`ServiceID`, `ServiceCode`, `ServiceName`, `CategoryID`, `Price`, `TaxRate`, `IsActive`) VALUES (29, 'SRVSD020', 'Seed Clinical Service 20', 28, 1950.00, 12.00, 1);
INSERT INTO `services` (`ServiceID`, `ServiceCode`, `ServiceName`, `CategoryID`, `Price`, `TaxRate`, `IsActive`) VALUES (30, 'SRVSD021', 'Seed Clinical Service 21', 29, 2005.00, 12.00, 1);
INSERT INTO `services` (`ServiceID`, `ServiceCode`, `ServiceName`, `CategoryID`, `Price`, `TaxRate`, `IsActive`) VALUES (31, 'SRVSD022', 'Seed Clinical Service 22', 30, 2060.00, 12.00, 1);
INSERT INTO `services` (`ServiceID`, `ServiceCode`, `ServiceName`, `CategoryID`, `Price`, `TaxRate`, `IsActive`) VALUES (32, 'SRVSD023', 'Seed Clinical Service 23', 31, 2115.00, 12.00, 1);
INSERT INTO `services` (`ServiceID`, `ServiceCode`, `ServiceName`, `CategoryID`, `Price`, `TaxRate`, `IsActive`) VALUES (33, 'SRVSD024', 'Seed Clinical Service 24', 32, 2170.00, 12.00, 1);
INSERT INTO `services` (`ServiceID`, `ServiceCode`, `ServiceName`, `CategoryID`, `Price`, `TaxRate`, `IsActive`) VALUES (34, 'SRVSD025', 'Seed Clinical Service 25', 33, 2225.00, 12.00, 1);
INSERT INTO `services` (`ServiceID`, `ServiceCode`, `ServiceName`, `CategoryID`, `Price`, `TaxRate`, `IsActive`) VALUES (35, 'SRVSD026', 'Seed Clinical Service 26', 34, 2280.00, 12.00, 1);
INSERT INTO `services` (`ServiceID`, `ServiceCode`, `ServiceName`, `CategoryID`, `Price`, `TaxRate`, `IsActive`) VALUES (36, 'SRVSD027', 'Seed Clinical Service 27', 35, 2335.00, 12.00, 1);
INSERT INTO `services` (`ServiceID`, `ServiceCode`, `ServiceName`, `CategoryID`, `Price`, `TaxRate`, `IsActive`) VALUES (37, 'SRVSD028', 'Seed Clinical Service 28', 36, 2390.00, 12.00, 1);
INSERT INTO `services` (`ServiceID`, `ServiceCode`, `ServiceName`, `CategoryID`, `Price`, `TaxRate`, `IsActive`) VALUES (38, 'SRVSD029', 'Seed Clinical Service 29', 37, 2445.00, 12.00, 1);
INSERT INTO `services` (`ServiceID`, `ServiceCode`, `ServiceName`, `CategoryID`, `Price`, `TaxRate`, `IsActive`) VALUES (39, 'SRVSD030', 'Seed Clinical Service 30', 38, 2500.00, 12.00, 1);

-- Data for specializations
INSERT INTO `specializations` (`SpecializationID`, `SpecializationCode`, `SpecializationName`, `Description`, `Department`) VALUES (11, 'SP01', 'Cardiology', 'Heart and vascular care', 'Cardiology');
INSERT INTO `specializations` (`SpecializationID`, `SpecializationCode`, `SpecializationName`, `Description`, `Department`) VALUES (12, 'SP02', 'Pediatrics', 'Child health and wellness', 'Pediatrics');
INSERT INTO `specializations` (`SpecializationID`, `SpecializationCode`, `SpecializationName`, `Description`, `Department`) VALUES (13, 'SP03', 'General Medicine', 'Primary adult care', 'Internal Medicine');
INSERT INTO `specializations` (`SpecializationID`, `SpecializationCode`, `SpecializationName`, `Description`, `Department`) VALUES (14, 'SP04', 'General Surgery', 'Operative and perioperative care', 'Surgery');
INSERT INTO `specializations` (`SpecializationID`, `SpecializationCode`, `SpecializationName`, `Description`, `Department`) VALUES (15, 'SP05', 'Gynecology', 'Women\'s health services', 'OB-GYN');
INSERT INTO `specializations` (`SpecializationID`, `SpecializationCode`, `SpecializationName`, `Description`, `Department`) VALUES (16, 'SP06', 'Neurology', 'Neurologic conditions', 'Neurology');
INSERT INTO `specializations` (`SpecializationID`, `SpecializationCode`, `SpecializationName`, `Description`, `Department`) VALUES (17, 'SP07', 'Orthopedics', 'Bones and joints', 'Orthopedics');
INSERT INTO `specializations` (`SpecializationID`, `SpecializationCode`, `SpecializationName`, `Description`, `Department`) VALUES (18, 'SP08', 'ENT', 'Ear, nose, and throat care', 'ENT');
INSERT INTO `specializations` (`SpecializationID`, `SpecializationCode`, `SpecializationName`, `Description`, `Department`) VALUES (19, 'SP09', 'Dermatology', 'Skin and hair conditions', 'Dermatology');
INSERT INTO `specializations` (`SpecializationID`, `SpecializationCode`, `SpecializationName`, `Description`, `Department`) VALUES (20, 'SP10', 'Psychiatry', 'Mental health care', 'Psychiatry');
INSERT INTO `specializations` (`SpecializationID`, `SpecializationCode`, `SpecializationName`, `Description`, `Department`) VALUES (21, 'SP11', 'Pulmonology', 'Respiratory disease care', 'Pulmonology');
INSERT INTO `specializations` (`SpecializationID`, `SpecializationCode`, `SpecializationName`, `Description`, `Department`) VALUES (22, 'SP12', 'Nephrology', 'Kidney health care', 'Nephrology');
INSERT INTO `specializations` (`SpecializationID`, `SpecializationCode`, `SpecializationName`, `Description`, `Department`) VALUES (23, 'SP13', 'Endocrinology', 'Hormonal and metabolic care', 'Endocrinology');
INSERT INTO `specializations` (`SpecializationID`, `SpecializationCode`, `SpecializationName`, `Description`, `Department`) VALUES (24, 'SP14', 'Gastroenterology', 'Digestive system care', 'Gastroenterology');
INSERT INTO `specializations` (`SpecializationID`, `SpecializationCode`, `SpecializationName`, `Description`, `Department`) VALUES (25, 'SP15', 'Oncology', 'Cancer care and follow-up', 'Oncology');
INSERT INTO `specializations` (`SpecializationID`, `SpecializationCode`, `SpecializationName`, `Description`, `Department`) VALUES (26, 'SP16', 'Ophthalmology', 'Eye care and surgery', 'Ophthalmology');
INSERT INTO `specializations` (`SpecializationID`, `SpecializationCode`, `SpecializationName`, `Description`, `Department`) VALUES (27, 'SP17', 'Urology', 'Urinary tract care', 'Urology');
INSERT INTO `specializations` (`SpecializationID`, `SpecializationCode`, `SpecializationName`, `Description`, `Department`) VALUES (28, 'SP18', 'Radiology', 'Imaging interpretation', 'Radiology');
INSERT INTO `specializations` (`SpecializationID`, `SpecializationCode`, `SpecializationName`, `Description`, `Department`) VALUES (29, 'SP19', 'Anesthesiology', 'Perioperative pain management', 'Anesthesiology');
INSERT INTO `specializations` (`SpecializationID`, `SpecializationCode`, `SpecializationName`, `Description`, `Department`) VALUES (30, 'SP20', 'Family Medicine', 'Continuity outpatient care', 'Family Medicine');
INSERT INTO `specializations` (`SpecializationID`, `SpecializationCode`, `SpecializationName`, `Description`, `Department`) VALUES (31, 'SP21', 'Infectious Disease', 'Complex infection management', 'Infectious Disease');
INSERT INTO `specializations` (`SpecializationID`, `SpecializationCode`, `SpecializationName`, `Description`, `Department`) VALUES (32, 'SP22', 'Rheumatology', 'Autoimmune and joint disorders', 'Rheumatology');
INSERT INTO `specializations` (`SpecializationID`, `SpecializationCode`, `SpecializationName`, `Description`, `Department`) VALUES (33, 'SP23', 'Hematology', 'Blood disorder management', 'Hematology');
INSERT INTO `specializations` (`SpecializationID`, `SpecializationCode`, `SpecializationName`, `Description`, `Department`) VALUES (34, 'SP24', 'Geriatrics', 'Senior care coordination', 'Geriatrics');
INSERT INTO `specializations` (`SpecializationID`, `SpecializationCode`, `SpecializationName`, `Description`, `Department`) VALUES (35, 'SP25', 'Emergency Medicine', 'Emergency department care', 'Emergency');
INSERT INTO `specializations` (`SpecializationID`, `SpecializationCode`, `SpecializationName`, `Description`, `Department`) VALUES (36, 'SP26', 'Rehabilitation Medicine', 'Physical recovery programs', 'Rehabilitation');
INSERT INTO `specializations` (`SpecializationID`, `SpecializationCode`, `SpecializationName`, `Description`, `Department`) VALUES (37, 'SP27', 'Pathology', 'Diagnostic laboratory pathology', 'Pathology');
INSERT INTO `specializations` (`SpecializationID`, `SpecializationCode`, `SpecializationName`, `Description`, `Department`) VALUES (38, 'SP28', 'Obstetrics', 'Pregnancy and delivery care', 'OB-GYN');
INSERT INTO `specializations` (`SpecializationID`, `SpecializationCode`, `SpecializationName`, `Description`, `Department`) VALUES (39, 'SP29', 'Allergy and Immunology', 'Allergy and immune care', 'Allergy');
INSERT INTO `specializations` (`SpecializationID`, `SpecializationCode`, `SpecializationName`, `Description`, `Department`) VALUES (40, 'SP30', 'Pain Management', 'Chronic pain treatment', 'Pain Management');

-- Data for staff
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (3, 7, 'STFSD007', 'Operations Administrator', 'Administration', 'Day', '2025-12-02 00:00:00.000000', 48000.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (4, 8, 'STFSD008', 'Senior Nurse', 'Nursing', 'Night', '2025-12-01 00:00:00.000000', 32000.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (5, 9, 'STFSD009', 'Front Desk Officer', 'Front Desk', 'Day', '2025-11-30 00:00:00.000000', 26000.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (6, 10, 'STFSD010', 'Clinical Pharmacist', 'Pharmacy', 'Day', '2025-11-29 00:00:00.000000', 36000.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (7, 11, 'STFSD011', 'Laboratory Technologist', 'Laboratory', 'Day', '2025-11-28 00:00:00.000000', 34000.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (8, 12, 'STFSD012', 'Accounting Officer', 'Finance', 'Day', '2025-11-27 00:00:00.000000', 38000.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (9, 13, 'STFSD013', 'HR Manager', 'Human Resources', 'Day', '2025-11-26 00:00:00.000000', 41000.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (10, 14, 'STFDD001', 'Consultant Doctor', 'Department 01', 'Day', '2024-12-31 00:00:00.000000', 53200.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (11, 15, 'STFDD002', 'Consultant Doctor', 'Department 02', 'Day', '2024-12-18 00:00:00.000000', 54400.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (12, 16, 'STFDD003', 'Consultant Doctor', 'Department 03', 'Day', '2024-12-05 00:00:00.000000', 55600.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (13, 17, 'STFDD004', 'Consultant Doctor', 'Department 04', 'Day', '2024-11-22 00:00:00.000000', 56800.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (14, 18, 'STFDD005', 'Consultant Doctor', 'Department 05', 'Day', '2024-11-09 00:00:00.000000', 58000.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (15, 19, 'STFDD006', 'Consultant Doctor', 'Department 06', 'Day', '2024-10-27 00:00:00.000000', 59200.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (16, 20, 'STFDD007', 'Consultant Doctor', 'Department 07', 'Day', '2024-10-14 00:00:00.000000', 60400.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (17, 21, 'STFDD008', 'Consultant Doctor', 'Department 08', 'Day', '2024-10-01 00:00:00.000000', 61600.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (18, 22, 'STFDD009', 'Consultant Doctor', 'Department 09', 'Day', '2024-09-18 00:00:00.000000', 62800.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (19, 23, 'STFDD010', 'Consultant Doctor', 'Department 10', 'Day', '2024-09-05 00:00:00.000000', 64000.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (20, 24, 'STFDD011', 'Consultant Doctor', 'Department 11', 'Day', '2024-08-23 00:00:00.000000', 65200.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (21, 25, 'STFDD012', 'Consultant Doctor', 'Department 12', 'Day', '2024-08-10 00:00:00.000000', 66400.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (22, 26, 'STFDD013', 'Consultant Doctor', 'Department 13', 'Day', '2024-07-28 00:00:00.000000', 67600.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (23, 27, 'STFDD014', 'Consultant Doctor', 'Department 14', 'Day', '2024-07-15 00:00:00.000000', 68800.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (24, 28, 'STFDD015', 'Consultant Doctor', 'Department 15', 'Day', '2024-07-02 00:00:00.000000', 70000.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (25, 29, 'STFDD016', 'Consultant Doctor', 'Department 16', 'Day', '2024-06-19 00:00:00.000000', 71200.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (26, 30, 'STFDD017', 'Consultant Doctor', 'Department 17', 'Day', '2024-06-06 00:00:00.000000', 72400.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (27, 31, 'STFDD018', 'Consultant Doctor', 'Department 18', 'Day', '2024-05-24 00:00:00.000000', 73600.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (28, 32, 'STFDD019', 'Consultant Doctor', 'Department 19', 'Day', '2024-05-11 00:00:00.000000', 74800.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (29, 33, 'STFDD020', 'Consultant Doctor', 'Department 20', 'Day', '2024-04-28 00:00:00.000000', 76000.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (30, 34, 'STFDD021', 'Consultant Doctor', 'Department 21', 'Day', '2024-04-15 00:00:00.000000', 77200.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (31, 35, 'STFDD022', 'Consultant Doctor', 'Department 22', 'Day', '2024-04-02 00:00:00.000000', 78400.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (32, 36, 'STFDD023', 'Consultant Doctor', 'Department 23', 'Day', '2024-03-20 00:00:00.000000', 79600.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (33, 37, 'STFDD024', 'Consultant Doctor', 'Department 24', 'Day', '2024-03-07 00:00:00.000000', 80800.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (34, 38, 'STFDD025', 'Consultant Doctor', 'Department 25', 'Day', '2024-02-23 00:00:00.000000', 82000.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (35, 39, 'STFDD026', 'Consultant Doctor', 'Department 26', 'Day', '2024-02-10 00:00:00.000000', 83200.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (36, 40, 'STFDD027', 'Consultant Doctor', 'Department 27', 'Day', '2024-01-28 00:00:00.000000', 84400.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (37, 41, 'STFDD028', 'Consultant Doctor', 'Department 28', 'Day', '2024-01-15 00:00:00.000000', 85600.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (38, 42, 'STFDD029', 'Consultant Doctor', 'Department 29', 'Day', '2024-01-02 00:00:00.000000', 86800.00);
INSERT INTO `staff` (`StaffID`, `UserID`, `StaffCode`, `Designation`, `Department`, `Shift`, `HireDate`, `Salary`) VALUES (39, 43, 'STFDD030', 'Consultant Doctor', 'Department 30', 'Day', '2023-12-20 00:00:00.000000', 88000.00);

-- Data for systemsettings
INSERT INTO `systemsettings` (`SettingID`, `SettingKey`, `SettingValue`, `Description`, `Category`, `LastModified`) VALUES (27, 'Seed.Setting.01', 'Value-01', 'Seed system setting 01.', 'Billing', '2026-03-09 15:55:09.000000');
INSERT INTO `systemsettings` (`SettingID`, `SettingKey`, `SettingValue`, `Description`, `Category`, `LastModified`) VALUES (28, 'Seed.Setting.02', 'Value-02', 'Seed system setting 02.', 'General', '2026-03-09 15:55:09.000000');
INSERT INTO `systemsettings` (`SettingID`, `SettingKey`, `SettingValue`, `Description`, `Category`, `LastModified`) VALUES (29, 'Seed.Setting.03', 'Value-03', 'Seed system setting 03.', 'Billing', '2026-03-09 15:55:09.000000');
INSERT INTO `systemsettings` (`SettingID`, `SettingKey`, `SettingValue`, `Description`, `Category`, `LastModified`) VALUES (30, 'Seed.Setting.04', 'Value-04', 'Seed system setting 04.', 'General', '2026-03-09 15:55:09.000000');
INSERT INTO `systemsettings` (`SettingID`, `SettingKey`, `SettingValue`, `Description`, `Category`, `LastModified`) VALUES (31, 'Seed.Setting.05', 'Value-05', 'Seed system setting 05.', 'Billing', '2026-03-09 15:55:09.000000');
INSERT INTO `systemsettings` (`SettingID`, `SettingKey`, `SettingValue`, `Description`, `Category`, `LastModified`) VALUES (32, 'Seed.Setting.06', 'Value-06', 'Seed system setting 06.', 'General', '2026-03-09 15:55:09.000000');
INSERT INTO `systemsettings` (`SettingID`, `SettingKey`, `SettingValue`, `Description`, `Category`, `LastModified`) VALUES (33, 'Seed.Setting.07', 'Value-07', 'Seed system setting 07.', 'Billing', '2026-03-09 15:55:10.000000');
INSERT INTO `systemsettings` (`SettingID`, `SettingKey`, `SettingValue`, `Description`, `Category`, `LastModified`) VALUES (34, 'Seed.Setting.08', 'Value-08', 'Seed system setting 08.', 'General', '2026-03-09 15:55:10.000000');
INSERT INTO `systemsettings` (`SettingID`, `SettingKey`, `SettingValue`, `Description`, `Category`, `LastModified`) VALUES (35, 'Seed.Setting.09', 'Value-09', 'Seed system setting 09.', 'Billing', '2026-03-09 15:55:10.000000');
INSERT INTO `systemsettings` (`SettingID`, `SettingKey`, `SettingValue`, `Description`, `Category`, `LastModified`) VALUES (36, 'Seed.Setting.10', 'Value-10', 'Seed system setting 10.', 'General', '2026-03-09 15:55:10.000000');
INSERT INTO `systemsettings` (`SettingID`, `SettingKey`, `SettingValue`, `Description`, `Category`, `LastModified`) VALUES (37, 'Seed.Setting.11', 'Value-11', 'Seed system setting 11.', 'Billing', '2026-03-09 15:55:10.000000');
INSERT INTO `systemsettings` (`SettingID`, `SettingKey`, `SettingValue`, `Description`, `Category`, `LastModified`) VALUES (38, 'Seed.Setting.12', 'Value-12', 'Seed system setting 12.', 'General', '2026-03-09 15:55:10.000000');
INSERT INTO `systemsettings` (`SettingID`, `SettingKey`, `SettingValue`, `Description`, `Category`, `LastModified`) VALUES (39, 'Seed.Setting.13', 'Value-13', 'Seed system setting 13.', 'Billing', '2026-03-09 15:55:10.000000');
INSERT INTO `systemsettings` (`SettingID`, `SettingKey`, `SettingValue`, `Description`, `Category`, `LastModified`) VALUES (40, 'Seed.Setting.14', 'Value-14', 'Seed system setting 14.', 'General', '2026-03-09 15:55:11.000000');
INSERT INTO `systemsettings` (`SettingID`, `SettingKey`, `SettingValue`, `Description`, `Category`, `LastModified`) VALUES (41, 'Seed.Setting.15', 'Value-15', 'Seed system setting 15.', 'Billing', '2026-03-09 15:55:11.000000');
INSERT INTO `systemsettings` (`SettingID`, `SettingKey`, `SettingValue`, `Description`, `Category`, `LastModified`) VALUES (42, 'Seed.Setting.16', 'Value-16', 'Seed system setting 16.', 'General', '2026-03-09 15:55:11.000000');
INSERT INTO `systemsettings` (`SettingID`, `SettingKey`, `SettingValue`, `Description`, `Category`, `LastModified`) VALUES (43, 'Seed.Setting.17', 'Value-17', 'Seed system setting 17.', 'Billing', '2026-03-09 15:55:11.000000');
INSERT INTO `systemsettings` (`SettingID`, `SettingKey`, `SettingValue`, `Description`, `Category`, `LastModified`) VALUES (44, 'Seed.Setting.18', 'Value-18', 'Seed system setting 18.', 'General', '2026-03-09 15:55:11.000000');
INSERT INTO `systemsettings` (`SettingID`, `SettingKey`, `SettingValue`, `Description`, `Category`, `LastModified`) VALUES (45, 'Seed.Setting.19', 'Value-19', 'Seed system setting 19.', 'Billing', '2026-03-09 15:55:11.000000');
INSERT INTO `systemsettings` (`SettingID`, `SettingKey`, `SettingValue`, `Description`, `Category`, `LastModified`) VALUES (46, 'Seed.Setting.20', 'Value-20', 'Seed system setting 20.', 'General', '2026-03-09 15:55:12.000000');
INSERT INTO `systemsettings` (`SettingID`, `SettingKey`, `SettingValue`, `Description`, `Category`, `LastModified`) VALUES (47, 'Seed.Setting.21', 'Value-21', 'Seed system setting 21.', 'Billing', '2026-03-09 15:55:12.000000');
INSERT INTO `systemsettings` (`SettingID`, `SettingKey`, `SettingValue`, `Description`, `Category`, `LastModified`) VALUES (48, 'Seed.Setting.22', 'Value-22', 'Seed system setting 22.', 'General', '2026-03-09 15:55:12.000000');
INSERT INTO `systemsettings` (`SettingID`, `SettingKey`, `SettingValue`, `Description`, `Category`, `LastModified`) VALUES (49, 'Seed.Setting.23', 'Value-23', 'Seed system setting 23.', 'Billing', '2026-03-09 15:55:12.000000');
INSERT INTO `systemsettings` (`SettingID`, `SettingKey`, `SettingValue`, `Description`, `Category`, `LastModified`) VALUES (50, 'Seed.Setting.24', 'Value-24', 'Seed system setting 24.', 'General', '2026-03-09 15:55:12.000000');
INSERT INTO `systemsettings` (`SettingID`, `SettingKey`, `SettingValue`, `Description`, `Category`, `LastModified`) VALUES (51, 'Seed.Setting.25', 'Value-25', 'Seed system setting 25.', 'Billing', '2026-03-09 15:55:12.000000');
INSERT INTO `systemsettings` (`SettingID`, `SettingKey`, `SettingValue`, `Description`, `Category`, `LastModified`) VALUES (52, 'Seed.Setting.26', 'Value-26', 'Seed system setting 26.', 'General', '2026-03-09 15:55:12.000000');
INSERT INTO `systemsettings` (`SettingID`, `SettingKey`, `SettingValue`, `Description`, `Category`, `LastModified`) VALUES (53, 'Seed.Setting.27', 'Value-27', 'Seed system setting 27.', 'Billing', '2026-03-09 15:55:13.000000');
INSERT INTO `systemsettings` (`SettingID`, `SettingKey`, `SettingValue`, `Description`, `Category`, `LastModified`) VALUES (54, 'Seed.Setting.28', 'Value-28', 'Seed system setting 28.', 'General', '2026-03-09 15:55:13.000000');
INSERT INTO `systemsettings` (`SettingID`, `SettingKey`, `SettingValue`, `Description`, `Category`, `LastModified`) VALUES (55, 'Seed.Setting.29', 'Value-29', 'Seed system setting 29.', 'Billing', '2026-03-09 15:55:13.000000');
INSERT INTO `systemsettings` (`SettingID`, `SettingKey`, `SettingValue`, `Description`, `Category`, `LastModified`) VALUES (56, 'Seed.Setting.30', 'Value-30', 'Seed system setting 30.', 'General', '2026-03-09 15:55:13.000000');

-- Data for userdetails
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (6, 7, 'Marian', 'Velasco', '1996-03-16 00:00:00.000000', 'F', '09900000007', 'Seed Address Velasco, Davao City', '09800000007', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (7, 8, 'Clarence', 'Rosario', '1996-03-17 00:00:00.000000', 'M', '09900000008', 'Seed Address Rosario, Davao City', '09800000008', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (8, 9, 'Aira', 'Lopez', '1996-03-18 00:00:00.000000', 'F', '09900000009', 'Seed Address Lopez, Davao City', '09800000009', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (9, 10, 'Neil', 'Sarmiento', '1996-03-19 00:00:00.000000', 'M', '09900000010', 'Seed Address Sarmiento, Davao City', '09800000010', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (10, 11, 'Daphne', 'Mercado', '1996-03-20 00:00:00.000000', 'F', '09900000011', 'Seed Address Mercado, Davao City', '09800000011', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (11, 12, 'Jonas', 'Reyes', '1996-03-21 00:00:00.000000', 'M', '09900000012', 'Seed Address Reyes, Davao City', '09800000012', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (12, 13, 'Karla', 'Navarro', '1996-03-22 00:00:00.000000', 'F', '09900000013', 'Seed Address Navarro, Davao City', '09800000013', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (13, 14, 'Ramon', 'Alegre', '1988-06-27 00:00:00.000000', 'M', '09700154321', 'Seed Doctor Address 01, Davao Region', '09600123456', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (14, 15, 'Liza', 'Cabahug', '1988-10-15 00:00:00.000000', 'M', '09700308642', 'Seed Doctor Address 02, Davao Region', '09600246912', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (15, 16, 'Edwin', 'Carandang', '1989-02-02 00:00:00.000000', 'F', '09700462963', 'Seed Doctor Address 03, Davao Region', '09600370368', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (16, 17, 'Rica', 'Matias', '1989-05-23 00:00:00.000000', 'M', '09700617284', 'Seed Doctor Address 04, Davao Region', '09600493824', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (17, 18, 'Marjorie', 'Talavera', '1989-09-10 00:00:00.000000', 'M', '09700771605', 'Seed Doctor Address 05, Davao Region', '09600617280', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (18, 19, 'Noel', 'Panganiban', '1989-12-29 00:00:00.000000', 'F', '09700925926', 'Seed Doctor Address 06, Davao Region', '09600740736', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (19, 20, 'Jessa', 'Villarta', '1990-04-18 00:00:00.000000', 'M', '09701080247', 'Seed Doctor Address 07, Davao Region', '09600864192', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (20, 21, 'Arnel', 'Quimpo', '1990-08-06 00:00:00.000000', 'M', '09701234568', 'Seed Doctor Address 08, Davao Region', '09600987648', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (21, 22, 'Kathlyn', 'Bermudez', '1990-11-24 00:00:00.000000', 'F', '09701388889', 'Seed Doctor Address 09, Davao Region', '09601111104', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (22, 23, 'Paolo', 'Serrano', '1991-03-14 00:00:00.000000', 'M', '09701543210', 'Seed Doctor Address 10, Davao Region', '09601234560', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (23, 24, 'Irene', 'Navarro', '1991-07-02 00:00:00.000000', 'M', '09701697531', 'Seed Doctor Address 11, Davao Region', '09601358016', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (24, 25, 'Victor', 'Mercado', '1991-10-20 00:00:00.000000', 'F', '09701851852', 'Seed Doctor Address 12, Davao Region', '09601481472', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (25, 26, 'Grace', 'Aquino', '1992-02-07 00:00:00.000000', 'M', '09702006173', 'Seed Doctor Address 13, Davao Region', '09601604928', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (26, 27, 'Alberto', 'David', '1992-05-27 00:00:00.000000', 'M', '09702160494', 'Seed Doctor Address 14, Davao Region', '09601728384', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (27, 28, 'Janice', 'Rosales', '1992-09-14 00:00:00.000000', 'F', '09702314815', 'Seed Doctor Address 15, Davao Region', '09601851840', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (28, 29, 'Marlon', 'Dizon', '1993-01-02 00:00:00.000000', 'M', '09702469136', 'Seed Doctor Address 16, Davao Region', '09601975296', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (29, 30, 'Hazel', 'Abad', '1993-04-22 00:00:00.000000', 'M', '09702623457', 'Seed Doctor Address 17, Davao Region', '09602098752', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (30, 31, 'Tyrone', 'Lopez', '1993-08-10 00:00:00.000000', 'F', '09702777778', 'Seed Doctor Address 18, Davao Region', '09602222208', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (31, 32, 'Lucille', 'Reyes', '1993-11-28 00:00:00.000000', 'M', '09702932099', 'Seed Doctor Address 19, Davao Region', '09602345664', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (32, 33, 'Dennis', 'Samonte', '1994-03-18 00:00:00.000000', 'M', '09703086420', 'Seed Doctor Address 20, Davao Region', '09602469120', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (33, 34, 'Mica', 'Salazar', '1994-07-06 00:00:00.000000', 'F', '09703240741', 'Seed Doctor Address 21, Davao Region', '09602592576', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (34, 35, 'Felix', 'Domingo', '1994-10-24 00:00:00.000000', 'M', '09703395062', 'Seed Doctor Address 22, Davao Region', '09602716032', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (35, 36, 'Jocelyn', 'Padilla', '1995-02-11 00:00:00.000000', 'M', '09703549383', 'Seed Doctor Address 23, Davao Region', '09602839488', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (36, 37, 'Carlo', 'Manalo', '1995-06-01 00:00:00.000000', 'F', '09703703704', 'Seed Doctor Address 24, Davao Region', '09602962944', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (37, 38, 'Noreen', 'Cabrera', '1995-09-19 00:00:00.000000', 'M', '09703858025', 'Seed Doctor Address 25, Davao Region', '09603086400', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (38, 39, 'Jerome', 'Natividad', '1996-01-07 00:00:00.000000', 'M', '09704012346', 'Seed Doctor Address 26, Davao Region', '09603209856', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (39, 40, 'Patricia', 'Trinidad', '1996-04-26 00:00:00.000000', 'F', '09704166667', 'Seed Doctor Address 27, Davao Region', '09603333312', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (40, 41, 'Allan', 'Ocampo', '1996-08-14 00:00:00.000000', 'M', '09704320988', 'Seed Doctor Address 28, Davao Region', '09603456768', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (41, 42, 'Shaina', 'Fernandez', '1996-12-02 00:00:00.000000', 'M', '09704475309', 'Seed Doctor Address 29, Davao Region', '09603580224', NULL);
INSERT INTO `userdetails` (`UserDetailID`, `UserID`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `ContactNumber`, `Address`, `EmergencyContact`, `ProfileImage`) VALUES (42, 43, 'Roderick', 'Lazaro', '1997-03-22 00:00:00.000000', 'F', '09704629630', 'Seed Doctor Address 30, Davao Region', '09603703680', NULL);

-- Data for userroles
INSERT INTO `userroles` (`RoleID`, `RoleName`, `Description`, `CreatedDate`) VALUES (9, 'Administrator', 'Full system access', '2026-03-09 11:50:02.000000');
INSERT INTO `userroles` (`RoleID`, `RoleName`, `Description`, `CreatedDate`) VALUES (10, 'SuperAdmin', 'Installation super administrator', '2026-03-09 11:50:02.000000');
INSERT INTO `userroles` (`RoleID`, `RoleName`, `Description`, `CreatedDate`) VALUES (11, 'Doctor', 'Medical staff', '2026-03-09 11:50:02.000000');
INSERT INTO `userroles` (`RoleID`, `RoleName`, `Description`, `CreatedDate`) VALUES (12, 'Nurse', 'Nursing staff', '2026-03-09 11:50:03.000000');
INSERT INTO `userroles` (`RoleID`, `RoleName`, `Description`, `CreatedDate`) VALUES (13, 'Receptionist', 'Front desk', '2026-03-09 11:50:03.000000');
INSERT INTO `userroles` (`RoleID`, `RoleName`, `Description`, `CreatedDate`) VALUES (14, 'Pharmacist', 'Pharmacy management', '2026-03-09 11:50:03.000000');
INSERT INTO `userroles` (`RoleID`, `RoleName`, `Description`, `CreatedDate`) VALUES (15, 'Lab Technician', 'Laboratory test management', '2026-03-09 11:50:03.000000');
INSERT INTO `userroles` (`RoleID`, `RoleName`, `Description`, `CreatedDate`) VALUES (16, 'Accountant', 'Billing and finance', '2026-03-09 11:50:03.000000');
INSERT INTO `userroles` (`RoleID`, `RoleName`, `Description`, `CreatedDate`) VALUES (17, 'HR Manager', 'Human resources', '2026-03-09 11:50:03.000000');
INSERT INTO `userroles` (`RoleID`, `RoleName`, `Description`, `CreatedDate`) VALUES (18, 'Seed Role 01', 'Generated sample role 01.', '2026-03-09 15:54:10.000000');
INSERT INTO `userroles` (`RoleID`, `RoleName`, `Description`, `CreatedDate`) VALUES (19, 'Seed Role 02', 'Generated sample role 02.', '2026-03-09 15:54:10.000000');
INSERT INTO `userroles` (`RoleID`, `RoleName`, `Description`, `CreatedDate`) VALUES (20, 'Seed Role 03', 'Generated sample role 03.', '2026-03-09 15:54:11.000000');
INSERT INTO `userroles` (`RoleID`, `RoleName`, `Description`, `CreatedDate`) VALUES (21, 'Seed Role 04', 'Generated sample role 04.', '2026-03-09 15:54:11.000000');
INSERT INTO `userroles` (`RoleID`, `RoleName`, `Description`, `CreatedDate`) VALUES (22, 'Seed Role 05', 'Generated sample role 05.', '2026-03-09 15:54:11.000000');
INSERT INTO `userroles` (`RoleID`, `RoleName`, `Description`, `CreatedDate`) VALUES (23, 'Seed Role 06', 'Generated sample role 06.', '2026-03-09 15:54:11.000000');
INSERT INTO `userroles` (`RoleID`, `RoleName`, `Description`, `CreatedDate`) VALUES (24, 'Seed Role 07', 'Generated sample role 07.', '2026-03-09 15:54:12.000000');
INSERT INTO `userroles` (`RoleID`, `RoleName`, `Description`, `CreatedDate`) VALUES (25, 'Seed Role 08', 'Generated sample role 08.', '2026-03-09 15:54:12.000000');
INSERT INTO `userroles` (`RoleID`, `RoleName`, `Description`, `CreatedDate`) VALUES (26, 'Seed Role 09', 'Generated sample role 09.', '2026-03-09 15:54:12.000000');
INSERT INTO `userroles` (`RoleID`, `RoleName`, `Description`, `CreatedDate`) VALUES (27, 'Seed Role 10', 'Generated sample role 10.', '2026-03-09 15:54:12.000000');
INSERT INTO `userroles` (`RoleID`, `RoleName`, `Description`, `CreatedDate`) VALUES (28, 'Seed Role 11', 'Generated sample role 11.', '2026-03-09 15:54:12.000000');
INSERT INTO `userroles` (`RoleID`, `RoleName`, `Description`, `CreatedDate`) VALUES (29, 'Seed Role 12', 'Generated sample role 12.', '2026-03-09 15:54:12.000000');
INSERT INTO `userroles` (`RoleID`, `RoleName`, `Description`, `CreatedDate`) VALUES (30, 'Seed Role 13', 'Generated sample role 13.', '2026-03-09 15:54:13.000000');
INSERT INTO `userroles` (`RoleID`, `RoleName`, `Description`, `CreatedDate`) VALUES (31, 'Seed Role 14', 'Generated sample role 14.', '2026-03-09 15:54:13.000000');
INSERT INTO `userroles` (`RoleID`, `RoleName`, `Description`, `CreatedDate`) VALUES (32, 'Seed Role 15', 'Generated sample role 15.', '2026-03-09 15:54:13.000000');
INSERT INTO `userroles` (`RoleID`, `RoleName`, `Description`, `CreatedDate`) VALUES (33, 'Seed Role 16', 'Generated sample role 16.', '2026-03-09 15:54:13.000000');
INSERT INTO `userroles` (`RoleID`, `RoleName`, `Description`, `CreatedDate`) VALUES (34, 'Seed Role 17', 'Generated sample role 17.', '2026-03-09 15:54:13.000000');
INSERT INTO `userroles` (`RoleID`, `RoleName`, `Description`, `CreatedDate`) VALUES (35, 'Seed Role 18', 'Generated sample role 18.', '2026-03-09 15:54:13.000000');
INSERT INTO `userroles` (`RoleID`, `RoleName`, `Description`, `CreatedDate`) VALUES (36, 'Seed Role 19', 'Generated sample role 19.', '2026-03-09 15:54:14.000000');
INSERT INTO `userroles` (`RoleID`, `RoleName`, `Description`, `CreatedDate`) VALUES (37, 'Seed Role 20', 'Generated sample role 20.', '2026-03-09 15:54:14.000000');
INSERT INTO `userroles` (`RoleID`, `RoleName`, `Description`, `CreatedDate`) VALUES (38, 'Seed Role 21', 'Generated sample role 21.', '2026-03-09 15:54:14.000000');

-- Data for users
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (6, 'admin', 'PBKDF2$100000$dwiR4XFlErznO4KvI6eiQQ==$zklNshmjDuYAx1cfxEPWT5LbkjTw8g5UD7C8CQJEt8w=', 'admin@hospital.local', 10, 1, '2026-03-10 14:24:18.000000', '2026-03-09 11:50:03.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (7, 'seed.admin', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.admin@hospital.local', 9, 1, '2026-03-07 23:55:12.000000', '2026-03-09 15:55:13.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (8, 'seed.nurse', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.nurse@hospital.local', 12, 1, '2026-03-07 23:55:12.000000', '2026-03-09 15:55:14.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (9, 'seed.reception', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.reception@hospital.local', 13, 1, '2026-03-07 23:55:13.000000', '2026-03-09 15:55:14.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (10, 'seed.pharmacist', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.pharmacist@hospital.local', 14, 1, '2026-03-07 23:55:13.000000', '2026-03-09 15:55:15.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (11, 'seed.labtech', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.labtech@hospital.local', 15, 1, '2026-03-07 23:55:14.000000', '2026-03-09 15:55:15.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (12, 'seed.accountant', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.accountant@hospital.local', 16, 1, '2026-03-07 23:55:14.000000', '2026-03-09 15:55:16.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (13, 'seed.hr', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.hr@hospital.local', 17, 1, '2026-03-07 23:55:15.000000', '2026-03-09 15:55:16.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (14, 'seed.doctor01', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.doctor01@hospital.local', 11, 1, '2026-03-08 23:55:16.000000', '2026-03-09 15:55:17.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (15, 'seed.doctor02', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.doctor02@hospital.local', 11, 1, '2026-03-08 23:55:17.000000', '2026-03-09 15:55:18.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (16, 'seed.doctor03', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.doctor03@hospital.local', 11, 1, '2026-03-08 23:55:18.000000', '2026-03-09 15:55:19.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (17, 'seed.doctor04', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.doctor04@hospital.local', 11, 1, '2026-03-08 23:55:19.000000', '2026-03-09 15:55:20.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (18, 'seed.doctor05', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.doctor05@hospital.local', 11, 1, '2026-03-08 23:55:20.000000', '2026-03-09 15:55:22.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (19, 'seed.doctor06', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.doctor06@hospital.local', 11, 1, '2026-03-08 23:55:22.000000', '2026-03-09 15:55:23.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (20, 'seed.doctor07', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.doctor07@hospital.local', 11, 1, '2026-03-08 23:55:23.000000', '2026-03-09 15:55:25.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (21, 'seed.doctor08', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.doctor08@hospital.local', 11, 1, '2026-03-08 23:55:25.000000', '2026-03-09 15:55:26.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (22, 'seed.doctor09', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.doctor09@hospital.local', 11, 1, '2026-03-08 23:55:26.000000', '2026-03-09 15:55:27.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (23, 'seed.doctor10', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.doctor10@hospital.local', 11, 1, '2026-03-08 23:55:27.000000', '2026-03-09 15:55:28.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (24, 'seed.doctor11', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.doctor11@hospital.local', 11, 1, '2026-03-08 23:55:28.000000', '2026-03-09 15:55:30.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (25, 'seed.doctor12', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.doctor12@hospital.local', 11, 1, '2026-03-08 23:55:29.000000', '2026-03-09 15:55:31.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (26, 'seed.doctor13', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.doctor13@hospital.local', 11, 1, '2026-03-08 23:55:32.000000', '2026-03-09 15:55:33.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (27, 'seed.doctor14', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.doctor14@hospital.local', 11, 1, '2026-03-08 23:55:34.000000', '2026-03-09 15:55:35.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (28, 'seed.doctor15', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.doctor15@hospital.local', 11, 1, '2026-03-08 23:55:35.000000', '2026-03-09 15:55:36.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (29, 'seed.doctor16', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.doctor16@hospital.local', 11, 1, '2026-03-08 23:55:38.000000', '2026-03-09 15:55:39.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (30, 'seed.doctor17', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.doctor17@hospital.local', 11, 1, '2026-03-08 23:55:41.000000', '2026-03-09 15:55:42.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (31, 'seed.doctor18', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.doctor18@hospital.local', 11, 1, '2026-03-08 23:55:42.000000', '2026-03-09 15:55:43.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (32, 'seed.doctor19', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.doctor19@hospital.local', 11, 1, '2026-03-08 23:55:44.000000', '2026-03-09 15:55:46.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (33, 'seed.doctor20', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.doctor20@hospital.local', 11, 1, '2026-03-08 23:55:47.000000', '2026-03-09 15:55:48.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (34, 'seed.doctor21', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.doctor21@hospital.local', 11, 1, '2026-03-08 23:55:48.000000', '2026-03-09 15:55:50.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (35, 'seed.doctor22', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.doctor22@hospital.local', 11, 1, '2026-03-08 23:55:51.000000', '2026-03-09 15:55:52.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (36, 'seed.doctor23', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.doctor23@hospital.local', 11, 1, '2026-03-08 23:55:52.000000', '2026-03-09 15:55:54.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (37, 'seed.doctor24', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.doctor24@hospital.local', 11, 1, '2026-03-08 23:55:54.000000', '2026-03-09 15:55:55.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (38, 'seed.doctor25', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.doctor25@hospital.local', 11, 1, '2026-03-08 23:55:55.000000', '2026-03-09 15:55:56.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (39, 'seed.doctor26', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.doctor26@hospital.local', 11, 1, '2026-03-08 23:55:56.000000', '2026-03-09 15:55:57.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (40, 'seed.doctor27', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.doctor27@hospital.local', 11, 1, '2026-03-08 23:55:58.000000', '2026-03-09 15:55:59.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (41, 'seed.doctor28', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.doctor28@hospital.local', 11, 1, '2026-03-08 23:55:59.000000', '2026-03-09 15:56:00.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (42, 'seed.doctor29', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.doctor29@hospital.local', 11, 1, '2026-03-08 23:56:01.000000', '2026-03-09 15:56:02.000000');
INSERT INTO `users` (`UserID`, `Username`, `PasswordHash`, `Email`, `RoleID`, `IsActive`, `LastLogin`, `CreatedDate`) VALUES (43, 'seed.doctor30', 'PBKDF2$100000$1H45/mJRchCF1b+4+6yv+g==$WcL063zi+riMUsso1wj+vSODCWxQTZqd9HQsSbAFkUI=', 'seed.doctor30@hospital.local', 11, 1, '2026-03-08 23:56:02.000000', '2026-03-09 15:56:04.000000');

-- Data for visits
INSERT INTO `visits` (`VisitID`, `VisitCode`, `PatientID`, `DoctorID`, `AppointmentID`, `VisitDate`, `Symptoms`, `Diagnosis`, `Treatment`, `FollowUpDate`, `VisitStatus`, `CreatedBy`) VALUES (2, 'VISSD001', 2, 3, 2, '2026-02-06 11:00:00.000000', 'Seed symptoms entry 01', 'Essential hypertension', 'Treatment plan 01 with monitoring instructions.', '2026-02-20 00:00:00.000000', 'Completed', 7);
INSERT INTO `visits` (`VisitID`, `VisitCode`, `PatientID`, `DoctorID`, `AppointmentID`, `VisitDate`, `Symptoms`, `Diagnosis`, `Treatment`, `FollowUpDate`, `VisitStatus`, `CreatedBy`) VALUES (3, 'VISSD002', 3, 4, 3, '2026-02-07 12:00:00.000000', 'Seed symptoms entry 02', 'Type 2 diabetes mellitus', 'Treatment plan 02 with monitoring instructions.', '2026-02-21 00:00:00.000000', 'Completed', 7);
INSERT INTO `visits` (`VisitID`, `VisitCode`, `PatientID`, `DoctorID`, `AppointmentID`, `VisitDate`, `Symptoms`, `Diagnosis`, `Treatment`, `FollowUpDate`, `VisitStatus`, `CreatedBy`) VALUES (4, 'VISSD003', 4, 5, 4, '2026-02-08 13:00:00.000000', 'Seed symptoms entry 03', 'Bronchial asthma', 'Treatment plan 03 with monitoring instructions.', '2026-02-22 00:00:00.000000', 'Completed', 7);
INSERT INTO `visits` (`VisitID`, `VisitCode`, `PatientID`, `DoctorID`, `AppointmentID`, `VisitDate`, `Symptoms`, `Diagnosis`, `Treatment`, `FollowUpDate`, `VisitStatus`, `CreatedBy`) VALUES (5, 'VISSD004', 5, 6, 5, '2026-02-09 14:00:00.000000', 'Seed symptoms entry 04', 'Hyperlipidemia', 'Treatment plan 04 with monitoring instructions.', NULL, 'Completed', 7);
INSERT INTO `visits` (`VisitID`, `VisitCode`, `PatientID`, `DoctorID`, `AppointmentID`, `VisitDate`, `Symptoms`, `Diagnosis`, `Treatment`, `FollowUpDate`, `VisitStatus`, `CreatedBy`) VALUES (6, 'VISSD005', 6, 7, 6, '2026-02-10 10:00:00.000000', 'Seed symptoms entry 05', 'Migraine episodes', 'Treatment plan 05 with monitoring instructions.', '2026-02-24 00:00:00.000000', 'Completed', 7);
INSERT INTO `visits` (`VisitID`, `VisitCode`, `PatientID`, `DoctorID`, `AppointmentID`, `VisitDate`, `Symptoms`, `Diagnosis`, `Treatment`, `FollowUpDate`, `VisitStatus`, `CreatedBy`) VALUES (7, 'VISSD006', 7, 8, 7, '2026-02-11 11:00:00.000000', 'Seed symptoms entry 06', 'Allergic rhinitis', 'Treatment plan 06 with monitoring instructions.', '2026-02-25 00:00:00.000000', 'Completed', 7);
INSERT INTO `visits` (`VisitID`, `VisitCode`, `PatientID`, `DoctorID`, `AppointmentID`, `VisitDate`, `Symptoms`, `Diagnosis`, `Treatment`, `FollowUpDate`, `VisitStatus`, `CreatedBy`) VALUES (8, 'VISSD007', 8, 9, 8, '2026-02-12 12:00:00.000000', 'Seed symptoms entry 07', 'Lumbar strain', 'Treatment plan 07 with monitoring instructions.', '2026-02-26 00:00:00.000000', 'Completed', 7);
INSERT INTO `visits` (`VisitID`, `VisitCode`, `PatientID`, `DoctorID`, `AppointmentID`, `VisitDate`, `Symptoms`, `Diagnosis`, `Treatment`, `FollowUpDate`, `VisitStatus`, `CreatedBy`) VALUES (9, 'VISSD008', 9, 10, 9, '2026-02-13 13:00:00.000000', 'Seed symptoms entry 08', 'Osteoarthritis', 'Treatment plan 08 with monitoring instructions.', NULL, 'Completed', 7);
INSERT INTO `visits` (`VisitID`, `VisitCode`, `PatientID`, `DoctorID`, `AppointmentID`, `VisitDate`, `Symptoms`, `Diagnosis`, `Treatment`, `FollowUpDate`, `VisitStatus`, `CreatedBy`) VALUES (10, 'VISSD009', 10, 11, 10, '2026-02-14 14:00:00.000000', 'Seed symptoms entry 09', 'Acute gastroenteritis', 'Treatment plan 09 with monitoring instructions.', '2026-02-28 00:00:00.000000', 'Completed', 7);
INSERT INTO `visits` (`VisitID`, `VisitCode`, `PatientID`, `DoctorID`, `AppointmentID`, `VisitDate`, `Symptoms`, `Diagnosis`, `Treatment`, `FollowUpDate`, `VisitStatus`, `CreatedBy`) VALUES (11, 'VISSD010', 11, 12, 11, '2026-02-15 10:00:00.000000', 'Seed symptoms entry 10', 'Urinary tract infection', 'Treatment plan 10 with monitoring instructions.', '2026-03-01 00:00:00.000000', 'Completed', 7);
INSERT INTO `visits` (`VisitID`, `VisitCode`, `PatientID`, `DoctorID`, `AppointmentID`, `VisitDate`, `Symptoms`, `Diagnosis`, `Treatment`, `FollowUpDate`, `VisitStatus`, `CreatedBy`) VALUES (12, 'VISSD011', 12, 13, 12, '2026-02-16 11:00:00.000000', 'Seed symptoms entry 11', 'Essential hypertension', 'Treatment plan 11 with monitoring instructions.', '2026-03-02 00:00:00.000000', 'Completed', 7);
INSERT INTO `visits` (`VisitID`, `VisitCode`, `PatientID`, `DoctorID`, `AppointmentID`, `VisitDate`, `Symptoms`, `Diagnosis`, `Treatment`, `FollowUpDate`, `VisitStatus`, `CreatedBy`) VALUES (13, 'VISSD012', 13, 14, 13, '2026-02-17 12:00:00.000000', 'Seed symptoms entry 12', 'Type 2 diabetes mellitus', 'Treatment plan 12 with monitoring instructions.', NULL, 'Completed', 7);
INSERT INTO `visits` (`VisitID`, `VisitCode`, `PatientID`, `DoctorID`, `AppointmentID`, `VisitDate`, `Symptoms`, `Diagnosis`, `Treatment`, `FollowUpDate`, `VisitStatus`, `CreatedBy`) VALUES (14, 'VISSD013', 14, 15, 14, '2026-02-18 13:00:00.000000', 'Seed symptoms entry 13', 'Bronchial asthma', 'Treatment plan 13 with monitoring instructions.', '2026-03-04 00:00:00.000000', 'Completed', 7);
INSERT INTO `visits` (`VisitID`, `VisitCode`, `PatientID`, `DoctorID`, `AppointmentID`, `VisitDate`, `Symptoms`, `Diagnosis`, `Treatment`, `FollowUpDate`, `VisitStatus`, `CreatedBy`) VALUES (15, 'VISSD014', 15, 16, 15, '2026-02-19 14:00:00.000000', 'Seed symptoms entry 14', 'Hyperlipidemia', 'Treatment plan 14 with monitoring instructions.', '2026-03-05 00:00:00.000000', 'Completed', 7);
INSERT INTO `visits` (`VisitID`, `VisitCode`, `PatientID`, `DoctorID`, `AppointmentID`, `VisitDate`, `Symptoms`, `Diagnosis`, `Treatment`, `FollowUpDate`, `VisitStatus`, `CreatedBy`) VALUES (16, 'VISSD015', 16, 17, 16, '2026-02-20 10:00:00.000000', 'Seed symptoms entry 15', 'Migraine episodes', 'Treatment plan 15 with monitoring instructions.', '2026-03-06 00:00:00.000000', 'Completed', 7);
INSERT INTO `visits` (`VisitID`, `VisitCode`, `PatientID`, `DoctorID`, `AppointmentID`, `VisitDate`, `Symptoms`, `Diagnosis`, `Treatment`, `FollowUpDate`, `VisitStatus`, `CreatedBy`) VALUES (17, 'VISSD016', 17, 18, 17, '2026-02-21 11:00:00.000000', 'Seed symptoms entry 16', 'Allergic rhinitis', 'Treatment plan 16 with monitoring instructions.', NULL, 'Completed', 7);
INSERT INTO `visits` (`VisitID`, `VisitCode`, `PatientID`, `DoctorID`, `AppointmentID`, `VisitDate`, `Symptoms`, `Diagnosis`, `Treatment`, `FollowUpDate`, `VisitStatus`, `CreatedBy`) VALUES (18, 'VISSD017', 18, 19, 18, '2026-02-22 12:00:00.000000', 'Seed symptoms entry 17', 'Lumbar strain', 'Treatment plan 17 with monitoring instructions.', '2026-03-08 00:00:00.000000', 'Completed', 7);
INSERT INTO `visits` (`VisitID`, `VisitCode`, `PatientID`, `DoctorID`, `AppointmentID`, `VisitDate`, `Symptoms`, `Diagnosis`, `Treatment`, `FollowUpDate`, `VisitStatus`, `CreatedBy`) VALUES (19, 'VISSD018', 19, 20, 19, '2026-02-23 13:00:00.000000', 'Seed symptoms entry 18', 'Osteoarthritis', 'Treatment plan 18 with monitoring instructions.', '2026-03-09 00:00:00.000000', 'Completed', 7);
INSERT INTO `visits` (`VisitID`, `VisitCode`, `PatientID`, `DoctorID`, `AppointmentID`, `VisitDate`, `Symptoms`, `Diagnosis`, `Treatment`, `FollowUpDate`, `VisitStatus`, `CreatedBy`) VALUES (20, 'VISSD019', 20, 21, 20, '2026-02-24 14:00:00.000000', 'Seed symptoms entry 19', 'Acute gastroenteritis', 'Treatment plan 19 with monitoring instructions.', '2026-03-10 00:00:00.000000', 'Completed', 7);
INSERT INTO `visits` (`VisitID`, `VisitCode`, `PatientID`, `DoctorID`, `AppointmentID`, `VisitDate`, `Symptoms`, `Diagnosis`, `Treatment`, `FollowUpDate`, `VisitStatus`, `CreatedBy`) VALUES (21, 'VISSD020', 21, 22, 21, '2026-02-25 10:00:00.000000', 'Seed symptoms entry 20', 'Urinary tract infection', 'Treatment plan 20 with monitoring instructions.', NULL, 'Completed', 7);
INSERT INTO `visits` (`VisitID`, `VisitCode`, `PatientID`, `DoctorID`, `AppointmentID`, `VisitDate`, `Symptoms`, `Diagnosis`, `Treatment`, `FollowUpDate`, `VisitStatus`, `CreatedBy`) VALUES (22, 'VISSD021', 22, 23, 22, '2026-02-26 11:00:00.000000', 'Seed symptoms entry 21', 'Essential hypertension', 'Treatment plan 21 with monitoring instructions.', '2026-03-12 00:00:00.000000', 'Completed', 7);
INSERT INTO `visits` (`VisitID`, `VisitCode`, `PatientID`, `DoctorID`, `AppointmentID`, `VisitDate`, `Symptoms`, `Diagnosis`, `Treatment`, `FollowUpDate`, `VisitStatus`, `CreatedBy`) VALUES (23, 'VISSD022', 23, 24, 23, '2026-02-27 12:00:00.000000', 'Seed symptoms entry 22', 'Type 2 diabetes mellitus', 'Treatment plan 22 with monitoring instructions.', '2026-03-13 00:00:00.000000', 'Completed', 7);
INSERT INTO `visits` (`VisitID`, `VisitCode`, `PatientID`, `DoctorID`, `AppointmentID`, `VisitDate`, `Symptoms`, `Diagnosis`, `Treatment`, `FollowUpDate`, `VisitStatus`, `CreatedBy`) VALUES (24, 'VISSD023', 24, 25, 24, '2026-02-28 13:00:00.000000', 'Seed symptoms entry 23', 'Bronchial asthma', 'Treatment plan 23 with monitoring instructions.', '2026-03-14 00:00:00.000000', 'Completed', 7);
INSERT INTO `visits` (`VisitID`, `VisitCode`, `PatientID`, `DoctorID`, `AppointmentID`, `VisitDate`, `Symptoms`, `Diagnosis`, `Treatment`, `FollowUpDate`, `VisitStatus`, `CreatedBy`) VALUES (25, 'VISSD024', 25, 26, 25, '2026-03-01 14:00:00.000000', 'Seed symptoms entry 24', 'Hyperlipidemia', 'Treatment plan 24 with monitoring instructions.', NULL, 'Completed', 7);
INSERT INTO `visits` (`VisitID`, `VisitCode`, `PatientID`, `DoctorID`, `AppointmentID`, `VisitDate`, `Symptoms`, `Diagnosis`, `Treatment`, `FollowUpDate`, `VisitStatus`, `CreatedBy`) VALUES (26, 'VISSD025', 26, 27, 26, '2026-03-02 10:00:00.000000', 'Seed symptoms entry 25', 'Migraine episodes', 'Treatment plan 25 with monitoring instructions.', '2026-03-16 00:00:00.000000', 'Completed', 7);
INSERT INTO `visits` (`VisitID`, `VisitCode`, `PatientID`, `DoctorID`, `AppointmentID`, `VisitDate`, `Symptoms`, `Diagnosis`, `Treatment`, `FollowUpDate`, `VisitStatus`, `CreatedBy`) VALUES (27, 'VISSD026', 27, 28, 27, '2026-03-03 11:00:00.000000', 'Seed symptoms entry 26', 'Allergic rhinitis', 'Treatment plan 26 with monitoring instructions.', '2026-03-17 00:00:00.000000', 'Completed', 7);
INSERT INTO `visits` (`VisitID`, `VisitCode`, `PatientID`, `DoctorID`, `AppointmentID`, `VisitDate`, `Symptoms`, `Diagnosis`, `Treatment`, `FollowUpDate`, `VisitStatus`, `CreatedBy`) VALUES (28, 'VISSD027', 28, 29, 28, '2026-03-04 12:00:00.000000', 'Seed symptoms entry 27', 'Lumbar strain', 'Treatment plan 27 with monitoring instructions.', '2026-03-18 00:00:00.000000', 'Completed', 7);
INSERT INTO `visits` (`VisitID`, `VisitCode`, `PatientID`, `DoctorID`, `AppointmentID`, `VisitDate`, `Symptoms`, `Diagnosis`, `Treatment`, `FollowUpDate`, `VisitStatus`, `CreatedBy`) VALUES (29, 'VISSD028', 29, 30, 29, '2026-03-05 13:00:00.000000', 'Seed symptoms entry 28', 'Osteoarthritis', 'Treatment plan 28 with monitoring instructions.', NULL, 'Completed', 7);
INSERT INTO `visits` (`VisitID`, `VisitCode`, `PatientID`, `DoctorID`, `AppointmentID`, `VisitDate`, `Symptoms`, `Diagnosis`, `Treatment`, `FollowUpDate`, `VisitStatus`, `CreatedBy`) VALUES (30, 'VISSD029', 30, 31, 30, '2026-03-06 14:00:00.000000', 'Seed symptoms entry 29', 'Acute gastroenteritis', 'Treatment plan 29 with monitoring instructions.', '2026-03-20 00:00:00.000000', 'Completed', 7);
INSERT INTO `visits` (`VisitID`, `VisitCode`, `PatientID`, `DoctorID`, `AppointmentID`, `VisitDate`, `Symptoms`, `Diagnosis`, `Treatment`, `FollowUpDate`, `VisitStatus`, `CreatedBy`) VALUES (31, 'VISSD030', 31, 32, 31, '2026-03-07 10:00:00.000000', 'Seed symptoms entry 30', 'Urinary tract infection', 'Treatment plan 30 with monitoring instructions.', '2026-03-21 00:00:00.000000', 'Completed', 7);

-- Data for wards
INSERT INTO `wards` (`WardID`, `WardCode`, `WardName`, `WardType`, `Description`, `TotalBeds`, `AvailableBeds`, `ChargePerDay`, `IsActive`) VALUES (10, 'WRDSD001', 'Seed Ward 01', 'General', 'Seeded ward 01 for admissions and room occupancy.', 2, 2, 1495.00, 1);
INSERT INTO `wards` (`WardID`, `WardCode`, `WardName`, `WardType`, `Description`, `TotalBeds`, `AvailableBeds`, `ChargePerDay`, `IsActive`) VALUES (11, 'WRDSD002', 'Seed Ward 02', 'Semi-Private', 'Seeded ward 02 for admissions and room occupancy.', 2, 2, 1590.00, 1);
INSERT INTO `wards` (`WardID`, `WardCode`, `WardName`, `WardType`, `Description`, `TotalBeds`, `AvailableBeds`, `ChargePerDay`, `IsActive`) VALUES (12, 'WRDSD003', 'Seed Ward 03', 'Private', 'Seeded ward 03 for admissions and room occupancy.', 2, 2, 1685.00, 1);
INSERT INTO `wards` (`WardID`, `WardCode`, `WardName`, `WardType`, `Description`, `TotalBeds`, `AvailableBeds`, `ChargePerDay`, `IsActive`) VALUES (13, 'WRDSD004', 'Seed Ward 04', 'ICU', 'Seeded ward 04 for admissions and room occupancy.', 2, 2, 1780.00, 1);
INSERT INTO `wards` (`WardID`, `WardCode`, `WardName`, `WardType`, `Description`, `TotalBeds`, `AvailableBeds`, `ChargePerDay`, `IsActive`) VALUES (14, 'WRDSD005', 'Seed Ward 05', 'General', 'Seeded ward 05 for admissions and room occupancy.', 2, 2, 1875.00, 1);
INSERT INTO `wards` (`WardID`, `WardCode`, `WardName`, `WardType`, `Description`, `TotalBeds`, `AvailableBeds`, `ChargePerDay`, `IsActive`) VALUES (15, 'WRDSD006', 'Seed Ward 06', 'Private', 'Seeded ward 06 for admissions and room occupancy.', 2, 2, 1970.00, 1);
INSERT INTO `wards` (`WardID`, `WardCode`, `WardName`, `WardType`, `Description`, `TotalBeds`, `AvailableBeds`, `ChargePerDay`, `IsActive`) VALUES (16, 'WRDSD007', 'Seed Ward 07', 'General', 'Seeded ward 07 for admissions and room occupancy.', 2, 2, 2065.00, 1);
INSERT INTO `wards` (`WardID`, `WardCode`, `WardName`, `WardType`, `Description`, `TotalBeds`, `AvailableBeds`, `ChargePerDay`, `IsActive`) VALUES (17, 'WRDSD008', 'Seed Ward 08', 'ICU', 'Seeded ward 08 for admissions and room occupancy.', 2, 2, 2160.00, 1);
INSERT INTO `wards` (`WardID`, `WardCode`, `WardName`, `WardType`, `Description`, `TotalBeds`, `AvailableBeds`, `ChargePerDay`, `IsActive`) VALUES (18, 'WRDSD009', 'Seed Ward 09', 'Private', 'Seeded ward 09 for admissions and room occupancy.', 2, 2, 2255.00, 1);
INSERT INTO `wards` (`WardID`, `WardCode`, `WardName`, `WardType`, `Description`, `TotalBeds`, `AvailableBeds`, `ChargePerDay`, `IsActive`) VALUES (19, 'WRDSD010', 'Seed Ward 10', 'Semi-Private', 'Seeded ward 10 for admissions and room occupancy.', 2, 2, 2350.00, 1);
INSERT INTO `wards` (`WardID`, `WardCode`, `WardName`, `WardType`, `Description`, `TotalBeds`, `AvailableBeds`, `ChargePerDay`, `IsActive`) VALUES (20, 'WRDSD011', 'Seed Ward 11', 'General', 'Seeded ward 11 for admissions and room occupancy.', 2, 2, 2445.00, 1);
INSERT INTO `wards` (`WardID`, `WardCode`, `WardName`, `WardType`, `Description`, `TotalBeds`, `AvailableBeds`, `ChargePerDay`, `IsActive`) VALUES (21, 'WRDSD012', 'Seed Ward 12', 'ICU', 'Seeded ward 12 for admissions and room occupancy.', 2, 2, 2540.00, 1);
INSERT INTO `wards` (`WardID`, `WardCode`, `WardName`, `WardType`, `Description`, `TotalBeds`, `AvailableBeds`, `ChargePerDay`, `IsActive`) VALUES (22, 'WRDSD013', 'Seed Ward 13', 'General', 'Seeded ward 13 for admissions and room occupancy.', 2, 2, 2635.00, 1);
INSERT INTO `wards` (`WardID`, `WardCode`, `WardName`, `WardType`, `Description`, `TotalBeds`, `AvailableBeds`, `ChargePerDay`, `IsActive`) VALUES (23, 'WRDSD014', 'Seed Ward 14', 'Semi-Private', 'Seeded ward 14 for admissions and room occupancy.', 2, 2, 2730.00, 1);
INSERT INTO `wards` (`WardID`, `WardCode`, `WardName`, `WardType`, `Description`, `TotalBeds`, `AvailableBeds`, `ChargePerDay`, `IsActive`) VALUES (24, 'WRDSD015', 'Seed Ward 15', 'Private', 'Seeded ward 15 for admissions and room occupancy.', 2, 2, 2825.00, 1);
INSERT INTO `wards` (`WardID`, `WardCode`, `WardName`, `WardType`, `Description`, `TotalBeds`, `AvailableBeds`, `ChargePerDay`, `IsActive`) VALUES (25, 'WRDSD016', 'Seed Ward 16', 'ICU', 'Seeded ward 16 for admissions and room occupancy.', 2, 1, 2920.00, 1);
INSERT INTO `wards` (`WardID`, `WardCode`, `WardName`, `WardType`, `Description`, `TotalBeds`, `AvailableBeds`, `ChargePerDay`, `IsActive`) VALUES (26, 'WRDSD017', 'Seed Ward 17', 'General', 'Seeded ward 17 for admissions and room occupancy.', 2, 1, 3015.00, 1);
INSERT INTO `wards` (`WardID`, `WardCode`, `WardName`, `WardType`, `Description`, `TotalBeds`, `AvailableBeds`, `ChargePerDay`, `IsActive`) VALUES (27, 'WRDSD018', 'Seed Ward 18', 'Private', 'Seeded ward 18 for admissions and room occupancy.', 2, 1, 3110.00, 1);
INSERT INTO `wards` (`WardID`, `WardCode`, `WardName`, `WardType`, `Description`, `TotalBeds`, `AvailableBeds`, `ChargePerDay`, `IsActive`) VALUES (28, 'WRDSD019', 'Seed Ward 19', 'General', 'Seeded ward 19 for admissions and room occupancy.', 2, 1, 3205.00, 1);
INSERT INTO `wards` (`WardID`, `WardCode`, `WardName`, `WardType`, `Description`, `TotalBeds`, `AvailableBeds`, `ChargePerDay`, `IsActive`) VALUES (29, 'WRDSD020', 'Seed Ward 20', 'ICU', 'Seeded ward 20 for admissions and room occupancy.', 2, 1, 3300.00, 1);
INSERT INTO `wards` (`WardID`, `WardCode`, `WardName`, `WardType`, `Description`, `TotalBeds`, `AvailableBeds`, `ChargePerDay`, `IsActive`) VALUES (30, 'WRDSD021', 'Seed Ward 21', 'Private', 'Seeded ward 21 for admissions and room occupancy.', 2, 1, 3395.00, 1);
INSERT INTO `wards` (`WardID`, `WardCode`, `WardName`, `WardType`, `Description`, `TotalBeds`, `AvailableBeds`, `ChargePerDay`, `IsActive`) VALUES (31, 'WRDSD022', 'Seed Ward 22', 'Semi-Private', 'Seeded ward 22 for admissions and room occupancy.', 2, 1, 3490.00, 1);
INSERT INTO `wards` (`WardID`, `WardCode`, `WardName`, `WardType`, `Description`, `TotalBeds`, `AvailableBeds`, `ChargePerDay`, `IsActive`) VALUES (32, 'WRDSD023', 'Seed Ward 23', 'General', 'Seeded ward 23 for admissions and room occupancy.', 2, 1, 3585.00, 1);
INSERT INTO `wards` (`WardID`, `WardCode`, `WardName`, `WardType`, `Description`, `TotalBeds`, `AvailableBeds`, `ChargePerDay`, `IsActive`) VALUES (33, 'WRDSD024', 'Seed Ward 24', 'ICU', 'Seeded ward 24 for admissions and room occupancy.', 2, 1, 3680.00, 1);
INSERT INTO `wards` (`WardID`, `WardCode`, `WardName`, `WardType`, `Description`, `TotalBeds`, `AvailableBeds`, `ChargePerDay`, `IsActive`) VALUES (34, 'WRDSD025', 'Seed Ward 25', 'General', 'Seeded ward 25 for admissions and room occupancy.', 2, 1, 3775.00, 1);
INSERT INTO `wards` (`WardID`, `WardCode`, `WardName`, `WardType`, `Description`, `TotalBeds`, `AvailableBeds`, `ChargePerDay`, `IsActive`) VALUES (35, 'WRDSD026', 'Seed Ward 26', 'Semi-Private', 'Seeded ward 26 for admissions and room occupancy.', 2, 1, 3870.00, 1);
INSERT INTO `wards` (`WardID`, `WardCode`, `WardName`, `WardType`, `Description`, `TotalBeds`, `AvailableBeds`, `ChargePerDay`, `IsActive`) VALUES (36, 'WRDSD027', 'Seed Ward 27', 'Private', 'Seeded ward 27 for admissions and room occupancy.', 2, 1, 3965.00, 1);
INSERT INTO `wards` (`WardID`, `WardCode`, `WardName`, `WardType`, `Description`, `TotalBeds`, `AvailableBeds`, `ChargePerDay`, `IsActive`) VALUES (37, 'WRDSD028', 'Seed Ward 28', 'ICU', 'Seeded ward 28 for admissions and room occupancy.', 2, 1, 4060.00, 1);
INSERT INTO `wards` (`WardID`, `WardCode`, `WardName`, `WardType`, `Description`, `TotalBeds`, `AvailableBeds`, `ChargePerDay`, `IsActive`) VALUES (38, 'WRDSD029', 'Seed Ward 29', 'General', 'Seeded ward 29 for admissions and room occupancy.', 2, 1, 4155.00, 1);
INSERT INTO `wards` (`WardID`, `WardCode`, `WardName`, `WardType`, `Description`, `TotalBeds`, `AvailableBeds`, `ChargePerDay`, `IsActive`) VALUES (39, 'WRDSD030', 'Seed Ward 30', 'Private', 'Seeded ward 30 for admissions and room occupancy.', 2, 1, 4250.00, 1);


CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `Admissions` AS select `admissions`.`AdmissionID` AS `AdmissionID`,`admissions`.`AdmissionNumber` AS `AdmissionNumber`,`admissions`.`PatientID` AS `PatientID`,`admissions`.`DoctorID` AS `DoctorID`,`admissions`.`RoomID` AS `RoomID`,`admissions`.`AdmissionDate` AS `AdmissionDate`,`admissions`.`ExpectedDischargeDate` AS `ExpectedDischargeDate`,`admissions`.`ActualDischargeDate` AS `ActualDischargeDate`,`admissions`.`AdmissionReason` AS `AdmissionReason`,`admissions`.`Diagnosis` AS `Diagnosis`,`admissions`.`Status` AS `Status`,`admissions`.`DischargeSummary` AS `DischargeSummary` from `admissions`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `AppointmentHistory` AS select `appointmenthistory`.`HistoryID` AS `HistoryID`,`appointmenthistory`.`AppointmentID` AS `AppointmentID`,`appointmenthistory`.`Status` AS `Status`,`appointmenthistory`.`ChangedBy` AS `ChangedBy`,`appointmenthistory`.`ChangedDate` AS `ChangedDate`,`appointmenthistory`.`Notes` AS `Notes` from `appointmenthistory`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `Appointments` AS select `appointments`.`AppointmentID` AS `AppointmentID`,`appointments`.`AppointmentCode` AS `AppointmentCode`,`appointments`.`PatientID` AS `PatientID`,`appointments`.`DoctorID` AS `DoctorID`,`appointments`.`AppointmentDate` AS `AppointmentDate`,`appointments`.`AppointmentTime` AS `AppointmentTime`,`appointments`.`AppointmentType` AS `AppointmentType`,`appointments`.`Status` AS `Status`,`appointments`.`Reason` AS `Reason`,`appointments`.`Duration` AS `Duration`,`appointments`.`CreatedBy` AS `CreatedBy`,`appointments`.`CreatedDate` AS `CreatedDate`,`appointments`.`Notes` AS `Notes` from `appointments`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `AuditLogs` AS select `auditlogs`.`LogID` AS `LogID`,`auditlogs`.`UserID` AS `UserID`,`auditlogs`.`Action` AS `Action`,`auditlogs`.`TableName` AS `TableName`,`auditlogs`.`RecordID` AS `RecordID`,`auditlogs`.`OldValue` AS `OldValue`,`auditlogs`.`NewValue` AS `NewValue`,`auditlogs`.`IPAddress` AS `IPAddress`,`auditlogs`.`MachineName` AS `MachineName`,`auditlogs`.`LogDate` AS `LogDate` from `auditlogs`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `BedAllocations` AS select `bedallocations`.`AllocationID` AS `AllocationID`,`bedallocations`.`AdmissionID` AS `AdmissionID`,`bedallocations`.`RoomID` AS `RoomID`,`bedallocations`.`BedNumber` AS `BedNumber`,`bedallocations`.`AllocationDate` AS `AllocationDate`,`bedallocations`.`DischargeDate` AS `DischargeDate`,`bedallocations`.`Status` AS `Status` from `bedallocations`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `Doctors` AS select `doctors`.`DoctorID` AS `DoctorID`,`doctors`.`UserID` AS `UserID`,`doctors`.`DoctorCode` AS `DoctorCode`,`doctors`.`SpecializationID` AS `SpecializationID`,`doctors`.`Qualification` AS `Qualification`,`doctors`.`LicenseNumber` AS `LicenseNumber`,`doctors`.`YearsOfExperience` AS `YearsOfExperience`,`doctors`.`ConsultationFee` AS `ConsultationFee`,`doctors`.`IsAvailable` AS `IsAvailable`,`doctors`.`JoiningDate` AS `JoiningDate` from `doctors`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `DoctorSchedules` AS select `doctorschedules`.`ScheduleID` AS `ScheduleID`,`doctorschedules`.`DoctorID` AS `DoctorID`,`doctorschedules`.`DayOfWeek` AS `DayOfWeek`,`doctorschedules`.`StartTime` AS `StartTime`,`doctorschedules`.`EndTime` AS `EndTime`,`doctorschedules`.`MaxAppointments` AS `MaxAppointments`,`doctorschedules`.`IsActive` AS `IsActive` from `doctorschedules`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `Inventory` AS select `inventory`.`InventoryID` AS `InventoryID`,`inventory`.`MedicineID` AS `MedicineID`,`inventory`.`BatchNumber` AS `BatchNumber`,`inventory`.`ExpiryDate` AS `ExpiryDate`,`inventory`.`Quantity` AS `Quantity`,`inventory`.`PurchasePrice` AS `PurchasePrice`,`inventory`.`SellingPrice` AS `SellingPrice`,`inventory`.`Supplier` AS `Supplier`,`inventory`.`PurchaseDate` AS `PurchaseDate`,`inventory`.`Location` AS `Location` from `inventory`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `InvoiceDetails` AS select `invoicedetails`.`DetailID` AS `DetailID`,`invoicedetails`.`InvoiceID` AS `InvoiceID`,`invoicedetails`.`ServiceID` AS `ServiceID`,`invoicedetails`.`Quantity` AS `Quantity`,`invoicedetails`.`UnitPrice` AS `UnitPrice`,`invoicedetails`.`TotalPrice` AS `TotalPrice` from `invoicedetails`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `Invoices` AS select `invoices`.`InvoiceID` AS `InvoiceID`,`invoices`.`InvoiceNumber` AS `InvoiceNumber`,`invoices`.`PatientID` AS `PatientID`,`invoices`.`AppointmentID` AS `AppointmentID`,`invoices`.`InvoiceDate` AS `InvoiceDate`,`invoices`.`DueDate` AS `DueDate`,`invoices`.`TotalAmount` AS `TotalAmount`,`invoices`.`Discount` AS `Discount`,`invoices`.`TaxAmount` AS `TaxAmount`,`invoices`.`GrandTotal` AS `GrandTotal`,`invoices`.`Status` AS `Status`,`invoices`.`CreatedBy` AS `CreatedBy`,`invoices`.`Notes` AS `Notes` from `invoices`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `LabOrderDetails` AS select `laborderdetails`.`OrderDetailID` AS `OrderDetailID`,`laborderdetails`.`OrderID` AS `OrderID`,`laborderdetails`.`TestID` AS `TestID`,`laborderdetails`.`ResultValue` AS `ResultValue`,`laborderdetails`.`ResultUnit` AS `ResultUnit`,`laborderdetails`.`NormalRange` AS `NormalRange`,`laborderdetails`.`IsNormal` AS `IsNormal`,`laborderdetails`.`Notes` AS `Notes`,`laborderdetails`.`TechnicianID` AS `TechnicianID`,`laborderdetails`.`CompletedDate` AS `CompletedDate` from `laborderdetails`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `LabOrders` AS select `laborders`.`OrderID` AS `OrderID`,`laborders`.`OrderCode` AS `OrderCode`,`laborders`.`VisitID` AS `VisitID`,`laborders`.`PatientID` AS `PatientID`,`laborders`.`DoctorID` AS `DoctorID`,`laborders`.`OrderDate` AS `OrderDate`,`laborders`.`Status` AS `Status`,`laborders`.`ResultDate` AS `ResultDate`,`laborders`.`Notes` AS `Notes` from `laborders`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `LabTests` AS select `labtests`.`TestID` AS `TestID`,`labtests`.`TestCode` AS `TestCode`,`labtests`.`TestName` AS `TestName`,`labtests`.`Category` AS `Category`,`labtests`.`NormalRange` AS `NormalRange`,`labtests`.`Unit` AS `Unit`,`labtests`.`Price` AS `Price` from `labtests`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `MedicalHistories` AS select `medicalhistories`.`HistoryID` AS `HistoryID`,`medicalhistories`.`PatientID` AS `PatientID`,`medicalhistories`.`HistoryType` AS `HistoryType`,`medicalhistories`.`Description` AS `Description`,`medicalhistories`.`DiagnosisDate` AS `DiagnosisDate`,`medicalhistories`.`Severity` AS `Severity`,`medicalhistories`.`Status` AS `Status`,`medicalhistories`.`RecordedBy` AS `RecordedBy`,`medicalhistories`.`RecordedDate` AS `RecordedDate` from `medicalhistories`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `MedicineCategories` AS select `medicinecategories`.`CategoryID` AS `CategoryID`,`medicinecategories`.`CategoryName` AS `CategoryName`,`medicinecategories`.`Description` AS `Description` from `medicinecategories`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `Medicines` AS select `medicines`.`MedicineID` AS `MedicineID`,`medicines`.`MedicineCode` AS `MedicineCode`,`medicines`.`MedicineName` AS `MedicineName`,`medicines`.`GenericName` AS `GenericName`,`medicines`.`CategoryID` AS `CategoryID`,`medicines`.`Manufacturer` AS `Manufacturer`,`medicines`.`UnitOfMeasure` AS `UnitOfMeasure`,`medicines`.`UnitPrice` AS `UnitPrice`,`medicines`.`SellingPrice` AS `SellingPrice`,`medicines`.`ReorderLevel` AS `ReorderLevel`,`medicines`.`IsActive` AS `IsActive` from `medicines`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `Notifications` AS select `notifications`.`NotificationID` AS `NotificationID`,`notifications`.`UserID` AS `UserID`,`notifications`.`Title` AS `Title`,`notifications`.`Message` AS `Message`,`notifications`.`NotificationType` AS `NotificationType`,`notifications`.`IsRead` AS `IsRead`,`notifications`.`CreatedDate` AS `CreatedDate`,`notifications`.`ExpiryDate` AS `ExpiryDate` from `notifications`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `PatientContacts` AS select `patientcontacts`.`ContactID` AS `ContactID`,`patientcontacts`.`PatientID` AS `PatientID`,`patientcontacts`.`ContactType` AS `ContactType`,`patientcontacts`.`ContactValue` AS `ContactValue`,`patientcontacts`.`IsPrimary` AS `IsPrimary` from `patientcontacts`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `Patients` AS select `patients`.`PatientID` AS `PatientID`,`patients`.`PatientCode` AS `PatientCode`,`patients`.`FirstName` AS `FirstName`,`patients`.`LastName` AS `LastName`,`patients`.`DateOfBirth` AS `DateOfBirth`,`patients`.`Gender` AS `Gender`,`patients`.`BloodGroup` AS `BloodGroup`,`patients`.`MaritalStatus` AS `MaritalStatus`,`patients`.`Nationality` AS `Nationality`,`patients`.`IdentificationType` AS `IdentificationType`,`patients`.`IdentificationNumber` AS `IdentificationNumber`,`patients`.`RegistrationDate` AS `RegistrationDate`,`patients`.`IsActive` AS `IsActive`,`patients`.`ProfileImage` AS `ProfileImage` from `patients`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `Payments` AS select `payments`.`PaymentID` AS `PaymentID`,`payments`.`PaymentNumber` AS `PaymentNumber`,`payments`.`InvoiceID` AS `InvoiceID`,`payments`.`PaymentDate` AS `PaymentDate`,`payments`.`PaymentMethod` AS `PaymentMethod`,`payments`.`Amount` AS `Amount`,`payments`.`ReferenceNumber` AS `ReferenceNumber`,`payments`.`ReceivedBy` AS `ReceivedBy`,`payments`.`Notes` AS `Notes` from `payments`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `PharmacySaleDetails` AS select `pharmacysaledetails`.`SaleDetailID` AS `SaleDetailID`,`pharmacysaledetails`.`SaleID` AS `SaleID`,`pharmacysaledetails`.`MedicineID` AS `MedicineID`,`pharmacysaledetails`.`BatchNumber` AS `BatchNumber`,`pharmacysaledetails`.`Quantity` AS `Quantity`,`pharmacysaledetails`.`UnitPrice` AS `UnitPrice`,`pharmacysaledetails`.`TotalPrice` AS `TotalPrice` from `pharmacysaledetails`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `PharmacySales` AS select `pharmacysales`.`SaleID` AS `SaleID`,`pharmacysales`.`SaleNumber` AS `SaleNumber`,`pharmacysales`.`PatientID` AS `PatientID`,`pharmacysales`.`SaleDate` AS `SaleDate`,`pharmacysales`.`TotalAmount` AS `TotalAmount`,`pharmacysales`.`Discount` AS `Discount`,`pharmacysales`.`NetAmount` AS `NetAmount`,`pharmacysales`.`PaymentStatus` AS `PaymentStatus`,`pharmacysales`.`SoldBy` AS `SoldBy` from `pharmacysales`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `PrescriptionDetails` AS select `prescriptiondetails`.`PrescriptionDetailID` AS `PrescriptionDetailID`,`prescriptiondetails`.`PrescriptionID` AS `PrescriptionID`,`prescriptiondetails`.`MedicineName` AS `MedicineName`,`prescriptiondetails`.`Dosage` AS `Dosage`,`prescriptiondetails`.`Frequency` AS `Frequency`,`prescriptiondetails`.`Duration` AS `Duration`,`prescriptiondetails`.`Instructions` AS `Instructions` from `prescriptiondetails`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `Prescriptions` AS select `prescriptions`.`PrescriptionID` AS `PrescriptionID`,`prescriptions`.`PrescriptionCode` AS `PrescriptionCode`,`prescriptions`.`VisitID` AS `VisitID`,`prescriptions`.`PatientID` AS `PatientID`,`prescriptions`.`DoctorID` AS `DoctorID`,`prescriptions`.`PrescriptionDate` AS `PrescriptionDate`,`prescriptions`.`Instructions` AS `Instructions`,`prescriptions`.`Status` AS `Status` from `prescriptions`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `Rooms` AS select `rooms`.`RoomID` AS `RoomID`,`rooms`.`RoomNumber` AS `RoomNumber`,`rooms`.`WardID` AS `WardID`,`rooms`.`RoomType` AS `RoomType`,`rooms`.`TotalBeds` AS `TotalBeds`,`rooms`.`AvailableBeds` AS `AvailableBeds`,`rooms`.`Facilities` AS `Facilities`,`rooms`.`RatePerDay` AS `RatePerDay`,`rooms`.`Status` AS `Status` from `rooms`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `ServiceCategories` AS select `servicecategories`.`CategoryID` AS `CategoryID`,`servicecategories`.`CategoryName` AS `CategoryName`,`servicecategories`.`Description` AS `Description` from `servicecategories`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `Services` AS select `services`.`ServiceID` AS `ServiceID`,`services`.`ServiceCode` AS `ServiceCode`,`services`.`ServiceName` AS `ServiceName`,`services`.`CategoryID` AS `CategoryID`,`services`.`Price` AS `Price`,`services`.`TaxRate` AS `TaxRate`,`services`.`IsActive` AS `IsActive` from `services`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `Specializations` AS select `specializations`.`SpecializationID` AS `SpecializationID`,`specializations`.`SpecializationCode` AS `SpecializationCode`,`specializations`.`SpecializationName` AS `SpecializationName`,`specializations`.`Description` AS `Description`,`specializations`.`Department` AS `Department` from `specializations`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `Staff` AS select `staff`.`StaffID` AS `StaffID`,`staff`.`UserID` AS `UserID`,`staff`.`StaffCode` AS `StaffCode`,`staff`.`Designation` AS `Designation`,`staff`.`Department` AS `Department`,`staff`.`Shift` AS `Shift`,`staff`.`HireDate` AS `HireDate`,`staff`.`Salary` AS `Salary` from `staff`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `SystemSettings` AS select `systemsettings`.`SettingID` AS `SettingID`,`systemsettings`.`SettingKey` AS `SettingKey`,`systemsettings`.`SettingValue` AS `SettingValue`,`systemsettings`.`Description` AS `Description`,`systemsettings`.`Category` AS `Category`,`systemsettings`.`LastModified` AS `LastModified` from `systemsettings`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `UserDetails` AS select `userdetails`.`UserDetailID` AS `UserDetailID`,`userdetails`.`UserID` AS `UserID`,`userdetails`.`FirstName` AS `FirstName`,`userdetails`.`LastName` AS `LastName`,`userdetails`.`DateOfBirth` AS `DateOfBirth`,`userdetails`.`Gender` AS `Gender`,`userdetails`.`ContactNumber` AS `ContactNumber`,`userdetails`.`Address` AS `Address`,`userdetails`.`EmergencyContact` AS `EmergencyContact`,`userdetails`.`ProfileImage` AS `ProfileImage` from `userdetails`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `UserRoles` AS select `userroles`.`RoleID` AS `RoleID`,`userroles`.`RoleName` AS `RoleName`,`userroles`.`Description` AS `Description`,`userroles`.`CreatedDate` AS `CreatedDate` from `userroles`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `Users` AS select `users`.`UserID` AS `UserID`,`users`.`Username` AS `Username`,`users`.`PasswordHash` AS `PasswordHash`,`users`.`Email` AS `Email`,`users`.`RoleID` AS `RoleID`,`users`.`IsActive` AS `IsActive`,`users`.`LastLogin` AS `LastLogin`,`users`.`CreatedDate` AS `CreatedDate` from `users`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `Visits` AS select `visits`.`VisitID` AS `VisitID`,`visits`.`VisitCode` AS `VisitCode`,`visits`.`PatientID` AS `PatientID`,`visits`.`DoctorID` AS `DoctorID`,`visits`.`AppointmentID` AS `AppointmentID`,`visits`.`VisitDate` AS `VisitDate`,`visits`.`Symptoms` AS `Symptoms`,`visits`.`Diagnosis` AS `Diagnosis`,`visits`.`Treatment` AS `Treatment`,`visits`.`FollowUpDate` AS `FollowUpDate`,`visits`.`VisitStatus` AS `VisitStatus`,`visits`.`CreatedBy` AS `CreatedBy` from `visits`;

CREATE OR REPLACE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `Wards` AS select `wards`.`WardID` AS `WardID`,`wards`.`WardCode` AS `WardCode`,`wards`.`WardName` AS `WardName`,`wards`.`WardType` AS `WardType`,`wards`.`Description` AS `Description`,`wards`.`TotalBeds` AS `TotalBeds`,`wards`.`AvailableBeds` AS `AvailableBeds`,`wards`.`ChargePerDay` AS `ChargePerDay`,`wards`.`IsActive` AS `IsActive` from `wards`;

SET FOREIGN_KEY_CHECKS = 1;
