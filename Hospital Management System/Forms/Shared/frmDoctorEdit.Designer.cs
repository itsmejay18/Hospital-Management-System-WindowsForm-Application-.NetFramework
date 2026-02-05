namespace HospitalManagementSystem.Forms.Shared
{
    partial class frmDoctorEdit
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblCode;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Label lblUserId;
        private System.Windows.Forms.ComboBox cboUser;
        private System.Windows.Forms.Label lblSpecId;
        private System.Windows.Forms.ComboBox cboSpecialization;
        private System.Windows.Forms.Label lblLicense;
        private System.Windows.Forms.TextBox txtLicense;
        private System.Windows.Forms.Label lblQualification;
        private System.Windows.Forms.TextBox txtQualification;
        private System.Windows.Forms.Label lblFee;
        private System.Windows.Forms.NumericUpDown numFee;
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
            this.lblUserId = new System.Windows.Forms.Label();
            this.cboUser = new System.Windows.Forms.ComboBox();
            this.lblSpecId = new System.Windows.Forms.Label();
            this.cboSpecialization = new System.Windows.Forms.ComboBox();
            this.lblLicense = new System.Windows.Forms.Label();
            this.txtLicense = new System.Windows.Forms.TextBox();
            this.lblQualification = new System.Windows.Forms.Label();
            this.txtQualification = new System.Windows.Forms.TextBox();
            this.lblFee = new System.Windows.Forms.Label();
            this.numFee = new System.Windows.Forms.NumericUpDown();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numFee)).BeginInit();
            this.SuspendLayout();
            // 
            // lblCode
            // 
            this.lblCode.AutoSize = true;
            this.lblCode.Location = new System.Drawing.Point(16, 18);
            this.lblCode.Name = "lblCode";
            this.lblCode.Size = new System.Drawing.Size(67, 15);
            this.lblCode.TabIndex = 0;
            this.lblCode.Text = "Doctor Code";
            // 
            // txtCode
            // 
            this.txtCode.Location = new System.Drawing.Point(140, 15);
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(220, 23);
            this.txtCode.TabIndex = 1;
            // 
            // lblUserId
            // 
            this.lblUserId.AutoSize = true;
            this.lblUserId.Location = new System.Drawing.Point(16, 52);
            this.lblUserId.Name = "lblUserId";
            this.lblUserId.Size = new System.Drawing.Size(47, 15);
            this.lblUserId.TabIndex = 2;
            this.lblUserId.Text = "User ID";
            // 
            // cboUser
            // 
            this.cboUser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboUser.FormattingEnabled = true;
            this.cboUser.Location = new System.Drawing.Point(140, 49);
            this.cboUser.Name = "cboUser";
            this.cboUser.Size = new System.Drawing.Size(220, 23);
            this.cboUser.TabIndex = 3;
            // 
            // lblSpecId
            // 
            this.lblSpecId.AutoSize = true;
            this.lblSpecId.Location = new System.Drawing.Point(16, 86);
            this.lblSpecId.Name = "lblSpecId";
            this.lblSpecId.Size = new System.Drawing.Size(100, 15);
            this.lblSpecId.TabIndex = 4;
            this.lblSpecId.Text = "Specialization ID";
            // 
            // cboSpecialization
            // 
            this.cboSpecialization.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSpecialization.FormattingEnabled = true;
            this.cboSpecialization.Location = new System.Drawing.Point(140, 83);
            this.cboSpecialization.Name = "cboSpecialization";
            this.cboSpecialization.Size = new System.Drawing.Size(220, 23);
            this.cboSpecialization.TabIndex = 5;
            // 
            // lblLicense
            // 
            this.lblLicense.AutoSize = true;
            this.lblLicense.Location = new System.Drawing.Point(16, 120);
            this.lblLicense.Name = "lblLicense";
            this.lblLicense.Size = new System.Drawing.Size(84, 15);
            this.lblLicense.TabIndex = 6;
            this.lblLicense.Text = "License Number";
            // 
            // txtLicense
            // 
            this.txtLicense.Location = new System.Drawing.Point(140, 117);
            this.txtLicense.Name = "txtLicense";
            this.txtLicense.Size = new System.Drawing.Size(220, 23);
            this.txtLicense.TabIndex = 7;
            // 
            // lblQualification
            // 
            this.lblQualification.AutoSize = true;
            this.lblQualification.Location = new System.Drawing.Point(16, 154);
            this.lblQualification.Name = "lblQualification";
            this.lblQualification.Size = new System.Drawing.Size(71, 15);
            this.lblQualification.TabIndex = 8;
            this.lblQualification.Text = "Qualification";
            // 
            // txtQualification
            // 
            this.txtQualification.Location = new System.Drawing.Point(140, 151);
            this.txtQualification.Name = "txtQualification";
            this.txtQualification.Size = new System.Drawing.Size(220, 23);
            this.txtQualification.TabIndex = 9;
            // 
            // lblFee
            // 
            this.lblFee.AutoSize = true;
            this.lblFee.Location = new System.Drawing.Point(16, 188);
            this.lblFee.Name = "lblFee";
            this.lblFee.Size = new System.Drawing.Size(90, 15);
            this.lblFee.TabIndex = 10;
            this.lblFee.Text = "Consultation Fee";
            // 
            // numFee
            // 
            this.numFee.DecimalPlaces = 2;
            this.numFee.Location = new System.Drawing.Point(140, 186);
            this.numFee.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numFee.Name = "numFee";
            this.numFee.Size = new System.Drawing.Size(220, 23);
            this.numFee.TabIndex = 11;
            // 
            // btnSave
            // 
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(140, 228);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 28);
            this.btnSave.TabIndex = 12;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(270, 228);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 28);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += (s, e) => this.Close();
            // 
            // frmDoctorEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 275);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.numFee);
            this.Controls.Add(this.lblFee);
            this.Controls.Add(this.txtQualification);
            this.Controls.Add(this.lblQualification);
            this.Controls.Add(this.txtLicense);
            this.Controls.Add(this.lblLicense);
            this.Controls.Add(this.cboSpecialization);
            this.Controls.Add(this.lblSpecId);
            this.Controls.Add(this.cboUser);
            this.Controls.Add(this.lblUserId);
            this.Controls.Add(this.txtCode);
            this.Controls.Add(this.lblCode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmDoctorEdit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Doctor";
            ((System.ComponentModel.ISupportInitialize)(this.numFee)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
