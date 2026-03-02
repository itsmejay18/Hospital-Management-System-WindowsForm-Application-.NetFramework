using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HospitalManagementSystem.BLL.Services;
using HospitalManagementSystem.Helpers;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.UserControls
{
    public partial class ucAppointments : UserControl
    {
        private readonly BindingList<Appointment> _appointments = new BindingList<Appointment>();
        private readonly AppointmentService _service = new AppointmentService();
        private readonly List<Appointment> _allAppointments = new List<Appointment>();

        private AppointmentEditorMode _editorMode = AppointmentEditorMode.View;
        private int? _editingAppointmentId;
        private ComboBox _searchFilter;

        private enum AppointmentEditorMode
        {
            View = 0,
            EditExisting = 1
        }

        private enum AppointmentSearchFilter
        {
            All = 0,
            Code = 1,
            Patient = 2,
            Doctor = 3,
            Status = 4,
            Type = 5,
            Date = 6
        }

        public ucAppointments()
        {
            InitializeComponent();
            ConfigureGrid();
            ConfigureDetailInputs();
            ConfigureSearchFilter();
            HookEvents();
            ApplyTheme();
            SetEditorMode(AppointmentEditorMode.View);
            Load += ucAppointments_Load;
        }

        private void ConfigureGrid()
        {
            dgvAppointments.AutoGenerateColumns = false;
            dgvAppointments.DataSource = _appointments;
            dgvAppointments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            colCode.FillWeight = 14F;
            colPatient.FillWeight = 24F;
            colDoctor.FillWeight = 24F;
            colDate.FillWeight = 18F;
            colStatus.FillWeight = 20F;
            colDate.DefaultCellStyle.Format = "yyyy-MM-dd";
        }

        private void ConfigureDetailInputs()
        {
            cboType.Items.Clear();
            cboType.Items.AddRange(new object[]
            {
                "Consultation",
                "Follow-up",
                "Emergency",
                "Check-up"
            });

            cboStatus.Items.Clear();
            cboStatus.Items.AddRange(new object[]
            {
                "Scheduled",
                "Confirmed",
                "Completed",
                "Cancelled",
                "No-show"
            });
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
                "Appointment Code",
                "Patient Name",
                "Doctor Name",
                "Status",
                "Appointment Type",
                "Date (yyyy-mm-dd)"
            });
            _searchFilter.SelectedIndex = 0;
            pnlSearch.Controls.Add(_searchFilter);
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
            dgvAppointments.SelectionChanged += dgvAppointments_SelectionChanged;
        }

        private async void ucAppointments_Load(object sender, EventArgs e)
        {
            await ReloadAsync().ConfigureAwait(true);
        }

        private async Task ReloadAsync(int? selectAppointmentId = null)
        {
            try
            {
                UseWaitCursor = true;
                var list = await _service.GetAllAsync().ConfigureAwait(true);
                _allAppointments.Clear();
                _allAppointments.AddRange(list);
                ApplyFilter(txtSearch.Text, selectAppointmentId);
                SetEditorMode(AppointmentEditorMode.View);
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

        private void ApplyFilter(string searchTerm, int? preferredAppointmentId = null)
        {
            var term = (searchTerm ?? string.Empty).Trim();
            var filtered = string.IsNullOrWhiteSpace(term)
                ? _allAppointments
                : _allAppointments.Where(appointment => MatchesSearch(appointment, term)).ToList();

            _appointments.RaiseListChangedEvents = false;
            _appointments.Clear();
            foreach (var item in filtered)
            {
                _appointments.Add(item);
            }

            _appointments.RaiseListChangedEvents = true;
            _appointments.ResetBindings();

            RestoreSelection(preferredAppointmentId);
        }

        private bool MatchesSearch(Appointment appointment, string term)
        {
            switch (GetSelectedSearchFilter())
            {
                case AppointmentSearchFilter.Code:
                    return ContainsInsensitive(appointment?.AppointmentCode, term);
                case AppointmentSearchFilter.Patient:
                    return ContainsInsensitive(appointment?.PatientName, term);
                case AppointmentSearchFilter.Doctor:
                    return ContainsInsensitive(appointment?.DoctorName, term);
                case AppointmentSearchFilter.Status:
                    return ContainsInsensitive(appointment?.Status, term);
                case AppointmentSearchFilter.Type:
                    return ContainsInsensitive(appointment?.AppointmentType, term);
                case AppointmentSearchFilter.Date:
                    return appointment != null && appointment.AppointmentDate.ToString("yyyy-MM-dd").IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0;
                case AppointmentSearchFilter.All:
                default:
                    return ContainsInsensitive(appointment?.AppointmentCode, term)
                           || ContainsInsensitive(appointment?.PatientName, term)
                           || ContainsInsensitive(appointment?.DoctorName, term)
                           || ContainsInsensitive(appointment?.Status, term)
                           || ContainsInsensitive(appointment?.AppointmentType, term)
                           || (appointment != null && appointment.AppointmentDate.ToString("yyyy-MM-dd").IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        private AppointmentSearchFilter GetSelectedSearchFilter()
        {
            if (_searchFilter == null || _searchFilter.SelectedIndex < 0)
            {
                return AppointmentSearchFilter.All;
            }

            return (AppointmentSearchFilter)_searchFilter.SelectedIndex;
        }

        private static bool ContainsInsensitive(string value, string searchText)
        {
            return !string.IsNullOrWhiteSpace(value)
                   && value.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void RestoreSelection(int? appointmentId)
        {
            if (dgvAppointments.Rows.Count == 0)
            {
                PopulateDetails(null);
                UpdateActionButtons();
                return;
            }

            DataGridViewRow targetRow = null;
            if (appointmentId.HasValue)
            {
                foreach (DataGridViewRow row in dgvAppointments.Rows)
                {
                    if (row.DataBoundItem is Appointment item && item.AppointmentID == appointmentId.Value)
                    {
                        targetRow = row;
                        break;
                    }
                }
            }

            if (targetRow == null)
            {
                targetRow = dgvAppointments.Rows[0];
            }

            if (targetRow.Cells.Count > 0)
            {
                dgvAppointments.CurrentCell = targetRow.Cells[0];
            }

            PopulateDetails(targetRow.DataBoundItem as Appointment);
            UpdateActionButtons();
        }

        private Appointment GetSelectedAppointment()
        {
            return dgvAppointments.CurrentRow?.DataBoundItem as Appointment;
        }

        private void PopulateDetails(Appointment appointment)
        {
            if (appointment == null)
            {
                txtCode.Clear();
                txtPatient.Clear();
                txtDoctor.Clear();
                cboType.SelectedIndex = -1;
                cboStatus.SelectedIndex = -1;
                nudDuration.Value = 0;
                txtReason.Clear();
                txtNotes.Clear();
                dtpDate.Value = DateTime.Today;
                dtpTime.Value = DateTime.Today;
                return;
            }

            txtCode.Text = appointment.AppointmentCode;
            txtPatient.Text = appointment.PatientName;
            txtDoctor.Text = appointment.DoctorName;
            dtpDate.Value = appointment.AppointmentDate == default ? DateTime.Today : appointment.AppointmentDate.Date;
            dtpTime.Value = DateTime.Today.Add(appointment.AppointmentTime);
            cboType.Text = appointment.AppointmentType ?? string.Empty;
            cboStatus.Text = appointment.Status ?? string.Empty;
            nudDuration.Value = ClampToNumeric(appointment.Duration, nudDuration);
            txtReason.Text = appointment.Reason;
            txtNotes.Text = appointment.Notes;
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

        private void SetEditorMode(AppointmentEditorMode mode)
        {
            _editorMode = mode;
            if (_editorMode == AppointmentEditorMode.View)
            {
                _editingAppointmentId = null;
            }

            var editable = _editorMode == AppointmentEditorMode.EditExisting;

            txtCode.ReadOnly = true;
            txtCode.BackColor = ThemeManager.Colors.SurfaceMuted;
            txtPatient.ReadOnly = true;
            txtPatient.BackColor = ThemeManager.Colors.SurfaceMuted;
            txtDoctor.ReadOnly = true;
            txtDoctor.BackColor = ThemeManager.Colors.SurfaceMuted;

            dtpDate.Enabled = editable;
            dtpTime.Enabled = editable;
            cboType.Enabled = editable;
            cboStatus.Enabled = editable;
            nudDuration.Enabled = editable;
            txtReason.ReadOnly = !editable;
            txtNotes.ReadOnly = !editable;
            txtReason.BackColor = editable ? ThemeManager.Colors.Surface : ThemeManager.Colors.SurfaceMuted;
            txtNotes.BackColor = editable ? ThemeManager.Colors.Surface : ThemeManager.Colors.SurfaceMuted;

            btnSave.Enabled = editable;
            btnCancel.Enabled = editable;

            txtSearch.Enabled = !editable;
            btnSearch.Enabled = !editable;
            btnRefresh.Enabled = !editable;
            dgvAppointments.Enabled = !editable;

            UpdateActionButtons();
            lblDetailsHint.Text = editable
                ? "Editing appointment. Update fields then click Save."
                : "Select an appointment to view details. Click Edit to unlock.";
        }

        private void UpdateActionButtons()
        {
            var hasSelection = GetSelectedAppointment() != null;
            var viewMode = _editorMode == AppointmentEditorMode.View;
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
                using (var dlg = new Forms.Shared.frmAppointmentEdit())
                {
                    IWin32Window owner = FindForm();
                    if (owner == null)
                    {
                        owner = this;
                    }
                    if (dlg.ShowDialog(owner) == DialogResult.OK)
                    {
                        await ReloadAsync().ConfigureAwait(true);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to add appointment: {ex.Message}", "Appointments", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedAppointment();
            if (selected == null)
            {
                MessageBox.Show("Select an appointment first.", "Appointments", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _editingAppointmentId = selected.AppointmentID;
            PopulateDetails(selected);
            SetEditorMode(AppointmentEditorMode.EditExisting);
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (_editorMode != AppointmentEditorMode.EditExisting)
            {
                return;
            }

            var appointmentId = _editingAppointmentId.GetValueOrDefault();
            var source = _allAppointments.FirstOrDefault(x => x.AppointmentID == appointmentId);
            if (source == null)
            {
                MessageBox.Show("Unable to determine selected appointment.", "Appointments", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(cboStatus.Text))
            {
                MessageBox.Show("Status is required.", "Appointments", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(cboType.Text))
            {
                MessageBox.Show("Appointment type is required.", "Appointments", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                UseWaitCursor = true;
                var payload = new Appointment
                {
                    AppointmentID = source.AppointmentID,
                    AppointmentCode = source.AppointmentCode,
                    PatientID = source.PatientID,
                    DoctorID = source.DoctorID,
                    AppointmentDate = dtpDate.Value.Date,
                    AppointmentTime = dtpTime.Value.TimeOfDay,
                    AppointmentType = cboType.Text.Trim(),
                    Status = cboStatus.Text.Trim(),
                    Reason = txtReason.Text.Trim(),
                    Duration = Convert.ToInt32(nudDuration.Value),
                    CreatedBy = source.CreatedBy,
                    CreatedDate = source.CreatedDate,
                    Notes = txtNotes.Text.Trim()
                };

                var updated = await _service.UpdateAsync(payload).ConfigureAwait(true);
                if (!updated)
                {
                    MessageBox.Show("No changes were saved. Please refresh and try again.", "Appointments", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                await ReloadAsync(payload.AppointmentID).ConfigureAwait(true);
                MessageBox.Show("Appointment updated successfully.", "Appointments", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to save appointment: {ex.Message}", "Appointments", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SetEditorMode(AppointmentEditorMode.View);
            PopulateDetails(GetSelectedAppointment());
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (_editorMode != AppointmentEditorMode.View)
            {
                MessageBox.Show("Finish or cancel editing before deleting.", "Appointments", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selected = GetSelectedAppointment();
            if (selected == null)
            {
                MessageBox.Show("Select an appointment to delete.", "Appointments", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show("Delete this appointment?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            try
            {
                UseWaitCursor = true;
                await _service.DeleteAsync(selected.AppointmentID).ConfigureAwait(true);
                await ReloadAsync().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to delete appointment: {ex.Message}", "Appointments", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void dgvAppointments_SelectionChanged(object sender, EventArgs e)
        {
            if (_editorMode != AppointmentEditorMode.View)
            {
                return;
            }

            PopulateDetails(GetSelectedAppointment());
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
            ThemeManager.StyleSearchTextBox(txtSearch, "Search appointment / patient / doctor");
            if (_searchFilter != null)
            {
                ThemeManager.StyleComboBox(_searchFilter);
            }
        }
    }
}
