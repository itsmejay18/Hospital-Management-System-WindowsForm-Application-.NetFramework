using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HospitalManagementSystem.BLL.Services;
using HospitalManagementSystem.Helpers;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.UserControls
{
    public partial class ucPatients : UserControl
    {
        private readonly BindingList<Patient> _patients = new BindingList<Patient>();
        private readonly PatientService _service = new PatientService();
        private PatientEditorMode _editorMode = PatientEditorMode.View;
        private int? _editingPatientId;

        private enum PatientEditorMode
        {
            View = 0,
            AddNew = 1,
            EditExisting = 2
        }

        public ucPatients()
        {
            InitializeComponent();
            ApplyTheme();
            ConfigureGrid();
            ConfigureDetailInputs();
            HookEvents();
            SetEditorMode(PatientEditorMode.View);
            Load += ucPatients_Load;
        }

        private void ConfigureGrid()
        {
            dgvPatients.AutoGenerateColumns = false;
            dgvPatients.DataSource = _patients;
            dgvPatients.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            colCode.FillWeight = 18F;
            colName.FillWeight = 34F;
            colGender.FillWeight = 14F;
            colDob.FillWeight = 20F;
            colStatus.FillWeight = 14F;
            colDob.DefaultCellStyle.Format = "yyyy-MM-dd";
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

            dgvPatients.SelectionChanged += dgvPatients_SelectionChanged;
        }

        private void ConfigureDetailInputs()
        {
            cboGender.Items.Clear();
            cboGender.Items.AddRange(new object[] { "Male", "Female", "Other", "M", "F" });
            cboGender.SelectedIndex = -1;
        }

        private async void ucPatients_Load(object sender, EventArgs e)
        {
            await ReloadAsync().ConfigureAwait(true);
        }

        private async Task ReloadAsync(int? selectPatientId = null)
        {
            try
            {
                UseWaitCursor = true;
                var list = await _service.GetAllAsync().ConfigureAwait(true);
                BindPatients(list);
                RestoreSelection(selectPatientId);
                SetEditorMode(PatientEditorMode.View);
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

        private async Task SearchAsync()
        {
            try
            {
                UseWaitCursor = true;
                var list = await _service.SearchAsync(txtSearch.Text).ConfigureAwait(true);
                BindPatients(list);
                RestoreSelection(null);
                SetEditorMode(PatientEditorMode.View);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Search failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void BindPatients(System.Collections.Generic.IEnumerable<Patient> list)
        {
            _patients.RaiseListChangedEvents = false;
            _patients.Clear();
            foreach (var item in list)
            {
                _patients.Add(item);
            }

            _patients.RaiseListChangedEvents = true;
            _patients.ResetBindings();
        }

        private void RestoreSelection(int? patientId)
        {
            if (dgvPatients.Rows.Count == 0)
            {
                PopulateDetails(null);
                UpdateActionButtons();
                return;
            }

            DataGridViewRow targetRow = null;
            if (patientId.HasValue)
            {
                foreach (DataGridViewRow row in dgvPatients.Rows)
                {
                    if (row.DataBoundItem is Patient patient && patient.PatientID == patientId.Value)
                    {
                        targetRow = row;
                        break;
                    }
                }
            }

            if (targetRow == null)
            {
                targetRow = dgvPatients.Rows[0];
            }

            if (targetRow.Cells.Count > 0)
            {
                dgvPatients.CurrentCell = targetRow.Cells[0];
            }

            PopulateDetails(targetRow.DataBoundItem as Patient);
            UpdateActionButtons();
        }

        private Patient GetSelectedPatient()
        {
            return dgvPatients.CurrentRow?.DataBoundItem as Patient;
        }

        private void PopulateDetails(Patient patient)
        {
            if (patient == null)
            {
                ClearDetailInputs();
                return;
            }

            txtCode.Text = patient.PatientCode;
            txtFirstName.Text = patient.FirstName;
            txtLastName.Text = patient.LastName;
            SetGenderSelection(patient.Gender);
            dtpDob.Value = patient.DateOfBirth == default ? DateTime.Today : patient.DateOfBirth.Date;
            txtBloodGroup.Text = patient.BloodGroup;
            txtMaritalStatus.Text = patient.MaritalStatus;
            txtNationality.Text = patient.Nationality;
            txtIdType.Text = patient.IdentificationType;
            txtIdNumber.Text = patient.IdentificationNumber;
            chkIsActive.Checked = patient.IsActive;
        }

        private void ClearDetailInputs()
        {
            txtCode.Clear();
            txtFirstName.Clear();
            txtLastName.Clear();
            cboGender.SelectedIndex = -1;
            dtpDob.Value = DateTime.Today;
            txtBloodGroup.Clear();
            txtMaritalStatus.Clear();
            txtNationality.Clear();
            txtIdType.Clear();
            txtIdNumber.Clear();
            chkIsActive.Checked = true;
        }

        private void SetGenderSelection(string value)
        {
            var normalized = (value ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(normalized))
            {
                cboGender.SelectedIndex = -1;
                return;
            }

            var existingIndex = cboGender.FindStringExact(normalized);
            if (existingIndex >= 0)
            {
                cboGender.SelectedIndex = existingIndex;
                return;
            }

            cboGender.Items.Add(normalized);
            cboGender.SelectedIndex = cboGender.Items.Count - 1;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            _editingPatientId = null;
            ClearDetailInputs();
            txtCode.Text = $"P-{DateTime.Now:yyyyMMddHHmmss}";
            chkIsActive.Checked = true;
            SetEditorMode(PatientEditorMode.AddNew);
            txtFirstName.Focus();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedPatient();
            if (selected == null)
            {
                MessageBox.Show("Select a patient first.", "Patients", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _editingPatientId = selected.PatientID;
            PopulateDetails(selected);
            SetEditorMode(PatientEditorMode.EditExisting);
            txtFirstName.Focus();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (_editorMode == PatientEditorMode.View)
            {
                return;
            }

            try
            {
                UseWaitCursor = true;
                var patient = BuildPatientFromEditor();

                if (_editorMode == PatientEditorMode.AddNew)
                {
                    var newId = await _service.AddAsync(patient).ConfigureAwait(true);
                    await ReloadAsync(newId).ConfigureAwait(true);
                    MessageBox.Show("Patient added successfully.", "Patients", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var patientId = _editingPatientId.GetValueOrDefault();
                if (patientId <= 0)
                {
                    MessageBox.Show("Unable to determine the selected patient record.", "Patients", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var existing = _patients.FirstOrDefault(p => p.PatientID == patientId);
                patient.PatientID = patientId;
                patient.RegistrationDate = existing?.RegistrationDate ?? DateTime.Now;

                var updated = await _service.UpdateAsync(patient).ConfigureAwait(true);
                if (!updated)
                {
                    MessageBox.Show("No changes were saved. The record may have been changed by another user.", "Patients", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                await ReloadAsync(patient.PatientID).ConfigureAwait(true);
                MessageBox.Show("Patient updated successfully.", "Patients", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to save patient: {ex.Message}", "Patients", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SetEditorMode(PatientEditorMode.View);
            PopulateDetails(GetSelectedPatient());
            UpdateActionButtons();
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (_editorMode != PatientEditorMode.View)
            {
                MessageBox.Show("Finish or cancel editing before deleting.", "Patients", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selected = GetSelectedPatient();
            if (selected == null)
            {
                MessageBox.Show("Select a patient to delete.", "Patients", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show("Delete this patient?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            try
            {
                UseWaitCursor = true;
                await _service.DeleteAsync(selected.PatientID).ConfigureAwait(true);
                await ReloadAsync().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to delete patient: {ex.Message}", "Patients", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            await SearchAsync().ConfigureAwait(true);
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            await ReloadAsync().ConfigureAwait(true);
        }

        private async void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.SuppressKeyPress = true;
            await SearchAsync().ConfigureAwait(true);
        }

        private void dgvPatients_SelectionChanged(object sender, EventArgs e)
        {
            if (_editorMode != PatientEditorMode.View)
            {
                return;
            }

            PopulateDetails(GetSelectedPatient());
            UpdateActionButtons();
        }

        private Patient BuildPatientFromEditor()
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                throw new InvalidOperationException("First name is required.");
            }

            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                throw new InvalidOperationException("Last name is required.");
            }

            if (dtpDob.Value.Date > DateTime.Today)
            {
                throw new InvalidOperationException("Date of birth cannot be in the future.");
            }

            return new Patient
            {
                PatientCode = string.IsNullOrWhiteSpace(txtCode.Text)
                    ? $"P-{DateTime.Now:yyyyMMddHHmmss}"
                    : txtCode.Text.Trim(),
                FirstName = txtFirstName.Text.Trim(),
                LastName = txtLastName.Text.Trim(),
                Gender = cboGender.Text.Trim(),
                DateOfBirth = dtpDob.Value.Date,
                BloodGroup = txtBloodGroup.Text.Trim(),
                MaritalStatus = txtMaritalStatus.Text.Trim(),
                Nationality = txtNationality.Text.Trim(),
                IdentificationType = txtIdType.Text.Trim(),
                IdentificationNumber = txtIdNumber.Text.Trim(),
                IsActive = chkIsActive.Checked,
                RegistrationDate = DateTime.Now
            };
        }

        private void SetEditorMode(PatientEditorMode mode)
        {
            _editorMode = mode;
            if (_editorMode == PatientEditorMode.View)
            {
                _editingPatientId = null;
            }

            var editable = _editorMode != PatientEditorMode.View;
            SetFieldsEditable(editable);

            btnSave.Enabled = editable;
            btnCancel.Enabled = editable;

            txtSearch.Enabled = !editable;
            btnSearch.Enabled = !editable;
            btnRefresh.Enabled = !editable;
            dgvPatients.Enabled = !editable;

            UpdateActionButtons();

            switch (_editorMode)
            {
                case PatientEditorMode.AddNew:
                    lblDetailsHint.Text = "Creating new patient. Fill required fields and click Save.";
                    break;
                case PatientEditorMode.EditExisting:
                    lblDetailsHint.Text = "Editing patient. Update fields and click Save.";
                    break;
                default:
                    lblDetailsHint.Text = "Select a patient to view details. Click Edit to unlock fields.";
                    break;
            }
        }

        private void UpdateActionButtons()
        {
            var hasSelection = GetSelectedPatient() != null;
            var isViewMode = _editorMode == PatientEditorMode.View;

            btnAdd.Enabled = isViewMode;
            btnEdit.Enabled = isViewMode && hasSelection;
            btnDelete.Enabled = isViewMode && hasSelection;
        }

        private void SetFieldsEditable(bool editable)
        {
            SetReadOnlyState(txtCode, !editable);
            SetReadOnlyState(txtFirstName, !editable);
            SetReadOnlyState(txtLastName, !editable);
            SetReadOnlyState(txtBloodGroup, !editable);
            SetReadOnlyState(txtMaritalStatus, !editable);
            SetReadOnlyState(txtNationality, !editable);
            SetReadOnlyState(txtIdType, !editable);
            SetReadOnlyState(txtIdNumber, !editable);

            cboGender.Enabled = editable;
            dtpDob.Enabled = editable;
            chkIsActive.Enabled = editable;
        }

        private static void SetReadOnlyState(TextBox textBox, bool isReadOnly)
        {
            textBox.ReadOnly = isReadOnly;
            textBox.BackColor = isReadOnly ? ThemeManager.Colors.SurfaceMuted : ThemeManager.Colors.Surface;
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
        }
    }
}
