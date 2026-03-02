using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
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
        private readonly UserService _userService = new UserService();
        private readonly BindingList<Specialization> _specializations = new BindingList<Specialization>();
        private readonly List<Doctor> _allDoctors = new List<Doctor>();

        private DoctorEditorMode _editorMode = DoctorEditorMode.View;
        private int? _editingDoctorId;
        private ComboBox _searchFilter;
        private PictureBox _picProfileImage;
        private Button _btnUploadPhoto;
        private byte[] _doctorProfileImageBytes;
        private bool _doctorImageDirty;
        private UserDetail _selectedDoctorDetail;

        private enum DoctorEditorMode
        {
            View = 0,
            EditExisting = 1
        }

        private enum DoctorSearchFilter
        {
            All = 0,
            Code = 1,
            Name = 2,
            Specialization = 3,
            License = 4,
            Availability = 5
        }

        public ucDoctors()
        {
            InitializeComponent();
            ConfigureGrid();
            ConfigureSearchFilter();
            ConfigureImageSection();
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
                "Doctor Code",
                "Doctor Name",
                "Specialization",
                "License No.",
                "Availability"
            });
            _searchFilter.SelectedIndex = 0;
            pnlSearch.Controls.Add(_searchFilter);
        }

        private void ConfigureImageSection()
        {
            var imagePanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 162,
                Padding = new Padding(0, 6, 0, 6)
            };

            var lblImage = new Label
            {
                AutoSize = true,
                Text = "Doctor Image",
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

            imagePanel.Controls.Add(lblImage);
            imagePanel.Controls.Add(_picProfileImage);
            imagePanel.Controls.Add(_btnUploadPhoto);
            grpDetails.Controls.Add(imagePanel);
            imagePanel.BringToFront();
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
                : _allDoctors.Where(doctor => MatchesDoctorSearch(doctor, filter)).ToList();

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

        private bool MatchesDoctorSearch(Doctor doctor, string searchText)
        {
            switch (GetSelectedSearchFilter())
            {
                case DoctorSearchFilter.Code:
                    return ContainsInsensitive(doctor?.DoctorCode, searchText);
                case DoctorSearchFilter.Name:
                    return ContainsInsensitive(doctor?.DoctorName, searchText);
                case DoctorSearchFilter.Specialization:
                    return ContainsInsensitive(doctor?.SpecializationName, searchText);
                case DoctorSearchFilter.License:
                    return ContainsInsensitive(doctor?.LicenseNumber, searchText);
                case DoctorSearchFilter.Availability:
                    return ContainsInsensitive(doctor != null && doctor.IsAvailable ? "Available" : "Unavailable", searchText);
                case DoctorSearchFilter.All:
                default:
                    return ContainsInsensitive(doctor?.DoctorCode, searchText)
                           || ContainsInsensitive(doctor?.DoctorName, searchText)
                           || ContainsInsensitive(doctor?.SpecializationName, searchText)
                           || ContainsInsensitive(doctor?.LicenseNumber, searchText)
                           || ContainsInsensitive(doctor != null && doctor.IsAvailable ? "Available" : "Unavailable", searchText);
            }
        }

        private DoctorSearchFilter GetSelectedSearchFilter()
        {
            if (_searchFilter == null || _searchFilter.SelectedIndex < 0)
            {
                return DoctorSearchFilter.All;
            }

            return (DoctorSearchFilter)_searchFilter.SelectedIndex;
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

            var selectedDoctor = targetRow.DataBoundItem as Doctor;
            PopulateDetails(selectedDoctor);
            _ = LoadSelectedDoctorDetailAsync(selectedDoctor);
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
                _selectedDoctorDetail = null;
                SetDoctorImage(null, markDirty: false);
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

            if (_selectedDoctorDetail == null || _selectedDoctorDetail.UserID != doctor.UserID)
            {
                SetDoctorImage(null, markDirty: false);
            }
            else
            {
                SetDoctorImage(_selectedDoctorDetail.ProfileImage, markDirty: false);
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
            if (_btnUploadPhoto != null)
            {
                _btnUploadPhoto.Enabled = true;
            }

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
            if (_searchFilter != null)
            {
                _searchFilter.SelectedIndex = 0;
            }
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
                var imageChanged = _doctorImageDirty;
                if (!updated && !imageChanged)
                {
                    MessageBox.Show("No changes were saved. Please refresh and try again.", "Doctors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                await SaveDoctorImageAsync(source.UserID, source.DoctorName).ConfigureAwait(true);
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
            var selectedDoctor = GetSelectedDoctor();
            PopulateDetails(selectedDoctor);
            _ = LoadSelectedDoctorDetailAsync(selectedDoctor);
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

            var selectedDoctor = GetSelectedDoctor();
            PopulateDetails(selectedDoctor);
            _ = LoadSelectedDoctorDetailAsync(selectedDoctor);
            UpdateActionButtons();
        }

        private async Task LoadSelectedDoctorDetailAsync(Doctor doctor)
        {
            if (doctor == null || doctor.UserID <= 0)
            {
                _selectedDoctorDetail = null;
                SetDoctorImage(null, markDirty: false);
                return;
            }

            try
            {
                _selectedDoctorDetail = await _userService.GetUserDetailAsync(doctor.UserID).ConfigureAwait(true);
                SetDoctorImage(_selectedDoctorDetail?.ProfileImage, markDirty: false);
            }
            catch
            {
                _selectedDoctorDetail = null;
                SetDoctorImage(null, markDirty: false);
            }
        }

        private async Task SaveDoctorImageAsync(int userId, string doctorName)
        {
            if (!_doctorImageDirty || userId <= 0)
            {
                return;
            }

            var detail = _selectedDoctorDetail;
            if (detail == null || detail.UserID != userId)
            {
                detail = await _userService.GetUserDetailAsync(userId).ConfigureAwait(true);
            }

            if (detail == null)
            {
                var names = BuildFallbackName(doctorName);
                detail = new UserDetail
                {
                    UserID = userId,
                    FirstName = names.Item1,
                    LastName = names.Item2,
                    ProfileImage = CloneBytes(_doctorProfileImageBytes)
                };
                await _userService.AddUserDetailAsync(detail).ConfigureAwait(true);
                _selectedDoctorDetail = detail;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(detail.FirstName) || string.IsNullOrWhiteSpace(detail.LastName))
                {
                    var names = BuildFallbackName(doctorName);
                    if (string.IsNullOrWhiteSpace(detail.FirstName))
                    {
                        detail.FirstName = names.Item1;
                    }

                    if (string.IsNullOrWhiteSpace(detail.LastName))
                    {
                        detail.LastName = names.Item2;
                    }
                }

                detail.ProfileImage = CloneBytes(_doctorProfileImageBytes);
                await _userService.UpdateUserDetailAsync(detail).ConfigureAwait(true);
                _selectedDoctorDetail = detail;
            }

            _doctorImageDirty = false;
        }

        private static Tuple<string, string> BuildFallbackName(string doctorName)
        {
            var cleaned = (doctorName ?? string.Empty).Trim().Replace('.', ' ').Replace('_', ' ');
            if (string.IsNullOrWhiteSpace(cleaned))
            {
                return Tuple.Create("Doctor", "Profile");
            }

            var tokens = cleaned.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 1)
            {
                return Tuple.Create(tokens[0], "Profile");
            }

            return Tuple.Create(tokens[0], string.Join(" ", tokens.Skip(1)));
        }

        private async void btnUploadPhoto_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedDoctor();
            if (selected == null)
            {
                MessageBox.Show("Select a doctor first.", "Doctors", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var persistImmediately = _editorMode == DoctorEditorMode.View;
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Select Doctor Image";
                dialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp";
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    SetDoctorImage(File.ReadAllBytes(dialog.FileName), markDirty: true);
                    if (persistImmediately)
                    {
                        await PersistDoctorImageImmediatelyAsync(selected).ConfigureAwait(true);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unable to load image: {ex.Message}", "Doctors", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async Task PersistDoctorImageImmediatelyAsync(Doctor selectedDoctor)
        {
            if (selectedDoctor == null || selectedDoctor.UserID <= 0)
            {
                MessageBox.Show("Selected doctor does not have a valid user account.", "Doctors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                UseWaitCursor = true;
                await SaveDoctorImageAsync(selectedDoctor.UserID, selectedDoctor.DoctorName).ConfigureAwait(true);
                await ReloadAsync(selectedDoctor.DoctorID).ConfigureAwait(true);
                MessageBox.Show("Doctor image uploaded successfully.", "Doctors", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to save doctor image: {ex.Message}", "Doctors", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void SetDoctorImage(byte[] bytes, bool markDirty)
        {
            _doctorProfileImageBytes = CloneBytes(bytes);
            _doctorImageDirty = markDirty;
            if (_picProfileImage == null)
            {
                return;
            }

            var oldImage = _picProfileImage.Image;
            _picProfileImage.Image = CreateImageFromBytes(_doctorProfileImageBytes);
            oldImage?.Dispose();
        }

        private static Image CreateImageFromBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }

            using (var stream = new MemoryStream(bytes))
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
            if (_searchFilter != null)
            {
                ThemeManager.StyleComboBox(_searchFilter);
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
