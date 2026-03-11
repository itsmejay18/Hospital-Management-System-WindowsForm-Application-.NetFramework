namespace HospitalManagementSystem.Forms.Shared
{
    partial class frmBackupRestore
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblConnectionInfo;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Label lblBackupType;
        private System.Windows.Forms.ComboBox cboBackupType;
        private System.Windows.Forms.Label lblBackupPath;
        private System.Windows.Forms.TextBox txtBackupPath;
        private System.Windows.Forms.Button btnBrowsePath;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.DataGridView dgvBackups;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCreated;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colChangedTables;
        private System.Windows.Forms.DataGridViewTextBoxColumn colChangedRows;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDependency;
        private System.Windows.Forms.Button btnBackup;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.Button btnOpenFolder;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblStatus;

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
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblConnectionInfo = new System.Windows.Forms.Label();
            this.lblInfo = new System.Windows.Forms.Label();
            this.lblBackupType = new System.Windows.Forms.Label();
            this.cboBackupType = new System.Windows.Forms.ComboBox();
            this.lblBackupPath = new System.Windows.Forms.Label();
            this.txtBackupPath = new System.Windows.Forms.TextBox();
            this.btnBrowsePath = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.dgvBackups = new System.Windows.Forms.DataGridView();
            this.colCreated = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colChangedTables = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colChangedRows = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDependency = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnBackup = new System.Windows.Forms.Button();
            this.btnRestore = new System.Windows.Forms.Button();
            this.btnOpenFolder = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBackups)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 13F);
            this.lblTitle.Location = new System.Drawing.Point(20, 16);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(184, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Database Backups";
            // 
            // lblConnectionInfo
            // 
            this.lblConnectionInfo.AutoSize = true;
            this.lblConnectionInfo.Location = new System.Drawing.Point(22, 49);
            this.lblConnectionInfo.Name = "lblConnectionInfo";
            this.lblConnectionInfo.Size = new System.Drawing.Size(112, 15);
            this.lblConnectionInfo.TabIndex = 1;
            this.lblConnectionInfo.Text = "Active source: - / -";
            // 
            // lblInfo
            // 
            this.lblInfo.Location = new System.Drawing.Point(22, 72);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(836, 34);
            this.lblInfo.TabIndex = 2;
            this.lblInfo.Text = "Full backup captures schema and all rows. Incremental saves changes since the mos" +
    "t recent backup. Differential saves changes since the last full backup. Target f" +
    "older can be on local disk, USB, or a mapped device.";
            // 
            // lblBackupType
            // 
            this.lblBackupType.AutoSize = true;
            this.lblBackupType.Location = new System.Drawing.Point(22, 120);
            this.lblBackupType.Name = "lblBackupType";
            this.lblBackupType.Size = new System.Drawing.Size(77, 15);
            this.lblBackupType.TabIndex = 3;
            this.lblBackupType.Text = "Backup Type";
            // 
            // cboBackupType
            // 
            this.cboBackupType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBackupType.FormattingEnabled = true;
            this.cboBackupType.Location = new System.Drawing.Point(112, 116);
            this.cboBackupType.Name = "cboBackupType";
            this.cboBackupType.Size = new System.Drawing.Size(156, 23);
            this.cboBackupType.TabIndex = 4;
            // 
            // lblBackupPath
            // 
            this.lblBackupPath.AutoSize = true;
            this.lblBackupPath.Location = new System.Drawing.Point(287, 120);
            this.lblBackupPath.Name = "lblBackupPath";
            this.lblBackupPath.Size = new System.Drawing.Size(88, 15);
            this.lblBackupPath.TabIndex = 5;
            this.lblBackupPath.Text = "Target Folder";
            // 
            // txtBackupPath
            // 
            this.txtBackupPath.Location = new System.Drawing.Point(381, 116);
            this.txtBackupPath.Name = "txtBackupPath";
            this.txtBackupPath.Size = new System.Drawing.Size(332, 23);
            this.txtBackupPath.TabIndex = 6;
            // 
            // btnBrowsePath
            // 
            this.btnBrowsePath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowsePath.Location = new System.Drawing.Point(719, 114);
            this.btnBrowsePath.Name = "btnBrowsePath";
            this.btnBrowsePath.Size = new System.Drawing.Size(67, 28);
            this.btnBrowsePath.TabIndex = 7;
            this.btnBrowsePath.Text = "Browse";
            this.btnBrowsePath.UseVisualStyleBackColor = true;
            this.btnBrowsePath.Click += new System.EventHandler(this.btnBrowsePath_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Location = new System.Drawing.Point(792, 114);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(66, 28);
            this.btnRefresh.TabIndex = 8;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // dgvBackups
            // 
            this.dgvBackups.AllowUserToAddRows = false;
            this.dgvBackups.AllowUserToDeleteRows = false;
            this.dgvBackups.AllowUserToResizeRows = false;
            this.dgvBackups.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvBackups.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBackups.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCreated,
            this.colType,
            this.colChangedTables,
            this.colChangedRows,
            this.colDependency});
            this.dgvBackups.Location = new System.Drawing.Point(25, 156);
            this.dgvBackups.MultiSelect = false;
            this.dgvBackups.Name = "dgvBackups";
            this.dgvBackups.ReadOnly = true;
            this.dgvBackups.RowHeadersVisible = false;
            this.dgvBackups.RowTemplate.Height = 30;
            this.dgvBackups.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBackups.Size = new System.Drawing.Size(833, 322);
            this.dgvBackups.TabIndex = 9;
            // 
            // colCreated
            // 
            this.colCreated.DataPropertyName = "CreatedAtLocal";
            this.colCreated.HeaderText = "Created";
            this.colCreated.Name = "colCreated";
            this.colCreated.ReadOnly = true;
            this.colCreated.Width = 170;
            // 
            // colType
            // 
            this.colType.DataPropertyName = "BackupKind";
            this.colType.HeaderText = "Type";
            this.colType.Name = "colType";
            this.colType.ReadOnly = true;
            this.colType.Width = 110;
            // 
            // colChangedTables
            // 
            this.colChangedTables.DataPropertyName = "ChangedTables";
            this.colChangedTables.HeaderText = "Changed Tables";
            this.colChangedTables.Name = "colChangedTables";
            this.colChangedTables.ReadOnly = true;
            this.colChangedTables.Width = 120;
            // 
            // colChangedRows
            // 
            this.colChangedRows.DataPropertyName = "ChangedRows";
            this.colChangedRows.HeaderText = "Changed Rows";
            this.colChangedRows.Name = "colChangedRows";
            this.colChangedRows.ReadOnly = true;
            this.colChangedRows.Width = 120;
            // 
            // colDependency
            // 
            this.colDependency.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colDependency.DataPropertyName = "DependencyText";
            this.colDependency.HeaderText = "Restore Dependency";
            this.colDependency.Name = "colDependency";
            this.colDependency.ReadOnly = true;
            // 
            // btnBackup
            // 
            this.btnBackup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBackup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBackup.Location = new System.Drawing.Point(25, 492);
            this.btnBackup.Name = "btnBackup";
            this.btnBackup.Size = new System.Drawing.Size(130, 34);
            this.btnBackup.TabIndex = 10;
            this.btnBackup.Text = "Create Backup";
            this.btnBackup.UseVisualStyleBackColor = true;
            this.btnBackup.Click += new System.EventHandler(this.btnBackup_Click);
            // 
            // btnRestore
            // 
            this.btnRestore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRestore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRestore.Location = new System.Drawing.Point(161, 492);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(130, 34);
            this.btnRestore.TabIndex = 11;
            this.btnRestore.Text = "Restore Selected";
            this.btnRestore.UseVisualStyleBackColor = true;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // btnOpenFolder
            // 
            this.btnOpenFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOpenFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenFolder.Location = new System.Drawing.Point(297, 492);
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Size = new System.Drawing.Size(130, 34);
            this.btnOpenFolder.TabIndex = 12;
            this.btnOpenFolder.Text = "Open Folder";
            this.btnOpenFolder.UseVisualStyleBackColor = true;
            this.btnOpenFolder.Click += new System.EventHandler(this.btnOpenFolder_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(744, 492);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(114, 34);
            this.btnClose.TabIndex = 13;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStatus.Location = new System.Drawing.Point(25, 536);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(833, 22);
            this.lblStatus.TabIndex = 14;
            this.lblStatus.Text = "Ready.";
            // 
            // frmBackupRestore
            // 
            this.AcceptButton = this.btnBackup;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(884, 567);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnOpenFolder);
            this.Controls.Add(this.btnRestore);
            this.Controls.Add(this.btnBackup);
            this.Controls.Add(this.dgvBackups);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnBrowsePath);
            this.Controls.Add(this.txtBackupPath);
            this.Controls.Add(this.lblBackupPath);
            this.Controls.Add(this.cboBackupType);
            this.Controls.Add(this.lblBackupType);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.lblConnectionInfo);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmBackupRestore";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Backup / Restore";
            ((System.ComponentModel.ISupportInitialize)(this.dgvBackups)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
