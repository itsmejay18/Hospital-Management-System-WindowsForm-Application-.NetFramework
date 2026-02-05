using System;
using System.Collections.Generic;
using System.Windows.Forms;
using HospitalManagementSystem.BLL.Services;
using HospitalManagementSystem.Helpers;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.Forms.Shared
{
    public partial class frmAppointmentEdit : Form
    {
        private readonly AppointmentService _service = new AppointmentService();
        private readonly PatientService _patientService = new PatientService();
        private readonly DoctorService _doctorService = new DoctorService();
        private readonly Appointment _appointment;
        private List<Patient> _patients = new List<Patient>();
        private List<Doctor> _doctors = new List<Doctor>();

        public frmAppointmentEdit()
            : this(new Appointment())
        {
        }

        public frmAppointmentEdit(Appointment appointment)
        {
            InitializeComponent();
            _appointment = appointment;
            Load += frmAppointmentEdit_Load;
            BindData();
        }

        private async void frmAppointmentEdit_Load(object sender, EventArgs e)
        {
            try
            {
                _patients = await _patientService.GetAllAsync().ConfigureAwait(true);
                cboPatient.DataSource = _patients;
                cboPatient.DisplayMember = "FullName";
                cboPatient.ValueMember = "PatientID";
                cboPatient.SelectedIndex = -1;

                _doctors = await _doctorService.GetAllAsync().ConfigureAwait(true);
                cboDoctor.DataSource = _doctors;
                cboDoctor.DisplayMember = "DoctorName";
                cboDoctor.ValueMember = "DoctorID";
                cboDoctor.SelectedIndex = -1;

                if (_appointment.PatientID > 0)
                {
                    cboPatient.SelectedValue = _appointment.PatientID;
                }

                if (_appointment.DoctorID > 0)
                {
                    cboDoctor.SelectedValue = _appointment.DoctorID;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Load failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BindData()
        {
            txtCode.Text = _appointment.AppointmentCode;
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
                _appointment.PatientID = cboPatient.SelectedValue is int patientId ? patientId : 0;
                _appointment.DoctorID = cboDoctor.SelectedValue is int doctorId ? doctorId : 0;
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
                    var createdBy = UserSession.CurrentUser?.UserID ?? 1;
                    await _service.ScheduleAsync(_appointment, createdBy).ConfigureAwait(true);
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
