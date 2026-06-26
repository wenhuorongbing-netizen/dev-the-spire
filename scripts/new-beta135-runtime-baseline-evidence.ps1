param(
    [string]$OutRoot = '.tools\runtime-evidence',
    [string]$EvidenceDir,
    [string]$GameRoot = '',
    [string]$PackageZipPath = '',
    [string]$PackageVersion = 'v0.1.0-private-beta.135',
    [string]$GameVersion = '0.107.1',
    [string]$RitsuLibVersion = '0.4.34',
    [string]$RitsuCompatBranch = '0.107.1',
    [int]$ExpectedRuntimePatchCount = 168,
    [string]$GodotLogAfterLaunchPath,
    [string]$ScreenshotPath,
    [switch]$Force
)

$ErrorActionPreference = 'Stop'
$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path

function Resolve-RepoRelativePath {
    param([Parameter(Mandatory = $true)][string]$Path)

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }

    return [System.IO.Path]::GetFullPath((Join-Path $repoRoot $Path))
}

function Normalize-FullPath {
    param([Parameter(Mandatory = $true)][string]$Path)

    [System.IO.Path]::GetFullPath($Path).TrimEnd([char[]]@('\', '/'))
}

function Get-Sts2PathFromDirectoryBuildProps {
    $propsPath = Join-Path $repoRoot 'Directory.Build.props'
    if (-not (Test-Path -LiteralPath $propsPath -PathType Leaf)) {
        return $null
    }

    try {
        [xml]$props = Get-Content -Raw -LiteralPath $propsPath -Encoding UTF8
        $node = @($props.Project.PropertyGroup | ForEach-Object { $_.Sts2Path } | Where-Object { -not [string]::IsNullOrWhiteSpace([string]$_) } | Select-Object -First 1)
        if ($node.Count -eq 0) {
            return $null
        }

        return [string]$node[0]
    } catch {
        return $null
    }
}

function Resolve-ConfiguredGameRoot {
    param([AllowEmptyString()][string]$Path)

    $candidate = $Path
    if ([string]::IsNullOrWhiteSpace($candidate)) {
        $candidate = Get-Sts2PathFromDirectoryBuildProps
    }

    if ([string]::IsNullOrWhiteSpace($candidate)) {
        throw 'Pass -GameRoot or set Directory.Build.props Sts2Path before preparing beta.135 runtime baseline evidence.'
    }

    return Normalize-FullPath -Path $candidate
}

function Resolve-ConfiguredPackageZipPath {
    param(
        [AllowEmptyString()][string]$Path,
        [Parameter(Mandatory = $true)][string]$PackageVersion
    )

    if (-not [string]::IsNullOrWhiteSpace($Path)) {
        return Resolve-RepoRelativePath -Path $Path
    }

    return [System.IO.Path]::GetFullPath((Join-Path $repoRoot "publish\SpirePlus-$PackageVersion.zip"))
}

function Get-GameRootAnchorMode {
    param([AllowEmptyString()][string]$ResolvedGameRoot)

    $configuredGameRoot = Get-Sts2PathFromDirectoryBuildProps
    if ([string]::IsNullOrWhiteSpace($configuredGameRoot)) {
        return 'noncanonical-override-test-only'
    }

    try {
        if ([System.StringComparer]::OrdinalIgnoreCase.Equals((Normalize-FullPath -Path $ResolvedGameRoot), (Normalize-FullPath -Path $configuredGameRoot))) {
            return 'configured-directory-build-props'
        }
    } catch {
    }

    return 'noncanonical-override-test-only'
}

function Get-PackageAnchorMode {
    param(
        [AllowEmptyString()][string]$ResolvedPackageZipPath,
        [Parameter(Mandatory = $true)][string]$PackageVersion
    )

    $canonicalPackageZipPath = Resolve-ConfiguredPackageZipPath -Path '' -PackageVersion $PackageVersion
    try {
        if ([System.StringComparer]::OrdinalIgnoreCase.Equals((Normalize-FullPath -Path $ResolvedPackageZipPath), (Normalize-FullPath -Path $canonicalPackageZipPath))) {
            return 'canonical-repo-publish'
        }
    } catch {
    }

    return 'noncanonical-override-test-only'
}

function Get-TrustAnchorMode {
    param(
        [Parameter(Mandatory = $true)][string]$GameRootAnchorMode,
        [Parameter(Mandatory = $true)][string]$PackageAnchorMode
    )

    if ($GameRootAnchorMode -eq 'noncanonical-override-test-only' -or $PackageAnchorMode -eq 'noncanonical-override-test-only') {
        return 'noncanonical-override-test-only'
    }

    return 'canonical-configured'
}

function Test-PathInsideDirectory {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$Directory
    )

    $normalizedPath = Normalize-FullPath -Path $Path
    $normalizedDirectory = Normalize-FullPath -Path $Directory
    if ([System.StringComparer]::OrdinalIgnoreCase.Equals($normalizedPath, $normalizedDirectory)) {
        return $true
    }

    $prefix = $normalizedDirectory + [System.IO.Path]::DirectorySeparatorChar
    return $normalizedPath.StartsWith($prefix, [System.StringComparison]::OrdinalIgnoreCase)
}

function Assert-BaselineEvidenceDir {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$Root
    )

    $normalizedPath = Normalize-FullPath -Path $Path
    $normalizedRoot = Normalize-FullPath -Path $Root
    if ([System.StringComparer]::OrdinalIgnoreCase.Equals($normalizedPath, $normalizedRoot)) {
        throw "EvidenceDir must be a child directory under $normalizedRoot, not the evidence root itself."
    }

    if (-not (Test-PathInsideDirectory -Path $normalizedPath -Directory $normalizedRoot)) {
        throw "EvidenceDir must stay under $normalizedRoot. Resolved path: $normalizedPath"
    }
}

function Assert-NoReparsePointInPath {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [string]$StopDirectory = $repoRoot
    )

    $current = Normalize-FullPath -Path $Path
    while (-not (Test-Path -LiteralPath $current) -and -not [string]::IsNullOrWhiteSpace($current)) {
        $parent = Split-Path -Parent $current
        if ([string]::IsNullOrWhiteSpace($parent) -or [System.StringComparer]::OrdinalIgnoreCase.Equals($parent, $current)) {
            break
        }

        $current = $parent
    }

    $normalizedStop = Normalize-FullPath -Path $StopDirectory
    if (-not (Test-PathInsideDirectory -Path $current -Directory $normalizedStop)) {
        throw "Evidence path must stay under repo root while checking reparse points. Path: $current RepoRoot: $normalizedStop"
    }

    while (-not [string]::IsNullOrWhiteSpace($current) -and (Test-PathInsideDirectory -Path $current -Directory $normalizedStop)) {
        $item = Get-Item -LiteralPath $current -Force
        if (($item.Attributes -band [System.IO.FileAttributes]::ReparsePoint) -ne 0) {
            throw "Evidence path must not pass through a reparse point: $current"
        }

        if ([System.StringComparer]::OrdinalIgnoreCase.Equals((Normalize-FullPath -Path $current), $normalizedStop)) {
            break
        }

        $current = Split-Path -Parent $current
    }
}

function Assert-NoReparsePointInExistingPath {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [string]$Description = 'Path'
    )

    $current = Normalize-FullPath -Path $Path
    while (-not [string]::IsNullOrWhiteSpace($current)) {
        if (Test-Path -LiteralPath $current) {
            $item = Get-Item -LiteralPath $current -Force
            if (($item.Attributes -band [System.IO.FileAttributes]::ReparsePoint) -ne 0) {
                throw "$Description must not pass through a reparse point: $current"
            }
        }

        $parent = Split-Path -Parent $current
        if ([string]::IsNullOrWhiteSpace($parent) -or [System.StringComparer]::OrdinalIgnoreCase.Equals($parent, $current)) {
            break
        }

        $current = $parent
    }
}

function Assert-LeafIsNotReparsePoint {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [string]$Description = 'File'
    )

    $item = Get-Item -LiteralPath $Path -Force
    if (($item.Attributes -band [System.IO.FileAttributes]::ReparsePoint) -ne 0) {
        throw "$Description must not be a reparse point: $Path"
    }
}

function Set-ObjectProperty {
    param(
        [Parameter(Mandatory = $true)]$Object,
        [Parameter(Mandatory = $true)][string]$Name,
        $Value
    )

    if ($Object.PSObject.Properties.Match($Name).Count -gt 0) {
        $Object.$Name = $Value
    } else {
        $Object | Add-Member -NotePropertyName $Name -NotePropertyValue $Value
    }
}

function Invoke-GitText {
    param([Parameter(Mandatory = $true)][string[]]$Arguments)

    try {
        $output = & git -C $repoRoot @Arguments 2>$null
        if ($LASTEXITCODE -ne 0) {
            return $null
        }

        return [string]($output -join "`n")
    } catch {
        return $null
    }
}

function Get-GitEvidence {
    $headSha = Invoke-GitText -Arguments @('rev-parse', 'HEAD')
    $headDecorated = Invoke-GitText -Arguments @('log', '-1', '--oneline', '--decorate')
    $branch = Invoke-GitText -Arguments @('branch', '--show-current')
    $upstream = Invoke-GitText -Arguments @('rev-parse', '--abbrev-ref', '--symbolic-full-name', '@{u}')
    $statusText = Invoke-GitText -Arguments @('status', '--short', '--branch')
    $statusLines = if ($null -eq $statusText) {
        @()
    } else {
        @($statusText -split "`r?`n" | Where-Object { $_ -ne '' })
    }
    $dirtyLines = @($statusLines | Where-Object { -not $_.StartsWith('## ', [System.StringComparison]::Ordinal) })
    $available = -not [string]::IsNullOrWhiteSpace($headSha) -and -not [string]::IsNullOrWhiteSpace($statusText)

    [pscustomobject]@{
        Available = $available
        RepoRoot = $repoRoot
        HeadSha = if ($null -eq $headSha) { '' } else { $headSha.Trim() }
        HeadDecorated = if ($null -eq $headDecorated) { '' } else { $headDecorated.Trim() }
        Branch = if ($null -eq $branch) { '' } else { $branch.Trim() }
        Upstream = if ($null -eq $upstream) { '' } else { $upstream.Trim() }
        StatusShortBranch = @($statusLines)
        Dirty = $dirtyLines.Count -gt 0
    }
}

function Get-FileRecord {
    param([Parameter(Mandatory = $true)][string]$Path)

    $resolved = if ([System.IO.Path]::IsPathRooted($Path)) {
        [System.IO.Path]::GetFullPath($Path)
    } else {
        [System.IO.Path]::GetFullPath((Join-Path $repoRoot $Path))
    }

    if (-not (Test-Path -LiteralPath $resolved -PathType Leaf)) {
        return [pscustomobject]@{
            Path = $resolved
            Exists = $false
            Length = $null
            Sha256 = $null
        }
    }

    $item = Get-Item -LiteralPath $resolved
    [pscustomobject]@{
        Path = $item.FullName
        Exists = $true
        Length = $item.Length
        Sha256 = (Get-FileHash -Algorithm SHA256 -LiteralPath $item.FullName).Hash.ToLowerInvariant()
    }
}

function Get-DirectoryFileRecords {
    param([Parameter(Mandatory = $true)][string]$Path)

    $resolved = if ([System.IO.Path]::IsPathRooted($Path)) {
        [System.IO.Path]::GetFullPath($Path)
    } else {
        [System.IO.Path]::GetFullPath((Join-Path $repoRoot $Path))
    }

    if (-not (Test-Path -LiteralPath $resolved -PathType Container)) {
        return @()
    }

    $basePath = $resolved.TrimEnd([char[]]@('\', '/')) + [System.IO.Path]::DirectorySeparatorChar
    return @(Get-ChildItem -LiteralPath $resolved -File -Recurse | ForEach-Object {
        $relativePath = if ($_.FullName.StartsWith($basePath, [System.StringComparison]::OrdinalIgnoreCase)) {
            $_.FullName.Substring($basePath.Length)
        } else {
            $_.Name
        }

        [pscustomobject]@{
            Path = $_.FullName
            RelativePath = $relativePath
            Length = $_.Length
            Sha256 = (Get-FileHash -Algorithm SHA256 -LiteralPath $_.FullName).Hash.ToLowerInvariant()
        }
    })
}

function Copy-OptionalArtifact {
    param(
        [AllowEmptyString()][string]$Source,
        [Parameter(Mandatory = $true)][string]$Destination
    )

    if ([string]::IsNullOrWhiteSpace($Source)) {
        return $false
    }

    $resolved = Resolve-RepoRelativePath -Path $Source
    if (-not (Test-Path -LiteralPath $resolved -PathType Leaf)) {
        throw "Artifact not found: $resolved"
    }
    Assert-NoReparsePointInExistingPath -Path $resolved -Description 'Artifact source path'
    Assert-LeafIsNotReparsePoint -Path $resolved -Description 'Artifact source'

    $resolvedDestination = Resolve-RepoRelativePath -Path $Destination
    Assert-NoReparsePointInPath -Path $resolvedDestination
    if (-not [System.StringComparer]::OrdinalIgnoreCase.Equals($resolved, $resolvedDestination)) {
        Copy-Item -LiteralPath $resolved -Destination $resolvedDestination -Force
    }
    Assert-LeafIsNotReparsePoint -Path $resolvedDestination -Description 'Artifact destination'

    return $true
}

$timestamp = [System.DateTimeOffset]::Now.ToString('yyyyMMdd-HHmmss')
$canonicalEvidenceRoot = Resolve-RepoRelativePath -Path '.tools\runtime-evidence'
$resolvedOutRoot = Resolve-RepoRelativePath -Path $OutRoot
if (-not (Test-PathInsideDirectory -Path $resolvedOutRoot -Directory $canonicalEvidenceRoot)) {
    throw "OutRoot must stay under $canonicalEvidenceRoot. Resolved path: $resolvedOutRoot"
}
Assert-NoReparsePointInPath -Path $resolvedOutRoot

$resolvedEvidenceDir = if ([string]::IsNullOrWhiteSpace($EvidenceDir)) {
    Join-Path $resolvedOutRoot "beta135-runtime-baseline-$timestamp"
} else {
    Resolve-RepoRelativePath -Path $EvidenceDir
}
Assert-BaselineEvidenceDir -Path $resolvedEvidenceDir -Root $canonicalEvidenceRoot
Assert-NoReparsePointInPath -Path $resolvedEvidenceDir

if (Test-Path -LiteralPath $resolvedEvidenceDir) {
    if (-not $Force) {
        throw "Evidence directory already exists: $resolvedEvidenceDir. Pass -Force only for an empty directory."
    }

    $existingEntry = @(Get-ChildItem -LiteralPath $resolvedEvidenceDir -Force -Recurse | Select-Object -First 1)
    if ($existingEntry.Count -gt 0) {
        throw "Refusing to reuse non-empty evidence directory: $resolvedEvidenceDir"
    }
}

New-Item -ItemType Directory -Path $resolvedEvidenceDir -Force | Out-Null

$ownerLogDestination = Join-Path $resolvedEvidenceDir 'godot.log.after-launch'
$ownerScreenshotDestination = Join-Path $resolvedEvidenceDir 'main-menu-screenshot.png'
$logProvided = Copy-OptionalArtifact -Source $GodotLogAfterLaunchPath -Destination $ownerLogDestination
$screenshotProvided = Copy-OptionalArtifact -Source $ScreenshotPath -Destination $ownerScreenshotDestination
$auditOutputPath = Join-Path $resolvedEvidenceDir 'godot-log-audit.json'
$logCheckReportPath = Join-Path $resolvedEvidenceDir 'runtime-baseline-log-check.json'

$resolvedGameRoot = Resolve-ConfiguredGameRoot -Path $GameRoot
$modsRoot = Join-Path $resolvedGameRoot 'mods'
$spirePlusInstallDir = Join-Path $modsRoot 'EZMicroBalance'
$ritsuInstallDir = Join-Path $modsRoot 'STS2-RitsuLib'
$zipPath = Resolve-ConfiguredPackageZipPath -Path $PackageZipPath -PackageVersion $PackageVersion
$gameRootAnchorMode = Get-GameRootAnchorMode -ResolvedGameRoot $resolvedGameRoot
$packageAnchorMode = Get-PackageAnchorMode -ResolvedPackageZipPath $zipPath -PackageVersion $PackageVersion
$trustAnchorMode = Get-TrustAnchorMode -GameRootAnchorMode $gameRootAnchorMode -PackageAnchorMode $packageAnchorMode
$gitEvidence = Get-GitEvidence
$pendingOwnerRunArtifacts = @(
    'godot.log.after-launch',
    'godot-log-audit.json',
    'runtime-baseline-log-check.json',
    'main-menu-screenshot.png'
) | Where-Object {
    ($_ -ne 'godot.log.after-launch' -or -not $logProvided) -and
        ($_ -ne 'main-menu-screenshot.png' -or -not $screenshotProvided)
}

$preflight = [pscustomobject]@{
    SchemaVersion = 1
    Status = if ($logProvided) { 'owner-log-provided' } else { 'pending-owner-run' }
    GeneratedAtUtc = [System.DateTimeOffset]::UtcNow.ToString('o')
    DoesNotLaunchGame = $true
    PackageVersion = $PackageVersion
    ExpectedGameVersion = $GameVersion
    ExpectedRitsuLibVersion = $RitsuLibVersion
    ExpectedRitsuCompatBranch = $RitsuCompatBranch
    ExpectedRuntimePatchCount = $ExpectedRuntimePatchCount
    ExpectedModManifestIds = @('EZMicroBalance', 'STS2-RitsuLib')
    RepoRoot = $repoRoot
    Git = $gitEvidence
    GameRoot = $resolvedGameRoot
    ModsRoot = $modsRoot
    SpirePlusInstallDir = $spirePlusInstallDir
    RitsuLibInstallDir = $ritsuInstallDir
    PackageZip = Get-FileRecord -Path $zipPath
    GameRootAnchorMode = $gameRootAnchorMode
    PackageAnchorMode = $packageAnchorMode
    TrustAnchorMode = $trustAnchorMode
    SpirePlusFiles = Get-DirectoryFileRecords -Path $spirePlusInstallDir
    RitsuLibFiles = Get-DirectoryFileRecords -Path $ritsuInstallDir
    PendingOwnerRunArtifacts = @($pendingOwnerRunArtifacts)
}

$commandText = @"
# beta.135 runtime baseline owner-run command
# This command prepares/updates the evidence scaffold only; it does not launch the game.
.\scripts\new-beta135-runtime-baseline-evidence.ps1 -EvidenceDir "$resolvedEvidenceDir" -GameRoot "$resolvedGameRoot" -PackageZipPath "$zipPath"

# Owner-run boundary:
# 1. Start Slay the Spire 2 through Steam with only STS2-RitsuLib and Spire Plus/EZMicroBalance enabled.
# 2. Wait until the main menu is visible.
# 3. Copy the full post-launch godot log into this evidence directory as:
#    $resolvedEvidenceDir\godot.log.after-launch
# 4. If a screenshot tool is stable, save the main-menu screenshot as:
#    $resolvedEvidenceDir\main-menu-screenshot.png
# 5. Run the log checker without launching the game. The checker updates run-manifest.json with log/audit/report hashes:
.\scripts\check-beta135-runtime-baseline-log.ps1 -EvidenceDir "$resolvedEvidenceDir" -LogPath "$resolvedEvidenceDir\godot.log.after-launch" -ExpectedPackageVersion $PackageVersion -ExpectedGameVersion $GameVersion -ExpectedRitsuLibVersion $RitsuLibVersion -ExpectedRitsuCompatBranch $RitsuCompatBranch -ExpectedPatchCount $ExpectedRuntimePatchCount -GameRoot "$resolvedGameRoot" -PackageZipPath "$zipPath" -FailOnMismatch
"@

$notesText = @"
# beta.135 Runtime Baseline Notes

Status: pending-owner-run

This evidence directory is a scaffold for the first beta.135 runtime baseline.
It does not prove startup, loader, main menu, gameplay, save-load, co-op, or
release readiness until the owner supplies a real `godot.log.after-launch`
captured from a Steam-launched Slay the Spire 2 session.

Expected target:

- Spire Plus package: $PackageVersion
- Slay the Spire 2: $GameVersion
- STS2-RitsuLib: $RitsuLibVersion
- Ritsu compatibility branch: $RitsuCompatBranch
- Expected runtime patch count: $ExpectedRuntimePatchCount
- Expected manifest ids found in the runtime log: EZMicroBalance, STS2-RitsuLib
- Source binding: Git HEAD $($gitEvidence.HeadSha), dirty worktree $($gitEvidence.Dirty)
- Source inventory note: beta.135 has 169 migrated RitsuLib patch classes, but
  the default package compiles/applies 168 runtime patch classes because the
  debug-only StS1 replacement prototype is compile-symbol gated.

Required owner-run artifacts:

- `godot.log.after-launch`
- `godot-log-audit.json`, generated by `scripts/audit-godot-log.ps1`
- `runtime-baseline-log-check.json`, generated by `scripts/check-beta135-runtime-baseline-log.ps1`
- `main-menu-screenshot.png`, if screenshot capture is stable

Hard no-claim boundary:

- Do not claim release-ready.
- Do not claim live gameplay ready.
- Do not claim save-load ready.
- Do not claim co-op ready.
- Do not treat historical beta.84, beta.128, or older package evidence as beta.135 runtime proof.
"@

$preflightPath = Join-Path $resolvedEvidenceDir 'preflight.json'
$manifestPath = Join-Path $resolvedEvidenceDir 'run-manifest.json'
$commandPath = Join-Path $resolvedEvidenceDir 'command.txt'
$notesPath = Join-Path $resolvedEvidenceDir 'runtime-baseline-notes.md'

$preflight | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $preflightPath -Encoding UTF8
$commandText | Set-Content -LiteralPath $commandPath -Encoding UTF8
$notesText | Set-Content -LiteralPath $notesPath -Encoding UTF8

$initialManifest = [pscustomobject]@{}
Set-ObjectProperty -Object $initialManifest -Name 'SchemaVersion' -Value 1
$initialStatus = if ($logProvided) { 'owner-log-provided' } else { 'pending-owner-run' }
Set-ObjectProperty -Object $initialManifest -Name 'Status' -Value $initialStatus
Set-ObjectProperty -Object $initialManifest -Name 'CreatedAtUtc' -Value ([System.DateTimeOffset]::UtcNow.ToString('o'))
Set-ObjectProperty -Object $initialManifest -Name 'EvidenceDir' -Value $resolvedEvidenceDir
Set-ObjectProperty -Object $initialManifest -Name 'CommandFile' -Value (Join-Path $resolvedEvidenceDir 'command.txt')
Set-ObjectProperty -Object $initialManifest -Name 'PreflightFile' -Value (Join-Path $resolvedEvidenceDir 'preflight.json')
Set-ObjectProperty -Object $initialManifest -Name 'NotesFile' -Value (Join-Path $resolvedEvidenceDir 'runtime-baseline-notes.md')
Set-ObjectProperty -Object $initialManifest -Name 'OwnerRunRequired' -Value $true
Set-ObjectProperty -Object $initialManifest -Name 'DoesNotLaunchGame' -Value $true
Set-ObjectProperty -Object $initialManifest -Name 'LaunchMethod' -Value 'owner-steam-launch-required'
Set-ObjectProperty -Object $initialManifest -Name 'PackageVersion' -Value $PackageVersion
Set-ObjectProperty -Object $initialManifest -Name 'GameVersion' -Value $GameVersion
Set-ObjectProperty -Object $initialManifest -Name 'RitsuLibVersion' -Value $RitsuLibVersion
Set-ObjectProperty -Object $initialManifest -Name 'RitsuCompatBranch' -Value $RitsuCompatBranch
Set-ObjectProperty -Object $initialManifest -Name 'ExpectedRuntimePatchCount' -Value $ExpectedRuntimePatchCount
Set-ObjectProperty -Object $initialManifest -Name 'ExpectedModManifestIds' -Value @('EZMicroBalance', 'STS2-RitsuLib')
Set-ObjectProperty -Object $initialManifest -Name 'Git' -Value $gitEvidence
Set-ObjectProperty -Object $initialManifest -Name 'GameRoot' -Value $resolvedGameRoot
Set-ObjectProperty -Object $initialManifest -Name 'ModsRoot' -Value $modsRoot
Set-ObjectProperty -Object $initialManifest -Name 'SpirePlusInstallDir' -Value $spirePlusInstallDir
Set-ObjectProperty -Object $initialManifest -Name 'RitsuLibInstallDir' -Value $ritsuInstallDir
Set-ObjectProperty -Object $initialManifest -Name 'PackageZipPath' -Value $zipPath
Set-ObjectProperty -Object $initialManifest -Name 'PackageZip' -Value (Get-FileRecord -Path $zipPath)
Set-ObjectProperty -Object $initialManifest -Name 'GameRootAnchorMode' -Value $gameRootAnchorMode
Set-ObjectProperty -Object $initialManifest -Name 'PackageAnchorMode' -Value $packageAnchorMode
Set-ObjectProperty -Object $initialManifest -Name 'TrustAnchorMode' -Value $trustAnchorMode
Set-ObjectProperty -Object $initialManifest -Name 'SpirePlusFiles' -Value (Get-DirectoryFileRecords -Path $spirePlusInstallDir)
Set-ObjectProperty -Object $initialManifest -Name 'RitsuLibFiles' -Value (Get-DirectoryFileRecords -Path $ritsuInstallDir)
Set-ObjectProperty -Object $initialManifest -Name 'RuntimePatchCountBoundary' -Value '169 migrated source patch classes; 168 default runtime patch classes because Sts1ReplacementPrototype is compile-symbol gated.'
Set-ObjectProperty -Object $initialManifest -Name 'ForbiddenClaims' -Value @('release-ready', 'live-gameplay-ready', 'save-load-ready', 'co-op-ready')
$initialManifest | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $manifestPath -Encoding UTF8

if ($logProvided) {
    & (Join-Path $PSScriptRoot 'check-beta135-runtime-baseline-log.ps1') `
        -EvidenceDir $resolvedEvidenceDir `
        -LogPath $ownerLogDestination `
        -ExpectedPackageVersion $PackageVersion `
        -ExpectedGameVersion $GameVersion `
        -ExpectedRitsuLibVersion $RitsuLibVersion `
        -ExpectedRitsuCompatBranch $RitsuCompatBranch `
        -ExpectedPatchCount $ExpectedRuntimePatchCount `
        -GameRoot $resolvedGameRoot `
        -PackageZipPath $zipPath `
        -FailOnMismatch
}

$manifest = if (Test-Path -LiteralPath $manifestPath -PathType Leaf) {
    try {
        Get-Content -LiteralPath $manifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
    } catch {
        [pscustomobject]@{}
    }
} else {
    [pscustomobject]@{}
}

Set-ObjectProperty -Object $manifest -Name 'SchemaVersion' -Value 1
$preserveCheckerValidatedAnchors = $false
if ($logProvided) {
    $checkerStatus = if ($manifest.PSObject.Properties.Match('Status').Count -gt 0) { [string]$manifest.Status } else { '' }
    if ($checkerStatus -ne 'runtime-baseline-log-markers-checked') {
        throw "Checker did not record successful runtime baseline status in run-manifest.json. Status: $checkerStatus"
    }

    Set-ObjectProperty -Object $manifest -Name 'Status' -Value $checkerStatus
    $preserveCheckerValidatedAnchors = $true
} else {
    Set-ObjectProperty -Object $manifest -Name 'Status' -Value 'pending-owner-run'
}
Set-ObjectProperty -Object $manifest -Name 'CreatedAtUtc' -Value ([System.DateTimeOffset]::UtcNow.ToString('o'))
Set-ObjectProperty -Object $manifest -Name 'EvidenceDir' -Value $resolvedEvidenceDir
Set-ObjectProperty -Object $manifest -Name 'CommandFile' -Value (Join-Path $resolvedEvidenceDir 'command.txt')
Set-ObjectProperty -Object $manifest -Name 'PreflightFile' -Value (Join-Path $resolvedEvidenceDir 'preflight.json')
Set-ObjectProperty -Object $manifest -Name 'NotesFile' -Value (Join-Path $resolvedEvidenceDir 'runtime-baseline-notes.md')
$godotLogAfterLaunchValue = if ($logProvided) { $ownerLogDestination } else { $null }
$mainMenuScreenshotValue = if ($screenshotProvided) { $ownerScreenshotDestination } else { $null }
$startupMainMenuScreenshotStatus = if ($screenshotProvided) { 'provided' } else { 'pending-owner-run' }
$runtimeLogStatus = if ($logProvided) { 'provided' } else { 'pending-owner-run' }
$logOriginProofStatus = if ($logProvided) { 'marker-only-origin-not-proven-by-offline-checker' } else { 'pending-owner-run' }
Set-ObjectProperty -Object $manifest -Name 'GodotLogAfterLaunch' -Value $godotLogAfterLaunchValue
Set-ObjectProperty -Object $manifest -Name 'GodotLogAfterLaunchRecord' -Value (Get-FileRecord -Path $ownerLogDestination)
Set-ObjectProperty -Object $manifest -Name 'MainMenuScreenshot' -Value $mainMenuScreenshotValue
Set-ObjectProperty -Object $manifest -Name 'MainMenuScreenshotRecord' -Value (Get-FileRecord -Path $ownerScreenshotDestination)
Set-ObjectProperty -Object $manifest -Name 'GodotLogAuditRecord' -Value (Get-FileRecord -Path $auditOutputPath)
Set-ObjectProperty -Object $manifest -Name 'RuntimeBaselineLogCheckRecord' -Value (Get-FileRecord -Path $logCheckReportPath)
Set-ObjectProperty -Object $manifest -Name 'StartupMainMenuScreenshotStatus' -Value $startupMainMenuScreenshotStatus
Set-ObjectProperty -Object $manifest -Name 'RuntimeLogStatus' -Value $runtimeLogStatus
$baselineLogCheckStatus = if ($manifest.PSObject.Properties.Match('BaselineLogCheckStatus').Count -gt 0) { [string]$manifest.BaselineLogCheckStatus } else { '' }
$runtimeAuditStatus = if ($logProvided -and $baselineLogCheckStatus -eq 'pass') { 'checked-clean' } elseif ($logProvided) { 'checked' } else { 'pending-log-check' }
Set-ObjectProperty -Object $manifest -Name 'RuntimeAuditStatus' -Value $runtimeAuditStatus
Set-ObjectProperty -Object $manifest -Name 'BuildStatus' -Value 'pending-local-command'
Set-ObjectProperty -Object $manifest -Name 'OwnerRunRequired' -Value $true
Set-ObjectProperty -Object $manifest -Name 'DoesNotLaunchGame' -Value $true
Set-ObjectProperty -Object $manifest -Name 'LaunchMethod' -Value 'owner-steam-launch-required'
Set-ObjectProperty -Object $manifest -Name 'PackageVersion' -Value $PackageVersion
Set-ObjectProperty -Object $manifest -Name 'GameVersion' -Value $GameVersion
Set-ObjectProperty -Object $manifest -Name 'RitsuLibVersion' -Value $RitsuLibVersion
Set-ObjectProperty -Object $manifest -Name 'RitsuCompatBranch' -Value $RitsuCompatBranch
Set-ObjectProperty -Object $manifest -Name 'ExpectedRuntimePatchCount' -Value $ExpectedRuntimePatchCount
Set-ObjectProperty -Object $manifest -Name 'ExpectedModManifestIds' -Value @('EZMicroBalance', 'STS2-RitsuLib')
Set-ObjectProperty -Object $manifest -Name 'GameRootAnchorMode' -Value $gameRootAnchorMode
Set-ObjectProperty -Object $manifest -Name 'PackageAnchorMode' -Value $packageAnchorMode
Set-ObjectProperty -Object $manifest -Name 'TrustAnchorMode' -Value $trustAnchorMode
if (-not $preserveCheckerValidatedAnchors) {
    Set-ObjectProperty -Object $manifest -Name 'Git' -Value $gitEvidence
    Set-ObjectProperty -Object $manifest -Name 'GameRoot' -Value $resolvedGameRoot
    Set-ObjectProperty -Object $manifest -Name 'ModsRoot' -Value $modsRoot
    Set-ObjectProperty -Object $manifest -Name 'SpirePlusInstallDir' -Value $spirePlusInstallDir
    Set-ObjectProperty -Object $manifest -Name 'RitsuLibInstallDir' -Value $ritsuInstallDir
    Set-ObjectProperty -Object $manifest -Name 'PackageZipPath' -Value $zipPath
    Set-ObjectProperty -Object $manifest -Name 'PackageZip' -Value (Get-FileRecord -Path $zipPath)
    Set-ObjectProperty -Object $manifest -Name 'SpirePlusFiles' -Value (Get-DirectoryFileRecords -Path $spirePlusInstallDir)
    Set-ObjectProperty -Object $manifest -Name 'RitsuLibFiles' -Value (Get-DirectoryFileRecords -Path $ritsuInstallDir)
} else {
    # Do not refresh scaffold trust anchors after checker success; the checker validated these manifest records.
}
Set-ObjectProperty -Object $manifest -Name 'RuntimePatchCountBoundary' -Value '169 migrated source patch classes; 168 default runtime patch classes because Sts1ReplacementPrototype is compile-symbol gated.'
Set-ObjectProperty -Object $manifest -Name 'LogOriginProofStatus' -Value $logOriginProofStatus
Set-ObjectProperty -Object $manifest -Name 'ForbiddenClaims' -Value @('release-ready', 'live-gameplay-ready', 'save-load-ready', 'co-op-ready')

$manifest | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $manifestPath -Encoding UTF8

Write-Output "runtime_baseline_scaffold status=$($manifest.Status) evidence_dir=$resolvedEvidenceDir"
