using System;
using System.Windows.Forms;

namespace HospitalManagementSystem.Forms
{
    public partial class frmLogin : Form
    {
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

        private void btnLogin_Click(object sender, EventArgs e)
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

                if (txtUsername.Text.Trim().Equals("admin", StringComparison.OrdinalIgnoreCase)
                    && txtPassword.Text == "admin123")
                {
                    var main = new frmMain();
                    Hide();
                    main.FormClosed += (_, __) => Close();
                    main.Show();
                    return;
                }

                MessageBox.Show("Invalid credentials.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
