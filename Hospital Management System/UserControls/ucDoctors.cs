using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HospitalManagementSystem.BLL.Services;
using HospitalManagementSystem.Forms.Shared;
using HospitalManagementSystem.Helpers;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.UserControls
{
    public partial class ucDoctors : UserControl
    {
        private readonly BindingList<Doctor> _doctors = new BindingList<Doctor>();
        private readonly DoctorService _service = new DoctorService();
        private readonly BindingList<Specialization> _specializations = new BindingList<Specialization>();
        private readonly List<Doctor> _allDoctors = new List<Doctor>();

        private DoctorEditorMode _editorMode = DoctorEditorMode.View;
        private int? _editingDoctorId;

        private enum DoctorEditorMode
        {
            View = 0,
            EditExisting = 1
        }

        public ucDoctors()
        {
            InitializeComponent();
            ConfigureGrid();
            HookEvents();
            ApplyTheme();
            SetEditorMode(DoctorEditorMode.View);
            Load += ucDoctors_Load;
        }

        private void ConfigureGrid()
        {
            dgvDoctors.AutoGenerateColumns = false;
            dgvDoctors.DataSource = _doctors;
            dgvDoctors.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            colCode.FillWeight = 15F;
            colName.FillWeight = 28F;
            colSpec.FillWeight = 27F;
            colFee.FillWeight = 15F;
            colAvailable.FillWeight = 15F;
            colFee.DefaultCellStyle.Format = "N2";
            cboSpecialization.DisplayMember = "SpecializationName";
            cboSpecialization.ValueMember = "SpecializationID";
            cboSpecialization.DataSource = _specializations;
        }

        private void HookEvents()
        {
            btnSearch.Click += btnSearch_Click;
            btnRefresh.Click += btnRefresh_Click;
            txtSearch.KeyDown += txtSearch_KeyDown;

            btnAdd.Click += btnAdd_Click;
            btnEdit.Click += btnEdit_Click;
            btnSave.Click += btnSave_Click;
            btnCancel.Click += btnCancel_Click;
            btnDelete.Click += btnDelete_Click;
            dgvDoctors.SelectionChanged += dgvDoctors_SelectionChanged;
        }

        private async void ucDoctors_Load(object sender, EventArgs e)
        {
            await LoadLookupsAsync().ConfigureAwait(true);
            await ReloadAsync().ConfigureAwait(true);
        }

        private async Task LoadLookupsAsync()
        {
            try
            {
                var specs = await _service.GetSpecializationsAsync().ConfigureAwait(true);
                _specializations.RaiseListChangedEvents = false;
                _specializations.Clear();
                foreach (var spec in specs)
                {
                    _specializations.Add(spec);
                }

                _specializations.RaiseListChangedEvents = true;
                _specializations.ResetBindings();
            }
            catch
            {
                // Keep the editor usable even if specialization lookup fails.
            }
        }

        private async Task ReloadAsync(int? selectDoctorId = null)
        {
            try
            {
                UseWaitCursor = true;
                var list = await _service.GetAllAsync().ConfigureAwait(true);
                _allDoctors.Clear();
                _allDoctors.AddRange(list);
                ApplyFilter(txtSearch.Text, selectDoctorId);
                SetEditorMode(DoctorEditorMode.View);
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

        private void ApplyFilter(string term, int? preferredDoctorId = null)
        {
            var filter = (term ?? string.Empty).Trim();
            var filtered = string.IsNullOrWhiteSpace(filter)
                ? _allDoctors
                : _allDoctors.Where(x =>
                    ContainsInsensitive(x.DoctorCode, filter)
                    || ContainsInsensitive(x.DoctorName, filter)
                    || ContainsInsensitive(x.SpecializationName, filter)).ToList();

            _doctors.RaiseListChangedEvents = false;
            _doctors.Clear();
            foreach (var doctor in filtered)
            {
                _doctors.Add(doctor);
            }

            _doctors.RaiseListChangedEvents = true;
            _doctors.ResetBindings();

            RestoreSelection(preferredDoctorId);
        }

        private static bool ContainsInsensitive(string value, string searchText)
        {
            return !string.IsNullOrWhiteSpace(value)
                   && value.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void RestoreSelection(int? doctorId)
        {
            if (dgvDoctors.Rows.Count == 0)
            {
                PopulateDetails(null);
                UpdateActionButtons();
                return;
            }

            DataGridViewRow targetRow = null;
            if (doctorId.HasValue)
            {
                foreach (DataGridViewRow row in dgvDoctors.Rows)
                {
                    if (row.DataBoundItem is Doctor doctor && doctor.DoctorID == doctorId.Value)
                    {
                        targetRow = row;
                        break;
                    }
                }
            }

            if (targetRow == null)
            {
                targetRow = dgvDoctors.Rows[0];
            }

            if (targetRow.Cells.Count > 0)
            {
                dgvDoctors.CurrentCell = targetRow.Cells[0];
            }

            PopulateDetails(targetRow.DataBoundItem as Doctor);
            UpdateActionButtons();
        }

        private Doctor GetSelectedDoctor()
        {
            return dgvDoctors.CurrentRow?.DataBoundItem as Doctor;
        }

        private void PopulateDetails(Doctor doctor)
        {
            if (doctor == null)
            {
                txtCode.Clear();
                txtName.Clear();
                cboSpecialization.SelectedIndex = -1;
                txtQualification.Clear();
                txtLicense.Clear();
                nudExperience.Value = 0;
                nudConsultationFee.Value = 0;
                chkAvailable.Checked = false;
                dtpJoiningDate.Value = DateTime.Today;
                return;
            }

            txtCode.Text = doctor.DoctorCode;
            txtName.Text = doctor.DoctorName;
            txtQualification.Text = doctor.Qualification;
            txtLicense.Text = doctor.LicenseNumber;
            nudExperience.Value = ClampToNumeric(doctor.YearsOfExperience ?? 0, nudExperience);
            nudConsultationFee.Value = ClampToNumeric(doctor.ConsultationFee ?? 0m, nudConsultationFee);
            chkAvailable.Checked = doctor.IsAvailable;
            dtpJoiningDate.Value = doctor.JoiningDate?.Date ?? DateTime.Today;

            if (doctor.SpecializationID.HasValue)
            {
                cboSpecialization.SelectedValue = doctor.SpecializationID.Value;
            }
            else
            {
                cboSpecialization.SelectedIndex = -1;
            }
        }

        private static decimal ClampToNumeric(decimal value, NumericUpDown input)
        {
            if (value < input.Minimum)
            {
                return input.Minimum;
            }

            if (value > input.Maximum)
            {
                return input.Maximum;
            }

            return value;
        }

        private static decimal ClampToNumeric(int value, NumericUpDown input)
        {
            if (value < input.Minimum)
            {
                return input.Minimum;
            }

            if (value > input.Maximum)
            {
                return input.Maximum;
            }

            return value;
        }

        private void SetEditorMode(DoctorEditorMode mode)
        {
            _editorMode = mode;
            if (_editorMode == DoctorEditorMode.View)
            {
                _editingDoctorId = null;
            }

            var editable = _editorMode == DoctorEditorMode.EditExisting;
            txtCode.ReadOnly = true;
            txtCode.BackColor = ThemeManager.Colors.SurfaceMuted;
            txtName.ReadOnly = true;
            txtName.BackColor = ThemeManager.Colors.SurfaceMuted;

            cboSpecialization.Enabled = editable;
            txtQualification.ReadOnly = !editable;
            txtLicense.ReadOnly = !editable;
            nudExperience.Enabled = editable;
            nudConsultationFee.Enabled = editable;
            chkAvailable.Enabled = editable;
            dtpJoiningDate.Enabled = editable;

            txtQualification.BackColor = editable ? ThemeManager.Colors.Surface : ThemeManager.Colors.SurfaceMuted;
            txtLicense.BackColor = editable ? ThemeManager.Colors.Surface : ThemeManager.Colors.SurfaceMuted;

            btnSave.Enabled = editable;
            btnCancel.Enabled = editable;

            txtSearch.Enabled = !editable;
            btnSearch.Enabled = !editable;
            btnRefresh.Enabled = !editable;
            dgvDoctors.Enabled = !editable;

            UpdateActionButtons();
            lblDetailsHint.Text = editable
                ? "Editing doctor. Update fields then click Save."
                : "Select a doctor to view details. Click Edit to unlock.";
        }

        private void UpdateActionButtons()
        {
            var hasSelection = GetSelectedDoctor() != null;
            var viewMode = _editorMode == DoctorEditorMode.View;

            btnAdd.Enabled = viewMode;
            btnEdit.Enabled = viewMode && hasSelection;
            btnDelete.Enabled = viewMode && hasSelection;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            ApplyFilter(txtSearch.Text);
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            await ReloadAsync().ConfigureAwait(true);
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.SuppressKeyPress = true;
            ApplyFilter(txtSearch.Text);
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                using (var dialog = new frmDoctorEdit())
                {
                    IWin32Window owner = FindForm();
                    if (owner == null)
                    {
                        owner = this;
                    }
                    if (dialog.ShowDialog(owner) == DialogResult.OK)
                    {
                        await ReloadAsync().ConfigureAwait(true);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to add doctor: {ex.Message}", "Doctors", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedDoctor();
            if (selected == null)
            {
                MessageBox.Show("Select a doctor first.", "Doctors", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _editingDoctorId = selected.DoctorID;
            PopulateDetails(selected);
            SetEditorMode(DoctorEditorMode.EditExisting);
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (_editorMode != DoctorEditorMode.EditExisting)
            {
                return;
            }

            var doctorId = _editingDoctorId.GetValueOrDefault();
            var source = _allDoctors.FirstOrDefault(x => x.DoctorID == doctorId);
            if (source == null)
            {
                MessageBox.Show("Unable to resolve selected doctor record.", "Doctors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                UseWaitCursor = true;
                int? specializationId = null;
                if (cboSpecialization.SelectedValue != null
                    && int.TryParse(cboSpecialization.SelectedValue.ToString(), out var parsedSpecId))
                {
                    specializationId = parsedSpecId;
                }

                var payload = new Doctor
                {
                    DoctorID = source.DoctorID,
                    UserID = source.UserID,
                    DoctorCode = source.DoctorCode,
                    SpecializationID = specializationId,
                    Qualification = txtQualification.Text.Trim(),
                    LicenseNumber = txtLicense.Text.Trim(),
                    YearsOfExperience = Convert.ToInt32(nudExperience.Value),
                    ConsultationFee = nudConsultationFee.Value,
                    IsAvailable = chkAvailable.Checked,
                    JoiningDate = dtpJoiningDate.Value.Date
                };

                var updated = await _service.UpdateAsync(payload).ConfigureAwait(true);
                if (!updated)
                {
                    MessageBox.Show("No changes were saved. Please refresh and try again.", "Doctors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                await ReloadAsync(payload.DoctorID).ConfigureAwait(true);
                MessageBox.Show("Doctor updated successfully.", "Doctors", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to save doctor: {ex.Message}", "Doctors", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SetEditorMode(DoctorEditorMode.View);
            PopulateDetails(GetSelectedDoctor());
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (_editorMode != DoctorEditorMode.View)
            {
                MessageBox.Show("Finish or cancel editing before deleting.", "Doctors", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selected = GetSelectedDoctor();
            if (selected == null)
            {
                MessageBox.Show("Select a doctor to delete.", "Doctors", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show("Delete this doctor?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            try
            {
                UseWaitCursor = true;
                await _service.DeleteAsync(selected.DoctorID).ConfigureAwait(true);
                await ReloadAsync().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to delete doctor: {ex.Message}", "Doctors", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void dgvDoctors_SelectionChanged(object sender, EventArgs e)
        {
            if (_editorMode != DoctorEditorMode.View)
            {
                return;
            }

            PopulateDetails(GetSelectedDoctor());
            UpdateActionButtons();
        }

        private void ApplyTheme()
        {
            ThemeManager.ApplyControlTheme(this);
            ThemeManager.StyleMasterDetailModule(this, pnlSearch, pnlButtons, splitMain, grpDetails, lblDetailsHint);
            ThemeManager.StyleButton(btnAdd, ThemeButtonKind.Primary);
            ThemeManager.StyleButton(btnEdit, ThemeButtonKind.Secondary);
            ThemeManager.StyleButton(btnSave, ThemeButtonKind.Primary);
            ThemeManager.StyleButton(btnCancel, ThemeButtonKind.Secondary);
            ThemeManager.StyleButton(btnDelete, ThemeButtonKind.Danger);
            ThemeManager.StyleButton(btnSearch, ThemeButtonKind.Primary);
            ThemeManager.StyleButton(btnRefresh, ThemeButtonKind.Secondary);
            ThemeManager.StyleSearchTextBox(txtSearch, "Search doctor code / name / specialization");
        }
    }
}
