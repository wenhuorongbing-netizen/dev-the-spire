param(
    [string]$RegistrationServicePath = 'EZMicroBalanceCode\Sts1Events\Runtime\Sts1EventRegistrationService.cs',
    [string]$FeatureGatePath = 'EZMicroBalanceCode\Sts1Events\Runtime\Sts1EventFeatureGate.cs',
    [string]$RegistryPath = 'EZMicroBalanceCode\Sts1Events\Runtime\Sts1EventRegistry.cs',
    [string]$ModelsRoot = 'EZMicroBalanceCode\Sts1Events\Models',
    [string]$ProjectPath = 'EZMicroBalance.csproj',
    [string]$CanonicalMatrixPath = 'docs\features\sts1-events\canonical-event-matrix.csv',
    [string]$OutFile,
    [switch]$FailOnMismatch
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
$metrics = [System.Collections.Generic.List[object]]::new()
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

function Read-PartialTypeText {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$TypeName
    )

    $resolved = Resolve-RepoPath $Path
    if (-not (Test-Path -LiteralPath $resolved -PathType Leaf)) {
        Write-Error "Partial type file not found: $resolved"
        exit 1
    }

    if ([System.IO.Path]::GetFileNameWithoutExtension($resolved) -ne $TypeName) {
        return [System.IO.File]::ReadAllText($resolved)
    }

    $directory = [System.IO.Path]::GetDirectoryName($resolved)
    $files = @(Get-ChildItem -LiteralPath $directory -Filter "$TypeName*.cs" -File | Sort-Object FullName)
    if ($files.Count -eq 0) {
        Write-Error "No $TypeName partial files found under: $directory"
        exit 1
    }

    return ($files | ForEach-Object { [System.IO.File]::ReadAllText($_.FullName) }) -join [System.Environment]::NewLine
}

function Read-RegistrationServiceText {
    param([Parameter(Mandatory = $true)][string]$Path)

    return Read-PartialTypeText -Path $Path -TypeName 'Sts1EventRegistrationService'
}

function Get-MethodSlice {
    param(
        [Parameter(Mandatory = $true)][string]$Text,
        [Parameter(Mandatory = $true)][string]$MethodName
    )

    $startToken = "public static void $MethodName"
    $start = $Text.IndexOf($startToken, [System.StringComparison]::Ordinal)
    if ($start -lt 0) {
        Write-Error "Method start not found: $MethodName"
        exit 1
    }

    $end = $Text.IndexOf('content.Apply();', $start, [System.StringComparison]::Ordinal)
    if ($end -lt 0) {
        Write-Error "content.Apply marker not found for method: $MethodName"
        exit 1
    }

    return $Text.Substring($start, $end - $start)
}

function Get-Registrations {
    param(
        [Parameter(Mandatory = $true)][string]$Block,
        [Parameter(Mandatory = $true)][string]$MethodName
    )

    $items = [System.Collections.Generic.List[object]]::new()

    foreach ($match in [regex]::Matches($Block, 'content\.ActEvent<\s*([A-Za-z0-9_]+)\s*,\s*([A-Za-z0-9_]+)\s*>\s*\(')) {
        $items.Add([pscustomobject]@{
            Method = $MethodName
            Kind = 'ActEvent'
            Act = $match.Groups[1].Value
            Event = $match.Groups[2].Value
        }) | Out-Null
    }

    foreach ($match in [regex]::Matches($Block, 'content\.SharedEvent<\s*([A-Za-z0-9_]+)\s*>\s*\(')) {
        $items.Add([pscustomobject]@{
            Method = $MethodName
            Kind = 'SharedEvent'
            Act = 'Shared'
            Event = $match.Groups[1].Value
        }) | Out-Null
    }

    return @($items)
}

function Convert-ClassNameToEventId {
    param([Parameter(Mandatory = $true)][string]$ClassName)

    $name = $ClassName -replace '^Sts1', ''
    $snake = [regex]::Replace($name, '([a-z0-9])([A-Z])', '$1_$2').ToLowerInvariant()
    return "sts1_$snake"
}

function Get-UniqueEventClasses {
    param([Parameter(Mandatory = $true)]$Registrations)

    return @($Registrations | Select-Object -ExpandProperty Event | Sort-Object -Unique)
}

function Get-EventIdsFromClasses {
    param([Parameter(Mandatory = $true)]$Classes)

    return @($Classes | ForEach-Object { Convert-ClassNameToEventId $_ } | Sort-Object -Unique)
}

function Get-FeatureGateIds {
    param(
        [Parameter(Mandatory = $true)][string]$Text,
        [Parameter(Mandatory = $true)][string]$PropertyName
    )

    $startToken = "$PropertyName { get; } ="
    $start = $Text.IndexOf($startToken, [System.StringComparison]::Ordinal)
    if ($start -lt 0) {
        Write-Error "Feature-gate id list not found: $PropertyName"
        exit 1
    }

    $end = $Text.IndexOf('];', $start, [System.StringComparison]::Ordinal)
    if ($end -lt 0) {
        Write-Error "Feature-gate id list terminator not found: $PropertyName"
        exit 1
    }

    $block = $Text.Substring($start, $end - $start)
    return @([regex]::Matches($block, '"(sts1_[^"]+)"') | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique)
}

function Add-Metric {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)]$Actual,
        [Parameter(Mandatory = $true)]$Expected
    )

    $passed = $Actual -eq $Expected
    $metrics.Add([pscustomobject]@{
        Name = $Name
        Actual = $Actual
        Expected = $Expected
        Passed = $passed
    }) | Out-Null

    if (-not $passed) {
        $mismatches.Add("$Name expected $Expected but found $Actual") | Out-Null
    }
}

function Add-SetCheck {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)]$Actual,
        [Parameter(Mandatory = $true)]$Expected
    )

    $actualJoined = (@($Actual) | Sort-Object -Unique) -join ','
    $expectedJoined = (@($Expected) | Sort-Object -Unique) -join ','
    Add-Metric -Name $Name -Actual $actualJoined -Expected $expectedJoined
}

$registrationService = Read-RegistrationServiceText $RegistrationServicePath
$featureGate = Read-PartialTypeText -Path $FeatureGatePath -TypeName 'Sts1EventFeatureGate'
$registry = Read-PartialTypeText -Path $RegistryPath -TypeName 'Sts1EventRegistry'
$project = Read-RepoText $ProjectPath
$resolvedModelsRoot = Resolve-RepoPath $ModelsRoot
$resolvedCanonicalMatrix = Resolve-RepoPath $CanonicalMatrixPath

if (-not (Test-Path -LiteralPath $resolvedModelsRoot)) {
    Write-Error "Model source root not found: $resolvedModelsRoot"
    exit 1
}

if (-not (Test-Path -LiteralPath $resolvedCanonicalMatrix)) {
    Write-Error "Canonical event matrix not found: $resolvedCanonicalMatrix"
    exit 1
}

$canaryRegistrations = @(Get-Registrations -Block (Get-MethodSlice -Text $registrationService -MethodName 'RegisterCanaryOnly') -MethodName 'RegisterCanaryOnly')
$batch1Registrations = @(Get-Registrations -Block (Get-MethodSlice -Text $registrationService -MethodName 'RegisterAdditiveBatch1') -MethodName 'RegisterAdditiveBatch1')
$registerAllRegistrations = @(Get-Registrations -Block (Get-MethodSlice -Text $registrationService -MethodName 'RegisterAll') -MethodName 'RegisterAll')

$canaryClasses = @(Get-UniqueEventClasses $canaryRegistrations)
$batch1Classes = @(Get-UniqueEventClasses $batch1Registrations)
$registerAllClasses = @(Get-UniqueEventClasses $registerAllRegistrations)

$canaryIds = @(Get-EventIdsFromClasses $canaryClasses)
$batch1Ids = @(Get-EventIdsFromClasses $batch1Classes)
$registerAllIds = @(Get-EventIdsFromClasses $registerAllClasses)
$featureCanaryIds = @(Get-FeatureGateIds -Text $featureGate -PropertyName 'CanaryEventIds')
$featureBatch1Ids = @(Get-FeatureGateIds -Text $featureGate -PropertyName 'AdditiveBatch1EventIds')

$registryEntryMatches = [regex]::Matches($registry, 'new\("([^"]+)",\s*"([^"]+)",\s*Sts1EventPhase\.([A-Za-z]+),\s*Sts1EventAct\.([A-Za-z0-9]+)\)')
$registryIds = @($registryEntryMatches | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique)
$expectedRegistryIds = @($registerAllIds + @('sts1_duplicator', 'sts1_neow', 'sts1_combat_start') | Sort-Object -Unique)

$modelFiles = @(Get-ChildItem -LiteralPath $resolvedModelsRoot -Recurse -Filter '*.cs' -File)
$compileExcludedMatches = [regex]::Matches($project, '<Compile\s+Remove="([^"]*Sts1Events/Models/[^"]+\.cs)"\s*/>')
$compileExcludedModelFiles = @($compileExcludedMatches | ForEach-Object { [System.IO.Path]::GetFileName($_.Groups[1].Value) } | Sort-Object -Unique)

$matrixRows = @(Import-Csv -LiteralPath $resolvedCanonicalMatrix)
$specialRows = @($matrixRows | Where-Object { $_.status -eq 'special-stub' })
$duplicateRows = @($matrixRows | Where-Object { $_.status -eq 'duplicate-wiki-entry' })

Add-Metric -Name 'canonical_rows' -Actual $matrixRows.Count -Expected 54
Add-Metric -Name 'public_wiki_baseline_rows' -Actual ($matrixRows.Count - $specialRows.Count) -Expected 52
Add-Metric -Name 'special_stub_rows' -Actual $specialRows.Count -Expected 2
Add-Metric -Name 'duplicate_wiki_rows' -Actual $duplicateRows.Count -Expected 4

Add-Metric -Name 'registry_entries' -Actual $registryEntryMatches.Count -Expected 50
Add-SetCheck -Name 'registry_ids_match_register_all_plus_exclusions_and_stubs' -Actual $registryIds -Expected $expectedRegistryIds
Add-Metric -Name 'registry_phase_canary' -Actual @($registryEntryMatches | Where-Object { $_.Groups[3].Value -eq 'Canary' }).Count -Expected 4
Add-Metric -Name 'registry_phase_simple' -Actual @($registryEntryMatches | Where-Object { $_.Groups[3].Value -eq 'Simple' }).Count -Expected 22
Add-Metric -Name 'registry_phase_card_service' -Actual @($registryEntryMatches | Where-Object { $_.Groups[3].Value -eq 'CardService' }).Count -Expected 9
Add-Metric -Name 'registry_phase_combat' -Actual @($registryEntryMatches | Where-Object { $_.Groups[3].Value -eq 'Combat' }).Count -Expected 5
Add-Metric -Name 'registry_phase_custom_ui' -Actual @($registryEntryMatches | Where-Object { $_.Groups[3].Value -eq 'CustomUi' }).Count -Expected 8
Add-Metric -Name 'registry_phase_special' -Actual @($registryEntryMatches | Where-Object { $_.Groups[3].Value -eq 'Special' }).Count -Expected 2

Add-Metric -Name 'model_files' -Actual $modelFiles.Count -Expected 48
Add-Metric -Name 'compile_excluded_model_files' -Actual $compileExcludedModelFiles.Count -Expected 1
Add-SetCheck -Name 'compile_excluded_models' -Actual $compileExcludedModelFiles -Expected @('Sts1Duplicator.cs')
Add-Metric -Name 'compiling_models' -Actual ($modelFiles.Count - $compileExcludedModelFiles.Count) -Expected 47

Add-Metric -Name 'canary_registration_calls' -Actual $canaryRegistrations.Count -Expected 6
Add-Metric -Name 'canary_event_types' -Actual $canaryClasses.Count -Expected 4
Add-SetCheck -Name 'canary_registration_ids_match_feature_gate' -Actual $canaryIds -Expected $featureCanaryIds

Add-Metric -Name 'additive_batch1_registration_calls' -Actual $batch1Registrations.Count -Expected 14
Add-Metric -Name 'additive_batch1_event_types' -Actual $batch1Classes.Count -Expected 10
Add-SetCheck -Name 'additive_batch1_registration_ids_match_feature_gate' -Actual $batch1Ids -Expected $featureBatch1Ids

Add-Metric -Name 'register_all_registration_calls' -Actual $registerAllRegistrations.Count -Expected 57
Add-Metric -Name 'register_all_event_types' -Actual $registerAllClasses.Count -Expected 47
Add-Metric -Name 'register_all_shared_calls' -Actual @($registerAllRegistrations | Where-Object { $_.Kind -eq 'SharedEvent' }).Count -Expected 14
Add-Metric -Name 'register_all_overgrowth_calls' -Actual @($registerAllRegistrations | Where-Object { $_.Act -eq 'Overgrowth' }).Count -Expected 10
Add-Metric -Name 'register_all_underdocks_calls' -Actual @($registerAllRegistrations | Where-Object { $_.Act -eq 'Underdocks' }).Count -Expected 10
Add-Metric -Name 'register_all_hive_calls' -Actual @($registerAllRegistrations | Where-Object { $_.Act -eq 'Hive' }).Count -Expected 14
Add-Metric -Name 'register_all_glory_calls' -Actual @($registerAllRegistrations | Where-Object { $_.Act -eq 'Glory' }).Count -Expected 9
Add-SetCheck -Name 'register_all_overgrowth_matches_underdocks' `
    -Actual @($registerAllRegistrations | Where-Object { $_.Act -eq 'Overgrowth' } | Select-Object -ExpandProperty Event) `
    -Expected @($registerAllRegistrations | Where-Object { $_.Act -eq 'Underdocks' } | Select-Object -ExpandProperty Event)

if ($registerAllClasses -contains 'Sts1Duplicator') {
    $mismatches.Add('RegisterAll unexpectedly includes Sts1Duplicator') | Out-Null
}

$report = [pscustomobject]@{
    Metrics = $metrics
    Mismatches = $mismatches
}

foreach ($metric in $metrics) {
    $status = if ($metric.Passed) { 'pass' } else { 'fail' }
    Write-Output "$($metric.Name)=$($metric.Actual) expected=$($metric.Expected) status=$status"
}

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

    $report | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $resolvedOutFile -Encoding UTF8
}

if ($FailOnMismatch -and $mismatches.Count -gt 0) {
    exit 1
}
