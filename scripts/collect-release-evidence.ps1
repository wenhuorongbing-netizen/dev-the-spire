param(
    [string]$EvidenceDir,

    [switch]$Launch,

    [switch]$NoLaunch,

    [switch]$MoveOtherMods,

    [switch]$MoveCurrentRuns
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
$runtimeRoot = Join-Path $repoRoot '.tools\runtime-evidence'
$liveSessionScript = Join-Path $PSScriptRoot 'spire-plus-live-session.ps1'
$verifierScript = Join-Path $PSScriptRoot 'verify-spire-plus-release-evidence.ps1'

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
        return [System.IO.Path]::GetFullPath((Join-Path $runtimeRoot "release-evidence-$stamp"))
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

function Get-PowerShellExecutable {
    $processPath = (Get-Process -Id $PID).Path
    if ($processPath -and (Test-Path -LiteralPath $processPath)) {
        return $processPath
    }

    if ($PSVersionTable.PSEdition -eq 'Core') {
        return 'pwsh'
    }

    return 'powershell.exe'
}

function Invoke-PowerShellFile {
    param(
        [Parameter(Mandatory = $true)][string]$ScriptPath,
        [Parameter(Mandatory = $true)][string[]]$ArgumentList
    )

    $powerShellExe = Get-PowerShellExecutable
    $childArgs = @('-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', $ScriptPath) + $ArgumentList
    $output = & $powerShellExe @childArgs 2>&1
    $exitCode = $LASTEXITCODE
    if ($null -eq $exitCode) {
        $exitCode = 0
    }

    return [pscustomobject]@{
        ExitCode = [int]$exitCode
        Output = @($output | ForEach-Object { $_.ToString() })
    }
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

function New-ManualRows {
    return @(
        [ordered]@{
            Id = 'fresh-current-package-loader-smoke'
            Category = 'loader'
            Status = 'pending'
            RequiredEvidence = @('command.txt', 'environment.json', 'package-hashes.json', 'godot.log', 'godot-log-audit.json', 'enabled-mods.txt')
            Notes = 'Must prove current installed Spire Plus package reaches main menu with BaseLib and EZMicroBalance only.'
        },
        [ordered]@{
            Id = 'clicked-ancient-ui-urda-morvi-lotha-vakuu'
            Category = 'clicked-ui'
            Status = 'pending'
            RequiredEvidence = @('foreground preflight json', 'clicked UI screenshots', 'godot.log', 'godot-log-audit.json', 'route note')
            Notes = 'Source review, main menu, or resource-load smoke does not close this row.'
        },
        [ordered]@{
            Id = 'ancient-save-load'
            Category = 'save-load'
            Status = 'pending'
            RequiredEvidence = @('before save screenshot/log', 'after load screenshot/log', 'save-load-note.md')
            Notes = 'Covers Urda, Morvi, Lotha, Ancient reward state, and relevant persistent markers.'
        },
        [ordered]@{
            Id = 'vakuu-victory-failure-death'
            Category = 'vakuu'
            Status = 'pending'
            RequiredEvidence = @('fight start evidence', 'victory return evidence', 'failure/death path evidence', 'godot-log-audit.json')
            Notes = 'Must include no black screen and no softlock proof.'
        },
        [ordered]@{
            Id = 'coop-two-client-disposition'
            Category = 'coop'
            Status = 'pending'
            RequiredEvidence = @('host command/log/audit', 'client command/log/audit', 'screenshots', 'result notes')
            Notes = 'StartRunLobby access alone is not co-op support proof.'
        },
        [ordered]@{
            Id = 'preview-tools-live-proof'
            Category = 'preview-tools'
            Status = 'pending'
            RequiredEvidence = @('Crystal Sphere evidence', 'transform preview evidence', 'log audit', 'RNG/state notes')
            Notes = 'Preview tools ship inside Spire Plus; do not use EZFuturePeek naming or independent-project evidence.'
        },
        [ordered]@{
            Id = 'release-evidence-verifier-pass'
            Category = 'verifier'
            Status = 'pending'
            RequiredEvidence = @('release-evidence-verifier-pass.json', 'verifier-output.txt')
            Notes = 'Create only after verify-spire-plus-release-evidence.ps1 exits 0 against filled live evidence.'
        }
    )
}

if ($Launch -and $NoLaunch) {
    throw 'Pass only one of -Launch or -NoLaunch.'
}

$evidenceFull = Get-EvidenceFullPath -RequestedPath $EvidenceDir
Assert-PathInside -Child $evidenceFull -Parent $runtimeRoot -Label 'Evidence'
New-DirectoryIfMissing -Path $evidenceFull

$selfTokens = @('.\scripts\collect-release-evidence.ps1')
if ($EvidenceDir) { $selfTokens += @('-EvidenceDir', $evidenceFull) }
if ($Launch) { $selfTokens += '-Launch' }
if ($NoLaunch) { $selfTokens += '-NoLaunch' }
if ($MoveOtherMods) { $selfTokens += '-MoveOtherMods' }
if ($MoveCurrentRuns) { $selfTokens += '-MoveCurrentRuns' }

$launchArgs = @('-Mode', 'Prepare', '-EvidenceDir', $evidenceFull)
if ($MoveOtherMods) { $launchArgs += '-MoveOtherMods' }
if ($MoveCurrentRuns) { $launchArgs += '-MoveCurrentRuns' }
$launchArgs += '-Launch'

$environment = [ordered]@{
    CreatedAt = (Get-Date).ToString('o')
    EvidenceKind = 'release-evidence'
    RepositoryRoot = $repoRoot
    GitHead = Get-GitValue -Arguments @('rev-parse', 'HEAD')
    GitStatusShort = Get-GitValue -Arguments @('status', '--short')
    Sts2Path = [Environment]::GetEnvironmentVariable('STS2_PATH', 'Process')
    GodotPath = [Environment]::GetEnvironmentVariable('GODOT_PATH', 'Process')
    BaseLibExpectedRuntimeLocation = '<GameRoot>\mods\BaseLib'
    ReleaseEvidenceLogging = [ordered]@{
        EnvironmentVariable = 'EZMB_RELEASE_EVIDENCE_LOG'
        RecommendedValue = '1'
    }
    LaunchRequested = [bool]$Launch
    NoLaunch = -not [bool]$Launch
    Scripts = [ordered]@{
        LiveSession = $liveSessionScript
        Verifier = $verifierScript
    }
}

$packageHashes = [ordered]@{
    CreatedAt = (Get-Date).ToString('o')
    Files = @(
        Get-HashRow -RelativePath 'EZMicroBalance.json'
        Get-HashRow -RelativePath 'publish\SpirePlus-v0.1.0-private-beta.0.zip'
        Get-HashRow -RelativePath 'publish\EZMicroBalance.dll'
        Get-HashRow -RelativePath 'publish\EZMicroBalance.pck'
        Get-HashRow -RelativePath 'publish\EZMicroBalance.json'
    )
}

$enabledModsTemplate = @(
    '# Enabled Mods Template',
    '',
    'Status: pending',
    '',
    'Expected current-package loader proof:',
    '- BaseLib',
    '- EZMicroBalance / Spire Plus',
    '',
    'Paste the loaded-mods log excerpt here. Do not mark this file passed from historical 16/22-field logs or source review.'
) -join [Environment]::NewLine

$readme = @(
    '# Release Evidence Folder',
    '',
    'Status: pending',
    '',
    'This folder is a collection template. It is not release proof until live logs, screenshots, manual notes, and verifier output are filled in.',
    '',
    'Required high-level evidence:',
    '- Fresh current-package loader smoke',
    '- Clicked Ancient UI',
    '- Save/load',
    '- Vakuu victory/failure/death',
    '- Two-client co-op disposition',
    '- Preview tools live proof',
    '- Release evidence verifier pass marker'
) -join [Environment]::NewLine

Format-DisplayCommand -Tokens $selfTokens | Set-Content -LiteralPath (Join-Path $evidenceFull 'command.txt') -Encoding UTF8
Save-Json -InputObject $environment -Path (Join-Path $evidenceFull 'environment.json')
Save-Json -InputObject $packageHashes -Path (Join-Path $evidenceFull 'package-hashes.json')
Save-Json -InputObject ([ordered]@{ Rows = @(New-ManualRows) }) -Path (Join-Path $evidenceFull 'manual-rows-template.json')
Save-Json -InputObject ([ordered]@{
    Status = 'pending'
    RequiredCommand = '.\scripts\verify-spire-plus-release-evidence.ps1'
    PassMarkerPath = 'release-evidence-verifier-pass.json'
    Notes = 'Write a pass marker only after the verifier exits 0 against filled live evidence.'
}) -Path (Join-Path $evidenceFull 'verifier-pass-marker-template.json')
$enabledModsTemplate | Set-Content -LiteralPath (Join-Path $evidenceFull 'enabled-mods-template.txt') -Encoding UTF8
$readme | Set-Content -LiteralPath (Join-Path $evidenceFull 'README.md') -Encoding UTF8

if (-not $Launch) {
    Write-Output "Prepared release evidence templates under $evidenceFull."
    Write-Output 'No game was launched. Live rows remain pending.'
    exit 0
}

if (-not (Test-Path -LiteralPath $liveSessionScript)) {
    throw "Missing live-session helper: $liveSessionScript"
}

$result = Invoke-PowerShellFile -ScriptPath $liveSessionScript -ArgumentList $launchArgs
if ($result.Output.Count -gt 0) {
    $result.Output | Tee-Object -FilePath (Join-Path $evidenceFull 'launch-output.txt')
}

if ($result.ExitCode -ne 0) {
    throw "Live-session prepare failed with exit code $($result.ExitCode)."
}

Write-Output "Live session launched. Fill evidence under $evidenceFull, then run the verifier before any release-ready claim."
