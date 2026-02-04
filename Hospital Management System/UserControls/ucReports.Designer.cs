namespace HospitalManagementSystem.UserControls
{
    partial class ucReports
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblPlaceholder;

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
            this.lblPlaceholder = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblPlaceholder
            // 
            this.lblPlaceholder.AutoSize = true;
            this.lblPlaceholder.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblPlaceholder.Location = new System.Drawing.Point(24, 24);
            this.lblPlaceholder.Name = "lblPlaceholder";
            this.lblPlaceholder.Size = new System.Drawing.Size(180, 21);
            this.lblPlaceholder.TabIndex = 0;
            this.lblPlaceholder.Text = "Reports coming soon";
            // 
            // ucReports
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblPlaceholder);
            this.Name = "ucReports";
            this.Size = new System.Drawing.Size(980, 590);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
