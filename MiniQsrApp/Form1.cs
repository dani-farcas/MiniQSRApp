using MiniQsrApp.Data;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Data;
using System.Text.RegularExpressions;

namespace MiniQsrApp
{
    public partial class Form1 : Form
    {
        private readonly string _dbPath;
        private readonly DatabaseHelper _db;

        private bool _isResizingLeftPanel = false;
        private int _leftPanelResizeStartX = 0;
        private int _leftPanelStartWidth = 0;
        private const int LeftPanelMinWidth = 120;
        private const int LeftPanelMaxWidth = 450;
        private const int ResizeGripWidth = 6;


        private readonly HashSet<string> _sqlKeywords = new HashSet<string>
{
    "select", "from", "where", "insert", "into", "values",
    "update", "set", "delete", "create", "table", "drop",
    "alter", "join", "left", "right", "inner", "outer",
    "on", "group", "by", "order", "having", "as",
    "and", "or", "not", "null", "is", "in", "like",
    "distinct", "limit", "pragma", "asc", "desc"
};
        public Form1()
        {
            InitializeComponent();

            leftPanel.MouseDown += leftPanel_MouseDown;
            leftPanel.MouseMove += leftPanel_MouseMove;
            leftPanel.MouseUp += leftPanel_MouseUp;
            txtSqlQuery.KeyUp += txtSqlQuery_KeyUp;

            leftPanel.Width = 0;

            // Initialisiert den Pfad zur lokalen SQLite-Datenbank.
            _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MiniQsr.db");

            // Erstellt die Datenbank-Hilfsklasse für alle DB-Operationen.
            _db = new DatabaseHelper(_dbPath);

            // Stellt sicher, dass die Datenbank und die benötigten Tabellen vorhanden sind.
            _db.InitializeDatabase();

            // Startzustand der linken Strukturleiste.
            leftPanel.Width = 0;

            // Initialisiert das Button-Design und Hover-Verhalten.
            InitializeButtonStyles();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void InitializeButtonStyles()
        {
            // Modernes Design für den SQL-Ausführen-Button.
            btnRunQuery.BackColor = Color.FromArgb(37, 99, 235);
            btnRunQuery.ForeColor = Color.White;
            btnRunQuery.FlatStyle = FlatStyle.Flat;
            btnRunQuery.FlatAppearance.BorderSize = 0;
            btnRunQuery.Cursor = Cursors.Hand;

            // Modernes Design für den PDF-Export-Button.
            btnExportPdf.BackColor = Color.FromArgb(16, 185, 129);
            btnExportPdf.ForeColor = Color.White;
            btnExportPdf.FlatStyle = FlatStyle.Flat;
            btnExportPdf.FlatAppearance.BorderSize = 0;
            btnExportPdf.Cursor = Cursors.Hand;
        }

        private void btnRunQuery_Click(object sender, EventArgs e)
        {
            try
            {
                // Liest die SQL-Abfrage aus dem Textfeld und entfernt unnötige Leerzeichen.
                string sql = txtSqlQuery.Text.Trim();

                if (string.IsNullOrWhiteSpace(sql))
                {
                    MessageBox.Show("Bitte geben Sie eine SQL-Abfrage ein.");
                    return;
                }

                // SELECT-Abfragen liefern Daten für das Grid zurück.
                if (sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                {
                    gridData.DataSource = _db.RunQuery(sql);

                    // Erstellt automatisch einen Titel für den aktuellen Bericht.
                    string reportTitle = GenerateReportTitleFromSql(sql);
                    lblReportTitle.Text = reportTitle;
                    lblReportTitle.Visible = true;
                }
                else
                {
                    // Nicht-SELECT-Befehle werden direkt ausgeführt.
                    _db.ExecuteCommand(sql);

                    // Nach Änderungen werden die aktuellen Daten neu geladen.
                    gridData.DataSource = _db.GetData();

                    // Für technische Befehle wird kein Berichtstitel angezeigt.
                    lblReportTitle.Text = string.Empty;
                    lblReportTitle.Visible = false;

                    MessageBox.Show("Befehl wurde erfolgreich ausgeführt.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL-Fehler: " + ex.Message);
            }
        }

        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            // Prüft, ob überhaupt exportierbare Daten vorhanden sind.
            if (gridData.Rows.Count == 0 || gridData.Columns.Count == 0)
            {
                MessageBox.Show("Es gibt keine Daten zum Exportieren.");
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PDF-Datei (*.pdf)|*.pdf";
                saveFileDialog.Title = "PDF speichern";
                string reportTitle = GenerateReportTitleFromSql(txtSqlQuery.Text);
                saveFileDialog.FileName = reportTitle + ".pdf";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportDataGridViewToPdf(saveFileDialog.FileName, reportTitle);
                    MessageBox.Show("PDF wurde erfolgreich exportiert.");
                }
            }
        }

        private void ExportDataGridViewToPdf(string filePath, string reportTitle)
        {
            // Erstellt ein neues PDF-Dokument.
            PdfDocument document = new PdfDocument
            {
                Info =
                {
                    Title = reportTitle
                }
            };

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Definiert die verwendeten Schriftarten.
            XFont titleFont = new XFont("Arial", 14);
            XFont headerFont = new XFont("Arial", 10);
            XFont cellFont = new XFont("Arial", 9);

            double margin = 40;
            double y = margin;
            double rowHeight = 20;
            double tableWidth = page.Width - 2 * margin;

            // Zählt nur sichtbare Spalten.
            int visibleColumnCount = 0;
            foreach (DataGridViewColumn column in gridData.Columns)
            {
                if (column.Visible)
                {
                    visibleColumnCount++;
                }
            }

            if (visibleColumnCount == 0)
            {
                MessageBox.Show("Es gibt keine sichtbaren Spalten zum Exportieren.");
                return;
            }

            double columnWidth = tableWidth / visibleColumnCount;

            // Zeichnet den Titel des Reports.
            gfx.DrawString(
                reportTitle,
                titleFont,
                XBrushes.Black,
                new XRect(margin, y, tableWidth, rowHeight),
                XStringFormats.TopLeft
            );

            y += 30;

            // Zeichnet die Tabellenüberschriften.
            double x = margin;

            foreach (DataGridViewColumn column in gridData.Columns)
            {
                if (!column.Visible)
                {
                    continue;
                }

                gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, x, y, columnWidth, rowHeight);
                gfx.DrawString(
                    column.HeaderText,
                    headerFont,
                    XBrushes.Black,
                    new XRect(x + 2, y + 3, columnWidth - 4, rowHeight - 6),
                    XStringFormats.TopLeft
                );

                x += columnWidth;
            }

            y += rowHeight;

            // Zeichnet alle Datenzeilen.
            foreach (DataGridViewRow row in gridData.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                // Erstellt automatisch eine neue Seite, wenn der Platz nicht mehr ausreicht.
                if (y + rowHeight > page.Height - margin)
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    y = margin;

                    x = margin;

                    foreach (DataGridViewColumn column in gridData.Columns)
                    {
                        if (!column.Visible)
                        {
                            continue;
                        }

                        gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, x, y, columnWidth, rowHeight);
                        gfx.DrawString(
                            column.HeaderText,
                            headerFont,
                            XBrushes.Black,
                            new XRect(x + 2, y + 3, columnWidth - 4, rowHeight - 6),
                            XStringFormats.TopLeft
                        );

                        x += columnWidth;
                    }

                    y += rowHeight;
                }

                x = margin;

                foreach (DataGridViewColumn column in gridData.Columns)
                {
                    if (!column.Visible)
                    {
                        continue;
                    }

                    string cellText = row.Cells[column.Index].Value?.ToString() ?? string.Empty;

                    gfx.DrawRectangle(XPens.Black, x, y, columnWidth, rowHeight);
                    gfx.DrawString(
                        cellText,
                        cellFont,
                        XBrushes.Black,
                        new XRect(x + 2, y + 3, columnWidth - 4, rowHeight - 6),
                        XStringFormats.TopLeft
                    );

                    x += columnWidth;
                }

                y += rowHeight;
            }

            // Speichert das Dokument auf dem gewählten Pfad.
            document.Save(filePath);
        }

        private void btnShowTables_Click(object sender, EventArgs e)
        {
            // Öffnet oder schließt die linke Strukturleiste.
            if (leftPanel.Width == 0)
            {
                leftPanel.Width = 180;
                LoadDatabaseStructure();
            }
            else
            {
                leftPanel.Width = 0;
            }
        }

        private void LoadDatabaseStructure()
        {
            // Entfernt die bisherige Struktur aus dem TreeView.
            treeDatabase.Nodes.Clear();

            // Lädt alle Tabellen aus der SQLite-Datenbank.
            DataTable tables = _db.RunQuery(
                "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;"
            );

            foreach (DataRow tableRow in tables.Rows)
            {
                string tableName = tableRow["name"].ToString() ?? string.Empty;

                TreeNode tableNode = new TreeNode(tableName);

                // Lädt die Spalteninformationen des aktuellen Tisches.
                DataTable columns = _db.RunQuery($"PRAGMA table_info({tableName});");

                foreach (DataRow columnRow in columns.Rows)
                {
                    string columnName = columnRow["name"].ToString() ?? string.Empty;
                    string columnType = columnRow["type"].ToString() ?? string.Empty;

                    TreeNode columnNode = new TreeNode($"{columnName} ({columnType})");
                    tableNode.Nodes.Add(columnNode);
                }

                treeDatabase.Nodes.Add(tableNode);
            }
        }

        private void leftPanel_MouseDown(object sender, MouseEventArgs e)
        {
            // Startet das manuelle Ändern der Breite, wenn die rechte Kante des Panels angeklickt wird.
            if (e.Button == MouseButtons.Left && e.X >= leftPanel.Width - ResizeGripWidth)
            {
                _isResizingLeftPanel = true;
                _leftPanelResizeStartX = Cursor.Position.X;
                _leftPanelStartWidth = leftPanel.Width;
            }
        }

        private void leftPanel_MouseMove(object sender, MouseEventArgs e)
        {
            // Zeigt einen horizontalen Resize-Cursor an, wenn sich die Maus am rechten Rand befindet.
            if (!_isResizingLeftPanel)
            {
                leftPanel.Cursor = e.X >= leftPanel.Width - ResizeGripWidth
                    ? Cursors.VSplit
                    : Cursors.Default;

                return;
            }

            // Berechnet die neue Breite anhand der Mausbewegung.
            int delta = Cursor.Position.X - _leftPanelResizeStartX;
            int newWidth = _leftPanelStartWidth + delta;

            // Begrenzt die minimale und maximale Breite des linken Panels.
            if (newWidth < LeftPanelMinWidth)
            {
                newWidth = LeftPanelMinWidth;
            }

            if (newWidth > LeftPanelMaxWidth)
            {
                newWidth = LeftPanelMaxWidth;
            }

            leftPanel.Width = newWidth;
        }

        private void leftPanel_MouseUp(object sender, MouseEventArgs e)
        {
            // Beendet den Resize-Vorgang nach dem Loslassen der linken Maustaste.
            if (e.Button == MouseButtons.Left)
            {
                _isResizingLeftPanel = false;
            }
        }

        private void btnRunQuery_MouseEnter(object sender, EventArgs e)
        {
            // Dunklere Hover-Farbe für den SQL-Button.
            btnRunQuery.BackColor = Color.FromArgb(29, 78, 216);
        }

        private void btnRunQuery_MouseLeave(object sender, EventArgs e)
        {
            // Standardfarbe für den SQL-Button.
            btnRunQuery.BackColor = Color.FromArgb(37, 99, 235);
        }

        private void btnExportPdf_MouseEnter(object sender, EventArgs e)
        {
            // Dunklere Hover-Farbe für den Export-Button.
            btnExportPdf.BackColor = Color.FromArgb(5, 150, 105);
        }

        private void btnExportPdf_MouseLeave(object sender, EventArgs e)
        {
            // Standardfarbe für den Export-Button.
            btnExportPdf.BackColor = Color.FromArgb(16, 185, 129);
        }

        private void txtSqlQuery_KeyUp(object sender, KeyEventArgs e)
        {
            // Reagiert nur bei Tasten, die normalerweise ein Wort beenden.
            if (e.KeyCode != Keys.Space && e.KeyCode != Keys.Enter && e.KeyCode != Keys.Tab)
            {
                return;
            }

            int caretPosition = txtSqlQuery.SelectionStart;
            string text = txtSqlQuery.Text;

            if (string.IsNullOrWhiteSpace(text) || caretPosition == 0)
            {
                return;
            }

            // Bestimmt den Text vor dem Cursor.
            string textBeforeCaret = text.Substring(0, caretPosition);

            // Entfernt das letzte Trennzeichen, damit das letzte Wort geprüft werden kann.
            string trimmedText = textBeforeCaret.TrimEnd(' ', '\r', '\n', '\t');

            if (string.IsNullOrWhiteSpace(trimmedText))
            {
                return;
            }

            // Sucht den Beginn des letzten Wortes.
            int lastSeparatorIndex = Math.Max(
                trimmedText.LastIndexOf(' '),
                Math.Max(trimmedText.LastIndexOf('\n'), trimmedText.LastIndexOf('\t'))
            );

            string lastWord = trimmedText.Substring(lastSeparatorIndex + 1);

            // Wandelt das Wort nur um, wenn es ein SQL-Keyword ist.
            if (!_sqlKeywords.Contains(lastWord.ToLower()))
            {
                return;
            }

            string upperWord = lastWord.ToUpper();

            // Wenn das Wort bereits groß ist, ist keine Änderung nötig.
            if (lastWord == upperWord)
            {
                return;
            }

            int wordStart = trimmedText.Length - lastWord.Length;

            // Ersetzt das letzte Wort durch die Großbuchstaben-Version.
            string newTextBeforeCaret = trimmedText.Substring(0, wordStart) + upperWord;

            // Fügt die ursprünglichen Trennzeichen wieder an.
            string trailingPart = textBeforeCaret.Substring(trimmedText.Length);
            string newFullText = newTextBeforeCaret + trailingPart + text.Substring(caretPosition);

            txtSqlQuery.Text = newFullText;
            txtSqlQuery.SelectionStart = caretPosition;
        }

        //PDF Automatic Title
        private string GenerateReportTitleFromSql(string sql)
        {
            // Erstellt einen Standardtitel, falls keine sinnvolle Struktur erkannt wird.
            const string defaultTitle = "SQL_Report";

            if (string.IsNullOrWhiteSpace(sql))
            {
                return defaultTitle;
            }

            string normalizedSql = sql.Trim();

            List<string> tableNames = new List<string>();

            // Liest die Haupttabelle nach FROM aus.
            Match fromMatch = Regex.Match(
                normalizedSql,
                @"\bFROM\s+([A-Za-z_][A-Za-z0-9_]*)",
                RegexOptions.IgnoreCase
            );

            if (fromMatch.Success)
            {
                tableNames.Add(fromMatch.Groups[1].Value);
            }

            // Liest alle Tabellen nach JOIN aus.
            MatchCollection joinMatches = Regex.Matches(
                normalizedSql,
                @"\bJOIN\s+([A-Za-z_][A-Za-z0-9_]*)",
                RegexOptions.IgnoreCase
            );

            foreach (Match match in joinMatches)
            {
                string tableName = match.Groups[1].Value;

                if (!tableNames.Contains(tableName, StringComparer.OrdinalIgnoreCase))
                {
                    tableNames.Add(tableName);
                }
            }

            bool hasWhere = Regex.IsMatch(normalizedSql, @"\bWHERE\b", RegexOptions.IgnoreCase);
            bool hasOrderBy = Regex.IsMatch(normalizedSql, @"\bORDER\s+BY\b", RegexOptions.IgnoreCase);
            bool hasGroupBy = Regex.IsMatch(normalizedSql, @"\bGROUP\s+BY\b", RegexOptions.IgnoreCase);

            bool hasAggregation =
                Regex.IsMatch(normalizedSql, @"\bCOUNT\s*\(", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(normalizedSql, @"\bSUM\s*\(", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(normalizedSql, @"\bAVG\s*\(", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(normalizedSql, @"\bMIN\s*\(", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(normalizedSql, @"\bMAX\s*\(", RegexOptions.IgnoreCase);

            string baseTitle;

            if (tableNames.Count > 0)
            {
                baseTitle = string.Join("_", tableNames);
            }
            else
            {
                baseTitle = "Abfrage";
            }

            // Baut den Titel abhängig von der Art der SQL-Abfrage.
            if (hasAggregation || hasGroupBy)
            {
                baseTitle = "Statistik_" + baseTitle;
            }
            else
            {
                baseTitle = "Bericht_" + baseTitle;
            }

            if (hasWhere)
            {
                baseTitle += "_mit_Filter";
            }

            if (hasOrderBy)
            {
                baseTitle += "_sortiert";
            }

            // Entfernt unzulässige Zeichen für Dateinamen.
            foreach (char invalidChar in Path.GetInvalidFileNameChars())
            {
                baseTitle = baseTitle.Replace(invalidChar.ToString(), string.Empty);
            }

            return string.IsNullOrWhiteSpace(baseTitle) ? defaultTitle : baseTitle;
        }

        private void lblReportTitle_Click(object sender, EventArgs e)
        {

        }
    }
}