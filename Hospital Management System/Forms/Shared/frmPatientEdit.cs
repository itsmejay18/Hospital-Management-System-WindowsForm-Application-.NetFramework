using System;
using System.Windows.Forms;
using HospitalManagementSystem.BLL.Services;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.Forms.Shared
{
    public partial class frmPatientEdit : Form
    {
        private readonly PatientService _service = new PatientService();
        private readonly Patient _patient;

        public frmPatientEdit()
            : this(new Patient())
        {
        }

        public frmPatientEdit(Patient patient)
        {
            InitializeComponent();
            _patient = patient;
            BindData();
        }

        private void BindData()
        {
            txtCode.Text = _patient.PatientCode;
            txtFirstName.Text = _patient.FirstName;
            txtLastName.Text = _patient.LastName;
            cboGender.Text = _patient.Gender;
            dtpDob.Value = _patient.DateOfBirth == default ? DateTime.Today : _patient.DateOfBirth;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _patient.PatientCode = string.IsNullOrWhiteSpace(txtCode.Text) ? _patient.PatientCode : txtCode.Text.Trim();
                _patient.FirstName = txtFirstName.Text.Trim();
                _patient.LastName = txtLastName.Text.Trim();
                _patient.Gender = cboGender.Text;
                _patient.DateOfBirth = dtpDob.Value.Date;

                if (_patient.PatientID == 0)
                {
                    if (string.IsNullOrWhiteSpace(_patient.PatientCode))
                    {
                        _patient.PatientCode = $"P-{DateTime.Now:yyyyMMddHHmmss}";
                    }

                    await _service.AddAsync(_patient).ConfigureAwait(true);
                }
                else
                {
                    await _service.UpdateAsync(_patient).ConfigureAwait(true);
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
