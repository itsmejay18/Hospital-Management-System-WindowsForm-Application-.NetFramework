using System;
using System.Windows.Forms;
using HospitalManagementSystem.BLL.Services;
using HospitalManagementSystem.Helpers;

namespace HospitalManagementSystem.UserControls
{
    public partial class ucDashboard : UserControl
    {
        private readonly DashboardService _service = new DashboardService();

        public ucDashboard()
        {
            InitializeComponent();
            ApplyTheme();
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

        private void ApplyTheme()
        {
            ThemeManager.ApplyControlTheme(this);
            ThemeManager.StyleCardPanel(pnlPatients);
            ThemeManager.StyleCardPanel(pnlDoctors);
            ThemeManager.StyleCardPanel(pnlRevenue);

            lblPatientsValue.Font = ThemeManager.Fonts.Kpi;
            lblDoctorsValue.Font = ThemeManager.Fonts.Kpi;
            lblRevenueValue.Font = ThemeManager.Fonts.Kpi;
        }
    }
}
