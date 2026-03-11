using System;
using System.Drawing;
using System.Windows.Forms;
using HospitalManagementSystem.BLL.Services;
using HospitalManagementSystem.DAL;
using HospitalManagementSystem.Forms.Shared;
using HospitalManagementSystem.Helpers;
using MySql.Data.MySqlClient;

namespace HospitalManagementSystem.UserControls
{
    public sealed class ucSettings : UserControl
    {
        private readonly AuthenticatedUser _user;

        private Button _btnTabSystem;
        private Button _btnTabDatabase;
        private Button _btnTabUsers;
        private Button _btnTabAudit;

        private TextBox _txtCompanyName;
        private TextBox _txtAddress;
        private TextBox _txtPhone;
        private TextBox _txtEmail;
        private NumericUpDown _numDailyRate;
        private NumericUpDown _numLateFee;
        private NumericUpDown _numTaxRate;
        private TextBox _txtBackupPath;
        private TextBox _txtDumpPath;
        private TextBox _txtSmtpHost;
        private NumericUpDown _numSmtpPort;
        private TextBox _txtSmtpUser;
        private TextBox _txtSmtpPassword;
        private CheckBox _chkSsl;
        private CheckBox _chkDarkMode;
        private ComboBox _cboDbMode;
        private TextBox _txtDbProfileKey;
        private TextBox _txtBootstrapConnection;
        private Button _btnBackupNow;
        private Button _btnRestore;
        private Button _btnSave;
        private Panel _pnlSystem;
        private Panel _pnlDatabase;
        private Panel _pnlContentHost;
        private ucUsers _usersModule;
        private ucAuditLogs _auditLogsModule;
        private Control _activeContent;

        private ComboBox _cboDbTransport;
        private TextBox _txtDbHost;
        private NumericUpDown _numDbPort;
        private TextBox _txtDbName;
        private TextBox _txtDbUsername;
        private TextBox _txtDbPassword;
        private CheckBox _chkDbSetActiveProfile;
        private Button _btnDbLoadProfile;
        private Button _btnDbSaveProfile;
        private Button _btnDbTestConnection;
        private Button _btnDbApplyRuntime;
        private Label _lblDbStatus;

        public ucSettings(AuthenticatedUser user)
        {
            _user = user ?? new AuthenticatedUser();
            BuildLayout();
            ApplyTheme();
            LoadSavedValues();
            ActivateSystemTab();
        }

        private void BuildLayout()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeManager.Colors.Background;

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = ThemeManager.Colors.Background
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 240F));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            var left = BuildLeftNav();
            _pnlSystem = BuildSystemPanel();
            _pnlDatabase = BuildDatabasePanel();
            _pnlContentHost = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(4),
                BackColor = ThemeManager.Colors.Background
            };
            _pnlContentHost.Controls.Add(_pnlSystem);
            _activeContent = _pnlSystem;

            root.Controls.Add(left, 0, 0);
            root.Controls.Add(_pnlContentHost, 1, 0);
            Controls.Add(root);
        }

        private Control BuildLeftNav()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                Margin = new Padding(4)
            };

            var lblTitle = new Label
            {
                Dock = DockStyle.Top,
                Height = 34,
                Text = "Settings",
                Font = new Font("Segoe UI Semibold", 16F, FontStyle.Regular)
            };

            var lblHint = new Label
            {
                Dock = DockStyle.Top,
                Height = 28,
                Text = "Sensitive tools are grouped",
                ForeColor = ThemeManager.Colors.TextSecondary
            };

            var tabs = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 192,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(0, 8, 0, 0)
            };

            _btnTabSystem = new Button
            {
                Text = "System",
                Width = 160,
                Height = 34,
                Margin = new Padding(0, 0, 0, 8)
            };
            _btnTabUsers = new Button
            {
                Text = "Users",
                Width = 160,
                Height = 34,
                Margin = new Padding(0, 0, 0, 8)
            };
            _btnTabDatabase = new Button
            {
                Text = "Database",
                Width = 160,
                Height = 34,
                Margin = new Padding(0, 0, 0, 8)
            };
            _btnTabAudit = new Button
            {
                Text = "Audit Logs",
                Width = 160,
                Height = 34,
                Margin = new Padding(0)
            };

            _btnTabSystem.Click += (_, __) => ActivateSystemTab();
            _btnTabDatabase.Click += (_, __) => ActivateDatabaseTab();
            _btnTabUsers.Click += btnTabUsers_Click;
            _btnTabAudit.Click += btnTabAudit_Click;

            tabs.Controls.Add(_btnTabSystem);
            tabs.Controls.Add(_btnTabDatabase);
            tabs.Controls.Add(_btnTabUsers);
            tabs.Controls.Add(_btnTabAudit);

            panel.Controls.Add(tabs);
            panel.Controls.Add(lblHint);
            panel.Controls.Add(lblTitle);
            return panel;
        }

        private Panel BuildSystemPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(14, 12, 14, 12),
                Margin = new Padding(4)
            };

            var scroll = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 2,
                RowCount = 0,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(0)
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            _txtCompanyName = CreateTextBox();
            _txtAddress = CreateTextBox();
            _txtPhone = CreateTextBox();
            _txtEmail = CreateTextBox();
            _numDailyRate = CreateNumeric(0, 1000000, 1000);
            _numLateFee = CreateNumeric(0, 1000000, 200);
            _numTaxRate = CreateNumeric(0, 100, 12);
            _txtBackupPath = CreateTextBox();
            _txtDumpPath = CreateTextBox();
            _txtSmtpHost = CreateTextBox();
            _numSmtpPort = CreateNumeric(1, 65535, 587);
            _txtSmtpUser = CreateTextBox();
            _txtSmtpPassword = CreateTextBox();
            _txtSmtpPassword.UseSystemPasswordChar = true;
            _chkSsl = new CheckBox { Text = "Enable SSL", AutoSize = true };
            _chkDarkMode = new CheckBox { Text = "Enable dark mode", AutoSize = true };
            _btnBackupNow = new Button { Text = "Backup Now", Width = 110, Height = 34 };
            _btnRestore = new Button { Text = "Restore", Width = 90, Height = 34 };
            _btnSave = new Button { Text = "Save Settings", Width = 130, Height = 36 };

            _btnBackupNow.Click += btnBackupNow_Click;
            _btnRestore.Click += btnRestore_Click;
            _btnSave.Click += btnSave_Click;

            AddRow(table, "Company Name", _txtCompanyName);
            AddRow(table, "Address", _txtAddress);
            AddRow(table, "Phone", _txtPhone);
            AddRow(table, "Email", _txtEmail);
            AddRow(table, "Default Daily Rate", _numDailyRate);
            AddRow(table, "Late Fee / Day", _numLateFee);
            AddRow(table, "Tax Rate (%)", _numTaxRate);
            AddRow(table, "Backup Path", _txtBackupPath);
            AddRow(table, "Legacy SQL Tool", _txtDumpPath);
            AddRow(table, "SMTP Host", _txtSmtpHost);
            AddRow(table, "SMTP Port", _numSmtpPort);
            AddRow(table, "SMTP User", _txtSmtpUser);
            AddRow(table, "SMTP Password", _txtSmtpPassword);
            AddRow(table, "SMTP SSL", _chkSsl);
            AddRow(table, "Dark Mode", _chkDarkMode);

            var backupActions = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0)
            };
            backupActions.Controls.Add(_btnBackupNow);
            backupActions.Controls.Add(_btnRestore);
            AddRow(table, "Backup/Restore", backupActions);

            var saveHost = new Panel
            {
                Dock = DockStyle.Top,
                Height = 54,
                Padding = new Padding(0, 10, 0, 0)
            };
            _btnSave.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnSave.Location = new Point(Math.Max(0, saveHost.Width - _btnSave.Width), 10);
            saveHost.Resize += (_, __) =>
            {
                _btnSave.Location = new Point(Math.Max(0, saveHost.ClientSize.Width - _btnSave.Width), 10);
            };
            saveHost.Controls.Add(_btnSave);

            scroll.Controls.Add(saveHost);
            scroll.Controls.Add(table);
            panel.Controls.Add(scroll);
            return panel;
        }

        private Panel BuildDatabasePanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(14, 12, 14, 12),
                Margin = new Padding(4)
            };

            var scroll = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            var lblTitle = new Label
            {
                Dock = DockStyle.Top,
                Height = 34,
                Text = "Database",
                Font = new Font("Segoe UI Semibold", 16F, FontStyle.Regular)
            };

            var lblHint = new Label
            {
                Dock = DockStyle.Top,
                Height = 28,
                Text = "Configure Local, Online, or Network MySQL connection profiles.",
                ForeColor = ThemeManager.Colors.TextSecondary
            };

            var actions = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 42,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0),
                Margin = new Padding(0, 0, 0, 8)
            };

            _btnDbLoadProfile = new Button { Text = "Load Profile", Width = 110, Height = 34, Margin = new Padding(0, 0, 8, 0) };
            _btnDbSaveProfile = new Button { Text = "Save Profile", Width = 110, Height = 34, Margin = new Padding(0, 0, 8, 0) };
            _btnDbTestConnection = new Button { Text = "Test Connection", Width = 120, Height = 34, Margin = new Padding(0, 0, 8, 0) };
            _btnDbApplyRuntime = new Button { Text = "Apply Runtime", Width = 120, Height = 34, Margin = new Padding(0) };

            _btnDbLoadProfile.Click += btnDbLoadProfile_Click;
            _btnDbSaveProfile.Click += btnDbSaveProfile_Click;
            _btnDbTestConnection.Click += btnDbTestConnection_Click;
            _btnDbApplyRuntime.Click += btnDbApplyRuntime_Click;

            actions.Controls.Add(_btnDbLoadProfile);
            actions.Controls.Add(_btnDbSaveProfile);
            actions.Controls.Add(_btnDbTestConnection);
            actions.Controls.Add(_btnDbApplyRuntime);

            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 2,
                RowCount = 0,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(0)
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            _cboDbMode = new ComboBox
            {
                Dock = DockStyle.Top,
                Height = 30,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cboDbMode.Items.AddRange(new object[]
            {
                DatabaseConnectionProfiles.LocalMode,
                DatabaseConnectionProfiles.OnlineMode,
                DatabaseConnectionProfiles.NetworkMode
            });

            _cboDbTransport = new ComboBox
            {
                Dock = DockStyle.Top,
                Height = 30,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cboDbTransport.Items.AddRange(new object[] { "Wired", "Wireless" });

            _txtDbProfileKey = CreateTextBox();
            _txtDbHost = CreateTextBox();
            _numDbPort = CreateNumeric(1, 65535, 3306);
            _txtDbName = CreateTextBox();
            _txtDbUsername = CreateTextBox();
            _txtDbPassword = CreateTextBox();
            _txtDbPassword.UseSystemPasswordChar = true;
            _chkDbSetActiveProfile = new CheckBox
            {
                Text = "Set as active profile for selected mode",
                AutoSize = true
            };
            _txtBootstrapConnection = CreateTextBox();

            _cboDbMode.SelectedIndexChanged += (_, __) => UpdateBootstrapConnectionPreview();
            _cboDbTransport.SelectedIndexChanged += (_, __) => UpdateBootstrapConnectionPreview();
            _txtDbProfileKey.TextChanged += (_, __) => UpdateBootstrapConnectionPreview();
            _txtDbHost.TextChanged += (_, __) => UpdateBootstrapConnectionPreview();
            _numDbPort.ValueChanged += (_, __) => UpdateBootstrapConnectionPreview();
            _txtDbName.TextChanged += (_, __) => UpdateBootstrapConnectionPreview();
            _txtDbUsername.TextChanged += (_, __) => UpdateBootstrapConnectionPreview();
            _txtDbPassword.TextChanged += (_, __) => UpdateBootstrapConnectionPreview();

            AddRow(table, "Mode", _cboDbMode);
            AddRow(table, "Transport", _cboDbTransport);
            AddRow(table, "Profile Key", _txtDbProfileKey);
            AddRow(table, "Host / IP", _txtDbHost);
            AddRow(table, "Port", _numDbPort);
            AddRow(table, "Database Name", _txtDbName);
            AddRow(table, "Username", _txtDbUsername);
            AddRow(table, "Password", _txtDbPassword);
            AddRow(table, "Profile", _chkDbSetActiveProfile);
            AddRow(table, "Bootstrap Connection", _txtBootstrapConnection);

            _lblDbStatus = new Label
            {
                Dock = DockStyle.Top,
                Height = 26,
                ForeColor = ThemeManager.Colors.TextSecondary,
                Text = "No profile found. Fill values and save profile."
            };

            scroll.Controls.Add(table);
            scroll.Controls.Add(actions);
            scroll.Controls.Add(_lblDbStatus);
            scroll.Controls.Add(lblHint);
            scroll.Controls.Add(lblTitle);
            panel.Controls.Add(scroll);
            return panel;
        }

        private static TextBox CreateTextBox()
        {
            return new TextBox
            {
                Dock = DockStyle.Top,
                Margin = new Padding(0),
                Height = 30
            };
        }

        private static NumericUpDown CreateNumeric(decimal min, decimal max, decimal value)
        {
            return new NumericUpDown
            {
                Dock = DockStyle.Top,
                Minimum = min,
                Maximum = max,
                Value = value,
                Height = 30,
                DecimalPlaces = 0
            };
        }

        private static void AddRow(TableLayoutPanel table, string label, Control input)
        {
            var row = table.RowCount++;
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));

            var lbl = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = label
            };

            var host = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 6, 0, 0)
            };
            input.Margin = new Padding(0);
            host.Controls.Add(input);

            table.Controls.Add(lbl, 0, row);
            table.Controls.Add(host, 1, row);
        }

        private void LoadSavedValues()
        {
            var profile = AppSettingsStore.Load();
            _txtCompanyName.Text = profile.CompanyName;
            _txtAddress.Text = profile.Address;
            _txtPhone.Text = profile.Phone;
            _txtEmail.Text = profile.Email;
            _numDailyRate.Value = ClampNumeric(_numDailyRate, profile.DefaultDailyRate);
            _numLateFee.Value = ClampNumeric(_numLateFee, profile.LateFeePerDay);
            _numTaxRate.Value = ClampNumeric(_numTaxRate, profile.TaxRatePercent);
            _txtBackupPath.Text = profile.BackupPath;
            _txtDumpPath.Text = profile.MySqlDumpPath;
            _txtSmtpHost.Text = profile.SmtpHost;
            _numSmtpPort.Value = ClampNumeric(_numSmtpPort, profile.SmtpPort);
            _txtSmtpUser.Text = string.IsNullOrWhiteSpace(profile.SmtpUser) ? _user.Username : profile.SmtpUser;
            _txtSmtpPassword.Text = profile.SmtpPassword;
            _chkSsl.Checked = profile.EnableSsl;
            _chkDarkMode.Checked = profile.EnableDarkMode;
            LoadDatabaseProfile(profile);
        }

        private void LoadDatabaseProfile(AppSettingsProfile profile)
        {
            if (_cboDbMode == null)
            {
                return;
            }

            var mode = DatabaseConnectionProfiles.NormalizeMode(profile.DatabaseMode);
            _cboDbMode.SelectedItem = mode;
            if (_cboDbMode.SelectedIndex < 0)
            {
                _cboDbMode.SelectedIndex = 0;
            }

            var transport = string.IsNullOrWhiteSpace(profile.DatabaseTransport) ? "Wired" : profile.DatabaseTransport.Trim();
            _cboDbTransport.SelectedItem = transport;
            if (_cboDbTransport.SelectedIndex < 0)
            {
                _cboDbTransport.SelectedIndex = 0;
            }

            _txtDbProfileKey.Text = profile.DbProfileKey ?? string.Empty;
            var preset = DatabaseConnectionProfiles.CreatePreset(mode);
            _txtDbHost.Text = string.IsNullOrWhiteSpace(profile.DatabaseHost) ? preset.Host : profile.DatabaseHost;
            _numDbPort.Value = ClampNumeric(_numDbPort, profile.DatabasePort);
            _txtDbName.Text = string.IsNullOrWhiteSpace(profile.DatabaseName) ? preset.DatabaseName : profile.DatabaseName;
            _txtDbUsername.Text = string.IsNullOrWhiteSpace(profile.DatabaseUsername) ? preset.Username : profile.DatabaseUsername;
            _txtDbPassword.Text = profile.DatabasePassword ?? string.Empty;
            _chkDbSetActiveProfile.Checked = profile.DatabaseSetActiveProfile;

            if (!string.IsNullOrWhiteSpace(profile.BootstrapConnection))
            {
                ApplyConnectionStringToFields(profile.BootstrapConnection);
                _txtBootstrapConnection.Text = profile.BootstrapConnection;
            }
            else
            {
                UpdateBootstrapConnectionPreview();
            }

            if (_lblDbStatus != null)
            {
                _lblDbStatus.Text = "Profile loaded.";
            }
        }

        private void ApplyConnectionStringToFields(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return;
            }

            try
            {
                var builder = new MySqlConnectionStringBuilder(connectionString);
                if (!string.IsNullOrWhiteSpace(builder.Server))
                {
                    _txtDbHost.Text = builder.Server;
                }

                if (builder.Port > 0)
                {
                    _numDbPort.Value = ClampNumeric(_numDbPort, builder.Port);
                }

                if (!string.IsNullOrWhiteSpace(builder.Database))
                {
                    _txtDbName.Text = builder.Database;
                }

                if (!string.IsNullOrWhiteSpace(builder.UserID))
                {
                    _txtDbUsername.Text = builder.UserID;
                }

                _txtDbPassword.Text = builder.Password ?? string.Empty;
            }
            catch
            {
                // Keep editable values if parsing fails.
            }
        }

        private string BuildConnectionStringFromFields()
        {
            var selectedMode = DatabaseConnectionProfiles.NormalizeMode(_cboDbMode?.SelectedItem?.ToString());
            var preset = DatabaseConnectionProfiles.CreatePreset(selectedMode);
            var builder = new MySqlConnectionStringBuilder
            {
                Server = string.IsNullOrWhiteSpace(_txtDbHost.Text) ? preset.Host : _txtDbHost.Text.Trim(),
                Port = Convert.ToUInt32(_numDbPort.Value),
                Database = string.IsNullOrWhiteSpace(_txtDbName.Text) ? preset.DatabaseName : _txtDbName.Text.Trim(),
                UserID = string.IsNullOrWhiteSpace(_txtDbUsername.Text) ? preset.Username : _txtDbUsername.Text.Trim(),
                Password = _txtDbPassword.Text ?? string.Empty,
                Pooling = true,
                CharacterSet = "utf8mb4",
                AllowPublicKeyRetrieval = true
            };

            return builder.ConnectionString;
        }

        private void UpdateBootstrapConnectionPreview()
        {
            if (_txtBootstrapConnection == null)
            {
                return;
            }

            _txtBootstrapConnection.Text = BuildConnectionStringFromFields();
        }

        private void ActivateSystemTab()
        {
            ThemeManager.StyleButton(_btnTabSystem, ThemeButtonKind.Primary);
            ThemeManager.StyleButton(_btnTabDatabase, ThemeButtonKind.Secondary);
            ThemeManager.StyleButton(_btnTabUsers, ThemeButtonKind.Secondary);
            ThemeManager.StyleButton(_btnTabAudit, ThemeButtonKind.Secondary);
            SwitchContent(_pnlSystem);
        }

        private void ActivateDatabaseTab()
        {
            ThemeManager.StyleButton(_btnTabSystem, ThemeButtonKind.Secondary);
            ThemeManager.StyleButton(_btnTabDatabase, ThemeButtonKind.Primary);
            ThemeManager.StyleButton(_btnTabUsers, ThemeButtonKind.Secondary);
            ThemeManager.StyleButton(_btnTabAudit, ThemeButtonKind.Secondary);
            SwitchContent(_pnlDatabase);
        }

        private void ActivateUsersTab()
        {
            ThemeManager.StyleButton(_btnTabSystem, ThemeButtonKind.Secondary);
            ThemeManager.StyleButton(_btnTabDatabase, ThemeButtonKind.Secondary);
            ThemeManager.StyleButton(_btnTabUsers, ThemeButtonKind.Primary);
            ThemeManager.StyleButton(_btnTabAudit, ThemeButtonKind.Secondary);

            if (_usersModule == null || _usersModule.IsDisposed)
            {
                _usersModule = new ucUsers { Dock = DockStyle.Fill };
            }

            SwitchContent(_usersModule);
        }

        private void SwitchContent(Control content)
        {
            if (content == null || _pnlContentHost == null || _activeContent == content)
            {
                return;
            }

            _pnlContentHost.SuspendLayout();
            _pnlContentHost.Controls.Clear();
            content.Dock = DockStyle.Fill;
            _pnlContentHost.Controls.Add(content);
            _activeContent = content;
            _pnlContentHost.ResumeLayout();
        }

        private void btnTabUsers_Click(object sender, EventArgs e)
        {
            ActivateUsersTab();
        }

        private void btnTabAudit_Click(object sender, EventArgs e)
        {
            ActivateAuditTab();
        }

        private void ActivateAuditTab()
        {
            ThemeManager.StyleButton(_btnTabSystem, ThemeButtonKind.Secondary);
            ThemeManager.StyleButton(_btnTabDatabase, ThemeButtonKind.Secondary);
            ThemeManager.StyleButton(_btnTabUsers, ThemeButtonKind.Secondary);
            ThemeManager.StyleButton(_btnTabAudit, ThemeButtonKind.Primary);

            if (_auditLogsModule == null || _auditLogsModule.IsDisposed)
            {
                _auditLogsModule = new ucAuditLogs { Dock = DockStyle.Fill };
            }

            SwitchContent(_auditLogsModule);
        }

        private void btnDbLoadProfile_Click(object sender, EventArgs e)
        {
            var profile = AppSettingsStore.Load();
            LoadDatabaseProfile(profile);
        }

        private void btnDbSaveProfile_Click(object sender, EventArgs e)
        {
            try
            {
                var profile = AppSettingsStore.Load();
                profile.DatabaseMode = _cboDbMode.SelectedItem?.ToString() ?? "Local";
                profile.DatabaseTransport = _cboDbTransport.SelectedItem?.ToString() ?? "Wired";
                profile.DbProfileKey = _txtDbProfileKey.Text.Trim();
                profile.DatabaseHost = _txtDbHost.Text.Trim();
                profile.DatabasePort = Convert.ToInt32(_numDbPort.Value);
                profile.DatabaseName = _txtDbName.Text.Trim();
                profile.DatabaseUsername = _txtDbUsername.Text.Trim();
                profile.DatabasePassword = _txtDbPassword.Text;
                profile.DatabaseSetActiveProfile = _chkDbSetActiveProfile.Checked;
                profile.BootstrapConnection = BuildConnectionStringFromFields();

                AppSettingsStore.Save(profile);
                _txtBootstrapConnection.Text = profile.BootstrapConnection;
                _lblDbStatus.Text = "Profile saved.";
                MessageBox.Show("Database profile saved.", "Database", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                _lblDbStatus.Text = "Save failed.";
                MessageBox.Show($"Unable to save database profile: {ex.Message}", "Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDbTestConnection_Click(object sender, EventArgs e)
        {
            try
            {
                UseWaitCursor = true;
                var connectionString = BuildConnectionStringFromFields();
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(true);
                }

                _lblDbStatus.Text = "Connection test succeeded.";
                MessageBox.Show("Database connection is successful.", "Database", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                _lblDbStatus.Text = "Connection test failed.";
                MessageBox.Show($"Unable to connect: {ex.Message}", "Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void btnDbApplyRuntime_Click(object sender, EventArgs e)
        {
            try
            {
                var connectionString = BuildConnectionStringFromFields();
                DatabaseConnection.SetRuntimeConnectionString(connectionString);
                _txtBootstrapConnection.Text = connectionString;

                if (_chkDbSetActiveProfile.Checked)
                {
                    var profile = AppSettingsStore.Load();
                    profile.DatabaseMode = _cboDbMode.SelectedItem?.ToString() ?? "Local";
                    profile.DatabaseTransport = _cboDbTransport.SelectedItem?.ToString() ?? "Wired";
                    profile.DbProfileKey = _txtDbProfileKey.Text.Trim();
                    profile.DatabaseHost = _txtDbHost.Text.Trim();
                    profile.DatabasePort = Convert.ToInt32(_numDbPort.Value);
                    profile.DatabaseName = _txtDbName.Text.Trim();
                    profile.DatabaseUsername = _txtDbUsername.Text.Trim();
                    profile.DatabasePassword = _txtDbPassword.Text;
                    profile.DatabaseSetActiveProfile = true;
                    profile.BootstrapConnection = connectionString;
                    AppSettingsStore.Save(profile);
                }

                _lblDbStatus.Text = "Runtime connection applied.";
                MessageBox.Show("Runtime database profile applied successfully.", "Database", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                _lblDbStatus.Text = "Runtime apply failed.";
                MessageBox.Show($"Unable to apply runtime profile: {ex.Message}", "Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBackupNow_Click(object sender, EventArgs e)
        {
            OpenBackupRestoreDialog();
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            OpenBackupRestoreDialog();
        }

        private void OpenBackupRestoreDialog()
        {
            using (var dialog = new frmBackupRestore())
            {
                IWin32Window owner = FindForm();
                if (owner == null)
                {
                    owner = this;
                }

                dialog.ShowDialog(owner);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var profile = new AppSettingsProfile
                {
                    CompanyName = _txtCompanyName.Text.Trim(),
                    Address = _txtAddress.Text.Trim(),
                    Phone = _txtPhone.Text.Trim(),
                    Email = _txtEmail.Text.Trim(),
                    DefaultDailyRate = _numDailyRate.Value,
                    LateFeePerDay = _numLateFee.Value,
                    TaxRatePercent = _numTaxRate.Value,
                    BackupPath = _txtBackupPath.Text.Trim(),
                    MySqlDumpPath = _txtDumpPath.Text.Trim(),
                    SmtpHost = _txtSmtpHost.Text.Trim(),
                    SmtpPort = Convert.ToInt32(_numSmtpPort.Value),
                    SmtpUser = _txtSmtpUser.Text.Trim(),
                    SmtpPassword = _txtSmtpPassword.Text,
                    EnableSsl = _chkSsl.Checked,
                    EnableDarkMode = _chkDarkMode.Checked,
                    DatabaseMode = _cboDbMode?.SelectedItem?.ToString() ?? "Local",
                    DatabaseTransport = _cboDbTransport?.SelectedItem?.ToString() ?? "Wired",
                    DbProfileKey = _txtDbProfileKey.Text.Trim(),
                    DatabaseHost = _txtDbHost.Text.Trim(),
                    DatabasePort = Convert.ToInt32(_numDbPort.Value),
                    DatabaseName = _txtDbName.Text.Trim(),
                    DatabaseUsername = _txtDbUsername.Text.Trim(),
                    DatabasePassword = _txtDbPassword.Text,
                    DatabaseSetActiveProfile = _chkDbSetActiveProfile.Checked,
                    BootstrapConnection = _txtBootstrapConnection.Text.Trim()
                };

                AppSettingsStore.Save(profile);
                MessageBox.Show("System settings saved successfully. Re-open modules to apply updated branding labels.", "Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to save settings: {ex.Message}", "Settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyTheme()
        {
            ThemeManager.ApplyControlTheme(this);
            ThemeManager.StyleCardPanel(_pnlSystem);
            ThemeManager.StyleCardPanel(_pnlDatabase);
            ThemeManager.StyleButton(_btnTabSystem, ThemeButtonKind.Primary);
            ThemeManager.StyleButton(_btnTabDatabase, ThemeButtonKind.Secondary);
            ThemeManager.StyleButton(_btnTabUsers, ThemeButtonKind.Secondary);
            ThemeManager.StyleButton(_btnTabAudit, ThemeButtonKind.Secondary);
            ThemeManager.StyleButton(_btnBackupNow, ThemeButtonKind.Secondary);
            ThemeManager.StyleButton(_btnRestore, ThemeButtonKind.Secondary);
            ThemeManager.StyleButton(_btnSave, ThemeButtonKind.Primary);
            ThemeManager.StyleButton(_btnDbLoadProfile, ThemeButtonKind.Secondary);
            ThemeManager.StyleButton(_btnDbSaveProfile, ThemeButtonKind.Primary);
            ThemeManager.StyleButton(_btnDbTestConnection, ThemeButtonKind.Secondary);
            ThemeManager.StyleButton(_btnDbApplyRuntime, ThemeButtonKind.Primary);

            ThemeManager.StyleTextBox(_txtCompanyName);
            ThemeManager.StyleTextBox(_txtAddress);
            ThemeManager.StyleTextBox(_txtPhone);
            ThemeManager.StyleTextBox(_txtEmail);
            ThemeManager.StyleTextBox(_txtBackupPath);
            ThemeManager.StyleTextBox(_txtDumpPath);
            ThemeManager.StyleTextBox(_txtSmtpHost);
            ThemeManager.StyleTextBox(_txtSmtpUser);
            ThemeManager.StyleTextBox(_txtSmtpPassword);
            ThemeManager.StyleTextBox(_txtDbProfileKey);
            ThemeManager.StyleTextBox(_txtDbHost);
            ThemeManager.StyleTextBox(_txtDbName);
            ThemeManager.StyleTextBox(_txtDbUsername);
            ThemeManager.StyleTextBox(_txtDbPassword);
            ThemeManager.StyleTextBox(_txtBootstrapConnection);
            ThemeManager.StyleNumericUpDown(_numDailyRate);
            ThemeManager.StyleNumericUpDown(_numLateFee);
            ThemeManager.StyleNumericUpDown(_numTaxRate);
            ThemeManager.StyleNumericUpDown(_numSmtpPort);
            ThemeManager.StyleNumericUpDown(_numDbPort);
            ThemeManager.StyleComboBox(_cboDbMode);
            ThemeManager.StyleComboBox(_cboDbTransport);
            ThemeManager.StyleCheckBox(_chkSsl);
            ThemeManager.StyleCheckBox(_chkDarkMode);
            ThemeManager.StyleCheckBox(_chkDbSetActiveProfile);
        }

        private static decimal ClampNumeric(NumericUpDown numeric, decimal value)
        {
            if (value < numeric.Minimum)
            {
                return numeric.Minimum;
            }

            if (value > numeric.Maximum)
            {
                return numeric.Maximum;
            }

            return value;
        }
    }
}
