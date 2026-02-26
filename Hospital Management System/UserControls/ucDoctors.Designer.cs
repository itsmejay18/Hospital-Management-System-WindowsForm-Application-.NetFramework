namespace HospitalManagementSystem.UserControls
{
    partial class ucDoctors
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
        private System.Windows.Forms.DataGridView dgvDoctors;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSpec;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFee;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colAvailable;
        private System.Windows.Forms.GroupBox grpDetails;
        private System.Windows.Forms.Label lblDetailsHint;
        private System.Windows.Forms.TableLayoutPanel tlpDetails;
        private System.Windows.Forms.Label lblCode;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblSpec;
        private System.Windows.Forms.ComboBox cboSpecialization;
        private System.Windows.Forms.Label lblQualification;
        private System.Windows.Forms.TextBox txtQualification;
        private System.Windows.Forms.Label lblLicense;
        private System.Windows.Forms.TextBox txtLicense;
        private System.Windows.Forms.Label lblExp;
        private System.Windows.Forms.NumericUpDown nudExperience;
        private System.Windows.Forms.Label lblFee;
        private System.Windows.Forms.NumericUpDown nudConsultationFee;
        private System.Windows.Forms.Label lblAvailable;
        private System.Windows.Forms.CheckBox chkAvailable;
        private System.Windows.Forms.Label lblJoining;
        private System.Windows.Forms.DateTimePicker dtpJoiningDate;

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
            this.dgvDoctors = new System.Windows.Forms.DataGridView();
            this.colCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSpec = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFee = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAvailable = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.grpDetails = new System.Windows.Forms.GroupBox();
            this.tlpDetails = new System.Windows.Forms.TableLayoutPanel();
            this.lblCode = new System.Windows.Forms.Label();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblSpec = new System.Windows.Forms.Label();
            this.cboSpecialization = new System.Windows.Forms.ComboBox();
            this.lblQualification = new System.Windows.Forms.Label();
            this.txtQualification = new System.Windows.Forms.TextBox();
            this.lblLicense = new System.Windows.Forms.Label();
            this.txtLicense = new System.Windows.Forms.TextBox();
            this.lblExp = new System.Windows.Forms.Label();
            this.nudExperience = new System.Windows.Forms.NumericUpDown();
            this.lblFee = new System.Windows.Forms.Label();
            this.nudConsultationFee = new System.Windows.Forms.NumericUpDown();
            this.lblAvailable = new System.Windows.Forms.Label();
            this.chkAvailable = new System.Windows.Forms.CheckBox();
            this.lblJoining = new System.Windows.Forms.Label();
            this.dtpJoiningDate = new System.Windows.Forms.DateTimePicker();
            this.lblDetailsHint = new System.Windows.Forms.Label();
            this.pnlSearch.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDoctors)).BeginInit();
            this.grpDetails.SuspendLayout();
            this.tlpDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudExperience)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudConsultationFee)).BeginInit();
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
            this.lblSearch.Size = new System.Drawing.Size(79, 15);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Search Doctor";
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
            this.splitMain.Panel1.Controls.Add(this.dgvDoctors);
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.grpDetails);
            this.splitMain.Size = new System.Drawing.Size(980, 492);
            this.splitMain.SplitterDistance = 620;
            this.splitMain.TabIndex = 2;
            // 
            // dgvDoctors
            // 
            this.dgvDoctors.AllowUserToAddRows = false;
            this.dgvDoctors.AllowUserToDeleteRows = false;
            this.dgvDoctors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDoctors.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCode,
            this.colName,
            this.colSpec,
            this.colFee,
            this.colAvailable});
            this.dgvDoctors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDoctors.Location = new System.Drawing.Point(0, 0);
            this.dgvDoctors.Name = "dgvDoctors";
            this.dgvDoctors.ReadOnly = true;
            this.dgvDoctors.RowTemplate.Height = 30;
            this.dgvDoctors.Size = new System.Drawing.Size(620, 492);
            this.dgvDoctors.TabIndex = 0;
            // 
            // colCode
            // 
            this.colCode.DataPropertyName = "DoctorCode";
            this.colCode.HeaderText = "Code";
            this.colCode.Name = "colCode";
            this.colCode.ReadOnly = true;
            this.colCode.Width = 90;
            // 
            // colName
            // 
            this.colName.DataPropertyName = "DoctorName";
            this.colName.HeaderText = "Doctor";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            this.colName.Width = 180;
            // 
            // colSpec
            // 
            this.colSpec.DataPropertyName = "SpecializationName";
            this.colSpec.HeaderText = "Specialization";
            this.colSpec.Name = "colSpec";
            this.colSpec.ReadOnly = true;
            this.colSpec.Width = 170;
            // 
            // colFee
            // 
            this.colFee.DataPropertyName = "ConsultationFee";
            this.colFee.HeaderText = "Fee";
            this.colFee.Name = "colFee";
            this.colFee.ReadOnly = true;
            this.colFee.Width = 80;
            // 
            // colAvailable
            // 
            this.colAvailable.DataPropertyName = "IsAvailable";
            this.colAvailable.HeaderText = "Available";
            this.colAvailable.Name = "colAvailable";
            this.colAvailable.ReadOnly = true;
            this.colAvailable.Width = 70;
            // 
            // grpDetails
            // 
            this.grpDetails.Controls.Add(this.tlpDetails);
            this.grpDetails.Controls.Add(this.lblDetailsHint);
            this.grpDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpDetails.Location = new System.Drawing.Point(0, 0);
            this.grpDetails.Name = "grpDetails";
            this.grpDetails.Padding = new System.Windows.Forms.Padding(10, 8, 10, 8);
            this.grpDetails.Size = new System.Drawing.Size(356, 492);
            this.grpDetails.TabIndex = 0;
            this.grpDetails.TabStop = false;
            this.grpDetails.Text = "Doctor Details";
            // 
            // tlpDetails
            // 
            this.tlpDetails.ColumnCount = 2;
            this.tlpDetails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 37F));
            this.tlpDetails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 63F));
            this.tlpDetails.Controls.Add(this.lblCode, 0, 0);
            this.tlpDetails.Controls.Add(this.txtCode, 1, 0);
            this.tlpDetails.Controls.Add(this.lblName, 0, 1);
            this.tlpDetails.Controls.Add(this.txtName, 1, 1);
            this.tlpDetails.Controls.Add(this.lblSpec, 0, 2);
            this.tlpDetails.Controls.Add(this.cboSpecialization, 1, 2);
            this.tlpDetails.Controls.Add(this.lblQualification, 0, 3);
            this.tlpDetails.Controls.Add(this.txtQualification, 1, 3);
            this.tlpDetails.Controls.Add(this.lblLicense, 0, 4);
            this.tlpDetails.Controls.Add(this.txtLicense, 1, 4);
            this.tlpDetails.Controls.Add(this.lblExp, 0, 5);
            this.tlpDetails.Controls.Add(this.nudExperience, 1, 5);
            this.tlpDetails.Controls.Add(this.lblFee, 0, 6);
            this.tlpDetails.Controls.Add(this.nudConsultationFee, 1, 6);
            this.tlpDetails.Controls.Add(this.lblAvailable, 0, 7);
            this.tlpDetails.Controls.Add(this.chkAvailable, 1, 7);
            this.tlpDetails.Controls.Add(this.lblJoining, 0, 8);
            this.tlpDetails.Controls.Add(this.dtpJoiningDate, 1, 8);
            this.tlpDetails.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpDetails.Location = new System.Drawing.Point(10, 24);
            this.tlpDetails.Name = "tlpDetails";
            this.tlpDetails.RowCount = 9;
            this.tlpDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tlpDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tlpDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tlpDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tlpDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tlpDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tlpDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tlpDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tlpDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tlpDetails.Size = new System.Drawing.Size(336, 324);
            this.tlpDetails.TabIndex = 0;
            // 
            // lblCode
            // 
            this.lblCode.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblCode.AutoSize = true;
            this.lblCode.Location = new System.Drawing.Point(3, 10);
            this.lblCode.Name = "lblCode";
            this.lblCode.Size = new System.Drawing.Size(35, 15);
            this.lblCode.TabIndex = 0;
            this.lblCode.Text = "Code";
            // 
            // txtCode
            // 
            this.txtCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCode.Location = new System.Drawing.Point(127, 6);
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(206, 23);
            this.txtCode.TabIndex = 1;
            // 
            // lblName
            // 
            this.lblName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(3, 46);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(78, 15);
            this.lblName.TabIndex = 2;
            this.lblName.Text = "Doctor Name";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(127, 42);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(206, 23);
            this.txtName.TabIndex = 3;
            // 
            // lblSpec
            // 
            this.lblSpec.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblSpec.AutoSize = true;
            this.lblSpec.Location = new System.Drawing.Point(3, 82);
            this.lblSpec.Name = "lblSpec";
            this.lblSpec.Size = new System.Drawing.Size(79, 15);
            this.lblSpec.TabIndex = 4;
            this.lblSpec.Text = "Specialization";
            // 
            // cboSpecialization
            // 
            this.cboSpecialization.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cboSpecialization.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSpecialization.FormattingEnabled = true;
            this.cboSpecialization.Location = new System.Drawing.Point(127, 78);
            this.cboSpecialization.Name = "cboSpecialization";
            this.cboSpecialization.Size = new System.Drawing.Size(206, 23);
            this.cboSpecialization.TabIndex = 5;
            // 
            // lblQualification
            // 
            this.lblQualification.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblQualification.AutoSize = true;
            this.lblQualification.Location = new System.Drawing.Point(3, 118);
            this.lblQualification.Name = "lblQualification";
            this.lblQualification.Size = new System.Drawing.Size(75, 15);
            this.lblQualification.TabIndex = 6;
            this.lblQualification.Text = "Qualification";
            // 
            // txtQualification
            // 
            this.txtQualification.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtQualification.Location = new System.Drawing.Point(127, 114);
            this.txtQualification.Name = "txtQualification";
            this.txtQualification.Size = new System.Drawing.Size(206, 23);
            this.txtQualification.TabIndex = 7;
            // 
            // lblLicense
            // 
            this.lblLicense.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblLicense.AutoSize = true;
            this.lblLicense.Location = new System.Drawing.Point(3, 154);
            this.lblLicense.Name = "lblLicense";
            this.lblLicense.Size = new System.Drawing.Size(84, 15);
            this.lblLicense.TabIndex = 8;
            this.lblLicense.Text = "License No.";
            // 
            // txtLicense
            // 
            this.txtLicense.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLicense.Location = new System.Drawing.Point(127, 150);
            this.txtLicense.Name = "txtLicense";
            this.txtLicense.Size = new System.Drawing.Size(206, 23);
            this.txtLicense.TabIndex = 9;
            // 
            // lblExp
            // 
            this.lblExp.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblExp.AutoSize = true;
            this.lblExp.Location = new System.Drawing.Point(3, 190);
            this.lblExp.Name = "lblExp";
            this.lblExp.Size = new System.Drawing.Size(67, 15);
            this.lblExp.TabIndex = 10;
            this.lblExp.Text = "Experience";
            // 
            // nudExperience
            // 
            this.nudExperience.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.nudExperience.Location = new System.Drawing.Point(127, 186);
            this.nudExperience.Maximum = new decimal(new int[] {
            70,
            0,
            0,
            0});
            this.nudExperience.Name = "nudExperience";
            this.nudExperience.Size = new System.Drawing.Size(206, 23);
            this.nudExperience.TabIndex = 11;
            // 
            // lblFee
            // 
            this.lblFee.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblFee.AutoSize = true;
            this.lblFee.Location = new System.Drawing.Point(3, 226);
            this.lblFee.Name = "lblFee";
            this.lblFee.Size = new System.Drawing.Size(91, 15);
            this.lblFee.TabIndex = 12;
            this.lblFee.Text = "Consultation Fee";
            // 
            // nudConsultationFee
            // 
            this.nudConsultationFee.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.nudConsultationFee.DecimalPlaces = 2;
            this.nudConsultationFee.Location = new System.Drawing.Point(127, 222);
            this.nudConsultationFee.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.nudConsultationFee.Name = "nudConsultationFee";
            this.nudConsultationFee.Size = new System.Drawing.Size(206, 23);
            this.nudConsultationFee.TabIndex = 13;
            // 
            // lblAvailable
            // 
            this.lblAvailable.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblAvailable.AutoSize = true;
            this.lblAvailable.Location = new System.Drawing.Point(3, 262);
            this.lblAvailable.Name = "lblAvailable";
            this.lblAvailable.Size = new System.Drawing.Size(54, 15);
            this.lblAvailable.TabIndex = 14;
            this.lblAvailable.Text = "Available";
            // 
            // chkAvailable
            // 
            this.chkAvailable.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkAvailable.AutoSize = true;
            this.chkAvailable.Location = new System.Drawing.Point(127, 260);
            this.chkAvailable.Name = "chkAvailable";
            this.chkAvailable.Size = new System.Drawing.Size(55, 19);
            this.chkAvailable.TabIndex = 15;
            this.chkAvailable.Text = "Yes";
            this.chkAvailable.UseVisualStyleBackColor = true;
            // 
            // lblJoining
            // 
            this.lblJoining.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblJoining.AutoSize = true;
            this.lblJoining.Location = new System.Drawing.Point(3, 298);
            this.lblJoining.Name = "lblJoining";
            this.lblJoining.Size = new System.Drawing.Size(70, 15);
            this.lblJoining.TabIndex = 16;
            this.lblJoining.Text = "Joining Date";
            // 
            // dtpJoiningDate
            // 
            this.dtpJoiningDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpJoiningDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpJoiningDate.Location = new System.Drawing.Point(127, 294);
            this.dtpJoiningDate.Name = "dtpJoiningDate";
            this.dtpJoiningDate.Size = new System.Drawing.Size(206, 23);
            this.dtpJoiningDate.TabIndex = 17;
            // 
            // lblDetailsHint
            // 
            this.lblDetailsHint.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblDetailsHint.Location = new System.Drawing.Point(10, 450);
            this.lblDetailsHint.Name = "lblDetailsHint";
            this.lblDetailsHint.Size = new System.Drawing.Size(336, 34);
            this.lblDetailsHint.TabIndex = 1;
            this.lblDetailsHint.Text = "Select a doctor to view details. Click Edit to unlock.";
            this.lblDetailsHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ucDoctors
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.pnlSearch);
            this.Name = "ucDoctors";
            this.Size = new System.Drawing.Size(980, 590);
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDoctors)).EndInit();
            this.grpDetails.ResumeLayout(false);
            this.tlpDetails.ResumeLayout(false);
            this.tlpDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudExperience)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudConsultationFee)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
