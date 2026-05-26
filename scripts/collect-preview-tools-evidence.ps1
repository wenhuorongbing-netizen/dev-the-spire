param(
    [string]$EvidenceDir,

    [switch]$Launch,

    [switch]$NoLaunch
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
$runtimeRoot = Join-Path $repoRoot '.tools\runtime-evidence'
$liveSessionScript = Join-Path $PSScriptRoot 'spire-plus-live-session.ps1'

function New-DirectoryIfMissing {
    param([Parameter(Mandatory = $true)][string]$Path)

    if (-not (Test-Path -LiteralPath $Path)) {
        New-Item -ItemType Directory -Path $Path -Force | Out-Null
    }
}

function Assert-PathInside {
    param(
        [Parameter(Mandatory = $true)][string]$Child,
        [Parameter(Mandatory = $true)][string]$Parent,
        [Parameter(Mandatory = $true)][string]$Label
    )

    $childFull = [System.IO.Path]::GetFullPath($Child).TrimEnd('\', '/')
    $parentFull = [System.IO.Path]::GetFullPath($Parent).TrimEnd('\', '/')
    $comparison = [System.StringComparison]::OrdinalIgnoreCase

    if ($childFull.Equals($parentFull, $comparison)) {
        return
    }

    if (-not $childFull.StartsWith($parentFull + '\', $comparison)) {
        throw "$Label path is outside expected root. Path: $childFull Root: $parentFull"
    }
}

function Get-EvidenceFullPath {
    param([string]$RequestedPath)

    New-DirectoryIfMissing -Path $runtimeRoot

    if ([string]::IsNullOrWhiteSpace($RequestedPath)) {
        $stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
        return [System.IO.Path]::GetFullPath((Join-Path $runtimeRoot "preview-tools-evidence-$stamp"))
    }

    $candidate = if ([System.IO.Path]::IsPathRooted($RequestedPath)) {
        $RequestedPath
    } else {
        Join-Path $repoRoot $RequestedPath
    }

    return [System.IO.Path]::GetFullPath($candidate)
}

function Save-Json {
    param(
        [Parameter(Mandatory = $true)]$InputObject,
        [Parameter(Mandatory = $true)][string]$Path
    )

    $InputObject | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $Path -Encoding UTF8
}

function Format-DisplayToken {
    param([Parameter(Mandatory = $true)][string]$Value)

    if ($Value -match '[\s`"]') {
        return '"' + ($Value -replace '"', '`"') + '"'
    }

    return $Value
}

function Format-DisplayCommand {
    param([Parameter(Mandatory = $true)][string[]]$Tokens)

    return (($Tokens | ForEach-Object { Format-DisplayToken -Value $_ }) -join ' ')
}

function Get-HashRow {
    param([Parameter(Mandatory = $true)][string]$RelativePath)

    $fullPath = Join-Path $repoRoot $RelativePath
    if (-not (Test-Path -LiteralPath $fullPath -PathType Leaf)) {
        return [ordered]@{
            Path = $RelativePath
            Exists = $false
            Sha256 = $null
            Length = $null
        }
    }

    $item = Get-Item -LiteralPath $fullPath
    return [ordered]@{
        Path = $RelativePath
        Exists = $true
        Sha256 = (Get-FileHash -LiteralPath $fullPath -Algorithm SHA256).Hash
        Length = $item.Length
    }
}

function Get-GitValue {
    param([Parameter(Mandatory = $true)][string[]]$Arguments)

    try {
        $value = & git -C $repoRoot @Arguments 2>$null
        if ($LASTEXITCODE -eq 0) {
            return ($value -join "`n").Trim()
        }
    } catch {
    }

    return $null
}

if ($Launch -and $NoLaunch) {
    throw 'Pass only one of -Launch or -NoLaunch.'
}

$evidenceFull = Get-EvidenceFullPath -RequestedPath $EvidenceDir
Assert-PathInside -Child $evidenceFull -Parent $runtimeRoot -Label 'Evidence'
New-DirectoryIfMissing -Path $evidenceFull
New-DirectoryIfMissing -Path (Join-Path $evidenceFull 'screenshots')

$selfTokens = @('.\scripts\collect-preview-tools-evidence.ps1')
if ($EvidenceDir) { $selfTokens += @('-EvidenceDir', $evidenceFull) }
if ($Launch) { $selfTokens += '-Launch' }
if ($NoLaunch) { $selfTokens += '-NoLaunch' }

$manualRows = @(
    [ordered]@{
        Id = 'crystal-sphere-toggle-visible'
        Feature = 'Preview tools'
        Status = 'pending'
        RequiredEvidence = @('Crystal Sphere screen screenshot', 'window-preflight.json', 'godot.log', 'godot-log-audit.json')
        Notes = 'Toggle between normal mask and preview visibility without claiming reward collection.'
    },
    [ordered]@{
        Id = 'crystal-sphere-no-charges-spent'
        Feature = 'Preview tools'
        Status = 'pending'
        RequiredEvidence = @('before/after charge evidence', 'result-note.md')
        Notes = 'No divination charge should be spent by previewing.'
    },
    [ordered]@{
        Id = 'crystal-sphere-no-reward'
        Feature = 'Preview tools'
        Status = 'pending'
        RequiredEvidence = @('before/after reward evidence', 'result-note.md')
        Notes = 'No ClearCell, RevealItem, CellClicked, AddReward, or item reveal behavior should occur.'
    },
    [ordered]@{
        Id = 'transform-preview-single'
        Feature = 'Preview tools'
        Status = 'pending'
        RequiredEvidence = @('single-card transform screenshot/log', 'result-note.md')
        Notes = 'Prediction matches committed result.'
    },
    [ordered]@{
        Id = 'transform-preview-multi'
        Feature = 'Preview tools'
        Status = 'pending'
        RequiredEvidence = @('multi-card transform screenshot/log', 'result-note.md')
        Notes = 'Prediction order matches committed replacement queue.'
    },
    [ordered]@{
        Id = 'transform-preview-combat'
        Feature = 'Preview tools'
        Status = 'pending'
        RequiredEvidence = @('combat-context screenshot/log', 'result-note.md')
        Notes = 'Combat transform context is source-backed and clears after selection.'
    },
    [ordered]@{
        Id = 'transform-preview-non-combat'
        Feature = 'Preview tools'
        Status = 'pending'
        RequiredEvidence = @('non-combat context screenshot/log', 'result-note.md')
        Notes = 'Event/relic transform context is source-backed and clears after selection.'
    },
    [ordered]@{
        Id = 'cancel-reopen-no-rng-advance'
        Feature = 'Preview tools'
        Status = 'pending'
        RequiredEvidence = @('cancel/reopen logs', 'RNG counter note')
        Notes = 'Cancel/reopen must not advance real RNG or reuse stale snapshots.'
    }
)

$environment = [ordered]@{
    CreatedAt = (Get-Date).ToString('o')
    EvidenceKind = 'preview-tools-evidence'
    RepositoryRoot = $repoRoot
    GitHead = Get-GitValue -Arguments @('rev-parse', 'HEAD')
    GitStatusShort = Get-GitValue -Arguments @('status', '--short')
    Naming = 'Preview tools are integrated into Spire Plus.'
    LaunchRequested = [bool]$Launch
    NoLaunch = -not [bool]$Launch
    ReleaseEvidenceLogging = [ordered]@{
        EnvironmentVariable = 'SPIREPLUS_RELEASE_EVIDENCE_LOG'
        LegacyEnvironmentVariable = 'EZMB_RELEASE_EVIDENCE_LOG'
        RecommendedValue = '1'
    }
}

$packageHashes = [ordered]@{
    CreatedAt = (Get-Date).ToString('o')
    Files = @(
        Get-HashRow -RelativePath 'EZMicroBalance.json'
        Get-HashRow -RelativePath 'publish\SpirePlus-v0.1.0-private-beta.26.zip'
        Get-HashRow -RelativePath 'publish\SpirePlus-v0.1.0-private-beta.26\EZMicroBalance\EZMicroBalance.dll'
        Get-HashRow -RelativePath 'publish\SpirePlus-v0.1.0-private-beta.26\EZMicroBalance\EZMicroBalance.pck'
        Get-HashRow -RelativePath 'publish\SpirePlus-v0.1.0-private-beta.26\EZMicroBalance\EZMicroBalance.json'
        Get-HashRow -RelativePath 'publish\SpirePlus-v0.1.0-private-beta.26\EZMicroBalance\README_INSTALL.txt'
    )
}

$instructions = @(
    '# Preview Tools Evidence',
    '',
    'Status: pending',
    '',
    'Collect Crystal Sphere and transform-preview proof from the integrated Spire Plus mod settings surface.',
    '',
    'Required checks:',
    '- Crystal Sphere toggle visible',
    '- No charges spent',
    '- No reward generated or revealed',
    '- Transform preview single-card',
    '- Transform preview multi-card',
    '- Transform preview combat',
    '- Transform preview non-combat',
    '- Cancel/reopen no RNG advance',
    '',
    'Do not mark any row passed from this template alone.'
) -join [Environment]::NewLine

Format-DisplayCommand -Tokens $selfTokens | Set-Content -LiteralPath (Join-Path $evidenceFull 'command.txt') -Encoding UTF8
Save-Json -InputObject $environment -Path (Join-Path $evidenceFull 'environment.json')
Save-Json -InputObject $packageHashes -Path (Join-Path $evidenceFull 'package-hashes.json')
Save-Json -InputObject ([ordered]@{ Rows = $manualRows }) -Path (Join-Path $evidenceFull 'manual-rows-template.json')
Save-Json -InputObject ([ordered]@{
    Status = 'pending'
    RequiredFiles = @('godot.log', 'godot-log-audit.json')
    BlockingPatterns = @('ERROR', 'Exception', '[SPIREPLUS-EVIDENCE]', '[EZMB-EVIDENCE]')
    Notes = 'Fill with audit output after copying live logs. This template is not a pass marker.'
}) -Path (Join-Path $evidenceFull 'log-audit-template.json')
$instructions | Set-Content -LiteralPath (Join-Path $evidenceFull 'manual-instructions.md') -Encoding UTF8

if (-not $Launch) {
    Write-Output "Prepared preview tools evidence templates under $evidenceFull."
    Write-Output 'No game was launched. Preview tools live rows remain pending.'
    exit 0
}

if (-not (Test-Path -LiteralPath $liveSessionScript)) {
    throw "Missing live-session helper: $liveSessionScript"
}

$launchArgs = @('-Mode', 'Prepare', '-EvidenceDir', $evidenceFull, '-Launch')
$powerShellExe = if ($PSVersionTable.PSEdition -eq 'Core') { 'pwsh' } else { 'powershell.exe' }
& $powerShellExe -NoProfile -ExecutionPolicy Bypass -File $liveSessionScript @launchArgs
if ($LASTEXITCODE -ne 0) {
    throw "Live-session prepare failed with exit code $LASTEXITCODE."
}

Write-Output "Live session launched. Fill preview tools evidence under $evidenceFull."
