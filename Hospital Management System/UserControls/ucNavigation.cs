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

        private enum NavIconType
        {
            Dashboard = 0,
            Patients = 1,
            Doctors = 2,
            Appointments = 3,
            Rooms = 4,
            Billing = 5,
            Users = 6,
            Reports = 7,
            Logout = 8
        }

        public event EventHandler DashboardClicked;
        public event EventHandler PatientsClicked;
        public event EventHandler DoctorsClicked;
        public event EventHandler AppointmentsClicked;
        public event EventHandler RoomsClicked;
        public event EventHandler BillingClicked;
        public event EventHandler UsersClicked;
        public event EventHandler ReportsClicked;
        public event EventHandler LogoutClicked;

        public ucNavigation()
        {
            InitializeComponent();
            ApplyTheme();
            SetActiveButton(btnDashboard);
        }

        public void ConfigureForRole(string roleName)
        {
            var normalized = (roleName ?? string.Empty).Trim().ToLowerInvariant();

            btnDashboard.Visible = true;
            btnPatients.Visible = true;
            btnDoctors.Visible = normalized == "administrator" || normalized == "receptionist";
            btnAppointments.Visible = true;
            btnRooms.Visible = normalized == "administrator" || normalized == "receptionist" || normalized == "nurse";
            btnBilling.Visible = normalized == "administrator" || normalized == "receptionist";
            btnUsers.Visible = normalized == "administrator";
            btnReports.Visible = normalized == "administrator" || normalized == "doctor";
            btnLogout.Visible = true;
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
            pnlHeader.Padding = new Padding(12, 14, 12, 10);
            flpMenu.Padding = new Padding(8, 10, 8, 8);
            pnlFooter.Padding = new Padding(8, 10, 8, 12);

            lblNavigation.ForeColor = ThemeManager.Colors.SidebarText;
            lblNavigation.Font = ThemeManager.Fonts.Medium;
            lblNavigation.Text = "\u2630  NAVIGATION";
            lblAppName.Text = "Hospital Management\nSystem";
            lblAppName.AutoEllipsis = false;
            lblAppName.TextAlign = ContentAlignment.MiddleLeft;
            lblAppName.ForeColor = ThemeManager.Colors.TextPrimary;
            ThemeManager.ApplyBrandingLogo(picLogo);
            ArrangeHeaderLayout();

            var navIconColor = ThemeManager.Colors.Primary;
            btnDashboard.Image = CreateBadgeIcon(NavIconType.Dashboard, navIconColor);
            btnPatients.Image = CreateBadgeIcon(NavIconType.Patients, navIconColor);
            btnDoctors.Image = CreateBadgeIcon(NavIconType.Doctors, navIconColor);
            btnAppointments.Image = CreateBadgeIcon(NavIconType.Appointments, navIconColor);
            btnRooms.Image = CreateBadgeIcon(NavIconType.Rooms, navIconColor);
            btnBilling.Image = CreateBadgeIcon(NavIconType.Billing, navIconColor);
            btnUsers.Image = CreateBadgeIcon(NavIconType.Users, navIconColor);
            btnReports.Image = CreateBadgeIcon(NavIconType.Reports, navIconColor);
            btnLogout.Image = CreateBadgeIcon(NavIconType.Logout, ThemeManager.Colors.Danger);

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
                btnUsers,
                btnReports);

            ThemeManager.StyleSidebarButton(btnLogout, isDanger: true, isActive: false);

            if (!_resizeHandlersHooked)
            {
                pnlHeader.Resize += (_, __) => ArrangeHeaderLayout();
                flpMenu.Resize += (_, __) => RefreshSidebarButtonLayout();
                _resizeHandlersHooked = true;
            }
        }

        private static Bitmap CreateBadgeIcon(NavIconType type, Color backgroundColor)
        {
            var icon = new Bitmap(20, 20);
            using (var graphics = Graphics.FromImage(icon))
            using (var backBrush = new SolidBrush(backgroundColor))
            using (var whitePen = new Pen(Color.White, 1.5F))
            using (var whiteBrush = new SolidBrush(Color.White))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                graphics.Clear(Color.Transparent);
                graphics.FillEllipse(backBrush, 0, 0, 19, 19);

                DrawIconGlyph(graphics, type, whitePen, whiteBrush);
            }

            return icon;
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
                case NavIconType.Reports:
                    graphics.DrawLine(pen, 5, 15, 5, 9);
                    graphics.DrawLine(pen, 9, 15, 9, 7);
                    graphics.DrawLine(pen, 13, 15, 13, 11);
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
            picLogo.Size = new Size(56, 56);
            picLogo.Location = new Point(14, 24);
            lblAppName.Location = new Point(picLogo.Right + 10, 20);
            lblAppName.Size = new Size(Math.Max(100, pnlHeader.Width - lblAppName.Left - 12), 62);
        }

        private void RefreshSidebarButtonLayout()
        {
            ThemeManager.StyleSidebarButton(btnDashboard, isDanger: false, isActive: _activeButton == btnDashboard);
            ThemeManager.StyleSidebarButton(btnPatients, isDanger: false, isActive: _activeButton == btnPatients);
            ThemeManager.StyleSidebarButton(btnDoctors, isDanger: false, isActive: _activeButton == btnDoctors);
            ThemeManager.StyleSidebarButton(btnAppointments, isDanger: false, isActive: _activeButton == btnAppointments);
            ThemeManager.StyleSidebarButton(btnRooms, isDanger: false, isActive: _activeButton == btnRooms);
            ThemeManager.StyleSidebarButton(btnBilling, isDanger: false, isActive: _activeButton == btnBilling);
            ThemeManager.StyleSidebarButton(btnUsers, isDanger: false, isActive: _activeButton == btnUsers);
            ThemeManager.StyleSidebarButton(btnReports, isDanger: false, isActive: _activeButton == btnReports);
            ThemeManager.StyleSidebarButton(btnLogout, isDanger: true, isActive: false);
        }
    }
}
