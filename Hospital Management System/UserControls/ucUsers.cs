using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using HospitalManagementSystem.BLL.Services;
using HospitalManagementSystem.Forms.Shared;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.UserControls
{
    public partial class ucUsers : UserControl
    {
        private readonly BindingList<User> _users = new BindingList<User>();
        private readonly UserService _service = new UserService();
        private readonly Timer _searchTimer = new Timer();

        public ucUsers()
        {
            InitializeComponent();
            dgvUsers.AutoGenerateColumns = false;
            dgvUsers.DataSource = _users;

            Load += ucUsers_Load;
            btnAdd.Click += btnAdd_Click;
            btnEdit.Click += btnEdit_Click;
            btnDelete.Click += btnDelete_Click;

            _searchTimer.Interval = 300;
            _searchTimer.Tick += SearchTimer_Tick;
            txtSearch.TextChanged += txtSearch_TextChanged;
        }

        private async void ucUsers_Load(object sender, EventArgs e)
        {
            await ReloadAsync(null).ConfigureAwait(true);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            _searchTimer.Stop();
            _searchTimer.Start();
        }

        private async void SearchTimer_Tick(object sender, EventArgs e)
        {
            _searchTimer.Stop();
            await ReloadAsync(txtSearch.Text.Trim()).ConfigureAwait(true);
        }

        private async Task ReloadAsync(string query)
        {
            try
            {
                UseWaitCursor = true;
                _users.Clear();
                var list = await _service.SearchAsync(query).ConfigureAwait(true);
                foreach (var item in list)
                {
                    _users.Add(item);
                }
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var dlg = new frmUserEdit())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _ = ReloadAsync(txtSearch.Text.Trim());
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow?.DataBoundItem is User user)
            {
                using (var dlg = new frmUserEdit(user))
                {
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        _ = ReloadAsync(txtSearch.Text.Trim());
                    }
                }
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow?.DataBoundItem is User user)
            {
                var confirm = MessageBox.Show("Delete this user?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm != DialogResult.Yes)
                {
                    return;
                }

                await _service.DeleteAsync(user.UserID).ConfigureAwait(true);
                await ReloadAsync(txtSearch.Text.Trim()).ConfigureAwait(true);
            }
        }
    }
}
