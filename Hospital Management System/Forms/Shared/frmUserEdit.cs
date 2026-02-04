using System;
using System.Windows.Forms;
using HospitalManagementSystem.BLL.Services;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.Forms.Shared
{
    public partial class frmUserEdit : Form
    {
        private readonly UserService _service = new UserService();
        private readonly User _user;

        public frmUserEdit()
            : this(new User())
        {
        }

        public frmUserEdit(User user)
        {
            InitializeComponent();
            _user = user;
            BindData();
        }

        private void BindData()
        {
            txtUsername.Text = _user.Username;
            txtEmail.Text = _user.Email;
            txtRoleId.Text = _user.RoleID == 0 ? string.Empty : _user.RoleID.ToString();
            chkActive.Checked = _user.IsActive;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _user.Username = txtUsername.Text.Trim();
                _user.Email = txtEmail.Text.Trim();
                _user.RoleID = int.TryParse(txtRoleId.Text.Trim(), out var roleId) ? roleId : 1;
                _user.IsActive = chkActive.Checked;

                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    if (_user.UserID == 0)
                    {
                        MessageBox.Show("Password is required for new users.", "Validation",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else
                {
                    // NOTE: Replace with hashing when you implement AuthenticationService.
                    _user.PasswordHash = txtPassword.Text.Trim();
                }

                if (_user.UserID == 0)
                {
                    await _service.AddAsync(_user).ConfigureAwait(true);
                }
                else
                {
                    await _service.UpdateAsync(_user).ConfigureAwait(true);
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Save failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
