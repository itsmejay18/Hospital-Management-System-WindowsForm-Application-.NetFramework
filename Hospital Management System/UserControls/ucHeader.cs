using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using HospitalManagementSystem.BLL.Services;
using HospitalManagementSystem.Helpers;

namespace HospitalManagementSystem.UserControls
{
    public partial class ucHeader : UserControl
    {
        private readonly UserService _userService = new UserService();
        private Button btnQuickAdd;
        private Button btnNotify;
        private Button btnProfileMenu;
        private PictureBox picAvatar;
        private Panel pnlDivider;
        private Panel pnlNotifyDot;
        private bool _dashboardMode;
        private int _currentUserId;

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

        public async void SetUser(int userId, string username, string roleName)
        {
            _currentUserId = userId;
            lblUser.Text = $"User: {username} ({roleName})";
            await LoadAvatarAsync(userId).ConfigureAwait(true);
            ArrangeLayout();
        }

        public async Task RefreshAvatarAsync()
        {
            await LoadAvatarAsync(_currentUserId).ConfigureAwait(true);
        }

        public async Task RefreshAvatarAsync(int userId)
        {
            _currentUserId = userId;
            await LoadAvatarAsync(_currentUserId).ConfigureAwait(true);
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

            Controls.Add(btnQuickAdd);
            Controls.Add(pnlDivider);
            Controls.Add(btnNotify);
            Controls.Add(pnlNotifyDot);
            Controls.Add(picAvatar);
            Controls.Add(btnProfileMenu);

            btnQuickAdd.Click += (_, __) => QuickAddPatientClicked?.Invoke(this, EventArgs.Empty);
            Resize += (_, __) => ArrangeLayout();
        }

        private void ArrangeLayout()
        {
            if (btnQuickAdd == null || btnNotify == null || picAvatar == null || btnProfileMenu == null)
            {
                return;
            }

            var showUtilityActions = !_dashboardMode;
            btnQuickAdd.Visible = showUtilityActions;
            pnlDivider.Visible = showUtilityActions;
            btnNotify.Visible = showUtilityActions;
            pnlNotifyDot.Visible = showUtilityActions;
            picAvatar.Visible = true;
            btnProfileMenu.Visible = true;

            lblTitle.Visible = true;
            lblUser.Visible = true;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 31);

            btnProfileMenu.Size = new Size(20, 34);
            btnProfileMenu.Location = new Point(Math.Max(Width - 24 - btnProfileMenu.Width, 220), 26);

            picAvatar.Size = new Size(34, 34);
            picAvatar.Location = new Point(Math.Max(btnProfileMenu.Left - 4 - picAvatar.Width, 184), 26);

            if (_dashboardMode)
            {
                lblUser.AutoSize = false;
                lblUser.TextAlign = ContentAlignment.MiddleRight;
                lblUser.Size = new Size(300, 22);
                lblUser.Location = new Point(Math.Max(picAvatar.Left - lblUser.Width - 12, 220), 31);
                return;
            }

            btnNotify.Size = new Size(34, 34);
            btnNotify.Location = new Point(Math.Max(picAvatar.Left - btnNotify.Width - 10, lblTitle.Right + 12), 26);
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

        private void ApplyTheme()
        {
            ThemeManager.StyleHeaderBar(this, lblTitle, lblUser, btnLogout);
            btnLogout.Visible = false;

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

            ThemeManager.ApplyBrandingLogo(picAvatar);
            ArrangeLayout();
        }

        private async Task LoadAvatarAsync(int userId)
        {
            if (picAvatar == null || userId <= 0)
            {
                ThemeManager.ApplyBrandingLogo(picAvatar);
                return;
            }

            try
            {
                var detail = await _userService.GetUserDetailAsync(userId).ConfigureAwait(true);
                if (detail?.ProfileImage == null || detail.ProfileImage.Length == 0)
                {
                    ThemeManager.ApplyBrandingLogo(picAvatar);
                    return;
                }

                using (var stream = new MemoryStream(detail.ProfileImage))
                using (var image = Image.FromStream(stream))
                {
                    var previousImage = picAvatar.Image;
                    picAvatar.Image = new Bitmap(image);
                    if (previousImage != null && !ReferenceEquals(previousImage, picAvatar.Image))
                    {
                        previousImage.Dispose();
                    }
                }
            }
            catch
            {
                ThemeManager.ApplyBrandingLogo(picAvatar);
            }
        }

        private enum ActionIconType
        {
            Notification = 0
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

                graphics.DrawArc(pen, 4, 4, 10, 9, 200, 140);
                graphics.DrawLine(pen, 7, 12, 11, 12);
                graphics.DrawLine(pen, 9, 12, 9, 14);
            }

            return icon;
        }
    }
}
