using MiniQsrApp.Data;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Data;
using System.Text.RegularExpressions;

namespace MiniQsrApp
{
    public partial class Form1 : Form
    {
        // #############################
        // KONSTANTEN & FELDER
        // UI Größen + DB + SQL Keywords
        // #############################

        private const int LeftPanelMinWidth = 120;
        private const int LeftPanelMaxWidth = 450;
        private const int ResizeGripWidth = 6;

        private readonly string _dbPath;
        private readonly DatabaseHelper _db;

        private bool _isResizingLeftPanel;
        private int _leftPanelResizeStartX;
        private int _leftPanelStartWidth;

        private readonly HashSet<string> _sqlKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "select","from","where","insert","into","values","update","set","delete",
            "create","table","drop","alter","join","left","right","inner","outer",
            "on","group","by","order","having","as","and","or","not","null","is",
            "in","like","distinct","limit","pragma","asc","desc"
        };

        // #############################
        // CONSTRUCTOR
        // Initialisiert DB, UI und Events
        // #############################

        public Form1()
        {
            InitializeComponent();

            _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MiniQsr.db");
            _db = new DatabaseHelper(_dbPath);

            InitializeApplication();
        }

        private void InitializeApplication()
        {
            _db.InitializeDatabase();

            leftPanel.Width = 0;
            lblReportTitle.Visible = false;

            InitializeButtonStyles();
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            leftPanel.MouseDown += leftPanel_MouseDown;
            leftPanel.MouseMove += leftPanel_MouseMove;
            leftPanel.MouseUp += leftPanel_MouseUp;
            txtSqlQuery.KeyUp += txtSqlQuery_KeyUp;
        }

        // #############################
        // BUTTON DESIGN
        // Einheitliches Styling
        // #############################

        private void InitializeButtonStyles()
        {
            ConfigureButton(btnRunQuery, Color.FromArgb(37, 99, 235));
            ConfigureButton(btnExportPdf, Color.FromArgb(16, 185, 129));
        }

        private void ConfigureButton(Button button, Color color)
        {
            button.BackColor = color;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;
        }

        // #############################
        // SQL AUSFÜHRUNG
        // SELECT → Anzeige
        // Andere → DB ändern
        // #############################

        private void btnRunQuery_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = txtSqlQuery.Text.Trim();

                if (string.IsNullOrWhiteSpace(sql))
                {
                    MessageBox.Show("Bitte geben Sie eine SQL-Abfrage ein.");
                    return;
                }

                if (sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                {
                    ExecuteSelect(sql);
                }
                else
                {
                    ExecuteNonSelect(sql);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL-Fehler: " + ex.Message);
            }
        }

        private void ExecuteSelect(string sql)
        {
            gridData.DataSource = _db.RunQuery(sql);

            string title = GenerateReportTitleFromSql(sql);
            lblReportTitle.Text = title;
            lblReportTitle.Visible = true;
        }

        private void ExecuteNonSelect(string sql)
        {
            _db.ExecuteCommand(sql);
            gridData.DataSource = _db.GetData();

            lblReportTitle.Visible = false;

            MessageBox.Show("Befehl wurde erfolgreich ausgeführt.");
        }

        // #############################
        // PDF EXPORT
        // Exportiert Grid in PDF
        // #############################

        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            if (gridData.Rows.Count == 0)
            {
                MessageBox.Show("Keine Daten vorhanden.");
                return;
            }

            string title = GenerateReportTitleFromSql(txtSqlQuery.Text);

            using SaveFileDialog dialog = new()
            {
                Filter = "PDF (*.pdf)|*.pdf",
                FileName = title + ".pdf"
            };

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            ExportPdf(dialog.FileName, title);

            MessageBox.Show("PDF exportiert.");
        }

        // #############################
        // PDF GENERIERUNG
        // Zeichnet Tabelle + Seitenumbruch
        // #############################

        private void ExportPdf(string filePath, string title)
        {
            PdfDocument doc = new();
            doc.Info.Title = title;

            PdfPage page = doc.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            XFont font = new("Arial", 9);
            XFont header = new("Arial", 10);
            XFont titleFont = new("Arial", 14);

            double margin = 40;
            double y = margin;
            double rowHeight = 20;

            gfx.DrawString(title, titleFont, XBrushes.Black,
                new XRect(margin, y, page.Width, rowHeight));

            y += 30;

            var visibleColumns = gridData.Columns
                .Cast<DataGridViewColumn>()
                .Where(c => c.Visible)
                .ToList();

            double colWidth = (page.Width - 2 * margin) / visibleColumns.Count;

            // Header
            double x = margin;
            foreach (var col in visibleColumns)
            {
                gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, x, y, colWidth, rowHeight);
                gfx.DrawString(col.HeaderText, header, XBrushes.Black,
                    new XRect(x + 2, y + 2, colWidth, rowHeight));
                x += colWidth;
            }

            y += rowHeight;

            // Rows
            foreach (DataGridViewRow row in gridData.Rows)
            {
                if (row.IsNewRow) continue;

                x = margin;

                foreach (var col in visibleColumns)
                {
                    string text = row.Cells[col.Index].Value?.ToString() ?? "";

                    gfx.DrawRectangle(XPens.Black, x, y, colWidth, rowHeight);
                    gfx.DrawString(text, font, XBrushes.Black,
                        new XRect(x + 2, y + 2, colWidth, rowHeight));

                    x += colWidth;
                }

                y += rowHeight;

                if (y > page.Height - margin)
                {
                    page = doc.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    y = margin;
                }
            }

            doc.Save(filePath);
        }

        // #############################
        // DATABASE STRUCTURE
        // Lädt Tabellen + Spalten in TreeView
        // #############################

        private void btnShowTables_Click(object sender, EventArgs e)
        {
            if (leftPanel.Width == 0)
            {
                leftPanel.Width = 180;
                LoadStructure();
            }
            else
            {
                leftPanel.Width = 0;
            }
        }

        private void LoadStructure()
        {
            treeDatabase.Nodes.Clear();

            var tables = _db.RunQuery("SELECT name FROM sqlite_master WHERE type='table'");

            foreach (DataRow t in tables.Rows)
            {
                string name = t["name"].ToString();
                TreeNode tableNode = new(name);

                var cols = _db.RunQuery($"PRAGMA table_info({name})");

                foreach (DataRow c in cols.Rows)
                {
                    tableNode.Nodes.Add($"{c["name"]} ({c["type"]})");
                }

                treeDatabase.Nodes.Add(tableNode);
            }
        }

        // #############################
        // PANEL RESIZE
        // Drag zum Vergrößern
        // #############################

        private void leftPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X >= leftPanel.Width - ResizeGripWidth)
            {
                _isResizingLeftPanel = true;
                _leftPanelResizeStartX = Cursor.Position.X;
                _leftPanelStartWidth = leftPanel.Width;
            }
        }

        private void leftPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isResizingLeftPanel)
            {
                leftPanel.Cursor = e.X >= leftPanel.Width - ResizeGripWidth
                    ? Cursors.VSplit
                    : Cursors.Default;
                return;
            }

            int delta = Cursor.Position.X - _leftPanelResizeStartX;
            int newWidth = _leftPanelStartWidth + delta;

            leftPanel.Width = Math.Max(LeftPanelMinWidth, Math.Min(newWidth, LeftPanelMaxWidth));
        }

        private void leftPanel_MouseUp(object sender, MouseEventArgs e)
        {
            _isResizingLeftPanel = false;
        }

        // #############################
        // SQL AUTO UPPERCASE
        // Keywords automatisch groß
        // #############################

        private void txtSqlQuery_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Space && e.KeyCode != Keys.Enter && e.KeyCode != Keys.Tab)
                return;

            int pos = txtSqlQuery.SelectionStart;
            string text = txtSqlQuery.Text;

            if (pos == 0) return;

            string before = text.Substring(0, pos);
            string trimmed = before.TrimEnd();

            int lastSpace = trimmed.LastIndexOf(' ');
            string word = trimmed[(lastSpace + 1)..];

            if (!_sqlKeywords.Contains(word)) return;

            string upper = word.ToUpper();
            txtSqlQuery.Text = text.Replace(word, upper);
            txtSqlQuery.SelectionStart = pos;
        }

        // #############################
        // REPORT TITLE GENERATOR
        // Erstellt Namen aus SQL
        // #############################

        private string GenerateReportTitleFromSql(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return "SQL_Report";

            var match = Regex.Match(sql, @"FROM\s+(\w+)", RegexOptions.IgnoreCase);

            string table = match.Success ? match.Groups[1].Value : "Report";

            return "Bericht_" + table;
        }
    }
}