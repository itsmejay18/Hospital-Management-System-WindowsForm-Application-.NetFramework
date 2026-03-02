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
    public partial class ucUsers : UserControl
    {
        private readonly BindingList<User> _users = new BindingList<User>();
        private readonly BindingList<UserRole> _roles = new BindingList<UserRole>();
        private readonly UserService _service = new UserService();

        private readonly List<User> _allUsers = new List<User>();
        private UserEditorMode _editorMode = UserEditorMode.View;
        private int? _editingUserId;
        private ComboBox _searchFilter;
        private PictureBox _picProfileImage;
        private Button _btnUploadPhoto;
        private byte[] _profileImageBytes;
        private bool _profileImageDirty;
        private UserDetail _selectedUserDetail;

        private enum UserEditorMode
        {
            View = 0,
            AddNew = 1,
            EditExisting = 2
        }

        private enum UserSearchFilter
        {
            All = 0,
            Username = 1,
            Email = 2,
            Role = 3,
            Status = 4
        }

        public ucUsers()
        {
            InitializeComponent();
            ConfigureGrid();
            ConfigureSearchFilter();
            ConfigureImageSection();
            HookEvents();
            ApplyTheme();
            SetEditorMode(UserEditorMode.View);
            Load += ucUsers_Load;
        }

        private void ConfigureGrid()
        {
            dgvUsers.AutoGenerateColumns = false;
            dgvUsers.DataSource = _users;
            dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            colUserId.FillWeight = 10F;
            colUsername.FillWeight = 21F;
            colEmail.FillWeight = 26F;
            colRoleId.FillWeight = 16F;
            colActive.FillWeight = 11F;
            colLastLogin.FillWeight = 16F;
            cboRole.DisplayMember = "RoleName";
            cboRole.ValueMember = "RoleID";
            cboRole.DataSource = _roles;
            colLastLogin.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";
            dgvUsers.CellFormatting += dgvUsers_CellFormatting;
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
                "Username",
                "Email",
                "Role",
                "Status"
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
            dgvUsers.SelectionChanged += dgvUsers_SelectionChanged;
        }

        private async void ucUsers_Load(object sender, EventArgs e)
        {
            await LoadRolesAsync().ConfigureAwait(true);
            await ReloadAsync().ConfigureAwait(true);
        }

        private async Task LoadRolesAsync()
        {
            var roles = await _service.GetRolesAsync().ConfigureAwait(true);
            _roles.RaiseListChangedEvents = false;
            _roles.Clear();
            foreach (var role in roles)
            {
                _roles.Add(role);
            }

            _roles.RaiseListChangedEvents = true;
            _roles.ResetBindings();
        }

        private async Task ReloadAsync(int? selectUserId = null)
        {
            try
            {
                UseWaitCursor = true;
                var query = txtSearch.Text.Trim();
                var list = await _service.GetAllAsync().ConfigureAwait(true);

                _allUsers.Clear();
                _allUsers.AddRange(list);
                ApplyFilter(query, selectUserId);
                SetEditorMode(UserEditorMode.View);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load users: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void ApplyFilter(string searchTerm, int? preferredUserId = null)
        {
            var term = (searchTerm ?? string.Empty).Trim();
            var filtered = string.IsNullOrWhiteSpace(term)
                ? _allUsers
                : _allUsers.Where(user => MatchesSearch(user, term)).ToList();

            _users.RaiseListChangedEvents = false;
            _users.Clear();
            foreach (var user in filtered)
            {
                _users.Add(user);
            }

            _users.RaiseListChangedEvents = true;
            _users.ResetBindings();
            RestoreSelection(preferredUserId);
        }

        private bool MatchesSearch(User user, string searchText)
        {
            switch (GetSelectedSearchFilter())
            {
                case UserSearchFilter.Username:
                    return ContainsInsensitive(user?.Username, searchText);
                case UserSearchFilter.Email:
                    return ContainsInsensitive(user?.Email, searchText);
                case UserSearchFilter.Role:
                    return ContainsInsensitive(GetRoleName(user?.RoleID ?? 0), searchText);
                case UserSearchFilter.Status:
                    return ContainsInsensitive(user != null && user.IsActive ? "Active" : "Inactive", searchText);
                case UserSearchFilter.All:
                default:
                    return ContainsInsensitive(user?.Username, searchText)
                           || ContainsInsensitive(user?.Email, searchText)
                           || ContainsInsensitive(GetRoleName(user?.RoleID ?? 0), searchText)
                           || ContainsInsensitive(user != null && user.IsActive ? "Active" : "Inactive", searchText);
            }
        }

        private UserSearchFilter GetSelectedSearchFilter()
        {
            if (_searchFilter == null || _searchFilter.SelectedIndex < 0)
            {
                return UserSearchFilter.All;
            }

            return (UserSearchFilter)_searchFilter.SelectedIndex;
        }

        private string GetRoleName(int roleId)
        {
            var role = _roles.FirstOrDefault(x => x.RoleID == roleId);
            return role?.RoleName ?? string.Empty;
        }

        private static bool ContainsInsensitive(string value, string searchText)
        {
            return !string.IsNullOrWhiteSpace(value)
                   && value.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void RestoreSelection(int? userId)
        {
            if (dgvUsers.Rows.Count == 0)
            {
                PopulateDetails(null);
                UpdateActionButtons();
                return;
            }

            DataGridViewRow targetRow = null;
            if (userId.HasValue)
            {
                foreach (DataGridViewRow row in dgvUsers.Rows)
                {
                    if (row.DataBoundItem is User user && user.UserID == userId.Value)
                    {
                        targetRow = row;
                        break;
                    }
                }
            }

            if (targetRow == null)
            {
                targetRow = dgvUsers.Rows[0];
            }

            if (targetRow.Cells.Count > 0)
            {
                dgvUsers.CurrentCell = targetRow.Cells[0];
            }

            var selectedUser = targetRow.DataBoundItem as User;
            PopulateDetails(selectedUser);
            _ = LoadSelectedUserDetailAsync(selectedUser);
            UpdateActionButtons();
        }

        private User GetSelectedUser()
        {
            return dgvUsers.CurrentRow?.DataBoundItem as User;
        }

        private void PopulateDetails(User user)
        {
            if (user == null)
            {
                txtUserId.Clear();
                txtUsername.Clear();
                txtEmail.Clear();
                cboRole.SelectedIndex = -1;
                chkActive.Checked = true;
                txtPassword.Clear();
                txtLastLogin.Clear();
                _selectedUserDetail = null;
                SetProfileImage(null, markDirty: false);
                return;
            }

            txtUserId.Text = user.UserID.ToString();
            txtUsername.Text = user.Username;
            txtEmail.Text = user.Email;
            cboRole.SelectedValue = user.RoleID;
            chkActive.Checked = user.IsActive;
            txtPassword.Clear();
            txtLastLogin.Text = user.LastLogin.HasValue
                ? user.LastLogin.Value.ToString("yyyy-MM-dd HH:mm")
                : "-";

            if (_selectedUserDetail == null || _selectedUserDetail.UserID != user.UserID)
            {
                SetProfileImage(null, markDirty: false);
            }
            else
            {
                SetProfileImage(_selectedUserDetail.ProfileImage, markDirty: false);
            }
        }

        private void SetEditorMode(UserEditorMode mode)
        {
            _editorMode = mode;
            if (_editorMode == UserEditorMode.View)
            {
                _editingUserId = null;
            }

            var editable = _editorMode != UserEditorMode.View;
            txtUserId.ReadOnly = true;
            txtUserId.BackColor = ThemeManager.Colors.SurfaceMuted;
            txtLastLogin.ReadOnly = true;
            txtLastLogin.BackColor = ThemeManager.Colors.SurfaceMuted;

            txtUsername.ReadOnly = !editable;
            txtEmail.ReadOnly = !editable;
            txtPassword.ReadOnly = !editable;
            txtUsername.BackColor = editable ? ThemeManager.Colors.Surface : ThemeManager.Colors.SurfaceMuted;
            txtEmail.BackColor = editable ? ThemeManager.Colors.Surface : ThemeManager.Colors.SurfaceMuted;
            txtPassword.BackColor = editable ? ThemeManager.Colors.Surface : ThemeManager.Colors.SurfaceMuted;
            cboRole.Enabled = editable;
            chkActive.Enabled = editable;

            btnSave.Enabled = editable;
            btnCancel.Enabled = editable;
            if (_btnUploadPhoto != null)
            {
                _btnUploadPhoto.Enabled = editable;
            }
            txtSearch.Enabled = !editable;
            btnSearch.Enabled = !editable;
            btnRefresh.Enabled = !editable;
            dgvUsers.Enabled = !editable;

            UpdateActionButtons();
            switch (_editorMode)
            {
                case UserEditorMode.AddNew:
                    lblDetailsHint.Text = "Creating user. Password is required for new user.";
                    break;
                case UserEditorMode.EditExisting:
                    lblDetailsHint.Text = "Editing user. Leave Password blank to keep current password.";
                    break;
                default:
                    lblDetailsHint.Text = "Select a user to view details. Click Edit to unlock fields.";
                    break;
            }
        }

        private void UpdateActionButtons()
        {
            var hasSelection = GetSelectedUser() != null;
            var viewMode = _editorMode == UserEditorMode.View;

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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            _editingUserId = null;
            _selectedUserDetail = null;
            txtUserId.Text = "(new)";
            txtUsername.Clear();
            txtEmail.Clear();
            txtPassword.Clear();
            txtLastLogin.Text = "-";
            chkActive.Checked = true;
            SetProfileImage(null, markDirty: false);
            if (_roles.Count > 0)
            {
                cboRole.SelectedIndex = 0;
            }

            SetEditorMode(UserEditorMode.AddNew);
            txtUsername.Focus();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedUser();
            if (selected == null)
            {
                MessageBox.Show("Select a user first.", "Users", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _editingUserId = selected.UserID;
            PopulateDetails(selected);
            SetEditorMode(UserEditorMode.EditExisting);
            txtUsername.Focus();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (_editorMode == UserEditorMode.View)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Username is required.", "Users", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!(cboRole.SelectedValue is int roleId) || roleId <= 0)
            {
                MessageBox.Show("Role is required.", "Users", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                UseWaitCursor = true;
                if (_editorMode == UserEditorMode.AddNew)
                {
                    if (string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        MessageBox.Show("Password is required for new users.", "Users", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var payload = new User
                    {
                        Username = txtUsername.Text.Trim(),
                        Email = txtEmail.Text.Trim(),
                        RoleID = roleId,
                        IsActive = chkActive.Checked,
                        LastLogin = null,
                        PasswordHash = txtPassword.Text
                    };

                    var newId = await _service.AddAsync(payload).ConfigureAwait(true);
                    await SaveProfileImageAsync(newId, payload.Username).ConfigureAwait(true);
                    await ReloadAsync(newId).ConfigureAwait(true);
                    MessageBox.Show("User added successfully.", "Users", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var userId = _editingUserId.GetValueOrDefault();
                var source = _allUsers.FirstOrDefault(x => x.UserID == userId);
                if (source == null)
                {
                    MessageBox.Show("Unable to resolve selected user record.", "Users", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var payloadUpdate = new User
                {
                    UserID = source.UserID,
                    Username = txtUsername.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    RoleID = roleId,
                    IsActive = chkActive.Checked,
                    LastLogin = source.LastLogin,
                    PasswordHash = string.IsNullOrWhiteSpace(txtPassword.Text)
                        ? source.PasswordHash
                        : txtPassword.Text
                };

                var updated = await _service.UpdateAsync(payloadUpdate).ConfigureAwait(true);
                var imageChanged = _profileImageDirty;
                if (!updated && !imageChanged)
                {
                    MessageBox.Show("No changes were saved. Please refresh and try again.", "Users", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                await SaveProfileImageAsync(payloadUpdate.UserID, payloadUpdate.Username).ConfigureAwait(true);
                await ReloadAsync(payloadUpdate.UserID).ConfigureAwait(true);
                MessageBox.Show("User updated successfully.", "Users", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to save user: {ex.Message}", "Users", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SetEditorMode(UserEditorMode.View);
            var selectedUser = GetSelectedUser();
            PopulateDetails(selectedUser);
            _ = LoadSelectedUserDetailAsync(selectedUser);
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (_editorMode != UserEditorMode.View)
            {
                MessageBox.Show("Finish or cancel editing before deleting.", "Users", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selected = GetSelectedUser();
            if (selected == null)
            {
                MessageBox.Show("Select a user to delete.", "Users", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show("Delete this user?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            try
            {
                UseWaitCursor = true;
                await _service.DeleteAsync(selected.UserID).ConfigureAwait(true);
                await ReloadAsync().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to delete user: {ex.Message}", "Users", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void dgvUsers_SelectionChanged(object sender, EventArgs e)
        {
            if (_editorMode != UserEditorMode.View)
            {
                return;
            }

            var selectedUser = GetSelectedUser();
            PopulateDetails(selectedUser);
            _ = LoadSelectedUserDetailAsync(selectedUser);
            UpdateActionButtons();
        }

        private void dgvUsers_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != colRoleId.Index || e.Value == null)
            {
                return;
            }

            if (!(e.Value is int roleId))
            {
                return;
            }

            var role = _roles.FirstOrDefault(x => x.RoleID == roleId);
            if (role == null)
            {
                return;
            }

            e.Value = role.RoleName;
            e.FormattingApplied = true;
        }

        private async Task LoadSelectedUserDetailAsync(User user)
        {
            if (user == null || user.UserID <= 0)
            {
                _selectedUserDetail = null;
                SetProfileImage(null, markDirty: false);
                return;
            }

            try
            {
                _selectedUserDetail = await _service.GetUserDetailAsync(user.UserID).ConfigureAwait(true);
                SetProfileImage(_selectedUserDetail?.ProfileImage, markDirty: false);
            }
            catch
            {
                _selectedUserDetail = null;
                SetProfileImage(null, markDirty: false);
            }
        }

        private async Task SaveProfileImageAsync(int userId, string username)
        {
            if (!_profileImageDirty || userId <= 0)
            {
                return;
            }

            var detail = _selectedUserDetail;
            if (detail == null || detail.UserID != userId)
            {
                detail = await _service.GetUserDetailAsync(userId).ConfigureAwait(true);
            }

            if (detail == null)
            {
                var names = BuildFallbackName(username);
                detail = new UserDetail
                {
                    UserID = userId,
                    FirstName = names.Item1,
                    LastName = names.Item2,
                    ProfileImage = CloneBytes(_profileImageBytes)
                };

                await _service.AddUserDetailAsync(detail).ConfigureAwait(true);
                _selectedUserDetail = detail;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(detail.FirstName) || string.IsNullOrWhiteSpace(detail.LastName))
                {
                    var names = BuildFallbackName(username);
                    if (string.IsNullOrWhiteSpace(detail.FirstName))
                    {
                        detail.FirstName = names.Item1;
                    }

                    if (string.IsNullOrWhiteSpace(detail.LastName))
                    {
                        detail.LastName = names.Item2;
                    }
                }

                detail.ProfileImage = CloneBytes(_profileImageBytes);
                await _service.UpdateUserDetailAsync(detail).ConfigureAwait(true);
                _selectedUserDetail = detail;
            }

            _profileImageDirty = false;
        }

        private static Tuple<string, string> BuildFallbackName(string username)
        {
            var cleaned = (username ?? string.Empty).Trim().Replace('.', ' ').Replace('_', ' ');
            if (string.IsNullOrWhiteSpace(cleaned))
            {
                return Tuple.Create("Hospital", "User");
            }

            var tokens = cleaned.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 1)
            {
                return Tuple.Create(tokens[0], "User");
            }

            return Tuple.Create(tokens[0], string.Join(" ", tokens.Skip(1)));
        }

        private void btnUploadPhoto_Click(object sender, EventArgs e)
        {
            if (_editorMode == UserEditorMode.View)
            {
                MessageBox.Show("Click Add or Edit to upload an image.", "Users", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Select User Image";
                dialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp";
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    SetProfileImage(File.ReadAllBytes(dialog.FileName), markDirty: true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unable to load image: {ex.Message}", "Users", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SetProfileImage(byte[] bytes, bool markDirty)
        {
            _profileImageBytes = CloneBytes(bytes);
            _profileImageDirty = markDirty;

            if (_picProfileImage == null)
            {
                return;
            }

            var oldImage = _picProfileImage.Image;
            _picProfileImage.Image = CreateImageFromBytes(_profileImageBytes);
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
            ThemeManager.StyleSearchTextBox(txtSearch, "Search username / email");
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
