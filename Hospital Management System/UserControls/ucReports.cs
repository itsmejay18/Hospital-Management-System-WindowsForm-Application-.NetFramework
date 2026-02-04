using System.Windows.Forms;
using HospitalManagementSystem.BLL.Services;
using HospitalManagementSystem.Forms.Shared;
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
            btnUsers.Click += btnUsers_Click;
            btnAuditLog.Click += btnAuditLog_Click;
            btnBackup.Click += btnBackup_Click;
            if (cboReport.Items.Count > 0)
            {
                cboReport.SelectedIndex = 0;
            }
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

        private void btnUsers_Click(object sender, System.EventArgs e)
        {
            using (var dlg = new frmUserEdit())
            {
                dlg.ShowDialog(this);
            }
        }

        private void btnAuditLog_Click(object sender, System.EventArgs e)
        {
            using (var dlg = new frmAuditLog())
            {
                dlg.ShowDialog(this);
            }
        }

        private void btnBackup_Click(object sender, System.EventArgs e)
        {
            using (var dlg = new frmBackupRestore())
            {
                dlg.ShowDialog(this);
            }
        }
    }
}
