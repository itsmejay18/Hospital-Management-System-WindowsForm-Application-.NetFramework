using System;
using System.ComponentModel;
using System.Windows.Forms;
using HospitalManagementSystem.BLL.Services;
using HospitalManagementSystem.Forms.Shared;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.UserControls
{
    public partial class ucDoctors : UserControl
    {
        private readonly BindingList<Doctor> _doctors = new BindingList<Doctor>();
        private readonly DoctorService _service = new DoctorService();

        public ucDoctors()
        {
            InitializeComponent();
            dgvDoctors.AutoGenerateColumns = false;
            dgvDoctors.DataSource = _doctors;
            Load += ucDoctors_Load;
        }

        private async void ucDoctors_Load(object sender, EventArgs e)
        {
            await ReloadAsync().ConfigureAwait(false);
        }

        private async System.Threading.Tasks.Task ReloadAsync()
        {
            try
            {
                UseWaitCursor = true;
                _doctors.Clear();
                var list = await _service.GetAllAsync().ConfigureAwait(true);
                foreach (var item in list)
                {
                    _doctors.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load doctors: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }
    }
}
