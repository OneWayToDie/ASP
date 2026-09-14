using System;
using System.Linq;

namespace AcademyAgain.Helpers
{
    public static class Weekdays
    {
        public const int Monday = 1;
        public const int Tuesday = 2;
        public const int Wednesday = 4;
        public const int Thursday = 8;
        public const int Friday = 16;
        public const int Saturday = 32;
        public const int Sunday = 64;

        public static readonly (int Bit, string Name)[] All = new[]
        {
            (Monday, "Пн"), (Tuesday, "Вт"), (Wednesday, "Ср"), (Thursday, "Чт"),
            (Friday, "Пт"), (Saturday, "Сб"), (Sunday, "Вс")
        };

        public static bool HasDay(int? mask, int bit)
            => mask != null && (mask.Value & bit) == bit;

        public static string ToDisplay(int? mask)
        {
            if (mask == null || mask.Value == 0)
                return "—";
            return string.Join(", ", All.Where(d => HasDay(mask, d.Bit)).Select(d => d.Name));
        }
    }

    public static class Photo
    {
        public static string? DataSrc(byte[]? bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return null;

            string? mime = null;
            if (bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
                mime = "image/jpeg";
            else if (bytes.Length >= 8 && bytes[0] == 0x89 && bytes[1] == 0x50
                     && bytes[2] == 0x4E && bytes[3] == 0x47)
                mime = "image/png";

            return mime is null ? null : $"data:{mime};base64,{Convert.ToBase64String(bytes)}";
        }
    }
}