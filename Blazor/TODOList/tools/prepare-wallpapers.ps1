# Prepares high-resolution wallpaper variants for the fullscreen background engine.
# Reads   : wwwroot/wallpapers/<genre>/*  (all image formats except animated .gif)
# Writes  : wwwroot/wallpapers2560/<genre>/<stem>.jpg
#          Each image is center-cropped to 16:9, then resized to 2560x1440.
#          JPEG quality $JpegQuality.
# Also processes the default fallback image (wwwroot/image/default-source.jpg)
# into a 16:9 crop at wwwroot/image/default-bg-1440.jpg so a generic square
# wallpaper no longer gets stretched to an enormous crop on landscape screens.
# Run again after adding new wallpapers:  powershell -File tools\prepare-wallpapers.ps1
[CmdletBinding()]
param(
    [int]$OutWidth  = 2560,
    [int]$OutHeight = 1440,
    [double]$TargetAspect = 16.0 / 9.0,
    [int]$JpegQuality = 90
)

$ErrorActionPreference = 'Stop'
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
Add-Type -AssemblyName System.Drawing

$root = Split-Path -Parent $PSScriptRoot
$src  = Join-Path $root 'wwwroot\wallpapers'
$dst  = Join-Path $root 'wwwroot\wallpapers2560'
$defaultSrc = Join-Path $root 'wwwroot\image\4ede3d257070f23ecb212b899fc8bda6.jpg'
$defaultDst = Join-Path $root 'wwwroot\image\default-bg-1440.jpg'

if (-not (Test-Path -LiteralPath $src)) { Write-Error "Source folder not found: $src"; exit 1 }

$jpegCodec = [System.Drawing.Imaging.ImageCodecInfo]::GetImageEncoders() | Where-Object { $_.MimeType -eq 'image/jpeg' }
$encParams = New-Object System.Drawing.Imaging.EncoderParameters(1)
$encParams.Param[0] = New-Object System.Drawing.Imaging.EncoderParameter([System.Drawing.Imaging.Encoder]::Quality, [long]$JpegQuality)

$converted = 0; $skipped = 0; $failed = 0

function Process-Image {
    param([System.Drawing.Image]$img, [string]$outFullPath)

    $sw = $img.Width
    $sh = $img.Height
    $srcAspect = [double]$sw / $sh

    if ($srcAspect -lt $TargetAspect) {
        $cropW = $sw
        $cropH = [Math]::Round($sw / $TargetAspect)
        $cropX = 0
        $cropY = [int][Math]::Round(($sh - $cropH) / 2.0)
    } else {
        $cropH = $sh
        $cropW = [Math]::Round($sh * $TargetAspect)
        $cropX = [int][Math]::Round(($sw - $cropW) / 2.0)
        $cropY = 0
    }

    $cropRect = New-Object System.Drawing.Rectangle($cropX, $cropY, $cropW, $cropH)

    $bmp = New-Object System.Drawing.Bitmap($OutWidth, $OutHeight)
    try {
        $g = [System.Drawing.Graphics]::FromImage($bmp)
        try {
            $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
            $g.SmoothingMode    = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
            $g.PixelOffsetMode  = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
            $g.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
            $destRect = New-Object System.Drawing.Rectangle(0, 0, $OutWidth, $OutHeight)
            $g.DrawImage($img, $destRect, $cropRect, [System.Drawing.GraphicsUnit]::Pixel)
        } finally { $g.Dispose() }

        New-Item -ItemType Directory -Force -Path (Split-Path -Parent $outFullPath) | Out-Null
        $bmp.Save($outFullPath, $jpegCodec, $encParams)
    } finally { $bmp.Dispose() }
}

function Load-Image {
    param([string]$path)

    $fs = [System.IO.File]::OpenRead($path)
    try { return [System.Drawing.Image]::FromStream($fs) }
    finally { $fs.Dispose() }
}

Get-ChildItem -LiteralPath $src -Directory | ForEach-Object {
    $genre = $_.Name
    $outDir = Join-Path $dst $genre
    Get-ChildItem -LiteralPath $_.FullName -File | Where-Object { $_.Extension -ne '.gif' } | ForEach-Object {
        $file = $_
        try {
            $img = Load-Image $file.FullName
            if ($null -eq $img) { $skipped++; return }

            $outPath = Join-Path $outDir ([System.IO.Path]::ChangeExtension($file.Name, '.jpg'))
            Process-Image $img $outPath
            $img.Dispose()
            $converted++
        }
        catch {
            $failed++
            Write-Output ("FAIL $genre / $($file.Name) : $($_.Exception.Message)")
        }
    }
}

if (Test-Path -LiteralPath $defaultSrc) {
    try {
        $img = Load-Image $defaultSrc
        Process-Image $img $defaultDst
        $img.Dispose()
        $converted++
        Write-Output ("default fallback -> $defaultDst")
    }
    catch {
        $failed++
        Write-Output ("FAIL default fallback : $($_.Exception.Message)")
    }
}

Write-Output ("--- done: converted=$converted skipped=$skipped failed=$failed ---")