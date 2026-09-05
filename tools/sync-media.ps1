# NIGHTFALL media sync.
#
# Media (music + wallpapers) intentionally lives OUT of git to keep the
# repository fast. This script is the single entry point for moving that
# media between environments.
#
# Usage:
#   .\sync-media.ps1 -Build                     # write tools/media-manifest.json (SHA256 + size per file)
#   .\sync-media.ps1 -Verify -Target <dir>      # check <dir> against the manifest
#   .\sync-media.ps1 -Push -Target <dir|ssh>    # copy media to target, then verify
#   .\sync-media.ps1 -Push -Target <dir> -DryRun
#
# Target forms:
#   C:\path\to\wwwroot        local folder (robocopy, Windows)
#   ssh://user@host:/path      remote host (rsync, recommended for VPS)
#
# Relative manifest paths use forward slashes and are rooted at the repo's
# wwwroot, so targets must point at the deployed app's wwwroot folder.

[CmdletBinding()]
param(
    [ValidateSet('Build', 'Verify', 'Push')]
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$Mode,

    [Parameter()][string]$Target,

    [Parameter()][string]$ManifestPath,

    [Parameter()][switch]$DryRun
)

$ErrorActionPreference = 'Stop'

if (-not $ManifestPath) { $ManifestPath = Join-Path $PSScriptRoot 'media-manifest.json' }

$RepoRoot = Split-Path $PSScriptRoot -Parent
$MediaRoot = Join-Path $RepoRoot 'Blazor\TODOList\wwwroot'
$MediaDirs = @('music', 'wallpapers', 'wallpapers2560')

function Get-FileSha256 {
    param([string]$Path)
    (Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash.ToLowerInvariant()
}

function Build-Manifest {
    $entries = @()
    foreach ($dir in $MediaDirs) {
        $src = Join-Path $MediaRoot $dir
        if (-not (Test-Path -LiteralPath $src)) {
            Write-Warning "Source folder not found, skipping: $src"
            continue
        }
        Get-ChildItem -LiteralPath $src -Recurse -File | ForEach-Object {
            $rel = $_.FullName.Substring($MediaRoot.Length + 1).Replace('\', '/')
            $entries += [pscustomobject]@{
                path = $rel
                size = $_.Length
                sha256 = Get-FileSha256 -Path $_.FullName
            }
        }
    }
    $entries = $entries | Sort-Object path
    $entries | ConvertTo-Json | Set-Content -LiteralPath $ManifestPath -Encoding UTF8
    $totalMb = [math]::Round(($entries | Measure-Object size -Sum).Sum / 1MB, 1)
    Write-Host "Manifest written: $ManifestPath ($($entries.Count) files, $totalMb MB)"
}

function Verify-Manifest {
    if (-not (Test-Path -LiteralPath $ManifestPath)) {
        throw "Manifest not found: $ManifestPath. Run -Build first."
    }
    if (-not $Target) { throw 'Verify requires -Target.' }
    if (-not (Test-Path -LiteralPath $Target)) { throw "Target not found: $Target" }

    $entries = Get-Content -LiteralPath $ManifestPath -Raw | ConvertFrom-Json
    $ok = 0; $missing = @(); $mismatch = @()
    foreach ($e in $entries) {
        $file = Join-Path $Target $e.path
        if (-not (Test-Path -LiteralPath $file)) { $missing += $e.path; continue }
        if ((Get-Item -LiteralPath $file).Length -ne $e.size) { $mismatch += $e.path; continue }
        $hash = Get-FileSha256 -Path $file
        if ($hash -ne $e.sha256) { $mismatch += $e.path; continue }
        $ok++
    }
    Write-Host "Verify: $ok OK, $($missing.Count) missing, $($mismatch.Count) mismatched"
    if ($missing.Count -gt 0) { Write-Host '  missing:'; $missing | ForEach-Object { Write-Host "    $_" } }
    if ($mismatch.Count -gt 0) { Write-Host '  mismatched:'; $mismatch | ForEach-Object { Write-Host "    $_" } }
}

function Push-Local {
    if (-not $Target) { throw 'Push requires -Target.' }
    if (-not (Test-Path -LiteralPath $Target)) { New-Item -ItemType Directory -Path $Target -Force | Out-Null }
    foreach ($dir in $MediaDirs) {
        $src = Join-Path $MediaRoot $dir
        if (-not (Test-Path -LiteralPath $src)) {
            Write-Warning "Source folder not found, skipping: $src"
            continue
        }
        $dst = Join-Path $Target $dir
        Write-Host "robocopy $src -> $dst"
        if (-not $DryRun) {
            & robocopy $src $dst /E /NFL /NDL /NJH /NJS | Out-Null
            if ($LASTEXITCODE -ge 8) { throw "robocopy failed for $dir (exit $LASTEXITCODE)" }
        }
    }
}

function Push-Ssh {
    if (-not (Get-Command rsync -ErrorAction SilentlyContinue)) {
        throw 'rsync not found on PATH. Install Git for Windows (includes rsync) or use a local Target.'
    }
    $remote = $Target -replace '^ssh://', ''
    foreach ($dir in $MediaDirs) {
        $src = (Join-Path $MediaRoot $dir).TrimEnd('\') + '\'
        Write-Host "rsync $src -> ${remote}:/…/$dir"
        if (-not $DryRun) {
            & rsync -a --info=stats1 $src "$remote/$dir/"
            if ($LASTEXITCODE -ne 0) { throw "rsync failed for $dir (exit $LASTEXITCODE)" }
        }
    }
}

switch ($Mode) {
    'Build'  { Build-Manifest }
    'Verify' { Verify-Manifest }
    'Push'   {
        if ($Target -match '^ssh://|^[^\\/]+@[^\\/:]+:') {
            Push-Ssh
        }
        else {
            Push-Local
        }
        if (-not $DryRun) { Verify-Manifest }
    }
}