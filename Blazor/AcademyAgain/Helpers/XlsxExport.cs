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
            sheet.Columns().AdjustToContents();
            sheet.RangeUsed()?.SetAutoFilter();

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }
    }
}