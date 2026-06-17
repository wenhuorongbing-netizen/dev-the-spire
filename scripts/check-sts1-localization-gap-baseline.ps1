param(
    [string]$OutFile,
    [switch]$FailOnMismatch
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
$checks = [System.Collections.Generic.List[object]]::new()
$mismatches = [System.Collections.Generic.List[string]]::new()

function Resolve-RepoPath {
    param([Parameter(Mandatory = $true)][string]$Path)

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }

    return [System.IO.Path]::GetFullPath((Join-Path $repoRoot $Path))
}

function Add-Check {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][bool]$Passed,
        [Parameter(Mandatory = $true)][string]$Detail
    )

    $checks.Add([pscustomobject]@{
        Name = $Name
        Passed = $Passed
        Detail = $Detail
    }) | Out-Null

    if (-not $Passed) {
        $mismatches.Add("${Name}: $Detail") | Out-Null
    }
}

function Add-SetCheck {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][string[]]$Actual,
        [Parameter(Mandatory = $true)][string[]]$Expected
    )

    $actualJoined = (@($Actual) | Sort-Object -Unique) -join ','
    $expectedJoined = (@($Expected) | Sort-Object -Unique) -join ','
    Add-Check -Name $Name -Passed ($actualJoined -eq $expectedJoined) -Detail "expected '$expectedJoined' but found '$actualJoined'"
}

$directEnabledModeKeys = @(
    'STS1_GOLDEN_IDOL.pages.LEAVE.description'
)

$simpleLaterKeys = @(
    'STS1_ANCIENT_WRITING.pages.ELEGANCE.description',
    'STS1_ANCIENT_WRITING.pages.SIMPLICITY.description',
    'STS1_AUGMENTER.pages.MUTATE.description',
    'STS1_AUGMENTER.pages.TRANSFORM.description',
    'STS1_MOAI_HEAD.pages.OFFER.description',
    'STS1_MOAI_HEAD.pages.WORSHIP.description'
)

$cardServiceKeys = @(
    'STS1_FALLING.pages.FLY.description',
    'STS1_FALLING.pages.HOLD_ON.description',
    'STS1_FALLING.pages.LET_GO.description',
    'STS1_KNOWING_SKULL.pages.QUESTION_1.description',
    'STS1_KNOWING_SKULL.pages.QUESTION_2.description',
    'STS1_KNOWING_SKULL.pages.QUESTION_3.description',
    'STS1_MIND_BLOOM.pages.AWAKE.description',
    'STS1_MIND_BLOOM.pages.RICH.description',
    'STS1_MIND_BLOOM.pages.WAR.description'
)

$combatBlockedKeys = @(
    'STS1_MASKED_BANDITS.pages.FIGHT.description',
    'STS1_MASKED_BANDITS.pages.PAY.description',
    'STS1_MYSTERIOUS_SPHERE.pages.OPEN.description',
    'STS1_SCORPION_NEST.pages.INVESTIGATE.description',
    'STS1_TREASURE_OOZE.pages.FIGHT.description',
    'STS1_TREASURE_OOZE.pages.OFFER.description'
)

$customUiLaterKeys = @(
    'STS1_DESIGNER.pages.REMOVE.description',
    'STS1_DESIGNER.pages.TRANSFORM.description',
    'STS1_DESIGNER.pages.UPGRADE.description',
    'STS1_FORGOTTEN_ALTAR.pages.DESECRATE.description',
    'STS1_FORGOTTEN_ALTAR.pages.OFFER.description',
    'STS1_FORGOTTEN_ALTAR.pages.PRAY.description',
    'STS1_TOMB_OF_LORD_RED_MASK.pages.OFFER_50.description',
    'STS1_TOMB_OF_LORD_RED_MASK.pages.OFFER_ALL.description',
    'STS1_WINDING_HALLS.pages.CONTINUE.description',
    'STS1_WINDING_HALLS.pages.EMBRACE.description',
    'STS1_WINDING_HALLS.pages.RETREAT.description'
)

$expectedMissingKeys = @(
    $directEnabledModeKeys +
    $simpleLaterKeys +
    $cardServiceKeys +
    $combatBlockedKeys +
    $customUiLaterKeys
)

$closurePlanPath = Resolve-RepoPath 'docs\features\sts1-events\localization-gap-closure-plan.md'
Add-Check -Name 'closure_plan_exists' -Passed (Test-Path -LiteralPath $closurePlanPath) -Detail "requires $closurePlanPath"

$closurePlanCueKeys = @()
if (Test-Path -LiteralPath $closurePlanPath) {
    foreach ($line in [System.IO.File]::ReadAllLines($closurePlanPath)) {
        if ($line -match '^\|\s*`([^`]+)`\s*\|') {
            $closurePlanCueKeys += $Matches[1]
        }
    }
}

Add-Check -Name 'closure_plan_cue_count_is_33' -Passed ($closurePlanCueKeys.Count -eq 33) -Detail "expected 33 closure-plan key cues but found $($closurePlanCueKeys.Count)"
Add-SetCheck -Name 'closure_plan_cue_keys_match_missing_baseline' -Actual $closurePlanCueKeys -Expected $expectedMissingKeys

$localizationChecker = Join-Path $PSScriptRoot 'check-sts1-localization-source-keys.ps1'
if (-not (Test-Path -LiteralPath $localizationChecker)) {
    Write-Error "Required checker not found: $localizationChecker"
    exit 1
}

$global:LASTEXITCODE = 0
$output = @(& $localizationChecker 2>&1)
$exitCode = $LASTEXITCODE
if ($null -eq $exitCode) {
    $exitCode = 0
}

Add-Check -Name 'localization_source_checker_exit_zero' -Passed ($exitCode -eq 0) -Detail "exit code $exitCode"

$missingLines = @($output | Where-Object { "$_" -like 'missing *' })
$missingKeys = @()
$missingBothLanguages = @()

foreach ($line in $missingLines) {
    $text = "$line"
    if ($text -match '^missing\s+(\S+)\s+\[([^\]]+)\]') {
        $key = $Matches[1]
        $languages = $Matches[2]
        $missingKeys += $key
        if ($languages -eq 'eng,zhs') {
            $missingBothLanguages += $key
        }
    }
}

$summaryLine = @($output | Where-Object { "$_" -like 'missing_source_referenced_keys=*' } | Select-Object -First 1)
$missingCount = $null
if ($summaryLine.Count -gt 0 -and "$($summaryLine[0])" -match '^missing_source_referenced_keys=(\d+)$') {
    $missingCount = [int]$Matches[1]
}

Add-Check -Name 'missing_count_is_33' -Passed ($missingCount -eq 33) -Detail "expected 33 missing source-referenced keys but found $missingCount"
Add-SetCheck -Name 'missing_key_set_matches_v19_baseline' -Actual $missingKeys -Expected $expectedMissingKeys
Add-SetCheck -Name 'all_missing_keys_absent_in_both_languages' -Actual $missingBothLanguages -Expected $expectedMissingKeys
Add-SetCheck -Name 'direct_enabled_mode_missing_keys' -Actual $directEnabledModeKeys -Expected @($missingKeys | Where-Object { $directEnabledModeKeys -contains $_ })
Add-Check -Name 'simple_later_missing_count_is_6' -Passed (@($missingKeys | Where-Object { $simpleLaterKeys -contains $_ }).Count -eq 6) -Detail 'expected 6 simple/later keys'
Add-Check -Name 'card_service_missing_count_is_9' -Passed (@($missingKeys | Where-Object { $cardServiceKeys -contains $_ }).Count -eq 9) -Detail 'expected 9 card-service keys'
Add-Check -Name 'combat_blocked_missing_count_is_6' -Passed (@($missingKeys | Where-Object { $combatBlockedKeys -contains $_ }).Count -eq 6) -Detail 'expected 6 combat-blocked keys'
Add-Check -Name 'custom_ui_missing_count_is_11' -Passed (@($missingKeys | Where-Object { $customUiLaterKeys -contains $_ }).Count -eq 11) -Detail 'expected 11 custom-UI keys'

$report = [pscustomobject]@{
    Checks = $checks
    Mismatches = $mismatches
    DirectEnabledModeKeys = $directEnabledModeKeys
    SimpleLaterKeys = $simpleLaterKeys
    CardServiceKeys = $cardServiceKeys
    CombatBlockedKeys = $combatBlockedKeys
    CustomUiLaterKeys = $customUiLaterKeys
    ClosurePlanCueKeys = $closurePlanCueKeys
}

foreach ($check in $checks) {
    $status = if ($check.Passed) { 'pass' } else { 'fail' }
    Write-Output "$($check.Name) status=$status"
}

Write-Output "checks=$($checks.Count)"
Write-Output "mismatches=$($mismatches.Count)"
Write-Output "known_localization_gap_baseline=33"
Write-Output "direct_enabled_mode_missing_keys=$($directEnabledModeKeys.Count)"
Write-Output "simple_later_missing_keys=$($simpleLaterKeys.Count)"
Write-Output "card_service_missing_keys=$($cardServiceKeys.Count)"
Write-Output "combat_blocked_missing_keys=$($combatBlockedKeys.Count)"
Write-Output "custom_ui_missing_keys=$($customUiLaterKeys.Count)"

foreach ($mismatch in $mismatches) {
    Write-Output "mismatch $mismatch"
}

if ($OutFile) {
    $resolvedOutFile = Resolve-RepoPath $OutFile
    $outDir = [System.IO.Path]::GetDirectoryName($resolvedOutFile)
    if ($outDir -and -not (Test-Path -LiteralPath $outDir)) {
        [void][System.IO.Directory]::CreateDirectory($outDir)
    }

    $report | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $resolvedOutFile -Encoding UTF8
}

if ($FailOnMismatch -and $mismatches.Count -gt 0) {
    exit 1
}
