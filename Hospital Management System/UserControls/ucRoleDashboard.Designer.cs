namespace HospitalManagementSystem.UserControls
{
    partial class ucRoleDashboard
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel pnlCard;
        private System.Windows.Forms.Label lblRoleTitle;
        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Label lblHint;

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
            this.pnlCard = new System.Windows.Forms.Panel();
            this.lblHint = new System.Windows.Forms.Label();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.lblRoleTitle = new System.Windows.Forms.Label();
            this.pnlCard.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlCard
            // 
            this.pnlCard.BackColor = System.Drawing.Color.White;
            this.pnlCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCard.Controls.Add(this.lblHint);
            this.pnlCard.Controls.Add(this.lblWelcome);
            this.pnlCard.Controls.Add(this.lblRoleTitle);
            this.pnlCard.Location = new System.Drawing.Point(24, 24);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Size = new System.Drawing.Size(620, 170);
            this.pnlCard.TabIndex = 0;
            // 
            // lblHint
            // 
            this.lblHint.AutoSize = true;
            this.lblHint.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblHint.Location = new System.Drawing.Point(20, 106);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(246, 19);
            this.lblHint.TabIndex = 2;
            this.lblHint.Text = "Use the left menu to access modules.";
            // 
            // lblWelcome
            // 
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblWelcome.Location = new System.Drawing.Point(20, 68);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(98, 19);
            this.lblWelcome.TabIndex = 1;
            this.lblWelcome.Text = "Welcome, user";
            // 
            // lblRoleTitle
            // 
            this.lblRoleTitle.AutoSize = true;
            this.lblRoleTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblRoleTitle.Location = new System.Drawing.Point(18, 28);
            this.lblRoleTitle.Name = "lblRoleTitle";
            this.lblRoleTitle.Size = new System.Drawing.Size(153, 25);
            this.lblRoleTitle.TabIndex = 0;
            this.lblRoleTitle.Text = "Role Dashboard";
            // 
            // ucRoleDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.pnlCard);
            this.Name = "ucRoleDashboard";
            this.Size = new System.Drawing.Size(980, 590);
            this.pnlCard.ResumeLayout(false);
            this.pnlCard.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
