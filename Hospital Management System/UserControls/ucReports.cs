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
            cboReport.SelectedIndexChanged += cboReport_SelectedIndexChanged;
            if (cboReport.Items.Count > 0)
            {
                cboReport.SelectedIndex = 0;
            }

            ConfigureGrid();
        }

        private async void btnLoad_Click(object sender, System.EventArgs e)
        {
            await LoadReportAsync().ConfigureAwait(true);
        }

        private async void cboReport_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            await LoadReportAsync().ConfigureAwait(true);
        }

        private async System.Threading.Tasks.Task LoadReportAsync()
        {
            try
            {
                UseWaitCursor = true;
                var selectedReport = cboReport.SelectedItem?.ToString() ?? cboReport.Text;
                var table = await _service.GetReportAsync(selectedReport).ConfigureAwait(true);
                dgvReport.DataSource = table;
                ConfigureColumns();
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

        private void ConfigureGrid()
        {
            dgvReport.AutoGenerateColumns = true;
            dgvReport.ReadOnly = true;
            dgvReport.RowHeadersVisible = false;
            dgvReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReport.MultiSelect = false;
            dgvReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvReport.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dgvReport.AllowUserToResizeRows = false;
        }

        private void ConfigureColumns()
        {
            foreach (DataGridViewColumn column in dgvReport.Columns)
            {
                var header = column.HeaderText ?? string.Empty;
                if (header.IndexOf("Date", System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    column.DefaultCellStyle.Format = "yyyy-MM-dd";
                }
                else if (header.IndexOf("Time", System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    column.DefaultCellStyle.Format = "HH:mm:ss";
                }
                else if (header.IndexOf("Total", System.StringComparison.OrdinalIgnoreCase) >= 0
                         || header.IndexOf("Price", System.StringComparison.OrdinalIgnoreCase) >= 0
                         || header.IndexOf("Fee", System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    column.DefaultCellStyle.Format = "N2";
                }
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
