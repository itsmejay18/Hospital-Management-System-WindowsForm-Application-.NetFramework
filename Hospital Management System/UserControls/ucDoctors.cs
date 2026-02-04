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
            btnAdd.Click += btnAdd_Click;
            btnEdit.Click += btnEdit_Click;
            btnDelete.Click += btnDelete_Click;
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var dlg = new frmDoctorEdit())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _ = ReloadAsync();
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvDoctors.CurrentRow?.DataBoundItem is Doctor doctor)
            {
                using (var dlg = new frmDoctorEdit(doctor))
                {
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        _ = ReloadAsync();
                    }
                }
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvDoctors.CurrentRow?.DataBoundItem is Doctor doctor)
            {
                var confirm = MessageBox.Show("Delete this doctor?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm != DialogResult.Yes)
                {
                    return;
                }

                await _service.DeleteAsync(doctor.DoctorID).ConfigureAwait(true);
                await ReloadAsync().ConfigureAwait(true);
            }
        }
    }
}
