using System;
using System.Drawing;
using System.Windows.Forms;
using HospitalManagementSystem.Helpers;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.Forms.Shared
{
    /// <summary>
    /// Add/edit dialog for room records.
    /// </summary>
    public sealed class frmRoomEdit : Form
    {
        private readonly Room _editingRoom;

        private TextBox txtRoomNumber;
        private NumericUpDown nudWardId;
        private TextBox txtRoomType;
        private NumericUpDown nudTotalBeds;
        private NumericUpDown nudAvailableBeds;
        private NumericUpDown nudRatePerDay;
        private ComboBox cboStatus;
        private TextBox txtFacilities;
        private Button btnSave;
        private Button btnCancel;

        /// <summary>
        /// Gets the edited room record.
        /// </summary>
        public Room Room { get; private set; }

        /// <summary>
        /// Initializes a new room edit form.
        /// </summary>
        public frmRoomEdit(Room room = null)
        {
            _editingRoom = room;
            InitializeLayout();
            ApplyTheme();
            LoadRoom();
        }

        private void InitializeLayout()
        {
            Text = _editingRoom == null ? "Add Room" : "Edit Room";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(500, 470);
            Font = new Font("Segoe UI", 9F);
            BackColor = Color.WhiteSmoke;

            var lblRoomNumber = CreateLabel("Room Number", 24, 24);
            txtRoomNumber = CreateTextBox(24, 44, 200);

            var lblWardId = CreateLabel("Ward ID (optional)", 248, 24);
            nudWardId = new NumericUpDown
            {
                Left = 248,
                Top = 44,
                Width = 200,
                Minimum = 0,
                Maximum = 100000
            };

            var lblRoomType = CreateLabel("Room Type", 24, 84);
            txtRoomType = CreateTextBox(24, 104, 200);

            var lblStatus = CreateLabel("Status", 248, 84);
            cboStatus = new ComboBox
            {
                Left = 248,
                Top = 104,
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboStatus.Items.AddRange(new object[] { "Available", "Occupied", "Maintenance" });
            cboStatus.SelectedIndex = 0;

            var lblTotalBeds = CreateLabel("Total Beds", 24, 144);
            nudTotalBeds = new NumericUpDown
            {
                Left = 24,
                Top = 164,
                Width = 200,
                Minimum = 1,
                Maximum = 200,
                Value = 1
            };
            nudTotalBeds.ValueChanged += nudTotalBeds_ValueChanged;

            var lblAvailableBeds = CreateLabel("Available Beds", 248, 144);
            nudAvailableBeds = new NumericUpDown
            {
                Left = 248,
                Top = 164,
                Width = 200,
                Minimum = 0,
                Maximum = 200,
                Value = 1
            };

            var lblRate = CreateLabel("Rate Per Day", 24, 204);
            nudRatePerDay = new NumericUpDown
            {
                Left = 24,
                Top = 224,
                Width = 200,
                Minimum = 0,
                Maximum = 1000000,
                DecimalPlaces = 2,
                Increment = 10
            };

            var lblFacilities = CreateLabel("Facilities", 24, 264);
            txtFacilities = new TextBox
            {
                Left = 24,
                Top = 284,
                Width = 424,
                Height = 100,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            btnSave = new Button
            {
                Text = "Save",
                Left = 248,
                Top = 410,
                Width = 95,
                Height = 32
            };
            ThemeManager.StyleButton(btnSave, ThemeButtonKind.Primary);
            btnSave.Click += btnSave_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                Left = 353,
                Top = 410,
                Width = 95,
                Height = 32
            };
            ThemeManager.StyleButton(btnCancel, ThemeButtonKind.Secondary);
            btnCancel.Click += (_, __) => DialogResult = DialogResult.Cancel;

            Controls.Add(lblRoomNumber);
            Controls.Add(txtRoomNumber);
            Controls.Add(lblWardId);
            Controls.Add(nudWardId);
            Controls.Add(lblRoomType);
            Controls.Add(txtRoomType);
            Controls.Add(lblStatus);
            Controls.Add(cboStatus);
            Controls.Add(lblTotalBeds);
            Controls.Add(nudTotalBeds);
            Controls.Add(lblAvailableBeds);
            Controls.Add(nudAvailableBeds);
            Controls.Add(lblRate);
            Controls.Add(nudRatePerDay);
            Controls.Add(lblFacilities);
            Controls.Add(txtFacilities);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
        }

        private static Label CreateLabel(string text, int left, int top)
        {
            return new Label
            {
                Text = text,
                Left = left,
                Top = top,
                AutoSize = true,
                ForeColor = Color.FromArgb(33, 49, 65)
            };
        }

        private static TextBox CreateTextBox(int left, int top, int width)
        {
            return new TextBox
            {
                Left = left,
                Top = top,
                Width = width
            };
        }

        private void LoadRoom()
        {
            if (_editingRoom == null)
            {
                return;
            }

            txtRoomNumber.Text = _editingRoom.RoomNumber;
            nudWardId.Value = _editingRoom.WardID.GetValueOrDefault();
            txtRoomType.Text = _editingRoom.RoomType;
            nudTotalBeds.Value = _editingRoom.TotalBeds <= 0 ? 1 : _editingRoom.TotalBeds;
            nudAvailableBeds.Maximum = nudTotalBeds.Value;
            nudAvailableBeds.Value = _editingRoom.AvailableBeds < 0
                ? 0
                : Math.Min((decimal)_editingRoom.AvailableBeds, nudAvailableBeds.Maximum);
            nudRatePerDay.Value = Math.Min((decimal)(_editingRoom.RatePerDay ?? 0m), nudRatePerDay.Maximum);
            cboStatus.SelectedItem = string.IsNullOrWhiteSpace(_editingRoom.Status) ? "Available" : _editingRoom.Status;
            txtFacilities.Text = _editingRoom.Facilities;
        }

        private void nudTotalBeds_ValueChanged(object sender, EventArgs e)
        {
            nudAvailableBeds.Maximum = nudTotalBeds.Value;
            if (nudAvailableBeds.Value > nudAvailableBeds.Maximum)
            {
                nudAvailableBeds.Value = nudAvailableBeds.Maximum;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRoomNumber.Text))
            {
                MessageBox.Show("Room number is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRoomNumber.Focus();
                return;
            }

            Room = _editingRoom ?? new Room();
            Room.RoomNumber = txtRoomNumber.Text.Trim();
            Room.WardID = nudWardId.Value <= 0 ? (int?)null : Convert.ToInt32(nudWardId.Value);
            Room.RoomType = txtRoomType.Text.Trim();
            Room.TotalBeds = Convert.ToInt32(nudTotalBeds.Value);
            Room.AvailableBeds = Convert.ToInt32(nudAvailableBeds.Value);
            Room.RatePerDay = nudRatePerDay.Value;
            Room.Status = cboStatus.SelectedItem?.ToString();
            Room.Facilities = txtFacilities.Text.Trim();

            DialogResult = DialogResult.OK;
        }

        private void ApplyTheme()
        {
            ThemeManager.ApplyFormTheme(this);
        }
    }
}
