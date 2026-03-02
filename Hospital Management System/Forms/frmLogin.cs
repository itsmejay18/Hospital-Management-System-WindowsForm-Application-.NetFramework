using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using HospitalManagementSystem.BLL.Services;
using HospitalManagementSystem.DAL;
using HospitalManagementSystem.Helpers;
using MySql.Data.MySqlClient;

namespace HospitalManagementSystem.Forms
{
    public partial class frmLogin : Form
    {
        private readonly AuthenticationService _authenticationService = new AuthenticationService();
        private const int CardHorizontalPadding = 36;
        private const int CardTopPadding = 18;
        private const int CardFieldGap = 12;

        public frmLogin()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            txtPassword.UseSystemPasswordChar = true;
            ApplyTheme();
            var companyName = AppSettingsStore.Load().CompanyName;
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                Text = companyName.Trim();
            }

            lblTitle.Text = "Hospital Management System";

            lblStatus.Text = "Ready";
            AcceptButton = btnLogin;
            Resize += (_, __) => ApplyResponsiveLayout();
            Shown += (_, __) => ApplyResponsiveLayout();
            ApplyResponsiveLayout();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            btnLogin.Enabled = false;
            try
            {
                if (string.IsNullOrWhiteSpace(txtUsername.Text))
                {
                    errorProvider1.SetError(txtUsername, "Username is required.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    errorProvider1.SetError(txtPassword, "Password is required.");
                    return;
                }

                errorProvider1.Clear();
                var connected = await EnsureDatabaseConnectionAsync().ConfigureAwait(true);
                if (!connected)
                {
                    lblStatus.Text = "Database connection cancelled.";
                    return;
                }

                lblStatus.Text = "Authenticating...";
                var authenticatedUser = await _authenticationService
                    .LoginAsync(txtUsername.Text.Trim(), txtPassword.Text)
                    .ConfigureAwait(true);
                if (authenticatedUser == null)
                {
                    MessageBox.Show("Invalid credentials.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    lblStatus.Text = "Authentication failed.";
                    return;
                }

                UserSession.Start(authenticatedUser);
                Hide();
                using (var main = new frmMain(authenticatedUser))
                {
                    main.ShowDialog(this);
                    if (main.LogoutRequested)
                    {
                        txtPassword.Clear();
                        lblStatus.Text = "Logged out.";
                        Show();
                        Activate();
                        txtUsername.Focus();
                        return;
                    }
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Connection or login failed.";
            }
            finally
            {
                btnLogin.Enabled = true;
            }
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        }

        private void ApplyTheme()
        {
            ThemeManager.ApplyFormTheme(this, styleChildren: false);
            ThemeManager.ApplyControlTheme(pnlContainer);
            ThemeManager.StyleCardPanel(pnlContainer, 16);
            ThemeManager.ApplyBrandingLogo(picLogo);
        }

        private void ApplyResponsiveLayout()
        {
            SuspendLayout();
            pnlContainer.SuspendLayout();

            var cardWidth = Math.Max(420, Math.Min(560, ClientSize.Width - 60));
            var cardHeight = Math.Max(430, Math.Min(500, ClientSize.Height - 70));
            pnlContainer.Size = new System.Drawing.Size(cardWidth, cardHeight);
            pnlContainer.Location = new System.Drawing.Point(
                Math.Max(20, (ClientSize.Width - cardWidth) / 2),
                Math.Max(20, (ClientSize.Height - cardHeight) / 2));

            var contentLeft = CardHorizontalPadding;
            var contentWidth = Math.Max(280, cardWidth - (CardHorizontalPadding * 2));
            var cursorY = CardTopPadding;

            var logoSize = Math.Max(84, Math.Min(116, contentWidth / 3));
            picLogo.Size = new System.Drawing.Size(logoSize, logoSize);
            picLogo.Location = new System.Drawing.Point((cardWidth - logoSize) / 2, cursorY);
            cursorY += logoSize + 8;

            lblTitle.AutoSize = false;
            lblTitle.Width = contentWidth;
            lblTitle.Height = 40;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Location = new System.Drawing.Point(contentLeft, cursorY);
            cursorY += lblTitle.Height + 8;

            lblUsername.Location = new System.Drawing.Point(contentLeft, cursorY);
            lblUsername.AutoSize = true;
            cursorY += lblUsername.Height + 5;

            txtUsername.Location = new System.Drawing.Point(contentLeft, cursorY);
            txtUsername.Width = contentWidth;
            cursorY += txtUsername.Height + CardFieldGap;

            lblPassword.Location = new System.Drawing.Point(contentLeft, cursorY);
            lblPassword.AutoSize = true;
            cursorY += lblPassword.Height + 5;

            txtPassword.Location = new System.Drawing.Point(contentLeft, cursorY);
            txtPassword.Width = contentWidth;
            cursorY += txtPassword.Height + CardFieldGap;

            chkRemember.AutoSize = true;
            chkRemember.Location = new System.Drawing.Point(contentLeft, cursorY);

            chkShowPassword.AutoSize = true;
            chkShowPassword.Location = new System.Drawing.Point(
                contentLeft + Math.Max(0, contentWidth - chkShowPassword.PreferredSize.Width),
                cursorY);
            cursorY += Math.Max(chkRemember.Height, chkShowPassword.Height) + 8;

            lnkForgot.AutoSize = true;
            lnkForgot.Location = new System.Drawing.Point(contentLeft, cursorY);
            cursorY += lnkForgot.Height + 16;

            btnLogin.Location = new System.Drawing.Point(contentLeft, cursorY);
            btnLogin.Size = new System.Drawing.Size(contentWidth, 40);
            cursorY += btnLogin.Height + 12;

            lblStatus.AutoSize = false;
            lblStatus.Width = contentWidth;
            lblStatus.Height = 18;
            lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            lblStatus.Location = new System.Drawing.Point(contentLeft, cursorY);

            pnlContainer.ResumeLayout();
            ResumeLayout();
        }

        private async Task<bool> EnsureDatabaseConnectionAsync()
        {
            while (true)
            {
                try
                {
                    lblStatus.Text = "Connecting to MySQL...";
                    await TestDatabaseConnectionAsync().ConfigureAwait(true);
                    return true;
                }
                catch (Exception ex)
                {
                    if (!ShowConnectionFallbackDialog(ex))
                    {
                        return false;
                    }

                    lblStatus.Text = "Retrying with selected connection...";
                }
            }
        }

        private bool ShowConnectionFallbackDialog(Exception exception)
        {
            var profile = AppSettingsStore.Load();
            var defaultHost = Sanitize(profile.DatabaseHost, "localhost");
            var defaultPort = profile.DatabasePort > 0 ? profile.DatabasePort : 3306;
            var defaultDatabase = Sanitize(profile.DatabaseName, "HospitalManagementSystem");
            var defaultUsername = Sanitize(profile.DatabaseUsername, "root");
            var defaultPassword = profile.DatabasePassword ?? string.Empty;
            var initialRoute = ResolveInitialRoute(profile);

            using (var dialog = new Form())
            {
                dialog.Text = "Database Connection Setup";
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.MinimizeBox = false;
                dialog.MaximizeBox = false;
                dialog.ShowInTaskbar = false;
                dialog.ClientSize = new Size(500, 400);

                var lblMessage = new Label
                {
                    AutoSize = false,
                    Location = new Point(16, 14),
                    Size = new Size(468, 48),
                    Text = "Cannot connect to MySQL host. Choose Local, Wired, or Wireless, then update connection details.",
                    TextAlign = ContentAlignment.MiddleLeft
                };

                var lblError = new Label
                {
                    AutoSize = false,
                    Location = new Point(16, 62),
                    Size = new Size(468, 40),
                    Text = $"Error: {GetRootErrorMessage(exception)}",
                    ForeColor = Color.DarkRed
                };

                var lblRoute = new Label
                {
                    AutoSize = true,
                    Location = new Point(16, 120),
                    Text = "Connection Route"
                };

                var cboRoute = new ComboBox
                {
                    Location = new Point(160, 116),
                    Size = new Size(324, 24),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                cboRoute.Items.AddRange(new object[] { "Local", "Wired", "Wireless" });
                cboRoute.SelectedItem = initialRoute;
                if (cboRoute.SelectedIndex < 0)
                {
                    cboRoute.SelectedIndex = 0;
                }

                var lblHost = new Label { AutoSize = true, Location = new Point(16, 156), Text = "Host" };
                var txtHost = new TextBox { Location = new Point(160, 152), Size = new Size(324, 23), Text = defaultHost };

                var lblPort = new Label { AutoSize = true, Location = new Point(16, 190), Text = "Port" };
                var numPort = new NumericUpDown
                {
                    Location = new Point(160, 186),
                    Size = new Size(120, 23),
                    Minimum = 1,
                    Maximum = 65535,
                    Value = defaultPort
                };

                var lblDatabase = new Label { AutoSize = true, Location = new Point(16, 224), Text = "Database" };
                var txtDatabase = new TextBox { Location = new Point(160, 220), Size = new Size(324, 23), Text = defaultDatabase };

                var lblUsername = new Label { AutoSize = true, Location = new Point(16, 258), Text = "Username" };
                var txtUsername = new TextBox { Location = new Point(160, 254), Size = new Size(324, 23), Text = defaultUsername };

                var lblPassword = new Label { AutoSize = true, Location = new Point(16, 292), Text = "Password" };
                var txtPassword = new TextBox
                {
                    Location = new Point(160, 288),
                    Size = new Size(324, 23),
                    Text = defaultPassword,
                    UseSystemPasswordChar = true
                };

                var chkSaveProfile = new CheckBox
                {
                    Location = new Point(160, 320),
                    AutoSize = true,
                    Text = "Save as active connection profile",
                    Checked = true
                };

                var btnCancel = new Button
                {
                    Text = "Cancel",
                    Location = new Point(292, 352),
                    Size = new Size(92, 30),
                    DialogResult = DialogResult.Cancel
                };
                var btnApply = new Button
                {
                    Text = "Apply && Retry",
                    Location = new Point(392, 352),
                    Size = new Size(92, 30),
                    DialogResult = DialogResult.OK
                };

                var wiredSuggestion = !string.Equals(initialRoute, "Local", StringComparison.OrdinalIgnoreCase)
                    ? defaultHost
                    : "192.168.1.10";
                var wirelessSuggestion = string.Equals(initialRoute, "Wireless", StringComparison.OrdinalIgnoreCase)
                    ? defaultHost
                    : "192.168.254.10";
                Action applyHostSuggestion = () =>
                {
                    var route = cboRoute.SelectedItem?.ToString() ?? "Local";
                    if (string.Equals(route, "Local", StringComparison.OrdinalIgnoreCase))
                    {
                        txtHost.Text = "localhost";
                        return;
                    }

                    txtHost.Text = string.Equals(route, "Wireless", StringComparison.OrdinalIgnoreCase)
                        ? wirelessSuggestion
                        : wiredSuggestion;
                };
                cboRoute.SelectedIndexChanged += (_, __) => applyHostSuggestion();
                applyHostSuggestion();

                btnApply.Click += (_, __) =>
                {
                    if (string.IsNullOrWhiteSpace(txtHost.Text))
                    {
                        MessageBox.Show("Host is required.", "Database", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dialog.DialogResult = DialogResult.None;
                        txtHost.Focus();
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(txtDatabase.Text))
                    {
                        MessageBox.Show("Database name is required.", "Database", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dialog.DialogResult = DialogResult.None;
                        txtDatabase.Focus();
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(txtUsername.Text))
                    {
                        MessageBox.Show("Database username is required.", "Database", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dialog.DialogResult = DialogResult.None;
                        txtUsername.Focus();
                        return;
                    }
                };

                dialog.Controls.Add(lblMessage);
                dialog.Controls.Add(lblError);
                dialog.Controls.Add(lblRoute);
                dialog.Controls.Add(cboRoute);
                dialog.Controls.Add(lblHost);
                dialog.Controls.Add(txtHost);
                dialog.Controls.Add(lblPort);
                dialog.Controls.Add(numPort);
                dialog.Controls.Add(lblDatabase);
                dialog.Controls.Add(txtDatabase);
                dialog.Controls.Add(lblUsername);
                dialog.Controls.Add(txtUsername);
                dialog.Controls.Add(lblPassword);
                dialog.Controls.Add(txtPassword);
                dialog.Controls.Add(chkSaveProfile);
                dialog.Controls.Add(btnCancel);
                dialog.Controls.Add(btnApply);
                dialog.AcceptButton = btnApply;
                dialog.CancelButton = btnCancel;

                var result = dialog.ShowDialog(this);
                if (result != DialogResult.OK)
                {
                    return false;
                }

                var routeValue = cboRoute.SelectedItem?.ToString() ?? "Local";
                var connectionString = BuildConnectionString(
                    txtHost.Text.Trim(),
                    Convert.ToInt32(numPort.Value),
                    txtDatabase.Text.Trim(),
                    txtUsername.Text.Trim(),
                    txtPassword.Text);

                DatabaseConnection.SetRuntimeConnectionString(connectionString);

                if (chkSaveProfile.Checked)
                {
                    profile.DatabaseMode = string.Equals(routeValue, "Local", StringComparison.OrdinalIgnoreCase)
                        ? "Local"
                        : "Network";
                    profile.DatabaseTransport = string.Equals(routeValue, "Wireless", StringComparison.OrdinalIgnoreCase)
                        ? "Wireless"
                        : "Wired";
                    profile.DatabaseHost = txtHost.Text.Trim();
                    profile.DatabasePort = Convert.ToInt32(numPort.Value);
                    profile.DatabaseName = txtDatabase.Text.Trim();
                    profile.DatabaseUsername = txtUsername.Text.Trim();
                    profile.DatabasePassword = txtPassword.Text;
                    profile.DatabaseSetActiveProfile = true;
                    profile.BootstrapConnection = connectionString;
                    try
                    {
                        AppSettingsStore.Save(profile);
                    }
                    catch (Exception saveEx)
                    {
                        MessageBox.Show(
                            $"Connection applied for this session, but profile save failed: {saveEx.Message}",
                            "Database",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                }

                return true;
            }
        }

        private static string BuildConnectionString(string host, int port, string database, string username, string password)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = Sanitize(host, "localhost"),
                Port = Convert.ToUInt32(port <= 0 ? 3306 : port),
                Database = Sanitize(database, "HospitalManagementSystem"),
                UserID = Sanitize(username, "root"),
                Password = password ?? string.Empty,
                Pooling = true,
                CharacterSet = "utf8mb4",
                AllowPublicKeyRetrieval = true
            };

            return builder.ConnectionString;
        }

        private static string ResolveInitialRoute(AppSettingsProfile profile)
        {
            if (profile == null)
            {
                return "Local";
            }

            if (string.Equals(profile.DatabaseMode, "Local", StringComparison.OrdinalIgnoreCase))
            {
                return "Local";
            }

            return string.Equals(profile.DatabaseTransport, "Wireless", StringComparison.OrdinalIgnoreCase)
                ? "Wireless"
                : "Wired";
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

            var message = current.Message;
            if (string.IsNullOrWhiteSpace(message))
            {
                return current.GetType().Name;
            }

            return message.Length > 180 ? message.Substring(0, 177) + "..." : message;
        }

        private static string Sanitize(string value, string fallback)
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        }

        private static async Task TestDatabaseConnectionAsync()
        {
            using (var connection = await DAL.DatabaseConnection.Instance.OpenConnectionAsync().ConfigureAwait(false))
            {
            }
        }
    }
}
