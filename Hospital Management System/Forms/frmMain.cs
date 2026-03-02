using System;
using System.Windows.Forms;
using HospitalManagementSystem.BLL.Services;
using HospitalManagementSystem.Helpers;
using HospitalManagementSystem.UserControls;

namespace HospitalManagementSystem.Forms
{
    public partial class frmMain : Form
    {
        private readonly AuthenticatedUser _currentUser;
        public bool LogoutRequested { get; private set; }

        public frmMain(AuthenticatedUser currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            ApplyTheme();
            HookNavigation();
        }

        private void HookNavigation()
        {
            ucNavigation1.DashboardClicked += (_, __) => ShowDashboard();
            ucNavigation1.PatientsClicked += (_, __) => ShowPatients();
            ucNavigation1.DoctorsClicked += (_, __) => ShowDoctors();
            ucNavigation1.AppointmentsClicked += (_, __) => ShowAppointments();
            ucNavigation1.RoomsClicked += (_, __) => ShowRooms();
            ucNavigation1.BillingClicked += (_, __) => ShowBilling();
            ucNavigation1.SettingsClicked += (_, __) => ShowSettings();
            ucNavigation1.UsersClicked += (_, __) => ShowUsers();
            ucNavigation1.ReportsClicked += (_, __) => ShowReports();
            ucNavigation1.ProfileClicked += (_, __) => ShowProfile();
            ucNavigation1.LogoutClicked += (_, __) => Logout();
            ucHeader1.LogoutClicked += (_, __) => Logout();
            ucHeader1.QuickAddPatientClicked += (_, __) => ShowPatients();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            ucHeader1.SetUser(_currentUser.Username, _currentUser.RoleName);
            ucNavigation1.ConfigureForRole(_currentUser.RoleName);
            lblStatus.Text = $"Signed in as {_currentUser.Username} ({_currentUser.RoleName})";
            ShowDashboard();
        }

        private void LoadModule(UserControl control, string title)
        {
            pnlContent.Controls.Clear();
            ThemeManager.ApplyControlTheme(control);
            control.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(control);
            ucHeader1.SetDashboardMode(control is ucDashboard || control is ucRoleDashboard);
            ucHeader1.SetTitle(title);
        }

        private void ShowDashboard()
        {
            if (CurrentUserHasRole("Administrator"))
            {
                LoadModule(new ucDashboard(), "Admin Dashboard");
                return;
            }

            var dashboard = new ucRoleDashboard();
            dashboard.Configure(_currentUser.RoleName, _currentUser.Username);
            LoadModule(dashboard, $"{_currentUser.RoleName} Dashboard");
        }

        private void ShowPatients()
        {
            if (!EnsureAccess("Patients", "Administrator", "Doctor", "Nurse", "Receptionist", "Pharmacist", "Lab Technician"))
            {
                return;
            }

            LoadModule(new ucPatients(), "Patients");
        }

        private void ShowDoctors()
        {
            if (!EnsureAccess("Doctors", "Administrator", "Doctor", "Receptionist", "HR Manager"))
            {
                return;
            }

            LoadModule(new ucDoctors(), "Doctors");
        }

        private void ShowAppointments()
        {
            if (!EnsureAccess("Appointments", "Administrator", "Doctor", "Nurse", "Receptionist", "Lab Technician"))
            {
                return;
            }

            LoadModule(new ucAppointments(), "Appointments");
        }

        private void ShowBilling()
        {
            if (!EnsureAccess("Billing", "Administrator", "Receptionist", "Accountant", "Pharmacist"))
            {
                return;
            }

            LoadModule(new ucBilling(), "Billing");
        }

        private void ShowRooms()
        {
            if (!EnsureAccess("Rooms & Occupancy", "Administrator", "Nurse", "Receptionist"))
            {
                return;
            }

            LoadModule(new ucRooms(), "Rooms & Occupancy");
        }

        private void ShowReports()
        {
            if (!EnsureAccess("Reports", "Administrator", "Doctor", "Accountant", "HR Manager", "Lab Technician", "Pharmacist"))
            {
                return;
            }

            LoadModule(new ucReports(), "Reports");
        }

        private void ShowUsers()
        {
            if (!EnsureAccess("User Management", "Administrator"))
            {
                return;
            }

            LoadModule(new ucUsers(), "Users");
        }

        private void ShowSettings()
        {
            if (!EnsureAccess("Settings", "Administrator"))
            {
                return;
            }

            LoadModule(new ucSettings(_currentUser), "Settings");
        }

        private void ShowProfile()
        {
            LoadModule(new ucProfile(_currentUser), "My Profile");
        }

        private void Logout()
        {
            var confirm = MessageBox.Show("Log out of the system?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            LogoutRequested = true;
            UserSession.End();
            Close();
        }

        private bool EnsureAccess(string moduleName, params string[] allowedRoles)
        {
            if (CurrentUserHasRole(allowedRoles))
            {
                return true;
            }

            MessageBox.Show(
                $"{_currentUser.RoleName} role cannot access {moduleName}.",
                "Access Denied",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return false;
        }

        private bool CurrentUserHasRole(params string[] allowedRoles)
        {
            var currentRole = NormalizeRole(_currentUser?.RoleName);
            if (string.IsNullOrWhiteSpace(currentRole) || allowedRoles == null || allowedRoles.Length == 0)
            {
                return false;
            }

            foreach (var allowedRole in allowedRoles)
            {
                if (string.Equals(currentRole, NormalizeRole(allowedRole), StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static string NormalizeRole(string roleName)
        {
            var token = (roleName ?? string.Empty)
                .Trim()
                .ToLowerInvariant()
                .Replace(" ", string.Empty);

            switch (token)
            {
                case "admin":
                case "systemadministrator":
                case "systemadmin":
                    return "administrator";
                case "frontdesk":
                case "frontdeskstaff":
                    return "receptionist";
                case "labtechnician":
                    return "lab technician";
                case "hrmanager":
                    return "hr manager";
                default:
                    return token;
            }
        }

        private void ApplyTheme()
        {
            ThemeManager.ApplyFormTheme(this, styleChildren: false);
            var companyName = AppSettingsStore.Load().CompanyName;
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                Text = companyName.Trim();
            }

            menuStrip1.Visible = false;
            statusStrip1.Visible = false;
            pnlLeft.BackColor = ThemeManager.Colors.Sidebar;
            pnlTop.BackColor = ThemeManager.Colors.Surface;
            pnlContent.BackColor = ThemeManager.Colors.Background;
            pnlContent.Padding = new Padding(12);
            if (statusStrip1.Visible)
            {
                ThemeManager.StyleStatusStrip(statusStrip1);
                lblStatus.ForeColor = ThemeManager.Colors.TextSecondary;
            }
        }
    }
}
