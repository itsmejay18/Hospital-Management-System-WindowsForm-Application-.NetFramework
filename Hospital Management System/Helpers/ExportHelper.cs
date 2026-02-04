using System.Data;
using System.IO;
using System.Windows.Forms;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace HospitalManagementSystem.Helpers
{
    /// <summary>
    /// Provides export helpers for DataGridView.
    /// </summary>
    public static class ExportHelper
    {
        /// <summary>
        /// Exports a DataGridView to Excel.
        /// </summary>
        public static void ExportToExcel(DataGridView grid, string filePath)
        {
            var table = new DataTable();
            foreach (DataGridViewColumn col in grid.Columns)
            {
                table.Columns.Add(col.HeaderText);
            }

            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow) continue;
                var dataRow = table.NewRow();
                for (var i = 0; i < grid.Columns.Count; i++)
                {
                    dataRow[i] = row.Cells[i].Value;
                }
                table.Rows.Add(dataRow);
            }

            using (var workbook = new XLWorkbook())
            {
                workbook.Worksheets.Add(table, "Report");
                workbook.SaveAs(filePath);
            }
        }

        /// <summary>
        /// Exports a DataGridView to CSV.
        /// </summary>
        public static void ExportToCsv(DataGridView grid, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                for (var i = 0; i < grid.Columns.Count; i++)
                {
                    writer.Write(grid.Columns[i].HeaderText);
                    if (i < grid.Columns.Count - 1) writer.Write(",");
                }
                writer.WriteLine();

                foreach (DataGridViewRow row in grid.Rows)
                {
                    if (row.IsNewRow) continue;
                    for (var i = 0; i < grid.Columns.Count; i++)
                    {
                        writer.Write(row.Cells[i].Value);
                        if (i < grid.Columns.Count - 1) writer.Write(",");
                    }
                    writer.WriteLine();
                }
            }
        }

        /// <summary>
        /// Exports a DataGridView to PDF.
        /// </summary>
        public static void ExportToPdf(DataGridView grid, string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                var doc = new Document();
                PdfWriter.GetInstance(doc, stream);
                doc.Open();

                var table = new PdfPTable(grid.Columns.Count);
                foreach (DataGridViewColumn column in grid.Columns)
                {
                    table.AddCell(new Phrase(column.HeaderText));
                }

                foreach (DataGridViewRow row in grid.Rows)
                {
                    if (row.IsNewRow) continue;
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        table.AddCell(new Phrase(cell.Value?.ToString() ?? string.Empty));
                    }
                }

                doc.Add(table);
                doc.Close();
            }
        }
    }
}
