param(
    [string]$EvidenceDir,

    [switch]$Launch,

    [switch]$NoLaunch
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
$runtimeRoot = Join-Path $repoRoot '.tools\runtime-evidence'

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
        return [System.IO.Path]::GetFullPath((Join-Path $runtimeRoot "coop-evidence-$stamp"))
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
foreach ($subdir in @('host', 'client', 'screenshots', 'notes')) {
    New-DirectoryIfMissing -Path (Join-Path $evidenceFull $subdir)
}

$selfTokens = @('.\scripts\collect-coop-evidence.ps1')
if ($EvidenceDir) { $selfTokens += @('-EvidenceDir', $evidenceFull) }
if ($Launch) { $selfTokens += '-Launch' }
if ($NoLaunch) { $selfTokens += '-NoLaunch' }

$manualRows = @(
    [ordered]@{
        Id = 'coop-host-join-clean-logs'
        Feature = 'co-op'
        Status = 'pending'
        RequiredEvidence = @('host/command.txt', 'host/godot.log', 'host/godot-log-audit.json', 'client/command.txt', 'client/godot.log', 'client/godot-log-audit.json')
        Notes = 'Both clients must load exactly the intended mod set with clean logs.'
    },
    [ordered]@{
        Id = 'coop-a11-a20-selection'
        Feature = 'A11-A20'
        Status = 'pending'
        RequiredEvidence = @('host screenshot', 'client screenshot', 'result note')
        Notes = 'Selection visibility is not gameplay support by itself.'
    },
    [ordered]@{
        Id = 'coop-ancients'
        Feature = 'Ancients'
        Status = 'pending'
        RequiredEvidence = @('host clicked Ancient screenshot/log', 'client synced state screenshot/log')
        Notes = 'Urda, Morvi, Lotha, and gated Vakuu need explicit disposition.'
    },
    [ordered]@{
        Id = 'coop-root-eyes'
        Feature = 'Root Eyes'
        Status = 'pending'
        RequiredEvidence = @('host map mutation evidence', 'client map consistency evidence', 'no desync logs')
        Notes = 'Shared map mutation requires host/client proof.'
    },
    [ordered]@{
        Id = 'coop-rootblight'
        Feature = 'Rootblight'
        Status = 'pending'
        RequiredEvidence = @('host combat/deck state', 'client combat/deck state', 'no desync logs')
        Notes = 'Ownership and deck state must be visible and consistent.'
    },
    [ordered]@{
        Id = 'coop-save-load-or-reconnect'
        Feature = 'save/load'
        Status = 'pending'
        RequiredEvidence = @('save or reconnect steps', 'host/client before-after logs', 'result note')
        Notes = 'If unsupported, record explicit release-note deferral instead of pass.'
    },
    [ordered]@{
        Id = 'coop-preview-tools-disposition'
        Feature = 'Preview tools'
        Status = 'pending'
        RequiredEvidence = @('host/client preview screenshots', 'fairness/disposition note')
        Notes = 'Information advantage must be documented; do not advertise fair co-op without proof.'
    }
)

$environment = [ordered]@{
    CreatedAt = (Get-Date).ToString('o')
    EvidenceKind = 'coop-evidence'
    RepositoryRoot = $repoRoot
    GitHead = Get-GitValue -Arguments @('rev-parse', 'HEAD')
    GitStatusShort = Get-GitValue -Arguments @('status', '--short')
    LaunchRequested = [bool]$Launch
    NoLaunch = -not [bool]$Launch
    Requirement = 'Two-client host/join proof with host and client logs, audits, screenshots, and result notes.'
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
        Get-HashRow -RelativePath 'publish\SpirePlus-v0.1.0-private-beta.8.zip'
        Get-HashRow -RelativePath 'publish\SpirePlus-v0.1.0-private-beta.8\EZMicroBalance\EZMicroBalance.dll'
        Get-HashRow -RelativePath 'publish\SpirePlus-v0.1.0-private-beta.8\EZMicroBalance\EZMicroBalance.pck'
        Get-HashRow -RelativePath 'publish\SpirePlus-v0.1.0-private-beta.8\EZMicroBalance\EZMicroBalance.json'
        Get-HashRow -RelativePath 'publish\SpirePlus-v0.1.0-private-beta.8\EZMicroBalance\README_INSTALL.txt'
    )
}

$hostTemplate = @(
    '# Host Command Template',
    '',
    'Status: pending',
    '',
    'Record exact host launch command, Steam account context, package path/hash, and enabled mods here.'
) -join [Environment]::NewLine

$clientTemplate = @(
    '# Client Command Template',
    '',
    'Status: pending',
    '',
    'Record exact client launch command, Steam account context, package path/hash, and enabled mods here.'
) -join [Environment]::NewLine

$resultTemplate = @(
    '# Co-op Result Notes',
    '',
    'Status: pending',
    '',
    'Do not mark co-op supported from lobby selection alone.',
    '',
    'Required disposition:',
    '- two-client host/join proof',
    '- both clients clean logs',
    '- no desync',
    '- relevant state visible/consistent',
    '- save/load or reconnect behavior explicit',
    '- release evidence row passed or explicitly deferred by owner'
) -join [Environment]::NewLine

$manualInstructions = @(
    '# Co-op Evidence Instructions',
    '',
    'Status: pending',
    '',
    'This helper does not launch co-op automatically. Co-op release evidence requires two real clients.',
    '',
    'Before testing:',
    '',
    '- Use exactly BaseLib plus Spire Plus on host and client.',
    '- Set `SPIREPLUS_RELEASE_EVIDENCE_LOG=1` for both clients.',
    '- Record package hashes from `package-hashes.json` before the run.',
    '',
    'Required checks:',
    '',
    '1. Host and client can join with clean `godot.log` and clean `godot-log-audit.json`.',
    '2. A11-A20 selection visibility and start-run behavior are recorded on host and client.',
    '3. Urda, Morvi, Lotha, and gated Vakuu have explicit co-op disposition notes.',
    '4. Root Eyes map previews either stay gated or show host/client-consistent state with no desync.',
    '5. Rootblight ownership, combat/deck state, and save/reconnect behavior are recorded.',
    '6. Crystal Sphere and transform preview tools have a fairness/disposition note.',
    '',
    'Put host logs under `host/`, client logs under `client/`, screenshots under `screenshots/`, and final notes under `notes/`.',
    '',
    'Do not mark co-op supported from lobby selection alone.'
) -join [Environment]::NewLine

Format-DisplayCommand -Tokens $selfTokens | Set-Content -LiteralPath (Join-Path $evidenceFull 'command.txt') -Encoding UTF8
Save-Json -InputObject $environment -Path (Join-Path $evidenceFull 'environment.json')
Save-Json -InputObject $packageHashes -Path (Join-Path $evidenceFull 'package-hashes.json')
Save-Json -InputObject ([ordered]@{ Rows = $manualRows }) -Path (Join-Path $evidenceFull 'manual-rows-template.json')
$manualInstructions | Set-Content -LiteralPath (Join-Path $evidenceFull 'manual-instructions.md') -Encoding UTF8
$hostTemplate | Set-Content -LiteralPath (Join-Path $evidenceFull 'host\command.txt') -Encoding UTF8
$clientTemplate | Set-Content -LiteralPath (Join-Path $evidenceFull 'client\command.txt') -Encoding UTF8
$resultTemplate | Set-Content -LiteralPath (Join-Path $evidenceFull 'notes\result-notes-template.md') -Encoding UTF8

Save-Json -InputObject ([ordered]@{
    Status = 'pending'
    Host = [ordered]@{
        RequiredFiles = @('host/command.txt', 'host/godot.log', 'host/godot-log-audit.json')
    }
    Client = [ordered]@{
        RequiredFiles = @('client/command.txt', 'client/godot.log', 'client/godot-log-audit.json')
    }
    Screenshots = @('screenshots/host-*.png', 'screenshots/client-*.png')
    Notes = 'Fill after a real two-client session. This template is not a pass marker.'
}) -Path (Join-Path $evidenceFull 'two-client-audit-template.json')

if ($Launch) {
    Write-Warning '-Launch is intentionally not automated for co-op because release evidence requires two real clients. Templates were generated only.'
}

Write-Output "Prepared co-op evidence templates under $evidenceFull."
Write-Output 'No co-op row was marked passed. Two-client live proof remains pending.'
