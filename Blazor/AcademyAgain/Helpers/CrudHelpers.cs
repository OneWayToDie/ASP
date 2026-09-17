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
            // JPEG: FF D8 FF
            if (bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
                mime = "image/jpeg";
            // PNG: 89 50 4E 47
            else if (bytes.Length >= 8 && bytes[0] == 0x89 && bytes[1] == 0x50
                     && bytes[2] == 0x4E && bytes[3] == 0x47)
                mime = "image/png";
            // WebP: RIFF????WEBP
            else if (bytes.Length >= 12 && bytes[0] == 0x52 && bytes[1] == 0x49
                     && bytes[2] == 0x46 && bytes[3] == 0x46
                     && bytes[8] == 0x57 && bytes[9] == 0x45
                     && bytes[10] == 0x42 && bytes[11] == 0x50)
                mime = "image/webp";
            // GIF: GIF87a / GIF89a
            else if (bytes.Length >= 6 && bytes[0] == 0x47 && bytes[1] == 0x49
                     && bytes[2] == 0x46 && bytes[3] == 0x38
                     && (bytes[4] == 0x37 || bytes[4] == 0x39) && bytes[5] == 0x61)
                mime = "image/gif";
            // BMP: BM
            else if (bytes.Length >= 2 && bytes[0] == 0x42 && bytes[1] == 0x4D)
                mime = "image/bmp";
            // ICO: 00 00 01 00
            else if (bytes.Length >= 4 && bytes[0] == 0x00 && bytes[1] == 0x00
                     && bytes[2] == 0x01 && bytes[3] == 0x00)
                mime = "image/x-icon";

            return mime is null ? null : $"data:{mime};base64,{Convert.ToBase64String(bytes)}";
        }
    }

    public static class Qr
    {
        public static string? PngDataUri(string content, int pixelsPerModule = 8)
        {
            try
            {
                using var generator = new QRCoder.QRCodeGenerator();
                using var qrData = generator.CreateQrCode(content, QRCoder.QRCodeGenerator.ECCLevel.Q);
                using var png = new QRCoder.PngByteQRCode(qrData);
                var bytes = png.GetGraphic(pixelsPerModule);
                return $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
            }
            catch
            {
                return null;
            }
        }
    }
}