using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HospitalManagementSystem.BLL.Services;
using HospitalManagementSystem.Helpers;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.Forms.Shared
{
    public partial class frmDoctorEdit : Form
    {
        private readonly DoctorService _service = new DoctorService();
        private readonly UserService _userService = new UserService();
        private readonly Doctor _doctor;
        private List<User> _doctorUsers = new List<User>();
        private List<Specialization> _specializations = new List<Specialization>();

        public frmDoctorEdit()
            : this(new Doctor())
        {
        }

        public frmDoctorEdit(Doctor doctor)
        {
            InitializeComponent();
            ThemeManager.ApplyFormTheme(this);
            _doctor = doctor;
            Load += frmDoctorEdit_Load;
            BindData();
        }

        private async void frmDoctorEdit_Load(object sender, EventArgs e)
        {
            try
            {
                _specializations = await _service.GetSpecializationsAsync().ConfigureAwait(true);
                cboSpecialization.DataSource = _specializations;
                cboSpecialization.DisplayMember = "SpecializationName";
                cboSpecialization.ValueMember = "SpecializationID";
                cboSpecialization.SelectedIndex = -1;

                var users = await _userService.GetAllAsync().ConfigureAwait(true);
                var roles = await _userService.GetRolesAsync().ConfigureAwait(true);
                var doctorRoleId = roles.FirstOrDefault(r => string.Equals(r.RoleName, "Doctor", StringComparison.OrdinalIgnoreCase))?.RoleID;
                var doctors = await _service.GetAllAsync().ConfigureAwait(true);
                var assignedDoctorUsers = doctors
                    .Where(d => d.UserID > 0)
                    .Select(d => d.UserID)
                    .ToHashSet();

                if (_doctor.UserID > 0)
                {
                    assignedDoctorUsers.Remove(_doctor.UserID);
                }

                _doctorUsers = users
                    .Where(u => doctorRoleId.HasValue && u.RoleID == doctorRoleId.Value && !assignedDoctorUsers.Contains(u.UserID))
                    .OrderBy(u => u.Username)
                    .ToList();

                if (_doctor.UserID > 0 && _doctorUsers.All(u => u.UserID != _doctor.UserID))
                {
                    var current = users.FirstOrDefault(u => u.UserID == _doctor.UserID);
                    if (current != null)
                    {
                        _doctorUsers.Insert(0, current);
                    }
                }

                cboUser.DataSource = _doctorUsers;
                cboUser.DisplayMember = "Username";
                cboUser.ValueMember = "UserID";
                cboUser.SelectedIndex = -1;

                if (_doctor.UserID > 0)
                {
                    cboUser.SelectedValue = _doctor.UserID;
                }

                if (_doctor.SpecializationID.HasValue)
                {
                    cboSpecialization.SelectedValue = _doctor.SpecializationID.Value;
                }

                if (_doctor.DoctorID == 0 && _doctorUsers.Count == 0)
                {
                    MessageBox.Show(
                        "No available user account with Doctor role. Create a Doctor user first in Users module.",
                        "Doctor",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Load failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BindData()
        {
            txtCode.Text = _doctor.DoctorCode;
            txtLicense.Text = _doctor.LicenseNumber;
            txtQualification.Text = _doctor.Qualification;
            numFee.Value = _doctor.ConsultationFee ?? 0m;

            if (string.IsNullOrWhiteSpace(txtCode.Text))
            {
                txtCode.Text = $"D-{DateTime.Now:yyyyMMddHHmmss}";
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _doctor.DoctorCode = txtCode.Text.Trim();
                _doctor.UserID = cboUser.SelectedValue is int userId ? userId : 0;
                _doctor.SpecializationID = cboSpecialization.SelectedValue is int specId ? (int?)specId : null;
                _doctor.LicenseNumber = txtLicense.Text.Trim();
                _doctor.Qualification = txtQualification.Text.Trim();
                _doctor.ConsultationFee = numFee.Value;
                _doctor.JoiningDate = _doctor.JoiningDate ?? DateTime.Today;

                if (string.IsNullOrWhiteSpace(_doctor.DoctorCode))
                {
                    MessageBox.Show("Doctor code is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_doctor.UserID <= 0)
                {
                    MessageBox.Show("Please select a Doctor user account.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_doctor.DoctorID == 0)
                {
                    await _service.AddAsync(_doctor).ConfigureAwait(true);
                }
                else
                {
                    await _service.UpdateAsync(_doctor).ConfigureAwait(true);
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
