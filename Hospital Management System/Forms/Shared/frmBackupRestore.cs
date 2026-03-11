using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HospitalManagementSystem.Helpers;

namespace HospitalManagementSystem.Forms.Shared
{
    public partial class frmBackupRestore : Form
    {
        private readonly DatabaseBackupService _backupService;
        private readonly BindingList<DatabaseBackupSetSummary> _backups = new BindingList<DatabaseBackupSetSummary>();

        public frmBackupRestore()
        {
            InitializeComponent();
            ThemeManager.ApplyFormTheme(this);
            _backupService = new DatabaseBackupService();
            ConfigureGrid();
            LoadDefaults();
        }

        private void ConfigureGrid()
        {
            dgvBackups.AutoGenerateColumns = false;
            dgvBackups.DataSource = _backups;
            dgvBackups.SelectionChanged += dgvBackups_SelectionChanged;
        }

        private void LoadDefaults()
        {
            cboBackupType.DataSource = Enum.GetValues(typeof(DatabaseBackupKind));
            cboBackupType.SelectedItem = DatabaseBackupKind.Full;
            txtBackupPath.Text = _backupService.GetDefaultBackupRoot();
            lblConnectionInfo.Text = $"Active source: {_backupService.SourceServer} / {_backupService.SourceDatabase}";
            LoadBackups();
            UpdateButtons();
        }

        private void LoadBackups()
        {
            _backups.RaiseListChangedEvents = false;
            _backups.Clear();
            foreach (var backup in _backupService.ListBackups(txtBackupPath.Text))
            {
                _backups.Add(backup);
            }

            _backups.RaiseListChangedEvents = true;
            _backups.ResetBindings();
            lblStatus.Text = _backups.Count == 0
                ? "No backup sets found in the selected folder."
                : $"Loaded {_backups.Count} backup set(s).";
        }

        private DatabaseBackupSetSummary GetSelectedBackup()
        {
            return dgvBackups.CurrentRow?.DataBoundItem as DatabaseBackupSetSummary;
        }

        private void dgvBackups_SelectionChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            var hasSelection = GetSelectedBackup() != null;
            btnRestore.Enabled = hasSelection;
            btnOpenFolder.Enabled = !string.IsNullOrWhiteSpace(txtBackupPath.Text);
        }

        private void PersistBackupPath()
        {
            var profile = AppSettingsStore.Load();
            profile.BackupPath = txtBackupPath.Text.Trim();
            AppSettingsStore.Save(profile);
        }

        private async void btnBackup_Click(object sender, EventArgs e)
        {
            if (!EnsureBackupPath())
            {
                return;
            }

            ToggleBusy(true);
            try
            {
                PersistBackupPath();
                var progress = new Progress<string>(message => lblStatus.Text = message);
                var requestedKind = (DatabaseBackupKind)cboBackupType.SelectedItem;
                var created = await Task.Run(() => _backupService.CreateBackup(requestedKind, txtBackupPath.Text.Trim(), progress)).ConfigureAwait(true);
                LoadBackups();
                SelectBackup(created.BackupId);

                var resultMessage = created.BackupKind == requestedKind
                    ? $"Backup created successfully: {created.BackupKind}."
                    : $"Backup created as {created.BackupKind} because a required baseline backup was not available or the schema changed.";
                MessageBox.Show(resultMessage, "Backup", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Backup failed.";
                MessageBox.Show($"Backup failed: {ex.Message}", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ToggleBusy(false);
            }
        }

        private async void btnRestore_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedBackup();
            if (selected == null)
            {
                MessageBox.Show("Select a backup set to restore.", "Restore", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                "This will overwrite the current database data with the selected backup chain.\r\n\r\nContinue?",
                "Confirm Restore",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            ToggleBusy(true);
            try
            {
                var progress = new Progress<string>(message => lblStatus.Text = message);
                await Task.Run(() => _backupService.RestoreBackup(txtBackupPath.Text.Trim(), selected.BackupId, progress)).ConfigureAwait(true);
                MessageBox.Show("Restore completed successfully.", "Restore", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Restore failed.";
                MessageBox.Show($"Restore failed: {ex.Message}", "Restore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ToggleBusy(false);
            }
        }

        private void btnBrowsePath_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Choose a backup folder on this device or another connected drive.";
                dialog.SelectedPath = Directory.Exists(txtBackupPath.Text) ? txtBackupPath.Text : _backupService.GetDefaultBackupRoot();
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                txtBackupPath.Text = dialog.SelectedPath;
                PersistBackupPath();
                LoadBackups();
                UpdateButtons();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (!EnsureBackupPath())
            {
                return;
            }

            LoadBackups();
            UpdateButtons();
        }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            if (!EnsureBackupPath())
            {
                return;
            }

            Process.Start("explorer.exe", txtBackupPath.Text.Trim());
        }

        private bool EnsureBackupPath()
        {
            var path = txtBackupPath.Text.Trim();
            if (string.IsNullOrWhiteSpace(path))
            {
                MessageBox.Show("Choose a backup target folder first.", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBackupPath.Focus();
                return false;
            }

            Directory.CreateDirectory(path);
            return true;
        }

        private void SelectBackup(string backupId)
        {
            if (string.IsNullOrWhiteSpace(backupId))
            {
                return;
            }

            foreach (DataGridViewRow row in dgvBackups.Rows)
            {
                var summary = row.DataBoundItem as DatabaseBackupSetSummary;
                if (summary != null && string.Equals(summary.BackupId, backupId, StringComparison.OrdinalIgnoreCase))
                {
                    if (row.Cells.Count > 0)
                    {
                        dgvBackups.CurrentCell = row.Cells[0];
                    }

                    row.Selected = true;
                    break;
                }
            }
        }

        private void ToggleBusy(bool isBusy)
        {
            UseWaitCursor = isBusy;
            btnBackup.Enabled = !isBusy;
            btnRestore.Enabled = !isBusy && GetSelectedBackup() != null;
            btnBrowsePath.Enabled = !isBusy;
            btnRefresh.Enabled = !isBusy;
            btnOpenFolder.Enabled = !isBusy;
            btnClose.Enabled = !isBusy;
        }
    }
}
