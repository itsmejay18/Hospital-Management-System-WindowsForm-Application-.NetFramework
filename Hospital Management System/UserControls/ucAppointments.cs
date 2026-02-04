using System;
using System.ComponentModel;
using System.Windows.Forms;
using HospitalManagementSystem.BLL.Services;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.UserControls
{
    public partial class ucAppointments : UserControl
    {
        private readonly BindingList<Appointment> _appointments = new BindingList<Appointment>();
        private readonly AppointmentService _service = new AppointmentService();

        public ucAppointments()
        {
            InitializeComponent();
            dgvAppointments.AutoGenerateColumns = false;
            dgvAppointments.DataSource = _appointments;
            Load += ucAppointments_Load;
            btnAdd.Click += btnAdd_Click;
            btnEdit.Click += btnEdit_Click;
            btnDelete.Click += btnDelete_Click;
        }

        private async void ucAppointments_Load(object sender, EventArgs e)
        {
            try
            {
                UseWaitCursor = true;
                _appointments.Clear();
                var list = await _service.GetAllAsync().ConfigureAwait(true);
                foreach (var item in list)
                {
                    _appointments.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load appointments: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var dlg = new Forms.Shared.frmAppointmentEdit())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _ = LoadAppointmentsAsync();
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvAppointments.CurrentRow?.DataBoundItem is Appointment appt)
            {
                using (var dlg = new Forms.Shared.frmAppointmentEdit(appt))
                {
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        _ = LoadAppointmentsAsync();
                    }
                }
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvAppointments.CurrentRow?.DataBoundItem is Appointment appt)
            {
                var confirm = MessageBox.Show("Delete this appointment?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm != DialogResult.Yes)
                {
                    return;
                }

                await _service.DeleteAsync(appt.AppointmentID).ConfigureAwait(true);
                await LoadAppointmentsAsync().ConfigureAwait(true);
            }
        }

        private async System.Threading.Tasks.Task LoadAppointmentsAsync()
        {
            try
            {
                UseWaitCursor = true;
                _appointments.Clear();
                var list = await _service.GetAllAsync().ConfigureAwait(true);
                foreach (var item in list)
                {
                    _appointments.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load appointments: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }
    }
}
