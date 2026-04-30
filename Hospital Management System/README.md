# Hospital Management System Project

This is the main Windows Forms application project for the Hospital Management System repository. It contains the login flow, dashboards, module user controls, business logic, data access layer, installer integration, and database-backed hospital workflows.

## What This Project Handles

The `Hospital Management System` project is the main desktop client used by hospital staff. It connects to MySQL and changes its navigation and dashboards based on the role of the signed-in user.

Core workflows handled in this project:

- user authentication and role-based access
- patient registration and maintenance
- doctor records and schedules
- appointment management
- room occupancy and admissions
- discharge workflow
- invoice review and payment processing
- report generation and export
- user management, audit logs, and settings

## Main Forms

- `Forms/frmLogin.cs` for sign-in and database connection fallback
- `Forms/frmMain.cs` for the main shell and module navigation
- `Forms/InstallerForm.cs` for first-run database setup
- `Forms/Shared/` for edit dialogs, audit log, backup/restore, and payment dialogs

## Main Modules

- `UserControls/ucDashboard.cs` for the administrator dashboard
- `UserControls/ucRoleDashboard.cs` for role-specific dashboards
- `UserControls/ucPatients.cs` for patient records
- `UserControls/ucDoctors.cs` for doctor management
- `UserControls/ucAppointments.cs` for appointments
- `UserControls/ucRooms.cs` for rooms, admissions, and discharge
- `UserControls/ucBilling.cs` for invoices and payment flow
- `UserControls/ucReports.cs` for report loading and export
- `UserControls/ucUsers.cs` for user administration
- `UserControls/ucSettings.cs` for system, database, users, and audit settings
- `UserControls/ucProfile.cs` for the signed-in user profile

## Role Access In This Project

### Administrator

- full access to every module
- only role with direct access to `Users` and `Settings`

### Doctor

- access patients, doctors, appointments, reports, and profile
- gets a doctor-specific dashboard with appointment-focused activity

### Nurse

- access patients, appointments, rooms/admissions, and profile
- gets a nurse-focused dashboard with admission and room workflow metrics

### Receptionist

- access patients, doctors, appointments, rooms/admissions, billing, and profile
- acts as the front-desk operations role

### Pharmacist

- access patients, billing, reports, and profile

### Lab Technician

- access patients, appointments, reports, and profile

### Accountant

- access billing, reports, and profile

### HR Manager

- access doctors, reports, and profile

## Architecture

```text
BLL/      Business logic services
DAL/      Repositories and database access
Models/   Core entities and DTOs
Forms/    Application forms and dialogs
Helpers/  Authorization, settings, theme, hashing, export, backup
UserControls/ Feature modules and dashboards
Assets/   Branding assets copied to output
```

## Security And Login Notes

- password hashing uses `PBKDF2`
- login still supports legacy stored formats for compatibility
- successful legacy logins are migrated to the current hash format

## Database Notes

This project supports multiple database profile modes:

- `Local`
- `Network`
- `Online`

The first-run installer can:

- create the target database
- install the schema from the SQL dump
- ensure required roles exist
- create or repair the bootstrap admin/superadmin account

## Build Requirements

- .NET Framework 4.7.2
- Windows
- Visual Studio with NuGet restore
- MySQL

## Run

```powershell
msbuild "Hospital Management System.csproj" /t:Build /p:Configuration=Debug
```

Then run the project from Visual Studio or start the built executable.

## Contributor Notes

- if you add a new module, update role checks in `frmMain.cs` and `ucNavigation.cs`
- if you change permissions, also review `AuthorizationHelper.cs`
- if you change schema or seed data, update the SQL files in the repository
- review connection defaults before publishing builds publicly
