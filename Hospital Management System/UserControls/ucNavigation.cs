using System;
using System.Drawing;
using System.Windows.Forms;
using HospitalManagementSystem.Helpers;

namespace HospitalManagementSystem.UserControls
{
    public partial class ucNavigation : UserControl
    {
        private Button _activeButton;

        public event EventHandler DashboardClicked;
        public event EventHandler PatientsClicked;
        public event EventHandler DoctorsClicked;
        public event EventHandler AppointmentsClicked;
        public event EventHandler RoomsClicked;
        public event EventHandler BillingClicked;
        public event EventHandler UsersClicked;
        public event EventHandler ReportsClicked;
        public event EventHandler LogoutClicked;

        public ucNavigation()
        {
            InitializeComponent();
            ApplyTheme();
            SetActiveButton(btnDashboard);
        }

        public void ConfigureForRole(string roleName)
        {
            var normalized = (roleName ?? string.Empty).Trim().ToLowerInvariant();

            btnDashboard.Visible = true;
            btnPatients.Visible = true;
            btnDoctors.Visible = normalized == "administrator" || normalized == "receptionist";
            btnAppointments.Visible = true;
            btnRooms.Visible = normalized == "administrator" || normalized == "receptionist" || normalized == "nurse";
            btnBilling.Visible = normalized == "administrator" || normalized == "receptionist";
            btnUsers.Visible = normalized == "administrator";
            btnReports.Visible = normalized == "administrator" || normalized == "doctor";
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnDashboard);
            DashboardClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnPatients_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnPatients);
            PatientsClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnDoctors_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnDoctors);
            DoctorsClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnAppointments_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnAppointments);
            AppointmentsClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnRooms_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnRooms);
            RoomsClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnBilling_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnBilling);
            BillingClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnUsers);
            UsersClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnReports);
            ReportsClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnLogout);
            LogoutClicked?.Invoke(this, EventArgs.Empty);
        }

        private void ApplyTheme()
        {
            BackColor = ThemeManager.Colors.Sidebar;
            flpMenu.BackColor = ThemeManager.Colors.Sidebar;
            lblAppName.Text = "Hospital Console";

            btnDashboard.Image = SystemIcons.Application.ToBitmap();
            btnPatients.Image = SystemIcons.Information.ToBitmap();
            btnDoctors.Image = SystemIcons.Shield.ToBitmap();
            btnAppointments.Image = SystemIcons.Asterisk.ToBitmap();
            btnRooms.Image = SystemIcons.Question.ToBitmap();
            btnBilling.Image = SystemIcons.WinLogo.ToBitmap();
            btnUsers.Image = SystemIcons.Warning.ToBitmap();
            btnReports.Image = SystemIcons.Exclamation.ToBitmap();
            btnLogout.Image = SystemIcons.Error.ToBitmap();

            ThemeManager.StyleSidebar(
                pnlHeader,
                lblAppName,
                flpMenu,
                btnDashboard,
                btnPatients,
                btnDoctors,
                btnAppointments,
                btnRooms,
                btnBilling,
                btnUsers,
                btnReports,
                btnLogout);
        }

        private void SetActiveButton(Button button)
        {
            if (button == null)
            {
                return;
            }

            var oldButton = _activeButton;
            if (oldButton != null && oldButton != button)
            {
                var oldIsDanger = oldButton == btnLogout;
                ThemeManager.StyleSidebarButton(oldButton, oldIsDanger, isActive: false);
            }

            var isDanger = button == btnLogout;
            ThemeManager.StyleSidebarButton(button, isDanger, isActive: !isDanger);
            _activeButton = button;
        }
    }
}
