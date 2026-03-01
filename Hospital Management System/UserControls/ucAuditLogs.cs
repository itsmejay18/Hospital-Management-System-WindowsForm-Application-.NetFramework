using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using HospitalManagementSystem.DAL;
using HospitalManagementSystem.Helpers;

namespace HospitalManagementSystem.UserControls
{
    public sealed class ucAuditLogs : UserControl
    {
        private readonly Panel _pnlSearch;
        private readonly Label _lblSearch;
        private readonly TextBox _txtSearch;
        private readonly Button _btnSearch;
        private readonly Button _btnRefresh;
        private readonly DataGridView _dgvAudit;
        private readonly Label _lblHint;

        public ucAuditLogs()
        {
            Dock = DockStyle.Fill;

            _pnlSearch = new Panel
            {
                Dock = DockStyle.Top,
                Height = 54,
                Padding = new Padding(12, 10, 12, 8)
            };

            _lblSearch = new Label
            {
                AutoSize = true,
                Text = "Search Logs",
                Left = 12,
                Top = 17
            };

            _txtSearch = new TextBox
            {
                Left = 99,
                Top = 14,
                Width = 340
            };

            _btnSearch = new Button
            {
                Text = "Search",
                Left = 445,
                Top = 12,
                Width = 88,
                Height = 28
            };

            _btnRefresh = new Button
            {
                Text = "Refresh",
                Left = 539,
                Top = 12,
                Width = 88,
                Height = 28
            };

            _pnlSearch.Controls.Add(_lblSearch);
            _pnlSearch.Controls.Add(_txtSearch);
            _pnlSearch.Controls.Add(_btnSearch);
            _pnlSearch.Controls.Add(_btnRefresh);

            _dgvAudit = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            _lblHint = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 34,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 12, 0),
                Text = "Audit logs loaded from database."
            };

            Controls.Add(_dgvAudit);
            Controls.Add(_lblHint);
            Controls.Add(_pnlSearch);

            _btnSearch.Click += btnSearch_Click;
            _btnRefresh.Click += btnRefresh_Click;
            _txtSearch.KeyDown += txtSearch_KeyDown;
            Load += ucAuditLogs_Load;

            ApplyTheme();
        }

        private async void ucAuditLogs_Load(object sender, EventArgs e)
        {
            await LoadAuditLogsAsync().ConfigureAwait(true);
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            await LoadAuditLogsAsync().ConfigureAwait(true);
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            _txtSearch.Clear();
            await LoadAuditLogsAsync().ConfigureAwait(true);
        }

        private async void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.SuppressKeyPress = true;
            await LoadAuditLogsAsync().ConfigureAwait(true);
        }

        private async Task LoadAuditLogsAsync()
        {
            try
            {
                UseWaitCursor = true;
                var searchTerm = _txtSearch.Text.Trim();
                var sql =
                    "SELECT LogID, UserID, Action, TableName, RecordID, OldValue, NewValue, IPAddress, MachineName, LogDate " +
                    "FROM AuditLogs " +
                    "WHERE (@term = '' " +
                    "   OR CAST(LogID AS CHAR) LIKE CONCAT('%', @term, '%') " +
                    "   OR CAST(UserID AS CHAR) LIKE CONCAT('%', @term, '%') " +
                    "   OR Action LIKE CONCAT('%', @term, '%') " +
                    "   OR TableName LIKE CONCAT('%', @term, '%') " +
                    "   OR CAST(RecordID AS CHAR) LIKE CONCAT('%', @term, '%') " +
                    "   OR IPAddress LIKE CONCAT('%', @term, '%') " +
                    "   OR MachineName LIKE CONCAT('%', @term, '%')) " +
                    "ORDER BY LogDate DESC";

                var parameters = new Dictionary<string, object>
                {
                    ["@term"] = searchTerm
                };

                var table = await DatabaseConnection.Instance.ExecuteQueryAsync(sql, parameters).ConfigureAwait(true);
                _dgvAudit.DataSource = table;

                if (_dgvAudit.Columns.Contains("LogDate"))
                {
                    _dgvAudit.Columns["LogDate"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
                }

                _lblHint.Text = $"{table.Rows.Count} audit log entries loaded.";
            }
            catch (Exception ex)
            {
                _lblHint.Text = "Failed to load audit logs.";
                MessageBox.Show($"Failed to load audit logs: {ex.Message}", "Audit Logs", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void ApplyTheme()
        {
            ThemeManager.ApplyControlTheme(this);
            ThemeManager.StyleModuleBarPanel(_pnlSearch);
            ThemeManager.StyleDataGridView(_dgvAudit);
            ThemeManager.StyleButton(_btnSearch, ThemeButtonKind.Primary);
            ThemeManager.StyleButton(_btnRefresh, ThemeButtonKind.Secondary);
            ThemeManager.StyleSearchTextBox(_txtSearch, "Search action / table / user / IP");
        }
    }
}
