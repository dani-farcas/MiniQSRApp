using MiniQsrApp.Data;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace MiniQsrApp
{
    public partial class Form1 : Form
    {
        #region Constants

        private const int LeftPanelMinWidth = 120;
        private const int LeftPanelMaxWidth = 450;
        private const int ResizeGripWidth = 6;

        #endregion Constants

        #region Private Fields

        private readonly string _dbPath;
        private readonly DatabaseHelper _db;

        private readonly HashSet<string> _sqlKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "select", "from", "where", "insert", "into", "values", "update", "set", "delete",
            "create", "table", "drop", "alter", "join", "left", "right", "inner", "outer",
            "on", "group", "by", "order", "having", "as", "and", "or", "not", "null", "is",
            "in", "like", "distinct", "limit", "pragma", "asc", "desc", "between", "exists",
            "case", "when", "then", "else", "end", "sum", "avg", "min", "max", "count",
            "union", "all", "any", "some", "glob", "match", "regexp", "escape", "with",
            "recursive", "without", "rowid", "if", "index", "view", "trigger", "begin",
            "transaction", "commit", "rollback", "vacuum", "analyze", "explain", "query",
            "plan", "attach", "detach", "reindex", "savepoint", "release", "rollback to",
            "coalesce", "ifnull", "nullif", "typeof", "quote", "random", "randomblob",
            "last_insert_rowid"
        };

        private bool _isResizingLeftPanel;
        private int _leftPanelResizeStartX;
        private int _leftPanelStartWidth;

        private List<string> _tableNames = new();
        private List<string> _columnNames = new();

        private static readonly HashSet<string> CurrencyColumnNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "Balance",
            "Amount",
            "TotalBalance",
            "AverageBalance",
            "TotalAmount",
            "Gesamte Guthaben",
            "Durchschnittliches Guthaben",
            "Umsatz"
        };

        #endregion Private Fields

        #region Constructors

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Initialisiert das Hauptformular, die Datenbank und die grundlegenden UI-Einstellungen.
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MiniQsr.db");
            _db = new DatabaseHelper(_dbPath);

            InitializeApplication();
            RegisterEvents();
        }

        #endregion Constructors

        #region Initialization

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Initialisiert die Anwendung und bereitet die Startansicht vor.
        /// </summary>
        private void InitializeApplication()
        {
            _db.InitializeDatabase();

            leftPanel.Width = 0;
            lblReportTitle.Visible = false;

            InitializeButtonStyles();
            InitializeGridStyles();
            LoadSqlSuggestionData();
        }

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Registriert alle benötigten UI-Ereignisse.
        /// </summary>
        private void RegisterEvents()
        {
            leftPanel.MouseDown += leftPanel_MouseDown;
            leftPanel.MouseMove += leftPanel_MouseMove;
            leftPanel.MouseUp += leftPanel_MouseUp;

            txtSqlQuery.KeyUp += txtSqlQuery_KeyUp;
            txtSqlQuery.KeyDown += txtSqlQuery_KeyDown;

            lstSqlSuggestions.DoubleClick += lstSqlSuggestions_DoubleClick;
            lstSqlSuggestions.KeyDown += lstSqlSuggestions_KeyDown;
        }

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Initialisiert das Styling der Schaltflächen.
        /// </summary>
        private void InitializeButtonStyles()
        {
            ConfigureButton(btnRunQuery, Color.FromArgb(37, 99, 235));
            ConfigureButton(btnExportPdf, Color.FromArgb(16, 185, 129));
        }

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Initialisiert die Standarddarstellung des Datenrasters.
        /// </summary>
        private void InitializeGridStyles()
        {
            gridData.AutoGenerateColumns = true;
            gridData.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridData.MultiSelect = false;
            gridData.ReadOnly = true;
            gridData.AllowUserToAddRows = false;
            gridData.AllowUserToDeleteRows = false;
            gridData.AllowUserToResizeRows = false;
            gridData.RowHeadersVisible = false;
            gridData.BackgroundColor = Color.White;
            gridData.BorderStyle = BorderStyle.None;
            gridData.EnableHeadersVisualStyles = false;

            gridData.ColumnHeadersDefaultCellStyle.BackColor = Color.Gainsboro;
            gridData.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            gridData.ColumnHeadersDefaultCellStyle.Font = new Font(gridData.Font, FontStyle.Bold);
        }

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Wendet ein einheitliches Standarddesign auf eine Schaltfläche an.
        /// </summary>
        /// <param name="button">Zu formatierende Schaltfläche.</param>
        /// <param name="color">Hintergrundfarbe der Schaltfläche.</param>
        private void ConfigureButton(Button button, Color color)
        {
            button.BackColor = color;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;
        }

        #endregion Initialization

        #region Data Execution

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Führt eine SELECT-Abfrage aus und zeigt das Ergebnis im Grid an.
        /// </summary>
        /// <param name="sql">Auszuführende SELECT-Abfrage.</param>
        private void ExecuteSelect(string sql)
        {
            try
            {
                gridData.DataSource = null;

                DataTable result = _db.RunQuery(sql);

                BindDataToGrid(result);

                lblReportTitle.Text = GenerateReportTitleFromSql(sql);
                lblReportTitle.Visible = true;
            }
            catch
            {
                gridData.DataSource = null;
                lblReportTitle.Visible = false;
                throw;
            }
        }

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Führt eine nicht-selektierende SQL-Anweisung aus und lädt die Standarddaten neu.
        /// </summary>
        /// <param name="sql">Auszuführende SQL-Anweisung.</param>
        private void ExecuteNonSelect(string sql)
        {
            _db.ExecuteCommand(sql);

            DataTable result = _db.GetData();
            BindDataToGrid(result);

            lblReportTitle.Visible = false;

            MessageBox.Show("Befehl wurde erfolgreich ausgeführt.");
        }

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Bindet Daten kontrolliert an das Grid und aktualisiert die Anzeige.
        /// </summary>
        /// <param name="data">Anzuzeigende Daten.</param>
        private void BindDataToGrid(DataTable data)
        {
            ResetGridBeforeBinding();

            gridData.DataSource = data;

            ApplyCurrencyFormatting();

            gridData.Refresh();
            gridData.ClearSelection();
        }

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Formatiert alle definierten Währungsspalten im deutschen Format.
        /// </summary>
        private void ApplyCurrencyFormatting()
        {
            CultureInfo germanCulture = new CultureInfo("de-DE");
            germanCulture.NumberFormat.CurrencySymbol = "€";

            foreach (DataGridViewColumn column in gridData.Columns)
            {
                if (!IsCurrencyColumn(column))
                    continue;

                column.DefaultCellStyle.Format = "C2";
                column.DefaultCellStyle.FormatProvider = germanCulture;
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                column.DefaultCellStyle.NullValue = "0,00 €";
            }
        }

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Setzt das Grid vor einem neuen Datenbinding sauber zurück.
        /// </summary>
        private void ResetGridBeforeBinding()
        {
            gridData.DataSource = null;
            gridData.Columns.Clear();
            gridData.AutoGenerateColumns = true;
        }

        #endregion Data Execution


        #region PDF Export

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Exportiert die aktuell angezeigten Grid-Daten als PDF-Datei.
        /// </summary>
        /// <param name="filePath">Zielpfad der PDF-Datei.</param>
        /// <param name="title">Titel des Reports.</param>
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

            gfx.DrawString(title, titleFont, XBrushes.Black, new XRect(margin, y, page.Width, rowHeight));
            y += 30;

            List<DataGridViewColumn> visibleColumns = gridData.Columns
                .Cast<DataGridViewColumn>()
                .Where(column => column.Visible)
                .ToList();

            if (visibleColumns.Count == 0)
            {
                return;
            }

            double colWidth = (page.Width - 2 * margin) / visibleColumns.Count;
            double x = margin;

            foreach (DataGridViewColumn column in visibleColumns)
            {
                gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, x, y, colWidth, rowHeight);
                gfx.DrawString(column.HeaderText, header, XBrushes.Black, new XRect(x + 2, y + 2, colWidth, rowHeight));
                x += colWidth;
            }

            y += rowHeight;

            foreach (DataGridViewRow row in gridData.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                x = margin;

                foreach (DataGridViewColumn column in visibleColumns)
                {
                    string text = row.Cells[column.Index].Value?.ToString() ?? string.Empty;

                    gfx.DrawRectangle(XPens.Black, x, y, colWidth, rowHeight);
                    gfx.DrawString(text, font, XBrushes.Black, new XRect(x + 2, y + 2, colWidth, rowHeight));

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

        #endregion PDF Export

        #region Database Structure

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Lädt die Tabellenstruktur der Datenbank in die TreeView.
        /// </summary>
        private void LoadStructure()
        {
            treeDatabase.Nodes.Clear();

            DataTable tables = _db.RunQuery("SELECT name FROM sqlite_master WHERE type='table' ORDER BY name");

            foreach (DataRow tableRow in tables.Rows)
            {
                string tableName = tableRow["name"]?.ToString() ?? string.Empty;
                TreeNode tableNode = new(tableName);

                DataTable columns = _db.RunQuery($"PRAGMA table_info({tableName})");

                foreach (DataRow columnRow in columns.Rows)
                {
                    string columnName = columnRow["name"]?.ToString() ?? string.Empty;
                    string columnType = columnRow["type"]?.ToString() ?? string.Empty;

                    tableNode.Nodes.Add($"{columnName} ({columnType})");
                }

                treeDatabase.Nodes.Add(tableNode);
            }
        }

        #endregion Database Structure

        #region Report Helpers

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Erstellt einen Reporttitel basierend auf dem verwendeten SQL-Statement.
        /// </summary>
        /// <param name="sql">SQL-Text zur Ermittlung des Reportnamens.</param>
        /// <returns>Generierter Reporttitel.</returns>
        private string GenerateReportTitleFromSql(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
            {
                return "SQL_Report";
            }

            Match match = Regex.Match(sql, @"FROM\s+(\w+)", RegexOptions.IgnoreCase);
            string tableName = match.Success ? match.Groups[1].Value : "Report";

            return $"Bericht_{tableName}";
        }

        #endregion Report Helpers

        #region SQL Autocomplete

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Lädt alle verfügbaren Tabellen- und Spaltennamen für die SQL-Autovervollständigung.
        /// </summary>
        private void LoadSqlSuggestionData()
        {
            _tableNames = _db.GetTables();
            _columnNames = _db.GetAllColumns();
        }

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Analysiert die aktuelle Cursorposition und zeigt passende SQL-Vorschläge an.
        /// </summary>
        private void ShowSqlSuggestions()
        {
            int caretPosition = txtSqlQuery.SelectionStart;
            string textBeforeCursor = txtSqlQuery.Text[..caretPosition];

            Match match = Regex.Match(textBeforeCursor, @"(\w+)$");

            if (!match.Success)
            {
                lstSqlSuggestions.Visible = false;
                return;
            }

            string currentWord = match.Groups[1].Value;

            if (string.IsNullOrWhiteSpace(currentWord))
            {
                lstSqlSuggestions.Visible = false;
                return;
            }

            List<string> source = GetSuggestionSource(textBeforeCursor);

            List<string> matches = source
                .Where(item => item.StartsWith(currentWord, StringComparison.OrdinalIgnoreCase))
                .OrderBy(item => item)
                .ToList();

            if (matches.Count == 0)
            {
                lstSqlSuggestions.Visible = false;
                return;
            }

            lstSqlSuggestions.Items.Clear();

            foreach (string item in matches)
            {
                lstSqlSuggestions.Items.Add(item);
            }

            lstSqlSuggestions.SelectedIndex = 0;
            lstSqlSuggestions.Width = 220;
            lstSqlSuggestions.Height = Math.Min(140, matches.Count * 22 + 4);
            lstSqlSuggestions.Left = txtSqlQuery.Left;
            lstSqlSuggestions.Top = txtSqlQuery.Bottom + 4;
            lstSqlSuggestions.Visible = true;
            lstSqlSuggestions.BringToFront();
        }

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Ermittelt anhand des bisherigen SQL-Texts, ob Tabellen- oder Spaltenvorschläge angezeigt werden sollen.
        /// </summary>
        /// <param name="textBeforeCursor">SQL-Text vor der Cursorposition.</param>
        /// <returns>Passende Vorschlagsquelle.</returns>
        private List<string> GetSuggestionSource(string textBeforeCursor)
        {
            string upperText = textBeforeCursor.ToUpperInvariant();

            if (Regex.IsMatch(upperText, @"\b(FROM|JOIN|UPDATE|INTO)\s+\w*$"))
            {
                return _tableNames;
            }

            if (Regex.IsMatch(upperText, @"\b(SELECT|WHERE|ON|AND|OR)\s+[\w\.]*$"))
            {
                return _columnNames;
            }

            return _tableNames;
        }

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Ersetzt das aktuell geschriebene Wort durch den ausgewählten SQL-Vorschlag.
        /// </summary>
        private void InsertSelectedSuggestionSmart()
        {
            if (lstSqlSuggestions.SelectedItem == null)
            {
                return;
            }

            string suggestion = lstSqlSuggestions.SelectedItem.ToString() ?? string.Empty;

            int caretPosition = txtSqlQuery.SelectionStart;
            string text = txtSqlQuery.Text;

            Match match = Regex.Match(text[..caretPosition], @"(\w+)$");

            if (!match.Success)
            {
                return;
            }

            int wordStart = match.Index;
            string newText = text[..wordStart] + suggestion + text[caretPosition..];

            txtSqlQuery.Text = newText;
            txtSqlQuery.SelectionStart = wordStart + suggestion.Length;
            txtSqlQuery.Focus();

            lstSqlSuggestions.Visible = false;
        }

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Fügt den ausgewählten Vorschlag am Ende des SQL-Textes ein.
        /// </summary>
        private void InsertSelectedSuggestion()
        {
            if (lstSqlSuggestions.SelectedItem == null)
            {
                return;
            }

            string suggestion = lstSqlSuggestions.SelectedItem.ToString() ?? string.Empty;

            txtSqlQuery.Text += suggestion + " ";
            txtSqlQuery.Focus();
            lstSqlSuggestions.Visible = false;
        }

        #endregion SQL Autocomplete

        #region SQL Formatting

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Formatiert die SQL-Abfrage, indem bekannte Schlüsselwörter in Großbuchstaben umgewandelt werden.
        /// </summary>
        /// <param name="sql">Zu formatierender SQL-Text.</param>
        /// <returns>Formatierter SQL-Text.</returns>
        private string FormatSql(string sql)
        {
            string[] tokens = Regex.Split(sql, @"(\W)");
            StringBuilder result = new();

            foreach (string token in tokens)
            {
                string cleanToken = token.Trim();

                if (_sqlKeywords.Contains(cleanToken))
                {
                    result.Append(cleanToken.ToUpperInvariant());
                }
                else
                {
                    result.Append(token);
                }
            }

            return result.ToString();
        }

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Formatiert den SQL-Text, ohne die aktuelle Cursorposition des Benutzers zu verändern.
        /// </summary>
        /// <param name="sql">Zu formatierender SQL-Text.</param>
        /// <param name="caretPosition">Aktuelle Cursorposition.</param>
        /// <param name="newCaretPosition">Neue Cursorposition nach der Formatierung.</param>
        /// <returns>Formatierter SQL-Text.</returns>
        private string FormatSqlPreserveCaret(string sql, int caretPosition, out int newCaretPosition)
        {
            MatchCollection matches = Regex.Matches(sql, @"\w+");
            StringBuilder result = new(sql);

            int offset = 0;
            newCaretPosition = caretPosition;

            foreach (Match match in matches)
            {
                string word = match.Value;

                if (!_sqlKeywords.Contains(word))
                {
                    continue;
                }

                string upperWord = word.ToUpperInvariant();

                if (word == upperWord)
                {
                    continue;
                }

                int startIndex = match.Index + offset;
                result.Remove(startIndex, word.Length);
                result.Insert(startIndex, upperWord);

                int difference = upperWord.Length - word.Length;
                offset += difference;

                if (startIndex < caretPosition)
                {
                    newCaretPosition += difference;
                }
            }

            return result.ToString();
        }

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Prüft, ob eine Spalte als Währungsfeld behandelt werden soll.
        /// Kombiniert manuelle Definitionen und automatische Erkennung.
        /// </summary>
        private bool IsCurrencyColumn(DataGridViewColumn column)
        {
            string name = column.Name;

            //  1. Manual override (de ex. "Gesamte Guthaben")
            if (CurrencyColumnNames.Contains(name))
                return IsNumeric(column);

            //  2. Naming convention (_EUR)
            if (name.EndsWith("", StringComparison.OrdinalIgnoreCase))
                return IsNumeric(column);

            //  2. Automatische Erkennung (technische Namen)
            if (name.Contains("Balance", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Amount", StringComparison.OrdinalIgnoreCase))
                return IsNumeric(column);

            return false;
        }

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Prüft, ob der Spaltentyp numerisch ist.
        /// </summary>
        private bool IsNumeric(DataGridViewColumn column)
        {
            return column.ValueType == typeof(decimal) ||
                   column.ValueType == typeof(double) ||
                   column.ValueType == typeof(float);
        }

        #endregion SQL Formatting

        #region Panel Resizing

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Startet die Größenänderung des linken Panels, wenn der Benutzer den Ziehbereich anklickt.
        /// </summary>
        /// <param name="sender">Quelle des Ereignisses.</param>
        /// <param name="e">Enthält Mausdaten.</param>
        private void leftPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X >= leftPanel.Width - ResizeGripWidth)
            {
                _isResizingLeftPanel = true;
                _leftPanelResizeStartX = Cursor.Position.X;
                _leftPanelStartWidth = leftPanel.Width;
            }
        }

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Ändert die Breite des linken Panels während des Ziehvorgangs.
        /// </summary>
        /// <param name="sender">Quelle des Ereignisses.</param>
        /// <param name="e">Enthält Mausdaten.</param>
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

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Beendet die Größenänderung des linken Panels.
        /// </summary>
        /// <param name="sender">Quelle des Ereignisses.</param>
        /// <param name="e">Enthält Mausdaten.</param>
        private void leftPanel_MouseUp(object sender, MouseEventArgs e)
        {
            _isResizingLeftPanel = false;
        }

        #endregion Panel Resizing

        #region Event Handlers

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Führt die eingegebene SQL-Abfrage aus und unterscheidet zwischen SELECT- und Nicht-SELECT-Anweisungen.
        /// </summary>
        /// <param name="sender">Quelle des Ereignisses.</param>
        /// <param name="e">Enthält Ereignisdaten.</param>
        private void btnRunQuery_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = txtSqlQuery.Text.Trim();
                sql = FormatSql(sql);
                txtSqlQuery.Text = sql;

                if (string.IsNullOrWhiteSpace(sql))
                {
                    MessageBox.Show("Bitte geben Sie eine SQL-Abfrage ein.");
                    return;
                }

                if (sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase) ||
                    sql.StartsWith("WITH", StringComparison.OrdinalIgnoreCase))
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

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Exportiert die aktuell angezeigten Daten als PDF-Datei.
        /// </summary>
        /// <param name="sender">Quelle des Ereignisses.</param>
        /// <param name="e">Enthält Ereignisdaten.</param>
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
            {
                return;
            }

            ExportPdf(dialog.FileName, title);

            MessageBox.Show("PDF exportiert.");
        }

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Blendet das linke Strukturpanel ein oder aus und lädt bei Bedarf die Datenbankstruktur.
        /// </summary>
        /// <param name="sender">Quelle des Ereignisses.</param>
        /// <param name="e">Enthält Ereignisdaten.</param>
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

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Wandelt erkannte SQL-Schlüsselwörter nach Eingabe automatisch in Großbuchstaben um.
        /// </summary>
        /// <param name="sender">Quelle des Ereignisses.</param>
        /// <param name="e">Enthält Tastaturdaten.</param>
        private void txtSqlQuery_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                int selectionStart = txtSqlQuery.SelectionStart;
                string originalText = txtSqlQuery.Text;

                string formattedText = FormatSqlPreserveCaret(originalText, selectionStart, out int newSelectionStart);

                if (formattedText != originalText)
                {
                    txtSqlQuery.Text = formattedText;
                    txtSqlQuery.SelectionStart = newSelectionStart;
                }
            }

            ShowSqlSuggestions();
        }

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Fängt Enter und Tab ab, um die ausgewählte Vorschlagsoption direkt in das SQL-Feld einzufügen.
        /// </summary>
        /// <param name="sender">Quelle des Ereignisses.</param>
        /// <param name="e">Enthält Tastaturdaten.</param>
        private void txtSqlQuery_KeyDown(object sender, KeyEventArgs e)
        {
            if (lstSqlSuggestions.Visible && lstSqlSuggestions.SelectedItem != null)
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    InsertSelectedSuggestionSmart();
                    e.SuppressKeyPress = true;
                }
            }
        }

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Fügt den ausgewählten Vorschlag per Doppelklick ein.
        /// </summary>
        /// <param name="sender">Quelle des Ereignisses.</param>
        /// <param name="e">Enthält Ereignisdaten.</param>
        private void lstSqlSuggestions_DoubleClick(object sender, EventArgs e)
        {
            InsertSelectedSuggestion();
        }

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Fügt den ausgewählten Vorschlag mit Enter ein.
        /// </summary>
        /// <param name="sender">Quelle des Ereignisses.</param>
        /// <param name="e">Enthält Tastaturdaten.</param>
        private void lstSqlSuggestions_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                InsertSelectedSuggestion();
                e.Handled = true;
            }
        }

        /// ----------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Platzhalter für ein Designer-Ereignis.
        /// </summary>
        /// <param name="sender">Quelle des Ereignisses.</param>
        /// <param name="e">Enthält Ereignisdaten.</param>
        private void lstSqlSuggestions_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        #endregion Event Handlers

    }
}