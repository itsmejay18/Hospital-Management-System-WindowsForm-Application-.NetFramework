namespace HospitalManagementSystem.UserControls
{
    partial class ucBilling
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvBilling;
        private System.Windows.Forms.DataGridViewTextBoxColumn colInvoice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPatient;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;

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
            this.dgvBilling = new System.Windows.Forms.DataGridView();
            this.colInvoice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPatient = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBilling)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvBilling
            // 
            this.dgvBilling.AllowUserToAddRows = false;
            this.dgvBilling.AllowUserToDeleteRows = false;
            this.dgvBilling.BackgroundColor = System.Drawing.Color.White;
            this.dgvBilling.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBilling.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colInvoice,
            this.colPatient,
            this.colAmount,
            this.colStatus});
            this.dgvBilling.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvBilling.Location = new System.Drawing.Point(0, 0);
            this.dgvBilling.Name = "dgvBilling";
            this.dgvBilling.ReadOnly = true;
            this.dgvBilling.RowTemplate.Height = 25;
            this.dgvBilling.Size = new System.Drawing.Size(980, 590);
            this.dgvBilling.TabIndex = 0;
            // 
            // colInvoice
            // 
            this.colInvoice.DataPropertyName = "InvoiceNumber";
            this.colInvoice.HeaderText = "Invoice";
            this.colInvoice.Name = "colInvoice";
            this.colInvoice.ReadOnly = true;
            // 
            // colPatient
            // 
            this.colPatient.DataPropertyName = "PatientID";
            this.colPatient.HeaderText = "Patient ID";
            this.colPatient.Name = "colPatient";
            this.colPatient.ReadOnly = true;
            this.colPatient.Width = 220;
            // 
            // colAmount
            // 
            this.colAmount.DataPropertyName = "GrandTotal";
            this.colAmount.HeaderText = "Amount";
            this.colAmount.Name = "colAmount";
            this.colAmount.ReadOnly = true;
            // 
            // colStatus
            // 
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            // 
            // ucBilling
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgvBilling);
            this.Name = "ucBilling";
            this.Size = new System.Drawing.Size(980, 590);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBilling)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
