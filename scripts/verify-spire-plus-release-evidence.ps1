param(
    [string]$EvidenceRoot = ".tools\runtime-evidence\release-ready-manual",

    [string]$ManifestPath,

    [string]$PackageSha256 = "",

    [string]$PackagePath = "",

    [string]$ExpectedGameVersion = "0.107.1",

    [string]$ExpectedRitsuLibVersion = "0.4.34",

    [string]$ExpectedRitsuCompatBranch = "0.107.1",

    [int]$ExpectedPatchCount = 168,

    [int]$MinScreenshotWidth = 800,

    [int]$MinScreenshotHeight = 450,

    [switch]$WriteTemplate,

    [switch]$AllowDeferred,

    [switch]$WritePassMarker,

    [string]$PassMarkerPath
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
. (Join-Path $PSScriptRoot 'spire-plus-package-evidence.ps1')

$currentPackageVersion = Get-SpirePlusManifestVersion -RepoRoot $repoRoot
$canonicalPackagePath = Get-SpirePlusPackageRelativePath -RepoRoot $repoRoot
$canonicalPackageFullPath = Resolve-SpirePlusPackagePath -RepoRoot $repoRoot -PackagePath $canonicalPackagePath
$canonicalPackageSha256 = ''
if (Test-Path -LiteralPath $canonicalPackageFullPath -PathType Leaf) {
    $canonicalPackageSha256 = Get-SpirePlusPackageSha256 -RepoRoot $repoRoot -PackagePath $canonicalPackagePath
}

if ([string]::IsNullOrWhiteSpace($PackagePath)) {
    $PackagePath = $canonicalPackagePath
}

if ([string]::IsNullOrWhiteSpace($PackageSha256)) {
    if (-not [string]::IsNullOrWhiteSpace($canonicalPackageSha256)) {
        $PackageSha256 = $canonicalPackageSha256
    }
}
if (-not [string]::IsNullOrWhiteSpace($PackageSha256)) {
    $PackageSha256 = $PackageSha256.ToUpperInvariant()
}

$requiredReleaseRows = @(
    @{ Id = 'fresh-current-package-loader-smoke'; Kind = 'loader'; Label = 'Fresh current-package loader smoke with current package hashes and clean log audit' },
    @{
        Id = 'mod-settings-current-display'
        Kind = 'clicked-ui'
        Label = 'Current Spire Plus Mod Settings list and config page proof'
        ExtraRequiredFiles = @('mod-settings-checklist.md')
    },
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
    },
    @{
        Id = 'player-text-tooltip-readability'
        Kind = 'gameplay'
        Label = 'Player-facing text, tooltip, and hover readability'
        ExtraRequiredFiles = @('player-text-qa-checklist.md')
    },
    @{
        Id = 'art-resource-routing-live-preview'
        Kind = 'clicked-ui'
        Label = 'Live UI preview proves event art, map icons, run-history icons, relic art, card art, and power art are not misrouted'
        ExtraRequiredFiles = @('art-resource-routing-checklist.md')
    },
    @{
        Id = 'vakuu-victory-no-black-screen'
        Kind = 'gameplay'
        Label = 'Vakuu victory returns to the event without a black screen'
        ExtraRequiredFiles = @('vakuu-victory-checklist.md')
    },
    @{
        Id = 'vakuu-failure-death-path'
        Kind = 'gameplay'
        Label = 'Vakuu failure and death path does not softlock'
        ExtraRequiredFiles = @('vakuu-failure-death-checklist.md')
    },
    @{
        Id = 'vakuu-active-fight-save-load'
        Kind = 'save-load'
        Label = 'Vakuu active child-combat save/load'
        ExtraRequiredFiles = @('vakuu-save-load-checklist.md')
    },
    @{ Id = 'ancient-state-save-load'; Kind = 'save-load'; Label = 'Urda, Morvi, Lotha, and Ancient reward state save/load' },
    @{
        Id = 'rootblight-visual-behavior'
        Kind = 'gameplay'
        Label = 'Rootblight and Blight Sprout visual/gameplay behavior'
        ExtraRequiredFiles = @('rootblight-behavior-checklist.md')
    },
    @{ Id = 'a11-natural-route-traversal'; Kind = 'gameplay'; Label = 'Natural A11 route click traversal' },
    @{
        Id = 'ascension-selector-localization'
        Kind = 'clicked-ui'
        Label = 'A11-A20 character-select Ascension selector localization'
    },
    @{
        Id = 'a19-a20-dedicated-boss-abilities'
        Kind = 'gameplay'
        Label = 'A19/A20 dedicated boss abilities and Branded Form live combat proof'
        ExtraRequiredFiles = @('boss-ability-checklist.md')
    },
    @{ Id = 'disable-mod-gameplay'; Kind = 'gameplay'; Label = 'RitsuLib-only disabled Spire Plus gameplay comparison' },
    @{
        Id = 'preview-tools-live-proof'
        Kind = 'preview-tools'
        Label = 'Live Preview tools evidence for Crystal Sphere and transform preview'
        ExtraRequiredFiles = @('preview-tools-checklist.md')
    },
    @{
        Id = 'coop-disposition'
        Kind = 'coop'
        Label = 'Two-client co-op disposition or explicit release-note deferral'
        ExtraRequiredFiles = @('coop-disposition-checklist.md')
    }
)

$invalidEvidenceNotePattern = '(?i)\b(not counted|invalid|main menu|wrong surface|covered by|not gameplay evidence|do not satisfy|does not satisfy|loader health only|marker-only|origin not proven)\b'
$invalidChecklistCellPattern = '(?i)^\s*(pending|todo|tbd|n/?a|none|not tested|untested|unknown|skip|skipped|-+)?\s*$'
$requiredReleaseLogOriginProofStatus = 'owner-live-release-log'
$releaseLogOriginForbiddenNotePattern = '(?i)(marker-only|baseline-log-markers|origin-not-proven|pending-owner-run|pending-owner-live-release-log|noncanonical-override-test-only|<[^>]+>|runtime[- ]baseline|godot\.log\.after-launch|runtime-baseline-log-check|preflight\.json)'
$releaseLogOriginPlaceholderValuePattern = '(?i)(^\s*-+\s*$|\b(todo|tbd|n/?a|none|unknown|pending)\b)'
$markerOnlyRuntimeBaselinePattern = '(?i)(marker-only|baseline-log-markers|origin-not-proven|pending-owner-run|noncanonical-override-test-only)'
$markerOnlyRuntimeBaselineRowFields = @(
    'EvidenceBoundary',
    'EvidenceKind',
    'EvidenceType',
    'RuntimeEvidenceKind',
    'RuntimeBaselineStatus',
    'LogOriginProofStatus',
    'TrustAnchorMode'
)
$markerOnlyRuntimeBaselineHardSentinelFiles = @(
    'runtime-baseline-log-check.json',
    'runtime-baseline-notes.md',
    'godot.log.after-launch',
    'preflight.json'
)
$markerOnlyRuntimeBaselineSoftSentinelFiles = @(
    'main-menu-screenshot.png'
)
$markerOnlyRuntimeBaselineManifestSpecificFields = @(
    'GodotLogAfterLaunch',
    'GodotLogAfterLaunchRecord',
    'RuntimeBaselineLogCheckRecord',
    'BaselineLogCheckUpdatedAtUtc',
    'StartupMainMenuScreenshotStatus',
    'PendingOwnerRunArtifacts',
    'GameRootAnchorMode',
    'PackageAnchorMode',
    'TrustAnchorMode'
)
$markerOnlyRuntimeBaselineManifestJsonPattern = '(?i)(RuntimeBaseline|GodotLogAfterLaunch|BaselineLogCheck|StartupMainMenuScreenshotStatus|PendingOwnerRunArtifacts|TrustAnchorMode|noncanonical-override-test-only|runtime-baseline-|godot\.log\.after-launch|runtime-baseline-log-check\.json|runtime-baseline-notes\.md)'

$requiredBossAbilityRows = @(
    'Ceremonial Beast',
    'The Kin',
    'Vantom',
    'Lagavulin Matriarch',
    'Soul Fysh',
    'Waterfall Giant',
    'Crab',
    'Knowledge Demon',
    'Insatiable Sandworm',
    'Aeonglass',
    'Queen',
    'Test Subject'
)

$requiredAncientRewardRows = @(
    @{ Ancient = 'Urda'; Reward = 'seedbed' },
    @{ Ancient = 'Urda'; Reward = 'humus_pact' },
    @{ Ancient = 'Urda'; Reward = 'molting' },
    @{ Ancient = 'Urda'; Reward = 'moss_map' },
    @{ Ancient = 'Urda'; Reward = 'trial_branch' },
    @{ Ancient = 'Urda'; Reward = 'shallow_root_relic' },
    @{ Ancient = 'Urda'; Reward = 'rooted_route' },
    @{ Ancient = 'Urda'; Reward = 'after_rain' },
    @{ Ancient = 'Urda'; Reward = 'root_sight' },
    @{ Ancient = 'Urda'; Reward = 'seed_bank' },
    @{ Ancient = 'Morvi'; Reward = 'forbidden_loan' },
    @{ Ancient = 'Morvi'; Reward = 'misprint_press' },
    @{ Ancient = 'Morvi'; Reward = 'red_ink_overdraft' },
    @{ Ancient = 'Morvi'; Reward = 'overdue_library' },
    @{ Ancient = 'Morvi'; Reward = 'open_book_exam' },
    @{ Ancient = 'Morvi'; Reward = 'paperstorm' },
    @{ Ancient = 'Morvi'; Reward = 'blueprint_proof' },
    @{ Ancient = 'Morvi'; Reward = 'debt_settlement' },
    @{ Ancient = 'Lotha'; Reward = 'mirror_rebuttal' },
    @{ Ancient = 'Lotha'; Reward = 'mirror_hall_echo' },
    @{ Ancient = 'Lotha'; Reward = 'presumption' },
    @{ Ancient = 'Lotha'; Reward = 'closed_court' },
    @{ Ancient = 'Lotha'; Reward = 'deferred_verdict' },
    @{ Ancient = 'Lotha'; Reward = 'death_reprieve' },
    @{ Ancient = 'Lotha'; Reward = 'single_sentence' },
    @{ Ancient = 'Lotha'; Reward = 'public_evidence' },
    @{ Ancient = 'Vakuu'; Reward = 'fight_option' },
    @{ Ancient = 'Vakuu'; Reward = 'victory_non_vakuu_choices' },
    @{ Ancient = 'Vakuu event'; Reward = 'sere_talon_pickup' },
    @{ Ancient = 'Tanx event'; Reward = 'claws_maul_transform' }
)

$requiredRootblightRows = @(
    'rootblight-start-eligibility',
    'normal-rootblight-continuity',
    'elite-single-sprout',
    'boss-two-sprouts-staggered',
    'husk-exhaust-block-timing',
    'combat-end-growth',
    'rootblight-cap-four',
    'rootblight-save-load',
    'ui-hover-art-readability'
)

$requiredArtRoutingRows = @(
    'title-home-preview',
    'urda-clicked-background',
    'morvi-clicked-background',
    'lotha-clicked-background',
    'vakuu-clicked-background',
    'map-icons',
    'run-history-icons',
    'option-relic-icons',
    'lasting-relic-icons',
    'card-art',
    'power-icons',
    'no-placeholder-or-official-art'
)

$requiredPlayerTextRows = @(
    'ascension-a11-a20',
    'firemark-and-banner',
    'boss-dedicated-abilities',
    'ancient-choice-text',
    'ancient-relic-hover',
    'cards-status-curses',
    'map-hover-stacks',
    'preview-tools-text',
    'vakuu-contracts',
    'en-zhs-key-parity'
)

$requiredVakuuVictoryRows = @(
    'fight-start-scene',
    'contract-turns',
    'locks-blood-debt',
    'victory-return',
    'non-vakuu-rewards',
    'no-black-screen',
    'log-clean'
)

$requiredVakuuFailureDeathRows = @(
    'fight-start-scene',
    'failure-path',
    'death-path',
    'room-state-after-exit',
    'no-softlock',
    'log-clean'
)

$requiredVakuuSaveLoadRows = @(
    'active-combat-save',
    'active-combat-load',
    'parent-event-state',
    'prefinished-save',
    'prefinished-load',
    'no-duplicate-heal-or-reward',
    'log-clean'
)

$requiredPreviewToolsRows = @(
    'crystal-sphere-button',
    'crystal-sphere-mask-only',
    'crystal-sphere-no-reward-claim',
    'transform-preview-visible',
    'transform-preview-matches-result',
    'transform-preview-no-state-mutation',
    'prismatic-gem-reward-hooks',
    'save-reopen-stability',
    'coop-gate-or-two-client-proof',
    'log-clean'
)

$requiredCoopRows = @(
    'coop-host-join-clean-logs',
    'coop-a11-a20-selection',
    'coop-ancients',
    'coop-root-eyes',
    'coop-rootblight',
    'coop-save-load-or-reconnect',
    'coop-preview-tools-disposition',
    'coop-release-note-disposition'
)

$requiredModSettingsRows = @(
    'ritsulib-visible-enabled',
    'spire-plus-list-display-name',
    'spire-plus-config-page-current-name',
    'ritsulib-migration-status-section',
    'ritsulib-runtime-dependency-card',
    'ritsulib-proof-boundary-card',
    'preview-tools-controls-render',
    'technical-id-compatibility',
    'legacy-mod-surfaces-absent',
    'clean-log-config-registration'
)

function Resolve-WorkspacePath {
    param([Parameter(Mandatory = $true)][string]$Path)

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }

    return [System.IO.Path]::GetFullPath((Join-Path $repoRoot $Path))
}

function Test-PathWithin {
    param(
        [Parameter(Mandatory = $true)][string]$BasePath,
        [Parameter(Mandatory = $true)][string]$ChildPath
    )

    $trimChars = [char[]]@([System.IO.Path]::DirectorySeparatorChar, [System.IO.Path]::AltDirectorySeparatorChar)
    $baseFull = [System.IO.Path]::GetFullPath($BasePath).TrimEnd($trimChars)
    $childFull = [System.IO.Path]::GetFullPath($ChildPath).TrimEnd($trimChars)
    $comparison = [System.StringComparison]::OrdinalIgnoreCase

    return $childFull.Equals($baseFull, $comparison) -or
        $childFull.StartsWith($baseFull + [System.IO.Path]::DirectorySeparatorChar, $comparison)
}

function Initialize-WindowsFileIdentityType {
    if (-not [System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform([System.Runtime.InteropServices.OSPlatform]::Windows)) {
        return $false
    }

    if ('SpirePlusReleaseEvidenceFileIdentity' -as [type]) {
        return $true
    }

    try {
        Add-Type -TypeDefinition @'
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

public static class SpirePlusReleaseEvidenceFileIdentity
{
    [StructLayout(LayoutKind.Sequential)]
    private struct ByHandleFileInformation
    {
        public uint FileAttributes;
        public System.Runtime.InteropServices.ComTypes.FILETIME CreationTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastAccessTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastWriteTime;
        public uint VolumeSerialNumber;
        public uint FileSizeHigh;
        public uint FileSizeLow;
        public uint NumberOfLinks;
        public uint FileIndexHigh;
        public uint FileIndexLow;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetFileInformationByHandle(SafeFileHandle hFile, out ByHandleFileInformation lpFileInformation);

    public static string GetIdentity(string path)
    {
        using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
        {
            ByHandleFileInformation information;
            if (!GetFileInformationByHandle(stream.SafeFileHandle, out information))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return string.Format(
                "{0:x8}:{1:x8}:{2:x8}",
                information.VolumeSerialNumber,
                information.FileIndexHigh,
                information.FileIndexLow);
        }
    }

    public static uint GetLinkCount(string path)
    {
        using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
        {
            ByHandleFileInformation information;
            if (!GetFileInformationByHandle(stream.SafeFileHandle, out information))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return information.NumberOfLinks;
        }
    }
}
'@ -ErrorAction Stop
        return $true
    } catch {
        return $false
    }
}

function Get-ExistingPathPhysicalIdentity {
    param([AllowEmptyString()][string]$Path)

    if ([string]::IsNullOrWhiteSpace($Path) -or -not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        return ''
    }

    if (-not (Initialize-WindowsFileIdentityType)) {
        return ''
    }

    try {
        return [SpirePlusReleaseEvidenceFileIdentity]::GetIdentity([System.IO.Path]::GetFullPath($Path))
    } catch {
        return ''
    }
}

function Test-SameExistingPathPhysicalIdentity {
    param(
        [AllowEmptyString()][string]$Left,
        [AllowEmptyString()][string]$Right
    )

    $leftIdentity = Get-ExistingPathPhysicalIdentity -Path $Left
    if ([string]::IsNullOrWhiteSpace($leftIdentity)) {
        return $false
    }

    $rightIdentity = Get-ExistingPathPhysicalIdentity -Path $Right
    return -not [string]::IsNullOrWhiteSpace($rightIdentity) -and
        [string]::Equals($leftIdentity, $rightIdentity, [System.StringComparison]::OrdinalIgnoreCase)
}

function Get-ExistingPathHardlinkCount {
    param([AllowEmptyString()][string]$Path)

    if ([string]::IsNullOrWhiteSpace($Path) -or -not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        return $null
    }

    if (-not (Initialize-WindowsFileIdentityType)) {
        return $null
    }

    try {
        return [int][SpirePlusReleaseEvidenceFileIdentity]::GetLinkCount([System.IO.Path]::GetFullPath($Path))
    } catch {
        return $null
    }
}

function Add-NoReparsePointInPathFailures {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$Label,
        [Parameter(Mandatory = $true)][System.Collections.Generic.List[string]]$Failures,
        [string]$StopDirectory = $repoRoot
    )

    $current = [System.IO.Path]::GetFullPath($Path).TrimEnd([char[]]@('\', '/'))
    while (-not (Test-Path -LiteralPath $current) -and -not [string]::IsNullOrWhiteSpace($current)) {
        $parent = Split-Path -Parent $current
        if ([string]::IsNullOrWhiteSpace($parent) -or [System.StringComparer]::OrdinalIgnoreCase.Equals($parent, $current)) {
            break
        }

        $current = $parent
    }

    $normalizedStop = [System.IO.Path]::GetFullPath($StopDirectory).TrimEnd([char[]]@('\', '/'))
    if (-not (Test-PathWithin -BasePath $normalizedStop -ChildPath $current)) {
        Add-Failure -Failures $Failures -Message "$Label path leaves the trusted workspace while checking reparse points: $current."
        return
    }

    while (-not [string]::IsNullOrWhiteSpace($current) -and (Test-PathWithin -BasePath $normalizedStop -ChildPath $current)) {
        $item = Get-Item -LiteralPath $current -Force
        if (($item.Attributes -band [System.IO.FileAttributes]::ReparsePoint) -ne 0) {
            Add-Failure -Failures $Failures -Message "$Label path must not pass through a reparse point: $current."
            return
        }

        if ([System.StringComparer]::OrdinalIgnoreCase.Equals(([System.IO.Path]::GetFullPath($current).TrimEnd([char[]]@('\', '/'))), $normalizedStop)) {
            break
        }

        $current = Split-Path -Parent $current
    }
}

function Add-OrdinaryEvidenceFileFailures {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$Label,
        [Parameter(Mandatory = $true)][System.Collections.Generic.List[string]]$Failures
    )

    Add-NoReparsePointInPathFailures -Path $Path -Label $Label -Failures $Failures
    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        return
    }

    $item = Get-Item -LiteralPath $Path -Force
    if (($item.Attributes -band [System.IO.FileAttributes]::ReparsePoint) -ne 0) {
        Add-Failure -Failures $Failures -Message "$Label must not be a reparse point: $Path."
    }

    $linkCount = Get-ExistingPathHardlinkCount -Path $Path
    if ($null -eq $linkCount) {
        Add-Failure -Failures $Failures -Message "$Label hardlink count could not be determined; release evidence must fail closed: $Path."
    } elseif ($linkCount -gt 1) {
        Add-Failure -Failures $Failures -Message "$Label must not be a hardlink with multiple filesystem names: $Path linkCount=$linkCount."
    }
}

function Add-OutputAliasFailures {
    param(
        [Parameter(Mandatory = $true)][string]$OutputPath,
        [Parameter(Mandatory = $true)][string[]]$ProtectedPaths,
        [Parameter(Mandatory = $true)][string]$Label,
        [Parameter(Mandatory = $true)][System.Collections.Generic.List[string]]$Failures
    )

    if (-not (Test-Path -LiteralPath $OutputPath -PathType Leaf)) {
        return
    }

    Add-OrdinaryEvidenceFileFailures -Path $OutputPath -Label $Label -Failures $Failures
    foreach ($protectedPath in @($ProtectedPaths)) {
        if ([string]::IsNullOrWhiteSpace($protectedPath) -or -not (Test-Path -LiteralPath $protectedPath -PathType Leaf)) {
            continue
        }

        $resolvedOutputPath = [System.IO.Path]::GetFullPath($OutputPath)
        $resolvedProtectedPath = [System.IO.Path]::GetFullPath($protectedPath)
        if ([System.StringComparer]::OrdinalIgnoreCase.Equals($resolvedOutputPath, $resolvedProtectedPath)) {
            Add-Failure -Failures $Failures -Message "$Label must not overwrite protected release evidence: $resolvedProtectedPath."
            continue
        }

        if (Test-SameExistingPathPhysicalIdentity -Left $resolvedOutputPath -Right $resolvedProtectedPath) {
            Add-Failure -Failures $Failures -Message "$Label must not share physical identity with protected release evidence: $resolvedOutputPath -> $resolvedProtectedPath."
        }
    }
}

function Resolve-EvidenceFilePath {
    param(
        [Parameter(Mandatory = $true)][string]$EvidenceDir,
        [Parameter(Mandatory = $true)][string]$Path
    )

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }

    return [System.IO.Path]::GetFullPath((Join-Path $EvidenceDir $Path))
}

function Get-PropertyValue {
    param(
        [Parameter(Mandatory = $true)]$Object,
        [Parameter(Mandatory = $true)][string]$Name,
        $Default = $null
    )

    if ($Object.PSObject.Properties.Name -contains $Name) {
        return $Object.$Name
    }

    return $Default
}

function Get-DefaultRequiredFiles {
    param([Parameter(Mandatory = $true)][string]$Kind)

    switch ($Kind) {
        'loader' {
            return @('run-manifest.json', 'command.txt', 'environment.json', 'package-hashes.json', 'godot.log', 'godot-log-audit.json', 'enabled-mods.txt', 'log-origin-note.md')
        }
        'clicked-ui' {
            return @('run-manifest.json', 'command.txt', 'window-preflight.json', 'godot.log', 'godot-log-audit.json', 'route-note.md', 'log-origin-note.md')
        }
        'save-load' {
            return @('run-manifest.json', 'command.txt', 'godot.log', 'godot-log-audit.json', 'save-load-note.md', 'log-origin-note.md')
        }
        'coop' {
            return @('run-manifest.json', 'command.txt', 'host-godot.log', 'host-godot-log-audit.json', 'client-godot.log', 'client-godot-log-audit.json', 'result-note.md', 'log-origin-note.md')
        }
        'preview-tools' {
            return @('run-manifest.json', 'command.txt', 'environment.json', 'package-hashes.json', 'godot.log', 'godot-log-audit.json', 'result-note.md', 'log-origin-note.md')
        }
        default {
            return @('run-manifest.json', 'command.txt', 'godot.log', 'godot-log-audit.json', 'result-note.md', 'log-origin-note.md')
        }
    }
}

function Get-RequiredRowExtraFiles {
    param([Parameter(Mandatory = $true)]$RequiredRow)

    if ($RequiredRow.ContainsKey('ExtraRequiredFiles')) {
        return @($RequiredRow.ExtraRequiredFiles)
    }

    return @()
}

function Merge-RequiredEvidenceFiles {
    param(
        [Parameter(Mandatory = $true)][string[]]$DefaultFiles,
        [object[]]$RowFiles = @()
    )

    $merged = [System.Collections.Generic.List[string]]::new()
    foreach ($file in $DefaultFiles) {
        if (-not [string]::IsNullOrWhiteSpace($file) -and -not $merged.Contains($file)) {
            [void]$merged.Add($file)
        }
    }

    foreach ($file in $RowFiles) {
        $fileString = [string]$file
        if (-not [string]::IsNullOrWhiteSpace($fileString) -and -not $merged.Contains($fileString)) {
            [void]$merged.Add($fileString)
        }
    }

    return @($merged)
}

function Get-RequiredEvidenceFilesForRow {
    param([Parameter(Mandatory = $true)]$RequiredRow)

    return @(Merge-RequiredEvidenceFiles `
            -DefaultFiles (Get-DefaultRequiredFiles -Kind $RequiredRow.Kind) `
            -RowFiles (Get-RequiredRowExtraFiles -RequiredRow $RequiredRow))
}

function Read-JsonOrNull {
    param([Parameter(Mandatory = $true)][string]$Path)

    try {
        return Get-Content -LiteralPath $Path -Raw -Encoding UTF8 | ConvertFrom-Json
    } catch {
        return $null
    }
}

function Test-NoLaunchOwnerRunRuntimeBaselineFields {
    param(
        $OwnerRunRequired,
        $DoesNotLaunchGame,
        [AllowEmptyString()][string]$LaunchMethod
    )

    return $OwnerRunRequired -is [bool] -and [bool]$OwnerRunRequired -and
        $DoesNotLaunchGame -is [bool] -and [bool]$DoesNotLaunchGame -and
        [string]::Equals($LaunchMethod, 'owner-steam-launch-required', [System.StringComparison]::OrdinalIgnoreCase)
}

function Test-ReleaseLogOriginNote {
    param(
        [Parameter(Mandatory = $true)][string]$NotePath,
        [Parameter(Mandatory = $true)][string]$RowId,
        [Parameter(Mandatory = $true)]$Failures
    )

    $note = Get-Content -LiteralPath $NotePath -Raw -Encoding UTF8
    if ($note -notmatch "(?im)^\s*LogOriginProofStatus\s*:\s*$([regex]::Escape($requiredReleaseLogOriginProofStatus))\s*$") {
        Add-Failure -Failures $Failures -Message "Row $RowId log-origin-note.md must declare LogOriginProofStatus: $requiredReleaseLogOriginProofStatus."
    }

    $sourceMatch = [regex]::Match($note, '(?im)^\s*Source\s*:\s*(?<Value>\S.*)$')
    if (-not $sourceMatch.Success) {
        Add-Failure -Failures $Failures -Message "Row $RowId log-origin-note.md must include a non-empty Source line for the owner/live release session."
    } elseif ($sourceMatch.Groups['Value'].Value -match $releaseLogOriginPlaceholderValuePattern) {
        Add-Failure -Failures $Failures -Message "Row $RowId log-origin-note.md Source line must not be a placeholder value."
    }

    $logFilesMatch = [regex]::Match($note, '(?im)^\s*Log files?\s*:\s*(?<Value>\S.*)$')
    if (-not $logFilesMatch.Success) {
        Add-Failure -Failures $Failures -Message "Row $RowId log-origin-note.md must include a non-empty Log files line naming the row log file(s)."
    } elseif ($logFilesMatch.Groups['Value'].Value -match $releaseLogOriginPlaceholderValuePattern) {
        Add-Failure -Failures $Failures -Message "Row $RowId log-origin-note.md Log files line must not be a placeholder value."
    }

    if ($note -match $releaseLogOriginForbiddenNotePattern) {
        Add-Failure -Failures $Failures -Message "Row $RowId log-origin-note.md must not retain pending/template placeholders or reference beta.135 runtime baseline, marker-only checks, no-launch owner-run scaffolds, or godot.log.after-launch."
    }
}

function Get-MarkerOnlyRuntimeBaselineEvidenceReasons {
    param(
        [Parameter(Mandatory = $true)]$Row,
        [Parameter(Mandatory = $true)][string]$EvidenceDir
    )

    $reasons = [System.Collections.Generic.List[string]]::new()
    foreach ($fieldName in $markerOnlyRuntimeBaselineRowFields) {
        $value = [string](Get-PropertyValue -Object $Row -Name $fieldName -Default '')
        if (-not [string]::IsNullOrWhiteSpace($value) -and $value -match $markerOnlyRuntimeBaselinePattern) {
            [void]$reasons.Add("row.$fieldName=$value")
        }
    }

    $rowOwnerRunRequired = Get-PropertyValue -Object $Row -Name 'OwnerRunRequired' -Default $null
    $rowDoesNotLaunchGame = Get-PropertyValue -Object $Row -Name 'DoesNotLaunchGame' -Default $null
    $rowLaunchMethod = [string](Get-PropertyValue -Object $Row -Name 'LaunchMethod' -Default '')
    if (Test-NoLaunchOwnerRunRuntimeBaselineFields -OwnerRunRequired $rowOwnerRunRequired -DoesNotLaunchGame $rowDoesNotLaunchGame -LaunchMethod $rowLaunchMethod) {
        [void]$reasons.Add('row is a no-launch owner-run runtime baseline scaffold')
    }

    $evidenceDirLeaf = Split-Path -Leaf $EvidenceDir
    if ($evidenceDirLeaf -match '(?i)^beta135-runtime-baseline[-_]') {
        [void]$reasons.Add("EvidenceDir leaf is beta.135 runtime baseline scaffold: $evidenceDirLeaf")
    }

    $hardSentinelFound = $false
    foreach ($relativePath in $markerOnlyRuntimeBaselineHardSentinelFiles) {
        $sentinelPath = Join-Path $EvidenceDir $relativePath
        if (Test-Path -LiteralPath $sentinelPath -PathType Leaf) {
            $hardSentinelFound = $true
            [void]$reasons.Add("baseline scaffold sentinel file present: $relativePath")
        }
    }

    if ($hardSentinelFound) {
        foreach ($relativePath in $markerOnlyRuntimeBaselineSoftSentinelFiles) {
            $sentinelPath = Join-Path $EvidenceDir $relativePath
            if (Test-Path -LiteralPath $sentinelPath -PathType Leaf) {
                [void]$reasons.Add("baseline scaffold companion file present: $relativePath")
            }
        }
    }

    $manifestPath = Join-Path $EvidenceDir 'run-manifest.json'
    if (Test-Path -LiteralPath $manifestPath -PathType Leaf) {
        $manifest = Read-JsonOrNull -Path $manifestPath
        if ($null -eq $manifest) {
            [void]$reasons.Add('run-manifest.json is present but not parseable')
            return @($reasons)
        }

        $manifestEvidenceKind = [string](Get-PropertyValue -Object $manifest -Name 'EvidenceKind' -Default '')
        $manifestEvidenceBoundary = [string](Get-PropertyValue -Object $manifest -Name 'EvidenceBoundary' -Default '')
        if (-not [string]::Equals($manifestEvidenceKind, 'release-evidence', [System.StringComparison]::OrdinalIgnoreCase) -or
            -not [string]::Equals($manifestEvidenceBoundary, 'live-release-row-required', [System.StringComparison]::OrdinalIgnoreCase)) {
            [void]$reasons.Add('run-manifest.json has no release-evidence provenance')
        }

        foreach ($fieldName in @('Status', 'RuntimeLogStatus', 'RuntimeAuditStatus', 'BaselineLogCheckStatus', 'LogOriginProofStatus', 'LaunchMethod', 'GameRootAnchorMode', 'PackageAnchorMode', 'TrustAnchorMode')) {
            $value = [string](Get-PropertyValue -Object $manifest -Name $fieldName -Default '')
            if (-not [string]::IsNullOrWhiteSpace($value) -and $value -match $markerOnlyRuntimeBaselinePattern) {
                [void]$reasons.Add("run-manifest.json $fieldName=$value")
            }
        }

        foreach ($fieldName in $markerOnlyRuntimeBaselineManifestSpecificFields) {
            if ($manifest.PSObject.Properties.Name -contains $fieldName) {
                [void]$reasons.Add("run-manifest.json contains beta.135 runtime baseline field: $fieldName")
            }
        }

        $manifestJson = $manifest | ConvertTo-Json -Depth 20 -Compress
        if ($manifestJson -match $markerOnlyRuntimeBaselineManifestJsonPattern) {
            [void]$reasons.Add('run-manifest.json contains beta.135 runtime baseline-specific keys or values')
        }

        $ownerRunRequired = Get-PropertyValue -Object $manifest -Name 'OwnerRunRequired' -Default $null
        $doesNotLaunchGame = Get-PropertyValue -Object $manifest -Name 'DoesNotLaunchGame' -Default $null
        $launchMethod = [string](Get-PropertyValue -Object $manifest -Name 'LaunchMethod' -Default '')
        if (Test-NoLaunchOwnerRunRuntimeBaselineFields -OwnerRunRequired $ownerRunRequired -DoesNotLaunchGame $doesNotLaunchGame -LaunchMethod $launchMethod) {
            [void]$reasons.Add('run-manifest.json is a no-launch owner-run runtime baseline scaffold')
        }

        $forbiddenClaims = @(Get-PropertyValue -Object $manifest -Name 'ForbiddenClaims' -Default @())
        foreach ($claim in $forbiddenClaims) {
            if ([string]::Equals([string]$claim, 'release-ready', [System.StringComparison]::OrdinalIgnoreCase)) {
                [void]$reasons.Add('run-manifest.json forbids release-ready claims')
                break
            }
        }
    }

    return @($reasons)
}

function Test-NativeJsonIntegerValue {
    param([object]$Value)

    return $Value -is [byte] -or
        $Value -is [sbyte] -or
        $Value -is [int16] -or
        $Value -is [uint16] -or
        $Value -is [int] -or
        $Value -is [uint32] -or
        $Value -is [long] -or
        $Value -is [uint64]
}

function Get-AuditJsonItems {
    param([Parameter(Mandatory = $true)][string]$RawJson)

    if ([string]::IsNullOrWhiteSpace($RawJson) -or -not $RawJson.TrimStart().StartsWith('[', [System.StringComparison]::Ordinal)) {
        return @()
    }

    try {
        $audit = $RawJson | ConvertFrom-Json
    } catch {
        return @()
    }
    if ($null -eq $audit) {
        return @()
    }

    if ($audit -is [System.Array]) {
        return @($audit)
    }

    return @($audit)
}

function Get-AuditSignatureVector {
    param([Parameter(Mandatory = $true)]$AuditItem)

    $signatureHitsProperty = @($AuditItem.PSObject.Properties | Where-Object { [string]::Equals($_.Name, 'SignatureHits', [System.StringComparison]::Ordinal) } | Select-Object -First 1)
    if ($signatureHitsProperty.Count -ne 1 -or $null -eq $signatureHitsProperty[0].Value -or -not ($signatureHitsProperty[0].Value -is [System.Array])) {
        return @()
    }

    $vector = [System.Collections.Generic.List[string]]::new()
    foreach ($hit in @($signatureHitsProperty[0].Value)) {
        if (-not ($hit.PSObject.Properties.Name -contains 'Name') -or [string]::IsNullOrWhiteSpace([string]$hit.Name)) {
            return @()
        }

        if (-not ($hit.PSObject.Properties.Name -contains 'Count') -or -not (Test-NativeJsonIntegerValue -Value $hit.Count)) {
            return @()
        }

        $vector.Add("$([string]$hit.Name)=$([long]$hit.Count)") | Out-Null
    }

    return @($vector.ToArray() | Sort-Object)
}

function Test-StringArrayEquals {
    param(
        [string[]]$Actual,
        [string[]]$Expected
    )

    $actualValues = @($Actual)
    $expectedValues = @($Expected)
    if ($actualValues.Count -ne $expectedValues.Count) {
        return $false
    }

    for ($i = 0; $i -lt $actualValues.Count; $i++) {
        if (-not [string]::Equals($actualValues[$i], $expectedValues[$i], [System.StringComparison]::Ordinal)) {
            return $false
        }
    }

    return $true
}

function Test-ReleaseRowTargetManifest {
    param(
        [Parameter(Mandatory = $true)][string]$TargetManifestPath,
        [Parameter(Mandatory = $true)][string]$RowId,
        [Parameter(Mandatory = $true)][System.Collections.Generic.List[string]]$Failures,
        [Parameter(Mandatory = $true)][string]$ExpectedPackageVersion,
        [Parameter(Mandatory = $true)][string]$ExpectedPackagePath,
        [Parameter(Mandatory = $true)][string]$ExpectedPackageFullPath,
        [Parameter(Mandatory = $true)][string]$ExpectedPackageSha256,
        [Parameter(Mandatory = $true)][string]$ExpectedGameVersion,
        [Parameter(Mandatory = $true)][string]$ExpectedRitsuLibVersion,
        [Parameter(Mandatory = $true)][string]$ExpectedRitsuCompatBranch,
        [Parameter(Mandatory = $true)][int]$ExpectedPatchCount
    )

    try {
        $targetManifest = Get-Content -LiteralPath $TargetManifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
    } catch {
        Add-Failure -Failures $Failures -Message "Row $RowId run-manifest.json is not valid JSON: $TargetManifestPath."
        return
    }

    if ($null -eq $targetManifest -or $targetManifest -is [System.Array]) {
        Add-Failure -Failures $Failures -Message "Row $RowId run-manifest.json must be a single JSON object: $TargetManifestPath."
        return
    }

    $expectedStringFields = [ordered]@{
        EvidenceKind = 'release-evidence'
        EvidenceBoundary = 'live-release-row-required'
        RowId = $RowId
        PackageVersion = $ExpectedPackageVersion
        PackageSha256 = $ExpectedPackageSha256
        ExpectedGameVersion = $ExpectedGameVersion
        ExpectedRitsuLibVersion = $ExpectedRitsuLibVersion
        ExpectedRitsuCompatBranch = $ExpectedRitsuCompatBranch
        ModId = 'EZMicroBalance'
        ModName = 'Spire Plus'
        PackageAnchorMode = 'canonical-repo-publish'
        TrustAnchorMode = 'canonical-current-release-target'
    }

    foreach ($entry in $expectedStringFields.GetEnumerator()) {
        $actual = [string](Get-PropertyValue -Object $targetManifest -Name $entry.Key -Default '')
        if (-not [string]::Equals($actual, [string]$entry.Value, [System.StringComparison]::OrdinalIgnoreCase)) {
            Add-Failure -Failures $Failures -Message "Row $RowId run-manifest.json $($entry.Key) must be '$($entry.Value)' for current release evidence. Current value: '$actual'."
        }
    }

    $packagePathValue = [string](Get-PropertyValue -Object $targetManifest -Name 'PackagePath' -Default '')
    if ([string]::IsNullOrWhiteSpace($packagePathValue)) {
        Add-Failure -Failures $Failures -Message "Row $RowId run-manifest.json PackagePath is missing."
    } else {
        $resolvedPackagePath = Resolve-WorkspacePath -Path $packagePathValue
        if (-not [string]::Equals($resolvedPackagePath, $ExpectedPackageFullPath, [System.StringComparison]::OrdinalIgnoreCase)) {
            Add-Failure -Failures $Failures -Message "Row $RowId run-manifest.json PackagePath must resolve to the canonical current package '$ExpectedPackagePath'. Current value: '$packagePathValue'."
        }
    }

    $patchCountValue = Get-PropertyValue -Object $targetManifest -Name 'ExpectedPatchCount' -Default $null
    if (-not (Test-NativeJsonIntegerValue -Value $patchCountValue) -or [int64]$patchCountValue -ne [int64]$ExpectedPatchCount) {
        Add-Failure -Failures $Failures -Message "Row $RowId run-manifest.json ExpectedPatchCount must be native integer $ExpectedPatchCount for current release evidence. Current value: '$patchCountValue'."
    }
}

function Get-ExpectedLogFileForAuditFile {
    param([Parameter(Mandatory = $true)][string]$AuditFile)

    return [regex]::Replace($AuditFile, '(?i)-log-audit\.json$', '.log')
}

function Read-CleanLogAudit {
    param(
        [Parameter(Mandatory = $true)][string]$AuditPath,
        [Parameter(Mandatory = $true)][string]$ExpectedLogPath
    )

    if (-not (Test-Path -LiteralPath $ExpectedLogPath -PathType Leaf)) {
        return $false
    }

    $raw = Get-Content -LiteralPath $AuditPath -Raw -Encoding UTF8
    $items = @(Get-AuditJsonItems -RawJson $raw)
    if ($items.Count -ne 1) {
        return $false
    }

    $item = $items[0]
    $resolvedExpectedLog = (Resolve-Path -LiteralPath $ExpectedLogPath).Path
    $expectedFile = Get-Item -LiteralPath $resolvedExpectedLog
    $expectedSha256 = (Get-FileHash -Algorithm SHA256 -LiteralPath $resolvedExpectedLog).Hash.ToLowerInvariant()

    if (-not ($item.PSObject.Properties.Name -contains 'AuditSchemaVersion') -or
        -not (Test-NativeJsonIntegerValue -Value $item.AuditSchemaVersion) -or
        [long]$item.AuditSchemaVersion -ne 2) {
        return $false
    }

    if (-not ($item.PSObject.Properties.Name -contains 'SignatureSetSha256') -or
        [string]::IsNullOrWhiteSpace([string]$item.SignatureSetSha256) -or
        -not ([string]$item.SignatureSetSha256 -match '^[A-Fa-f0-9]{64}$')) {
        return $false
    }

    if (-not ($item.PSObject.Properties.Name -contains 'Clean') -or -not ($item.Clean -is [bool]) -or -not [bool]$item.Clean) {
        return $false
    }

    if (-not ($item.PSObject.Properties.Name -contains 'Path') -or [string]::IsNullOrWhiteSpace([string]$item.Path)) {
        return $false
    }

    $retainedLogPath = [System.IO.Path]::GetFullPath([string]$item.Path)
    if (-not [string]::Equals($retainedLogPath, [System.IO.Path]::GetFullPath($resolvedExpectedLog), [System.StringComparison]::OrdinalIgnoreCase)) {
        return $false
    }

    if (-not ($item.PSObject.Properties.Name -contains 'Length') -or -not (Test-NativeJsonIntegerValue -Value $item.Length) -or [long]$item.Length -ne [long]$expectedFile.Length) {
        return $false
    }

    if (-not ($item.PSObject.Properties.Name -contains 'Sha256') -or
        [string]::IsNullOrWhiteSpace([string]$item.Sha256) -or
        -not ([string]$item.Sha256 -match '^[A-Fa-f0-9]{64}$') -or
        -not [string]::Equals(([string]$item.Sha256).ToLowerInvariant(), $expectedSha256, [System.StringComparison]::Ordinal)) {
        return $false
    }

    $retainedSignatureVector = @(Get-AuditSignatureVector -AuditItem $item)
    if ($retainedSignatureVector.Count -eq 0) {
        return $false
    }

    foreach ($entry in $retainedSignatureVector) {
        if (-not $entry.EndsWith('=0', [System.StringComparison]::Ordinal)) {
            return $false
        }
    }

    $auditScript = Join-Path $PSScriptRoot 'audit-godot-log.ps1'
    try {
        $recomputedRaw = & $auditScript -Path $resolvedExpectedLog
    } catch {
        return $false
    }

    $recomputedItems = @(Get-AuditJsonItems -RawJson ($recomputedRaw -join [Environment]::NewLine))
    if ($recomputedItems.Count -ne 1) {
        return $false
    }

    $recomputed = $recomputedItems[0]
    if (-not ($recomputed.PSObject.Properties.Name -contains 'AuditSchemaVersion') -or
        -not (Test-NativeJsonIntegerValue -Value $recomputed.AuditSchemaVersion) -or
        [long]$recomputed.AuditSchemaVersion -ne [long]$item.AuditSchemaVersion) {
        return $false
    }

    if (-not ($recomputed.PSObject.Properties.Name -contains 'Clean') -or -not ($recomputed.Clean -is [bool]) -or -not [bool]$recomputed.Clean) {
        return $false
    }

    $recomputedSignatureVector = @(Get-AuditSignatureVector -AuditItem $recomputed)
    if ($recomputedSignatureVector.Count -eq 0) {
        return $false
    }

    if (-not [string]::Equals(([string]$item.SignatureSetSha256).ToLowerInvariant(), ([string]$recomputed.SignatureSetSha256).ToLowerInvariant(), [System.StringComparison]::Ordinal)) {
        return $false
    }

    if (-not (Test-StringArrayEquals -Actual $retainedSignatureVector -Expected $recomputedSignatureVector)) {
        return $false
    }

    return $true
}

function Test-PackageHashesEvidence {
    param(
        [Parameter(Mandatory = $true)][string]$PackageHashesPath,
        [Parameter(Mandatory = $true)][string]$RowId,
        [System.Collections.Generic.List[string]]$Failures,
        [Parameter(Mandatory = $true)][string]$ManifestPackagePath
    )

    try {
        $packageHashes = Get-Content -LiteralPath $PackageHashesPath -Raw -Encoding UTF8 | ConvertFrom-Json
    } catch {
        Add-Failure -Failures $Failures -Message "Row $RowId package-hashes.json is not valid JSON: $PackageHashesPath."
        return
    }

    $files = @(Get-PropertyValue -Object $packageHashes -Name 'Files' -Default @())
    if ($files.Count -eq 0) {
        Add-Failure -Failures $Failures -Message "Row $RowId package-hashes.json has no Files rows."
        return
    }

    $rowsByPath = [System.Collections.Generic.Dictionary[string, object]]::new([System.StringComparer]::Ordinal)
    $rowPathCaseMap = [System.Collections.Generic.Dictionary[string, string]]::new([System.StringComparer]::OrdinalIgnoreCase)
    foreach ($file in $files) {
        $rowPath = [string](Get-PropertyValue -Object $file -Name 'Path' -Default '')
        if ([string]::IsNullOrWhiteSpace($rowPath)) {
            Add-Failure -Failures $Failures -Message "Row $RowId package-hashes.json contains a file row without Path."
            continue
        }

        if ($rowsByPath.ContainsKey($rowPath)) {
            Add-Failure -Failures $Failures -Message "Row $RowId package-hashes.json contains duplicate file row path: $rowPath."
            continue
        }

        if ($rowPathCaseMap.ContainsKey($rowPath)) {
            $previousPath = $rowPathCaseMap[$rowPath]
            if (-not [string]::Equals($previousPath, $rowPath, [System.StringComparison]::Ordinal)) {
                Add-Failure -Failures $Failures -Message "Row $RowId package-hashes.json contains case-drifted duplicate file row path: $rowPath conflicts with $previousPath."
                continue
            }
        } else {
            $rowPathCaseMap.Add($rowPath, $rowPath)
        }

        $rowsByPath.Add($rowPath, $file)
    }

    foreach ($stalePath in @('publish\EZMicroBalance.dll', 'publish\EZMicroBalance.pck', 'publish\EZMicroBalance.json')) {
        if ($rowsByPath.ContainsKey($stalePath)) {
            Add-Failure -Failures $Failures -Message "Row $RowId package-hashes.json still records stale root publish artifact path: $stalePath."
        } elseif ($rowPathCaseMap.ContainsKey($stalePath)) {
            Add-Failure -Failures $Failures -Message "Row $RowId package-hashes.json still records case-drifted stale root publish artifact path: $($rowPathCaseMap[$stalePath])."
        }
    }

    $expectedPaths = Get-SpirePlusPackageArtifactRelativePaths -RepoRoot $repoRoot -PackagePath $ManifestPackagePath

    foreach ($expectedPath in $expectedPaths) {
        if (-not $rowsByPath.ContainsKey($expectedPath)) {
            if ($rowPathCaseMap.ContainsKey($expectedPath)) {
                Add-Failure -Failures $Failures -Message "Row $RowId package-hashes.json uses case-drifted current package artifact row: expected $expectedPath but found $($rowPathCaseMap[$expectedPath])."
            } else {
                Add-Failure -Failures $Failures -Message "Row $RowId package-hashes.json is missing current package artifact row: $expectedPath."
            }
            continue
        }

        $hashRow = $rowsByPath[$expectedPath]
        $exists = [bool](Get-PropertyValue -Object $hashRow -Name 'Exists' -Default $false)
        if (-not $exists) {
            Add-Failure -Failures $Failures -Message "Row $RowId package-hashes.json marks current package artifact missing: $expectedPath."
            continue
        }

        $artifactFull = Resolve-WorkspacePath -Path $expectedPath
        if (-not (Test-Path -LiteralPath $artifactFull -PathType Leaf)) {
            Add-Failure -Failures $Failures -Message "Row $RowId package-hashes.json artifact path does not exist in workspace: $expectedPath."
            continue
        }

        $expectedHash = Get-SpirePlusFileSha256 -Path $artifactFull
        $actualHash = ([string](Get-PropertyValue -Object $hashRow -Name 'Sha256' -Default '')).ToUpperInvariant()
        if ($actualHash -ne $expectedHash) {
            Add-Failure -Failures $Failures -Message "Row $RowId package-hashes.json hash for $expectedPath is '$actualHash' but current file hash is '$expectedHash'."
        }
    }
}

function Test-PreflightForeground {
    param([Parameter(Mandatory = $true)][string]$PreflightPath)

    $preflight = Get-Content -LiteralPath $PreflightPath -Raw -Encoding UTF8 | ConvertFrom-Json
    return [bool](Get-PropertyValue -Object $preflight -Name 'SpireForeground' -Default $false)
}

function Test-PngSignature {
    param([Parameter(Mandatory = $true)][string]$Path)

    return $null -ne (Get-PngDimensions -Path $Path)
}

function Get-PngDimensions {
    param([Parameter(Mandatory = $true)][string]$Path)

    $stream = [System.IO.File]::OpenRead($Path)
    try {
        if ($stream.Length -lt 24) {
            return $null
        }

        $bytes = [byte[]]::new(24)
        [void]$stream.Read($bytes, 0, 24)
        $signature = [byte[]]@(137, 80, 78, 71, 13, 10, 26, 10)

        for ($index = 0; $index -lt $signature.Length; $index++) {
            if ($bytes[$index] -ne $signature[$index]) {
                return $null
            }
        }

        if ([char]$bytes[12] -ne 'I' -or [char]$bytes[13] -ne 'H' -or [char]$bytes[14] -ne 'D' -or [char]$bytes[15] -ne 'R') {
            return $null
        }

        $width =
            ([int]$bytes[16] -shl 24) -bor
            ([int]$bytes[17] -shl 16) -bor
            ([int]$bytes[18] -shl 8) -bor
            [int]$bytes[19]
        $height =
            ([int]$bytes[20] -shl 24) -bor
            ([int]$bytes[21] -shl 16) -bor
            ([int]$bytes[22] -shl 8) -bor
            [int]$bytes[23]

        return [pscustomobject]@{
            Width = $width
            Height = $height
        }
    } finally {
        $stream.Dispose()
    }
}

function Test-PngMinimumDimensions {
    param([Parameter(Mandatory = $true)][string]$Path)

    $dimensions = Get-PngDimensions -Path $Path
    if ($null -eq $dimensions) {
        return $false
    }

    return $dimensions.Width -ge $MinScreenshotWidth -and $dimensions.Height -ge $MinScreenshotHeight
}

function Test-ChecklistCellFilled {
    param([string]$Value)

    if ([string]::IsNullOrWhiteSpace($Value)) {
        return $false
    }

    return $Value -notmatch $invalidChecklistCellPattern
}

function Test-ChecklistContainsTemplateInstruction {
    param(
        [Parameter(Mandatory = $true)][string]$Checklist,
        [Parameter(Mandatory = $true)][string]$ChecklistName
    )

    return $Checklist -match [regex]::Escape("Copy this file to ``$ChecklistName``") -or
        $Checklist -match [regex]::Escape("Template reference for ``$ChecklistName``")
}

function Test-BossAbilityChecklist {
    param(
        [Parameter(Mandatory = $true)][string]$ChecklistPath,
        [System.Collections.Generic.List[string]]$Failures,
        [Parameter(Mandatory = $true)][string]$RowId
    )

    $checklist = Get-Content -LiteralPath $ChecklistPath -Raw -Encoding UTF8
    if (Test-ChecklistContainsTemplateInstruction -Checklist $checklist -ChecklistName 'boss-ability-checklist.md') {
        Add-Failure -Failures $Failures -Message "Row $RowId boss-ability-checklist.md still contains the unfilled template instruction."
    }

    foreach ($boss in $requiredBossAbilityRows) {
        $bossPattern = '^\|\s*' + [regex]::Escape($boss) + '\s*\|'
        $line = @($checklist -split "`r?`n" | Where-Object { $_ -match $bossPattern } | Select-Object -First 1)
        if ($line.Count -eq 0) {
            Add-Failure -Failures $Failures -Message "Row $RowId boss-ability-checklist.md is missing Boss row: $boss."
            continue
        }

        $cells = @(([string]$line[0]).Split('|') | ForEach-Object { $_.Trim() })
        if ($cells.Count -lt 6) {
            Add-Failure -Failures $Failures -Message "Row $RowId boss-ability-checklist.md row for $boss does not have the expected table cells."
            continue
        }

        $liveResult = $cells[4]
        $evidenceFiles = $cells[5]
        if (-not (Test-ChecklistCellFilled -Value $liveResult)) {
            Add-Failure -Failures $Failures -Message "Row $RowId boss-ability-checklist.md row for $boss has no filled Live result cell."
        }

        if (-not (Test-ChecklistCellFilled -Value $evidenceFiles)) {
            Add-Failure -Failures $Failures -Message "Row $RowId boss-ability-checklist.md row for $boss has no filled Evidence file(s) cell."
        }
    }
}

function Test-AncientRewardRelicsChecklist {
    param(
        [Parameter(Mandatory = $true)][string]$ChecklistPath,
        [System.Collections.Generic.List[string]]$Failures,
        [Parameter(Mandatory = $true)][string]$RowId
    )

    $checklist = Get-Content -LiteralPath $ChecklistPath -Raw -Encoding UTF8
    if (Test-ChecklistContainsTemplateInstruction -Checklist $checklist -ChecklistName 'ancient-reward-relics-checklist.md') {
        Add-Failure -Failures $Failures -Message "Row $RowId ancient-reward-relics-checklist.md still contains the unfilled template instruction."
    }

    foreach ($requiredReward in $requiredAncientRewardRows) {
        $ancient = [string]$requiredReward.Ancient
        $reward = [string]$requiredReward.Reward
        $rowPattern = '^\|\s*' + [regex]::Escape($ancient) + '\s*\|\s*' + [regex]::Escape($reward) + '\s*\|'
        $line = @($checklist -split "`r?`n" | Where-Object { $_ -match $rowPattern } | Select-Object -First 1)
        if ($line.Count -eq 0) {
            Add-Failure -Failures $Failures -Message "Row $RowId ancient-reward-relics-checklist.md is missing Ancient reward row: $ancient / $reward."
            continue
        }

        $cells = @(([string]$line[0]).Split('|') | ForEach-Object { $_.Trim() })
        if ($cells.Count -lt 8) {
            Add-Failure -Failures $Failures -Message "Row $RowId ancient-reward-relics-checklist.md row for $ancient / $reward does not have the expected table cells."
            continue
        }

        $screenResult = $cells[4]
        $relicResult = $cells[5]
        $evidenceFiles = $cells[6]
        if (-not (Test-ChecklistCellFilled -Value $screenResult)) {
            Add-Failure -Failures $Failures -Message "Row $RowId ancient-reward-relics-checklist.md row for $ancient / $reward has no filled Screen option result cell."
        }

        if (-not (Test-ChecklistCellFilled -Value $relicResult)) {
            Add-Failure -Failures $Failures -Message "Row $RowId ancient-reward-relics-checklist.md row for $ancient / $reward has no filled Relic bar / hover result cell."
        }

        if (-not (Test-ChecklistCellFilled -Value $evidenceFiles)) {
            Add-Failure -Failures $Failures -Message "Row $RowId ancient-reward-relics-checklist.md row for $ancient / $reward has no filled Evidence file(s) cell."
        }
    }
}

function Test-RootblightBehaviorChecklist {
    param(
        [Parameter(Mandatory = $true)][string]$ChecklistPath,
        [System.Collections.Generic.List[string]]$Failures,
        [Parameter(Mandatory = $true)][string]$RowId
    )

    $checklist = Get-Content -LiteralPath $ChecklistPath -Raw -Encoding UTF8
    if (Test-ChecklistContainsTemplateInstruction -Checklist $checklist -ChecklistName 'rootblight-behavior-checklist.md') {
        Add-Failure -Failures $Failures -Message "Row $RowId rootblight-behavior-checklist.md still contains the unfilled template instruction."
    }

    foreach ($scenario in $requiredRootblightRows) {
        $rowPattern = '^\|\s*' + [regex]::Escape($scenario) + '\s*\|'
        $line = @($checklist -split "`r?`n" | Where-Object { $_ -match $rowPattern } | Select-Object -First 1)
        if ($line.Count -eq 0) {
            Add-Failure -Failures $Failures -Message "Row $RowId rootblight-behavior-checklist.md is missing Rootblight scenario row: $scenario."
            continue
        }

        $cells = @(([string]$line[0]).Split('|') | ForEach-Object { $_.Trim() })
        if ($cells.Count -lt 6) {
            Add-Failure -Failures $Failures -Message "Row $RowId rootblight-behavior-checklist.md row for $scenario does not have the expected table cells."
            continue
        }

        $liveResult = $cells[3]
        $evidenceFiles = $cells[4]
        if (-not (Test-ChecklistCellFilled -Value $liveResult)) {
            Add-Failure -Failures $Failures -Message "Row $RowId rootblight-behavior-checklist.md row for $scenario has no filled Live result cell."
        }

        if (-not (Test-ChecklistCellFilled -Value $evidenceFiles)) {
            Add-Failure -Failures $Failures -Message "Row $RowId rootblight-behavior-checklist.md row for $scenario has no filled Evidence file(s) cell."
        }
    }
}

function Test-ArtResourceRoutingChecklist {
    param(
        [Parameter(Mandatory = $true)][string]$ChecklistPath,
        [System.Collections.Generic.List[string]]$Failures,
        [Parameter(Mandatory = $true)][string]$RowId
    )

    $checklist = Get-Content -LiteralPath $ChecklistPath -Raw -Encoding UTF8
    if (Test-ChecklistContainsTemplateInstruction -Checklist $checklist -ChecklistName 'art-resource-routing-checklist.md') {
        Add-Failure -Failures $Failures -Message "Row $RowId art-resource-routing-checklist.md still contains the unfilled template instruction."
    }

    foreach ($surface in $requiredArtRoutingRows) {
        $rowPattern = '^\|\s*' + [regex]::Escape($surface) + '\s*\|'
        $line = @($checklist -split "`r?`n" | Where-Object { $_ -match $rowPattern } | Select-Object -First 1)
        if ($line.Count -eq 0) {
            Add-Failure -Failures $Failures -Message "Row $RowId art-resource-routing-checklist.md is missing art surface row: $surface."
            continue
        }

        $cells = @(([string]$line[0]).Split('|') | ForEach-Object { $_.Trim() })
        if ($cells.Count -lt 6) {
            Add-Failure -Failures $Failures -Message "Row $RowId art-resource-routing-checklist.md row for $surface does not have the expected table cells."
            continue
        }

        $liveResult = $cells[3]
        $evidenceFiles = $cells[4]
        if (-not (Test-ChecklistCellFilled -Value $liveResult)) {
            Add-Failure -Failures $Failures -Message "Row $RowId art-resource-routing-checklist.md row for $surface has no filled Live result cell."
        }

        if (-not (Test-ChecklistCellFilled -Value $evidenceFiles)) {
            Add-Failure -Failures $Failures -Message "Row $RowId art-resource-routing-checklist.md row for $surface has no filled Evidence file(s) cell."
        }
    }
}

function Test-PlayerTextQaChecklist {
    param(
        [Parameter(Mandatory = $true)][string]$ChecklistPath,
        [System.Collections.Generic.List[string]]$Failures,
        [Parameter(Mandatory = $true)][string]$RowId
    )

    $checklist = Get-Content -LiteralPath $ChecklistPath -Raw -Encoding UTF8
    if (Test-ChecklistContainsTemplateInstruction -Checklist $checklist -ChecklistName 'player-text-qa-checklist.md') {
        Add-Failure -Failures $Failures -Message "Row $RowId player-text-qa-checklist.md still contains the unfilled template instruction."
    }

    foreach ($surface in $requiredPlayerTextRows) {
        $rowPattern = '^\|\s*' + [regex]::Escape($surface) + '\s*\|'
        $line = @($checklist -split "`r?`n" | Where-Object { $_ -match $rowPattern } | Select-Object -First 1)
        if ($line.Count -eq 0) {
            Add-Failure -Failures $Failures -Message "Row $RowId player-text-qa-checklist.md is missing player-text row: $surface."
            continue
        }

        $cells = @(([string]$line[0]).Split('|') | ForEach-Object { $_.Trim() })
        if ($cells.Count -lt 7) {
            Add-Failure -Failures $Failures -Message "Row $RowId player-text-qa-checklist.md row for $surface does not have the expected table cells."
            continue
        }

        $englishResult = $cells[3]
        $chineseResult = $cells[4]
        $evidenceFiles = $cells[5]
        if (-not (Test-ChecklistCellFilled -Value $englishResult)) {
            Add-Failure -Failures $Failures -Message "Row $RowId player-text-qa-checklist.md row for $surface has no filled EN result cell."
        }

        if (-not (Test-ChecklistCellFilled -Value $chineseResult)) {
            Add-Failure -Failures $Failures -Message "Row $RowId player-text-qa-checklist.md row for $surface has no filled ZHS result cell."
        }

        if (-not (Test-ChecklistCellFilled -Value $evidenceFiles)) {
            Add-Failure -Failures $Failures -Message "Row $RowId player-text-qa-checklist.md row for $surface has no filled Evidence file(s) cell."
        }
    }
}

function Test-SimpleChecklistRows {
    param(
        [Parameter(Mandatory = $true)][string]$ChecklistPath,
        [Parameter(Mandatory = $true)][string[]]$RequiredRows,
        [Parameter(Mandatory = $true)][string]$TemplateInstruction,
        [Parameter(Mandatory = $true)][string]$ChecklistName,
        [System.Collections.Generic.List[string]]$Failures,
        [Parameter(Mandatory = $true)][string]$RowId
    )

    $checklist = Get-Content -LiteralPath $ChecklistPath -Raw -Encoding UTF8
    if ($checklist -match [regex]::Escape($TemplateInstruction) -or
        (Test-ChecklistContainsTemplateInstruction -Checklist $checklist -ChecklistName $ChecklistName)) {
        Add-Failure -Failures $Failures -Message "Row $RowId $ChecklistName still contains the unfilled template instruction."
    }

    foreach ($requiredRow in $RequiredRows) {
        $rowPattern = '^\|\s*' + [regex]::Escape($requiredRow) + '\s*\|'
        $line = @($checklist -split "`r?`n" | Where-Object { $_ -match $rowPattern } | Select-Object -First 1)
        if ($line.Count -eq 0) {
            Add-Failure -Failures $Failures -Message "Row $RowId $ChecklistName is missing scenario row: $requiredRow."
            continue
        }

        $cells = @(([string]$line[0]).Split('|') | ForEach-Object { $_.Trim() })
        if ($cells.Count -lt 6) {
            Add-Failure -Failures $Failures -Message "Row $RowId $ChecklistName row for $requiredRow does not have the expected table cells."
            continue
        }

        $liveResult = $cells[3]
        $evidenceFiles = $cells[4]
        if (-not (Test-ChecklistCellFilled -Value $liveResult)) {
            Add-Failure -Failures $Failures -Message "Row $RowId $ChecklistName row for $requiredRow has no filled Live result cell."
        }

        if (-not (Test-ChecklistCellFilled -Value $evidenceFiles)) {
            Add-Failure -Failures $Failures -Message "Row $RowId $ChecklistName row for $requiredRow has no filled Evidence file(s) cell."
        }
    }
}

function New-TemplateManifest {
    param([Parameter(Mandatory = $true)][string]$OutputPath)

    $rows = foreach ($required in $requiredReleaseRows) {
        [ordered]@{
            Id = $required.Id
            Label = $required.Label
            Kind = $required.Kind
            Status = 'pending'
            EvidenceDir = ''
            EvidenceBoundary = 'live-release-row-required'
            RequiredFiles = @(Get-RequiredEvidenceFilesForRow -RequiredRow $required)
            ScreenshotFile = if ($required.Kind -eq 'clicked-ui') { '' } else { $null }
            LogOriginProofStatus = 'pending-owner-live-release-log'
            ResultNote = ''
            ReleaseNote = ''
            ExplicitOwnerDecision = $false
        }
    }

    $template = [ordered]@{
        PackageSha256 = $PackageSha256
        PackagePath = $PackagePath
        CreatedAt = (Get-Date).ToString('o')
        Rows = @($rows)
    }

    $parent = Split-Path -Parent $OutputPath
    if ($parent) {
        New-Item -ItemType Directory -Path $parent -Force | Out-Null
    }

    $template | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $OutputPath -Encoding UTF8
}

function Write-PassMarker {
    param(
        [Parameter(Mandatory = $true)][string]$OutputPath,
        [Parameter(Mandatory = $true)]$Summary
    )

    $parent = Split-Path -Parent $OutputPath
    if ($parent) {
        New-Item -ItemType Directory -Path $parent -Force | Out-Null
    }

    [ordered]@{
        Status = 'pass'
        Verifier = 'verify-spire-plus-release-evidence.ps1'
        ManifestPath = $Summary.ManifestPath
        EvidenceRoot = $Summary.EvidenceRoot
        PackageSha256 = $Summary.PackageSha256
        PackagePath = $Summary.PackagePath
        ActualPackageSha256 = $Summary.ActualPackageSha256
        CheckedAt = $Summary.CheckedAt
        AllowDeferred = $Summary.AllowDeferred
        RequiredRowCount = $Summary.RequiredRowCount
        WarningCount = $Summary.WarningCount
        Warnings = @($Summary.Warnings)
    } | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $OutputPath -Encoding UTF8
}

function Add-Failure {
    param(
        [System.Collections.Generic.List[string]]$Failures,
        [Parameter(Mandatory = $true)][string]$Message
    )

    [void]$Failures.Add($Message)
}

function Add-Warning {
    param(
        [System.Collections.Generic.List[string]]$Warnings,
        [Parameter(Mandatory = $true)][string]$Message
    )

    [void]$Warnings.Add($Message)
}

$evidenceRootFull = Resolve-WorkspacePath -Path $EvidenceRoot
if ([string]::IsNullOrWhiteSpace($ManifestPath)) {
    $ManifestPath = Join-Path $evidenceRootFull 'release-evidence-manifest.json'
}

$manifestFull = Resolve-WorkspacePath -Path $ManifestPath
if (-not (Test-PathWithin -BasePath $evidenceRootFull -ChildPath $manifestFull)) {
    Write-Error "ManifestPath is outside EvidenceRoot: $manifestFull."
    exit 1
}

if ([string]::IsNullOrWhiteSpace($PassMarkerPath)) {
    $PassMarkerPath = Join-Path $evidenceRootFull 'release-evidence-verifier-pass.json'
}

$passMarkerFull = Resolve-WorkspacePath -Path $PassMarkerPath
if (-not (Test-PathWithin -BasePath $evidenceRootFull -ChildPath $passMarkerFull)) {
    Write-Error "PassMarkerPath is outside EvidenceRoot: $passMarkerFull."
    exit 1
}

if ($WriteTemplate) {
    New-TemplateManifest -OutputPath $manifestFull
    Write-Output "Wrote release evidence template: $manifestFull"
    exit 0
}

if (-not (Test-Path -LiteralPath $manifestFull)) {
    Write-Error "Missing release evidence manifest: $manifestFull. Run this script with -WriteTemplate first, then fill the evidence rows."
    exit 1
}

$manifest = Get-Content -LiteralPath $manifestFull -Raw -Encoding UTF8 | ConvertFrom-Json
$failures = [System.Collections.Generic.List[string]]::new()
$warnings = [System.Collections.Generic.List[string]]::new()
$protectedEvidencePaths = [System.Collections.Generic.List[string]]::new()
[void]$protectedEvidencePaths.Add($manifestFull)

Add-NoReparsePointInPathFailures -Path $evidenceRootFull -Label 'EvidenceRoot' -Failures $failures
Add-NoReparsePointInPathFailures -Path $manifestFull -Label 'ManifestPath' -Failures $failures
Add-OrdinaryEvidenceFileFailures -Path $manifestFull -Label 'Release evidence manifest' -Failures $failures
Add-NoReparsePointInPathFailures -Path $passMarkerFull -Label 'PassMarkerPath' -Failures $failures
Add-OutputAliasFailures -OutputPath $passMarkerFull -ProtectedPaths @($manifestFull) -Label 'Pass marker' -Failures $failures

$packageFullFromArgument = Resolve-SpirePlusPackagePath -RepoRoot $repoRoot -PackagePath $PackagePath
if (-not [string]::Equals($packageFullFromArgument, $canonicalPackageFullPath, [System.StringComparison]::OrdinalIgnoreCase)) {
    Add-Failure -Failures $failures -Message "PackagePath must be the canonical current Spire Plus package '$canonicalPackagePath' for release evidence verification. Current value: '$PackagePath'."
}

if ([string]::IsNullOrWhiteSpace($canonicalPackageSha256)) {
    Add-Failure -Failures $failures -Message "Canonical current Spire Plus package does not exist or has no hash: $canonicalPackageFullPath."
} elseif (-not [string]::Equals($PackageSha256, $canonicalPackageSha256, [System.StringComparison]::OrdinalIgnoreCase)) {
    Add-Failure -Failures $failures -Message "PackageSha256 must match the canonical current Spire Plus package '$canonicalPackagePath'. Current value: '$PackageSha256'. Expected: '$canonicalPackageSha256'."
}

$manifestPackageSha256 = Get-PropertyValue -Object $manifest -Name 'PackageSha256' -Default ''
if ($manifestPackageSha256 -ne $PackageSha256) {
    Add-Failure -Failures $failures -Message "Manifest PackageSha256 '$manifestPackageSha256' does not match current package '$PackageSha256'."
}

$manifestPackagePath = [string](Get-PropertyValue -Object $manifest -Name 'PackagePath' -Default $PackagePath)
if ([string]::IsNullOrWhiteSpace($manifestPackagePath)) {
    $manifestPackagePath = $PackagePath
}

$packageFull = Resolve-WorkspacePath -Path $manifestPackagePath
if (-not [string]::Equals($packageFull, $canonicalPackageFullPath, [System.StringComparison]::OrdinalIgnoreCase)) {
    Add-Failure -Failures $failures -Message "Manifest PackagePath must resolve to canonical current Spire Plus package '$canonicalPackagePath'. Current value: '$manifestPackagePath'."
}

$actualPackageSha256 = ''
if (-not (Test-Path -LiteralPath $packageFull -PathType Leaf)) {
    Add-Failure -Failures $failures -Message "Package under test does not exist: $packageFull."
} else {
    $actualPackageSha256 = Get-SpirePlusFileSha256 -Path $packageFull
    if ($actualPackageSha256 -ne $PackageSha256) {
        Add-Failure -Failures $failures -Message "Actual package SHA256 '$actualPackageSha256' for '$packageFull' does not match current package '$PackageSha256'."
    }
}

$rows = @(Get-PropertyValue -Object $manifest -Name 'Rows' -Default @())
$rowMap = @{}
$requiredRowIds = @{}
foreach ($required in $requiredReleaseRows) {
    $requiredRowIds[$required.Id] = $true
}

foreach ($row in $rows) {
    $id = [string](Get-PropertyValue -Object $row -Name 'Id' -Default '')
    if ([string]::IsNullOrWhiteSpace($id)) {
        Add-Warning -Warnings $warnings -Message 'Release evidence manifest contains a row with no Id; it is ignored.'
        continue
    }

    if ($rowMap.ContainsKey($id)) {
        Add-Failure -Failures $failures -Message "Duplicate release evidence row id: $id."
        continue
    }

    if (-not $requiredRowIds.ContainsKey($id)) {
        Add-Warning -Warnings $warnings -Message "Unknown release evidence row id ignored: $id."
    }

    $rowMap[$id] = $row
}

foreach ($required in $requiredReleaseRows) {
    if (-not $rowMap.ContainsKey($required.Id)) {
        Add-Failure -Failures $failures -Message "Missing required release evidence row: $($required.Id) ($($required.Label))."
        continue
    }

    $row = $rowMap[$required.Id]
    $status = ([string](Get-PropertyValue -Object $row -Name 'Status' -Default '')).ToLowerInvariant()
    $rowKind = [string](Get-PropertyValue -Object $row -Name 'Kind' -Default $required.Kind)
    if (-not [string]::Equals($rowKind, $required.Kind, [System.StringComparison]::OrdinalIgnoreCase)) {
        Add-Failure -Failures $failures -Message "Row $($required.Id) Kind '$rowKind' does not match required kind '$($required.Kind)'."
    }

    if ($status -eq 'deferred') {
        $explicitOwnerDecision = [bool](Get-PropertyValue -Object $row -Name 'ExplicitOwnerDecision' -Default $false)
        $releaseNote = [string](Get-PropertyValue -Object $row -Name 'ReleaseNote' -Default '')
        if (-not $AllowDeferred) {
            Add-Failure -Failures $failures -Message "Row $($required.Id) is deferred. Re-run with -AllowDeferred only after the owner explicitly accepts a release-note deferral."
            continue
        }

        if (-not $explicitOwnerDecision -or [string]::IsNullOrWhiteSpace($releaseNote)) {
            Add-Failure -Failures $failures -Message "Row $($required.Id) is deferred without ExplicitOwnerDecision=true and a ReleaseNote."
        }
        continue
    }

    if ($status -ne 'pass') {
        Add-Failure -Failures $failures -Message "Row $($required.Id) is not pass or accepted deferred. Current status: '$status'."
        continue
    }

    $rowEvidenceBoundary = [string](Get-PropertyValue -Object $row -Name 'EvidenceBoundary' -Default '')
    if (-not [string]::Equals($rowEvidenceBoundary, 'live-release-row-required', [System.StringComparison]::OrdinalIgnoreCase)) {
        Add-Failure -Failures $failures -Message "Row $($required.Id) EvidenceBoundary must be live-release-row-required for release pass rows. Current value: '$rowEvidenceBoundary'."
    }

    $rowLogOriginProofStatus = [string](Get-PropertyValue -Object $row -Name 'LogOriginProofStatus' -Default '')
    if (-not [string]::Equals($rowLogOriginProofStatus, $requiredReleaseLogOriginProofStatus, [System.StringComparison]::OrdinalIgnoreCase)) {
        Add-Failure -Failures $failures -Message "Row $($required.Id) LogOriginProofStatus must be $requiredReleaseLogOriginProofStatus for release pass rows. Current value: '$rowLogOriginProofStatus'."
    }

    $evidenceDirRaw = [string](Get-PropertyValue -Object $row -Name 'EvidenceDir' -Default '')
    if ([string]::IsNullOrWhiteSpace($evidenceDirRaw)) {
        Add-Failure -Failures $failures -Message "Row $($required.Id) has pass status but no EvidenceDir."
        continue
    }

    $evidenceDir = Resolve-WorkspacePath -Path $evidenceDirRaw
    if (-not (Test-PathWithin -BasePath $evidenceRootFull -ChildPath $evidenceDir)) {
        Add-Failure -Failures $failures -Message "Row $($required.Id) EvidenceDir is outside EvidenceRoot: $evidenceDir."
        continue
    }

    if (-not (Test-Path -LiteralPath $evidenceDir -PathType Container)) {
        Add-Failure -Failures $failures -Message "Row $($required.Id) EvidenceDir does not exist: $evidenceDir."
        continue
    }
    Add-NoReparsePointInPathFailures -Path $evidenceDir -Label "Row $($required.Id) EvidenceDir" -Failures $failures

    $markerOnlyRuntimeBaselineReasons = @(Get-MarkerOnlyRuntimeBaselineEvidenceReasons -Row $row -EvidenceDir $evidenceDir)
    if ($markerOnlyRuntimeBaselineReasons.Count -gt 0) {
        Add-Failure -Failures $failures -Message "Row $($required.Id) uses marker-only runtime baseline evidence, which cannot satisfy release required rows: $($markerOnlyRuntimeBaselineReasons -join '; ')."
        continue
    }

    $defaultRequiredFiles = @(Get-RequiredEvidenceFilesForRow -RequiredRow $required)
    $rowRequiredFiles = @(Get-PropertyValue -Object $row -Name 'RequiredFiles' -Default @())
    $requiredFiles = @(Merge-RequiredEvidenceFiles -DefaultFiles $defaultRequiredFiles -RowFiles $rowRequiredFiles)
    foreach ($requiredFile in $requiredFiles) {
        $requiredFileString = [string]$requiredFile
        if ([string]::IsNullOrWhiteSpace($requiredFileString)) {
            continue
        }

        $filePath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path $requiredFileString
        if (-not (Test-PathWithin -BasePath $evidenceDir -ChildPath $filePath)) {
            Add-Failure -Failures $failures -Message "Row $($required.Id) required evidence file path escapes EvidenceDir: $requiredFileString."
            continue
        }

        [void]$protectedEvidencePaths.Add($filePath)
        if (-not (Test-Path -LiteralPath $filePath -PathType Leaf)) {
            Add-Failure -Failures $failures -Message "Row $($required.Id) missing required evidence file: $filePath."
            continue
        }
        Add-OrdinaryEvidenceFileFailures -Path $filePath -Label "Row $($required.Id) required evidence file '$requiredFileString'" -Failures $failures

        if ((Get-Item -LiteralPath $filePath).Length -eq 0) {
            Add-Failure -Failures $failures -Message "Row $($required.Id) required evidence file is empty: $filePath."
        }

        if ($requiredFileString -eq 'package-hashes.json') {
            Test-PackageHashesEvidence `
                -PackageHashesPath $filePath `
                -RowId $required.Id `
                -Failures $failures `
                -ManifestPackagePath $manifestPackagePath
        }

        if ($requiredFileString.EndsWith('.md', [System.StringComparison]::OrdinalIgnoreCase)) {
            $note = Get-Content -LiteralPath $filePath -Raw -Encoding UTF8
            if ([string]::IsNullOrWhiteSpace($note)) {
                Add-Failure -Failures $failures -Message "Row $($required.Id) evidence note '$requiredFileString' is empty."
            }

            if ($note -match $invalidEvidenceNotePattern) {
                Add-Failure -Failures $failures -Message "Row $($required.Id) evidence note '$requiredFileString' describes invalid or non-counting evidence."
            }
        }

        if ([string]::Equals($requiredFileString, 'log-origin-note.md', [System.StringComparison]::OrdinalIgnoreCase)) {
            Test-ReleaseLogOriginNote -NotePath $filePath -RowId $required.Id -Failures $failures
        }
    }

    $rowTargetManifestPath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path 'run-manifest.json'
    if ((Test-PathWithin -BasePath $evidenceDir -ChildPath $rowTargetManifestPath) -and
        (Test-Path -LiteralPath $rowTargetManifestPath -PathType Leaf)) {
        Test-ReleaseRowTargetManifest `
            -TargetManifestPath $rowTargetManifestPath `
            -RowId $required.Id `
            -Failures $failures `
            -ExpectedPackageVersion $currentPackageVersion `
            -ExpectedPackagePath $canonicalPackagePath `
            -ExpectedPackageFullPath $canonicalPackageFullPath `
            -ExpectedPackageSha256 $PackageSha256 `
            -ExpectedGameVersion $ExpectedGameVersion `
            -ExpectedRitsuLibVersion $ExpectedRitsuLibVersion `
            -ExpectedRitsuCompatBranch $ExpectedRitsuCompatBranch `
            -ExpectedPatchCount $ExpectedPatchCount
    }

    $logAuditFiles = @($requiredFiles | Where-Object { ([string]$_).EndsWith('godot-log-audit.json', [System.StringComparison]::OrdinalIgnoreCase) })
    foreach ($logAuditFile in $logAuditFiles) {
        $logAuditFileString = [string]$logAuditFile
        $auditPath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path $logAuditFileString
        if (-not (Test-PathWithin -BasePath $evidenceDir -ChildPath $auditPath)) {
            continue
        }

        $expectedLogFile = Get-ExpectedLogFileForAuditFile -AuditFile $logAuditFileString
        if ([string]::Equals($expectedLogFile, $logAuditFileString, [System.StringComparison]::OrdinalIgnoreCase)) {
            Add-Failure -Failures $failures -Message "Row $($required.Id) log audit file does not use the expected *-log-audit.json name: $logAuditFileString."
            continue
        }

        $expectedLogPath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path $expectedLogFile
        if (-not (Test-PathWithin -BasePath $evidenceDir -ChildPath $expectedLogPath)) {
            Add-Failure -Failures $failures -Message "Row $($required.Id) log audit expected log path escapes EvidenceDir: $expectedLogFile."
            continue
        }

        if ((Test-Path -LiteralPath $auditPath -PathType Leaf) -and -not (Read-CleanLogAudit -AuditPath $auditPath -ExpectedLogPath $expectedLogPath)) {
            Add-Failure -Failures $failures -Message "Row $($required.Id) log audit is not clean: $auditPath."
        }
    }

    if ($required.Kind -eq 'clicked-ui') {
        $preflightPath = Join-Path $evidenceDir 'window-preflight.json'
        if ((Test-Path -LiteralPath $preflightPath -PathType Leaf) -and -not (Test-PreflightForeground -PreflightPath $preflightPath)) {
            Add-Failure -Failures $failures -Message "Row $($required.Id) window-preflight.json does not prove Slay the Spire 2 was foreground."
        }

        $screenshotFile = [string](Get-PropertyValue -Object $row -Name 'ScreenshotFile' -Default '')
        if (-not [string]::IsNullOrWhiteSpace($screenshotFile)) {
            $screenshotPath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path $screenshotFile
            if (-not (Test-PathWithin -BasePath $evidenceDir -ChildPath $screenshotPath)) {
                Add-Failure -Failures $failures -Message "Row $($required.Id) screenshot path escapes EvidenceDir: $screenshotFile."
            } elseif (-not (Test-Path -LiteralPath $screenshotPath -PathType Leaf)) {
                [void]$protectedEvidencePaths.Add($screenshotPath)
                Add-Failure -Failures $failures -Message "Row $($required.Id) screenshot file is missing: $screenshotPath."
            } elseif ((Get-Item -LiteralPath $screenshotPath).Length -eq 0) {
                [void]$protectedEvidencePaths.Add($screenshotPath)
                Add-Failure -Failures $failures -Message "Row $($required.Id) screenshot file is empty: $screenshotPath."
            } else {
                [void]$protectedEvidencePaths.Add($screenshotPath)
                Add-OrdinaryEvidenceFileFailures -Path $screenshotPath -Label "Row $($required.Id) screenshot file '$screenshotFile'" -Failures $failures
                if (-not (Test-PngSignature -Path $screenshotPath)) {
                    Add-Failure -Failures $failures -Message "Row $($required.Id) screenshot file is not a valid PNG: $screenshotPath."
                } elseif (-not (Test-PngMinimumDimensions -Path $screenshotPath)) {
                    Add-Failure -Failures $failures -Message "Row $($required.Id) screenshot file is too small: $screenshotPath. Minimum is ${MinScreenshotWidth}x${MinScreenshotHeight}."
                }
            }
        } else {
            $screenshots = @(Get-ChildItem -LiteralPath $evidenceDir -Filter '*.png' -File -ErrorAction SilentlyContinue)
            foreach ($screenshot in $screenshots) {
                [void]$protectedEvidencePaths.Add($screenshot.FullName)
                Add-OrdinaryEvidenceFileFailures -Path $screenshot.FullName -Label "Row $($required.Id) discovered screenshot '$($screenshot.Name)'" -Failures $failures
            }
            if ($screenshots.Count -eq 0) {
                Add-Failure -Failures $failures -Message "Row $($required.Id) needs a PNG screenshot or ScreenshotFile."
            } elseif (-not ($screenshots | Where-Object { $_.Length -gt 0 } | Select-Object -First 1)) {
                Add-Failure -Failures $failures -Message "Row $($required.Id) has only empty PNG screenshots in $evidenceDir."
            } elseif (-not ($screenshots | Where-Object { $_.Length -gt 0 -and (Test-PngSignature -Path $_.FullName) } | Select-Object -First 1)) {
                Add-Failure -Failures $failures -Message "Row $($required.Id) has no valid non-empty PNG screenshots in $evidenceDir."
            } elseif (-not ($screenshots | Where-Object { $_.Length -gt 0 -and (Test-PngMinimumDimensions -Path $_.FullName) } | Select-Object -First 1)) {
                Add-Failure -Failures $failures -Message "Row $($required.Id) has no valid PNG screenshots at least ${MinScreenshotWidth}x${MinScreenshotHeight} in $evidenceDir."
            }
        }

    }

    $resultNote = [string](Get-PropertyValue -Object $row -Name 'ResultNote' -Default '')
    if ([string]::IsNullOrWhiteSpace($resultNote)) {
        Add-Failure -Failures $failures -Message "Row $($required.Id) is pass but has no ResultNote."
    } elseif ($resultNote -match $invalidEvidenceNotePattern) {
        Add-Failure -Failures $failures -Message "Row $($required.Id) ResultNote describes invalid or non-counting evidence."
    }

    if ($required.Id -eq 'a19-a20-dedicated-boss-abilities') {
        $checklistPath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path 'boss-ability-checklist.md'
        if ((Test-PathWithin -BasePath $evidenceDir -ChildPath $checklistPath) -and
            (Test-Path -LiteralPath $checklistPath -PathType Leaf)) {
            Test-BossAbilityChecklist -ChecklistPath $checklistPath -Failures $failures -RowId $required.Id
        }
    }

    if ($required.Id -eq 'ancient-reward-visible-relics') {
        $checklistPath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path 'ancient-reward-relics-checklist.md'
        if ((Test-PathWithin -BasePath $evidenceDir -ChildPath $checklistPath) -and
            (Test-Path -LiteralPath $checklistPath -PathType Leaf)) {
            Test-AncientRewardRelicsChecklist -ChecklistPath $checklistPath -Failures $failures -RowId $required.Id
        }
    }

    if ($required.Id -eq 'rootblight-visual-behavior') {
        $checklistPath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path 'rootblight-behavior-checklist.md'
        if ((Test-PathWithin -BasePath $evidenceDir -ChildPath $checklistPath) -and
            (Test-Path -LiteralPath $checklistPath -PathType Leaf)) {
            Test-RootblightBehaviorChecklist -ChecklistPath $checklistPath -Failures $failures -RowId $required.Id
        }
    }

    if ($required.Id -eq 'art-resource-routing-live-preview') {
        $checklistPath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path 'art-resource-routing-checklist.md'
        if ((Test-PathWithin -BasePath $evidenceDir -ChildPath $checklistPath) -and
            (Test-Path -LiteralPath $checklistPath -PathType Leaf)) {
            Test-ArtResourceRoutingChecklist -ChecklistPath $checklistPath -Failures $failures -RowId $required.Id
        }
    }

    if ($required.Id -eq 'player-text-tooltip-readability') {
        $checklistPath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path 'player-text-qa-checklist.md'
        if ((Test-PathWithin -BasePath $evidenceDir -ChildPath $checklistPath) -and
            (Test-Path -LiteralPath $checklistPath -PathType Leaf)) {
            Test-PlayerTextQaChecklist -ChecklistPath $checklistPath -Failures $failures -RowId $required.Id
        }
    }

    if ($required.Id -eq 'vakuu-victory-no-black-screen') {
        $checklistPath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path 'vakuu-victory-checklist.md'
        if ((Test-PathWithin -BasePath $evidenceDir -ChildPath $checklistPath) -and
            (Test-Path -LiteralPath $checklistPath -PathType Leaf)) {
            Test-SimpleChecklistRows `
                -ChecklistPath $checklistPath `
                -RequiredRows $requiredVakuuVictoryRows `
                -TemplateInstruction 'Copy this file to `vakuu-victory-checklist.md`' `
                -ChecklistName 'vakuu-victory-checklist.md' `
                -Failures $failures `
                -RowId $required.Id
        }
    }

    if ($required.Id -eq 'vakuu-failure-death-path') {
        $checklistPath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path 'vakuu-failure-death-checklist.md'
        if ((Test-PathWithin -BasePath $evidenceDir -ChildPath $checklistPath) -and
            (Test-Path -LiteralPath $checklistPath -PathType Leaf)) {
            Test-SimpleChecklistRows `
                -ChecklistPath $checklistPath `
                -RequiredRows $requiredVakuuFailureDeathRows `
                -TemplateInstruction 'Copy this file to `vakuu-failure-death-checklist.md`' `
                -ChecklistName 'vakuu-failure-death-checklist.md' `
                -Failures $failures `
                -RowId $required.Id
        }
    }

    if ($required.Id -eq 'vakuu-active-fight-save-load') {
        $checklistPath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path 'vakuu-save-load-checklist.md'
        if ((Test-PathWithin -BasePath $evidenceDir -ChildPath $checklistPath) -and
            (Test-Path -LiteralPath $checklistPath -PathType Leaf)) {
            Test-SimpleChecklistRows `
                -ChecklistPath $checklistPath `
                -RequiredRows $requiredVakuuSaveLoadRows `
                -TemplateInstruction 'Copy this file to `vakuu-save-load-checklist.md`' `
                -ChecklistName 'vakuu-save-load-checklist.md' `
                -Failures $failures `
                -RowId $required.Id
        }
    }

    if ($required.Id -eq 'preview-tools-live-proof') {
        $checklistPath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path 'preview-tools-checklist.md'
        if ((Test-PathWithin -BasePath $evidenceDir -ChildPath $checklistPath) -and
            (Test-Path -LiteralPath $checklistPath -PathType Leaf)) {
            Test-SimpleChecklistRows `
                -ChecklistPath $checklistPath `
                -RequiredRows $requiredPreviewToolsRows `
                -TemplateInstruction 'Copy this file to `preview-tools-checklist.md`' `
                -ChecklistName 'preview-tools-checklist.md' `
                -Failures $failures `
                -RowId $required.Id
        }
    }

    if ($required.Id -eq 'coop-disposition') {
        $checklistPath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path 'coop-disposition-checklist.md'
        if ((Test-PathWithin -BasePath $evidenceDir -ChildPath $checklistPath) -and
            (Test-Path -LiteralPath $checklistPath -PathType Leaf)) {
            Test-SimpleChecklistRows `
                -ChecklistPath $checklistPath `
                -RequiredRows $requiredCoopRows `
                -TemplateInstruction 'Copy this file to `coop-disposition-checklist.md`' `
                -ChecklistName 'coop-disposition-checklist.md' `
                -Failures $failures `
                -RowId $required.Id
        }
    }

    if ($required.Id -eq 'mod-settings-current-display') {
        $checklistPath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path 'mod-settings-checklist.md'
        if ((Test-PathWithin -BasePath $evidenceDir -ChildPath $checklistPath) -and
            (Test-Path -LiteralPath $checklistPath -PathType Leaf)) {
            Test-SimpleChecklistRows `
                -ChecklistPath $checklistPath `
                -RequiredRows $requiredModSettingsRows `
                -TemplateInstruction 'Copy this file to `mod-settings-checklist.md`' `
                -ChecklistName 'mod-settings-checklist.md' `
                -Failures $failures `
                -RowId $required.Id
        }
    }
}

Add-OutputAliasFailures `
    -OutputPath $passMarkerFull `
    -ProtectedPaths @($protectedEvidencePaths.ToArray()) `
    -Label 'Pass marker' `
    -Failures $failures

$summary = [ordered]@{
    ManifestPath = $manifestFull
    EvidenceRoot = $evidenceRootFull
    CheckedAt = (Get-Date).ToString('o')
    PackageSha256 = $PackageSha256
    PackagePath = $packageFull
    ActualPackageSha256 = $actualPackageSha256
    MinScreenshotWidth = $MinScreenshotWidth
    MinScreenshotHeight = $MinScreenshotHeight
    AllowDeferred = [bool]$AllowDeferred
    RequiredRowCount = $requiredReleaseRows.Count
    FailureCount = $failures.Count
    WarningCount = $warnings.Count
    Failures = @($failures)
    Warnings = @($warnings)
    WritePassMarker = [bool]$WritePassMarker
    PassMarkerPath = $passMarkerFull
}

$summary | ConvertTo-Json -Depth 20

if ($failures.Count -gt 0) {
    exit 1
}

if ($WritePassMarker) {
    Write-PassMarker -OutputPath $passMarkerFull -Summary $summary
}
