# Prepares high-resolution wallpaper variants for the fullscreen background engine.
# Reads   : wwwroot/wallpapers/<genre>/*  (all image formats except animated .gif)
# Writes  : wwwroot/wallpapers2560/<genre>/<stem>.jpg
#          Long side is scaled to $TargetPx (never enlarged beyond a 1.0 factor),
#          aspect ratio is preserved, JPEG quality $JpegQuality.
# Run again after adding new wallpapers:  powershell -File tools\prepare-wallpapers.ps1
[CmdletBinding()]
param(
    [int]$TargetPx = 2560,
    [int]$JpegQuality = 90
)

$ErrorActionPreference = 'Stop'
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
Add-Type -AssemblyName System.Drawing

$root = Split-Path -Parent $PSScriptRoot
$src  = Join-Path $root 'wwwroot\wallpapers'
$dst  = Join-Path $root 'wwwroot\wallpapers2560'

if (-not (Test-Path -LiteralPath $src)) { Write-Error "Source folder not found: $src"; exit 1 }

$jpegCodec = [System.Drawing.Imaging.ImageCodecInfo]::GetImageEncoders() | Where-Object { $_.MimeType -eq 'image/jpeg' }
$encParams = New-Object System.Drawing.Imaging.EncoderParameters(1)
$encParams.Param[0] = New-Object System.Drawing.Imaging.EncoderParameter([System.Drawing.Imaging.Encoder]::Quality, [long]$JpegQuality)

$converted = 0; $skipped = 0; $failed = 0

Get-ChildItem -LiteralPath $src -Directory | ForEach-Object {
    $genre = $_.Name
    $outDir = Join-Path $dst $genre
    Get-ChildItem -LiteralPath $_.FullName -File | Where-Object { $_.Extension -ne '.gif' } | ForEach-Object {
        $file = $_
        try {
            $fs = [System.IO.File]::OpenRead($file.FullName)
            $img = $null
            try { $img = [System.Drawing.Image]::FromStream($fs) }
            finally { $fs.Dispose() }

            if ($null -eq $img) { $skipped++; return }

            $scale = [double]$TargetPx / [Math]::Max($img.Width, $img.Height)
            $nw = [Math]::Max(1, [int][Math]::Round($img.Width * $scale))
            $nh = [Math]::Max(1, [int][Math]::Round($img.Height * $scale))

            $bmp = New-Object System.Drawing.Bitmap($nw, $nh)
            try {
                $g = [System.Drawing.Graphics]::FromImage($bmp)
                try {
                    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
                    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
                    $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
                    $g.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
                    $g.DrawImage($img, (New-Object System.Drawing.Rectangle(0, 0, $nw, $nh)), 0, 0, $img.Width, $img.Height, [System.Drawing.GraphicsUnit]::Pixel)
                } finally { $g.Dispose() }

                New-Item -ItemType Directory -Force -Path $outDir | Out-Null
                $outName = [System.IO.Path]::ChangeExtension($file.Name, '.jpg')
                $bmp.Save((Join-Path $outDir $outName), $jpegCodec, $encParams)
                $converted++
            } finally { $bmp.Dispose() }
            $img.Dispose()
        }
        catch {
            $failed++
            Write-Output ("FAIL $genre / $($file.Name) : $($_.Exception.Message)")
        }
    }
}

Write-Output ("--- done: converted=$converted skipped=$skipped failed=$failed ---")