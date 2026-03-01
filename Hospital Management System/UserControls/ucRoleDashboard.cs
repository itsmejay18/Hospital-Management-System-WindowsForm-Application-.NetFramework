using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using HospitalManagementSystem.BLL.Services;
using HospitalManagementSystem.Helpers;

namespace HospitalManagementSystem.UserControls
{
    public partial class ucRoleDashboard : UserControl
    {
        private sealed class MetricCardView
        {
            public Panel Panel { get; set; }

            public Label TitleLabel { get; set; }

            public Label ValueLabel { get; set; }

            public Label SubtitleLabel { get; set; }
        }

        private readonly DashboardService _service = new DashboardService();
        private readonly List<MetricCardView> _metricCards = new List<MetricCardView>();

        private TableLayoutPanel _tlpRoot;
        private Label _lblTitle;
        private Label _lblSubtitle;
        private Label _lblActivityTitle;
        private Label _lblQueueTitle;
        private DataGridView _dgvActivity;
        private DataGridView _dgvQueue;
        private string _roleName = "User";
        private string _username = "user";
        private bool _isLoading;

        public ucRoleDashboard()
        {
            InitializeComponent();
            BuildLayout();
            ApplyTheme();
        }

        public void Configure(string roleName, string username)
        {
            _roleName = string.IsNullOrWhiteSpace(roleName) ? "User" : roleName.Trim();
            _username = string.IsNullOrWhiteSpace(username) ? "user" : username.Trim();

            _lblTitle.Text = $"{_roleName} Dashboard";
            _lblSubtitle.Text = BuildSubtitle(_roleName, _username);
            _lblActivityTitle.Text = BuildActivityTitle(_roleName);
            _lblQueueTitle.Text = BuildQueueTitle(_roleName);

            _ = LoadRoleDataAsync();
        }

        private async Task LoadRoleDataAsync()
        {
            if (_isLoading || IsDisposed)
            {
                return;
            }

            try
            {
                _isLoading = true;
                UseWaitCursor = true;

                var userId = UserSession.CurrentUser?.UserID ?? 0;
                var metricsTask = _service.GetRoleMetricsAsync(_roleName, userId);
                var activityTask = _service.GetRoleActivityAsync(_roleName, userId);
                var queueTask = _service.GetRoleQueueAsync(_roleName, userId);

                await Task.WhenAll(metricsTask, activityTask, queueTask).ConfigureAwait(true);

                BindMetricCards(metricsTask.Result);
                BindGrid(_dgvActivity, activityTask.Result, "No activity records available.");
                BindGrid(_dgvQueue, queueTask.Result, "No queue records available.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to load {_roleName} dashboard data: {ex.Message}", "Dashboard", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                BindGrid(_dgvActivity, null, "Unable to load activity data.");
                BindGrid(_dgvQueue, null, "Unable to load queue data.");
            }
            finally
            {
                _isLoading = false;
                UseWaitCursor = false;
            }
        }

        private void BuildLayout()
        {
            Controls.Clear();
            _metricCards.Clear();

            _tlpRoot = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(8)
            };
            _tlpRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _tlpRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
            _tlpRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 136F));
            _tlpRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var pnlHeader = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(8, 6, 8, 2)
            };
            _lblTitle = new Label
            {
                Dock = DockStyle.Top,
                Height = 34,
                TextAlign = ContentAlignment.BottomLeft,
                Font = new Font("Segoe UI Semibold", 17F, FontStyle.Regular),
                Text = "Role Dashboard"
            };
            _lblSubtitle = new Label
            {
                Dock = DockStyle.Top,
                Height = 24,
                TextAlign = ContentAlignment.TopLeft,
                Font = ThemeManager.Fonts.Regular,
                Text = "Role snapshot"
            };
            pnlHeader.Controls.Add(_lblSubtitle);
            pnlHeader.Controls.Add(_lblTitle);

            var tlpCards = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                Margin = new Padding(0, 4, 0, 4)
            };
            tlpCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

            for (var i = 0; i < 4; i++)
            {
                var cardView = CreateMetricCard();
                _metricCards.Add(cardView);
                tlpCards.Controls.Add(cardView.Panel, i, 0);
            }

            var tlpGrids = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0, 4, 0, 0)
            };
            tlpGrids.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52F));
            tlpGrids.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48F));

            var activityPanel = CreateGridCard(out _lblActivityTitle, out _dgvActivity);
            var queuePanel = CreateGridCard(out _lblQueueTitle, out _dgvQueue);

            _lblActivityTitle.Text = "Activity";
            _lblQueueTitle.Text = "Queue";

            tlpGrids.Controls.Add(activityPanel, 0, 0);
            tlpGrids.Controls.Add(queuePanel, 1, 0);

            _tlpRoot.Controls.Add(pnlHeader, 0, 0);
            _tlpRoot.Controls.Add(tlpCards, 0, 1);
            _tlpRoot.Controls.Add(tlpGrids, 0, 2);

            Controls.Add(_tlpRoot);
        }

        private MetricCardView CreateMetricCard()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(4),
                Padding = new Padding(14, 14, 14, 12),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblSubtitle = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 24,
                Font = ThemeManager.Fonts.Regular,
                TextAlign = ContentAlignment.BottomLeft,
                Text = "-"
            };

            var lblValue = new Label
            {
                Dock = DockStyle.Top,
                Height = 44,
                Font = new Font("Segoe UI Semibold", 24F, FontStyle.Regular),
                TextAlign = ContentAlignment.BottomLeft,
                Text = "0"
            };

            var lblTitle = new Label
            {
                Dock = DockStyle.Top,
                Height = 28,
                Font = new Font("Segoe UI Semibold", 11F, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "Metric"
            };

            panel.Controls.Add(lblSubtitle);
            panel.Controls.Add(lblValue);
            panel.Controls.Add(lblTitle);

            return new MetricCardView
            {
                Panel = panel,
                TitleLabel = lblTitle,
                ValueLabel = lblValue,
                SubtitleLabel = lblSubtitle
            };
        }

        private static Panel CreateGridCard(out Label titleLabel, out DataGridView grid)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(4),
                BorderStyle = BorderStyle.FixedSingle
            };

            titleLabel = new Label
            {
                Dock = DockStyle.Top,
                Height = 34,
                Padding = new Padding(12, 0, 0, 0),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI Semibold", 12F, FontStyle.Regular),
                Text = "Data"
            };

            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                MultiSelect = false
            };

            panel.Controls.Add(grid);
            panel.Controls.Add(titleLabel);
            return panel;
        }

        private void BindMetricCards(IList<DashboardService.RoleMetricItem> metrics)
        {
            var safeMetrics = metrics == null
                ? new List<DashboardService.RoleMetricItem>()
                : new List<DashboardService.RoleMetricItem>(metrics);

            while (safeMetrics.Count < _metricCards.Count)
            {
                safeMetrics.Add(new DashboardService.RoleMetricItem("Metric", 0, "No data"));
            }

            for (var i = 0; i < _metricCards.Count; i++)
            {
                var metric = safeMetrics[i];
                var card = _metricCards[i];
                card.TitleLabel.Text = metric.Title;
                card.ValueLabel.Text = metric.IsCurrency
                    ? FormatPeso(metric.Value)
                    : FormatValue(metric.Value);
                card.SubtitleLabel.Text = string.IsNullOrWhiteSpace(metric.Subtitle) ? " " : metric.Subtitle;
            }
        }

        private void BindGrid(DataGridView grid, DataTable table, string emptyMessage)
        {
            if (grid == null)
            {
                return;
            }

            if (table == null || table.Rows.Count == 0)
            {
                var fallback = new DataTable();
                fallback.Columns.Add("Info");
                fallback.Rows.Add(emptyMessage);
                grid.DataSource = fallback;
            }
            else
            {
                grid.DataSource = table;
            }

            ThemeManager.StyleDataGridView(grid);
        }

        private void ApplyTheme()
        {
            ThemeManager.ApplyControlTheme(this);
            BackColor = ThemeManager.Colors.Background;
            if (_tlpRoot != null)
            {
                _tlpRoot.BackColor = ThemeManager.Colors.Background;
            }

            if (_lblTitle != null)
            {
                _lblTitle.ForeColor = ThemeManager.Colors.TextPrimary;
            }

            if (_lblSubtitle != null)
            {
                _lblSubtitle.ForeColor = ThemeManager.Colors.TextSecondary;
            }

            if (_lblActivityTitle != null)
            {
                _lblActivityTitle.ForeColor = ThemeManager.Colors.TextPrimary;
            }

            if (_lblQueueTitle != null)
            {
                _lblQueueTitle.ForeColor = ThemeManager.Colors.TextPrimary;
            }

            foreach (var card in _metricCards)
            {
                ThemeManager.StyleCardPanel(card.Panel);
                card.TitleLabel.ForeColor = ThemeManager.Colors.TextSecondary;
                card.ValueLabel.ForeColor = ThemeManager.Colors.TextPrimary;
                card.SubtitleLabel.ForeColor = ThemeManager.Colors.TextSecondary;
            }

            ThemeManager.StyleDataGridView(_dgvActivity);
            ThemeManager.StyleDataGridView(_dgvQueue);
        }

        private static string BuildSubtitle(string roleName, string username)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return $"Welcome, {username}.";
            }

            return $"Welcome, {username}. Live {roleName.ToLowerInvariant()} operations from database records.";
        }

        private static string BuildActivityTitle(string roleName)
        {
            if (string.Equals(roleName, "Doctor", StringComparison.OrdinalIgnoreCase))
            {
                return "Upcoming Appointments";
            }

            if (string.Equals(roleName, "Nurse", StringComparison.OrdinalIgnoreCase))
            {
                return "Current Admissions";
            }

            if (string.Equals(roleName, "Receptionist", StringComparison.OrdinalIgnoreCase))
            {
                return "Today's Front Desk Queue";
            }

            if (string.Equals(roleName, "Pharmacist", StringComparison.OrdinalIgnoreCase))
            {
                return "Recent Pharmacy Sales";
            }

            if (string.Equals(roleName, "Lab Technician", StringComparison.OrdinalIgnoreCase))
            {
                return "Lab Worklist";
            }

            if (string.Equals(roleName, "Accountant", StringComparison.OrdinalIgnoreCase))
            {
                return "Invoice Operations";
            }

            if (string.Equals(roleName, "HR Manager", StringComparison.OrdinalIgnoreCase))
            {
                return "Staff Directory";
            }

            return "Recent Activity";
        }

        private static string BuildQueueTitle(string roleName)
        {
            if (string.Equals(roleName, "Doctor", StringComparison.OrdinalIgnoreCase))
            {
                return "Completed Consultations";
            }

            if (string.Equals(roleName, "Nurse", StringComparison.OrdinalIgnoreCase))
            {
                return "Room Occupancy Queue";
            }

            if (string.Equals(roleName, "Receptionist", StringComparison.OrdinalIgnoreCase))
            {
                return "Pending Billing Queue";
            }

            if (string.Equals(roleName, "Pharmacist", StringComparison.OrdinalIgnoreCase))
            {
                return "Low Stock Queue";
            }

            if (string.Equals(roleName, "Lab Technician", StringComparison.OrdinalIgnoreCase))
            {
                return "Completed Results";
            }

            if (string.Equals(roleName, "Accountant", StringComparison.OrdinalIgnoreCase))
            {
                return "Recent Payments";
            }

            if (string.Equals(roleName, "HR Manager", StringComparison.OrdinalIgnoreCase))
            {
                return "User Account Status";
            }

            return "Work Queue";
        }

        private static string FormatValue(decimal value)
        {
            if (Math.Abs(value % 1m) > 0m)
            {
                return value.ToString("N2", CultureInfo.GetCultureInfo("en-PH"));
            }

            return value.ToString("N0", CultureInfo.GetCultureInfo("en-PH"));
        }

        private static string FormatPeso(decimal value)
        {
            return string.Concat("\u20B1", value.ToString("N2", CultureInfo.GetCultureInfo("en-PH")));
        }
    }
}
