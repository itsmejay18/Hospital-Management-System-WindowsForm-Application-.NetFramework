namespace HospitalManagementSystem.Forms.Shared
{
    partial class frmAuditLog
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvAudit;

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
            this.dgvAudit = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAudit)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvAudit
            // 
            this.dgvAudit.AllowUserToAddRows = false;
            this.dgvAudit.AllowUserToDeleteRows = false;
            this.dgvAudit.BackgroundColor = System.Drawing.Color.White;
            this.dgvAudit.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAudit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAudit.Location = new System.Drawing.Point(0, 0);
            this.dgvAudit.Name = "dgvAudit";
            this.dgvAudit.ReadOnly = true;
            this.dgvAudit.RowTemplate.Height = 25;
            this.dgvAudit.Size = new System.Drawing.Size(900, 520);
            this.dgvAudit.TabIndex = 0;
            // 
            // frmAuditLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 520);
            this.Controls.Add(this.dgvAudit);
            this.Name = "frmAuditLog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Audit Log";
            ((System.ComponentModel.ISupportInitialize)(this.dgvAudit)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
