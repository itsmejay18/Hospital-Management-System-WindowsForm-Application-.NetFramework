using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using HospitalManagementSystem.BLL.Services;
using HospitalManagementSystem.Forms.Shared;
using HospitalManagementSystem.Helpers;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.UserControls
{
    /// <summary>
    /// Manages room records and patient room transactions.
    /// </summary>
    public sealed class ucRooms : UserControl
    {
        private readonly RoomService _service = new RoomService();
        private readonly BindingList<Room> _rooms = new BindingList<Room>();
        private readonly BindingList<Admission> _admissions = new BindingList<Admission>();

        private TextBox txtRoomSearch;
        private Button btnRoomSearch;
        private Button btnRoomRefresh;
        private Button btnRoomAdd;
        private Button btnRoomEdit;
        private Button btnRoomDelete;
        private DataGridView dgvRooms;

        private ComboBox cboPatient;
        private ComboBox cboDoctor;
        private ComboBox cboRoom;
        private DateTimePicker dtpExpectedDischarge;
        private TextBox txtAdmissionReason;
        private TextBox txtDiagnosis;
        private Button btnAdmit;

        private TextBox txtAdmissionSearch;
        private Button btnAdmissionSearch;
        private Button btnAdmissionRefresh;
        private TextBox txtDischargeSummary;
        private Button btnDischarge;
        private DataGridView dgvAdmissions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ucRooms"/> class.
        /// </summary>
        public ucRooms()
        {
            InitializeLayout();
            ApplyTheme();
            HookEvents();
            Load += ucRooms_Load;
        }

        private void InitializeLayout()
        {
            BackColor = ThemeManager.Colors.Background;

            var tabs = new TabControl
            {
                Dock = DockStyle.Fill,
                Appearance = TabAppearance.Normal
            };

            var tabRooms = new TabPage("Rooms")
            {
                BackColor = BackColor
            };
            tabRooms.Controls.Add(BuildRoomGridPanel());
            tabRooms.Controls.Add(BuildRoomActionPanel());
            tabRooms.Controls.Add(BuildRoomSearchPanel());

            var tabTransactions = new TabPage("Admissions")
            {
                BackColor = BackColor
            };
            tabTransactions.Controls.Add(BuildAdmissionGridPanel());
            tabTransactions.Controls.Add(BuildAdmissionSearchPanel());
            tabTransactions.Controls.Add(BuildAdmitPanel());

            tabs.TabPages.Add(tabRooms);
            tabs.TabPages.Add(tabTransactions);

            Controls.Add(tabs);
        }

        private Control BuildRoomSearchPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 54,
                Padding = new Padding(12, 12, 12, 8),
                BackColor = ThemeManager.Colors.Surface
            };

            var lbl = new Label
            {
                Text = "Search Rooms:",
                Left = 12,
                Top = 18,
                AutoSize = true
            };

            txtRoomSearch = new TextBox
            {
                Left = 108,
                Top = 14,
                Width = 280,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            btnRoomSearch = CreatePrimaryButton("Search", 396, 12, 90);
            btnRoomRefresh = CreateOutlineButton("Refresh", 492, 12, 90);
            btnRoomSearch.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRoomRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            panel.Controls.Add(lbl);
            panel.Controls.Add(txtRoomSearch);
            panel.Controls.Add(btnRoomSearch);
            panel.Controls.Add(btnRoomRefresh);

            void LayoutSearchBar()
            {
                var right = panel.ClientSize.Width - 12;
                btnRoomRefresh.Left = Math.Max(400, right - btnRoomRefresh.Width);
                btnRoomSearch.Left = btnRoomRefresh.Left - btnRoomSearch.Width - 8;
                txtRoomSearch.Width = Math.Max(180, btnRoomSearch.Left - txtRoomSearch.Left - 8);
            }

            panel.Resize += (_, __) => LayoutSearchBar();
            panel.HandleCreated += (_, __) => LayoutSearchBar();
            return panel;
        }

        private Control BuildRoomActionPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(12, 8, 12, 8),
                BackColor = ThemeManager.Colors.Surface
            };

            btnRoomAdd = CreatePrimaryButton("Add Room", 12, 10, 110);
            btnRoomEdit = CreateOutlineButton("Edit Room", 128, 10, 110);
            btnRoomDelete = CreateDangerButton("Delete Room", 244, 10, 110);

            panel.Controls.Add(btnRoomAdd);
            panel.Controls.Add(btnRoomEdit);
            panel.Controls.Add(btnRoomDelete);
            return panel;
        }

        private Control BuildRoomGridPanel()
        {
            dgvRooms = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = false,
                BackgroundColor = ThemeManager.Colors.Surface,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false
            };

            dgvRooms.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "RoomNumber",
                HeaderText = "Room",
                Width = 110
            });
            dgvRooms.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "WardName",
                HeaderText = "Ward",
                Width = 140
            });
            dgvRooms.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "RoomType",
                HeaderText = "Type",
                Width = 120
            });
            dgvRooms.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TotalBeds",
                HeaderText = "Total Beds",
                Width = 90
            });
            dgvRooms.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "AvailableBeds",
                HeaderText = "Available",
                Width = 90
            });
            dgvRooms.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "RatePerDay",
                HeaderText = "Rate / Day",
                Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            });
            dgvRooms.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Status",
                HeaderText = "Status",
                Width = 110
            });
            dgvRooms.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Facilities",
                HeaderText = "Facilities",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dgvRooms.DataSource = _rooms;
            return dgvRooms;
        }

        private Control BuildAdmitPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 190,
                Padding = new Padding(12, 8, 12, 8),
                BackColor = ThemeManager.Colors.Surface
            };

            var lblPatient = CreatePanelLabel("Patient", 14, 14);
            cboPatient = new ComboBox
            {
                Left = 14,
                Top = 34,
                Width = 220,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            var lblDoctor = CreatePanelLabel("Doctor", 248, 14);
            cboDoctor = new ComboBox
            {
                Left = 248,
                Top = 34,
                Width = 220,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            var lblRoom = CreatePanelLabel("Room", 482, 14);
            cboRoom = new ComboBox
            {
                Left = 482,
                Top = 34,
                Width = 220,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            var lblExpected = CreatePanelLabel("Expected Discharge", 14, 70);
            dtpExpectedDischarge = new DateTimePicker
            {
                Left = 14,
                Top = 90,
                Width = 220,
                Format = DateTimePickerFormat.Short,
                ShowCheckBox = true
            };

            var lblReason = CreatePanelLabel("Admission Reason", 248, 70);
            txtAdmissionReason = new TextBox
            {
                Left = 248,
                Top = 90,
                Width = 454
            };

            var lblDiagnosis = CreatePanelLabel("Initial Diagnosis", 14, 124);
            txtDiagnosis = new TextBox
            {
                Left = 14,
                Top = 144,
                Width = 688
            };

            btnAdmit = CreatePrimaryButton("Admit Patient", 714, 142, 130);
            btnAdmit.Height = 30;

            panel.Controls.Add(lblPatient);
            panel.Controls.Add(cboPatient);
            panel.Controls.Add(lblDoctor);
            panel.Controls.Add(cboDoctor);
            panel.Controls.Add(lblRoom);
            panel.Controls.Add(cboRoom);
            panel.Controls.Add(lblExpected);
            panel.Controls.Add(dtpExpectedDischarge);
            panel.Controls.Add(lblReason);
            panel.Controls.Add(txtAdmissionReason);
            panel.Controls.Add(lblDiagnosis);
            panel.Controls.Add(txtDiagnosis);
            panel.Controls.Add(btnAdmit);
            return panel;
        }

        private Control BuildAdmissionSearchPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 54,
                Padding = new Padding(12, 10, 12, 8),
                BackColor = ThemeManager.Colors.Surface
            };

            var lblSearch = new Label
            {
                Text = "Search:",
                Left = 12,
                Top = 18,
                AutoSize = true
            };
            txtAdmissionSearch = new TextBox
            {
                Left = 66,
                Top = 14,
                Width = 210,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            btnAdmissionSearch = CreatePrimaryButton("Search", 282, 12, 85);
            btnAdmissionRefresh = CreateOutlineButton("Refresh", 373, 12, 85);
            btnAdmissionSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnAdmissionRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            var lblDischarge = new Label
            {
                Text = "Discharge Summary:",
                Left = 478,
                Top = 18,
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            txtDischargeSummary = new TextBox
            {
                Left = 598,
                Top = 14,
                Width = 210,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            btnDischarge = CreateDangerButton("Discharge", 816, 12, 110);
            btnDischarge.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            panel.Controls.Add(lblSearch);
            panel.Controls.Add(txtAdmissionSearch);
            panel.Controls.Add(btnAdmissionSearch);
            panel.Controls.Add(btnAdmissionRefresh);
            panel.Controls.Add(lblDischarge);
            panel.Controls.Add(txtDischargeSummary);
            panel.Controls.Add(btnDischarge);

            void LayoutAdmissionSearchBar()
            {
                var right = panel.ClientSize.Width - 12;
                btnDischarge.Left = Math.Max(560, right - btnDischarge.Width);
                txtDischargeSummary.Left = btnDischarge.Left - txtDischargeSummary.Width - 8;
                lblDischarge.Left = txtDischargeSummary.Left - lblDischarge.PreferredWidth - 8;
            }

            panel.Resize += (_, __) => LayoutAdmissionSearchBar();
            panel.HandleCreated += (_, __) => LayoutAdmissionSearchBar();
            return panel;
        }

        private Control BuildAdmissionGridPanel()
        {
            dgvAdmissions = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = false,
                BackgroundColor = ThemeManager.Colors.Surface,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false
            };

            dgvAdmissions.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "AdmissionNumber",
                HeaderText = "Admission #",
                Width = 120
            });
            dgvAdmissions.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "PatientName",
                HeaderText = "Patient",
                Width = 190
            });
            dgvAdmissions.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DoctorName",
                HeaderText = "Doctor",
                Width = 180
            });
            dgvAdmissions.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "RoomNumber",
                HeaderText = "Room",
                Width = 80
            });
            dgvAdmissions.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "AdmissionDate",
                HeaderText = "Admission Date",
                Width = 130,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd HH:mm" }
            });
            dgvAdmissions.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ExpectedDischargeDate",
                HeaderText = "Expected Discharge",
                Width = 130,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd" }
            });
            dgvAdmissions.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Status",
                HeaderText = "Status",
                Width = 110
            });
            dgvAdmissions.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "AdmissionReason",
                HeaderText = "Reason",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dgvAdmissions.DataSource = _admissions;
            return dgvAdmissions;
        }

        private static Label CreatePanelLabel(string text, int left, int top)
        {
            return new Label
            {
                Text = text,
                Left = left,
                Top = top,
                AutoSize = true,
                ForeColor = ThemeManager.Colors.TextPrimary
            };
        }

        private static Button CreatePrimaryButton(string text, int left, int top, int width)
        {
            var button = new Button
            {
                Text = text,
                Left = left,
                Top = top,
                Width = width,
                Height = 30,
                BackColor = ThemeManager.Colors.Primary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            ThemeManager.StyleButton(button, ThemeButtonKind.Primary);
            return button;
        }

        private static Button CreateOutlineButton(string text, int left, int top, int width)
        {
            var button = new Button
            {
                Text = text,
                Left = left,
                Top = top,
                Width = width,
                Height = 30,
                BackColor = ThemeManager.Colors.Surface,
                ForeColor = ThemeManager.Colors.TextPrimary,
                FlatStyle = FlatStyle.Flat
            };
            ThemeManager.StyleButton(button, ThemeButtonKind.Secondary);
            return button;
        }

        private static Button CreateDangerButton(string text, int left, int top, int width)
        {
            var button = new Button
            {
                Text = text,
                Left = left,
                Top = top,
                Width = width,
                Height = 30,
                BackColor = ThemeManager.Colors.Danger,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            ThemeManager.StyleButton(button, ThemeButtonKind.Danger);
            return button;
        }

        private void HookEvents()
        {
            btnRoomSearch.Click += async (_, __) => await LoadRoomsAsync(txtRoomSearch.Text.Trim()).ConfigureAwait(true);
            btnRoomRefresh.Click += async (_, __) =>
            {
                txtRoomSearch.Clear();
                await LoadRoomsAsync().ConfigureAwait(true);
            };
            btnRoomAdd.Click += btnRoomAdd_Click;
            btnRoomEdit.Click += btnRoomEdit_Click;
            btnRoomDelete.Click += btnRoomDelete_Click;

            btnAdmit.Click += btnAdmit_Click;
            btnAdmissionSearch.Click += async (_, __) => await LoadAdmissionsAsync(txtAdmissionSearch.Text.Trim()).ConfigureAwait(true);
            btnAdmissionRefresh.Click += async (_, __) =>
            {
                txtAdmissionSearch.Clear();
                await LoadAdmissionsAsync().ConfigureAwait(true);
            };
            btnDischarge.Click += btnDischarge_Click;
        }

        private async void ucRooms_Load(object sender, EventArgs e)
        {
            await ReloadAllAsync().ConfigureAwait(true);
        }

        private async Task ReloadAllAsync()
        {
            try
            {
                UseWaitCursor = true;
                await LoadRoomsAsync().ConfigureAwait(true);
                await LoadAdmissionsAsync().ConfigureAwait(true);
                await LoadLookupsAsync().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load room module: {ex.Message}", "Rooms", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private async Task LoadRoomsAsync(string searchText = null)
        {
            _rooms.Clear();
            var rooms = await _service.GetRoomsAsync(searchText).ConfigureAwait(true);
            foreach (var room in rooms)
            {
                _rooms.Add(room);
            }
        }

        private async Task LoadAdmissionsAsync(string searchText = null)
        {
            _admissions.Clear();
            var admissions = await _service.GetAdmissionsAsync(searchText, activeOnly: true).ConfigureAwait(true);
            foreach (var admission in admissions)
            {
                _admissions.Add(admission);
            }
        }

        private async Task LoadLookupsAsync()
        {
            var patients = await _service.GetPatientLookupAsync().ConfigureAwait(true);
            var doctors = await _service.GetDoctorLookupAsync().ConfigureAwait(true);
            var rooms = await _service.GetAvailableRoomLookupAsync().ConfigureAwait(true);

            cboPatient.DataSource = patients;
            cboPatient.DisplayMember = "Name";
            cboPatient.ValueMember = "Id";

            cboDoctor.DataSource = doctors;
            cboDoctor.DisplayMember = "Name";
            cboDoctor.ValueMember = "Id";

            cboRoom.DataSource = rooms;
            cboRoom.DisplayMember = "Name";
            cboRoom.ValueMember = "Id";
        }

        private async void btnRoomAdd_Click(object sender, EventArgs e)
        {
            using (var dialog = new frmRoomEdit())
            {
                IWin32Window owner = FindForm();
                if (owner == null)
                {
                    owner = this;
                }
                if (dialog.ShowDialog(owner) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    UseWaitCursor = true;
                    await _service.AddRoomAsync(dialog.Room).ConfigureAwait(true);
                    await ReloadAllAsync().ConfigureAwait(true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unable to add room: {ex.Message}", "Rooms", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    UseWaitCursor = false;
                }
            }
        }

        private async void btnRoomEdit_Click(object sender, EventArgs e)
        {
            if (!(dgvRooms.CurrentRow?.DataBoundItem is Room selected))
            {
                MessageBox.Show("Select a room to edit.", "Rooms", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var copy = new Room
            {
                RoomID = selected.RoomID,
                RoomNumber = selected.RoomNumber,
                WardID = selected.WardID,
                RoomType = selected.RoomType,
                TotalBeds = selected.TotalBeds,
                AvailableBeds = selected.AvailableBeds,
                Facilities = selected.Facilities,
                RatePerDay = selected.RatePerDay,
                Status = selected.Status
            };

            using (var dialog = new frmRoomEdit(copy))
            {
                IWin32Window owner = FindForm();
                if (owner == null)
                {
                    owner = this;
                }
                if (dialog.ShowDialog(owner) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    UseWaitCursor = true;
                    await _service.UpdateRoomAsync(dialog.Room).ConfigureAwait(true);
                    await ReloadAllAsync().ConfigureAwait(true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unable to update room: {ex.Message}", "Rooms", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    UseWaitCursor = false;
                }
            }
        }

        private async void btnRoomDelete_Click(object sender, EventArgs e)
        {
            if (!(dgvRooms.CurrentRow?.DataBoundItem is Room selected))
            {
                MessageBox.Show("Select a room to delete.", "Rooms", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show(
                $"Delete room {selected.RoomNumber}?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            try
            {
                UseWaitCursor = true;
                var deleted = await _service.DeleteRoomAsync(selected.RoomID).ConfigureAwait(true);
                if (!deleted)
                {
                    MessageBox.Show("Room was not deleted. It may have active references.", "Rooms", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                await ReloadAllAsync().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to delete room: {ex.Message}", "Rooms", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private async void btnAdmit_Click(object sender, EventArgs e)
        {
            if (!(cboPatient.SelectedItem is LookupItem patient))
            {
                MessageBox.Show("Select a patient.", "Admit Patient", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!(cboDoctor.SelectedItem is LookupItem doctor))
            {
                MessageBox.Show("Select a doctor.", "Admit Patient", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!(cboRoom.SelectedItem is LookupItem room))
            {
                MessageBox.Show("Select an available room.", "Admit Patient", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                UseWaitCursor = true;
                var expectedDischarge = dtpExpectedDischarge.Checked ? dtpExpectedDischarge.Value.Date : (DateTime?)null;
                await _service.AdmitPatientAsync(
                        patient.Id,
                        doctor.Id,
                        room.Id,
                        expectedDischarge,
                        txtAdmissionReason.Text.Trim(),
                        txtDiagnosis.Text.Trim())
                    .ConfigureAwait(true);

                txtAdmissionReason.Clear();
                txtDiagnosis.Clear();
                dtpExpectedDischarge.Checked = false;

                await ReloadAllAsync().ConfigureAwait(true);
                MessageBox.Show("Patient admitted successfully.", "Admission", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to admit patient: {ex.Message}", "Admission", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private async void btnDischarge_Click(object sender, EventArgs e)
        {
            if (!(dgvAdmissions.CurrentRow?.DataBoundItem is Admission admission))
            {
                MessageBox.Show("Select an admission to discharge.", "Discharge", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!string.Equals(admission.Status, "Admitted", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Only admitted patients can be discharged.", "Discharge", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show(
                $"Discharge admission {admission.AdmissionNumber}?",
                "Confirm Discharge",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            try
            {
                UseWaitCursor = true;
                var success = await _service.DischargeAdmissionAsync(admission.AdmissionID, txtDischargeSummary.Text.Trim()).ConfigureAwait(true);
                if (!success)
                {
                    MessageBox.Show("Discharge did not complete. The selected record may have changed.", "Discharge", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                txtDischargeSummary.Clear();
                await ReloadAllAsync().ConfigureAwait(true);
                MessageBox.Show("Patient discharged successfully.", "Discharge", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to discharge patient: {ex.Message}", "Discharge", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void ApplyTheme()
        {
            ThemeManager.ApplyControlTheme(this);
            ThemeManager.StyleDataGridView(dgvRooms);
            ThemeManager.StyleDataGridView(dgvAdmissions);
            ThemeManager.StyleSearchTextBox(txtRoomSearch, "Search room number / ward / type");
            ThemeManager.StyleSearchTextBox(txtAdmissionSearch, "Search admission # / patient / room");
            ThemeManager.StyleSearchTextBox(txtDischargeSummary, "Optional discharge note");
        }
    }
}
