param(
    [string]$PackageId = 'STS2.RitsuLib',

    [string]$RuntimeModId = 'STS2-RitsuLib',

    [string]$ProjectPath = 'EZMicroBalance.csproj',

    [string]$ManifestPath = 'EZMicroBalance.json',

    [string]$NuGetIndexUrl,

    [string]$ExpectedLatestVersion,

    [string]$OutFile,

    [switch]$FailOnMismatch
)

# Networked governance check for keeping the compile package and runtime
# manifest minimum aligned with the latest public STS2.RitsuLib package line.
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

function Normalize-Version {
    param([AllowEmptyString()][string]$Version)

    if ([string]::IsNullOrWhiteSpace($Version)) {
        return ''
    }

    $normalized = $Version.Trim()
    if ($normalized.StartsWith('v', [System.StringComparison]::OrdinalIgnoreCase)) {
        return $normalized.Substring(1)
    }

    return $normalized
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

function Read-JsonFile {
    param([Parameter(Mandatory = $true)][string]$Path)

    try {
        return Get-Content -Raw -LiteralPath $Path -Encoding UTF8 | ConvertFrom-Json
    } catch {
        Add-Check -Name 'manifest_json_valid' -Passed $false -Detail "invalid JSON in $Path`: $($_.Exception.Message)"
        return $null
    }
}

$projectFullPath = Resolve-RepoPath -Path $ProjectPath
$manifestFullPath = Resolve-RepoPath -Path $ManifestPath
if (-not $NuGetIndexUrl) {
    $packageKey = $PackageId.ToLowerInvariant()
    $NuGetIndexUrl = "https://api.nuget.org/v3-flatcontainer/$packageKey/index.json"
}

Add-Check -Name 'project_file_exists' -Passed (Test-Path -LiteralPath $projectFullPath -PathType Leaf) -Detail "expected project file at $projectFullPath"
Add-Check -Name 'manifest_file_exists' -Passed (Test-Path -LiteralPath $manifestFullPath -PathType Leaf) -Detail "expected manifest at $manifestFullPath"

$nugetVersions = @()
try {
    $nugetIndex = Invoke-RestMethod -Uri $NuGetIndexUrl -UseBasicParsing
    $nugetVersions = @($nugetIndex.versions)
    Add-Check -Name 'nuget_flat_container_index_fetch' -Passed ($nugetVersions.Count -gt 0) -Detail "fetched $($nugetVersions.Count) versions from $NuGetIndexUrl"
} catch {
    Add-Check -Name 'nuget_flat_container_index_fetch' -Passed $false -Detail "failed to fetch $NuGetIndexUrl`: $($_.Exception.Message)"
}

$latestNuGetVersion = if ($nugetVersions.Count -gt 0) { [string]$nugetVersions[-1] } else { '' }
$projectPackageVersion = ''
$projectPackageReferenceCount = 0
if (Test-Path -LiteralPath $projectFullPath -PathType Leaf) {
    [xml]$projectXml = Get-Content -Raw -LiteralPath $projectFullPath -Encoding UTF8
    $projectReferences = @($projectXml.SelectNodes('//PackageReference') | Where-Object {
        [string]$_.Include -eq $PackageId
    })
    $projectPackageReferenceCount = $projectReferences.Count
    if ($projectPackageReferenceCount -eq 1) {
        $projectPackageVersion = [string]$projectReferences[0].Version
    }

    Add-Check -Name 'project_has_single_ritsulib_package_reference' -Passed ($projectPackageReferenceCount -eq 1) -Detail "$PackageId PackageReference count=$projectPackageReferenceCount"
}

$manifestDependencyVersion = ''
$manifestDependencyCount = 0
$manifest = if (Test-Path -LiteralPath $manifestFullPath -PathType Leaf) {
    Read-JsonFile -Path $manifestFullPath
} else {
    $null
}

if ($manifest) {
    $manifestDependencies = @($manifest.dependencies)
    $runtimeDependencies = @($manifestDependencies | Where-Object {
        [string]$_.id -eq $RuntimeModId
    })
    $manifestDependencyCount = $runtimeDependencies.Count
    if ($manifestDependencyCount -eq 1) {
        $manifestDependencyVersion = if ($runtimeDependencies[0].min_version) {
            [string]$runtimeDependencies[0].min_version
        } elseif ($runtimeDependencies[0].version) {
            [string]$runtimeDependencies[0].version
        } else {
            ''
        }
    }

    Add-Check -Name 'manifest_has_single_ritsulib_runtime_dependency' -Passed ($manifestDependencyCount -eq 1) -Detail "$RuntimeModId dependency count=$manifestDependencyCount"
}

$normalizedLatest = Normalize-Version -Version $latestNuGetVersion
$normalizedProject = Normalize-Version -Version $projectPackageVersion
$normalizedManifest = Normalize-Version -Version $manifestDependencyVersion
$normalizedExpected = Normalize-Version -Version $ExpectedLatestVersion

if ($ExpectedLatestVersion) {
    Add-Check -Name 'nuget_latest_matches_expected' -Passed ($normalizedLatest -eq $normalizedExpected) -Detail "latest=$latestNuGetVersion expected=$ExpectedLatestVersion"
}

Add-Check -Name 'project_package_matches_nuget_latest' -Passed ($normalizedProject -and $normalizedProject -eq $normalizedLatest) -Detail "project=$projectPackageVersion latest=$latestNuGetVersion"
Add-Check -Name 'manifest_dependency_matches_project_package' -Passed ($normalizedManifest -and $normalizedManifest -eq $normalizedProject) -Detail "manifest=$manifestDependencyVersion project=$projectPackageVersion"
Add-Check -Name 'manifest_dependency_matches_nuget_latest' -Passed ($normalizedManifest -and $normalizedManifest -eq $normalizedLatest) -Detail "manifest=$manifestDependencyVersion latest=$latestNuGetVersion"

$report = [pscustomobject]@{
    SchemaVersion = 1
    CreatedAt = (Get-Date).ToString('o')
    Passed = $mismatches.Count -eq 0
    PackageId = $PackageId
    RuntimeModId = $RuntimeModId
    NuGetIndexUrl = $NuGetIndexUrl
    NuGetVersionCount = $nugetVersions.Count
    LatestNuGetVersion = $latestNuGetVersion
    ProjectPath = $projectFullPath
    ProjectPackageReferenceCount = $projectPackageReferenceCount
    ProjectPackageVersion = $projectPackageVersion
    ManifestPath = $manifestFullPath
    ManifestDependencyCount = $manifestDependencyCount
    ManifestDependencyVersion = $manifestDependencyVersion
    Checks = $checks
    Mismatches = $mismatches
}

foreach ($check in $checks) {
    $status = if ($check.Passed) { 'pass' } else { 'fail' }
    Write-Output "$($check.Name) status=$status"
}

Write-Output "latest_nuget_version=$latestNuGetVersion"
Write-Output "project_package_version=$projectPackageVersion"
Write-Output "manifest_dependency_version=$manifestDependencyVersion"
Write-Output "checks=$($checks.Count)"
Write-Output "mismatches=$($mismatches.Count)"

foreach ($mismatch in $mismatches) {
    Write-Output "mismatch $mismatch"
}

if ($OutFile) {
    $resolvedOutFile = Resolve-RepoPath -Path $OutFile
    $outDir = [System.IO.Path]::GetDirectoryName($resolvedOutFile)
    if ($outDir -and -not (Test-Path -LiteralPath $outDir)) {
        [void][System.IO.Directory]::CreateDirectory($outDir)
    }

    $report | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $resolvedOutFile -Encoding UTF8
}

if ($FailOnMismatch -and $mismatches.Count -gt 0) {
    exit 1
}
