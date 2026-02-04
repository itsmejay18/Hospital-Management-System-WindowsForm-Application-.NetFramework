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
    }
}
