param(
    [string]$GameRoot = $env:STS2_PATH,
    [switch]$NoRefreshFromInstalled
)

$ErrorActionPreference = 'Stop'

$repoRoot = Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')
$manifestPath = Join-Path $repoRoot 'EZMicroBalance.json'
$manifest = Get-Content -Raw -LiteralPath $manifestPath | ConvertFrom-Json

if ($manifest.id -ne 'EZMicroBalance') {
    throw "Expected stable manifest id EZMicroBalance, found '$($manifest.id)'."
}

if ($manifest.name -ne 'Spire Plus') {
    throw "Expected player-facing name Spire Plus, found '$($manifest.name)'."
}

if ([string]::IsNullOrWhiteSpace($GameRoot)) {
    $GameRoot = 'D:\Steam\steamapps\common\Slay the Spire 2'
}

$publishRoot = Join-Path $repoRoot 'publish'
$stagingRoot = Join-Path $publishRoot 'package-staging'
$stagingModDir = Join-Path $stagingRoot 'EZMicroBalance'
$versionedRoot = Join-Path $publishRoot "SpirePlus-$($manifest.version)"
$versionedModDir = Join-Path $versionedRoot 'EZMicroBalance'
$zipPath = Join-Path $publishRoot "SpirePlus-$($manifest.version).zip"
$legacyZipPath = Join-Path $publishRoot "EZMicroBalance-$($manifest.version).zip"
$installedModDir = Join-Path $GameRoot 'mods\EZMicroBalance'
$requiredArtifactFiles = @('EZMicroBalance.dll', 'EZMicroBalance.json', 'EZMicroBalance.pck')

function Assert-UnderPath {
    param(
        [Parameter(Mandatory)] [string]$Candidate,
        [Parameter(Mandatory)] [string]$Parent
    )

    $parentFull = [System.IO.Path]::GetFullPath($Parent).TrimEnd('\') + '\'
    $candidateFull = [System.IO.Path]::GetFullPath($Candidate)
    if (-not $candidateFull.StartsWith($parentFull, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to modify path outside expected parent. Candidate: $candidateFull Parent: $parentFull"
    }
}

function Assert-RequiredArtifactFilesPresent {
    param(
        [Parameter(Mandatory)] [string]$Directory,
        [Parameter(Mandatory)] [string[]]$FileNames,
        [Parameter(Mandatory)] [string]$MissingMessagePrefix
    )

    foreach ($fileName in $FileNames) {
        $path = Join-Path $Directory $fileName
        if (-not (Test-Path -LiteralPath $path)) {
            throw "${MissingMessagePrefix}: $path"
        }
    }
}

function Assert-StagedManifestMatchesRepository {
    param(
        [Parameter(Mandatory)] [string]$StagedManifestPath,
        [Parameter(Mandatory)] [object]$RepositoryManifest
    )

    $stagedManifest = Get-Content -Raw -LiteralPath $StagedManifestPath | ConvertFrom-Json
    foreach ($propertyName in @('id', 'name', 'version')) {
        if ($stagedManifest.$propertyName -ne $RepositoryManifest.$propertyName) {
            throw "Staged manifest $propertyName mismatch. Expected '$($RepositoryManifest.$propertyName)', found '$($stagedManifest.$propertyName)'."
        }
    }
}

New-Item -ItemType Directory -Force -Path $publishRoot, $stagingModDir | Out-Null

if (-not $NoRefreshFromInstalled) {
    if (-not (Test-Path -LiteralPath $installedModDir)) {
        throw "Installed mod directory not found: $installedModDir. Run dotnet publish first or pass -NoRefreshFromInstalled."
    }

    Assert-RequiredArtifactFilesPresent `
        -Directory $installedModDir `
        -FileNames $requiredArtifactFiles `
        -MissingMessagePrefix 'Installed artifact missing'

    foreach ($fileName in $requiredArtifactFiles) {
        $source = Join-Path $installedModDir $fileName
        Copy-Item -LiteralPath $source -Destination (Join-Path $stagingModDir $fileName) -Force
    }
}
else {
    Assert-RequiredArtifactFilesPresent `
        -Directory $stagingModDir `
        -FileNames $requiredArtifactFiles `
        -MissingMessagePrefix 'NoRefreshFromInstalled uses existing package staging, but required artifact is missing'
}

Assert-StagedManifestMatchesRepository `
    -StagedManifestPath (Join-Path $stagingModDir 'EZMicroBalance.json') `
    -RepositoryManifest $manifest

$readmePath = Join-Path $stagingModDir 'README_INSTALL.txt'
@"
Spire Plus manual-test package

Archive: SpirePlus-$($manifest.version).zip
Display name: Spire Plus
Technical compatibility id: EZMicroBalance
Version: $($manifest.version)
Requires: BaseLib v3.1.4

Install:
1. Extract this archive into the Slay the Spire 2 mods folder exactly as packaged.
2. Keep legacy EzDailyContent disabled or absent.
3. Enable Spire Plus in the game's Mod Settings.
4. If the game's Mods list shows EZMicroBalance as the mod name, the package is stale or the display-name route regressed.

Test focus:
- Urda, Morvi, Lotha, and Vakuu Ancient rewards.
- A11-A20 progression: wider maps, Firemarked Elites, Rootblight, Banner Rooms, boss dedicated abilities, and Branded Form.
- Preview tools: Crystal Sphere peek and transform preview now live inside this same Spire Plus mod.
- Save/load, death/failure paths, and co-op still need manual proof.

Notes:
- This is a manual-test build, not release-ready.
- The compatibility id remains EZMicroBalance this cycle so existing saves, config, and legacy gates keep working.
- EZMicroBalance is a technical folder/id only; player-facing screens should say Spire Plus.
- Crystal Sphere peek and transform preview are part of this Spire Plus package.
- Ancient selections now grant visible marker relics so the chosen blessing stays readable in the relic bar.
- Ascension 21-30 and custom-character content are not included.
"@ | Set-Content -LiteralPath $readmePath -Encoding UTF8

if (-not $NoRefreshFromInstalled -and (Test-Path -LiteralPath $installedModDir)) {
    Copy-Item -LiteralPath $readmePath -Destination (Join-Path $installedModDir 'README_INSTALL.txt') -Force
}

foreach ($target in @($versionedRoot, $zipPath, $legacyZipPath)) {
    Assert-UnderPath -Candidate $target -Parent $publishRoot
    if (Test-Path -LiteralPath $target) {
        Remove-Item -LiteralPath $target -Recurse -Force
    }
}

New-Item -ItemType Directory -Force -Path $versionedRoot | Out-Null
Copy-Item -LiteralPath $stagingModDir -Destination $versionedRoot -Recurse -Force

Add-Type -AssemblyName System.IO.Compression.FileSystem
Add-Type -AssemblyName System.IO.Compression
$fixedZipTimestamp = [System.DateTimeOffset]::new(2026, 1, 1, 0, 0, 0, [System.TimeSpan]::Zero)
$zipStream = [System.IO.File]::Open($zipPath, [System.IO.FileMode]::CreateNew, [System.IO.FileAccess]::ReadWrite)
try {
    $archive = [System.IO.Compression.ZipArchive]::new($zipStream, [System.IO.Compression.ZipArchiveMode]::Create, $false)
    try {
        Get-ChildItem -LiteralPath $versionedModDir -File |
            Sort-Object -Property Name |
            ForEach-Object {
                $entry = $archive.CreateEntry(
                    "EZMicroBalance/$($_.Name)",
                    [System.IO.Compression.CompressionLevel]::Optimal)
                $entry.LastWriteTime = $fixedZipTimestamp
                $entryStream = $entry.Open()
                try {
                    $fileStream = [System.IO.File]::OpenRead($_.FullName)
                    try {
                        $fileStream.CopyTo($entryStream)
                    }
                    finally {
                        $fileStream.Dispose()
                    }
                }
                finally {
                    $entryStream.Dispose()
                }
            }
    }
    finally {
        $archive.Dispose()
    }
}
finally {
    $zipStream.Dispose()
}

$gameRootZipPath = Join-Path $GameRoot "SpirePlus-$($manifest.version).zip"
Copy-Item -LiteralPath $zipPath -Destination $gameRootZipPath -Force

Write-Host "Created $zipPath"
Write-Host "Copied $gameRootZipPath"
