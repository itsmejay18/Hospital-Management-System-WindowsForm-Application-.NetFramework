namespace HospitalManagementSystem.UserControls
{
    partial class ucBilling
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnProcessPayment;
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
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnProcessPayment = new System.Windows.Forms.Button();
            this.dgvBilling = new System.Windows.Forms.DataGridView();
            this.colInvoice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPatient = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBilling)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlButtons
            // 
            this.pnlButtons.BackColor = System.Drawing.Color.White;
            this.pnlButtons.Controls.Add(this.btnProcessPayment);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlButtons.Location = new System.Drawing.Point(0, 0);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.pnlButtons.Size = new System.Drawing.Size(980, 48);
            this.pnlButtons.TabIndex = 0;
            // 
            // btnProcessPayment
            // 
            this.btnProcessPayment.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProcessPayment.Location = new System.Drawing.Point(12, 10);
            this.btnProcessPayment.Name = "btnProcessPayment";
            this.btnProcessPayment.Size = new System.Drawing.Size(140, 28);
            this.btnProcessPayment.TabIndex = 0;
            this.btnProcessPayment.Text = "Process Payment";
            this.btnProcessPayment.UseVisualStyleBackColor = true;
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
            this.dgvBilling.Location = new System.Drawing.Point(0, 48);
            this.dgvBilling.Name = "dgvBilling";
            this.dgvBilling.ReadOnly = true;
            this.dgvBilling.RowTemplate.Height = 25;
            this.dgvBilling.Size = new System.Drawing.Size(980, 542);
            this.dgvBilling.TabIndex = 1;
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
            this.Controls.Add(this.pnlButtons);
            this.Name = "ucBilling";
            this.Size = new System.Drawing.Size(980, 590);
            this.pnlButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBilling)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
