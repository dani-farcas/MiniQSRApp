namespace MiniQsrApp
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support.
        /// </summary>
        private void InitializeComponent()
        {
            btnRunQuery = new Button();
            txtSqlQuery = new TextBox();
            btnExportPdf = new Button();
            btnShowTabels = new Button();
            topPanel = new Panel();
            lstSqlSuggestions = new ListBox();
            flowLayoutPanel1 = new FlowLayoutPanel();
            gridContainer = new Panel();
            gridData = new DataGridView();
            lblReportTitle = new Label();
            leftPanel = new Panel();
            treeDatabase = new TreeView();
            topPanel.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            gridContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridData).BeginInit();
            leftPanel.SuspendLayout();
            SuspendLayout();
            // 
            // btnRunQuery
            // 
            btnRunQuery.BackColor = Color.FromArgb(37, 99, 235);
            btnRunQuery.Cursor = Cursors.Hand;
            btnRunQuery.FlatAppearance.BorderSize = 0;
            btnRunQuery.FlatStyle = FlatStyle.Flat;
            btnRunQuery.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnRunQuery.ForeColor = Color.White;
            btnRunQuery.Location = new Point(326, 8);
            btnRunQuery.Name = "btnRunQuery";
            btnRunQuery.Size = new Size(126, 29);
            btnRunQuery.TabIndex = 2;
            btnRunQuery.Text = "SQL ausführen";
            btnRunQuery.UseVisualStyleBackColor = false;
            btnRunQuery.Click += btnRunQuery_Click;
            // 
            // txtSqlQuery
            // 
            txtSqlQuery.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtSqlQuery.BackColor = Color.FromArgb(224, 224, 224);
            txtSqlQuery.Dock = DockStyle.Top;
            txtSqlQuery.Location = new Point(10, 10);
            txtSqlQuery.Multiline = true;
            txtSqlQuery.Name = "txtSqlQuery";
            txtSqlQuery.PlaceholderText = "SQL Befehle:";
            txtSqlQuery.ScrollBars = ScrollBars.Vertical;
            txtSqlQuery.Size = new Size(589, 414);
            txtSqlQuery.TabIndex = 1;
            txtSqlQuery.WordWrap = false;
            // 
            // btnExportPdf
            // 
            btnExportPdf.BackColor = Color.FromArgb(16, 185, 129);
            btnExportPdf.Cursor = Cursors.Hand;
            btnExportPdf.FlatAppearance.BorderSize = 0;
            btnExportPdf.FlatStyle = FlatStyle.Flat;
            btnExportPdf.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnExportPdf.ForeColor = Color.White;
            btnExportPdf.Location = new Point(458, 8);
            btnExportPdf.Name = "btnExportPdf";
            btnExportPdf.Size = new Size(118, 29);
            btnExportPdf.TabIndex = 1;
            btnExportPdf.Text = "Export PDF";
            btnExportPdf.UseVisualStyleBackColor = false;
            btnExportPdf.Click += btnExportPdf_Click;
            // 
            // btnShowTabels
            // 
            btnShowTabels.BackColor = Color.Maroon;
            btnShowTabels.Cursor = Cursors.Hand;
            btnShowTabels.FlatAppearance.BorderSize = 0;
            btnShowTabels.FlatStyle = FlatStyle.Flat;
            btnShowTabels.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnShowTabels.ForeColor = Color.White;
            btnShowTabels.Location = new Point(170, 8);
            btnShowTabels.Name = "btnShowTabels";
            btnShowTabels.Size = new Size(150, 29);
            btnShowTabels.TabIndex = 3;
            btnShowTabels.Text = "Tabellen anzeigen";
            btnShowTabels.UseVisualStyleBackColor = false;
            btnShowTabels.Click += btnShowTables_Click;
            // 
            // topPanel
            // 
            topPanel.BackColor = SystemColors.Window;
            topPanel.Controls.Add(lstSqlSuggestions);
            topPanel.Controls.Add(txtSqlQuery);
            topPanel.Controls.Add(flowLayoutPanel1);
            topPanel.Dock = DockStyle.Top;
            topPanel.Location = new Point(191, 0);
            topPanel.Name = "topPanel";
            topPanel.Padding = new Padding(10);
            topPanel.Size = new Size(609, 487);
            topPanel.TabIndex = 3;
            // 
            // lstSqlSuggestions
            // 
            lstSqlSuggestions.FormattingEnabled = true;
            lstSqlSuggestions.IntegralHeight = false;
            lstSqlSuggestions.Location = new Point(13, 7);
            lstSqlSuggestions.Name = "lstSqlSuggestions";
            lstSqlSuggestions.Size = new Size(543, 417);
            lstSqlSuggestions.TabIndex = 5;
            lstSqlSuggestions.Visible = false;
            lstSqlSuggestions.SelectedIndexChanged += lstSqlSuggestions_SelectedIndexChanged;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(btnExportPdf);
            flowLayoutPanel1.Controls.Add(btnRunQuery);
            flowLayoutPanel1.Controls.Add(btnShowTabels);
            flowLayoutPanel1.Dock = DockStyle.Bottom;
            flowLayoutPanel1.FlowDirection = FlowDirection.RightToLeft;
            flowLayoutPanel1.Location = new Point(10, 430);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Padding = new Padding(0, 5, 10, 5);
            flowLayoutPanel1.Size = new Size(589, 47);
            flowLayoutPanel1.TabIndex = 4;
            flowLayoutPanel1.WrapContents = false;
            // 
            // gridContainer
            // 
            gridContainer.Controls.Add(gridData);
            gridContainer.Controls.Add(lblReportTitle);
            gridContainer.Dock = DockStyle.Fill;
            gridContainer.Location = new Point(191, 487);
            gridContainer.Name = "gridContainer";
            gridContainer.Padding = new Padding(10);
            gridContainer.Size = new Size(609, 247);
            gridContainer.TabIndex = 4;
            // 
            // gridData
            // 
            gridData.BackgroundColor = SystemColors.Window;
            gridData.BorderStyle = BorderStyle.Fixed3D;
            gridData.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridData.Dock = DockStyle.Fill;
            gridData.Location = new Point(10, 45);
            gridData.Name = "gridData";
            gridData.RowHeadersWidth = 51;
            gridData.Size = new Size(589, 192);
            gridData.TabIndex = 0;
            // 
            // lblReportTitle
            // 
            lblReportTitle.Dock = DockStyle.Top;
            lblReportTitle.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblReportTitle.ForeColor = Color.Black;
            lblReportTitle.Location = new Point(10, 10);
            lblReportTitle.Name = "lblReportTitle";
            lblReportTitle.Padding = new Padding(10, 10, 0, 0);
            lblReportTitle.Size = new Size(589, 35);
            lblReportTitle.TabIndex = 1;
            lblReportTitle.Text = "label1";
            lblReportTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblReportTitle.Visible = false;
            // 
            // leftPanel
            // 
            leftPanel.BackColor = Color.FromArgb(224, 224, 224);
            leftPanel.BorderStyle = BorderStyle.Fixed3D;
            leftPanel.Controls.Add(treeDatabase);
            leftPanel.Dock = DockStyle.Left;
            leftPanel.Location = new Point(0, 0);
            leftPanel.Name = "leftPanel";
            leftPanel.Padding = new Padding(10);
            leftPanel.Size = new Size(191, 734);
            leftPanel.TabIndex = 5;
            // 
            // treeDatabase
            // 
            treeDatabase.Dock = DockStyle.Fill;
            treeDatabase.Location = new Point(10, 10);
            treeDatabase.Name = "treeDatabase";
            treeDatabase.Size = new Size(167, 710);
            treeDatabase.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 734);
            Controls.Add(gridContainer);
            Controls.Add(topPanel);
            Controls.Add(leftPanel);
            Name = "Form1";
            Text = "SQL_Reports";
            topPanel.ResumeLayout(false);
            topPanel.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            gridContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridData).EndInit();
            leftPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button btnRunQuery;
        private TextBox txtSqlQuery;
        private Button btnExportPdf;
        private Button btnShowTabels;
        private Panel topPanel;
        private Panel gridContainer;
        private DataGridView gridData;
        private Panel leftPanel;
        private TreeView treeDatabase;
        private FlowLayoutPanel flowLayoutPanel1;
        private Label lblReportTitle;
        private ListBox lstSqlSuggestions;
    }
}