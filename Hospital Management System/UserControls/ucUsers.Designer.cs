namespace HospitalManagementSystem.UserControls
{
    partial class ucUsers
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel pnlSearch;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.DataGridView dgvUsers;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUserId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUsername;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEmail;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRoleId;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colActive;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLastLogin;
        private System.Windows.Forms.GroupBox grpDetails;
        private System.Windows.Forms.Label lblDetailsHint;
        private System.Windows.Forms.Panel pnlDetailScroll;
        private System.Windows.Forms.TableLayoutPanel tlpDetails;
        private System.Windows.Forms.Label lblUserId;
        private System.Windows.Forms.TextBox txtUserId;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label lblRole;
        private System.Windows.Forms.ComboBox cboRole;
        private System.Windows.Forms.Label lblActiveName;
        private System.Windows.Forms.CheckBox chkActive;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblLastLogin;
        private System.Windows.Forms.TextBox txtLastLogin;

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
            this.pnlSearch = new System.Windows.Forms.Panel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.dgvUsers = new System.Windows.Forms.DataGridView();
            this.colUserId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUsername = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEmail = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRoleId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colActive = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colLastLogin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grpDetails = new System.Windows.Forms.GroupBox();
            this.pnlDetailScroll = new System.Windows.Forms.Panel();
            this.tlpDetails = new System.Windows.Forms.TableLayoutPanel();
            this.lblUserId = new System.Windows.Forms.Label();
            this.txtUserId = new System.Windows.Forms.TextBox();
            this.lblUsername = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.lblRole = new System.Windows.Forms.Label();
            this.cboRole = new System.Windows.Forms.ComboBox();
            this.lblActiveName = new System.Windows.Forms.Label();
            this.chkActive = new System.Windows.Forms.CheckBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblLastLogin = new System.Windows.Forms.Label();
            this.txtLastLogin = new System.Windows.Forms.TextBox();
            this.lblDetailsHint = new System.Windows.Forms.Label();
            this.pnlSearch.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).BeginInit();
            this.grpDetails.SuspendLayout();
            this.pnlDetailScroll.SuspendLayout();
            this.tlpDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSearch
            // 
            this.pnlSearch.Controls.Add(this.btnRefresh);
            this.pnlSearch.Controls.Add(this.btnSearch);
            this.pnlSearch.Controls.Add(this.txtSearch);
            this.pnlSearch.Controls.Add(this.lblSearch);
            this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSearch.Location = new System.Drawing.Point(0, 0);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Padding = new System.Windows.Forms.Padding(12, 10, 12, 8);
            this.pnlSearch.Size = new System.Drawing.Size(980, 54);
            this.pnlSearch.TabIndex = 0;
            // 
            // btnRefresh
            // 
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Location = new System.Drawing.Point(514, 12);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(88, 28);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            // 
            // btnSearch
            // 
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.Location = new System.Drawing.Point(420, 12);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(88, 28);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(99, 14);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(315, 23);
            this.txtSearch.TabIndex = 1;
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(12, 17);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(66, 15);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Search User";
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.btnDelete);
            this.pnlButtons.Controls.Add(this.btnCancel);
            this.pnlButtons.Controls.Add(this.btnSave);
            this.pnlButtons.Controls.Add(this.btnEdit);
            this.pnlButtons.Controls.Add(this.btnAdd);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlButtons.Location = new System.Drawing.Point(0, 54);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Padding = new System.Windows.Forms.Padding(12, 6, 12, 6);
            this.pnlButtons.Size = new System.Drawing.Size(980, 44);
            this.pnlButtons.TabIndex = 1;
            // 
            // btnDelete
            // 
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Location = new System.Drawing.Point(388, 8);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(90, 28);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(292, 8);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 28);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(196, 8);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 28);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnEdit
            // 
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEdit.Location = new System.Drawing.Point(100, 8);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(90, 28);
            this.btnEdit.TabIndex = 1;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            // 
            // btnAdd
            // 
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(4, 8);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(90, 28);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "Add New";
            this.btnAdd.UseVisualStyleBackColor = true;
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitMain.Location = new System.Drawing.Point(0, 98);
            this.splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.dgvUsers);
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.grpDetails);
            this.splitMain.Size = new System.Drawing.Size(980, 492);
            this.splitMain.SplitterDistance = 620;
            this.splitMain.TabIndex = 2;
            // 
            // dgvUsers
            // 
            this.dgvUsers.AllowUserToAddRows = false;
            this.dgvUsers.AllowUserToDeleteRows = false;
            this.dgvUsers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUsers.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colUserId,
            this.colUsername,
            this.colEmail,
            this.colRoleId,
            this.colActive,
            this.colLastLogin});
            this.dgvUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvUsers.Location = new System.Drawing.Point(0, 0);
            this.dgvUsers.Name = "dgvUsers";
            this.dgvUsers.ReadOnly = true;
            this.dgvUsers.RowTemplate.Height = 30;
            this.dgvUsers.Size = new System.Drawing.Size(620, 492);
            this.dgvUsers.TabIndex = 0;
            // 
            // colUserId
            // 
            this.colUserId.DataPropertyName = "UserID";
            this.colUserId.HeaderText = "ID";
            this.colUserId.Name = "colUserId";
            this.colUserId.ReadOnly = true;
            this.colUserId.Width = 60;
            // 
            // colUsername
            // 
            this.colUsername.DataPropertyName = "Username";
            this.colUsername.HeaderText = "Username";
            this.colUsername.Name = "colUsername";
            this.colUsername.ReadOnly = true;
            this.colUsername.Width = 150;
            // 
            // colEmail
            // 
            this.colEmail.DataPropertyName = "Email";
            this.colEmail.HeaderText = "Email";
            this.colEmail.Name = "colEmail";
            this.colEmail.ReadOnly = true;
            this.colEmail.Width = 180;
            // 
            // colRoleId
            // 
            this.colRoleId.DataPropertyName = "RoleID";
            this.colRoleId.HeaderText = "Role";
            this.colRoleId.Name = "colRoleId";
            this.colRoleId.ReadOnly = true;
            this.colRoleId.Width = 90;
            // 
            // colActive
            // 
            this.colActive.DataPropertyName = "IsActive";
            this.colActive.HeaderText = "Active";
            this.colActive.Name = "colActive";
            this.colActive.ReadOnly = true;
            this.colActive.Width = 60;
            // 
            // colLastLogin
            // 
            this.colLastLogin.DataPropertyName = "LastLogin";
            this.colLastLogin.HeaderText = "Last Login";
            this.colLastLogin.Name = "colLastLogin";
            this.colLastLogin.ReadOnly = true;
            this.colLastLogin.Width = 120;
            // 
            // grpDetails
            // 
            this.grpDetails.Controls.Add(this.pnlDetailScroll);
            this.grpDetails.Controls.Add(this.lblDetailsHint);
            this.grpDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpDetails.Location = new System.Drawing.Point(0, 0);
            this.grpDetails.Name = "grpDetails";
            this.grpDetails.Padding = new System.Windows.Forms.Padding(10, 8, 10, 8);
            this.grpDetails.Size = new System.Drawing.Size(356, 492);
            this.grpDetails.TabIndex = 0;
            this.grpDetails.TabStop = false;
            this.grpDetails.Text = "User Details";
            // 
            // pnlDetailScroll
            // 
            this.pnlDetailScroll.AutoScroll = true;
            this.pnlDetailScroll.Controls.Add(this.tlpDetails);
            this.pnlDetailScroll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDetailScroll.Location = new System.Drawing.Point(10, 24);
            this.pnlDetailScroll.Name = "pnlDetailScroll";
            this.pnlDetailScroll.Size = new System.Drawing.Size(336, 426);
            this.pnlDetailScroll.TabIndex = 0;
            // 
            // tlpDetails
            // 
            this.tlpDetails.AutoSize = true;
            this.tlpDetails.ColumnCount = 2;
            this.tlpDetails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 37F));
            this.tlpDetails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 63F));
            this.tlpDetails.Controls.Add(this.lblUserId, 0, 0);
            this.tlpDetails.Controls.Add(this.txtUserId, 1, 0);
            this.tlpDetails.Controls.Add(this.lblUsername, 0, 1);
            this.tlpDetails.Controls.Add(this.txtUsername, 1, 1);
            this.tlpDetails.Controls.Add(this.lblEmail, 0, 2);
            this.tlpDetails.Controls.Add(this.txtEmail, 1, 2);
            this.tlpDetails.Controls.Add(this.lblRole, 0, 3);
            this.tlpDetails.Controls.Add(this.cboRole, 1, 3);
            this.tlpDetails.Controls.Add(this.lblActiveName, 0, 4);
            this.tlpDetails.Controls.Add(this.chkActive, 1, 4);
            this.tlpDetails.Controls.Add(this.lblPassword, 0, 5);
            this.tlpDetails.Controls.Add(this.txtPassword, 1, 5);
            this.tlpDetails.Controls.Add(this.lblLastLogin, 0, 6);
            this.tlpDetails.Controls.Add(this.txtLastLogin, 1, 6);
            this.tlpDetails.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpDetails.Location = new System.Drawing.Point(0, 0);
            this.tlpDetails.Name = "tlpDetails";
            this.tlpDetails.Padding = new System.Windows.Forms.Padding(0, 4, 0, 8);
            this.tlpDetails.RowCount = 7;
            this.tlpDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tlpDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tlpDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tlpDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tlpDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tlpDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tlpDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tlpDetails.Size = new System.Drawing.Size(336, 264);
            this.tlpDetails.TabIndex = 0;
            // 
            // lblUserId
            // 
            this.lblUserId.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblUserId.AutoSize = true;
            this.lblUserId.Location = new System.Drawing.Point(3, 10);
            this.lblUserId.Name = "lblUserId";
            this.lblUserId.Size = new System.Drawing.Size(18, 15);
            this.lblUserId.TabIndex = 0;
            this.lblUserId.Text = "ID";
            // 
            // txtUserId
            // 
            this.txtUserId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUserId.Location = new System.Drawing.Point(127, 6);
            this.txtUserId.Name = "txtUserId";
            this.txtUserId.Size = new System.Drawing.Size(206, 23);
            this.txtUserId.TabIndex = 1;
            // 
            // lblUsername
            // 
            this.lblUsername.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(3, 46);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(60, 15);
            this.lblUsername.TabIndex = 2;
            this.lblUsername.Text = "Username";
            // 
            // txtUsername
            // 
            this.txtUsername.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUsername.Location = new System.Drawing.Point(127, 42);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(206, 23);
            this.txtUsername.TabIndex = 3;
            // 
            // lblEmail
            // 
            this.lblEmail.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblEmail.AutoSize = true;
            this.lblEmail.Location = new System.Drawing.Point(3, 82);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(36, 15);
            this.lblEmail.TabIndex = 4;
            this.lblEmail.Text = "Email";
            // 
            // txtEmail
            // 
            this.txtEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEmail.Location = new System.Drawing.Point(127, 78);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(206, 23);
            this.txtEmail.TabIndex = 5;
            // 
            // lblRole
            // 
            this.lblRole.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblRole.AutoSize = true;
            this.lblRole.Location = new System.Drawing.Point(3, 118);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(30, 15);
            this.lblRole.TabIndex = 6;
            this.lblRole.Text = "Role";
            // 
            // cboRole
            // 
            this.cboRole.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cboRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRole.FormattingEnabled = true;
            this.cboRole.Location = new System.Drawing.Point(127, 114);
            this.cboRole.Name = "cboRole";
            this.cboRole.Size = new System.Drawing.Size(206, 23);
            this.cboRole.TabIndex = 7;
            // 
            // lblActiveName
            // 
            this.lblActiveName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblActiveName.AutoSize = true;
            this.lblActiveName.Location = new System.Drawing.Point(3, 154);
            this.lblActiveName.Name = "lblActiveName";
            this.lblActiveName.Size = new System.Drawing.Size(38, 15);
            this.lblActiveName.TabIndex = 8;
            this.lblActiveName.Text = "Active";
            // 
            // chkActive
            // 
            this.chkActive.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkActive.AutoSize = true;
            this.chkActive.Location = new System.Drawing.Point(127, 152);
            this.chkActive.Name = "chkActive";
            this.chkActive.Size = new System.Drawing.Size(55, 19);
            this.chkActive.TabIndex = 9;
            this.chkActive.Text = "Yes";
            this.chkActive.UseVisualStyleBackColor = true;
            // 
            // lblPassword
            // 
            this.lblPassword.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(3, 190);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(57, 15);
            this.lblPassword.TabIndex = 10;
            this.lblPassword.Text = "Password";
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Location = new System.Drawing.Point(127, 186);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(206, 23);
            this.txtPassword.TabIndex = 11;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // lblLastLogin
            // 
            this.lblLastLogin.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblLastLogin.AutoSize = true;
            this.lblLastLogin.Location = new System.Drawing.Point(3, 226);
            this.lblLastLogin.Name = "lblLastLogin";
            this.lblLastLogin.Size = new System.Drawing.Size(58, 15);
            this.lblLastLogin.TabIndex = 12;
            this.lblLastLogin.Text = "Last Login";
            // 
            // txtLastLogin
            // 
            this.txtLastLogin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLastLogin.Location = new System.Drawing.Point(127, 222);
            this.txtLastLogin.Name = "txtLastLogin";
            this.txtLastLogin.Size = new System.Drawing.Size(206, 23);
            this.txtLastLogin.TabIndex = 13;
            // 
            // lblDetailsHint
            // 
            this.lblDetailsHint.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblDetailsHint.Location = new System.Drawing.Point(10, 450);
            this.lblDetailsHint.Name = "lblDetailsHint";
            this.lblDetailsHint.Size = new System.Drawing.Size(336, 34);
            this.lblDetailsHint.TabIndex = 1;
            this.lblDetailsHint.Text = "Select a user to view details. Click Edit to unlock fields.";
            this.lblDetailsHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ucUsers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.pnlSearch);
            this.Name = "ucUsers";
            this.Size = new System.Drawing.Size(980, 590);
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).EndInit();
            this.grpDetails.ResumeLayout(false);
            this.pnlDetailScroll.ResumeLayout(false);
            this.pnlDetailScroll.PerformLayout();
            this.tlpDetails.ResumeLayout(false);
            this.tlpDetails.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
