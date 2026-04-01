# Hospital Management System

Hospital Management System is a Windows Forms desktop application for managing hospital operations such as patient records, doctor management, appointments, room admissions, billing, user accounts, reports, and administrative settings. It is built for a multi-user hospital environment where each staff role sees only the modules relevant to their work.

## What This System Is

This system is designed to help a hospital or clinic manage daily operations in one desktop platform connected to a MySQL database.

It supports:

- patient registration and profile management
- doctor records and schedules
- appointment booking and status tracking
- room and bed management
- admissions and discharge workflows
- billing, invoices, and payment processing
- audit logs, backup and restore, and exportable reports
- role-based dashboards for different staff users

## User Roles

The application is role-based. Access changes depending on the signed-in user.

### Administrator

- full access to all modules
- manage patients, doctors, appointments, rooms, billing, reports, users, and settings
- open audit logs and database tools
- manage backup and restore settings

### Doctor

- open doctor dashboard
- view patients and doctor records
- access appointments
- open medical and operational reports
- manage own profile

### Nurse

- open nurse dashboard
- access patients
- manage appointments
- work with rooms, admissions, and discharge workflows
- manage own profile

### Receptionist

- open receptionist dashboard
- register and manage patients
- manage doctors and appointments
- handle room assignments and admissions
- process billing-related front-desk work
- manage own profile

### Pharmacist

- open pharmacist dashboard
- access patient-related billing context
- work with billing screens and pharmacy-related reports
- manage own profile

### Lab Technician

- open lab technician dashboard
- access patients and appointments
- use reports related to laboratory activity and billing
- manage own profile

### Accountant

- open accountant dashboard
- manage invoices and payment processing
- access financial and patient payment reports
- manage own profile

### HR Manager

- open HR dashboard
- access doctor records
- review reports relevant to staffing and performance
- manage own profile

## Main Features

- role-based authentication and navigation
- first-run installer for database setup
- local, network, and online database connection profiles
- patient records with profile image support
- doctor records, schedules, and profile images
- appointment scheduling and tracking
- room occupancy, admissions, and discharge summary handling
- invoice and payment processing
- audit logs
- backup and restore tools
- report export to Excel, CSV, and PDF
- configurable branding, SMTP, and hospital settings

## Available Reports

The reporting module includes:

- Patients
- Appointments
- Billing
- Pharmacy
- Doctor Performance
- Room Occupancy
- Doctor Schedules
- Payments Per Patient
- Patient Medicines
- Laboratory Billing
- Statistical Summary

## Tech Stack

- C#
- .NET Framework 4.7.2
- Windows Forms
- MySQL
- Dapper
- MySql.Data
- ClosedXML
- iTextSharp
- Newtonsoft.Json

## Solution Structure

```text
Hospital Management System/   Main WinForms application source
database/                     SQL seed and upgrade scripts
Dumps/                        Main SQL dump/schema
Installer/                    Installer and packaged builds
tools/                        Utility tooling such as seed helpers
Assets/                       Branding and image assets
```

Inside the main project, the code is organized into:

- `BLL/` for business logic services
- `DAL/` for repositories and database access
- `Models/` for entities and DTOs
- `Forms/` for main forms and shared dialogs
- `UserControls/` for feature modules and dashboards
- `Helpers/` for app settings, authorization, export, backup, hashing, and theming

## Getting Started

### Prerequisites

- Windows
- Visual Studio 2022 or compatible version for .NET Framework projects
- .NET Framework 4.7.2 Developer Pack
- MySQL Server
- NuGet package restore enabled

### Setup

1. Clone the repository.
2. Open `Hospital Management System.sln`.
3. Restore NuGet packages.
4. Create or prepare a MySQL database.
5. Import the main schema from `Dumps/hospitalmanagementsystem.sql`.
6. Optional: import `database/seed_all_tables.sql` for sample data.
7. Run the `Hospital Management System` project.
8. On first run, use the installer or database connection dialog to configure the MySQL connection.

## Sample Accounts

The repository includes sample credentials in `users.txt`, and seed scripts also use `admin123` as the default password for several demo users.

Common sample accounts:

| Username | Password | Typical Role |
| --- | --- | --- |
| `admin` | `admin123` | Administrator / bootstrap account |
| `dr.smith` | `admin123` | Doctor |
| `dr.jones` | `admin123` | Doctor |
| `nurse.mary` | `admin123` | Nurse |
| `reception.john` | `admin123` | Receptionist |

## Build And Run

Build from Visual Studio, or use MSBuild from a Developer Command Prompt:

```powershell
msbuild "Hospital Management System.sln" /t:Build /p:Configuration=Debug
```

## Notes For GitHub Publishing

- Review and remove environment-specific database credentials before publishing publicly.
- Check `DatabaseDefaults.cs`, installer defaults, and configuration files for hard-coded connection values.
- If you change schema or seed data, update the SQL files in `database/` or `Dumps/`.
- Consider adding a `LICENSE` file before open-source publication.

## Contributing

Contributions are welcome.

Suggested workflow:

1. Fork the repository.
2. Create a feature branch.
3. Make your changes.
4. Test the affected module manually.
5. Update SQL scripts if the database schema changes.
6. Open a pull request with a short summary of the change.
