using System;
using System.Windows.Forms;

namespace HospitalManagementSystem.UserControls
{
    public partial class ucNavigation : UserControl
    {
        public event EventHandler DashboardClicked;
        public event EventHandler PatientsClicked;
        public event EventHandler DoctorsClicked;
        public event EventHandler AppointmentsClicked;
        public event EventHandler BillingClicked;
        public event EventHandler UsersClicked;
        public event EventHandler ReportsClicked;

        public ucNavigation()
        {
            InitializeComponent();
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            DashboardClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnPatients_Click(object sender, EventArgs e)
        {
            PatientsClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnDoctors_Click(object sender, EventArgs e)
        {
            DoctorsClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnAppointments_Click(object sender, EventArgs e)
        {
            AppointmentsClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnBilling_Click(object sender, EventArgs e)
        {
            BillingClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            UsersClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            ReportsClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
