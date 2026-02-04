namespace HospitalManagementSystem.Forms.Shared
{
    partial class frmInvoicePayment
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblAmount;
        private System.Windows.Forms.NumericUpDown numAmount;
        private System.Windows.Forms.Label lblMethod;
        private System.Windows.Forms.ComboBox cboMethod;
        private System.Windows.Forms.Label lblReference;
        private System.Windows.Forms.TextBox txtReference;
        private System.Windows.Forms.Button btnProcess;
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
            this.lblAmount = new System.Windows.Forms.Label();
            this.numAmount = new System.Windows.Forms.NumericUpDown();
            this.lblMethod = new System.Windows.Forms.Label();
            this.cboMethod = new System.Windows.Forms.ComboBox();
            this.lblReference = new System.Windows.Forms.Label();
            this.txtReference = new System.Windows.Forms.TextBox();
            this.btnProcess = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numAmount)).BeginInit();
            this.SuspendLayout();
            // 
            // lblAmount
            // 
            this.lblAmount.AutoSize = true;
            this.lblAmount.Location = new System.Drawing.Point(16, 18);
            this.lblAmount.Name = "lblAmount";
            this.lblAmount.Size = new System.Drawing.Size(51, 15);
            this.lblAmount.TabIndex = 0;
            this.lblAmount.Text = "Amount";
            // 
            // numAmount
            // 
            this.numAmount.DecimalPlaces = 2;
            this.numAmount.Location = new System.Drawing.Point(110, 16);
            this.numAmount.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numAmount.Name = "numAmount";
            this.numAmount.Size = new System.Drawing.Size(220, 23);
            this.numAmount.TabIndex = 1;
            // 
            // lblMethod
            // 
            this.lblMethod.AutoSize = true;
            this.lblMethod.Location = new System.Drawing.Point(16, 52);
            this.lblMethod.Name = "lblMethod";
            this.lblMethod.Size = new System.Drawing.Size(50, 15);
            this.lblMethod.TabIndex = 2;
            this.lblMethod.Text = "Method";
            // 
            // cboMethod
            // 
            this.cboMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMethod.FormattingEnabled = true;
            this.cboMethod.Items.AddRange(new object[] {
            "Cash",
            "Credit Card",
            "Debit Card",
            "Insurance",
            "Online"});
            this.cboMethod.Location = new System.Drawing.Point(110, 49);
            this.cboMethod.Name = "cboMethod";
            this.cboMethod.Size = new System.Drawing.Size(220, 23);
            this.cboMethod.TabIndex = 3;
            // 
            // lblReference
            // 
            this.lblReference.AutoSize = true;
            this.lblReference.Location = new System.Drawing.Point(16, 86);
            this.lblReference.Name = "lblReference";
            this.lblReference.Size = new System.Drawing.Size(63, 15);
            this.lblReference.TabIndex = 4;
            this.lblReference.Text = "Reference";
            // 
            // txtReference
            // 
            this.txtReference.Location = new System.Drawing.Point(110, 83);
            this.txtReference.Name = "txtReference";
            this.txtReference.Size = new System.Drawing.Size(220, 23);
            this.txtReference.TabIndex = 5;
            // 
            // btnProcess
            // 
            this.btnProcess.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProcess.Location = new System.Drawing.Point(110, 125);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(90, 28);
            this.btnProcess.TabIndex = 6;
            this.btnProcess.Text = "Process";
            this.btnProcess.UseVisualStyleBackColor = true;
            this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(240, 125);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 28);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += (s, e) => this.Close();
            // 
            // frmInvoicePayment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 175);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnProcess);
            this.Controls.Add(this.txtReference);
            this.Controls.Add(this.lblReference);
            this.Controls.Add(this.cboMethod);
            this.Controls.Add(this.lblMethod);
            this.Controls.Add(this.numAmount);
            this.Controls.Add(this.lblAmount);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmInvoicePayment";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Process Payment";
            ((System.ComponentModel.ISupportInitialize)(this.numAmount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
