# Hospital Management System

Hospital Management System is a role-based Windows Forms desktop application built to support day-to-day hospital operations. It centralizes patient information, doctor records, appointments, room admissions, billing, reports, user administration, and system configuration in a single MySQL-backed application.

This repository contains the main source code, SQL schema and seed files, installer assets, and deployment helpers for the system.

## Overview

The goal of this project is to help hospital staff work from one connected platform instead of using separate spreadsheets, paper records, or disconnected tools. The application is designed for multiple staff roles, and each role is shown only the modules that match its responsibilities.

At a high level, the system supports:

- patient registration and profile maintenance
- doctor records, schedules, and profile images
- appointment scheduling and status tracking
- room occupancy monitoring
- patient admissions and discharge handling
- invoice review and payment processing
- report generation with export to Excel, CSV, and PDF
- user management, audit logs, backup, restore, and administrative settings

## User Roles And Access

The application uses role-based access control. Navigation, dashboards, and module access depend on the signed-in user's role.

### Administrator

The Administrator is the highest operational role in the system.

- full access to all modules
- manage patients, doctors, appointments, rooms, billing, reports, users, and settings
- open audit logs
- configure database, backup, branding, and system settings

### Doctor

The Doctor role is focused on patient care and appointment-related work.

- access patients
- access doctors
- access appointments
- access reports
- open a doctor-specific dashboard
- manage own profile

### Nurse

The Nurse role supports patient care, room monitoring, and admission workflows.

- access patients
- access appointments
- access rooms and admissions
- open a nurse-specific dashboard
- manage own profile

### Receptionist

The Receptionist role is the front-desk and intake role in the system.

- register and manage patients
- access doctors
- manage appointments
- manage room assignments and admissions
- access billing
- open a receptionist-specific dashboard
- manage own profile

### Pharmacist

The Pharmacist role is connected to billing, medicine-related records, and pharmacy reporting.

- access patient-related billing workflows
- access billing
- access reports
- open a pharmacist-specific dashboard
- manage own profile

### Lab Technician

The Lab Technician role supports appointment-related coordination and laboratory reporting workflows.

- access patients
- access appointments
- access reports
- open a lab technician-specific dashboard
- manage own profile

### Accountant

The Accountant role handles financial review and payment processing.

- access billing
- access reports
- open an accountant-specific dashboard
- manage own profile

### HR Manager

The HR Manager role focuses on staff-related oversight and reporting.

- access doctor records
- access reports
- open an HR-specific dashboard
- manage own profile

## Main Modules

Based on the current codebase, the main end-user modules available in the application are:

### Dashboard

- administrator dashboard
- role-specific dashboards for staff users
- live operational metrics pulled from the database

### Patients

- patient registration
- demographic details
- contact information
- profile image support
- search and filtering

### Doctors

- doctor profile management
- specialization and consultation fee management
- doctor image support
- doctor schedule-related workflows

### Appointments

- appointment creation and editing
- patient and doctor scheduling
- appointment type and status tracking
- search and filtering

### Rooms And Admissions

- room list and occupancy monitoring
- bed availability tracking
- patient admission
- discharge processing
- admission reason and diagnosis support

### Billing

- invoice listing
- invoice review
- payment processing
- patient billing lookup

### Reports

- load operational and financial reports from the database
- export to Excel
- export to CSV
- export to PDF
- quick access to user, audit, and backup dialogs

### Users

- user account administration
- role assignment
- active/inactive status management
- profile image support

### Settings

- company branding details
- backup path and SQL dump path
- SMTP configuration
- theme settings
- database profile setup
- audit and user management access for administrators

### Profile

- personal user details
- profile image updates
- self-service profile maintenance

## Available Reports

The reporting module currently includes:

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

Reports can be exported in:

- Excel
- CSV
- PDF

## Technology Stack

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
tools/                        Utility tools such as seed helpers
Assets/                       Branding and image assets
```

Inside the main project, the code is primarily organized into:

- `BLL/` for business logic services
- `DAL/` for repositories, DTOs, and database access
- `Models/` for domain entities
- `Forms/` for login, main shell, installer, and shared dialogs
- `UserControls/` for the feature modules and dashboards
- `Helpers/` for authorization, theming, settings, backup, hashing, and export utilities

The solution currently contains one main application project:

- `Hospital Management System.sln`
- `Hospital Management System/Hospital Management System.csproj`

## Security And Authentication

The project currently uses:

- role-based authorization checks
- session-aware navigation
- PBKDF2 password hashing for the current password format
- compatibility handling for legacy password formats during login
- automatic migration of legacy passwords to the current hash format after successful authentication

## Database And Installation

The application supports multiple database connection modes:

- `Local`
- `Network`
- `Online`

The first-run installer and runtime database connection screens can help:

- test MySQL connectivity
- save a working database profile
- apply a runtime connection string
- create the target database if needed
- install the schema from the SQL dump
- ensure required user roles exist
- create or repair the bootstrap administrator account

## Getting Started

### Prerequisites

- Windows
- Visual Studio 2022 or another version that supports .NET Framework projects
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
8. On first launch, use the installer or the database connection dialog to configure MySQL access.
9. Choose the database mode that matches your environment: `Local`, `Network`, or `Online`.

## Sample Accounts

The repository includes sample credentials in `users.txt`, and the seed scripts also use `admin123` as the default password for several demo accounts.

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

## Current Scope

From the current project scan, this repository is focused on the desktop hospital management application itself. It does not currently include a separate automated test project in the solution.

## Notes Before Publishing Publicly

Before making this repository public on GitHub, review the project carefully for environment-specific configuration.

Important areas to check:

- database host, username, and password defaults
- installer defaults
- application configuration files
- stored connection profile values

Recommended cleanup steps:

1. remove hard-coded production or personal database credentials
2. replace them with safe placeholders or environment-specific setup instructions
3. keep only sample credentials that are clearly marked as demo/test data
4. add a `LICENSE` file if you plan to open-source the project

## Contributing

Contributions are welcome.

Suggested workflow:

1. fork the repository
2. create a feature branch
3. make your changes
4. test the affected module manually
5. update SQL scripts if the schema changes
6. open a pull request with a short summary of the change
