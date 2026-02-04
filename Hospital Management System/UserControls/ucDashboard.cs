using System;
using System.Windows.Forms;
using HospitalManagementSystem.BLL.Services;

namespace HospitalManagementSystem.UserControls
{
    public partial class ucDashboard : UserControl
    {
        private readonly DashboardService _service = new DashboardService();

        public ucDashboard()
        {
            InitializeComponent();
            Load += ucDashboard_Load;
        }

        private async void ucDashboard_Load(object sender, EventArgs e)
        {
            try
            {
                UseWaitCursor = true;
                var patients = await _service.GetTotalPatientsAsync().ConfigureAwait(true);
                var doctors = await _service.GetTotalDoctorsAsync().ConfigureAwait(true);
                var revenue = await _service.GetTotalRevenueAsync().ConfigureAwait(true);

                lblPatientsValue.Text = patients.ToString();
                lblDoctorsValue.Text = doctors.ToString();
                lblRevenueValue.Text = $"${revenue:N2}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dashboard error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }
    }
}
