using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HospitalManagementSystem.BLL.Services;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.Forms.Shared
{
    public partial class frmUserEdit : Form
    {
        private readonly UserService _service = new UserService();
        private readonly User _user;
        private List<UserRole> _roles = new List<UserRole>();

        public frmUserEdit()
            : this(new User())
        {
        }

        public frmUserEdit(User user)
        {
            InitializeComponent();
            _user = user;
            Load += frmUserEdit_Load;
            BindData();
        }

        private async void frmUserEdit_Load(object sender, EventArgs e)
        {
            try
            {
                _roles = await _service.GetRolesAsync().ConfigureAwait(true);
                cboRole.DataSource = _roles;
                cboRole.DisplayMember = "RoleName";
                cboRole.ValueMember = "RoleID";

                if (_user.RoleID > 0)
                {
                    cboRole.SelectedValue = _user.RoleID;
                }
                else
                {
                    var defaultRole = _roles.FirstOrDefault(r => string.Equals(r.RoleName, "Receptionist", StringComparison.OrdinalIgnoreCase));
                    cboRole.SelectedValue = defaultRole?.RoleID ?? _roles.FirstOrDefault()?.RoleID;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to load roles: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BindData()
        {
            txtUsername.Text = _user.Username;
            txtEmail.Text = _user.Email;
            chkActive.Checked = _user.IsActive;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _user.Username = txtUsername.Text.Trim();
                _user.Email = txtEmail.Text.Trim();
                _user.RoleID = cboRole.SelectedValue is int roleId ? roleId : 1;
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
