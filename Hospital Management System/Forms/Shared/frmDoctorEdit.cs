using System;
using System.Windows.Forms;
using HospitalManagementSystem.BLL.Services;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.Forms.Shared
{
    public partial class frmDoctorEdit : Form
    {
        private readonly DoctorService _service = new DoctorService();
        private readonly Doctor _doctor;

        public frmDoctorEdit()
            : this(new Doctor())
        {
        }

        public frmDoctorEdit(Doctor doctor)
        {
            InitializeComponent();
            _doctor = doctor;
            BindData();
        }

        private void BindData()
        {
            txtCode.Text = _doctor.DoctorCode;
            txtUserId.Text = _doctor.UserID == 0 ? string.Empty : _doctor.UserID.ToString();
            txtSpecializationId.Text = _doctor.SpecializationID?.ToString() ?? string.Empty;
            txtLicense.Text = _doctor.LicenseNumber;
            txtQualification.Text = _doctor.Qualification;
            numFee.Value = _doctor.ConsultationFee ?? 0m;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _doctor.DoctorCode = txtCode.Text.Trim();
                _doctor.UserID = int.TryParse(txtUserId.Text.Trim(), out var userId) ? userId : 0;
                _doctor.SpecializationID = int.TryParse(txtSpecializationId.Text.Trim(), out var specId) ? (int?)specId : null;
                _doctor.LicenseNumber = txtLicense.Text.Trim();
                _doctor.Qualification = txtQualification.Text.Trim();
                _doctor.ConsultationFee = numFee.Value;

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
