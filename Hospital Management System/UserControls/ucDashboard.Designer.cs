namespace HospitalManagementSystem.UserControls
{
    partial class ucDashboard
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.TableLayoutPanel tlpCards;
        private System.Windows.Forms.Panel pnlPatients;
        private System.Windows.Forms.PictureBox picPatientsIcon;
        private System.Windows.Forms.Label lblPatientsValue;
        private System.Windows.Forms.Label lblPatients;
        private System.Windows.Forms.Panel pnlDoctors;
        private System.Windows.Forms.PictureBox picDoctorsIcon;
        private System.Windows.Forms.Label lblDoctorsValue;
        private System.Windows.Forms.Label lblDoctors;
        private System.Windows.Forms.Panel pnlRevenue;
        private System.Windows.Forms.PictureBox picRevenueIcon;
        private System.Windows.Forms.Label lblRevenueValue;
        private System.Windows.Forms.Label lblRevenue;
        private System.Windows.Forms.TableLayoutPanel tlpCharts;
        private System.Windows.Forms.Panel pnlGenderChart;
        private System.Windows.Forms.Label lblGenderChartTitle;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartGender;
        private System.Windows.Forms.Panel pnlRevenueChart;
        private System.Windows.Forms.Label lblRevenueChartTitle;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartRevenueTrend;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.tlpCards = new System.Windows.Forms.TableLayoutPanel();
            this.pnlPatients = new System.Windows.Forms.Panel();
            this.picPatientsIcon = new System.Windows.Forms.PictureBox();
            this.lblPatientsValue = new System.Windows.Forms.Label();
            this.lblPatients = new System.Windows.Forms.Label();
            this.pnlDoctors = new System.Windows.Forms.Panel();
            this.picDoctorsIcon = new System.Windows.Forms.PictureBox();
            this.lblDoctorsValue = new System.Windows.Forms.Label();
            this.lblDoctors = new System.Windows.Forms.Label();
            this.pnlRevenue = new System.Windows.Forms.Panel();
            this.picRevenueIcon = new System.Windows.Forms.PictureBox();
            this.lblRevenueValue = new System.Windows.Forms.Label();
            this.lblRevenue = new System.Windows.Forms.Label();
            this.tlpCharts = new System.Windows.Forms.TableLayoutPanel();
            this.pnlGenderChart = new System.Windows.Forms.Panel();
            this.chartGender = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.lblGenderChartTitle = new System.Windows.Forms.Label();
            this.pnlRevenueChart = new System.Windows.Forms.Panel();
            this.chartRevenueTrend = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.lblRevenueChartTitle = new System.Windows.Forms.Label();
            this.tlpMain.SuspendLayout();
            this.tlpCards.SuspendLayout();
            this.pnlPatients.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPatientsIcon)).BeginInit();
            this.pnlDoctors.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picDoctorsIcon)).BeginInit();
            this.pnlRevenue.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picRevenueIcon)).BeginInit();
            this.tlpCharts.SuspendLayout();
            this.pnlGenderChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartGender)).BeginInit();
            this.pnlRevenueChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartRevenueTrend)).BeginInit();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Controls.Add(this.tlpCards, 0, 0);
            this.tlpMain.Controls.Add(this.tlpCharts, 0, 1);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(16, 16);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 2;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Size = new System.Drawing.Size(948, 558);
            this.tlpMain.TabIndex = 0;
            // 
            // tlpCards
            // 
            this.tlpCards.ColumnCount = 3;
            this.tlpCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlpCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlpCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlpCards.Controls.Add(this.pnlPatients, 0, 0);
            this.tlpCards.Controls.Add(this.pnlDoctors, 1, 0);
            this.tlpCards.Controls.Add(this.pnlRevenue, 2, 0);
            this.tlpCards.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpCards.Location = new System.Drawing.Point(0, 0);
            this.tlpCards.Margin = new System.Windows.Forms.Padding(0);
            this.tlpCards.Name = "tlpCards";
            this.tlpCards.RowCount = 1;
            this.tlpCards.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCards.Size = new System.Drawing.Size(948, 150);
            this.tlpCards.TabIndex = 0;
            // 
            // pnlPatients
            // 
            this.pnlPatients.BackColor = System.Drawing.Color.White;
            this.pnlPatients.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlPatients.Controls.Add(this.picPatientsIcon);
            this.pnlPatients.Controls.Add(this.lblPatientsValue);
            this.pnlPatients.Controls.Add(this.lblPatients);
            this.pnlPatients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlPatients.Location = new System.Drawing.Point(4, 4);
            this.pnlPatients.Margin = new System.Windows.Forms.Padding(4);
            this.pnlPatients.Name = "pnlPatients";
            this.pnlPatients.Size = new System.Drawing.Size(308, 142);
            this.pnlPatients.TabIndex = 0;
            // 
            // picPatientsIcon
            // 
            this.picPatientsIcon.Location = new System.Drawing.Point(255, 14);
            this.picPatientsIcon.Name = "picPatientsIcon";
            this.picPatientsIcon.Size = new System.Drawing.Size(38, 38);
            this.picPatientsIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPatientsIcon.TabIndex = 2;
            this.picPatientsIcon.TabStop = false;
            // 
            // lblPatientsValue
            // 
            this.lblPatientsValue.AutoSize = true;
            this.lblPatientsValue.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblPatientsValue.Location = new System.Drawing.Point(14, 70);
            this.lblPatientsValue.Name = "lblPatientsValue";
            this.lblPatientsValue.Size = new System.Drawing.Size(48, 37);
            this.lblPatientsValue.TabIndex = 1;
            this.lblPatientsValue.Text = "0";
            // 
            // lblPatients
            // 
            this.lblPatients.AutoSize = true;
            this.lblPatients.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblPatients.Location = new System.Drawing.Point(14, 18);
            this.lblPatients.Name = "lblPatients";
            this.lblPatients.Size = new System.Drawing.Size(109, 19);
            this.lblPatients.TabIndex = 0;
            this.lblPatients.Text = "Total Patients";
            // 
            // pnlDoctors
            // 
            this.pnlDoctors.BackColor = System.Drawing.Color.White;
            this.pnlDoctors.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlDoctors.Controls.Add(this.picDoctorsIcon);
            this.pnlDoctors.Controls.Add(this.lblDoctorsValue);
            this.pnlDoctors.Controls.Add(this.lblDoctors);
            this.pnlDoctors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDoctors.Location = new System.Drawing.Point(320, 4);
            this.pnlDoctors.Margin = new System.Windows.Forms.Padding(4);
            this.pnlDoctors.Name = "pnlDoctors";
            this.pnlDoctors.Size = new System.Drawing.Size(308, 142);
            this.pnlDoctors.TabIndex = 1;
            // 
            // picDoctorsIcon
            // 
            this.picDoctorsIcon.Location = new System.Drawing.Point(255, 14);
            this.picDoctorsIcon.Name = "picDoctorsIcon";
            this.picDoctorsIcon.Size = new System.Drawing.Size(38, 38);
            this.picDoctorsIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picDoctorsIcon.TabIndex = 2;
            this.picDoctorsIcon.TabStop = false;
            // 
            // lblDoctorsValue
            // 
            this.lblDoctorsValue.AutoSize = true;
            this.lblDoctorsValue.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblDoctorsValue.Location = new System.Drawing.Point(14, 70);
            this.lblDoctorsValue.Name = "lblDoctorsValue";
            this.lblDoctorsValue.Size = new System.Drawing.Size(48, 37);
            this.lblDoctorsValue.TabIndex = 1;
            this.lblDoctorsValue.Text = "0";
            // 
            // lblDoctors
            // 
            this.lblDoctors.AutoSize = true;
            this.lblDoctors.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblDoctors.Location = new System.Drawing.Point(14, 18);
            this.lblDoctors.Name = "lblDoctors";
            this.lblDoctors.Size = new System.Drawing.Size(104, 19);
            this.lblDoctors.TabIndex = 0;
            this.lblDoctors.Text = "Total Doctors";
            // 
            // pnlRevenue
            // 
            this.pnlRevenue.BackColor = System.Drawing.Color.White;
            this.pnlRevenue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlRevenue.Controls.Add(this.picRevenueIcon);
            this.pnlRevenue.Controls.Add(this.lblRevenueValue);
            this.pnlRevenue.Controls.Add(this.lblRevenue);
            this.pnlRevenue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRevenue.Location = new System.Drawing.Point(636, 4);
            this.pnlRevenue.Margin = new System.Windows.Forms.Padding(4);
            this.pnlRevenue.Name = "pnlRevenue";
            this.pnlRevenue.Size = new System.Drawing.Size(308, 142);
            this.pnlRevenue.TabIndex = 2;
            // 
            // picRevenueIcon
            // 
            this.picRevenueIcon.Location = new System.Drawing.Point(255, 14);
            this.picRevenueIcon.Name = "picRevenueIcon";
            this.picRevenueIcon.Size = new System.Drawing.Size(38, 38);
            this.picRevenueIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picRevenueIcon.TabIndex = 2;
            this.picRevenueIcon.TabStop = false;
            // 
            // lblRevenueValue
            // 
            this.lblRevenueValue.AutoSize = true;
            this.lblRevenueValue.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblRevenueValue.Location = new System.Drawing.Point(14, 70);
            this.lblRevenueValue.Name = "lblRevenueValue";
            this.lblRevenueValue.Size = new System.Drawing.Size(54, 37);
            this.lblRevenueValue.TabIndex = 1;
            this.lblRevenueValue.Text = "₱0";
            // 
            // lblRevenue
            // 
            this.lblRevenue.AutoSize = true;
            this.lblRevenue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblRevenue.Location = new System.Drawing.Point(14, 18);
            this.lblRevenue.Name = "lblRevenue";
            this.lblRevenue.Size = new System.Drawing.Size(107, 19);
            this.lblRevenue.TabIndex = 0;
            this.lblRevenue.Text = "Total Revenue";
            // 
            // tlpCharts
            // 
            this.tlpCharts.ColumnCount = 2;
            this.tlpCharts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpCharts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpCharts.Controls.Add(this.pnlGenderChart, 0, 0);
            this.tlpCharts.Controls.Add(this.pnlRevenueChart, 1, 0);
            this.tlpCharts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpCharts.Location = new System.Drawing.Point(0, 162);
            this.tlpCharts.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.tlpCharts.Name = "tlpCharts";
            this.tlpCharts.RowCount = 1;
            this.tlpCharts.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCharts.Size = new System.Drawing.Size(948, 396);
            this.tlpCharts.TabIndex = 1;
            // 
            // pnlGenderChart
            // 
            this.pnlGenderChart.BackColor = System.Drawing.Color.White;
            this.pnlGenderChart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlGenderChart.Controls.Add(this.chartGender);
            this.pnlGenderChart.Controls.Add(this.lblGenderChartTitle);
            this.pnlGenderChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGenderChart.Location = new System.Drawing.Point(4, 4);
            this.pnlGenderChart.Margin = new System.Windows.Forms.Padding(4);
            this.pnlGenderChart.Name = "pnlGenderChart";
            this.pnlGenderChart.Size = new System.Drawing.Size(466, 388);
            this.pnlGenderChart.TabIndex = 0;
            // 
            // chartGender
            // 
            chartArea1.Name = "ChartArea1";
            this.chartGender.ChartAreas.Add(chartArea1);
            this.chartGender.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Alignment = System.Drawing.StringAlignment.Center;
            legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            legend1.Name = "Legend1";
            this.chartGender.Legends.Add(legend1);
            this.chartGender.Location = new System.Drawing.Point(0, 34);
            this.chartGender.Name = "chartGender";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            series1.Legend = "Legend1";
            series1.Name = "srGender";
            this.chartGender.Series.Add(series1);
            this.chartGender.Size = new System.Drawing.Size(464, 352);
            this.chartGender.TabIndex = 1;
            this.chartGender.Text = "chartGender";
            // 
            // lblGenderChartTitle
            // 
            this.lblGenderChartTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblGenderChartTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.lblGenderChartTitle.Location = new System.Drawing.Point(0, 0);
            this.lblGenderChartTitle.Name = "lblGenderChartTitle";
            this.lblGenderChartTitle.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.lblGenderChartTitle.Size = new System.Drawing.Size(464, 34);
            this.lblGenderChartTitle.TabIndex = 0;
            this.lblGenderChartTitle.Text = "Patients by Gender";
            this.lblGenderChartTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlRevenueChart
            // 
            this.pnlRevenueChart.BackColor = System.Drawing.Color.White;
            this.pnlRevenueChart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlRevenueChart.Controls.Add(this.chartRevenueTrend);
            this.pnlRevenueChart.Controls.Add(this.lblRevenueChartTitle);
            this.pnlRevenueChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRevenueChart.Location = new System.Drawing.Point(478, 4);
            this.pnlRevenueChart.Margin = new System.Windows.Forms.Padding(4);
            this.pnlRevenueChart.Name = "pnlRevenueChart";
            this.pnlRevenueChart.Size = new System.Drawing.Size(466, 388);
            this.pnlRevenueChart.TabIndex = 1;
            // 
            // chartRevenueTrend
            // 
            chartArea2.Name = "ChartArea1";
            this.chartRevenueTrend.ChartAreas.Add(chartArea2);
            this.chartRevenueTrend.Dock = System.Windows.Forms.DockStyle.Fill;
            legend2.Enabled = false;
            legend2.Name = "Legend1";
            this.chartRevenueTrend.Legends.Add(legend2);
            this.chartRevenueTrend.Location = new System.Drawing.Point(0, 34);
            this.chartRevenueTrend.Name = "chartRevenueTrend";
            series2.BorderWidth = 3;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series2.Color = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(70)))), ((int)(((byte)(229)))));
            series2.Legend = "Legend1";
            series2.MarkerSize = 7;
            series2.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            series2.Name = "srRevenue";
            this.chartRevenueTrend.Series.Add(series2);
            this.chartRevenueTrend.Size = new System.Drawing.Size(464, 352);
            this.chartRevenueTrend.TabIndex = 1;
            this.chartRevenueTrend.Text = "chartRevenueTrend";
            // 
            // lblRevenueChartTitle
            // 
            this.lblRevenueChartTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblRevenueChartTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.lblRevenueChartTitle.Location = new System.Drawing.Point(0, 0);
            this.lblRevenueChartTitle.Name = "lblRevenueChartTitle";
            this.lblRevenueChartTitle.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.lblRevenueChartTitle.Size = new System.Drawing.Size(464, 34);
            this.lblRevenueChartTitle.TabIndex = 0;
            this.lblRevenueChartTitle.Text = "Monthly Paid Revenue";
            this.lblRevenueChartTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ucDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.tlpMain);
            this.Name = "ucDashboard";
            this.Padding = new System.Windows.Forms.Padding(16);
            this.Size = new System.Drawing.Size(980, 590);
            this.tlpMain.ResumeLayout(false);
            this.tlpCards.ResumeLayout(false);
            this.pnlPatients.ResumeLayout(false);
            this.pnlPatients.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPatientsIcon)).EndInit();
            this.pnlDoctors.ResumeLayout(false);
            this.pnlDoctors.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picDoctorsIcon)).EndInit();
            this.pnlRevenue.ResumeLayout(false);
            this.pnlRevenue.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picRevenueIcon)).EndInit();
            this.tlpCharts.ResumeLayout(false);
            this.pnlGenderChart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartGender)).EndInit();
            this.pnlRevenueChart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartRevenueTrend)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
