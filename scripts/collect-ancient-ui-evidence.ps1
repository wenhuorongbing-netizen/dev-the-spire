param(
    [ValidateSet('Prepare', 'Restore')]
    [string]$Mode = 'Prepare',

    [ValidateSet('URDA', 'MORVI', 'LOTHA', 'VAKUU')]
    [string]$Ancient,

    [string]$EvidenceDir,

    [switch]$ForceVakuuFight,

    [switch]$MoveOtherMods,

    [switch]$MoveCurrentRuns,

    [switch]$Launch,

    [switch]$NoPreflight
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
$runtimeRoot = Join-Path $repoRoot '.tools\runtime-evidence'
$preflightScript = Join-Path $PSScriptRoot 'check-spire-window-preflight.ps1'
$liveSessionScript = Join-Path $PSScriptRoot 'spire-plus-live-session.ps1'

$expectedOptionCounts = [ordered]@{
    URDA = 4
    MORVI = 3
    LOTHA = 3
    VakuuFightEnabledSinglePlayer = 4
    VakuuFightDisabledOrIneligible = 3
    VakuuForceFight = 1
}

$devConsoleCommands = @{
    URDA = 'ancient EZMB_URDA'
    MORVI = 'ancient EZMB_MORVI'
    LOTHA = 'ancient EZMB_LOTHA'
    VAKUU = 'ancient VAKUU'
}

$manualRoutes = @{
    URDA = 'Start or continue a single-player run and click the forced Act 1 Urda Ancient.'
    MORVI = 'Start or continue a single-player run and click the forced Act 2 Morvi Ancient.'
    LOTHA = 'Start or continue a single-player run and click the forced Act 3 Lotha Ancient.'
    VAKUU = 'Start or continue a single-player run and click the forced Act 3 Vakuu Ancient.'
}

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
    param(
        [string]$RequestedPath,
        [string]$AncientName
    )

    New-DirectoryIfMissing -Path $runtimeRoot

    if ([string]::IsNullOrWhiteSpace($RequestedPath)) {
        $stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
        $leaf = "ancient-ui-click-$($AncientName.ToLowerInvariant())-$stamp"
        return [System.IO.Path]::GetFullPath((Join-Path $runtimeRoot $leaf))
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

function Set-ProcessEnvironment {
    param([Parameter(Mandatory = $true)]$Variables)

    $previous = @{}
    foreach ($entry in $Variables.GetEnumerator()) {
        $previous[$entry.Key] = [Environment]::GetEnvironmentVariable($entry.Key, 'Process')
        [Environment]::SetEnvironmentVariable($entry.Key, [string]$entry.Value, 'Process')
    }

    return $previous
}

function Restore-ProcessEnvironment {
    param([Parameter(Mandatory = $true)]$PreviousValues)

    foreach ($entry in $PreviousValues.GetEnumerator()) {
        [Environment]::SetEnvironmentVariable($entry.Key, $entry.Value, 'Process')
    }
}

function Get-ForceEnvironment {
    param(
        [Parameter(Mandatory = $true)][string]$AncientName,
        [switch]$VakuuFight
    )

    $variables = [ordered]@{
        SPIREPLUS_FORCE_ANCIENT = $AncientName
        EZMB_FORCE_ANCIENT = $AncientName
    }

    if ($AncientName -eq 'VAKUU' -and $VakuuFight) {
        $variables['SPIREPLUS_FORCE_VAKUU_FIGHT'] = '1'
        $variables['EZMB_FORCE_VAKUU_FIGHT'] = '1'
    }

    return $variables
}

function Get-ExpectedOptionCountForRun {
    param(
        [Parameter(Mandatory = $true)][string]$AncientName,
        [switch]$VakuuFight
    )

    if ($AncientName -eq 'VAKUU') {
        if ($VakuuFight) {
            return $expectedOptionCounts['VakuuForceFight']
        }

        return $expectedOptionCounts['VakuuFightEnabledSinglePlayer']
    }

    return $expectedOptionCounts[$AncientName]
}

function New-ManualInstructions {
    param(
        [Parameter(Mandatory = $true)][string]$AncientName,
        [Parameter(Mandatory = $true)][string]$EvidenceFull,
        [Parameter(Mandatory = $true)]$ForceEnvironment,
        [Parameter(Mandatory = $true)][int]$ExpectedOptionCountForThisRun,
        [Parameter(Mandatory = $true)][string]$DevConsoleCommand,
        [Parameter(Mandatory = $true)][string]$ManualRoute,
        [Parameter(Mandatory = $true)][string]$WrapperLaunchCommand,
        [Parameter(Mandatory = $true)][string]$RestoreCommand,
        [Parameter(Mandatory = $true)]$Preflight,
        [switch]$VakuuFight
    )

    $screenshotName = "01-$($AncientName.ToLowerInvariant())-clicked-ui.png"
    $envLines = @($ForceEnvironment.GetEnumerator() | ForEach-Object { "- $($_.Key)=$($_.Value)" })
    $preflightLine = if ($Preflight['Skipped']) {
        'Prepare preflight was skipped by -NoPreflight.'
    } elseif ($Preflight['Success']) {
        "Prepare preflight wrote $($Preflight['OutFile'])."
    } else {
        "Prepare preflight did not pass: $($Preflight['Error'])"
    }

    $vakuuNote = if ($AncientName -eq 'VAKUU') {
        if ($VakuuFight) {
            'This focused run sets the current source force-fight gate, so the expected visible option count is 1 fight option. For the normal single-player fight-enabled screen, expect 4 options; with the fight gate disabled or ineligible, expect 3.'
        } else {
            'For Vakuu, the current source expects 4 options in normal single-player when the fight gate is enabled, or 3 if the fight gate is disabled or ineligible. Use -ForceVakuuFight only for a focused one-option fight smoke.'
        }
    } else {
        ''
    }

    $lines = @(
        "# Ancient UI Evidence Instructions: $AncientName",
        '',
        "Evidence directory: $EvidenceFull",
        '',
        'Known pending result: this helper prepares evidence. It does not prove clicked UI by itself.',
        '',
        '## Force environment for launched process',
        '',
        'When -Launch is used, the helper sets these process environment variables before calling the live-session helper:',
        ''
    ) + $envLines + @(
        '',
        '## Open the Ancient',
        '',
        "Expected visible option count for this prepared run: $ExpectedOptionCountForThisRun.",
        $vakuuNote,
        "DevConsole render-smoke command: $DevConsoleCommand",
        "Manual route: $ManualRoute",
        '',
        'Use DevConsole only when this row is being recorded as UI render smoke, not natural gameplay proof.',
        '',
        '## Capture files under this evidence directory',
        '',
        "- Screenshot: $screenshotName",
        '- Foreground preflight before the screenshot: window-preflight.json',
        '- Copied game log after the screenshot: godot.log',
        '- Log audit after copying the log: godot-log-audit.json',
        '- Route note: route-note.md, stating natural map click or DevConsole render smoke',
        '',
        'Recommended capture commands after the game is foreground on the Ancient UI:',
        '',
        "    .\scripts\check-spire-window-preflight.ps1 -OutFile `"$EvidenceFull\window-preflight.json`" -RequireSpireForeground",
        "    Copy-Item `"$env:APPDATA\SlayTheSpire2\logs\godot.log`" `"$EvidenceFull\godot.log`" -Force",
        "    .\scripts\audit-godot-log.ps1 -Path `"$EvidenceFull\godot.log`" -OutFile `"$EvidenceFull\godot-log-audit.json`" -FailOnHit",
        '',
        '## Launch and restore',
        '',
        "Launch command for the tester: $WrapperLaunchCommand",
        "Restore command after evidence is captured: $RestoreCommand",
        '',
        $preflightLine,
        '',
        'Do not mark clicked Ancient UI verified until the screenshot, foreground preflight, copied log, log audit, and route note exist in this directory.'
    )

    return ($lines | Where-Object { $null -ne $_ }) -join [Environment]::NewLine
}

function New-PreparePreflight {
    param(
        [Parameter(Mandatory = $true)][string]$EvidenceFull,
        [switch]$Skip,
        [switch]$WillLaunch
    )

    $preflightOut = Join-Path $EvidenceFull 'prepare-window-preflight.json'
    $preflight = [ordered]@{
        Skipped = [bool]$Skip
        Script = $preflightScript
        OutFile = $preflightOut
        Success = $false
        ExitCode = $null
        Error = $null
        BlocksLaunch = $false
        SpireRunningBeforeLaunch = $null
        SpireForegroundBeforeLaunch = $null
    }

    if ($Skip) {
        $preflight['Success'] = $true
        return $preflight
    }

    if (-not (Test-Path -LiteralPath $preflightScript)) {
        $preflight['Error'] = "Missing preflight script: $preflightScript"
        $preflight['BlocksLaunch'] = [bool]$WillLaunch
        return $preflight
    }

    $result = Invoke-PowerShellFile -ScriptPath $preflightScript -ArgumentList @('-OutFile', $preflightOut)
    $preflight['ExitCode'] = $result.ExitCode
    if ($result.ExitCode -ne 0) {
        $preflight['Error'] = "Preflight command exited with $($result.ExitCode)."
        $preflight['BlocksLaunch'] = [bool]$WillLaunch
        return $preflight
    }

    if (Test-Path -LiteralPath $preflightOut) {
        try {
            $preflightJson = Get-Content -LiteralPath $preflightOut -Raw -Encoding UTF8 | ConvertFrom-Json
            $preflight['SpireRunningBeforeLaunch'] = [bool]$preflightJson.SpireRunning
            $preflight['SpireForegroundBeforeLaunch'] = [bool]$preflightJson.SpireForeground
            if ($WillLaunch -and [bool]$preflightJson.SpireRunning) {
                $preflight['Error'] = 'SlayTheSpire2 appears to be running before launch. Restore or close the game before preparing a new forced-Ancient session.'
                $preflight['BlocksLaunch'] = $true
                return $preflight
            }
        } catch {
            $preflight['Error'] = "Could not parse preflight output: $($_.Exception.Message)"
            $preflight['BlocksLaunch'] = [bool]$WillLaunch
            return $preflight
        }
    }

    $preflight['Success'] = $true
    return $preflight
}

function Invoke-Restore {
    param([Parameter(Mandatory = $true)][string]$EvidenceFull)

    if (-not (Test-Path -LiteralPath $liveSessionScript)) {
        throw "Missing live-session helper: $liveSessionScript"
    }

    $notePath = Join-Path $EvidenceFull 'ancient-ui-evidence-restore-note.md'
    Add-Content -LiteralPath $notePath -Encoding UTF8 -Value "Restore requested: $((Get-Date).ToString('o'))"

    $restoreArgs = @(
        '-Mode', 'Restore',
        '-EvidenceDir', $EvidenceFull,
        '-StopGameOnRestore',
        '-PreserveNewCurrentRunsOnRestore'
    )

    $result = Invoke-PowerShellFile -ScriptPath $liveSessionScript -ArgumentList $restoreArgs
    if ($result.Output.Count -gt 0) {
        $result.Output | Write-Output
    }

    if ($result.ExitCode -ne 0) {
        Add-Content -LiteralPath $notePath -Encoding UTF8 -Value "Restore failed with exit code $($result.ExitCode): $((Get-Date).ToString('o'))"
        throw "Live-session restore failed with exit code $($result.ExitCode). See $notePath."
    }

    Add-Content -LiteralPath $notePath -Encoding UTF8 -Value "Restore completed: $((Get-Date).ToString('o'))"
}

if ($Mode -eq 'Restore') {
    if (-not $EvidenceDir) {
        throw 'Pass -EvidenceDir when restoring Ancient UI evidence state.'
    }

    $restoreEvidenceFull = Get-EvidenceFullPath -RequestedPath $EvidenceDir -AncientName 'restore'
    Assert-PathInside -Child $restoreEvidenceFull -Parent $runtimeRoot -Label 'Evidence'
    Invoke-Restore -EvidenceFull $restoreEvidenceFull
    exit 0
}

if (-not $Ancient) {
    throw 'Pass -Ancient in Prepare mode.'
}

$ancientName = $Ancient.ToUpperInvariant()
if ($ForceVakuuFight -and $ancientName -ne 'VAKUU') {
    throw '-ForceVakuuFight is valid only when -Ancient VAKUU is used.'
}

$evidenceFull = Get-EvidenceFullPath -RequestedPath $EvidenceDir -AncientName $ancientName
Assert-PathInside -Child $evidenceFull -Parent $runtimeRoot -Label 'Evidence'
New-DirectoryIfMissing -Path $evidenceFull

$forceEnvironment = Get-ForceEnvironment -AncientName $ancientName -VakuuFight:$ForceVakuuFight
$expectedOptionCountForThisRun = Get-ExpectedOptionCountForRun -AncientName $ancientName -VakuuFight:$ForceVakuuFight

$wrapperLaunchArgs = @('-Mode', 'Prepare', '-Ancient', $ancientName, '-EvidenceDir', $evidenceFull)
if ($ForceVakuuFight) { $wrapperLaunchArgs += '-ForceVakuuFight' }
if ($MoveOtherMods) { $wrapperLaunchArgs += '-MoveOtherMods' }
if ($MoveCurrentRuns) { $wrapperLaunchArgs += '-MoveCurrentRuns' }
if ($NoPreflight) { $wrapperLaunchArgs += '-NoPreflight' }
$wrapperLaunchArgs += '-Launch'
$wrapperLaunchCommand = Format-DisplayCommand -Tokens (@('.\scripts\collect-ancient-ui-evidence.ps1') + $wrapperLaunchArgs)
$restoreCommand = Format-DisplayCommand -Tokens @('.\scripts\collect-ancient-ui-evidence.ps1', '-Mode', 'Restore', '-EvidenceDir', $evidenceFull)

$livePrepareArgs = @('-Mode', 'Prepare', '-EvidenceDir', $evidenceFull)
if ($MoveOtherMods) { $livePrepareArgs += '-MoveOtherMods' }
if ($MoveCurrentRuns) { $livePrepareArgs += '-MoveCurrentRuns' }
$livePrepareArgs += '-Launch'
$livePrepareCommand = Format-DisplayCommand -Tokens (@('.\scripts\spire-plus-live-session.ps1') + $livePrepareArgs)

$preflight = New-PreparePreflight -EvidenceFull $evidenceFull -Skip:$NoPreflight -WillLaunch:$Launch

$planPath = Join-Path $evidenceFull 'ancient-ui-evidence-plan.json'
$instructionsPath = Join-Path $evidenceFull 'manual-instructions.md'
$plan = [ordered]@{
    Ancient = $ancientName
    CreatedAt = (Get-Date).ToString('o')
    EvidenceDir = $evidenceFull
    ForceEnvironment = $forceEnvironment
    ExpectedOptionCounts = $expectedOptionCounts
    ExpectedOptionCountForThisRun = $expectedOptionCountForThisRun
    ExpectedDevConsoleCommand = $devConsoleCommands[$ancientName]
    ManualRouteInstruction = $manualRoutes[$ancientName]
    ForceVakuuFight = [bool]$ForceVakuuFight
    LaunchRequested = [bool]$Launch
    MoveOtherMods = [bool]$MoveOtherMods
    MoveCurrentRuns = [bool]$MoveCurrentRuns
    NoPreflight = [bool]$NoPreflight
    Preflight = $preflight
    HelperScripts = [ordered]@{
        Preflight = $preflightScript
        LiveSession = $liveSessionScript
    }
    LaunchCommandSummary = [ordered]@{
        TesterCommand = $wrapperLaunchCommand
        LiveSessionCommand = $livePrepareCommand
    }
    RequiredEvidenceFiles = @(
        "01-$($ancientName.ToLowerInvariant())-clicked-ui.png",
        'window-preflight.json',
        'godot.log',
        'godot-log-audit.json',
        'route-note.md'
    )
    PendingNotice = 'This helper prepares evidence. It does not prove clicked UI by itself.'
}

Save-Json -InputObject $plan -Path $planPath

$instructions = New-ManualInstructions `
    -AncientName $ancientName `
    -EvidenceFull $evidenceFull `
    -ForceEnvironment $forceEnvironment `
    -ExpectedOptionCountForThisRun $expectedOptionCountForThisRun `
    -DevConsoleCommand $devConsoleCommands[$ancientName] `
    -ManualRoute $manualRoutes[$ancientName] `
    -WrapperLaunchCommand $wrapperLaunchCommand `
    -RestoreCommand $restoreCommand `
    -Preflight $preflight `
    -VakuuFight:$ForceVakuuFight
$instructions | Set-Content -LiteralPath $instructionsPath -Encoding UTF8

if ($preflight['BlocksLaunch']) {
    Write-Output "Prepared Ancient UI evidence files under $evidenceFull."
    Write-Output "Launch skipped because preflight did not pass. See $planPath and $instructionsPath."
    throw 'Preflight blocked launch.'
}

if (-not $Launch) {
    Write-Output "Prepared Ancient UI evidence files under $evidenceFull."
    Write-Output "Plan: $planPath"
    Write-Output "Manual instructions: $instructionsPath"
    Write-Output "Launch command: $wrapperLaunchCommand"
    exit 0
}

if (-not (Test-Path -LiteralPath $liveSessionScript)) {
    throw "Missing live-session helper: $liveSessionScript"
}

$previousEnvironment = Set-ProcessEnvironment -Variables $forceEnvironment
try {
    $result = Invoke-PowerShellFile -ScriptPath $liveSessionScript -ArgumentList $livePrepareArgs
    if ($result.Output.Count -gt 0) {
        $result.Output | Write-Output
    }

    $plan['LiveSessionExitCode'] = $result.ExitCode
    $plan['LiveSessionCompletedAt'] = (Get-Date).ToString('o')
    Save-Json -InputObject $plan -Path $planPath

    if ($result.ExitCode -ne 0) {
        throw "Live-session prepare failed with exit code $($result.ExitCode)."
    }
} finally {
    Restore-ProcessEnvironment -PreviousValues $previousEnvironment
}

Write-Output "Ancient UI live session launched for $ancientName. Follow $instructionsPath, then run: $restoreCommand"
