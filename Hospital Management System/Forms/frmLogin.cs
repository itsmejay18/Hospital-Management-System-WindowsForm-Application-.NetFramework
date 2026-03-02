using System;
using System.Windows.Forms;
using HospitalManagementSystem.BLL.Services;
using HospitalManagementSystem.Helpers;

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
                lblStatus.Text = "Connecting to MySQL...";
                await TestDatabaseConnectionAsync().ConfigureAwait(true);
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

        private static async System.Threading.Tasks.Task TestDatabaseConnectionAsync()
        {
            using (var connection = await DAL.DatabaseConnection.Instance.OpenConnectionAsync().ConfigureAwait(false))
            {
            }
        }
    }
}
