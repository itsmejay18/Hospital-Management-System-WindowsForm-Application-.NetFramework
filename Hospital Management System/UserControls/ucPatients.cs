using System;
using System.ComponentModel;
using System.Windows.Forms;
using HospitalManagementSystem.BLL.Services;
using HospitalManagementSystem.Forms.Shared;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.UserControls
{
    public partial class ucPatients : UserControl
    {
        private readonly BindingList<Patient> _patients = new BindingList<Patient>();
        private readonly PatientService _service = new PatientService();

        public ucPatients()
        {
            InitializeComponent();
            dgvPatients.AutoGenerateColumns = false;
            dgvPatients.DataSource = _patients;
            Load += ucPatients_Load;
            btnAdd.Click += btnAdd_Click;
            btnEdit.Click += btnEdit_Click;
            btnDelete.Click += btnDelete_Click;
        }

        private async void ucPatients_Load(object sender, EventArgs e)
        {
            await ReloadAsync().ConfigureAwait(false);
        }

        private async System.Threading.Tasks.Task ReloadAsync()
        {
            try
            {
                UseWaitCursor = true;
                _patients.Clear();
                var list = await _service.GetAllAsync().ConfigureAwait(true);
                foreach (var item in list)
                {
                    _patients.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load patients: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var dlg = new frmPatientEdit())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _ = ReloadAsync();
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvPatients.CurrentRow?.DataBoundItem is Patient patient)
            {
                using (var dlg = new frmPatientEdit(patient))
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
            if (dgvPatients.CurrentRow?.DataBoundItem is Patient patient)
            {
                var confirm = MessageBox.Show("Delete this patient?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm != DialogResult.Yes)
                {
                    return;
                }

                await _service.DeleteAsync(patient.PatientID).ConfigureAwait(true);
                await ReloadAsync().ConfigureAwait(true);
            }
        }
    }
}
