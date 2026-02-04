namespace HospitalManagementSystem.UserControls
{
    partial class ucReports
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.ComboBox cboReport;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnExportExcel;
        private System.Windows.Forms.Button btnExportCsv;
        private System.Windows.Forms.Button btnExportPdf;
        private System.Windows.Forms.DataGridView dgvReport;

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
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.cboReport = new System.Windows.Forms.ComboBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnExportExcel = new System.Windows.Forms.Button();
            this.btnExportCsv = new System.Windows.Forms.Button();
            this.btnExportPdf = new System.Windows.Forms.Button();
            this.dgvReport = new System.Windows.Forms.DataGridView();
            this.pnlButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReport)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlButtons
            // 
            this.pnlButtons.BackColor = System.Drawing.Color.White;
            this.pnlButtons.Controls.Add(this.btnExportPdf);
            this.pnlButtons.Controls.Add(this.btnExportCsv);
            this.pnlButtons.Controls.Add(this.btnExportExcel);
            this.pnlButtons.Controls.Add(this.btnLoad);
            this.pnlButtons.Controls.Add(this.cboReport);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlButtons.Location = new System.Drawing.Point(0, 0);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.pnlButtons.Size = new System.Drawing.Size(980, 48);
            this.pnlButtons.TabIndex = 0;
            // 
            // cboReport
            // 
            this.cboReport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboReport.FormattingEnabled = true;
            this.cboReport.Items.AddRange(new object[] {
            "Patients",
            "Appointments",
            "Billing",
            "Pharmacy",
            "DoctorPerformance"});
            this.cboReport.Location = new System.Drawing.Point(12, 12);
            this.cboReport.Name = "cboReport";
            this.cboReport.Size = new System.Drawing.Size(180, 23);
            this.cboReport.TabIndex = 0;
            // 
            // btnLoad
            // 
            this.btnLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoad.Location = new System.Drawing.Point(198, 10);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(80, 28);
            this.btnLoad.TabIndex = 1;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            // 
            // btnExportExcel
            // 
            this.btnExportExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportExcel.Location = new System.Drawing.Point(300, 10);
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.Size = new System.Drawing.Size(110, 28);
            this.btnExportExcel.TabIndex = 2;
            this.btnExportExcel.Text = "Export Excel";
            this.btnExportExcel.UseVisualStyleBackColor = true;
            // 
            // btnExportCsv
            // 
            this.btnExportCsv.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportCsv.Location = new System.Drawing.Point(416, 10);
            this.btnExportCsv.Name = "btnExportCsv";
            this.btnExportCsv.Size = new System.Drawing.Size(110, 28);
            this.btnExportCsv.TabIndex = 3;
            this.btnExportCsv.Text = "Export CSV";
            this.btnExportCsv.UseVisualStyleBackColor = true;
            // 
            // btnExportPdf
            // 
            this.btnExportPdf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportPdf.Location = new System.Drawing.Point(532, 10);
            this.btnExportPdf.Name = "btnExportPdf";
            this.btnExportPdf.Size = new System.Drawing.Size(110, 28);
            this.btnExportPdf.TabIndex = 4;
            this.btnExportPdf.Text = "Export PDF";
            this.btnExportPdf.UseVisualStyleBackColor = true;
            // 
            // dgvReport
            // 
            this.dgvReport.AllowUserToAddRows = false;
            this.dgvReport.AllowUserToDeleteRows = false;
            this.dgvReport.BackgroundColor = System.Drawing.Color.White;
            this.dgvReport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvReport.Location = new System.Drawing.Point(0, 48);
            this.dgvReport.Name = "dgvReport";
            this.dgvReport.ReadOnly = true;
            this.dgvReport.RowTemplate.Height = 25;
            this.dgvReport.Size = new System.Drawing.Size(980, 542);
            this.dgvReport.TabIndex = 1;
            // 
            // ucReports
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgvReport);
            this.Controls.Add(this.pnlButtons);
            this.Name = "ucReports";
            this.Size = new System.Drawing.Size(980, 590);
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReport)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
