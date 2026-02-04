using System;
using System.Windows.Forms;
using HospitalManagementSystem.UserControls;

namespace HospitalManagementSystem.Forms
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            HookNavigation();
        }

        private void HookNavigation()
        {
            ucNavigation1.DashboardClicked += (_, __) => ShowDashboard();
            ucNavigation1.PatientsClicked += (_, __) => ShowPatients();
            ucNavigation1.DoctorsClicked += (_, __) => ShowDoctors();
            ucNavigation1.AppointmentsClicked += (_, __) => ShowAppointments();
            ucNavigation1.BillingClicked += (_, __) => ShowBilling();
            ucNavigation1.ReportsClicked += (_, __) => ShowReports();
            ucHeader1.LogoutClicked += (_, __) => Logout();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
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
            LoadModule(new ucDashboard(), "Dashboard");
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

        private void Logout()
        {
            var confirm = MessageBox.Show("Log out of the system?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            Hide();
            using (var login = new frmLogin())
            {
                login.ShowDialog();
            }

            Close();
        }
    }
}
