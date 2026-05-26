param(
    [switch]$DryRun
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
$publishRoot = Join-Path $repoRoot 'publish'
$manifestPath = Join-Path $repoRoot 'EZMicroBalance.json'

function Assert-UnderPath {
    param(
        [Parameter(Mandatory)] [string]$Candidate,
        [Parameter(Mandatory)] [string]$Parent
    )

    $parentFull = [System.IO.Path]::GetFullPath($Parent).TrimEnd('\') + '\'
    $candidateFull = [System.IO.Path]::GetFullPath($Candidate)
    if (-not $candidateFull.StartsWith($parentFull, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to prune path outside expected parent. Candidate: $candidateFull Parent: $parentFull"
    }
}

function Get-RepoRelativePath {
    param(
        [Parameter(Mandatory)] [string]$Path
    )

    $repoFull = [System.IO.Path]::GetFullPath($repoRoot).TrimEnd('\') + '\'
    $pathFull = [System.IO.Path]::GetFullPath($Path)
    if ($pathFull.StartsWith($repoFull, [System.StringComparison]::OrdinalIgnoreCase)) {
        return $pathFull.Substring($repoFull.Length)
    }

    return $pathFull
}

if (-not (Test-Path -LiteralPath $publishRoot -PathType Container)) {
    Write-Host 'publish directory does not exist; nothing to prune.'
    return
}

$manifest = Get-Content -Raw -LiteralPath $manifestPath -Encoding UTF8 | ConvertFrom-Json
$currentVersion = [string]$manifest.version
if ($currentVersion -notmatch '^v0\.1\.0-private-beta\.(\d+)$') {
    throw "Unsupported Spire Plus private-beta version format: $currentVersion"
}

$currentBetaNumber = [int]$Matches[1]
$packagePrefix = 'SpirePlus-v0.1.0-private-beta.'
$escapedPackagePrefix = [regex]::Escape($packagePrefix)
$currentPackageName = "SpirePlus-$currentVersion"
$currentZipPath = Join-Path $publishRoot "$currentPackageName.zip"
if (-not (Test-Path -LiteralPath $currentZipPath -PathType Leaf)) {
    throw "Current package zip must exist before pruning stale packages: $(Get-RepoRelativePath -Path $currentZipPath)"
}

$candidates = @(Get-ChildItem -LiteralPath $publishRoot -Force |
    Where-Object {
        $_.Name -match "^$escapedPackagePrefix(\d+)(\.zip)?$" -and
        [int]$Matches[1] -lt $currentBetaNumber
    } |
    Sort-Object -Property Name)

foreach ($candidate in $candidates) {
    Assert-UnderPath -Candidate $candidate.FullName -Parent $publishRoot
    $relativePath = Get-RepoRelativePath -Path $candidate.FullName
    if ($DryRun) {
        Write-Host "Would remove $relativePath"
    }
    else {
        Remove-Item -LiteralPath $candidate.FullName -Recurse -Force
        Write-Host "Removed $relativePath"
    }
}

if ($DryRun) {
    Write-Host "Dry run complete: $($candidates.Count) stale publish package output(s) would be removed."
}
else {
    Write-Host "Pruned $($candidates.Count) stale publish package output(s)."
}
