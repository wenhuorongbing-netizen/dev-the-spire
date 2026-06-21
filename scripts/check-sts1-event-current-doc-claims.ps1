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

function Read-RepoText {
    param([Parameter(Mandatory = $true)][string]$Path)

    $resolved = Resolve-RepoPath $Path
    if (-not (Test-Path -LiteralPath $resolved)) {
        Write-Error "Required file not found: $resolved"
        exit 1
    }

    return [System.IO.File]::ReadAllText($resolved)
}

function ConvertTo-RepoRelativePath {
    param([Parameter(Mandatory = $true)][string]$Path)

    $fullPath = [System.IO.Path]::GetFullPath($Path)
    if ($fullPath.StartsWith($repoRoot, [System.StringComparison]::OrdinalIgnoreCase)) {
        return $fullPath.Substring($repoRoot.Length).TrimStart('\', '/')
    }

    return $fullPath
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

function Add-ContainsCheck {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][string]$Text,
        [Parameter(Mandatory = $true)][string]$Needle
    )

    Add-Check -Name $Name -Passed ($Text.Contains($Needle)) -Detail "requires '$Needle'"
}

function Add-RegexCheck {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][string]$Text,
        [Parameter(Mandatory = $true)][string]$Pattern
    )

    Add-Check -Name $Name -Passed ([regex]::IsMatch($Text, $Pattern, [System.Text.RegularExpressions.RegexOptions]::Singleline)) -Detail "requires pattern '$Pattern'"
}

function Add-NoRegexCheck {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][string[]]$Paths,
        [Parameter(Mandatory = $true)][string]$Pattern
    )

    $hits = [System.Collections.Generic.List[string]]::new()

    foreach ($path in $Paths) {
        $resolved = Resolve-RepoPath $path
        if (-not (Test-Path -LiteralPath $resolved)) {
            $hits.Add("${path}: missing file") | Out-Null
            continue
        }

        $lines = [System.IO.File]::ReadAllLines($resolved)
        for ($i = 0; $i -lt $lines.Length; $i++) {
            if ([regex]::IsMatch($lines[$i], $Pattern)) {
                $lineNumber = $i + 1
                $hits.Add("${path}:${lineNumber}: $($lines[$i])") | Out-Null
            }
        }
    }

    $detail = if ($hits.Count -eq 0) {
        "must not match pattern '$Pattern'"
    } else {
        "unexpected matches for '$Pattern': $($hits -join ' | ')"
    }

    Add-Check -Name $Name -Passed ($hits.Count -eq 0) -Detail $detail
}

function Test-AutoSlayProofCommandText {
    param([AllowEmptyString()][string]$Text)

    $normalized = $Text -replace '\s+', ' '
    $scriptPathPattern = '(?:(?:\.{1,2}[\\/])?scripts[\\/])check-spire-plus-autoslay-packet\.ps1'
    $scriptPattern = '^\s*(?:&\s*)?(?:"{0}"|''{0}''|{0}\b)' -f $scriptPathPattern
    return [regex]::IsMatch($normalized, $scriptPattern, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase) -and
        [regex]::IsMatch($normalized, '(?i)(^|\s)-FailOnMismatch(\s|$)')
}

function Add-AutoSlayProofCommandTargetCheck {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][string[]]$Paths
    )

    $hits = [System.Collections.Generic.List[string]]::new()

    foreach ($path in $Paths) {
        $resolved = Resolve-RepoPath $path
        if (-not (Test-Path -LiteralPath $resolved)) {
            $hits.Add("${path}: missing file") | Out-Null
            continue
        }

        $lines = [System.IO.File]::ReadAllLines($resolved)
        $logicalLine = ''
        $logicalStart = 1

        for ($i = 0; $i -lt $lines.Length; $i++) {
            $line = $lines[$i]
            if ($logicalLine.Length -eq 0) {
                $logicalStart = $i + 1
            }

            $trimmedRight = $line.TrimEnd()
            $continues = $trimmedRight.EndsWith('`')
            $segment = if ($continues) {
                $trimmedRight.Substring(0, $trimmedRight.Length - 1)
            } else {
                $line
            }

            $logicalLine = if ($logicalLine.Length -eq 0) {
                $segment
            } else {
                "$logicalLine $segment"
            }

            if ($continues) {
                continue
            }

            $normalized = $logicalLine -replace '\s+', ' '
            $isAutoSlayProofCommand = Test-AutoSlayProofCommandText -Text $normalized
            $hasExpectedAncientIds = [regex]::IsMatch($normalized, '(?i)(^|\s)-ExpectedAncientIds(\s|$)')

            if ($isAutoSlayProofCommand -and -not $hasExpectedAncientIds) {
                $hits.Add("${path}:${logicalStart}: $normalized") | Out-Null
            }

            $logicalLine = ''
        }
    }

    $detail = if ($hits.Count -eq 0) {
        'AutoSlay proof commands in active docs include -ExpectedAncientIds'
    } else {
        "AutoSlay proof commands missing -ExpectedAncientIds: $($hits -join ' | ')"
    }

    Add-Check -Name $Name -Passed ($hits.Count -eq 0) -Detail $detail
}

function Add-AutoSlayProofCommandNoSwitchCheck {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][string[]]$Paths,
        [Parameter(Mandatory = $true)][string]$SwitchName
    )

    $hits = [System.Collections.Generic.List[string]]::new()
    $switchPattern = '(?i)(^|\s)-{0}(\s|$)' -f [regex]::Escape($SwitchName)

    foreach ($path in $Paths) {
        $resolved = Resolve-RepoPath $path
        if (-not (Test-Path -LiteralPath $resolved)) {
            $hits.Add("${path}: missing file") | Out-Null
            continue
        }

        $lines = [System.IO.File]::ReadAllLines($resolved)
        $logicalLine = ''
        $logicalStart = 1

        for ($i = 0; $i -lt $lines.Length; $i++) {
            $line = $lines[$i]
            if ($logicalLine.Length -eq 0) {
                $logicalStart = $i + 1
            }

            $trimmedRight = $line.TrimEnd()
            $continues = $trimmedRight.EndsWith('`')
            $segment = if ($continues) {
                $trimmedRight.Substring(0, $trimmedRight.Length - 1)
            } else {
                $line
            }

            $logicalLine = if ($logicalLine.Length -eq 0) {
                $segment
            } else {
                "$logicalLine $segment"
            }

            if ($continues) {
                continue
            }

            $normalized = $logicalLine -replace '\s+', ' '
            $isAutoSlayProofCommand = Test-AutoSlayProofCommandText -Text $normalized
            $hasDisallowedSwitch = [regex]::IsMatch($normalized, $switchPattern)

            if ($isAutoSlayProofCommand -and $hasDisallowedSwitch) {
                $hits.Add("${path}:${logicalStart}: $normalized") | Out-Null
            }

            $logicalLine = ''
        }
    }

    $detail = if ($hits.Count -eq 0) {
        "AutoSlay proof commands in active docs do not use -$SwitchName"
    } else {
        "AutoSlay proof commands unexpectedly use -${SwitchName}: $($hits -join ' | ')"
    }

    Add-Check -Name $Name -Passed ($hits.Count -eq 0) -Detail $detail
}

function Get-AutoSlayProofCommands {
    param(
        [Parameter(Mandatory = $true)][string[]]$Paths
    )

    $commands = [System.Collections.Generic.List[object]]::new()

    foreach ($path in $Paths) {
        $resolved = Resolve-RepoPath $path
        if (-not (Test-Path -LiteralPath $resolved)) {
            continue
        }

        $lines = [System.IO.File]::ReadAllLines($resolved)
        $logicalLine = ''
        $logicalStart = 1

        for ($i = 0; $i -lt $lines.Length; $i++) {
            $line = $lines[$i]
            if ($logicalLine.Length -eq 0) {
                $logicalStart = $i + 1
            }

            $trimmedRight = $line.TrimEnd()
            $continues = $trimmedRight.EndsWith('`')
            $segment = if ($continues) {
                $trimmedRight.Substring(0, $trimmedRight.Length - 1)
            } else {
                $line
            }

            $logicalLine = if ($logicalLine.Length -eq 0) {
                $segment
            } else {
                "$logicalLine $segment"
            }

            if ($continues) {
                continue
            }

            $normalized = $logicalLine -replace '\s+', ' '
            $isAutoSlayProofCommand = Test-AutoSlayProofCommandText -Text $normalized

            if ($isAutoSlayProofCommand) {
                $commands.Add([pscustomobject]@{
                    Path = $path
                    Line = $logicalStart
                    Text = $normalized
                }) | Out-Null
            }

            $logicalLine = ''
        }
    }

    return @($commands)
}

function Add-AutoSlayProofCommandPresentCheck {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][string[]]$Paths
    )

    $commands = @(Get-AutoSlayProofCommands -Paths $Paths)
    $detail = if ($commands.Count -gt 0) {
        "recognized AutoSlay proof commands: $(@($commands | ForEach-Object { "$($_.Path):$($_.Line)" }) -join ', ')"
    } else {
        'active docs must include at least one recognized check-spire-plus-autoslay-packet.ps1 -FailOnMismatch proof command'
    }

    Add-Check -Name $Name -Passed ($commands.Count -gt 0) -Detail $detail
}

function Add-AutoSlayProofCommandRequiredSwitchesCheck {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][string[]]$Paths,
        [Parameter(Mandatory = $true)][string[]]$SwitchNames
    )

    $hits = [System.Collections.Generic.List[string]]::new()
    $requiredSwitchPatterns = @{}
    foreach ($switchName in $SwitchNames) {
        $requiredSwitchPatterns[$switchName] = '(?i)(^|\s)-{0}(\s|$)' -f [regex]::Escape($switchName)
    }

    foreach ($path in $Paths) {
        if (-not (Test-Path -LiteralPath (Resolve-RepoPath $path))) {
            $hits.Add("${path}: missing file") | Out-Null
        }
    }

    foreach ($command in (Get-AutoSlayProofCommands -Paths $Paths)) {
        $missingSwitches = @($SwitchNames | Where-Object { -not [regex]::IsMatch($command.Text, $requiredSwitchPatterns[$_]) })
        if ($missingSwitches.Count -gt 0) {
            $hits.Add("$($command.Path):$($command.Line): missing $($missingSwitches -join ', '): $($command.Text)") | Out-Null
        }
    }

    $detail = if ($hits.Count -eq 0) {
        "AutoSlay proof commands in active docs include required switches: $($SwitchNames -join ', ')"
    } else {
        "AutoSlay proof commands missing required switches: $($hits -join ' | ')"
    }

    Add-Check -Name $Name -Passed ($hits.Count -eq 0) -Detail $detail
}

function Add-AutoSlayProofCommandSwitchValuesCheck {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][string[]]$Paths,
        [Parameter(Mandatory = $true)][hashtable]$SwitchValues
    )

    $hits = [System.Collections.Generic.List[string]]::new()
    $switchValuePatterns = @{}
    foreach ($switchName in $SwitchValues.Keys) {
        $escapedSwitch = [regex]::Escape($switchName)
        $escapedValue = [regex]::Escape([string]$SwitchValues[$switchName])
        $switchValuePatterns[$switchName] = '(?i)(^|\s)-{0}\s+["'']?{1}["'']?(\s|$)' -f $escapedSwitch, $escapedValue
    }

    foreach ($path in $Paths) {
        if (-not (Test-Path -LiteralPath (Resolve-RepoPath $path))) {
            $hits.Add("${path}: missing file") | Out-Null
        }
    }

    foreach ($command in (Get-AutoSlayProofCommands -Paths $Paths)) {
        $wrongValues = @($SwitchValues.Keys | Sort-Object | Where-Object { -not [regex]::IsMatch($command.Text, $switchValuePatterns[$_]) })
        if ($wrongValues.Count -gt 0) {
            $expected = @($wrongValues | ForEach-Object { "-$_ $($SwitchValues[$_])" }) -join ', '
            $hits.Add("$($command.Path):$($command.Line): expected $expected`: $($command.Text)") | Out-Null
        }
    }

    $expectedValues = @($SwitchValues.Keys | Sort-Object | ForEach-Object { "-$_ $($SwitchValues[$_])" }) -join ', '
    $detail = if ($hits.Count -eq 0) {
        "AutoSlay proof commands in active docs pin current target values: $expectedValues"
    } else {
        "AutoSlay proof commands missing current target values: $($hits -join ' | ')"
    }

    Add-Check -Name $Name -Passed ($hits.Count -eq 0) -Detail $detail
}

$agents = Read-RepoText 'AGENTS.md'
$projectState = Read-RepoText 'PROJECT_STATE.md'
$rootReadme = Read-RepoText 'README.md'
$goalGuard = Read-RepoText 'docs\goal.md'
$docRestructureSpec = Read-RepoText 'docs\doc-restructure-spec.md'
$restructureDoc = Read-RepoText 'docs\restructure.md'
$projectMap = Read-RepoText 'docs\PROJECT_MAP.md'
$patchBoundaries = Read-RepoText 'docs\architecture\patch-boundaries.md'
$betaCompatibility = Read-RepoText 'docs\BETA_COMPATIBILITY.md'
$remoteDevelopmentSetup = Read-RepoText 'docs\REMOTE_DEVELOPMENT_SETUP.md'
$sourceApiDriftAudit = Read-RepoText 'docs\audits\v0.106-source-api-drift.md'
$docsReadme = Read-RepoText 'docs\README.md'
$rootTestPlan = Read-RepoText 'docs\test-plan.md'
$migrationDoc = Read-RepoText 'docs\migration.md'
$goalMigrationDoc = Read-RepoText 'docs\goals\migration.md'
$goalDebugDoc = Read-RepoText 'docs\goals\debug.md'
$goalRefactorDoc = Read-RepoText 'docs\goals\refactor.md'
$goalEventDoc = Read-RepoText 'docs\goals\event.md'
$testReadyGoal = Read-RepoText 'docs\test-ready-development-goal.md'
$devEnvironment = Read-RepoText 'docs\dev-environment.md'
$rootIssues = Read-RepoText 'docs\issues.md'
$activeReview = Read-RepoText 'docs\review.md'
$toReview = Read-RepoText 'docs\toreview.md'
$legacyV5MonthlySpec = Read-RepoText 'docs\goals\sts1_event_port_strict_audit_monthly_spec_v5_overnight_subagents.md'
$ritsuIntegrationDoc = Read-RepoText 'docs\integrations\ritsulib.md'
$ritsuMonthlyDevSpec = Read-RepoText 'docs\features\ritsulib-migration\monthly-dev-spec.md'
$ritsuBatch4cCandidates = Read-RepoText 'docs\features\ritsulib-migration\batch-4c-candidates.md'
$ritsuRuntimeHardBlock = Read-RepoText 'docs\features\ritsulib-migration\runtime-hard-block-report-20260531.md'
$m5RevisionLRuntimeHardBlocker = Read-RepoText 'docs\goals\m5-revision-l-runtime-hard-blocker.md'
$m5RevisionLRuntimeSmokePlan = Read-RepoText 'docs\goals\m5-revision-l-runtime-smoke-plan.md'
$m5RevisionLFinalReport = Read-RepoText 'docs\goals\m5-revision-l-final-report.md'
$m5RevisionLOwnerPacket = Read-RepoText 'docs\goals\m5-revision-l-owner-review-packet.md'
$m5RevisionLDirtyLedger = Read-RepoText 'docs\goals\m5-revision-l-dirty-ledger.md'
$m5RevisionLCommitSlices = Read-RepoText 'docs\goals\m5-revision-l-commit-slices.md'
$m5RevisionLWarningLedger = Read-RepoText 'docs\goals\m5-revision-l-warning-ledger.md'
$m5RevisionMFinalReport = Read-RepoText 'docs\goals\m5-revision-m-final-report.md'
$m5RevisionMOwnerPacket = Read-RepoText 'docs\goals\m5-revision-m-owner-review-packet.md'
$m5RevisionMRuntimeDriftReport = Read-RepoText 'docs\goals\m5-revision-m-runtime-drift-report.md'
$m5RevisionMPatchFailureLedger = Read-RepoText 'docs\goals\m5-revision-m-patch-failure-ledger.md'
$m5RevisionMVersionDecision = Read-RepoText 'docs\goals\m5-revision-m-version-decision.md'
$m5RevisionMCommitSlices = Read-RepoText 'docs\goals\m5-revision-m-commit-slices.md'
$goalWarningLedger = Read-RepoText 'docs\goals\warning-ledger.md'
$overnightRunLedger = Read-RepoText 'docs\goals\overnight-run-ledger.md'
$overnightRunStatus = Read-RepoText 'docs\goals\overnight-run-status.md'
$scriptsReadme = Read-RepoText 'scripts\README.md'
$runtimeMonkeyDocs = Read-RepoText 'docs\testing\runtime-monkey-stability.md'
$liveSessionScript = Read-RepoText 'scripts\spire-plus-live-session.ps1'
$staticSuiteScript = Read-RepoText 'scripts\check-sts1-event-static-suite.ps1'
$runtimeEvidencePacketScript = Read-RepoText 'scripts\check-sts1-runtime-evidence-packet.ps1'
$enabledModeLogScript = Read-RepoText 'scripts\check-sts1-enabled-mode-runtime-log.ps1'
$runtimePreflightScript = Read-RepoText 'scripts\check-sts1-runtime-preflight.ps1'
$gateLedgerCheckerScript = Read-RepoText 'scripts\check-sts1-v19-gate-ledger.ps1'
$v20FinalGateOverlayCheckerScript = Read-RepoText 'scripts\check-sts1-v20-final-gate-overlay.ps1'
$staticFileHygieneScript = Read-RepoText 'scripts\check-sts1-static-file-hygiene.ps1'
$runtimeChecklist = Read-RepoText 'docs\features\ritsulib-migration\runtime-smoke-checklist.md'
$nextOvernight = Read-RepoText 'docs\features\ritsulib-migration\next-overnight-run.md'
$liveRiskIssue = Read-RepoText 'docs\issues\ISSUE-2026-05-28-STS1EVENTS-INCOMPLETE-SKELETON-LIVE-RISK.md'
$sts1FeatureGoal = Read-RepoText 'docs\features\sts1-events\goal.md'
$implementationPlan = Read-RepoText 'docs\features\sts1-events\implementation-plan.md'
$featureReadme = Read-RepoText 'docs\features\sts1-events\README.md'
$canonicalEventMatrix = Read-RepoText 'docs\features\sts1-events\canonical-event-matrix.csv'
$registryReconciliation = Read-RepoText 'docs\features\sts1-events\registry-reconciliation.md'
$sts1ActEventRegistrationResearch = Read-RepoText 'docs\features\sts1-events\source-research\sts2-act-event-registration.md'
$sts1EventEngineResearch = Read-RepoText 'docs\features\sts1-events\source-research\sts2-event-engine.md'
$statusBoard = Read-RepoText 'docs\features\sts1-events\status-board.md'
$testPlan = Read-RepoText 'docs\features\sts1-events\test-plan.md'
$multiplayerFailClosedGuard = Read-RepoText 'docs\features\sts1-events\multiplayer-fail-closed-guard.md'
$localizationDoc = Read-RepoText 'docs\features\sts1-events\localization.md'
$localizationClosurePlan = Read-RepoText 'docs\features\sts1-events\localization-gap-closure-plan.md'
$localizationGapScan = Read-RepoText 'docs\features\sts1-events\localization-source-gap-scan-20260611.md'
$gateMap = Read-RepoText 'docs\features\sts1-events\v19-gate-evidence-map.md'
$gateLedger = Read-RepoText 'docs\features\sts1-events\v19-gate-ledger.csv'
$v20FinalGateOverlay = Read-RepoText 'docs\features\sts1-events\v20-final-gate-overlay.csv'
$subagentCoverage = Read-RepoText 'docs\features\sts1-events\v19-subagent-coverage.md'
$hardStop = Read-RepoText 'docs\features\sts1-events\hard-stop-blocker-report-v19-validation-coordination-20260611.md'
$hardStopV20 = Read-RepoText 'docs\features\sts1-events\hard-stop-blocker-report-v20-coordination-pause-20260617.md'
$historicalHardStopV2 = Read-RepoText 'docs\features\sts1-events\hard-stop-blocker-report.md'
$historicalO24Handoff = Read-RepoText 'docs\features\sts1-events\o24-handoff.md'
$historicalHardStopV12 = Read-RepoText 'docs\features\sts1-events\hard-stop-blocker-report-v12.md'
$historicalHardStopV13 = Read-RepoText 'docs\features\sts1-events\hard-stop-blocker-report-v13.md'
$historicalHardStopV14 = Read-RepoText 'docs\features\sts1-events\hard-stop-blocker-report-v14.md'
$historicalHardStopV15 = Read-RepoText 'docs\features\sts1-events\hard-stop-blocker-report-v15.md'
$currentValidation = Read-RepoText 'docs\reviews\current-validation.md'
$historicalOvernightRunReview = Read-RepoText 'docs\reviews\overnight-run-20260529.md'
$historicalRefactorQaReview = Read-RepoText 'docs\reviews\refactor-qa-20260602.md'
$historicalRefactorQaRound2Review = Read-RepoText 'docs\reviews\refactor-qa-20260602-round2.md'
$privateBetaHandoff = Read-RepoText 'docs\private-beta-verification-handoff.md'
$releaseChecklist = Read-RepoText 'docs\release-checklist.md'
$releaseEvidenceStatus = Read-RepoText 'docs\release-evidence-status.md'
$privateBetaReleaseAudit = Read-RepoText 'docs\private-beta-release-completion-audit.md'
$testReadyCompletionAudit = Read-RepoText 'docs\test-ready-completion-audit.md'
$platformTesting = Read-RepoText 'docs\platform-testing.md'

$currentClaimFiles = @(
    'AGENTS.md',
    'PROJECT_STATE.md',
    'README.md',
    'docs\goal.md',
    'docs\doc-restructure-spec.md',
    'docs\restructure.md',
    'docs\PROJECT_MAP.md',
    'docs\architecture\patch-boundaries.md',
    'docs\BETA_COMPATIBILITY.md',
    'docs\REMOTE_DEVELOPMENT_SETUP.md',
    'docs\audits\v0.106-source-api-drift.md',
    'docs\README.md',
    'docs\platform-testing.md',
    'docs\test-plan.md',
    'docs\migration.md',
    'docs\goals\migration.md',
    'docs\goals\debug.md',
    'docs\goals\refactor.md',
    'docs\goals\sts1_event_port_strict_audit_monthly_spec_v5_overnight_subagents.md',
    'docs\integrations\ritsulib.md',
    'docs\features\ritsulib-migration\monthly-dev-spec.md',
    'docs\features\ritsulib-migration\batch-4c-candidates.md',
    'docs\features\ritsulib-migration\runtime-hard-block-report-20260531.md',
    'docs\goals\m5-revision-l-runtime-hard-blocker.md',
    'docs\goals\m5-revision-l-runtime-smoke-plan.md',
    'docs\goals\m5-revision-l-final-report.md',
    'docs\goals\m5-revision-l-owner-review-packet.md',
    'docs\goals\m5-revision-l-dirty-ledger.md',
    'docs\goals\m5-revision-l-commit-slices.md',
    'docs\goals\m5-revision-l-warning-ledger.md',
    'docs\goals\m5-revision-m-final-report.md',
    'docs\goals\m5-revision-m-owner-review-packet.md',
    'docs\goals\m5-revision-m-runtime-drift-report.md',
    'docs\goals\m5-revision-m-patch-failure-ledger.md',
    'docs\goals\m5-revision-m-version-decision.md',
    'docs\goals\m5-revision-m-commit-slices.md',
    'docs\goals\warning-ledger.md',
    'docs\goals\overnight-run-ledger.md',
    'docs\goals\overnight-run-status.md',
    'docs\goals\event.md',
    'docs\test-ready-development-goal.md',
    'docs\dev-environment.md',
    'docs\issues.md',
    'docs\review.md',
    'docs\toreview.md',
    'docs\features\ritsulib-migration\runtime-smoke-checklist.md',
    'docs\features\ritsulib-migration\next-overnight-run.md',
    'docs\issues\ISSUE-2026-05-28-STS1EVENTS-INCOMPLETE-SKELETON-LIVE-RISK.md',
    'docs\features\sts1-events\README.md',
    'docs\features\sts1-events\localization.md',
    'docs\features\sts1-events\localization-gap-closure-plan.md',
    'docs\features\sts1-events\localization-source-gap-scan-20260611.md',
    'docs\features\sts1-events\registry-reconciliation.md',
    'docs\features\sts1-events\status-board.md',
    'docs\features\sts1-events\v19-gate-evidence-map.md',
    'docs\features\sts1-events\v19-gate-ledger.csv',
    'docs\features\sts1-events\v19-subagent-coverage.md',
    'docs\features\sts1-events\test-plan.md',
    'docs\features\sts1-events\hard-stop-blocker-report-v19-validation-coordination-20260611.md',
    'docs\features\sts1-events\hard-stop-blocker-report-v20-coordination-pause-20260617.md',
    'docs\reviews\current-validation.md',
    'docs\reviews\overnight-run-20260529.md',
    'docs\reviews\refactor-qa-20260602.md',
    'docs\reviews\refactor-qa-20260602-round2.md',
    'docs\private-beta-verification-handoff.md',
    'docs\release-checklist.md',
    'docs\release-evidence-status.md',
    'docs\private-beta-release-completion-audit.md',
    'docs\test-ready-completion-audit.md',
    'scripts\README.md'
)

$sts1FeatureClaimFiles = @(Get-ChildItem -LiteralPath (Resolve-RepoPath 'docs\features\sts1-events') -Recurse -File |
    Where-Object { $_.Extension -in @('.md', '.csv') } |
    ForEach-Object { (ConvertTo-RepoRelativePath $_.FullName).Replace('/', '\') })

$currentClaimFiles = @($currentClaimFiles + $sts1FeatureClaimFiles | Sort-Object -Unique)

Add-Check -Name 'current_claim_scan_includes_agents' -Passed ($currentClaimFiles -contains 'AGENTS.md') -Detail 'AGENTS.md must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_project_state' -Passed ($currentClaimFiles -contains 'PROJECT_STATE.md') -Detail 'PROJECT_STATE.md must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_root_readme' -Passed ($currentClaimFiles -contains 'README.md') -Detail 'README.md must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_goal_guard' -Passed ($currentClaimFiles -contains 'docs\goal.md') -Detail 'root goal guard must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_doc_restructure_spec' -Passed ($currentClaimFiles -contains 'docs\doc-restructure-spec.md') -Detail 'doc restructure spec must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_restructure_doc' -Passed ($currentClaimFiles -contains 'docs\restructure.md') -Detail 'restructure source design must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_patch_boundaries' -Passed ($currentClaimFiles -contains 'docs\architecture\patch-boundaries.md') -Detail 'patch-boundaries must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_beta_compatibility' -Passed ($currentClaimFiles -contains 'docs\BETA_COMPATIBILITY.md') -Detail 'beta compatibility doc must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_remote_development_setup' -Passed ($currentClaimFiles -contains 'docs\REMOTE_DEVELOPMENT_SETUP.md') -Detail 'remote development setup must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_source_api_drift_audit' -Passed ($currentClaimFiles -contains 'docs\audits\v0.106-source-api-drift.md') -Detail 'v0.106 source/API drift audit must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_docs_readme' -Passed ($currentClaimFiles -contains 'docs\README.md') -Detail 'docs README must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_platform_testing' -Passed ($currentClaimFiles -contains 'docs\platform-testing.md') -Detail 'platform testing guide must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_root_test_plan' -Passed ($currentClaimFiles -contains 'docs\test-plan.md') -Detail 'root test-plan must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_scripts_readme' -Passed ($currentClaimFiles -contains 'scripts\README.md') -Detail 'scripts README must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_migration_doc' -Passed ($currentClaimFiles -contains 'docs\migration.md') -Detail 'migration doc must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_goal_migration_doc' -Passed ($currentClaimFiles -contains 'docs\goals\migration.md') -Detail 'goal migration doc must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_goal_debug_doc' -Passed ($currentClaimFiles -contains 'docs\goals\debug.md') -Detail 'goal debug doc must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_goal_refactor_doc' -Passed ($currentClaimFiles -contains 'docs\goals\refactor.md') -Detail 'goal refactor doc must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_legacy_v5_monthly_spec' -Passed ($currentClaimFiles -contains 'docs\goals\sts1_event_port_strict_audit_monthly_spec_v5_overnight_subagents.md') -Detail 'legacy v5 monthly spec must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_ritsu_integration' -Passed ($currentClaimFiles -contains 'docs\integrations\ritsulib.md') -Detail 'RitsuLib integration doc must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_ritsu_monthly_spec' -Passed ($currentClaimFiles -contains 'docs\features\ritsulib-migration\monthly-dev-spec.md') -Detail 'RitsuLib monthly spec must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_ritsu_batch4c' -Passed ($currentClaimFiles -contains 'docs\features\ritsulib-migration\batch-4c-candidates.md') -Detail 'Batch 4c proposal must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_ritsu_runtime_checklist' -Passed ($currentClaimFiles -contains 'docs\features\ritsulib-migration\runtime-smoke-checklist.md') -Detail 'RitsuLib runtime smoke checklist must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_ritsu_runtime_hard_block' -Passed ($currentClaimFiles -contains 'docs\features\ritsulib-migration\runtime-hard-block-report-20260531.md') -Detail 'RitsuLib runtime hard-block report must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_ritsu_next_overnight' -Passed ($currentClaimFiles -contains 'docs\features\ritsulib-migration\next-overnight-run.md') -Detail 'RitsuLib next overnight run plan must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_m5_l_runtime_hard_blocker' -Passed ($currentClaimFiles -contains 'docs\goals\m5-revision-l-runtime-hard-blocker.md') -Detail 'M5 Revision L runtime hard-blocker must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_m5_l_runtime_smoke_plan' -Passed ($currentClaimFiles -contains 'docs\goals\m5-revision-l-runtime-smoke-plan.md') -Detail 'M5 Revision L runtime smoke plan must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_m5_l_final_report' -Passed ($currentClaimFiles -contains 'docs\goals\m5-revision-l-final-report.md') -Detail 'M5 Revision L final report must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_m5_l_owner_packet' -Passed ($currentClaimFiles -contains 'docs\goals\m5-revision-l-owner-review-packet.md') -Detail 'M5 Revision L owner packet must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_m5_l_dirty_ledger' -Passed ($currentClaimFiles -contains 'docs\goals\m5-revision-l-dirty-ledger.md') -Detail 'M5 Revision L dirty ledger must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_m5_l_commit_slices' -Passed ($currentClaimFiles -contains 'docs\goals\m5-revision-l-commit-slices.md') -Detail 'M5 Revision L commit slices must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_m5_l_warning_ledger' -Passed ($currentClaimFiles -contains 'docs\goals\m5-revision-l-warning-ledger.md') -Detail 'M5 Revision L warning ledger must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_m5_final_report' -Passed ($currentClaimFiles -contains 'docs\goals\m5-revision-m-final-report.md') -Detail 'M5 final report must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_m5_owner_packet' -Passed ($currentClaimFiles -contains 'docs\goals\m5-revision-m-owner-review-packet.md') -Detail 'M5 owner packet must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_m5_runtime_drift' -Passed ($currentClaimFiles -contains 'docs\goals\m5-revision-m-runtime-drift-report.md') -Detail 'M5 runtime drift report must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_m5_patch_failure_ledger' -Passed ($currentClaimFiles -contains 'docs\goals\m5-revision-m-patch-failure-ledger.md') -Detail 'M5 patch failure ledger must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_m5_version_decision' -Passed ($currentClaimFiles -contains 'docs\goals\m5-revision-m-version-decision.md') -Detail 'M5 version decision must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_m5_commit_slices' -Passed ($currentClaimFiles -contains 'docs\goals\m5-revision-m-commit-slices.md') -Detail 'M5 commit slices must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_goal_warning_ledger' -Passed ($currentClaimFiles -contains 'docs\goals\warning-ledger.md') -Detail 'current warning ledger must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_overnight_run_ledger' -Passed ($currentClaimFiles -contains 'docs\goals\overnight-run-ledger.md') -Detail 'overnight-run ledger must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_overnight_run_status' -Passed ($currentClaimFiles -contains 'docs\goals\overnight-run-status.md') -Detail 'overnight-run status must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_current_validation' -Passed ($currentClaimFiles -contains 'docs\reviews\current-validation.md') -Detail 'current-validation must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_historical_overnight_review' -Passed ($currentClaimFiles -contains 'docs\reviews\overnight-run-20260529.md') -Detail 'historical overnight review must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_historical_refactor_qa' -Passed ($currentClaimFiles -contains 'docs\reviews\refactor-qa-20260602.md') -Detail 'historical refactor QA review must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_historical_refactor_qa_round2' -Passed ($currentClaimFiles -contains 'docs\reviews\refactor-qa-20260602-round2.md') -Detail 'historical refactor QA round 2 review must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_v20_hard_stop' -Passed ($currentClaimFiles -contains 'docs\features\sts1-events\hard-stop-blocker-report-v20-coordination-pause-20260617.md') -Detail 'v20 hard-stop report must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_private_beta_handoff' -Passed ($currentClaimFiles -contains 'docs\private-beta-verification-handoff.md') -Detail 'private-beta handoff must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_release_checklist' -Passed ($currentClaimFiles -contains 'docs\release-checklist.md') -Detail 'release checklist must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_release_evidence_status' -Passed ($currentClaimFiles -contains 'docs\release-evidence-status.md') -Detail 'release evidence status must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_private_beta_release_audit' -Passed ($currentClaimFiles -contains 'docs\private-beta-release-completion-audit.md') -Detail 'private-beta release audit must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_test_ready_completion_audit' -Passed ($currentClaimFiles -contains 'docs\test-ready-completion-audit.md') -Detail 'test-ready completion audit must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_goal_event' -Passed ($currentClaimFiles -contains 'docs\goals\event.md') -Detail 'active event goal must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_test_ready_goal' -Passed ($currentClaimFiles -contains 'docs\test-ready-development-goal.md') -Detail 'test-ready goal must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_dev_environment' -Passed ($currentClaimFiles -contains 'docs\dev-environment.md') -Detail 'dev-environment must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_issues' -Passed ($currentClaimFiles -contains 'docs\issues.md') -Detail 'issues.md must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_review' -Passed ($currentClaimFiles -contains 'docs\review.md') -Detail 'review.md must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_toreview' -Passed ($currentClaimFiles -contains 'docs\toreview.md') -Detail 'toreview.md must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_sts1_feature_tree' -Passed ($sts1FeatureClaimFiles.Count -ge 80) -Detail "StS1 feature tree scan expected at least 80 docs, found $($sts1FeatureClaimFiles.Count)"
Add-Check -Name 'current_claim_scan_includes_sts1_goal' -Passed ($currentClaimFiles -contains 'docs\features\sts1-events\goal.md') -Detail 'StS1 feature goal must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_sts1_implementation_plan' -Passed ($currentClaimFiles -contains 'docs\features\sts1-events\implementation-plan.md') -Detail 'StS1 implementation plan must be in stale claim scan scope'
Add-Check -Name 'current_claim_scan_includes_sts1_event_spec' -Passed ($currentClaimFiles -contains 'docs\features\sts1-events\event-specs\golden-idol.md') -Detail 'StS1 event specs must be in stale claim scan scope'

Add-ContainsCheck -Name 'active_review_latest_addendum_beta93' -Text $activeReview -Needle 'Date: 2026-05-26; latest addendum: 2026-06-21'
Add-ContainsCheck -Name 'private_beta_handoff_current_beta96_date_boundary' -Text $privateBetaHandoff -Needle 'Date: 2026-06-21 beta.97 handoff summary; older May notes below are retained only as historical context.'
Add-ContainsCheck -Name 'agents_current_beta93_additive_path' -Text $agents -Needle 'Previous beta.93 AdditiveBatch1 loader/registration proof exists under `.tools/runtime-evidence/v01071-beta93-ritsulib0431-additivebatch1-direct-20260621/` as previous-package loader/registration evidence only.'
Add-ContainsCheck -Name 'agents_current_dependency_ritsulib_only' -Text $agents -Needle 'Current runtime dependency setup has official `STS2-RitsuLib` `v0.4.31` installed'
Add-ContainsCheck -Name 'agents_current_off_not_live_ready' -Text $agents -Needle 'Do not claim live-ready or release-ready because gameplay, clicked Ancient UI, save-load, current beta.97 enabled-mode registration/gameplay proof, replacement functional proof, multiplayer fail-closed, independent QA rerun, and versioned tester-package handoff remain pending; recapture current HEAD/worktree before any later tester handoff.'
Add-NoRegexCheck -Name 'agents_no_current_prefer_externalmod_rule' -Paths @('AGENTS.md') -Pattern 'Prefer ExternalMod|Install ExternalMod `v3\.2\.1`|verify ExternalMod and Spire Plus'
Add-ContainsCheck -Name 'project_state_current_off_non_claim' -Text $projectState -Needle 'Do not claim live-ready or release-ready because gameplay, clicked Ancient UI, save-load, co-op, beta.97 loader/settings proof, current beta.97 enabled-mode registration/gameplay proof, independent QA rerun, and tester-package handoff decisions remain pending.'
Add-ContainsCheck -Name 'project_state_sts1_enabled_modes_boundary' -Text $projectState -Needle 'Beta.96 RitsuLib Mod Settings clicked UI proof is captured under `.tools/runtime-evidence/beta96-ritsulib-mod-settings-clicked-ui-20260621-160701/`: the session temporarily isolated every mod except `STS2-RitsuLib` and `EZMicroBalance`, opened Settings -> `Mod Settings (RitsuLib)`, showed only `RitsuLib` and `Spire Plus`, rendered the Spire Plus Migration Status / runtime dependency / evidence-boundary / Preview Tools page, retained same-session `godot.log`, and audited clean.'
Add-ContainsCheck -Name 'project_state_autoslay_expected_ancient_ids_plan_summary' -Text $projectState -Needle 'requires proof-mode `-ExpectedAncientIds` to be provided, to match retained `autoslay-plan.json` `ExpectedAncientIds`, to appear in retained `autoslay-summary.json`, to retain `AncientIdCounts` that exactly match `Runs[].AncientId` aggregation with positive counts for every requested Ancient id and no extra zero-count Ancient ids, to reject non-positive `-MinRuns`, and to have sidecar-plus-current-log event traversal select that same Ancient id inside the ordered event-room sequence'
Add-ContainsCheck -Name 'project_state_autoslay_ancient_id_normalization' -Text $projectState -Needle 'uppercases expected, plan, summary, and traversed Ancient ids for target-coverage comparison while retaining exact per-run `run-result.json` versus `autoslay-summary.json` AncientId self-consistency'
Add-ContainsCheck -Name 'project_state_pause_safe_current_doc_claims_1090' -Text $projectState -Needle 'current-doc claims 1090 / 0'
Add-ContainsCheck -Name 'project_state_v20_overlay_static_alignment' -Text $projectState -Needle 'StS1 no-launch current-doc claims passed 897 / 0, later superseded by the 941 / 0 no-launch current-doc guard after the v20 final-gate overlay, v20 hard-stop report, PROJECT_STATE static-summary alignment, and active current-guidance route alignment; runtime preflight passed 23 / 0, static suite passed 14 / 0, static-file hygiene passed 11 / 0, v19 gate ledger passed 531 / 0, and v19 subagent coverage passed 66 / 0 in the pushed slice. Later pause-safe static alignment passed static suite 15 / 0, current-doc claims 956 / 0 after tuple-aware enabled-mode log verifier, CanaryOnly current-pass, repo-manifest runtime-preflight drift guard alignment, beta.86 AdditiveBatch1 doc alignment, and retained-loader subagent split, static-file hygiene 11 / 0, v19 gate ledger 534 / 0, v20 final-gate overlay 29 / 0, and subagent coverage 70 / 0 without closing runtime, gameplay, QA, release, or handoff gates.'
Add-ContainsCheck -Name 'root_readme_beta88_additive_path' -Text $rootReadme -Needle 'previous beta.93 AdditiveBatch1 loader/registration proof remains previous-package context only.'
Add-ContainsCheck -Name 'root_readme_historical_enabled_modes_only' -Text $rootReadme -Needle 'diagnostic Off, CanaryOnly, and AdditiveBatch1 loader smokes remain clean historical `v0.106.1` evidence only'
Add-ContainsCheck -Name 'root_readme_sts1_enabled_smokes_current_split' -Text $rootReadme -Needle 'The previous beta.96 RitsuLib-only Off proof reached main menu with exactly `STS2-RitsuLib` and `EZMicroBalance` loaded, clean audit, StS1Events disabled with 0 registration lines, and packet verifier 43 / 0; previous beta.93 AdditiveBatch1 registered 10 event types through 14 calls with verifier 31 / 0 and packet 61 / 0 for the previous package only.'
Add-ContainsCheck -Name 'root_readme_manual_rows_pending' -Text $rootReadme -Needle 'Manual feature verification, gameplay, clicked UI, save-load, preview-tools, Vakuu, beta.97 loader/settings proof, current enabled-mode registration/gameplay, co-op, independent QA rerun, and versioned tester-package handoff are still pending.'
Add-ContainsCheck -Name 'goal_guard_test_ready_not_release_ready' -Text $goalGuard -Needle 'Current target: test-ready manual build, not release-ready.'
Add-ContainsCheck -Name 'goal_guard_no_source_only_completion' -Text $goalGuard -Needle 'No source-only pass may mark this goal complete.'
Add-ContainsCheck -Name 'goal_guard_runtime_rows_need_evidence' -Text $goalGuard -Needle 'Runtime rows need game logs, screenshots, manual notes, or two-client evidence from the current beta package.'
Add-ContainsCheck -Name 'doc_restructure_spec_historical_only' -Text $docRestructureSpec -Needle 'This file is historical restructure planning only.'
Add-ContainsCheck -Name 'doc_restructure_spec_event_md_current' -Text $docRestructureSpec -Needle 'Use `docs/goals/event.md`, `docs/features/sts1-events/v19-gate-evidence-map.md`, `docs/features/sts1-events/v19-gate-ledger.csv`, `docs/features/sts1-events/v20-final-gate-overlay.csv`, `docs/features/sts1-events/hard-stop-blocker-report-v20-coordination-pause-20260617.md`, `PROJECT_STATE.md`, and `docs/test-ready-development-goal.md` for current StS1 event guidance'
Add-ContainsCheck -Name 'restructure_doc_event_md_current' -Text $restructureDoc -Needle 'Use `docs/goals/event.md`, `docs/features/sts1-events/v19-gate-evidence-map.md`, `docs/features/sts1-events/v19-gate-ledger.csv`, `docs/features/sts1-events/v20-final-gate-overlay.csv`, `docs/features/sts1-events/hard-stop-blocker-report-v20-coordination-pause-20260617.md`, `PROJECT_STATE.md`, and `docs/test-ready-development-goal.md` for current StS1 event guidance'
Add-ContainsCheck -Name 'restructure_doc_current_ritsulib_only_target' -Text $restructureDoc -Needle 'Current package/runtime target is Spire Plus `v0.1.0-private-beta.97` on Slay'
Add-ContainsCheck -Name 'restructure_doc_current_source_workspace_rule' -Text $restructureDoc -Needle 'scripts\check-local-godot-source-workspace.ps1 -RequireCurrentSourceSnapshot'
Add-ContainsCheck -Name 'doc_restructure_spec_beta93_nonclaim' -Text $docRestructureSpec -Needle 'previous beta.93 proves only RitsuLib-only `v0.107.1` Off and AdditiveBatch1 loader/registration behavior, beta.85/beta.87/beta.88/beta.90 rows remain previous-context evidence, and current CanaryOnly gameplay/runtime, save-load, replacement, multiplayer, QA, and handoff proof remain pending or blocked.'
Add-ContainsCheck -Name 'project_map_register_all_57' -Text $projectMap -Needle 'RegisterAll is now 57 calls'
Add-ContainsCheck -Name 'project_map_batch1_14' -Text $projectMap -Needle 'AdditiveBatch1 is 10 event types / 14 calls'
Add-ContainsCheck -Name 'project_map_act1_duplicate_list' -Text $projectMap -Needle 'Big Fish, Golden Idol, The Cleric, and Shining Light moved to Act 1 bucket registration'
Add-ContainsCheck -Name 'project_map_v19_gate_ledger' -Text $projectMap -Needle 'Use `v19-gate-evidence-map.md`, `v19-gate-ledger.csv`, `v20-final-gate-overlay.csv`, `hard-stop-blocker-report-v20-coordination-pause-20260617.md`, and `v19-subagent-coverage.md` for the current O0-O76 gate split, O76-O84 final documentation/handoff overlay, current v20 hard-stop/next-run point, and subagent split.'
Add-ContainsCheck -Name 'project_map_v19_subagent_coverage' -Text $projectMap -Needle '`docs/features/sts1-events/v19-gate-ledger.csv`, `docs/features/sts1-events/v20-final-gate-overlay.csv`, `docs/features/sts1-events/hard-stop-blocker-report-v20-coordination-pause-20260617.md`, and `docs/features/sts1-events/v19-subagent-coverage.md`'
Add-ContainsCheck -Name 'patch_boundaries_sts1_revision_n_boundary' -Text $patchBoundaries -Needle '2026-06-21 Revision P StS1Events Boundary'
Add-ContainsCheck -Name 'patch_boundaries_sts1_source_only' -Text $patchBoundaries -Needle 'this architecture file is source-boundary guidance only'
Add-ContainsCheck -Name 'patch_boundaries_sts1_no_runtime_proof' -Text $patchBoundaries -Needle 'Current StS1 event work routes through `docs/goals/event.md`, `docs/features/sts1-events/v19-gate-evidence-map.md`, `docs/features/sts1-events/v19-gate-ledger.csv`, `docs/features/sts1-events/v20-final-gate-overlay.csv`, `docs/features/sts1-events/hard-stop-blocker-report-v20-coordination-pause-20260617.md`, `PROJECT_STATE.md`, and `docs/test-ready-development-goal.md`'
Add-ContainsCheck -Name 'historical_overnight_review_v20_current_route' -Text $historicalOvernightRunReview -Needle 'Current StS1 event work routes through `docs/goals/event.md`, `docs/features/sts1-events/v19-gate-evidence-map.md`, `docs/features/sts1-events/v19-gate-ledger.csv`, `docs/features/sts1-events/v20-final-gate-overlay.csv`, `docs/features/sts1-events/hard-stop-blocker-report-v20-coordination-pause-20260617.md`, `PROJECT_STATE.md`, and `docs/test-ready-development-goal.md`'
Add-ContainsCheck -Name 'historical_refactor_qa_v20_current_route' -Text $historicalRefactorQaReview -Needle 'Current StS1 event work routes through `docs/goals/event.md`, `docs/features/sts1-events/v19-gate-evidence-map.md`, `docs/features/sts1-events/v19-gate-ledger.csv`, `docs/features/sts1-events/v20-final-gate-overlay.csv`, `docs/features/sts1-events/hard-stop-blocker-report-v20-coordination-pause-20260617.md`, `PROJECT_STATE.md`, and `docs/test-ready-development-goal.md`'
Add-ContainsCheck -Name 'historical_refactor_qa_round2_v20_current_route' -Text $historicalRefactorQaRound2Review -Needle 'Current StS1 event work routes through `docs/goals/event.md`, `docs/features/sts1-events/v19-gate-evidence-map.md`, `docs/features/sts1-events/v19-gate-ledger.csv`, `docs/features/sts1-events/v20-final-gate-overlay.csv`, `docs/features/sts1-events/hard-stop-blocker-report-v20-coordination-pause-20260617.md`, `PROJECT_STATE.md`, and `docs/test-ready-development-goal.md`'
Add-ContainsCheck -Name 'patch_boundaries_sts1_beta88_additive_nonclaim' -Text $patchBoundaries -Needle 'beta.85 proves retained `v0.107.0` default-Off and CanaryOnly loader behavior, beta.87 proves retained `v0.107.0` AdditiveBatch1 loader/registration behavior, beta.88 proves previous package `v0.107.1` AdditiveBatch1 loader/registration behavior, and previous beta.93 proves `v0.107.1` RitsuLib-only Off plus AdditiveBatch1 loader/registration behavior with STS2-RitsuLib `0.4.31`, 25/25 patches, 10 event types / 14 calls, clean audit, enabled-mode verifier 31 / 0, and runtime packet verifier 61 / 0.'
Add-ContainsCheck -Name 'patch_boundaries_sts1_owner_canary_4_6' -Text $patchBoundaries -Needle 'CanaryOnly source shape is 4 event types / 6 registration calls'
Add-ContainsCheck -Name 'patch_boundaries_sts1_registration_canary_4_6' -Text $patchBoundaries -Needle 'CanaryOnly registers exactly 4 event types through 6 registration calls'
Add-Check -Name 'patch_boundaries_no_canary_shared_event_claim' -Passed (-not $patchBoundaries.Contains('exactly 4 shared events registered')) -Detail 'patch boundaries must not describe current CanaryOnly proof as exactly 4 shared events'
Add-ContainsCheck -Name 'beta_compat_revision_m_boundary' -Text $betaCompatibility -Needle '2026-06-21 Current Compatibility Boundary'
Add-ContainsCheck -Name 'beta_compat_beta96_target' -Text $betaCompatibility -Needle 'Spire Plus `v0.1.0-private-beta.97`'
Add-ContainsCheck -Name 'beta_compat_ritsulib_only_dependency' -Text $betaCompatibility -Needle 'only `STS2-RitsuLib >= 0.4.31` as the shared runtime dependency'
Add-ContainsCheck -Name 'beta_compat_settings_ui_path' -Text $betaCompatibility -Needle '.tools/runtime-evidence/beta96-ritsulib-mod-settings-clicked-ui-20260621-160701/'
Add-ContainsCheck -Name 'beta_compat_loader_settings_nonclaim' -Text $betaCompatibility -Needle 'Treat loader and settings evidence as scoped proof only: gameplay, save-load, replacement, multiplayer, independent QA, package handoff, and release-ready compatibility proof remain pending.'
Add-ContainsCheck -Name 'remote_setup_current_boundary' -Text $remoteDevelopmentSetup -Needle '2026-06-21 Current Boundary'
Add-ContainsCheck -Name 'remote_setup_beta96_off_path' -Text $remoteDevelopmentSetup -Needle '.tools/runtime-evidence/v01071-beta96-ritsulib0431-off-direct-20260621-185056/'
Add-ContainsCheck -Name 'remote_setup_off_only_nonclaim' -Text $remoteDevelopmentSetup -Needle 'That proof covers startup and loader registration only; remote setup, gameplay, save-load, replacement, multiplayer, QA, package handoff, and release-ready compatibility still require their own current evidence on the target machine.'
Add-ContainsCheck -Name 'source_api_drift_audit_current_boundary' -Text $sourceApiDriftAudit -Needle '2026-06-21 Current Boundary'
Add-ContainsCheck -Name 'source_api_drift_audit_historical_v01061' -Text $sourceApiDriftAudit -Needle 'This audit remains historical `v0.106.1` source-shape evidence.'
Add-ContainsCheck -Name 'source_api_drift_audit_current_nonclaim' -Text $sourceApiDriftAudit -Needle 'Do not use this audit as current `v0.107.1` API parity, CanaryOnly, AdditiveBatch1, gameplay, save-load, replacement, multiplayer, handoff, or release-ready proof'
Add-ContainsCheck -Name 'docs_readme_sts1_entry' -Text $docsReadme -Needle 'StS1 event prototype'
Add-ContainsCheck -Name 'docs_readme_sts1_gate_ledger' -Text $docsReadme -Needle 'features/sts1-events/v19-gate-ledger.csv`, `features/sts1-events/v20-final-gate-overlay.csv`, `features/sts1-events/hard-stop-blocker-report-v20-coordination-pause-20260617.md`, and `features/sts1-events/v19-subagent-coverage.md` for current O0-O76 gate split, O76-O84 final documentation/handoff overlay, current v20 hard-stop/next-run point, and subagent status'
Add-ContainsCheck -Name 'docs_readme_sts1_subagent_coverage' -Text $docsReadme -Needle 'features/sts1-events/v19-subagent-coverage.md'
Add-ContainsCheck -Name 'docs_readme_sts1_beta96_settings_boundary' -Text $docsReadme -Needle 'beta.96 settings-page proof covers previous-package RitsuLib UI visibility only'
Add-ContainsCheck -Name 'docs_readme_sts1_additive_pending_counts' -Text $docsReadme -Needle 'previous beta.93 AdditiveBatch1 proof covers previous-package loader/registration only with 10 event types / 14 registration calls'
Add-ContainsCheck -Name 'root_test_plan_beta88_additive_path' -Text $rootTestPlan -Needle '.tools/runtime-evidence/v01071-beta93-ritsulib0431-additivebatch1-direct-20260621'
Add-ContainsCheck -Name 'root_test_plan_historical_enabled_modes_only' -Text $rootTestPlan -Needle 'historical RitsuLib diagnostic loader gates exist for Off, CanaryOnly, and AdditiveBatch1 modes'
Add-ContainsCheck -Name 'root_test_plan_off_canary_loader_only' -Text $rootTestPlan -Needle 'The beta.85 Off/CanaryOnly smokes remain older dependency/game-version context,'
Add-ContainsCheck -Name 'root_test_plan_additive_still_requires_clean_packet' -Text $rootTestPlan -Needle 'Installed beta.97 package parity is the current package evidence.'
Add-ContainsCheck -Name 'root_test_plan_manual_ritsulib_only' -Text $rootTestPlan -Needle 'Confirm the enabled mod set for this lane contains only `STS2-RitsuLib`'
Add-Check -Name 'root_test_plan_no_previous_package_setup' -Passed (-not ($rootTestPlan -match '(?i)previous[- ]package')) -Detail 'root test plan must not instruct current testers to enable or expect the old dependency package'
Add-ContainsCheck -Name 'migration_doc_compact_current_index' -Text $migrationDoc -Needle 'This file is the compact active index for migration status.'
Add-ContainsCheck -Name 'migration_doc_routes_to_ritsu_integration' -Text $migrationDoc -Needle '`docs/integrations/ritsulib.md` for the dependency, installed runtime, API,'
Add-ContainsCheck -Name 'migration_doc_beta96_boundary' -Text $migrationDoc -Needle 'Spire Plus is on the beta.97 RitsuLib-only target:'
Add-ContainsCheck -Name 'migration_doc_beta96_off_loader_only' -Text $migrationDoc -Needle 'Previous beta.96 RitsuLib-only Off proof is startup/loading evidence only.'
Add-ContainsCheck -Name 'migration_doc_remaining_runtime_blockers' -Text $migrationDoc -Needle 'Gameplay, event screenshots, save-load, image/render, replacement functional'
Add-ContainsCheck -Name 'goal_migration_current_github_scope' -Text $goalMigrationDoc -Needle 'GitHub `main`'
Add-ContainsCheck -Name 'goal_migration_beta96_off_loader_pass' -Text $goalMigrationDoc -Needle 'Previous beta.96 Off proof under `.tools/runtime-evidence/v01071-beta96-ritsulib0431-off-direct-20260621-185056/`'
Add-ContainsCheck -Name 'goal_migration_beta85_ectoplasm_root_cause' -Text $goalMigrationDoc -Needle 'Spire Plus is a RitsuLib-only mod.'
Add-ContainsCheck -Name 'goal_migration_beta85_package_runtime_clean' -Text $goalMigrationDoc -Needle '`EZMicroBalance.json` declares only `STS2-RitsuLib >= 0.4.31`'
Add-ContainsCheck -Name 'goal_migration_beta87_current_override' -Text $goalMigrationDoc -Needle 'Previous beta.93 AdditiveBatch1 proof under `.tools/runtime-evidence/v01071-beta93-ritsulib0431-additivebatch1-direct-20260621/` reached main menu'
Add-ContainsCheck -Name 'goal_debug_beta85_off_loader_nonclaim' -Text $goalDebugDoc -Needle 'loader proof'
Add-ContainsCheck -Name 'goal_debug_beta87_current_override' -Text $goalDebugDoc -Needle 'Beta.85/beta.86/beta.87 loader proof remains previous-package/game-version context, and beta.88/beta.93/beta.96 proof remains previous-package context.'
Add-ContainsCheck -Name 'goal_refactor_current_static_governance_note' -Text $goalRefactorDoc -Needle 'source/static/no-game'
Add-ContainsCheck -Name 'goal_refactor_beta85_off_nonclaim' -Text $goalRefactorDoc -Needle 'CanaryOnly'
Add-ContainsCheck -Name 'goal_refactor_current_canary_4_6' -Text $goalRefactorDoc -Needle '4 event types / 6 registration calls'
Add-ContainsCheck -Name 'goal_refactor_current_batch1_10_14' -Text $goalRefactorDoc -Needle '10 event types / 14 registration calls'
Add-ContainsCheck -Name 'goal_refactor_direct_off_default_only' -Text $goalRefactorDoc -Needle 'claim enabled-mode safe'
Add-ContainsCheck -Name 'goal_refactor_direct_canary_current_4_6' -Text $goalRefactorDoc -Needle 'CanaryOnly beta.85 / v0.107.0 smoke'
Add-ContainsCheck -Name 'goal_refactor_beta87_current_override' -Text $goalRefactorDoc -Needle 'Beta.85/beta.86/beta.87 loader proof remains previous-package/game-version context, beta.88 remains previous-package context, beta.93 AdditiveBatch1 is previous-package RitsuLib-only loader/registration proof, and beta.96 Off is previous-package RitsuLib-only loader proof after the beta.97 settings I18N package bump.'
Add-ContainsCheck -Name 'goal_refactor_direct_final_priority' -Text $goalRefactorDoc -Needle 'current enabled-mode proof'
Add-Check -Name 'goal_refactor_no_old_exact_canary_registration_claim' -Passed (-not $goalRefactorDoc.Contains('exactly 4 canary registrations')) -Detail 'goal refactor current instructions must use event type / registration-call counts'
Add-ContainsCheck -Name 'goal_event_beta96_current_loader_boundary' -Text $goalEventDoc -Needle 'As of 2026-06-21, current package truth is beta.97 on Slay the Spire 2 `v0.107.1` with STS2-RitsuLib `v0.4.31`. Beta.97 Off loader and clicked settings UI proof remain pending after the RitsuLib settings-page I18N resource migration.'
Add-ContainsCheck -Name 'goal_event_beta88_loader_nonclaim' -Text $goalEventDoc -Needle 'This beta.88 packet is loader/registration proof only. It still does not close event gameplay, clicked UI, save-load, EN/ZHS runtime render, image/license/render, replacement functional behavior, multiplayer/fail-closed, independent QA, game-native AutoSlay batch proof, release, or tester handoff gates.'
Add-ContainsCheck -Name 'goal_event_priority_canary_current_4_6' -Text $goalEventDoc -Needle 'preserve retained beta.85 CanaryOnly loader proof as previous-package/game-version context'
Add-ContainsCheck -Name 'goal_event_go_nogo_canary_current_4_6' -Text $goalEventDoc -Needle '4 event types / 6 registration calls.'
Add-ContainsCheck -Name 'goal_event_direct_canary_current_4_6' -Text $goalEventDoc -Needle 'beta.85 CanaryOnly loader proof'
Add-ContainsCheck -Name 'goal_event_coordination_pause_boundary' -Text $goalEventDoc -Needle 'While the same-repository migration validation lane is active, this event goal must not start new `dotnet build`, `dotnet test`, `dotnet publish`, package/release-evidence validation, game/runtime smoke, staging, commit, or push processes from this thread.'
Add-ContainsCheck -Name 'goal_event_pause_static_work_boundary' -Text $goalEventDoc -Needle 'Allowed work during the pause is read-only/static checking, documentation/guard alignment, and no-resource/no-code governance cleanup that does not require build, publish, package, or version-bump validation.'
Add-RegexCheck -Name 'goal_event_downstream_pause_boundary' -Text $goalEventDoc -Pattern 'Runtime, gameplay, QA, build/test/publish'
Add-RegexCheck -Name 'goal_event_direct_instruction_after_pause' -Text $goalEventDoc -Pattern 'coordination pause.{0,40}Mandatory Overnight Run v20'
Add-ContainsCheck -Name 'goal_event_latest_beta94_governance_checkpoint' -Text $goalEventDoc -Needle 'Latest beta.97 governance checkpoint after the RitsuLib settings-page I18N resource migration: static/source docs must be recaptured for the current package before any handoff.'
Add-ContainsCheck -Name 'goal_event_validation_matrix_current_doc_claims_1331' -Text $goalEventDoc -Needle 'current-doc-claims: 1331 checks / 0 mismatches'
Add-ContainsCheck -Name 'goal_event_validation_matrix_v20_overlay_29' -Text $goalEventDoc -Needle 'v20 final-gate overlay: 29 checks / 0 mismatches'
Add-ContainsCheck -Name 'goal_event_validation_matrix_runtime_preflight_27' -Text $goalEventDoc -Needle 'runtime-preflight: 27 checks / 0 mismatches (local v0.107.1 / beta.93 target; read-only source/prereq only)'
Add-ContainsCheck -Name 'goal_event_validation_matrix_subagent_70' -Text $goalEventDoc -Needle 'v19 subagent coverage: 70 checks / 0 mismatches'
Add-ContainsCheck -Name 'goal_event_validation_matrix_diff_check_generic_crlf' -Text $goalEventDoc -Needle 'git diff --check: exit 0 with CRLF normalization warnings only; no whitespace errors'
Add-ContainsCheck -Name 'goal_event_source_workspace_checkpoint_beta93' -Text $goalEventDoc -Needle 'source workspace: 60 checks / 0 mismatches against installed v0.107.1, Spire Plus v0.1.0-private-beta.97, and STS2-RitsuLib 0.4.31 / compat 0.107.1'
Add-ContainsCheck -Name 'goal_event_autoslay_batch_still_open' -Text $goalEventDoc -Needle 'does not itself close gameplay, save/load, replacement, multiplayer, QA, game-native AutoSlay batch proof, release, or handoff gates'
Add-ContainsCheck -Name 'goal_event_autoslay_expected_ancient_ids_followup' -Text $goalEventDoc -Needle 'Latest pause-safe AutoSlay target-coverage follow-up'
Add-ContainsCheck -Name 'goal_event_autoslay_expected_ancient_ids_nonclaim' -Text $goalEventDoc -Needle 'This improves future game-native monkey proof quality but remains static/verifier evidence only'
Add-ContainsCheck -Name 'goal_event_autoslay_expected_ancient_ids_plan_match' -Text $goalEventDoc -Needle 'plan_expected_ancient_ids_match_parameter'
Add-ContainsCheck -Name 'goal_event_autoslay_expected_ancient_ids_required_for_proof_mode' -Text $goalEventDoc -Needle 'expected_ancient_ids_required_for_proof_mode'
Add-ContainsCheck -Name 'goal_event_autoslay_ancient_id_normalization_proof_mode_followup' -Text $goalEventDoc -Needle 'Latest pause-safe AutoSlay AncientId normalization/proof-mode/summary-count follow-up'
Add-RegexCheck -Name 'goal_event_lower_audit_current_doc_claims_1331' -Text $goalEventDoc -Pattern '\| Current doc claims\s+\|[^\r\n]*1331 checks / 0 mismatches'
Add-ContainsCheck -Name 'goal_event_direct_localization_nonproof' -Text $goalEventDoc -Needle 'Fixing `STS1_GOLDEN_IDOL.pages.LEAVE.description` only removes the direct localization missing-key blocker'
Add-ContainsCheck -Name 'goal_event_canary_loader_current_pass_section' -Text $goalEventDoc -Needle 'Retained beta.85 CanaryOnly loader registration proof remains previous-package/game-version loader context for `O25` and loader-packet `O39`; recapture current CanaryOnly before broader current-runtime claims.'
Add-ContainsCheck -Name 'goal_event_beta88_additive_loader_current_pass_section' -Text $goalEventDoc -Needle 'Previous beta.93 RitsuLib-only AdditiveBatch1 loader registration proof can be treated as current-pass for `O33`.'
Add-ContainsCheck -Name 'goal_event_off_canary_loader_nonextension_boundary' -Text $goalEventDoc -Needle 'Retained beta.85 Off, retained beta.85 CanaryOnly, retained beta.87 AdditiveBatch1, previous package beta.88 AdditiveBatch1, and previous beta.93 AdditiveBatch1 loader proof must not be extended to:'
Add-Check -Name 'goal_event_no_stale_canary_enabled_pending_claim' -Passed (-not $goalEventDoc.Contains('CanaryOnly exact enabled-mode proof')) -Detail 'active event goal must not list CanaryOnly exact enabled-mode proof as pending after beta.85 CanaryOnly verifier packet'
Add-Check -Name 'goal_event_no_old_exact_4_claim' -Passed (-not $goalEventDoc.Contains('exactly 4 canary registrations')) -Detail 'active event goal must use 4 event types / 6 registration calls wording'
Add-ContainsCheck -Name 'test_ready_goal_sts1_enabled_smokes_boundary' -Text $testReadyGoal -Needle 'The beta.85 Off/CanaryOnly, beta.86 AdditiveBatch1, beta.87 AdditiveBatch1 `v0.107.0`, beta.88 previous package `v0.107.1`, and beta.90 RitsuLib-only packets remain previous-package or previous package-context loader evidence.'
Add-ContainsCheck -Name 'test_ready_goal_direct_localization_nonproof' -Text $testReadyGoal -Needle 'Closing only the direct Golden Idol localization key remains a localization unblocker; it does not prove gameplay or replace verifier reports.'
Add-ContainsCheck -Name 'test_ready_goal_beta94_preflight_drift' -Text $testReadyGoal -Needle 'The current retained package target is `publish/SpirePlus-v0.1.0-private-beta.97.zip`; build, publish, package refresh, installed-package check, and source-workspace check have run for the settings-page I18N resource migration.'
Add-ContainsCheck -Name 'dev_environment_sts1_enabled_smokes_current_split' -Text $devEnvironment -Needle 'beta.93 RitsuLib-only AdditiveBatch1 proof is clean previous-package context under `.tools\runtime-evidence\v01071-beta93-ritsulib0431-additivebatch1-direct-20260621`, with 10 event types / 14 registration calls, enabled-mode verifier 31 / 0, and packet verifier 61 / 0.'
Add-ContainsCheck -Name 'root_issues_sts1_enabled_smokes_boundary' -Text $rootIssues -Needle 'Previous beta.96 RitsuLib-only Off loader proof exists at `.tools/runtime-evidence/v01071-beta96-ritsulib0431-off-direct-20260621-185056/`.'
Add-ContainsCheck -Name 'active_review_sts1_enabled_smokes_boundary' -Text $activeReview -Needle '2026-06-17 enabled-mode runtime split: fresh beta.85 CanaryOnly proof under `.tools/runtime-evidence/v01070-beta85-canary-20260617-233621/` reached main menu'
Add-ContainsCheck -Name 'active_review_direct_localization_nonproof' -Text $activeReview -Needle 'Closing only the direct Golden Idol localization key remains a localization unblocker; it does not prove gameplay or replace verifier reports.'
Add-ContainsCheck -Name 'toreview_sts1_enabled_smokes_current_split' -Text $toReview -Needle 'beta.97 build, publish/package refresh, installed-package parity, and source-workspace check are the current package target.'
Add-ContainsCheck -Name 'toreview_direct_localization_nonproof' -Text $toReview -Needle 'Closing only the direct Golden Idol localization key remains a localization unblocker; it does not prove gameplay or replace future gameplay verifier reports.'
Add-ContainsCheck -Name 'legacy_v5_monthly_spec_current_override' -Text $legacyV5MonthlySpec -Needle 'This v5 audit/spec is historical planning context only.'
Add-ContainsCheck -Name 'legacy_v5_monthly_spec_not_current_event_guidance' -Text $legacyV5MonthlySpec -Needle 'Do not use its O0-O12 overnight gates, old registration assumptions, or old task'
Add-ContainsCheck -Name 'legacy_v5_monthly_spec_v19_pointer' -Text $legacyV5MonthlySpec -Needle 'Current StS1 event work routes through `docs/goals/event.md`'
Add-ContainsCheck -Name 'legacy_v5_monthly_spec_beta93_additive_nonclaim' -Text $legacyV5MonthlySpec -Needle 'AdditiveBatch1 loader/registration with STS2-RitsuLib `0.4.31`'
Add-ContainsCheck -Name 'historical_overnight_review_revision_m_note' -Text $historicalOvernightRunReview -Needle 'This 2026-05-29 review is historical no-game/source-governance context only.'
Add-ContainsCheck -Name 'historical_overnight_review_no_current_completion' -Text $historicalOvernightRunReview -Needle 'Do not use its `GREEN STOP`, `DONE`, `PASS`, `CanaryOnly = exactly 4`, warning count, test count, or Pack status as current `event.md` completion or runtime proof.'
Add-ContainsCheck -Name 'historical_refactor_qa_revision_m_note' -Text $historicalRefactorQaReview -Needle 'This 2026-06-02 QA report is historical `v0.106.1` loader-gate context only.'
Add-ContainsCheck -Name 'historical_refactor_qa_no_current_proof' -Text $historicalRefactorQaReview -Needle 'Do not use its `CONDITIONAL PASS`, Off/CanaryOnly/AdditiveBatch1 `PASS`, RitsuLib `v0.3.10`, beta.84, warning-count, dirty-worktree, or package status as current `event.md` proof.'
Add-ContainsCheck -Name 'historical_refactor_qa_round2_revision_m_note' -Text $historicalRefactorQaRound2Review -Needle 'This 2026-06-02 Round 2 QA report is historical `v0.106.1` loader-gate context only.'
Add-ContainsCheck -Name 'historical_refactor_qa_round2_no_current_proof' -Text $historicalRefactorQaRound2Review -Needle 'Do not use its `CONDITIONAL PASS`, Off/CanaryOnly/AdditiveBatch1 `PASS`, RitsuLib `v0.3.10`, beta.84, warning-count, dirty-worktree, mod-isolation, or package status as current `event.md` proof.'
Add-ContainsCheck -Name 'ritsu_integration_beta96_off_nonclaim' -Text $ritsuIntegrationDoc -Needle 'Previous beta.96 direct Off proof at `.tools/runtime-evidence/v01071-beta96-ritsulib0431-off-direct-20260621-185056/` reached main menu'
Add-ContainsCheck -Name 'ritsu_integration_batch4c_proposal_only' -Text $ritsuIntegrationDoc -Needle 'Batch 4c: proposal-only candidate list'
Add-ContainsCheck -Name 'ritsu_monthly_current_off_nonclaim' -Text $ritsuMonthlyDevSpec -Needle 'Beta.97 Off loader smoke remains pending after the settings-page I18N resource migration; previous beta.96 Off loader smoke is clean previous-package startup/loading context only.'
Add-ContainsCheck -Name 'ritsu_monthly_no_advance_from_off_alone' -Text $ritsuMonthlyDevSpec -Needle 'do not advance to CanaryOnly, AdditiveBatch1, replacement, or gameplay proof from the Off smoke alone.'
Add-ContainsCheck -Name 'ritsu_batch4c_not_approval' -Text $ritsuBatch4cCandidates -Needle 'This list is not a migration approval.'
Add-ContainsCheck -Name 'ritsu_batch4c_release_blocked' -Text $ritsuBatch4cCandidates -Needle 'Release-ready remains blocked by gameplay, screenshot, save-load, image/render, replacement, multiplayer, independent QA, and tester-package handoff evidence.'
Add-ContainsCheck -Name 'ritsu_runtime_hard_block_current_note' -Text $ritsuRuntimeHardBlock -Needle '# RitsuLib Runtime Boundary Report'
Add-ContainsCheck -Name 'ritsu_runtime_hard_block_beta96_target' -Text $ritsuRuntimeHardBlock -Needle 'Spire Plus `v0.1.0-private-beta.97`'
Add-ContainsCheck -Name 'ritsu_runtime_hard_block_settings_ui_path' -Text $ritsuRuntimeHardBlock -Needle '.tools/runtime-evidence/beta96-ritsulib-mod-settings-clicked-ui-20260621-160701/'
Add-ContainsCheck -Name 'ritsu_runtime_hard_block_current_enabled_pending' -Text $ritsuRuntimeHardBlock -Needle 'Current hard blocks are gameplay, clicked'
Add-ContainsCheck -Name 'ritsu_runtime_hard_block_manual_gates_pending' -Text $ritsuRuntimeHardBlock -Needle 'current beta.97 enabled-mode'
Add-ContainsCheck -Name 'ritsu_runtime_hard_block_coordination_boundary' -Text $ritsuRuntimeHardBlock -Needle 'After coordination clears, capture current enabled-mode'
Add-ContainsCheck -Name 'm5_l_runtime_hard_blocker_batch1_current_count' -Text $m5RevisionLRuntimeHardBlocker -Needle 'historical log used the then-current 10 event types / 11 registration-call shape; current source expects 10 event types / 14 calls'
Add-ContainsCheck -Name 'm5_l_runtime_hard_blocker_historical_nonclaim' -Text $m5RevisionLRuntimeHardBlocker -Needle 'Do not use the historical `v0.106.1` loader smokes or the red beta.84 smoke as current runtime proof.'
Add-ContainsCheck -Name 'm5_l_runtime_smoke_plan_batch1_current_count' -Text $m5RevisionLRuntimeSmokePlan -Needle 'current source expects 10 event types / 14 calls'
Add-ContainsCheck -Name 'm5_l_runtime_smoke_plan_gameplay_nonclaim' -Text $m5RevisionLRuntimeSmokePlan -Needle 'Runtime smoke does not prove gameplay.'
Add-ContainsCheck -Name 'm5_l_final_report_revision_m_supersession' -Text $m5RevisionLFinalReport -Needle 'Revision M supersession note, 2026-06-11: this report''s beta.84 package/runtime boundary is historical.'
Add-ContainsCheck -Name 'm5_l_final_report_beta85_off_only' -Text $m5RevisionLFinalReport -Needle 'Current beta.85 has clean `v0.107.0` default-Off loader proof only'
Add-ContainsCheck -Name 'm5_l_owner_packet_revision_m_supersession' -Text $m5RevisionLOwnerPacket -Needle 'Revision M supersession note, 2026-06-11: this packet''s beta.84 package/runtime boundary is historical.'
Add-ContainsCheck -Name 'm5_l_owner_packet_enabled_pending' -Text $m5RevisionLOwnerPacket -Needle 'current CanaryOnly, AdditiveBatch1, gameplay, save-load, replacement, multiplayer, QA, and release-ready proof remain pending.'
Add-ContainsCheck -Name 'm5_l_dirty_ledger_revision_m_supersession' -Text $m5RevisionLDirtyLedger -Needle 'Revision M supersession note, 2026-06-11: this dirty-ledger scope is historical owner-review context.'
Add-ContainsCheck -Name 'm5_l_dirty_ledger_beta85_off_only' -Text $m5RevisionLDirtyLedger -Needle 'Current beta.85 has clean `v0.107.0` default-Off loader proof only'
Add-ContainsCheck -Name 'm5_l_commit_slices_revision_m_supersession' -Text $m5RevisionLCommitSlices -Needle 'Revision M supersession note, 2026-06-11: this commit-slice plan is historical owner-review context.'
Add-ContainsCheck -Name 'm5_l_commit_slices_current_pointer' -Text $m5RevisionLCommitSlices -Needle 'Use `docs/goals/m5-revision-m-commit-slices.md`, `PROJECT_STATE.md`, and the Revision M docs for current proof claims.'
Add-ContainsCheck -Name 'm5_l_warning_ledger_revision_m_supersession' -Text $m5RevisionLWarningLedger -Needle 'Revision M supersession note, 2026-06-11: this warning ledger is historical owner-review context for the Revision L burn-down.'
Add-ContainsCheck -Name 'm5_l_warning_ledger_no_runtime_proof' -Text $m5RevisionLWarningLedger -Needle 'warning-clean source is not enabled-mode, gameplay, save-load, replacement, multiplayer, QA, or release-ready proof.'
Add-ContainsCheck -Name 'm5_final_report_not_release_ready' -Text $m5RevisionMFinalReport -Needle 'Status: Complete for Off loader runtime-drift closure; not live-ready or release-ready.'
Add-ContainsCheck -Name 'm5_final_report_no_overlapping_validation' -Text $m5RevisionMFinalReport -Needle 'do not start overlapping validation lanes.'
Add-ContainsCheck -Name 'm5_owner_packet_loader_package_only' -Text $m5RevisionMOwnerPacket -Needle 'Accept as loader-smoke package, not gameplay/release proof'
Add-ContainsCheck -Name 'm5_owner_packet_canary_after_coordination' -Text $m5RevisionMOwnerPacket -Needle 'May run only after process coordination'
Add-ContainsCheck -Name 'm5_runtime_drift_beta87_additive_path' -Text $m5RevisionMRuntimeDriftReport -Needle 'Retained beta.87 AdditiveBatch1 proof is `.tools/runtime-evidence/v01070-beta87-additive-batch1-direct-20260618-152531/`'
Add-ContainsCheck -Name 'm5_runtime_drift_enabled_modes_pending' -Text $m5RevisionMRuntimeDriftReport -Needle 'Fresh beta.87 default-Off and CanaryOnly smokes are not recorded; retained beta.85/beta.86 evidence is previous-package loader context.'
Add-ContainsCheck -Name 'm5_runtime_drift_stop_line' -Text $m5RevisionMRuntimeDriftReport -Needle 'Do not extend that to runtime-ready, live-ready, or release-ready'
Add-ContainsCheck -Name 'm5_patch_failure_current_package_proof' -Text $m5RevisionMPatchFailureLedger -Needle '.tools/runtime-evidence/v01070-beta87-additive-batch1-direct-20260618-152531/godot.log.after-launch` was the then-current beta.87 package proof'
Add-ContainsCheck -Name 'm5_patch_failure_enabled_modes_next_only' -Text $m5RevisionMPatchFailureLedger -Needle 'Required next proof is no longer loader smoke; run gameplay, save-load, render, replacement, multiplayer, and QA checks only if process coordination is clear.'
Add-ContainsCheck -Name 'm5_version_decision_nonclaim' -Text $m5RevisionMVersionDecision -Needle 'Do not use beta.85 version docs as gameplay, live-ready, or release-ready proof.'
Add-ContainsCheck -Name 'm5_commit_slices_no_commit_boundary' -Text $m5RevisionMCommitSlices -Needle 'Status: planning only; no commit or push authorized from this paused validation lane.'
Add-ContainsCheck -Name 'm5_commit_slices_sts1_default_off' -Text $m5RevisionMCommitSlices -Needle 'This should remain default-Off/staged until runtime event proof exists.'
Add-ContainsCheck -Name 'goal_warning_ledger_revision_m_note' -Text $goalWarningLedger -Needle 'Revision M note, 2026-06-11: beta.85 validation keeps the nullable warning blocker closed'
Add-ContainsCheck -Name 'goal_warning_ledger_no_runtime_proof' -Text $goalWarningLedger -Needle 'Do not use 0 warnings as current CanaryOnly, AdditiveBatch1, gameplay, save-load, replacement, multiplayer, QA, or release-ready proof.'
Add-ContainsCheck -Name 'overnight_run_ledger_revision_m_supersession' -Text $overnightRunLedger -Needle 'Revision M supersession note, 2026-06-11: this ledger is historical Revision L owner-review context.'
Add-ContainsCheck -Name 'overnight_run_ledger_beta85_off_only' -Text $overnightRunLedger -Needle 'Current beta.85 has clean `v0.107.0` default-Off loader proof only'
Add-ContainsCheck -Name 'overnight_run_status_revision_m_supersession' -Text $overnightRunStatus -Needle 'Revision M supersession note, 2026-06-11: this status is historical Revision L owner-review context.'
Add-ContainsCheck -Name 'overnight_run_status_current_pending' -Text $overnightRunStatus -Needle 'current CanaryOnly, AdditiveBatch1, gameplay, save-load, replacement, multiplayer, QA, clean-worktree, and release-ready proof remain pending.'
Add-ContainsCheck -Name 'scripts_readme_sts1_static_suite' -Text $scriptsReadme -Needle 'check-sts1-event-static-suite.ps1'
Add-ContainsCheck -Name 'scripts_readme_static_suite_enabled_log_expected_shape' -Text $scriptsReadme -Needle 'enabled-log expected-shape'
Add-ContainsCheck -Name 'scripts_readme_static_suite_file_hygiene' -Text $scriptsReadme -Needle 'static-file hygiene'
Add-ContainsCheck -Name 'scripts_readme_static_suite_subagent_coverage' -Text $scriptsReadme -Needle 'v19 subagent coverage'
Add-ContainsCheck -Name 'scripts_readme_static_suite_v20_overlay' -Text $scriptsReadme -Needle 'v20 final-gate overlay'
Add-ContainsCheck -Name 'scripts_readme_v20_overlay_checker' -Text $scriptsReadme -Needle 'v20-final-gate-overlay.csv` O76-O84 overlay, including final documentation, owner-action, no-unsupported-commit/push, release-claim, final-summary, and next-run boundaries'
Add-ContainsCheck -Name 'scripts_readme_v20_subagent_coverage_shape' -Text $scriptsReadme -Needle '15-role v20 `docs/goals/event.md` subagent coverage shape while retaining the v19 filename'
Add-ContainsCheck -Name 'scripts_readme_sts1_static_file_hygiene' -Text $scriptsReadme -Needle 'check-sts1-static-file-hygiene.ps1'
Add-ContainsCheck -Name 'scripts_readme_sts1_gate_ledger' -Text $scriptsReadme -Needle 'check-sts1-v19-gate-ledger.ps1'
Add-ContainsCheck -Name 'scripts_readme_sts1_subagent_coverage' -Text $scriptsReadme -Needle 'check-sts1-v19-subagent-coverage.ps1'
Add-ContainsCheck -Name 'scripts_readme_sts1_runtime_packet' -Text $scriptsReadme -Needle 'check-sts1-runtime-evidence-packet.ps1'
Add-ContainsCheck -Name 'scripts_readme_sts1_enabled_log' -Text $scriptsReadme -Needle 'check-sts1-enabled-mode-runtime-log.ps1'
Add-ContainsCheck -Name 'scripts_readme_sts1_runtime_preflight' -Text $scriptsReadme -Needle 'check-sts1-runtime-preflight.ps1'
Add-ContainsCheck -Name 'scripts_readme_sts1_no_launch_boundary' -Text $scriptsReadme -Needle 'without launching the game or running `dotnet`'
Add-ContainsCheck -Name 'scripts_readme_runtime_preflight_no_launch_boundary' -Text $scriptsReadme -Needle 'Optional no-launch StS1 runtime preflight for machines with the game installed'
Add-ContainsCheck -Name 'scripts_readme_runtime_preflight_source_shape_boundary' -Text $scriptsReadme -Needle 'source-only CanaryOnly/AdditiveBatch1 expected shapes'
Add-ContainsCheck -Name 'scripts_readme_runtime_packet_mode_metadata' -Text $scriptsReadme -Needle 'StS1 mode environment metadata for enabled-mode packets'
Add-ContainsCheck -Name 'scripts_readme_runtime_packet_game_version' -Text $scriptsReadme -Needle 'game release version'
Add-ContainsCheck -Name 'scripts_readme_enabled_log_audit_required' -Text $scriptsReadme -Needle 'the audit path is required for enabled-mode copied-log proof'
Add-ContainsCheck -Name 'scripts_readme_enabled_log_outfile_report' -Text $scriptsReadme -Needle 'pass `-OutFile` to keep the verifier report in the evidence folder'
Add-ContainsCheck -Name 'scripts_readme_enabled_log_fail_on_mismatch' -Text $scriptsReadme -Needle '`-FailOnMismatch` so evidence-command mismatches fail'
Add-ContainsCheck -Name 'scripts_readme_runtime_packet_outfile_report' -Text $scriptsReadme -Needle 'pass `-OutFile` to keep the packet verifier report in the evidence folder'
Add-ContainsCheck -Name 'scripts_readme_runtime_packet_fail_on_mismatch' -Text $scriptsReadme -Needle '`-FailOnMismatch` so evidence-command mismatches fail'
Add-ContainsCheck -Name 'scripts_readme_autoslay_packet_verifier' -Text $scriptsReadme -Needle 'check-spire-plus-autoslay-packet.ps1'
Add-ContainsCheck -Name 'scripts_readme_autoslay_packet_native_runner' -Text $scriptsReadme -Needle '`GameNativeAutoSlay` runner identity'
Add-ContainsCheck -Name 'scripts_readme_autoslay_packet_no_launch_boundary' -Text $scriptsReadme -Needle 'it does not launch the game or make source-only AutoSlay checks runtime proof'
Add-ContainsCheck -Name 'scripts_readme_autoslay_packet_requires_launcher_proof' -Text $scriptsReadme -Needle 'hashed launcher/mod-hook provenance for `AutoSlayer.Start(seed, logFile)`'
Add-ContainsCheck -Name 'scripts_readme_autoslay_packet_requires_run_result' -Text $scriptsReadme -Needle 'per-seed `run-result.json`'
Add-ContainsCheck -Name 'scripts_readme_autoslay_packet_requires_passed_ancient_state' -Text $scriptsReadme -Needle '`Passed=true`, empty failure/hang arrays, `EventKind: Ancient`, `AncientId`'
Add-ContainsCheck -Name 'scripts_readme_autoslay_packet_requires_timestamp_order' -Text $scriptsReadme -Needle 'parseable ordered start/end timestamps'
Add-ContainsCheck -Name 'scripts_readme_autoslay_packet_requires_runtime_log_growth' -Text $scriptsReadme -Needle 'runtime `LogGrew=true`, runtime log initial/final length growth, and main-menu/runtime no-log-growth timeout rejection'
Add-ContainsCheck -Name 'scripts_readme_autoslay_packet_requires_probe_phases' -Text $scriptsReadme -Needle 'both `main-menu` and `runtime` phases, runtime sample log-length growth beyond `RuntimeObservation.LogInitialLengthBytes`, plus `run-result.json` ProcessId binding'
Add-ContainsCheck -Name 'scripts_readme_autoslay_packet_requires_expected_ancient_ids' -Text $scriptsReadme -Needle 'required proof-mode `-ExpectedAncientIds` plan/summary/traversed target coverage'
Add-ContainsCheck -Name 'scripts_readme_runtime_packet_requires_runtime_log_growth' -Text $scriptsReadme -Needle 'no main-menu/runtime log-growth timeout, command-bearing runtime `LogGrew=true` with `RuntimeLogGrowthRequired`'
Add-ContainsCheck -Name 'scripts_readme_runtime_packet_rejects_iteration_escape_paths' -Text $scriptsReadme -Needle 'result log/probe paths resolve inside the current `iteration-####` directory and match the retained files'
Add-ContainsCheck -Name 'scripts_readme_analyzer_reports_runtime_probe_log_growth_mismatch' -Text $scriptsReadme -Needle 'runtime `LogGrew` versus retained sample `LogLengthBytes` drift'
Add-ContainsCheck -Name 'scripts_readme_analyzer_requires_runtime_monkey_iteration_local_paths' -Text $scriptsReadme -Needle 'runtime monkey result log/probe paths must resolve inside the per-iteration directory'
Add-ContainsCheck -Name 'scripts_readme_analyzer_requires_runtime_monkey_retained_standard_files' -Text $scriptsReadme -Needle 'runtime monkey result log/probe paths must resolve inside the per-iteration directory and match the retained standard files'
Add-ContainsCheck -Name 'scripts_readme_analyzer_direct_smoke_root_target' -Text $scriptsReadme -Needle 'failed direct smoke evidence root with `direct-smoke-summary.json`'
Add-ContainsCheck -Name 'scripts_readme_analyzer_direct_smoke_externalmod_drift' -Text $scriptsReadme -Needle 'Bound previous package dirty-audit signatures such as `dependency patch failure` and `[ERROR] [previous package]` route to `PackageRuntimeDrift`'
Add-ContainsCheck -Name 'scripts_readme_analyzer_direct_smoke_externalmod_patch_details' -Text $scriptsReadme -Needle '`dependency patch failures` details such as `AdjustCustomMessageKeys::Fuckery()` undefined target-method failures'
Add-ContainsCheck -Name 'scripts_readme_analyzer_direct_smoke_externalmod_relic_collection_detail' -Text $scriptsReadme -Needle '`NRelicCollectionCategory::LoadRelics` instruction matcher failures'
Add-ContainsCheck -Name 'scripts_readme_analyzer_direct_smoke_no_coop_false_positive' -Text $scriptsReadme -Needle 'Explanatory text that merely mentions `SPIREPLUS_ALLOW_UNVERIFIED_COOP_*` does not by itself create a co-op override blocker'
Add-ContainsCheck -Name 'scripts_readme_autoslay_packet_requires_current_slice_binding' -Text $scriptsReadme -Needle '`godot.log.before`, `godot.log.after-launch`, byte-sliced `godot.log.current-iteration`'
Add-ContainsCheck -Name 'scripts_readme_autoslay_packet_requires_audit_recomputation' -Text $scriptsReadme -Needle '`godot-log-audit.json` binding/recomputation'
Add-ContainsCheck -Name 'scripts_readme_autoslay_packet_requires_sts1_mode_binding' -Text $scriptsReadme -Needle '`sts1-mode-log-check.json` path/length/hash binding'
Add-ContainsCheck -Name 'scripts_readme_autoslay_packet_requires_expected_patch_count' -Text $scriptsReadme -Needle '`-ExpectedPatchCount`'
Add-ContainsCheck -Name 'scripts_readme_autoslay_packet_requires_ordered_dual_log_event_sequence' -Text $scriptsReadme -Needle 'ordered Ancient event-room traversal markers such as `Entering Event room`, `Detected Ancient event, clicking through dialogue`, and `Selecting event option: <AncientId>` in both the sidecar and current-iteration logs'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_packet_verifier' -Text $runtimeMonkeyDocs -Needle '.\scripts\check-spire-plus-autoslay-packet.ps1'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_rejects_non_native' -Text $runtimeMonkeyDocs -Needle 'rejects packets that do not identify'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_requires_event_markers' -Text $runtimeMonkeyDocs -Needle '`Entering Event room`, `Detected Ancient event, clicking through dialogue`'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_requires_launcher_proof' -Text $runtimeMonkeyDocs -Needle 'structured launcher/mod-hook provenance for'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_requires_ancient_id' -Text $runtimeMonkeyDocs -Needle '`EventKind: Ancient`'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_requires_timestamp_order' -Text $runtimeMonkeyDocs -Needle 'ordered run-result timestamps'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_requires_runtime_log_growth' -Text $runtimeMonkeyDocs -Needle 'clean `RuntimeObservation` with `LogGrew: true`'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_requires_probe_phases' -Text $runtimeMonkeyDocs -Needle '`main-menu` and `runtime` probe phases'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_analyzer_reports_probe_phases' -Text $runtimeMonkeyDocs -Needle '`runtime` probe phases'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_analyzer_reports_timestamps' -Text $runtimeMonkeyDocs -Needle 'reversed run-result timestamps'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_expected_ancient_ids_command' -Text $runtimeMonkeyDocs -Needle '-ExpectedAncientIds VAKUU,URDA,MORVI,LOTHA'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_expected_ancient_ids_required_for_proof_mode' -Text $runtimeMonkeyDocs -Needle 'In `-FailOnMismatch` proof mode, `-ExpectedAncientIds` is required'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_expected_ancient_ids_failure' -Text $runtimeMonkeyDocs -Needle 'any requested `-ExpectedAncientIds` value is'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_expected_ancient_ids_required_failure' -Text $runtimeMonkeyDocs -Needle 'Omitting the target set fails `expected_ancient_ids_required_for_proof_mode`'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_expected_ancient_ids_plan_summary' -Text $runtimeMonkeyDocs -Needle 'traversed-id coverage'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_requires_ancient_id_counts' -Text $runtimeMonkeyDocs -Needle '`AncientIdCounts` keyed by normalized'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_summary_batch_metadata_binding' -Text $runtimeMonkeyDocs -Needle 'top-level `RunnerKind`, `Sts1EventMode`, package/game/Ritsu targets'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_analyzer_summary_plan_binding' -Text $runtimeMonkeyDocs -Needle 'top-level AutoSlay analyzer summary-plan batch metadata drift'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_analyzer_missing_summary_plan_targets' -Text $runtimeMonkeyDocs -Needle 'non-positive `ExpectedPatchCount` or empty'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_rejects_extra_zero_ancient_id_counts' -Text $runtimeMonkeyDocs -Needle 'Extra zero-count keys still fail'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_failed_runs_counts_failure_hang_rows' -Text $runtimeMonkeyDocs -Needle '`FailureReasonCodes`, and `HangSignals`; top-level green fields are not'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_requires_run_result_hash_binding' -Text $runtimeMonkeyDocs -Needle '`RunResultSha256`, and the hash must match'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_rejects_non_positive_min_runs' -Text $runtimeMonkeyDocs -Needle 'never set it to 0 or'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_requires_ordered_selection_id' -Text $runtimeMonkeyDocs -Needle 'not merely present somewhere else'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_ancient_id_normalization' -Text $runtimeMonkeyDocs -Needle 'case-insensitively after normalizing them to uppercase'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_allow_missing_not_proof_mode' -Text $runtimeMonkeyDocs -Needle 'Do not combine `-AllowMissingEventTraversal` with `-FailOnMismatch`'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_proof_requires_current_target_switches' -Text $runtimeMonkeyDocs -Needle 'Omitting the current package/game/Ritsu/patch target switches fails'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_current_package_target' -Text $runtimeMonkeyDocs -Needle '-ExpectedPackageVersion v0.1.0-private-beta.97'
Add-ContainsCheck -Name 'runtime_monkey_docs_runtime_current_package_target' -Text $runtimeMonkeyDocs -Needle '.\scripts\check-spire-plus-runtime-monkey-packet.ps1 `
  -EvidenceDir .tools\runtime-evidence\<monkey-stability-dir> `
  -ExpectedIterations 5 `
  -ExpectedPackageVersion v0.1.0-private-beta.97'
Add-ContainsCheck -Name 'runtime_monkey_docs_runtime_proof_requires_current_target_switches' -Text $runtimeMonkeyDocs -Needle 'In `-FailOnMismatch` proof mode, the current package/game/Ritsu/patch target'
Add-ContainsCheck -Name 'current_validation_autoslay_expected_ancient_ids_plan_summary' -Text $currentValidation -Needle 'to match retained `autoslay-plan.json` `ExpectedAncientIds`, to appear in retained `autoslay-summary.json`, and to have sidecar-plus-current-log event traversal bound to the same Ancient id'
Add-ContainsCheck -Name 'review_autoslay_expected_ancient_ids_plan_summary' -Text $activeReview -Needle 'to match retained `autoslay-plan.json` `ExpectedAncientIds`, to appear in retained `autoslay-summary.json`, and to have sidecar-plus-current-log event traversal bound to the same Ancient id'
Add-ContainsCheck -Name 'runtime_monkey_docs_runtime_requires_runtime_log_growth' -Text $runtimeMonkeyDocs -Needle '`RuntimeLogGrowthRequired`, log-length, main-menu/runtime'
Add-ContainsCheck -Name 'runtime_monkey_docs_runtime_startup_only_log_growth_boundary' -Text $runtimeMonkeyDocs -Needle 'Startup-only or no-command observations do not require idle'
Add-ContainsCheck -Name 'runtime_monkey_docs_runtime_probe_log_growth_binding' -Text $runtimeMonkeyDocs -Needle 'the `PostCommandRuntime` samples'' `LogLengthBytes`'
Add-ContainsCheck -Name 'runtime_monkey_docs_analyzer_reports_probe_log_growth_mismatch' -Text $runtimeMonkeyDocs -Needle 'runtime log-growth timeline drift as `RuntimeHarness` blockers'
Add-ContainsCheck -Name 'runtime_monkey_docs_analyzer_rejects_iteration_escape_paths' -Text $runtimeMonkeyDocs -Needle '`iteration-result.json` log or probe paths that resolve outside the current'
Add-ContainsCheck -Name 'runtime_monkey_docs_analyzer_rejects_noncanonical_paths' -Text $runtimeMonkeyDocs -Needle 'shadow/nonstandard files under that directory'
Add-ContainsCheck -Name 'runtime_monkey_docs_analyzer_direct_smoke_root_target' -Text $runtimeMonkeyDocs -Needle 'the analyzer recognizes'
Add-ContainsCheck -Name 'runtime_monkey_docs_analyzer_direct_smoke_externalmod_drift' -Text $runtimeMonkeyDocs -Needle '`dependency patch failure` and `[ERROR] [previous package]` to `PackageRuntimeDrift`'
Add-ContainsCheck -Name 'runtime_monkey_docs_analyzer_direct_smoke_externalmod_patch_details' -Text $runtimeMonkeyDocs -Needle '`dependency patch failures` array records'
Add-ContainsCheck -Name 'runtime_monkey_docs_analyzer_direct_smoke_externalmod_relic_collection_detail' -Text $runtimeMonkeyDocs -Needle '`NRelicCollectionCategory::LoadRelics` instruction'
Add-ContainsCheck -Name 'runtime_monkey_docs_analyzer_direct_smoke_no_coop_false_positive' -Text $runtimeMonkeyDocs -Needle 'only explicit `coop_*override_enabled` runtime markers should produce that'
Add-ContainsCheck -Name 'runtime_monkey_docs_analyzer_autoslay_run_result_hash_binding' -Text $runtimeMonkeyDocs -Needle 'requires `autoslay-summary.json` `Runs[].RunResultSha256` to match the retained'
Add-ContainsCheck -Name 'runtime_monkey_docs_analyzer_autoslay_summary_signal_binding' -Text $runtimeMonkeyDocs -Needle 'requires retained summary row `Passed`, `FailureReasonCodes`, and'
Add-ContainsCheck -Name 'runtime_monkey_docs_packet_rejects_iteration_escape_paths' -Text $runtimeMonkeyDocs -Needle 'The packet checker rejects `iteration-result.json` log/probe'
Add-ContainsCheck -Name 'runtime_monkey_docs_runtime_packet_native_array_shapes' -Text $runtimeMonkeyDocs -Needle 'retained `runtime-probe-samples.json` must be'
Add-ContainsCheck -Name 'runtime_monkey_docs_runtime_packet_command_corpus_array_shape' -Text $runtimeMonkeyDocs -Needle '`CommandCorpus`, `PlannedCommands`, `Results[]`'
Add-ContainsCheck -Name 'runtime_monkey_docs_analyzer_runtime_summary_array_shape_invalid' -Text $runtimeMonkeyDocs -Needle '`monkey-summary.json` `Results` / `FailedIterationIds` shapes'
Add-ContainsCheck -Name 'runtime_monkey_docs_analyzer_autoslay_summary_runs_shape_invalid' -Text $runtimeMonkeyDocs -Needle '`autoslay_summary_shape_invalid` `RuntimeHarness` blocker'
Add-ContainsCheck -Name 'runtime_monkey_docs_analyzer_autoslay_summary_counter_mismatch' -Text $runtimeMonkeyDocs -Needle 'record `autoslay_summary_counter_mismatch` before any AutoSlay owner routing'
Add-ContainsCheck -Name 'runtime_monkey_docs_analyzer_autoslay_launcher_provenance_mismatch' -Text $runtimeMonkeyDocs -Needle 'autoslay_launcher_provenance_mismatch'
Add-ContainsCheck -Name 'runtime_monkey_docs_analyzer_autoslay_artifact_trust_closes_log_owner' -Text $runtimeMonkeyDocs -Needle 'if run, probe, sidecar, audit, or StS1'
Add-ContainsCheck -Name 'runtime_failure_analyzer_closes_autoslay_log_owner_after_artifact_recompute' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle '$logOwnerArea = ''Runtime.Unknown'''
Add-ContainsCheck -Name 'runtime_runner_runtime_log_growth_blocks_clean_pass' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\run-spire-plus-monkey-stability.ps1') -Raw -Encoding UTF8) -Needle 'runtime_log_stalled'
Add-ContainsCheck -Name 'runtime_packet_script_requires_runtime_log_growth' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'runtime_observation_log_grew'
Add-ContainsCheck -Name 'runtime_packet_script_tracks_command_log_growth_requirement' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'runtime_observation_log_growth_requirement_matches_command'
Add-ContainsCheck -Name 'runtime_packet_script_binds_probe_log_growth_to_runtime_observation' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'runtime_probe_samples_log_growth_matches_runtime_observation'
Add-ContainsCheck -Name 'runtime_packet_script_allows_startup_only_idle_log' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'StartupOnly/no-command observations do not require idle main-menu log growth'
Add-ContainsCheck -Name 'runtime_packet_script_rejects_top_level_array_shape_drift' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'plan_planned_commands_array'
Add-ContainsCheck -Name 'runtime_packet_script_rejects_command_corpus_array_shape_drift' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'plan_command_corpus_array'
Add-ContainsCheck -Name 'runtime_packet_script_rejects_per_iteration_array_shape_drift' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'runtime_probe_samples_array'
Add-ContainsCheck -Name 'runtime_monkey_docs_require_iteration_number_coverage' -Text $runtimeMonkeyDocs -Needle 'duplicate, missing, non-positive, or out-of-range'
Add-ContainsCheck -Name 'runtime_packet_script_requires_plan_iteration_coverage' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'plan_planned_iteration_numbers_cover_expected'
Add-ContainsCheck -Name 'runtime_packet_script_requires_summary_iteration_coverage' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'summary_result_iteration_numbers_cover_expected'
Add-ContainsCheck -Name 'runtime_monkey_docs_require_plan_expected_patch_count' -Text $runtimeMonkeyDocs -Needle 'positive `ExpectedPatchCount`'
Add-ContainsCheck -Name 'runtime_monkey_docs_require_runner_script_hash_binding' -Text $runtimeMonkeyDocs -Needle '`RunnerScriptPath` and `RunnerScriptSha256`'
Add-ContainsCheck -Name 'runtime_monkey_docs_require_command_corpus_hash_binding' -Text $runtimeMonkeyDocs -Needle '`CommandCorpusPath` and `CommandCorpusSha256`'
Add-ContainsCheck -Name 'runtime_monkey_docs_require_iteration_command_file_binding' -Text $runtimeMonkeyDocs -Needle '`iteration-000N\command.txt`'
Add-ContainsCheck -Name 'runtime_monkey_docs_require_iteration_command_file_hash_binding' -Text $runtimeMonkeyDocs -Needle '`CommandFilePath` and'
Add-ContainsCheck -Name 'runtime_monkey_docs_require_summary_command_file_hash_binding' -Text $runtimeMonkeyDocs -Needle 'summary `Results[]` row must retain the same command-file path and'
Add-ContainsCheck -Name 'runtime_monkey_docs_require_restore_summary_counter_binding' -Text $runtimeMonkeyDocs -Needle 'summary restore counters'
Add-ContainsCheck -Name 'runtime_monkey_docs_require_summary_max_telemetry_binding' -Text $runtimeMonkeyDocs -Needle 'Summary max telemetry must match the maximum values recomputed from'
Add-ContainsCheck -Name 'runtime_monkey_docs_require_summary_live_session_prepare_binding' -Text $runtimeMonkeyDocs -Needle 'Each `Results[]` live-session prepare-output path/SHA256 field must match'
Add-ContainsCheck -Name 'runtime_monkey_docs_require_summary_runtime_probe_binding' -Text $runtimeMonkeyDocs -Needle 'Each `Results[]` runtime-probe path/SHA256 field must match'
Add-ContainsCheck -Name 'runtime_monkey_docs_require_summary_live_session_state_binding' -Text $runtimeMonkeyDocs -Needle 'Each `Results[]` live-session state path/SHA256 field must match'
Add-ContainsCheck -Name 'runtime_monkey_docs_require_live_session_child_evidence_dir_binding' -Text $runtimeMonkeyDocs -Needle '`prepare-output.json`, `session-state.json`, and `restore-state.json`'
Add-ContainsCheck -Name 'runtime_monkey_docs_require_summary_failure_hang_binding' -Text $runtimeMonkeyDocs -Needle '`Results[]` row must also retain empty `FailureReasonCodes` and'
Add-ContainsCheck -Name 'runtime_monkey_docs_require_summary_failure_counter_binding' -Text $runtimeMonkeyDocs -Needle 'Top-level failed-iteration ids, failure-reason maps'
Add-ContainsCheck -Name 'runtime_monkey_docs_require_summary_batch_metadata_binding' -Text $runtimeMonkeyDocs -Needle 'Top-level `monkey-summary.json` batch metadata'
Add-ContainsCheck -Name 'runtime_monkey_docs_require_analyzer_summary_counter_mismatch' -Text $runtimeMonkeyDocs -Needle 'summary counter mismatch versus `Results[]`'
Add-ContainsCheck -Name 'runtime_monkey_docs_require_analyzer_summary_result_mismatch' -Text $runtimeMonkeyDocs -Needle '`Results[]` row mismatch versus canonical'
Add-ContainsCheck -Name 'runtime_monkey_docs_require_analyzer_summary_plan_mismatch' -Text $runtimeMonkeyDocs -Needle 'batch metadata drift versus'
Add-ContainsCheck -Name 'runtime_monkey_docs_require_analyzer_missing_summary_plan_targets' -Text $runtimeMonkeyDocs -Needle 'missing or blank target fields'
Add-ContainsCheck -Name 'runtime_monkey_docs_require_analyzer_plan_result_mismatch' -Text $runtimeMonkeyDocs -Needle '`PlannedCommands` row mismatch versus canonical'
Add-ContainsCheck -Name 'runtime_monkey_docs_require_analyzer_missing_plan_blocker' -Text $runtimeMonkeyDocs -Needle 'missing or malformed batch plan'
Add-ContainsCheck -Name 'runtime_runner_writes_runner_script_hash' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\run-spire-plus-monkey-stability.ps1') -Raw -Encoding UTF8) -Needle 'RunnerScriptSha256 = $runnerScriptSha256'
Add-ContainsCheck -Name 'runtime_runner_writes_command_corpus_hash' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\run-spire-plus-monkey-stability.ps1') -Raw -Encoding UTF8) -Needle 'CommandCorpusSha256 = $commandCorpusSha256'
Add-ContainsCheck -Name 'runtime_runner_writes_iteration_command_file' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\run-spire-plus-monkey-stability.ps1') -Raw -Encoding UTF8) -Needle "Join-Path `$iterationDir 'command.txt'"
Add-ContainsCheck -Name 'runtime_runner_writes_iteration_command_file_hash' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\run-spire-plus-monkey-stability.ps1') -Raw -Encoding UTF8) -Needle 'CommandFileSha256 = $commandFileSha256'
Add-ContainsCheck -Name 'runtime_runner_writes_summary_batch_metadata' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\run-spire-plus-monkey-stability.ps1') -Raw -Encoding UTF8) -Needle 'ExpectedPatchCount = $ExpectedPatchCount'
Add-ContainsCheck -Name 'runtime_packet_script_requires_plan_expected_patch_count' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'plan_expected_patch_count_positive'
Add-ContainsCheck -Name 'runtime_packet_script_uses_effective_expected_patch_count' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'effectiveExpectedPatchCount'
Add-ContainsCheck -Name 'runtime_packet_script_requires_package_version_parameter' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'expected_package_version_parameter_provided'
Add-ContainsCheck -Name 'runtime_packet_script_requires_game_version_parameter' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'expected_game_version_parameter_provided'
Add-ContainsCheck -Name 'runtime_packet_script_requires_ritsu_lib_version_parameter' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'expected_ritsu_lib_version_parameter_provided'
Add-ContainsCheck -Name 'runtime_packet_script_requires_ritsu_compat_branch_parameter' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'expected_ritsu_compat_branch_parameter_provided'
Add-ContainsCheck -Name 'runtime_packet_script_requires_patch_count_parameter' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'expected_patch_count_parameter_provided'
Add-ContainsCheck -Name 'runtime_packet_script_requires_runner_hash_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'plan_runner_script_hash_matches_current_runner'
Add-ContainsCheck -Name 'runtime_packet_script_requires_command_corpus_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'plan_command_corpus_file_matches_plan'
Add-ContainsCheck -Name 'runtime_packet_script_requires_iteration_command_file_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'command_file_matches_plan'
Add-ContainsCheck -Name 'runtime_packet_script_requires_iteration_command_file_hash_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'command_file_sha256_matches_retained_file'
Add-ContainsCheck -Name 'runtime_packet_script_binds_summary_command_file_path' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'summary_result_command_file_path_matches_iteration'
Add-ContainsCheck -Name 'runtime_packet_script_binds_summary_command_file_sha256' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'summary_result_command_file_sha256_matches_iteration'
Add-ContainsCheck -Name 'runtime_packet_script_binds_summary_live_session_prepare_output_path' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'summary_result_live_session_prepare_output_path_matches_iteration'
Add-ContainsCheck -Name 'runtime_packet_script_binds_summary_live_session_prepare_output_sha256' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'summary_result_live_session_prepare_output_sha256_matches_iteration'
Add-ContainsCheck -Name 'runtime_packet_script_binds_summary_runtime_probe_samples_path' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'summary_result_runtime_probe_samples_path_matches_iteration'
Add-ContainsCheck -Name 'runtime_packet_script_binds_summary_runtime_probe_samples_sha256' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'summary_result_runtime_probe_samples_sha256_matches_iteration'
Add-ContainsCheck -Name 'runtime_packet_script_binds_summary_live_session_session_state_path' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'summary_result_live_session_session_state_path_matches_iteration'
Add-ContainsCheck -Name 'runtime_packet_script_binds_summary_live_session_session_state_sha256' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'summary_result_live_session_session_state_sha256_matches_iteration'
Add-ContainsCheck -Name 'runtime_packet_script_binds_summary_live_session_restore_state_path' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'summary_result_live_session_restore_state_path_matches_iteration'
Add-ContainsCheck -Name 'runtime_packet_script_binds_summary_live_session_restore_state_sha256' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'summary_result_live_session_restore_state_sha256_matches_iteration'
Add-ContainsCheck -Name 'runtime_packet_script_requires_prepare_output_evidence_dir_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'prepare_output_evidence_dir_matches_iteration'
Add-ContainsCheck -Name 'runtime_packet_script_requires_session_state_evidence_dir_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'session_state_evidence_dir_matches_iteration'
Add-ContainsCheck -Name 'runtime_packet_script_requires_restore_state_evidence_dir_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'restore_state_evidence_dir_matches_iteration'
Add-ContainsCheck -Name 'runtime_packet_script_binds_summary_failure_reason_counts' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'summary_failure_reason_counts_match_results'
Add-ContainsCheck -Name 'runtime_packet_script_binds_summary_process_exit_count' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'summary_process_exit_count_matches_results'
Add-ContainsCheck -Name 'runtime_packet_script_binds_summary_command_ack_count' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'summary_command_ack_missing_count_matches_results'
Add-ContainsCheck -Name 'runtime_packet_script_binds_restore_summary_counter_zero' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'summary_live_session_restore_leak_count_zero'
Add-ContainsCheck -Name 'runtime_packet_script_binds_restore_summary_counter_results' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'summary_live_session_restore_leak_count_matches_results'
Add-ContainsCheck -Name 'runtime_packet_script_binds_summary_max_main_menu' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'summary_max_main_menu_elapsed_matches_results'
Add-ContainsCheck -Name 'runtime_packet_script_binds_summary_max_log_growth' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'summary_max_seconds_without_log_growth_matches_results'
Add-ContainsCheck -Name 'runtime_packet_script_binds_summary_max_unresponsive' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'summary_max_consecutive_unresponsive_matches_results'
Add-ContainsCheck -Name 'runtime_packet_script_binds_summary_failure_reason_codes_empty' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'summary_result_failure_reason_codes_empty'
Add-ContainsCheck -Name 'runtime_packet_script_binds_summary_failure_reason_codes_match' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'summary_result_failure_reason_codes_match_iteration'
Add-ContainsCheck -Name 'runtime_packet_script_binds_summary_hang_signals_match' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'summary_result_hang_signals_match_iteration'
Add-ContainsCheck -Name 'runtime_packet_script_binds_summary_batch_metadata_to_plan' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'summary_expected_patch_count_matches_plan'
Add-ContainsCheck -Name 'runtime_analyzer_rejects_summary_counter_mismatch' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'runtime_monkey_summary_counter_mismatch'
Add-ContainsCheck -Name 'runtime_analyzer_rejects_summary_result_mismatch' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'runtime_monkey_summary_result_mismatch'
Add-ContainsCheck -Name 'runtime_analyzer_rejects_summary_plan_mismatch' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'runtime_monkey_summary_plan_mismatch'
Add-ContainsCheck -Name 'runtime_analyzer_requires_summary_plan_target_field_presence' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'IsNullOrWhiteSpace($planValue)'
Add-ContainsCheck -Name 'runtime_analyzer_requires_summary_plan_patch_count_positive' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle '$planPatchCount -le 0'
Add-ContainsCheck -Name 'runtime_analyzer_rejects_plan_result_mismatch' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'runtime_monkey_plan_result_mismatch'
Add-ContainsCheck -Name 'runtime_analyzer_rejects_missing_runtime_monkey_plan' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'runtime_monkey_plan_missing_or_invalid'
Add-ContainsCheck -Name 'runtime_analyzer_rejects_runtime_summary_array_shape_invalid' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'Results must be retained as a native JSON array'
Add-ContainsCheck -Name 'runtime_analyzer_rejects_autoslay_summary_runs_shape_invalid' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'autoslay_summary_shape_invalid'
Add-ContainsCheck -Name 'runtime_analyzer_rejects_autoslay_summary_counter_mismatch' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'autoslay_summary_counter_mismatch'
Add-ContainsCheck -Name 'runtime_analyzer_rejects_autoslay_launcher_provenance_mismatch' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'autoslay_launcher_provenance_mismatch'
Add-ContainsCheck -Name 'runtime_packet_script_rejects_before_log_escape' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'godot_log_before_under_iteration_dir'
Add-ContainsCheck -Name 'runtime_packet_script_rejects_after_launch_log_escape' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'godot_log_after_launch_under_iteration_dir'
Add-ContainsCheck -Name 'runtime_packet_script_rejects_godot_current_log_escape' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'godot_current_iteration_log_under_iteration_dir'
Add-ContainsCheck -Name 'runtime_packet_script_rejects_current_log_escape' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'current_iteration_log_under_iteration_dir'
Add-ContainsCheck -Name 'runtime_packet_script_rejects_probe_escape' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'runtime_probe_samples_under_iteration_dir'
Add-ContainsCheck -Name 'runtime_packet_script_binds_probe_expected_start_time_to_live_session' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'runtime_probe_samples_expected_process_start_time_matches_live_session'
Add-ContainsCheck -Name 'runtime_packet_script_binds_probe_expected_path_to_live_session' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'runtime_probe_samples_expected_process_path_matches_live_session'
Add-ContainsCheck -Name 'runtime_packet_script_rejects_probe_identity_mismatch' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-runtime-monkey-packet.ps1') -Raw -Encoding UTF8) -Needle 'runtime_probe_samples_all_match_live_session_identity'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_packet_result_path_escape' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'RuntimeMonkeyPacketCheckerRejectsResultPathsOutsideIterationDirectory'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_iteration_number_coverage' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'RuntimeMonkeyPacketCheckerRejectsDuplicateAndMissingIterationNumbers'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_plan_patch_count_fallback' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'RuntimeMonkeyPacketCheckerUsesPlanPatchCountWhenParameterIsOmitted'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_missing_current_target_parameters' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'RuntimeMonkeyPacketCheckerRejectsProofModeWhenCurrentTargetParametersAreOmitted'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_runner_script_hash_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'RuntimeMonkeyPacketCheckerBindsPlanToCurrentRunnerScript'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_command_corpus_hash_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'RuntimeMonkeyPacketCheckerBindsCommandCorpusFileToPlan'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_iteration_command_file_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'RuntimeMonkeyPacketCheckerBindsIterationCommandFileToPlan'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_iteration_command_file_hash_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'command_file_sha256_matches_retained_file status=fail'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_summary_command_file_hash_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'RuntimeMonkeyPacketCheckerBindsSummaryCommandFileHashToIterationResult'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_summary_live_session_prepare_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'RuntimeMonkeyPacketCheckerBindsSummaryLiveSessionPrepareOutputToIterationResult'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_summary_runtime_probe_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'RuntimeMonkeyPacketCheckerBindsSummaryRuntimeProbeSamplesToIterationResult'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_summary_live_session_state_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'RuntimeMonkeyPacketCheckerBindsSummaryLiveSessionStateFilesToIterationResult'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_live_session_child_evidence_dir_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'RuntimeMonkeyPacketCheckerRejectsLiveSessionChildEvidenceDirDrift'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_restore_summary_counter_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'RuntimeMonkeyPacketCheckerRejectsStaleRestoreSummaryCounters'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_summary_failure_counter_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'RuntimeMonkeyPacketCheckerBindsSummaryFailureCountersToResults'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_summary_max_telemetry_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'RuntimeMonkeyPacketCheckerBindsSummaryMaxTelemetryToResults'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_summary_failure_hang_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'RuntimeMonkeyPacketCheckerBindsSummaryFailureAndHangSignalsToIterationResult'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_summary_batch_metadata_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'RuntimeMonkeyPacketCheckerBindsSummaryBatchMetadataToPlan'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_packet_command_corpus_array_shape_drift' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'plan_command_corpus_array status=fail'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_packet_top_level_array_shape_drift' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'RuntimeMonkeyPacketCheckerRejectsMalformedNativeArrayFieldsWithoutCrashing'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_packet_per_iteration_array_shape_drift' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.PacketArrayShape.cs') -Raw -Encoding UTF8) -Needle 'RuntimeMonkeyPacketCheckerRejectsMalformedPerIterationArrayEvidence'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_analyzer_summary_counter_mismatch' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'RuntimeFailureAnalyzerRejectsRuntimeMonkeySummaryCounterDrift'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_analyzer_summary_result_mismatch' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'RuntimeFailureAnalyzerRejectsRuntimeMonkeySummaryResultDrift'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_analyzer_summary_plan_mismatch' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'RuntimeFailureAnalyzerRejectsRuntimeMonkeySummaryPlanDrift'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_analyzer_missing_summary_plan_targets' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'RuntimeFailureAnalyzerRejectsRuntimeMonkeyMissingSummaryPlanTargets'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_analyzer_plan_result_mismatch' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'RuntimeFailureAnalyzerRejectsRuntimeMonkeyPlanResultDrift'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_analyzer_missing_runtime_monkey_plan' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'RuntimeFailureAnalyzerRejectsRuntimeMonkeyBatchWithoutPlan'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_analyzer_runtime_summary_array_shape_invalid' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.AnalyzerArrayShape.cs') -Raw -Encoding UTF8) -Needle 'RuntimeFailureAnalyzerRejectsMalformedRuntimeMonkeySummaryArrayShape'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_analyzer_autoslay_summary_runs_shape_invalid' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.AnalyzerArrayShape.cs') -Raw -Encoding UTF8) -Needle 'RuntimeFailureAnalyzerRejectsMalformedGameNativeAutoSlaySummaryRunsShape'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_analyzer_autoslay_summary_counter_mismatch' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.AnalyzerArrayShape.cs') -Raw -Encoding UTF8) -Needle 'RuntimeFailureAnalyzerRejectsGameNativeAutoSlaySummaryCounterDrift'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_analyzer_autoslay_launcher_provenance_mismatch' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'RuntimeFailureAnalyzerRejectsGameNativeAutoSlayLauncherProvenanceDrift'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_probe_expected_identity_drift' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'RuntimeMonkeyPacketCheckerRejectsProbeExpectedIdentityDrift'
Add-ContainsCheck -Name 'autoslay_packet_script_rejects_missing_event_traversal' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'batch_event_room_traversal_observed'
Add-ContainsCheck -Name 'autoslay_packet_script_requires_autoslayer_start' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'AutoSlayer.Start(seed, logFile)'
Add-ContainsCheck -Name 'autoslay_packet_script_requires_current_slice_match' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'current_iteration_log_matches_after_launch_slice'
Add-ContainsCheck -Name 'autoslay_packet_script_requires_audit_recompute' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'audit_recomputed_from_current_iteration_log'
Add-ContainsCheck -Name 'autoslay_packet_script_requires_sts1_mode_log_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'sts1_mode_log_check_log_sha256_matches_current_iteration_log'
Add-ContainsCheck -Name 'autoslay_packet_script_requires_run_result_launch_provenance' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'run_result_stale_process_count_zero'
Add-ContainsCheck -Name 'autoslay_packet_script_requires_launcher_sha_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'plan_launcher_sha256_matches'
Add-ContainsCheck -Name 'autoslay_packet_script_requires_run_result_passed' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'run_result_passed_true'
Add-ContainsCheck -Name 'autoslay_packet_script_requires_probe_phases' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'runtime_probe_samples_main_menu_phase_observed'
Add-ContainsCheck -Name 'autoslay_packet_script_requires_timestamp_order' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'run_result_timestamp_order_valid'
Add-ContainsCheck -Name 'autoslay_packet_script_requires_runtime_log_growth' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'run_result_runtime_log_grew'
Add-ContainsCheck -Name 'autoslay_packet_script_binds_probe_expected_start_time_to_run_result' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'runtime_probe_samples_expected_process_start_time_matches_run_result'
Add-ContainsCheck -Name 'autoslay_packet_script_binds_probe_expected_path_to_run_result' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'runtime_probe_samples_expected_process_path_matches_run_result'
Add-ContainsCheck -Name 'autoslay_packet_script_rejects_probe_identity_mismatch' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'runtime_probe_samples_all_match_expected_identity'
Add-ContainsCheck -Name 'autoslay_packet_script_requires_ancient_identity' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'event_kind_is_ancient'
Add-ContainsCheck -Name 'autoslay_packet_script_requires_ancient_dialogue_marker' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'Detected Ancient event, clicking through dialogue'
Add-ContainsCheck -Name 'autoslay_packet_script_expected_ancient_ids_parameter' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle '[string[]]$ExpectedAncientIds = @()'
Add-ContainsCheck -Name 'autoslay_packet_script_requires_package_version_parameter' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'expected_package_version_parameter_provided'
Add-ContainsCheck -Name 'autoslay_packet_script_requires_game_version_parameter' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'expected_game_version_parameter_provided'
Add-ContainsCheck -Name 'autoslay_packet_script_requires_ritsu_lib_version_parameter' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'expected_ritsu_lib_version_parameter_provided'
Add-ContainsCheck -Name 'autoslay_packet_script_requires_ritsu_compat_branch_parameter' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'expected_ritsu_compat_branch_parameter_provided'
Add-ContainsCheck -Name 'autoslay_packet_script_requires_patch_count_parameter' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'expected_patch_count_parameter_provided'
Add-ContainsCheck -Name 'autoslay_packet_script_disallows_allow_missing_event_traversal_proof_mode' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'allow_missing_event_traversal_not_proof_mode'
Add-ContainsCheck -Name 'autoslay_packet_script_expected_ancient_ids_normalization_helper' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'function Get-NormalizedAncientIdTokens'
Add-ContainsCheck -Name 'autoslay_packet_script_expected_ancient_ids_required_for_proof_mode' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'expected_ancient_ids_required_for_proof_mode'
Add-ContainsCheck -Name 'autoslay_packet_script_expected_ancient_ids_comma_split' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle "-split ','"
Add-ContainsCheck -Name 'autoslay_packet_script_expected_ancient_ids_plan_match' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'plan_expected_ancient_ids_match_parameter'
Add-ContainsCheck -Name 'autoslay_packet_script_expected_ancient_ids_summary_check' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'summary_expected_ancient_ids_observed'
Add-ContainsCheck -Name 'autoslay_packet_script_requires_ancient_id_counts' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'summary_ancient_id_counts_match_runs'
Add-ContainsCheck -Name 'autoslay_packet_script_requires_positive_min_runs' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'min_runs_positive'
Add-ContainsCheck -Name 'autoslay_packet_script_expected_ancient_ids_traversed_check' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'expected_ancient_ids_have_event_traversal'
Add-ContainsCheck -Name 'autoslay_packet_script_expected_ancient_ids_observed_normalized' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle '$observedAncientIdSet.Add($normalizedAncientId)'
Add-ContainsCheck -Name 'autoslay_packet_script_expected_ancient_ids_traversed_normalized' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle '$traversedAncientIdSet.Add($ancientId.Trim().ToUpperInvariant())'
Add-ContainsCheck -Name 'autoslay_packet_script_expected_ancient_ids_selection_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'autoslay_log_selects_ancient_id'
Add-ContainsCheck -Name 'autoslay_packet_script_requires_ordered_selection_id' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle '$eventSelectionNeedleForOrder'
Add-ContainsCheck -Name 'autoslay_packet_script_expected_ancient_ids_plan_report_field' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'PlanExpectedAncientIds'
Add-ContainsCheck -Name 'autoslay_packet_script_expected_ancient_ids_report_field' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'MissingExpectedAncientIds'
Add-ContainsCheck -Name 'autoslay_packet_script_expected_ancient_ids_traversed_report_field' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'MissingExpectedTraversedAncientIds'
Add-ContainsCheck -Name 'autoslay_packet_script_binds_summary_batch_metadata_to_plan' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'summary_expected_patch_count_matches_plan'
Add-ContainsCheck -Name 'autoslay_packet_script_failed_runs_counts_problem_rows' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle '$summaryProblemRunRows = @($summaryRuns | Where-Object {'
Add-ContainsCheck -Name 'autoslay_packet_script_failed_runs_reports_problem_rows' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'problemRows=$($summaryProblemRunRows.Count)'
Add-ContainsCheck -Name 'autoslay_packet_script_requires_run_result_hash_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'run_result_sha256_matches_retained_file'
Add-ContainsCheck -Name 'autoslay_tests_cover_mixed_case_ancient_id_normalization' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'mixedCaseAncientIdResult'
Add-ContainsCheck -Name 'autoslay_tests_cover_ancient_id_normalization_contract' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'AutoSlay packet verifier should normalize AncientId target coverage case'
Add-ContainsCheck -Name 'autoslay_tests_cover_ancient_id_counts_mismatch' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'mismatchedAncientIdCountsResult'
Add-ContainsCheck -Name 'autoslay_tests_cover_extra_zero_ancient_id_counts' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'extraZeroAncientIdCountsResult'
Add-ContainsCheck -Name 'autoslay_tests_cover_non_positive_min_runs' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'nonPositiveMinRunsResult'
Add-ContainsCheck -Name 'autoslay_tests_cover_stale_selection_before_wrong_actual_ancient' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'staleSelectionBeforeActualWrongAncientResult'
Add-ContainsCheck -Name 'autoslay_tests_cover_allow_missing_event_traversal_proof_mode' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'allowMissingEventTraversalProofResult'
Add-ContainsCheck -Name 'autoslay_tests_cover_probe_expected_identity_drift' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'runtimeProbeExpectedIdentityDriftResult'
Add-ContainsCheck -Name 'autoslay_tests_cover_summary_problem_signal_counter_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.GameNativeAutoSlay.cs') -Raw -Encoding UTF8) -Needle 'autoslay_seed_failed'
Add-ContainsCheck -Name 'autoslay_tests_cover_summary_problem_rows_detail' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.GameNativeAutoSlay.cs') -Raw -Encoding UTF8) -Needle 'problemRows=1'
Add-ContainsCheck -Name 'autoslay_tests_cover_run_result_hash_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.GameNativeAutoSlay.cs') -Raw -Encoding UTF8) -Needle 'run_0001_run_result_sha256_matches_retained_file status=fail'
Add-ContainsCheck -Name 'autoslay_tests_cover_summary_batch_metadata_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.GameNativeAutoSlay.cs') -Raw -Encoding UTF8) -Needle 'GameNativeAutoSlayPacketVerifierBindsSummaryBatchMetadataToPlan'
Add-ContainsCheck -Name 'autoslay_tests_cover_missing_current_target_parameters' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.GameNativeAutoSlay.cs') -Raw -Encoding UTF8) -Needle 'GameNativeAutoSlayPacketVerifierRejectsProofModeWhenCurrentTargetParametersAreOmitted'
Add-ContainsCheck -Name 'runtime_failure_analyzer_reads_autoslay_summary' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'autoslay-summary.json'
Add-ContainsCheck -Name 'runtime_failure_analyzer_binds_autoslay_summary_batch_metadata_to_plan' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'autoslay_summary_plan_mismatch'
Add-ContainsCheck -Name 'runtime_failure_analyzer_requires_autoslay_summary_plan_target_field_presence' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle '[string]::IsNullOrWhiteSpace($planValue)'
Add-ContainsCheck -Name 'runtime_failure_analyzer_requires_autoslay_summary_expected_ancient_ids_nonempty' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle '$planExpectedAncientIds.Count -eq 0'
Add-ContainsCheck -Name 'runtime_failure_analyzer_tests_cover_autoslay_summary_plan_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'RuntimeFailureAnalyzerRejectsAutoSlaySummaryPlanDrift'
Add-ContainsCheck -Name 'runtime_failure_analyzer_tests_cover_autoslay_missing_summary_plan_targets' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'RuntimeFailureAnalyzerRejectsAutoSlayMissingSummaryPlanTargets'
Add-ContainsCheck -Name 'runtime_failure_analyzer_reads_run_result' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'run-result.json'
Add-ContainsCheck -Name 'runtime_failure_analyzer_reads_direct_smoke_summary' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'direct-smoke-summary.json'
Add-ContainsCheck -Name 'runtime_failure_analyzer_direct_smoke_runner_kind' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle '$runnerKind = ''DirectSmoke'''
Add-ContainsCheck -Name 'runtime_failure_analyzer_direct_smoke_scenario_tag' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle '$scenarioTag = ''direct-smoke'''
Add-ContainsCheck -Name 'runtime_failure_analyzer_routes_externalmod_errors_to_package_drift' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'ExternalMod.*(?:HarmonyException|Patching exception|patch(?:ing)? exception|failed)'
Add-ContainsCheck -Name 'runtime_failure_analyzer_extracts_externalmod_patch_failures' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'function Get-ExternalModFailureDetails'
Add-ContainsCheck -Name 'runtime_failure_analyzer_reports_externalmod_patch_failures' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'ExternalModFailures = @($externalModFailures)'
Add-ContainsCheck -Name 'runtime_failure_analyzer_extracts_undefined_target_method' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'Undefined target method for patch method'
Add-ContainsCheck -Name 'runtime_failure_analyzer_extracts_instruction_match_failure' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'Failed to find match'
Add-ContainsCheck -Name 'runtime_failure_analyzer_coop_override_requires_runtime_marker' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle '\bcoop_[a-z0-9_]*override_enabled\b'
Add-ContainsCheck -Name 'runtime_failure_analyzer_tracks_autoslay_artifact_field_names' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'FieldName = ''AutoSlayLogPath'''
Add-ContainsCheck -Name 'runtime_failure_analyzer_checks_retained_autoslay_path_fields' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle '$artifactFieldRetained = Test-JsonProperty -Object $result -Name ([string]$artifact.FieldName)'
Add-ContainsCheck -Name 'runtime_failure_analyzer_checks_autoslay_artifact_exists_safely' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'Test-Path -LiteralPath $artifactPath -PathType Leaf'
Add-ContainsCheck -Name 'runtime_failure_analyzer_marks_autoslay_artifact_trust_false' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle '$autoSlayRunArtifactsTrustedForOwner = $false'
Add-ContainsCheck -Name 'runtime_failure_analyzer_requires_autoslay_run_result_per_seed_dir' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'RunResultPathMatchesExpectedPerSeedDir'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_run_result_root_shared_rejected' -Text $runtimeMonkeyDocs -Needle 'root/shared `run-result.json` paths'
Add-ContainsCheck -Name 'autoslay_packet_script_requires_exact_top_level_run_result_path' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'run_result_path_matches_expected_per_seed_dir'
Add-ContainsCheck -Name 'autoslay_packet_script_requires_exact_top_level_probe_path' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'runtime_probe_samples_path_matches_expected_per_seed_file'
Add-ContainsCheck -Name 'autoslay_packet_script_requires_exact_top_level_current_log_path' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'current_iteration_log_path_matches_expected_per_seed_file'
Add-ContainsCheck -Name 'autoslay_packet_script_requires_exact_top_level_sts1_report_path' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\check-spire-plus-autoslay-packet.ps1') -Raw -Encoding UTF8) -Needle 'sts1_mode_check_path_matches_expected_per_seed_file'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_packet_exact_top_level_run_result_path' -Text $runtimeMonkeyDocs -Needle 'top-level per-seed path'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_packet_exact_standard_artifact_paths' -Text $runtimeMonkeyDocs -Needle 'Every standard per-seed artifact path must resolve exactly'
Add-ContainsCheck -Name 'runtime_monkey_docs_autoslay_packet_rejects_nested_shadow_run_dirs' -Text $runtimeMonkeyDocs -Needle 'nested shadow `run-####`'
Add-ContainsCheck -Name 'runtime_failure_analyzer_requires_autoslay_before_after_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'current_iteration_log_before_after_binding_missing'
Add-ContainsCheck -Name 'runtime_failure_analyzer_requires_autoslay_run_result_hash_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'autoslay_run_result_summary_hash_mismatch'
Add-ContainsCheck -Name 'runtime_failure_analyzer_requires_autoslay_summary_signal_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'autoslay_summary_failure_reason_codes_mismatch'
Add-ContainsCheck -Name 'runtime_failure_analyzer_requires_autoslay_probe_phases' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'autoslay_runtime_probe_main_menu_phase_missing'
Add-ContainsCheck -Name 'runtime_failure_analyzer_requires_autoslay_timestamp_order' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'autoslay_run_result_timestamp_order_invalid'
Add-ContainsCheck -Name 'runtime_failure_analyzer_requires_runtime_log_growth' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle "'LogGrew'"
Add-ContainsCheck -Name 'runtime_failure_analyzer_reports_runtime_log_stalled' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'runtime_log_stalled'
Add-ContainsCheck -Name 'runtime_failure_analyzer_reports_probe_log_growth_mismatch' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'runtime_monkey_probe_runtime_log_growth_mismatch'
Add-ContainsCheck -Name 'runtime_failure_analyzer_reports_runtime_monkey_probe_identity_mismatch' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'runtime_monkey_probe_process_identity_mismatch'
Add-ContainsCheck -Name 'runtime_failure_analyzer_reports_runtime_monkey_current_slice_escape' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'runtime_monkey_current_iteration_log_outside_iteration_dir'
Add-ContainsCheck -Name 'runtime_failure_analyzer_reports_runtime_monkey_probe_escape' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'runtime_monkey_runtime_probe_samples_outside_iteration_dir'
Add-ContainsCheck -Name 'runtime_failure_analyzer_reports_runtime_monkey_current_slice_noncanonical' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'runtime_monkey_current_iteration_log_not_retained_file'
Add-ContainsCheck -Name 'runtime_failure_analyzer_reports_runtime_monkey_probe_noncanonical' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'runtime_monkey_runtime_probe_samples_not_retained_file'
Add-ContainsCheck -Name 'runtime_failure_analyzer_reports_runtime_monkey_artifact_trust' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'scripts\analyze-spire-plus-runtime-failure.ps1') -Raw -Encoding UTF8) -Needle 'RuntimeMonkeyRunArtifactsTrustedForOwner'
Add-ContainsCheck -Name 'runtime_monkey_tests_pin_probe_identity_signal' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle '"runtime_monkey_probe_process_identity_mismatch"'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_analyzer_noncanonical_path_rejection' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'RuntimeFailureAnalyzerRejectsRuntimeMonkeyArtifactsThatDoNotMatchRetainedFiles'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_autoslay_analyzer_run_result_hash_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'autoslay_run_result_summary_hash_mismatch'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_autoslay_analyzer_summary_signal_binding' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'autoslay_summary_failure_reason_codes_mismatch'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_autoslay_root_shared_run_result_path' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'RuntimeFailureAnalyzerRejectsGameNativeAutoSlayRootSharedRunResultPath'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_autoslay_packet_exact_top_level_run_result_path' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.GameNativeAutoSlay.cs') -Raw -Encoding UTF8) -Needle 'run_0001_run_result_path_matches_expected_per_seed_dir status=fail'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_autoslay_packet_exact_top_level_standard_artifact_paths' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.GameNativeAutoSlay.cs') -Raw -Encoding UTF8) -Needle 'run_0001_sts1_mode_check_path_matches_expected_per_seed_file status=fail'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_autoslay_audit_closes_log_owner' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'Assert.False(staleAuditIteration.GetProperty("LogTextTrustedForOwner").GetBoolean())'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_autoslay_sts1_closes_log_owner' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'Assert.False(staleSts1Iteration.GetProperty("LogTextTrustedForOwner").GetBoolean())'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_direct_smoke_package_drift' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.DirectSmokeAnalyzer.cs') -Raw -Encoding UTF8) -Needle 'RuntimeFailureAnalyzerRoutesDirectSmokeDirtyAuditToPackageRuntimeDrift'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_direct_smoke_externalmod_patch_details' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.DirectSmokeAnalyzer.cs') -Raw -Encoding UTF8) -Needle 'ExternalModFailures'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_direct_smoke_adjust_custom_message_keys' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.DirectSmokeAnalyzer.cs') -Raw -Encoding UTF8) -Needle 'AdjustCustomMessageKeys::Fuckery()'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_direct_smoke_relic_collection_load_relics' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.DirectSmokeAnalyzer.cs') -Raw -Encoding UTF8) -Needle 'NRelicCollectionCategory::LoadRelics'
Add-ContainsCheck -Name 'runtime_monkey_tests_direct_smoke_no_coop_false_positive' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.DirectSmokeAnalyzer.cs') -Raw -Encoding UTF8) -Needle 'coop_override_enabled_runtime_failure'
Add-ContainsCheck -Name 'runtime_monkey_tests_cover_malformed_autoslay_path' -Text (Get-Content -LiteralPath (Resolve-RepoPath 'tests\EZMicroBalance.Tests\RuntimeMonkeyStabilityGuardTests.cs') -Raw -Encoding UTF8) -Needle 'runtime-failure-analysis-malformed-autoslay-path.json'
Add-ContainsCheck -Name 'static_suite_invokes_registry_shape_fail_closed' -Text $staticSuiteScript -Needle 'Invoke-StaticStep -Name ''registry-shape'' -ScriptName ''check-sts1-event-registry-shape.ps1'' -Parameters @{ FailOnMismatch = $true }'
Add-ContainsCheck -Name 'static_suite_invokes_canary_expected_shape' -Text $staticSuiteScript -Needle 'Invoke-StaticStep -Name ''enabled-log-canary-expected-shape'' -ScriptName ''check-sts1-enabled-mode-runtime-log.ps1'' -Parameters @{ Mode = ''CanaryOnly''; PrintExpected = $true }'
Add-ContainsCheck -Name 'static_suite_invokes_additive_expected_shape' -Text $staticSuiteScript -Needle 'Invoke-StaticStep -Name ''enabled-log-additive-batch1-expected-shape'' -ScriptName ''check-sts1-enabled-mode-runtime-log.ps1'' -Parameters @{ Mode = ''AdditiveBatch1''; PrintExpected = $true }'
Add-ContainsCheck -Name 'static_suite_invokes_current_doc_claims_fail_closed' -Text $staticSuiteScript -Needle 'Invoke-StaticStep -Name ''current-doc-claims'' -ScriptName ''check-sts1-event-current-doc-claims.ps1'' -Parameters @{ FailOnMismatch = $true }'
Add-ContainsCheck -Name 'static_suite_invokes_file_hygiene_fail_closed' -Text $staticSuiteScript -Needle 'Invoke-StaticStep -Name ''static-file-hygiene'' -ScriptName ''check-sts1-static-file-hygiene.ps1'' -Parameters @{ FailOnMismatch = $true }'
Add-ContainsCheck -Name 'static_suite_invokes_gate_ledger_fail_closed' -Text $staticSuiteScript -Needle 'Invoke-StaticStep -Name ''v19-gate-ledger'' -ScriptName ''check-sts1-v19-gate-ledger.ps1'' -Parameters @{ FailOnMismatch = $true }'
Add-ContainsCheck -Name 'static_suite_invokes_v20_overlay_fail_closed' -Text $staticSuiteScript -Needle 'Invoke-StaticStep -Name ''v20-final-gate-overlay'' -ScriptName ''check-sts1-v20-final-gate-overlay.ps1'' -Parameters @{ FailOnMismatch = $true }'
Add-ContainsCheck -Name 'static_suite_invokes_subagent_coverage_fail_closed' -Text $staticSuiteScript -Needle 'Invoke-StaticStep -Name ''v19-subagent-coverage'' -ScriptName ''check-sts1-v19-subagent-coverage.ps1'' -Parameters @{ FailOnMismatch = $true }'
Add-ContainsCheck -Name 'static_suite_invokes_spec_notes_fail_closed' -Text $staticSuiteScript -Needle 'Invoke-StaticStep -Name ''event-spec-registration-notes'' -ScriptName ''check-sts1-event-spec-registration-notes.ps1'' -Parameters @{ FailOnMismatch = $true }'
Add-ContainsCheck -Name 'static_suite_invokes_feature_gates_fail_closed' -Text $staticSuiteScript -Needle 'Invoke-StaticStep -Name ''feature-gates'' -ScriptName ''check-sts1-event-feature-gates.ps1'' -Parameters @{ FailOnMismatch = $true }'
Add-ContainsCheck -Name 'static_suite_invokes_parity_blockers_fail_closed' -Text $staticSuiteScript -Needle 'Invoke-StaticStep -Name ''parity-blockers'' -ScriptName ''check-sts1-event-parity-blockers.ps1'' -Parameters @{ FailOnMismatch = $true }'
Add-ContainsCheck -Name 'static_suite_invokes_asset_safety_fail_closed' -Text $staticSuiteScript -Needle 'Invoke-StaticStep -Name ''asset-safety'' -ScriptName ''check-sts1-event-asset-safety.ps1'' -Parameters @{ FailOnMismatch = $true }'
Add-ContainsCheck -Name 'static_suite_invokes_multiplayer_shape_fail_closed' -Text $staticSuiteScript -Needle 'Invoke-StaticStep -Name ''multiplayer-shape'' -ScriptName ''check-sts1-event-multiplayer-shape.ps1'' -Parameters @{ FailOnMismatch = $true }'
Add-ContainsCheck -Name 'static_suite_invokes_localization_source_keys' -Text $staticSuiteScript -Needle 'Invoke-StaticStep -Name ''localization-source-keys'' -ScriptName ''check-sts1-localization-source-keys.ps1'' -Parameters $localizationParameters'
Add-ContainsCheck -Name 'static_suite_invokes_localization_gap_baseline_fail_closed' -Text $staticSuiteScript -Needle 'Invoke-StaticStep -Name ''localization-gap-baseline'' -ScriptName ''check-sts1-localization-gap-baseline.ps1'' -Parameters @{ FailOnMismatch = $true }'
Add-ContainsCheck -Name 'static_suite_prints_step_count' -Text $staticSuiteScript -Needle 'Write-Output "static_suite_steps=$($steps.Count)"'
Add-ContainsCheck -Name 'static_suite_prints_failure_count' -Text $staticSuiteScript -Needle 'Write-Output "static_suite_failures=$($failures.Count)"'
Add-ContainsCheck -Name 'static_suite_fails_on_suite_failures' -Text $staticSuiteScript -Needle 'if ($failures.Count -gt 0) {'
Add-ContainsCheck -Name 'static_file_hygiene_scans_agents' -Text $staticFileHygieneScript -Needle "'AGENTS.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_project_state' -Text $staticFileHygieneScript -Needle "'PROJECT_STATE.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_root_readme' -Text $staticFileHygieneScript -Needle "'README.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_goal_guard' -Text $staticFileHygieneScript -Needle "'docs\goal.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_doc_restructure_spec' -Text $staticFileHygieneScript -Needle "'docs\doc-restructure-spec.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_restructure_doc' -Text $staticFileHygieneScript -Needle "'docs\restructure.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_patch_boundaries' -Text $staticFileHygieneScript -Needle "'docs\architecture\patch-boundaries.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_beta_compatibility' -Text $staticFileHygieneScript -Needle "'docs\BETA_COMPATIBILITY.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_remote_development_setup' -Text $staticFileHygieneScript -Needle "'docs\REMOTE_DEVELOPMENT_SETUP.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_source_api_drift_audit' -Text $staticFileHygieneScript -Needle "'docs\audits\v0.106-source-api-drift.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_platform_testing' -Text $staticFileHygieneScript -Needle "'docs\platform-testing.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_root_test_plan' -Text $staticFileHygieneScript -Needle "'docs\test-plan.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_migration_doc' -Text $staticFileHygieneScript -Needle "'docs\migration.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_goal_migration_doc' -Text $staticFileHygieneScript -Needle "'docs\goals\migration.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_goal_debug_doc' -Text $staticFileHygieneScript -Needle "'docs\goals\debug.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_goal_refactor_doc' -Text $staticFileHygieneScript -Needle "'docs\goals\refactor.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_legacy_v5_monthly_spec' -Text $staticFileHygieneScript -Needle "'docs\goals\sts1_event_port_strict_audit_monthly_spec_v5_overnight_subagents.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_ritsu_integration' -Text $staticFileHygieneScript -Needle "'docs\integrations\ritsulib.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_ritsu_monthly_spec' -Text $staticFileHygieneScript -Needle "'docs\features\ritsulib-migration\monthly-dev-spec.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_ritsu_batch4c' -Text $staticFileHygieneScript -Needle "'docs\features\ritsulib-migration\batch-4c-candidates.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_ritsu_runtime_checklist' -Text $staticFileHygieneScript -Needle "'docs\features\ritsulib-migration\runtime-smoke-checklist.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_ritsu_runtime_hard_block' -Text $staticFileHygieneScript -Needle "'docs\features\ritsulib-migration\runtime-hard-block-report-20260531.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_ritsu_next_overnight' -Text $staticFileHygieneScript -Needle "'docs\features\ritsulib-migration\next-overnight-run.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_m5_l_runtime_hard_blocker' -Text $staticFileHygieneScript -Needle "'docs\goals\m5-revision-l-runtime-hard-blocker.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_m5_l_runtime_smoke_plan' -Text $staticFileHygieneScript -Needle "'docs\goals\m5-revision-l-runtime-smoke-plan.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_m5_l_final_report' -Text $staticFileHygieneScript -Needle "'docs\goals\m5-revision-l-final-report.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_m5_l_owner_packet' -Text $staticFileHygieneScript -Needle "'docs\goals\m5-revision-l-owner-review-packet.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_m5_l_dirty_ledger' -Text $staticFileHygieneScript -Needle "'docs\goals\m5-revision-l-dirty-ledger.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_m5_l_commit_slices' -Text $staticFileHygieneScript -Needle "'docs\goals\m5-revision-l-commit-slices.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_m5_l_warning_ledger' -Text $staticFileHygieneScript -Needle "'docs\goals\m5-revision-l-warning-ledger.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_m5_final_report' -Text $staticFileHygieneScript -Needle "'docs\goals\m5-revision-m-final-report.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_m5_owner_packet' -Text $staticFileHygieneScript -Needle "'docs\goals\m5-revision-m-owner-review-packet.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_m5_runtime_drift' -Text $staticFileHygieneScript -Needle "'docs\goals\m5-revision-m-runtime-drift-report.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_m5_patch_failure_ledger' -Text $staticFileHygieneScript -Needle "'docs\goals\m5-revision-m-patch-failure-ledger.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_m5_version_decision' -Text $staticFileHygieneScript -Needle "'docs\goals\m5-revision-m-version-decision.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_m5_commit_slices' -Text $staticFileHygieneScript -Needle "'docs\goals\m5-revision-m-commit-slices.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_goal_warning_ledger' -Text $staticFileHygieneScript -Needle "'docs\goals\warning-ledger.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_overnight_run_ledger' -Text $staticFileHygieneScript -Needle "'docs\goals\overnight-run-ledger.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_overnight_run_status' -Text $staticFileHygieneScript -Needle "'docs\goals\overnight-run-status.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_historical_overnight_review' -Text $staticFileHygieneScript -Needle "'docs\reviews\overnight-run-20260529.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_historical_refactor_qa' -Text $staticFileHygieneScript -Needle "'docs\reviews\refactor-qa-20260602.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_historical_refactor_qa_round2' -Text $staticFileHygieneScript -Needle "'docs\reviews\refactor-qa-20260602-round2.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_release_evidence_status' -Text $staticFileHygieneScript -Needle "'docs\release-evidence-status.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_private_beta_release_audit' -Text $staticFileHygieneScript -Needle "'docs\private-beta-release-completion-audit.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_test_ready_completion_audit' -Text $staticFileHygieneScript -Needle "'docs\test-ready-completion-audit.md'"
Add-ContainsCheck -Name 'static_file_hygiene_scans_scripts_readme' -Text $staticFileHygieneScript -Needle "'scripts\README.md'"
Add-ContainsCheck -Name 'static_file_hygiene_guards_replacement_chars' -Text $staticFileHygieneScript -Needle "sts1_hygiene_no_replacement_chars"
Add-ContainsCheck -Name 'static_file_hygiene_guards_ritsu_monthly_spec_scope' -Text $staticFileHygieneScript -Needle "sts1_hygiene_scans_ritsu_monthly_spec"
Add-ContainsCheck -Name 'static_file_hygiene_guards_ritsu_batch4c_scope' -Text $staticFileHygieneScript -Needle "sts1_hygiene_scans_ritsu_batch4c"
Add-ContainsCheck -Name 'static_file_hygiene_guards_runtime_checklist_scope' -Text $staticFileHygieneScript -Needle "sts1_hygiene_scans_ritsu_runtime_checklist"
Add-ContainsCheck -Name 'static_file_hygiene_guards_next_overnight_scope' -Text $staticFileHygieneScript -Needle "sts1_hygiene_scans_ritsu_next_overnight"
Add-ContainsCheck -Name 'static_file_hygiene_guards_status_board_ascii_chain' -Text $staticFileHygieneScript -Needle "sts1_hygiene_status_board_status_chain_ascii"
Add-ContainsCheck -Name 'static_file_hygiene_guards_status_board_mojibake' -Text $staticFileHygieneScript -Needle "sts1_hygiene_status_board_no_arrow_mojibake"

Add-ContainsCheck -Name 'live_session_records_sts1_mode_env' -Text $liveSessionScript -Needle 'Sts1EventModeEnvironment = [string]$env:SPIREPLUS_STS1_EVENT_MODE'
Add-ContainsCheck -Name 'live_session_records_sts1_unsafe_env' -Text $liveSessionScript -Needle 'Sts1UnsafeModeEnvironment = [string]$env:SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES'
Add-ContainsCheck -Name 'live_session_copies_game_release_info' -Text $liveSessionScript -Needle "game-release-info.json"
Add-ContainsCheck -Name 'live_session_default_game_root_current_e_drive' -Text $liveSessionScript -Needle "[string]`$GameRoot = 'E:\Steam\steamapps\common\Slay the Spire 2'"
Add-ContainsCheck -Name 'live_session_default_steam_exe_current_e_drive' -Text $liveSessionScript -Needle "[string]`$SteamExe = 'E:\Steam\steam.exe'"
Add-ContainsCheck -Name 'runtime_packet_requires_enabled_mode_env' -Text $runtimeEvidencePacketScript -Needle 'session_sts1_mode_env_matches_mode'
Add-ContainsCheck -Name 'runtime_packet_rejects_unsafe_env' -Text $runtimeEvidencePacketScript -Needle 'session_no_unsafe_sts1_mode_env'
Add-ContainsCheck -Name 'runtime_packet_requires_exact_allowed_mod_ids' -Text $runtimeEvidencePacketScript -Needle 'session_allowed_mod_ids_exact'
Add-ContainsCheck -Name 'runtime_packet_rejects_moved_allowed_mods' -Text $runtimeEvidencePacketScript -Needle 'session_moved_mods_do_not_include_allowed_mods'
Add-ContainsCheck -Name 'runtime_packet_checks_moved_mod_source_root' -Text $runtimeEvidencePacketScript -Needle 'session_moved_mod_sources_under_mods_root'
Add-ContainsCheck -Name 'runtime_packet_checks_moved_mod_isolation_root' -Text $runtimeEvidencePacketScript -Needle 'session_moved_mod_destinations_under_isolated_mods'
Add-ContainsCheck -Name 'runtime_packet_checks_restore_mod_count' -Text $runtimeEvidencePacketScript -Needle 'restore_mod_count_matches_session_moved_mods'
Add-ContainsCheck -Name 'runtime_packet_checks_restore_current_run_count' -Text $runtimeEvidencePacketScript -Needle 'restore_current_run_count_matches_session_moved_runs'
Add-ContainsCheck -Name 'runtime_packet_requires_enabled_package_version_param' -Text $runtimeEvidencePacketScript -Needle 'enabled_expected_package_version_parameter_provided'
Add-ContainsCheck -Name 'runtime_packet_requires_enabled_ritsu_branch_param' -Text $runtimeEvidencePacketScript -Needle 'enabled_expected_ritsu_compat_branch_parameter_provided'
Add-ContainsCheck -Name 'runtime_packet_requires_enabled_ritsu_lib_version_param' -Text $runtimeEvidencePacketScript -Needle 'enabled_expected_ritsu_lib_version_parameter_provided'
Add-ContainsCheck -Name 'runtime_packet_requires_enabled_game_version_param' -Text $runtimeEvidencePacketScript -Needle 'enabled_expected_game_version_parameter_provided'
Add-ContainsCheck -Name 'runtime_packet_requires_enabled_outfile_param' -Text $runtimeEvidencePacketScript -Needle 'enabled_outfile_parameter_provided'
Add-ContainsCheck -Name 'runtime_packet_rejects_enabled_missing_session_bypass' -Text $runtimeEvidencePacketScript -Needle 'enabled_session_state_cannot_be_legacy_optional'
Add-ContainsCheck -Name 'runtime_packet_rejects_enabled_missing_restore_bypass' -Text $runtimeEvidencePacketScript -Needle 'enabled_restore_state_cannot_be_legacy_optional'
Add-ContainsCheck -Name 'runtime_packet_retains_enabled_log_check_json' -Text $runtimeEvidencePacketScript -Needle 'enabled_mode_log_check_json_retained'
Add-ContainsCheck -Name 'runtime_packet_checks_game_release_info' -Text $runtimeEvidencePacketScript -Needle 'expected_game_version_in_release_info'
Add-ContainsCheck -Name 'runtime_packet_checks_ritsu_lib_version_in_log' -Text $runtimeEvidencePacketScript -Needle 'expected_ritsu_lib_version_in_log'
Add-ContainsCheck -Name 'runtime_packet_legacy_off_game_version_fallback' -Text $runtimeEvidencePacketScript -Needle 'expected_game_version_in_log_legacy_off_packet'
Add-ContainsCheck -Name 'runtime_packet_passes_expected_package_to_log_verifier' -Text $runtimeEvidencePacketScript -Needle '$verifierParams[''ExpectedPackageVersion''] = $ExpectedPackageVersion'
Add-ContainsCheck -Name 'runtime_packet_passes_expected_ritsu_to_log_verifier' -Text $runtimeEvidencePacketScript -Needle '$verifierParams[''ExpectedRitsuCompatBranch''] = $ExpectedRitsuCompatBranch'
Add-ContainsCheck -Name 'runtime_packet_passes_expected_ritsu_lib_to_log_verifier' -Text $runtimeEvidencePacketScript -Needle '$verifierParams[''ExpectedRitsuLibVersion''] = $ExpectedRitsuLibVersion'
Add-ContainsCheck -Name 'runtime_packet_passes_expected_game_to_log_verifier' -Text $runtimeEvidencePacketScript -Needle '$verifierParams[''ExpectedGameVersion''] = $ExpectedGameVersion'
Add-ContainsCheck -Name 'enabled_log_requires_package_version_param' -Text $enabledModeLogScript -Needle 'enabled_expected_package_version_parameter_provided'
Add-ContainsCheck -Name 'enabled_log_requires_ritsu_branch_param' -Text $enabledModeLogScript -Needle 'enabled_expected_ritsu_compat_branch_parameter_provided'
Add-ContainsCheck -Name 'enabled_log_requires_ritsu_lib_version_param' -Text $enabledModeLogScript -Needle 'enabled_expected_ritsu_lib_version_parameter_provided'
Add-ContainsCheck -Name 'enabled_log_requires_game_version_param' -Text $enabledModeLogScript -Needle 'enabled_expected_game_version_parameter_provided'
Add-ContainsCheck -Name 'enabled_log_requires_audit_path_param' -Text $enabledModeLogScript -Needle 'enabled_audit_path_parameter_provided'
Add-ContainsCheck -Name 'enabled_log_requires_outfile_param' -Text $enabledModeLogScript -Needle 'enabled_outfile_parameter_provided'
Add-ContainsCheck -Name 'enabled_log_checks_package_version_in_log' -Text $enabledModeLogScript -Needle 'expected_package_version_in_log'
Add-ContainsCheck -Name 'enabled_log_checks_ritsu_branch_in_log' -Text $enabledModeLogScript -Needle 'expected_ritsu_compat_branch_in_log'
Add-ContainsCheck -Name 'enabled_log_ritsu_branch_line_hits' -Text $enabledModeLogScript -Needle 'Get-RitsuCompatBranchLineHits'
Add-ContainsCheck -Name 'enabled_log_ritsu_branch_not_substring_only' -Text $enabledModeLogScript -Needle 'expected explicit RitsuLib compat branch line'
Add-ContainsCheck -Name 'enabled_log_checks_ritsu_lib_version_in_log' -Text $enabledModeLogScript -Needle 'expected_ritsu_lib_version_in_log'
Add-ContainsCheck -Name 'enabled_log_ritsu_lib_version_line_hits' -Text $enabledModeLogScript -Needle 'Get-RitsuLibVersionLineHits'
Add-ContainsCheck -Name 'enabled_log_ritsu_lib_version_not_substring_only' -Text $enabledModeLogScript -Needle 'expected explicit RitsuLib package version'
Add-ContainsCheck -Name 'enabled_log_checks_game_version_in_log' -Text $enabledModeLogScript -Needle 'expected_game_version_in_log'
Add-ContainsCheck -Name 'enabled_log_game_version_line_hits' -Text $enabledModeLogScript -Needle 'Get-GameVersionLineHits'
Add-ContainsCheck -Name 'enabled_log_game_version_not_substring_only' -Text $enabledModeLogScript -Needle 'expected explicit game version line'
Add-ContainsCheck -Name 'enabled_log_checks_registration_call_count' -Text $enabledModeLogScript -Needle 'observed_registration_call_count'
Add-ContainsCheck -Name 'enabled_log_prints_registration_tuples' -Text $enabledModeLogScript -Needle 'expected_registration_tuples='
Add-ContainsCheck -Name 'enabled_log_prints_observed_registration_tuples' -Text $enabledModeLogScript -Needle 'observed_registration_tuples='
Add-ContainsCheck -Name 'enabled_log_prints_missing_registration_tuples' -Text $enabledModeLogScript -Needle 'missing_registration_tuples='
Add-ContainsCheck -Name 'enabled_log_prints_unexpected_registration_tuples' -Text $enabledModeLogScript -Needle 'unexpected_registration_tuples='
Add-ContainsCheck -Name 'enabled_log_checks_registration_tuples' -Text $enabledModeLogScript -Needle 'observed_registration_tuples_match_expected'
Add-ContainsCheck -Name 'enabled_log_reports_tuple_diffs' -Text $enabledModeLogScript -Needle 'MissingRegistrationTuples'
Add-ContainsCheck -Name 'runtime_preflight_default_game_root_current_e_drive' -Text $runtimePreflightScript -Needle "[string]`$GameRoot = 'E:\Steam\steamapps\common\Slay the Spire 2'"
Add-ContainsCheck -Name 'runtime_preflight_checks_repo_spire_plus_manifest' -Text $runtimePreflightScript -Needle 'repo_spire_plus_manifest'
Add-ContainsCheck -Name 'runtime_preflight_checks_repo_spire_plus_version' -Text $runtimePreflightScript -Needle 'repo_spire_plus_version_matches_expected'
Add-ContainsCheck -Name 'runtime_preflight_checks_ritsu_manifest' -Text $runtimePreflightScript -Needle 'mod_manifest.json'
Add-ContainsCheck -Name 'runtime_preflight_checks_ritsu_compat_target' -Text $runtimePreflightScript -Needle 'compat-target.txt'
Add-ContainsCheck -Name 'runtime_preflight_checks_installed_spire_plus_manifest' -Text $runtimePreflightScript -Needle 'EZMicroBalance.json'
Add-ContainsCheck -Name 'runtime_preflight_invokes_canary_expected_shape' -Text $runtimePreflightScript -Needle "Invoke-ExpectedShape -Mode 'CanaryOnly'"
Add-ContainsCheck -Name 'runtime_preflight_invokes_additive_expected_shape' -Text $runtimePreflightScript -Needle "Invoke-ExpectedShape -Mode 'AdditiveBatch1'"

$sts1ScriptNames = @(Get-ChildItem -LiteralPath (Resolve-RepoPath 'scripts') -Filter 'check-sts1*.ps1' -File |
    Sort-Object Name |
    Select-Object -ExpandProperty Name)

foreach ($scriptName in $sts1ScriptNames) {
    $checkName = "scripts_readme_lists_$($scriptName -replace '[^A-Za-z0-9]+', '_')"
    Add-ContainsCheck -Name $checkName -Text $scriptsReadme -Needle $scriptName
}

Add-ContainsCheck -Name 'runtime_checklist_beta96_target' -Text $runtimeChecklist -Needle 'Spire Plus `v0.1.0-private-beta.97`'
Add-ContainsCheck -Name 'runtime_checklist_settings_ui_path' -Text $runtimeChecklist -Needle '.tools/runtime-evidence/beta96-ritsulib-mod-settings-clicked-ui-20260621-160701/'
Add-ContainsCheck -Name 'runtime_checklist_beta96_loader_pass' -Text $runtimeChecklist -Needle 'The previous beta.96 Off packet is retained at'
Add-Check -Name 'runtime_checklist_no_previous_package_setup' -Passed (-not ($runtimeChecklist -match '(?i)previous[- ]package')) -Detail 'runtime smoke checklist must not route current validation through the old dependency package'
Add-ContainsCheck -Name 'runtime_canary_current_expected_4_6' -Text $runtimeChecklist -Needle '4 canary event types / 6 registration calls'
Add-RegexCheck -Name 'runtime_batch1_expected_14' -Text $runtimeChecklist -Pattern '\| AdditiveBatch1 \|[^\r\n]*14 registration calls / 10 event types[^\r\n]*PENDING beta\.97 recapture'
Add-ContainsCheck -Name 'runtime_success_criteria_canary_4_6' -Text $runtimeChecklist -Needle 'CanaryOnly proves 4 canary event types through 6 registration calls'
Add-ContainsCheck -Name 'runtime_success_criteria_batch1_14' -Text $runtimeChecklist -Needle 'AdditiveBatch1 proves 10 event types through 14 registration calls.'
Add-ContainsCheck -Name 'runtime_exit_status_beta96_pending' -Text $runtimeChecklist -Needle 'Current exit status: beta.97 package parity passes; clicked RitsuLib Mod'
Add-ContainsCheck -Name 'runtime_checklist_coordination_boundary' -Text $runtimeChecklist -Needle 'Coordination boundary: run this checklist''s launch, gameplay, build, publish,'
Add-ContainsCheck -Name 'runtime_checklist_after_pause_controlled_lane' -Text $runtimeChecklist -Needle 'The previous beta.96 Off packet is retained at'
Add-ContainsCheck -Name 'runtime_checklist_live_session_prepare_explicit_paths' -Text $runtimeChecklist -Needle "-GameRoot 'E:\Steam\steamapps\common\Slay the Spire 2'"
Add-ContainsCheck -Name 'runtime_checklist_live_session_prepare_explicit_steam' -Text $runtimeChecklist -Needle "-SteamExe 'E:\Steam\steam.exe'"
Add-ContainsCheck -Name 'runtime_checklist_live_session_prepare_steam_user' -Text $runtimeChecklist -Needle '-SteamUserId $steamUserId'
Add-ContainsCheck -Name 'runtime_checklist_live_session_prepare_mode' -Text $runtimeChecklist -Needle '-Mode Prepare'
Add-ContainsCheck -Name 'runtime_checklist_live_session_prepare_isolation_flags' -Text $runtimeChecklist -Needle '-MoveOtherMods'
Add-ContainsCheck -Name 'runtime_checklist_live_session_prepare_run_isolation' -Text $runtimeChecklist -Needle '-MoveCurrentRuns'
Add-ContainsCheck -Name 'runtime_checklist_live_session_prepare_launch' -Text $runtimeChecklist -Needle '-Launch'
Add-ContainsCheck -Name 'runtime_checklist_live_session_restore_mode' -Text $runtimeChecklist -Needle '-Mode Restore'
Add-ContainsCheck -Name 'runtime_checklist_live_session_restore_stop_game' -Text $runtimeChecklist -Needle '-StopGameOnRestore'
Add-ContainsCheck -Name 'runtime_checklist_live_session_restore_preserves_current_runs' -Text $runtimeChecklist -Needle '-PreserveNewCurrentRunsOnRestore'
Add-ContainsCheck -Name 'runtime_checklist_live_session_keeps_ritsulib_enabled' -Text $runtimeChecklist -Needle '`STS2-RitsuLib` is not moved out by any mod-isolation step'
Add-ContainsCheck -Name 'runtime_enabled_log_verifier' -Text $runtimeChecklist -Needle 'check-sts1-enabled-mode-runtime-log.ps1'
Add-ContainsCheck -Name 'runtime_enabled_log_verifier_non_claim' -Text $runtimeChecklist -Needle 'That output is not enabled-mode proof'
Add-ContainsCheck -Name 'runtime_enabled_log_verifier_requires_package_target' -Text $runtimeChecklist -Needle 'log verifier requires explicit expected'
Add-ContainsCheck -Name 'runtime_enabled_log_verifier_registration_call_count' -Text $runtimeChecklist -Needle 'observed registered event-line count matches the source-derived'
Add-ContainsCheck -Name 'runtime_enabled_log_verifier_tuple_check' -Text $runtimeChecklist -Needle 'observed registration tuples parsed from `Registered act event` /'
Add-ContainsCheck -Name 'runtime_enabled_log_verifier_tuple_fallback_boundary' -Text $runtimeChecklist -Needle 'future logs lose act/shared tuple detail'
Add-ContainsCheck -Name 'runtime_evidence_packet_verifier' -Text $runtimeChecklist -Needle 'check-sts1-runtime-evidence-packet.ps1'
Add-ContainsCheck -Name 'runtime_evidence_packet_no_launch' -Text $runtimeChecklist -Needle 'packet has the expected copied files'
Add-ContainsCheck -Name 'runtime_evidence_packet_mode_metadata' -Text $runtimeChecklist -Needle 'Sts1EventModeEnvironment'
Add-ContainsCheck -Name 'runtime_evidence_packet_exact_allowed_mod_ids' -Text $runtimeChecklist -Needle 'equal to STS2-RitsuLib and EZMicroBalance for the current RitsuLib-only lane'
Add-ContainsCheck -Name 'runtime_checklist_enabled_log_verifier_outfile_canary' -Text $runtimeChecklist -Needle '.\scripts\check-sts1-enabled-mode-runtime-log.ps1 -Mode CanaryOnly -LogPath "<evidence>\godot.log.current-iteration" -AuditPath "<evidence>\godot-log-current-iteration-audit.json" -ExpectedPackageVersion v0.1.0-private-beta.97 -ExpectedRitsuCompatBranch 0.107.1 -ExpectedRitsuLibVersion 0.4.31 -ExpectedGameVersion 0.107.1 -OutFile "<evidence>\enabled-mode-log-check.json" -FailOnMismatch'
Add-ContainsCheck -Name 'runtime_checklist_enabled_log_verifier_outfile_batch1' -Text $runtimeChecklist -Needle '.\scripts\check-sts1-enabled-mode-runtime-log.ps1 -Mode AdditiveBatch1 -LogPath "<evidence>\godot.log.current-iteration" -AuditPath "<evidence>\godot-log-current-iteration-audit.json" -ExpectedPackageVersion v0.1.0-private-beta.97 -ExpectedRitsuCompatBranch 0.107.1 -ExpectedRitsuLibVersion 0.4.31 -ExpectedGameVersion 0.107.1 -OutFile "<evidence>\enabled-mode-log-check.json" -FailOnMismatch'
Add-ContainsCheck -Name 'runtime_checklist_packet_verifier_outfile_canary' -Text $runtimeChecklist -Needle '.\scripts\check-sts1-runtime-evidence-packet.ps1 -Mode CanaryOnly -EvidenceDir "<evidence>" -ExpectedPackageVersion v0.1.0-private-beta.97 -ExpectedRitsuCompatBranch 0.107.1 -ExpectedRitsuLibVersion 0.4.31 -ExpectedGameVersion 0.107.1 -OutFile "<evidence>\runtime-evidence-packet-check.json" -FailOnMismatch'
Add-ContainsCheck -Name 'runtime_checklist_packet_verifier_outfile_batch1' -Text $runtimeChecklist -Needle '.\scripts\check-sts1-runtime-evidence-packet.ps1 -Mode AdditiveBatch1 -EvidenceDir "<evidence>" -ExpectedPackageVersion v0.1.0-private-beta.97 -ExpectedRitsuCompatBranch 0.107.1 -ExpectedRitsuLibVersion 0.4.31 -ExpectedGameVersion 0.107.1 -OutFile "<evidence>\runtime-evidence-packet-check.json" -FailOnMismatch'
Add-ContainsCheck -Name 'runtime_checklist_keeps_verifier_reports' -Text $runtimeChecklist -Needle 'Keep `enabled-mode-log-check.json` and `runtime-evidence-packet-check.json`'
Add-ContainsCheck -Name 'runtime_evidence_packet_moved_mod_roots' -Text $runtimeChecklist -Needle 'moved-mod source/destination paths stay under the recorded mods root'
Add-ContainsCheck -Name 'runtime_evidence_packet_restore_counts' -Text $runtimeChecklist -Needle 'restore counts match the session moved-mod'
Add-ContainsCheck -Name 'runtime_evidence_packet_rejects_unsafe_leakage' -Text $runtimeChecklist -Needle 'rejects unsafe-mode'
Add-ContainsCheck -Name 'runtime_evidence_packet_rejects_missing_state_bypass' -Text $runtimeChecklist -Needle '`-AllowMissingSessionState` / `-AllowMissingRestoreState`'
Add-ContainsCheck -Name 'runtime_evidence_packet_requires_package_version' -Text $runtimeChecklist -Needle 'requires explicit expected package-version, Ritsu compat-branch,'
Add-ContainsCheck -Name 'runtime_evidence_packet_requires_game_version' -Text $runtimeChecklist -Needle 'RitsuLib package-version, and game-version checks'
Add-ContainsCheck -Name 'runtime_evidence_packet_uses_current_slice' -Text $runtimeChecklist -Needle '`godot.log.current-iteration` as canonical proof'
Add-ContainsCheck -Name 'runtime_evidence_packet_binds_retained_current_slice' -Text $runtimeChecklist -Needle 'retained current slices must'
Add-ContainsCheck -Name 'runtime_evidence_packet_rejects_full_log_canonical_input' -Text $runtimeChecklist -Needle 'rejects full-log-only canonical verifier input'
Add-ContainsCheck -Name 'runtime_evidence_packet_rejects_stale_current_slice' -Text $runtimeChecklist -Needle 'byte-match `godot.log.after-launch` after the `godot.log.before` prefix'
Add-ContainsCheck -Name 'runtime_evidence_packet_script_checks_current_slice_binding' -Text $runtimeEvidencePacketScript -Needle "Add-Check -Name 'current_slice_matches_before_after'"
Add-ContainsCheck -Name 'runtime_evidence_packet_script_reports_current_slice_binding' -Text $runtimeEvidencePacketScript -Needle 'CurrentSliceMatchesBeforeAfter'

Add-ContainsCheck -Name 'next_overnight_beta94_totals' -Text $nextOvernight -Needle 'Latest beta.97 no-game/package validation is summarized in `PROJECT_STATE.md` and `docs/reviews/current-validation.md`: build/publish/package refresh, installed-package parity, runtime preflight, and source-workspace checks passed for the current dependency target; beta.97 loader/settings proof remains pending after the RitsuLib I18N settings resource migration.'
Add-ContainsCheck -Name 'next_overnight_latest_observed_head_recapture_boundary' -Text $nextOvernight -Needle 'Use `git log -1 --oneline --decorate` and `git status --short --branch` as the source of truth; older run-start hashes from prior follow-ups are historical notes and must not be reused for handoff.'
Add-ContainsCheck -Name 'goal_migration_latest_observed_head_recapture_boundary' -Text $goalMigrationDoc -Needle 'Recapture `git log -1 --oneline --decorate` and `git status --short --branch` at the start of each continuation and immediately before handoff; older run-start hashes are historical notes, not current status.'
Add-NoRegexCheck -Name 'no_brittle_current_pushed_head_pin_in_migration_docs' -Paths @('docs\goals\migration.md', 'docs\features\ritsulib-migration\next-overnight-run.md') -Pattern 'Current pushed HEAD is `[0-9a-f]{7,40}`|latest observed pushed HEAD is `[0-9a-f]{7,40}`'
Add-ContainsCheck -Name 'next_overnight_head_validation_beta87' -Text $nextOvernight -Needle 'Current HEAD validation refreshed after the beta.97 RitsuLib-only settings page I18N pass; recheck again before handoff if any later edits appear.'
Add-ContainsCheck -Name 'next_overnight_coordination_boundary' -Text $nextOvernight -Needle 'Coordination boundary: do not run overlapping validation, package/release, runtime/game smoke, staging, commit, or push steps.'
Add-ContainsCheck -Name 'next_overnight_post_baseline_scope' -Text $nextOvernight -Needle 'Any dirty files after the latest pushed HEAD are post-baseline follow-up scope. Classify them before any validation claim, package handoff, commit, or push.'
Add-ContainsCheck -Name 'next_overnight_historical_canary_old_shape' -Text $nextOvernight -Needle 'CanaryOnly: `.tools\runtime-evidence\smoke-k1-canary3-20260602-151104`, exactly 4 canary registrations in the old source shape, clean audit. Current source expects 4 event types through 6 registration calls.'
Add-ContainsCheck -Name 'next_overnight_historical_batch1_old_shape' -Text $nextOvernight -Needle 'AdditiveBatch1: `.tools\runtime-evidence\additive-batch1-20260602-150445`, 10 event types through the old 11 registration calls, clean audit. Current source expects 10 event types through 14 registration calls.'
Add-ContainsCheck -Name 'next_overnight_enabled_smoke_boundary' -Text $nextOvernight -Needle 'Retained CanaryOnly enabled-mode proof is clean under `.tools/runtime-evidence/v01070-beta85-canary-20260617-233621/`: 4 event types / 6 registration lines'
Add-ContainsCheck -Name 'next_overnight_canary_smoke_step_before_gameplay' -Text $nextOvernight -Needle 'Before any StS1 canary gameplay claim, recapture current-version CanaryOnly loader proof; `.tools/runtime-evidence/v01070-beta85-canary-20260617-233621/` is retained previous-package/game-version context only.'
Add-ContainsCheck -Name 'next_overnight_batch1_smoke_step_before_gameplay' -Text $nextOvernight -Needle 'Before any AdditiveBatch1 gameplay claim, confirm the beta.97 package and STS2-RitsuLib `v0.4.31` / `lib\0.107.1` runtime are installed and recapture current-package enabled-mode proof'
Add-ContainsCheck -Name 'next_overnight_current_canary_success_closed' -Text $nextOvernight -Needle '- [x] Retained `v0.107.0` beta.85 CanaryOnly smoke proves 4 event types / 6 registration calls with retained verifier reports.'
Add-ContainsCheck -Name 'next_overnight_current_batch1_success_closed' -Text $nextOvernight -Needle '- [x] Retained `v0.107.0` beta.87 AdditiveBatch1 smoke proves 10 event types / 14 registration calls with retained verifier reports; beta.85 13/14 attempt remains root-cause history only.'
Add-ContainsCheck -Name 'ritsu_monthly_enabled_smokes_boundary' -Text $ritsuMonthlyDevSpec -Needle 'Previous beta.93 AdditiveBatch1 enabled-mode smoke is clean with 10 event types / 14 registration calls and retained verifier reports for previous-package registration context only.'
Add-ContainsCheck -Name 'ritsu_monthly_handoff_status_recap' -Text $ritsuMonthlyDevSpec -Needle 'Handoff must recapture HEAD and worktree status after any later edits.'
Add-ContainsCheck -Name 'ritsu_monthly_canary_smoke_before_gameplay' -Text $ritsuMonthlyDevSpec -Needle 'Use the retained CanaryOnly enabled-mode smoke at `.tools/runtime-evidence/v01070-beta85-canary-20260617-233621/` only as beta.85 / `v0.107.0` previous-package proof; recapture before making a current-version CanaryOnly claim.'
Add-ContainsCheck -Name 'ritsu_monthly_batch1_smoke_before_gameplay' -Text $ritsuMonthlyDevSpec -Needle 'Use the retained AdditiveBatch1 enabled-mode smoke at `.tools/runtime-evidence/v01071-beta93-ritsulib0431-additivebatch1-direct-20260621/` only as beta.93 / `v0.107.1` 10 event types / 14 registration-call proof; recapture it after any package/source change before using it for a new claim.'
Add-ContainsCheck -Name 'ritsu_monthly_gameplay_after_enabled_smokes' -Text $ritsuMonthlyDevSpec -Needle 'Only after those enabled-mode smokes match current source shape, capture CanaryOnly gameplay evidence for Big Fish, Golden Idol, The Lab, and Divine Fountain.'
Add-ContainsCheck -Name 'ritsu_batch4c_enabled_smokes_boundary' -Text $ritsuBatch4cCandidates -Needle 'Previous beta.93 AdditiveBatch1 retained log and packet verifiers passed with 10 event types / 14 registration lines and exact tuple parity. This is loader/registration proof only and is not Batch 4c approval, gameplay proof, or handoff proof.'
Add-ContainsCheck -Name 'ritsu_batch4c_event_runtime_claim_requires_enabled_smokes' -Text $ritsuBatch4cCandidates -Needle 'Before any Batch 4c follow-up is cited as StS1 event runtime readiness, cite the retained current AdditiveBatch1 10 event types / 14 registration-line smoke with retained verifier reports and add the missing gameplay evidence'

Add-ContainsCheck -Name 'issue_current_off_plus_canary_correction' -Text $liveRiskIssue -Needle 'Retained beta.85 `v0.107.0` proof covers default-Off plus CanaryOnly loader registration only as previous-package/game-version context, while previous beta.93 RitsuLib-only proof covers Off plus AdditiveBatch1 loader/registration shape only'
Add-ContainsCheck -Name 'issue_batch1_14' -Text $liveRiskIssue -Needle '14 registrations / 10 event types'
Add-ContainsCheck -Name 'issue_all_draft_57' -Text $liveRiskIssue -Needle '57 registration calls (47 compiling event types)'
Add-ContainsCheck -Name 'issue_loader_default_off_plus_canary' -Text $liveRiskIssue -Needle 'current `godot.log` loader proof exists for default-Off and CanaryOnly loader registration only'

Add-ContainsCheck -Name 'sts1_goal_revision_n_boundary' -Text $sts1FeatureGoal -Needle 'Current Boundary (Revision P / beta.93)'
Add-ContainsCheck -Name 'sts1_goal_not_complete' -Text $sts1FeatureGoal -Needle 'This goal is not complete.'
Add-ContainsCheck -Name 'sts1_goal_default_off_canary_additive_boundary' -Text $sts1FeatureGoal -Needle 'Previous beta.93 proof under `.tools/runtime-evidence/v01071-beta93-ritsulib0431-additivebatch1-direct-20260621/` covers AdditiveBatch1 loader/registration shape on Slay the Spire 2 `v0.107.1` with RitsuLib `v0.4.31` / `lib\0.107.1` and only STS2-RitsuLib as the shared runtime dependency: 25/25 Spire Plus patches, 10 event types / 14 registration calls, clean audit, retained enabled-mode verifier 31 / 0, and runtime packet verifier 61 / 0.'
Add-ContainsCheck -Name 'sts1_goal_historical_enabled_only' -Text $sts1FeatureGoal -Needle 'Historical `v0.106.1` CanaryOnly/AdditiveBatch1 loader evidence must stay historical'
Add-ContainsCheck -Name 'sts1_goal_compile_package_current_split' -Text $sts1FeatureGoal -Needle 'Events must compile against the current repository package reference: `STS2.RitsuLib` `0.4.31`; Spire Plus must not require previous package.'
Add-ContainsCheck -Name 'sts1_goal_runtime_ritsu_variant_split' -Text $sts1FeatureGoal -Needle 'Current runtime reproof targets the installed official `STS2-RitsuLib` `v0.4.31` variant pack with `lib\0.107.1`.'
Add-NoRegexCheck -Name 'sts1_goal_no_compile_against_ritsulib_0416_claim' -Paths @('docs\features\sts1-events\goal.md') -Pattern 'compile against current RitsuLib `0\.4\.16`'
Add-ContainsCheck -Name 'implementation_plan_revision_n_boundary' -Text $implementationPlan -Needle 'Current boundary, Revision P / beta.93'
Add-ContainsCheck -Name 'implementation_plan_default_off_only' -Text $implementationPlan -Needle 'Retained beta.85 `v0.107.0` loader evidence covers default-Off startup/patch application'
Add-ContainsCheck -Name 'implementation_plan_current_counts' -Text $implementationPlan -Needle 'Previous beta.93 proof under `.tools/runtime-evidence/v01071-beta93-ritsulib0431-off-direct-20260621/` and `.tools/runtime-evidence/v01071-beta93-ritsulib0431-additivebatch1-direct-20260621/` covers `v0.107.1` RitsuLib-only Off/AdditiveBatch1 loader/registration only with STS2-RitsuLib `v0.4.31`, 25/25 patches, clean audit, retained enabled-mode verifier 31 / 0, and AdditiveBatch1 runtime packet verifier 61 / 0.'
Add-ContainsCheck -Name 'canonical_matrix_duplicator_current_api_surface' -Text $canonicalEventMatrix -Needle 'CardSelectCmd.FromDeckForRewards and CardSelectorPrefs.DuplicateSelectionPrompt unavailable in current game/RitsuLib API surface'
Add-NoRegexCheck -Name 'canonical_matrix_no_stale_ritsulib_032_duplicator_blocker' -Paths @('docs\features\sts1-events\canonical-event-matrix.csv') -Pattern 'RitsuLib 0\.3\.2'

Add-ContainsCheck -Name 'feature_readme_localization_closure_plan' -Text $featureReadme -Needle 'localization-gap-closure-plan.md'
Add-ContainsCheck -Name 'feature_readme_v20_subagent_coverage_shape' -Text $featureReadme -Needle 'Static coverage ledger for the 15-role v20 subagent coverage shape while retaining the v19 filename'
Add-ContainsCheck -Name 'feature_readme_v20_final_gate_overlay' -Text $featureReadme -Needle 'Machine-readable O76-O84 final documentation and handoff overlay from the v20 stop condition'
Add-ContainsCheck -Name 'feature_readme_v20_hard_stop_report' -Text $featureReadme -Needle 'Current v20 coordination-pause hard-stop report and next-run start point'
Add-ContainsCheck -Name 'localization_doc_closure_plan' -Text $localizationDoc -Needle 'localization-gap-closure-plan.md'
Add-ContainsCheck -Name 'localization_doc_direct_key_not_additive_proof' -Text $localizationDoc -Needle 'Closing only `STS1_GOLDEN_IDOL.pages.LEAVE.description` remains a localization unblocker; it does not prove gameplay, and it does not replace `enabled-mode-log-check.json` and `runtime-evidence-packet-check.json` verifier reports.'
Add-ContainsCheck -Name 'test_plan_localization_closure_plan' -Text $testPlan -Needle 'docs/features/sts1-events/localization-gap-closure-plan.md'
Add-ContainsCheck -Name 'status_board_localization_closure_plan' -Text $statusBoard -Needle 'localization-gap-closure-plan.md'
Add-ContainsCheck -Name 'closure_plan_no_resource_change_scope' -Text $localizationClosurePlan -Needle 'This plan changes no localization/resource files.'
Add-ContainsCheck -Name 'closure_plan_pause_boundary' -Text $localizationClosurePlan -Needle 'Do not add resource keys while the validation coordination pause is active.'
Add-ContainsCheck -Name 'closure_plan_no_original_text_copy' -Text $localizationClosurePlan -Needle 'Do not copy original StS1 event text.'
Add-ContainsCheck -Name 'closure_plan_direct_key' -Text $localizationClosurePlan -Needle 'STS1_GOLDEN_IDOL.pages.LEAVE.description'
Add-ContainsCheck -Name 'closure_plan_remaining_32' -Text $localizationClosurePlan -Needle 'remaining 32 keys'
Add-ContainsCheck -Name 'closure_plan_versioning' -Text $localizationClosurePlan -Needle 'increment the Spire Plus package version'
Add-ContainsCheck -Name 'closure_plan_fail_on_missing' -Text $localizationClosurePlan -Needle '.\scripts\check-sts1-localization-source-keys.ps1 -FailOnMissing'
Add-ContainsCheck -Name 'closure_plan_pass_a_no_enabled_mode_proof' -Text $localizationClosurePlan -Needle 'Pass A does not by itself prove CanaryOnly or AdditiveBatch1 gameplay, save-load, or render behavior. It only removes the direct source-referenced missing-key blocker for those modes. It also does not replace retained enabled-mode log verifier or runtime evidence packets. O25 and O33 have loader/registration proof only; gameplay, localization render, save-load, image/render, replacement, multiplayer, QA, and handoff rows stay open.'
Add-ContainsCheck -Name 'closure_plan_runtime_canary_current_4_6' -Text $localizationClosurePlan -Needle 'Fresh current-version CanaryOnly enabled-mode smoke with 4 event types / 6 source registration calls if CanaryOnly-specific localization claims are made.'
Add-ContainsCheck -Name 'closure_plan_runtime_batch1_beta93_10_14' -Text $localizationClosurePlan -Needle 'Previous beta.93 `v0.107.1` AdditiveBatch1 enabled-mode smoke with 10 event types / 14 source registration calls, or a fresher package-matched recapture if the worktree/package changes.'
Add-ContainsCheck -Name 'closure_plan_behavior_cues' -Text $localizationClosurePlan -Needle '## Source Behavior Cues'
Add-ContainsCheck -Name 'closure_plan_all_key_cues_present' -Text $localizationClosurePlan -Needle 'STS1_WINDING_HALLS.pages.CONTINUE.description'
Add-ContainsCheck -Name 'closure_plan_blocked_combat_cue' -Text $localizationClosurePlan -Needle 'Blocked combat placeholder'
Add-ContainsCheck -Name 'localization_gap_scan_gap_baseline_12' -Text $localizationGapScan -Needle 'gap baseline checker: 12 checks / 0 mismatches'
Add-ContainsCheck -Name 'localization_gap_scan_closure_plan_coverage' -Text $localizationGapScan -Needle 'closure-plan cue coverage for all 33 missing keys'
Add-ContainsCheck -Name 'localization_gap_scan_direct_key_not_additive_proof' -Text $localizationGapScan -Needle 'Fixing the direct Golden Idol key only removes a missing-key blocker. It does not prove gameplay behavior or replace `enabled-mode-log-check.json` and `runtime-evidence-packet-check.json`.'
Add-ContainsCheck -Name 'test_plan_gap_baseline_closure_plan_coverage' -Text $testPlan -Needle 'closure-plan cue coverage for every missing key'
Add-ContainsCheck -Name 'test_plan_direct_localization_nonproof' -Text $testPlan -Needle 'Fixing the direct Golden Idol key only removes the missing-key blocker; it does not prove gameplay behavior or replace the enabled-mode log verifier/runtime evidence packet.'

Add-ContainsCheck -Name 'sts1_act_registration_research_revision_m_boundary' -Text $sts1ActEventRegistrationResearch -Needle '2026-06-18 Revision M Current Boundary'
Add-ContainsCheck -Name 'sts1_act_registration_research_source_not_gameplay' -Text $sts1ActEventRegistrationResearch -Needle 'This source-research note documents current intended StS1 registration shape and static source evidence. It is not current `v0.107.1` gameplay proof.'
Add-ContainsCheck -Name 'sts1_act_registration_research_beta93_nonclaim' -Text $sts1ActEventRegistrationResearch -Needle 'Previous beta.93 proves only RitsuLib-only Off and AdditiveBatch1 loader/registration behavior; beta.85/beta.87/beta.88/beta.90 rows remain previous-context evidence. CanaryOnly gameplay/runtime, save-load, replacement, multiplayer, QA, handoff, and release-ready proof still require fresh current evidence.'
Add-ContainsCheck -Name 'sts1_event_engine_research_revision_m_boundary' -Text $sts1EventEngineResearch -Needle '2026-06-18 Revision M Current Boundary'
Add-ContainsCheck -Name 'sts1_event_engine_research_source_not_gameplay' -Text $sts1EventEngineResearch -Needle 'This source-research note documents event-engine APIs and source patterns for StS1 event implementation. It is not current `v0.107.1` gameplay proof.'
Add-ContainsCheck -Name 'sts1_event_engine_research_beta93_nonclaim' -Text $sts1EventEngineResearch -Needle 'Previous beta.93 proves only RitsuLib-only Off and AdditiveBatch1 loader/registration behavior; beta.85/beta.87/beta.88/beta.90 rows remain previous-context evidence. CanaryOnly gameplay/runtime, save-load, replacement, multiplayer, QA, handoff, and release-ready proof still require fresh current evidence.'

Add-ContainsCheck -Name 'registry_title_57_14' -Text $registryReconciliation -Needle '# Registry Reconciliation - 52 / 54 / 50 / 48 / 47 / 57 / 14'
Add-ContainsCheck -Name 'registry_register_all_57' -Text $registryReconciliation -Needle '| RegisterAll calls | 57 |'
Add-ContainsCheck -Name 'registry_batch1_14' -Text $registryReconciliation -Needle '| AdditiveBatch1 calls | 14 |'

Add-ContainsCheck -Name 'status_board_current_enabled_boundary' -Text $statusBoard -Needle 'Previous beta.93 has clean RitsuLib-only `v0.107.1` Off and AdditiveBatch1 direct loader/registration proof with STS2-RitsuLib `v0.4.31`, 25/25 Spire Plus patches applied, 10 event types / 14 registration calls, and exact tuple parity.'
Add-ContainsCheck -Name 'status_board_allowed_status_chain_ascii' -Text $statusBoard -Needle 'planned -> spec-drafted -> wiki-verified -> api-verified -> implemented -> compiled -> test-guarded -> asset-mapped -> loc-render-verified -> manual-verified -> save-load-verified'
Add-RegexCheck -Name 'status_board_canary_current_pass' -Text $statusBoard -Pattern 'Canary runtime launch.*previous-package pass'
Add-RegexCheck -Name 'status_board_batch1_current_pass' -Text $statusBoard -Pattern 'AdditiveBatch1 runtime launch.*current loader/registration pass'
Add-ContainsCheck -Name 'status_board_diff_check_crlf_warning_boundary' -Text $statusBoard -Needle '`git diff --check --` exits clean; current pause-safe reruns emit CRLF normalization warnings only for existing tracked files and no whitespace errors.'
Add-ContainsCheck -Name 'status_board_pause_safe_static_v19_guards_20260615' -Text $statusBoard -Needle '| **v19 2026-06-15 pause-safe static verification** | `docs/reviews/current-validation.md` records static suite 14 / 0, then-current current-doc claims 872 / 0, static-file hygiene 11 / 0, v19 gate ledger 531 / 0, and `git diff --check --` exit 0 with CRLF warnings only; this is static evidence only. |'
Add-ContainsCheck -Name 'status_board_date_v20_static_alignment_20260617' -Text $statusBoard -Needle '> Last updated: 2026-06-21 (previous beta.93 `v0.107.1` RitsuLib-only Off and AdditiveBatch1 verifier packets captured and passed for loader/registration only; latest CanaryOnly-mode packet remains beta.85 previous-package context; gameplay and release proof remain pending)'
Add-ContainsCheck -Name 'status_board_v20_final_gate_overlay_20260617' -Text $statusBoard -Needle '| **v20 O76-O84 final-gate overlay** | `docs/features/sts1-events/v20-final-gate-overlay.csv` records final documentation, owner-action, no-unsupported-commit/push, release-claim, final-summary, and next-run boundaries from `docs/goals/event.md`; `scripts/check-sts1-v20-final-gate-overlay.ps1 -FailOnMismatch` is static/non-runtime evidence only. |'
Add-ContainsCheck -Name 'status_board_v20_hard_stop_report_20260617' -Text $statusBoard -Needle '| **v20 coordination-pause hard stop** | `docs/features/sts1-events/hard-stop-blocker-report-v20-coordination-pause-20260617.md` records the current O0-O84 pause reason, exact blocked/current-pending gates, owner actions, no unsupported commit/push, and next-run start point. It is not completion or runtime proof. |'
Add-ContainsCheck -Name 'status_board_pause_safe_static_v20_alignment_20260617' -Text $statusBoard -Needle '| **v20 2026-06-17/18 pause-safe static alignment** | The retained v20 static alignment remains static-only: static suite 15 / 0, beta.86 runtime preflight 27 / 0, static-file hygiene 11 / 0, v19 gate ledger 534 / 0, v20 final-gate overlay 29 / 0, and subagent coverage 70 / 0. Later active summary cleanup aligned current-doc claims to 962 / 0'
Add-ContainsCheck -Name 'status_board_pause_safe_static_v20_autoslay_1025' -Text $statusBoard -Needle 'runtime-monkey AutoSlay boundary/source-contract, packet-verifier, analyzer, and runtime `RuntimeLogGrowthRequired` / command-bearing `LogGrew` / no-log-growth-timeout hardening later raised the active current-doc guard to 1025 / 0 and static-file hygiene to 12 / 0 while preserving the same static-only boundary'
Add-ContainsCheck -Name 'status_board_pause_safe_static_v20_runtime_monkey_containment_1090' -Text $statusBoard -Needle 'the follow-up runtime monkey iteration-local artifact containment, packet escape-path, analyzer noncanonical-path, probe process identity, and AutoSlay malformed-path guards raised the current-doc guard to 1090 / 0'
Add-ContainsCheck -Name 'status_board_pause_safe_static_v20_autoslay_nonclaim' -Text $statusBoard -Needle 'does not close gameplay or game-native AutoSlay batch gates'
Add-ContainsCheck -Name 'status_board_v20_subagent_role_guard_20260617' -Text $statusBoard -Needle '| **v20 2026-06-17 subagent role guard** | `docs/features/sts1-events/v19-subagent-coverage.md` records the 15-role v20 subagent coverage shape while retaining the v19 filename; `scripts/check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returns 70 / 0 and remains static/non-runtime evidence only. |'
Add-ContainsCheck -Name 'status_board_build_beta93_boundary' -Text $statusBoard -Needle '| Build | beta.93 validated 0 errors / 0 warnings | `PROJECT_STATE.md` and `docs/dev-environment.md` record the beta.93 RitsuLib-only build as 0 errors / 0 warnings. This remains no-game build validation, not event gameplay proof. |'
Add-ContainsCheck -Name 'status_board_tests_beta93_boundary' -Text $statusBoard -Needle '| Tests | beta.93 current guard/focused lanes passed; retained split lane passed | `PROJECT_STATE.md` and `docs/reviews/current-validation.md` record the trusted split test strategy, migration-focused guard totals, current package/artifact validation status, and the latest runtime packet-checker recapture. This proves automated guard/package coverage only, not enabled-mode or gameplay proof. |'
Add-ContainsCheck -Name 'status_board_format_beta93_boundary' -Text $statusBoard -Needle '| Format | beta.93 post-migration format passed | `PROJECT_STATE.md` and `docs/reviews/current-validation.md` record format/diff-check/patch-inventory/batch-classifier checks passing after the beta.93 RitsuLib-only migration validation and current-doc alignment. This remains no-game validation and should be recaptured after future code, resource, package, or handoff changes. |'
Add-ContainsCheck -Name 'status_board_o14_source_identity_static' -Text $statusBoard -Needle '| CanaryOnly source identity (O14) | **static-pass / previous-package runtime pass** |'
Add-ContainsCheck -Name 'status_board_o15_source_identity_static' -Text $statusBoard -Needle '| AdditiveBatch1 source identity (O15) | **static-pass / current loader pass** |'
Add-ContainsCheck -Name 'status_board_direct_key_not_additive_proof' -Text $statusBoard -Needle 'Closing only `STS1_GOLDEN_IDOL.pages.LEAVE.description` remains a localization unblocker; it does not prove gameplay, and it does not replace `enabled-mode-log-check.json` and `runtime-evidence-packet-check.json` verifier reports.'
Add-ContainsCheck -Name 'status_board_replacement_o54_o57' -Text $statusBoard -Needle 'Replacement functional proof (O54-O57)'
Add-ContainsCheck -Name 'status_board_multiplayer_o58' -Text $statusBoard -Needle 'Multiplayer fail-closed (O58)'
Add-ContainsCheck -Name 'status_board_qa_o65' -Text $statusBoard -Needle 'QA Red-Team (O65)'
Add-ContainsCheck -Name 'status_board_gate_ledger' -Text $statusBoard -Needle 'v19 O0-O76 per-gate ledger'
Add-ContainsCheck -Name 'status_board_current_validation_summary_includes_current_validation' -Text $statusBoard -Needle '| Current validation summary | `PROJECT_STATE.md`, `docs/dev-environment.md`, `docs/reviews/current-validation.md` |'
Add-ContainsCheck -Name 'status_board_enabled_log_verifier' -Text $statusBoard -Needle 'No-launch enabled-mode log verifier'
Add-ContainsCheck -Name 'status_board_runtime_preflight_checker' -Text $statusBoard -Needle '| No-launch runtime preflight checker | `scripts/check-sts1-runtime-preflight.ps1 -FailOnMismatch` reads repo and installed manifests plus source-only expected shapes; it does not launch the game or close enabled-mode/runtime gates. |'
Add-ContainsCheck -Name 'status_board_enabled_log_verifier_audit_path' -Text $statusBoard -Needle '-AuditPath <future-audit>'
Add-ContainsCheck -Name 'status_board_enabled_log_verifier_requires_package_target' -Text $statusBoard -Needle 'enabled-mode copied logs must prove the expected Spire Plus package version, Ritsu compat branch, RitsuLib package version, and game version text in the log'
Add-ContainsCheck -Name 'status_board_enabled_log_verifier_outfile_report' -Text $statusBoard -Needle '-OutFile <future-evidence-dir>\enabled-mode-log-check.json'
Add-ContainsCheck -Name 'status_board_enabled_log_verifier_tuple_check' -Text $statusBoard -Needle 'It verifies registration-call count, event class set, and observed registration tuples parsed from `Registered act event` / `Registered shared event` lines when tuple details are present.'
Add-ContainsCheck -Name 'status_board_enabled_log_verifier_tuple_fallback_boundary' -Text $statusBoard -Needle 'If future logs lose act/shared tuple detail, Act-bucket proof remains source-derived until gameplay evidence proves those targets directly.'
Add-ContainsCheck -Name 'status_board_runtime_evidence_packet_verifier' -Text $statusBoard -Needle 'No-launch runtime evidence packet verifier'
Add-ContainsCheck -Name 'status_board_runtime_packet_mode_metadata' -Text $statusBoard -Needle 'matching `Sts1EventModeEnvironment` metadata'
Add-ContainsCheck -Name 'status_board_runtime_packet_requires_package_target' -Text $statusBoard -Needle 'explicit package/version target checks'
Add-ContainsCheck -Name 'status_board_runtime_packet_game_release_info' -Text $statusBoard -Needle 'matching `game-release-info.json`'
Add-ContainsCheck -Name 'status_board_runtime_packet_rejects_missing_state_bypass' -Text $statusBoard -Needle 'no `-AllowMissingSessionState` / `-AllowMissingRestoreState` bypass'
Add-ContainsCheck -Name 'status_board_runtime_packet_retains_log_check_json' -Text $statusBoard -Needle 'retained `enabled-mode-log-check.json`'
Add-ContainsCheck -Name 'status_board_runtime_packet_outfile_report' -Text $statusBoard -Needle '-OutFile <future-evidence-dir>\runtime-evidence-packet-check.json'
Add-ContainsCheck -Name 'status_board_off_packet_report_20260615' -Text $statusBoard -Needle '| **v19 beta.85 Off packet verifier report** | `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/runtime-evidence-packet-check.json` (no-launch rerun with explicit package/Ritsu/game targets: Off packet checks=34 / mismatches=0; nested log verifier checks=10 / mismatches=0; default-Off evidence only) |'
Add-ContainsCheck -Name 'status_board_subagent_packet_checklist_20260615' -Text $statusBoard -Needle '| Pause-safe subagent packet checklist | 2026-06-15 static pass | `docs/features/sts1-events/v19-subagent-coverage.md` now records future post-pause packet requirements for CanaryOnly, AdditiveBatch1, gameplay, localization/resource, replacement, multiplayer, QA, and release-doc role owners; `scripts/check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 63 / 0, the aggregate static suite stayed 14 / 0, current-doc claims returned 872 / 0, static-file hygiene returned 11 / 0, v19 gate ledger returned 531 / 0, and `git diff --check --` exited 0 with CRLF warnings only. This is no-launch/static evidence only. |'
Add-ContainsCheck -Name 'status_board_remaining_gate_split_20260615' -Text $statusBoard -Needle 'multiplayer runtime/ZHS rows (O58 and O64), independent QA (O65), and final owner/handoff rows (O72-O75). Static classification/safety rows (O59-O63), documentation-in-progress rows (O66-O71), and O76 do not close runtime or completion gates.'
Add-ContainsCheck -Name 'multiplayer_fail_closed_canary_current_4_6' -Text $multiplayerFailClosedGuard -Needle '`SPIREPLUS_STS1_EVENT_MODE=CanaryOnly` - verify 4 event types / 6 registration calls before any multiplayer claim'

Add-ContainsCheck -Name 'test_plan_enabled_log_verifier_expected_shape' -Text $testPlan -Needle '.\scripts\check-sts1-enabled-mode-runtime-log.ps1 -Mode CanaryOnly -PrintExpected'
Add-ContainsCheck -Name 'test_plan_canary_expected_4_6' -Text $testPlan -Needle 'CanaryOnly 4 event types / 6 source registration calls'
Add-ContainsCheck -Name 'test_plan_runtime_preflight_command' -Text $testPlan -Needle '.\scripts\check-sts1-runtime-preflight.ps1 -FailOnMismatch'
Add-ContainsCheck -Name 'test_plan_runtime_preflight_no_launch_boundary' -Text $testPlan -Needle 'It is a prerequisite check only; it does not launch the game, audit a runtime log, or prove enabled-mode runtime/gameplay.'
Add-ContainsCheck -Name 'test_plan_beta86_split_totals' -Text $testPlan -Needle 'Tests: split no-build lanes at the current accurate count, last recorded as 489 passed / 0 failed / 39 skipped / 528 total.'
Add-ContainsCheck -Name 'test_plan_beta86_artifact_subset_totals' -Text $testPlan -Needle 'Opt-in artifact subset: last recorded as 67 passed / 0 failed / 2 skipped / 69 total.'
Add-ContainsCheck -Name 'test_plan_manual_smoke_runtime_preflight_prereq' -Text $testPlan -Needle 'Run `.\scripts\check-sts1-runtime-preflight.ps1 -FailOnMismatch` and stop before launching if the installed game, RitsuLib, repo/installed Spire Plus package manifests, or source-only expected shapes do not match the documented post-pause package target.'
Add-ContainsCheck -Name 'test_plan_manual_smoke_canary_4_6' -Text $testPlan -Needle 'Preserve beta.85 Off and CanaryOnly proof as previous-package loader context only.'
Add-ContainsCheck -Name 'test_plan_manual_smoke_batch1_10_14' -Text $testPlan -Needle 'Treat `.tools\runtime-evidence\v01071-beta93-ritsulib0431-additivebatch1-direct-20260621` as previous beta.93 `v0.107.1` AdditiveBatch1 loader smoke only: 10 event types / 14 registration calls'
Add-ContainsCheck -Name 'test_plan_packet_current_slice_canonical' -Text $testPlan -Needle 'The packet verifier uses `godot.log.current-iteration` as the canonical enabled-mode log'
Add-ContainsCheck -Name 'test_plan_packet_derives_current_slice' -Text $testPlan -Needle 'derives the slice only when `godot.log.before` is a byte prefix of `godot.log.after-launch`'
Add-ContainsCheck -Name 'test_plan_packet_generates_current_slice_audit' -Text $testPlan -Needle 'generates `godot-log-current-iteration-audit.json`'
$testPlanLiveSessionPrepareCommand = ".\scripts\spire-plus-live-session.ps1 -Mode Prepare -EvidenceDir `$evidence -GameRoot 'E:\Steam\steamapps\common\Slay the Spire 2' -SteamExe 'E:\Steam\steam.exe' -SteamUserId `$steamUserId -MoveOtherMods -MoveCurrentRuns -Launch"
$testPlanLiveSessionPrepareCommandCount = [regex]::Matches($testPlan, [regex]::Escape($testPlanLiveSessionPrepareCommand)).Count
Add-Check -Name 'test_plan_live_session_prepare_commands_isolated_launch_count' -Passed ($testPlanLiveSessionPrepareCommandCount -eq 2) -Detail "requires two isolated live-session prepare commands with explicit E-drive paths, Steam user id, mod/run isolation, and launch; found $testPlanLiveSessionPrepareCommandCount"
$testPlanLiveSessionRestoreCommand = ".\scripts\spire-plus-live-session.ps1 -Mode Restore -EvidenceDir `$evidence -StopGameOnRestore -PreserveNewCurrentRunsOnRestore"
$testPlanLiveSessionRestoreCommandCount = [regex]::Matches($testPlan, [regex]::Escape($testPlanLiveSessionRestoreCommand)).Count
Add-Check -Name 'test_plan_live_session_restore_commands_safe_count' -Passed ($testPlanLiveSessionRestoreCommandCount -eq 2) -Detail "requires two live-session restore commands with StopGameOnRestore and PreserveNewCurrentRunsOnRestore; found $testPlanLiveSessionRestoreCommandCount"
Add-ContainsCheck -Name 'test_plan_enabled_log_verifier_non_claim' -Text $testPlan -Needle 'It is not runtime proof.'
Add-ContainsCheck -Name 'test_plan_enabled_log_verifier_requires_package_target' -Text $testPlan -Needle 'the packet verifier requires explicit `-ExpectedPackageVersion`, `-ExpectedRitsuCompatBranch`, `-ExpectedRitsuLibVersion`, and `-ExpectedGameVersion` checks for enabled-mode evidence'
Add-ContainsCheck -Name 'test_plan_enabled_log_verifier_registration_call_count' -Text $testPlan -Needle 'The copied-log verifier requires the observed registered event-line count to match the source-derived registration-call count'
Add-ContainsCheck -Name 'test_plan_enabled_log_verifier_tuple_check' -Text $testPlan -Needle 'observed `Registered act event` / `Registered shared event` tuples to match the source-derived tuple set when tuple detail is present'
Add-ContainsCheck -Name 'test_plan_enabled_log_verifier_tuple_fallback_boundary' -Text $testPlan -Needle 'If future logs lose act/shared tuple detail, Act-bucket proof remains source-derived until gameplay evidence proves those targets directly.'
Add-ContainsCheck -Name 'test_plan_hardcoded_beta85_future_version_boundary' -Text $testPlan -Needle 'Replace the hardcoded beta.86 package version with the newly built/installed package version after any versioned code, resource, localization, package, or handoff change.'
Add-ContainsCheck -Name 'test_plan_runtime_evidence_packet_off' -Text $testPlan -Needle '.\scripts\check-sts1-runtime-evidence-packet.ps1 -Mode AdditiveBatch1 -EvidenceDir ".tools\runtime-evidence\v01071-beta93-ritsulib0431-additivebatch1-direct-20260621" -LogFileName "godot.log.current-iteration" -ExpectedPackageVersion v0.1.0-private-beta.93 -ExpectedRitsuCompatBranch 0.107.1 -ExpectedRitsuLibVersion 0.4.31 -ExpectedGameVersion 0.107.1 -OutFile ".tools\runtime-evidence\v01071-beta93-ritsulib0431-additivebatch1-direct-20260621\runtime-evidence-packet-check.json" -FailOnMismatch'
Add-ContainsCheck -Name 'test_plan_runtime_evidence_packet_canary' -Text $testPlan -Needle '.\scripts\check-sts1-runtime-evidence-packet.ps1 -Mode CanaryOnly -EvidenceDir $evidence -ExpectedPackageVersion v0.1.0-private-beta.97 -ExpectedRitsuCompatBranch 0.107.1 -ExpectedRitsuLibVersion 0.4.31 -ExpectedGameVersion 0.107.1 -OutFile "$evidence\runtime-evidence-packet-check.json" -FailOnMismatch'
Add-ContainsCheck -Name 'test_plan_runtime_evidence_packet_batch1' -Text $testPlan -Needle '.\scripts\check-sts1-runtime-evidence-packet.ps1 -Mode AdditiveBatch1 -EvidenceDir $evidence -ExpectedPackageVersion v0.1.0-private-beta.97 -ExpectedRitsuCompatBranch 0.107.1 -ExpectedRitsuLibVersion 0.4.31 -ExpectedGameVersion 0.107.1 -OutFile "$evidence\runtime-evidence-packet-check.json" -FailOnMismatch'
Add-ContainsCheck -Name 'test_plan_runtime_evidence_packet_no_launch' -Text $testPlan -Needle 'This validates the packet shape, StS1 mode environment metadata, and nested log/audit result; it does not launch the game.'
Add-ContainsCheck -Name 'test_plan_runtime_packet_mode_metadata' -Text $testPlan -Needle 'Current helper-created `session-state.json` records `Sts1EventModeEnvironment`'
Add-ContainsCheck -Name 'test_plan_runtime_packet_rejects_unsafe_leakage' -Text $testPlan -Needle 'rejects unsafe-mode environment leakage'
Add-ContainsCheck -Name 'test_plan_runtime_packet_rejects_missing_state_bypass' -Text $testPlan -Needle 'enabled-mode packet checks must not use `-AllowMissingSessionState` or `-AllowMissingRestoreState`'
Add-ContainsCheck -Name 'test_plan_runtime_packet_requires_package_target' -Text $testPlan -Needle 'the packet verifier requires explicit `-ExpectedPackageVersion`, `-ExpectedRitsuCompatBranch`, `-ExpectedRitsuLibVersion`, and `-ExpectedGameVersion` checks for enabled-mode evidence'
Add-ContainsCheck -Name 'test_plan_runtime_packet_keeps_game_release_info' -Text $testPlan -Needle 'keep `session-state.json`, `settings.save.before`, `game-release-info.json`'
Add-ContainsCheck -Name 'test_plan_runtime_packet_keeps_verifier_reports' -Text $testPlan -Needle 'enabled-mode-log-check.json`, `runtime-evidence-packet-check.json`'
Add-ContainsCheck -Name 'test_plan_v19_gate_ledger_checker' -Text $testPlan -Needle '.\scripts\check-sts1-v19-gate-ledger.ps1 -FailOnMismatch'
Add-ContainsCheck -Name 'test_plan_v19_gate_ledger_csv' -Text $testPlan -Needle 'The ledger file is `docs/features/sts1-events/v19-gate-ledger.csv`.'
Add-ContainsCheck -Name 'test_plan_v20_final_gate_overlay_checker' -Text $testPlan -Needle '.\scripts\check-sts1-v20-final-gate-overlay.ps1 -FailOnMismatch'
Add-ContainsCheck -Name 'test_plan_v20_final_gate_overlay_non_replacement' -Text $testPlan -Needle 'This does not replace the v19 O0-O76 ledger; it tracks the v20 final documentation, owner-action, no-unsupported-commit/push, release-claim, final-summary, and next-run boundaries without closing runtime or handoff gates.'
Add-ContainsCheck -Name 'test_plan_v20_hard_stop_trace' -Text $testPlan -Needle 'The current v20 hard-stop trace is `docs/features/sts1-events/hard-stop-blocker-report-v20-coordination-pause-20260617.md`.'
Add-ContainsCheck -Name 'test_plan_v19_historical_trace_boundary' -Text $testPlan -Needle 'the v19 trace at `docs/features/sts1-events/hard-stop-blocker-report-v19-validation-coordination-20260611.md` is historical beta.85/v0.107.0 coordination-blocker context only'
Add-ContainsCheck -Name 'test_plan_v19_subagent_checker' -Text $testPlan -Needle '.\scripts\check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch'
Add-ContainsCheck -Name 'test_plan_v19_subagent_non_claim' -Text $testPlan -Needle 'This verifies all 15 `docs/goals/event.md` subagent roles are represented without treating read-only/static subagent work as runtime, gameplay, or QA proof.'
Add-ContainsCheck -Name 'test_plan_replacement_o53_o57_split' -Text $testPlan -Needle '`O54-O57` (replacement functional proof)'
Add-ContainsCheck -Name 'test_plan_replacement_o53_source_guard_split' -Text $testPlan -Needle '`O53` is source-guarded only, `O59-O63` are static classification/safety rows, `O66-O71` are documentation-in-progress rows, and `O76` is a static non-completion invariant; none of those static/documentation rows close runtime or handoff gates.'
Add-ContainsCheck -Name 'test_plan_multiplayer_o58_o64_split' -Text $testPlan -Needle '`O58` and `O64` (multiplayer/ZHS runtime rows)'

Add-ContainsCheck -Name 'gate_map_o0_o76_scope' -Text $gateMap -Needle 'Scope: `docs/goals/event.md` Mandatory Overnight Run v19/v21 carry-forward, previous `v0.107.1` / beta.93 loader truth.'
Add-ContainsCheck -Name 'gate_map_o11_o20_static_suite' -Text $gateMap -Needle '| `O11-O20` | static source/doc pass for current count and gate shape |'
Add-ContainsCheck -Name 'gate_map_o21_o24_off_pass' -Text $gateMap -Needle '| `O21-O24` | previous beta.93 RitsuLib-only loader pass |'
Add-ContainsCheck -Name 'gate_map_o25_o29_mixed_current_pass' -Text $gateMap -Needle '| `O25-O29` | previous-package Canary pass / gameplay blocked |'
Add-ContainsCheck -Name 'gate_map_o53_o57_replacement_blocked' -Text $gateMap -Needle '| `O53-O57` | source-guarded only / functional proof blocked |'
Add-ContainsCheck -Name 'gate_map_o58_o64_runtime_static_split' -Text $gateMap -Needle '| `O58-O64` | runtime rows blocked / static classification rows pass | `O59-O63` have static multiplayer-shape, parity-blocker, asset-safety, and localization-source coverage; combat blockers and temporary substitutes are documented. `O58` multiplayer runtime proof and `O64` ZHS screenshots remain blocked. | Multiplayer fail-closed runtime proof and ZHS screenshots after enabled-mode/gameplay evidence; keep static classification/safety rows from being treated as runtime proof. |'
Add-ContainsCheck -Name 'gate_map_o65_o76_qa_docs_owner_split' -Text $gateMap -Needle '| `O65-O76` | QA/final gates blocked or current-pending / docs in progress / invariant static | `O66-O71` are documentation-in-progress; `O76` preserves the non-completion invariant. `O65` independent QA and `O72-O75` owner/final handoff gates remain current-pending or blocked. | Independent QA/Red-Team after runtime evidence, documentation refresh from evidence paths, owner action decisions, unsupported commit/push prevention, final blocked-gate summary, and all gates green before completion. |'
Add-ContainsCheck -Name 'gate_map_static_suite_15' -Text $gateMap -Needle 'static_suite_steps=15'
Add-ContainsCheck -Name 'gate_map_static_suite_file_hygiene' -Text $gateMap -Needle 'static-file hygiene'
Add-ContainsCheck -Name 'gate_map_static_suite_subagent_coverage' -Text $gateMap -Needle 'v19 subagent coverage'
Add-ContainsCheck -Name 'gate_map_date_current_static_governance' -Text $gateMap -Needle 'Date: 2026-06-15 (v19 opened 2026-06-11; latest pause-safe static-governance update)'
Add-ContainsCheck -Name 'gate_map_non_claim_historical_enabled' -Text $gateMap -Needle 'Historical `v0.106.1` CanaryOnly/AdditiveBatch1 proof must not be used as current `v0.107.1` enabled-mode proof.'
Add-ContainsCheck -Name 'gate_map_enabled_log_verifier_non_claim' -Text $gateMap -Needle 'They do not close gameplay gates; `O33` is closed only as loader-registration proof by the previous beta.93 AdditiveBatch1 packet'
Add-ContainsCheck -Name 'gate_map_runtime_evidence_packet_verifier' -Text $gateMap -Needle 'check-sts1-runtime-evidence-packet.ps1'
Add-ContainsCheck -Name 'gate_map_runtime_evidence_packet_off_game_version' -Text $gateMap -Needle '.\scripts\check-sts1-runtime-evidence-packet.ps1 -Mode AdditiveBatch1 -EvidenceDir ".tools\runtime-evidence\v01071-beta93-ritsulib0431-additivebatch1-direct-20260621" -LogFileName "godot.log.current-iteration" -ExpectedPackageVersion v0.1.0-private-beta.93 -ExpectedRitsuCompatBranch 0.107.1 -ExpectedRitsuLibVersion 0.4.31 -ExpectedGameVersion 0.107.1 -OutFile ".tools\runtime-evidence\v01071-beta93-ritsulib0431-additivebatch1-direct-20260621\runtime-evidence-packet-check.json" -FailOnMismatch'
Add-ContainsCheck -Name 'gate_map_off_packet_rerun_20260615' -Text $gateMap -Needle '2026-06-15 pause-safe rerun: the Off packet command above was run against the already-captured beta.85 evidence folder with explicit package, Ritsu compat-branch, RitsuLib package-version, and game-version targets; it returned packet `checks=34`, `mismatches=0`, with nested log verifier `checks=10`, `mismatches=0`; the retained verifier report is `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/runtime-evidence-packet-check.json`.'
Add-ContainsCheck -Name 'gate_map_off_packet_rerun_non_claim' -Text $gateMap -Needle 'This only re-verifies existing default-Off evidence; it did not close enabled-mode or gameplay gates by itself.'
Add-ContainsCheck -Name 'gate_map_pause_safe_static_guard_rerun_20260615' -Text $gateMap -Needle '2026-06-15 pause-safe static guard rerun: `scripts/check-sts1-event-static-suite.ps1` completed 14 static steps with 0 suite failures and kept the 33-key localization gap known/non-failing; `scripts/check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 715 checks / 0 mismatches in that pass, later superseded by the 872-check follow-up below; `scripts/check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches; `git diff --check --` exited 0 with CRLF normalization warnings only for `AGENTS.md` and `docs/goals/refactor.md`. This is no-launch/static evidence only.'
Add-ContainsCheck -Name 'gate_map_subagent_packet_checklist_guard_rerun_20260615' -Text $gateMap -Needle '2026-06-15 pause-safe subagent packet checklist hardening: `scripts/check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 61 checks / 0 mismatches after adding future packet requirements for RitsuLib package-version checks, retained verifier reports, no legacy enabled-mode packet bypasses, and the class-only runtime-log limitation. The follow-up owner/final-handoff non-authorization static rerun returned static suite 14 / 0, current-doc claims 793 / 0, v19 gate ledger 531 / 0, subagent coverage 63 / 0, and `git diff --check --` exit 0 with CRLF warnings only; this was later superseded by the direct enabled-mode audit-path guard below. This is no-launch/static evidence only and does not close any runtime gate.'
Add-ContainsCheck -Name 'gate_map_static_suite_composition_guard_20260615' -Text $gateMap -Needle '2026-06-15 pause-safe static-suite composition hardening: `scripts/check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` now guards the aggregate static suite''s full 14-step composition, the fail-closed wrappers for guard scripts, and the suite step/failure summary output. The follow-up static rerun returned static suite 14 / 0, current-doc claims 832 / 0, static-file hygiene 7 / 0, v19 gate ledger 531 / 0, subagent coverage 63 / 0, and focused `git diff --check --` exit 0; this was later superseded by the retained audit-log command guard below. This is no-launch/static evidence only and does not close any runtime gate.'
Add-ContainsCheck -Name 'gate_map_audit_log_command_guard_20260615' -Text $gateMap -Needle '2026-06-15 pause-safe retained audit-log command hardening: `scripts/check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` now guards active `audit-godot-log.ps1` command examples so future enabled-mode smoke instructions keep `-OutFile`, `-FailOnHit`, and the retained `godot-log-audit.json` report, with a count check covering both CanaryOnly and AdditiveBatch1 smoke sections. The follow-up static rerun returned static suite 14 / 0, current-doc claims 838 / 0, static-file hygiene 7 / 0, v19 gate ledger 531 / 0, subagent coverage 63 / 0, and focused `git diff --check --` exit 0; this was later superseded by the live-session command guard below. This is no-launch/static evidence only and does not close any runtime gate.'
Add-ContainsCheck -Name 'gate_map_live_session_command_guard_20260615' -Text $gateMap -Needle '2026-06-15 pause-safe live-session command hardening: `scripts/check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` now guards active `spire-plus-live-session.ps1` command examples so future enabled-mode smoke instructions keep explicit E-drive `-GameRoot` / `-SteamExe`, `-SteamUserId`, `-MoveOtherMods`, `-MoveCurrentRuns`, `-Launch`, `-StopGameOnRestore`, and `-PreserveNewCurrentRunsOnRestore`; count checks cover both CanaryOnly and AdditiveBatch1 smoke sections. The follow-up static rerun returned static suite 14 / 0, current-doc claims 850 / 0, static-file hygiene 7 / 0, v19 gate ledger 531 / 0, subagent coverage 63 / 0, and focused `git diff --check --` exit 0; this was later superseded by the runtime-smoke checklist prerequisite guard below. This is no-launch/static evidence only and does not close any runtime gate.'
Add-ContainsCheck -Name 'gate_map_runtime_smoke_checklist_live_session_prereq_guard_20260615' -Text $gateMap -Needle '2026-06-15 pause-safe runtime-smoke checklist live-session prerequisite guard: `docs/features/ritsulib-migration/runtime-smoke-checklist.md` now requires helper prepare with explicit E-drive `-GameRoot` / `-SteamExe`, `-SteamUserId`, `-Mode Prepare -MoveOtherMods -MoveCurrentRuns -Launch`, restore with `-Mode Restore -StopGameOnRestore -PreserveNewCurrentRunsOnRestore`, and keeping `STS2-RitsuLib` enabled. The follow-up static rerun returned static suite 14 / 0, current-doc claims 856 / 0, static-file hygiene 7 / 0, v19 gate ledger 531 / 0, subagent coverage 63 / 0, and focused `git diff --check --` exit 0; this was later superseded by the runtime-smoke checklist scan-scope guard below. This is no-launch/static evidence only and does not close any runtime gate.'
Add-ContainsCheck -Name 'gate_map_runtime_smoke_checklist_scan_scope_guard_20260615' -Text $gateMap -Needle '2026-06-15 pause-safe runtime-smoke checklist scan-scope guard: `scripts/check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` now directly asserts that `docs/features/ritsulib-migration/runtime-smoke-checklist.md` is part of the broad current-claim stale scan, so its future command examples and current/historical runtime claims stay under the same no-overclaim rules as the StS1 event docs. The follow-up static rerun returned static suite 14 / 0, current-doc claims 859 / 0, static-file hygiene 7 / 0, v19 gate ledger 531 / 0, subagent coverage 63 / 0, and focused `git diff --check --` exit 0; this was later superseded by the runtime-smoke checklist static-file hygiene scope guard below. This is no-launch/static evidence only and does not close any runtime gate.'
Add-ContainsCheck -Name 'gate_map_runtime_smoke_checklist_static_file_hygiene_scope_guard_20260615' -Text $gateMap -Needle '2026-06-15 pause-safe runtime-smoke checklist static-file hygiene scope guard: `scripts/check-sts1-static-file-hygiene.ps1 -FailOnMismatch` now directly asserts that `docs/features/ritsulib-migration/runtime-smoke-checklist.md` is in static-file hygiene scope, so whitespace, final-newline, NUL-byte, and replacement-character checks cover that active runtime checklist. The follow-up static rerun returned static suite 14 / 0, current-doc claims 863 / 0, static-file hygiene 8 / 0, v19 gate ledger 531 / 0, subagent coverage 63 / 0, and focused `git diff --check --` exit 0; this was later superseded by the next-overnight scope guard below. This is no-launch/static evidence only and does not close any runtime gate.'
Add-ContainsCheck -Name 'gate_map_next_overnight_scope_guard_20260615' -Text $gateMap -Needle '2026-06-15 pause-safe next-overnight runtime-plan scope guard: `scripts/check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` now directly asserts that `docs/features/ritsulib-migration/next-overnight-run.md` is in broad current-claim stale-scan scope, and `scripts/check-sts1-static-file-hygiene.ps1 -FailOnMismatch` now directly asserts that it is in static-file hygiene scope. The follow-up static rerun returned static suite 14 / 0, current-doc claims 868 / 0, static-file hygiene 9 / 0, v19 gate ledger 531 / 0, subagent coverage 63 / 0, and focused `git diff --check --` exit 0; this was later superseded by the RitsuLib planning-doc hygiene guard below. This is no-launch/static evidence only and does not close any runtime gate.'
Add-ContainsCheck -Name 'gate_map_ritsu_planning_doc_static_file_hygiene_guard_20260615' -Text $gateMap -Needle '2026-06-15 pause-safe RitsuLib planning-doc static-file hygiene guard: `scripts/check-sts1-static-file-hygiene.ps1 -FailOnMismatch` now directly asserts that `docs/features/ritsulib-migration/monthly-dev-spec.md` and `docs/features/ritsulib-migration/batch-4c-candidates.md` are in static-file hygiene scope. The follow-up static rerun returned static suite 14 / 0, current-doc claims 872 / 0, static-file hygiene 11 / 0, v19 gate ledger 531 / 0, subagent coverage 63 / 0, and focused `git diff --check --` exit 0. This is no-launch/static evidence only and does not close any runtime gate.'
Add-ContainsCheck -Name 'gate_map_v20_subagent_alignment_20260617' -Text $gateMap -Needle 'Later active summary cleanup after `eaaeb5a1` updated current-doc summary counts to 962 / 0 in `PROJECT_STATE.md`, `docs/goals/event.md`, `docs/reviews/current-validation.md`, the status board, this gate map, and the current-doc guard; runtime-monkey AutoSlay boundary/source-contract, packet-verifier, analyzer, and runtime `RuntimeLogGrowthRequired` / command-bearing `LogGrew` / no-log-growth-timeout hardening later raised the active current-doc guard to 1025 / 0 and static-file hygiene to 12 / 0 while preserving the same static-only boundary.'
Add-ContainsCheck -Name 'gate_map_v20_runtime_monkey_containment_1090' -Text $gateMap -Needle 'the follow-up runtime monkey iteration-local artifact containment, packet escape-path, analyzer noncanonical-path, probe process identity, and AutoSlay malformed-path guards raised the current-doc guard to 1090 / 0'
Add-ContainsCheck -Name 'gate_map_v20_autoslay_nonclaim' -Text $gateMap -Needle 'does not close gameplay or game-native AutoSlay batch gates'
Add-ContainsCheck -Name 'gate_map_v20_final_overlay_note' -Text $gateMap -Needle 'v20 O76-O84 final-gate overlay'
Add-ContainsCheck -Name 'gate_map_v20_hard_stop_report_note' -Text $gateMap -Needle '2026-06-17 v20 coordination-pause hard stop: `docs/features/sts1-events/hard-stop-blocker-report-v20-coordination-pause-20260617.md` records the current O0-O84 blocked/current-pending gates'
Add-ContainsCheck -Name 'gate_map_v20_hard_stop_non_claim' -Text $gateMap -Needle 'The v20 hard-stop report is also a pause record, not a completion claim.'
Add-ContainsCheck -Name 'gate_map_o0_o10_beta86_split_totals' -Text $gateMap -Needle '| `O0-O10` | beta.86 package/build/post-doc split pass / post-baseline recapture pending | `PROJECT_STATE.md` records beta.86 build 0 errors / 0 warnings, publish/package refresh, installed package parity, opt-in artifact subset 67 / 0 / 2 / 69, split test lanes 489 / 0 / 39 / 528'
Add-ContainsCheck -Name 'gate_map_runtime_packet_mode_metadata' -Text $gateMap -Needle 'matching `Sts1EventModeEnvironment` metadata'
Add-ContainsCheck -Name 'gate_map_enabled_log_requires_package_target' -Text $gateMap -Needle 'Future smoke logs must be checked with `-LogPath`, `-AuditPath`, `-ExpectedPackageVersion`, `-ExpectedRitsuCompatBranch`, `-ExpectedRitsuLibVersion`, `-ExpectedGameVersion`, and `-OutFile <evidence>\enabled-mode-log-check.json`'
Add-ContainsCheck -Name 'gate_map_packet_requires_game_target' -Text $gateMap -Needle 'explicit package/Ritsu-compat/RitsuLib-version/game-version targets'
Add-ContainsCheck -Name 'gate_map_packet_rejects_missing_state_bypass' -Text $gateMap -Needle 'no `-AllowMissingSessionState` / `-AllowMissingRestoreState` bypass'
Add-ContainsCheck -Name 'gate_map_enabled_log_tuple_check' -Text $gateMap -Needle 'The copied-log verifier checks registration-call count, class set, and observed registration tuples parsed from `Registered act event` / `Registered shared event` lines when tuple detail is present.'
Add-ContainsCheck -Name 'gate_map_enabled_log_tuple_fallback_boundary' -Text $gateMap -Needle 'If future logs lose act/shared tuple detail, Act-bucket registration tuples remain source-derived review output until gameplay evidence proves those targets directly.'
Add-ContainsCheck -Name 'gate_map_packet_outfile_report' -Text $gateMap -Needle '`-OutFile <evidence>\runtime-evidence-packet-check.json`'
Add-ContainsCheck -Name 'gate_map_v19_gate_ledger' -Text $gateMap -Needle 'v19-gate-ledger.csv'
Add-ContainsCheck -Name 'gate_map_o30_o41_mixed' -Text $gateMap -Needle '| `O30-O41` | mixed static/current-pass/blocked |'

Add-ContainsCheck -Name 'gate_ledger_o25_current_pass' -Text $gateLedger -Needle 'O25,O21-O29,Retained v0.107 CanaryOnly enabled-mode smoke exact 4 event types / 6 calls,previous-package-pass'
Add-ContainsCheck -Name 'v20_overlay_o76' -Text $v20FinalGateOverlay -Needle 'O76,O76-O84,current-validation updated,documentation-in-progress'
Add-ContainsCheck -Name 'v20_overlay_o81_pause_boundary' -Text $v20FinalGateOverlay -Needle 'O81,O76-O84,no unsupported commit/push,current-pending,Coordination pause forbids commit/push from this thread; package/runtime baseline remains eaaeb5a1 and later governance/test follow-up commits require exact HEAD/worktree recapture'
Add-ContainsCheck -Name 'v20_overlay_o82_release_claim_boundary' -Text $v20FinalGateOverlay -Needle 'O82,O76-O84,release-ready claim absent unless gates pass,static-pass,Current docs explicitly say release/live ready no and gates remain blocked'
Add-ContainsCheck -Name 'v20_overlay_o83_hard_stop_summary' -Text $v20FinalGateOverlay -Needle 'O83,O76-O84,final summary states blocked gates honestly,static-pass,docs/features/sts1-events/hard-stop-blocker-report-v20-coordination-pause-20260617.md lists unresolved gates and says O83 is not completion'
Add-ContainsCheck -Name 'v20_overlay_o84_next_run_boundary' -Text $v20FinalGateOverlay -Needle 'O84,O76-O84,next-run start point lists unresolved gates only,static-pass,docs/features/sts1-events/hard-stop-blocker-report-v20-coordination-pause-20260617.md starts the next run from validation recapture and downstream runtime gates while preserving retained loader proof'
Add-ContainsCheck -Name 'v20_overlay_checker_expected_ids' -Text $v20FinalGateOverlayCheckerScript -Needle '$expectedIds = @(76..84 | ForEach-Object { "O$_" })'
Add-ContainsCheck -Name 'v20_overlay_checker_no_runtime_completion_status' -Text $v20FinalGateOverlayCheckerScript -Needle 'v20_overlay_no_runtime_completion_status'
Add-ContainsCheck -Name 'v20_overlay_checker_o82_static_pass' -Text $v20FinalGateOverlayCheckerScript -Needle "O82 = 'static-pass'"
Add-ContainsCheck -Name 'v20_overlay_checker_o83_static_pass' -Text $v20FinalGateOverlayCheckerScript -Needle "O83 = 'static-pass'"
Add-ContainsCheck -Name 'v20_overlay_checker_o84_static_pass' -Text $v20FinalGateOverlayCheckerScript -Needle "O84 = 'static-pass'"
Add-ContainsCheck -Name 'gate_map_canary_expected_4_6' -Text $gateMap -Needle 'beta.85 CanaryOnly proof at `.tools/runtime-evidence/v01070-beta85-canary-20260617-233621/` reached main menu, audited clean, and retained log/packet verifiers passed 4 event types / 6 registration calls.'
Add-ContainsCheck -Name 'gate_ledger_checker_o0_beta93_baseline_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O0'; Field = 'evidence'; Needle = 'PROJECT_STATE.md records beta.93 RitsuLib-only package/runtime baseline and requires exact HEAD/worktree recapture for later governance/test follow-up commits'"
Add-ContainsCheck -Name 'gate_ledger_checker_o1_rerun_build_action' -Text $gateLedgerCheckerScript -Needle "GateId = 'O1'; Field = 'next_action'; Needle = 'Rerun build after any code/config change or before handoff'"
Add-ContainsCheck -Name 'gate_ledger_checker_o2_split_lanes_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O2'; Field = 'evidence'; Needle = 'PROJECT_STATE.md records current trusted split/focused lanes plus retained split coverage 475 / 0 / 21 / 496'"
Add-ContainsCheck -Name 'gate_ledger_checker_o3_skipped_tests_action' -Text $gateLedgerCheckerScript -Needle "GateId = 'O3'; Field = 'next_action'; Needle = 'Refresh skipped-test explanation with current test output before handoff'"
Add-ContainsCheck -Name 'gate_ledger_checker_o4_zero_warning_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O4'; Field = 'evidence'; Needle = 'PROJECT_STATE.md records beta.93 zero-warning validation'"
Add-ContainsCheck -Name 'gate_ledger_checker_o5_format_action' -Text $gateLedgerCheckerScript -Needle "GateId = 'O5'; Field = 'next_action'; Needle = 'Rerun dotnet format after edits or before handoff'"
Add-ContainsCheck -Name 'gate_ledger_checker_o6_diff_static_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O6'; Field = 'evidence'; Needle = 'git diff --check is static and rerun during coordination pause'"
Add-ContainsCheck -Name 'gate_ledger_checker_o7_patch_inventory_action' -Text $gateLedgerCheckerScript -Needle "GateId = 'O7'; Field = 'next_action'; Needle = 'Refresh patch inventory after coordination pause'"
Add-ContainsCheck -Name 'gate_ledger_checker_o8_batch_classifier_action' -Text $gateLedgerCheckerScript -Needle "GateId = 'O8'; Field = 'next_action'; Needle = 'Refresh classifier after coordination pause'"
Add-ContainsCheck -Name 'gate_ledger_checker_o9_beta93_zip_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O9'; Field = 'evidence'; Needle = 'ZIP SHA 56636753F598B360B3798ED681ED84C3CA08CEC173E7EBA70134F4BC68EF964A'"
Add-ContainsCheck -Name 'gate_ledger_checker_o10_dirty_worktree_scope_action' -Text $gateLedgerCheckerScript -Needle "GateId = 'O10'; Field = 'next_action'; Needle = 'Owner/agent must classify exact scope before commit or handoff'"
Add-ContainsCheck -Name 'gate_ledger_checker_o11_status_board_no_done_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O11'; Field = 'requirement'; Needle = 'Status board current and no generic Done'"
Add-ContainsCheck -Name 'gate_ledger_checker_o13_off_not_enabled_action' -Text $gateLedgerCheckerScript -Needle "GateId = 'O13'; Field = 'next_action'; Needle = 'Do not extend Off proof to enabled modes'"
Add-ContainsCheck -Name 'gate_ledger_checker_o14_4_6_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O14'; Field = 'requirement'; Needle = 'CanaryOnly source identity exact 4 event types / 6 calls'"
Add-ContainsCheck -Name 'gate_ledger_checker_o14_source_identity_action' -Text $gateLedgerCheckerScript -Needle "GateId = 'O14'; Field = 'next_action'; Needle = 'Keep source identity aligned with runtime verifier expectations'"
Add-ContainsCheck -Name 'gate_ledger_checker_o25_4_6_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O25'; Field = 'requirement'; Needle = 'Retained v0.107 CanaryOnly enabled-mode smoke exact 4 event types / 6 calls'"
Add-ContainsCheck -Name 'gate_ledger_checker_o15_10_14_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O15'; Field = 'requirement'; Needle = 'AdditiveBatch1 source identity exact 10 types / 14 calls'"
Add-ContainsCheck -Name 'gate_ledger_checker_o15_runtime_proof_action' -Text $gateLedgerCheckerScript -Needle "GateId = 'O15'; Field = 'next_action'; Needle = 'Keep source identity aligned with runtime verifier expectations'"
Add-ContainsCheck -Name 'gate_ledger_checker_o16_unsafe_gate_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O16'; Field = 'requirement'; Needle = 'AdditiveAllDraft unsafe gate source guarded'"
Add-ContainsCheck -Name 'gate_ledger_checker_o17_replacement_runtime_action' -Text $gateLedgerCheckerScript -Needle "GateId = 'O17'; Field = 'next_action'; Needle = 'Runtime replacement proof still required'"
Add-ContainsCheck -Name 'gate_ledger_checker_o18_spec_notes_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O18'; Field = 'requirement'; Needle = 'Per-event spec registration notes current'"
Add-ContainsCheck -Name 'gate_ledger_checker_o19_33_key_gap_action' -Text $gateLedgerCheckerScript -Needle "GateId = 'O19'; Field = 'next_action'; Needle = 'Close 33-key gap only in versioned resource pass'"
Add-ContainsCheck -Name 'gate_ledger_checker_o20_static_suite_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O20'; Field = 'evidence'; Needle = 'check-sts1-event-static-suite.ps1'"
Add-ContainsCheck -Name 'gate_ledger_checker_o21_v01071_path_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O21'; Field = 'requirement'; Needle = 'Current v0.107.1 game path and dependency path recorded'"
Add-ContainsCheck -Name 'gate_ledger_checker_o22_ritsulib_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O22'; Field = 'requirement'; Needle = 'STS2-RitsuLib v0.4.31 with lib/0.107.1 installed'"
Add-ContainsCheck -Name 'gate_ledger_checker_o23_current_log_path_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O23'; Field = 'evidence'; Needle = '.tools/runtime-evidence/v01071-beta93-ritsulib0431-additivebatch1-direct-20260621/godot.log.current-iteration'"
Add-ContainsCheck -Name 'gate_ledger_checker_o24_preserve_current_additive_action' -Text $gateLedgerCheckerScript -Needle "GateId = 'O24'; Field = 'next_action'; Needle = 'Preserve as current RitsuLib-only AdditiveBatch1 loader proof only'"
Add-ContainsCheck -Name 'gate_ledger_checker_o24_packet_report_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O24'; Field = 'evidence'; Needle = 'runtime-evidence-packet-check.json checks=61 mismatches=0'"
Add-ContainsCheck -Name 'gate_ledger_checker_o33_10_14_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O33'; Field = 'requirement'; Needle = 'Current v0.107.1 AdditiveBatch1 enabled-mode smoke exact 10 types / 14 calls'"
Add-ContainsCheck -Name 'gate_ledger_checker_o25_preserve_canary_loader_action' -Text $gateLedgerCheckerScript -Needle "GateId = 'O25'; Field = 'next_action'; Needle = 'Preserve as beta.85 CanaryOnly loader proof only'"
Add-ContainsCheck -Name 'gate_ledger_checker_o26_big_fish_runtime_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O26'; Field = 'requirement'; Needle = 'Big Fish runtime screenshot/result proof'"
Add-ContainsCheck -Name 'gate_ledger_checker_o26_no_encounter_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O26'; Field = 'evidence'; Needle = 'No current encounter screenshot or result log'"
Add-ContainsCheck -Name 'gate_ledger_checker_o27_golden_idol_runtime_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O27'; Field = 'requirement'; Needle = 'Golden Idol runtime screenshot/result proof'"
Add-ContainsCheck -Name 'gate_ledger_checker_o27_no_encounter_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O27'; Field = 'evidence'; Needle = 'No current encounter screenshot or result log'"
Add-ContainsCheck -Name 'gate_ledger_checker_o28_lab_runtime_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O28'; Field = 'requirement'; Needle = 'The Lab runtime screenshot/result proof'"
Add-ContainsCheck -Name 'gate_ledger_checker_o28_no_encounter_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O28'; Field = 'evidence'; Needle = 'No current encounter screenshot or result log'"
Add-ContainsCheck -Name 'gate_ledger_checker_o29_divine_fountain_runtime_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O29'; Field = 'requirement'; Needle = 'Divine Fountain runtime screenshot/result proof'"
Add-ContainsCheck -Name 'gate_ledger_checker_o29_no_encounter_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O29'; Field = 'evidence'; Needle = 'No current encounter screenshot or result log'"
Add-ContainsCheck -Name 'gate_ledger_checker_o30_canary_review_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O30'; Field = 'requirement'; Needle = 'Canary event code review remains current'"
Add-ContainsCheck -Name 'gate_ledger_checker_o30_canary_review_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O30'; Field = 'evidence'; Needle = 'canary-source-api-proof.md and static source/docs'"
Add-ContainsCheck -Name 'gate_ledger_checker_o31_result_logs_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O31'; Field = 'requirement'; Needle = 'Canary result logs for all four events'"
Add-ContainsCheck -Name 'gate_ledger_checker_o31_no_result_logs_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O31'; Field = 'evidence'; Needle = 'No current result logs'"
Add-ContainsCheck -Name 'gate_ledger_checker_o32_prepost_state_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O32'; Field = 'requirement'; Needle = 'Canary pre/post state evidence'"
Add-ContainsCheck -Name 'gate_ledger_checker_o32_no_prepost_state_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O32'; Field = 'evidence'; Needle = 'No current pre/post state evidence'"
Add-ContainsCheck -Name 'gate_ledger_checker_o33_batch1_align_package_action' -Text $gateLedgerCheckerScript -Needle "GateId = 'O33'; Field = 'next_action'; Needle = 'Loader-registration proof only; gameplay/render/save-load rows remain separate'"
Add-ContainsCheck -Name 'gate_ledger_checker_o34_save_load_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O34'; Field = 'requirement'; Needle = 'Canary save/load proof'"
Add-ContainsCheck -Name 'gate_ledger_checker_o34_no_save_load_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O34'; Field = 'evidence'; Needle = 'No save/load evidence'"
Add-ContainsCheck -Name 'gate_ledger_checker_o35_en_render_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O35'; Field = 'requirement'; Needle = 'Canary EN render proof'"
Add-ContainsCheck -Name 'gate_ledger_checker_o35_no_en_render_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O35'; Field = 'evidence'; Needle = 'No current EN screenshot/render proof'"
Add-ContainsCheck -Name 'gate_ledger_checker_o36_zhs_render_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O36'; Field = 'requirement'; Needle = 'Canary ZHS render proof'"
Add-ContainsCheck -Name 'gate_ledger_checker_o36_no_zhs_render_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O36'; Field = 'evidence'; Needle = 'No current ZHS screenshot/render proof'"
Add-ContainsCheck -Name 'gate_ledger_checker_o37_image_license_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O37'; Field = 'requirement'; Needle = 'Canary image/license/render decision'"
Add-ContainsCheck -Name 'gate_ledger_checker_o37_no_art_decision_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O37'; Field = 'evidence'; Needle = 'No redistributable StS1 event art decision'"
Add-ContainsCheck -Name 'gate_ledger_checker_o38_parity_gap_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O38'; Field = 'requirement'; Needle = 'Canary parity-gap disposition'"
Add-ContainsCheck -Name 'gate_ledger_checker_o38_non_parity_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O38'; Field = 'evidence'; Needle = 'Golden Idol relic substitute and image gaps remain non-parity'"
Add-ContainsCheck -Name 'gate_ledger_checker_o39_audit_packet_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O39'; Field = 'requirement'; Needle = 'Canary runtime audit packet complete'"
Add-ContainsCheck -Name 'gate_ledger_checker_o39_packet_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O39'; Field = 'evidence'; Needle = 'runtime-evidence-packet-check.json checks=45 mismatches=0'"
Add-ContainsCheck -Name 'gate_ledger_checker_o40_gameplay_docs_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O40'; Field = 'requirement'; Needle = 'Canary gameplay notes integrated into docs'"
Add-ContainsCheck -Name 'gate_ledger_checker_o40_no_gameplay_docs_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O40'; Field = 'evidence'; Needle = 'No current gameplay evidence to summarize'"
Add-ContainsCheck -Name 'gate_ledger_checker_o41_owner_qa_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O41'; Field = 'requirement'; Needle = 'Canary owner/QA acceptance'"
Add-ContainsCheck -Name 'gate_ledger_checker_o41_no_qa_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O41'; Field = 'evidence'; Needle = 'No independent QA after runtime proof'"
Add-ContainsCheck -Name 'gate_ledger_checker_o42_purifier_runtime_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O42'; Field = 'requirement'; Needle = 'Purifier runtime proof'"
Add-ContainsCheck -Name 'gate_ledger_checker_o42_no_encounter_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O42'; Field = 'evidence'; Needle = 'No current encounter screenshot/result log'"
Add-ContainsCheck -Name 'gate_ledger_checker_o43_upgrade_shrine_runtime_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O43'; Field = 'requirement'; Needle = 'Upgrade Shrine runtime proof'"
Add-ContainsCheck -Name 'gate_ledger_checker_o43_no_encounter_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O43'; Field = 'evidence'; Needle = 'No current encounter screenshot/result log'"
Add-ContainsCheck -Name 'gate_ledger_checker_o44_golden_shrine_runtime_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O44'; Field = 'requirement'; Needle = 'Golden Shrine runtime proof'"
Add-ContainsCheck -Name 'gate_ledger_checker_o44_no_encounter_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O44'; Field = 'evidence'; Needle = 'No current encounter screenshot/result log'"
Add-ContainsCheck -Name 'gate_ledger_checker_o45_cleric_runtime_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O45'; Field = 'requirement'; Needle = 'The Cleric runtime proof'"
Add-ContainsCheck -Name 'gate_ledger_checker_o45_no_encounter_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O45'; Field = 'evidence'; Needle = 'No current encounter screenshot/result log'"
Add-ContainsCheck -Name 'gate_ledger_checker_o46_old_beggar_runtime_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O46'; Field = 'requirement'; Needle = 'Old Beggar runtime proof'"
Add-ContainsCheck -Name 'gate_ledger_checker_o46_no_encounter_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O46'; Field = 'evidence'; Needle = 'No current encounter screenshot/result log'"
Add-ContainsCheck -Name 'gate_ledger_checker_o47_shining_light_runtime_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O47'; Field = 'requirement'; Needle = 'Shining Light runtime proof'"
Add-ContainsCheck -Name 'gate_ledger_checker_o47_no_encounter_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O47'; Field = 'evidence'; Needle = 'No current encounter screenshot/result log'"
Add-ContainsCheck -Name 'gate_ledger_checker_o48_simple_save_load_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O48'; Field = 'requirement'; Needle = 'Simple batch save/load proof'"
Add-ContainsCheck -Name 'gate_ledger_checker_o48_no_save_load_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O48'; Field = 'evidence'; Needle = 'No current save/load evidence'"
Add-ContainsCheck -Name 'gate_ledger_checker_o49_bilingual_render_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O49'; Field = 'requirement'; Needle = 'Simple batch EN/ZHS render proof'"
Add-ContainsCheck -Name 'gate_ledger_checker_o49_no_bilingual_render_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O49'; Field = 'evidence'; Needle = 'No current bilingual render screenshots'"
Add-ContainsCheck -Name 'gate_ledger_checker_o50_image_license_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O50'; Field = 'requirement'; Needle = 'Simple batch image/license/render decision'"
Add-ContainsCheck -Name 'gate_ledger_checker_o50_no_art_decision_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O50'; Field = 'evidence'; Needle = 'No redistributable StS1 event art decision'"
Add-ContainsCheck -Name 'gate_ledger_checker_o51_audit_packet_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O51'; Field = 'requirement'; Needle = 'Simple batch runtime audit packet complete'"
Add-ContainsCheck -Name 'gate_ledger_checker_o51_no_clean_packet_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O51'; Field = 'evidence'; Needle = 'Clean AdditiveBatch1 loader packet exists but no simple-batch gameplay packet'"
Add-ContainsCheck -Name 'gate_ledger_checker_o52_independent_qa_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O52'; Field = 'requirement'; Needle = 'Simple batch independent QA'"
Add-ContainsCheck -Name 'gate_ledger_checker_o52_qa_impossible_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O52'; Field = 'evidence'; Needle = 'QA impossible before gameplay proof'"
Add-ContainsCheck -Name 'gate_ledger_checker_o53_replacement_source_gate_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O53'; Field = 'requirement'; Needle = 'ReplacementPrototype source gate present'"
Add-ContainsCheck -Name 'gate_ledger_checker_o53_replacement_source_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O53'; Field = 'evidence'; Needle = 'feature-gate checker and replacement source'"
Add-ContainsCheck -Name 'gate_ledger_checker_o53_keep_debug_unsafe_action' -Text $gateLedgerCheckerScript -Needle "GateId = 'O53'; Field = 'next_action'; Needle = 'Keep debug/unsafe gate'"
Add-ContainsCheck -Name 'gate_ledger_checker_o54_unknown_room_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O54'; Field = 'requirement'; Needle = 'Replacement unknown-room proof'"
Add-ContainsCheck -Name 'gate_ledger_checker_o54_no_runtime_replacement_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O54'; Field = 'evidence'; Needle = 'No debug runtime replacement evidence'"
Add-ContainsCheck -Name 'gate_ledger_checker_o55_act_bucket_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O55'; Field = 'requirement'; Needle = 'Replacement act-bucket proof'"
Add-ContainsCheck -Name 'gate_ledger_checker_o55_no_runtime_replacement_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O55'; Field = 'evidence'; Needle = 'No debug runtime replacement evidence'"
Add-ContainsCheck -Name 'gate_ledger_checker_o56_event_bag_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O56'; Field = 'requirement'; Needle = 'Replacement event-bag/no-repeat proof'"
Add-ContainsCheck -Name 'gate_ledger_checker_o56_no_runtime_replacement_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O56'; Field = 'evidence'; Needle = 'No debug runtime replacement evidence'"
Add-ContainsCheck -Name 'gate_ledger_checker_o57_save_load_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O57'; Field = 'requirement'; Needle = 'Replacement save/load proof'"
Add-ContainsCheck -Name 'gate_ledger_checker_o57_no_replacement_save_load_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O57'; Field = 'evidence'; Needle = 'No debug runtime replacement save/load evidence'"
Add-ContainsCheck -Name 'gate_ledger_checker_o58_multiplayer_runtime_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O58'; Field = 'requirement'; Needle = 'Multiplayer fail-closed runtime proof'"
Add-ContainsCheck -Name 'gate_ledger_checker_o58_no_multiplayer_runtime_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O58'; Field = 'evidence'; Needle = 'No current multiplayer runtime proof'"
Add-ContainsCheck -Name 'gate_ledger_checker_o59_isshared_matrix_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O59'; Field = 'requirement'; Needle = 'IsShared source matrix current'"
Add-ContainsCheck -Name 'gate_ledger_checker_o59_multiplayer_shape_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O59'; Field = 'evidence'; Needle = 'multiplayer-shape checker'"
Add-ContainsCheck -Name 'gate_ledger_checker_o60_combat_blockers_next_action' -Text $gateLedgerCheckerScript -Needle "GateId = 'O60'; Field = 'next_action'; Needle = 'Keep combat blocked until encounter models exist'"
Add-ContainsCheck -Name 'gate_ledger_checker_o61_temporary_non_parity_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O61'; Field = 'requirement'; Needle = 'Temporary substitutes marked non-parity'"
Add-ContainsCheck -Name 'gate_ledger_checker_o62_content_parity_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O62'; Field = 'requirement'; Needle = 'Content parity gaps current'"
Add-ContainsCheck -Name 'gate_ledger_checker_o63_asset_safety_next_action' -Text $gateLedgerCheckerScript -Needle "GateId = 'O63'; Field = 'next_action'; Needle = 'Keep zero tracked StS1 original images'"
Add-ContainsCheck -Name 'gate_ledger_checker_o64_zhs_screenshots_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O64'; Field = 'requirement'; Needle = 'ZHS screenshots for StS1 events'"
Add-ContainsCheck -Name 'gate_ledger_checker_o64_no_zhs_runtime_screenshots' -Text $gateLedgerCheckerScript -Needle "GateId = 'O64'; Field = 'evidence'; Needle = 'No current ZHS runtime screenshots'"
Add-ContainsCheck -Name 'gate_ledger_checker_o65_no_independent_qa_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O65'; Field = 'evidence'; Needle = 'No independent QA after current runtime evidence'"
Add-ContainsCheck -Name 'gate_ledger_checker_o66_current_validation_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O66'; Field = 'evidence'; Needle = 'docs/reviews/current-validation.md references v19 map'"
Add-ContainsCheck -Name 'gate_ledger_checker_o67_status_board_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O67'; Field = 'evidence'; Needle = 'status-board records beta.93 RitsuLib-only AdditiveBatch1 current proof plus beta.85/beta.87/beta.88 previous-context loader proof'"
Add-ContainsCheck -Name 'gate_ledger_checker_o68_hard_stop_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O68'; Field = 'evidence'; Needle = 'hard-stop report records coordination blocker'"
Add-ContainsCheck -Name 'gate_ledger_checker_o69_private_beta_handoff_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O69'; Field = 'evidence'; Needle = 'private-beta handoff references v19 map'"
Add-ContainsCheck -Name 'gate_ledger_checker_o70_release_checklist_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O70'; Field = 'evidence'; Needle = 'release checklist references v19 map'"
Add-ContainsCheck -Name 'gate_ledger_checker_o71_owner_actions_evidence' -Text $gateLedgerCheckerScript -Needle "GateId = 'O71'; Field = 'evidence'; Needle = 'hard-stop report owner actions listed'"
Add-ContainsCheck -Name 'gate_ledger_checker_o72_no_unsupported_commit_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O72'; Field = 'requirement'; Needle = 'no unsupported commit decision'"
Add-ContainsCheck -Name 'gate_ledger_checker_o73_no_unsupported_push_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O73'; Field = 'requirement'; Needle = 'no unsupported push decision'"
Add-ContainsCheck -Name 'gate_ledger_checker_o74_final_summary_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O74'; Field = 'requirement'; Needle = 'final summary lists blocked gates'"
Add-ContainsCheck -Name 'gate_ledger_checker_o75_do_not_complete_action' -Text $gateLedgerCheckerScript -Needle "GateId = 'O75'; Field = 'next_action'; Needle = 'Do not mark event.md complete'"
Add-ContainsCheck -Name 'gate_ledger_checker_o76_hard_stop_non_completion_requirement' -Text $gateLedgerCheckerScript -Needle "GateId = 'O76'; Field = 'requirement'; Needle = 'hard-stop is not completion invariant preserved'"
Add-ContainsCheck -Name 'gate_ledger_o33_blocked' -Text $gateLedger -Needle 'O33,O30-O41,Current v0.107.1 AdditiveBatch1 enabled-mode smoke exact 10 types / 14 calls,current-pass'
Add-ContainsCheck -Name 'gate_ledger_o75_blocked' -Text $gateLedger -Needle 'O75,O65-O76,all O0-O76 gates passing before completion,blocked'
Add-ContainsCheck -Name 'gate_ledger_act_mapping' -Text $gateLedger -Needle 'Act 1 maps to Overgrowth + Underdocks'
Add-ContainsCheck -Name 'gate_ledger_package_sha' -Text $gateLedger -Needle 'ZIP SHA 56636753F598B360B3798ED681ED84C3CA08CEC173E7EBA70134F4BC68EF964A'
Add-ContainsCheck -Name 'gate_ledger_off_default_only' -Text $gateLedger -Needle 'loader-registration only'
Add-ContainsCheck -Name 'gate_ledger_canary_packet_v01070' -Text $gateLedger -Needle '.tools/runtime-evidence/v01070-beta85-canary-20260617-233621/godot.log.after-launch'
Add-ContainsCheck -Name 'gate_ledger_batch1_failed_packet_v01070' -Text $gateLedger -Needle 'exact act/shared tuple parity including Sts1TheCleric in Overgrowth and Underdocks'

Add-ContainsCheck -Name 'subagent_coverage_non_claim' -Text $subagentCoverage -Needle 'This is a coverage ledger, not a completion claim.'
Add-ContainsCheck -Name 'subagent_coverage_roles_15' -Text $subagentCoverage -Needle 'Release Documentation Subagent'
Add-ContainsCheck -Name 'subagent_coverage_date_current_static_governance' -Text $subagentCoverage -Needle 'Date: 2026-06-17 (v19 file retained; latest v20 role update)'
Add-ContainsCheck -Name 'subagent_coverage_static_only' -Text $subagentCoverage -Needle 'read-only explorer subagents for static sidecar audits only'
Add-ContainsCheck -Name 'subagent_coverage_no_qa_claim' -Text $subagentCoverage -Needle 'Read-only explorer audits are not independent QA/Red-Team acceptance.'
Add-ContainsCheck -Name 'subagent_coverage_no_authorization' -Text $subagentCoverage -Needle 'This ledger does not authorize commit, push, release, or private-beta readiness claims.'
Add-ContainsCheck -Name 'subagent_coverage_replacement_gate_split' -Text $subagentCoverage -Needle '`O53` is source-guarded only; `O54-O57` replacement functional proof remains blocked in `v19-gate-ledger.csv`.'
Add-ContainsCheck -Name 'subagent_coverage_multiplayer_runtime_static_split' -Text $subagentCoverage -Needle '| Multiplayer / IsShared Subagent | Runtime fail-closed multiplayer proof before any multiplayer gameplay claim; source `IsShared` classification remains static evidence only. | O58 runtime proof; O59 static classification |'
Add-ContainsCheck -Name 'subagent_coverage_qa_o65_split' -Text $subagentCoverage -Needle '| QA / Red-Team Subagent | Independent pass/fail QA only after current runtime/gameplay packets exist. | O65 |'
Add-ContainsCheck -Name 'subagent_coverage_release_docs_o66_o71_split' -Text $subagentCoverage -Needle '| Release Documentation Subagent | Update current-validation, status-board, hard-stop/monthly review, private beta handoff, release checklist, and owner action list from evidence paths. | O66-O71 |'
Add-ContainsCheck -Name 'subagent_coverage_owner_final_o72_o76_split' -Text $subagentCoverage -Needle '| Owner / Final Handoff | Make explicit owner decisions for commit/push scope, final blocked-gate summary, and the all-gates-green-before-completion invariant. | O72-O76 |'

Add-ContainsCheck -Name 'hard_stop_enabled_log_verifier' -Text $hardStop -Needle 'scripts/check-sts1-enabled-mode-runtime-log.ps1'
Add-ContainsCheck -Name 'hard_stop_enabled_log_verifier_non_claim' -Text $hardStop -Needle 'it does not create CanaryOnly/AdditiveBatch1 proof without fresh runtime logs'
Add-ContainsCheck -Name 'hard_stop_enabled_log_verifier_requires_package_target' -Text $hardStop -Needle 'future CanaryOnly/AdditiveBatch1 copied-log checks require explicit package, Ritsu compat-branch, RitsuLib package-version, and game-version targets'
Add-ContainsCheck -Name 'hard_stop_owner_action_enabled_log_expected_ritsu_lib_version' -Text $hardStop -Needle 'with explicit `-ExpectedPackageVersion`, `-ExpectedRitsuCompatBranch`, `-ExpectedRitsuLibVersion`, `-ExpectedGameVersion`, `-OutFile`, and `-FailOnMismatch` arguments.'
Add-ContainsCheck -Name 'hard_stop_owner_action_packet_no_legacy_bypass' -Text $hardStop -Needle 'no enabled-mode `-AllowMissingSessionState` / `-AllowMissingRestoreState` bypass.'
Add-ContainsCheck -Name 'hard_stop_runtime_evidence_packet_verifier' -Text $hardStop -Needle 'scripts/check-sts1-runtime-evidence-packet.ps1'
Add-ContainsCheck -Name 'hard_stop_off_packet_rerun_20260615' -Text $hardStop -Needle 'During the 2026-06-15 coordination pause, reran no-launch `check-sts1-runtime-evidence-packet.ps1` against the already-captured beta.85 Off evidence'
Add-ContainsCheck -Name 'hard_stop_off_packet_rerun_outfile_20260615' -Text $hardStop -Needle 'retained `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/runtime-evidence-packet-check.json`'
Add-ContainsCheck -Name 'hard_stop_off_packet_rerun_non_claim' -Text $hardStop -Needle "This earlier 33-check rerun was later superseded by action 122's 34-check rerun with explicit RitsuLib package-version target; it re-verifies existing default-Off evidence only and does not close O25, O33, enabled-mode, gameplay, save/load, replacement, multiplayer, image/render, or QA gates."
Add-ContainsCheck -Name 'hard_stop_latest_addendum_date' -Text $hardStop -Needle 'Date: 2026-06-11; latest addendum: 2026-06-21'
Add-ContainsCheck -Name 'hard_stop_beta93_supersession_note' -Text $hardStop -Needle 'Supersession note, 2026-06-21: this report remains the historical v19 coordination-blocker trace.'
Add-ContainsCheck -Name 'hard_stop_beta85_capture_time_only' -Text $hardStop -Needle 'Historical capture-time project state was beta.85 on Slay the Spire 2 `v0.107.0` with RitsuLib `v0.4.16`.'
Add-ContainsCheck -Name 'hard_stop_gate_range_split_runtime_blocked' -Text $hardStop -Needle '`O26-O29`, `O31-O41`, `O42-O52`, `O54-O58`, `O64`, `O65`, and `O72-O75`: downstream gameplay, save/load, render, replacement functional proof, multiplayer runtime proof, QA, owner-decision, and handoff gates remain blocked or current-pending because the current enabled-mode proof gates above are still missing.'
Add-ContainsCheck -Name 'hard_stop_gate_range_split_static_rows' -Text $hardStop -Needle '`O53`, `O59-O63`, `O66-O71`, and `O76`: static/source/documentation rows have current static evidence or documentation-in-progress status, but they do not close their parent runtime ranges or permit completion, handoff, commit, push, or release claims while enabled-mode/gameplay proof is absent.'
Add-ContainsCheck -Name 'hard_stop_ritsu_plan_pause_boundary_scope' -Text $hardStop -Needle 'Added explicit same-repository validation-pause boundaries to `docs/features/ritsulib-migration/next-overnight-run.md` and `docs/features/ritsulib-migration/monthly-dev-spec.md`'
Add-ContainsCheck -Name 'hard_stop_pause_safe_static_verification_20260615' -Text $hardStop -Needle '113. After adding the RitsuLib plan pause-boundary guards, reran pause-safe static verification only: `check-sts1-event-static-suite.ps1` completed 14 static steps with 0 suite failures and the known 33-key localization gap, `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 695 checks / 0 mismatches, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, and `git diff --check --` exited 0 with CRLF warnings only for `AGENTS.md` and `docs/goals/refactor.md`. No validation/runtime/release/stage/commit/push process was started, and no runtime gate was closed.'
Add-ContainsCheck -Name 'hard_stop_dirty_worktree_wording_verification_20260615' -Text $hardStop -Needle '114. Corrected `docs/features/ritsulib-migration/next-overnight-run.md` so its dirty-worktree note no longer calls the tracked `docs/features/ritsulib-migration/batch-4c-candidates.md` file untracked, then guarded the current wording against regression. Reran pause-safe static verification only: `check-sts1-event-static-suite.ps1` completed 14 static steps with 0 suite failures and the known 33-key localization gap, `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 698 checks / 0 mismatches, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, and `git diff --check --` exited 0 with CRLF warnings only for `AGENTS.md` and `docs/goals/refactor.md`. No validation/runtime/release/stage/commit/push process was started, and no runtime gate was closed.'
Add-ContainsCheck -Name 'hard_stop_current_validation_historical_target_fix_verification_20260615' -Text $hardStop -Needle '115. Tightened the historical target-fix smoke paragraph in `docs/reviews/current-validation.md` so the old CanaryOnly 4-registration result is explicitly scoped to its source/runtime state and cannot be read as current beta.85 enabled-mode proof. Reran pause-safe static verification only: `check-sts1-event-static-suite.ps1` completed 14 static steps with 0 suite failures and the known 33-key localization gap, `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 701 checks / 0 mismatches, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, and `git diff --check --` exited 0 with CRLF warnings only for `AGENTS.md` and `docs/goals/refactor.md`. No validation/runtime/release/stage/commit/push process was started, and no runtime gate was closed.'
Add-ContainsCheck -Name 'hard_stop_unscoped_canary4_content_guard_verification_20260615' -Text $hardStop -Needle '116. Added a broader current-claim stale-phrase guard so unscoped CanaryOnly 4-content-registration proof wording cannot return to active/current claim files while historical lane-scoped wording remains allowed. Reran pause-safe static verification only: `check-sts1-event-static-suite.ps1` completed 14 static steps with 0 suite failures and the known 33-key localization gap, `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 703 checks / 0 mismatches, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, and `git diff --check --` exited 0 with CRLF warnings only for `AGENTS.md` and `docs/goals/refactor.md`. No validation/runtime/release/stage/commit/push process was started, and no runtime gate was closed.'
Add-ContainsCheck -Name 'hard_stop_localization_pass_a_nonproof_guard_verification_20260615' -Text $hardStop -Needle '117. Guarded the localization closure plan so Pass A cannot be read as CanaryOnly/AdditiveBatch1 runtime proof or as a replacement for the enabled-mode log verifier and runtime evidence packet. Reran pause-safe static verification only: `check-sts1-event-static-suite.ps1` completed 14 static steps with 0 suite failures and the known 33-key localization gap, `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 705 checks / 0 mismatches, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, and `git diff --check --` exited 0 with CRLF warnings only for `AGENTS.md` and `docs/goals/refactor.md`. No validation/runtime/release/stage/commit/push process was started, and no runtime gate was closed.'
Add-ContainsCheck -Name 'hard_stop_goal_testplan_direct_localization_nonproof_guard_verification_20260615' -Text $hardStop -Needle '118. Mirrored the direct Golden Idol localization non-proof boundary into `docs/goals/event.md` and `docs/features/sts1-events/test-plan.md`, then guarded both active docs so closing the missing key cannot be treated as closing O25/O33 or replacing enabled-mode verifier reports. Reran pause-safe static verification only: `check-sts1-event-static-suite.ps1` completed 14 static steps with 0 suite failures and the known 33-key localization gap, `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 708 checks / 0 mismatches, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, and `git diff --check --` exited 0 with CRLF warnings only for `AGENTS.md` and `docs/goals/refactor.md`. No validation/runtime/release/stage/commit/push process was started, and no runtime gate was closed.'
Add-ContainsCheck -Name 'hard_stop_handoff_release_direct_localization_nonproof_guard_verification_20260615' -Text $hardStop -Needle '119. Mirrored the direct Golden Idol localization non-proof boundary into tester handoff and release-checklist surfaces, then guarded both so outward-facing docs cannot treat the direct-key fix as O25/O33 enabled-mode proof or verifier-report replacement. Reran pause-safe static verification only: `check-sts1-event-static-suite.ps1` completed 14 static steps with 0 suite failures and the known 33-key localization gap, `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 711 checks / 0 mismatches, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, and `git diff --check --` exited 0 with CRLF warnings only for `AGENTS.md` and `docs/goals/refactor.md`. No validation/runtime/release/stage/commit/push process was started, and no runtime gate was closed.'
Add-ContainsCheck -Name 'hard_stop_active_summary_direct_localization_nonproof_guard_verification_20260615' -Text $hardStop -Needle '120. Mirrored the direct Golden Idol localization non-proof boundary into `docs/test-ready-development-goal.md`, `docs/review.md`, and `docs/toreview.md`, then guarded those active summary surfaces so they cannot treat the direct-key fix as O25/O33 enabled-mode proof or verifier-report replacement. Reran pause-safe static verification only: `check-sts1-event-static-suite.ps1` completed 14 static steps with 0 suite failures and the known 33-key localization gap, `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 715 checks / 0 mismatches in that pass and was later superseded by action 121''s 744-check rerun, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, and `git diff --check --` exited 0 with CRLF warnings only for `AGENTS.md` and `docs/goals/refactor.md`. No validation/runtime/release/stage/commit/push process was started, and no runtime gate was closed.'
Add-ContainsCheck -Name 'hard_stop_enabled_mode_verifier_contract_guard_verification_20260615' -Text $hardStop -Needle '121. Hardened the future enabled-mode evidence contract after read-only subagent audit: `check-sts1-enabled-mode-runtime-log.ps1` now requires `-ExpectedRitsuLibVersion` and retained `-OutFile` reports for enabled-mode copied logs, prints source-derived registration tuples while preserving the class-only runtime-log limitation, and `check-sts1-runtime-evidence-packet.ps1` now passes the RitsuLib-version target into the nested log verifier, writes/retains `enabled-mode-log-check.json`, rejects enabled-mode `-AllowMissingSessionState` / `-AllowMissingRestoreState`, and requires retained packet `-OutFile`. Updated `docs/features/sts1-events/test-plan.md`, `status-board.md`, `v19-gate-evidence-map.md`, `v19-subagent-coverage.md`, `scripts/README.md`, and `docs/features/ritsulib-migration/runtime-smoke-checklist.md` to require RitsuLib package-version checks, retained verifier JSON, no legacy enabled-mode packet bypasses, and a separate Act-target tuple proof boundary. Reran pause-safe static verification only: `check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 61 checks / 0 mismatches, `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 744 checks / 0 mismatches, the aggregate static suite remained 14 steps / 0 suite failures with the known 33-key localization gap, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, and `git diff --check --` exited 0 with CRLF warnings only for `AGENTS.md` and `docs/goals/refactor.md`. No build/test/publish/runtime/release/stage/commit/push process was started, and no runtime gate was closed.'
Add-ContainsCheck -Name 'hard_stop_off_packet_ritsu_target_rerun_20260615' -Text $hardStop -Needle '122. Reran the already-captured beta.85 Off packet verifier with explicit `-ExpectedRitsuLibVersion 0.4.16`, updating the retained default-Off packet report to `checks=34` / `mismatches=0` with nested log verifier `checks=10` / `mismatches=0`. Updated `docs/features/sts1-events/test-plan.md`, `v19-gate-evidence-map.md`, `status-board.md`, `docs/reviews/current-validation.md`, `v19-gate-ledger.csv`, `docs/goals/event.md`, and the static guards so active Off packet commands cannot drop the RitsuLib package-version target, stale `33`/`750`/`751`/`752`/`753`/`754` summary claims, unscoped historical runtime PASS wording, or sandbox-only current-claim artifact links, while also asserting `docs/restructure.md` stays in current-claim and static-file-hygiene scope. Reran pause-safe static verification only: `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 756 checks / 0 mismatches, `check-sts1-event-static-suite.ps1` returned 14 static steps / 0 suite failures with the known 33-key localization gap, `check-sts1-static-file-hygiene.ps1 -FailOnMismatch` returned 7 checks / 0 mismatches, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, `check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 61 checks / 0 mismatches, and `git diff --check --` on the touched StS1 docs/scripts exited 0. No build/test/publish/runtime/release/stage/commit/push process was started, and no runtime gate was closed.'
Add-ContainsCheck -Name 'hard_stop_v19_date_header_guard_20260615' -Text $hardStop -Needle '123. Updated `docs/features/sts1-events/v19-gate-evidence-map.md` and `docs/features/sts1-events/v19-subagent-coverage.md` date headers to show the 2026-06-15 pause-safe static-governance update while preserving the 2026-06-11 v19 origin context, then guarded those headers and added stale `756` active-summary protection. Reran pause-safe static verification only: `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 759 checks / 0 mismatches, `check-sts1-event-static-suite.ps1` returned 14 static steps / 0 suite failures with the known 33-key localization gap, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, `check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 61 checks / 0 mismatches, and `git diff --check --` on the touched StS1 docs/scripts exited 0. No build/test/publish/runtime/release/stage/commit/push process was started, and no runtime gate was closed.'
Add-ContainsCheck -Name 'hard_stop_active_report_date_guard_20260615' -Text $hardStop -Needle '124. Updated the active current-validation and v19 hard-stop report date headers so both show the 2026-06-15 latest addendum instead of looking like June 11-only evidence, then guarded those headers and added stale `759` active-summary protection. Reran pause-safe static verification only: `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 762 checks / 0 mismatches, `check-sts1-event-static-suite.ps1` returned 14 static steps / 0 suite failures with the known 33-key localization gap, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, `check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 61 checks / 0 mismatches, and `git diff --check --` on the touched StS1 docs/scripts exited 0. No build/test/publish/runtime/release/stage/commit/push process was started, and no runtime gate was closed.'
Add-ContainsCheck -Name 'hard_stop_localization_scan_direct_key_nonproof_guard_20260615' -Text $hardStop -Needle '125. Mirrored the direct Golden Idol missing-key non-proof boundary into `docs/features/sts1-events/localization-source-gap-scan-20260611.md`, then guarded the source-gap scan so the direct-key fix cannot be read as O25/O33 enabled-mode proof or as a replacement for retained verifier reports. Reran pause-safe static verification only: `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 764 checks / 0 mismatches, `check-sts1-event-static-suite.ps1` returned 14 static steps / 0 suite failures with the known 33-key localization gap, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, `check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 61 checks / 0 mismatches, and `git diff --check --` on the touched StS1 docs/scripts exited 0. No build/test/publish/runtime/release/stage/commit/push process was started, and no runtime gate was closed.'
Add-ContainsCheck -Name 'hard_stop_gate_range_split_guard_20260615' -Text $hardStop -Needle '126. Split the compact hard-stop and test-plan range wording so `O53` remains source-guarded, `O54-O57` remain replacement functional-proof blockers, `O59-O63` remain static classification/safety evidence, and documentation-in-progress rows are not flattened into runtime blockers. Reran pause-safe static verification only: `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 769 checks / 0 mismatches, `check-sts1-event-static-suite.ps1` returned 14 static steps / 0 suite failures with the known 33-key localization gap, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, `check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 61 checks / 0 mismatches, and focused `git diff --check --` on the touched StS1 docs/scripts exited 0. No build/test/publish/runtime/release/stage/commit/push process was started, and no runtime gate was closed.'
Add-ContainsCheck -Name 'hard_stop_subagent_audit_followup_guard_20260615' -Text $hardStop -Needle '127. Applied read-only subagent audit findings by classifying status-board `O14`/`O15` as static source-identity gates, marking current format revalidation as paused instead of current, and mirroring the direct Golden Idol localization non-proof boundary into `docs/features/sts1-events/localization.md` and `status-board.md`. Reran pause-safe static verification only: `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 775 checks / 0 mismatches, `check-sts1-event-static-suite.ps1` returned 14 static steps / 0 suite failures with the known 33-key localization gap, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, `check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 61 checks / 0 mismatches, and focused `git diff --check --` on the touched StS1 docs/scripts exited 0. No build/test/publish/runtime/release/stage/commit/push process was started, and no runtime gate was closed.'
Add-ContainsCheck -Name 'hard_stop_status_board_remaining_gate_split_20260615' -Text $hardStop -Needle '128. Tightened the status-board remaining-gate summary so runtime blockers (`O58`, `O64`, `O65`, `O72-O75`) are not flattened together with static classification/safety rows (`O59-O63`), documentation-in-progress rows (`O66-O71`), or the O76 non-completion invariant. Reran pause-safe static verification only: `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 778 checks / 0 mismatches, `check-sts1-event-static-suite.ps1` returned 14 static steps / 0 suite failures with the known 33-key localization gap, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, `check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 61 checks / 0 mismatches, and focused `git diff --check --` on the touched StS1 docs/scripts exited 0. No build/test/publish/runtime/release/stage/commit/push process was started, and no runtime gate was closed.'
Add-ContainsCheck -Name 'hard_stop_subagent_post_pause_split_20260615' -Text $hardStop -Needle '129. Split the post-pause subagent evidence checklist so QA (`O65`), release documentation (`O66-O71`), and owner/final handoff (`O72-O76`) are no longer collapsed into one broad `O65-O75` row; also clarified the multiplayer row as `O58` runtime proof plus `O59` static classification. Reran pause-safe static verification only: `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 784 checks / 0 mismatches, `check-sts1-event-static-suite.ps1` returned 14 static steps / 0 suite failures with the known 33-key localization gap, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, `check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 61 checks / 0 mismatches, and focused `git diff --check --` on the touched StS1 docs/scripts exited 0. No build/test/publish/runtime/release/stage/commit/push process was started, and no runtime gate was closed.'
Add-ContainsCheck -Name 'hard_stop_evidence_map_range_split_20260615' -Text $hardStop -Needle '130. Tightened the v19 evidence-map range table so `O58-O64` distinguishes `O58`/`O64` runtime blockers from `O59-O63` static classification/safety coverage, and `O65-O76` distinguishes `O65` and `O72-O75` current-pending/blocked rows from `O66-O71` documentation-in-progress and the `O76` non-completion invariant. Reran pause-safe static verification only: `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 788 checks / 0 mismatches, `check-sts1-event-static-suite.ps1` returned 14 static steps / 0 suite failures with the known 33-key localization gap, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, `check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 61 checks / 0 mismatches, and focused `git diff --check --` on the touched StS1 docs/scripts exited 0. No build/test/publish/runtime/release/stage/commit/push process was started, and no runtime gate was closed.'
Add-ContainsCheck -Name 'hard_stop_event_goal_static_checkpoint_20260615' -Text $hardStop -Needle '131. Mirrored the latest pause-safe static checkpoint into `docs/goals/event.md` so the active goal itself records the validation-pause boundary, exact static-only checker results, and non-claim scope. Guarded that checkpoint in `check-sts1-event-current-doc-claims.ps1` and reran pause-safe static verification only: `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 790 checks / 0 mismatches, `check-sts1-event-static-suite.ps1` returned 14 static steps / 0 suite failures with the known 33-key localization gap, `check-sts1-static-file-hygiene.ps1 -FailOnMismatch` returned 7 checks / 0 mismatches, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, `check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 61 checks / 0 mismatches, and focused `git diff --check --` on the touched StS1 docs/scripts exited 0. No build/test/publish/runtime/release/stage/commit/push process was started, and no runtime gate was closed.'
Add-ContainsCheck -Name 'hard_stop_event_goal_stale_count_scope_20260615' -Text $hardStop -Needle '132. Added `docs/goals/event.md` to the tight active-summary stale-count guard so its latest pause-safe checkpoint cannot regress to old current-doc claim counts while the active goal remains open. Reran pause-safe static verification only: `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 791 checks / 0 mismatches, `check-sts1-event-static-suite.ps1` returned 14 static steps / 0 suite failures with the known 33-key localization gap, `check-sts1-static-file-hygiene.ps1 -FailOnMismatch` returned 7 checks / 0 mismatches, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, `check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 61 checks / 0 mismatches, and focused `git diff --check --` on the touched StS1 docs/scripts exited 0. No build/test/publish/runtime/release/stage/commit/push process was started, and no runtime gate was closed.'
Add-ContainsCheck -Name 'hard_stop_subagent_owner_final_nonclaim_20260615' -Text $hardStop -Needle '133. Hardened `scripts/check-sts1-v19-subagent-coverage.ps1` so the post-pause `Owner / Final Handoff` row and the subagent-ledger non-authorization boundary are directly asserted. Reran pause-safe static verification only: `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 793 checks / 0 mismatches, `check-sts1-event-static-suite.ps1` returned 14 static steps / 0 suite failures with the known 33-key localization gap, `check-sts1-static-file-hygiene.ps1 -FailOnMismatch` returned 7 checks / 0 mismatches, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, `check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 63 checks / 0 mismatches, and focused `git diff --check --` on the touched StS1 docs/scripts exited 0. No build/test/publish/runtime/release/stage/commit/push process was started, and no runtime gate was closed.'
Add-ContainsCheck -Name 'hard_stop_enabled_log_audit_path_guard_20260615' -Text $hardStop -Needle '134. Hardened `scripts/check-sts1-enabled-mode-runtime-log.ps1` so future CanaryOnly/AdditiveBatch1 copied-log checks must include `-AuditPath` for `godot-log-audit.json`, documented that requirement in `scripts/README.md`, and guarded the script plus active command examples in `scripts/check-sts1-event-current-doc-claims.ps1`. Reran pause-safe static verification only: `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 798 checks / 0 mismatches, `check-sts1-event-static-suite.ps1` returned 14 static steps / 0 suite failures with the known 33-key localization gap, `check-sts1-static-file-hygiene.ps1 -FailOnMismatch` returned 7 checks / 0 mismatches, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, `check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 63 checks / 0 mismatches, and focused `git diff --check --` on the touched StS1 docs/scripts exited 0. No build/test/publish/runtime/release/stage/commit/push process was started, and no runtime gate was closed.'
Add-ContainsCheck -Name 'hard_stop_v19_gate_ledger' -Text $hardStop -Needle 'scripts/check-sts1-v19-gate-ledger.ps1'
Add-ContainsCheck -Name 'hard_stop_v19_subagent_coverage' -Text $hardStop -Needle 'scripts/check-sts1-v19-subagent-coverage.ps1'
Add-ContainsCheck -Name 'hard_stop_explicit_ritsu_branch_line_matching' -Text $hardStop -Needle 'explicit RitsuLib compat branch line matching'
Add-ContainsCheck -Name 'hard_stop_ritsu_branch_helper' -Text $hardStop -Needle 'Get-RitsuCompatBranchLineHits'
Add-ContainsCheck -Name 'hard_stop_explicit_game_version_line_matching' -Text $hardStop -Needle 'explicit game version line matching'
Add-ContainsCheck -Name 'hard_stop_game_version_helper' -Text $hardStop -Needle 'Get-GameVersionLineHits'
Add-ContainsCheck -Name 'hard_stop_legacy_off_game_version_fallback' -Text $hardStop -Needle 'expected_game_version_in_log_legacy_off_packet'
Add-ContainsCheck -Name 'hard_stop_ledger_high_risk_assertions' -Text $hardStop -Needle 'high-risk row content assertions'
Add-ContainsCheck -Name 'hard_stop_ledger_package_sha' -Text $hardStop -Needle 'B182F6DCD8E88D9209C28997901C9EF5E9947F79E1CA93FD47E7F38625140CEE'
Add-ContainsCheck -Name 'hard_stop_ledger_act_mapping' -Text $hardStop -Needle 'Overgrowth + Underdocks'
Add-ContainsCheck -Name 'hard_stop_ledger_batch1_count' -Text $hardStop -Needle '10 types / 14 calls'
Add-ContainsCheck -Name 'hard_stop_ledger_off_default_only' -Text $hardStop -Needle 'default-Off evidence only'
Add-ContainsCheck -Name 'hard_stop_no_current_enabled_packets' -Text $hardStop -Needle 'no beta.85 CanaryOnly/AdditiveBatch1 evidence packet exists for `v0.107.0`'
Add-ContainsCheck -Name 'hard_stop_static_file_hygiene' -Text $hardStop -Needle 'scripts/check-sts1-static-file-hygiene.ps1'
Add-ContainsCheck -Name 'hard_stop_static_file_hygiene_untracked_boundary' -Text $hardStop -Needle 'including untracked files that `git diff --check --` does not cover'
Add-ContainsCheck -Name 'hard_stop_static_file_hygiene_project_state_scope' -Text $hardStop -Needle '`PROJECT_STATE.md` in static-file hygiene scope'
Add-ContainsCheck -Name 'hard_stop_agents_current_path_guard_action' -Text $hardStop -Needle '`AGENTS.md` current beta.85 Off evidence path with `PROJECT_STATE.md`'
Add-ContainsCheck -Name 'hard_stop_root_readme_claim_scan_action' -Text $hardStop -Needle 'root `README.md` to the StS1 stale-claim and static-file-hygiene scopes'
Add-ContainsCheck -Name 'hard_stop_goal_guard_platform_scope' -Text $hardStop -Needle 'root goal guard and platform-testing guide now carry stale-claim and static-file-hygiene coverage'
Add-ContainsCheck -Name 'hard_stop_doc_restructure_scope' -Text $hardStop -Needle 'documentation restructure spec now carries a current historical-only override'
Add-ContainsCheck -Name 'hard_stop_patch_boundaries_scope' -Text $hardStop -Needle 'patch-boundaries.md` now carries a Revision M StS1Events source-boundary note'
Add-ContainsCheck -Name 'hard_stop_patch_boundaries_4_6_scope' -Text $hardStop -Needle 'patch-boundaries.md` now uses the current CanaryOnly 4 event types / 6 registration-call source shape'
Add-ContainsCheck -Name 'hard_stop_beta_compatibility_scope' -Text $hardStop -Needle 'Beta compatibility doc now carries a Revision M current-boundary note'
Add-ContainsCheck -Name 'hard_stop_remote_setup_scope' -Text $hardStop -Needle 'remote development setup doc now carries a Revision M current-boundary note'
Add-ContainsCheck -Name 'hard_stop_source_api_drift_audit_scope' -Text $hardStop -Needle 'v0.106 source/API drift audit now carries a Revision M current-boundary note'
Add-ContainsCheck -Name 'hard_stop_source_research_boundary_scope' -Text $hardStop -Needle 'StS1 source-research docs now carry Revision M current-boundary notes'
Add-ContainsCheck -Name 'hard_stop_sts1_goal_plan_boundary_scope' -Text $hardStop -Needle 'StS1 feature goal and implementation plan now carry Revision M current-boundary notes'
Add-ContainsCheck -Name 'hard_stop_canonical_matrix_duplicator_scope' -Text $hardStop -Needle 'canonical event matrix Duplicator blocker now references the current game/RitsuLib API surface'
Add-ContainsCheck -Name 'hard_stop_sts1_goal_compile_runtime_split_scope' -Text $hardStop -Needle 'StS1 feature goal now distinguishes the `STS2.RitsuLib` `0.3.2` compile package from the installed `STS2-RitsuLib` `v0.4.16` runtime variant pack'
Add-ContainsCheck -Name 'hard_stop_historical_hard_stop_report_scope' -Text $hardStop -Needle 'historical v12-v15 StS1 hard-stop reports now carry 2026-06-11 supersession notes'
Add-ContainsCheck -Name 'hard_stop_docs_readme_claim_scan_action' -Text $hardStop -Needle '`docs/README.md` to the StS1 stale-claim scope'
Add-ContainsCheck -Name 'hard_stop_test_plan_scripts_readme_claim_scan_action' -Text $hardStop -Needle '`docs/test-plan.md` and `scripts/README.md` to the stale-claim scope'
Add-ContainsCheck -Name 'hard_stop_static_suite_14' -Text $hardStop -Needle 'static_suite_steps=14'
Add-ContainsCheck -Name 'hard_stop_static_suite_failures_zero' -Text $hardStop -Needle 'static_suite_failures=0'
Add-ContainsCheck -Name 'hard_stop_known_localization_gap_33' -Text $hardStop -Needle 'known_localization_gap=33'
Add-ContainsCheck -Name 'hard_stop_current_claim_scan_scope' -Text $hardStop -Needle 'current-validation, private-beta handoff, and release checklist docs'
Add-ContainsCheck -Name 'hard_stop_goal_claim_scan_scope' -Text $hardStop -Needle 'active event goal, test-ready goal, dev-environment, and root issues docs'
Add-ContainsCheck -Name 'hard_stop_review_claim_scan_scope' -Text $hardStop -Needle 'active review and toreview docs'
Add-ContainsCheck -Name 'hard_stop_sts1_feature_tree_claim_scan_scope' -Text $hardStop -Needle 'entire `docs/features/sts1-events` markdown/CSV tree'
Add-ContainsCheck -Name 'hard_stop_o60_o63_ledger_row_pins' -Text $hardStop -Needle 'O60-O63 now directly assert combat blocker, temporary non-parity, content parity, and asset/license safety row wording'
Add-ContainsCheck -Name 'hard_stop_project_state_claim_scan_scope' -Text $hardStop -Needle '`PROJECT_STATE.md` is included in stale-claim scan scope'
Add-ContainsCheck -Name 'hard_stop_current_validation_boundary_guard_action' -Text $hardStop -Needle 'guarded the current-validation O25-O76 pending/blocked boundary, Off-proof non-extension rule, and overlapping-validation-lane pause'
Add-ContainsCheck -Name 'hard_stop_revision_l_m_runtime_docs_scope' -Text $hardStop -Needle 'Revision L/M runtime goal docs to stale-claim and static-file-hygiene scopes'
Add-ContainsCheck -Name 'hard_stop_revision_l_supersession_notes' -Text $hardStop -Needle 'Revision L final/owner docs now carry Revision M supersession notes'
Add-ContainsCheck -Name 'hard_stop_revision_l_commit_dirty_supersession_notes' -Text $hardStop -Needle 'Revision L dirty/commit docs now carry Revision M supersession notes'
Add-ContainsCheck -Name 'hard_stop_warning_ledger_scope' -Text $hardStop -Needle 'Revision L/current warning ledgers now carry no-runtime-proof notes'
Add-ContainsCheck -Name 'hard_stop_overnight_status_scope' -Text $hardStop -Needle 'overnight run ledger/status now carry Revision M supersession notes'
Add-ContainsCheck -Name 'hard_stop_debug_refactor_scope' -Text $hardStop -Needle 'debug/refactor goal docs now carry Revision M current-override notes'
Add-ContainsCheck -Name 'hard_stop_status_board_diff_boundary' -Text $hardStop -Needle 'status-board.md` now records `git diff --check --` as exit-clean with CRLF normalization warnings only'
Add-ContainsCheck -Name 'hard_stop_event_goal_canary_4_6_scope' -Text $hardStop -Needle '`docs/goals/event.md`, `docs/features/sts1-events/multiplayer-fail-closed-guard.md`, and `docs/features/sts1-events/localization-gap-closure-plan.md` now use the current CanaryOnly requirement, 4 event types / 6 registration calls'
Add-ContainsCheck -Name 'hard_stop_test_plan_canary_4_6_scope' -Text $hardStop -Needle '`docs/features/sts1-events/test-plan.md` manual smoke order now requires CanaryOnly proof as 4 event types / 6 registration calls'
Add-ContainsCheck -Name 'hard_stop_test_plan_batch1_10_14_scope' -Text $hardStop -Needle '`docs/features/sts1-events/test-plan.md` manual smoke order now spells out the AdditiveBatch1 10 event types / 14 registration calls source shape'
Add-ContainsCheck -Name 'hard_stop_test_plan_verifier_report_scope' -Text $hardStop -Needle '`docs/features/sts1-events/test-plan.md` future CanaryOnly/AdditiveBatch1 smoke commands now pass `-OutFile` to both no-launch verifiers'
Add-ContainsCheck -Name 'hard_stop_runtime_smoke_checklist_verifier_report_scope' -Text $hardStop -Needle '`docs/features/ritsulib-migration/runtime-smoke-checklist.md` now mirrors that `-OutFile` verifier-report retention'
Add-ContainsCheck -Name 'hard_stop_summary_docs_verifier_report_scope' -Text $hardStop -Needle '`docs/features/sts1-events/status-board.md`, `docs/features/sts1-events/v19-gate-evidence-map.md`, and `scripts/README.md` now also describe retained verifier reports'
Add-ContainsCheck -Name 'hard_stop_next_overnight_enabled_before_gameplay_scope' -Text $hardStop -Needle '`docs/features/ritsulib-migration/next-overnight-run.md` now requires fresh current CanaryOnly 4 event types / 6 registration-call and AdditiveBatch1 10 event types / 14 registration-call smokes before StS1 gameplay evidence'
Add-ContainsCheck -Name 'hard_stop_monthly_batch4c_enabled_before_gameplay_scope' -Text $hardStop -Needle '`docs/features/ritsulib-migration/monthly-dev-spec.md` and `docs/features/ritsulib-migration/batch-4c-candidates.md` now keep fresh current CanaryOnly/AdditiveBatch1 enabled-mode smoke proof ahead of StS1 gameplay, handoff, or event-runtime claims.'
Add-ContainsCheck -Name 'hard_stop_test_ready_goal_enabled_boundary_scope' -Text $hardStop -Needle '`docs/test-ready-development-goal.md` now carries the same current StS1 enabled-mode boundary'
Add-ContainsCheck -Name 'hard_stop_active_summary_enabled_boundary_scope' -Text $hardStop -Needle '`docs/dev-environment.md`, `docs/issues.md`, `docs/review.md`, and `docs/toreview.md` now carry the current StS1 enabled-mode count boundary'
Add-ContainsCheck -Name 'hard_stop_handoff_release_enabled_boundary_scope' -Text $hardStop -Needle '`README.md`, `docs/README.md`, `docs/test-plan.md`, `docs/private-beta-verification-handoff.md`, and `docs/release-checklist.md` now also state the current CanaryOnly 4 event types / 6 registration-call and AdditiveBatch1 10 event types / 14 registration-call requirements before StS1 gameplay or handoff claims.'
Add-ContainsCheck -Name 'hard_stop_event_goal_pause_boundary_scope' -Text $hardStop -Needle '`docs/goals/event.md` now carries the same-repository validation-pause boundary, so its runtime-priority section cannot be read as permission to start `dotnet`, package/release-evidence, runtime smoke, staging, commit, or push processes while the migration validation lane is active.'
Add-ContainsCheck -Name 'hard_stop_owner_enabled_actions_pause_lift_scope' -Text $hardStop -Needle 'The owner-action list now repeats the pause-lift condition on the CanaryOnly/AdditiveBatch1 capture rows'
Add-ContainsCheck -Name 'hard_stop_owner_canary_after_pause' -Text $hardStop -Needle 'After the coordination pause is lifted, capture fresh current `v0.107.1` CanaryOnly enabled-mode smoke proving the exact canary set.'
Add-ContainsCheck -Name 'hard_stop_owner_batch1_after_pause' -Text $hardStop -Needle 'After the coordination pause is lifted, capture fresh current `v0.107.1` AdditiveBatch1 enabled-mode smoke proving 10 event types / 14 calls.'
Add-ContainsCheck -Name 'hard_stop_event_goal_downstream_pause_scope' -Text $hardStop -Needle '`docs/goals/event.md` now also repeats the pause boundary immediately before the runtime work-order sections'
Add-ContainsCheck -Name 'hard_stop_runtime_checklist_pause_scope' -Text $hardStop -Needle '`docs/features/ritsulib-migration/runtime-smoke-checklist.md` now carries a top-level coordination boundary'
Add-ContainsCheck -Name 'hard_stop_subagent_20260615_audits_scope' -Text $hardStop -Needle '`docs/features/sts1-events/v19-subagent-coverage.md` now records the 2026-06-15 read-only explorer audits'
Add-ContainsCheck -Name 'hard_stop_legacy_v5_monthly_scope' -Text $hardStop -Needle 'legacy v5 StS1 monthly spec now carries a Revision M current-override note'
Add-ContainsCheck -Name 'hard_stop_historical_review_scope' -Text $hardStop -Needle 'historical StS1 review reports now carry Revision M supersession notes'

Add-ContainsCheck -Name 'historical_hard_stop_v2_current_v19_pointer' -Text $historicalHardStopV2 -Needle 'Current status is tracked by `docs/features/sts1-events/v19-gate-evidence-map.md`, `docs/features/sts1-events/v19-gate-ledger.csv`, `docs/features/sts1-events/status-board.md`, and `docs/features/sts1-events/hard-stop-blocker-report-v19-validation-coordination-20260611.md`.'
Add-ContainsCheck -Name 'historical_hard_stop_v2_o76_scope' -Text $historicalHardStopV2 -Needle 'current v19 O0-O76 gates include default-Off-only proof, static source/doc guards, and many current-pending or blocked runtime rows.'
Add-ContainsCheck -Name 'historical_o24_handoff_current_v19_pointer' -Text $historicalO24Handoff -Needle 'Current status lives in `docs/features/sts1-events/v19-gate-evidence-map.md`, `docs/features/sts1-events/v19-gate-ledger.csv`, `docs/features/sts1-events/status-board.md`, and `docs/features/sts1-events/hard-stop-blocker-report-v19-validation-coordination-20260611.md`.'

Add-ContainsCheck -Name 'historical_hard_stop_v12_superseded' -Text $historicalHardStopV12 -Needle 'Superseded note, 2026-06-11: this report is historical hard-stop evidence only.'
Add-ContainsCheck -Name 'historical_hard_stop_v12_current_counts' -Text $historicalHardStopV12 -Needle 'Current source registration shape is 57 RegisterAll calls and AdditiveBatch1 10 event types / 14 registration calls'
Add-ContainsCheck -Name 'historical_hard_stop_v13_superseded' -Text $historicalHardStopV13 -Needle 'Superseded note, 2026-06-11: this report is historical hard-stop evidence only.'
Add-ContainsCheck -Name 'historical_hard_stop_v13_current_counts' -Text $historicalHardStopV13 -Needle 'Current source registration shape is 57 RegisterAll calls and AdditiveBatch1 10 event types / 14 registration calls'
Add-ContainsCheck -Name 'historical_hard_stop_v14_superseded' -Text $historicalHardStopV14 -Needle 'Superseded note, 2026-06-11: this report is historical hard-stop evidence only.'
Add-ContainsCheck -Name 'historical_hard_stop_v14_current_counts' -Text $historicalHardStopV14 -Needle 'Current source registration shape is 57 RegisterAll calls and AdditiveBatch1 10 event types / 14 registration calls'
Add-ContainsCheck -Name 'historical_hard_stop_v15_superseded' -Text $historicalHardStopV15 -Needle 'Superseded note, 2026-06-11: this report is historical root-cause evidence only.'
Add-ContainsCheck -Name 'historical_hard_stop_v15_default_off_only' -Text $historicalHardStopV15 -Needle 'Current beta.85 `v0.107.0` proof covers default-Off loader startup and patch application only'

Add-ContainsCheck -Name 'current_validation_mentions_gate_map' -Text $currentValidation -Needle 'The current O0-O76 gate map is `docs/features/sts1-events/v19-gate-evidence-map.md`, and the per-gate ledger is `docs/features/sts1-events/v19-gate-ledger.csv` guarded by `scripts/check-sts1-v19-gate-ledger.ps1`'
Add-ContainsCheck -Name 'current_validation_downstream_remains_pending_or_blocked' -Text $currentValidation -Needle 'This addendum closes only beta.87 package/source-shape loader registration proof for AdditiveBatch1.'
Add-ContainsCheck -Name 'current_validation_off_canary_proof_not_extended' -Text $currentValidation -Needle 'It does not prove event encounter gameplay, clicked UI, save-load, image rendering, replacement functional behavior, multiplayer fail-closed behavior, independent QA, release handoff, live-ready, or private-beta release readiness.'
Add-ContainsCheck -Name 'current_validation_off_packet_report_20260615' -Text $currentValidation -Needle 'The same already-captured beta.85 Off packet has a retained no-launch verifier report at `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/runtime-evidence-packet-check.json`: Off packet checks=34 / mismatches=0 after adding the explicit RitsuLib package-version target, nested log verifier checks=10 / mismatches=0.'
Add-ContainsCheck -Name 'current_validation_off_packet_report_non_claim' -Text $currentValidation -Needle 'This is evidence bookkeeping for default-Off proof only, not AdditiveBatch1, gameplay, save-load, replacement, multiplayer, image/render, or QA proof.'
Add-ContainsCheck -Name 'current_validation_latest_addendum_date' -Text $currentValidation -Needle 'Date: 2026-06-11; latest addendum: 2026-06-21'
Add-ContainsCheck -Name 'hard_stop_verifier_fail_on_mismatch_guard_20260615' -Text $hardStop -Needle '135. Hardened active verifier command examples so future copied-log and packet verifier evidence commands cannot drop `-FailOnMismatch` silently. `scripts/README.md` now documents `-FailOnMismatch` for both `check-sts1-enabled-mode-runtime-log.ps1` and `check-sts1-runtime-evidence-packet.ps1`, and `scripts/check-sts1-event-current-doc-claims.ps1` now rejects active command examples that include `-LogPath`/`-Mode` without `-FailOnMismatch`. Reran pause-safe static verification only: `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 803 checks / 0 mismatches, `check-sts1-event-static-suite.ps1` returned 14 static steps / 0 suite failures with the known 33-key localization gap, `check-sts1-static-file-hygiene.ps1 -FailOnMismatch` returned 7 checks / 0 mismatches, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, `check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 63 checks / 0 mismatches, and focused `git diff --check --` on the touched StS1 docs/scripts exited 0. No build/test/publish/runtime/release/stage/commit/push process was started, and no runtime gate was closed.'
Add-ContainsCheck -Name 'hard_stop_verifier_target_switch_guard_20260615' -Text $hardStop -Needle '136. Hardened active verifier command examples so future copied-log and packet verifier evidence commands cannot drop explicit target switches silently. `scripts/check-sts1-event-current-doc-claims.ps1` now rejects active copied-log and packet command examples that include `-LogPath` or `-Mode` without `-ExpectedPackageVersion`, `-ExpectedRitsuCompatBranch`, `-ExpectedRitsuLibVersion`, and `-ExpectedGameVersion`. Reran pause-safe static verification only: `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 812 checks / 0 mismatches, `check-sts1-event-static-suite.ps1` returned 14 static steps / 0 suite failures with the known 33-key localization gap, `check-sts1-static-file-hygiene.ps1 -FailOnMismatch` returned 7 checks / 0 mismatches, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, `check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 63 checks / 0 mismatches, and focused `git diff --check --` on the touched StS1 docs/scripts exited 0. No build/test/publish/runtime/release/stage/commit/push process was started, and no runtime gate was closed.'
Add-ContainsCheck -Name 'hard_stop_enabled_packet_missing_state_bypass_guard_20260615' -Text $hardStop -Needle '137. Hardened active packet verifier command examples so future CanaryOnly/AdditiveBatch1 evidence commands cannot reintroduce the legacy missing-state bypass switches. `scripts/check-sts1-event-current-doc-claims.ps1` now rejects active enabled-mode packet commands that include `-AllowMissingSessionState` or `-AllowMissingRestoreState`. Reran pause-safe static verification only: `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 814 checks / 0 mismatches, `check-sts1-event-static-suite.ps1` returned 14 static steps / 0 suite failures with the known 33-key localization gap, `check-sts1-static-file-hygiene.ps1 -FailOnMismatch` returned 7 checks / 0 mismatches, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, `check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 63 checks / 0 mismatches, and focused `git diff --check --` on the touched StS1 docs/scripts exited 0. No build/test/publish/runtime/release/stage/commit/push process was started, and no runtime gate was closed.'
Add-ContainsCheck -Name 'hard_stop_static_suite_composition_guard_20260615' -Text $hardStop -Needle '138. Hardened `scripts/check-sts1-event-current-doc-claims.ps1` so the aggregate static suite''s full 14-step composition, fail-closed guard wrappers, and step/failure summary output are directly asserted. Reran pause-safe static verification only: `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 832 checks / 0 mismatches, `check-sts1-event-static-suite.ps1` returned 14 static steps / 0 suite failures with the known 33-key localization gap, `check-sts1-static-file-hygiene.ps1 -FailOnMismatch` returned 7 checks / 0 mismatches, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, `check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 63 checks / 0 mismatches, and focused `git diff --check --` on the touched StS1 docs/scripts exited 0. No build/test/publish/runtime/release/stage/commit/push process was started, and no runtime gate was closed.'
Add-ContainsCheck -Name 'hard_stop_audit_log_command_guard_20260615' -Text $hardStop -Needle '139. Hardened active `audit-godot-log.ps1` command examples so future enabled-mode smoke instructions must retain `godot-log-audit.json` with `-OutFile` and fail on audit hits with `-FailOnHit`; the guard also checks that both CanaryOnly and AdditiveBatch1 smoke sections include the retained audit command. Reran pause-safe static verification only: `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 838 checks / 0 mismatches, `check-sts1-event-static-suite.ps1` returned 14 static steps / 0 suite failures with the known 33-key localization gap, `check-sts1-static-file-hygiene.ps1 -FailOnMismatch` returned 7 checks / 0 mismatches, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, `check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 63 checks / 0 mismatches, and focused `git diff --check --` on the touched StS1 docs/scripts exited 0. No build/test/publish/runtime/release/stage/commit/push process was started, and no runtime gate was closed.'
Add-ContainsCheck -Name 'hard_stop_live_session_command_guard_20260615' -Text $hardStop -Needle '140. Hardened active `spire-plus-live-session.ps1` command examples so future enabled-mode smoke instructions must keep explicit E-drive `-GameRoot` / `-SteamExe`, `-SteamUserId`, `-MoveOtherMods`, `-MoveCurrentRuns`, `-Launch`, `-StopGameOnRestore`, and `-PreserveNewCurrentRunsOnRestore`; count checks cover both CanaryOnly and AdditiveBatch1 smoke sections. Reran pause-safe static verification only: `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 850 checks / 0 mismatches, `check-sts1-event-static-suite.ps1` returned 14 static steps / 0 suite failures with the known 33-key localization gap, `check-sts1-static-file-hygiene.ps1 -FailOnMismatch` returned 7 checks / 0 mismatches, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, `check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 63 checks / 0 mismatches, and focused `git diff --check --` on the touched StS1 docs/scripts exited 0. No build/test/publish/runtime/release/stage/commit/push process was started, and no runtime gate was closed.'
Add-ContainsCheck -Name 'hard_stop_runtime_smoke_checklist_live_session_prereq_guard_20260615' -Text $hardStop -Needle '141. Hardened `docs/features/ritsulib-migration/runtime-smoke-checklist.md` so its `spire-plus-live-session.ps1` prerequisite now requires explicit E-drive `-GameRoot` / `-SteamExe`, `-SteamUserId`, `-Mode Prepare -MoveOtherMods -MoveCurrentRuns -Launch`, restore with `-Mode Restore -StopGameOnRestore -PreserveNewCurrentRunsOnRestore`, and keeping `STS2-RitsuLib` enabled. Reran pause-safe static verification only: `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 856 checks / 0 mismatches, `check-sts1-event-static-suite.ps1` returned 14 static steps / 0 suite failures with the known 33-key localization gap, `check-sts1-static-file-hygiene.ps1 -FailOnMismatch` returned 7 checks / 0 mismatches, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, `check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 63 checks / 0 mismatches, and focused `git diff --check --` on the touched StS1 docs/scripts exited 0. No build/test/publish/runtime/release/stage/commit/push process was started, and no runtime gate was closed. This was later superseded by action 142 below.'
Add-ContainsCheck -Name 'hard_stop_runtime_smoke_checklist_scan_scope_guard_20260615' -Text $hardStop -Needle '142. Hardened `scripts/check-sts1-event-current-doc-claims.ps1` so `docs/features/ritsulib-migration/runtime-smoke-checklist.md` is directly asserted in broad current-claim stale-scan scope. Reran pause-safe static verification only: `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 859 checks / 0 mismatches, `check-sts1-event-static-suite.ps1` returned 14 static steps / 0 suite failures with the known 33-key localization gap, `check-sts1-static-file-hygiene.ps1 -FailOnMismatch` returned 7 checks / 0 mismatches, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, `check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 63 checks / 0 mismatches, and focused `git diff --check --` on the touched StS1 docs/scripts exited 0. No build/test/publish/runtime/release/stage/commit/push process was started, and no runtime gate was closed. This was later superseded by action 143 below.'
Add-ContainsCheck -Name 'hard_stop_static_file_hygiene_runtime_checklist_scope_guard_20260615' -Text $hardStop -Needle '143. Hardened `scripts/check-sts1-static-file-hygiene.ps1` so `docs/features/ritsulib-migration/runtime-smoke-checklist.md` is directly asserted in static-file hygiene scope, then guarded that assertion from `scripts/check-sts1-event-current-doc-claims.ps1`. Reran pause-safe static verification only: `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 863 checks / 0 mismatches, `check-sts1-event-static-suite.ps1` returned 14 static steps / 0 suite failures with the known 33-key localization gap, `check-sts1-static-file-hygiene.ps1 -FailOnMismatch` returned 8 checks / 0 mismatches, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, `check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 63 checks / 0 mismatches, and focused `git diff --check --` on the touched StS1 docs/scripts exited 0. No build/test/publish/runtime/release/stage/commit/push process was started, and no runtime gate was closed. This was later superseded by action 144 below.'
Add-ContainsCheck -Name 'hard_stop_next_overnight_scope_guard_20260615' -Text $hardStop -Needle '144. Hardened `scripts/check-sts1-event-current-doc-claims.ps1` and `scripts/check-sts1-static-file-hygiene.ps1` so `docs/features/ritsulib-migration/next-overnight-run.md` is directly asserted in broad current-claim stale-scan scope and static-file hygiene scope. Reran pause-safe static verification only: `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 868 checks / 0 mismatches, `check-sts1-event-static-suite.ps1` returned 14 static steps / 0 suite failures with the known 33-key localization gap, `check-sts1-static-file-hygiene.ps1 -FailOnMismatch` returned 9 checks / 0 mismatches, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, `check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 63 checks / 0 mismatches, and focused `git diff --check --` on the touched StS1 docs/scripts exited 0. No build/test/publish/runtime/release/stage/commit/push process was started, and no runtime gate was closed. This was later superseded by action 145 below.'
Add-ContainsCheck -Name 'hard_stop_ritsu_planning_doc_static_file_hygiene_guard_20260615' -Text $hardStop -Needle '145. Hardened `scripts/check-sts1-static-file-hygiene.ps1` so `docs/features/ritsulib-migration/monthly-dev-spec.md` and `docs/features/ritsulib-migration/batch-4c-candidates.md` are directly asserted in static-file hygiene scope, then guarded those assertions from `scripts/check-sts1-event-current-doc-claims.ps1`. Reran pause-safe static verification only: `check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 872 checks / 0 mismatches, `check-sts1-event-static-suite.ps1` returned 14 static steps / 0 suite failures with the known 33-key localization gap, `check-sts1-static-file-hygiene.ps1 -FailOnMismatch` returned 11 checks / 0 mismatches, `check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches, `check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 63 checks / 0 mismatches, and focused `git diff --check --` on the touched StS1 docs/scripts exited 0. No build/test/publish/runtime/release/stage/commit/push process was started, and no runtime gate was closed.'

Add-ContainsCheck -Name 'hard_stop_v20_title' -Text $hardStopV20 -Needle '# StS1 Event Port v20 Coordination Pause Hard Stop'
Add-ContainsCheck -Name 'hard_stop_v20_not_completion' -Text $hardStopV20 -Needle 'It is not a completion claim and does not close gameplay, save-load, replacement, multiplayer, image/render, QA, handoff, or release gates. Later shared validation updates closed CanaryOnly loader proof and AdditiveBatch1 loader/registration proof only.'
Add-ContainsCheck -Name 'hard_stop_v20_exact_gate_id' -Text $hardStopV20 -Needle '## Exact Gate Id'
Add-ContainsCheck -Name 'hard_stop_v20_blocked_o25_o33' -Text $hardStopV20 -Needle '`O25`: retained `v0.107.0` CanaryOnly enabled-mode smoke has since been captured by the shared validation lane and is loader proof only; this thread did not create it. Previous beta.96 Off loader proof exists on `v0.107.1` as previous-package startup proof after the beta.97 package refresh, but current beta.97 CanaryOnly enabled-mode proof still needs recapture.'
Add-ContainsCheck -Name 'hard_stop_v20_current_head_2c2801dd' -Text $hardStopV20 -Needle '| Current HEAD observed in this thread | `2c2801dd (HEAD -> main, origin/main, origin/HEAD) Split Distinguished Cape guards` |'
Add-ContainsCheck -Name 'hard_stop_v20_current_dirty_exact_recapture' -Text $hardStopV20 -Needle '| Current worktree observed in this thread | `git status --short` after the `2c2801dd` recapture reported only this hard-stop recapture alignment slice: modified `docs/features/sts1-events/hard-stop-blocker-report-v20-coordination-pause-20260617.md` and modified `scripts/check-sts1-event-current-doc-claims.ps1`; treat later dirty files as separate scope requiring exact recapture before commit or handoff. |'
Add-ContainsCheck -Name 'hard_stop_v20_owner_actions' -Text $hardStopV20 -Needle '## Owner / External Action Required'
Add-ContainsCheck -Name 'hard_stop_v20_final_blocked_summary' -Text $hardStopV20 -Needle '## Final Blocked-Gate Summary'
Add-ContainsCheck -Name 'hard_stop_v20_next_run_start' -Text $hardStopV20 -Needle '## Next Run Start Point'
Add-ContainsCheck -Name 'hard_stop_v20_no_commit_push' -Text $hardStopV20 -Needle 'No staging, commit, or push was performed from this thread while this hard stop was written.'
Add-ContainsCheck -Name 'hard_stop_v20_why_continuation_impossible' -Text $hardStopV20 -Needle '## Why Continuation Is Impossible In This Moment'
Add-ContainsCheck -Name 'hard_stop_v20_next_run_o0_o25_o33' -Text $hardStopV20 -Needle 'Recapture O0-O15 in one non-overlapping validation lane.'

Add-ContainsCheck -Name 'current_validation_pause_safe_static_verification_20260615' -Text $currentValidation -Needle 'Pause-safe static verification was rerun after adding active summary direct localization non-proof guards: `scripts/check-sts1-event-static-suite.ps1` completed 14 static steps with 0 suite failures, keeping the 33-key localization gap as known/non-failing; `scripts/check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 715 checks / 0 mismatches in that pass, later superseded by the 872-check follow-up below; `scripts/check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 531 checks / 0 mismatches.'
Add-ContainsCheck -Name 'current_validation_enabled_mode_verifier_contract_guard_verification_20260615' -Text $currentValidation -Needle 'Pause-safe subagent evidence-packet checklist hardening, replacement gate-range split hardening, O14/O15 source-identity classification, format-pause wording, direct-key localization non-proof mirroring, status-board remaining-gate split hardening, post-pause QA/release/owner row splitting, evidence-map runtime/static range splitting, active event-goal checkpoint guarding, active-goal stale-count scope guarding, subagent owner/final-handoff non-authorization guarding, direct enabled-mode copied-log `-AuditPath` hardening, verifier `-FailOnMismatch` command guards, verifier expected-target command guards, enabled-mode packet missing-state bypass guards, aggregate static-suite composition/fail-closed wrapper guards, retained `audit-godot-log.ps1` command guards, live-session prepare/restore command guards, runtime-smoke checklist live-session prerequisite guard, runtime-smoke checklist broad stale-scan inclusion guard, runtime-smoke checklist static-file hygiene scope guard, next-overnight runtime-plan stale-scan/static-hygiene scope guards, and RitsuLib monthly/Batch 4c static-file hygiene scope guards were added to `docs/goals/event.md`, `docs/features/sts1-events/v19-subagent-coverage.md`, `docs/features/sts1-events/test-plan.md`, `docs/features/sts1-events/status-board.md`, `docs/features/sts1-events/localization.md`, `docs/features/sts1-events/v19-gate-evidence-map.md`, `docs/features/ritsulib-migration/runtime-smoke-checklist.md`, `docs/features/ritsulib-migration/next-overnight-run.md`, `docs/features/ritsulib-migration/monthly-dev-spec.md`, `docs/features/ritsulib-migration/batch-4c-candidates.md`, `scripts/README.md`, and the v19 hard-stop report, then guarded by `scripts/check-sts1-v19-subagent-coverage.ps1`, `scripts/check-sts1-static-file-hygiene.ps1 -FailOnMismatch`, and `scripts/check-sts1-event-current-doc-claims.ps1 -FailOnMismatch`. The follow-up static rerun returned `scripts/check-sts1-event-static-suite.ps1` 14 static steps / 0 suite failures, `scripts/check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` 872 checks / 0 mismatches, `scripts/check-sts1-static-file-hygiene.ps1 -FailOnMismatch` 11 checks / 0 mismatches, `scripts/check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` 531 checks / 0 mismatches, `scripts/check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` 63 checks / 0 mismatches, and `git diff --check --` exit 0 with CRLF warnings only.'
Add-ContainsCheck -Name 'current_validation_v20_static_alignment_20260617' -Text $currentValidation -Needle 'StS1 v20 static alignment follow-up stayed inside the same pause boundary: after tuple-aware enabled-mode log verifier, CanaryOnly current-pass, repo-manifest runtime-preflight drift guard alignment, beta.86 AdditiveBatch1 doc alignment, and retained-loader subagent split, `scripts/check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 956 checks / 0 mismatches; `scripts/check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 70 checks / 0 mismatches across 15 roles'
Add-ContainsCheck -Name 'current_validation_v20_overlay_route' -Text $currentValidation -Needle 'The O76-O84 final documentation/handoff overlay is `docs/features/sts1-events/v20-final-gate-overlay.csv` guarded by `scripts/check-sts1-v20-final-gate-overlay.ps1`; it is static-only and does not close runtime or handoff gates.'
Add-ContainsCheck -Name 'current_validation_v20_hard_stop_report' -Text $currentValidation -Needle 'The v20 coordination-pause hard-stop report is `docs/features/sts1-events/hard-stop-blocker-report-v20-coordination-pause-20260617.md`; it records the current blocked gates and next-run start point without closing gameplay or handoff gates.'
Add-ContainsCheck -Name 'current_validation_read_only_runtime_preflight_20260617' -Text $currentValidation -Needle 'The later beta.86-target preflight returned 27 checks / 0 mismatches after the versioned package refresh. This is prerequisite/source-shape evidence only; enabled-mode proof comes only from the retained beta.86 direct AdditiveBatch1 verifier packet, and gameplay/handoff rows remain open.'
Add-ContainsCheck -Name 'current_validation_pause_safe_static_non_claim_20260615' -Text $currentValidation -Needle 'This static verification does not close O33, gameplay, save/load, replacement, multiplayer, image/render, QA, release, or handoff gates; O25 was closed separately by the retained beta.85 CanaryOnly runtime packet, not by static verification.'
Add-ContainsCheck -Name 'current_validation_validation_pause_boundary' -Text $currentValidation -Needle 'do not start overlapping validation lanes'
Add-ContainsCheck -Name 'current_validation_static_suite_15' -Text $currentValidation -Needle 'The current expected summary is 15 static steps, 0 suite failures'
Add-ContainsCheck -Name 'current_validation_static_suite_file_hygiene' -Text $currentValidation -Needle 'static-file hygiene'
Add-ContainsCheck -Name 'current_validation_localization_closure_plan' -Text $currentValidation -Needle 'docs/features/sts1-events/localization-gap-closure-plan.md'
Add-ContainsCheck -Name 'current_validation_historical_target_fix_old_shape_nonclaim' -Text $currentValidation -Needle 'Historical Off mode proved 0 StS1 registration lines; historical CanaryOnly proved exactly 4 canary content registrations for that source/runtime state. Retained beta.85 default-Off/CanaryOnly and beta.87 AdditiveBatch1 proof are previous `v0.107.0` loader context; previous beta.96 RitsuLib-only Off proof is previous-package startup evidence after the beta.97 settings I18N package refresh, while previous beta.93 AdditiveBatch1 proof is previous-package registration context only.'
Add-ContainsCheck -Name 'handoff_mentions_gate_map' -Text $privateBetaHandoff -Needle 'Use `docs/features/sts1-events/v19-gate-evidence-map.md` and `docs/features/sts1-events/v19-gate-ledger.csv` for the current O0-O76 gate split, plus `docs/features/sts1-events/v20-final-gate-overlay.csv` for the O76-O84 final documentation/handoff overlay and `docs/features/sts1-events/hard-stop-blocker-report-v20-coordination-pause-20260617.md` for the current v20 hard-stop/next-run start point; the ledgers are guarded by `scripts/check-sts1-v19-gate-ledger.ps1` and `scripts/check-sts1-v20-final-gate-overlay.ps1`'
Add-ContainsCheck -Name 'handoff_localization_closure_plan' -Text $privateBetaHandoff -Needle 'docs/features/sts1-events/localization-gap-closure-plan.md'
Add-ContainsCheck -Name 'handoff_direct_localization_nonproof' -Text $privateBetaHandoff -Needle 'Closing only the direct Golden Idol missing key remains a localization unblocker, not gameplay proof or a replacement for verifier reports.'
Add-ContainsCheck -Name 'handoff_sts1_enabled_smokes_current_split' -Text $privateBetaHandoff -Needle 'Current AdditiveBatch1 enabled-mode proof remains loader/registration evidence only and does not prove gameplay.'
Add-ContainsCheck -Name 'release_checklist_mentions_gate_map' -Text $releaseChecklist -Needle 'Use `docs/features/sts1-events/v19-gate-evidence-map.md` and `docs/features/sts1-events/v19-gate-ledger.csv`, guarded by `scripts/check-sts1-v19-gate-ledger.ps1`, for the current O0-O76 gate split, plus `docs/features/sts1-events/v20-final-gate-overlay.csv`, guarded by `scripts/check-sts1-v20-final-gate-overlay.ps1`, for the O76-O84 final documentation/handoff overlay and `docs/features/sts1-events/hard-stop-blocker-report-v20-coordination-pause-20260617.md` for the current v20 hard-stop/next-run start point before any event handoff claim.'
Add-ContainsCheck -Name 'release_checklist_localization_closure_plan' -Text $releaseChecklist -Needle 'docs/features/sts1-events/localization-gap-closure-plan.md'
Add-ContainsCheck -Name 'release_checklist_direct_localization_nonproof' -Text $releaseChecklist -Needle 'Closing only the direct Golden Idol missing key remains a localization unblocker, not gameplay proof or a replacement for verifier reports.'
Add-ContainsCheck -Name 'release_checklist_sts1_enabled_smokes_current_split' -Text $releaseChecklist -Needle 'Before any simple-batch gameplay or handoff claim, AdditiveBatch1 gameplay screenshots/logs must be captured separately; before any canary gameplay claim, the retained CanaryOnly proof must stay tied to the current package/source shape and gameplay screenshots/logs must be captured separately.'
Add-ContainsCheck -Name 'release_evidence_status_beta85_off_nonclaim' -Text $releaseEvidenceStatus -Needle 'beta.96 RitsuLib-only Off direct loader smoke and beta.96 Mod Settings page proof are previous-package context after beta.97. Gameplay, clicked Ancient UI, save-load, preview-tools live behavior, beta.97 loader/settings proof, current enabled-mode proof, Vakuu, co-op, and full release-evidence packaging rows remain pending.'
Add-ContainsCheck -Name 'release_evidence_status_manual_evidence_pending' -Text $releaseEvidenceStatus -Needle 'Gameplay/manual evidence remains pending.'
Add-ContainsCheck -Name 'private_beta_release_audit_loader_only' -Text $privateBetaReleaseAudit -Needle 'Pending for beta.97; gameplay evidence pending'
Add-ContainsCheck -Name 'private_beta_release_audit_not_release_ready' -Text $privateBetaReleaseAudit -Needle 'It is not private-beta release-ready.'
Add-ContainsCheck -Name 'private_beta_release_audit_current_beta94_date_boundary' -Text $privateBetaReleaseAudit -Needle 'Original audit date: 2026-05-15; latest beta.97 RitsuLib-only package refresh: 2026-06-21; previous beta.96 Off loader proof and previous beta.93 AdditiveBatch1 evidence remain historical loader/registration proof only.'
Add-ContainsCheck -Name 'test_ready_completion_audit_current_beta94_date_boundary' -Text $testReadyCompletionAudit -Needle 'Original audit date: 2026-05-14; latest beta.97 RitsuLib-only settings-page I18N package refresh: 2026-06-21; previous beta.96 Off loader proof and previous beta.93 AdditiveBatch1 evidence remain historical loader/registration proof only.'
Add-ContainsCheck -Name 'test_ready_completion_audit_beta85_loader_nonclaim' -Text $testReadyCompletionAudit -Needle 'These hashes and earlier loader proof do not close beta.97 loader proof, live gameplay, save-load, death/failure path, or co-op verification.'
Add-ContainsCheck -Name 'test_ready_completion_audit_manual_rows_pending' -Text $testReadyCompletionAudit -Needle 'Live gameplay, save-load, death/failure-path, co-op, and clicked Ancient UI remain pending.'
Add-ContainsCheck -Name 'platform_testing_current_beta85_zip' -Text $platformTesting -Needle 'publish/SpirePlus-v0.1.0-private-beta.97.zip'
Add-ContainsCheck -Name 'platform_testing_hashes_not_gameplay' -Text $platformTesting -Needle 'Passing the installed package checks proves only that the same files are installed'
Add-ContainsCheck -Name 'platform_testing_rows_stay_open' -Text $platformTesting -Needle 'Keep those rows open until their own screenshots, logs, and result notes exist.'

Add-NoRegexCheck -Name 'no_stale_project_map_counts' -Paths $currentClaimFiles -Pattern 'RegisterAll is now 56|10 event types / 13 calls'
Add-NoRegexCheck -Name 'no_stale_mode_matrix_counts' -Paths $currentClaimFiles -Pattern '11 registrations / 10 event types|54 registration calls \(47'
Add-NoRegexCheck -Name 'no_current_loader_overclaim_sentence' -Paths $currentClaimFiles -Pattern 'loader proof is now captured for Off, CanaryOnly, and AdditiveBatch1|source-guarded and loader-verified'
Add-NoRegexCheck -Name 'no_current_canary_pass_row' -Paths $currentClaimFiles -Pattern '\| CanaryOnly \|.*\[PASS\].*K1'
Add-NoRegexCheck -Name 'no_current_batch1_pass_row' -Paths $currentClaimFiles -Pattern '\| AdditiveBatch1 \|.*\[PASS\].*(11 registration|10 event types through 11)'
Add-NoRegexCheck -Name 'no_stale_v19_runtime_gate_ranges' -Paths $currentClaimFiles -Pattern 'O34-O47.*replacement|O51-O52.*independent QA|Replacement functional proof \(O43-O46\)|Multiplayer fail-closed \(O47\)|QA Red-Team \(O51/O52\)'
Add-NoRegexCheck -Name 'no_stale_static_suite_step_count' -Paths $currentClaimFiles -Pattern 'static_suite_steps=10|10 static steps|static_suite_steps=12|12 static steps|static_suite_steps=13|13 static steps'
Add-NoRegexCheck -Name 'no_stale_localization_gap_baseline_check_count' -Paths $currentClaimFiles -Pattern 'gap baseline checker: 9 checks / 0 mismatches'
Add-NoRegexCheck -Name 'no_stale_beta85_off_runtime_fix_path' -Paths $currentClaimFiles -Pattern 'v01070-beta85-off-runtime-fix-20260611-024832'
Add-NoRegexCheck -Name 'no_current_source_workspace_command_without_expected_package_target' -Paths $currentClaimFiles -Pattern 'check-local-godot-source-workspace\.ps1(?=[^\r\n]*-RequireCurrentSourceSnapshot)(?=[^\r\n]*-ExpectedRitsuLibVersion)(?![^\r\n]*-ExpectedPackageVersion)'
Add-NoRegexCheck -Name 'no_current_migration_residue_from_externalmod_or_old_claim_counts' -Paths $currentClaimFiles -Pattern 'current-doc claims (1316|1320|1324|1325|1326|1328) / 0|current-doc claims passed (1316|1320|1324|1325|1326|1328) / 0|returned (1316|1320|1324|1325|1326|1328) checks / 0 mismatches|ExternalMod and RitsuLib runtime dependency|ExternalMod \+ RitsuLib runtime dependency|manifest [^\r\n]*ExternalMod[^\r\n]*RitsuLib runtime dependency'
$currentDocClaimSummaryFiles = @('PROJECT_STATE.md', 'docs\goals\event.md', 'docs\features\sts1-events\status-board.md', 'docs\reviews\current-validation.md', 'docs\features\sts1-events\v19-gate-evidence-map.md')
Add-NoRegexCheck -Name 'no_unsuperseded_legacy_current_doc_claim_counts_in_active_summaries' -Paths $currentDocClaimSummaryFiles -Pattern 'current-doc claims (692|695|698|701|703|705|708|711|715) / 0|current-doc claims passed (692|695|698|701|703|705|708|711|715) / 0|returned (692|695|698|701|703|705|708|711|715) checks / 0 mismatches(?![^\r\n]*(later superseded|was later superseded))'
Add-NoRegexCheck -Name 'no_unsuperseded_recent_current_doc_claim_counts_in_active_summaries' -Paths $currentDocClaimSummaryFiles -Pattern 'current-doc claims (744|745|746|748|749|750|751|752|753|754|756|759|761|762|763|764|768|769|775|778|783|784|787|788|790|791|792|793|798|802|803|811|812|813|814|832|836|838|848|850|856|859|863|868|881|890|893|896|897|898|914|915) / 0(?![^\r\n]*(later superseded|was later superseded))|current-doc claims passed (744|745|746|748|749|750|751|752|753|754|756|759|761|762|763|764|768|769|775|778|783|784|787|788|790|791|792|793|798|802|803|811|812|813|814|832|836|838|848|850|856|859|863|868|881|890|893|896|897|898|914|915) / 0(?![^\r\n]*(later superseded|was later superseded))|returned (744|745|746|748|749|750|751|752|753|754|756|759|761|762|763|764|768|769|775|778|783|784|787|788|790|791|792|793|798|802|803|811|812|813|814|832|836|838|848|850|856|859|863|868|881|890|893|896|897|898|914|915) checks / 0 mismatches(?![^\r\n]*(later superseded|was later superseded))|(744|745|746|748|749|750|751|752|753|754|756|759|761|762|763|764|768|769|775|778|783|784|787|788|790|791|792|793|798|802|803|811|812|813|814|832|836|838|848|850|856|859|863|868|881|890|893|896|897|898|914|915)-check follow-up'
Add-NoRegexCheck -Name 'no_status_board_flattened_runtime_gate_ranges' -Paths @('docs\features\sts1-events\status-board.md') -Pattern 'multiplayer fail-closed/runtime rows \(O58-O64\)|independent QA/final handoff rows \(O65-O75\)'
Add-NoRegexCheck -Name 'no_subagent_coverage_flattened_qa_release_owner_range' -Paths @('docs\features\sts1-events\v19-subagent-coverage.md') -Pattern 'QA / Red-Team Subagent \+ Release Documentation Subagent.*O65-O75'
Add-NoRegexCheck -Name 'no_gate_map_flattened_o58_o76_ranges' -Paths @('docs\features\sts1-events\v19-gate-evidence-map.md') -Pattern '\| `O58-O64` \| static classification pass / runtime proof blocked \||\| `O65-O76` \| blocked / documentation in progress \|'
Add-NoRegexCheck -Name 'no_off_packet_command_without_ritsu_lib_target_in_active_sts1_docs' -Paths @('docs\features\sts1-events\test-plan.md', 'docs\features\sts1-events\v19-gate-evidence-map.md') -Pattern 'check-sts1-runtime-evidence-packet\.ps1 -Mode Off(?![^\r\n]*-ExpectedRitsuLibVersion)'
Add-NoRegexCheck -Name 'no_runtime_packet_command_without_expected_package_target' -Paths $currentClaimFiles -Pattern '^\s*\.\\scripts\\check-sts1-runtime-evidence-packet\.ps1(?=.*-Mode)(?!.*-ExpectedPackageVersion)'
Add-NoRegexCheck -Name 'no_runtime_packet_command_without_expected_ritsu_branch_target' -Paths $currentClaimFiles -Pattern '^\s*\.\\scripts\\check-sts1-runtime-evidence-packet\.ps1(?=.*-Mode)(?!.*-ExpectedRitsuCompatBranch)'
Add-NoRegexCheck -Name 'no_runtime_packet_command_without_expected_ritsu_lib_target' -Paths $currentClaimFiles -Pattern '^\s*\.\\scripts\\check-sts1-runtime-evidence-packet\.ps1(?=.*-Mode)(?!.*-ExpectedRitsuLibVersion)'
Add-NoRegexCheck -Name 'no_runtime_packet_command_without_expected_game_target' -Paths $currentClaimFiles -Pattern '^\s*\.\\scripts\\check-sts1-runtime-evidence-packet\.ps1(?=.*-Mode)(?!.*-ExpectedGameVersion)'
Add-NoRegexCheck -Name 'no_runtime_packet_command_without_outfile' -Paths $currentClaimFiles -Pattern '^\s*\.\\scripts\\check-sts1-runtime-evidence-packet\.ps1(?=.*-Mode)(?!.*-OutFile)'
Add-NoRegexCheck -Name 'no_runtime_packet_command_without_fail_on_mismatch' -Paths $currentClaimFiles -Pattern '^\s*\.\\scripts\\check-sts1-runtime-evidence-packet\.ps1(?=.*-Mode)(?!.*-FailOnMismatch)'
Add-NoRegexCheck -Name 'no_enabled_runtime_packet_command_with_missing_state_bypass' -Paths $currentClaimFiles -Pattern '^\s*\.\\scripts\\check-sts1-runtime-evidence-packet\.ps1(?=.*-Mode\s+(CanaryOnly|AdditiveBatch1))(?=.*-(AllowMissingSessionState|AllowMissingRestoreState))'
$autoSlayProofCommandFiles = @($currentClaimFiles + @('docs\testing\runtime-monkey-stability.md')) | Sort-Object -Unique
Add-Check -Name 'autoslay_packet_proof_command_scan_includes_runtime_monkey_docs' -Passed ($autoSlayProofCommandFiles -contains 'docs\testing\runtime-monkey-stability.md') -Detail 'AutoSlay proof command scan must include docs\testing\runtime-monkey-stability.md because the canonical proof command template lives there'
$autoSlayProofCommandRecognizerAccepts = @(
    '.\scripts\check-spire-plus-autoslay-packet.ps1 -FailOnMismatch',
    '& ".\scripts\check-spire-plus-autoslay-packet.ps1" -FailOnMismatch',
    "& '.\scripts\check-spire-plus-autoslay-packet.ps1' -FailOnMismatch"
)
$autoSlayProofCommandRecognizerRejects = @(
    '.\scripts\check-spire-plus-autoslay-packet.ps1 -OutFile "<evidence>\autoslay-packet-check.json"',
    '.\scripts\check-spire-plus-autoslay-packet.ps1x -FailOnMismatch',
    '.\scripts\check-sts1-runtime-evidence-packet.ps1 -FailOnMismatch'
)
$autoSlayProofCommandRecognizerFailures = @(
    @($autoSlayProofCommandRecognizerAccepts | Where-Object { -not (Test-AutoSlayProofCommandText -Text $_) } | ForEach-Object { "missed expected: $_" }) +
    @($autoSlayProofCommandRecognizerRejects | Where-Object { Test-AutoSlayProofCommandText -Text $_ } | ForEach-Object { "accepted unexpected: $_" })
)
Add-Check -Name 'autoslay_packet_proof_command_recognizes_quoted_paths' -Passed ($autoSlayProofCommandRecognizerFailures.Count -eq 0) -Detail $(if ($autoSlayProofCommandRecognizerFailures.Count -eq 0) { 'AutoSlay proof command recognizer accepts bare, double-quoted, and single-quoted PowerShell script paths and rejects non-proof commands' } else { $autoSlayProofCommandRecognizerFailures -join ' | ' })
Add-AutoSlayProofCommandPresentCheck -Name 'autoslay_packet_proof_command_present' -Paths $autoSlayProofCommandFiles
Add-AutoSlayProofCommandTargetCheck -Name 'autoslay_packet_proof_commands_include_expected_ancient_ids' -Paths $autoSlayProofCommandFiles
Add-AutoSlayProofCommandNoSwitchCheck -Name 'autoslay_packet_proof_commands_do_not_allow_missing_event_traversal' -Paths $autoSlayProofCommandFiles -SwitchName 'AllowMissingEventTraversal'
Add-AutoSlayProofCommandRequiredSwitchesCheck -Name 'autoslay_packet_proof_commands_include_current_targets_and_report' -Paths $autoSlayProofCommandFiles -SwitchNames @('MinRuns', 'ExpectedPackageVersion', 'ExpectedGameVersion', 'ExpectedRitsuLibVersion', 'ExpectedRitsuCompatBranch', 'ExpectedPatchCount', 'OutFile')
Add-AutoSlayProofCommandSwitchValuesCheck -Name 'autoslay_packet_proof_commands_pin_current_target_values' -Paths $autoSlayProofCommandFiles -SwitchValues @{
    MinRuns = '1000'
    ExpectedPackageVersion = 'v0.1.0-private-beta.97'
    ExpectedGameVersion = '0.107.1'
    ExpectedRitsuLibVersion = '0.4.31'
    ExpectedRitsuCompatBranch = '0.107.1'
    ExpectedPatchCount = '25'
}
Add-AutoSlayProofCommandSwitchValuesCheck -Name 'autoslay_packet_proof_commands_pin_current_ancient_targets' -Paths $autoSlayProofCommandFiles -SwitchValues @{
    ExpectedAncientIds = 'VAKUU,URDA,MORVI,LOTHA'
}
Add-NoRegexCheck -Name 'no_live_session_prepare_command_without_game_root' -Paths $currentClaimFiles -Pattern '^\s*\.\\scripts\\spire-plus-live-session\.ps1(?=.*-Mode\s+Prepare)(?!.*-GameRoot)'
Add-NoRegexCheck -Name 'no_live_session_prepare_command_without_steam_exe' -Paths $currentClaimFiles -Pattern '^\s*\.\\scripts\\spire-plus-live-session\.ps1(?=.*-Mode\s+Prepare)(?!.*-SteamExe)'
Add-NoRegexCheck -Name 'no_live_session_prepare_command_without_steam_user_id' -Paths $currentClaimFiles -Pattern '^\s*\.\\scripts\\spire-plus-live-session\.ps1(?=.*-Mode\s+Prepare)(?!.*-SteamUserId)'
Add-NoRegexCheck -Name 'no_live_session_prepare_command_without_move_other_mods' -Paths $currentClaimFiles -Pattern '^\s*\.\\scripts\\spire-plus-live-session\.ps1(?=.*-Mode\s+Prepare)(?!.*-MoveOtherMods)'
Add-NoRegexCheck -Name 'no_live_session_prepare_command_without_move_current_runs' -Paths $currentClaimFiles -Pattern '^\s*\.\\scripts\\spire-plus-live-session\.ps1(?=.*-Mode\s+Prepare)(?!.*-MoveCurrentRuns)'
Add-NoRegexCheck -Name 'no_live_session_prepare_command_without_launch' -Paths $currentClaimFiles -Pattern '^\s*\.\\scripts\\spire-plus-live-session\.ps1(?=.*-Mode\s+Prepare)(?!.*-Launch)'
Add-NoRegexCheck -Name 'no_live_session_restore_command_without_stop_game' -Paths $currentClaimFiles -Pattern '^\s*\.\\scripts\\spire-plus-live-session\.ps1(?=.*-Mode\s+Restore)(?!.*-StopGameOnRestore)'
Add-NoRegexCheck -Name 'no_live_session_restore_command_without_preserve_current_runs' -Paths $currentClaimFiles -Pattern '^\s*\.\\scripts\\spire-plus-live-session\.ps1(?=.*-Mode\s+Restore)(?!.*-PreserveNewCurrentRunsOnRestore)'
Add-NoRegexCheck -Name 'no_audit_godot_log_command_without_outfile' -Paths $currentClaimFiles -Pattern '^\s*\.\\scripts\\audit-godot-log\.ps1(?=.*godot\.log)(?!.*-OutFile)'
Add-NoRegexCheck -Name 'no_audit_godot_log_command_without_fail_on_hit' -Paths $currentClaimFiles -Pattern '^\s*\.\\scripts\\audit-godot-log\.ps1(?=.*godot\.log)(?!.*-FailOnHit)'
Add-NoRegexCheck -Name 'no_audit_godot_log_command_without_audit_json' -Paths $currentClaimFiles -Pattern '^\s*\.\\scripts\\audit-godot-log\.ps1(?=.*godot\.log)(?!.*godot-log(?:-current-iteration)?-audit\.json)'
Add-NoRegexCheck -Name 'no_enabled_log_command_without_expected_package_target' -Paths $currentClaimFiles -Pattern '^\s*\.\\scripts\\check-sts1-enabled-mode-runtime-log\.ps1(?=.*-LogPath)(?!.*-ExpectedPackageVersion)'
Add-NoRegexCheck -Name 'no_enabled_log_command_without_expected_ritsu_branch_target' -Paths $currentClaimFiles -Pattern '^\s*\.\\scripts\\check-sts1-enabled-mode-runtime-log\.ps1(?=.*-LogPath)(?!.*-ExpectedRitsuCompatBranch)'
Add-NoRegexCheck -Name 'no_enabled_log_command_without_expected_ritsu_lib_target' -Paths $currentClaimFiles -Pattern '^\s*\.\\scripts\\check-sts1-enabled-mode-runtime-log\.ps1(?=.*-LogPath)(?!.*-ExpectedRitsuLibVersion)'
Add-NoRegexCheck -Name 'no_enabled_log_command_without_expected_game_target' -Paths $currentClaimFiles -Pattern '^\s*\.\\scripts\\check-sts1-enabled-mode-runtime-log\.ps1(?=.*-LogPath)(?!.*-ExpectedGameVersion)'
Add-NoRegexCheck -Name 'no_enabled_log_command_without_audit_path' -Paths $currentClaimFiles -Pattern '^\s*\.\\scripts\\check-sts1-enabled-mode-runtime-log\.ps1(?=.*-LogPath)(?!.*-AuditPath)'
Add-NoRegexCheck -Name 'no_enabled_log_command_without_outfile' -Paths $currentClaimFiles -Pattern '^\s*\.\\scripts\\check-sts1-enabled-mode-runtime-log\.ps1(?=.*-LogPath)(?!.*-OutFile)'
Add-NoRegexCheck -Name 'no_enabled_log_command_without_fail_on_mismatch' -Paths $currentClaimFiles -Pattern '^\s*\.\\scripts\\check-sts1-enabled-mode-runtime-log\.ps1(?=.*-LogPath)(?!.*-FailOnMismatch)'
Add-NoRegexCheck -Name 'no_enabled_log_command_uses_after_launch_as_canonical_log' -Paths $currentClaimFiles -Pattern '^\s*\.\\scripts\\check-sts1-enabled-mode-runtime-log\.ps1(?=.*-LogPath\s+"?[^"\r\n]*godot\.log\.after-launch)'
Add-NoRegexCheck -Name 'no_next_overnight_untracked_batch4c_claim' -Paths @('docs\features\ritsulib-migration\next-overnight-run.md') -Pattern 'untracked `docs/features/ritsulib-migration/batch-4c-candidates\.md`'
Add-NoRegexCheck -Name 'no_current_validation_unscoped_canary4_content_claim' -Paths @('docs\reviews\current-validation.md') -Pattern 'CanaryOnly proves exactly 4 canary content registrations\.'
Add-NoRegexCheck -Name 'no_unscoped_canary4_content_claim_in_current_claims' -Paths $currentClaimFiles -Pattern 'CanaryOnly proves exactly 4 canary content registrations\.'
Add-NoRegexCheck -Name 'no_current_validation_unscoped_historical_runtime_pass_claims' -Paths @('docs\reviews\current-validation.md') -Pattern '^\s*## Revision J Current Snapshot\s*$|\| PASS \|.*(Off-mode Steam smoke|CanaryOnly|AdditiveBatch1|Clean audit)|Sts1Events (Off|CanaryOnly) runtime proof: \*\*PASS\*\*|Runtime loader gate: Off and CanaryOnly diagnostic smokes now pass'
Add-NoRegexCheck -Name 'no_current_claim_sandbox_only_artifact_links' -Paths $currentClaimFiles -Pattern 'sandbox:/mnt'

$report = [pscustomobject]@{
    Checks = $checks
    Mismatches = $mismatches
}

foreach ($check in $checks) {
    $status = if ($check.Passed) { 'pass' } else { 'fail' }
    Write-Output "$($check.Name) status=$status"
}

Write-Output "checks=$($checks.Count)"
Write-Output "mismatches=$($mismatches.Count)"

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
