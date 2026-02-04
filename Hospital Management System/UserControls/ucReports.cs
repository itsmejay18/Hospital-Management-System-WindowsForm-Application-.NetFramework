using System.Windows.Forms;
using HospitalManagementSystem.BLL.Services;
using HospitalManagementSystem.Helpers;

namespace HospitalManagementSystem.UserControls
{
    public partial class ucReports : UserControl
    {
        private readonly ReportService _service = new ReportService();

        public ucReports()
        {
            InitializeComponent();
            btnExportExcel.Click += btnExportExcel_Click;
            btnExportCsv.Click += btnExportCsv_Click;
            btnExportPdf.Click += btnExportPdf_Click;
            btnLoad.Click += btnLoad_Click;
        }

        private async void btnLoad_Click(object sender, System.EventArgs e)
        {
            try
            {
                UseWaitCursor = true;
                var table = await _service.GetReportAsync(cboReport.Text).ConfigureAwait(true);
                dgvReport.DataSource = table;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Report load failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void btnExportExcel_Click(object sender, System.EventArgs e)
        {
            using (var sfd = new SaveFileDialog { Filter = "Excel (*.xlsx)|*.xlsx" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    ExportHelper.ExportToExcel(dgvReport, sfd.FileName);
                }
            }
        }

        private void btnExportCsv_Click(object sender, System.EventArgs e)
        {
            using (var sfd = new SaveFileDialog { Filter = "CSV (*.csv)|*.csv" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    ExportHelper.ExportToCsv(dgvReport, sfd.FileName);
                }
            }
        }

        private void btnExportPdf_Click(object sender, System.EventArgs e)
        {
            using (var sfd = new SaveFileDialog { Filter = "PDF (*.pdf)|*.pdf" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    ExportHelper.ExportToPdf(dgvReport, sfd.FileName);
                }
            }
        }
    }
}
