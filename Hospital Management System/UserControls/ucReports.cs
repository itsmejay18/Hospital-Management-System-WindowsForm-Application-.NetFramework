using System.Windows.Forms;
using System.Linq;
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
            ApplyTheme();
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
            UpdateExportButtonState();
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
                UpdateExportButtonState();
            }
            catch (System.Exception ex)
            {
                dgvReport.DataSource = null;
                UpdateExportButtonState();
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
            ThemeManager.StyleDataGridView(dgvReport);
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
                         || header.IndexOf("Amount", System.StringComparison.OrdinalIgnoreCase) >= 0
                         || header.IndexOf("Balance", System.StringComparison.OrdinalIgnoreCase) >= 0
                         || header.IndexOf("Paid", System.StringComparison.OrdinalIgnoreCase) >= 0
                         || header.IndexOf("Invoiced", System.StringComparison.OrdinalIgnoreCase) >= 0
                         || header.IndexOf("Price", System.StringComparison.OrdinalIgnoreCase) >= 0
                         || header.IndexOf("Fee", System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    column.DefaultCellStyle.Format = "N2";
                }
            }
        }

        private void btnExportExcel_Click(object sender, System.EventArgs e)
        {
            if (!CanExport())
            {
                MessageBox.Show("Load a report with data before exporting.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog { Filter = "Excel (*.xlsx)|*.xlsx" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ExportHelper.ExportToExcel(dgvReport, sfd.FileName);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Excel export failed: {ex.Message}", "Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnExportCsv_Click(object sender, System.EventArgs e)
        {
            if (!CanExport())
            {
                MessageBox.Show("Load a report with data before exporting.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog { Filter = "CSV (*.csv)|*.csv" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ExportHelper.ExportToCsv(dgvReport, sfd.FileName);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"CSV export failed: {ex.Message}", "Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnExportPdf_Click(object sender, System.EventArgs e)
        {
            if (!CanExport())
            {
                MessageBox.Show("Load a report with data before exporting.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog { Filter = "PDF (*.pdf)|*.pdf" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ExportHelper.ExportToPdf(dgvReport, sfd.FileName);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"PDF export failed: {ex.Message}", "Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnUsers_Click(object sender, System.EventArgs e)
        {
            using (var dlg = new frmUserEdit())
            {
                IWin32Window owner = FindForm();
                if (owner == null)
                {
                    owner = this;
                }
                dlg.ShowDialog(owner);
            }
        }

        private void btnAuditLog_Click(object sender, System.EventArgs e)
        {
            using (var dlg = new frmAuditLog())
            {
                IWin32Window owner = FindForm();
                if (owner == null)
                {
                    owner = this;
                }
                dlg.ShowDialog(owner);
            }
        }

        private void btnBackup_Click(object sender, System.EventArgs e)
        {
            using (var dlg = new frmBackupRestore())
            {
                IWin32Window owner = FindForm();
                if (owner == null)
                {
                    owner = this;
                }
                dlg.ShowDialog(owner);
            }
        }

        private void ApplyTheme()
        {
            ThemeManager.ApplyControlTheme(this);
            ThemeManager.StyleModuleBarPanel(pnlButtons);
            ThemeManager.StyleButton(btnLoad, ThemeButtonKind.Primary);
            ThemeManager.StyleButton(btnExportExcel, ThemeButtonKind.Secondary);
            ThemeManager.StyleButton(btnExportCsv, ThemeButtonKind.Secondary);
            ThemeManager.StyleButton(btnExportPdf, ThemeButtonKind.Secondary);
            ThemeManager.StyleButton(btnUsers, ThemeButtonKind.Secondary);
            ThemeManager.StyleButton(btnAuditLog, ThemeButtonKind.Secondary);
            ThemeManager.StyleButton(btnBackup, ThemeButtonKind.Secondary);
            ThemeManager.StyleComboBox(cboReport);
        }

        private bool CanExport()
        {
            return dgvReport.Columns.Count > 0
                && dgvReport.Rows.Cast<DataGridViewRow>().Any(row => !row.IsNewRow);
        }

        private void UpdateExportButtonState()
        {
            var enabled = CanExport();
            btnExportExcel.Enabled = enabled;
            btnExportCsv.Enabled = enabled;
            btnExportPdf.Enabled = enabled;
        }
    }
}
