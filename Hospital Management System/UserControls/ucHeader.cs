using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using HospitalManagementSystem.Helpers;

namespace HospitalManagementSystem.UserControls
{
    public partial class ucHeader : UserControl
    {
        private TextBox txtSearch;
        private Button btnQuickAdd;
        private Button btnNotify;
        private Button btnProfileMenu;
        private PictureBox picSearchIcon;
        private PictureBox picAvatar;
        private Panel pnlDivider;
        private Panel pnlNotifyDot;
        private bool _searchHintActive;
        private bool _dashboardMode;

        public ucHeader()
        {
            InitializeComponent();
            BuildActions();
            ApplyTheme();
        }

        public event EventHandler LogoutClicked;
        public event EventHandler QuickAddPatientClicked;

        public void SetTitle(string title)
        {
            lblTitle.Text = title;
            ArrangeLayout();
        }

        public void SetUser(string username, string roleName)
        {
            lblUser.Text = $"User: {username} ({roleName})";
            ArrangeLayout();
        }

        public void SetDashboardMode(bool isDashboard)
        {
            _dashboardMode = isDashboard;
            ArrangeLayout();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            LogoutClicked?.Invoke(this, EventArgs.Empty);
        }

        private void BuildActions()
        {
            txtSearch = new TextBox
            {
                Name = "txtSearch",
                Width = 300,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BorderStyle = BorderStyle.None
            };

            btnQuickAdd = new Button
            {
                Name = "btnQuickAdd",
                Text = "+  Add patient",
                Width = 126,
                Height = 34,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            btnNotify = new Button
            {
                Name = "btnNotify",
                Width = 34,
                Height = 34,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Text = string.Empty,
                Image = CreateActionIcon(ActionIconType.Notification),
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            btnProfileMenu = new Button
            {
                Name = "btnProfileMenu",
                Width = 24,
                Height = 34,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat,
                Text = "\u25BE",
                TabStop = false
            };

            picSearchIcon = new PictureBox
            {
                Name = "picSearchIcon",
                Size = new Size(18, 18),
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                SizeMode = PictureBoxSizeMode.CenterImage,
                Image = CreateActionIcon(ActionIconType.Search)
            };

            picAvatar = new PictureBox
            {
                Name = "picAvatar",
                Size = new Size(34, 34),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            pnlNotifyDot = new Panel
            {
                Name = "pnlNotifyDot",
                Size = new Size(8, 8),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            pnlDivider = new Panel
            {
                Name = "pnlDivider",
                Width = 1,
                Height = 28,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            Controls.Add(picSearchIcon);
            Controls.Add(txtSearch);
            Controls.Add(btnQuickAdd);
            Controls.Add(pnlDivider);
            Controls.Add(btnNotify);
            Controls.Add(pnlNotifyDot);
            Controls.Add(picAvatar);
            Controls.Add(btnProfileMenu);

            txtSearch.Enter += txtSearch_Enter;
            txtSearch.Leave += txtSearch_Leave;
            btnQuickAdd.Click += (_, __) => QuickAddPatientClicked?.Invoke(this, EventArgs.Empty);
            Resize += (_, __) => ArrangeLayout();
        }

        private void ArrangeLayout()
        {
            if (txtSearch == null || btnQuickAdd == null || btnNotify == null || picAvatar == null)
            {
                return;
            }

            var showDashboardActions = !_dashboardMode;
            txtSearch.Visible = showDashboardActions;
            picSearchIcon.Visible = showDashboardActions;
            btnQuickAdd.Visible = showDashboardActions;
            pnlDivider.Visible = showDashboardActions;
            btnNotify.Visible = showDashboardActions;
            pnlNotifyDot.Visible = showDashboardActions;
            picAvatar.Visible = showDashboardActions;
            btnProfileMenu.Visible = showDashboardActions;

            lblTitle.Visible = true;
            lblUser.Visible = true;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 31);

            if (_dashboardMode)
            {
                lblUser.AutoSize = false;
                lblUser.TextAlign = ContentAlignment.MiddleRight;
                lblUser.Size = new Size(300, 22);
                lblUser.Location = new Point(Math.Max(Width - lblUser.Width - 24, 220), 31);
                return;
            }

            var searchX = _dashboardMode ? 20 : lblTitle.Right + 18;
            var rightReserved = 20 + 34 + 10 + 34 + 10 + 1 + 10 + 126;
            var searchWidth = _dashboardMode
                ? Math.Max(260, Width - searchX - rightReserved - 20)
                : Math.Min(360, Math.Max(220, Width / 3));

            picSearchIcon.Location = new Point(searchX + 12, 34);
            txtSearch.Location = new Point(searchX + 34, 31);
            txtSearch.Size = new Size(searchWidth - 42, 22);

            picAvatar.Size = new Size(34, 34);
            picAvatar.Location = new Point(Math.Max(Width - 44 - picAvatar.Width, searchX + searchWidth + 8), 26);

            btnProfileMenu.Location = new Point(picAvatar.Right + 4, 26);
            btnProfileMenu.Size = new Size(20, 34);

            btnNotify.Size = new Size(34, 34);
            btnNotify.Location = new Point(picAvatar.Left - btnNotify.Width - 10, 26);
            pnlNotifyDot.Location = new Point(btnNotify.Right - 9, btnNotify.Top + 4);

            pnlDivider.Location = new Point(btnNotify.Left - pnlDivider.Width - 10, 29);
            pnlDivider.Size = new Size(1, 28);

            btnQuickAdd.Size = new Size(126, 34);
            btnQuickAdd.Location = new Point(pnlDivider.Left - btnQuickAdd.Width - 12, 26);

            if (!_dashboardMode)
            {
                lblUser.AutoSize = false;
                lblUser.TextAlign = ContentAlignment.MiddleRight;
                lblUser.Size = new Size(250, 18);
                lblUser.Location = new Point(btnQuickAdd.Left - lblUser.Width - 12, 8);
                lblUser.BringToFront();
            }
        }

        private void SetSearchHint()
        {
            _searchHintActive = true;
            txtSearch.Text = "Search here...";
            txtSearch.ForeColor = ThemeManager.Colors.TextSecondary;
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            if (!_searchHintActive)
            {
                return;
            }

            _searchHintActive = false;
            txtSearch.Text = string.Empty;
            txtSearch.ForeColor = ThemeManager.Colors.TextPrimary;
        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                return;
            }

            SetSearchHint();
        }

        private void ApplyTheme()
        {
            ThemeManager.StyleHeaderBar(this, lblTitle, lblUser, btnLogout);
            btnLogout.Visible = false;

            ThemeManager.StyleTextBox(txtSearch);
            ThemeManager.StyleButton(btnQuickAdd, ThemeButtonKind.Primary);
            ThemeManager.StyleButton(btnNotify, ThemeButtonKind.Secondary);
            btnNotify.FlatAppearance.BorderColor = ThemeManager.Colors.Border;
            btnNotify.ImageAlign = ContentAlignment.MiddleCenter;
            btnNotify.Text = string.Empty;
            btnNotify.BackColor = ThemeManager.Colors.Surface;
            btnNotify.ForeColor = ThemeManager.Colors.TextPrimary;
            btnNotify.Region = new Region(new Rectangle(0, 0, btnNotify.Width, btnNotify.Height));

            ThemeManager.StyleButton(btnProfileMenu, ThemeButtonKind.Secondary);
            btnProfileMenu.FlatAppearance.BorderSize = 0;
            btnProfileMenu.BackColor = ThemeManager.Colors.Surface;
            btnProfileMenu.ForeColor = ThemeManager.Colors.TextSecondary;
            pnlDivider.BackColor = ThemeManager.Colors.Border;
            pnlNotifyDot.BackColor = ColorTranslator.FromHtml("#FF5A73");
            picSearchIcon.BackColor = ThemeManager.Colors.Surface;

            ThemeManager.ApplyBrandingLogo(picAvatar);
            SetSearchHint();
            ArrangeLayout();
        }

        private enum ActionIconType
        {
            Search = 0,
            Notification = 1
        }

        private static Bitmap CreateActionIcon(ActionIconType type)
        {
            var icon = new Bitmap(18, 18);
            using (var graphics = Graphics.FromImage(icon))
            using (var pen = new Pen(ThemeManager.Colors.TextSecondary, 1.6F))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.Clear(Color.Transparent);
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;

                if (type == ActionIconType.Search)
                {
                    graphics.DrawEllipse(pen, 2.5F, 2.5F, 9F, 9F);
                    graphics.DrawLine(pen, 10, 10, 15, 15);
                }
                else
                {
                    graphics.DrawArc(pen, 4, 4, 10, 9, 200, 140);
                    graphics.DrawLine(pen, 7, 12, 11, 12);
                    graphics.DrawLine(pen, 9, 12, 9, 14);
                }
            }

            return icon;
        }
    }
}
