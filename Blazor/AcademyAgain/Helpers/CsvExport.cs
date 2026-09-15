using System.Text;

namespace AcademyAgain.Helpers
{
    public static class CsvExport
    {
        public static string Escape(string? value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            return value;
        }

        public static string Build(IEnumerable<IEnumerable<string?>> rows)
        {
            var sb = new StringBuilder();
            sb.Append('\uFEFF');
            foreach (var row in rows)
            {
                sb.AppendLine(string.Join(",", row.Select(Escape)));
            }
            return sb.ToString();
        }
    }
}