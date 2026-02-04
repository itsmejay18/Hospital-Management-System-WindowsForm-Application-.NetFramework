using System;
using System.Data;
using System.Windows.Forms;
using HospitalManagementSystem.DAL;

namespace HospitalManagementSystem.Forms.Shared
{
    public partial class frmAuditLog : Form
    {
        public frmAuditLog()
        {
            InitializeComponent();
            Load += frmAuditLog_Load;
        }

        private async void frmAuditLog_Load(object sender, EventArgs e)
        {
            try
            {
                var table = await DatabaseConnection.Instance.ExecuteQueryAsync(
                    "SELECT * FROM AuditLogs ORDER BY LogDate DESC").ConfigureAwait(true);
                dgvAudit.DataSource = table;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load audit logs: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
