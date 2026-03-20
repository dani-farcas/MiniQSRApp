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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            gridData = new DataGridView();
            btnRunQuery = new Button();
            txtSqlQuery = new TextBox();
            topPanel = new Panel();
            btnShowTabels = new Button();
            btnExportPdf = new Button();
            gridContainer = new Panel();
            ((System.ComponentModel.ISupportInitialize)gridData).BeginInit();
            topPanel.SuspendLayout();
            gridContainer.SuspendLayout();
            SuspendLayout();
            // 
            // gridData
            // 
            gridData.BackgroundColor = SystemColors.Window;
            gridData.BorderStyle = BorderStyle.Fixed3D;
            gridData.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridData.Dock = DockStyle.Fill;
            gridData.Location = new Point(10, 10);
            gridData.Name = "gridData";
            gridData.RowHeadersWidth = 51;
            gridData.Size = new Size(780, 509);
            gridData.TabIndex = 0;
            gridData.CellContentClick += gridData_CellContentClick;
            // 
            // btnRunQuery
            // 
            btnRunQuery.BackColor = Color.FromArgb(37, 99, 235);
            btnRunQuery.Cursor = Cursors.Hand;
            btnRunQuery.FlatAppearance.BorderSize = 0;
            btnRunQuery.FlatStyle = FlatStyle.Flat;
            btnRunQuery.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnRunQuery.ForeColor = Color.White;
            btnRunQuery.Location = new Point(513, 170);
            btnRunQuery.Name = "btnRunQuery";
            btnRunQuery.Size = new Size(126, 29);
            btnRunQuery.TabIndex = 2;
            btnRunQuery.Text = "SQL ausführen";
            btnRunQuery.UseVisualStyleBackColor = false;
            btnRunQuery.Click += btnRunQuery_Click;
            btnRunQuery.MouseEnter += btnRunQuery_MouseEnter_1;
            btnRunQuery.MouseLeave += btnRunQuery_MouseLeave_1;
            // 
            // txtSqlQuery
            // 
            txtSqlQuery.BackColor = Color.FromArgb(224, 224, 224);
            txtSqlQuery.Dock = DockStyle.Top;
            txtSqlQuery.Location = new Point(10, 10);
            txtSqlQuery.Multiline = true;
            txtSqlQuery.Name = "txtSqlQuery";
            txtSqlQuery.PlaceholderText = "SQL:";
            txtSqlQuery.ScrollBars = ScrollBars.Vertical;
            txtSqlQuery.Size = new Size(780, 155);
            txtSqlQuery.TabIndex = 1;
            txtSqlQuery.WordWrap = false;
            txtSqlQuery.TextChanged += txtSqlQuery_TextChanged;
            // 
            // topPanel
            // 
            topPanel.BackColor = SystemColors.Window;
            topPanel.Controls.Add(btnShowTabels);
            topPanel.Controls.Add(btnExportPdf);
            topPanel.Controls.Add(txtSqlQuery);
            topPanel.Controls.Add(btnRunQuery);
            topPanel.Dock = DockStyle.Top;
            topPanel.Location = new Point(0, 0);
            topPanel.Name = "topPanel";
            topPanel.Padding = new Padding(10);
            topPanel.Size = new Size(800, 205);
            topPanel.TabIndex = 3;
            // 
            // btnShowTabels
            // 
            btnShowTabels.BackColor = Color.Maroon;
            btnShowTabels.Cursor = Cursors.Hand;
            btnShowTabels.FlatAppearance.BorderSize = 0;
            btnShowTabels.FlatStyle = FlatStyle.Flat;
            btnShowTabels.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnShowTabels.ForeColor = Color.White;
            btnShowTabels.Location = new Point(43, 171);
            btnShowTabels.Name = "btnShowTabels";
            btnShowTabels.Size = new Size(150, 29);
            btnShowTabels.TabIndex = 3;
            btnShowTabels.Text = "Tabellen anzeigen";
            btnShowTabels.UseVisualStyleBackColor = false;
            // 
            // btnExportPdf
            // 
            btnExportPdf.BackColor = Color.FromArgb(0, 192, 0);
            btnExportPdf.Cursor = Cursors.Hand;
            btnExportPdf.FlatAppearance.BorderSize = 0;
            btnExportPdf.FlatStyle = FlatStyle.Flat;
            btnExportPdf.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnExportPdf.ForeColor = Color.White;
            btnExportPdf.Location = new Point(645, 170);
            btnExportPdf.Name = "btnExportPdf";
            btnExportPdf.Size = new Size(118, 29);
            btnExportPdf.TabIndex = 1;
            btnExportPdf.Text = "Export PDF";
            btnExportPdf.UseVisualStyleBackColor = false;
            btnExportPdf.Click += btnExportPdf_Click;
            btnExportPdf.MouseEnter += btnExportPdf_MouseEnter_1;
            btnExportPdf.MouseLeave += btnExportPdf_MouseLeave_1;
            // 
            // gridContainer
            // 
            gridContainer.Controls.Add(gridData);
            gridContainer.Dock = DockStyle.Fill;
            gridContainer.Location = new Point(0, 205);
            gridContainer.Name = "gridContainer";
            gridContainer.Padding = new Padding(10);
            gridContainer.Size = new Size(800, 529);
            gridContainer.TabIndex = 4;
            gridContainer.Paint += panel1_Paint;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 734);
            Controls.Add(gridContainer);
            Controls.Add(topPanel);
            Name = "Form1";
            Text = "SQL_Reports";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)gridData).EndInit();
            topPanel.ResumeLayout(false);
            topPanel.PerformLayout();
            gridContainer.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DataGridView gridData;
        private Button btnRunQuery;
        private TextBox txtSqlQuery;
        private Panel topPanel;
        private Panel gridContainer;
        private Button btnExportPdf;
        private Button btnShowTabels;
    }
}
