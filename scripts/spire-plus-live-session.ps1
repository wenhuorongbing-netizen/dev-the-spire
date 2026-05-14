param(
    [ValidateSet('Prepare', 'Restore')]
    [string]$Mode = 'Prepare',

    [string]$EvidenceDir,

    [string]$GameRoot = 'D:\Steam\steamapps\common\Slay the Spire 2',

    [string]$SteamExe = 'D:\Steam\steam.exe',

    [string]$SteamUserId,

    [ValidateSet('', 'eng', 'zhs')]
    [string]$Language = '',

    [switch]$DisableSpirePlus,

    [switch]$MoveOtherMods,

    [switch]$MoveCurrentRuns,

    [switch]$Launch,

    [switch]$StopGameOnRestore,

    [switch]$PreserveNewCurrentRunsOnRestore
)

$ErrorActionPreference = 'Stop'

$repoRoot = Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')
$runtimeRoot = Join-Path $repoRoot.Path '.tools\runtime-evidence'
$defaultAllowedModIds = @('BaseLib', 'EZMicroBalance')

function New-DirectoryIfMissing {
    param([Parameter(Mandatory = $true)][string]$Path)
    if (-not (Test-Path -LiteralPath $Path)) {
        New-Item -ItemType Directory -Path $Path -Force | Out-Null
    }
}

function Get-ResolvedFullPath {
    param([Parameter(Mandatory = $true)][string]$Path)
    if (Test-Path -LiteralPath $Path) {
        return (Resolve-Path -LiteralPath $Path).Path
    }

    $parent = Split-Path -Parent $Path
    $leaf = Split-Path -Leaf $Path
    $resolvedParent = Resolve-Path -LiteralPath $parent
    return (Join-Path $resolvedParent.Path $leaf)
}

function Assert-PathInside {
    param(
        [Parameter(Mandatory = $true)][string]$Child,
        [Parameter(Mandatory = $true)][string]$Parent,
        [Parameter(Mandatory = $true)][string]$Label
    )

    $childFull = [System.IO.Path]::GetFullPath($Child).TrimEnd('\')
    $parentFull = [System.IO.Path]::GetFullPath($Parent).TrimEnd('\')
    $comparison = [System.StringComparison]::OrdinalIgnoreCase

    if ($childFull.Equals($parentFull, $comparison)) {
        return
    }

    if (-not $childFull.StartsWith($parentFull + '\', $comparison)) {
        throw "$Label path is outside expected root. Path: $childFull Root: $parentFull"
    }
}

function Get-HashOrNull {
    param([Parameter(Mandatory = $true)][string]$Path)
    if (Test-Path -LiteralPath $Path) {
        return (Get-FileHash -Algorithm SHA256 -LiteralPath $Path).Hash.ToUpperInvariant()
    }

    return $null
}

function Get-SteamUserRoot {
    param([string]$RequestedSteamUserId)

    $steamRoot = Join-Path $env:APPDATA 'SlayTheSpire2\steam'
    if ($RequestedSteamUserId) {
        $candidate = Join-Path $steamRoot $RequestedSteamUserId
        if (-not (Test-Path -LiteralPath $candidate)) {
            throw "Steam user directory not found: $candidate"
        }

        return (Resolve-Path -LiteralPath $candidate).Path
    }

    $settingsOwners = @(Get-ChildItem -LiteralPath $steamRoot -Directory -ErrorAction SilentlyContinue |
        Where-Object { Test-Path -LiteralPath (Join-Path $_.FullName 'settings.save') })

    if ($settingsOwners.Count -ne 1) {
        $ids = ($settingsOwners | ForEach-Object { $_.Name }) -join ', '
        throw "Could not infer Steam user id. Pass -SteamUserId. Candidates: $ids"
    }

    return $settingsOwners[0].FullName
}

function Save-Json {
    param(
        [Parameter(Mandatory = $true)]$InputObject,
        [Parameter(Mandatory = $true)][string]$Path
    )

    $InputObject | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $Path -Encoding UTF8
}

function Set-SpirePlusSettings {
    param(
        [Parameter(Mandatory = $true)][string]$SettingsPath,
        [string]$RequestedLanguage,
        [switch]$DisableSpirePlus
    )

    $settings = Get-Content -LiteralPath $SettingsPath -Raw -Encoding UTF8 | ConvertFrom-Json

    if ($RequestedLanguage) {
        $settings.language = $RequestedLanguage
    }

    if (-not $settings.PSObject.Properties['mod_settings']) {
        $settings | Add-Member -MemberType NoteProperty -Name mod_settings -Value ([pscustomobject]@{})
    }

    if (-not $settings.mod_settings.PSObject.Properties['mod_list']) {
        $settings.mod_settings | Add-Member -MemberType NoteProperty -Name mod_list -Value @()
    }

    if (-not $settings.PSObject.Properties['mods_enabled']) {
        $settings | Add-Member -MemberType NoteProperty -Name mods_enabled -Value $true
    } else {
        $settings.mods_enabled = $true
    }

    $enabledMods = @(
        [pscustomobject]@{
            id = 'BaseLib'
            is_enabled = $true
            source = 'mods_directory'
        }
    )

    if (-not $DisableSpirePlus) {
        $enabledMods += [pscustomobject]@{
            id = 'EZMicroBalance'
            is_enabled = $true
            source = 'mods_directory'
        }
    }

    $settings.mod_settings.mod_list = $enabledMods

    Save-Json -InputObject $settings -Path $SettingsPath
}

function Move-ModsForIsolation {
    param(
        [Parameter(Mandatory = $true)][string]$ModsRoot,
        [Parameter(Mandatory = $true)][string]$DestinationRoot,
        [Parameter(Mandatory = $true)][string[]]$AllowedModIds
    )

    New-DirectoryIfMissing -Path $DestinationRoot
    Assert-PathInside -Child $DestinationRoot -Parent $runtimeRoot -Label 'Mod isolation destination'

    $moved = @()
    foreach ($entry in Get-ChildItem -LiteralPath $ModsRoot -Force) {
        if ($AllowedModIds -contains $entry.Name) {
            continue
        }

        $destination = Join-Path $DestinationRoot $entry.Name
        if (Test-Path -LiteralPath $destination) {
            throw "Isolation destination already exists: $destination"
        }

        Assert-PathInside -Child $entry.FullName -Parent $ModsRoot -Label 'Mod isolation source'
        Move-Item -LiteralPath $entry.FullName -Destination $destination
        $moved += [pscustomobject]@{
            Name = $entry.Name
            From = $entry.FullName
            To = $destination
        }
    }

    return $moved
}

function Move-CurrentRuns {
    param(
        [Parameter(Mandatory = $true)][string[]]$SaveRoots,
        [Parameter(Mandatory = $true)][string]$DestinationRoot
    )

    $runNames = @(
        'current_run.save',
        'current_run.save.backup',
        'current_run_mp.save',
        'current_run_mp.save.backup'
    )

    New-DirectoryIfMissing -Path $DestinationRoot
    Assert-PathInside -Child $DestinationRoot -Parent $runtimeRoot -Label 'Current-run destination'

    $moved = @()
    foreach ($saveRoot in $SaveRoots) {
        if (-not (Test-Path -LiteralPath $saveRoot)) {
            continue
        }

        foreach ($runName in $runNames) {
            $source = Join-Path $saveRoot $runName
            if (-not (Test-Path -LiteralPath $source)) {
                continue
            }

            $relativeBucket = ($saveRoot -replace '[:\\]+', '_').Trim('_')
            $destinationDir = Join-Path $DestinationRoot $relativeBucket
            New-DirectoryIfMissing -Path $destinationDir
            $destination = Join-Path $destinationDir $runName
            if (Test-Path -LiteralPath $destination) {
                throw "Current-run destination already exists: $destination"
            }

            Assert-PathInside -Child $source -Parent $saveRoot -Label 'Current-run source'
            Move-Item -LiteralPath $source -Destination $destination
            $moved += [pscustomobject]@{
                Name = $runName
                From = $source
                To = $destination
            }
        }
    }

    return $moved
}

function Restore-MovedItems {
    param(
        [Parameter(Mandatory = $true)]$Items,
        [Parameter(Mandatory = $true)][string]$DestinationRoot,
        [Parameter(Mandatory = $true)][string]$SourceRoot,
        [Parameter(Mandatory = $true)][string]$Label
    )

    $restored = @()
    foreach ($item in @($Items)) {
        if (-not $item.To -or -not (Test-Path -LiteralPath $item.To)) {
            continue
        }

        if (Test-Path -LiteralPath $item.From) {
            throw "$Label restore target already exists: $($item.From)"
        }

        Assert-PathInside -Child $item.To -Parent $SourceRoot -Label "$Label restore source"
        Assert-PathInside -Child $item.From -Parent $DestinationRoot -Label "$Label restore destination"
        New-DirectoryIfMissing -Path (Split-Path -Parent $item.From)
        Move-Item -LiteralPath $item.To -Destination $item.From
        $restored += $item
    }

    return $restored
}

function Move-NewCurrentRunsBeforeRestore {
    param(
        [Parameter(Mandatory = $true)]$Items,
        [Parameter(Mandatory = $true)][string]$EvidenceRoot,
        [Parameter(Mandatory = $true)][string]$DestinationRoot
    )

    $destinationRootFull = [System.IO.Path]::GetFullPath($DestinationRoot).TrimEnd('\')
    $preserveRoot = Join-Path $EvidenceRoot 'test-created-current-runs-before-restore'
    New-DirectoryIfMissing -Path $preserveRoot
    Assert-PathInside -Child $preserveRoot -Parent $runtimeRoot -Label 'Test-created current-run preservation'

    $moved = @()
    foreach ($item in @($Items)) {
        if (-not $item.From) {
            continue
        }

        $target = [System.IO.Path]::GetFullPath($item.From)
        if (-not $target.StartsWith($destinationRootFull + '\', [System.StringComparison]::OrdinalIgnoreCase)) {
            continue
        }

        if (-not (Test-Path -LiteralPath $target)) {
            continue
        }

        $bucket = ($destinationRootFull -replace '[:\\]+', '_').Trim('_')
        $preserveDir = Join-Path $preserveRoot $bucket
        New-DirectoryIfMissing -Path $preserveDir
        $preservePath = Join-Path $preserveDir (Split-Path -Leaf $target)
        if (Test-Path -LiteralPath $preservePath) {
            $stamp = Get-Date -Format 'yyyyMMdd-HHmmss-fff'
            $preservePath = Join-Path $preserveDir "$stamp-$((Split-Path -Leaf $target))"
        }

        Assert-PathInside -Child $target -Parent $DestinationRoot -Label 'Test-created current-run source'
        Move-Item -LiteralPath $target -Destination $preservePath
        $moved += [pscustomobject]@{
            Name = Split-Path -Leaf $target
            From = $target
            To = $preservePath
        }
    }

    if ($moved.Count -gt 0) {
        Save-Json -InputObject @($moved) -Path (Join-Path $preserveRoot 'moved-test-created-current-runs.json')
    }

    return $moved
}

function Stop-SpireProcesses {
    $targets = @(Get-Process | Where-Object {
        $_.ProcessName -like '*Slay*' -or $_.ProcessName -eq 'Godot_v4.5.1-stable_mono_win64'
    })

    foreach ($process in $targets) {
        Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
    }

    return $targets
}

if ($Mode -eq 'Prepare') {
    if ($DisableSpirePlus -and -not $MoveOtherMods) {
        throw 'DisableSpirePlus requires -MoveOtherMods so EZMicroBalance is temporarily isolated out of the mods folder and restored afterward.'
    }

    if (-not $EvidenceDir) {
        $stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
        $evidencePrefix = if ($DisableSpirePlus) { 'live-spire-plus-disabled-session' } else { 'live-spire-plus-session' }
        $EvidenceDir = Join-Path $runtimeRoot "$evidencePrefix-$stamp"
    }

    New-DirectoryIfMissing -Path $EvidenceDir
    $evidenceFull = Get-ResolvedFullPath -Path $EvidenceDir
    Assert-PathInside -Child $evidenceFull -Parent $runtimeRoot -Label 'Evidence'

    $gameRootFull = Get-ResolvedFullPath -Path $GameRoot
    $modsRoot = Join-Path $gameRootFull 'mods'
    $steamUserRoot = Get-SteamUserRoot -RequestedSteamUserId $SteamUserId
    $settingsPath = Join-Path $steamUserRoot 'settings.save'
    $settingsBackupPath = Join-Path $steamUserRoot 'settings.save.backup'
    $steamSaves = Join-Path $steamUserRoot 'modded\profile1\saves'
    $defaultSaves = Join-Path $env:APPDATA 'SlayTheSpire2\default\1\modded\profile1\saves'
    $logPath = Join-Path $env:APPDATA 'SlayTheSpire2\logs\godot.log'

    Copy-Item -LiteralPath $settingsPath -Destination (Join-Path $evidenceFull 'settings.save.before') -Force
    if (Test-Path -LiteralPath $settingsBackupPath) {
        Copy-Item -LiteralPath $settingsBackupPath -Destination (Join-Path $evidenceFull 'settings.save.backup.before') -Force
    }
    if (Test-Path -LiteralPath $logPath) {
        Copy-Item -LiteralPath $logPath -Destination (Join-Path $evidenceFull 'godot.log.before') -Force
    }

    $movedMods = @()
    $allowedModIds = if ($DisableSpirePlus) { @('BaseLib') } else { $defaultAllowedModIds }
    if ($MoveOtherMods) {
        $movedMods = Move-ModsForIsolation -ModsRoot $modsRoot -DestinationRoot (Join-Path $evidenceFull 'isolated-mods') -AllowedModIds $allowedModIds
    }

    $movedRuns = @()
    if ($MoveCurrentRuns) {
        $movedRuns = Move-CurrentRuns -SaveRoots @($steamSaves, $defaultSaves) -DestinationRoot (Join-Path $evidenceFull 'temporarily-removed-current-runs')
    }

    Set-SpirePlusSettings -SettingsPath $settingsPath -RequestedLanguage $Language -DisableSpirePlus:$DisableSpirePlus

    $state = [ordered]@{
        EvidenceDir = $evidenceFull
        PreparedAt = (Get-Date).ToString('o')
        GameRoot = $gameRootFull
        ModsRoot = $modsRoot
        SteamExe = $SteamExe
        SteamUserRoot = $steamUserRoot
        SettingsPath = $settingsPath
        SettingsBackupPath = $settingsBackupPath
        SteamSaves = $steamSaves
        DefaultSaves = $defaultSaves
        LogPath = $logPath
        Language = $Language
        DisableSpirePlus = [bool]$DisableSpirePlus
        AllowedModIds = @($allowedModIds)
        MoveOtherMods = [bool]$MoveOtherMods
        MoveCurrentRuns = [bool]$MoveCurrentRuns
        MovedMods = @($movedMods)
        MovedCurrentRuns = @($movedRuns)
        SettingsHashBefore = Get-HashOrNull -Path (Join-Path $evidenceFull 'settings.save.before')
        SettingsHashAfterPrepare = Get-HashOrNull -Path $settingsPath
    }

    if ($Launch) {
        if (-not (Test-Path -LiteralPath $SteamExe)) {
            throw "Steam executable not found: $SteamExe"
        }

        $process = Start-Process -FilePath $SteamExe -ArgumentList @('-applaunch', '2868840') -PassThru
        $state['LaunchedProcessId'] = $process.Id
        $state['LaunchedAt'] = (Get-Date).ToString('o')
    }

    Save-Json -InputObject $state -Path (Join-Path $evidenceFull 'session-state.json')
    $state | ConvertTo-Json -Depth 20
    exit 0
}

if (-not $EvidenceDir) {
    throw 'Pass -EvidenceDir when restoring a live session.'
}

$evidenceRestoreFull = Get-ResolvedFullPath -Path $EvidenceDir
Assert-PathInside -Child $evidenceRestoreFull -Parent $runtimeRoot -Label 'Evidence'
$sessionPath = Join-Path $evidenceRestoreFull 'session-state.json'
if (-not (Test-Path -LiteralPath $sessionPath)) {
    throw "Session state not found: $sessionPath"
}

$session = Get-Content -LiteralPath $sessionPath -Raw -Encoding UTF8 | ConvertFrom-Json
$stopped = @()
if ($StopGameOnRestore) {
    $stopped = Stop-SpireProcesses
}

$restoredMods = Restore-MovedItems `
    -Items @($session.MovedMods) `
    -DestinationRoot $session.ModsRoot `
    -SourceRoot (Join-Path $evidenceRestoreFull 'isolated-mods') `
    -Label 'Mod'

$currentRunDestinations = @($session.SteamSaves, $session.DefaultSaves) | Where-Object { $_ }
$restoredRuns = @()
foreach ($destinationRoot in $currentRunDestinations) {
    $itemsForRoot = @($session.MovedCurrentRuns | Where-Object {
        $_.From -and ([System.IO.Path]::GetFullPath($_.From).StartsWith([System.IO.Path]::GetFullPath($destinationRoot).TrimEnd('\') + '\', [System.StringComparison]::OrdinalIgnoreCase))
    })
    if ($itemsForRoot.Count -eq 0) {
        continue
    }

    if ($PreserveNewCurrentRunsOnRestore) {
        Move-NewCurrentRunsBeforeRestore `
            -Items $itemsForRoot `
            -EvidenceRoot $evidenceRestoreFull `
            -DestinationRoot $destinationRoot | Out-Null
    }

    $restoredRuns += Restore-MovedItems `
        -Items $itemsForRoot `
        -DestinationRoot $destinationRoot `
        -SourceRoot (Join-Path $evidenceRestoreFull 'temporarily-removed-current-runs') `
        -Label 'Current-run'
}

$settingsBefore = Join-Path $evidenceRestoreFull 'settings.save.before'
if (Test-Path -LiteralPath $settingsBefore) {
    Copy-Item -LiteralPath $settingsBefore -Destination $session.SettingsPath -Force
}

$settingsBackupBefore = Join-Path $evidenceRestoreFull 'settings.save.backup.before'
if (Test-Path -LiteralPath $settingsBackupBefore) {
    Copy-Item -LiteralPath $settingsBackupBefore -Destination $session.SettingsBackupPath -Force
}

if (Test-Path -LiteralPath $session.LogPath) {
    Copy-Item -LiteralPath $session.LogPath -Destination (Join-Path $evidenceRestoreFull 'godot.log.after-restore') -Force
}

$restoreState = [ordered]@{
    EvidenceDir = $evidenceRestoreFull
    RestoredAt = (Get-Date).ToString('o')
    StoppedProcesses = @($stopped | ForEach-Object { [pscustomobject]@{ ProcessName = $_.ProcessName; Id = $_.Id } })
    RestoredModCount = @($restoredMods).Count
    RestoredCurrentRunCount = @($restoredRuns).Count
    SettingsHashAfterRestore = Get-HashOrNull -Path $session.SettingsPath
    SettingsBackupHashAfterRestore = Get-HashOrNull -Path $session.SettingsBackupPath
}

Save-Json -InputObject $restoreState -Path (Join-Path $evidenceRestoreFull 'restore-state.json')
$restoreState | ConvertTo-Json -Depth 20
