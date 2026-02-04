using System;
using System.Windows.Forms;
using HospitalManagementSystem.BLL.Services;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.Forms.Shared
{
    public partial class frmAppointmentEdit : Form
    {
        private readonly AppointmentService _service = new AppointmentService();
        private readonly Appointment _appointment;

        public frmAppointmentEdit()
            : this(new Appointment())
        {
        }

        public frmAppointmentEdit(Appointment appointment)
        {
            InitializeComponent();
            _appointment = appointment;
            BindData();
        }

        private void BindData()
        {
            txtCode.Text = _appointment.AppointmentCode;
            txtPatientId.Text = _appointment.PatientID == 0 ? string.Empty : _appointment.PatientID.ToString();
            txtDoctorId.Text = _appointment.DoctorID == 0 ? string.Empty : _appointment.DoctorID.ToString();
            dtpDate.Value = _appointment.AppointmentDate == default ? DateTime.Today : _appointment.AppointmentDate;
            dtpTime.Value = DateTime.Today.Add(_appointment.AppointmentTime);
            cboType.Text = _appointment.AppointmentType;
            cboStatus.Text = _appointment.Status;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _appointment.AppointmentCode = string.IsNullOrWhiteSpace(txtCode.Text)
                    ? _appointment.AppointmentCode
                    : txtCode.Text.Trim();
                _appointment.PatientID = int.TryParse(txtPatientId.Text.Trim(), out var patientId) ? patientId : 0;
                _appointment.DoctorID = int.TryParse(txtDoctorId.Text.Trim(), out var doctorId) ? doctorId : 0;
                _appointment.AppointmentDate = dtpDate.Value.Date;
                _appointment.AppointmentTime = dtpTime.Value.TimeOfDay;
                _appointment.AppointmentType = cboType.Text;
                _appointment.Status = cboStatus.Text;
                if (!_appointment.CreatedDate.HasValue)
                {
                    _appointment.CreatedDate = DateTime.Now;
                }

                if (string.IsNullOrWhiteSpace(_appointment.AppointmentCode))
                {
                    _appointment.AppointmentCode = $"APP-{DateTime.Now:yyyyMMddHHmmss}";
                }

                if (_appointment.AppointmentID == 0)
                {
                    await _service.ScheduleAsync(_appointment, 1).ConfigureAwait(true);
                }
                else
                {
                    await _service.UpdateAsync(_appointment).ConfigureAwait(true);
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
