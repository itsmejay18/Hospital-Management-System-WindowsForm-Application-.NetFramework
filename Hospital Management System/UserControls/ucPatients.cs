using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
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
        private ComboBox _searchFilter;
        private ComboBox _cboNationality;
        private ComboBox _cboIdType;
        private PictureBox _picProfileImage;
        private Button _btnUploadPhoto;
        private byte[] _patientPhotoBytes;

        private static readonly string[] NationalityOptions =
        {
            "Afghanistan", "Albania", "Algeria", "Andorra", "Angola", "Antigua and Barbuda", "Argentina", "Armenia", "Australia", "Austria",
            "Azerbaijan", "Bahamas", "Bahrain", "Bangladesh", "Barbados", "Belarus", "Belgium", "Belize", "Benin", "Bhutan",
            "Bolivia", "Bosnia and Herzegovina", "Botswana", "Brazil", "Brunei", "Bulgaria", "Burkina Faso", "Burundi", "Cabo Verde", "Cambodia",
            "Cameroon", "Canada", "Central African Republic", "Chad", "Chile", "China", "Colombia", "Comoros", "Congo", "Costa Rica",
            "Croatia", "Cuba", "Cyprus", "Czech Republic", "Democratic Republic of the Congo", "Denmark", "Djibouti", "Dominica", "Dominican Republic", "Ecuador",
            "Egypt", "El Salvador", "Equatorial Guinea", "Eritrea", "Estonia", "Eswatini", "Ethiopia", "Fiji", "Finland", "France",
            "Gabon", "Gambia", "Georgia", "Germany", "Ghana", "Greece", "Grenada", "Guatemala", "Guinea", "Guinea-Bissau",
            "Guyana", "Haiti", "Honduras", "Hungary", "Iceland", "India", "Indonesia", "Iran", "Iraq", "Ireland",
            "Israel", "Italy", "Jamaica", "Japan", "Jordan", "Kazakhstan", "Kenya", "Kiribati", "Kuwait", "Kyrgyzstan",
            "Laos", "Latvia", "Lebanon", "Lesotho", "Liberia", "Libya", "Liechtenstein", "Lithuania", "Luxembourg", "Madagascar",
            "Malawi", "Malaysia", "Maldives", "Mali", "Malta", "Marshall Islands", "Mauritania", "Mauritius", "Mexico", "Micronesia",
            "Moldova", "Monaco", "Mongolia", "Montenegro", "Morocco", "Mozambique", "Myanmar", "Namibia", "Nauru", "Nepal",
            "Netherlands", "New Zealand", "Nicaragua", "Niger", "Nigeria", "North Korea", "North Macedonia", "Norway", "Oman", "Pakistan",
            "Palau", "Palestine", "Panama", "Papua New Guinea", "Paraguay", "Peru", "Philippines", "Poland", "Portugal", "Qatar",
            "Romania", "Russia", "Rwanda", "Saint Kitts and Nevis", "Saint Lucia", "Saint Vincent and the Grenadines", "Samoa", "San Marino", "Sao Tome and Principe", "Saudi Arabia",
            "Senegal", "Serbia", "Seychelles", "Sierra Leone", "Singapore", "Slovakia", "Slovenia", "Solomon Islands", "Somalia", "South Africa",
            "South Korea", "South Sudan", "Spain", "Sri Lanka", "Sudan", "Suriname", "Sweden", "Switzerland", "Syria", "Taiwan",
            "Tajikistan", "Tanzania", "Thailand", "Timor-Leste", "Togo", "Tonga", "Trinidad and Tobago", "Tunisia", "Turkey", "Turkmenistan",
            "Tuvalu", "Uganda", "Ukraine", "United Arab Emirates", "United Kingdom", "United States", "Uruguay", "Uzbekistan", "Vanuatu", "Venezuela",
            "Vietnam", "Yemen", "Zambia", "Zimbabwe"
        };

        private static readonly string[] IdentificationTypeOptions =
        {
            "Driver's License",
            "Voter's ID",
            "School ID",
            "PhilHealth ID",
            "Passport",
            "SSS ID",
            "Postal ID",
            "NBI Clearance",
            "Senior Citizen ID",
            "National ID",
            "PRC ID",
            "TIN ID",
            "UMID",
            "Company ID",
            "Barangay ID"
        };

        private enum PatientEditorMode
        {
            View = 0,
            AddNew = 1,
            EditExisting = 2
        }

        private enum PatientSearchFilter
        {
            All = 0,
            Code = 1,
            Name = 2,
            Gender = 3,
            IdNumber = 4
        }

        public ucPatients()
        {
            InitializeComponent();
            ConfigureGrid();
            ConfigureDetailInputs();
            ConfigureSearchFilter();
            ConfigurePhotoSection();
            HookEvents();
            ApplyTheme();
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

        private void ConfigureSearchFilter()
        {
            _searchFilter = new ComboBox
            {
                Name = "cboSearchFilter",
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(608, 12),
                Size = new Size(168, 23)
            };

            _searchFilter.Items.AddRange(new object[]
            {
                "All Fields",
                "Patient Code",
                "Patient Name",
                "Gender",
                "ID Number"
            });
            _searchFilter.SelectedIndex = 0;
            pnlSearch.Controls.Add(_searchFilter);
        }

        private void ConfigurePhotoSection()
        {
            var photoPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 164,
                Padding = new Padding(0, 6, 0, 6)
            };

            var lblPhoto = new Label
            {
                AutoSize = true,
                Text = "Profile Image",
                Location = new Point(3, 8)
            };

            _picProfileImage = new PictureBox
            {
                Location = new Point(6, 30),
                Size = new Size(120, 120),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };

            _btnUploadPhoto = new Button
            {
                Text = "Upload Image",
                Location = new Point(136, 74),
                Size = new Size(118, 32)
            };
            _btnUploadPhoto.Click += btnUploadPhoto_Click;

            photoPanel.Controls.Add(lblPhoto);
            photoPanel.Controls.Add(_picProfileImage);
            photoPanel.Controls.Add(_btnUploadPhoto);
            pnlDetailScroll.Controls.Add(photoPanel);
            photoPanel.BringToFront();
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
            cboGender.Items.AddRange(new object[] { "Male", "Female", "Other" });
            cboGender.SelectedIndex = -1;

            InitializePatientDropdownEditors();
            PopulatePatientDropdownSources();
        }

        private void InitializePatientDropdownEditors()
        {
            _cboNationality = CreateDropdownEditor(txtNationality, "cboNationality");
            _cboIdType = CreateDropdownEditor(txtIdType, "cboIdentificationType");
            ReplaceEditorControl(txtNationality, _cboNationality);
            ReplaceEditorControl(txtIdType, _cboIdType);
        }

        private void PopulatePatientDropdownSources()
        {
            if (_cboNationality != null)
            {
                _cboNationality.Items.Clear();
                _cboNationality.Items.AddRange(NationalityOptions.Cast<object>().ToArray());
                _cboNationality.SelectedIndex = -1;
            }

            if (_cboIdType != null)
            {
                _cboIdType.Items.Clear();
                _cboIdType.Items.AddRange(IdentificationTypeOptions.Cast<object>().ToArray());
                _cboIdType.SelectedIndex = -1;
            }
        }

        private static ComboBox CreateDropdownEditor(TextBox original, string name)
        {
            return new ComboBox
            {
                Name = name,
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = original.Margin,
                Font = original.Font,
                TabIndex = original.TabIndex
            };
        }

        private static void ReplaceEditorControl(Control oldControl, Control newControl)
        {
            if (oldControl == null || newControl == null)
            {
                return;
            }

            var parent = oldControl.Parent as TableLayoutPanel;
            if (parent == null)
            {
                return;
            }

            var column = parent.GetColumn(oldControl);
            var row = parent.GetRow(oldControl);
            parent.Controls.Remove(oldControl);
            oldControl.Visible = false;
            oldControl.TabStop = false;
            parent.Controls.Add(newControl, column, row);
            parent.SetColumnSpan(newControl, parent.GetColumnSpan(oldControl));
            parent.SetRowSpan(newControl, parent.GetRowSpan(oldControl));
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
                var list = await _service.GetAllAsync().ConfigureAwait(true);
                BindPatients(ApplySearchFilter(list, txtSearch.Text));
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

        private IEnumerable<Patient> ApplySearchFilter(IEnumerable<Patient> source, string searchText)
        {
            if (source == null)
            {
                return Enumerable.Empty<Patient>();
            }

            var term = (searchText ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(term))
            {
                return source;
            }

            switch (GetSelectedSearchFilter())
            {
                case PatientSearchFilter.Code:
                    return source.Where(patient => ContainsInsensitive(patient.PatientCode, term));
                case PatientSearchFilter.Name:
                    return source.Where(patient =>
                        ContainsInsensitive(patient.FirstName, term)
                        || ContainsInsensitive(patient.LastName, term)
                        || ContainsInsensitive(patient.FullName, term));
                case PatientSearchFilter.Gender:
                    return source.Where(patient => ContainsInsensitive(MapGenderCodeToDisplay(patient.Gender), term));
                case PatientSearchFilter.IdNumber:
                    return source.Where(patient => ContainsInsensitive(patient.IdentificationNumber, term));
                case PatientSearchFilter.All:
                default:
                    return source.Where(patient =>
                        ContainsInsensitive(patient.PatientCode, term)
                        || ContainsInsensitive(patient.FirstName, term)
                        || ContainsInsensitive(patient.LastName, term)
                        || ContainsInsensitive(patient.FullName, term)
                        || ContainsInsensitive(patient.IdentificationNumber, term)
                        || ContainsInsensitive(patient.BloodGroup, term)
                        || ContainsInsensitive(patient.Nationality, term));
            }
        }

        private PatientSearchFilter GetSelectedSearchFilter()
        {
            if (_searchFilter == null || _searchFilter.SelectedIndex < 0)
            {
                return PatientSearchFilter.All;
            }

            return (PatientSearchFilter)_searchFilter.SelectedIndex;
        }

        private static bool ContainsInsensitive(string value, string searchText)
        {
            return !string.IsNullOrWhiteSpace(value)
                   && value.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
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
            SetNationalityValue(patient.Nationality);
            SetIdentificationTypeValue(patient.IdentificationType);
            txtIdNumber.Text = patient.IdentificationNumber;
            chkIsActive.Checked = patient.IsActive;
            SetPatientImage(patient.ProfileImage);
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
            SetNationalityValue(null);
            SetIdentificationTypeValue(null);
            txtIdNumber.Clear();
            chkIsActive.Checked = true;
            SetPatientImage(null);
        }

        private void SetGenderSelection(string value)
        {
            var normalized = MapGenderCodeToDisplay((value ?? string.Empty).Trim());
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
            if (_searchFilter != null)
            {
                _searchFilter.SelectedIndex = 0;
            }
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
                Gender = MapGenderDisplayToCode(cboGender.Text),
                DateOfBirth = dtpDob.Value.Date,
                BloodGroup = txtBloodGroup.Text.Trim(),
                MaritalStatus = txtMaritalStatus.Text.Trim(),
                Nationality = GetNationalityValue(),
                IdentificationType = GetIdentificationTypeValue(),
                IdentificationNumber = txtIdNumber.Text.Trim(),
                IsActive = chkIsActive.Checked,
                RegistrationDate = DateTime.Now,
                ProfileImage = CloneBytes(_patientPhotoBytes)
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
            SetReadOnlyState(txtIdNumber, !editable);

            cboGender.Enabled = editable;
            dtpDob.Enabled = editable;
            chkIsActive.Enabled = editable;
            SetDropdownReadOnlyState(_cboNationality, !editable);
            SetDropdownReadOnlyState(_cboIdType, !editable);
            if (_btnUploadPhoto != null)
            {
                _btnUploadPhoto.Enabled = true;
            }
        }

        private static void SetReadOnlyState(TextBox textBox, bool isReadOnly)
        {
            textBox.ReadOnly = isReadOnly;
            textBox.BackColor = isReadOnly ? ThemeManager.Colors.SurfaceMuted : ThemeManager.Colors.Surface;
        }

        private static void SetDropdownReadOnlyState(ComboBox comboBox, bool isReadOnly)
        {
            if (comboBox == null)
            {
                return;
            }

            comboBox.Enabled = !isReadOnly;
            comboBox.BackColor = isReadOnly ? ThemeManager.Colors.SurfaceMuted : ThemeManager.Colors.Surface;
        }

        private string GetNationalityValue()
        {
            return (_cboNationality?.Text ?? string.Empty).Trim();
        }

        private string GetIdentificationTypeValue()
        {
            return (_cboIdType?.Text ?? string.Empty).Trim();
        }

        private void SetNationalityValue(string value)
        {
            SetDropdownSelection(_cboNationality, value);
        }

        private void SetIdentificationTypeValue(string value)
        {
            SetDropdownSelection(_cboIdType, value);
        }

        private static void SetDropdownSelection(ComboBox comboBox, string value)
        {
            if (comboBox == null)
            {
                return;
            }

            var normalized = (value ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(normalized))
            {
                comboBox.SelectedIndex = -1;
                return;
            }

            var foundIndex = comboBox.FindStringExact(normalized);
            if (foundIndex < 0)
            {
                comboBox.Items.Add(normalized);
                foundIndex = comboBox.Items.Count - 1;
            }

            comboBox.SelectedIndex = foundIndex;
        }

        private async void btnUploadPhoto_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedPatient();
            var persistImmediately = _editorMode == PatientEditorMode.View && selected != null && selected.PatientID > 0;
            if (_editorMode == PatientEditorMode.View && selected == null)
            {
                MessageBox.Show("Select a patient first, or click Add New.", "Patients", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Select Patient Image";
                dialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp";
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    SetPatientImage(File.ReadAllBytes(dialog.FileName));
                    if (persistImmediately)
                    {
                        await SavePatientImageAsync(selected.PatientID).ConfigureAwait(true);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unable to load image: {ex.Message}", "Patients", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async Task SavePatientImageAsync(int patientId)
        {
            var source = _patients.FirstOrDefault(patient => patient.PatientID == patientId);
            if (source == null)
            {
                MessageBox.Show("Unable to find selected patient record.", "Patients", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                UseWaitCursor = true;
                var payload = new Patient
                {
                    PatientID = source.PatientID,
                    PatientCode = source.PatientCode,
                    FirstName = source.FirstName,
                    LastName = source.LastName,
                    DateOfBirth = source.DateOfBirth,
                    Gender = source.Gender,
                    BloodGroup = source.BloodGroup,
                    MaritalStatus = source.MaritalStatus,
                    Nationality = source.Nationality,
                    IdentificationType = source.IdentificationType,
                    IdentificationNumber = source.IdentificationNumber,
                    RegistrationDate = source.RegistrationDate ?? DateTime.Now,
                    IsActive = source.IsActive,
                    ProfileImage = CloneBytes(_patientPhotoBytes)
                };

                var updated = await _service.UpdateAsync(payload).ConfigureAwait(true);
                if (!updated)
                {
                    MessageBox.Show("Patient image was not saved. Please try again.", "Patients", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                await ReloadAsync(patientId).ConfigureAwait(true);
                MessageBox.Show("Patient image uploaded successfully.", "Patients", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to save patient image: {ex.Message}", "Patients", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void SetPatientImage(byte[] imageBytes)
        {
            _patientPhotoBytes = CloneBytes(imageBytes);
            if (_picProfileImage == null)
            {
                return;
            }

            var oldImage = _picProfileImage.Image;
            _picProfileImage.Image = CreateImageFromBytes(_patientPhotoBytes);
            oldImage?.Dispose();
        }

        private static Image CreateImageFromBytes(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
            {
                return null;
            }

            using (var stream = new MemoryStream(imageBytes))
            using (var image = Image.FromStream(stream))
            {
                return new Bitmap(image);
            }
        }

        private static byte[] CloneBytes(byte[] source)
        {
            if (source == null || source.Length == 0)
            {
                return null;
            }

            var copy = new byte[source.Length];
            Buffer.BlockCopy(source, 0, copy, 0, source.Length);
            return copy;
        }

        private static string MapGenderDisplayToCode(string displayValue)
        {
            var token = (displayValue ?? string.Empty).Trim();
            if (token.Equals("Male", StringComparison.OrdinalIgnoreCase) || token.Equals("M", StringComparison.OrdinalIgnoreCase))
            {
                return "M";
            }

            if (token.Equals("Female", StringComparison.OrdinalIgnoreCase) || token.Equals("F", StringComparison.OrdinalIgnoreCase))
            {
                return "F";
            }

            if (token.Equals("Other", StringComparison.OrdinalIgnoreCase) || token.Equals("O", StringComparison.OrdinalIgnoreCase))
            {
                return "O";
            }

            return string.Empty;
        }

        private static string MapGenderCodeToDisplay(string codeValue)
        {
            var token = (codeValue ?? string.Empty).Trim();
            if (token.Equals("M", StringComparison.OrdinalIgnoreCase) || token.Equals("Male", StringComparison.OrdinalIgnoreCase))
            {
                return "Male";
            }

            if (token.Equals("F", StringComparison.OrdinalIgnoreCase) || token.Equals("Female", StringComparison.OrdinalIgnoreCase))
            {
                return "Female";
            }

            if (token.Equals("O", StringComparison.OrdinalIgnoreCase) || token.Equals("Other", StringComparison.OrdinalIgnoreCase))
            {
                return "Other";
            }

            return string.Empty;
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
            ThemeManager.StyleSearchTextBox(txtSearch, "Search patient code / name");
            if (_searchFilter != null)
            {
                ThemeManager.StyleComboBox(_searchFilter);
            }

            if (_cboNationality != null)
            {
                ThemeManager.StyleComboBox(_cboNationality);
            }

            if (_cboIdType != null)
            {
                ThemeManager.StyleComboBox(_cboIdType);
            }

            if (_btnUploadPhoto != null)
            {
                ThemeManager.StyleButton(_btnUploadPhoto, ThemeButtonKind.Secondary);
            }

            if (_picProfileImage != null)
            {
                _picProfileImage.BackColor = ThemeManager.Colors.SurfaceMuted;
            }
        }
    }
}
