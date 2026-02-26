using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HospitalManagementSystem.Helpers
{
    /// <summary>
    /// Centralized modern UI theme configuration for WinForms.
    /// </summary>
    public static class ThemeManager
    {
        private static readonly Dictionary<Control, EventHandler> RoundedResizeHandlers =
            new Dictionary<Control, EventHandler>();

        /// <summary>
        /// Semantic color palette.
        /// </summary>
        public static class Colors
        {
            public static readonly Color Primary = ColorTranslator.FromHtml("#4F46E5");
            public static readonly Color PrimaryHover = ColorTranslator.FromHtml("#4338CA");
            public static readonly Color PrimaryPressed = ColorTranslator.FromHtml("#3730A3");
            public static readonly Color PrimarySoft = ColorTranslator.FromHtml("#E0E7FF");
            public static readonly Color Background = ColorTranslator.FromHtml("#F3F4F6");
            public static readonly Color Surface = Color.White;
            public static readonly Color SurfaceMuted = ColorTranslator.FromHtml("#EEF2FF");
            public static readonly Color Border = ColorTranslator.FromHtml("#E5E7EB");
            public static readonly Color TextPrimary = ColorTranslator.FromHtml("#111827");
            public static readonly Color TextSecondary = ColorTranslator.FromHtml("#6B7280");
            public static readonly Color Danger = ColorTranslator.FromHtml("#DC2626");
            public static readonly Color DangerHover = ColorTranslator.FromHtml("#B91C1C");
            public static readonly Color Sidebar = ColorTranslator.FromHtml("#111827");
            public static readonly Color SidebarItem = ColorTranslator.FromHtml("#1F2937");
            public static readonly Color SidebarItemHover = ColorTranslator.FromHtml("#374151");
            public static readonly Color SidebarItemActive = ColorTranslator.FromHtml("#4F46E5");
            public static readonly Color SidebarText = ColorTranslator.FromHtml("#E5E7EB");
        }

        /// <summary>
        /// Shared fonts.
        /// </summary>
        public static class Fonts
        {
            public static readonly Font Regular = new Font("Segoe UI", 9F, FontStyle.Regular);
            public static readonly Font Medium = new Font("Segoe UI Semibold", 9F, FontStyle.Regular);
            public static readonly Font Heading = new Font("Segoe UI Semibold", 12F, FontStyle.Regular);
            public static readonly Font Kpi = new Font("Segoe UI Semibold", 20F, FontStyle.Regular);
        }

        /// <summary>
        /// Applies base form theme.
        /// </summary>
        public static void ApplyFormTheme(Form form, bool styleChildren = true)
        {
            if (form == null)
            {
                return;
            }

            form.BackColor = Colors.Background;
            form.ForeColor = Colors.TextPrimary;
            form.Font = Fonts.Regular;
            StyleStripControls(form);

            if (styleChildren)
            {
                ApplyControlTheme(form);
            }
        }

        /// <summary>
        /// Recursively styles common controls.
        /// </summary>
        public static void ApplyControlTheme(Control root)
        {
            if (root == null)
            {
                return;
            }

            ApplySingleControlTheme(root);

            foreach (Control child in root.Controls)
            {
                ApplyControlTheme(child);
            }
        }

        /// <summary>
        /// Styles top application header.
        /// </summary>
        public static void StyleHeaderBar(Control headerRoot, Label titleLabel, Label userLabel, Button logoutButton)
        {
            if (headerRoot != null)
            {
                headerRoot.BackColor = Colors.Surface;
            }

            if (titleLabel != null)
            {
                titleLabel.Font = Fonts.Heading;
                titleLabel.ForeColor = Colors.TextPrimary;
            }

            if (userLabel != null)
            {
                userLabel.Font = Fonts.Medium;
                userLabel.ForeColor = Colors.TextSecondary;
            }

            if (logoutButton != null)
            {
                StyleButton(logoutButton, ThemeButtonKind.Danger);
                logoutButton.Width = Math.Max(logoutButton.Width, 90);
            }
        }

        /// <summary>
        /// Styles sidebar navigation.
        /// </summary>
        public static void StyleSidebar(Panel headerPanel, Label appNameLabel, FlowLayoutPanel menuPanel, params Button[] menuButtons)
        {
            if (headerPanel != null)
            {
                headerPanel.BackColor = Colors.Primary;
            }

            if (appNameLabel != null)
            {
                appNameLabel.ForeColor = Color.White;
                appNameLabel.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Regular);
            }

            if (menuPanel != null)
            {
                menuPanel.BackColor = Colors.Sidebar;
            }

            if (menuButtons == null)
            {
                return;
            }

            foreach (var button in menuButtons)
            {
                if (button == null)
                {
                    continue;
                }

                var isDanger = button.Name.IndexOf("logout", StringComparison.OrdinalIgnoreCase) >= 0
                               || button.Text.IndexOf("logout", StringComparison.OrdinalIgnoreCase) >= 0;
                StyleSidebarButton(button, isDanger, isActive: false);
            }
        }

        /// <summary>
        /// Sets a sidebar button as active/inactive.
        /// </summary>
        public static void StyleSidebarButton(Button button, bool isDanger, bool isActive)
        {
            if (button == null)
            {
                return;
            }

            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = isDanger ? Colors.DangerHover : Colors.SidebarItemHover;
            button.FlatAppearance.MouseDownBackColor = isDanger ? Colors.Danger : Colors.PrimaryPressed;
            button.Font = Fonts.Medium;
            button.TextAlign = ContentAlignment.MiddleLeft;
            button.TextImageRelation = TextImageRelation.ImageBeforeText;
            button.Padding = new Padding(12, 0, 0, 0);
            button.Margin = new Padding(3, 5, 3, 0);
            button.Cursor = Cursors.Hand;
            button.Width = 194;
            button.Height = 40;

            if (isDanger)
            {
                button.BackColor = Colors.Danger;
                button.ForeColor = Color.White;
                return;
            }

            button.BackColor = isActive ? Colors.SidebarItemActive : Colors.SidebarItem;
            button.ForeColor = Color.White;
        }

        /// <summary>
        /// Styles panel as elevated card.
        /// </summary>
        public static void StyleCardPanel(Panel panel, int radius = 12)
        {
            if (panel == null)
            {
                return;
            }

            panel.BackColor = Colors.Surface;
            panel.BorderStyle = BorderStyle.None;
            ApplyRoundedCorners(panel, radius);
            panel.Paint -= DrawCardBorder;
            panel.Paint += DrawCardBorder;
        }

        /// <summary>
        /// Styles a standard button.
        /// </summary>
        public static void StyleButton(Button button, ThemeButtonKind kind)
        {
            if (button == null)
            {
                return;
            }

            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.Font = Fonts.Medium;
            button.Cursor = Cursors.Hand;
            button.Height = Math.Max(button.Height, 32);
            button.Padding = new Padding(Math.Max(button.Padding.Left, 10), 0, Math.Max(button.Padding.Right, 10), 0);

            switch (kind)
            {
                case ThemeButtonKind.Primary:
                    button.BackColor = Colors.Primary;
                    button.ForeColor = Color.White;
                    button.FlatAppearance.BorderColor = Colors.Primary;
                    button.FlatAppearance.MouseOverBackColor = Colors.PrimaryHover;
                    button.FlatAppearance.MouseDownBackColor = Colors.PrimaryPressed;
                    break;
                case ThemeButtonKind.Danger:
                    button.BackColor = Colors.Danger;
                    button.ForeColor = Color.White;
                    button.FlatAppearance.BorderColor = Colors.Danger;
                    button.FlatAppearance.MouseOverBackColor = Colors.DangerHover;
                    button.FlatAppearance.MouseDownBackColor = Colors.DangerHover;
                    break;
                case ThemeButtonKind.Secondary:
                default:
                    button.BackColor = Colors.Surface;
                    button.ForeColor = Colors.TextPrimary;
                    button.FlatAppearance.BorderColor = Colors.Border;
                    button.FlatAppearance.MouseOverBackColor = Colors.SurfaceMuted;
                    button.FlatAppearance.MouseDownBackColor = Colors.PrimarySoft;
                    break;
            }

            ApplyRoundedCorners(button, 8);
        }

        /// <summary>
        /// Styles DataGridView with modern dashboard look.
        /// </summary>
        public static void StyleDataGridView(DataGridView grid)
        {
            if (grid == null)
            {
                return;
            }

            grid.EnableHeadersVisualStyles = false;
            grid.BackgroundColor = Colors.Surface;
            grid.BorderStyle = BorderStyle.None;
            grid.GridColor = Colors.Border;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AllowUserToResizeRows = false;
            grid.RowTemplate.Height = 30;
            grid.ColumnHeadersHeight = 36;

            grid.ColumnHeadersDefaultCellStyle.BackColor = Colors.Primary;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Colors.Primary;
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = Fonts.Medium;
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            grid.DefaultCellStyle.BackColor = Colors.Surface;
            grid.DefaultCellStyle.ForeColor = Colors.TextPrimary;
            grid.DefaultCellStyle.SelectionBackColor = Colors.PrimarySoft;
            grid.DefaultCellStyle.SelectionForeColor = Colors.TextPrimary;
            grid.DefaultCellStyle.Font = Fonts.Regular;
            grid.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#F9FAFB");
            grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = Colors.PrimarySoft;
            grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = Colors.TextPrimary;
        }

        /// <summary>
        /// Styles text input control.
        /// </summary>
        public static void StyleTextBox(TextBox textBox)
        {
            if (textBox == null)
            {
                return;
            }

            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.BackColor = Colors.Surface;
            textBox.ForeColor = Colors.TextPrimary;
            textBox.Font = Fonts.Regular;
        }

        /// <summary>
        /// Styles combo input control.
        /// </summary>
        public static void StyleComboBox(ComboBox comboBox)
        {
            if (comboBox == null)
            {
                return;
            }

            comboBox.BackColor = Colors.Surface;
            comboBox.ForeColor = Colors.TextPrimary;
            comboBox.Font = Fonts.Regular;
            comboBox.FlatStyle = FlatStyle.Flat;
        }

        /// <summary>
        /// Styles date input control.
        /// </summary>
        public static void StyleDateTimePicker(DateTimePicker picker)
        {
            if (picker == null)
            {
                return;
            }

            picker.CalendarTitleBackColor = Colors.Primary;
            picker.CalendarTitleForeColor = Color.White;
            picker.CalendarMonthBackground = Colors.Surface;
            picker.CalendarForeColor = Colors.TextPrimary;
            picker.Font = Fonts.Regular;
        }

        /// <summary>
        /// Styles numeric input control.
        /// </summary>
        public static void StyleNumericUpDown(NumericUpDown numericUpDown)
        {
            if (numericUpDown == null)
            {
                return;
            }

            numericUpDown.BackColor = Colors.Surface;
            numericUpDown.ForeColor = Colors.TextPrimary;
            numericUpDown.Font = Fonts.Regular;
            numericUpDown.BorderStyle = BorderStyle.FixedSingle;
        }

        /// <summary>
        /// Styles checkbox controls.
        /// </summary>
        public static void StyleCheckBox(CheckBox checkBox)
        {
            if (checkBox == null)
            {
                return;
            }

            checkBox.Font = Fonts.Regular;
            checkBox.ForeColor = Colors.TextPrimary;
            if (ShouldUseTransparentBackground(checkBox.Parent))
            {
                checkBox.BackColor = Color.Transparent;
            }
        }

        /// <summary>
        /// Styles link label controls.
        /// </summary>
        public static void StyleLinkLabel(LinkLabel linkLabel)
        {
            if (linkLabel == null)
            {
                return;
            }

            linkLabel.Font = Fonts.Regular;
            linkLabel.ActiveLinkColor = Colors.PrimaryPressed;
            linkLabel.LinkColor = Colors.Primary;
            linkLabel.VisitedLinkColor = Colors.PrimaryHover;
            linkLabel.LinkBehavior = LinkBehavior.HoverUnderline;
        }

        /// <summary>
        /// Styles menu strip controls.
        /// </summary>
        public static void StyleMenuStrip(MenuStrip menuStrip)
        {
            if (menuStrip == null)
            {
                return;
            }

            menuStrip.RenderMode = ToolStripRenderMode.Professional;
            menuStrip.Renderer = new ToolStripProfessionalRenderer(new ThemeToolStripColorTable());
            menuStrip.BackColor = Colors.Surface;
            menuStrip.ForeColor = Colors.TextPrimary;
            menuStrip.Font = Fonts.Regular;
            menuStrip.GripStyle = ToolStripGripStyle.Hidden;
            StyleToolStripItems(menuStrip.Items);
        }

        /// <summary>
        /// Styles status strip controls.
        /// </summary>
        public static void StyleStatusStrip(StatusStrip statusStrip)
        {
            if (statusStrip == null)
            {
                return;
            }

            statusStrip.RenderMode = ToolStripRenderMode.Professional;
            statusStrip.Renderer = new ToolStripProfessionalRenderer(new ThemeToolStripColorTable());
            statusStrip.BackColor = Colors.Surface;
            statusStrip.ForeColor = Colors.TextSecondary;
            statusStrip.Font = Fonts.Regular;
            statusStrip.SizingGrip = false;
            StyleToolStripItems(statusStrip.Items);
        }

        /// <summary>
        /// Styles generic tool strip controls.
        /// </summary>
        public static void StyleToolStrip(ToolStrip toolStrip)
        {
            if (toolStrip == null)
            {
                return;
            }

            toolStrip.RenderMode = ToolStripRenderMode.Professional;
            toolStrip.Renderer = new ToolStripProfessionalRenderer(new ThemeToolStripColorTable());
            toolStrip.BackColor = Colors.Surface;
            toolStrip.ForeColor = Colors.TextPrimary;
            toolStrip.Font = Fonts.Regular;
            toolStrip.GripStyle = ToolStripGripStyle.Hidden;
            StyleToolStripItems(toolStrip.Items);
        }

        /// <summary>
        /// Styles tab container controls.
        /// </summary>
        public static void StyleTabControl(TabControl tabControl)
        {
            if (tabControl == null)
            {
                return;
            }

            tabControl.BackColor = Colors.Background;
            tabControl.Font = Fonts.Medium;
            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl.Padding = new Point(16, 6);
            tabControl.ItemSize = new Size(130, Math.Max(tabControl.ItemSize.Height, 34));
            tabControl.DrawItem -= DrawTabItem;
            tabControl.DrawItem += DrawTabItem;
        }

        /// <summary>
        /// Styles tab page controls.
        /// </summary>
        public static void StyleTabPage(TabPage tabPage)
        {
            if (tabPage == null)
            {
                return;
            }

            tabPage.BackColor = Colors.Background;
            tabPage.ForeColor = Colors.TextPrimary;
            tabPage.Font = Fonts.Regular;
        }

        /// <summary>
        /// Styles flow layout controls.
        /// </summary>
        public static void StyleFlowLayoutPanel(FlowLayoutPanel panel)
        {
            if (panel == null)
            {
                return;
            }

            if (panel.BackColor == Color.Empty
                || panel.BackColor == SystemColors.Control
                || panel.BackColor == Color.WhiteSmoke)
            {
                panel.BackColor = Colors.Background;
            }
        }

        /// <summary>
        /// Styles table layout controls.
        /// </summary>
        public static void StyleTableLayoutPanel(TableLayoutPanel panel)
        {
            if (panel == null)
            {
                return;
            }

            if (panel.BackColor == Color.Empty
                || panel.BackColor == SystemColors.Control
                || panel.BackColor == Color.WhiteSmoke)
            {
                panel.BackColor = Colors.Background;
            }
        }

        /// <summary>
        /// Styles labels.
        /// </summary>
        public static void StyleLabel(Label label)
        {
            if (label == null)
            {
                return;
            }

            var fontSize = label.Font?.Size ?? Fonts.Regular.Size;
            var isHeading = IsHeadingLabel(label);
            var isDarkParent = IsDarkColor(label.Parent?.BackColor ?? Color.Empty);

            label.ForeColor = label.ForeColor == Color.White || isDarkParent
                ? Color.White
                : IsSecondaryLabel(label)
                    ? Colors.TextSecondary
                    : Colors.TextPrimary;

            if (isHeading)
            {
                label.Font = new Font("Segoe UI Semibold", Math.Max(fontSize, 12F), FontStyle.Regular);
                return;
            }

            if (label.Font != null && label.Font.Bold)
            {
                label.Font = new Font("Segoe UI Semibold", fontSize, FontStyle.Regular);
            }
            else
            {
                label.Font = new Font("Segoe UI", fontSize, FontStyle.Regular);
            }
        }

        /// <summary>
        /// Applies a neutral panel style for non-card containers.
        /// </summary>
        public static void StylePanel(Panel panel)
        {
            if (panel == null)
            {
                return;
            }

            if (panel.BackColor == Color.Empty
                || panel.BackColor == SystemColors.Control
                || panel.BackColor == Color.WhiteSmoke)
            {
                panel.BackColor = Colors.Background;
            }
        }

        private static void DrawCardBorder(object sender, PaintEventArgs e)
        {
            if (!(sender is Panel panel))
            {
                return;
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);
            using (var path = CreateRoundedPath(rect, 12))
            using (var pen = new Pen(Colors.Border))
            {
                e.Graphics.DrawPath(pen, path);
            }
        }

        private static void DrawTabItem(object sender, DrawItemEventArgs e)
        {
            if (!(sender is TabControl tabControl))
            {
                return;
            }

            if (e.Index < 0 || e.Index >= tabControl.TabPages.Count)
            {
                return;
            }

            var tabPage = tabControl.TabPages[e.Index];
            var isSelected = tabControl.SelectedIndex == e.Index;
            var rect = new Rectangle(e.Bounds.X + 2, e.Bounds.Y + 3, e.Bounds.Width - 4, e.Bounds.Height - 4);

            if (rect.Width <= 0 || rect.Height <= 0)
            {
                return;
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var path = CreateRoundedPath(rect, 6))
            using (var brush = new SolidBrush(isSelected ? Colors.Surface : Color.White))
            using (var pen = new Pen(isSelected ? Colors.Primary : Colors.Border))
            {
                e.Graphics.FillPath(brush, path);
                e.Graphics.DrawPath(pen, path);
            }

            TextRenderer.DrawText(
                e.Graphics,
                tabPage.Text,
                Fonts.Medium,
                rect,
                isSelected ? Colors.Primary : Colors.TextSecondary,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        private static void ApplySingleControlTheme(Control control)
        {
            switch (control)
            {
                case MenuStrip menuStrip:
                    StyleMenuStrip(menuStrip);
                    break;
                case StatusStrip statusStrip:
                    StyleStatusStrip(statusStrip);
                    break;
                case ToolStrip toolStrip:
                    StyleToolStrip(toolStrip);
                    break;
                case DataGridView grid:
                    StyleDataGridView(grid);
                    break;
                case Button button:
                    StyleButton(button, ResolveButtonKind(button));
                    break;
                case TextBox textBox:
                    StyleTextBox(textBox);
                    break;
                case ComboBox comboBox:
                    StyleComboBox(comboBox);
                    break;
                case DateTimePicker dateTimePicker:
                    StyleDateTimePicker(dateTimePicker);
                    break;
                case NumericUpDown numericUpDown:
                    StyleNumericUpDown(numericUpDown);
                    break;
                case CheckBox checkBox:
                    StyleCheckBox(checkBox);
                    break;
                case LinkLabel linkLabel:
                    StyleLinkLabel(linkLabel);
                    break;
                case TabControl tabControl:
                    StyleTabControl(tabControl);
                    break;
                case TabPage tabPage:
                    StyleTabPage(tabPage);
                    break;
                case FlowLayoutPanel flowLayoutPanel:
                    StyleFlowLayoutPanel(flowLayoutPanel);
                    break;
                case TableLayoutPanel tableLayoutPanel:
                    StyleTableLayoutPanel(tableLayoutPanel);
                    break;
                case Panel panel:
                    if (ShouldStyleAsCard(panel))
                    {
                        StyleCardPanel(panel);
                    }
                    else
                    {
                        StylePanel(panel);
                    }

                    break;
                case Label label:
                    StyleLabel(label);
                    break;
            }

            if (control is UserControl userControl)
            {
                StyleUserControl(userControl);
            }
        }

        private static void StyleUserControl(UserControl userControl)
        {
            if (userControl == null)
            {
                return;
            }

            if (userControl.BackColor == Color.Empty
                || userControl.BackColor == SystemColors.Control
                || userControl.BackColor == Color.WhiteSmoke)
            {
                userControl.BackColor = Colors.Background;
            }
        }

        private static void StyleStripControls(Form form)
        {
            foreach (Control control in form.Controls)
            {
                if (control is MenuStrip menuStrip)
                {
                    StyleMenuStrip(menuStrip);
                }
                else if (control is StatusStrip statusStrip)
                {
                    StyleStatusStrip(statusStrip);
                }
                else if (control is ToolStrip toolStrip)
                {
                    StyleToolStrip(toolStrip);
                }
            }
        }

        private static void StyleToolStripItems(ToolStripItemCollection items)
        {
            if (items == null)
            {
                return;
            }

            foreach (ToolStripItem item in items)
            {
                if (item == null)
                {
                    continue;
                }

                item.Font = Fonts.Regular;
                item.ForeColor = Colors.TextPrimary;

                if (item is ToolStripStatusLabel statusLabel)
                {
                    statusLabel.ForeColor = Colors.TextSecondary;
                    statusLabel.BorderSides = ToolStripStatusLabelBorderSides.None;
                }
                else if (item is ToolStripMenuItem menuItem)
                {
                    menuItem.BackColor = Colors.Surface;
                    menuItem.ForeColor = Colors.TextPrimary;

                    if (menuItem.DropDown != null)
                    {
                        menuItem.DropDown.BackColor = Colors.Surface;
                        menuItem.DropDown.ForeColor = Colors.TextPrimary;
                        menuItem.DropDown.Font = Fonts.Regular;
                    }

                    StyleToolStripItems(menuItem.DropDownItems);
                }
            }
        }

        private static bool ShouldUseTransparentBackground(Control parent)
        {
            if (parent == null)
            {
                return false;
            }

            return parent.BackColor == Colors.Surface
                   || parent.BackColor == Colors.Background
                   || parent.BackColor == Color.White;
        }

        private static bool IsHeadingLabel(Label label)
        {
            if (label == null)
            {
                return false;
            }

            var name = label.Name?.ToLowerInvariant() ?? string.Empty;
            return name.Contains("title")
                   || name.Contains("header")
                   || name.Contains("welcome")
                   || name.Contains("role")
                   || name.Contains("section");
        }

        private static bool IsSecondaryLabel(Label label)
        {
            if (label == null)
            {
                return false;
            }

            var name = label.Name?.ToLowerInvariant() ?? string.Empty;
            return name.Contains("status")
                   || name.Contains("hint")
                   || name.Contains("user")
                   || name.Contains("subtitle");
        }

        private static bool IsDarkColor(Color color)
        {
            if (color == Color.Empty)
            {
                return false;
            }

            return color.GetBrightness() < 0.45;
        }

        private static void ApplyRoundedCorners(Control control, int radius)
        {
            if (control == null || radius <= 0)
            {
                return;
            }

            void UpdateRegion()
            {
                if (control.Width <= 0 || control.Height <= 0)
                {
                    return;
                }

                using (var path = CreateRoundedPath(new Rectangle(0, 0, control.Width, control.Height), radius))
                {
                    var oldRegion = control.Region;
                    control.Region = new Region(path);
                    oldRegion?.Dispose();
                }
            }

            UpdateRegion();
            if (RoundedResizeHandlers.TryGetValue(control, out var existingHandler))
            {
                control.Resize -= existingHandler;
            }

            EventHandler handler = (_, __) => UpdateRegion();
            control.Resize += handler;
            if (!RoundedResizeHandlers.ContainsKey(control))
            {
                control.Disposed += (_, __) => RoundedResizeHandlers.Remove(control);
            }

            RoundedResizeHandlers[control] = handler;
        }

        private static GraphicsPath CreateRoundedPath(Rectangle bounds, int radius)
        {
            var diameter = radius * 2;
            var path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static ThemeButtonKind ResolveButtonKind(Button button)
        {
            if (button == null)
            {
                return ThemeButtonKind.Secondary;
            }

            var token = $"{button.Name} {button.Text}".ToLowerInvariant();
            if (token.Contains("delete")
                || token.Contains("logout")
                || token.Contains("remove")
                || token.Contains("discharge"))
            {
                return ThemeButtonKind.Danger;
            }

            if (token.Contains("add")
                || token.Contains("save")
                || token.Contains("login")
                || token.Contains("process")
                || token.Contains("admit"))
            {
                return ThemeButtonKind.Primary;
            }

            return ThemeButtonKind.Secondary;
        }

        private static bool ShouldStyleAsCard(Panel panel)
        {
            if (panel == null)
            {
                return false;
            }

            var name = panel.Name?.ToLowerInvariant() ?? string.Empty;
            if (name.Contains("header")
                || name.Contains("sidebar")
                || name.Contains("navigation")
                || name.Contains("menu")
                || name.Contains("content")
                || name.Contains("left")
                || name.Contains("top"))
            {
                return false;
            }

            return name.Contains("card")
                   || name.Contains("search")
                   || name.Contains("buttons")
                   || name.Contains("patients")
                   || name.Contains("doctors")
                   || name.Contains("revenue")
                   || name.Contains("container")
                   || name.Contains("summary")
                   || name.Contains("actions")
                   || (panel.BackColor == Color.White
                       && (panel.Dock == DockStyle.Top || panel.Dock == DockStyle.None)
                       && panel.Height >= 42
                       && panel.Height <= 220);
        }

        private sealed class ThemeToolStripColorTable : ProfessionalColorTable
        {
            public override Color MenuItemSelected => Colors.SurfaceMuted;

            public override Color MenuItemBorder => Colors.Primary;

            public override Color MenuBorder => Colors.Border;

            public override Color MenuItemPressedGradientBegin => Colors.Surface;

            public override Color MenuItemPressedGradientMiddle => Colors.Surface;

            public override Color MenuItemPressedGradientEnd => Colors.Surface;

            public override Color ToolStripDropDownBackground => Colors.Surface;

            public override Color ImageMarginGradientBegin => Colors.Surface;

            public override Color ImageMarginGradientMiddle => Colors.Surface;

            public override Color ImageMarginGradientEnd => Colors.Surface;

            public override Color StatusStripGradientBegin => Colors.Surface;

            public override Color StatusStripGradientEnd => Colors.Surface;
        }
    }

    /// <summary>
    /// Button style variants.
    /// </summary>
    public enum ThemeButtonKind
    {
        Secondary = 0,
        Primary = 1,
        Danger = 2
    }
}
