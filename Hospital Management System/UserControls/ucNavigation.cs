using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using HospitalManagementSystem.Helpers;

namespace HospitalManagementSystem.UserControls
{
    public partial class ucNavigation : UserControl
    {
        private Button _activeButton;
        private bool _resizeHandlersHooked;
        private bool _shapeHooked;
        private bool _hoverStateHandlersHooked;
        private Label _lblOtherMenu;

        private enum NavIconType
        {
            Dashboard = 0,
            Patients = 1,
            Doctors = 2,
            Appointments = 3,
            Rooms = 4,
            Billing = 5,
            Settings = 6,
            Users = 7,
            Reports = 8,
            Profile = 9,
            Logout = 10
        }

        public event EventHandler DashboardClicked;
        public event EventHandler PatientsClicked;
        public event EventHandler DoctorsClicked;
        public event EventHandler AppointmentsClicked;
        public event EventHandler RoomsClicked;
        public event EventHandler BillingClicked;
        public event EventHandler SettingsClicked;
        public event EventHandler UsersClicked;
        public event EventHandler ReportsClicked;
        public event EventHandler ProfileClicked;
        public event EventHandler LogoutClicked;

        public ucNavigation()
        {
            InitializeComponent();
            ApplyTheme();
            SetActiveButton(btnDashboard);
        }

        public void ConfigureForRole(string roleName)
        {
            var normalized = NormalizeRoleKey(roleName);

            btnDashboard.Visible = true;
            btnPatients.Visible = HasRole(normalized, "administrator", "doctor", "nurse", "receptionist", "pharmacist", "labtechnician");
            btnDoctors.Visible = HasRole(normalized, "administrator", "doctor", "receptionist", "hrmanager");
            btnAppointments.Visible = HasRole(normalized, "administrator", "doctor", "nurse", "receptionist", "labtechnician");
            btnRooms.Visible = HasRole(normalized, "administrator", "receptionist", "nurse");
            btnBilling.Visible = HasRole(normalized, "administrator", "receptionist", "accountant", "pharmacist");
            btnSettings.Visible = HasRole(normalized, "administrator");
            btnUsers.Visible = HasRole(normalized, "administrator");
            btnReports.Visible = HasRole(normalized, "administrator", "doctor", "accountant", "hrmanager", "labtechnician", "pharmacist");
            btnProfile.Visible = true;
            btnLogout.Visible = true;
            if (_lblOtherMenu != null)
            {
                _lblOtherMenu.Visible = btnSettings.Visible || btnUsers.Visible || btnReports.Visible;
            }
        }

        private static string NormalizeRoleKey(string roleName)
        {
            var token = (roleName ?? string.Empty)
                .Trim()
                .ToLowerInvariant()
                .Replace(" ", string.Empty);

            switch (token)
            {
                case "admin":
                case "systemadministrator":
                case "systemadmin":
                    return "administrator";
                case "frontdesk":
                case "frontdeskstaff":
                    return "receptionist";
                default:
                    return token;
            }
        }

        private static bool HasRole(string currentRole, params string[] allowedRoles)
        {
            if (string.IsNullOrWhiteSpace(currentRole) || allowedRoles == null || allowedRoles.Length == 0)
            {
                return false;
            }

            foreach (var role in allowedRoles)
            {
                if (string.Equals(currentRole, NormalizeRoleKey(role), StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnDashboard);
            DashboardClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnPatients_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnPatients);
            PatientsClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnDoctors_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnDoctors);
            DoctorsClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnAppointments_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnAppointments);
            AppointmentsClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnRooms_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnRooms);
            RoomsClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnBilling_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnBilling);
            BillingClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnSettings);
            SettingsClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnUsers);
            UsersClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnReports);
            ReportsClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnProfile);
            ProfileClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            LogoutClicked?.Invoke(this, EventArgs.Empty);
        }

        private void ApplyTheme()
        {
            BackColor = ThemeManager.Colors.Sidebar;
            pnlHeader.BackColor = ThemeManager.Colors.Sidebar;
            flpMenu.BackColor = ThemeManager.Colors.Sidebar;
            pnlSection.BackColor = ThemeManager.Colors.Sidebar;
            pnlFooter.BackColor = ThemeManager.Colors.Sidebar;
            pnlHeader.BorderStyle = BorderStyle.None;
            pnlSection.BorderStyle = BorderStyle.None;
            pnlFooter.BorderStyle = BorderStyle.None;
            pnlHeader.Padding = new Padding(14, 14, 14, 8);
            flpMenu.Padding = new Padding(8, 8, 8, 8);
            pnlFooter.Padding = new Padding(8, 8, 8, 10);

            lblNavigation.ForeColor = ThemeManager.Colors.SidebarText;
            lblNavigation.Font = ThemeManager.Fonts.Medium;
            lblNavigation.Text = "MENU";
            var companyName = AppSettingsStore.Load().CompanyName;
            lblAppName.Text = BuildSidebarBrandLabel(companyName);
            lblAppName.AutoEllipsis = false;
            lblAppName.TextAlign = ContentAlignment.MiddleLeft;
            lblAppName.ForeColor = ThemeManager.Colors.SidebarText;
            ThemeManager.ApplyBrandingLogo(picLogo);
            ArrangeHeaderLayout();

            var navIconColor = ThemeManager.Colors.SidebarText;
            btnDashboard.Image = CreateBadgeIcon(NavIconType.Dashboard, navIconColor);
            btnPatients.Image = CreateBadgeIcon(NavIconType.Patients, navIconColor);
            btnDoctors.Image = CreateBadgeIcon(NavIconType.Doctors, navIconColor);
            btnAppointments.Image = CreateBadgeIcon(NavIconType.Appointments, navIconColor);
            btnRooms.Image = CreateBadgeIcon(NavIconType.Rooms, navIconColor);
            btnBilling.Image = CreateBadgeIcon(NavIconType.Billing, navIconColor);
            btnSettings.Image = CreateBadgeIcon(NavIconType.Settings, navIconColor);
            btnUsers.Image = CreateBadgeIcon(NavIconType.Users, navIconColor);
            btnReports.Image = CreateBadgeIcon(NavIconType.Reports, navIconColor);
            btnProfile.Image = CreateBadgeIcon(NavIconType.Profile, navIconColor);
            btnLogout.Image = CreateBadgeIcon(NavIconType.Logout, ThemeManager.Colors.SidebarText);

            ThemeManager.StyleSidebar(
                pnlHeader,
                lblAppName,
                flpMenu,
                btnDashboard,
                btnPatients,
                btnDoctors,
                btnAppointments,
                btnRooms,
                btnBilling,
                btnSettings,
                btnUsers,
                btnReports);

            ThemeManager.StyleSidebarButton(btnProfile, isDanger: false, isActive: false);
            ThemeManager.StyleSidebarButton(btnLogout, isDanger: true, isActive: false);
            EnsureSectionLabels();
            ArrangeMenuSections();

            if (!_resizeHandlersHooked)
            {
                pnlHeader.Resize += (_, __) => ArrangeHeaderLayout();
                flpMenu.Resize += (_, __) => RefreshSidebarButtonLayout();
                _resizeHandlersHooked = true;
            }

            ApplySidebarShape();
            if (!_shapeHooked)
            {
                Resize += (_, __) => ApplySidebarShape();
                _shapeHooked = true;
            }

            HookHoverStateHandlers();
        }

        private static Bitmap CreateBadgeIcon(NavIconType type, Color iconColor)
        {
            var icon = new Bitmap(20, 20);
            using (var graphics = Graphics.FromImage(icon))
            using (var pen = new Pen(iconColor, 1.6F))
            using (var brush = new SolidBrush(iconColor))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                graphics.Clear(Color.Transparent);
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;
                DrawIconGlyph(graphics, type, pen, brush);
            }

            return icon;
        }

        private static string BuildSidebarBrandLabel(string companyName)
        {
            var text = string.IsNullOrWhiteSpace(companyName)
                ? "Hospital Management System"
                : companyName.Trim();

            var parts = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length <= 2)
            {
                return text;
            }

            var firstLine = string.Join(" ", parts, 0, Math.Min(2, parts.Length));
            var secondLine = string.Join(" ", parts, 2, parts.Length - 2);
            return string.IsNullOrWhiteSpace(secondLine) ? firstLine : $"{firstLine}\n{secondLine}";
        }

        private static void DrawIconGlyph(Graphics graphics, NavIconType type, Pen pen, Brush brush)
        {
            switch (type)
            {
                case NavIconType.Dashboard:
                    graphics.FillRectangle(brush, 5, 5, 4, 4);
                    graphics.FillRectangle(brush, 11, 5, 4, 4);
                    graphics.FillRectangle(brush, 5, 11, 4, 4);
                    graphics.FillRectangle(brush, 11, 11, 4, 4);
                    break;
                case NavIconType.Patients:
                    graphics.DrawEllipse(pen, 8, 5, 4, 4);
                    graphics.DrawArc(pen, 5, 9, 10, 6, 200, 140);
                    break;
                case NavIconType.Doctors:
                    graphics.DrawEllipse(pen, 8, 4, 4, 4);
                    graphics.DrawArc(pen, 5, 8, 10, 6, 200, 140);
                    graphics.DrawLine(pen, 5, 14, 7, 14);
                    graphics.DrawLine(pen, 6, 13, 6, 15);
                    break;
                case NavIconType.Appointments:
                    graphics.DrawRectangle(pen, 5, 6, 10, 9);
                    graphics.DrawLine(pen, 5, 9, 15, 9);
                    graphics.DrawLine(pen, 8, 4, 8, 7);
                    graphics.DrawLine(pen, 12, 4, 12, 7);
                    break;
                case NavIconType.Rooms:
                    graphics.DrawRectangle(pen, 4, 11, 12, 4);
                    graphics.DrawRectangle(pen, 4, 9, 4, 2);
                    graphics.DrawLine(pen, 9, 11, 9, 15);
                    break;
                case NavIconType.Billing:
                    using (var font = new Font("Segoe UI Semibold", 8F, FontStyle.Regular, GraphicsUnit.Point))
                    {
                        var format = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        };
                        graphics.DrawString("\u20B1", font, brush, new RectangleF(0, 0, 19, 19), format);
                    }
                    break;
                case NavIconType.Users:
                    graphics.DrawEllipse(pen, 5, 6, 4, 4);
                    graphics.DrawEllipse(pen, 10, 6, 4, 4);
                    graphics.DrawArc(pen, 3, 10, 8, 5, 200, 140);
                    graphics.DrawArc(pen, 8, 10, 8, 5, 200, 140);
                    break;
                case NavIconType.Settings:
                    graphics.DrawEllipse(pen, 6, 6, 8, 8);
                    graphics.DrawLine(pen, 10, 3, 10, 5);
                    graphics.DrawLine(pen, 10, 15, 10, 17);
                    graphics.DrawLine(pen, 3, 10, 5, 10);
                    graphics.DrawLine(pen, 15, 10, 17, 10);
                    graphics.DrawLine(pen, 5, 5, 6.5F, 6.5F);
                    graphics.DrawLine(pen, 13.5F, 13.5F, 15, 15);
                    graphics.DrawLine(pen, 13.5F, 6.5F, 15, 5);
                    graphics.DrawLine(pen, 5, 15, 6.5F, 13.5F);
                    break;
                case NavIconType.Reports:
                    graphics.DrawLine(pen, 5, 15, 5, 9);
                    graphics.DrawLine(pen, 9, 15, 9, 7);
                    graphics.DrawLine(pen, 13, 15, 13, 11);
                    break;
                case NavIconType.Profile:
                    graphics.DrawEllipse(pen, 7, 5, 6, 6);
                    graphics.DrawArc(pen, 4, 11, 12, 7, 200, 140);
                    break;
                case NavIconType.Logout:
                    graphics.DrawRectangle(pen, 5, 5, 7, 10);
                    graphics.DrawLine(pen, 11, 10, 16, 10);
                    graphics.DrawLine(pen, 14, 8, 16, 10);
                    graphics.DrawLine(pen, 14, 12, 16, 10);
                    break;
            }
        }

        private void SetActiveButton(Button button)
        {
            if (button == null)
            {
                return;
            }

            var oldButton = _activeButton;
            if (oldButton != null && oldButton != button)
            {
                var oldIsDanger = oldButton == btnLogout;
                ThemeManager.StyleSidebarButton(oldButton, oldIsDanger, isActive: false);
            }

            var isDanger = button == btnLogout;
            ThemeManager.StyleSidebarButton(button, isDanger, isActive: !isDanger);
            _activeButton = button;
        }

        private void ArrangeHeaderLayout()
        {
            picLogo.Size = new Size(52, 52);
            picLogo.Location = new Point(14, 28);
            lblAppName.Location = new Point(picLogo.Right + 10, 22);
            lblAppName.Size = new Size(Math.Max(100, pnlHeader.Width - lblAppName.Left - 12), 62);
        }

        private void EnsureSectionLabels()
        {
            if (_lblOtherMenu != null)
            {
                return;
            }

            _lblOtherMenu = new Label
            {
                Name = "lblOtherMenu",
                AutoSize = false,
                Height = 24,
                Width = 220,
                Text = "OTHER MENU",
                Padding = new Padding(14, 0, 0, 0),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Regular),
                ForeColor = ColorTranslator.FromHtml("#C8F8F1"),
                Margin = new Padding(0, 8, 0, 6)
            };

            flpMenu.Controls.Add(_lblOtherMenu);
        }

        private void ArrangeMenuSections()
        {
            if (_lblOtherMenu == null)
            {
                return;
            }

            var targetIndex = flpMenu.Controls.GetChildIndex(btnSettings);
            flpMenu.Controls.SetChildIndex(_lblOtherMenu, targetIndex);
            _lblOtherMenu.Width = Math.Max(170, flpMenu.ClientSize.Width - 16);
        }

        private void ApplySidebarShape()
        {
            if (Width <= 0 || Height <= 0)
            {
                return;
            }

            var radius = 18;
            using (var path = new GraphicsPath())
            {
                path.StartFigure();
                path.AddLine(0, 0, Width - radius, 0);
                path.AddArc(Width - (radius * 2), 0, radius * 2, radius * 2, 270, 90);
                path.AddLine(Width, radius, Width, Height - radius);
                path.AddArc(Width - (radius * 2), Height - (radius * 2), radius * 2, radius * 2, 0, 90);
                path.AddLine(Width - radius, Height, 0, Height);
                path.CloseFigure();

                var oldRegion = Region;
                Region = new Region(path);
                oldRegion?.Dispose();
            }
        }

        private void RefreshSidebarButtonLayout()
        {
            ThemeManager.StyleSidebarButton(btnDashboard, isDanger: false, isActive: _activeButton == btnDashboard);
            ThemeManager.StyleSidebarButton(btnPatients, isDanger: false, isActive: _activeButton == btnPatients);
            ThemeManager.StyleSidebarButton(btnDoctors, isDanger: false, isActive: _activeButton == btnDoctors);
            ThemeManager.StyleSidebarButton(btnAppointments, isDanger: false, isActive: _activeButton == btnAppointments);
            ThemeManager.StyleSidebarButton(btnRooms, isDanger: false, isActive: _activeButton == btnRooms);
            ThemeManager.StyleSidebarButton(btnBilling, isDanger: false, isActive: _activeButton == btnBilling);
            ThemeManager.StyleSidebarButton(btnSettings, isDanger: false, isActive: _activeButton == btnSettings);
            ThemeManager.StyleSidebarButton(btnUsers, isDanger: false, isActive: _activeButton == btnUsers);
            ThemeManager.StyleSidebarButton(btnReports, isDanger: false, isActive: _activeButton == btnReports);
            ThemeManager.StyleSidebarButton(btnProfile, isDanger: false, isActive: _activeButton == btnProfile);
            ThemeManager.StyleSidebarButton(btnLogout, isDanger: true, isActive: false);
            ArrangeMenuSections();
        }

        private void HookHoverStateHandlers()
        {
            if (_hoverStateHandlersHooked)
            {
                return;
            }

            foreach (var button in GetSidebarButtons())
            {
                button.MouseEnter += SidebarButtonStateGuard;
                button.MouseLeave += SidebarButtonStateGuard;
            }

            _hoverStateHandlersHooked = true;
        }

        private Button[] GetSidebarButtons()
        {
            return new[]
            {
                btnDashboard,
                btnPatients,
                btnDoctors,
                btnAppointments,
                btnRooms,
                btnBilling,
                btnSettings,
                btnUsers,
                btnReports,
                btnProfile,
                btnLogout
            };
        }

        private void SidebarButtonStateGuard(object sender, EventArgs e)
        {
            if (_activeButton == null || _activeButton == btnLogout)
            {
                return;
            }

            ThemeManager.StyleSidebarButton(_activeButton, isDanger: false, isActive: true);
        }
    }
}
