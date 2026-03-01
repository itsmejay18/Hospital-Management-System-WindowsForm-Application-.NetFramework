using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using HospitalManagementSystem.BLL.Services;
using HospitalManagementSystem.Helpers;

namespace HospitalManagementSystem.UserControls
{
    public sealed class ucProfile : UserControl
    {
        private readonly AuthenticatedUser _user;
        private SplitContainer _profileSplit;
        private const int PreferredLeftPaneWidth = 320;

        private Label _lblCardName;
        private Label _lblCardRole;
        private PictureBox _picProfile;
        private TextBox _txtUsername;
        private TextBox _txtFullName;
        private TextBox _txtEmail;
        private TextBox _txtPhone;
        private TextBox _txtRole;
        private Button _btnUploadPhoto;
        private Button _btnSaveProfile;

        public ucProfile(AuthenticatedUser user)
        {
            _user = user ?? new AuthenticatedUser();
            BuildLayout();
            ApplyTheme();
            BindProfile();
        }

        private void BuildLayout()
        {
            BackColor = ThemeManager.Colors.Background;
            Dock = DockStyle.Fill;

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = ThemeManager.Colors.Background,
                Padding = new Padding(0)
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 92F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var pnlHeader = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(4, 4, 4, 8),
                Padding = new Padding(18, 14, 12, 8)
            };

            var lblTitle = new Label
            {
                Dock = DockStyle.Top,
                Height = 38,
                Text = "My Profile",
                Font = new Font("Segoe UI Semibold", 20F, FontStyle.Regular),
                ForeColor = ThemeManager.Colors.TextPrimary
            };

            var lblSubtitle = new Label
            {
                Dock = DockStyle.Top,
                Height = 28,
                Text = "Manage your account details and photo",
                Font = ThemeManager.Fonts.Regular,
                ForeColor = ThemeManager.Colors.TextSecondary
            };

            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Controls.Add(lblTitle);

            _profileSplit = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(4),
                BackColor = ThemeManager.Colors.Border,
                SplitterWidth = 6,
                Panel1MinSize = 0,
                Panel2MinSize = 0
            };

            var pnlLeft = BuildProfileCard();
            var pnlRight = BuildAccountForm();

            _profileSplit.Panel1.Padding = new Padding(0);
            _profileSplit.Panel2.Padding = new Padding(0);
            _profileSplit.Panel1.Controls.Add(pnlLeft);
            _profileSplit.Panel2.Controls.Add(pnlRight);
            _profileSplit.SizeChanged += (_, __) => ApplyProfileSplitDistance();

            root.Controls.Add(pnlHeader, 0, 0);
            root.Controls.Add(_profileSplit, 0, 1);
            Controls.Add(root);
            Load += (_, __) => ApplyProfileSplitDistance();
        }

        private Control BuildProfileCard()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(14, 18, 14, 14),
                Margin = new Padding(0)
            };

            _lblCardName = new Label
            {
                Dock = DockStyle.Top,
                Height = 36,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI Semibold", 18F, FontStyle.Regular)
            };

            _lblCardRole = new Label
            {
                Dock = DockStyle.Top,
                Height = 26,
                TextAlign = ContentAlignment.MiddleCenter
            };

            var photoFrame = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                Margin = new Padding(0),
                BorderStyle = BorderStyle.FixedSingle
            };

            _picProfile = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            photoFrame.Controls.Add(_picProfile);

            panel.Controls.Add(photoFrame);
            panel.Controls.Add(_lblCardRole);
            panel.Controls.Add(_lblCardName);
            return panel;
        }

        private Control BuildAccountForm()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(18, 18, 18, 14),
                Margin = new Padding(0)
            };

            var lblSection = new Label
            {
                Dock = DockStyle.Top,
                Height = 34,
                Text = "Account Information",
                Font = new Font("Segoe UI Semibold", 16F, FontStyle.Regular)
            };

            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 2,
                RowCount = 6,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(0, 8, 0, 0)
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            _txtUsername = CreateTextBox();
            _txtFullName = CreateTextBox();
            _txtEmail = CreateTextBox();
            _txtPhone = CreateTextBox();
            _txtRole = CreateTextBox();
            _txtRole.ReadOnly = true;

            AddField(table, 0, "Username", _txtUsername);
            AddField(table, 1, "Full Name", _txtFullName);
            AddField(table, 2, "Email", _txtEmail);
            AddField(table, 3, "Phone", _txtPhone);
            AddField(table, 4, "Role", _txtRole);

            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(0),
                WrapContents = false
            };

            _btnUploadPhoto = new Button
            {
                Text = "Upload Photo",
                Width = 130,
                Height = 36,
                Margin = new Padding(0, 0, 10, 0)
            };

            _btnSaveProfile = new Button
            {
                Text = "Save Profile",
                Width = 130,
                Height = 36,
                Margin = new Padding(0)
            };

            _btnUploadPhoto.Click += btnUploadPhoto_Click;
            _btnSaveProfile.Click += btnSaveProfile_Click;

            buttonPanel.Controls.Add(_btnUploadPhoto);
            buttonPanel.Controls.Add(_btnSaveProfile);

            panel.Controls.Add(buttonPanel);
            panel.Controls.Add(table);
            panel.Controls.Add(lblSection);
            return panel;
        }

        private static TextBox CreateTextBox()
        {
            return new TextBox
            {
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 8),
                Height = 30
            };
        }

        private static void AddField(TableLayoutPanel table, int rowIndex, string labelText, Control input)
        {
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));

            var label = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = labelText
            };

            var host = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 8, 0, 0)
            };
            host.Controls.Add(input);

            table.Controls.Add(label, 0, rowIndex);
            table.Controls.Add(host, 1, rowIndex);
        }

        private void BindProfile()
        {
            var role = string.IsNullOrWhiteSpace(_user.RoleName) ? "User" : _user.RoleName;
            var username = string.IsNullOrWhiteSpace(_user.Username) ? "admin" : _user.Username;
            var fullName = BuildDisplayName(username);

            _lblCardName.Text = fullName;
            _lblCardRole.Text = role;
            _txtUsername.Text = username;
            _txtFullName.Text = fullName;
            _txtEmail.Text = $"{username}@hospital.local";
            _txtPhone.Text = "+63 9XX XXX XXXX";
            _txtRole.Text = role;
            ThemeManager.ApplyBrandingLogo(_picProfile);
        }

        private static string BuildDisplayName(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return "Administrator";
            }

            var cleaned = username.Replace('.', ' ').Replace('_', ' ').Trim();
            if (cleaned.Length == 0)
            {
                return "Administrator";
            }

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cleaned);
        }

        private void btnUploadPhoto_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Select Profile Photo";
                dialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp";
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    using (var image = Image.FromFile(dialog.FileName))
                    {
                        var old = _picProfile.Image;
                        _picProfile.Image = new Bitmap(image);
                        old?.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unable to load image: {ex.Message}", "Profile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSaveProfile_Click(object sender, EventArgs e)
        {
            _lblCardName.Text = string.IsNullOrWhiteSpace(_txtFullName.Text) ? _lblCardName.Text : _txtFullName.Text.Trim();
            _txtRole.Text = string.IsNullOrWhiteSpace(_txtRole.Text) ? _lblCardRole.Text : _txtRole.Text.Trim();
            _lblCardRole.Text = _txtRole.Text;
            MessageBox.Show("Profile details updated successfully.", "Profile", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ApplyTheme()
        {
            ThemeManager.ApplyControlTheme(this);
            if (_profileSplit != null && !_profileSplit.IsDisposed)
            {
                ThemeManager.StyleSplitContainer(_profileSplit);
            }

            foreach (Control control in Controls)
            {
                if (control is TableLayoutPanel table && table.Controls.Count > 1)
                {
                    foreach (Control nested in table.Controls)
                    {
                        if (nested is SplitContainer split)
                        {
                            ThemeManager.StyleSplitContainer(split);
                            if (split.Panel1.Controls.Count > 0 && split.Panel1.Controls[0] is Panel panel1)
                            {
                                ThemeManager.StyleCardPanel(panel1);
                            }

                            if (split.Panel2.Controls.Count > 0 && split.Panel2.Controls[0] is Panel panel2)
                            {
                                ThemeManager.StyleCardPanel(panel2);
                            }
                        }
                    }
                }
            }

            ThemeManager.StyleButton(_btnUploadPhoto, ThemeButtonKind.Secondary);
            ThemeManager.StyleButton(_btnSaveProfile, ThemeButtonKind.Primary);
            ThemeManager.StyleTextBox(_txtUsername);
            ThemeManager.StyleTextBox(_txtFullName);
            ThemeManager.StyleTextBox(_txtEmail);
            ThemeManager.StyleTextBox(_txtPhone);
            ThemeManager.StyleTextBox(_txtRole);
            _txtRole.BackColor = ThemeManager.Colors.SurfaceMuted;
            _lblCardRole.ForeColor = ThemeManager.Colors.TextSecondary;
        }

        private void ApplyProfileSplitDistance()
        {
            if (_profileSplit == null || _profileSplit.IsDisposed)
            {
                return;
            }

            var width = _profileSplit.ClientSize.Width;
            if (width <= 0)
            {
                return;
            }

            try
            {
                var splitter = Math.Max(1, _profileSplit.SplitterWidth);
                var available = Math.Max(0, width - splitter);
                if (available <= 0)
                {
                    return;
                }

                var minPane = 120;
                var min = Math.Min(minPane, Math.Max(0, available / 2));
                var max = Math.Max(min, available - minPane);
                if (max < min)
                {
                    var mid = available / 2;
                    min = mid;
                    max = mid;
                }

                var target = Math.Max(min, Math.Min(max, PreferredLeftPaneWidth));
                if (target == _profileSplit.SplitterDistance)
                {
                    return;
                }

                _profileSplit.SplitterDistance = target;
            }
            catch (InvalidOperationException)
            {
                // Ignore transient layout edge cases during early control initialization.
            }
        }
    }
}
