using System;
using System.Collections.Generic;
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
        private static readonly Color[] PiePalette =
        {
            ColorTranslator.FromHtml("#7CC2EC"),
            ColorTranslator.FromHtml("#5DB3E6"),
            ColorTranslator.FromHtml("#93CEEF"),
            ColorTranslator.FromHtml("#4DA6DD"),
            ColorTranslator.FromHtml("#B9DCF3")
        };
        private Panel _pnlAppointments;
        private PictureBox _picAppointmentsIcon;
        private Label _lblAppointmentsValue;
        private Label _lblAppointments;

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
                var patientsTask = _service.GetTotalPatientsAsync();
                var doctorsTask = _service.GetTotalDoctorsAsync();
                var appointmentsTask = _service.GetTotalAppointmentsAsync();
                var revenueTask = _service.GetTotalRevenueAsync();
                var genderDistributionTask = _service.GetPatientGenderDistributionAsync();
                var revenueTrendTask = _service.GetMonthlyRevenueTrendAsync(6);

                await Task.WhenAll(
                    patientsTask,
                    doctorsTask,
                    appointmentsTask,
                    revenueTask,
                    genderDistributionTask,
                    revenueTrendTask).ConfigureAwait(true);

                lblPatientsValue.Text = patientsTask.Result.ToString();
                lblDoctorsValue.Text = doctorsTask.Result.ToString();
                _lblAppointmentsValue.Text = appointmentsTask.Result.ToString();
                lblRevenueValue.Text = FormatPeso(revenueTask.Result);
                BindGenderChart(genderDistributionTask.Result);
                BindRevenueChart(revenueTrendTask.Result);
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

        private void ApplyTheme()
        {
            ThemeManager.ApplyControlTheme(this);
            ThemeManager.StyleCardPanel(pnlPatients);
            ThemeManager.StyleCardPanel(pnlDoctors);
            ThemeManager.StyleCardPanel(_pnlAppointments);
            ThemeManager.StyleCardPanel(pnlRevenue);
            ThemeManager.StyleCardPanel(pnlGenderChart);
            ThemeManager.StyleCardPanel(pnlRevenueChart);

            lblPatientsValue.Font = ThemeManager.Fonts.Kpi;
            lblDoctorsValue.Font = ThemeManager.Fonts.Kpi;
            _lblAppointmentsValue.Font = ThemeManager.Fonts.Kpi;
            lblRevenueValue.Font = ThemeManager.Fonts.Kpi;
            lblGenderChartTitle.Font = ThemeManager.Fonts.Medium;
            lblRevenueChartTitle.Font = ThemeManager.Fonts.Medium;
            lblRevenueChartTitle.Text = "Revenue Trend";
            lblGenderChartTitle.Text = "Patient Distribution";

            SetPictureIcon(picPatientsIcon, CreateStatIcon(StatIconType.Patients, ColorTranslator.FromHtml("#7CC2EC")));
            SetPictureIcon(picDoctorsIcon, CreateStatIcon(StatIconType.Doctors, ColorTranslator.FromHtml("#5DB3E6")));
            SetPictureIcon(_picAppointmentsIcon, CreateStatIcon(StatIconType.Appointments, ColorTranslator.FromHtml("#93CEEF")));
            SetPictureIcon(picRevenueIcon, CreateStatIcon(StatIconType.Revenue, ColorTranslator.FromHtml("#4DA6DD")));

            ConfigureChartSurface(chartGender, hideAxes: true);
            ConfigureChartSurface(chartRevenueTrend, hideAxes: false);
            var revenueSeries = chartRevenueTrend.Series["srRevenue"];
            revenueSeries.Color = ThemeManager.Colors.Primary;
            revenueSeries.MarkerColor = ThemeManager.Colors.PrimaryPressed;
            revenueSeries.BorderWidth = 3;
            revenueSeries.MarkerStyle = MarkerStyle.Circle;
            revenueSeries.MarkerSize = 7;

            var pieSeries = chartGender.Series["srGender"];
            pieSeries["PieLabelStyle"] = "Outside";
            pieSeries["PieLineColor"] = "Silver";
            pieSeries.Font = ThemeManager.Fonts.Regular;
        }

        private void BindGenderChart(IList<DashboardService.GenderBreakdownItem> items)
        {
            var series = chartGender.Series["srGender"];
            series.Points.Clear();

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

        private void BindRevenueChart(IList<DashboardService.RevenueTrendItem> items)
        {
            var series = chartRevenueTrend.Series["srRevenue"];
            series.Points.Clear();

            foreach (var item in items)
            {
                var pointIndex = series.Points.AddXY(item.MonthLabel, item.TotalAmount);
                series.Points[pointIndex].ToolTip = $"{item.MonthLabel}: {FormatPeso(item.TotalAmount)}";
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
            area.AxisY.Title = "Amount (\u20B1)";
            area.AxisY.TitleFont = ThemeManager.Fonts.Regular;
            area.AxisY.TitleForeColor = ThemeManager.Colors.TextSecondary;
            area.AxisX.LineColor = ThemeManager.Colors.Border;
            area.AxisY.LineColor = ThemeManager.Colors.Border;
            area.AxisX.MajorGrid.LineColor = ColorTranslator.FromHtml("#E2F0FA");
            area.AxisY.MajorGrid.LineColor = ColorTranslator.FromHtml("#E2F0FA");

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
            }
            else
            {
                area.AxisX.Interval = 1;
                area.AxisX.MajorGrid.Enabled = false;
                area.AxisX.IsMarginVisible = false;
                area.AxisY.MajorGrid.Enabled = true;
            }
        }

        private void EnsureDashboardLayout()
        {
            if (_pnlAppointments == null)
            {
                _pnlAppointments = new Panel
                {
                    Name = "pnlAppointments",
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    Dock = DockStyle.Fill,
                    Margin = new Padding(4)
                };

                _picAppointmentsIcon = new PictureBox
                {
                    Name = "picAppointmentsIcon",
                    Location = new Point(255, 14),
                    Size = new Size(38, 38),
                    SizeMode = PictureBoxSizeMode.Zoom
                };

                _lblAppointmentsValue = new Label
                {
                    Name = "lblAppointmentsValue",
                    AutoSize = true,
                    Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                    Location = new Point(14, 70),
                    Text = "0"
                };

                _lblAppointments = new Label
                {
                    Name = "lblAppointments",
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    Location = new Point(14, 18),
                    Text = "Total Appointments"
                };

                _pnlAppointments.Controls.Add(_picAppointmentsIcon);
                _pnlAppointments.Controls.Add(_lblAppointmentsValue);
                _pnlAppointments.Controls.Add(_lblAppointments);
            }

            tlpCards.SuspendLayout();
            tlpCards.ColumnStyles.Clear();
            tlpCards.ColumnCount = 4;
            tlpCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

            if (!tlpCards.Controls.Contains(_pnlAppointments))
            {
                tlpCards.Controls.Add(_pnlAppointments, 2, 0);
            }

            tlpCards.SetCellPosition(pnlPatients, new TableLayoutPanelCellPosition(0, 0));
            tlpCards.SetCellPosition(pnlDoctors, new TableLayoutPanelCellPosition(1, 0));
            tlpCards.SetCellPosition(_pnlAppointments, new TableLayoutPanelCellPosition(2, 0));
            tlpCards.SetCellPosition(pnlRevenue, new TableLayoutPanelCellPosition(3, 0));
            tlpCards.ResumeLayout();

            tlpCharts.Controls.Clear();
            tlpCharts.Controls.Add(pnlRevenueChart, 0, 0);
            tlpCharts.Controls.Add(pnlGenderChart, 1, 0);
        }

        private enum StatIconType
        {
            Patients = 0,
            Doctors = 1,
            Revenue = 2,
            Appointments = 3
        }

        private static Bitmap CreateStatIcon(StatIconType type, Color backgroundColor)
        {
            var icon = new Bitmap(38, 38);
            using (var graphics = Graphics.FromImage(icon))
            using (var backgroundBrush = new SolidBrush(backgroundColor))
            using (var foregroundBrush = new SolidBrush(Color.White))
            using (var pen = new Pen(Color.White, 2.2F))
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
    }
}
