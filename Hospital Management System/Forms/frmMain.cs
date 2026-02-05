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

        public frmMain(AuthenticatedUser currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            HookNavigation();
        }

        private void HookNavigation()
        {
            ucNavigation1.DashboardClicked += (_, __) => ShowDashboard();
            ucNavigation1.PatientsClicked += (_, __) => ShowPatients();
            ucNavigation1.DoctorsClicked += (_, __) => ShowDoctors();
            ucNavigation1.AppointmentsClicked += (_, __) => ShowAppointments();
            ucNavigation1.BillingClicked += (_, __) => ShowBilling();
            ucNavigation1.UsersClicked += (_, __) => ShowUsers();
            ucNavigation1.ReportsClicked += (_, __) => ShowReports();
            ucNavigation1.LogoutClicked += (_, __) => Logout();
            ucHeader1.LogoutClicked += (_, __) => Logout();
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
            control.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(control);
            ucHeader1.SetTitle(title);
        }

        private void ShowDashboard()
        {
            if (string.Equals(_currentUser.RoleName, "Administrator", StringComparison.OrdinalIgnoreCase))
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
            LoadModule(new ucPatients(), "Patients");
        }

        private void ShowDoctors()
        {
            LoadModule(new ucDoctors(), "Doctors");
        }

        private void ShowAppointments()
        {
            LoadModule(new ucAppointments(), "Appointments");
        }

        private void ShowBilling()
        {
            LoadModule(new ucBilling(), "Billing");
        }

        private void ShowReports()
        {
            LoadModule(new ucReports(), "Reports");
        }

        private void ShowUsers()
        {
            if (!string.Equals(_currentUser.RoleName, "Administrator", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Only administrators can open User Management.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            LoadModule(new ucUsers(), "Users");
        }

        private void Logout()
        {
            var confirm = MessageBox.Show("Log out of the system?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            UserSession.End();
            Hide();
            using (var login = new frmLogin())
            {
                login.ShowDialog();
            }

            Close();
        }
    }
}
