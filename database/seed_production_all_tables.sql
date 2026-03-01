SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS=1;
SET @run := DATE_FORMAT(NOW(), '%y%m%d%H%i%s');
SET @admin_hash := (SELECT PasswordHash FROM users WHERE Username='admin' LIMIT 1);

START TRANSACTION;

INSERT INTO users (Username, PasswordHash, Email, RoleID, IsActive, LastLogin)
VALUES
(CONCAT('acct.', @run), @admin_hash, CONCAT('acct.', @run, '@hospital.local'), (SELECT RoleID FROM userroles WHERE RoleName='Accountant' LIMIT 1), 1, '2026-02-27 09:11:00'),
(CONCAT('pharma.', @run), @admin_hash, CONCAT('pharma.', @run, '@hospital.local'), (SELECT RoleID FROM userroles WHERE RoleName='Pharmacist' LIMIT 1), 1, '2026-02-27 10:42:00'),
(CONCAT('labtech.', @run), @admin_hash, CONCAT('labtech.', @run, '@hospital.local'), (SELECT RoleID FROM userroles WHERE RoleName='Lab Technician' LIMIT 1), 1, '2026-02-26 08:26:00'),
(CONCAT('hr.', @run), @admin_hash, CONCAT('hr.', @run, '@hospital.local'), (SELECT RoleID FROM userroles WHERE RoleName='HR Manager' LIMIT 1), 1, '2026-02-25 14:10:00');

INSERT INTO userdetails (UserID, FirstName, LastName, DateOfBirth, Gender, ContactNumber, Address, EmergencyContact)
SELECT u.UserID, ud.FirstName, ud.LastName, ud.DateOfBirth, ud.Gender, ud.ContactNumber, ud.Address, ud.EmergencyContact
FROM (
    SELECT CONCAT('acct.', @run) AS Username, 'Jovelyn' AS FirstName, 'Marasigan' AS LastName, '1989-08-17' AS DateOfBirth, 'F' AS Gender, '09177122334' AS ContactNumber, 'Brgy. Bajada, Davao City, Davao del Sur' AS Address, '09175550001' AS EmergencyContact
    UNION ALL SELECT CONCAT('pharma.', @run), 'Frederick', 'Daculan', '1988-11-05', 'M', '09178233445', 'Brgy. San Isidro, Panabo City, Davao del Norte', '09175550002'
    UNION ALL SELECT CONCAT('labtech.', @run), 'Sheryl', 'Nicdao', '1992-03-30', 'F', '09179344556', 'Brgy. Poblacion, Tagum City, Davao del Norte', '09175550003'
    UNION ALL SELECT CONCAT('hr.', @run), 'Ivan', 'Tampus', '1987-06-21', 'M', '09170455667', 'Brgy. Talomo, Davao City, Davao del Sur', '09175550004'
) ud
INNER JOIN users u ON u.Username = ud.Username;

INSERT INTO staff (UserID, StaffCode, Designation, Department, Shift, HireDate, Salary)
SELECT u.UserID,
       CASE
          WHEN u.Username = CONCAT('acct.', @run) THEN CONCAT('STF', RIGHT(@run, 6), '01')
          WHEN u.Username = CONCAT('pharma.', @run) THEN CONCAT('STF', RIGHT(@run, 6), '02')
          WHEN u.Username = CONCAT('labtech.', @run) THEN CONCAT('STF', RIGHT(@run, 6), '03')
          ELSE CONCAT('STF', RIGHT(@run, 6), '04')
       END,
       CASE
          WHEN u.Username = CONCAT('acct.', @run) THEN 'Senior Accountant'
          WHEN u.Username = CONCAT('pharma.', @run) THEN 'Chief Pharmacist'
          WHEN u.Username = CONCAT('labtech.', @run) THEN 'Medical Technologist'
          ELSE 'HR Manager'
       END,
       CASE
          WHEN u.Username = CONCAT('acct.', @run) THEN 'Finance'
          WHEN u.Username = CONCAT('pharma.', @run) THEN 'Pharmacy'
          WHEN u.Username = CONCAT('labtech.', @run) THEN 'Laboratory'
          ELSE 'Human Resources'
       END,
       'Day',
       CASE
          WHEN u.Username = CONCAT('acct.', @run) THEN '2023-05-03'
          WHEN u.Username = CONCAT('pharma.', @run) THEN '2023-07-19'
          WHEN u.Username = CONCAT('labtech.', @run) THEN '2023-09-11'
          ELSE '2023-02-08'
       END,
       CASE
          WHEN u.Username = CONCAT('acct.', @run) THEN 42000
          WHEN u.Username = CONCAT('pharma.', @run) THEN 39000
          WHEN u.Username = CONCAT('labtech.', @run) THEN 36000
          ELSE 45000
       END
FROM users u
WHERE u.Username IN (CONCAT('acct.', @run), CONCAT('pharma.', @run), CONCAT('labtech.', @run), CONCAT('hr.', @run));

DROP TEMPORARY TABLE IF EXISTS tmp_doctor_seed;
CREATE TEMPORARY TABLE tmp_doctor_seed (
  rn INT PRIMARY KEY,
  FirstName VARCHAR(50),
  LastName VARCHAR(50),
  Gender CHAR(1),
  SpecializationName VARCHAR(100),
  Qualification VARCHAR(255),
  Experience INT,
  Fee DECIMAL(10,2),
  JoinDate DATE,
  Department VARCHAR(100)
);

INSERT INTO tmp_doctor_seed VALUES
(1,'Ramon','Alegre','M','Cardiology','MD, FPCP, FPCC',15,2200.00,'2023-03-11','Cardiology'),
(2,'Liza','Cabahug','F','Pediatrics','MD, DPS',11,1800.00,'2023-04-18','Pediatrics'),
(3,'Edwin','Carandang','M','General Medicine','MD, FPCP',13,2000.00,'2023-06-05','Internal Medicine'),
(4,'Rica','Matias','F','General Surgery','MD, FPSGS',17,3200.00,'2023-07-22','Surgery'),
(5,'Marjorie','Talavera','F','Gynecology','MD, FPOGS',14,2500.00,'2023-08-14','OB-GYN'),
(6,'Noel','Panganiban','M','Neurology','MD, DPNS',16,2800.00,'2023-10-03','Neurology'),
(7,'Jessa','Villarta','F','Orthopedics','MD, FPSO',12,2600.00,'2024-01-19','Orthopedics'),
(8,'Arnel','Quimpo','M','ENT','MD, FPSOHNS',10,2100.00,'2024-03-08','ENT'),
(9,'Kathlyn','Bermudez','F','Dermatology','MD, FPDS',9,1900.00,'2024-04-27','Dermatology'),
(10,'Paolo','Serrano','M','Psychiatry','MD, DPSP',8,2300.00,'2024-06-15','Psychiatry');

INSERT INTO users (Username, PasswordHash, Email, RoleID, IsActive, LastLogin)
SELECT CONCAT('dr.', LOWER(REPLACE(ds.LastName,' ','')), '.', RIGHT(@run,4), LPAD(ds.rn,2,'0')),
       @admin_hash,
       CONCAT('dr.', LOWER(REPLACE(ds.LastName,' ','')), '.', RIGHT(@run,4), LPAD(ds.rn,2,'0'), '@hospital.local'),
       (SELECT RoleID FROM userroles WHERE RoleName='Doctor' LIMIT 1),
       1,
       '2026-02-28 08:00:00'
FROM tmp_doctor_seed ds;

INSERT INTO userdetails (UserID, FirstName, LastName, DateOfBirth, Gender, ContactNumber, Address, EmergencyContact)
SELECT u.UserID,
       ds.FirstName,
       ds.LastName,
       DATE_ADD('1978-01-01', INTERVAL (ds.rn * 310) DAY),
       ds.Gender,
       CONCAT('09', LPAD(100000000 + ds.rn * 777777, 9, '0')),
       CONCAT('Brgy. ', ELT(ds.rn, 'San Isidro','Poblacion','Mabini','San Jose','Sto. Nino','Maligaya','Buhangin','Talomo','Fatima','Agdao'), ', ', ELT(ds.rn, 'Davao City','Panabo City','Tagum City','Mati City','General Santos City','Kidapawan City','Koronadal City','Cagayan de Oro','Iligan City','Butuan City')),
       CONCAT('09', LPAD(200000000 + ds.rn * 666666, 9, '0'))
FROM tmp_doctor_seed ds
INNER JOIN users u ON u.Username = CONCAT('dr.', LOWER(REPLACE(ds.LastName,' ','')), '.', RIGHT(@run,4), LPAD(ds.rn,2,'0'));

INSERT INTO staff (UserID, StaffCode, Designation, Department, Shift, HireDate, Salary)
SELECT u.UserID,
       CONCAT('MDS', RIGHT(@run, 6), LPAD(ds.rn,2,'0')),
       'Consultant Doctor',
       ds.Department,
       'Day',
       ds.JoinDate,
       32000 + (ds.Experience * 1800)
FROM tmp_doctor_seed ds
INNER JOIN users u ON u.Username = CONCAT('dr.', LOWER(REPLACE(ds.LastName,' ','')), '.', RIGHT(@run,4), LPAD(ds.rn,2,'0'));

INSERT INTO doctors (UserID, DoctorCode, SpecializationID, Qualification, LicenseNumber, YearsOfExperience, ConsultationFee, IsAvailable, JoiningDate)
SELECT u.UserID,
       CONCAT('DOC', @run, LPAD(ds.rn,2,'0')),
       s.SpecializationID,
       ds.Qualification,
       CONCAT('PRC-MD-', RIGHT(@run, 8), LPAD(ds.rn,3,'0')),
       ds.Experience,
       ds.Fee,
       1,
       ds.JoinDate
FROM tmp_doctor_seed ds
INNER JOIN users u ON u.Username = CONCAT('dr.', LOWER(REPLACE(ds.LastName,' ','')), '.', RIGHT(@run,4), LPAD(ds.rn,2,'0'))
LEFT JOIN specializations s ON s.SpecializationName = ds.SpecializationName;

INSERT INTO doctorschedules (DoctorID, DayOfWeek, StartTime, EndTime, MaxAppointments, IsActive)
SELECT d.DoctorID,
       dayref.DayOfWeek,
       '08:00:00',
       CASE WHEN MOD(dayref.DayOfWeek + dayref.rn, 2) = 0 THEN '16:30:00' ELSE '17:00:00' END,
       16 + MOD(dayref.DayOfWeek + dayref.rn, 6),
       1
FROM (
  SELECT rn, DayOfWeek
  FROM (
    SELECT 1 AS rn, 1 AS DayOfWeek UNION ALL SELECT 1,2 UNION ALL SELECT 1,3 UNION ALL SELECT 1,4 UNION ALL SELECT 1,5
    UNION ALL SELECT 2,1 UNION ALL SELECT 2,2 UNION ALL SELECT 2,3 UNION ALL SELECT 2,4 UNION ALL SELECT 2,5
    UNION ALL SELECT 3,1 UNION ALL SELECT 3,2 UNION ALL SELECT 3,3 UNION ALL SELECT 3,4 UNION ALL SELECT 3,5
    UNION ALL SELECT 4,1 UNION ALL SELECT 4,2 UNION ALL SELECT 4,3 UNION ALL SELECT 4,4 UNION ALL SELECT 4,5
    UNION ALL SELECT 5,1 UNION ALL SELECT 5,2 UNION ALL SELECT 5,3 UNION ALL SELECT 5,4 UNION ALL SELECT 5,5
    UNION ALL SELECT 6,1 UNION ALL SELECT 6,2 UNION ALL SELECT 6,3 UNION ALL SELECT 6,4 UNION ALL SELECT 6,5
    UNION ALL SELECT 7,1 UNION ALL SELECT 7,2 UNION ALL SELECT 7,3 UNION ALL SELECT 7,4 UNION ALL SELECT 7,5
    UNION ALL SELECT 8,1 UNION ALL SELECT 8,2 UNION ALL SELECT 8,3 UNION ALL SELECT 8,4 UNION ALL SELECT 8,5
    UNION ALL SELECT 9,1 UNION ALL SELECT 9,2 UNION ALL SELECT 9,3 UNION ALL SELECT 9,4 UNION ALL SELECT 9,5
    UNION ALL SELECT 10,1 UNION ALL SELECT 10,2 UNION ALL SELECT 10,3 UNION ALL SELECT 10,4 UNION ALL SELECT 10,5
  ) x
) dayref
INNER JOIN doctors d ON d.DoctorCode = CONCAT('DOC', @run, LPAD(dayref.rn,2,'0'));

DROP TEMPORARY TABLE IF EXISTS tmp_given_names;
CREATE TEMPORARY TABLE tmp_given_names (
  id INT AUTO_INCREMENT PRIMARY KEY,
  FirstName VARCHAR(50),
  Gender CHAR(1)
);

INSERT INTO tmp_given_names (FirstName, Gender) VALUES
('Andrei','M'),('Paolo','M'),('Miguel','M'),('Jericho','M'),('Rafael','M'),('Bryan','M'),('Carlo','M'),('Nathaniel','M'),('Joshua','M'),('Kevin','M'),
('Alvin','M'),('Jomar','M'),('Dennis','M'),('Arvin','M'),('Ronald','M'),('Emmanuel','M'),('Mark','M'),('Lester','M'),('Erwin','M'),('Gerald','M'),
('Alyssa','F'),('Katrina','F'),('Janelle','F'),('Camille','F'),('Patricia','F'),('Bea','F'),('Angelica','F'),('Rica','F'),('Clarisse','F'),('Shaina','F'),
('Mica','F'),('Bianca','F'),('Trisha','F'),('Joyce','F'),('Kaye','F'),('Erika','F'),('Dianne','F'),('Nica','F'),('Hazel','F'),('Janine','F');

DROP TEMPORARY TABLE IF EXISTS tmp_surnames;
CREATE TEMPORARY TABLE tmp_surnames (
  id INT AUTO_INCREMENT PRIMARY KEY,
  LastName VARCHAR(50)
);

INSERT INTO tmp_surnames (LastName) VALUES
('Dela Cruz'),('Santos'),('Reyes'),('Bautista'),('Garcia'),('Mendoza'),('Torres'),('Ramos'),('Flores'),('Gonzales'),
('Fernandez'),('Navarro'),('Villanueva'),('Aguilar'),('Castillo'),('Soriano'),('Domingo'),('Aquino'),('Mercado'),('Salazar'),
('Pascual'),('De Guzman'),('Valdez'),('Cabrera'),('Padilla'),('Lim'),('Tan'),('Abad'),('Rosales'),('Malabanan'),
('Manalo'),('San Pedro'),('Trinidad'),('Yap'),('Ocampo'),('Natividad'),('Arce'),('Buenaventura'),('Samonte'),('Crisostomo'),
('Lazaro'),('Panganiban'),('Talavera'),('Lopez'),('Serrano'),('Alcantara'),('Balagtas'),('Cabangon'),('David'),('Pineda');

SET @gcount := (SELECT COUNT(*) FROM tmp_given_names);
SET @scount := (SELECT COUNT(*) FROM tmp_surnames);

INSERT INTO patients (PatientCode, FirstName, LastName, DateOfBirth, Gender, BloodGroup, MaritalStatus, Nationality, IdentificationType, IdentificationNumber, RegistrationDate, IsActive)
WITH RECURSIVE seq AS (
    SELECT 1 AS n
    UNION ALL
    SELECT n + 1 FROM seq WHERE n < 50
)
SELECT CONCAT('PAT', @run, LPAD(s.n,2,'0')),
       g.FirstName,
       sn.LastName,
       DATE_ADD('1950-01-01', INTERVAL MOD(s.n * 521, 25500) DAY),
       g.Gender,
       ELT(1 + MOD(s.n * 3, 8), 'A+','A-','B+','B-','AB+','AB-','O+','O-'),
       ELT(1 + MOD(s.n, 3), 'Single', 'Married', 'Widowed'),
       'Filipino',
       ELT(1 + MOD(s.n, 4), 'PhilSys', 'Passport', 'Driver''s License', 'Voter''s ID'),
       CONCAT('ID-', RIGHT(@run,8), '-', LPAD(s.n,4,'0')),
       DATE_ADD('2023-01-10 08:00:00', INTERVAL MOD(s.n * 29, 1260) DAY) + INTERVAL MOD(s.n * 17, 660) MINUTE,
       1
FROM seq s
INNER JOIN tmp_given_names g ON g.id = 1 + MOD(s.n * 7, @gcount)
INNER JOIN tmp_surnames sn ON sn.id = 1 + MOD(s.n * 11, @scount);

DROP TEMPORARY TABLE IF EXISTS tmp_seed_patients;
CREATE TEMPORARY TABLE tmp_seed_patients AS
SELECT ROW_NUMBER() OVER (ORDER BY PatientID) AS rn,
       PatientID,
       PatientCode,
       RegistrationDate
FROM patients
WHERE PatientCode LIKE CONCAT('PAT', @run, '%');

SET @pcount := (SELECT COUNT(*) FROM tmp_seed_patients);

INSERT INTO patientcontacts (PatientID, ContactType, ContactValue, IsPrimary)
SELECT p.PatientID,
       'Phone',
       CONCAT('09', LPAD(100000000 + MOD(p.rn * 9137, 900000000), 9, '0')),
       1
FROM tmp_seed_patients p;

INSERT INTO patientcontacts (PatientID, ContactType, ContactValue, IsPrimary)
SELECT p.PatientID,
       'Address',
       CONCAT('Blk ', 1 + MOD(p.rn * 3, 120),
              ' Lot ', 1 + MOD(p.rn * 5, 60),
              ', Brgy. ', ELT(1 + MOD(p.rn, 10), 'San Isidro','Poblacion','Mabini','San Jose','Sto. Nino','Maligaya','Buhangin','Talomo','Fatima','Agdao'),
              ', ', ELT(1 + MOD(p.rn, 8), 'Davao City','Tagum City','Panabo City','Mati City','General Santos City','Kidapawan City','Koronadal City','Cagayan de Oro'),
              ', ', ELT(1 + MOD(p.rn, 6), 'Davao del Sur','Davao del Norte','Davao de Oro','South Cotabato','Bukidnon','Misamis Oriental')),
       1
FROM tmp_seed_patients p;

INSERT INTO patientcontacts (PatientID, ContactType, ContactValue, IsPrimary)
SELECT p.PatientID,
       'Email',
       CONCAT('patient', LPAD(p.rn,3,'0'), '.', RIGHT(@run,4), '@mail.local'),
       0
FROM tmp_seed_patients p;

INSERT INTO medicalhistories (PatientID, HistoryType, Description, DiagnosisDate, Severity, Status, RecordedBy, RecordedDate)
SELECT p.PatientID,
       'Chronic Condition',
       ELT(1 + MOD(p.rn, 8),
           'Essential hypertension',
           'Type 2 diabetes mellitus',
           'Bronchial asthma',
           'Hyperlipidemia',
           'Migraine episodes',
           'Allergic rhinitis',
           'Lumbar strain',
           'Osteoarthritis'),
       DATE_SUB(DATE(p.RegistrationDate), INTERVAL 30 + MOD(p.rn * 17, 900) DAY),
       ELT(1 + MOD(p.rn, 3), 'Mild','Moderate','Severe'),
       'Active',
       (SELECT UserID FROM users WHERE Username='admin' LIMIT 1),
       DATE_SUB(p.RegistrationDate, INTERVAL 2 DAY)
FROM tmp_seed_patients p
WHERE MOD(p.rn, 2) = 0;

DROP TEMPORARY TABLE IF EXISTS tmp_seed_doctors;
CREATE TEMPORARY TABLE tmp_seed_doctors AS
SELECT ROW_NUMBER() OVER (ORDER BY DoctorID) AS rn,
       DoctorID,
       DoctorCode,
       UserID
FROM doctors
WHERE DoctorCode LIKE CONCAT('DOC', @run, '%');

SET @dcount := (SELECT COUNT(*) FROM tmp_seed_doctors);

INSERT INTO appointments (AppointmentCode, PatientID, DoctorID, AppointmentDate, AppointmentTime, AppointmentType, Status, Reason, Duration, CreatedBy, CreatedDate, Notes)
WITH RECURSIVE seq AS (
    SELECT 1 AS n
    UNION ALL
    SELECT n + 1 FROM seq WHERE n < 120
)
SELECT CONCAT('APT', @run, LPAD(s.n,4,'0')),
       p.PatientID,
       d.DoctorID,
       DATE(LEAST('2026-12-20', DATE_ADD(DATE(p.RegistrationDate), INTERVAL 7 + MOD(s.n * 13, 900) DAY))),
       ELT(1 + MOD(s.n, 10), '08:00:00','08:30:00','09:00:00','09:30:00','10:00:00','10:30:00','11:00:00','13:00:00','14:00:00','15:00:00'),
       ELT(1 + MOD(s.n, 4), 'Consultation', 'Follow-up', 'Emergency', 'Check-up'),
       CASE
         WHEN s.n <= 78 THEN 'Completed'
         WHEN s.n <= 102 THEN 'Scheduled'
         ELSE 'Cancelled'
       END,
       ELT(1 + MOD(s.n, 12),
           'Chest discomfort and elevated blood pressure',
           'Persistent cough and intermittent fever',
           'Follow-up for diabetes management',
           'Prenatal check-up and fetal monitoring',
           'Joint pain and morning stiffness',
           'Skin rash with itching for one week',
           'Migraine episodes with nausea',
           'Pediatric vaccination consultation',
           'Abdominal pain after meals',
           'Routine annual physical examination',
           'Urinary tract infection symptoms',
           'Minor trauma from motorcycle fall'),
       ELT(1 + MOD(s.n, 4), 15, 20, 30, 45),
       (SELECT UserID FROM users WHERE RoleID=4 ORDER BY UserID LIMIT 1),
       GREATEST(
         p.RegistrationDate,
         DATE_SUB(
           CONCAT(DATE(LEAST('2026-12-20', DATE_ADD(DATE(p.RegistrationDate), INTERVAL 7 + MOD(s.n * 13, 900) DAY))), ' ', ELT(1 + MOD(s.n, 10), '08:00:00','08:30:00','09:00:00','09:30:00','10:00:00','10:30:00','11:00:00','13:00:00','14:00:00','15:00:00')),
           INTERVAL 1 + MOD(s.n, 12) DAY
         )
       ),
       CASE WHEN s.n <= 102 THEN 'Patient advised to arrive 15 minutes before schedule.' ELSE 'Cancelled by patient due to schedule conflict.' END
FROM seq s
INNER JOIN tmp_seed_patients p ON p.rn = 1 + MOD(s.n * 3, @pcount)
INNER JOIN tmp_seed_doctors d ON d.rn = 1 + MOD(s.n * 5, @dcount);

INSERT INTO appointmenthistory (AppointmentID, Status, ChangedBy, ChangedDate, Notes)
SELECT a.AppointmentID,
       a.Status,
       (SELECT UserID FROM users WHERE RoleID=4 ORDER BY UserID LIMIT 1),
       CONCAT(a.AppointmentDate, ' ', a.AppointmentTime) - INTERVAL 1 HOUR,
       CONCAT('Status set to ', a.Status)
FROM appointments a
WHERE a.AppointmentCode LIKE CONCAT('APT', @run, '%');

DROP TEMPORARY TABLE IF EXISTS tmp_completed_appointments;
CREATE TEMPORARY TABLE tmp_completed_appointments AS
SELECT ROW_NUMBER() OVER (ORDER BY a.AppointmentDate, a.AppointmentTime, a.AppointmentID) AS rn,
       a.AppointmentID,
       a.AppointmentCode,
       a.PatientID,
       a.DoctorID,
       a.AppointmentDate,
       a.AppointmentTime,
       a.AppointmentType,
       a.Reason
FROM appointments a
WHERE a.AppointmentCode LIKE CONCAT('APT', @run, '%')
  AND a.Status = 'Completed';

INSERT INTO visits (VisitCode, PatientID, DoctorID, AppointmentID, VisitDate, Symptoms, Diagnosis, Treatment, FollowUpDate, VisitStatus, CreatedBy)
SELECT CONCAT('VIS', @run, LPAD(c.rn,4,'0')),
       c.PatientID,
       c.DoctorID,
       c.AppointmentID,
       CONCAT(c.AppointmentDate, ' ', c.AppointmentTime) + INTERVAL 45 MINUTE,
       c.Reason,
       ELT(1 + MOD(c.rn, 12),
           'Essential hypertension',
           'Acute upper respiratory tract infection',
           'Type 2 diabetes mellitus',
           'Normal pregnancy, second trimester',
           'Osteoarthritis, bilateral knees',
           'Atopic dermatitis',
           'Migraine without aura',
           'Acute gastroenteritis',
           'Bronchial asthma, mild persistent',
           'Urinary tract infection',
           'Lumbar muscle strain',
           'Generalized anxiety disorder'),
       ELT(1 + MOD(c.rn, 10),
           'Medication adjustment and lifestyle counseling',
           'Seven-day oral antibiotic regimen',
           'Nebulization and inhaler education',
           'Hydration therapy and symptomatic treatment',
           'Pain management and physical therapy referral',
           'Topical medication and trigger avoidance plan',
           'Observation and follow-up in two weeks',
           'Nutritional counseling and home monitoring',
           'Prenatal vitamins and routine screening',
           'Wound cleaning and dressing replacement'),
       CASE WHEN MOD(c.rn, 5) < 3 THEN DATE_ADD(c.AppointmentDate, INTERVAL 14 DAY) ELSE NULL END,
       'Completed',
       (SELECT UserID FROM users WHERE Username='admin' LIMIT 1)
FROM tmp_completed_appointments c;

DROP TEMPORARY TABLE IF EXISTS tmp_seed_visits;
CREATE TEMPORARY TABLE tmp_seed_visits AS
SELECT ROW_NUMBER() OVER (ORDER BY VisitID) AS rn,
       VisitID,
       VisitCode,
       PatientID,
       DoctorID,
       VisitDate
FROM visits
WHERE VisitCode LIKE CONCAT('VIS', @run, '%');

INSERT INTO prescriptions (PrescriptionCode, VisitID, PatientID, DoctorID, PrescriptionDate, Instructions, Status)
SELECT CONCAT('RX', @run, LPAD(v.rn,4,'0')),
       v.VisitID,
       v.PatientID,
       v.DoctorID,
       v.VisitDate + INTERVAL 20 MINUTE,
       'Take medications on time and return for follow-up as scheduled.',
       CASE WHEN MOD(v.rn, 10) < 7 THEN 'Active' ELSE 'Completed' END
FROM tmp_seed_visits v
WHERE MOD(v.rn, 5) <> 0;

INSERT INTO prescriptiondetails (PrescriptionID, MedicineName, Dosage, Frequency, Duration, Instructions)
SELECT p.PrescriptionID,
       ELT(1 + MOD(ROW_NUMBER() OVER (ORDER BY p.PrescriptionID), 12),
           'Losartan 50mg',
           'Amlodipine 5mg',
           'Metformin 500mg',
           'Atorvastatin 20mg',
           'Amoxicillin 500mg',
           'Azithromycin 500mg',
           'Ibuprofen 400mg',
           'Cetirizine 10mg',
           'Omeprazole 20mg',
           'Salbutamol inhaler',
           'Ferrous sulfate 325mg',
           'Paracetamol 500mg'),
       ELT(1 + MOD(p.PrescriptionID, 6), '1 tablet','1 capsule','5 mL syrup','2 tablets','1 puff','1 sachet'),
       ELT(1 + MOD(p.PrescriptionID, 5), 'Once daily','Twice daily','Every 8 hours','Every 12 hours','As needed'),
       CONCAT(3 + MOD(p.PrescriptionID, 12), ' days'),
       'After meals unless otherwise advised.'
FROM prescriptions p
WHERE p.PrescriptionCode LIKE CONCAT('RX', @run, '%');

DROP TEMPORARY TABLE IF EXISTS tmp_rooms_rn;
CREATE TEMPORARY TABLE tmp_rooms_rn AS
SELECT ROW_NUMBER() OVER (ORDER BY RoomID) AS rn, RoomID
FROM rooms;
SET @rcount := (SELECT COUNT(*) FROM tmp_rooms_rn);

INSERT INTO admissions (AdmissionNumber, PatientID, DoctorID, RoomID, AdmissionDate, ExpectedDischargeDate, ActualDischargeDate, AdmissionReason, Diagnosis, Status, DischargeSummary)
SELECT CONCAT('ADM', @run, LPAD(c.rn,4,'0')),
       c.PatientID,
       c.DoctorID,
       r.RoomID,
       CONCAT(c.AppointmentDate, ' ', c.AppointmentTime) + INTERVAL MOD(c.rn, 3) DAY + INTERVAL 2 HOUR,
       DATE(CONCAT(c.AppointmentDate, ' ', c.AppointmentTime) + INTERVAL MOD(c.rn, 3) DAY + INTERVAL (3 + MOD(c.rn, 7)) DAY),
       CASE WHEN c.rn <= 14 THEN CONCAT(c.AppointmentDate, ' ', c.AppointmentTime) + INTERVAL MOD(c.rn, 3) DAY + INTERVAL (3 + MOD(c.rn, 7)) DAY + INTERVAL 3 HOUR ELSE NULL END,
       'Inpatient monitoring required after initial emergency assessment.',
       ELT(1 + MOD(c.rn, 10),
           'Community-acquired pneumonia',
           'Uncontrolled hypertension',
           'Acute appendicitis',
           'Diabetic ketoacidosis',
           'Severe dehydration',
           'Acute asthma exacerbation',
           'Post-operative recovery',
           'Complicated urinary tract infection',
           'Suspected dengue fever',
           'Observation for chest pain'),
       CASE WHEN c.rn <= 14 THEN 'Discharged' ELSE 'Admitted' END,
       CASE WHEN c.rn <= 14 THEN 'Patient improved and discharged with home medication plan.' ELSE NULL END
FROM tmp_completed_appointments c
INNER JOIN tmp_rooms_rn r ON r.rn = 1 + MOD(c.rn, @rcount)
WHERE c.rn <= 20;

INSERT INTO bedallocations (AdmissionID, RoomID, BedNumber, AllocationDate, DischargeDate, Status)
SELECT a.AdmissionID,
       a.RoomID,
       CONCAT('B-', LPAD(1 + MOD(ROW_NUMBER() OVER (ORDER BY a.AdmissionID), 6),2,'0')),
       a.AdmissionDate,
       a.ActualDischargeDate,
       CASE WHEN a.Status='Discharged' THEN 'Discharged' ELSE 'Occupied' END
FROM admissions a
WHERE a.AdmissionNumber LIKE CONCAT('ADM', @run, '%');

DROP TEMPORARY TABLE IF EXISTS tmp_appt_non_cancel;
CREATE TEMPORARY TABLE tmp_appt_non_cancel AS
SELECT ROW_NUMBER() OVER (ORDER BY a.AppointmentDate, a.AppointmentTime, a.AppointmentID) AS rn,
       a.AppointmentID,
       a.PatientID,
       a.AppointmentCode,
       a.AppointmentDate,
       a.AppointmentTime,
       a.AppointmentType,
       a.Status
FROM appointments a
WHERE a.AppointmentCode LIKE CONCAT('APT', @run, '%')
  AND a.Status <> 'Cancelled';

INSERT INTO invoices (InvoiceNumber, PatientID, AppointmentID, InvoiceDate, DueDate, TotalAmount, Discount, TaxAmount, GrandTotal, Status, CreatedBy, Notes)
SELECT CONCAT('INV', @run, LPAD(t.rn,4,'0')),
       t.PatientID,
       t.AppointmentID,
       CONCAT(t.AppointmentDate, ' ', t.AppointmentTime) + INTERVAL MOD(t.rn,3) DAY,
       CONCAT(t.AppointmentDate, ' ', t.AppointmentTime) + INTERVAL (10 + MOD(t.rn,12)) DAY,
       @base_amount := LEAST(85000, GREATEST(1500,
            CASE t.AppointmentType
               WHEN 'Consultation' THEN 2200 + MOD(t.rn * 137, 2200)
               WHEN 'Follow-up' THEN 1500 + MOD(t.rn * 119, 1300)
               WHEN 'Check-up' THEN 2000 + MOD(t.rn * 157, 2400)
               WHEN 'Emergency' THEN 9500 + MOD(t.rn * 173, 21000)
               ELSE 2200
            END
            + CASE WHEN EXISTS (
                SELECT 1
                FROM admissions a
                WHERE a.PatientID = t.PatientID
                  AND ABS(DATEDIFF(a.AdmissionDate, t.AppointmentDate)) <= 7
            ) THEN 12000 + MOD(t.rn * 211, 30000) ELSE 0 END
       )),
       @discount_amount := CASE WHEN MOD(t.rn, 7) = 0 THEN 300 + MOD(t.rn * 41, 1700) ELSE 0 END,
       @tax_amount := ROUND((@base_amount - @discount_amount) * 0.12, 2),
       ROUND((@base_amount - @discount_amount) + @tax_amount, 2),
       CASE
         WHEN t.Status = 'Scheduled' THEN 'Pending'
         ELSE CASE WHEN MOD(t.rn,10) < 5 THEN 'Paid' WHEN MOD(t.rn,10) < 8 THEN 'Partial' ELSE 'Pending' END
       END,
       (SELECT UserID FROM users WHERE Username='admin' LIMIT 1),
       'Auto-generated billing record linked to consultation workflow.'
FROM tmp_appt_non_cancel t;

SET @svc_count := (SELECT COUNT(*) FROM services);

INSERT INTO invoicedetails (InvoiceID, ServiceID, Quantity, UnitPrice)
SELECT i.InvoiceID,
       s.ServiceID,
       1 + MOD(x.rn, 3),
       ROUND(i.TotalAmount / (1 + MOD(x.rn, 3)), 2)
FROM (
   SELECT ROW_NUMBER() OVER (ORDER BY InvoiceID) AS rn, InvoiceID
   FROM invoices
   WHERE InvoiceNumber LIKE CONCAT('INV', @run, '%')
) x
INNER JOIN invoices i ON i.InvoiceID = x.InvoiceID
INNER JOIN (
   SELECT ROW_NUMBER() OVER (ORDER BY ServiceID) AS rn, ServiceID
   FROM services
) s ON s.rn = 1 + MOD(x.rn, @svc_count);

INSERT INTO payments (PaymentNumber, InvoiceID, PaymentDate, PaymentMethod, Amount, ReferenceNumber, ReceivedBy, Notes)
SELECT CONCAT('PAY', @run, LPAD(ROW_NUMBER() OVER (ORDER BY i.InvoiceID),4,'0')),
       i.InvoiceID,
       i.InvoiceDate + INTERVAL (1 + MOD(i.InvoiceID, 5)) DAY,
       ELT(1 + MOD(i.InvoiceID, 5), 'Cash', 'Credit Card', 'Debit Card', 'Online', 'Insurance'),
       CASE
         WHEN i.Status = 'Paid' THEN i.GrandTotal
         ELSE ROUND(i.GrandTotal * (0.35 + (MOD(i.InvoiceID, 30) / 100)), 2)
       END,
       CONCAT('REF-', RIGHT(@run, 8), '-', LPAD(i.InvoiceID,4,'0')),
       (SELECT UserID FROM users WHERE Username = CONCAT('acct.', @run) LIMIT 1),
       'Payment encoded by billing desk.'
FROM invoices i
WHERE i.InvoiceNumber LIKE CONCAT('INV', @run, '%')
  AND i.Status IN ('Paid', 'Partial');

DROP TEMPORARY TABLE IF EXISTS tmp_lab_visits;
CREATE TEMPORARY TABLE tmp_lab_visits AS
SELECT ROW_NUMBER() OVER (ORDER BY v.VisitDate, v.VisitID) AS rn,
       v.VisitID,
       v.PatientID,
       v.DoctorID,
       v.VisitDate
FROM visits v
WHERE v.VisitCode LIKE CONCAT('VIS', @run, '%')
LIMIT 30;

INSERT INTO laborders (OrderCode, VisitID, PatientID, DoctorID, OrderDate, Status, ResultDate, Notes)
SELECT CONCAT('LAB', @run, LPAD(v.rn,4,'0')),
       v.VisitID,
       v.PatientID,
       v.DoctorID,
       v.VisitDate + INTERVAL (1 + MOD(v.rn, 12)) HOUR,
       CASE WHEN v.rn <= 16 THEN 'Completed' WHEN v.rn <= 24 THEN 'In Progress' ELSE 'Pending' END,
       CASE WHEN v.rn <= 16 THEN v.VisitDate + INTERVAL (12 + MOD(v.rn, 24)) HOUR ELSE NULL END,
       'Lab request generated from consultation workflow.'
FROM tmp_lab_visits v;

SET @test_count := (SELECT COUNT(*) FROM labtests);

INSERT INTO laborderdetails (OrderID, TestID, ResultValue, ResultUnit, NormalRange, IsNormal, Notes, TechnicianID, CompletedDate)
SELECT lo.OrderID,
       t.TestID,
       CASE WHEN lo.Status='Completed' THEN CAST(80 + MOD(lo.OrderID * 7, 70) AS CHAR) ELSE NULL END,
       'mg/dL',
       '70-140',
       CASE WHEN lo.Status='Completed' THEN CASE WHEN MOD(lo.OrderID, 4) = 0 THEN 0 ELSE 1 END ELSE NULL END,
       'Validated by laboratory section.',
       (SELECT UserID FROM users WHERE Username = CONCAT('labtech.', @run) LIMIT 1),
       lo.ResultDate
FROM laborders lo
INNER JOIN (
   SELECT ROW_NUMBER() OVER (ORDER BY TestID) AS rn, TestID
   FROM labtests
) t ON t.rn = 1 + MOD(lo.OrderID, @test_count)
WHERE lo.OrderCode LIKE CONCAT('LAB', @run, '%');

INSERT INTO pharmacysales (SaleNumber, PatientID, SaleDate, TotalAmount, Discount, NetAmount, PaymentStatus, SoldBy)
WITH RECURSIVE seq AS (
    SELECT 1 AS n
    UNION ALL
    SELECT n + 1 FROM seq WHERE n < 25
)
SELECT CONCAT('SAL', @run, LPAD(s.n,4,'0')),
       p.PatientID,
       DATE_ADD('2024-01-10 08:00:00', INTERVAL MOD(s.n * 27, 900) DAY) + INTERVAL MOD(s.n * 19, 700) MINUTE,
       0,
       0,
       0,
       CASE WHEN MOD(s.n, 10) < 5 THEN 'Paid' WHEN MOD(s.n, 10) < 8 THEN 'Partial' ELSE 'Pending' END,
       (SELECT UserID FROM users WHERE Username = CONCAT('pharma.', @run) LIMIT 1)
FROM seq s
INNER JOIN tmp_seed_patients p ON p.rn = 1 + MOD(s.n * 7, @pcount);

SET @med_count := (SELECT COUNT(*) FROM medicines);

INSERT INTO pharmacysaledetails (SaleID, MedicineID, BatchNumber, Quantity, UnitPrice)
SELECT ps.SaleID,
       m.MedicineID,
       CONCAT('BT', RIGHT(@run, 6), '-', LPAD(ROW_NUMBER() OVER (ORDER BY ps.SaleID),4,'0'), '-A'),
       5 + MOD(ps.SaleID * 3, 20),
       45 + MOD(ps.SaleID * 7, 80)
FROM pharmacysales ps
INNER JOIN (
   SELECT ROW_NUMBER() OVER (ORDER BY MedicineID) AS rn, MedicineID
   FROM medicines
) m ON m.rn = 1 + MOD(ps.SaleID, @med_count)
WHERE ps.SaleNumber LIKE CONCAT('SAL', @run, '%');

INSERT INTO pharmacysaledetails (SaleID, MedicineID, BatchNumber, Quantity, UnitPrice)
SELECT ps.SaleID,
       m.MedicineID,
       CONCAT('BT', RIGHT(@run, 6), '-', LPAD(ROW_NUMBER() OVER (ORDER BY ps.SaleID),4,'0'), '-B'),
       4 + MOD(ps.SaleID * 5, 18),
       30 + MOD(ps.SaleID * 11, 90)
FROM pharmacysales ps
INNER JOIN (
   SELECT ROW_NUMBER() OVER (ORDER BY MedicineID) AS rn, MedicineID
   FROM medicines
) m ON m.rn = 1 + MOD(ps.SaleID + 3, @med_count)
WHERE ps.SaleNumber LIKE CONCAT('SAL', @run, '%');

UPDATE pharmacysales ps
INNER JOIN (
   SELECT SaleID, ROUND(SUM(Quantity * UnitPrice), 2) AS Gross
   FROM pharmacysaledetails
   GROUP BY SaleID
) agg ON agg.SaleID = ps.SaleID
SET ps.TotalAmount = agg.Gross,
    ps.Discount = CASE WHEN agg.Gross > 2500 THEN 100 + MOD(ps.SaleID * 19, 300) ELSE 30 + MOD(ps.SaleID * 7, 60) END,
    ps.NetAmount = ROUND(agg.Gross - (CASE WHEN agg.Gross > 2500 THEN 100 + MOD(ps.SaleID * 19, 300) ELSE 30 + MOD(ps.SaleID * 7, 60) END), 2)
WHERE ps.SaleNumber LIKE CONCAT('SAL', @run, '%');

DROP TEMPORARY TABLE IF EXISTS tmp_users_rn;
CREATE TEMPORARY TABLE tmp_users_rn AS
SELECT ROW_NUMBER() OVER (ORDER BY UserID) AS rn,
       UserID
FROM users;
SET @ucount := (SELECT COUNT(*) FROM tmp_users_rn);

INSERT INTO notifications (UserID, Title, Message, NotificationType, IsRead, CreatedDate, ExpiryDate)
WITH RECURSIVE seq AS (
    SELECT 1 AS n
    UNION ALL
    SELECT n + 1 FROM seq WHERE n < 20
)
SELECT u.UserID,
       ELT(1 + MOD(s.n, 5), 'Schedule Alert', 'Payment Reminder', 'Lab Result Update', 'Inventory Notice', 'Admission Update'),
       ELT(1 + MOD(s.n, 5),
           'Please review pending tasks assigned for today.',
           'One or more patient balances are due this week.',
           'New laboratory results are ready for validation.',
           'Selected medicine batches are below reorder level.',
           'Admission dashboard has updated room utilization.'),
       'System',
       MOD(s.n, 2),
       DATE_ADD('2025-01-15 08:00:00', INTERVAL MOD(s.n * 23, 420) DAY) + INTERVAL MOD(s.n * 17, 700) MINUTE,
       DATE_ADD('2025-01-15 08:00:00', INTERVAL MOD(s.n * 23, 420) + 45 DAY)
FROM seq s
INNER JOIN tmp_users_rn u ON u.rn = 1 + MOD(s.n, @ucount);

INSERT INTO auditlogs (UserID, Action, TableName, RecordID, OldValue, NewValue, IPAddress, MachineName, LogDate)
WITH RECURSIVE seq AS (
    SELECT 1 AS n
    UNION ALL
    SELECT n + 1 FROM seq WHERE n < 30
)
SELECT u.UserID,
       ELT(1 + MOD(s.n, 4), 'CREATE', 'UPDATE', 'VIEW', 'EXPORT'),
       ELT(1 + MOD(s.n, 6), 'Patients', 'Appointments', 'Invoices', 'LabOrders', 'Admissions', 'PharmacySales'),
       1 + MOD(s.n * 13, 120),
       NULL,
       NULL,
       CONCAT('192.168.10.', 10 + MOD(s.n, 40)),
       CONCAT('NURSE-STATION-', 1 + MOD(s.n, 6)),
       DATE_ADD('2024-06-01 07:00:00', INTERVAL MOD(s.n * 19, 640) DAY) + INTERVAL MOD(s.n * 11, 800) MINUTE
FROM seq s
INNER JOIN tmp_users_rn u ON u.rn = 1 + MOD(s.n * 3, @ucount);

COMMIT;
