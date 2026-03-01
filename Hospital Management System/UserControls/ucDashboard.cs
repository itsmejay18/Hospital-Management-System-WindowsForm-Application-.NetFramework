using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HospitalManagementSystem.BLL.Services;
using HospitalManagementSystem.Helpers;

namespace HospitalManagementSystem.UserControls
{
    public partial class ucDashboard : UserControl
    {
        private readonly DashboardService _service = new DashboardService();
        private readonly ReportService _reportService = new ReportService();

        private static readonly Color[] PiePalette =
        {
            ColorTranslator.FromHtml("#19C2AF"),
            ColorTranslator.FromHtml("#10B99D"),
            ColorTranslator.FromHtml("#0FA388"),
            ColorTranslator.FromHtml("#0A8A73"),
            ColorTranslator.FromHtml("#36D2BA")
        };

        private Panel _pnlGreeting;
        private Label _lblGreetingTitle;
        private Label _lblGreetingSubtitle;
        private Label _lblDateChip;

        private Panel _pnlAppointments;
        private PictureBox _picAppointmentsIcon;
        private Label _lblAppointmentsValue;
        private Label _lblAppointments;
        private Label _lblAppointmentsTrend;
        private Label _lblPatientsTrend;
        private Label _lblDoctorsTrend;
        private Label _lblRevenueTrend;

        private Panel _pnlSchedule;
        private Label _lblScheduleHeader;
        private Panel _pnlScheduleCalendar;
        private FlowLayoutPanel _flpSchedule;
        private Panel _pnlChartMeta;
        private Label _lblChartRangeWeek;
        private Label _lblChartRangeMonth;
        private Label _lblChartRangeYear;

        private TableLayoutPanel _tlpBottom;
        private Panel _pnlRecentUpdates;
        private Panel _pnlStaffPerformance;
        private DataGridView _dgvRecentUpdates;
        private DataGridView _dgvStaffPerformance;
        private Label _lblDistributionSummary;
        private const int StatIconTop = 14;
        private const int StatIconRightMargin = 14;

        public ucDashboard()
        {
            InitializeComponent();
            EnsureDashboardLayout();
            ApplyTheme();
            Load += ucDashboard_Load;
        }

        private async void ucDashboard_Load(object sender, EventArgs e)
        {
            try
            {
                UseWaitCursor = true;
                var todayAppointmentsTask = _service.GetTodayAppointmentsAsync();
                var pendingApprovalsTask = _service.GetPendingApprovalsAsync();
                var monthlyCollectionsTask = _service.GetCurrentMonthCollectionsAsync();
                var occupancyRateTask = _service.GetRoomOccupancyRateAsync();
                var appointmentTrendTask = _service.GetMonthlyAppointmentTrendAsync(6);
                var distributionTask = _service.GetEntityDistributionAsync();
                var recentUpdatesTask = LoadRecentUpdatesSafeAsync();
                var staffPerformanceTask = LoadStaffPerformanceSafeAsync();

                await Task.WhenAll(
                    todayAppointmentsTask,
                    pendingApprovalsTask,
                    monthlyCollectionsTask,
                    occupancyRateTask,
                    appointmentTrendTask,
                    distributionTask,
                    recentUpdatesTask,
                    staffPerformanceTask).ConfigureAwait(true);

                ApplySummaryData(
                    todayAppointmentsTask.Result,
                    pendingApprovalsTask.Result,
                    monthlyCollectionsTask.Result,
                    occupancyRateTask.Result,
                    distributionTask.Result);
                BindDistributionChart(distributionTask.Result);
                BindActivityTrendChart(appointmentTrendTask.Result);
                PopulateRecentUpdatesGrid(recentUpdatesTask.Result);
                PopulateStaffPerformanceGrid(staffPerformanceTask.Result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dashboard error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void EnsureDashboardLayout()
        {
            EnsureAppointmentsCard();
            EnsureTopCardDetails();
            EnsureBottomTables();
            EnsureDistributionSummary();

            tlpMain.SuspendLayout();
            tlpMain.Controls.Clear();
            tlpMain.RowStyles.Clear();
            tlpMain.RowCount = 3;
            tlpMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 120F));
            tlpMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 300F));
            tlpMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpMain.Controls.Add(tlpCards, 0, 0);
            tlpMain.Controls.Add(tlpCharts, 0, 1);
            tlpMain.Controls.Add(_tlpBottom, 0, 2);
            tlpMain.ResumeLayout();

            tlpCards.SuspendLayout();
            tlpCards.ColumnStyles.Clear();
            tlpCards.ColumnCount = 4;
            tlpCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            if (!tlpCards.Controls.Contains(_pnlAppointments))
            {
                tlpCards.Controls.Add(_pnlAppointments, 0, 0);
            }

            tlpCards.SetCellPosition(_pnlAppointments, new TableLayoutPanelCellPosition(0, 0));
            tlpCards.SetCellPosition(pnlPatients, new TableLayoutPanelCellPosition(1, 0));
            tlpCards.SetCellPosition(pnlDoctors, new TableLayoutPanelCellPosition(2, 0));
            tlpCards.SetCellPosition(pnlRevenue, new TableLayoutPanelCellPosition(3, 0));
            tlpCards.ResumeLayout();
            AlignSummaryCardIcons();

            tlpCharts.SuspendLayout();
            tlpCharts.Controls.Clear();
            tlpCharts.ColumnStyles.Clear();
            tlpCharts.ColumnCount = 2;
            tlpCharts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80F));
            tlpCharts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tlpCharts.Controls.Add(pnlRevenueChart, 0, 0);
            tlpCharts.Controls.Add(pnlGenderChart, 1, 0);
            tlpCharts.ResumeLayout();
        }

        private void EnsureGreetingPanel()
        {
            if (_pnlGreeting != null)
            {
                return;
            }

            _pnlGreeting = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(4, 0, 4, 8),
                Padding = new Padding(0, 0, 0, 0),
                BackColor = ThemeManager.Colors.Background
            };

            var tlpGreeting = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };
            tlpGreeting.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            tlpGreeting.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));

            var pnlWelcome = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(8, 8, 8, 8)
            };

            _lblGreetingTitle = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 34,
                TextAlign = ContentAlignment.BottomLeft,
                Font = new Font("Segoe UI Semibold", 15F, FontStyle.Regular),
                Text = "Hello"
            };

            _lblGreetingSubtitle = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 24,
                TextAlign = ContentAlignment.TopLeft,
                Font = ThemeManager.Fonts.Regular,
                Text = "Here is the latest update for the last 7 days."
            };

            pnlWelcome.Controls.Add(_lblGreetingSubtitle);
            pnlWelcome.Controls.Add(_lblGreetingTitle);

            var pnlDate = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(8, 18, 8, 8)
            };

            _lblDateChip = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Right,
                Width = 192,
                Height = 38,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                Font = ThemeManager.Fonts.Medium
            };

            pnlDate.Controls.Add(_lblDateChip);
            tlpGreeting.Controls.Add(pnlWelcome, 0, 0);
            tlpGreeting.Controls.Add(pnlDate, 1, 0);
            _pnlGreeting.Controls.Add(tlpGreeting);
        }

        private void EnsureAppointmentsCard()
        {
            if (_pnlAppointments != null)
            {
                return;
            }

            _pnlAppointments = new Panel
            {
                Name = "pnlAppointments",
                BackColor = ThemeManager.Colors.Surface,
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill,
                Margin = new Padding(4)
            };

            _picAppointmentsIcon = new PictureBox
            {
                Name = "picAppointmentsIcon",
                Location = new Point(14, 14),
                Size = new Size(40, 40),
                SizeMode = PictureBoxSizeMode.Zoom
            };

            _lblAppointments = new Label
            {
                Name = "lblAppointments",
                AutoSize = true,
                Font = new Font("Segoe UI Semibold", 11F, FontStyle.Regular),
                Location = new Point(62, 22),
                Text = "Appointments"
            };

            _lblAppointmentsValue = new Label
            {
                Name = "lblAppointmentsValue",
                AutoSize = true,
                Font = new Font("Segoe UI Semibold", 22F, FontStyle.Regular),
                Location = new Point(14, 76),
                Text = "0"
            };

            _pnlAppointments.Controls.Add(_picAppointmentsIcon);
            _pnlAppointments.Controls.Add(_lblAppointments);
            _pnlAppointments.Controls.Add(_lblAppointmentsValue);
        }

        private void EnsureTopCardDetails()
        {
            if (_lblAppointmentsTrend == null)
            {
                _lblAppointmentsTrend = new Label
                {
                    AutoSize = true,
                    Location = new Point(16, 118),
                    Text = "from last week"
                };
                _pnlAppointments.Controls.Add(_lblAppointmentsTrend);
            }

            if (_lblPatientsTrend == null)
            {
                _lblPatientsTrend = new Label
                {
                    AutoSize = true,
                    Location = new Point(14, 118),
                    Text = "active records"
                };
                pnlPatients.Controls.Add(_lblPatientsTrend);
            }

            if (_lblDoctorsTrend == null)
            {
                _lblDoctorsTrend = new Label
                {
                    AutoSize = true,
                    Location = new Point(14, 118),
                    Text = "on shift"
                };
                pnlDoctors.Controls.Add(_lblDoctorsTrend);
            }

            if (_lblRevenueTrend == null)
            {
                _lblRevenueTrend = new Label
                {
                    AutoSize = true,
                    Location = new Point(14, 118),
                    Text = "weekly performance"
                };
                pnlRevenue.Controls.Add(_lblRevenueTrend);
            }

            pnlPatients.Resize -= SummaryCard_Resize;
            pnlDoctors.Resize -= SummaryCard_Resize;
            pnlRevenue.Resize -= SummaryCard_Resize;
            pnlPatients.Resize += SummaryCard_Resize;
            pnlDoctors.Resize += SummaryCard_Resize;
            pnlRevenue.Resize += SummaryCard_Resize;
        }

        private void EnsureSchedulePanel()
        {
            if (_pnlSchedule != null)
            {
                return;
            }

            _pnlSchedule = new Panel
            {
                Name = "pnlSchedule",
                Dock = DockStyle.Fill,
                Margin = new Padding(4)
            };

            _lblScheduleHeader = new Label
            {
                Dock = DockStyle.Top,
                Height = 44,
                Padding = new Padding(12, 0, 0, 0),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI Semibold", 12F, FontStyle.Regular),
                Text = "Today Schedule"
            };

            _pnlScheduleCalendar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 78,
                Padding = new Padding(10, 8, 10, 6)
            };
            BuildScheduleCalendarStrip();

            _flpSchedule = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(12, 6, 12, 8)
            };
            _flpSchedule.Resize += (_, __) => ResizeScheduleItems();

            _pnlSchedule.Controls.Add(_flpSchedule);
            _pnlSchedule.Controls.Add(_pnlScheduleCalendar);
            _pnlSchedule.Controls.Add(_lblScheduleHeader);
            BuildScheduleRows();
        }

        private void BuildScheduleCalendarStrip()
        {
            if (_pnlScheduleCalendar == null)
            {
                return;
            }

            _pnlScheduleCalendar.Controls.Clear();
            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 6,
                RowCount = 1,
                BackColor = ThemeManager.Colors.SurfaceMuted
            };

            for (var i = 0; i < 6; i++)
            {
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.6667F));
            }

            var start = DateTime.Now.Date;
            for (var i = 0; i < 6; i++)
            {
                var day = start.AddDays(i);
                var panel = new Panel
                {
                    Dock = DockStyle.Fill,
                    Margin = new Padding(0),
                    Padding = new Padding(0),
                    BackColor = i == 1 ? ThemeManager.Colors.Primary : ThemeManager.Colors.SurfaceMuted
                };

                var lblDay = new Label
                {
                    Dock = DockStyle.Top,
                    Height = 30,
                    TextAlign = ContentAlignment.BottomCenter,
                    Font = new Font("Segoe UI Semibold", 10F, FontStyle.Regular),
                    Text = day.Day.ToString(CultureInfo.InvariantCulture),
                    ForeColor = i == 1 ? Color.White : ThemeManager.Colors.TextPrimary
                };

                var lblDow = new Label
                {
                    Dock = DockStyle.Top,
                    Height = 18,
                    TextAlign = ContentAlignment.TopCenter,
                    Font = new Font("Segoe UI", 8F, FontStyle.Regular),
                    Text = day.ToString("ddd", CultureInfo.InvariantCulture).ToLowerInvariant(),
                    ForeColor = i == 1 ? Color.White : ThemeManager.Colors.TextSecondary
                };

                panel.Controls.Add(lblDow);
                panel.Controls.Add(lblDay);
                table.Controls.Add(panel, i, 0);
            }

            _pnlScheduleCalendar.Controls.Add(table);
        }

        private void EnsureChartMeta()
        {
            if (_pnlChartMeta != null)
            {
                return;
            }

            _pnlChartMeta = new Panel
            {
                Dock = DockStyle.None,
                Size = new Size(196, 24),
                Padding = new Padding(0, 0, 10, 0),
                BackColor = ThemeManager.Colors.Surface
            };

            _lblChartRangeYear = new Label
            {
                Dock = DockStyle.Right,
                Width = 84,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Year-2026"
            };

            _lblChartRangeMonth = new Label
            {
                Dock = DockStyle.Right,
                Width = 56,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Month"
            };

            _lblChartRangeWeek = new Label
            {
                Dock = DockStyle.Right,
                Width = 52,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Week"
            };

            _pnlChartMeta.Controls.Add(_lblChartRangeYear);
            _pnlChartMeta.Controls.Add(_lblChartRangeMonth);
            _pnlChartMeta.Controls.Add(_lblChartRangeWeek);
            pnlRevenueChart.Controls.Add(_pnlChartMeta);
            _pnlChartMeta.BringToFront();
            RepositionChartMeta();
            pnlRevenueChart.Resize += (_, __) => RepositionChartMeta();
        }

        private void RepositionChartMeta()
        {
            if (_pnlChartMeta == null)
            {
                return;
            }

            _pnlChartMeta.Location = new Point(Math.Max(120, pnlRevenueChart.ClientSize.Width - _pnlChartMeta.Width - 8), 6);
        }

        private void BuildScheduleRows()
        {
            _flpSchedule.Controls.Clear();
            _flpSchedule.Controls.Add(CreateScheduleItem("09:00", "Doctor rounds", "09:00am - 10:00am"));
            _flpSchedule.Controls.Add(CreateScheduleItem("10:00", "Dentist meetup", "10:00am - 11:00am"));
            _flpSchedule.Controls.Add(CreateScheduleItem("12:00", "Procedures", "12:00pm - 04:00pm"));
            _flpSchedule.Controls.Add(CreateScheduleItem("04:00", "Billing review", "04:00pm - 05:00pm"));
            ResizeScheduleItems();
        }

        private void ResizeScheduleItems()
        {
            if (_flpSchedule == null)
            {
                return;
            }

            var width = Math.Max(246, _flpSchedule.ClientSize.Width - 24);
            foreach (Control control in _flpSchedule.Controls)
            {
                control.Width = width;
            }
        }

        private Control CreateScheduleItem(string time, string title, string duration)
        {
            var wrapper = new Panel
            {
                Width = 270,
                Height = 74,
                Margin = new Padding(0, 0, 0, 8),
                BackColor = ThemeManager.Colors.Surface
            };

            var timeLabel = new Label
            {
                AutoSize = false,
                Width = 56,
                Dock = DockStyle.Left,
                Text = time,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = ThemeManager.Colors.TextSecondary,
                Font = ThemeManager.Fonts.Regular
            };

            var entry = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10, 8, 8, 6),
                BackColor = ThemeManager.Colors.SurfaceMuted
            };

            var leftBar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 4,
                BackColor = ThemeManager.Colors.Primary
            };

            var lblTitle = new Label
            {
                Dock = DockStyle.Top,
                Height = 22,
                Text = title,
                Font = new Font("Segoe UI Semibold", 10F, FontStyle.Regular),
                ForeColor = ThemeManager.Colors.TextPrimary
            };

            var lblDuration = new Label
            {
                Dock = DockStyle.Top,
                Height = 18,
                Text = duration,
                Font = ThemeManager.Fonts.Regular,
                ForeColor = ThemeManager.Colors.TextSecondary
            };

            entry.Controls.Add(lblDuration);
            entry.Controls.Add(lblTitle);
            entry.Controls.Add(leftBar);
            wrapper.Controls.Add(entry);
            wrapper.Controls.Add(timeLabel);
            return wrapper;
        }

        private void EnsureBottomTables()
        {
            if (_tlpBottom != null)
            {
                return;
            }

            _tlpBottom = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                Margin = new Padding(0, 8, 0, 0)
            };
            _tlpBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
            _tlpBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58F));

            _pnlRecentUpdates = CreateGridCard("Recent Updates", out _dgvRecentUpdates);
            _pnlStaffPerformance = CreateGridCard("Staff Performance", out _dgvStaffPerformance);

            ConfigureRecentUpdatesGrid();
            ConfigureStaffPerformanceGrid();

            _tlpBottom.Controls.Add(_pnlRecentUpdates, 0, 0);
            _tlpBottom.Controls.Add(_pnlStaffPerformance, 1, 0);
        }

        private static Panel CreateGridCard(string title, out DataGridView grid)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(4)
            };

            var lblTitle = new Label
            {
                Dock = DockStyle.Top,
                Height = 36,
                Padding = new Padding(10, 0, 0, 0),
                Text = title,
                Font = new Font("Segoe UI Semibold", 12F, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft
            };

            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            panel.Controls.Add(grid);
            panel.Controls.Add(lblTitle);
            return panel;
        }

        private void ConfigureRecentUpdatesGrid()
        {
            if (_dgvRecentUpdates == null)
            {
                return;
            }

            _dgvRecentUpdates.Columns.Clear();
            _dgvRecentUpdates.Columns.Add("colType", "Type");
            _dgvRecentUpdates.Columns.Add("colRef", "ReferenceNo");
            _dgvRecentUpdates.Columns.Add("colStatus", "Status");
            _dgvRecentUpdates.Columns.Add("colUpdated", "UpdatedAt");
        }

        private void ConfigureStaffPerformanceGrid()
        {
            if (_dgvStaffPerformance == null)
            {
                return;
            }

            _dgvStaffPerformance.Columns.Clear();
            _dgvStaffPerformance.Columns.Add("colStaff", "Staff");
            _dgvStaffPerformance.Columns.Add("colRole", "Role");
            _dgvStaffPerformance.Columns.Add("colConsultations", "Consultations");
            _dgvStaffPerformance.Columns.Add("colCompleted", "Completed");
            _dgvStaffPerformance.Columns.Add("colOverdue", "Cancelled");
            _dgvStaffPerformance.Columns.Add("colRevenue", "Revenue");
            _dgvStaffPerformance.Columns.Add("colPending", "Pending");
        }

        private void EnsureDistributionSummary()
        {
            if (_lblDistributionSummary != null)
            {
                return;
            }

            _lblDistributionSummary = new Label
            {
                Dock = DockStyle.None,
                Height = 80,
                Padding = new Padding(12, 8, 12, 8),
                TextAlign = ContentAlignment.TopLeft
            };

            pnlGenderChart.Controls.Add(_lblDistributionSummary);
            _lblDistributionSummary.BringToFront();
            chartGender.Dock = DockStyle.None;
            pnlGenderChart.Resize -= GenderChartHost_Resize;
            pnlGenderChart.Resize += GenderChartHost_Resize;
            LayoutDistributionChart();
        }

        private void ApplyTheme()
        {
            ThemeManager.ApplyControlTheme(this);

            ThemeManager.StyleCardPanel(pnlPatients);
            ThemeManager.StyleCardPanel(pnlDoctors);
            ThemeManager.StyleCardPanel(_pnlAppointments);
            ThemeManager.StyleCardPanel(pnlRevenue);
            ThemeManager.StyleCardPanel(pnlGenderChart);
            ThemeManager.StyleCardPanel(pnlRevenueChart);
            ThemeManager.StyleCardPanel(_pnlRecentUpdates);
            ThemeManager.StyleCardPanel(_pnlStaffPerformance);

            BackColor = ThemeManager.Colors.Background;
            tlpMain.BackColor = ThemeManager.Colors.Background;
            tlpCards.BackColor = ThemeManager.Colors.Background;
            tlpCharts.BackColor = ThemeManager.Colors.Background;
            _tlpBottom.BackColor = ThemeManager.Colors.Background;

            lblPatients.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Regular);
            lblDoctors.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Regular);
            lblRevenue.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Regular);
            _lblAppointments.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Regular);
            _lblAppointmentsTrend.Font = ThemeManager.Fonts.Regular;
            _lblPatientsTrend.Font = ThemeManager.Fonts.Regular;
            _lblDoctorsTrend.Font = ThemeManager.Fonts.Regular;
            _lblRevenueTrend.Font = ThemeManager.Fonts.Regular;

            lblPatientsValue.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Regular);
            lblDoctorsValue.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Regular);
            lblRevenueValue.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Regular);
            _lblAppointmentsValue.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Regular);

            lblRevenueChartTitle.Text = "Operational Trend (Last 6 Months)";
            lblGenderChartTitle.Text = "Entity Distribution";
            lblRevenueChartTitle.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Regular);

            _pnlAppointments.BackColor = ColorTranslator.FromHtml("#0E6D67");
            _lblAppointments.ForeColor = Color.White;
            _lblAppointmentsValue.ForeColor = Color.White;
            _lblAppointmentsTrend.ForeColor = ColorTranslator.FromHtml("#C8FFF4");
            _lblPatientsTrend.ForeColor = ThemeManager.Colors.TextSecondary;
            _lblDoctorsTrend.ForeColor = ThemeManager.Colors.TextSecondary;
            _lblRevenueTrend.ForeColor = ThemeManager.Colors.TextSecondary;

            if (_lblGreetingTitle != null)
            {
                _lblGreetingTitle.ForeColor = ThemeManager.Colors.TextPrimary;
            }

            if (_lblGreetingSubtitle != null)
            {
                _lblGreetingSubtitle.ForeColor = ThemeManager.Colors.TextSecondary;
            }

            if (_lblDateChip != null)
            {
                _lblDateChip.BackColor = ThemeManager.Colors.Background;
                _lblDateChip.BorderStyle = BorderStyle.None;
                _lblDateChip.ForeColor = ThemeManager.Colors.TextSecondary;
                _lblDateChip.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Regular);
            }

            SetPictureIcon(_picAppointmentsIcon, CreateStatIcon(StatIconType.Appointments, Color.White, ColorTranslator.FromHtml("#0E6D67")));
            SetPictureIcon(picPatientsIcon, CreateStatIcon(StatIconType.Patients, ColorTranslator.FromHtml("#19C2AF"), Color.White));
            SetPictureIcon(picDoctorsIcon, CreateStatIcon(StatIconType.Doctors, ColorTranslator.FromHtml("#10B99D"), Color.White));
            SetPictureIcon(picRevenueIcon, CreateStatIcon(StatIconType.Revenue, ColorTranslator.FromHtml("#0FB89D"), Color.White));
            AlignSummaryCardIcons();

            ConfigureChartSurface(chartRevenueTrend, hideAxes: false);
            ConfigureChartSurface(chartGender, hideAxes: true);
            pnlGenderChart.Visible = true;

            var totalSeries = chartRevenueTrend.Series["srRevenue"];
            totalSeries.LegendText = "Total Appointments";
            totalSeries.Name = "srRevenue";
            totalSeries.Color = ColorTranslator.FromHtml("#19C2AF");
            totalSeries.MarkerColor = ColorTranslator.FromHtml("#0E6D67");
            totalSeries.BorderWidth = 3;
            totalSeries.MarkerStyle = MarkerStyle.Circle;
            totalSeries.MarkerSize = 7;

            var completedSeries = chartRevenueTrend.Series.IndexOf("srCompleted") >= 0
                ? chartRevenueTrend.Series["srCompleted"]
                : chartRevenueTrend.Series.Add("srCompleted");
            completedSeries.ChartType = SeriesChartType.Spline;
            completedSeries.BorderWidth = 3;
            completedSeries.Color = ColorTranslator.FromHtml("#0A4F49");
            completedSeries.MarkerStyle = MarkerStyle.Circle;
            completedSeries.MarkerSize = 6;
            completedSeries.LegendText = "Completed";
            completedSeries.ChartArea = chartRevenueTrend.ChartAreas[0].Name;
            completedSeries.Legend = chartRevenueTrend.Legends[0].Name;

            chartRevenueTrend.Legends[0].Enabled = false;
            chartRevenueTrend.Legends[0].Docking = Docking.Bottom;
            chartRevenueTrend.Legends[0].Alignment = StringAlignment.Center;

            var pieSeries = chartGender.Series["srGender"];
            pieSeries.ChartType = SeriesChartType.Pie;
            pieSeries["PieLabelStyle"] = "Disabled";
            pieSeries.Font = ThemeManager.Fonts.Regular;
            if (chartGender.Legends.Count > 0)
            {
                chartGender.Legends[0].Enabled = false;
            }

            if (chartGender.ChartAreas.Count > 0)
            {
                var pieArea = chartGender.ChartAreas[0];
                pieArea.Position.Auto = false;
                pieArea.Position = new ElementPosition(2, 2, 96, 96);
                pieArea.InnerPlotPosition.Auto = false;
                pieArea.InnerPlotPosition = new ElementPosition(8, 8, 84, 84);
            }
            LayoutDistributionChart();

            ThemeManager.StyleDataGridView(_dgvRecentUpdates);
            ThemeManager.StyleDataGridView(_dgvStaffPerformance);
            if (_lblDistributionSummary != null)
            {
                _lblDistributionSummary.ForeColor = ThemeManager.Colors.TextSecondary;
                _lblDistributionSummary.Font = ThemeManager.Fonts.Regular;
            }
        }

        private void BindDistributionChart(IList<DashboardService.GenderBreakdownItem> items)
        {
            var series = chartGender.Series["srGender"];
            series.Points.Clear();

            if (items == null || items.Count == 0)
            {
                var emptyPoint = series.Points.AddXY("No Data", 1);
                series.Points[emptyPoint].Color = ColorTranslator.FromHtml("#CFEAE4");
                series.Points[emptyPoint].LegendText = "No Data";
                return;
            }

            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var pointIndex = series.Points.AddXY(item.Label, item.Total);
                var point = series.Points[pointIndex];
                point.Color = PiePalette[i % PiePalette.Length];
                point.Label = item.Total > 0 ? item.Total.ToString() : string.Empty;
                point.LegendText = $"{item.Label} ({item.Total})";
            }
        }

        private void BindActivityTrendChart(IList<DashboardService.AppointmentTrendItem> items)
        {
            var totalSeries = chartRevenueTrend.Series["srRevenue"];
            var completedSeries = chartRevenueTrend.Series.IndexOf("srCompleted") >= 0
                ? chartRevenueTrend.Series["srCompleted"]
                : chartRevenueTrend.Series.Add("srCompleted");
            totalSeries.Points.Clear();
            completedSeries.Points.Clear();

            if (items == null || items.Count == 0)
            {
                for (var i = 0; i < 6; i++)
                {
                    var month = DateTime.Now.AddMonths(-(5 - i)).ToString("MMM", CultureInfo.InvariantCulture);
                    totalSeries.Points.AddXY(month, 0);
                    completedSeries.Points.AddXY(month, 0);
                }

                return;
            }

            foreach (var item in items)
            {
                var label = DateTime.TryParseExact(
                    item.MonthLabel,
                    "yyyy-MM",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var parsed)
                    ? parsed.ToString("MMM", CultureInfo.InvariantCulture)
                    : item.MonthLabel;

                var totalValue = Math.Max(0, item.TotalAppointments);
                var completedValue = Math.Max(0, item.CompletedAppointments);

                var totalPointIndex = totalSeries.Points.AddXY(label, totalValue);
                var completedPointIndex = completedSeries.Points.AddXY(label, completedValue);
                totalSeries.Points[totalPointIndex].ToolTip = $"{label}: total {totalValue}";
                completedSeries.Points[completedPointIndex].ToolTip = $"{label}: completed {completedValue}";
            }
        }

        private void ApplySummaryData(
            int todayAppointments,
            int pendingApprovals,
            decimal monthlyCollections,
            decimal occupancyRate,
            IList<DashboardService.GenderBreakdownItem> distribution)
        {
            if (_lblGreetingTitle != null)
            {
                _lblGreetingTitle.Text = "Executive Dashboard";
            }

            if (_lblGreetingSubtitle != null)
            {
                _lblGreetingSubtitle.Text = "Operational performance and activity overview";
            }

            if (_lblDateChip != null)
            {
                _lblDateChip.Text = $"{DateTime.Now:dddd, dd MMMM yyyy}";
            }

            var safeOccupancy = Math.Max(0m, Math.Min(100m, occupancyRate));

            _lblAppointments.Text = "Today Appointments";
            _lblAppointmentsValue.Text = todayAppointments.ToString(CultureInfo.InvariantCulture);
            _lblAppointmentsTrend.Text = "Scheduled consultations";

            lblPatients.Text = "Pending Actions";
            lblPatientsValue.Text = Math.Max(0, pendingApprovals).ToString(CultureInfo.InvariantCulture);
            _lblPatientsTrend.Text = "Workflows awaiting decision";

            lblDoctors.Text = "Monthly Collections";
            lblDoctorsValue.Text = FormatPeso(monthlyCollections);
            _lblDoctorsTrend.Text = "Current month billing";

            lblRevenue.Text = "Occupancy Rate";
            lblRevenueValue.Text = $"{safeOccupancy:0.##}%";
            _lblRevenueTrend.Text = "Room utilization";

            if (_lblDistributionSummary != null)
            {
                if (distribution == null || distribution.Count == 0)
                {
                    _lblDistributionSummary.Text = "No distribution data available.";
                }
                else
                {
                    var total = 0;
                    foreach (var item in distribution)
                    {
                        total += Math.Max(0, item.Total);
                    }

                    if (total <= 0)
                    {
                        _lblDistributionSummary.Text = "No distribution data available.";
                    }
                    else
                    {
                        var lines = new List<string>(distribution.Count);
                        foreach (var item in distribution)
                        {
                            var pct = Math.Round((item.Total * 100M) / total, 0);
                            lines.Add($"{item.Label}: {item.Total} ({pct}%)");
                        }

                        _lblDistributionSummary.Text = string.Join(Environment.NewLine, lines);
                    }
                }
            }
        }

        private async Task<DataTable> LoadRecentUpdatesSafeAsync()
        {
            try
            {
                return await _reportService.GetRecentAppointmentUpdatesAsync(8).ConfigureAwait(true);
            }
            catch
            {
                return new DataTable();
            }
        }

        private async Task<DataTable> LoadStaffPerformanceSafeAsync()
        {
            try
            {
                return await _reportService.GetStaffPerformanceSnapshotAsync(8).ConfigureAwait(true);
            }
            catch
            {
                return new DataTable();
            }
        }

        private void PopulateRecentUpdatesGrid(DataTable table)
        {
            if (_dgvRecentUpdates == null)
            {
                return;
            }

            _dgvRecentUpdates.Rows.Clear();
            if (table == null || table.Rows.Count == 0)
            {
                _dgvRecentUpdates.Rows.Add("System", "No recent updates", "Info", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                return;
            }

            foreach (DataRow row in table.Rows)
            {
                _dgvRecentUpdates.Rows.Add(
                    row["Type"]?.ToString(),
                    row["ReferenceNo"]?.ToString(),
                    row["Status"]?.ToString(),
                    row["UpdatedAt"]?.ToString());
            }
        }

        private void PopulateStaffPerformanceGrid(DataTable table)
        {
            if (_dgvStaffPerformance == null)
            {
                return;
            }

            _dgvStaffPerformance.Rows.Clear();
            if (table == null || table.Rows.Count == 0)
            {
                _dgvStaffPerformance.Rows.Add("No Staff Data", "-", 0, 0, 0, "0.00", 0);
                return;
            }

            foreach (DataRow row in table.Rows)
            {
                var revenueRaw = row["Revenue"] == DBNull.Value ? 0m : Convert.ToDecimal(row["Revenue"]);
                var consultationsRaw = 0;
                if (table.Columns.Contains("Consultations"))
                {
                    consultationsRaw = row["Consultations"] == DBNull.Value ? 0 : Convert.ToInt32(row["Consultations"]);
                }
                else if (table.Columns.Contains("Rentals"))
                {
                    consultationsRaw = row["Rentals"] == DBNull.Value ? 0 : Convert.ToInt32(row["Rentals"]);
                }

                _dgvStaffPerformance.Rows.Add(
                    row["Staff"]?.ToString(),
                    row["Role"]?.ToString(),
                    consultationsRaw,
                    row["Completed"] == DBNull.Value ? 0 : Convert.ToInt32(row["Completed"]),
                    row["Overdue"] == DBNull.Value ? 0 : Convert.ToInt32(row["Overdue"]),
                    revenueRaw.ToString("N2", CultureInfo.GetCultureInfo("en-PH")),
                    row["Pending"] == DBNull.Value ? 0 : Convert.ToInt32(row["Pending"]));
            }
        }

        private static void ConfigureChartSurface(Chart chart, bool hideAxes)
        {
            if (chart == null || chart.ChartAreas.Count == 0)
            {
                return;
            }

            chart.BackColor = ThemeManager.Colors.Surface;
            chart.BorderlineColor = ThemeManager.Colors.Border;
            chart.BorderlineDashStyle = ChartDashStyle.Solid;
            chart.BorderlineWidth = 1;
            chart.Palette = ChartColorPalette.None;
            chart.AntiAliasing = AntiAliasingStyles.All;
            chart.TextAntiAliasingQuality = TextAntiAliasingQuality.High;

            foreach (Legend legend in chart.Legends)
            {
                legend.BackColor = ThemeManager.Colors.Surface;
                legend.ForeColor = ThemeManager.Colors.TextSecondary;
                legend.Font = ThemeManager.Fonts.Regular;
            }

            var area = chart.ChartAreas[0];
            area.BackColor = ThemeManager.Colors.Surface;
            area.AxisX.LabelStyle.Font = ThemeManager.Fonts.Regular;
            area.AxisY.LabelStyle.Font = ThemeManager.Fonts.Regular;
            area.AxisX.LabelStyle.ForeColor = ThemeManager.Colors.TextSecondary;
            area.AxisY.LabelStyle.ForeColor = ThemeManager.Colors.TextSecondary;
            area.AxisY.LabelStyle.Format = "N0";
            area.AxisY.Title = hideAxes ? string.Empty : "Count";
            area.AxisY.TitleFont = ThemeManager.Fonts.Regular;
            area.AxisY.TitleForeColor = ThemeManager.Colors.TextSecondary;
            area.AxisX.LineColor = ThemeManager.Colors.Border;
            area.AxisY.LineColor = ThemeManager.Colors.Border;
            area.AxisX.MajorGrid.LineColor = ColorTranslator.FromHtml("#D7ECE8");
            area.AxisY.MajorGrid.LineColor = ColorTranslator.FromHtml("#D7ECE8");

            if (hideAxes)
            {
                area.AxisX.MajorGrid.Enabled = false;
                area.AxisX.LineWidth = 0;
                area.AxisX.LabelStyle.Enabled = false;
                area.AxisX.MajorTickMark.Enabled = false;
                area.AxisY.MajorGrid.Enabled = false;
                area.AxisY.LineWidth = 0;
                area.AxisY.LabelStyle.Enabled = false;
                area.AxisY.MajorTickMark.Enabled = false;
                area.Position.Auto = false;
                area.Position = new ElementPosition(2, 2, 96, 96);
                area.InnerPlotPosition.Auto = false;
                area.InnerPlotPosition = new ElementPosition(8, 8, 84, 84);
            }
            else
            {
                area.AxisX.Interval = 1;
                area.AxisX.MajorGrid.Enabled = false;
                area.AxisX.IsMarginVisible = false;
                area.AxisY.MajorGrid.Enabled = true;
            }
        }

        private enum StatIconType
        {
            Patients = 0,
            Doctors = 1,
            Revenue = 2,
            Appointments = 3
        }

        private static Bitmap CreateStatIcon(StatIconType type, Color backgroundColor, Color glyphColor)
        {
            var icon = new Bitmap(38, 38);
            using (var graphics = Graphics.FromImage(icon))
            using (var backgroundBrush = new SolidBrush(backgroundColor))
            using (var foregroundBrush = new SolidBrush(glyphColor))
            using (var pen = new Pen(glyphColor, 2.2F))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                graphics.Clear(Color.Transparent);
                graphics.FillEllipse(backgroundBrush, 0, 0, 37, 37);
                DrawStatGlyph(graphics, type, pen, foregroundBrush);
            }

            return icon;
        }

        private static void DrawStatGlyph(Graphics graphics, StatIconType type, Pen pen, Brush brush)
        {
            switch (type)
            {
                case StatIconType.Patients:
                    graphics.DrawEllipse(pen, 14, 9, 10, 10);
                    graphics.DrawArc(pen, 8, 18, 22, 12, 200, 140);
                    break;
                case StatIconType.Doctors:
                    graphics.DrawEllipse(pen, 13, 8, 10, 10);
                    graphics.DrawArc(pen, 8, 18, 20, 11, 200, 140);
                    graphics.DrawLine(pen, 23, 20, 30, 20);
                    graphics.DrawLine(pen, 26.5F, 16.5F, 26.5F, 23.5F);
                    break;
                case StatIconType.Revenue:
                    using (var font = new Font("Segoe UI Semibold", 16F, FontStyle.Regular, GraphicsUnit.Point))
                    {
                        var format = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        };
                        graphics.DrawString("\u20B1", font, brush, new RectangleF(0, 0, 37, 37), format);
                    }
                    break;
                case StatIconType.Appointments:
                    graphics.DrawRectangle(pen, 9, 10, 20, 16);
                    graphics.DrawLine(pen, 9, 14, 29, 14);
                    graphics.DrawLine(pen, 14, 7, 14, 12);
                    graphics.DrawLine(pen, 24, 7, 24, 12);
                    break;
            }
        }

        private static string FormatPeso(decimal value)
        {
            var culture = CultureInfo.GetCultureInfo("en-PH");
            return string.Concat("\u20B1", value.ToString("N2", culture));
        }

        private static void SetPictureIcon(PictureBox pictureBox, Bitmap image)
        {
            if (pictureBox == null)
            {
                return;
            }

            var oldImage = pictureBox.Image;
            pictureBox.Image = image;
            oldImage?.Dispose();
        }

        private void SummaryCard_Resize(object sender, EventArgs e)
        {
            AlignSummaryCardIcons();
        }

        private void AlignSummaryCardIcons()
        {
            AlignIconToTopRight(pnlPatients, picPatientsIcon);
            AlignIconToTopRight(pnlDoctors, picDoctorsIcon);
            AlignIconToTopRight(pnlRevenue, picRevenueIcon);
        }

        private static void AlignIconToTopRight(Control container, Control icon)
        {
            if (container == null || icon == null)
            {
                return;
            }

            var iconX = Math.Max(8, container.ClientSize.Width - icon.Width - StatIconRightMargin);
            icon.Location = new Point(iconX, StatIconTop);
            icon.BringToFront();
        }

        private void GenderChartHost_Resize(object sender, EventArgs e)
        {
            LayoutDistributionChart();
        }

        private void LayoutDistributionChart()
        {
            if (_lblDistributionSummary == null || chartGender == null || lblGenderChartTitle == null)
            {
                return;
            }

            const int sidePadding = 12;
            const int bottomPadding = 8;
            const int spacing = 6;

            var summaryHeight = _lblDistributionSummary.Height;
            var summaryTop = Math.Max(lblGenderChartTitle.Bottom + 90, pnlGenderChart.ClientSize.Height - summaryHeight - bottomPadding);
            _lblDistributionSummary.SetBounds(
                sidePadding,
                summaryTop,
                Math.Max(120, pnlGenderChart.ClientSize.Width - (sidePadding * 2)),
                summaryHeight);

            var availableLeft = sidePadding;
            var availableTop = lblGenderChartTitle.Bottom + spacing;
            var availableWidth = Math.Max(120, pnlGenderChart.ClientSize.Width - (sidePadding * 2));
            var availableHeight = Math.Max(120, _lblDistributionSummary.Top - availableTop - spacing);
            var size = Math.Max(120, Math.Min(availableWidth, availableHeight));
            var x = availableLeft + Math.Max(0, (availableWidth - size) / 2);
            var y = availableTop + Math.Max(0, (availableHeight - size) / 2);

            chartGender.SetBounds(x, y, size, size);
            chartGender.BringToFront();
            _lblDistributionSummary.BringToFront();
        }
    }
}
