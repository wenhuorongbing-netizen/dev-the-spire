param(
    [string]$EvidenceRoot,

    [switch]$SkipPendingVerifier
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

function Get-HandoffRoot {
    param([string]$RequestedPath)

    New-DirectoryIfMissing -Path $runtimeRoot

    if ([string]::IsNullOrWhiteSpace($RequestedPath)) {
        $stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
        return [System.IO.Path]::GetFullPath((Join-Path $runtimeRoot "manual-test-handoff-$stamp"))
    }

    $candidate = if ([System.IO.Path]::IsPathRooted($RequestedPath)) {
        $RequestedPath
    } else {
        Join-Path $repoRoot $RequestedPath
    }

    return [System.IO.Path]::GetFullPath($candidate)
}

function Get-RepoRelativePath {
    param([Parameter(Mandatory = $true)][string]$Path)

    $fullPath = [System.IO.Path]::GetFullPath($Path).TrimEnd('\', '/')
    $repoFull = [System.IO.Path]::GetFullPath($repoRoot).TrimEnd('\', '/')
    $comparison = [System.StringComparison]::OrdinalIgnoreCase

    if ($fullPath.Equals($repoFull, $comparison)) {
        return '.'
    }

    if ($fullPath.StartsWith($repoFull + '\', $comparison)) {
        return $fullPath.Substring($repoFull.Length + 1)
    }

    return $fullPath
}

function Format-PowerShellSingleQuotedArgument {
    param([Parameter(Mandatory = $true)][string]$Value)

    return "'" + ($Value -replace "'", "''") + "'"
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

function Invoke-RepoScript {
    param(
        [Parameter(Mandatory = $true)][string]$ScriptName,
        [Parameter(Mandatory = $true)][string[]]$ArgumentList
    )

    $scriptPath = Join-Path $PSScriptRoot $ScriptName
    if (-not (Test-Path -LiteralPath $scriptPath -PathType Leaf)) {
        throw "Missing helper script: $scriptPath"
    }

    $powerShellExe = Get-PowerShellExecutable
    $childArgs = @('-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', $scriptPath) + $ArgumentList
    $output = & $powerShellExe @childArgs 2>&1
    $exitCode = $LASTEXITCODE
    if ($null -eq $exitCode) {
        $exitCode = 0
    }

    return [pscustomobject]@{
        Script = $scriptPath
        ExitCode = [int]$exitCode
        Output = @($output | ForEach-Object { $_.ToString() })
    }
}

function Assert-Success {
    param([Parameter(Mandatory = $true)]$Result)

    if ($Result.Output.Count -gt 0) {
        $Result.Output | Write-Output
    }

    if ($Result.ExitCode -ne 0) {
        throw "$($Result.Script) failed with exit code $($Result.ExitCode)."
    }
}

function Get-FileSha256 {
    param([Parameter(Mandatory = $true)][string]$Path)

    for ($attempt = 1; $attempt -le 5; $attempt++) {
        try {
            return (Get-FileHash -Algorithm SHA256 -LiteralPath $Path).Hash.ToUpperInvariant()
        } catch {
            if ($attempt -eq 5) {
                throw
            }

            Start-Sleep -Milliseconds (100 * $attempt)
        }
    }
}

function Get-PreservedCurrentLoaderRow {
    param([Parameter(Mandatory = $true)][string]$ManifestPath)

    if (-not (Test-Path -LiteralPath $ManifestPath -PathType Leaf)) {
        return $null
    }

    $manifest = Get-Content -Raw -LiteralPath $ManifestPath -Encoding UTF8 | ConvertFrom-Json
    $manifestPackagePath = [string]$manifest.PackagePath
    $manifestPackageSha256 = [string]$manifest.PackageSha256
    if ([string]::IsNullOrWhiteSpace($manifestPackagePath) -or [string]::IsNullOrWhiteSpace($manifestPackageSha256)) {
        return $null
    }

    $packageFullPath = if ([System.IO.Path]::IsPathRooted($manifestPackagePath)) {
        $manifestPackagePath
    } else {
        Join-Path $repoRoot $manifestPackagePath
    }

    if (-not (Test-Path -LiteralPath $packageFullPath -PathType Leaf)) {
        return $null
    }

    if ((Get-FileSha256 -Path $packageFullPath) -ne $manifestPackageSha256.ToUpperInvariant()) {
        return $null
    }

    $loaderRow = @($manifest.Rows | Where-Object {
            [string]$_.Id -eq 'fresh-current-package-loader-smoke' -and
            [string]$_.Status -eq 'pass'
        } | Select-Object -First 1)

    if ($loaderRow.Count -eq 0) {
        return $null
    }

    $evidenceDir = [string]$loaderRow[0].EvidenceDir
    if ([string]::IsNullOrWhiteSpace($evidenceDir) -or -not (Test-Path -LiteralPath $evidenceDir -PathType Container)) {
        return $null
    }

    $packageHashesPath = Join-Path $evidenceDir 'package-hashes.json'
    if (-not (Test-Path -LiteralPath $packageHashesPath -PathType Leaf)) {
        return $null
    }

    $packageHashes = Get-Content -Raw -LiteralPath $packageHashesPath -Encoding UTF8 | ConvertFrom-Json
    $packageHashRow = @($packageHashes.Files | Where-Object {
            [string]$_.Path -eq $manifestPackagePath -and
            [string]$_.Sha256 -eq $manifestPackageSha256
        } | Select-Object -First 1)
    if ($packageHashRow.Count -eq 0) {
        return $null
    }

    return $loaderRow[0]
}

function Set-ReleaseManifestRow {
    param(
        [Parameter(Mandatory = $true)][string]$ManifestPath,
        [Parameter(Mandatory = $true)]$Row
    )

    $manifest = Get-Content -Raw -LiteralPath $ManifestPath -Encoding UTF8 | ConvertFrom-Json
    $rows = @($manifest.Rows | ForEach-Object {
            if ([string]$_.Id -eq [string]$Row.Id) {
                $Row
            } else {
                $_
            }
        })

    $manifest.Rows = @($rows)
    $manifest | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $ManifestPath -Encoding UTF8
}

function Move-StaleCurrentLoaderEvidence {
    param([Parameter(Mandatory = $true)][string]$ReleaseEvidenceRoot)

    $loaderEvidenceDir = Join-Path $ReleaseEvidenceRoot 'fresh-current-package-loader-smoke'
    if (-not (Test-Path -LiteralPath $loaderEvidenceDir -PathType Container)) {
        return $null
    }

    $staleEvidenceFiles = @(
        'godot.log',
        'godot-log-audit.json',
        'enabled-mods.txt'
    )
    $existingStaleFiles = @($staleEvidenceFiles | Where-Object {
            Test-Path -LiteralPath (Join-Path $loaderEvidenceDir $_) -PathType Leaf
        })
    if ($existingStaleFiles.Count -eq 0) {
        return $null
    }

    $archiveRoot = Join-Path $loaderEvidenceDir '.stale-loader-evidence'
    New-DirectoryIfMissing -Path $archiveRoot
    $archiveDir = Join-Path $archiveRoot (Get-Date -Format 'yyyyMMdd-HHmmss')
    New-DirectoryIfMissing -Path $archiveDir

    foreach ($fileName in $existingStaleFiles) {
        $sourcePath = Join-Path $loaderEvidenceDir $fileName
        $targetPath = Join-Path $archiveDir $fileName
        Move-Item -LiteralPath $sourcePath -Destination $targetPath -Force
    }

    @(
        '# Stale loader evidence',
        '',
        'These files were moved out of `fresh-current-package-loader-smoke` because the current package hash changed or the loader pass row could not be preserved.',
        'They are historical context only. Capture a fresh `godot.log`, `godot-log-audit.json`, and `enabled-mods.txt` before marking the current loader row pass.'
    ) -join [Environment]::NewLine | Set-Content -LiteralPath (Join-Path $archiveDir 'README.md') -Encoding UTF8

    return $archiveDir
}

$handoffRoot = Get-HandoffRoot -RequestedPath $EvidenceRoot
Assert-PathInside -Child $handoffRoot -Parent $runtimeRoot -Label 'EvidenceRoot'
New-DirectoryIfMissing -Path $handoffRoot
$releaseManifestPath = Join-Path $handoffRoot 'release\release-evidence-manifest.json'
$preservedCurrentLoaderRow = Get-PreservedCurrentLoaderRow -ManifestPath $releaseManifestPath
$staleCurrentLoaderArchive = $null

$summary = [ordered]@{
    CreatedAt = (Get-Date).ToString('o')
    HandoffRoot = $handoffRoot
    NoLaunch = $null -eq $preservedCurrentLoaderRow
    PendingVerifierChecked = -not [bool]$SkipPendingVerifier
    Sections = [ordered]@{}
    Notice = if ($null -ne $preservedCurrentLoaderRow) {
        'Current-package loader row is filled. Remaining gameplay, clicked UI, save-load, preview-tools, and co-op rows are pending.'
    } else {
        'No game was launched. These folders are templates only; live rows remain pending until filled with screenshots, logs, and notes.'
    }
}

Assert-Success (Invoke-RepoScript -ScriptName 'collect-release-evidence.ps1' -ArgumentList @(
        '-NoLaunch',
        '-EvidenceDir', (Join-Path $handoffRoot 'release')
    ))
$summary.Sections['release'] = Join-Path $handoffRoot 'release'

if ($null -ne $preservedCurrentLoaderRow) {
    Set-ReleaseManifestRow -ManifestPath $releaseManifestPath -Row $preservedCurrentLoaderRow
} else {
    $staleCurrentLoaderArchive = Move-StaleCurrentLoaderEvidence -ReleaseEvidenceRoot (Join-Path $handoffRoot 'release')
}

Assert-Success (Invoke-RepoScript -ScriptName 'collect-vakuu-fight-evidence.ps1' -ArgumentList @(
        '-NoLaunch',
        '-EvidenceDir', (Join-Path $handoffRoot 'vakuu')
    ))
$summary.Sections['vakuu'] = Join-Path $handoffRoot 'vakuu'

Assert-Success (Invoke-RepoScript -ScriptName 'collect-preview-tools-evidence.ps1' -ArgumentList @(
        '-NoLaunch',
        '-EvidenceDir', (Join-Path $handoffRoot 'preview-tools')
    ))
$summary.Sections['preview-tools'] = Join-Path $handoffRoot 'preview-tools'

Assert-Success (Invoke-RepoScript -ScriptName 'collect-coop-evidence.ps1' -ArgumentList @(
        '-NoLaunch',
        '-EvidenceDir', (Join-Path $handoffRoot 'coop')
    ))
$summary.Sections['coop'] = Join-Path $handoffRoot 'coop'

$ancientUiRoot = Join-Path $handoffRoot 'ancient-ui'
foreach ($ancient in @('URDA', 'MORVI', 'LOTHA', 'VAKUU')) {
    Assert-Success (Invoke-RepoScript -ScriptName 'collect-ancient-ui-evidence.ps1' -ArgumentList @(
            '-Mode', 'Prepare',
            '-Ancient', $ancient,
            '-EvidenceDir', (Join-Path $ancientUiRoot $ancient),
            '-NoPreflight'
        ))
}

Assert-Success (Invoke-RepoScript -ScriptName 'collect-ancient-ui-evidence.ps1' -ArgumentList @(
        '-Mode', 'Prepare',
        '-Ancient', 'VAKUU',
        '-ForceVakuuFight',
        '-EvidenceDir', (Join-Path $ancientUiRoot 'VAKUU-FIGHT'),
        '-NoPreflight'
    ))
$summary.Sections['ancient-ui'] = $ancientUiRoot

if (-not $SkipPendingVerifier) {
    $verifyResult = Invoke-RepoScript -ScriptName 'verify-spire-plus-release-evidence.ps1' -ArgumentList @(
        '-EvidenceRoot', (Join-Path $handoffRoot 'release')
    )

    if ($verifyResult.ExitCode -eq 0) {
        throw 'Pending release evidence unexpectedly passed verification.'
    }

    $outputText = $verifyResult.Output -join [Environment]::NewLine
    if ($outputText -notmatch 'FailureCount' -or $outputText -notmatch 'pending') {
        throw 'Pending release evidence failed for an unexpected reason; expected pending-row failures.'
    }

    $pendingReport = $outputText | ConvertFrom-Json
    $expectedRequiredRowCount = 21
    $expectedFailureCount = if ($null -ne $preservedCurrentLoaderRow) { 20 } else { 21 }
    if ([int]$pendingReport.RequiredRowCount -ne $expectedRequiredRowCount -or [int]$pendingReport.FailureCount -ne $expectedFailureCount) {
        throw "Pending release evidence should fail closed on exactly $expectedFailureCount live rows. RequiredRowCount=$($pendingReport.RequiredRowCount) FailureCount=$($pendingReport.FailureCount)."
    }

    $summary.PendingVerifierExitCode = $verifyResult.ExitCode
    $summary.PendingVerifierExpectedFailure = $true
    $summary.PendingVerifierRequiredRowCount = [int]$pendingReport.RequiredRowCount
    $summary.PendingVerifierFailureCount = [int]$pendingReport.FailureCount
    $summary.PendingVerifierWarningCount = [int]$pendingReport.WarningCount
    if ($null -ne $preservedCurrentLoaderRow) {
        $summary.CurrentVerifierFailureCount = [int]$pendingReport.FailureCount
        $summary.CurrentLoaderEvidenceDir = [string]$preservedCurrentLoaderRow.EvidenceDir
    }
    if ($null -ne $staleCurrentLoaderArchive) {
        $summary.StaleCurrentLoaderArchive = $staleCurrentLoaderArchive
    }
}

$releaseManifest = Get-Content -Raw -LiteralPath $releaseManifestPath | ConvertFrom-Json
$packagePath = [string]$releaseManifest.PackagePath
$packageSha256 = [string]$releaseManifest.PackageSha256
$releaseRootRelative = Get-RepoRelativePath -Path (Join-Path $handoffRoot 'release')
$releaseManifestRelative = Get-RepoRelativePath -Path $releaseManifestPath
$verifierCommand =
    'scripts\verify-spire-plus-release-evidence.ps1 -EvidenceRoot ' +
    (Format-PowerShellSingleQuotedArgument -Value $releaseRootRelative) +
    ' -ManifestPath ' +
    (Format-PowerShellSingleQuotedArgument -Value $releaseManifestRelative) +
    ' -WritePassMarker'

$summary.PackagePath = $packagePath
$summary.PackageSha256 = $packageSha256
$summary.VerifierCommand = $verifierCommand

$summaryPath = Join-Path $handoffRoot 'handoff-summary.json'
$summary | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $summaryPath -Encoding UTF8

$loaderRowFilled = $null -ne $preservedCurrentLoaderRow
$expectedFailureCount = if ($loaderRowFilled -and $summary.Contains('CurrentVerifierFailureCount')) {
    [int]$summary.CurrentVerifierFailureCount
} elseif ($summary.Contains('PendingVerifierFailureCount')) {
    [int]$summary.PendingVerifierFailureCount
} else {
    21
}
$startHereIntro = if ($loaderRowFilled) {
    'This folder is a manual-test evidence scaffold. The current-package loader row is filled; Mod Settings, gameplay, UI, save-load, preview-tools, and co-op rows remain pending until a tester adds screenshots, logs, notes, and checklist results.'
} else {
    'This folder is a manual-test evidence scaffold. It was generated without launching the game, so every live row remains pending until a tester adds screenshots, logs, notes, and checklist results.'
}
$staleArchiveRelative = if ($null -ne $staleCurrentLoaderArchive) {
    Get-RepoRelativePath -Path $staleCurrentLoaderArchive
} else {
    $null
}
$handoffSummaryLines = if ($loaderRowFilled) {
    @(
        '- `handoff-summary.json` records the current scaffold plus preserved loader proof.',
        '- `PendingVerifierRequiredRowCount=21`.',
        ('- `CurrentVerifierFailureCount=' + $expectedFailureCount + '`.'),
        '- `PendingVerifierWarningCount=0`.',
        '- Current loader smoke has filled `release/fresh-current-package-loader-smoke/`; remaining rows still need live Mod Settings and feature evidence.'
    )
} else {
    $lines = @(
        '- `handoff-summary.json` records this no-launch scaffold contract.',
        '- `PendingVerifierRequiredRowCount=21`.',
        '- `PendingVerifierFailureCount=21`.',
        '- `PendingVerifierWarningCount=0`.',
        '- These numbers mean the scaffold is expected to fail until live evidence is filled.'
    )
    if ($null -ne $staleArchiveRelative) {
        $lines += ('- Stale loader files from a non-preserved package hash were moved to `' + $staleArchiveRelative + '`; capture fresh loader files before marking the current loader row pass.')
    }

    $lines
}
$recommendedOrderLoaderLine = if ($loaderRowFilled) {
    '3. The current loader row is already filled. Fill `release/mod-settings-current-display/` with current Mods-list and Spire Plus config-page screenshots before feature rows.'
} else {
    '3. Run the installed package and fill `release/fresh-current-package-loader-smoke/` first, then fill `release/mod-settings-current-display/` with current Mods-list and Spire Plus config-page screenshots.'
}

$startHereLines = @(
    '# Spire Plus Tester Start Here',
    '',
    $startHereIntro,
    '',
    '## Package under test',
    '',
    '- Player-facing mod: `Spire Plus`.',
    '- Install note: enable `Spire Plus` in game. The current compatibility folder inside the package is `EZMicroBalance`.',
    ('- ZIP: `' + $packagePath + '`.'),
    ('- ZIP SHA256: `' + $packageSha256 + '`.'),
    '',
    '## Handoff summary',
    ''
) + $handoffSummaryLines + @(
    '',
    '## Recommended order',
    '',
    '1. Verify the installed package hashes and packaged Sere Talon / Tanx Claws split:',
    '   ```powershell',
    '   .\scripts\check-installed-spire-plus-package.ps1 -ModDirectory "D:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance"',
    '   ```',
    ("2. Run the release verifier before live testing once. It should fail closed with $expectedFailureCount pending live rows; a pass at this point means the evidence scaffold is wrong."),
    $recommendedOrderLoaderLine,
    '4. Capture clicked Ancient UI evidence in `ancient-ui/` for Urda, Morvi, Lotha, Vakuu normal, and Vakuu force-fight.',
    '5. Fill gameplay checklist rows under `release/` for Ancient rewards, player text, art routing, Rootblight, A11-A20, and A19/A20 boss abilities.',
    '6. Test the hidden Vakuu fight using the focused `vakuu/` and release Vakuu rows.',
    '7. Test preview tools and co-op last; these rows need clean logs and either two-client proof or an explicit accepted deferral.',
    '',
    '## Focused current regression check',
    '',
    '- Vakuu event option: pick `Sere Talon`. It must be the Vakuu relic that offers 4 Curses, lets you choose 1, then adds that Curse, 2 Wish, and 1 Wish+.',
    '- It must not show Tanx Claws relic art, title, or Maul-transform text. If the effect is curse choice plus 2 Wish and 1 Wish+ but the art is still Tanx Claws, treat it as a Spire Plus UI/package-load issue.',
    '- Capture the event option, relic bar, inspect screen, and hover tooltip for Sere Talon.',
    '- Check `godot.log` for Sere Talon route lines on `Ancient event option button`, `RelicModel packed icon texture`, `RelicModel big icon texture`, `NRelic small node`, and `NRelic large node`, or a Sere Talon icon route skip warning.',
    '- Test Tanx Claws separately. Tanx Claws should remain the Maul-transform relic and should create upgraded Maul cards.',
    '',
    '## Evidence rule',
    '',
    'Do not mark rows pass from source review. A pass row needs live files in its row folder and a final `release-evidence-verifier-pass.json` from `scripts\verify-spire-plus-release-evidence.ps1`.',
    '',
    'Verifier command after live evidence is filled:',
    '',
    '```powershell',
    $verifierCommand,
    '```'
)
$startHere = $startHereLines -join [Environment]::NewLine
$startHere | Set-Content -LiteralPath (Join-Path $handoffRoot 'TESTER_START_HERE.md') -Encoding UTF8

$readmeIntro = if ($loaderRowFilled) {
    'This folder preserves current-package loader smoke evidence. The remaining rows are manual-test templates, not gameplay proof.'
} else {
    'This folder was generated without launching the game. It is a template set for manual testing, not proof that any live row passed.'
}
$readmeSummaryLines = if ($loaderRowFilled) {
    @(
        ('Summary: `handoff-summary.json` records `PendingVerifierRequiredRowCount=21`, current failure count `' + $expectedFailureCount + '`, and `PendingVerifierWarningCount=0`.'),
        '',
        'These are scaffold/verifier values, not gameplay proof.',
        ''
    )
} else {
    $lines = @(
        'Summary: `handoff-summary.json` records `PendingVerifierRequiredRowCount=21`, `PendingVerifierFailureCount=21`, and `PendingVerifierWarningCount=0`.',
        '',
        'Those are expected no-launch values, not live proof.',
        ''
    )
    if ($null -ne $staleArchiveRelative) {
        $lines += 'Stale loader files from the previous hash were archived at `' + $staleArchiveRelative + '`. Capture fresh loader files in `release/fresh-current-package-loader-smoke/` before marking that row pass.'
        $lines += ''
    }

    $lines
}

$readmeLines = @(
    '# Spire Plus Manual Test Handoff',
    '',
    "Created: $($summary.CreatedAt)",
    '',
    $readmeIntro,
    '',
    ('Package under test: `' + $packagePath + '` (`' + $packageSha256 + '`).'),
    ''
) + $readmeSummaryLines + @(
    'Start with `TESTER_START_HERE.md` for the recommended manual-test order and final verifier command.',
    '',
    '## Folders',
    '',
    '- `release/`: verifier-readable release evidence manifest and row folders.',
    '- `ancient-ui/`: focused clicked-UI capture plans for Urda, Morvi, Lotha, Vakuu normal, and Vakuu force-fight.',
    '- `vakuu/`: focused hidden Vakuu fight gameplay/save-load rows.',
    '- `preview-tools/`: Crystal Sphere and transform preview rows.',
    '- `coop/`: two-client co-op rows.',
    '',
    'Verifier command after live evidence is filled:',
    '',
    '```powershell',
    $verifierCommand,
    '```',
    '',
    'Run the verifier only after the release rows have live screenshots, logs, and notes. Pending rows are expected to fail closed.'
)
$readme = $readmeLines -join [Environment]::NewLine
$readme | Set-Content -LiteralPath (Join-Path $handoffRoot 'README.md') -Encoding UTF8

Write-Output "Prepared complete manual-test handoff under $handoffRoot"
Write-Output "Summary: $summaryPath"
if ($loaderRowFilled) {
    Write-Output 'Current-package loader row was preserved. Remaining Mod Settings, gameplay, clicked UI, save-load, preview-tools, and co-op rows remain pending.'
} else {
    Write-Output 'No game was launched. All live rows remain pending.'
}

exit 0
