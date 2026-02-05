using System;
using System.Windows.Forms;
using HospitalManagementSystem.BLL.Services;
using HospitalManagementSystem.Helpers;

namespace HospitalManagementSystem.Forms
{
    public partial class frmLogin : Form
    {
        private readonly AuthenticationService _authenticationService = new AuthenticationService();

        public frmLogin()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            txtPassword.UseSystemPasswordChar = true;
            lblStatus.Text = "Ready";
            AcceptButton = btnLogin;
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
                var authenticatedUser = await _authenticationService
                    .LoginAsync(txtUsername.Text.Trim(), txtPassword.Text)
                    .ConfigureAwait(true);
                if (authenticatedUser == null)
                {
                    MessageBox.Show("Invalid credentials.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                UserSession.Start(authenticatedUser);
                var main = new frmMain(authenticatedUser);
                Hide();
                main.FormClosed += (_, __) => Close();
                main.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        }
    }
}
