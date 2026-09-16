using ClosedXML.Excel;

namespace AcademyAgain.Helpers
{
    public static class XlsxExport
    {
        public static byte[] Build(IReadOnlyList<string?[]> rows)
        {
            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Данные");

            for (int r = 0; r < rows.Count; r++)
            {
                for (int c = 0; c < rows[r].Length; c++)
                {
                    sheet.Cell(r + 1, c + 1).Value = rows[r][c] ?? "";
                }
            }

            sheet.SheetView.FreezeRows(1);
            ApplyColumnWidths(sheet, rows);
            sheet.RangeUsed()?.SetAutoFilter();

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }

        private static void ApplyColumnWidths(IXLWorksheet sheet, IReadOnlyList<string?[]> rows)
        {
            int columnCount = 0;
            foreach (var row in rows)
            {
                columnCount = Math.Max(columnCount, row.Length);
            }

            for (int c = 0; c < columnCount; c++)
            {
                int maxLength = 0;
                foreach (var row in rows)
                {
                    if (c < row.Length && row[c] is { Length: > 1 })
                    {
                        maxLength = Math.Max(maxLength, row[c]!.Length);
                    }
                }
                sheet.Column(c + 1).Width = Math.Clamp(maxLength + 2, 6, 50);
            }
        }
    }
}