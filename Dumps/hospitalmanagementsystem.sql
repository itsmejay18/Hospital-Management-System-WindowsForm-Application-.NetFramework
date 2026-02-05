-- MySQL dump 10.13  Distrib 8.0.44, for Win64 (x86_64)
--
-- Host: localhost    Database: hospitalmanagementsystem
-- ------------------------------------------------------
-- Server version	8.0.44

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `admissions`
--

DROP TABLE IF EXISTS `admissions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `admissions` (
  `AdmissionID` int NOT NULL AUTO_INCREMENT,
  `AdmissionNumber` varchar(30) NOT NULL,
  `PatientID` int NOT NULL,
  `DoctorID` int NOT NULL,
  `RoomID` int DEFAULT NULL,
  `AdmissionDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `ExpectedDischargeDate` date DEFAULT NULL,
  `ActualDischargeDate` datetime DEFAULT NULL,
  `AdmissionReason` text,
  `Diagnosis` text,
  `Status` enum('Admitted','Discharged','Transferred') DEFAULT 'Admitted',
  `DischargeSummary` text,
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
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `admissions`
--

LOCK TABLES `admissions` WRITE;
/*!40000 ALTER TABLE `admissions` DISABLE KEYS */;
INSERT INTO `admissions` VALUES (1,'ADM-SEED-001',1,1,8,'2026-02-04 20:33:16','2026-02-07',NULL,'Observation','Under evaluation','Admitted',NULL);
/*!40000 ALTER TABLE `admissions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `appointmenthistory`
--

DROP TABLE IF EXISTS `appointmenthistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `appointmenthistory` (
  `HistoryID` int NOT NULL AUTO_INCREMENT,
  `AppointmentID` int NOT NULL,
  `Status` varchar(20) DEFAULT NULL,
  `ChangedBy` int DEFAULT NULL,
  `ChangedDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `Notes` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`HistoryID`),
  KEY `AppointmentID` (`AppointmentID`),
  KEY `ChangedBy` (`ChangedBy`),
  CONSTRAINT `appointmenthistory_ibfk_1` FOREIGN KEY (`AppointmentID`) REFERENCES `appointments` (`AppointmentID`) ON DELETE CASCADE,
  CONSTRAINT `appointmenthistory_ibfk_2` FOREIGN KEY (`ChangedBy`) REFERENCES `users` (`UserID`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `appointmenthistory`
--

LOCK TABLES `appointmenthistory` WRITE;
/*!40000 ALTER TABLE `appointmenthistory` DISABLE KEYS */;
INSERT INTO `appointmenthistory` VALUES (1,1,'Scheduled',1,'2026-02-04 20:33:15','Seeded status');
/*!40000 ALTER TABLE `appointmenthistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `appointments`
--

DROP TABLE IF EXISTS `appointments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `appointments` (
  `AppointmentID` int NOT NULL AUTO_INCREMENT,
  `AppointmentCode` varchar(20) NOT NULL,
  `PatientID` int NOT NULL,
  `DoctorID` int NOT NULL,
  `AppointmentDate` date NOT NULL,
  `AppointmentTime` time NOT NULL,
  `AppointmentType` enum('Consultation','Follow-up','Emergency','Check-up') DEFAULT NULL,
  `Status` enum('Scheduled','Confirmed','Completed','Cancelled','No-show') DEFAULT 'Scheduled',
  `Reason` varchar(500) DEFAULT NULL,
  `Duration` int DEFAULT '15',
  `CreatedBy` int DEFAULT NULL,
  `CreatedDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `Notes` text,
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
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `appointments`
--

LOCK TABLES `appointments` WRITE;
/*!40000 ALTER TABLE `appointments` DISABLE KEYS */;
INSERT INTO `appointments` VALUES (1,'APP-SEED-001',1,1,'2026-02-04','10:00:00','Consultation','Scheduled','Initial consultation',15,5,'2026-02-04 20:33:15','Seed appointment');
/*!40000 ALTER TABLE `appointments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `auditlogs`
--

DROP TABLE IF EXISTS `auditlogs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `auditlogs` (
  `LogID` int NOT NULL AUTO_INCREMENT,
  `UserID` int DEFAULT NULL,
  `Action` varchar(100) NOT NULL,
  `TableName` varchar(100) DEFAULT NULL,
  `RecordID` int DEFAULT NULL,
  `OldValue` json DEFAULT NULL,
  `NewValue` json DEFAULT NULL,
  `IPAddress` varchar(50) DEFAULT NULL,
  `MachineName` varchar(100) DEFAULT NULL,
  `LogDate` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`LogID`),
  KEY `UserID` (`UserID`),
  CONSTRAINT `auditlogs_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `auditlogs`
--

LOCK TABLES `auditlogs` WRITE;
/*!40000 ALTER TABLE `auditlogs` DISABLE KEYS */;
INSERT INTO `auditlogs` VALUES (1,1,'SEED_INSERT','System',1,NULL,'{\"seed\": \"completed\"}','127.0.0.1','LOCALHOST','2026-02-04 20:33:16');
/*!40000 ALTER TABLE `auditlogs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bedallocations`
--

DROP TABLE IF EXISTS `bedallocations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bedallocations` (
  `AllocationID` int NOT NULL AUTO_INCREMENT,
  `AdmissionID` int NOT NULL,
  `RoomID` int NOT NULL,
  `BedNumber` varchar(10) DEFAULT NULL,
  `AllocationDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `DischargeDate` datetime DEFAULT NULL,
  `Status` varchar(20) DEFAULT 'Occupied',
  PRIMARY KEY (`AllocationID`),
  KEY `AdmissionID` (`AdmissionID`),
  KEY `RoomID` (`RoomID`),
  CONSTRAINT `bedallocations_ibfk_1` FOREIGN KEY (`AdmissionID`) REFERENCES `admissions` (`AdmissionID`) ON DELETE CASCADE,
  CONSTRAINT `bedallocations_ibfk_2` FOREIGN KEY (`RoomID`) REFERENCES `rooms` (`RoomID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bedallocations`
--

LOCK TABLES `bedallocations` WRITE;
/*!40000 ALTER TABLE `bedallocations` DISABLE KEYS */;
INSERT INTO `bedallocations` VALUES (1,1,8,'B1','2026-02-04 20:33:16',NULL,'Occupied');
/*!40000 ALTER TABLE `bedallocations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `doctors`
--

DROP TABLE IF EXISTS `doctors`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `doctors` (
  `DoctorID` int NOT NULL AUTO_INCREMENT,
  `UserID` int NOT NULL,
  `DoctorCode` varchar(20) NOT NULL,
  `SpecializationID` int DEFAULT NULL,
  `Qualification` varchar(255) DEFAULT NULL,
  `LicenseNumber` varchar(50) DEFAULT NULL,
  `YearsOfExperience` int DEFAULT NULL,
  `ConsultationFee` decimal(10,2) DEFAULT NULL,
  `IsAvailable` tinyint(1) DEFAULT '1',
  `JoiningDate` date DEFAULT (curdate()),
  PRIMARY KEY (`DoctorID`),
  UNIQUE KEY `UserID` (`UserID`),
  UNIQUE KEY `DoctorCode` (`DoctorCode`),
  KEY `idx_doctors_specialization` (`SpecializationID`),
  KEY `idx_doctors_available` (`IsAvailable`),
  CONSTRAINT `doctors_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`) ON DELETE CASCADE,
  CONSTRAINT `doctors_ibfk_2` FOREIGN KEY (`SpecializationID`) REFERENCES `specializations` (`SpecializationID`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `doctors`
--

LOCK TABLES `doctors` WRITE;
/*!40000 ALTER TABLE `doctors` DISABLE KEYS */;
INSERT INTO `doctors` VALUES (1,2,'D-001',1,'MD Cardiology','LIC12345',NULL,50.00,1,'2026-02-03'),(2,3,'D-002',3,'MS Orthopedics','LIC12346',NULL,60.00,1,'2026-02-03');
/*!40000 ALTER TABLE `doctors` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `doctorschedules`
--

DROP TABLE IF EXISTS `doctorschedules`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `doctorschedules` (
  `ScheduleID` int NOT NULL AUTO_INCREMENT,
  `DoctorID` int NOT NULL,
  `DayOfWeek` int DEFAULT NULL,
  `StartTime` time NOT NULL,
  `EndTime` time NOT NULL,
  `MaxAppointments` int DEFAULT '20',
  `IsActive` tinyint(1) DEFAULT '1',
  PRIMARY KEY (`ScheduleID`),
  KEY `DoctorID` (`DoctorID`),
  CONSTRAINT `doctorschedules_ibfk_1` FOREIGN KEY (`DoctorID`) REFERENCES `doctors` (`DoctorID`) ON DELETE CASCADE,
  CONSTRAINT `doctorschedules_chk_1` CHECK ((`DayOfWeek` between 1 and 7))
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `doctorschedules`
--

LOCK TABLES `doctorschedules` WRITE;
/*!40000 ALTER TABLE `doctorschedules` DISABLE KEYS */;
INSERT INTO `doctorschedules` VALUES (1,1,1,'09:00:00','13:00:00',15,1),(2,1,1,'14:00:00','18:00:00',15,1),(3,1,3,'09:00:00','13:00:00',15,1),(4,2,2,'10:00:00','14:00:00',12,1),(5,2,4,'10:00:00','14:00:00',12,1),(6,1,2,'09:00:00','13:00:00',20,1);
/*!40000 ALTER TABLE `doctorschedules` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `inventory`
--

DROP TABLE IF EXISTS `inventory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `inventory` (
  `InventoryID` int NOT NULL AUTO_INCREMENT,
  `MedicineID` int NOT NULL,
  `BatchNumber` varchar(100) DEFAULT NULL,
  `ExpiryDate` date DEFAULT NULL,
  `Quantity` int NOT NULL DEFAULT '0',
  `PurchasePrice` decimal(10,2) DEFAULT NULL,
  `SellingPrice` decimal(10,2) DEFAULT NULL,
  `Supplier` varchar(200) DEFAULT NULL,
  `PurchaseDate` date DEFAULT (curdate()),
  `Location` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`InventoryID`),
  KEY `idx_inventory_expiry` (`ExpiryDate`),
  KEY `idx_inventory_medicine` (`MedicineID`),
  KEY `idx_inventory_quantity` (`Quantity`),
  CONSTRAINT `inventory_ibfk_1` FOREIGN KEY (`MedicineID`) REFERENCES `medicines` (`MedicineID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `inventory`
--

LOCK TABLES `inventory` WRITE;
/*!40000 ALTER TABLE `inventory` DISABLE KEYS */;
INSERT INTO `inventory` VALUES (1,1,'BATCH-001','2025-12-31',5000,0.08,0.50,'Pharma Supply Co.','2026-02-03',NULL),(2,2,'BATCH-002','2024-06-30',2500,0.18,1.00,'Pharma Supply Co.','2026-02-03',NULL),(3,3,'BATCH-003','2025-09-30',4000,0.12,0.75,'Medi Distributors','2026-02-03',NULL),(4,4,'BATCH-004','2024-11-30',1500,0.20,1.25,'Health Suppliers','2026-02-03',NULL),(5,5,'BATCH-005','2026-03-31',8000,0.06,0.40,'Vitamin Corp','2026-02-03',NULL),(6,6,'BATCH-006','2025-08-31',3000,0.10,0.60,'Diabetes Meds Inc.','2026-02-03',NULL),(7,7,'BATCH-007','2024-12-31',500,2.00,12.50,'Respiratory Pharma','2026-02-03',NULL),(8,8,'BATCH-008','2025-05-31',2000,0.25,1.50,'GI Pharmaceuticals','2026-02-03',NULL),(9,9,'BATCH-SEED-001','2027-02-04',1000,0.08,0.50,'Seed Supplier','2026-02-04','Main Pharmacy');
/*!40000 ALTER TABLE `inventory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `invoicedetails`
--

DROP TABLE IF EXISTS `invoicedetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `invoicedetails` (
  `DetailID` int NOT NULL AUTO_INCREMENT,
  `InvoiceID` int NOT NULL,
  `ServiceID` int NOT NULL,
  `Quantity` int DEFAULT '1',
  `UnitPrice` decimal(10,2) DEFAULT NULL,
  `TotalPrice` decimal(10,2) GENERATED ALWAYS AS ((`Quantity` * `UnitPrice`)) STORED,
  PRIMARY KEY (`DetailID`),
  KEY `InvoiceID` (`InvoiceID`),
  KEY `ServiceID` (`ServiceID`),
  CONSTRAINT `invoicedetails_ibfk_1` FOREIGN KEY (`InvoiceID`) REFERENCES `invoices` (`InvoiceID`) ON DELETE CASCADE,
  CONSTRAINT `invoicedetails_ibfk_2` FOREIGN KEY (`ServiceID`) REFERENCES `services` (`ServiceID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `invoicedetails`
--

LOCK TABLES `invoicedetails` WRITE;
/*!40000 ALTER TABLE `invoicedetails` DISABLE KEYS */;
INSERT INTO `invoicedetails` (`DetailID`, `InvoiceID`, `ServiceID`, `Quantity`, `UnitPrice`) VALUES (1,1,9,1,50.00);
/*!40000 ALTER TABLE `invoicedetails` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `invoices`
--

DROP TABLE IF EXISTS `invoices`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `invoices` (
  `InvoiceID` int NOT NULL AUTO_INCREMENT,
  `InvoiceNumber` varchar(30) NOT NULL,
  `PatientID` int NOT NULL,
  `AppointmentID` int DEFAULT NULL,
  `InvoiceDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `DueDate` datetime DEFAULT NULL,
  `TotalAmount` decimal(10,2) DEFAULT '0.00',
  `Discount` decimal(10,2) DEFAULT '0.00',
  `TaxAmount` decimal(10,2) DEFAULT '0.00',
  `GrandTotal` decimal(10,2) DEFAULT '0.00',
  `Status` enum('Pending','Paid','Partial','Cancelled') DEFAULT 'Pending',
  `CreatedBy` int DEFAULT NULL,
  `Notes` text,
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
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `invoices`
--

LOCK TABLES `invoices` WRITE;
/*!40000 ALTER TABLE `invoices` DISABLE KEYS */;
INSERT INTO `invoices` VALUES (1,'INV-SEED-001',1,1,'2026-02-04 20:33:15','2026-03-06 20:33:15',50.00,0.00,2.50,52.50,'Pending',1,'Seed invoice');
/*!40000 ALTER TABLE `invoices` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `laborderdetails`
--

DROP TABLE IF EXISTS `laborderdetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `laborderdetails` (
  `OrderDetailID` int NOT NULL AUTO_INCREMENT,
  `OrderID` int NOT NULL,
  `TestID` int NOT NULL,
  `ResultValue` varchar(200) DEFAULT NULL,
  `ResultUnit` varchar(50) DEFAULT NULL,
  `NormalRange` varchar(200) DEFAULT NULL,
  `IsNormal` tinyint(1) DEFAULT NULL,
  `Notes` text,
  `TechnicianID` int DEFAULT NULL,
  `CompletedDate` datetime DEFAULT NULL,
  PRIMARY KEY (`OrderDetailID`),
  KEY `OrderID` (`OrderID`),
  KEY `TestID` (`TestID`),
  KEY `TechnicianID` (`TechnicianID`),
  CONSTRAINT `laborderdetails_ibfk_1` FOREIGN KEY (`OrderID`) REFERENCES `laborders` (`OrderID`) ON DELETE CASCADE,
  CONSTRAINT `laborderdetails_ibfk_2` FOREIGN KEY (`TestID`) REFERENCES `labtests` (`TestID`) ON DELETE CASCADE,
  CONSTRAINT `laborderdetails_ibfk_3` FOREIGN KEY (`TechnicianID`) REFERENCES `users` (`UserID`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `laborderdetails`
--

LOCK TABLES `laborderdetails` WRITE;
/*!40000 ALTER TABLE `laborderdetails` DISABLE KEYS */;
INSERT INTO `laborderdetails` VALUES (1,1,11,'Pending','N/A','N/A',NULL,'Awaiting result',1,NULL);
/*!40000 ALTER TABLE `laborderdetails` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `laborders`
--

DROP TABLE IF EXISTS `laborders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `laborders` (
  `OrderID` int NOT NULL AUTO_INCREMENT,
  `OrderCode` varchar(20) NOT NULL,
  `VisitID` int DEFAULT NULL,
  `PatientID` int NOT NULL,
  `DoctorID` int NOT NULL,
  `OrderDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `Status` enum('Pending','In Progress','Completed','Cancelled') DEFAULT 'Pending',
  `ResultDate` datetime DEFAULT NULL,
  `Notes` text,
  PRIMARY KEY (`OrderID`),
  UNIQUE KEY `OrderCode` (`OrderCode`),
  KEY `VisitID` (`VisitID`),
  KEY `PatientID` (`PatientID`),
  KEY `DoctorID` (`DoctorID`),
  CONSTRAINT `laborders_ibfk_1` FOREIGN KEY (`VisitID`) REFERENCES `visits` (`VisitID`) ON DELETE SET NULL,
  CONSTRAINT `laborders_ibfk_2` FOREIGN KEY (`PatientID`) REFERENCES `patients` (`PatientID`) ON DELETE CASCADE,
  CONSTRAINT `laborders_ibfk_3` FOREIGN KEY (`DoctorID`) REFERENCES `doctors` (`DoctorID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `laborders`
--

LOCK TABLES `laborders` WRITE;
/*!40000 ALTER TABLE `laborders` DISABLE KEYS */;
INSERT INTO `laborders` VALUES (1,'LORD-SEED-001',1,1,1,'2026-02-04 20:33:15','Pending',NULL,'Seed lab order');
/*!40000 ALTER TABLE `laborders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `labtests`
--

DROP TABLE IF EXISTS `labtests`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `labtests` (
  `TestID` int NOT NULL AUTO_INCREMENT,
  `TestCode` varchar(20) NOT NULL,
  `TestName` varchar(200) NOT NULL,
  `Category` varchar(100) DEFAULT NULL,
  `NormalRange` varchar(200) DEFAULT NULL,
  `Unit` varchar(50) DEFAULT NULL,
  `Price` decimal(10,2) DEFAULT NULL,
  PRIMARY KEY (`TestID`),
  UNIQUE KEY `TestCode` (`TestCode`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `labtests`
--

LOCK TABLES `labtests` WRITE;
/*!40000 ALTER TABLE `labtests` DISABLE KEYS */;
INSERT INTO `labtests` VALUES (1,'LAB-001','Complete Blood Count','Hematology','Varies','N/A',25.00),(2,'LAB-002','Blood Glucose Fasting','Biochemistry','70-100','mg/dL',10.00),(3,'LAB-003','Lipid Profile','Biochemistry','Varies','mg/dL',30.00),(4,'LAB-004','Liver Function Test','Biochemistry','Varies','U/L',40.00),(5,'LAB-005','Urine Analysis','Urine','Varies','N/A',15.00),(6,'LAB-006','Thyroid Profile','Hormones','Varies','µIU/mL',50.00),(7,'LAB-007','HIV Test','Serology','Negative','N/A',35.00),(8,'LAB-008','COVID-19 PCR','Microbiology','Negative','N/A',75.00),(9,'LAB-009','Hemoglobin A1c','Diabetes','4.0-5.6','%',20.00),(10,'LAB-010','Creatinine','Kidney','0.6-1.2','mg/dL',12.00),(11,'LAB-SEED-001','Complete Blood Count','Hematology','Normal','N/A',35.00);
/*!40000 ALTER TABLE `labtests` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `medicalhistories`
--

DROP TABLE IF EXISTS `medicalhistories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `medicalhistories` (
  `HistoryID` int NOT NULL AUTO_INCREMENT,
  `PatientID` int NOT NULL,
  `HistoryType` varchar(50) DEFAULT NULL,
  `Description` varchar(500) DEFAULT NULL,
  `DiagnosisDate` date DEFAULT NULL,
  `Severity` varchar(20) DEFAULT NULL,
  `Status` varchar(20) DEFAULT 'Active',
  `RecordedBy` int DEFAULT NULL,
  `RecordedDate` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`HistoryID`),
  KEY `PatientID` (`PatientID`),
  KEY `RecordedBy` (`RecordedBy`),
  CONSTRAINT `medicalhistories_ibfk_1` FOREIGN KEY (`PatientID`) REFERENCES `patients` (`PatientID`) ON DELETE CASCADE,
  CONSTRAINT `medicalhistories_ibfk_2` FOREIGN KEY (`RecordedBy`) REFERENCES `users` (`UserID`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `medicalhistories`
--

LOCK TABLES `medicalhistories` WRITE;
/*!40000 ALTER TABLE `medicalhistories` DISABLE KEYS */;
INSERT INTO `medicalhistories` VALUES (1,1,'Allergy','Seasonal allergy','2026-02-04','Mild','Active',1,'2026-02-04 20:31:59');
/*!40000 ALTER TABLE `medicalhistories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `medicinecategories`
--

DROP TABLE IF EXISTS `medicinecategories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `medicinecategories` (
  `CategoryID` int NOT NULL AUTO_INCREMENT,
  `CategoryName` varchar(100) NOT NULL,
  `Description` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`CategoryID`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `medicinecategories`
--

LOCK TABLES `medicinecategories` WRITE;
/*!40000 ALTER TABLE `medicinecategories` DISABLE KEYS */;
INSERT INTO `medicinecategories` VALUES (1,'Antibiotics','Antibacterial medications'),(2,'Analgesics','Pain relievers'),(3,'Antipyretics','Fever reducers'),(4,'Antihistamines','Allergy medications'),(5,'Cardiovascular','Heart and blood pressure medications'),(6,'Gastrointestinal','Stomach and digestive medications'),(7,'Vitamins','Vitamins and supplements'),(8,'Diabetes','Diabetes medications'),(9,'Respiratory','Asthma and respiratory medications');
/*!40000 ALTER TABLE `medicinecategories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `medicines`
--

DROP TABLE IF EXISTS `medicines`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `medicines` (
  `MedicineID` int NOT NULL AUTO_INCREMENT,
  `MedicineCode` varchar(20) NOT NULL,
  `MedicineName` varchar(200) NOT NULL,
  `GenericName` varchar(200) DEFAULT NULL,
  `CategoryID` int DEFAULT NULL,
  `Manufacturer` varchar(200) DEFAULT NULL,
  `UnitOfMeasure` varchar(50) DEFAULT NULL,
  `UnitPrice` decimal(10,2) NOT NULL,
  `SellingPrice` decimal(10,2) NOT NULL,
  `ReorderLevel` int DEFAULT '10',
  `IsActive` tinyint(1) DEFAULT '1',
  PRIMARY KEY (`MedicineID`),
  UNIQUE KEY `MedicineCode` (`MedicineCode`),
  KEY `CategoryID` (`CategoryID`),
  CONSTRAINT `medicines_ibfk_1` FOREIGN KEY (`CategoryID`) REFERENCES `medicinecategories` (`CategoryID`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `medicines`
--

LOCK TABLES `medicines` WRITE;
/*!40000 ALTER TABLE `medicines` DISABLE KEYS */;
INSERT INTO `medicines` VALUES (1,'MED-001','Paracetamol 500mg','Paracetamol',3,'Generic Pharma','Tablet',0.10,0.50,1000,1),(2,'MED-002','Amoxicillin 250mg','Amoxicillin',1,'Generic Pharma','Capsule',0.20,1.00,500,1),(3,'MED-003','Ibuprofen 400mg','Ibuprofen',2,'Generic Pharma','Tablet',0.15,0.75,800,1),(4,'MED-004','Cetirizine 10mg','Cetirizine',4,'Generic Pharma','Tablet',0.25,1.25,300,1),(5,'MED-005','Vitamin C 500mg','Ascorbic Acid',7,'Generic Pharma','Tablet',0.08,0.40,1500,1),(6,'MED-006','Metformin 500mg','Metformin',8,'Generic Pharma','Tablet',0.12,0.60,600,1),(7,'MED-007','Salbutamol Inhaler','Salbutamol',9,'Generic Pharma','Inhaler',2.50,12.50,100,1),(8,'MED-008','Omeprazole 20mg','Omeprazole',6,'Generic Pharma','Capsule',0.30,1.50,400,1),(9,'MED-SEED-001','Paracetamol 500mg','Paracetamol',2,'Seed Pharma','Tablet',0.10,0.50,100,1);
/*!40000 ALTER TABLE `medicines` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `notifications`
--

DROP TABLE IF EXISTS `notifications`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `notifications` (
  `NotificationID` int NOT NULL AUTO_INCREMENT,
  `UserID` int NOT NULL,
  `Title` varchar(200) NOT NULL,
  `Message` text NOT NULL,
  `NotificationType` varchar(50) DEFAULT NULL,
  `IsRead` tinyint(1) DEFAULT '0',
  `CreatedDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `ExpiryDate` datetime DEFAULT NULL,
  PRIMARY KEY (`NotificationID`),
  KEY `UserID` (`UserID`),
  CONSTRAINT `notifications_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `notifications`
--

LOCK TABLES `notifications` WRITE;
/*!40000 ALTER TABLE `notifications` DISABLE KEYS */;
INSERT INTO `notifications` VALUES (1,1,'Seed Complete','Database seed data inserted.','System',0,'2026-02-04 20:33:16','2026-02-11 20:33:16');
/*!40000 ALTER TABLE `notifications` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `patientcontacts`
--

DROP TABLE IF EXISTS `patientcontacts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `patientcontacts` (
  `ContactID` int NOT NULL AUTO_INCREMENT,
  `PatientID` int NOT NULL,
  `ContactType` enum('Phone','Email','Address') DEFAULT NULL,
  `ContactValue` varchar(255) NOT NULL,
  `IsPrimary` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`ContactID`),
  KEY `PatientID` (`PatientID`),
  CONSTRAINT `patientcontacts_ibfk_1` FOREIGN KEY (`PatientID`) REFERENCES `patients` (`PatientID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `patientcontacts`
--

LOCK TABLES `patientcontacts` WRITE;
/*!40000 ALTER TABLE `patientcontacts` DISABLE KEYS */;
INSERT INTO `patientcontacts` VALUES (1,1,'Phone','+10000001001',1);
/*!40000 ALTER TABLE `patientcontacts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `patients`
--

DROP TABLE IF EXISTS `patients`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `patients` (
  `PatientID` int NOT NULL AUTO_INCREMENT,
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
  `RegistrationDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `IsActive` tinyint(1) DEFAULT '1',
  PRIMARY KEY (`PatientID`),
  UNIQUE KEY `PatientCode` (`PatientCode`),
  KEY `idx_patients_patientcode` (`PatientCode`),
  KEY `idx_patients_name` (`LastName`,`FirstName`),
  KEY `idx_patients_dob` (`DateOfBirth`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `patients`
--

LOCK TABLES `patients` WRITE;
/*!40000 ALTER TABLE `patients` DISABLE KEYS */;
INSERT INTO `patients` VALUES (1,'P-SEED-001','Alice','Brown','1995-05-05','F','O+','Single','USA','NationalID','ALICE-001','2026-02-04 20:31:59',1);
/*!40000 ALTER TABLE `patients` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `payments`
--

DROP TABLE IF EXISTS `payments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `payments` (
  `PaymentID` int NOT NULL AUTO_INCREMENT,
  `PaymentNumber` varchar(30) NOT NULL,
  `InvoiceID` int NOT NULL,
  `PaymentDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `PaymentMethod` enum('Cash','Credit Card','Debit Card','Insurance','Online') DEFAULT NULL,
  `Amount` decimal(10,2) NOT NULL,
  `ReferenceNumber` varchar(100) DEFAULT NULL,
  `ReceivedBy` int DEFAULT NULL,
  `Notes` text,
  PRIMARY KEY (`PaymentID`),
  UNIQUE KEY `PaymentNumber` (`PaymentNumber`),
  KEY `InvoiceID` (`InvoiceID`),
  KEY `ReceivedBy` (`ReceivedBy`),
  CONSTRAINT `payments_ibfk_1` FOREIGN KEY (`InvoiceID`) REFERENCES `invoices` (`InvoiceID`) ON DELETE CASCADE,
  CONSTRAINT `payments_ibfk_2` FOREIGN KEY (`ReceivedBy`) REFERENCES `users` (`UserID`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `payments`
--

LOCK TABLES `payments` WRITE;
/*!40000 ALTER TABLE `payments` DISABLE KEYS */;
INSERT INTO `payments` VALUES (1,'PAY-SEED-001',1,'2026-02-04 20:33:15','Cash',25.00,'REF-SEED-001',1,'Partial seed payment');
/*!40000 ALTER TABLE `payments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pharmacysaledetails`
--

DROP TABLE IF EXISTS `pharmacysaledetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pharmacysaledetails` (
  `SaleDetailID` int NOT NULL AUTO_INCREMENT,
  `SaleID` int NOT NULL,
  `MedicineID` int NOT NULL,
  `BatchNumber` varchar(100) DEFAULT NULL,
  `Quantity` int NOT NULL,
  `UnitPrice` decimal(10,2) DEFAULT NULL,
  `TotalPrice` decimal(10,2) GENERATED ALWAYS AS ((`Quantity` * `UnitPrice`)) STORED,
  PRIMARY KEY (`SaleDetailID`),
  KEY `SaleID` (`SaleID`),
  KEY `MedicineID` (`MedicineID`),
  CONSTRAINT `pharmacysaledetails_ibfk_1` FOREIGN KEY (`SaleID`) REFERENCES `pharmacysales` (`SaleID`) ON DELETE CASCADE,
  CONSTRAINT `pharmacysaledetails_ibfk_2` FOREIGN KEY (`MedicineID`) REFERENCES `medicines` (`MedicineID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pharmacysaledetails`
--

LOCK TABLES `pharmacysaledetails` WRITE;
/*!40000 ALTER TABLE `pharmacysaledetails` DISABLE KEYS */;
INSERT INTO `pharmacysaledetails` (`SaleDetailID`, `SaleID`, `MedicineID`, `BatchNumber`, `Quantity`, `UnitPrice`) VALUES (1,1,9,'BATCH-SEED-001',5,0.50);
/*!40000 ALTER TABLE `pharmacysaledetails` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pharmacysales`
--

DROP TABLE IF EXISTS `pharmacysales`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pharmacysales` (
  `SaleID` int NOT NULL AUTO_INCREMENT,
  `SaleNumber` varchar(30) NOT NULL,
  `PatientID` int DEFAULT NULL,
  `SaleDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `TotalAmount` decimal(10,2) DEFAULT '0.00',
  `Discount` decimal(10,2) DEFAULT '0.00',
  `NetAmount` decimal(10,2) DEFAULT '0.00',
  `PaymentStatus` enum('Pending','Paid','Partial') DEFAULT 'Pending',
  `SoldBy` int DEFAULT NULL,
  PRIMARY KEY (`SaleID`),
  UNIQUE KEY `SaleNumber` (`SaleNumber`),
  KEY `PatientID` (`PatientID`),
  KEY `SoldBy` (`SoldBy`),
  CONSTRAINT `pharmacysales_ibfk_1` FOREIGN KEY (`PatientID`) REFERENCES `patients` (`PatientID`) ON DELETE SET NULL,
  CONSTRAINT `pharmacysales_ibfk_2` FOREIGN KEY (`SoldBy`) REFERENCES `users` (`UserID`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pharmacysales`
--

LOCK TABLES `pharmacysales` WRITE;
/*!40000 ALTER TABLE `pharmacysales` DISABLE KEYS */;
INSERT INTO `pharmacysales` VALUES (1,'PSALE-SEED-001',1,'2026-02-04 20:33:15',10.00,0.00,10.00,'Paid',1);
/*!40000 ALTER TABLE `pharmacysales` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `prescriptiondetails`
--

DROP TABLE IF EXISTS `prescriptiondetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `prescriptiondetails` (
  `PrescriptionDetailID` int NOT NULL AUTO_INCREMENT,
  `PrescriptionID` int NOT NULL,
  `MedicineName` varchar(200) NOT NULL,
  `Dosage` varchar(100) DEFAULT NULL,
  `Frequency` varchar(100) DEFAULT NULL,
  `Duration` varchar(50) DEFAULT NULL,
  `Instructions` text,
  PRIMARY KEY (`PrescriptionDetailID`),
  KEY `PrescriptionID` (`PrescriptionID`),
  CONSTRAINT `prescriptiondetails_ibfk_1` FOREIGN KEY (`PrescriptionID`) REFERENCES `prescriptions` (`PrescriptionID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `prescriptiondetails`
--

LOCK TABLES `prescriptiondetails` WRITE;
/*!40000 ALTER TABLE `prescriptiondetails` DISABLE KEYS */;
INSERT INTO `prescriptiondetails` VALUES (1,1,'Paracetamol','500mg','Twice daily','5 days','With water');
/*!40000 ALTER TABLE `prescriptiondetails` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `prescriptions`
--

DROP TABLE IF EXISTS `prescriptions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `prescriptions` (
  `PrescriptionID` int NOT NULL AUTO_INCREMENT,
  `PrescriptionCode` varchar(20) NOT NULL,
  `VisitID` int NOT NULL,
  `PatientID` int NOT NULL,
  `DoctorID` int NOT NULL,
  `PrescriptionDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `Instructions` text,
  `Status` varchar(20) DEFAULT 'Active',
  PRIMARY KEY (`PrescriptionID`),
  UNIQUE KEY `PrescriptionCode` (`PrescriptionCode`),
  KEY `VisitID` (`VisitID`),
  KEY `PatientID` (`PatientID`),
  KEY `DoctorID` (`DoctorID`),
  CONSTRAINT `prescriptions_ibfk_1` FOREIGN KEY (`VisitID`) REFERENCES `visits` (`VisitID`) ON DELETE CASCADE,
  CONSTRAINT `prescriptions_ibfk_2` FOREIGN KEY (`PatientID`) REFERENCES `patients` (`PatientID`) ON DELETE CASCADE,
  CONSTRAINT `prescriptions_ibfk_3` FOREIGN KEY (`DoctorID`) REFERENCES `doctors` (`DoctorID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `prescriptions`
--

LOCK TABLES `prescriptions` WRITE;
/*!40000 ALTER TABLE `prescriptions` DISABLE KEYS */;
INSERT INTO `prescriptions` VALUES (1,'RX-SEED-001',1,1,1,'2026-02-04 20:33:15','Take after meals','Active');
/*!40000 ALTER TABLE `prescriptions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `rooms`
--

DROP TABLE IF EXISTS `rooms`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `rooms` (
  `RoomID` int NOT NULL AUTO_INCREMENT,
  `RoomNumber` varchar(20) NOT NULL,
  `WardID` int DEFAULT NULL,
  `RoomType` varchar(50) DEFAULT NULL,
  `TotalBeds` int DEFAULT '1',
  `AvailableBeds` int DEFAULT '0',
  `Facilities` text,
  `RatePerDay` decimal(10,2) DEFAULT NULL,
  `Status` varchar(20) DEFAULT 'Available',
  PRIMARY KEY (`RoomID`),
  UNIQUE KEY `RoomNumber` (`RoomNumber`),
  KEY `WardID` (`WardID`),
  CONSTRAINT `rooms_ibfk_1` FOREIGN KEY (`WardID`) REFERENCES `wards` (`WardID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `rooms`
--

LOCK TABLES `rooms` WRITE;
/*!40000 ALTER TABLE `rooms` DISABLE KEYS */;
INSERT INTO `rooms` VALUES (1,'101',1,'General',4,4,NULL,100.00,'Available'),(2,'102',1,'General',4,4,NULL,100.00,'Available'),(3,'201',2,'General',3,3,NULL,100.00,'Available'),(4,'301',3,'Private',1,1,NULL,200.00,'Available'),(5,'302',3,'Private',1,1,NULL,200.00,'Available'),(6,'401',4,'ICU',1,1,NULL,500.00,'Available'),(7,'501',5,'Maternity',2,2,NULL,150.00,'Available'),(8,'RM-SEED-001',9,'General',2,2,'AC, Monitoring',150.00,'Available');
/*!40000 ALTER TABLE `rooms` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `servicecategories`
--

DROP TABLE IF EXISTS `servicecategories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `servicecategories` (
  `CategoryID` int NOT NULL AUTO_INCREMENT,
  `CategoryName` varchar(100) NOT NULL,
  `Description` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`CategoryID`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `servicecategories`
--

LOCK TABLES `servicecategories` WRITE;
/*!40000 ALTER TABLE `servicecategories` DISABLE KEYS */;
INSERT INTO `servicecategories` VALUES (1,'Consultation','Doctor consultation fees'),(2,'Laboratory','Lab tests and diagnostics'),(3,'Pharmacy','Medicines and drugs'),(4,'Room Charges','Hospital room charges'),(5,'Procedure','Medical procedures'),(6,'Emergency','Emergency services'),(7,'Diagnostics','Diagnostic services'),(8,'Others','Other services');
/*!40000 ALTER TABLE `servicecategories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `services`
--

DROP TABLE IF EXISTS `services`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `services` (
  `ServiceID` int NOT NULL AUTO_INCREMENT,
  `ServiceCode` varchar(20) NOT NULL,
  `ServiceName` varchar(200) NOT NULL,
  `CategoryID` int DEFAULT NULL,
  `Price` decimal(10,2) NOT NULL,
  `TaxRate` decimal(5,2) DEFAULT '0.00',
  `IsActive` tinyint(1) DEFAULT '1',
  PRIMARY KEY (`ServiceID`),
  UNIQUE KEY `ServiceCode` (`ServiceCode`),
  KEY `CategoryID` (`CategoryID`),
  CONSTRAINT `services_ibfk_1` FOREIGN KEY (`CategoryID`) REFERENCES `servicecategories` (`CategoryID`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `services`
--

LOCK TABLES `services` WRITE;
/*!40000 ALTER TABLE `services` DISABLE KEYS */;
INSERT INTO `services` VALUES (1,'CON-GEN','General Consultation',1,30.00,0.00,1),(2,'CON-SPEC','Specialist Consultation',1,50.00,0.00,1),(3,'LAB-CBC','Complete Blood Count',2,25.00,5.00,1),(4,'LAB-URINE','Urine Analysis',2,15.00,5.00,1),(5,'ROOM-GEN','General Ward - Per Day',4,100.00,10.00,1),(6,'ROOM-PVT','Private Room - Per Day',4,200.00,10.00,1),(7,'EMG-BASIC','Basic Emergency',6,150.00,0.00,1),(8,'XRAY-CHEST','Chest X-Ray',7,80.00,5.00,1),(9,'SRV-SEED-001','General Consultation',1,50.00,5.00,1);
/*!40000 ALTER TABLE `services` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `specializations`
--

DROP TABLE IF EXISTS `specializations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `specializations` (
  `SpecializationID` int NOT NULL AUTO_INCREMENT,
  `SpecializationCode` varchar(20) NOT NULL,
  `SpecializationName` varchar(100) NOT NULL,
  `Description` varchar(255) DEFAULT NULL,
  `Department` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`SpecializationID`),
  UNIQUE KEY `SpecializationCode` (`SpecializationCode`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `specializations`
--

LOCK TABLES `specializations` WRITE;
/*!40000 ALTER TABLE `specializations` DISABLE KEYS */;
INSERT INTO `specializations` VALUES (1,'CARD','Cardiology',NULL,'Cardiology'),(2,'NEURO','Neurology',NULL,'Neurology'),(3,'ORTHO','Orthopedics',NULL,'Orthopedics'),(4,'PED','Pediatrics',NULL,'Pediatrics'),(5,'GYN','Gynecology',NULL,'Gynecology'),(6,'DERMA','Dermatology',NULL,'Dermatology'),(7,'ENT','ENT',NULL,'ENT'),(8,'GEN','General Medicine',NULL,'Medicine'),(9,'SURG','General Surgery',NULL,'Surgery'),(10,'PSY','Psychiatry',NULL,'Psychiatry');
/*!40000 ALTER TABLE `specializations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `staff`
--

DROP TABLE IF EXISTS `staff`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `staff` (
  `StaffID` int NOT NULL AUTO_INCREMENT,
  `UserID` int NOT NULL,
  `StaffCode` varchar(20) NOT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `Department` varchar(100) DEFAULT NULL,
  `Shift` varchar(20) DEFAULT NULL,
  `HireDate` date DEFAULT (curdate()),
  `Salary` decimal(10,2) DEFAULT NULL,
  PRIMARY KEY (`StaffID`),
  UNIQUE KEY `UserID` (`UserID`),
  UNIQUE KEY `StaffCode` (`StaffCode`),
  CONSTRAINT `staff_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `staff`
--

LOCK TABLES `staff` WRITE;
/*!40000 ALTER TABLE `staff` DISABLE KEYS */;
INSERT INTO `staff` VALUES (1,4,'S-001','Head Nurse','Emergency','Day','2026-02-03',3500.00),(2,5,'S-002','Receptionist','Front Desk','Day','2026-02-03',2500.00);
/*!40000 ALTER TABLE `staff` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `systemsettings`
--

DROP TABLE IF EXISTS `systemsettings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `systemsettings` (
  `SettingID` int NOT NULL AUTO_INCREMENT,
  `SettingKey` varchar(100) NOT NULL,
  `SettingValue` text,
  `Description` varchar(255) DEFAULT NULL,
  `Category` varchar(50) DEFAULT NULL,
  `LastModified` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`SettingID`),
  UNIQUE KEY `SettingKey` (`SettingKey`)
) ENGINE=InnoDB AUTO_INCREMENT=27 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `systemsettings`
--

LOCK TABLES `systemsettings` WRITE;
/*!40000 ALTER TABLE `systemsettings` DISABLE KEYS */;
INSERT INTO `systemsettings` VALUES (1,'HospitalName','General Hospital','Name of the hospital','General','2026-02-03 22:26:40'),(2,'HospitalAddress','123 Medical Street, Health City','Hospital address','General','2026-02-03 22:26:40'),(3,'HospitalPhone','+1 (234) 567-8900','Hospital contact number','General','2026-02-03 22:26:40'),(4,'HospitalEmail','info@generalhospital.com','Hospital email','General','2026-02-03 22:26:40'),(5,'Currency','USD','Default currency','Financial','2026-02-03 22:26:40'),(6,'TaxRate','10.00','Default tax rate percentage','Financial','2026-02-03 22:26:40'),(7,'AppointmentDuration','15','Default appointment duration in minutes','Appointments','2026-02-03 22:26:40'),(8,'MaxAppointmentsPerDay','30','Maximum appointments per doctor per day','Appointments','2026-02-03 22:26:40'),(9,'AutoGeneratePatientCode','true','Auto generate patient codes','System','2026-02-03 22:26:40'),(10,'AutoGenerateInvoice','true','Auto generate invoice numbers','System','2026-02-03 22:26:40'),(11,'DateFormat','yyyy-MM-dd','Date format','System','2026-02-03 22:26:40'),(12,'TimeFormat','HH:mm:ss','Time format','System','2026-02-03 22:26:40'),(13,'BackupPath','C:HospitalBackups','Database backup path','System','2026-02-03 22:26:40'),(14,'BackupFrequency','daily','Backup frequency','System','2026-02-03 22:26:40'),(15,'SessionTimeout','30','Session timeout in minutes','Security','2026-02-03 22:26:40'),(16,'PasswordExpiryDays','90','Password expiry in days','Security','2026-02-03 22:26:40'),(17,'MinPasswordLength','8','Minimum password length','Security','2026-02-03 22:26:40'),(18,'MaxLoginAttempts','3','Maximum login attempts before lock','Security','2026-02-03 22:26:40'),(19,'LockoutDuration','30','Account lockout duration in minutes','Security','2026-02-03 22:26:40'),(20,'EmailNotifications','true','Enable email notifications','Notifications','2026-02-03 22:26:40'),(21,'SMSNotifications','false','Enable SMS notifications','Notifications','2026-02-03 22:26:40'),(22,'AppointmentReminderHours','24','Appointment reminder hours before','Notifications','2026-02-03 22:26:40'),(23,'PharmacyLowStockThreshold','20','Low stock threshold percentage','Inventory','2026-02-03 22:26:40'),(24,'LabResultTimeoutHours','48','Lab result timeout in hours','Laboratory','2026-02-03 22:26:40'),(25,'InvoiceDueDays','30','Invoice due days','Billing','2026-02-03 22:26:40'),(26,'SeedConfigured','true','Flag indicating seed script ran','System','2026-02-04 20:33:16');
/*!40000 ALTER TABLE `systemsettings` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `userdetails`
--

DROP TABLE IF EXISTS `userdetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `userdetails` (
  `UserDetailID` int NOT NULL AUTO_INCREMENT,
  `UserID` int NOT NULL,
  `FirstName` varchar(50) NOT NULL,
  `LastName` varchar(50) NOT NULL,
  `DateOfBirth` date DEFAULT NULL,
  `Gender` enum('M','F','O') DEFAULT NULL,
  `ContactNumber` varchar(20) DEFAULT NULL,
  `Address` varchar(255) DEFAULT NULL,
  `EmergencyContact` varchar(20) DEFAULT NULL,
  `ProfileImage` longblob,
  PRIMARY KEY (`UserDetailID`),
  UNIQUE KEY `UserID` (`UserID`),
  CONSTRAINT `userdetails_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `userdetails`
--

LOCK TABLES `userdetails` WRITE;
/*!40000 ALTER TABLE `userdetails` DISABLE KEYS */;
INSERT INTO `userdetails` VALUES (1,1,'System','Administrator','1980-01-01','M','+1234567890',NULL,NULL,NULL),(2,2,'John','Smith','1975-05-15','M','+1234567891',NULL,NULL,NULL),(3,3,'Sarah','Jones','1982-08-20','F','+1234567892',NULL,NULL,NULL),(4,4,'Mary','Johnson','1990-03-10','F','+1234567893',NULL,NULL,NULL),(5,5,'John','Doe','1988-11-25','M','+1234567894',NULL,NULL,NULL);
/*!40000 ALTER TABLE `userdetails` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `userroles`
--

DROP TABLE IF EXISTS `userroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `userroles` (
  `RoleID` int NOT NULL AUTO_INCREMENT,
  `RoleName` varchar(50) NOT NULL,
  `Description` varchar(255) DEFAULT NULL,
  `CreatedDate` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`RoleID`),
  UNIQUE KEY `RoleName` (`RoleName`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `userroles`
--

LOCK TABLES `userroles` WRITE;
/*!40000 ALTER TABLE `userroles` DISABLE KEYS */;
INSERT INTO `userroles` VALUES (1,'Administrator','Full system access','2026-02-03 22:26:40'),(2,'Doctor','Medical staff with patient care access','2026-02-03 22:26:40'),(3,'Nurse','Nursing staff with limited access','2026-02-03 22:26:40'),(4,'Receptionist','Front desk and appointment management','2026-02-03 22:26:40'),(5,'Pharmacist','Pharmacy management','2026-02-03 22:26:40'),(6,'Lab Technician','Laboratory test management','2026-02-03 22:26:40'),(7,'Accountant','Billing and financial management','2026-02-03 22:26:40'),(8,'HR Manager','Human resources management','2026-02-03 22:26:40');
/*!40000 ALTER TABLE `userroles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `UserID` int NOT NULL AUTO_INCREMENT,
  `Username` varchar(50) NOT NULL,
  `PasswordHash` varchar(255) NOT NULL,
  `Email` varchar(100) DEFAULT NULL,
  `RoleID` int NOT NULL,
  `IsActive` tinyint(1) DEFAULT '1',
  `LastLogin` datetime DEFAULT NULL,
  `CreatedDate` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`UserID`),
  UNIQUE KEY `Username` (`Username`),
  UNIQUE KEY `Email` (`Email`),
  KEY `idx_users_username` (`Username`),
  KEY `idx_users_role` (`RoleID`,`IsActive`),
  CONSTRAINT `users_ibfk_1` FOREIGN KEY (`RoleID`) REFERENCES `userroles` (`RoleID`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'admin','$2a$12$LQv3c1yqB3CdeL4gGgGgT.E3QY9Y4W9zYbJ9cZ8vYbJ9cZ8vYbJ9c','admin@hospital.com',1,1,'2026-02-04 20:45:23','2026-02-03 22:26:40'),(2,'dr.smith','$2a$12$LQv3c1yqB3CdeL4gGgGgT.E3QY9Y4W9zYbJ9cZ8vYbJ9cZ8vYbJ9c','dr.smith@hospital.com',2,1,'2026-02-04 20:23:33','2026-02-03 22:26:40'),(3,'dr.jones','$2a$12$LQv3c1yqB3CdeL4gGgGgT.E3QY9Y4W9zYbJ9cZ8vYbJ9cZ8vYbJ9c','dr.jones@hospital.com',2,1,NULL,'2026-02-03 22:26:40'),(4,'nurse.mary','$2a$12$LQv3c1yqB3CdeL4gGgGgT.E3QY9Y4W9zYbJ9cZ8vYbJ9cZ8vYbJ9c','nurse.mary@hospital.com',3,1,'2026-02-04 20:24:18','2026-02-03 22:26:40'),(5,'reception.john','$2a$12$LQv3c1yqB3CdeL4gGgGgT.E3QY9Y4W9zYbJ9cZ8vYbJ9cZ8vYbJ9c','reception@hospital.com',4,1,'2026-02-04 20:24:41','2026-02-03 22:26:40');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `visits`
--

DROP TABLE IF EXISTS `visits`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `visits` (
  `VisitID` int NOT NULL AUTO_INCREMENT,
  `VisitCode` varchar(20) NOT NULL,
  `PatientID` int NOT NULL,
  `DoctorID` int NOT NULL,
  `AppointmentID` int DEFAULT NULL,
  `VisitDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `Symptoms` text,
  `Diagnosis` text,
  `Treatment` text,
  `FollowUpDate` date DEFAULT NULL,
  `VisitStatus` varchar(20) DEFAULT 'Completed',
  `CreatedBy` int DEFAULT NULL,
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
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `visits`
--

LOCK TABLES `visits` WRITE;
/*!40000 ALTER TABLE `visits` DISABLE KEYS */;
INSERT INTO `visits` VALUES (1,'VIS-SEED-001',1,1,1,'2026-02-04 20:33:15','Headache','Migraine','Pain management','2026-02-11','Completed',2);
/*!40000 ALTER TABLE `visits` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `wards`
--

DROP TABLE IF EXISTS `wards`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `wards` (
  `WardID` int NOT NULL AUTO_INCREMENT,
  `WardCode` varchar(20) NOT NULL,
  `WardName` varchar(100) NOT NULL,
  `WardType` varchar(50) DEFAULT NULL,
  `Description` varchar(255) DEFAULT NULL,
  `TotalBeds` int DEFAULT '0',
  `AvailableBeds` int DEFAULT '0',
  `ChargePerDay` decimal(10,2) DEFAULT NULL,
  `IsActive` tinyint(1) DEFAULT '1',
  PRIMARY KEY (`WardID`),
  UNIQUE KEY `WardCode` (`WardCode`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `wards`
--

LOCK TABLES `wards` WRITE;
/*!40000 ALTER TABLE `wards` DISABLE KEYS */;
INSERT INTO `wards` VALUES (1,'WARD-001','General Ward A','General',NULL,30,30,100.00,1),(2,'WARD-002','General Ward B','General',NULL,25,25,100.00,1),(3,'WARD-003','Private Ward','Private',NULL,15,15,200.00,1),(4,'WARD-004','ICU','ICU',NULL,10,10,500.00,1),(5,'WARD-005','Maternity Ward','Maternity',NULL,20,20,150.00,1),(6,'WARD-006','Pediatric Ward','Pediatric',NULL,18,18,120.00,1),(7,'WARD-007','Isolation Ward','Isolation',NULL,8,8,250.00,1),(8,'WARD-008','Post-Op Ward','Post-Operative',NULL,12,12,180.00,1),(9,'WARD-SEED-001','General Ward Seed','General','Seed ward',20,20,120.00,1);
/*!40000 ALTER TABLE `wards` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-02-05 11:56:45
