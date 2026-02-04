namespace HospitalManagementSystem.Forms.Shared
{
    partial class frmAppointmentEdit
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblCode;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Label lblPatientId;
        private System.Windows.Forms.TextBox txtPatientId;
        private System.Windows.Forms.Label lblDoctorId;
        private System.Windows.Forms.TextBox txtDoctorId;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.DateTimePicker dtpTime;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.ComboBox cboType;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox cboStatus;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;

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
            this.lblCode = new System.Windows.Forms.Label();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.lblPatientId = new System.Windows.Forms.Label();
            this.txtPatientId = new System.Windows.Forms.TextBox();
            this.lblDoctorId = new System.Windows.Forms.Label();
            this.txtDoctorId = new System.Windows.Forms.TextBox();
            this.lblDate = new System.Windows.Forms.Label();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.lblTime = new System.Windows.Forms.Label();
            this.dtpTime = new System.Windows.Forms.DateTimePicker();
            this.lblType = new System.Windows.Forms.Label();
            this.cboType = new System.Windows.Forms.ComboBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cboStatus = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblCode
            // 
            this.lblCode.AutoSize = true;
            this.lblCode.Location = new System.Drawing.Point(16, 18);
            this.lblCode.Name = "lblCode";
            this.lblCode.Size = new System.Drawing.Size(36, 15);
            this.lblCode.TabIndex = 0;
            this.lblCode.Text = "Code";
            // 
            // txtCode
            // 
            this.txtCode.Location = new System.Drawing.Point(120, 15);
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(220, 23);
            this.txtCode.TabIndex = 1;
            // 
            // lblPatientId
            // 
            this.lblPatientId.AutoSize = true;
            this.lblPatientId.Location = new System.Drawing.Point(16, 52);
            this.lblPatientId.Name = "lblPatientId";
            this.lblPatientId.Size = new System.Drawing.Size(60, 15);
            this.lblPatientId.TabIndex = 2;
            this.lblPatientId.Text = "Patient ID";
            // 
            // txtPatientId
            // 
            this.txtPatientId.Location = new System.Drawing.Point(120, 49);
            this.txtPatientId.Name = "txtPatientId";
            this.txtPatientId.Size = new System.Drawing.Size(220, 23);
            this.txtPatientId.TabIndex = 3;
            // 
            // lblDoctorId
            // 
            this.lblDoctorId.AutoSize = true;
            this.lblDoctorId.Location = new System.Drawing.Point(16, 86);
            this.lblDoctorId.Name = "lblDoctorId";
            this.lblDoctorId.Size = new System.Drawing.Size(58, 15);
            this.lblDoctorId.TabIndex = 4;
            this.lblDoctorId.Text = "Doctor ID";
            // 
            // txtDoctorId
            // 
            this.txtDoctorId.Location = new System.Drawing.Point(120, 83);
            this.txtDoctorId.Name = "txtDoctorId";
            this.txtDoctorId.Size = new System.Drawing.Size(220, 23);
            this.txtDoctorId.TabIndex = 5;
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Location = new System.Drawing.Point(16, 120);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(31, 15);
            this.lblDate.TabIndex = 6;
            this.lblDate.Text = "Date";
            // 
            // dtpDate
            // 
            this.dtpDate.Location = new System.Drawing.Point(120, 117);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(220, 23);
            this.dtpDate.TabIndex = 7;
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(16, 154);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(33, 15);
            this.lblTime.TabIndex = 8;
            this.lblTime.Text = "Time";
            // 
            // dtpTime
            // 
            this.dtpTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpTime.Location = new System.Drawing.Point(120, 151);
            this.dtpTime.Name = "dtpTime";
            this.dtpTime.ShowUpDown = true;
            this.dtpTime.Size = new System.Drawing.Size(220, 23);
            this.dtpTime.TabIndex = 9;
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(16, 188);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(31, 15);
            this.lblType.TabIndex = 10;
            this.lblType.Text = "Type";
            // 
            // cboType
            // 
            this.cboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboType.FormattingEnabled = true;
            this.cboType.Items.AddRange(new object[] {
            "Consultation",
            "Follow-up",
            "Emergency",
            "Check-up"});
            this.cboType.Location = new System.Drawing.Point(120, 185);
            this.cboType.Name = "cboType";
            this.cboType.Size = new System.Drawing.Size(220, 23);
            this.cboType.TabIndex = 11;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(16, 222);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(39, 15);
            this.lblStatus.TabIndex = 12;
            this.lblStatus.Text = "Status";
            // 
            // cboStatus
            // 
            this.cboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStatus.FormattingEnabled = true;
            this.cboStatus.Items.AddRange(new object[] {
            "Scheduled",
            "Confirmed",
            "Completed",
            "Cancelled",
            "No-show"});
            this.cboStatus.Location = new System.Drawing.Point(120, 219);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(220, 23);
            this.cboStatus.TabIndex = 13;
            // 
            // btnSave
            // 
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(120, 262);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 28);
            this.btnSave.TabIndex = 14;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(250, 262);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 28);
            this.btnCancel.TabIndex = 15;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += (s, e) => this.Close();
            // 
            // frmAppointmentEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(370, 310);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.cboStatus);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.cboType);
            this.Controls.Add(this.lblType);
            this.Controls.Add(this.dtpTime);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.dtpDate);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.txtDoctorId);
            this.Controls.Add(this.lblDoctorId);
            this.Controls.Add(this.txtPatientId);
            this.Controls.Add(this.lblPatientId);
            this.Controls.Add(this.txtCode);
            this.Controls.Add(this.lblCode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmAppointmentEdit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Appointment";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
