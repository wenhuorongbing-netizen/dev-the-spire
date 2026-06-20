param(
    [string]$EvidenceDir,

    [ValidateSet('None', 'List', 'Page', 'Both')]
    [string]$Capture = 'None',

    [switch]$RequireSpireForeground,

    [switch]$NoPreflight,

    [switch]$NoLaunch
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
. (Join-Path $PSScriptRoot 'spire-plus-package-evidence.ps1')
$runtimeRoot = Join-Path $repoRoot '.tools\runtime-evidence'
$windowPreflightScript = Join-Path $PSScriptRoot 'check-spire-window-preflight.ps1'
$captureScript = Join-Path $PSScriptRoot 'capture-spire-window.ps1'

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
        return [System.IO.Path]::GetFullPath((Join-Path $runtimeRoot "mod-settings-current-display-$stamp"))
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

function Set-Text {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$Text
    )

    $Text | Set-Content -LiteralPath $Path -Encoding UTF8
}

function Set-TextIfMissing {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$Text
    )

    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        Set-Text -Path $Path -Text $Text
    }
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

function Invoke-HelperScript {
    param(
        [Parameter(Mandatory = $true)][string]$ScriptPath,
        [Parameter(Mandatory = $true)][string[]]$Arguments,
        [Parameter(Mandatory = $true)][string]$OutputPath
    )

    $powerShellExe = Get-PowerShellExecutable
    $childArgs = @('-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', $ScriptPath) + $Arguments
    $output = & $powerShellExe @childArgs 2>&1
    $exitCode = if ($null -eq $LASTEXITCODE) { 0 } else { [int]$LASTEXITCODE }
    @($output | ForEach-Object { $_.ToString() }) | Set-Content -LiteralPath $OutputPath -Encoding UTF8

    return [pscustomobject]@{
        ExitCode = $exitCode
        OutputPath = $OutputPath
        Output = @($output | ForEach-Object { $_.ToString() })
    }
}

if ($NoLaunch -and $Capture -ne 'None') {
    throw 'Do not combine -NoLaunch with screenshot capture. Run once with -NoLaunch for templates, then rerun with -Capture after manually navigating the game UI.'
}

if (-not (Test-Path -LiteralPath $windowPreflightScript -PathType Leaf)) {
    throw "Missing window preflight helper: $windowPreflightScript"
}

if (-not (Test-Path -LiteralPath $captureScript -PathType Leaf)) {
    throw "Missing window capture helper: $captureScript"
}

$evidenceFull = Get-EvidenceFullPath -RequestedPath $EvidenceDir
Assert-PathInside -Child $evidenceFull -Parent $runtimeRoot -Label 'Evidence'
New-DirectoryIfMissing -Path $evidenceFull
$screenshotsDir = Join-Path $evidenceFull 'screenshots'
New-DirectoryIfMissing -Path $screenshotsDir

$selfTokens = @('.\scripts\collect-mod-settings-evidence.ps1')
if ($EvidenceDir) { $selfTokens += @('-EvidenceDir', $evidenceFull) }
if ($Capture -ne 'None') { $selfTokens += @('-Capture', $Capture) }
if ($RequireSpireForeground) { $selfTokens += '-RequireSpireForeground' }
if ($NoPreflight) { $selfTokens += '-NoPreflight' }
if ($NoLaunch) { $selfTokens += '-NoLaunch' }

Set-Text -Path (Join-Path $evidenceFull 'command.txt') -Text (Format-DisplayCommand -Tokens $selfTokens)

$packageHashes = [ordered]@{
    CreatedAt = (Get-Date).ToString('o')
    Files = @(
        Get-HashRow -RelativePath 'EZMicroBalance.json'
        foreach ($artifactPath in (Get-SpirePlusPackageArtifactRelativePaths -RepoRoot $repoRoot)) {
            Get-HashRow -RelativePath $artifactPath
        }
    )
}
Save-Json -InputObject $packageHashes -Path (Join-Path $evidenceFull 'package-hashes.json')

$gitEvidence = Get-SpirePlusGitEvidence -RepoRoot $repoRoot
$environment = [ordered]@{
    CreatedAt = (Get-Date).ToString('o')
    EvidenceKind = 'mod-settings-current-display'
    RepositoryRoot = $repoRoot
    GitHead = $gitEvidence.Head
    GitStatusShort = $gitEvidence.StatusShort
    GitBranchStatus = $gitEvidence.BranchStatus
    GitUpstream = $gitEvidence.Upstream
    GitUpstreamHead = $gitEvidence.UpstreamHead
    GitPushedHead = $gitEvidence.PushedHead
    GitHeadMatchesUpstream = $gitEvidence.HeadMatchesUpstream
    Git = $gitEvidence
    PackagePath = Get-SpirePlusPackageRelativePath -RepoRoot $repoRoot
    PackageVersion = Get-SpirePlusManifestVersion -RepoRoot $repoRoot
    Capture = $Capture
    CaptureRequested = $Capture -ne 'None'
    NoLaunch = $Capture -eq 'None'
    RequiredEvidenceBoundary = 'This helper creates or captures Mod Settings UI evidence files only. It does not prove gameplay, save-load, co-op, release readiness, or private beta readiness.'
}
Save-Json -InputObject $environment -Path (Join-Path $evidenceFull 'environment.json')

$checklistTemplate = @'
# Mod Settings Current Display Checklist

Template reference for `mod-settings-checklist.md`. Fill the working `mod-settings-checklist.md` with live results before marking this row pass.

| Scenario ID | Expected behavior | Live result | Evidence file(s) |
| --- | --- | --- | --- |
| ritsulib-visible-enabled | STS2-RitsuLib appears in Settings -> Mod Settings and is enabled for the session. |  |  |
| spire-plus-list-display-name | The Mods list shows the player-facing name Spire Plus for the current package. |  |  |
| spire-plus-config-page-current-name | Opening the Spire Plus config page shows current Spire Plus display text, not the older EZ Micro Balance page-level text. |  |  |
| technical-id-compatibility | EZMicroBalance appears only as the technical manifest id, folder, or log/config id where applicable; it is not the primary player-facing mod name. |  |  |
| legacy-mod-surfaces-absent | Legacy EzDailyContent and standalone EZFuturePeek mod surfaces are absent or disabled. |  |  |
| clean-log-config-registration | The same-session godot.log includes current package/config registration evidence and the clean log audit has no release-blocking signatures. |  |  |
'@

$workingChecklist = $checklistTemplate -replace 'Template reference for `mod-settings-checklist.md`\. Fill the working `mod-settings-checklist.md` with live results before marking this row pass\.', 'Fill this checklist with live results before marking this row pass.'
Set-Text -Path (Join-Path $evidenceFull 'mod-settings-checklist-template.md') -Text $checklistTemplate
Set-TextIfMissing -Path (Join-Path $evidenceFull 'mod-settings-checklist.md') -Text $workingChecklist

$routeNote = @'
# Mod Settings Route Note

Fill with the exact route used to open the current package's Mod Settings list and Spire Plus config page.

Required before pass:
- normal Steam-client path or explicitly documented direct smoke path;
- language used;
- screenshot filenames for the Mod Settings list and Spire Plus page;
- confirmation that STS2-RitsuLib and Spire Plus are enabled;
- confirmation that legacy EzDailyContent and standalone EZFuturePeek surfaces are absent or disabled.
'@
Set-TextIfMissing -Path (Join-Path $evidenceFull 'route-note.md') -Text $routeNote

$resultNote = @'
# Mod Settings Result Note

Pending live evidence. Do not mark this row pass until screenshots, same-session log/audit files, route note, and filled checklist are present.
'@
Set-TextIfMissing -Path (Join-Path $evidenceFull 'result-note.md') -Text $resultNote

$manualRows = [ordered]@{
    CreatedAt = (Get-Date).ToString('o')
    Rows = @(
        [ordered]@{
            Id = 'mod-settings-current-display'
            Label = 'Current Spire Plus Mod Settings list and config page proof'
            Kind = 'clicked-ui'
            Status = 'pending'
            EvidenceDir = $evidenceFull
            RequiredFiles = @('command.txt', 'environment.json', 'package-hashes.json', 'window-preflight.json', 'godot.log', 'godot-log-audit.json', 'route-note.md', 'result-note.md', 'mod-settings-checklist.md')
            ScreenshotFile = ''
            ResultNote = ''
            ExplicitOwnerDecision = $false
            Checkpoints = @(
                'STS2-RitsuLib appears and is enabled.',
                'Spire Plus appears as the player-facing name in the current package.',
                'Spire Plus settings page opens and renders without errors.',
                'EZMicroBalance is used only as technical compatibility id where applicable.',
                'Legacy EzDailyContent and standalone EZFuturePeek surfaces are absent or disabled.',
                'Same-session godot.log and clean audit are retained.'
            )
            Notes = 'Pending Mod Settings UI evidence. Screenshots and logs are required before pass.'
        }
    )
}
Save-Json -InputObject $manualRows -Path (Join-Path $evidenceFull 'manual-rows-template.json')

if (-not $NoPreflight) {
    $preflightArgs = @('-OutFile', (Join-Path $evidenceFull 'window-preflight.json'))
    if ($RequireSpireForeground -or $Capture -ne 'None') {
        $preflightArgs += '-RequireSpireForeground'
    }

    $preflight = Invoke-HelperScript -ScriptPath $windowPreflightScript -Arguments $preflightArgs -OutputPath (Join-Path $evidenceFull 'window-preflight-output.txt')
    if ($preflight.ExitCode -ne 0) {
        throw "Window preflight failed with exit code $($preflight.ExitCode). See $($preflight.OutputPath)."
    }
}

$capturedFiles = @()
if ($Capture -ne 'None') {
    $captureTargets = switch ($Capture) {
        'List' { @('list') }
        'Page' { @('page') }
        'Both' { @('list', 'page') }
        default { @() }
    }

    foreach ($target in $captureTargets) {
        $pngPath = Join-Path $screenshotsDir "mod-settings-$target.png"
        $captureArgs = @('-OutFile', $pngPath, '-RequireSpireForeground')
        $capture = Invoke-HelperScript -ScriptPath $captureScript -Arguments $captureArgs -OutputPath (Join-Path $screenshotsDir "mod-settings-$target.capture.json")
        if ($capture.ExitCode -ne 0) {
            throw "Window capture for $target failed with exit code $($capture.ExitCode). See $($capture.OutputPath)."
        }

        $capturedFiles += $pngPath
    }
}

$readme = @"
# Mod Settings Current Display Evidence

This folder is a pending evidence scaffold for the RitsuLib migration Mod Settings UI gate. It does not prove gameplay, save-load, co-op, release readiness, or private beta readiness.

Required files before pass:
- `command.txt`
- `environment.json`
- `package-hashes.json`
- `window-preflight.json`
- `godot.log`
- `godot-log-audit.json`
- `route-note.md`
- `result-note.md`
- `mod-settings-checklist.md`
- screenshots for the Mod Settings list and the Spire Plus config page

Suggested capture flow:

```powershell
.\scripts\collect-mod-settings-evidence.ps1 -NoLaunch
# Launch through the normal Steam-client live-session path, open Settings -> Mod Settings, and bring Slay the Spire 2 to the foreground.
.\scripts\collect-mod-settings-evidence.ps1 -EvidenceDir "$evidenceFull" -Capture List -RequireSpireForeground
# Open the Spire Plus config page, then capture again.
.\scripts\collect-mod-settings-evidence.ps1 -EvidenceDir "$evidenceFull" -Capture Page -RequireSpireForeground
```

Captured screenshots this run:
$((@($capturedFiles) | ForEach-Object { "- $_" }) -join [Environment]::NewLine)
"@
Set-Text -Path (Join-Path $evidenceFull 'README.md') -Text $readme

[pscustomobject]@{
    EvidenceDir = $evidenceFull
    ManualRowsTemplate = Join-Path $evidenceFull 'manual-rows-template.json'
    Checklist = Join-Path $evidenceFull 'mod-settings-checklist.md'
    Capture = $Capture
    CapturedFiles = @($capturedFiles)
    Status = 'pending'
    ClaimBoundary = 'Pending scaffold only; live Mod Settings proof still requires screenshots, log/audit files, route note, and filled checklist.'
} | ConvertTo-Json -Depth 10
