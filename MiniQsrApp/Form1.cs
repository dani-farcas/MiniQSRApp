using MiniQsrApp.Data;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace MiniQsrApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MiniQsr.db");

            var db = new DatabaseHelper(dbPath);

            db.InitializeDatabase();
            //db.InsertTestData();

            //gridData.DataSource = db.GetData();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void gridData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnRunQuery_Click(object sender, EventArgs e)
        {
            try
            {
                string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MiniQsr.db");

                var db = new DatabaseHelper(dbPath);

                string sql = txtSqlQuery.Text.Trim();

                if (sql.ToUpper().StartsWith("SELECT"))
                {
                    gridData.DataSource = db.RunQuery(sql);
                }
                else
                {
                    db.ExecuteCommand(sql);
                    gridData.DataSource = db.GetData();
                    MessageBox.Show("Befehl ausgeführt!");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL Fehler: " + ex.Message);
            }
        }

        private void txtSqlQuery_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnRunQuery_MouseEnter(object sender, EventArgs e)
        {
            btnRunQuery.BackColor = Color.FromArgb(29, 78, 216);
        }

        private void btnRunQuery_MouseLeave(object sender, EventArgs e)
        {
            btnRunQuery.BackColor = Color.FromArgb(37, 99, 235);
        }

        private void btnRunQuery_MouseEnter_1(object sender, EventArgs e)
        {

        }

        private void btnRunQuery_MouseLeave_1(object sender, EventArgs e)
        {

        }

        private void btnExportPdf_MouseEnter(object sender, EventArgs e)
        {
            btnExportPdf.BackColor = Color.FromArgb(5, 150, 105);
        }

        private void btnExportPdf_MouseLeave(object sender, EventArgs e)
        {
            btnExportPdf.BackColor = Color.FromArgb(16, 185, 129);
        }

        private void btnExportPdf_MouseEnter_1(object sender, EventArgs e)
        {

        }

        private void btnExportPdf_MouseLeave_1(object sender, EventArgs e)
        {

        }

        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            if (gridData.Rows.Count == 0 || gridData.Columns.Count == 0) 
            {
                MessageBox.Show("Es gibt keine Daten zum Exportieren.");
                return;
            }
            using(SaveFileDialog saveFileDialog = new SaveFileDialog()) 
            {
                saveFileDialog.Filter = "PDF-Datei (*.pdf) | *.pdf";
                saveFileDialog.Title = "PDF speichern";
                saveFileDialog.FileName = "SQL_Report.pdf";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportDataGridViewToPdf(saveFileDialog.FileName);
                    MessageBox.Show("PDF wurde erfolgreich exportiert.");
                }
            }
        }

        private void ExportDataGridViewToPdf(string filePath)
        {
            PdfDocument document = new PdfDocument();
            document.Info.Title = "SQL Report";

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            XFont titleFont = new XFont("Arial", 14);
            XFont headerFont = new XFont("Arial", 10);
            XFont cellFont = new XFont("Arial", 9);

            double margin = 40;
            double y = margin;
            double rowHeight = 20;
            double tableWidth = page.Width - 2 * margin;

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

            gfx.DrawString("SQL-Abfrage Ergebnis", titleFont, XBrushes.Black,
                new XRect(margin, y, tableWidth, rowHeight),
                XStringFormats.TopLeft);

            y += 30;

            double x = margin;

            foreach (DataGridViewColumn column in gridData.Columns)
            {
                if (!column.Visible)
                {
                    continue;
                }

                gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, x, y, columnWidth, rowHeight);
                gfx.DrawString(column.HeaderText, headerFont, XBrushes.Black,
                    new XRect(x + 2, y + 3, columnWidth - 4, rowHeight - 6),
                    XStringFormats.TopLeft);

                x += columnWidth;
            }

            y += rowHeight;

            foreach (DataGridViewRow row in gridData.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

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
                        gfx.DrawString(column.HeaderText, headerFont, XBrushes.Black,
                            new XRect(x + 2, y + 3, columnWidth - 4, rowHeight - 6),
                            XStringFormats.TopLeft);

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
                    gfx.DrawString(cellText, cellFont, XBrushes.Black,
                        new XRect(x + 2, y + 3, columnWidth - 4, rowHeight - 6),
                        XStringFormats.TopLeft);

                    x += columnWidth;
                }

                y += rowHeight;
            }

            document.Save(filePath);
        }
    }
}
