namespace HospitalManagementSystem.UserControls
{
    partial class ucDashboard
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel tlpCards;
        private System.Windows.Forms.Panel pnlPatients;
        private System.Windows.Forms.Panel pnlDoctors;
        private System.Windows.Forms.Panel pnlRevenue;
        private System.Windows.Forms.Label lblPatients;
        private System.Windows.Forms.Label lblDoctors;
        private System.Windows.Forms.Label lblRevenue;
        private System.Windows.Forms.Label lblPatientsValue;
        private System.Windows.Forms.Label lblDoctorsValue;
        private System.Windows.Forms.Label lblRevenueValue;

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
            this.tlpCards = new System.Windows.Forms.TableLayoutPanel();
            this.pnlPatients = new System.Windows.Forms.Panel();
            this.pnlDoctors = new System.Windows.Forms.Panel();
            this.pnlRevenue = new System.Windows.Forms.Panel();
            this.lblPatients = new System.Windows.Forms.Label();
            this.lblDoctors = new System.Windows.Forms.Label();
            this.lblRevenue = new System.Windows.Forms.Label();
            this.lblPatientsValue = new System.Windows.Forms.Label();
            this.lblDoctorsValue = new System.Windows.Forms.Label();
            this.lblRevenueValue = new System.Windows.Forms.Label();
            this.tlpCards.SuspendLayout();
            this.pnlPatients.SuspendLayout();
            this.pnlDoctors.SuspendLayout();
            this.pnlRevenue.SuspendLayout();
            this.SuspendLayout();
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
            this.tlpCards.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpCards.Location = new System.Drawing.Point(16, 16);
            this.tlpCards.Name = "tlpCards";
            this.tlpCards.RowCount = 1;
            this.tlpCards.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tlpCards.Size = new System.Drawing.Size(948, 140);
            this.tlpCards.TabIndex = 0;
            // 
            // pnlPatients
            // 
            this.pnlPatients.BackColor = System.Drawing.Color.White;
            this.pnlPatients.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlPatients.Controls.Add(this.lblPatientsValue);
            this.pnlPatients.Controls.Add(this.lblPatients);
            this.pnlPatients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlPatients.Location = new System.Drawing.Point(3, 3);
            this.pnlPatients.Name = "pnlPatients";
            this.pnlPatients.Padding = new System.Windows.Forms.Padding(12);
            this.pnlPatients.Size = new System.Drawing.Size(310, 134);
            this.pnlPatients.TabIndex = 0;
            // 
            // pnlDoctors
            // 
            this.pnlDoctors.BackColor = System.Drawing.Color.White;
            this.pnlDoctors.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlDoctors.Controls.Add(this.lblDoctorsValue);
            this.pnlDoctors.Controls.Add(this.lblDoctors);
            this.pnlDoctors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDoctors.Location = new System.Drawing.Point(319, 3);
            this.pnlDoctors.Name = "pnlDoctors";
            this.pnlDoctors.Padding = new System.Windows.Forms.Padding(12);
            this.pnlDoctors.Size = new System.Drawing.Size(310, 134);
            this.pnlDoctors.TabIndex = 1;
            // 
            // pnlRevenue
            // 
            this.pnlRevenue.BackColor = System.Drawing.Color.White;
            this.pnlRevenue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlRevenue.Controls.Add(this.lblRevenueValue);
            this.pnlRevenue.Controls.Add(this.lblRevenue);
            this.pnlRevenue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRevenue.Location = new System.Drawing.Point(635, 3);
            this.pnlRevenue.Name = "pnlRevenue";
            this.pnlRevenue.Padding = new System.Windows.Forms.Padding(12);
            this.pnlRevenue.Size = new System.Drawing.Size(310, 134);
            this.pnlRevenue.TabIndex = 2;
            // 
            // lblPatients
            // 
            this.lblPatients.AutoSize = true;
            this.lblPatients.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblPatients.Location = new System.Drawing.Point(12, 12);
            this.lblPatients.Name = "lblPatients";
            this.lblPatients.Size = new System.Drawing.Size(109, 19);
            this.lblPatients.TabIndex = 0;
            this.lblPatients.Text = "Total Patients";
            // 
            // lblDoctors
            // 
            this.lblDoctors.AutoSize = true;
            this.lblDoctors.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblDoctors.Location = new System.Drawing.Point(12, 12);
            this.lblDoctors.Name = "lblDoctors";
            this.lblDoctors.Size = new System.Drawing.Size(104, 19);
            this.lblDoctors.TabIndex = 0;
            this.lblDoctors.Text = "Total Doctors";
            // 
            // lblRevenue
            // 
            this.lblRevenue.AutoSize = true;
            this.lblRevenue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblRevenue.Location = new System.Drawing.Point(12, 12);
            this.lblRevenue.Name = "lblRevenue";
            this.lblRevenue.Size = new System.Drawing.Size(107, 19);
            this.lblRevenue.TabIndex = 0;
            this.lblRevenue.Text = "Total Revenue";
            // 
            // lblPatientsValue
            // 
            this.lblPatientsValue.AutoSize = true;
            this.lblPatientsValue.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblPatientsValue.Location = new System.Drawing.Point(12, 50);
            this.lblPatientsValue.Name = "lblPatientsValue";
            this.lblPatientsValue.Size = new System.Drawing.Size(48, 37);
            this.lblPatientsValue.TabIndex = 1;
            this.lblPatientsValue.Text = "0";
            // 
            // lblDoctorsValue
            // 
            this.lblDoctorsValue.AutoSize = true;
            this.lblDoctorsValue.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblDoctorsValue.Location = new System.Drawing.Point(12, 50);
            this.lblDoctorsValue.Name = "lblDoctorsValue";
            this.lblDoctorsValue.Size = new System.Drawing.Size(48, 37);
            this.lblDoctorsValue.TabIndex = 1;
            this.lblDoctorsValue.Text = "0";
            // 
            // lblRevenueValue
            // 
            this.lblRevenueValue.AutoSize = true;
            this.lblRevenueValue.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblRevenueValue.Location = new System.Drawing.Point(12, 50);
            this.lblRevenueValue.Name = "lblRevenueValue";
            this.lblRevenueValue.Size = new System.Drawing.Size(48, 37);
            this.lblRevenueValue.TabIndex = 1;
            this.lblRevenueValue.Text = "$0";
            // 
            // ucDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.tlpCards);
            this.Name = "ucDashboard";
            this.Padding = new System.Windows.Forms.Padding(16);
            this.Size = new System.Drawing.Size(980, 590);
            this.tlpCards.ResumeLayout(false);
            this.pnlPatients.ResumeLayout(false);
            this.pnlPatients.PerformLayout();
            this.pnlDoctors.ResumeLayout(false);
            this.pnlDoctors.PerformLayout();
            this.pnlRevenue.ResumeLayout(false);
            this.pnlRevenue.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
