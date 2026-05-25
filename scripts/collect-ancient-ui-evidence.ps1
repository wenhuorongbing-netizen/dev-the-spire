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
    VakuuNormal = 3
    VakuuFightOptInSinglePlayer = 4
    VakuuForceFight = 1
}

$devConsoleCommands = @{
    URDA = 'ancient EZMB_URDA'
    MORVI = 'ancient EZMB_MORVI'
    LOTHA = 'ancient EZMB_LOTHA'
    VAKUU = 'ancient VAKUU'
}

$unsavedTestCommands = @{
    URDA = 'spireplus_test_ancient URDA confirm'
    MORVI = 'spireplus_test_ancient MORVI confirm'
    LOTHA = 'spireplus_test_ancient LOTHA confirm'
    VAKUU = 'spireplus_test_ancient VAKUU confirm'
    VakuuFight = 'spireplus_test_ancient VAKUU confirm fight'
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

        return $expectedOptionCounts['VakuuNormal']
    }

    return $expectedOptionCounts[$AncientName]
}

function Get-UnsavedTestCommandForRun {
    param(
        [Parameter(Mandatory = $true)][string]$AncientName,
        [switch]$VakuuFight
    )

    if ($AncientName -eq 'VAKUU' -and $VakuuFight) {
        return $unsavedTestCommands['VakuuFight']
    }

    return $unsavedTestCommands[$AncientName]
}

function New-ManualInstructions {
    param(
        [Parameter(Mandatory = $true)][string]$AncientName,
        [Parameter(Mandatory = $true)][string]$EvidenceFull,
        [Parameter(Mandatory = $true)]$ForceEnvironment,
        [Parameter(Mandatory = $true)][int]$ExpectedOptionCountForThisRun,
        [Parameter(Mandatory = $true)][string]$UnsavedTestCommand,
        [Parameter(Mandatory = $true)][string]$DevConsoleCommand,
        [Parameter(Mandatory = $true)][string]$ManualRoute,
        [Parameter(Mandatory = $true)][string]$WrapperLaunchCommand,
        [Parameter(Mandatory = $true)][string]$RestoreCommand,
        [Parameter(Mandatory = $true)]$Preflight,
        [switch]$VakuuFight
    )

    $screenshotName = "01-$($AncientName.ToLowerInvariant())-clicked-ui.png"
    $preferredEnvLines = @($ForceEnvironment.GetEnumerator() | Where-Object { $_.Key.StartsWith('SPIREPLUS_', [System.StringComparison]::Ordinal) } | ForEach-Object { "- $($_.Key)=$($_.Value)" })
    $legacyEnvLines = @($ForceEnvironment.GetEnumerator() | Where-Object { $_.Key.StartsWith('EZMB_', [System.StringComparison]::Ordinal) } | ForEach-Object { "- $($_.Key)=$($_.Value)" })
    $preflightLine = if ($Preflight['Skipped']) {
        'Prepare preflight was skipped by -NoPreflight.'
    } elseif ($Preflight['Success']) {
        "Prepare preflight wrote $($Preflight['OutFile'])."
    } else {
        "Prepare preflight did not pass: $($Preflight['Error'])"
    }

    $vakuuNote = if ($AncientName -eq 'VAKUU') {
        if ($VakuuFight) {
            'This focused run sets the unfinished Vakuu force-fight gate, so the expected visible option count is 1 fight option. For the normal Vakuu screen, expect 3 options; if SPIREPLUS_ENABLE_VAKUU_FIGHT or legacy EZMB_ENABLE_VAKUU_FIGHT is set, expect 4.'
        } else {
            'For Vakuu, the current source expects 3 options by default. Set SPIREPLUS_ENABLE_VAKUU_FIGHT=1 or legacy EZMB_ENABLE_VAKUU_FIGHT=1 only for the unfinished opt-in fight, or use -ForceVakuuFight for a focused one-option fight smoke.'
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
        'When -Launch is used, the helper sets these preferred Spire Plus process environment variables before calling the live-session helper:',
        ''
    ) + $preferredEnvLines + @(
        '',
        'The legacy aliases below are also set for compatibility with existing source gates and old local scripts:',
        ''
    ) + $legacyEnvLines + @(
        '',
        '## Open the Ancient',
        '',
        "Expected visible option count for this prepared run: $ExpectedOptionCountForThisRun.",
        $vakuuNote,
        "Preferred unsaved UI smoke command: $UnsavedTestCommand",
        "Legacy active-run DevConsole render-smoke command: $DevConsoleCommand",
        "Manual route: $ManualRoute",
        '',
        'Use the preferred Spire Plus command from the main menu to start an unsaved single-player test run and open this Ancient. Use the legacy active-run command only after a run is already in progress.',
        'Either command route is UI render smoke, not natural gameplay proof.',
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
        "    .\scripts\capture-spire-window.ps1 -OutFile `"$EvidenceFull\$screenshotName`" -RequireSpireForeground",
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

function Get-ReleaseEvidenceRowId {
    param(
        [Parameter(Mandatory = $true)][string]$AncientName,
        [switch]$VakuuFight
    )

    if ($AncientName -eq 'VAKUU') {
        if ($VakuuFight) {
            return 'ancient-ui-vakuu-fight'
        }

        return 'ancient-ui-vakuu-normal'
    }

    return "ancient-ui-$($AncientName.ToLowerInvariant())"
}

function New-ManualRows {
    param(
        [Parameter(Mandatory = $true)][string]$AncientName,
        [Parameter(Mandatory = $true)][string]$EvidenceFull,
        [Parameter(Mandatory = $true)][int]$ExpectedOptionCountForThisRun,
        [Parameter(Mandatory = $true)][string]$UnsavedTestCommand,
        [switch]$VakuuFight
    )

    $screenshotName = "01-$($AncientName.ToLowerInvariant())-clicked-ui.png"
    return @(
        [ordered]@{
            Id = Get-ReleaseEvidenceRowId -AncientName $AncientName -VakuuFight:$VakuuFight
            Feature = 'Clicked Ancient UI'
            Kind = 'clicked-ui'
            Status = 'pending'
            EvidenceDir = $EvidenceFull
            RequiredEvidence = @(
                'command.txt',
                'window-preflight.json',
                'godot.log',
                'godot-log-audit.json',
                'route-note.md',
                $screenshotName
            )
            ScreenshotFile = $screenshotName
            ResultNote = ''
            ExpectedOptionCount = $ExpectedOptionCountForThisRun
            PreferredDevConsoleCommand = $UnsavedTestCommand
            Notes = 'Fill this row only after live foreground screenshot and clean log evidence exist.'
        }
    )
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
$unsavedTestCommandForThisRun = Get-UnsavedTestCommandForRun -AncientName $ancientName -VakuuFight:$ForceVakuuFight

$wrapperLaunchArgs = @('-Mode', 'Prepare', '-Ancient', $ancientName, '-EvidenceDir', $evidenceFull)
if ($ForceVakuuFight) { $wrapperLaunchArgs += '-ForceVakuuFight' }
if ($MoveOtherMods) { $wrapperLaunchArgs += '-MoveOtherMods' }
if ($MoveCurrentRuns) { $wrapperLaunchArgs += '-MoveCurrentRuns' }
if ($NoPreflight) { $wrapperLaunchArgs += '-NoPreflight' }
$wrapperLaunchArgs += '-Launch'
$wrapperLaunchCommand = Format-DisplayCommand -Tokens (@('.\scripts\collect-ancient-ui-evidence.ps1') + $wrapperLaunchArgs)
$restoreCommand = Format-DisplayCommand -Tokens @('.\scripts\collect-ancient-ui-evidence.ps1', '-Mode', 'Restore', '-EvidenceDir', $evidenceFull)

$selfTokens = @('.\scripts\collect-ancient-ui-evidence.ps1', '-Mode', 'Prepare', '-Ancient', $ancientName, '-EvidenceDir', $evidenceFull)
if ($ForceVakuuFight) { $selfTokens += '-ForceVakuuFight' }
if ($MoveOtherMods) { $selfTokens += '-MoveOtherMods' }
if ($MoveCurrentRuns) { $selfTokens += '-MoveCurrentRuns' }
if ($NoPreflight) { $selfTokens += '-NoPreflight' }
if ($Launch) { $selfTokens += '-Launch' }
$selfCommand = Format-DisplayCommand -Tokens $selfTokens

$livePrepareArgs = @('-Mode', 'Prepare', '-EvidenceDir', $evidenceFull)
if ($MoveOtherMods) { $livePrepareArgs += '-MoveOtherMods' }
if ($MoveCurrentRuns) { $livePrepareArgs += '-MoveCurrentRuns' }
$livePrepareArgs += '-Launch'
$livePrepareCommand = Format-DisplayCommand -Tokens (@('.\scripts\spire-plus-live-session.ps1') + $livePrepareArgs)

$preflight = New-PreparePreflight -EvidenceFull $evidenceFull -Skip:$NoPreflight -WillLaunch:$Launch

$planPath = Join-Path $evidenceFull 'ancient-ui-evidence-plan.json'
$instructionsPath = Join-Path $evidenceFull 'manual-instructions.md'
$commandPath = Join-Path $evidenceFull 'command.txt'
$environmentPath = Join-Path $evidenceFull 'environment.json'
$packageHashesPath = Join-Path $evidenceFull 'package-hashes.json'
$manualRowsPath = Join-Path $evidenceFull 'manual-rows-template.json'
$logAuditTemplatePath = Join-Path $evidenceFull 'log-audit-template.json'
$plan = [ordered]@{
    Ancient = $ancientName
    CreatedAt = (Get-Date).ToString('o')
    EvidenceDir = $evidenceFull
    ForceEnvironment = $forceEnvironment
    ExpectedOptionCounts = $expectedOptionCounts
    ExpectedOptionCountForThisRun = $expectedOptionCountForThisRun
    PreferredUnsavedDevConsoleCommand = $unsavedTestCommandForThisRun
    ExpectedDevConsoleCommand = $unsavedTestCommandForThisRun
    LegacyActiveRunDevConsoleCommand = $devConsoleCommands[$ancientName]
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

$environment = [ordered]@{
    CreatedAt = (Get-Date).ToString('o')
    EvidenceKind = 'ancient-ui-clicked-evidence'
    RepositoryRoot = $repoRoot
    GitHead = Get-GitValue -Arguments @('rev-parse', 'HEAD')
    GitStatusShort = Get-GitValue -Arguments @('status', '--short')
    Ancient = $ancientName
    ForceVakuuFight = [bool]$ForceVakuuFight
    LaunchRequested = [bool]$Launch
    NoLaunch = -not [bool]$Launch
    NoPreflight = [bool]$NoPreflight
    MoveOtherMods = [bool]$MoveOtherMods
    MoveCurrentRuns = [bool]$MoveCurrentRuns
    ForceEnvironment = $forceEnvironment
    ExpectedOptionCountForThisRun = $expectedOptionCountForThisRun
    PreferredUnsavedDevConsoleCommand = $unsavedTestCommandForThisRun
    ExpectedDevConsoleCommand = $unsavedTestCommandForThisRun
    LegacyActiveRunDevConsoleCommand = $devConsoleCommands[$ancientName]
    ManualRouteInstruction = $manualRoutes[$ancientName]
    Scripts = [ordered]@{
        Preflight = $preflightScript
        LiveSession = $liveSessionScript
    }
    LaunchCommandSummary = [ordered]@{
        PrepareCommand = $selfCommand
        TesterCommand = $wrapperLaunchCommand
        LiveSessionCommand = $livePrepareCommand
        RestoreCommand = $restoreCommand
    }
}

$packageHashes = [ordered]@{
    CreatedAt = (Get-Date).ToString('o')
    Files = @(
        Get-HashRow -RelativePath 'EZMicroBalance.json'
        Get-HashRow -RelativePath 'publish\SpirePlus-v0.1.0-private-beta.20.zip'
        Get-HashRow -RelativePath 'publish\SpirePlus-v0.1.0-private-beta.20\EZMicroBalance\EZMicroBalance.dll'
        Get-HashRow -RelativePath 'publish\SpirePlus-v0.1.0-private-beta.20\EZMicroBalance\EZMicroBalance.pck'
        Get-HashRow -RelativePath 'publish\SpirePlus-v0.1.0-private-beta.20\EZMicroBalance\EZMicroBalance.json'
        Get-HashRow -RelativePath 'publish\SpirePlus-v0.1.0-private-beta.20\EZMicroBalance\README_INSTALL.txt'
    )
}

$commandLines = @(
    "Prepare command: $selfCommand",
    "Tester launch command: $wrapperLaunchCommand",
    "Live-session command used by -Launch: $livePrepareCommand",
    "Restore command: $restoreCommand",
    "Preferred UI-smoke command: $unsavedTestCommandForThisRun",
    "Legacy active-run render-smoke command: $($devConsoleCommands[$ancientName])"
)
$commandLines -join [Environment]::NewLine | Set-Content -LiteralPath $commandPath -Encoding UTF8
Save-Json -InputObject $environment -Path $environmentPath
Save-Json -InputObject $packageHashes -Path $packageHashesPath
Save-Json -InputObject ([ordered]@{
    Rows = @(New-ManualRows `
            -AncientName $ancientName `
            -EvidenceFull $evidenceFull `
            -ExpectedOptionCountForThisRun $expectedOptionCountForThisRun `
            -UnsavedTestCommand $unsavedTestCommandForThisRun `
            -VakuuFight:$ForceVakuuFight)
}) -Path $manualRowsPath
Save-Json -InputObject ([ordered]@{
    Status = 'pending'
    RequiredFiles = @('godot.log', 'godot-log-audit.json')
    BlockingPatterns = @('ERROR', 'Exception', '[SPIREPLUS-EVIDENCE]', '[EZMB-EVIDENCE]')
    Notes = 'Fill with audit output after copying live logs. This template is not a pass marker.'
}) -Path $logAuditTemplatePath

$instructions = New-ManualInstructions `
    -AncientName $ancientName `
    -EvidenceFull $evidenceFull `
    -ForceEnvironment $forceEnvironment `
    -ExpectedOptionCountForThisRun $expectedOptionCountForThisRun `
    -UnsavedTestCommand $unsavedTestCommandForThisRun `
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
