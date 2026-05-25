param(
    [string]$EvidenceDir,

    [string]$PackageSha256 = "8FD25AE6EFECCD76CFEDA13B99CAB355DF02824EDA595A4F8F1A0BBABDFC5D0E",

    [string]$PackagePath = "publish\SpirePlus-v0.1.0-private-beta.5.zip",

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

function Write-ChecklistFiles {
    param(
        [Parameter(Mandatory = $true)][string]$TemplateContent,
        [Parameter(Mandatory = $true)][string]$TemplatePath,
        [Parameter(Mandatory = $true)][string]$WorkingPath
    )

    $TemplateContent | Set-Content -LiteralPath $TemplatePath -Encoding UTF8

    $workingLines = @($TemplateContent -split "`r?`n" | ForEach-Object {
            if ($_ -match '^Copy this file to `[^`]+` and fill it with live results before marking this row pass\.$' -or
                $_ -match '^Template reference for `[^`]+`\. Fill the working `[^`]+` with live results before marking this row pass\.$') {
                'Fill this checklist with live results before marking this row pass.'
            } else {
                $_
            }
        })
    ($workingLines -join [Environment]::NewLine) | Set-Content -LiteralPath $WorkingPath -Encoding UTF8
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

function Get-DefaultRequiredFiles {
    param([Parameter(Mandatory = $true)][string]$Kind)

    switch ($Kind) {
        'loader' {
            return @('command.txt', 'environment.json', 'package-hashes.json', 'godot.log', 'godot-log-audit.json', 'enabled-mods.txt')
        }
        'clicked-ui' {
            return @('command.txt', 'window-preflight.json', 'godot.log', 'godot-log-audit.json', 'route-note.md')
        }
        'save-load' {
            return @('command.txt', 'godot.log', 'godot-log-audit.json', 'save-load-note.md')
        }
        'coop' {
            return @('command.txt', 'host-godot.log', 'host-godot-log-audit.json', 'client-godot.log', 'client-godot-log-audit.json', 'result-note.md')
        }
        'preview-tools' {
            return @('command.txt', 'environment.json', 'package-hashes.json', 'godot.log', 'godot-log-audit.json', 'result-note.md')
        }
        default {
            return @('command.txt', 'godot.log', 'godot-log-audit.json', 'result-note.md')
        }
    }
}

function New-ReleaseRow {
    param(
        [Parameter(Mandatory = $true)][string]$Id,
        [Parameter(Mandatory = $true)][string]$Kind,
        [Parameter(Mandatory = $true)][string]$Label,
        [Parameter(Mandatory = $true)][string]$EvidenceFull,
        [string[]]$ExtraRequiredFiles = @(),
        [string[]]$Checkpoints = @()
    )

    $requiredFiles = @((Get-DefaultRequiredFiles -Kind $Kind) + $ExtraRequiredFiles |
        ForEach-Object { [string]$_ } |
        Where-Object { -not [string]::IsNullOrWhiteSpace($_) })

    return [ordered]@{
        Id = $Id
        Label = $Label
        Kind = $Kind
        Status = 'pending'
        EvidenceDir = $EvidenceFull
        RequiredFiles = $requiredFiles
        ScreenshotFile = if ($Kind -eq 'clicked-ui') { '' } else { $null }
        ResultNote = ''
        ReleaseNote = ''
        ExplicitOwnerDecision = $false
        Checkpoints = @($Checkpoints)
        Notes = 'Fill with live evidence before changing Status to pass. Source review alone does not close this row.'
    }
}

function New-ManualRows {
    param([Parameter(Mandatory = $true)][string]$EvidenceFull)

    $requiredRows = @(
        @{ Id = 'fresh-current-package-loader-smoke'; Kind = 'loader'; Label = 'Fresh current-package loader smoke with current package hashes and clean log audit' },
        @{ Id = 'ancient-ui-urda'; Kind = 'clicked-ui'; Label = 'Urda clicked Ancient UI' },
        @{ Id = 'ancient-ui-morvi'; Kind = 'clicked-ui'; Label = 'Morvi clicked Ancient UI' },
        @{ Id = 'ancient-ui-lotha'; Kind = 'clicked-ui'; Label = 'Lotha clicked Ancient UI' },
        @{ Id = 'ancient-ui-vakuu-normal'; Kind = 'clicked-ui'; Label = 'Vakuu normal clicked Ancient UI' },
        @{ Id = 'ancient-ui-vakuu-fight'; Kind = 'clicked-ui'; Label = 'Vakuu force-fight clicked Ancient UI' },
        @{
            Id = 'ancient-reward-visible-relics'
            Kind = 'gameplay'
            Label = 'Selected Ancient rewards visible as relics with readable hover tips'
            ExtraRequiredFiles = @('ancient-reward-relics-checklist.md')
            Checkpoints = @(
                'Every Urda, Morvi, and Lotha initial reward option is visible as an option relic on the Ancient screen.',
                'Every selected lasting Ancient reward appears in the player relic bar with readable hover text.',
                'Vakuu fight option and post-victory non-Vakuu reward choices use visible relic choices and do not rely on hidden text-only effects.',
                'English and Simplified Chinese hover text are checked for fit/readability where the tester can switch language.'
            )
        },
        @{
            Id = 'player-text-tooltip-readability'
            Kind = 'gameplay'
            Label = 'Player-facing text, tooltip, and hover readability'
            ExtraRequiredFiles = @('player-text-qa-checklist.md')
            Checkpoints = @(
                'Check English and Simplified Chinese text separately; do not mark one language as proof for the other.',
                'Confirm descriptions use player-facing terms, short effect sentences, concrete numbers, and no implementation/planning wording.',
                'Confirm dynamic values, rich-text tags, and line fit are readable in card text, relic hover, power hover, map hover, and event option rows.',
                'Record any confusing wording verbatim in result-note.md before marking the row pass.'
            )
        },
        @{
            Id = 'art-resource-routing-live-preview'
            Kind = 'clicked-ui'
            Label = 'Live UI preview proves event art, map icons, run-history icons, relic art, card art, and power art are not misrouted'
            ExtraRequiredFiles = @('art-resource-routing-checklist.md')
            Checkpoints = @(
                'Confirm large Ancient/event art is used only on clicked Ancient or event screens, not as map thumbnails or option relic icons.',
                'Confirm map icons, run-history icons, option relic icons, lasting relic icons, card art, and power icons each use the expected small-format asset.',
                'Confirm title/home preview and Ancient clicked screens fit their UI frames at the test resolution.',
                'Confirm no placeholder crop, NOPE image, or copied official non-art asset appears in the tested UI.'
            )
        },
        @{
            Id = 'vakuu-victory-no-black-screen'
            Kind = 'gameplay'
            Label = 'Vakuu victory returns to the event without a black screen'
            ExtraRequiredFiles = @('vakuu-victory-checklist.md')
            Checkpoints = @(
                'Confirm the dedicated Vakuu monster and scene appear before victory.',
                'Confirm contracts, locks, Blood Debt, and victory reward choices are visible.',
                'Confirm victory returns to a usable event/reward/map state with no black screen or softlock.'
            )
        },
        @{
            Id = 'vakuu-failure-death-path'
            Kind = 'gameplay'
            Label = 'Vakuu failure and death path does not softlock'
            ExtraRequiredFiles = @('vakuu-failure-death-checklist.md')
            Checkpoints = @(
                'Confirm non-lethal failure path exits cleanly if reachable.',
                'Confirm death path reaches the expected game-over/run-end flow.',
                'Confirm logs show no stale parent event, room stack, or reward-screen exception.'
            )
        },
        @{
            Id = 'vakuu-active-fight-save-load'
            Kind = 'save-load'
            Label = 'Vakuu active child-combat save/load'
            ExtraRequiredFiles = @('vakuu-save-load-checklist.md')
            Checkpoints = @(
                'Save during active Vakuu child combat, reload, and confirm combat state and parent event state.',
                'Save around post-combat reward/return, reload, and confirm no duplicate Ancient heal or stale parent event.',
                'Attach logs and screenshots for before/after load.'
            )
        },
        @{ Id = 'ancient-state-save-load'; Kind = 'save-load'; Label = 'Urda, Morvi, Lotha, and Ancient reward state save/load' },
        @{
            Id = 'rootblight-visual-behavior'
            Kind = 'gameplay'
            Label = 'Rootblight and Blight Sprout visual/gameplay behavior'
            ExtraRequiredFiles = @('rootblight-behavior-checklist.md')
            Checkpoints = @(
                'Confirm A14+ Rootblight appears before ordinary combat and normal fights advance existing Rootblight without expecting Blight Sprout cards.',
                'Confirm Blight Sprout appears only in the current A15 Boss and A18 eligible Elite contexts instead of disappearing from those fights.',
                'Confirm unhandled Blight Sprouts grow into Rootblight after combat and that existing Rootblight follows the current cap/split rules.',
                'Confirm save/load does not erase pending Blight Sprouts, Rootblight cards, or current run repair state.',
                'Confirm card art, card hover, combat notices, and EN/ZHS wording remain readable.'
            )
        },
        @{ Id = 'a11-natural-route-traversal'; Kind = 'gameplay'; Label = 'Natural A11 route click traversal' },
        @{
            Id = 'ascension-selector-localization'
            Kind = 'clicked-ui'
            Label = 'A11-A20 character-select Ascension selector localization'
            Checkpoints = @(
                'Open the character-select Ascension selector in English and Simplified Chinese if possible.',
                'A20 must show Branded Form / 烙印形态instead of ascension.LEVEL_20.title.',
                'A20 description must be readable instead of ascension.LEVEL_20.description.',
                'Spot-check A11-A19 titles/descriptions for the same raw-key regression.'
            )
        },
        @{
            Id = 'a19-a20-dedicated-boss-abilities'
            Kind = 'gameplay'
            Label = 'A19/A20 dedicated boss abilities and Branded Form live combat proof'
            ExtraRequiredFiles = @('boss-ability-checklist.md')
            Checkpoints = @(
                'A19 uses boss-specific dedicated abilities, not one generic Royal Seal ability.',
                'A20 Branded Form applies only to the second Act 3 Boss.',
                'Attack-changing abilities show final intent before damage resolves.',
                'Martyr Oath, Ink Return, Plating Wake, Soul Tide, Unweakenable, Claw Calibration, Marginal Note, Escape Fatigue, Time Sand Reflow, Royal Decree, and Experimental Record are each tested on the matching Boss.',
                'Multiplayer-sensitive values follow the v4.1 scaling rules or are explicitly deferred with owner approval.'
            )
        },
        @{ Id = 'disable-mod-gameplay'; Kind = 'gameplay'; Label = 'BaseLib-only disabled Spire Plus gameplay comparison' },
        @{
            Id = 'preview-tools-live-proof'
            Kind = 'preview-tools'
            Label = 'Live Preview tools evidence for Crystal Sphere and transform preview'
            ExtraRequiredFiles = @('preview-tools-checklist.md')
            Checkpoints = @(
                'Confirm Crystal Sphere peek only changes the mask/visibility and does not call reward reveal or claim flows.',
                'Confirm transform preview matches the actual transform result while using forked RNG and not mutating run state.',
                'Confirm Prismatic Gem preview handles reward modifiers without hiding late Core reward changes.',
                'Confirm save/reopen and multiplayer gate or two-client proof are recorded.'
            )
        },
        @{
            Id = 'coop-disposition'
            Kind = 'coop'
            Label = 'Two-client co-op disposition or explicit release-note deferral'
            ExtraRequiredFiles = @('coop-disposition-checklist.md')
            Checkpoints = @(
                'Record host and client package hashes, enabled mods, and clean logs.',
                'Record whether A11-A20 selection, Ancient rewards, Root Eyes, Rootblight, save/reconnect, and preview tools are proven or explicitly deferred.',
                'Do not mark co-op supported from lobby selection alone.',
                'If unsupported/unverified, include an owner-approved deferral note rather than a pass claim.'
            )
        }
    )

    return @($requiredRows | ForEach-Object {
            $rowEvidenceFull = Join-Path $EvidenceFull $_.Id
            $extraRequiredFiles = if ($_.ContainsKey('ExtraRequiredFiles')) { @($_.ExtraRequiredFiles) } else { @() }
            $checkpoints = if ($_.ContainsKey('Checkpoints')) { @($_.Checkpoints) } else { @() }
            New-ReleaseRow `
                -Id $_.Id `
                -Kind $_.Kind `
                -Label $_.Label `
                -EvidenceFull $rowEvidenceFull `
                -ExtraRequiredFiles $extraRequiredFiles `
                -Checkpoints $checkpoints
        })
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
        EnvironmentVariable = 'SPIREPLUS_RELEASE_EVIDENCE_LOG'
        LegacyEnvironmentVariable = 'EZMB_RELEASE_EVIDENCE_LOG'
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
        Get-HashRow -RelativePath $PackagePath
        Get-HashRow -RelativePath 'publish\SpirePlus-v0.1.0-private-beta.5\EZMicroBalance\EZMicroBalance.dll'
        Get-HashRow -RelativePath 'publish\SpirePlus-v0.1.0-private-beta.5\EZMicroBalance\EZMicroBalance.pck'
        Get-HashRow -RelativePath 'publish\SpirePlus-v0.1.0-private-beta.5\EZMicroBalance\EZMicroBalance.json'
        Get-HashRow -RelativePath 'publish\SpirePlus-v0.1.0-private-beta.5\EZMicroBalance\README_INSTALL.txt'
    )
}

$enabledModsTemplate = @(
    '# Enabled Mods Template',
    '',
    'Status: pending',
    '',
    'Expected current-package loader proof:',
    '- BaseLib',
    '- Spire Plus',
    '',
    'Paste the loaded-mods log excerpt here. Do not mark this file passed from historical 16/22-field logs or source review.'
) -join [Environment]::NewLine

$manualRows = @(New-ManualRows -EvidenceFull $evidenceFull)
$rowListLines = @($manualRows | ForEach-Object {
        "- $($_.Id) [$($_.Kind)] - $($_.Label)"
    })

$readmeLines = @(
    '# Release Evidence Folder',
    '',
    'Status: pending',
    '',
    'This folder is a collection template. It is not release proof until live logs, screenshots, manual notes, and verifier output are filled in.',
    '',
    'Each verifier row has its own subfolder. Keep row evidence inside that row folder so one log or note cannot accidentally stand in for unrelated proof.',
    '',
    'Required verifier row IDs:'
) + $rowListLines + @(
    '',
    'Verifier pass marker:',
    '- release-evidence-verifier-pass.json'
)

$readme = $readmeLines -join [Environment]::NewLine

$releaseManifest = [ordered]@{
    PackageSha256 = $PackageSha256
    PackagePath = $PackagePath
    CreatedAt = (Get-Date).ToString('o')
    Rows = $manualRows
}

Format-DisplayCommand -Tokens $selfTokens | Set-Content -LiteralPath (Join-Path $evidenceFull 'command.txt') -Encoding UTF8
Save-Json -InputObject $environment -Path (Join-Path $evidenceFull 'environment.json')
Save-Json -InputObject $packageHashes -Path (Join-Path $evidenceFull 'package-hashes.json')
Save-Json -InputObject ([ordered]@{ Rows = $manualRows }) -Path (Join-Path $evidenceFull 'manual-rows-template.json')
Save-Json -InputObject $releaseManifest -Path (Join-Path $evidenceFull 'release-evidence-manifest.json')
Save-Json -InputObject ([ordered]@{
    Status = 'pending'
    RequiredCommand = '.\scripts\verify-spire-plus-release-evidence.ps1'
    PassMarkerPath = 'release-evidence-verifier-pass.json'
    Notes = 'Write a pass marker only after the verifier exits 0 against filled live evidence.'
}) -Path (Join-Path $evidenceFull 'verifier-pass-marker-template.json')
$readme | Set-Content -LiteralPath (Join-Path $evidenceFull 'README.md') -Encoding UTF8

foreach ($row in $manualRows) {
    $rowEvidenceFull = [string]$row.EvidenceDir
    Assert-PathInside -Child $rowEvidenceFull -Parent $evidenceFull -Label "Row $($row.Id) evidence"
    New-DirectoryIfMissing -Path $rowEvidenceFull

    Format-DisplayCommand -Tokens $selfTokens | Set-Content -LiteralPath (Join-Path $rowEvidenceFull 'command.txt') -Encoding UTF8

    $requiredFiles = @($row.RequiredFiles | ForEach-Object { [string]$_ })
    if ($requiredFiles -contains 'environment.json') {
        Save-Json -InputObject $environment -Path (Join-Path $rowEvidenceFull 'environment.json')
    }

    if ($requiredFiles -contains 'package-hashes.json') {
        Save-Json -InputObject $packageHashes -Path (Join-Path $rowEvidenceFull 'package-hashes.json')
    }

    if ($row.Id -eq 'fresh-current-package-loader-smoke') {
        $enabledModsTemplate | Set-Content -LiteralPath (Join-Path $rowEvidenceFull 'enabled-mods-template.txt') -Encoding UTF8
    }

    $rowReadmeLines = @(
        "# $($row.Id)",
        '',
        "Kind: $($row.Kind)",
        "Status: pending",
        '',
        $row.Label,
        '',
        'Required files for pass status:'
    ) + @($requiredFiles | ForEach-Object { "- $_" }) + @(
        '',
        'Do not change this row to pass until the live evidence files and the manifest ResultNote describe this specific row.'
    )

    $checkpoints = @($row.Checkpoints | ForEach-Object { [string]$_ } | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
    if ($checkpoints.Count -gt 0) {
        $rowReadmeLines += @(
            '',
            'Manual checkpoints:'
        ) + @($checkpoints | ForEach-Object { "- $_" })
    }

    ($rowReadmeLines -join [Environment]::NewLine) | Set-Content -LiteralPath (Join-Path $rowEvidenceFull 'README.md') -Encoding UTF8

    if ($row.Id -eq 'ancient-reward-visible-relics') {
        $sereTalonCn = -join @([char]0x74E6, [char]0x5E93, [char]0x539F, [char]0x521D, [char]0x4E4B, [char]0x722A)
        $tanxClawsCn = -join @([char]0x5766, [char]0x514B, [char]0x65AF, [char]0x5229, [char]0x722A)
        $maulCn = -join @([char]0x6495, [char]0x54AC)
        $ancientRewardChecklist = @(
            '# Ancient Reward Visible Relics Checklist',
            '',
            'Template reference for `ancient-reward-relics-checklist.md`. Fill the working `ancient-reward-relics-checklist.md` with live results before marking this row pass.',
            '',
            'Required evidence:',
            '- `godot.log` from the live run with `SPIREPLUS_RELEASE_EVIDENCE_LOG=1` when possible.',
            '- `godot-log-audit.json` showing no release-blocking signatures.',
            '- `result-note.md` summarizing which Ancient rewards were tested and which rows failed.',
            '- Screenshots are strongly recommended for Ancient option rows, relic bar placement, and hover readability.',
            '',
            'Reward rows:',
            '',
            '| Ancient | Reward ID | Expected option/relic | Screen option visible | Relic bar / hover result | Evidence file(s) |',
            '| --- | --- | --- | --- | --- | --- |',
            '| Urda | seedbed | UrdaSeedbedOptionRelic |  |  |  |',
            '| Urda | humus_pact | UrdaHumusPactOptionRelic |  |  |  |',
            '| Urda | molting | UrdaMoltingOptionRelic |  |  |  |',
            '| Urda | moss_map | UrdaMossMapOptionRelic |  |  |  |',
            '| Urda | trial_branch | UrdaTrialBranchOptionRelic |  |  |  |',
            '| Urda | shallow_root_relic | UrdaShallowRootRelicOptionRelic |  |  |  |',
            '| Urda | elite_root | UrdaEliteRootOptionRelic |  |  |  |',
            '| Urda | rooted_route | UrdaRootedRouteOptionRelic |  |  |  |',
            '| Urda | after_rain | UrdaAfterRainOptionRelic |  |  |  |',
            '| Urda | root_sight | UrdaRootSightOptionRelic |  |  |  |',
            '| Urda | seed_bank | UrdaSeedBankOptionRelic |  |  |  |',
            '| Morvi | forbidden_loan | MorviForbiddenLoanOptionRelic |  |  |  |',
            '| Morvi | misprint_press | MorviMisprintPressOptionRelic |  |  |  |',
            '| Morvi | red_ink_overdraft | MorviRedInkOverdraftOptionRelic |  |  |  |',
            '| Morvi | overdue_library | MorviOverdueLibraryOptionRelic |  |  |  |',
            '| Morvi | open_book_exam | MorviOpenBookExamOptionRelic |  |  |  |',
            '| Morvi | paperstorm | MorviPaperstormOptionRelic |  |  |  |',
            '| Morvi | blueprint_proof | MorviBlueprintProofOptionRelic |  |  |  |',
            '| Morvi | debt_settlement | MorviDebtSettlementOptionRelic |  |  |  |',
            '| Lotha | mirror_rebuttal | LothaMirrorRebuttalOptionRelic |  |  |  |',
            '| Lotha | mirror_hall_echo | LothaMirrorHallEchoOptionRelic |  |  |  |',
            '| Lotha | presumption | LothaPresumptionOptionRelic |  |  |  |',
            '| Lotha | closed_court | LothaClosedCourtOptionRelic |  |  |  |',
            '| Lotha | deferred_verdict | LothaDeferredVerdictOptionRelic |  |  |  |',
            '| Lotha | death_reprieve | LothaDeathReprieveOptionRelic |  |  |  |',
            '| Lotha | single_sentence | LothaSingleSentenceOptionRelic |  |  |  |',
            '| Lotha | public_evidence | LothaPublicEvidenceOptionRelic |  |  |  |',
            '| Vakuu | fight_option | VakuuFightOptionRelic |  |  |  |',
            '| Vakuu | victory_non_vakuu_choices | Non-Vakuu Act 3 Ancient reward relic choices after winning Vakuu |  |  |  |',
            "| Vakuu event | sere_talon_pickup | Vakuu's Sere Talon / $sereTalonCn lets the player choose 1 of 4 Curses, then adds that Curse, 2 Wish, and 1 Wish+; verify event-option art, relic-bar art, inspect art, hover text, and surface-specific log routes such as ``Ancient event option button``, ``RelicModel packed icon texture``, ``RelicModel big icon texture``, ``NRelic small node``, and ``NRelic large node`` are not Tanx Claws. |  |  |  |",
            "| Tanx event | claws_maul_transform | Tanx Claws / $tanxClawsCn transforms cards into upgraded Maul / $maulCn+ cards. |  |  |  |",
            '',
            'Do not use this row to cover Ancient clicked background art, Vakuu victory return, save/load, or co-op. Those have separate verifier rows.'
        ) -join [Environment]::NewLine

        Write-ChecklistFiles -TemplateContent $ancientRewardChecklist -TemplatePath (Join-Path $rowEvidenceFull 'ancient-reward-relics-checklist-template.md') -WorkingPath (Join-Path $rowEvidenceFull 'ancient-reward-relics-checklist.md')
    }

    if ($row.Id -eq 'rootblight-visual-behavior') {
        $rootblightChecklist = @(
            '# Rootblight / Blight Sprout Behavior Checklist',
            '',
            'Template reference for `rootblight-behavior-checklist.md`. Fill the working `rootblight-behavior-checklist.md` with live results before marking this row pass.',
            '',
            'Required evidence:',
            '- `godot.log` from the live run with `SPIREPLUS_RELEASE_EVIDENCE_LOG=1` when possible.',
            '- `godot-log-audit.json` showing no release-blocking signatures.',
            '- `result-note.md` summarizing which Ascension level, act, and combat type was tested.',
            '- Screenshots are strongly recommended for card hand visibility, card hover, combat notices, and post-save/load state.',
            '',
            'Scenario rows:',
            '',
            '| Scenario ID | Expected behavior | Live result | Evidence file(s) |',
            '| --- | --- | --- | --- |',
            '| rootblight-start-eligibility | A14+ run starts/repairs Rootblight setup only after a real deck card exists; no silent permanent disable if the deck is temporarily unavailable. |  |  |',
            '| normal-rootblight-continuity | Ordinary normal combats do not add Blight Sprout in the current design; they should still mark existing Rootblight, show it in the deck/hand flow, and resolve combat-end growth/cap rules. |  |  |',
            '| elite-single-sprout | Elite combat adds exactly one Blight Sprout at the expected timing and does not duplicate across reload/reentry. |  |  |',
            '| boss-two-sprouts-staggered | Act 2/3 Boss combat adds two Blight Sprouts on the staggered turns, with both cards visible when expected. |  |  |',
            '| husk-exhaust-block-timing | Withered Husk grants exactly 3 Block when it is Exhausted, not when it merely has Ethereal/Void text or sits in hand. |  |  |',
            '| combat-end-growth | An unresolved Blight Sprout grows into Rootblight after combat; handled/planted Sprouts do not grow. |  |  |',
            '| rootblight-cap-four | Rootblight respects the current maximum and Rootblight III split/growth rule without exceeding 4 cards. |  |  |',
            '| rootblight-save-load | Save/load before Sprout entry, during combat, and after combat preserves pending markers and deck state. |  |  |',
            '| ui-hover-art-readability | Blight Sprout, Rootblight, Seedbed/Husk interactions, card art, and EN/ZHS hover text are visible and readable. |  |  |',
            '',
            'Do not use this row to cover A11 route traversal, Ancient clicked UI, or co-op. Those have separate verifier rows.'
        ) -join [Environment]::NewLine

        Write-ChecklistFiles -TemplateContent $rootblightChecklist -TemplatePath (Join-Path $rowEvidenceFull 'rootblight-behavior-checklist-template.md') -WorkingPath (Join-Path $rowEvidenceFull 'rootblight-behavior-checklist.md')
    }

    if ($row.Id -eq 'art-resource-routing-live-preview') {
        $artRoutingChecklist = @(
            '# Art / Resource Routing Checklist',
            '',
            'Template reference for `art-resource-routing-checklist.md`. Fill the working `art-resource-routing-checklist.md` with live results before marking this row pass.',
            '',
            'Required evidence:',
            '- `window-preflight.json` proving Slay the Spire 2 was the foreground window for clicked UI screenshots.',
            '- `godot.log` and clean `godot-log-audit.json` from the same session.',
            '- `route-note.md` describing which screens were opened and which screenshots correspond to each row.',
            '- PNG screenshots at least 800x450. Use multiple screenshots when one surface cannot show all rows.',
            '',
            'Surface rows:',
            '',
            '| Surface ID | Expected routing | Live result | Evidence file(s) |',
            '| --- | --- | --- | --- |',
            '| title-home-preview | Spire Plus title/home preview image fits the UI frame and does not stretch, crop critical subject matter, or use stale pre-refresh branding. |  |  |',
            '| urda-clicked-background | Urda large background appears only on the clicked Ancient screen/event surface and fits behind option rows. |  |  |',
            '| morvi-clicked-background | Morvi large background appears only on the clicked Ancient screen/event surface and fits behind option rows. |  |  |',
            '| lotha-clicked-background | Lotha large background appears only on the clicked Ancient screen/event surface and fits behind option rows. |  |  |',
            '| vakuu-clicked-background | Vakuu normal and force-fight clicked screens use the intended large art without hiding option text. |  |  |',
            '| map-icons | Ancient, Root Eyes, Firemark, Banner, Deep Branch, and other map markers use readable small icons, not full-size event art. |  |  |',
            '| run-history-icons | Run-history icons use small-format art and remain distinguishable from map icons and clicked-screen backgrounds. |  |  |',
            '| option-relic-icons | Ancient option relic choices use option relic icons and do not reuse clicked-screen backgrounds or placeholder crops. |  |  |',
            '| lasting-relic-icons | Selected lasting Ancient rewards appear in the relic bar with readable small icons and hover art/text. |  |  |',
            '| card-art | Rootblight, Blight Sprout, Husk, Contract, Rain Breath, Seedbed-related cards, and preview cards use the expected card portraits. |  |  |',
            '| power-icons | Firemark, Banner, A19/A20 dedicated abilities, Vakuu powers, Seedbed, and Rootblight powers use visible non-NOPE icons. |  |  |',
            '| no-placeholder-or-official-art | No tested surface shows NOPE, generic temporary art, placeholder crops, stale logo text, or copied official non-art source material. |  |  |',
            '',
            'Do not use this row to cover gameplay correctness. Ancient reward relic visibility, Rootblight behavior, A19/A20 combat, and Vakuu return paths have separate verifier rows.'
        ) -join [Environment]::NewLine

        Write-ChecklistFiles -TemplateContent $artRoutingChecklist -TemplatePath (Join-Path $rowEvidenceFull 'art-resource-routing-checklist-template.md') -WorkingPath (Join-Path $rowEvidenceFull 'art-resource-routing-checklist.md')
    }

    if ($row.Id -eq 'player-text-tooltip-readability') {
        $playerTextChecklist = @(
            '# Player Text / Tooltip QA Checklist',
            '',
            'Template reference for `player-text-qa-checklist.md`. Fill the working `player-text-qa-checklist.md` with live results before marking this row pass.',
            '',
            'Required evidence:',
            '- `godot.log` and clean `godot-log-audit.json` from the same session.',
            '- `result-note.md` with exact wording problems if any row fails.',
            '- Screenshots are strongly recommended for cramped hover, map hover, card text, power text, event options, and relic bar hover.',
            '',
            'Text rows:',
            '',
            '| Surface ID | Expected text quality | EN result | ZHS result | Evidence file(s) |',
            '| --- | --- | --- | --- | --- |',
            '| ascension-a11-a20 | A11-A20 descriptions are short, concrete, use Dedicated Ability/Branded Form wording, and avoid stale Royal Seal/King Brand terms. |  |  |  |',
            '| firemark-and-banner | Firemark and Banner text shows current-act values, explains Host/Overflow/Forge Armor/Shieldwall clearly, and avoids slash-table wording in live hovers. |  |  |  |',
            '| boss-dedicated-abilities | A19/A20 Boss power hovers explain the matching Boss ability with final damage/intent implications and multiplayer caps where relevant. |  |  |  |',
            '| ancient-choice-text | Urda, Morvi, Lotha, Vakuu option rows explain what the player gains or risks without implementation terms. |  |  |  |',
            '| ancient-relic-hover | Option relic and selected-relic hover text is readable from the relic bar and matches the actual active reward. |  |  |  |',
            '| cards-status-curses | Blight Sprout, Rootblight, Husk, Seedbed, Contract, Rain Breath, Marginal Note, and generated temporary cards use clear card/status wording. |  |  |  |',
            '| map-hover-stacks | Root Eyes, Firemarked Elite, Banner, Deep Branch, Boss ability, and Branded Form map hovers stack without hiding each other. |  |  |  |',
            '| preview-tools-text | Crystal Sphere peek, transform preview, and Prismatic Gem preview text explain preview behavior without implying reward claim or RNG mutation. |  |  |  |',
            '| vakuu-contracts | Vakuu contracts, Blood Debt, locks, Cash Out, and victory reward choices explain the greed/stop decision and post-fight settlement. |  |  |  |',
            '| en-zhs-key-parity | EN and ZHS keys, dynamic variables, and rich-text tags match for tested surfaces; no mojibake or missing localization appears. |  |  |  |',
            '',
            'Do not use this row to cover gameplay correctness. Ancient rewards, art routing, Rootblight behavior, A19/A20 combat, Vakuu return, save-load, and co-op have separate verifier rows.'
        ) -join [Environment]::NewLine

        Write-ChecklistFiles -TemplateContent $playerTextChecklist -TemplatePath (Join-Path $rowEvidenceFull 'player-text-qa-checklist-template.md') -WorkingPath (Join-Path $rowEvidenceFull 'player-text-qa-checklist.md')
    }

    if ($row.Id -eq 'vakuu-victory-no-black-screen') {
        $vakuuVictoryChecklist = @(
            '# Vakuu Victory / No Black Screen Checklist',
            '',
            'Template reference for `vakuu-victory-checklist.md`. Fill the working `vakuu-victory-checklist.md` with live results before marking this row pass.',
            '',
            'Required evidence:',
            '- `godot.log` and clean `godot-log-audit.json` from the same session.',
            '- `result-note.md` describing the route, broken locks, Blood Debt, and post-victory screen.',
            '- Screenshots are strongly recommended for fight start, contract/lock state, victory return, and reward choices.',
            '',
            'Scenario rows:',
            '',
            '| Scenario ID | Expected behavior | Live result | Evidence file(s) |',
            '| --- | --- | --- | --- |',
            '| fight-start-scene | Dedicated Vakuu monster and encounter scene appear, not a placeholder or normal Ancient screen. |  |  |',
            '| contract-turns | Contract choices appear on expected turns and do not softlock the hand. |  |  |',
            '| locks-blood-debt | Stolen Vault locks, broken-lock count, Blood Debt, Gold/HP settlement, and lethal-hit lock counting are visible and coherent. |  |  |',
            '| victory-return | Winning returns to a usable event/reward/map state. |  |  |',
            '| non-vakuu-rewards | Victory offers non-Vakuu Ancient reward choices and no normal combat card reward. |  |  |',
            '| no-black-screen | The screen does not go black, freeze, or require force quit after victory. |  |  |',
            '| log-clean | Logs contain no release-blocking exception, stale parent event, room stack, or reward-screen error. |  |  |',
            '',
            'Do not use this row to cover failure/death or save-load. Those have separate verifier rows.'
        ) -join [Environment]::NewLine

        Write-ChecklistFiles -TemplateContent $vakuuVictoryChecklist -TemplatePath (Join-Path $rowEvidenceFull 'vakuu-victory-checklist-template.md') -WorkingPath (Join-Path $rowEvidenceFull 'vakuu-victory-checklist.md')
    }

    if ($row.Id -eq 'vakuu-failure-death-path') {
        $vakuuFailureDeathChecklist = @(
            '# Vakuu Failure / Death Checklist',
            '',
            'Template reference for `vakuu-failure-death-checklist.md`. Fill the working `vakuu-failure-death-checklist.md` with live results before marking this row pass.',
            '',
            'Required evidence:',
            '- `godot.log` and clean `godot-log-audit.json` from the same session.',
            '- `result-note.md` describing whether failure, death, or both were reached.',
            '- Screenshots are strongly recommended for the last fight state and the post-failure/death screen.',
            '',
            'Scenario rows:',
            '',
            '| Scenario ID | Expected behavior | Live result | Evidence file(s) |',
            '| --- | --- | --- | --- |',
            '| fight-start-scene | Dedicated Vakuu fight starts before the failure/death path is tested. |  |  |',
            '| failure-path | A non-death failure path exits cleanly if the design exposes one. If not reachable, record why. |  |  |',
            '| death-path | Death reaches the expected run-end/game-over flow without stale Ancient UI or hidden reward screens. |  |  |',
            '| room-state-after-exit | Room, event, reward, and map state remain coherent after failure/death. |  |  |',
            '| no-softlock | The game remains responsive or reaches the expected terminal run state. |  |  |',
            '| log-clean | Logs contain no release-blocking exception, stale parent event, room stack, or reward-screen error. |  |  |',
            '',
            'Do not use this row to cover victory return or save-load. Those have separate verifier rows.'
        ) -join [Environment]::NewLine

        Write-ChecklistFiles -TemplateContent $vakuuFailureDeathChecklist -TemplatePath (Join-Path $rowEvidenceFull 'vakuu-failure-death-checklist-template.md') -WorkingPath (Join-Path $rowEvidenceFull 'vakuu-failure-death-checklist.md')
    }

    if ($row.Id -eq 'vakuu-active-fight-save-load') {
        $vakuuSaveLoadChecklist = @(
            '# Vakuu Save / Load Checklist',
            '',
            'Template reference for `vakuu-save-load-checklist.md`. Fill the working `vakuu-save-load-checklist.md` with live results before marking this row pass.',
            '',
            'Required evidence:',
            '- `godot.log` and clean `godot-log-audit.json` from the same session.',
            '- `save-load-note.md` describing each save point and reload result.',
            '- Screenshots are strongly recommended before and after each load.',
            '',
            'Scenario rows:',
            '',
            '| Scenario ID | Expected behavior | Live result | Evidence file(s) |',
            '| --- | --- | --- | --- |',
            '| active-combat-save | Save succeeds during active Vakuu child combat. |  |  |',
            '| active-combat-load | Reload restores the active fight, Vakuu state, contracts, locks, and Blood Debt coherently. |  |  |',
            '| parent-event-state | Parent event/room stack state is not lost or duplicated after active-fight reload. |  |  |',
            '| prefinished-save | Save succeeds around post-combat reward/return after Vakuu is defeated. |  |  |',
            '| prefinished-load | Reload restores the reward/return state without black screen or stale combat room. |  |  |',
            '| no-duplicate-heal-or-reward | Reload does not duplicate Ancient heal, Vakuu rewards, normal combat rewards, or parent event cleanup. |  |  |',
            '| log-clean | Logs contain no release-blocking save/load, room stack, parent event, or reward-screen error. |  |  |',
            '',
            'Do not use this row to cover victory without reload or failure/death. Those have separate verifier rows.'
        ) -join [Environment]::NewLine

        Write-ChecklistFiles -TemplateContent $vakuuSaveLoadChecklist -TemplatePath (Join-Path $rowEvidenceFull 'vakuu-save-load-checklist-template.md') -WorkingPath (Join-Path $rowEvidenceFull 'vakuu-save-load-checklist.md')
    }

    if ($row.Id -eq 'preview-tools-live-proof') {
        $previewToolsChecklist = @(
            '# Preview Tools Checklist',
            '',
            'Template reference for `preview-tools-checklist.md`. Fill the working `preview-tools-checklist.md` with live results before marking this row pass.',
            '',
            'Required evidence:',
            '- `environment.json` and `package-hashes.json` for the tested package.',
            '- `godot.log` and clean `godot-log-audit.json` from the same session.',
            '- `result-note.md` describing Crystal Sphere, transform preview, Prismatic Gem, save/reopen, and multiplayer disposition.',
            '- Screenshots are strongly recommended for preview on/off and transform preview/result comparison.',
            '',
            'Scenario rows:',
            '',
            '| Scenario ID | Expected behavior | Live result | Evidence file(s) |',
            '| --- | --- | --- | --- |',
            '| crystal-sphere-button | Crystal Sphere screen shows the Spire Plus peek control in the expected UI area. |  |  |',
            '| crystal-sphere-mask-only | Toggling peek only changes ScryMask visibility/opacity and does not call cell clear/reveal/claim behavior. |  |  |',
            '| crystal-sphere-no-reward-claim | No relic, potion, card, curse, or gold reward is granted or consumed by previewing. |  |  |',
            '| transform-preview-visible | Transform preview displays a concrete predicted replacement instead of cycling fake random cards. |  |  |',
            '| transform-preview-matches-result | The shown transform preview matches the actual transformed card result for the tested card(s). |  |  |',
            '| transform-preview-no-state-mutation | Previewing does not advance real RNG, create real mutable replacement cards, or change deck/reward state before confirmation. |  |  |',
            '| prismatic-gem-reward-hooks | Prismatic Gem preview reflects reward-modifying hooks without suppressing later Core reward changes. |  |  |',
            '| save-reopen-stability | Save/reopen around preview screens does not change the previewed result or corrupt the reward/room state. |  |  |',
            '| coop-gate-or-two-client-proof | Multiplayer behavior is either gated with a clear warning/log or proven with two-client evidence. |  |  |',
            '| log-clean | Logs contain no preview-tool exception, RNG drift warning, reward-state error, or co-op desync marker. |  |  |',
            '',
            'Do not use this row to cover Ancient reward relic visibility, Vakuu, Rootblight, or general UI art. Those have separate verifier rows.'
        ) -join [Environment]::NewLine

        Write-ChecklistFiles -TemplateContent $previewToolsChecklist -TemplatePath (Join-Path $rowEvidenceFull 'preview-tools-checklist-template.md') -WorkingPath (Join-Path $rowEvidenceFull 'preview-tools-checklist.md')
    }

    if ($row.Id -eq 'coop-disposition') {
        $coopChecklist = @(
            '# Co-op Disposition Checklist',
            '',
            'Template reference for `coop-disposition-checklist.md`. Fill the working `coop-disposition-checklist.md` with live results before marking this row pass.',
            '',
            'Required evidence:',
            '- `host-godot.log` and clean `host-godot-log-audit.json`.',
            '- `client-godot.log` and clean `client-godot-log-audit.json`.',
            '- `result-note.md` explaining tested host/client setup and any accepted deferrals.',
            '- Screenshots are strongly recommended for host/client selection, Ancient state, map state, combat state, and reconnect/save-load checks.',
            '',
            'Scenario rows:',
            '',
            '| Scenario ID | Expected behavior | Live result | Evidence file(s) |',
            '| --- | --- | --- | --- |',
            '| coop-host-join-clean-logs | Host and client load exactly BaseLib plus Spire Plus with matching package hashes and clean logs. |  |  |',
            '| coop-a11-a20-selection | A11-A20 selection/start-run behavior is recorded on host and client; selection visibility alone is not gameplay support. |  |  |',
            '| coop-ancients | Urda, Morvi, Lotha, and gated Vakuu have explicit host/client disposition notes for reward state and relic visibility. |  |  |',
            '| coop-root-eyes | Root Eyes map preview either stays gated in co-op or shows host/client-consistent map state with no desync. |  |  |',
            '| coop-rootblight | Rootblight ownership, combat/deck state, and Sprout growth are visible and consistent on host and client. |  |  |',
            '| coop-save-load-or-reconnect | Save/load or reconnect behavior is proven with host/client before-after logs, or explicitly deferred by owner. |  |  |',
            '| coop-preview-tools-disposition | Crystal Sphere, transform preview, and Prismatic Gem preview have a fairness/disposition note and no desync evidence. |  |  |',
            '| coop-release-note-disposition | Final co-op wording is explicit: supported with evidence, gated, unsupported, or owner-approved deferred. |  |  |',
            '',
            'Do not use this row to cover single-player behavior. Single-player Ancient, Vakuu, Rootblight, preview, and A19/A20 rows have separate verifier gates.'
        ) -join [Environment]::NewLine

        Write-ChecklistFiles -TemplateContent $coopChecklist -TemplatePath (Join-Path $rowEvidenceFull 'coop-disposition-checklist-template.md') -WorkingPath (Join-Path $rowEvidenceFull 'coop-disposition-checklist.md')
    }

    if ($row.Id -eq 'a19-a20-dedicated-boss-abilities') {
        $bossChecklist = @(
            '# A19/A20 Dedicated Boss Ability Checklist',
            '',
            'Template reference for `boss-ability-checklist.md`. Fill the working `boss-ability-checklist.md` with live results before marking this row pass.',
            '',
            'Required evidence:',
            '- `godot.log` from the live run with `SPIREPLUS_RELEASE_EVIDENCE_LOG=1` when possible.',
            '- `godot-log-audit.json` showing no release-blocking signatures.',
            '- `result-note.md` summarizing which Bosses were tested and which rows failed.',
            '- Screenshots are optional for this gameplay row but strongly recommended for intent and hover issues.',
            '',
            'Boss rows:',
            '',
            '| Boss | A19 ability | A20 Branded Form check | Live result | Evidence file(s) |',
            '| --- | --- | --- | --- | --- |',
            '| Ceremonial Beast | Holy Daze caps stun hits and grants Strength. | Branded Form grants the higher Strength value. |  |  |',
            '| The Kin | Martyr Oath consumes up to 2 follower-death stacks and updates attack intent. | Same-turn double follower death grants exactly 1 Artifact; attack bonus is higher. |  |  |',
            '| Vantom | Ink Return restores a percentage of cleared Slippery once. | Higher restore percentage/caps apply. |  |  |',
            '| Lagavulin Matriarch | Plating Wake grants Multiplating based on wake source and Soul Siphon reduces it. | Branded Form values and reduction differ as documented. |  |  |',
            '| Soul Fysh | Soul Tide converts unanswered Beckons into capped next-turn Block and grants Artifact on Intangible. | Higher per-Beckon Block and cap apply. |  |  |',
            '| Waterfall Giant | Unweakenable clears Weak/negative Strength for the explosion and applies Vulnerable to affected players. | Vulnerable duration is higher. |  |  |',
            '| Crab | Claw Calibration reacts to claw HP-ratio gaps and updates attack intent. | Lower threshold and higher attack bonus apply. |  |  |',
            '| Knowledge Demon | Marginal Note and Deep Thought add side costs without hard-locking Sloth/Waste Away. | Deep Thought cap and side-cost rules match v4.1. |  |  |',
            '| Insatiable Sandworm | Escape Fatigue grants Vigor after generated Escape cards. | Higher Vigor applies with team cap. |  |  |',
            '| Aeonglass | Time Sand Reflow adds Wither after Fade and clears by spent energy. | Eye Lasers extra hit appears in intent only while Time Sand remains. |  |  |',
            '| Queen | Royal Decree marks one Bound card and team-caps Majesty/Torch Head Strength. | Majesty cap and spend limit are higher. |  |  |',
            '| Test Subject | Experimental Record shows a phase-change notice and residual sample power. | Two different samples appear on phase change. |  |  |',
            '',
            'Do not use this row to cover A11 map traversal, Ancient UI, Vakuu fight return, or co-op. Those have separate verifier rows.'
        ) -join [Environment]::NewLine

        Write-ChecklistFiles -TemplateContent $bossChecklist -TemplatePath (Join-Path $rowEvidenceFull 'boss-ability-checklist-template.md') -WorkingPath (Join-Path $rowEvidenceFull 'boss-ability-checklist.md')
    }
}

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
