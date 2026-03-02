using System;
using System.Drawing;
using System.Windows.Forms;

namespace HospitalManagementSystem.Forms
{
    public sealed class InstallerForm : Form
    {
        private TextBox _txtServer;
        private NumericUpDown _numPort;
        private TextBox _txtDatabase;
        private TextBox _txtUsername;
        private TextBox _txtPassword;
        private Label _lblStatus;
        private Button _btnTestConnection;
        private Button _btnInstall;

        public InstallerForm()
        {
            BuildLayout();
            LoadDefaults();
        }

        private void BuildLayout()
        {
            Text = "First-Run Installer";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(560, 420);

            var lblTitle = new Label
            {
                AutoSize = false,
                Location = new Point(16, 12),
                Size = new Size(528, 30),
                Font = new Font("Segoe UI Semibold", 13F, FontStyle.Regular),
                Text = "Hospital Management System - Installer"
            };

            var lblSub = new Label
            {
                AutoSize = false,
                Location = new Point(16, 44),
                Size = new Size(528, 32),
                Text = "Configure MySQL and install full schema on first run."
            };

            var table = new TableLayoutPanel
            {
                Location = new Point(16, 84),
                Size = new Size(528, 208),
                ColumnCount = 2,
                RowCount = 5
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            for (var i = 0; i < 5; i++)
            {
                table.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            }

            _txtServer = new TextBox { Dock = DockStyle.Fill };
            _numPort = new NumericUpDown
            {
                Dock = DockStyle.Left,
                Width = 120,
                Minimum = 1,
                Maximum = 65535,
                Value = 3306
            };
            _txtDatabase = new TextBox { Dock = DockStyle.Fill };
            _txtUsername = new TextBox { Dock = DockStyle.Fill };
            _txtPassword = new TextBox { Dock = DockStyle.Fill, UseSystemPasswordChar = true };

            table.Controls.Add(CreateLabel("Server"), 0, 0);
            table.Controls.Add(_txtServer, 1, 0);
            table.Controls.Add(CreateLabel("Port"), 0, 1);
            table.Controls.Add(_numPort, 1, 1);
            table.Controls.Add(CreateLabel("Database Name"), 0, 2);
            table.Controls.Add(_txtDatabase, 1, 2);
            table.Controls.Add(CreateLabel("Username"), 0, 3);
            table.Controls.Add(_txtUsername, 1, 3);
            table.Controls.Add(CreateLabel("Password"), 0, 4);
            table.Controls.Add(_txtPassword, 1, 4);

            _btnTestConnection = new Button
            {
                Text = "Test Connection",
                Size = new Size(132, 34),
                Location = new Point(16, 304)
            };
            _btnTestConnection.Click += btnTestConnection_Click;

            _btnInstall = new Button
            {
                Text = "Install",
                Size = new Size(132, 34),
                Location = new Point(412, 304)
            };
            _btnInstall.Click += btnInstall_Click;

            _lblStatus = new Label
            {
                AutoSize = false,
                Location = new Point(16, 350),
                Size = new Size(528, 58),
                Text = "Enter connection details then click Test Connection."
            };

            var lblInfo = new Label
            {
                AutoSize = false,
                Location = new Point(16, 336),
                Size = new Size(528, 18),
                Text = "Default SuperAdmin user will be created automatically."
            };

            Controls.Add(lblTitle);
            Controls.Add(lblSub);
            Controls.Add(table);
            Controls.Add(_btnTestConnection);
            Controls.Add(_btnInstall);
            Controls.Add(lblInfo);
            Controls.Add(_lblStatus);

            AcceptButton = _btnInstall;
        }

        private static Label CreateLabel(string text)
        {
            return new Label
            {
                Text = text,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill
            };
        }

        private void LoadDefaults()
        {
            var defaults = InstallationManager.LoadSuggestedOptions();
            _txtServer.Text = defaults.Server;
            _numPort.Value = defaults.Port > 0 ? defaults.Port : 3306;
            _txtDatabase.Text = defaults.DatabaseName;
            _txtUsername.Text = defaults.Username;
            _txtPassword.Text = defaults.Password;
        }

        private InstallationOptions BuildOptions()
        {
            return new InstallationOptions
            {
                Server = (_txtServer.Text ?? string.Empty).Trim(),
                Port = Convert.ToInt32(_numPort.Value),
                DatabaseName = (_txtDatabase.Text ?? string.Empty).Trim(),
                Username = (_txtUsername.Text ?? string.Empty).Trim(),
                Password = _txtPassword.Text ?? string.Empty
            };
        }

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            var options = BuildOptions();
            if (InstallationManager.TestConnection(options, out var message))
            {
                _lblStatus.Text = message;
                MessageBox.Show(message, "Installer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _lblStatus.Text = $"Connection failed: {message}";
            MessageBox.Show($"Connection failed: {message}", "Installer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            try
            {
                UseWaitCursor = true;
                Enabled = false;
                _lblStatus.Text = "Installing full schema. Please wait...";

                var options = BuildOptions();
                InstallationManager.Install(options, InstallationManager.DefaultSuperAdminPassword);

                _lblStatus.Text = "Installation completed successfully.";
                MessageBox.Show(
                    $"Installation complete.\r\n\r\nSuperAdmin Username: {InstallationManager.SuperAdminUsername}\r\nSuperAdmin Password: {InstallationManager.DefaultSuperAdminPassword}",
                    "Installer",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                _lblStatus.Text = $"Installation failed: {ex.Message}";
                MessageBox.Show($"Installation failed: {ex.Message}", "Installer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Enabled = true;
                UseWaitCursor = false;
            }
        }
    }
}
