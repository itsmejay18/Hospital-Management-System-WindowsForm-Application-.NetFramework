using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using HospitalManagementSystem.DAL;
using HospitalManagementSystem.Helpers;
using MySql.Data.MySqlClient;

namespace HospitalManagementSystem.Forms
{
    public sealed class frmDatabaseConnection : Form
    {
        private readonly Dictionary<string, DatabaseConnectionProfile> _profiles =
            new Dictionary<string, DatabaseConnectionProfile>(StringComparer.OrdinalIgnoreCase);

        private readonly Label _lblModeHint;
        private readonly Label _lblError;
        private readonly ComboBox _cboMode;
        private readonly TextBox _txtHost;
        private readonly NumericUpDown _numPort;
        private readonly TextBox _txtDatabase;
        private readonly TextBox _txtUsername;
        private readonly TextBox _txtPassword;
        private readonly CheckBox _chkSaveProfile;
        private readonly Label _lblStatus;
        private string _activeMode;

        public frmDatabaseConnection(AppSettingsProfile settingsProfile, string errorMessage = null)
        {
            Text = "Database Connection";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MinimizeBox = false;
            MaximizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(560, 420);

            var lblTitle = new Label
            {
                AutoSize = false,
                Location = new Point(16, 14),
                Size = new Size(528, 30),
                Font = new Font("Segoe UI Semibold", 13F, FontStyle.Regular),
                Text = "Choose Database Route"
            };

            var lblMessage = new Label
            {
                AutoSize = false,
                Location = new Point(16, 48),
                Size = new Size(528, 36),
                Text = "Select Local, Online, or Network. Online is prefilled for your Hostinger database."
            };

            _lblError = new Label
            {
                AutoSize = false,
                Location = new Point(16, 88),
                Size = new Size(528, 34),
                ForeColor = Color.DarkRed,
                Text = string.IsNullOrWhiteSpace(errorMessage) ? string.Empty : $"Last error: {errorMessage}"
            };

            var table = new TableLayoutPanel
            {
                Location = new Point(16, 130),
                Size = new Size(528, 214),
                ColumnCount = 2,
                RowCount = 7
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            for (var i = 0; i < 7; i++)
            {
                table.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            }

            _cboMode = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cboMode.Items.AddRange(new object[]
            {
                DatabaseConnectionProfiles.LocalMode,
                DatabaseConnectionProfiles.OnlineMode,
                DatabaseConnectionProfiles.NetworkMode
            });
            _cboMode.SelectedIndexChanged += cboMode_SelectedIndexChanged;

            _lblModeHint = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.DimGray
            };

            _txtHost = new TextBox { Dock = DockStyle.Fill };
            _numPort = new NumericUpDown
            {
                Dock = DockStyle.Left,
                Width = 120,
                Minimum = 1,
                Maximum = 65535,
                Value = DatabaseDefaults.Port
            };
            _txtDatabase = new TextBox { Dock = DockStyle.Fill };
            _txtUsername = new TextBox { Dock = DockStyle.Fill };
            _txtPassword = new TextBox { Dock = DockStyle.Fill, UseSystemPasswordChar = true };
            _chkSaveProfile = new CheckBox
            {
                Text = "Save as active startup connection",
                AutoSize = true,
                Checked = true
            };

            table.Controls.Add(CreateLabel("Route"), 0, 0);
            table.Controls.Add(_cboMode, 1, 0);
            table.Controls.Add(CreateLabel("Preset"), 0, 1);
            table.Controls.Add(_lblModeHint, 1, 1);
            table.Controls.Add(CreateLabel("Host"), 0, 2);
            table.Controls.Add(_txtHost, 1, 2);
            table.Controls.Add(CreateLabel("Port"), 0, 3);
            table.Controls.Add(_numPort, 1, 3);
            table.Controls.Add(CreateLabel("Database"), 0, 4);
            table.Controls.Add(_txtDatabase, 1, 4);
            table.Controls.Add(CreateLabel("Username"), 0, 5);
            table.Controls.Add(_txtUsername, 1, 5);
            table.Controls.Add(CreateLabel("Password"), 0, 6);
            table.Controls.Add(_txtPassword, 1, 6);

            var pnlOptions = new Panel
            {
                Location = new Point(16, 348),
                Size = new Size(528, 24)
            };
            pnlOptions.Controls.Add(_chkSaveProfile);

            var btnTest = new Button
            {
                Text = "Test Connection",
                Location = new Point(16, 382),
                Size = new Size(132, 30)
            };
            btnTest.Click += btnTest_Click;

            var btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(356, 382),
                Size = new Size(88, 30),
                DialogResult = DialogResult.Cancel
            };

            var btnConnect = new Button
            {
                Text = "Connect",
                Location = new Point(456, 382),
                Size = new Size(88, 30)
            };
            btnConnect.Click += btnConnect_Click;

            _lblStatus = new Label
            {
                AutoSize = false,
                Location = new Point(156, 386),
                Size = new Size(192, 22),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "Ready."
            };

            Controls.Add(lblTitle);
            Controls.Add(lblMessage);
            Controls.Add(_lblError);
            Controls.Add(table);
            Controls.Add(pnlOptions);
            Controls.Add(btnTest);
            Controls.Add(btnCancel);
            Controls.Add(btnConnect);
            Controls.Add(_lblStatus);

            AcceptButton = btnConnect;
            CancelButton = btnCancel;

            SeedProfiles(settingsProfile);
            _cboMode.SelectedItem = DatabaseConnectionProfiles.NormalizeMode(settingsProfile?.DatabaseMode);
            if (_cboMode.SelectedIndex < 0)
            {
                _cboMode.SelectedItem = DatabaseConnectionProfiles.OnlineMode;
            }

            ApplySelectedMode();
        }

        internal DatabaseConnectionProfile SelectedProfile { get; private set; }

        public string SelectedConnectionString { get; private set; }

        private static Label CreateLabel(string text)
        {
            return new Label
            {
                Text = text,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill
            };
        }

        private void SeedProfiles(AppSettingsProfile settingsProfile)
        {
            _profiles[DatabaseConnectionProfiles.LocalMode] = DatabaseConnectionProfiles.CreatePreset(DatabaseConnectionProfiles.LocalMode);
            _profiles[DatabaseConnectionProfiles.OnlineMode] = DatabaseConnectionProfiles.CreatePreset(DatabaseConnectionProfiles.OnlineMode);
            _profiles[DatabaseConnectionProfiles.NetworkMode] = DatabaseConnectionProfiles.CreatePreset(DatabaseConnectionProfiles.NetworkMode);

            var activeProfile = DatabaseConnectionProfiles.CreateFromAppSettings(settingsProfile);
            _profiles[activeProfile.Mode] = activeProfile.Clone();
        }

        private string SelectedMode =>
            DatabaseConnectionProfiles.NormalizeMode(_cboMode.SelectedItem?.ToString());

        private void cboMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplySelectedMode();
        }

        private void ApplySelectedMode()
        {
            SaveCurrentMode();

            var mode = SelectedMode;
            var profile = _profiles[mode].Clone();
            _activeMode = mode;

            _txtHost.Text = profile.Host;
            _numPort.Value = profile.Port > 0 ? profile.Port : DatabaseDefaults.Port;
            _txtDatabase.Text = profile.DatabaseName;
            _txtUsername.Text = profile.Username;
            _txtPassword.Text = profile.Password;
            _lblModeHint.Text = GetModeHint(mode);
            _lblStatus.Text = $"Using {mode} preset.";
        }

        private void SaveCurrentMode()
        {
            if (string.IsNullOrWhiteSpace(_activeMode) || !_profiles.ContainsKey(_activeMode))
            {
                return;
            }

            _profiles[_activeMode] = BuildCurrentProfile();
        }

        private DatabaseConnectionProfile BuildCurrentProfile()
        {
            var mode = SelectedMode;
            var existing = _profiles.ContainsKey(mode)
                ? _profiles[mode]
                : DatabaseConnectionProfiles.CreatePreset(mode);

            return new DatabaseConnectionProfile
            {
                Mode = mode,
                Transport = existing.Transport,
                Host = (_txtHost.Text ?? string.Empty).Trim(),
                Port = Convert.ToInt32(_numPort.Value),
                DatabaseName = (_txtDatabase.Text ?? string.Empty).Trim(),
                Username = (_txtUsername.Text ?? string.Empty).Trim(),
                Password = _txtPassword.Text ?? string.Empty
            };
        }

        private async void btnTest_Click(object sender, EventArgs e)
        {
            if (!ValidateFields())
            {
                return;
            }

            var profile = BuildCurrentProfile();
            await TestConnectionAsync(profile, updateStatusOnly: true).ConfigureAwait(true);
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            if (!ValidateFields())
            {
                return;
            }

            var profile = BuildCurrentProfile();
            var connected = await TestConnectionAsync(profile, updateStatusOnly: false).ConfigureAwait(true);
            if (!connected)
            {
                return;
            }

            var connectionString = profile.BuildConnectionString();
            DatabaseConnection.SetRuntimeConnectionString(connectionString);

            if (_chkSaveProfile.Checked)
            {
                var settings = AppSettingsStore.Load();
                DatabaseConnectionProfiles.ApplyToAppSettings(settings, profile);
                AppSettingsStore.Save(settings);
            }

            SelectedProfile = profile;
            SelectedConnectionString = connectionString;
            DialogResult = DialogResult.OK;
            Close();
        }

        private async Task<bool> TestConnectionAsync(DatabaseConnectionProfile profile, bool updateStatusOnly)
        {
            try
            {
                UseWaitCursor = true;
                _lblStatus.Text = "Testing connection...";

                using (var connection = new MySqlConnection(profile.BuildConnectionString()))
                {
                    await connection.OpenAsync().ConfigureAwait(true);
                }

                _lblError.Text = string.Empty;
                _lblStatus.Text = "Connection succeeded.";
                if (updateStatusOnly)
                {
                    MessageBox.Show("Database connection is successful.", "Database", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                return true;
            }
            catch (Exception ex)
            {
                _lblError.Text = $"Connection failed: {GetRootErrorMessage(ex)}";
                _lblStatus.Text = "Connection failed.";
                if (updateStatusOnly)
                {
                    MessageBox.Show($"Unable to connect: {GetRootErrorMessage(ex)}", "Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return false;
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(_txtHost.Text))
            {
                MessageBox.Show("Host is required.", "Database", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtHost.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(_txtDatabase.Text))
            {
                MessageBox.Show("Database name is required.", "Database", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtDatabase.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(_txtUsername.Text))
            {
                MessageBox.Show("Database username is required.", "Database", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtUsername.Focus();
                return false;
            }

            return true;
        }

        private static string GetModeHint(string mode)
        {
            switch (DatabaseConnectionProfiles.NormalizeMode(mode))
            {
                case DatabaseConnectionProfiles.LocalMode:
                    return "Local preset uses localhost with a default root/root profile.";
                case DatabaseConnectionProfiles.NetworkMode:
                    return "Network preset is for another MySQL server on your LAN.";
                default:
                    return "Online is prefilled for your Hostinger database.";
            }
        }

        private static string GetRootErrorMessage(Exception exception)
        {
            if (exception == null)
            {
                return "Unknown connection failure.";
            }

            var current = exception;
            while (current.InnerException != null)
            {
                current = current.InnerException;
            }

            return string.IsNullOrWhiteSpace(current.Message) ? current.GetType().Name : current.Message;
        }
    }
}
