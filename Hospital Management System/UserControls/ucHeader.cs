using System;
using System.Windows.Forms;

namespace HospitalManagementSystem.UserControls
{
    public partial class ucHeader : UserControl
    {
        public ucHeader()
        {
            InitializeComponent();
        }

        public event EventHandler LogoutClicked;

        public void SetTitle(string title)
        {
            lblTitle.Text = title;
        }

        public void SetUser(string username, string roleName)
        {
            lblUser.Text = $"User: {username} ({roleName})";
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            LogoutClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
