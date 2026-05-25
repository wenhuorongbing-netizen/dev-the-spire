param(
    [ValidateSet('Markdown', 'Json')]
    [string]$Format = 'Markdown',

    [string]$OutputPath,

    [string]$PathspecDirectory,

    [switch]$FailOnUnclassified
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path

function ConvertTo-SlashPath {
    param([Parameter(Mandatory = $true)][string]$Path)

    return ($Path -replace '\\', '/')
}

function Get-StatusPath {
    param([Parameter(Mandatory = $true)][string]$Line)

    if ($Line.Length -lt 4) {
        return $Line.Trim()
    }

    $path = $Line.Substring(3).Trim()
    if ($path.Contains(' -> ')) {
        $path = ($path -split ' -> ')[-1].Trim()
    }

    return $path.Trim('"')
}

function Get-WorktreeBatch {
    param([Parameter(Mandatory = $true)][string]$Path)

    $p = ConvertTo-SlashPath -Path $Path

    if ($p -eq '.gitignore' -or
        $p -eq 'output/.gdignore' -or
        $p.StartsWith('output/playwright/', [System.StringComparison]::OrdinalIgnoreCase)) {
        return 0
    }

    if ($p.StartsWith('website/', [System.StringComparison]::OrdinalIgnoreCase) -or
        $p.StartsWith('forum/', [System.StringComparison]::OrdinalIgnoreCase) -or
        $p.StartsWith('docs/features/forum/', [System.StringComparison]::OrdinalIgnoreCase) -or
        $p -match '^tests/EZMicroBalance\.Tests/WebsiteContent.*\.cs(\.uid)?$') {
        return 7
    }

    if ($p -match '^tests/EZMicroBalance\.Tests/[^/]+\.cs\.uid$') {
        return 5
    }

    if ($p.StartsWith('EZMicroBalanceCode/Ascension/', [System.StringComparison]::OrdinalIgnoreCase) -or
        $p -match '^EZMicroBalance/localization/[^/]+/ascension\.json$' -or
        $p.StartsWith('docs/features/ascension-11-20/', [System.StringComparison]::OrdinalIgnoreCase) -or
        $p -match '^tests/EZMicroBalance\.Tests/(Ascension|BossDedicated|EnemyDamage).*\.cs(\.uid)?$') {
        return 4
    }

    if ($p.StartsWith('EZMicroBalanceCode/Ancients/', [System.StringComparison]::OrdinalIgnoreCase) -or
        $p -match '^EZMicroBalance/localization/[^/]+/(ancients|cards|card_keywords|powers|relics)\.json$' -or
        $p.StartsWith('docs/features/ancient-expansion-urda/', [System.StringComparison]::OrdinalIgnoreCase) -or
        $p.StartsWith('docs/features/ancients-rework-v4/', [System.StringComparison]::OrdinalIgnoreCase) -or
        $p -match '^tests/EZMicroBalance\.Tests/(Ancient|Urda|Morvi|Lotha|Vakuu).*\.cs(\.uid)?$') {
        return 3
    }

    if ($p.StartsWith('EZMicroBalance/images/', [System.StringComparison]::OrdinalIgnoreCase) -or
        $p -eq 'export_presets.cfg' -or
        $p.StartsWith('docs/features/ancient-expansion-v2.2/', [System.StringComparison]::OrdinalIgnoreCase) -or
        $p -eq 'docs/issues/waiting-tests.md') {
        return 6
    }

    if ($p.StartsWith('scripts/', [System.StringComparison]::OrdinalIgnoreCase) -or
        $p -eq 'EZMicroBalance.json' -or
        $p -eq 'EZMicroBalance.csproj' -or
        $p -eq 'project.godot' -or
        $p -eq 'EZMicroBalanceCode/MainFile.cs' -or
        $p.StartsWith('EZMicroBalanceCode/Config/', [System.StringComparison]::OrdinalIgnoreCase) -or
        $p.StartsWith('EZMicroBalanceCode/Diagnostics/', [System.StringComparison]::OrdinalIgnoreCase) -or
        $p.StartsWith('EZMicroBalanceCode/Modding/', [System.StringComparison]::OrdinalIgnoreCase) -or
        $p.StartsWith('EZMicroBalanceCode/Preview/', [System.StringComparison]::OrdinalIgnoreCase) -or
        $p -match '^EZMicroBalance/localization/[^/]+/settings_ui\.json$' -or
        $p.StartsWith('docs/features/preview-tools/', [System.StringComparison]::OrdinalIgnoreCase) -or
        $p.StartsWith('.github/', [System.StringComparison]::OrdinalIgnoreCase) -or
        $p -eq 'tests/EZMicroBalance.Tests/README.md' -or
        $p -eq 'docs/patch-inventory.md' -or
        $p -eq 'docs/platform-testing.md' -or
        $p -match '^tests/EZMicroBalance\.Tests/(Release|Engineering|TestInfrastructure|CrossPlatform|SourceApiDrift|ActiveSourceManifest|Documentation|MultiplayerPolicy|PreviewTools|SaveStateContracts|ModInfoLocalization).*\.cs(\.uid)?$') {
        return 5
    }

    if ($p -eq 'AGENTS.md' -or
        $p -eq 'PROJECT_STATE.md' -or
        $p -eq 'README.md' -or
        $p -eq 'docs/BETA_COMPATIBILITY.md' -or
        $p -match '^docs/(issues|toreview|review|dev-environment|private-beta|release|test-ready|test-plan|mod-changelog).*\.md$') {
        return 1
    }

    if ($p.StartsWith('docs/architecture/', [System.StringComparison]::OrdinalIgnoreCase) -or
        $p.StartsWith('docs/specs/', [System.StringComparison]::OrdinalIgnoreCase) -or
        $p.StartsWith('docs/month-plan/', [System.StringComparison]::OrdinalIgnoreCase) -or
        $p -eq 'docs/architecture-ez-micro-balance.md' -or
        $p -eq 'docs/REMOTE_DEVELOPMENT_SETUP.md' -or
        $p.StartsWith('docs/archive/feature-inputs/', [System.StringComparison]::OrdinalIgnoreCase) -or
        $p.StartsWith('docs/archive/feature-audits/', [System.StringComparison]::OrdinalIgnoreCase) -or
        $p -in @('docs/README.md', 'docs/PROJECT_MAP.md', 'docs/doc-inventory.md', 'docs/goal.md', 'docs/git-commit-push-policy.md', 'docs/worktree-cleanup-audit.md')) {
        return 2
    }

    if ($p -eq 'EZMicroBalanceCode/README.md' -or
        $p.StartsWith('docs/audits/', [System.StringComparison]::OrdinalIgnoreCase) -or
        $p -match '^docs/(介绍|/+344/+273/+213/+347/+273/+215)\.md$') {
        return 8
    }

    return -1
}

function Resolve-RepoOutputPath {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$Label
    )

    $outputFull = if ([System.IO.Path]::IsPathRooted($Path)) {
        [System.IO.Path]::GetFullPath($Path)
    } else {
        [System.IO.Path]::GetFullPath((Join-Path $repoRoot $Path))
    }

    $repoFull = [System.IO.Path]::GetFullPath($repoRoot).TrimEnd('\', '/')
    if (-not $outputFull.StartsWith($repoFull + [System.IO.Path]::DirectorySeparatorChar, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "$Label must stay inside the repository. Path: $outputFull"
    }

    return $outputFull
}

$statusLines = & git -C $repoRoot status --short
if ($LASTEXITCODE -ne 0) {
    throw 'git status --short failed.'
}

$rows = foreach ($line in $statusLines) {
    if ([string]::IsNullOrWhiteSpace($line)) {
        continue
    }

    $path = Get-StatusPath -Line $line
    $batch = Get-WorktreeBatch -Path $path
    [ordered]@{
        Status = $line.Substring(0, [System.Math]::Min(2, $line.Length)).Trim()
        Path = $path
        Batch = $batch
    }
}

$batchNames = [ordered]@{
    '0' = 'Local output hygiene'
    '1' = 'Status and release docs'
    '2' = 'Governance and architecture docs'
    '3' = 'Ancient source and tests'
    '4' = 'Ascension source and tests'
    '5' = 'Scripts, CI, and validation tests'
    '6' = 'Ancient art and waiting-test docs'
    '7' = 'Website public-info surface'
    '8' = 'Stray docs and audits'
    '-1' = 'Unclassified'
}

$summary = foreach ($key in $batchNames.Keys) {
    $batch = [int]$key
    $items = @($rows | Where-Object { $_.Batch -eq $batch })
    [ordered]@{
        Batch = $batch
        Name = $batchNames[$key]
        Count = $items.Count
        Paths = @($items | ForEach-Object { $_.Path })
    }
}

$report = [ordered]@{
    CreatedAt = (Get-Date).ToString('o')
    RepositoryRoot = $repoRoot
    Command = 'git status --short'
    TotalDirtyEntries = @($rows).Count
    Summary = $summary
}

$pathspecRecords = @()
if ($PathspecDirectory) {
    $pathspecFull = Resolve-RepoOutputPath -Path $PathspecDirectory -Label 'PathspecDirectory'
    if (-not (Test-Path -LiteralPath $pathspecFull)) {
        New-Item -ItemType Directory -Path $pathspecFull -Force | Out-Null
    }

    $utf8NoBom = [System.Text.UTF8Encoding]::new($false)
    foreach ($item in $summary) {
        if ($item.Batch -lt 0) {
            continue
        }

        $pathspecPath = Join-Path $pathspecFull ("batch-{0}.pathspec" -f $item.Batch)
        [System.IO.File]::WriteAllLines($pathspecPath, [string[]]@($item.Paths), $utf8NoBom)
        $pathspecRecords += [ordered]@{
            Batch = $item.Batch
            Name = $item.Name
            Count = $item.Count
            PathspecPath = $pathspecPath
            GitAddCommand = "git add --pathspec-from-file=""$pathspecPath"""
        }
    }

    $report.PathspecDirectory = $pathspecFull
    $report.SuggestedGitAddCommands = $pathspecRecords
    $report | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath (Join-Path $pathspecFull 'manifest.json') -Encoding UTF8
}

if ($Format -eq 'Json') {
    $content = $report | ConvertTo-Json -Depth 20
} else {
    $lines = @(
        '# Worktree Batch Report',
        '',
        "Generated: $($report.CreatedAt)",
        '',
        ('Command: `' + $report.Command + '`'),
        '',
        "Total dirty entries: $($report.TotalDirtyEntries)",
        '',
        '| Batch | Count | Name |',
        '| --- | ---: | --- |'
    )

    foreach ($item in $summary) {
        $lines += "| $($item.Batch) | $($item.Count) | $($item.Name) |"
    }

    foreach ($item in $summary) {
        if ($item.Count -eq 0) {
            continue
        }

        $lines += ''
        $lines += "## Batch $($item.Batch): $($item.Name)"
        $pathspec = @($pathspecRecords | Where-Object { $_.Batch -eq $item.Batch } | Select-Object -First 1)
        if ($pathspec.Count -gt 0) {
            $lines += ''
            $lines += ('Suggested staging command: `' + $pathspec[0].GitAddCommand + '`')
            $lines += ''
        }

        foreach ($path in $item.Paths) {
            $lines += ('- `' + $path + '`')
        }
    }

    $content = $lines -join [Environment]::NewLine
}

if ($OutputPath) {
    $outputFull = Resolve-RepoOutputPath -Path $OutputPath -Label 'OutputPath'
    $parent = Split-Path -Parent $outputFull
    if ($parent -and -not (Test-Path -LiteralPath $parent)) {
        New-Item -ItemType Directory -Path $parent -Force | Out-Null
    }

    $content | Set-Content -LiteralPath $outputFull -Encoding UTF8
} else {
    Write-Output $content
}

$unclassifiedCount = @($rows | Where-Object { $_.Batch -eq -1 }).Count
if ($FailOnUnclassified -and $unclassifiedCount -gt 0) {
    throw "Found $unclassifiedCount unclassified dirty worktree entries."
}
