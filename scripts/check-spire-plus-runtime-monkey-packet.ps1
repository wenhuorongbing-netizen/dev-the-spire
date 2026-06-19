param(
    [Parameter(Mandatory = $true)]
    [string]$EvidenceDir,

    [int]$ExpectedIterations = 0,

    [string]$ExpectedPackageVersion,

    [string]$ExpectedGameVersion,

    [string]$ExpectedRitsuLibVersion,

    [string]$ExpectedRitsuCompatBranch,

    [int]$ExpectedPatchCount = 0,

    [string]$OutFile,

    [switch]$RequireCurrentSourceSnapshot,

    [switch]$FailOnMismatch
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
$logAuditScript = Join-Path $PSScriptRoot 'audit-godot-log.ps1'
$sts1EnabledModeLogVerifierScript = Join-Path $PSScriptRoot 'check-sts1-enabled-mode-runtime-log.ps1'
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

function Test-JsonProperty {
    param(
        [AllowNull()]$Object,
        [Parameter(Mandatory = $true)][string]$Name
    )

    if ($null -eq $Object) {
        return $false
    }

    foreach ($property in @($Object.PSObject.Properties)) {
        if ([string]::Equals($property.Name, $Name, [System.StringComparison]::Ordinal)) {
            return $true
        }
    }

    return $false
}

function Get-JsonValue {
    param(
        [AllowNull()]$Object,
        [Parameter(Mandatory = $true)][string]$Name,
        $DefaultValue = $null
    )

    if (Test-JsonProperty -Object $Object -Name $Name) {
        return $Object.$Name
    }

    return $DefaultValue
}

function Read-JsonOrNull {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$CheckName
    )

    try {
        return [System.IO.File]::ReadAllText($Path) | ConvertFrom-Json
    } catch {
        Add-Check -Name $CheckName -Passed $false -Detail "invalid JSON in $Path`: $($_.Exception.Message)"
        return $null
    }
}

function Contains-Text {
    param(
        [AllowEmptyString()][string]$Text,
        [Parameter(Mandatory = $true)][string]$Needle
    )

    if ([string]::IsNullOrWhiteSpace($Needle)) {
        return $false
    }

    return $Text.IndexOf($Needle, [System.StringComparison]::OrdinalIgnoreCase) -ge 0
}

function Get-CanonicalCommandAckPattern {
    param([AllowEmptyString()][string]$Command)

    if ([string]::IsNullOrWhiteSpace($Command)) {
        return ''
    }

    if ($Command -match '(?i)^\s*spireplus_test_ancient\s+VAKUU\s+confirm\s+fight\b') {
        return '\[SPIREPLUS-EVIDENCE\]\s+VakuuFight\s+fight_option_shown\b'
    }

    if ($Command -match '(?i)^\s*spireplus_test_ancient\s+([A-Z0-9_]+)\s+confirm\b') {
        $target = $Matches[1].ToUpperInvariant()
        if ($target.StartsWith('EZMB_', [System.StringComparison]::OrdinalIgnoreCase)) {
            $target = $target.Substring(5)
        }

        return "\[Spire Plus\] Starting unsaved live-test run for $([regex]::Escape($target)) Ancient UI evidence\."
    }

    return ''
}

function Get-CanonicalCommandOwnerArea {
    param([AllowEmptyString()][string]$Command)

    if ($Command -match '(?i)^\s*spireplus_test_ancient\s+([A-Z0-9_]+)\s+confirm\b(.*)$') {
        $target = $Matches[1].ToUpperInvariant()
        if ($target.StartsWith('EZMB_', [System.StringComparison]::OrdinalIgnoreCase)) {
            $target = $target.Substring(5)
        }

        $tail = $Matches[2]
        if ($target -eq 'VAKUU' -and $tail -match '(?i)\bfight\b') {
            return 'Ancients.Vakuu.FightOptionSetup'
        }

        switch ($target) {
            'URDA' { return 'Ancients.Urda.MapSaveState' }
            'MORVI' { return 'Ancients.Morvi.CardPlayState' }
            'LOTHA' { return 'Ancients.Lotha.CardPlayState' }
            'VAKUU' { return 'Ancients.Vakuu' }
        }
    }

    return ''
}

function Get-CanonicalCommandScenarioTag {
    param([AllowEmptyString()][string]$Command)

    if ($Command -match '(?i)^\s*spireplus_test_ancient\s+([A-Z0-9_]+)\s+confirm\b(.*)$') {
        $target = $Matches[1].ToUpperInvariant()
        if ($target.StartsWith('EZMB_', [System.StringComparison]::OrdinalIgnoreCase)) {
            $target = $target.Substring(5)
        }

        if ($target -eq 'VAKUU' -and $Matches[2] -match '(?i)\bfight\b') {
            return 'vakuu-fight'
        }

        return "ancient-ui-$($target.ToLowerInvariant())"
    }

    return ''
}

function Get-PatchCountLineHits {
    param(
        [AllowEmptyString()][string]$Text,
        [Parameter(Mandatory = $true)][int]$ExpectedCount
    )

    if ($ExpectedCount -le 0) {
        return 0
    }

    $count = [regex]::Escape([string]$ExpectedCount)
    $patchSummary = "(?im)^\[INFO\]\s+\[EZMicroBalance\]\s+\[Patcher - SpirePlus\]\s+Patch application complete:\s+$count\s+applied,\s+0\s+ignored,\s+0\s+failed,\s+$count\s+total\s*$"
    $registeredSummary = "(?im)^\[INFO\]\s+\[EZMicroBalance\]\s+ModPatcher applied\s+$count\s+patches\s+\($count\s+registered\)\.?\s*$"
    if ([regex]::IsMatch($Text, $patchSummary) -and [regex]::IsMatch($Text, $registeredSummary)) {
        return 1
    }

    return 0
}

function ConvertTo-AuditSummary {
    param(
        [Parameter(Mandatory = $true)][string]$Json,
        [Parameter(Mandatory = $true)][string]$Path
    )

    $items = @($Json | ConvertFrom-Json)
    $dirtyItems = 0
    $hitCount = 0
    $itemPaths = [System.Collections.Generic.List[string]]::new()
    $itemLengths = [System.Collections.Generic.List[long]]::new()
    $itemSha256s = [System.Collections.Generic.List[string]]::new()

    foreach ($item in $items) {
        if (-not (Test-JsonProperty -Object $item -Name 'Clean') -or -not [bool]$item.Clean) {
            $dirtyItems++
        }

        if ((Test-JsonProperty -Object $item -Name 'Path') -and -not [string]::IsNullOrWhiteSpace([string]$item.Path)) {
            $itemPaths.Add([System.IO.Path]::GetFullPath([string]$item.Path)) | Out-Null
        }

        if (Test-JsonProperty -Object $item -Name 'Length') {
            $itemLengths.Add([long]$item.Length) | Out-Null
        }

        if ((Test-JsonProperty -Object $item -Name 'Sha256') -and -not [string]::IsNullOrWhiteSpace([string]$item.Sha256)) {
            $itemSha256s.Add(([string]$item.Sha256).ToLowerInvariant()) | Out-Null
        }

        if (-not (Test-JsonProperty -Object $item -Name 'SignatureHits')) {
            continue
        }

        foreach ($hit in @($item.SignatureHits)) {
            if (Test-JsonProperty -Object $hit -Name 'Count') {
                $hitCount += [int]$hit.Count
            }
        }
    }

    return [pscustomobject]@{
        Path = $Path
        Items = $items.Count
        ItemPaths = @($itemPaths)
        ItemLengths = @($itemLengths)
        ItemSha256s = @($itemSha256s)
        DirtyItems = $dirtyItems
        SignatureHitCount = $hitCount
        Clean = ($items.Count -gt 0 -and $dirtyItems -eq 0 -and $hitCount -eq 0)
    }
}

function Read-AuditSummary {
    param([Parameter(Mandatory = $true)][string]$Path)

    $json = [System.IO.File]::ReadAllText($Path)
    return ConvertTo-AuditSummary -Json $json -Path $Path
}

function Invoke-RecomputedAuditSummary {
    param([Parameter(Mandatory = $true)][string]$LogPath)

    $auditJson = (& $logAuditScript -Path $LogPath | Out-String)
    if ([string]::IsNullOrWhiteSpace($auditJson)) {
        throw "audit-godot-log.ps1 returned empty output for $LogPath"
    }

    return ConvertTo-AuditSummary -Json $auditJson -Path '<recomputed>'
}

function Get-CheckSignatureArray {
    param([AllowNull()]$Items)

    if ($null -eq $Items) {
        return @()
    }

    return @($Items | ForEach-Object {
        $name = [string](Get-JsonValue -Object $_ -Name 'Name' -DefaultValue '')
        $passed = [bool](Get-JsonValue -Object $_ -Name 'Passed' -DefaultValue $false)
        $detail = [string](Get-JsonValue -Object $_ -Name 'Detail' -DefaultValue '')
        "$name|$passed|$detail"
    })
}

function Invoke-RecomputedSts1ModeLogCheck {
    param(
        [Parameter(Mandatory = $true)][string]$Mode,
        [Parameter(Mandatory = $true)][string]$LogPath,
        [Parameter(Mandatory = $true)][string]$AuditPath,
        [AllowEmptyString()][string]$EffectiveExpectedPackageVersion,
        [AllowEmptyString()][string]$EffectiveExpectedGameVersion,
        [AllowEmptyString()][string]$EffectiveExpectedRitsuLibVersion,
        [AllowEmptyString()][string]$EffectiveExpectedRitsuCompatBranch
    )

    $outFile = Join-Path ([System.IO.Path]::GetTempPath()) "spireplus-sts1-mode-log-check-$([System.Guid]::NewGuid().ToString('N')).json"
    try {
        $verifierParams = @{
            Mode = $Mode
            LogPath = $LogPath
            AuditPath = $AuditPath
            OutFile = $outFile
        }

        if (-not [string]::IsNullOrWhiteSpace($EffectiveExpectedPackageVersion)) {
            $verifierParams['ExpectedPackageVersion'] = $EffectiveExpectedPackageVersion
        }

        if (-not [string]::IsNullOrWhiteSpace($EffectiveExpectedGameVersion)) {
            $verifierParams['ExpectedGameVersion'] = $EffectiveExpectedGameVersion
        }

        if (-not [string]::IsNullOrWhiteSpace($EffectiveExpectedRitsuLibVersion)) {
            $verifierParams['ExpectedRitsuLibVersion'] = $EffectiveExpectedRitsuLibVersion
        }

        if (-not [string]::IsNullOrWhiteSpace($EffectiveExpectedRitsuCompatBranch)) {
            $verifierParams['ExpectedRitsuCompatBranch'] = $EffectiveExpectedRitsuCompatBranch
        }

        $verifierOutput = @(& $sts1EnabledModeLogVerifierScript @verifierParams 2>&1)
        if (-not (Test-Path -LiteralPath $outFile -PathType Leaf)) {
            throw "check-sts1-enabled-mode-runtime-log.ps1 did not write a recomputed report. Output: $($verifierOutput -join [Environment]::NewLine)"
        }

        return [System.IO.File]::ReadAllText($outFile) | ConvertFrom-Json
    } finally {
        if (Test-Path -LiteralPath $outFile -PathType Leaf) {
            Remove-Item -LiteralPath $outFile -Force
        }
    }
}

function Resolve-EvidenceFile {
    param([Parameter(Mandatory = $true)][string]$RelativePath)

    return [System.IO.Path]::GetFullPath((Join-Path $resolvedEvidenceDir $RelativePath))
}

function Resolve-ChildOrAbsolutePath {
    param(
        [Parameter(Mandatory = $true)][string]$BaseDir,
        [AllowEmptyString()][string]$Path
    )

    if ([string]::IsNullOrWhiteSpace($Path)) {
        return ''
    }

    try {
        if ([System.IO.Path]::IsPathRooted($Path)) {
            return [System.IO.Path]::GetFullPath($Path)
        }

        return [System.IO.Path]::GetFullPath((Join-Path $BaseDir $Path))
    } catch {
        return ''
    }
}

function Read-TextAfterByteOffset {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][long]$Offset
    )

    $stream = [System.IO.File]::Open($Path, [System.IO.FileMode]::Open, [System.IO.FileAccess]::Read, [System.IO.FileShare]::ReadWrite)
    try {
        [void]$stream.Seek($Offset, [System.IO.SeekOrigin]::Begin)
        $reader = [System.IO.StreamReader]::new($stream, [System.Text.Encoding]::UTF8, $true, 4096, $true)
        try {
            return $reader.ReadToEnd()
        } finally {
            $reader.Dispose()
        }
    } finally {
        $stream.Dispose()
    }
}

function Normalize-LogSliceForComparison {
    param([AllowNull()][string]$Text)

    if ($null -eq $Text) {
        return ''
    }

    $normalized = $Text
    if ($normalized.Length -gt 0 -and $normalized[0] -eq [char]0xFEFF) {
        $normalized = $normalized.Substring(1)
    }

    return $normalized -replace "[`r`n]+$", ''
}

function Test-BytePrefix {
    param(
        [Parameter(Mandatory = $true)][AllowEmptyCollection()][byte[]]$Prefix,
        [Parameter(Mandatory = $true)][AllowEmptyCollection()][byte[]]$Content
    )

    if ($Prefix.Length -gt $Content.Length) {
        return $false
    }

    for ($i = 0; $i -lt $Prefix.Length; $i++) {
        if ($Prefix[$i] -ne $Content[$i]) {
            return $false
        }
    }

    return $true
}

function Test-CurrentSliceBinding {
    param(
        [Parameter(Mandatory = $true)][string]$BeforePath,
        [Parameter(Mandatory = $true)][string]$AfterPath,
        [Parameter(Mandatory = $true)][string]$CurrentPath
    )

    $result = [ordered]@{
        BeforeExists = Test-Path -LiteralPath $BeforePath -PathType Leaf
        AfterExists = Test-Path -LiteralPath $AfterPath -PathType Leaf
        CurrentExists = Test-Path -LiteralPath $CurrentPath -PathType Leaf
        PrefixMatches = $false
        SliceMatches = $false
        Detail = ''
    }

    if (-not $result.BeforeExists -or -not $result.AfterExists -or -not $result.CurrentExists) {
        $result.Detail = 'requires godot.log.before, godot.log.after-launch, and godot.log.current-iteration'
        return [pscustomobject]$result
    }

    try {
        $beforeBytes = [System.IO.File]::ReadAllBytes($BeforePath)
        $afterBytes = [System.IO.File]::ReadAllBytes($AfterPath)
        $currentBytes = [System.IO.File]::ReadAllBytes($CurrentPath)
        $result.PrefixMatches = Test-BytePrefix -Prefix $beforeBytes -Content $afterBytes
        if (-not $result.PrefixMatches) {
            $result.Detail = 'godot.log.after-launch does not have godot.log.before as a byte prefix'
            return [pscustomobject]$result
        }

        $sliceLength = $afterBytes.Length - $beforeBytes.Length
        if ($currentBytes.Length -ne $sliceLength) {
            $result.Detail = "current slice length $($currentBytes.Length) does not match after-before length $sliceLength"
            return [pscustomobject]$result
        }

        for ($i = 0; $i -lt $sliceLength; $i++) {
            if ($currentBytes[$i] -ne $afterBytes[$beforeBytes.Length + $i]) {
                $result.Detail = "current slice differs from after-launch at byte $i after the before-log prefix"
                return [pscustomobject]$result
            }
        }

        $result.SliceMatches = $true
        $result.Detail = 'godot.log.current-iteration matches godot.log.after-launch after the godot.log.before byte prefix'
        return [pscustomobject]$result
    } catch {
        $result.Detail = $_.Exception.Message
        return [pscustomobject]$result
    }
}

function Test-PathUnderDirectory {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$Directory
    )

    if ([string]::IsNullOrWhiteSpace($Path) -or [string]::IsNullOrWhiteSpace($Directory)) {
        return $false
    }

    try {
        $directoryFull = [System.IO.Path]::GetFullPath($Directory).TrimEnd([System.IO.Path]::DirectorySeparatorChar, [System.IO.Path]::AltDirectorySeparatorChar) + [System.IO.Path]::DirectorySeparatorChar
        $pathFull = [System.IO.Path]::GetFullPath($Path)
        return $pathFull.StartsWith($directoryFull, [System.StringComparison]::OrdinalIgnoreCase)
    } catch {
        return $false
    }
}

function Get-ArrayCount {
    param([AllowNull()]$Value)

    if ($null -eq $Value) {
        return 0
    }

    return @($Value).Count
}

function Get-JsonArrayProperty {
    param(
        [AllowNull()]$Object,
        [Parameter(Mandatory = $true)][string]$Name
    )

    if ($null -eq $Object) {
        return [pscustomobject]@{
            Present = $false
            IsArray = $false
            Count = -1
            Value = @()
        }
    }

    $property = @($Object.PSObject.Properties | Where-Object { [string]::Equals($_.Name, $Name, [System.StringComparison]::Ordinal) } | Select-Object -First 1)
    if ($property.Count -ne 1 -or $null -eq $property[0].Value -or -not ($property[0].Value -is [System.Array])) {
        return [pscustomobject]@{
            Present = $property.Count -eq 1
            IsArray = $false
            Count = -1
            Value = @()
        }
    }

    $value = @($property[0].Value)
    return [pscustomobject]@{
        Present = $true
        IsArray = $true
        Count = $value.Count
        Value = $value
    }
}

function Get-FileSha256OrEmpty {
    param([AllowEmptyString()][string]$Path)

    if ([string]::IsNullOrWhiteSpace($Path)) {
        return ''
    }

    try {
        if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
            return ''
        }

        return (Get-FileHash -Algorithm SHA256 -LiteralPath $Path).Hash.ToLowerInvariant()
    } catch {
        return ''
    }
}

function Test-Sha256Text {
    param([AllowEmptyString()][string]$Value)

    return -not [string]::IsNullOrWhiteSpace($Value) -and $Value -match '^[A-Fa-f0-9]{64}$'
}

function ConvertTo-DateTimeUtcOrNull {
    param([AllowNull()]$Value)

    if ($null -eq $Value) {
        return $null
    }

    if ($Value -is [datetime]) {
        return ([datetime]$Value).ToUniversalTime()
    }

    $valueString = [string]$Value
    if ([string]::IsNullOrWhiteSpace($valueString)) {
        return $null
    }

    try {
        return [datetime]::Parse($valueString).ToUniversalTime()
    } catch {
        return $null
    }
}

function ConvertTo-StringArray {
    param([AllowNull()]$Value)

    if ($null -eq $Value) {
        return @()
    }

    return @($Value | ForEach-Object { [string]$_ })
}

function Test-StringArrayEquals {
    param(
        [Alias('Left')][AllowNull()]$Actual,
        [Alias('Right')][AllowNull()]$Expected
    )

    $actualArray = @(ConvertTo-StringArray -Value $Actual)
    $expectedArray = @(ConvertTo-StringArray -Value $Expected)
    if ($actualArray.Count -ne $expectedArray.Count) {
        return $false
    }

    for ($index = 0; $index -lt $actualArray.Count; $index++) {
        if (-not [string]::Equals($actualArray[$index], $expectedArray[$index], [System.StringComparison]::Ordinal)) {
            return $false
        }
    }

    return $true
}

function ConvertTo-NormalizedPathOrEmpty {
    param([AllowEmptyString()][string]$Path)

    if ([string]::IsNullOrWhiteSpace($Path)) {
        return ''
    }

    try {
        return [System.IO.Path]::GetFullPath($Path)
    } catch {
        return ''
    }
}

function Test-AllJsonPropertiesPresent {
    param(
        [AllowNull()]$Items,
        [Parameter(Mandatory = $true)][string]$Name
    )

    foreach ($item in @($Items)) {
        if (-not (Test-JsonProperty -Object $item -Name $Name) -or $null -eq $item.$Name) {
            return $false
        }
    }

    return @($Items).Count -gt 0
}

function Test-AllJsonPropertiesRetained {
    param(
        [AllowNull()]$Items,
        [Parameter(Mandatory = $true)][string]$Name
    )

    foreach ($item in @($Items)) {
        if (-not (Test-JsonProperty -Object $item -Name $Name)) {
            return $false
        }
    }

    return @($Items).Count -gt 0
}

function Test-AnyJsonPropertyTrue {
    param(
        [AllowNull()]$Items,
        [Parameter(Mandatory = $true)][string]$Name
    )

    foreach ($item in @($Items)) {
        if ([bool](Get-JsonValue -Object $item -Name $Name -DefaultValue $false)) {
            return $true
        }
    }

    return $false
}

function Test-NoJsonPropertyTrue {
    param(
        [AllowNull()]$Items,
        [Parameter(Mandatory = $true)][string]$Name
    )

    foreach ($item in @($Items)) {
        if ([bool](Get-JsonValue -Object $item -Name $Name -DefaultValue $false)) {
            return $false
        }
    }

    return $true
}

function Test-NoJsonPropertyFalse {
    param(
        [AllowNull()]$Items,
        [Parameter(Mandatory = $true)][string]$Name
    )

    foreach ($item in @($Items)) {
        if ((Test-JsonProperty -Object $item -Name $Name) -and $null -ne $item.$Name -and -not [bool]$item.$Name) {
            return $false
        }
    }

    return $true
}

function Get-ValueCounts {
    param(
        [AllowNull()]$Items,
        [Parameter(Mandatory = $true)][string]$PropertyName
    )

    $counts = [ordered]@{}
    foreach ($item in @($Items)) {
        $value = [string](Get-JsonValue -Object $item -Name $PropertyName -DefaultValue '')
        if ([string]::IsNullOrWhiteSpace($value)) {
            $value = '<none>'
        }

        if (-not $counts.Contains($value)) {
            $counts[$value] = 0
        }

        $counts[$value]++
    }

    return $counts
}

function Get-CountMapTotal {
    param([AllowNull()]$CountMap)

    if ($null -eq $CountMap) {
        return 0
    }

    $total = 0
    foreach ($property in @($CountMap.PSObject.Properties)) {
        $total += [int]$property.Value
    }

    return $total
}

function Test-CountMapMatches {
    param(
        [AllowNull()]$ActualCountMap,
        [Parameter(Mandatory = $true)]$ExpectedCounts
    )

    if ($null -eq $ActualCountMap) {
        return $false
    }

    $actualProperties = @($ActualCountMap.PSObject.Properties)
    $expectedKeys = @($ExpectedCounts.Keys)
    if ($actualProperties.Count -ne $expectedKeys.Count) {
        return $false
    }

    foreach ($key in $expectedKeys) {
        $actualProperty = $ActualCountMap.PSObject.Properties[$key]
        if ($null -eq $actualProperty -or [int]$actualProperty.Value -ne [int]$ExpectedCounts[$key]) {
            return $false
        }
    }

    return $true
}

function Get-IterationNumberAudit {
    param(
        [AllowNull()]$Items,
        [int]$ExpectedCount
    )

    $iterationNumbers = @($Items | ForEach-Object { [int](Get-JsonValue -Object $_ -Name 'Iteration' -DefaultValue 0) })
    $nonPositiveNumbers = @($iterationNumbers | Where-Object { $_ -le 0 })
    $duplicateNumbers = @($iterationNumbers |
        Where-Object { $_ -gt 0 } |
        Group-Object |
        Where-Object { $_.Count -gt 1 } |
        ForEach-Object { [int]$_.Name })
    $outOfRangeNumbers = @()
    $missingNumbers = @()

    if ($ExpectedCount -gt 0) {
        $seen = [System.Collections.Generic.HashSet[int]]::new()
        foreach ($iterationNumber in $iterationNumbers) {
            if ($iterationNumber -gt 0 -and $iterationNumber -le $ExpectedCount) {
                $seen.Add($iterationNumber) | Out-Null
            } elseif ($iterationNumber -gt $ExpectedCount) {
                $outOfRangeNumbers += $iterationNumber
            }
        }

        for ($iterationNumber = 1; $iterationNumber -le $ExpectedCount; $iterationNumber++) {
            if (-not $seen.Contains($iterationNumber)) {
                $missingNumbers += $iterationNumber
            }
        }
    }

    return [pscustomobject]@{
        Numbers = @($iterationNumbers)
        DuplicateNumbers = @($duplicateNumbers)
        MissingNumbers = @($missingNumbers)
        NonPositiveNumbers = @($nonPositiveNumbers)
        OutOfRangeNumbers = @($outOfRangeNumbers)
    }
}

$resolvedEvidenceDir = Resolve-RepoPath $EvidenceDir
if (-not (Test-Path -LiteralPath $resolvedEvidenceDir -PathType Container)) {
    Write-Error "Evidence directory not found: $resolvedEvidenceDir"
    exit 1
}

$planPath = Resolve-EvidenceFile 'monkey-plan.json'
$summaryPath = Resolve-EvidenceFile 'monkey-summary.json'

Write-Output "evidence_dir=$resolvedEvidenceDir"

$planExists = Test-Path -LiteralPath $planPath -PathType Leaf
$summaryExists = Test-Path -LiteralPath $summaryPath -PathType Leaf
Add-Check -Name 'monkey_plan_exists' -Passed $planExists -Detail 'requires monkey-plan.json'
Add-Check -Name 'monkey_summary_exists' -Passed $summaryExists -Detail 'requires monkey-summary.json from a launched monkey run'

$plan = $null
$summary = $null
if ($planExists) {
    $plan = Read-JsonOrNull -Path $planPath -CheckName 'monkey_plan_json_valid'
    if ($null -ne $plan) {
        Add-Check -Name 'plan_json_valid' -Passed $true -Detail 'monkey-plan.json parsed'
    }
}

if ($summaryExists) {
    $summary = Read-JsonOrNull -Path $summaryPath -CheckName 'monkey_summary_json_valid'
    if ($null -ne $summary) {
        Add-Check -Name 'summary_json_valid' -Passed $true -Detail 'monkey-summary.json parsed'
    }
}

$planIterations = if ($null -ne $plan) { [int](Get-JsonValue -Object $plan -Name 'Iterations' -DefaultValue 0) } else { 0 }
$planUnresponsiveSampleThreshold = if ($null -ne $plan) { [int](Get-JsonValue -Object $plan -Name 'UnresponsiveSampleThreshold' -DefaultValue 0) } else { 0 }
$summaryRequestedIterations = if ($null -ne $summary) { [int](Get-JsonValue -Object $summary -Name 'RequestedIterations' -DefaultValue 0) } else { 0 }
$summaryCompletedIterations = if ($null -ne $summary) { [int](Get-JsonValue -Object $summary -Name 'CompletedIterations' -DefaultValue 0) } else { 0 }
$expectedIterationCount = $ExpectedIterations
$planScenario = if ($null -ne $plan) { [string](Get-JsonValue -Object $plan -Name 'Scenario' -DefaultValue '') } else { '' }
$planCommandSelectionMode = if ($null -ne $plan) { [string](Get-JsonValue -Object $plan -Name 'CommandSelectionMode' -DefaultValue '') } else { '' }
$planPlannedCommands = @(
    if ($null -ne $plan -and (Test-JsonProperty -Object $plan -Name 'PlannedCommands')) {
        $plan.PlannedCommands
    }
)
$summaryResults = @()
if ($expectedIterationCount -le 0 -and $planIterations -gt 0) {
    $expectedIterationCount = $planIterations
}

if ($expectedIterationCount -le 0 -and $summaryRequestedIterations -gt 0) {
    $expectedIterationCount = $summaryRequestedIterations
}

if ($null -ne $plan) {
    Add-Check -Name 'plan_hang_probe_schema_version' -Passed ([int](Get-JsonValue -Object $plan -Name 'HangProbeSchemaVersion' -DefaultValue 0) -eq 1) -Detail 'HangProbeSchemaVersion must be 1'
    Add-Check -Name 'plan_launch_true' -Passed ([bool](Get-JsonValue -Object $plan -Name 'Launch' -DefaultValue $false)) -Detail 'monkey packets require a launched run; dry-run plans have no runtime evidence'
    Add-Check -Name 'plan_iterations_positive' -Passed ($planIterations -gt 0) -Detail "plan Iterations must be positive; found $planIterations"
    Add-Check -Name 'plan_scenario_present' -Passed (-not [string]::IsNullOrWhiteSpace([string](Get-JsonValue -Object $plan -Name 'Scenario' -DefaultValue ''))) -Detail 'Scenario must identify the planned risk lane'
    Add-Check -Name 'plan_command_selection_mode_present' -Passed (-not [string]::IsNullOrWhiteSpace([string](Get-JsonValue -Object $plan -Name 'CommandSelectionMode' -DefaultValue ''))) -Detail 'CommandSelectionMode must be retained'
    Add-Check -Name 'plan_command_corpus_source_present' -Passed (-not [string]::IsNullOrWhiteSpace([string](Get-JsonValue -Object $plan -Name 'CommandCorpusSource' -DefaultValue ''))) -Detail 'CommandCorpusSource must be retained'
    if (-not [string]::IsNullOrWhiteSpace($ExpectedPackageVersion)) {
        Add-Check -Name 'plan_expected_package_version_matches' -Passed ([string]::Equals([string](Get-JsonValue -Object $plan -Name 'ExpectedPackageVersion' -DefaultValue ''), $ExpectedPackageVersion, [System.StringComparison]::Ordinal)) -Detail "monkey-plan ExpectedPackageVersion must match '$ExpectedPackageVersion'"
    }
    if (-not [string]::IsNullOrWhiteSpace($ExpectedGameVersion)) {
        Add-Check -Name 'plan_expected_game_version_matches' -Passed ([string]::Equals([string](Get-JsonValue -Object $plan -Name 'ExpectedGameVersion' -DefaultValue ''), $ExpectedGameVersion, [System.StringComparison]::Ordinal)) -Detail "monkey-plan ExpectedGameVersion must match '$ExpectedGameVersion'"
    }
    if (-not [string]::IsNullOrWhiteSpace($ExpectedRitsuLibVersion)) {
        Add-Check -Name 'plan_expected_ritsulib_version_matches' -Passed ([string]::Equals([string](Get-JsonValue -Object $plan -Name 'ExpectedRitsuLibVersion' -DefaultValue ''), $ExpectedRitsuLibVersion, [System.StringComparison]::Ordinal)) -Detail "monkey-plan ExpectedRitsuLibVersion must match '$ExpectedRitsuLibVersion'"
    }
    if (-not [string]::IsNullOrWhiteSpace($ExpectedRitsuCompatBranch)) {
        Add-Check -Name 'plan_expected_ritsu_compat_branch_matches' -Passed ([string]::Equals([string](Get-JsonValue -Object $plan -Name 'ExpectedRitsuCompatBranch' -DefaultValue ''), $ExpectedRitsuCompatBranch, [System.StringComparison]::Ordinal)) -Detail "monkey-plan ExpectedRitsuCompatBranch must match '$ExpectedRitsuCompatBranch'"
    }
    $sourceWorkspaceCheckPath = Resolve-ChildOrAbsolutePath -BaseDir $resolvedEvidenceDir -Path ([string](Get-JsonValue -Object $plan -Name 'SourceWorkspaceCheckPath' -DefaultValue ''))
    $sourceWorkspaceCheckSha256 = [string](Get-JsonValue -Object $plan -Name 'SourceWorkspaceCheckSha256' -DefaultValue '')
    $sourceWorkspace = Get-JsonValue -Object $plan -Name 'SourceWorkspace' -DefaultValue $null
    $sourceWorkspaceReport = $null
    $sourceWorkspaceCheckExists = $sourceWorkspaceCheckPath -and (Test-Path -LiteralPath $sourceWorkspaceCheckPath -PathType Leaf)
    Add-Check -Name 'plan_source_workspace_check_path_present' -Passed (-not [string]::IsNullOrWhiteSpace($sourceWorkspaceCheckPath)) -Detail 'SourceWorkspaceCheckPath must bind the packet to the local recovered-source workspace check'
    Add-Check -Name 'plan_source_workspace_check_under_evidence_dir' -Passed ($sourceWorkspaceCheckPath -and (Test-PathUnderDirectory -Path $sourceWorkspaceCheckPath -Directory $resolvedEvidenceDir)) -Detail 'SourceWorkspaceCheckPath must stay inside the evidence directory'
    Add-Check -Name 'plan_source_workspace_check_exists' -Passed $sourceWorkspaceCheckExists -Detail 'requires retained local-godot-source-workspace-check.json'
    Add-Check -Name 'plan_source_workspace_check_hash_present' -Passed (-not [string]::IsNullOrWhiteSpace($sourceWorkspaceCheckSha256)) -Detail 'SourceWorkspaceCheckSha256 must be retained for packet/source snapshot binding'
    if ($sourceWorkspaceCheckExists -and -not [string]::IsNullOrWhiteSpace($sourceWorkspaceCheckSha256)) {
        Add-Check -Name 'plan_source_workspace_check_hash_matches' -Passed ([string]::Equals((Get-FileSha256OrEmpty -Path $sourceWorkspaceCheckPath), $sourceWorkspaceCheckSha256, [System.StringComparison]::OrdinalIgnoreCase)) -Detail 'SourceWorkspaceCheckSha256 must match the retained source-workspace JSON report'
    }
    if ($sourceWorkspaceCheckExists) {
        $sourceWorkspaceReport = Read-JsonOrNull -Path $sourceWorkspaceCheckPath -CheckName 'plan_source_workspace_check_json_valid'
        if ($null -ne $sourceWorkspaceReport) {
            Add-Check -Name 'plan_source_workspace_check_json_valid' -Passed $true -Detail 'retained source-workspace JSON report parsed'
            $sourceWorkspaceReportMismatches = Get-ArrayCount -Value (Get-JsonValue -Object $sourceWorkspaceReport -Name 'Mismatches' -DefaultValue @())
            $sourceWorkspaceReportPolicy = Get-JsonValue -Object $sourceWorkspaceReport -Name 'EvidenceUsePolicy' -DefaultValue $null
            Add-Check -Name 'plan_source_workspace_report_passed' -Passed ([bool](Get-JsonValue -Object $sourceWorkspaceReport -Name 'Passed' -DefaultValue $false)) -Detail 'retained source-workspace report must have Passed=true'
            Add-Check -Name 'plan_source_workspace_report_mismatches_field_present' -Passed (Test-JsonProperty -Object $sourceWorkspaceReport -Name 'Mismatches') -Detail 'retained source-workspace report must retain Mismatches'
            Add-Check -Name 'plan_source_workspace_report_mismatches_empty' -Passed ((Test-JsonProperty -Object $sourceWorkspaceReport -Name 'Mismatches') -and $sourceWorkspaceReportMismatches -eq 0) -Detail "retained source-workspace report mismatches must be empty; found $sourceWorkspaceReportMismatches"
            Add-Check -Name 'plan_source_workspace_report_not_runtime_proof' -Passed ([bool](Get-JsonValue -Object $sourceWorkspaceReportPolicy -Name 'NotRuntimeProof' -DefaultValue $false)) -Detail 'source-workspace report must record that source inspection is not runtime proof'
            Add-Check -Name 'plan_source_workspace_report_local_source_reference_only' -Passed ([bool](Get-JsonValue -Object $sourceWorkspaceReportPolicy -Name 'LocalSourceReferenceOnly' -DefaultValue $false)) -Detail 'source-workspace report must record local-source-reference-only policy'
            Add-Check -Name 'plan_source_workspace_report_authorized_local_install_only' -Passed ([bool](Get-JsonValue -Object $sourceWorkspaceReportPolicy -Name 'AuthorizedLocalInstallOnly' -DefaultValue $false)) -Detail 'source-workspace report must record authorized-local-install-only policy'
            Add-Check -Name 'plan_source_workspace_report_authorized_source_origin_field_present' -Passed (Test-JsonProperty -Object $sourceWorkspaceReportPolicy -Name 'AuthorizedSourceOriginVerified') -Detail 'source-workspace report must retain whether the GDRE Opening file was verified against the installed game PCK'
            Add-Check -Name 'plan_source_workspace_report_third_party_dumps_prohibited' -Passed ([bool](Get-JsonValue -Object $sourceWorkspaceReportPolicy -Name 'ThirdPartyDumpsProhibited' -DefaultValue $false)) -Detail 'source-workspace report must record that third-party dumps are prohibited'
        }
    }
    Add-Check -Name 'plan_source_workspace_summary_present' -Passed ($null -ne $sourceWorkspace) -Detail 'SourceWorkspace summary must retain source version/disposition and evidence-use policy'
    if ($null -ne $sourceWorkspace) {
        Add-Check -Name 'plan_source_workspace_checked' -Passed ([bool](Get-JsonValue -Object $sourceWorkspace -Name 'Checked' -DefaultValue $false)) -Detail 'SourceWorkspace.Checked must be true'
        Add-Check -Name 'plan_source_workspace_not_runtime_proof' -Passed ([bool](Get-JsonValue -Object $sourceWorkspace -Name 'NotRuntimeProof' -DefaultValue $false)) -Detail 'SourceWorkspace must record that source inspection is not runtime proof'
        Add-Check -Name 'plan_source_workspace_disposition_present' -Passed (-not [string]::IsNullOrWhiteSpace([string](Get-JsonValue -Object $sourceWorkspace -Name 'Disposition' -DefaultValue ''))) -Detail 'SourceWorkspace must retain the recovered-source disposition'
        Add-Check -Name 'plan_source_workspace_authorized_source_origin_field_present' -Passed (Test-JsonProperty -Object $sourceWorkspace -Name 'AuthorizedSourceOriginVerified') -Detail 'SourceWorkspace summary must retain whether source-origin verification passed'
        Add-Check -Name 'plan_source_workspace_origin_matches_installed_game_pck_field_present' -Passed (Test-JsonProperty -Object $sourceWorkspace -Name 'OriginMatchesInstalledGamePck') -Detail 'SourceWorkspace summary must retain GDRE Opening file to installed PCK binding state'
        Add-Check -Name 'plan_source_workspace_ritsulib_version_field_present' -Passed (Test-JsonProperty -Object $sourceWorkspace -Name 'RitsuLibVersion') -Detail 'SourceWorkspace summary must retain RitsuLib manifest version'
        Add-Check -Name 'plan_source_workspace_ritsulib_manifest_hash_field_present' -Passed (Test-JsonProperty -Object $sourceWorkspace -Name 'RitsuLibManifestSha256') -Detail 'SourceWorkspace summary must retain RitsuLib manifest hash'
        Add-Check -Name 'plan_source_workspace_ritsulib_variant_dll_hash_field_present' -Passed (Test-JsonProperty -Object $sourceWorkspace -Name 'RitsuLibVariantDllSha256') -Detail 'SourceWorkspace summary must retain selected RitsuLib variant DLL hash'
    }
    if ($null -ne $sourceWorkspace -and $null -ne $sourceWorkspaceReport) {
        $reportRecoveredSource = Get-JsonValue -Object $sourceWorkspaceReport -Name 'RecoveredSource' -DefaultValue $null
        $reportGame = Get-JsonValue -Object $sourceWorkspaceReport -Name 'Game' -DefaultValue $null
        $reportPolicy = Get-JsonValue -Object $sourceWorkspaceReport -Name 'EvidenceUsePolicy' -DefaultValue $null
        $reportRitsuLib = Get-JsonValue -Object $sourceWorkspaceReport -Name 'RitsuLib' -DefaultValue $null
        Add-Check -Name 'plan_source_workspace_report_origin_matches_installed_game_pck_field_present' -Passed (Test-JsonProperty -Object $reportRecoveredSource -Name 'OriginMatchesInstalledGamePck') -Detail 'retained source-workspace report must retain GDRE Opening file to installed PCK binding state'
        Add-Check -Name 'plan_source_workspace_report_ritsulib_field_present' -Passed ($null -ne $reportRitsuLib) -Detail 'retained source-workspace report must retain RitsuLib provenance'
        if ($null -ne $reportRitsuLib) {
            foreach ($name in @('Version', 'CompatBranch', 'ManifestPath', 'ManifestSha256', 'VariantsPath', 'VariantsSha256', 'VariantDllPath', 'VariantDllSha256', 'ExpectedVariantDllSha256', 'CompatTargetPath', 'CompatTargetText')) {
                Add-Check -Name "plan_source_workspace_report_ritsulib_$($name.ToLowerInvariant())_present" -Passed (Test-JsonProperty -Object $reportRitsuLib -Name $name) -Detail "retained source-workspace report must include RitsuLib.$name"
            }
        }
        $summaryMatchesReport =
            ([bool](Get-JsonValue -Object $sourceWorkspace -Name 'Passed' -DefaultValue $false) -eq [bool](Get-JsonValue -Object $sourceWorkspaceReport -Name 'Passed' -DefaultValue $false)) -and
            ([string](Get-JsonValue -Object $sourceWorkspace -Name 'SourceVersion' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRecoveredSource -Name 'Version' -DefaultValue '')) -and
            ([string](Get-JsonValue -Object $sourceWorkspace -Name 'SourceCommit' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRecoveredSource -Name 'Commit' -DefaultValue '')) -and
            ([string](Get-JsonValue -Object $sourceWorkspace -Name 'SourceBranch' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRecoveredSource -Name 'Branch' -DefaultValue '')) -and
            ([string](Get-JsonValue -Object $sourceWorkspace -Name 'SourceMainAssemblyHash' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRecoveredSource -Name 'MainAssemblyHash' -DefaultValue '')) -and
            ([string](Get-JsonValue -Object $sourceWorkspace -Name 'InstalledGameVersion' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportGame -Name 'Version' -DefaultValue '')) -and
            ([string](Get-JsonValue -Object $sourceWorkspace -Name 'InstalledGameCommit' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportGame -Name 'Commit' -DefaultValue '')) -and
            ([string](Get-JsonValue -Object $sourceWorkspace -Name 'InstalledGameBranch' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportGame -Name 'Branch' -DefaultValue '')) -and
            ([string](Get-JsonValue -Object $sourceWorkspace -Name 'InstalledGameMainAssemblyHash' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportGame -Name 'MainAssemblyHash' -DefaultValue '')) -and
            ([string](Get-JsonValue -Object $sourceWorkspace -Name 'Disposition' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRecoveredSource -Name 'Disposition' -DefaultValue '')) -and
            ([bool](Get-JsonValue -Object $sourceWorkspace -Name 'MatchesInstalledGame' -DefaultValue $false) -eq [bool](Get-JsonValue -Object $reportRecoveredSource -Name 'MatchesInstalledGame' -DefaultValue $false)) -and
            ([string](Get-JsonValue -Object $sourceWorkspace -Name 'OriginPckPath' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRecoveredSource -Name 'OriginPckPath' -DefaultValue '')) -and
            ([bool](Get-JsonValue -Object $sourceWorkspace -Name 'OriginMatchesInstalledGamePck' -DefaultValue $false) -eq [bool](Get-JsonValue -Object $reportRecoveredSource -Name 'OriginMatchesInstalledGamePck' -DefaultValue $false)) -and
            ([string](Get-JsonValue -Object $sourceWorkspace -Name 'RitsuLibVersion' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRitsuLib -Name 'Version' -DefaultValue '')) -and
            ([string](Get-JsonValue -Object $sourceWorkspace -Name 'RitsuLibCompatBranch' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRitsuLib -Name 'CompatBranch' -DefaultValue '')) -and
            ([string](Get-JsonValue -Object $sourceWorkspace -Name 'RitsuLibManifestPath' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRitsuLib -Name 'ManifestPath' -DefaultValue '')) -and
            ([string](Get-JsonValue -Object $sourceWorkspace -Name 'RitsuLibManifestSha256' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRitsuLib -Name 'ManifestSha256' -DefaultValue '')) -and
            ([string](Get-JsonValue -Object $sourceWorkspace -Name 'RitsuLibVariantsPath' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRitsuLib -Name 'VariantsPath' -DefaultValue '')) -and
            ([string](Get-JsonValue -Object $sourceWorkspace -Name 'RitsuLibVariantsSha256' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRitsuLib -Name 'VariantsSha256' -DefaultValue '')) -and
            ([string](Get-JsonValue -Object $sourceWorkspace -Name 'RitsuLibVariantDllPath' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRitsuLib -Name 'VariantDllPath' -DefaultValue '')) -and
            ([string](Get-JsonValue -Object $sourceWorkspace -Name 'RitsuLibVariantDllSha256' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRitsuLib -Name 'VariantDllSha256' -DefaultValue '')) -and
            ([string](Get-JsonValue -Object $sourceWorkspace -Name 'RitsuLibExpectedVariantDllSha256' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRitsuLib -Name 'ExpectedVariantDllSha256' -DefaultValue '')) -and
            ([string](Get-JsonValue -Object $sourceWorkspace -Name 'RitsuLibCompatTargetPath' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRitsuLib -Name 'CompatTargetPath' -DefaultValue '')) -and
            ([string](Get-JsonValue -Object $sourceWorkspace -Name 'RitsuLibCompatTargetText' -DefaultValue '') -eq [string](Get-JsonValue -Object $reportRitsuLib -Name 'CompatTargetText' -DefaultValue '')) -and
            ([bool](Get-JsonValue -Object $sourceWorkspace -Name 'RefreshSourceSnapshotBeforeCurrentApiClaims' -DefaultValue $true) -eq [bool](Get-JsonValue -Object $reportPolicy -Name 'RefreshSourceSnapshotBeforeCurrentApiClaims' -DefaultValue $true)) -and
            ([bool](Get-JsonValue -Object $sourceWorkspace -Name 'NotRuntimeProof' -DefaultValue $false) -eq [bool](Get-JsonValue -Object $reportPolicy -Name 'NotRuntimeProof' -DefaultValue $false)) -and
            ([bool](Get-JsonValue -Object $sourceWorkspace -Name 'AuthorizedSourceOriginVerified' -DefaultValue $false) -eq [bool](Get-JsonValue -Object $reportPolicy -Name 'AuthorizedSourceOriginVerified' -DefaultValue $false)) -and
            ([string](Get-JsonValue -Object $sourceWorkspace -Name 'ReportSha256' -DefaultValue '') -eq $sourceWorkspaceCheckSha256)
        Add-Check -Name 'plan_source_workspace_report_matches_summary' -Passed $summaryMatchesReport -Detail 'SourceWorkspace summary must match the retained source-workspace report and ReportSha256'

        $currentSourceRequired = [bool]$RequireCurrentSourceSnapshot -or [bool](Get-JsonValue -Object $sourceWorkspace -Name 'RequireCurrentSourceSnapshot' -DefaultValue $false)
        if ($currentSourceRequired) {
            Add-Check -Name 'plan_source_workspace_required_current_snapshot_matches_game' -Passed ([bool](Get-JsonValue -Object $reportRecoveredSource -Name 'MatchesInstalledGame' -DefaultValue $false)) -Detail 'RequireCurrentSourceSnapshot requires RecoveredSource.MatchesInstalledGame=true in the retained report'
            Add-Check -Name 'plan_source_workspace_required_current_snapshot_origin_verified' -Passed ([bool](Get-JsonValue -Object $reportRecoveredSource -Name 'OriginMatchesInstalledGamePck' -DefaultValue $false)) -Detail 'RequireCurrentSourceSnapshot requires RecoveredSource.OriginMatchesInstalledGamePck=true in the retained report'
            Add-Check -Name 'plan_source_workspace_required_authorized_source_origin_verified' -Passed ([bool](Get-JsonValue -Object $reportPolicy -Name 'AuthorizedSourceOriginVerified' -DefaultValue $false)) -Detail 'RequireCurrentSourceSnapshot requires EvidenceUsePolicy.AuthorizedSourceOriginVerified=true in the retained report'
        }
    } elseif ($null -ne $sourceWorkspace) {
        Add-Check -Name 'plan_source_workspace_report_matches_summary' -Passed $false -Detail 'SourceWorkspace summary cannot be trusted without a valid retained source-workspace report'
    }
    Add-Check -Name 'plan_observation_interval_positive' -Passed ([int](Get-JsonValue -Object $plan -Name 'ObservationIntervalSeconds' -DefaultValue 0) -gt 0) -Detail 'ObservationIntervalSeconds must be present and positive'
    Add-Check -Name 'plan_unresponsive_sample_threshold_positive' -Passed ($planUnresponsiveSampleThreshold -gt 0) -Detail 'UnresponsiveSampleThreshold must be present and positive'
    Add-Check -Name 'plan_no_log_growth_timeout_positive' -Passed ([int](Get-JsonValue -Object $plan -Name 'NoLogGrowthTimeoutSeconds' -DefaultValue 0) -gt 0) -Detail 'NoLogGrowthTimeoutSeconds must be present and positive'
    Add-Check -Name 'plan_process_probe_present' -Passed (Test-JsonProperty -Object $plan -Name 'ProcessProbe') -Detail 'ProcessProbe must describe process/window checks'
    Add-Check -Name 'plan_log_growth_probe_present' -Passed (Test-JsonProperty -Object $plan -Name 'LogGrowthProbe') -Detail 'LogGrowthProbe must describe log-growth checks'
    Add-Check -Name 'plan_command_scenario_matrix_present' -Passed (Test-JsonProperty -Object $plan -Name 'CommandScenarioMatrix') -Detail 'CommandScenarioMatrix must map commands to owner areas'
    Add-Check -Name 'plan_command_ack_patterns_present' -Passed (Test-JsonProperty -Object $plan -Name 'CommandAckPatterns') -Detail 'CommandAckPatterns must be retained even when empty'
    Add-Check -Name 'plan_planned_commands_count_matches_iterations' -Passed ($planPlannedCommands.Count -eq $planIterations) -Detail "PlannedCommands must match plan Iterations; planned=$($planPlannedCommands.Count), iterations=$planIterations"

    $plannedScenarioTagCounts = Get-ValueCounts -Items $planPlannedCommands -PropertyName 'ScenarioTag'
    $plannedOwnerAreaCounts = Get-ValueCounts -Items $planPlannedCommands -PropertyName 'OwnerArea'
    $plannedCommandCounts = Get-ValueCounts -Items $planPlannedCommands -PropertyName 'Command'
    Add-Check -Name 'plan_planned_scenario_tag_counts_match' -Passed (Test-CountMapMatches -ActualCountMap (Get-JsonValue -Object $plan -Name 'PlannedScenarioTagCounts' -DefaultValue $null) -ExpectedCounts $plannedScenarioTagCounts) -Detail 'PlannedScenarioTagCounts must match PlannedCommands'
    Add-Check -Name 'plan_planned_owner_area_counts_match' -Passed (Test-CountMapMatches -ActualCountMap (Get-JsonValue -Object $plan -Name 'PlannedOwnerAreaCounts' -DefaultValue $null) -ExpectedCounts $plannedOwnerAreaCounts) -Detail 'PlannedOwnerAreaCounts must match PlannedCommands'
    Add-Check -Name 'plan_planned_command_counts_match' -Passed (Test-CountMapMatches -ActualCountMap (Get-JsonValue -Object $plan -Name 'PlannedCommandCounts' -DefaultValue $null) -ExpectedCounts $plannedCommandCounts) -Detail 'PlannedCommandCounts must match PlannedCommands'

    $plannedVakuuFightCount = @($planPlannedCommands | Where-Object { [string](Get-JsonValue -Object $_ -Name 'ScenarioTag' -DefaultValue '') -eq 'vakuu-fight' }).Count
    Add-Check -Name 'plan_vakuu_fight_planned_count_matches' -Passed ([int](Get-JsonValue -Object $plan -Name 'PlannedVakuuFightIterationCount' -DefaultValue -1) -eq $plannedVakuuFightCount) -Detail "PlannedVakuuFightIterationCount must match PlannedCommands; expected $plannedVakuuFightCount"
    if ($planScenario -eq 'VakuuFightSmoke') {
        Add-Check -Name 'plan_vakuu_fight_smoke_all_iterations_are_fight' -Passed ($plannedVakuuFightCount -eq $planIterations) -Detail "VakuuFightSmoke must plan only vakuu-fight iterations; found $plannedVakuuFightCount of $planIterations"
    } elseif ($planScenario -eq 'AncientUiPlusVakuuFight') {
        Add-Check -Name 'plan_ancient_ui_plus_vakuu_fight_includes_fight' -Passed ($plannedVakuuFightCount -gt 0) -Detail 'AncientUiPlusVakuuFight must include at least one vakuu-fight iteration'
        if ($planCommandSelectionMode -eq 'RoundRobin' -and $planIterations -eq 1000) {
            Add-Check -Name 'plan_ancient_ui_plus_vakuu_fight_1000_balanced' -Passed ($plannedVakuuFightCount -eq 200) -Detail "1000-iteration AncientUiPlusVakuuFight RoundRobin plan must include exactly 200 vakuu-fight iterations; found $plannedVakuuFightCount"
        }
    }

    if ($ExpectedIterations -gt 0) {
        Add-Check -Name 'expected_iterations_match_plan' -Passed ($planIterations -eq $ExpectedIterations) -Detail "expected $ExpectedIterations iterations in plan, found $planIterations"
    }
}

if ($null -ne $summary) {
    if (Test-JsonProperty -Object $summary -Name 'Results') {
        $summaryResults = @($summary.Results)
    }

    Add-Check -Name 'summary_hang_probe_schema_version' -Passed ([int](Get-JsonValue -Object $summary -Name 'HangProbeSchemaVersion' -DefaultValue 0) -eq 1) -Detail 'summary HangProbeSchemaVersion must be 1'
    Add-Check -Name 'summary_passed' -Passed ([bool](Get-JsonValue -Object $summary -Name 'Passed' -DefaultValue $false)) -Detail 'monkey-summary.json Passed must be true'
    Add-Check -Name 'summary_failed_iterations_zero' -Passed ([int](Get-JsonValue -Object $summary -Name 'FailedIterations' -DefaultValue -1) -eq 0) -Detail "FailedIterations must be 0; found $(Get-JsonValue -Object $summary -Name 'FailedIterations' -DefaultValue 'missing')"
    $failedIterationIdsCount = Get-ArrayCount -Value (Get-JsonValue -Object $summary -Name 'FailedIterationIds' -DefaultValue @())
    Add-Check -Name 'summary_failed_iteration_ids_empty' -Passed ($failedIterationIdsCount -eq 0) -Detail 'FailedIterationIds must be empty for a clean packet'
    Add-Check -Name 'summary_process_exit_count_zero' -Passed ([int](Get-JsonValue -Object $summary -Name 'ProcessExitCount' -DefaultValue -1) -eq 0) -Detail 'ProcessExitCount must be 0'
    Add-Check -Name 'summary_main_window_missing_count_zero' -Passed ([int](Get-JsonValue -Object $summary -Name 'MainWindowMissingCount' -DefaultValue -1) -eq 0) -Detail 'MainWindowMissingCount must be 0'
    Add-Check -Name 'summary_live_session_binding_missing_count_zero' -Passed ([int](Get-JsonValue -Object $summary -Name 'LiveSessionBindingMissingCount' -DefaultValue -1) -eq 0) -Detail 'LiveSessionBindingMissingCount must be 0'
    Add-Check -Name 'summary_godot_log_before_missing_count_zero' -Passed ([int](Get-JsonValue -Object $summary -Name 'GodotLogBeforeMissingCount' -DefaultValue -1) -eq 0) -Detail 'GodotLogBeforeMissingCount must be 0'
    Add-Check -Name 'summary_current_iteration_log_missing_count_zero' -Passed ([int](Get-JsonValue -Object $summary -Name 'CurrentIterationLogMissingCount' -DefaultValue -1) -eq 0) -Detail 'CurrentIterationLogMissingCount must be 0'
    Add-Check -Name 'summary_unresponsive_iteration_count_zero' -Passed ([int](Get-JsonValue -Object $summary -Name 'UnresponsiveIterationCount' -DefaultValue -1) -eq 0) -Detail 'UnresponsiveIterationCount must be 0'
    Add-Check -Name 'summary_stale_process_observed_count_zero' -Passed ([int](Get-JsonValue -Object $summary -Name 'StaleProcessObservedCount' -DefaultValue -1) -eq 0) -Detail 'StaleProcessObservedCount must be 0 because pre-existing SlayTheSpire2 processes can contaminate the shared godot.log'
    Add-Check -Name 'summary_log_stall_iteration_count_zero' -Passed ([int](Get-JsonValue -Object $summary -Name 'LogStallIterationCount' -DefaultValue -1) -eq 0) -Detail 'LogStallIterationCount must be 0'
    Add-Check -Name 'summary_command_ack_missing_count_zero' -Passed ([int](Get-JsonValue -Object $summary -Name 'CommandAckMissingCount' -DefaultValue -1) -eq 0) -Detail 'CommandAckMissingCount must be 0'
    Add-Check -Name 'summary_max_consecutive_unresponsive_recorded' -Passed ([int](Get-JsonValue -Object $summary -Name 'MaxConsecutiveUnresponsiveSamples' -DefaultValue -1) -ge 0) -Detail 'MaxConsecutiveUnresponsiveSamples must be recorded'
    $resultScenarioTagCounts = Get-ValueCounts -Items $summaryResults -PropertyName 'ScenarioTag'
    $resultOwnerAreaCounts = Get-ValueCounts -Items $summaryResults -PropertyName 'OwnerArea'
    $resultCommandCounts = Get-ValueCounts -Items $summaryResults -PropertyName 'Command'
    Add-Check -Name 'summary_scenario_tag_counts_match_results' -Passed (Test-CountMapMatches -ActualCountMap (Get-JsonValue -Object $summary -Name 'ScenarioTagCounts' -DefaultValue $null) -ExpectedCounts $resultScenarioTagCounts) -Detail 'ScenarioTagCounts must match summary Results'
    Add-Check -Name 'summary_owner_area_counts_match_results' -Passed (Test-CountMapMatches -ActualCountMap (Get-JsonValue -Object $summary -Name 'OwnerAreaCounts' -DefaultValue $null) -ExpectedCounts $resultOwnerAreaCounts) -Detail 'OwnerAreaCounts must match summary Results'
    Add-Check -Name 'summary_command_counts_match_results' -Passed (Test-CountMapMatches -ActualCountMap (Get-JsonValue -Object $summary -Name 'CommandCounts' -DefaultValue $null) -ExpectedCounts $resultCommandCounts) -Detail 'CommandCounts must match summary Results'
    $summaryVakuuFightCount = @($summaryResults | Where-Object { [string](Get-JsonValue -Object $_ -Name 'ScenarioTag' -DefaultValue '') -eq 'vakuu-fight' }).Count
    Add-Check -Name 'summary_vakuu_fight_iteration_count_matches_results' -Passed ([int](Get-JsonValue -Object $summary -Name 'VakuuFightIterationCount' -DefaultValue -1) -eq $summaryVakuuFightCount) -Detail "VakuuFightIterationCount must match summary Results; expected $summaryVakuuFightCount"
    $failureReasonCountsValue = Get-JsonValue -Object $summary -Name 'FailureReasonCounts' -DefaultValue $null
    $failureReasonCountProperties = @()
    if ($null -ne $failureReasonCountsValue) {
        $failureReasonCountProperties = @($failureReasonCountsValue.PSObject.Properties)
    }

    Add-Check -Name 'summary_failure_reason_counts_empty' -Passed ($failureReasonCountProperties.Count -eq 0) -Detail 'FailureReasonCounts must be empty for a clean packet'

    if ($expectedIterationCount -gt 0) {
        Add-Check -Name 'summary_requested_iterations_match_expected' -Passed ($summaryRequestedIterations -eq $expectedIterationCount) -Detail "summary RequestedIterations expected $expectedIterationCount, found $summaryRequestedIterations"
        Add-Check -Name 'summary_completed_iterations_match_expected' -Passed ($summaryCompletedIterations -eq $expectedIterationCount) -Detail "summary CompletedIterations expected $expectedIterationCount, found $summaryCompletedIterations"
        Add-Check -Name 'summary_result_count_matches_expected' -Passed ($summaryResults.Count -eq $expectedIterationCount) -Detail "summary Results expected $expectedIterationCount entries, found $($summaryResults.Count)"
    }

    $failedSummaryResults = @($summaryResults | Where-Object { -not [bool](Get-JsonValue -Object $_ -Name 'Passed' -DefaultValue $false) })
    Add-Check -Name 'summary_results_all_passed' -Passed ($failedSummaryResults.Count -eq 0) -Detail "summary Results contains $($failedSummaryResults.Count) failed entries"
}

$plannedIterationAudit = Get-IterationNumberAudit -Items $planPlannedCommands -ExpectedCount $expectedIterationCount
Add-Check -Name 'plan_planned_iteration_numbers_unique' -Passed ($plannedIterationAudit.DuplicateNumbers.Count -eq 0 -and $plannedIterationAudit.NonPositiveNumbers.Count -eq 0) -Detail "PlannedCommands Iteration values must be positive and unique; duplicates=$($plannedIterationAudit.DuplicateNumbers -join ',') nonPositive=$($plannedIterationAudit.NonPositiveNumbers -join ',')"
if ($expectedIterationCount -gt 0) {
    Add-Check -Name 'plan_planned_iteration_numbers_cover_expected' -Passed ($plannedIterationAudit.MissingNumbers.Count -eq 0 -and $plannedIterationAudit.OutOfRangeNumbers.Count -eq 0) -Detail "PlannedCommands Iteration values must cover 1..$expectedIterationCount exactly once; missing=$($plannedIterationAudit.MissingNumbers -join ',') outOfRange=$($plannedIterationAudit.OutOfRangeNumbers -join ',')"
}

$summaryIterationAudit = Get-IterationNumberAudit -Items $summaryResults -ExpectedCount $expectedIterationCount
Add-Check -Name 'summary_result_iteration_numbers_unique' -Passed ($summaryIterationAudit.DuplicateNumbers.Count -eq 0 -and $summaryIterationAudit.NonPositiveNumbers.Count -eq 0) -Detail "monkey-summary.json Results Iteration values must be positive and unique; duplicates=$($summaryIterationAudit.DuplicateNumbers -join ',') nonPositive=$($summaryIterationAudit.NonPositiveNumbers -join ',')"
if ($expectedIterationCount -gt 0) {
    Add-Check -Name 'summary_result_iteration_numbers_cover_expected' -Passed ($summaryIterationAudit.MissingNumbers.Count -eq 0 -and $summaryIterationAudit.OutOfRangeNumbers.Count -eq 0) -Detail "monkey-summary.json Results Iteration values must cover 1..$expectedIterationCount exactly once; missing=$($summaryIterationAudit.MissingNumbers -join ',') outOfRange=$($summaryIterationAudit.OutOfRangeNumbers -join ',')"
}

$plannedByIteration = @{}
foreach ($plannedCommand in $planPlannedCommands) {
    $plannedIteration = [int](Get-JsonValue -Object $plannedCommand -Name 'Iteration' -DefaultValue 0)
    if ($plannedIteration -gt 0 -and -not $plannedByIteration.ContainsKey($plannedIteration)) {
        $plannedByIteration[$plannedIteration] = $plannedCommand
    }
}

$summaryResultByIteration = @{}
foreach ($summaryResult in $summaryResults) {
    $summaryIteration = [int](Get-JsonValue -Object $summaryResult -Name 'Iteration' -DefaultValue 0)
    if ($summaryIteration -gt 0 -and -not $summaryResultByIteration.ContainsKey($summaryIteration)) {
        $summaryResultByIteration[$summaryIteration] = $summaryResult
    }
}

$iterationDirectories = @(Get-ChildItem -LiteralPath $resolvedEvidenceDir -Directory -Filter 'iteration-*' | Sort-Object -Property Name)
if ($expectedIterationCount -gt 0) {
    Add-Check -Name 'iteration_directory_count_matches_expected' -Passed ($iterationDirectories.Count -eq $expectedIterationCount) -Detail "expected $expectedIterationCount iteration-* directories, found $($iterationDirectories.Count)"
} else {
    Add-Check -Name 'expected_iterations_resolved' -Passed $false -Detail 'could not resolve expected iteration count from -ExpectedIterations, monkey-plan.json, or monkey-summary.json'
}

for ($iteration = 1; $iteration -le $expectedIterationCount; $iteration++) {
    $iterationName = 'iteration-{0:D4}' -f $iteration
    $iterationDir = Resolve-EvidenceFile $iterationName
    $iterationDirExists = Test-Path -LiteralPath $iterationDir -PathType Container
    Add-Check -Name "${iterationName}_directory_exists" -Passed $iterationDirExists -Detail "requires $iterationName directory"

    if (-not $iterationDirExists) {
        continue
    }

    $plannedForIteration = $null
    if ($plannedByIteration.ContainsKey($iteration)) {
        $plannedForIteration = $plannedByIteration[$iteration]
    }

    $summaryForIteration = $null
    if ($summaryResultByIteration.ContainsKey($iteration)) {
        $summaryForIteration = $summaryResultByIteration[$iteration]
    }

    $resultPath = Join-Path $iterationDir 'iteration-result.json'
    $prepareOutputPath = Join-Path $iterationDir 'prepare-output.json'
    $sessionStatePath = Join-Path $iterationDir 'session-state.json'
    $restoreStatePath = Join-Path $iterationDir 'restore-state.json'
    $beforeLogPath = Join-Path $iterationDir 'godot.log.before'
    $logPath = Join-Path $iterationDir 'godot.log.after-launch'
    $currentIterationLogPath = Join-Path $iterationDir 'godot.log.current-iteration'
    $auditPath = Join-Path $iterationDir 'godot-log-audit.json'
    $probeSamplesCandidate = Join-Path $iterationDir 'runtime-probe-samples.json'
    $sts1ModeCheckPath = Join-Path $iterationDir 'sts1-mode-log-check.json'
    $resultExists = Test-Path -LiteralPath $resultPath -PathType Leaf
    $prepareOutputExists = Test-Path -LiteralPath $prepareOutputPath -PathType Leaf
    $sessionStateExists = Test-Path -LiteralPath $sessionStatePath -PathType Leaf
    $restoreStateExists = Test-Path -LiteralPath $restoreStatePath -PathType Leaf
    $beforeLogExists = Test-Path -LiteralPath $beforeLogPath -PathType Leaf
    $logExists = Test-Path -LiteralPath $logPath -PathType Leaf
    $currentIterationLogExists = Test-Path -LiteralPath $currentIterationLogPath -PathType Leaf
    $auditExists = Test-Path -LiteralPath $auditPath -PathType Leaf
    $sts1ModeCheckExists = Test-Path -LiteralPath $sts1ModeCheckPath -PathType Leaf

    Add-Check -Name "${iterationName}_iteration_result_exists" -Passed $resultExists -Detail 'requires iteration-result.json'
    Add-Check -Name "${iterationName}_prepare_output_exists" -Passed $prepareOutputExists -Detail 'requires retained prepare-output.json from the launched prepare phase'
    Add-Check -Name "${iterationName}_session_state_exists" -Passed $sessionStateExists -Detail 'requires retained session-state.json from live-session prepare'
    Add-Check -Name "${iterationName}_restore_state_exists" -Passed $restoreStateExists -Detail 'requires retained restore-state.json from live-session restore'
    Add-Check -Name "${iterationName}_godot_log_before_exists" -Passed $beforeLogExists -Detail 'requires retained godot.log.before pre-launch snapshot'
    Add-Check -Name "${iterationName}_godot_log_exists" -Passed $logExists -Detail 'requires godot.log.after-launch'
    Add-Check -Name "${iterationName}_current_iteration_log_exists" -Passed $currentIterationLogExists -Detail 'requires godot.log.current-iteration sliced from the accepted scan offset'
    Add-Check -Name "${iterationName}_audit_json_exists" -Passed $auditExists -Detail 'requires godot-log-audit.json'
    Add-Check -Name "${iterationName}_sts1_mode_log_check_exists" -Passed $sts1ModeCheckExists -Detail 'requires retained sts1-mode-log-check.json'
    Add-Check -Name "${iterationName}_plan_entry_exists" -Passed ($null -ne $plannedForIteration) -Detail 'monkey-plan.json must include a PlannedCommands row for this iteration'
    Add-Check -Name "${iterationName}_summary_result_exists" -Passed ($null -ne $summaryForIteration) -Detail 'monkey-summary.json Results must include a row for this iteration'

    $iterationResult = $null
    $prepareOutput = $null
    $sessionState = $null
    $restoreState = $null
    if ($prepareOutputExists) {
        $prepareOutput = Read-JsonOrNull -Path $prepareOutputPath -CheckName "${iterationName}_prepare_output_json_valid"
        if ($null -ne $prepareOutput) {
            Add-Check -Name "${iterationName}_prepare_output_json_valid" -Passed $true -Detail 'prepare-output.json parsed'
        }
    }
    if ($sessionStateExists) {
        $sessionState = Read-JsonOrNull -Path $sessionStatePath -CheckName "${iterationName}_session_state_json_valid"
        if ($null -ne $sessionState) {
            Add-Check -Name "${iterationName}_session_state_json_valid" -Passed $true -Detail 'session-state.json parsed'
        }
    }
    if ($restoreStateExists) {
        $restoreState = Read-JsonOrNull -Path $restoreStatePath -CheckName "${iterationName}_restore_state_json_valid"
        if ($null -ne $restoreState) {
            Add-Check -Name "${iterationName}_restore_state_json_valid" -Passed $true -Detail 'restore-state.json parsed'
        }
    }
    if ($resultExists) {
        $iterationResult = Read-JsonOrNull -Path $resultPath -CheckName "${iterationName}_iteration_result_json_valid"
        if ($null -ne $iterationResult) {
            Add-Check -Name "${iterationName}_iteration_result_json_valid" -Passed $true -Detail 'iteration-result.json parsed'
            $resultIterationNumber = [int](Get-JsonValue -Object $iterationResult -Name 'Iteration' -DefaultValue 0)
            Add-Check -Name "${iterationName}_iteration_number_matches_directory" -Passed ($resultIterationNumber -eq $iteration) -Detail "iteration-result.json Iteration must match directory $iterationName; found $resultIterationNumber"
            Add-Check -Name "${iterationName}_hang_probe_schema_version" -Passed ([int](Get-JsonValue -Object $iterationResult -Name 'HangProbeSchemaVersion' -DefaultValue 0) -eq 1) -Detail 'iteration HangProbeSchemaVersion must be 1'
            Add-Check -Name "${iterationName}_scenario_present" -Passed (-not [string]::IsNullOrWhiteSpace([string](Get-JsonValue -Object $iterationResult -Name 'Scenario' -DefaultValue ''))) -Detail 'Scenario must be retained for packet binding'
            Add-Check -Name "${iterationName}_scenario_tag_present" -Passed (-not [string]::IsNullOrWhiteSpace([string](Get-JsonValue -Object $iterationResult -Name 'ScenarioTag' -DefaultValue ''))) -Detail 'ScenarioTag must be retained for triage'
            Add-Check -Name "${iterationName}_owner_area_present" -Passed (-not [string]::IsNullOrWhiteSpace([string](Get-JsonValue -Object $iterationResult -Name 'OwnerArea' -DefaultValue ''))) -Detail 'OwnerArea must be retained for triage'
            Add-Check -Name "${iterationName}_command_selection_mode_present" -Passed (-not [string]::IsNullOrWhiteSpace([string](Get-JsonValue -Object $iterationResult -Name 'CommandSelectionMode' -DefaultValue ''))) -Detail 'CommandSelectionMode must be retained for triage'
            Add-Check -Name "${iterationName}_main_menu_reached" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'MainMenuReached' -DefaultValue $false)) -Detail 'MainMenuReached must be true; false means main-menu timeout or launch failure'
            Add-Check -Name "${iterationName}_main_menu_observation_passed" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'MainMenuObservationPassed' -DefaultValue $false)) -Detail 'MainMenuObservationPassed must be true'
            Add-Check -Name "${iterationName}_runtime_observation_passed" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'RuntimeObservationPassed' -DefaultValue $false)) -Detail 'RuntimeObservationPassed must be true'
            Add-Check -Name "${iterationName}_startup_log_probe_passed" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'StartupLogProbePassed' -DefaultValue $false)) -Detail 'StartupLogProbePassed must be true'
            Add-Check -Name "${iterationName}_post_command_log_probe_passed" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'PostCommandLogProbePassed' -DefaultValue $false)) -Detail 'PostCommandLogProbePassed must be true'
            Add-Check -Name "${iterationName}_responsiveness_probe_passed" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'ResponsivenessProbePassed' -DefaultValue $false)) -Detail 'ResponsivenessProbePassed must be true'
            $resultCommandForRuntimeObservation = [string](Get-JsonValue -Object $iterationResult -Name 'Command' -DefaultValue '')
            $resultCommandAckRequiredForRuntimeObservation = [bool](Get-JsonValue -Object $iterationResult -Name 'CommandAckRequired' -DefaultValue $false)
            $runtimeLogGrowthRequiredForIteration = -not [string]::IsNullOrWhiteSpace($resultCommandForRuntimeObservation) -or $resultCommandAckRequiredForRuntimeObservation
            Add-Check -Name "${iterationName}_command_ack_observed" -Passed ((-not $resultCommandAckRequiredForRuntimeObservation) -or [bool](Get-JsonValue -Object $iterationResult -Name 'CommandAckObserved' -DefaultValue $false)) -Detail 'required command acknowledgement must be observed when applicable'
            $failureReasonCodesCount = Get-ArrayCount -Value (Get-JsonValue -Object $iterationResult -Name 'FailureReasonCodes' -DefaultValue @())
            $hangSignalsCount = Get-ArrayCount -Value (Get-JsonValue -Object $iterationResult -Name 'HangSignals' -DefaultValue @())
            Add-Check -Name "${iterationName}_failure_reason_codes_empty" -Passed ($failureReasonCodesCount -eq 0) -Detail 'FailureReasonCodes must be empty for a clean packet'
            Add-Check -Name "${iterationName}_hang_signals_empty" -Passed ($hangSignalsCount -eq 0) -Detail 'HangSignals must be empty for a clean packet'
            Add-Check -Name "${iterationName}_game_process_id_positive" -Passed ([int](Get-JsonValue -Object $iterationResult -Name 'GameProcessId' -DefaultValue 0) -gt 0) -Detail 'GameProcessId must identify SlayTheSpire2'
            $iterationGameProcessId = [int](Get-JsonValue -Object $iterationResult -Name 'GameProcessId' -DefaultValue 0)
            Add-Check -Name "${iterationName}_main_window_observed" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'MainWindowObserved' -DefaultValue $false)) -Detail 'MainWindowObserved must be true'
            Add-Check -Name "${iterationName}_main_menu_elapsed_recorded" -Passed ([double](Get-JsonValue -Object $iterationResult -Name 'MainMenuElapsedSeconds' -DefaultValue 0) -gt 0) -Detail 'MainMenuElapsedSeconds must be positive'
            Add-Check -Name "${iterationName}_max_no_log_growth_recorded" -Passed ([int](Get-JsonValue -Object $iterationResult -Name 'MaxSecondsWithoutLogGrowth' -DefaultValue -1) -ge 0) -Detail 'MaxSecondsWithoutLogGrowth must be recorded'
            $iterationMaxUnresponsive = [int](Get-JsonValue -Object $iterationResult -Name 'MaxConsecutiveUnresponsiveSamples' -DefaultValue -1)
            Add-Check -Name "${iterationName}_max_consecutive_unresponsive_recorded" -Passed ($iterationMaxUnresponsive -ge 0) -Detail 'MaxConsecutiveUnresponsiveSamples must be recorded'
            if ($planUnresponsiveSampleThreshold -gt 0) {
                Add-Check -Name "${iterationName}_max_consecutive_unresponsive_below_threshold" -Passed ($iterationMaxUnresponsive -lt $planUnresponsiveSampleThreshold) -Detail "MaxConsecutiveUnresponsiveSamples must stay below threshold $planUnresponsiveSampleThreshold; found $iterationMaxUnresponsive"
            }
            $iterationStaleProcessCount = [int](Get-JsonValue -Object $iterationResult -Name 'StaleProcessCount' -DefaultValue -1)
            Add-Check -Name "${iterationName}_stale_process_observed_false" -Passed (-not [bool](Get-JsonValue -Object $iterationResult -Name 'StaleProcessObserved' -DefaultValue $true)) -Detail 'StaleProcessObserved must be false; stale pre-existing processes can contaminate shared godot.log evidence'
            Add-Check -Name "${iterationName}_stale_process_count_zero" -Passed ($iterationStaleProcessCount -eq 0) -Detail "StaleProcessCount must be recorded as 0; found $iterationStaleProcessCount"

            $resultPrepareOutputPath = Resolve-ChildOrAbsolutePath -BaseDir $iterationDir -Path ([string](Get-JsonValue -Object $iterationResult -Name 'LiveSessionPrepareOutputPath' -DefaultValue ''))
            $resultPrepareOutputSha256 = [string](Get-JsonValue -Object $iterationResult -Name 'LiveSessionPrepareOutputSha256' -DefaultValue '')
            $resultSessionStatePath = Resolve-ChildOrAbsolutePath -BaseDir $iterationDir -Path ([string](Get-JsonValue -Object $iterationResult -Name 'LiveSessionSessionStatePath' -DefaultValue ''))
            $resultSessionStateSha256 = [string](Get-JsonValue -Object $iterationResult -Name 'LiveSessionSessionStateSha256' -DefaultValue '')
            $resultRestoreStatePath = Resolve-ChildOrAbsolutePath -BaseDir $iterationDir -Path ([string](Get-JsonValue -Object $iterationResult -Name 'LiveSessionRestoreStatePath' -DefaultValue ''))
            $resultRestoreStateSha256 = [string](Get-JsonValue -Object $iterationResult -Name 'LiveSessionRestoreStateSha256' -DefaultValue '')
            $resultLiveSessionEvidenceDir = Resolve-ChildOrAbsolutePath -BaseDir $iterationDir -Path ([string](Get-JsonValue -Object $iterationResult -Name 'LiveSessionEvidenceDir' -DefaultValue ''))
            $resultLiveSessionLauncherKind = [string](Get-JsonValue -Object $iterationResult -Name 'LiveSessionLauncherKind' -DefaultValue '')
            $resultLiveSessionSteamAppId = [string](Get-JsonValue -Object $iterationResult -Name 'LiveSessionSteamAppId' -DefaultValue '')
            $resultLiveSessionLaunchFilePath = [string](Get-JsonValue -Object $iterationResult -Name 'LiveSessionLaunchFilePath' -DefaultValue '')
            $resultLiveSessionLaunchArgumentList = @(ConvertTo-StringArray -Value (Get-JsonValue -Object $iterationResult -Name 'LiveSessionLaunchArgumentList' -DefaultValue @()))
            $resultLiveSessionLaunchedProcessId = [int](Get-JsonValue -Object $iterationResult -Name 'LiveSessionLaunchedProcessId' -DefaultValue 0)
            $resultLiveSessionLaunchedAt = Get-JsonValue -Object $iterationResult -Name 'LiveSessionLaunchedAt' -DefaultValue $null
            $resultLiveSessionLaunchReturnedAt = Get-JsonValue -Object $iterationResult -Name 'LiveSessionLaunchReturnedAt' -DefaultValue $null
            $resultLiveSessionLaunchedAtUtc = ConvertTo-DateTimeUtcOrNull -Value $resultLiveSessionLaunchedAt
            $resultLiveSessionLaunchReturnedAtUtc = ConvertTo-DateTimeUtcOrNull -Value $resultLiveSessionLaunchReturnedAt
            $resultLiveSessionPidProbeStartedAtUtc = ConvertTo-DateTimeUtcOrNull -Value (Get-JsonValue -Object $iterationResult -Name 'LiveSessionPidProbeStartedAtUtc' -DefaultValue $null)
            $resultLiveSessionPidProbeFinishedAtUtc = ConvertTo-DateTimeUtcOrNull -Value (Get-JsonValue -Object $iterationResult -Name 'LiveSessionPidProbeFinishedAtUtc' -DefaultValue $null)
            $resultLiveSessionSelectedGameProcessId = [int](Get-JsonValue -Object $iterationResult -Name 'LiveSessionSelectedGameProcessId' -DefaultValue 0)
            $resultLiveSessionSelectedGameProcessStartTimeUtc = ConvertTo-DateTimeUtcOrNull -Value (Get-JsonValue -Object $iterationResult -Name 'LiveSessionSelectedGameProcessStartTimeUtc' -DefaultValue $null)
            $resultLiveSessionSelectedGameProcessStartTimeText = if ($null -ne $resultLiveSessionSelectedGameProcessStartTimeUtc) { $resultLiveSessionSelectedGameProcessStartTimeUtc.ToString('o') } else { '' }
            $resultLiveSessionSelectedGameProcessPath = [string](Get-JsonValue -Object $iterationResult -Name 'LiveSessionSelectedGameProcessPath' -DefaultValue '')
            $resultGameProcessStartTimeUtc = ConvertTo-DateTimeUtcOrNull -Value (Get-JsonValue -Object $iterationResult -Name 'GameProcessStartTimeUtc' -DefaultValue $null)
            $resultGameProcessStartTimeText = if ($null -ne $resultGameProcessStartTimeUtc) { $resultGameProcessStartTimeUtc.ToString('o') } else { '' }
            $resultGameProcessPath = [string](Get-JsonValue -Object $iterationResult -Name 'GameProcessPath' -DefaultValue '')
            $resultLiveSessionLaunchFilePathFull = ConvertTo-NormalizedPathOrEmpty -Path $resultLiveSessionLaunchFilePath
            $resultLiveSessionSelectedGameProcessPathFull = ConvertTo-NormalizedPathOrEmpty -Path $resultLiveSessionSelectedGameProcessPath
            $resultGameProcessPathFull = ConvertTo-NormalizedPathOrEmpty -Path $resultGameProcessPath
            $expectedSteamLaunchArgumentList = @('-applaunch', '2868840')

            Add-Check -Name "${iterationName}_live_session_prepare_output_under_iteration_dir" -Passed ($resultPrepareOutputPath -and (Test-PathUnderDirectory -Path $resultPrepareOutputPath -Directory $iterationDir)) -Detail 'LiveSessionPrepareOutputPath must stay inside the current iteration directory'
            Add-Check -Name "${iterationName}_live_session_prepare_output_leaf_expected" -Passed ($resultPrepareOutputPath -and ([System.IO.Path]::GetFileName($resultPrepareOutputPath) -eq 'prepare-output.json')) -Detail 'LiveSessionPrepareOutputPath must end with prepare-output.json'
            Add-Check -Name "${iterationName}_live_session_prepare_output_path_matches_retained_file" -Passed ($resultPrepareOutputPath -and ([System.StringComparer]::OrdinalIgnoreCase.Equals($resultPrepareOutputPath, [System.IO.Path]::GetFullPath($prepareOutputPath)))) -Detail 'LiveSessionPrepareOutputPath must point to the retained prepare-output.json file'
            Add-Check -Name "${iterationName}_live_session_prepare_output_sha256_recorded" -Passed (-not [string]::IsNullOrWhiteSpace($resultPrepareOutputSha256)) -Detail 'LiveSessionPrepareOutputSha256 must be retained'
            if ($prepareOutputExists) {
                Add-Check -Name "${iterationName}_live_session_prepare_output_sha256_matches_retained_file" -Passed ([System.StringComparer]::OrdinalIgnoreCase.Equals($resultPrepareOutputSha256, (Get-FileSha256OrEmpty -Path $prepareOutputPath))) -Detail 'LiveSessionPrepareOutputSha256 must match retained prepare-output.json'
            }

            Add-Check -Name "${iterationName}_live_session_session_state_under_iteration_dir" -Passed ($resultSessionStatePath -and (Test-PathUnderDirectory -Path $resultSessionStatePath -Directory $iterationDir)) -Detail 'LiveSessionSessionStatePath must stay inside the current iteration directory'
            Add-Check -Name "${iterationName}_live_session_session_state_leaf_expected" -Passed ($resultSessionStatePath -and ([System.IO.Path]::GetFileName($resultSessionStatePath) -eq 'session-state.json')) -Detail 'LiveSessionSessionStatePath must end with session-state.json'
            Add-Check -Name "${iterationName}_live_session_session_state_path_matches_retained_file" -Passed ($resultSessionStatePath -and ([System.StringComparer]::OrdinalIgnoreCase.Equals($resultSessionStatePath, [System.IO.Path]::GetFullPath($sessionStatePath)))) -Detail 'LiveSessionSessionStatePath must point to the retained session-state.json file'
            Add-Check -Name "${iterationName}_live_session_session_state_sha256_recorded" -Passed (-not [string]::IsNullOrWhiteSpace($resultSessionStateSha256)) -Detail 'LiveSessionSessionStateSha256 must be retained'
            if ($sessionStateExists) {
                Add-Check -Name "${iterationName}_live_session_session_state_sha256_matches_retained_file" -Passed ([System.StringComparer]::OrdinalIgnoreCase.Equals($resultSessionStateSha256, (Get-FileSha256OrEmpty -Path $sessionStatePath))) -Detail 'LiveSessionSessionStateSha256 must match retained session-state.json'
            }

            Add-Check -Name "${iterationName}_live_session_restore_state_under_iteration_dir" -Passed ($resultRestoreStatePath -and (Test-PathUnderDirectory -Path $resultRestoreStatePath -Directory $iterationDir)) -Detail 'LiveSessionRestoreStatePath must stay inside the current iteration directory'
            Add-Check -Name "${iterationName}_live_session_restore_state_leaf_expected" -Passed ($resultRestoreStatePath -and ([System.IO.Path]::GetFileName($resultRestoreStatePath) -eq 'restore-state.json')) -Detail 'LiveSessionRestoreStatePath must end with restore-state.json'
            Add-Check -Name "${iterationName}_live_session_restore_state_path_matches_retained_file" -Passed ($resultRestoreStatePath -and ([System.StringComparer]::OrdinalIgnoreCase.Equals($resultRestoreStatePath, [System.IO.Path]::GetFullPath($restoreStatePath)))) -Detail 'LiveSessionRestoreStatePath must point to the retained restore-state.json file'
            Add-Check -Name "${iterationName}_live_session_restore_state_sha256_recorded" -Passed (-not [string]::IsNullOrWhiteSpace($resultRestoreStateSha256)) -Detail 'LiveSessionRestoreStateSha256 must be retained'
            if ($restoreStateExists) {
                Add-Check -Name "${iterationName}_live_session_restore_state_sha256_matches_retained_file" -Passed ([System.StringComparer]::OrdinalIgnoreCase.Equals($resultRestoreStateSha256, (Get-FileSha256OrEmpty -Path $restoreStatePath))) -Detail 'LiveSessionRestoreStateSha256 must match retained restore-state.json'
            }

            Add-Check -Name "${iterationName}_result_live_session_evidence_dir_matches_iteration" -Passed ($resultLiveSessionEvidenceDir -and ([System.StringComparer]::OrdinalIgnoreCase.Equals($resultLiveSessionEvidenceDir, [System.IO.Path]::GetFullPath($iterationDir)))) -Detail 'LiveSessionEvidenceDir must match the current iteration directory'
            Add-Check -Name "${iterationName}_result_live_session_launcher_kind_steam_app_launch" -Passed ([string]::Equals($resultLiveSessionLauncherKind, 'SteamAppLaunch', [System.StringComparison]::Ordinal)) -Detail 'LiveSessionLauncherKind must prove Steam -applaunch startup'
            Add-Check -Name "${iterationName}_result_live_session_steam_app_id_matches_sts2" -Passed ([string]::Equals($resultLiveSessionSteamAppId, '2868840', [System.StringComparison]::Ordinal)) -Detail 'LiveSessionSteamAppId must be Slay the Spire 2 Steam app id 2868840'
            Add-Check -Name "${iterationName}_result_live_session_launch_file_path_recorded" -Passed (-not [string]::IsNullOrWhiteSpace($resultLiveSessionLaunchFilePathFull)) -Detail 'LiveSessionLaunchFilePath must be retained'
            Add-Check -Name "${iterationName}_result_live_session_launch_argument_list_matches_sts2" -Passed (Test-StringArrayEquals -Actual $resultLiveSessionLaunchArgumentList -Expected $expectedSteamLaunchArgumentList) -Detail 'LiveSessionLaunchArgumentList must be -applaunch 2868840'
            Add-Check -Name "${iterationName}_result_live_session_launched_process_id_positive" -Passed ($resultLiveSessionLaunchedProcessId -gt 0) -Detail 'LiveSessionLaunchedProcessId must retain the Steam launch process id'
            Add-Check -Name "${iterationName}_result_live_session_launched_at_parseable" -Passed ($null -ne $resultLiveSessionLaunchedAtUtc) -Detail 'LiveSessionLaunchedAt must be parseable'
            Add-Check -Name "${iterationName}_result_live_session_launch_returned_at_parseable" -Passed ($null -ne $resultLiveSessionLaunchReturnedAtUtc) -Detail 'LiveSessionLaunchReturnedAt must be parseable'
            if ($null -ne $resultLiveSessionLaunchedAtUtc -and $null -ne $resultLiveSessionLaunchReturnedAtUtc) {
                Add-Check -Name "${iterationName}_result_live_session_launch_returned_after_launched_at" -Passed ($resultLiveSessionLaunchReturnedAtUtc -ge $resultLiveSessionLaunchedAtUtc) -Detail 'LiveSessionLaunchReturnedAt must be at or after LiveSessionLaunchedAt'
            }

            Add-Check -Name "${iterationName}_result_live_session_pid_attribution_schema_version_positive" -Passed ([int](Get-JsonValue -Object $iterationResult -Name 'LiveSessionPidAttributionSchemaVersion' -DefaultValue 0) -gt 0) -Detail 'LiveSessionPidAttributionSchemaVersion must be retained'
            Add-Check -Name "${iterationName}_result_live_session_pid_attribution_passed" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'LiveSessionPidAttributionPassed' -DefaultValue $false)) -Detail 'LiveSessionPidAttributionPassed must be true'
            Add-Check -Name "${iterationName}_result_live_session_pid_attribution_method_present" -Passed (-not [string]::IsNullOrWhiteSpace([string](Get-JsonValue -Object $iterationResult -Name 'LiveSessionPidAttributionMethod' -DefaultValue ''))) -Detail 'LiveSessionPidAttributionMethod must describe how the game process was selected'
            Add-Check -Name "${iterationName}_result_live_session_pid_probe_started_at_parseable" -Passed ($null -ne $resultLiveSessionPidProbeStartedAtUtc) -Detail 'LiveSessionPidProbeStartedAtUtc must be parseable'
            Add-Check -Name "${iterationName}_result_live_session_pid_probe_finished_at_parseable" -Passed ($null -ne $resultLiveSessionPidProbeFinishedAtUtc) -Detail 'LiveSessionPidProbeFinishedAtUtc must be parseable'
            if ($null -ne $resultLiveSessionPidProbeStartedAtUtc -and $null -ne $resultLiveSessionPidProbeFinishedAtUtc) {
                Add-Check -Name "${iterationName}_result_live_session_pid_probe_finished_after_started" -Passed ($resultLiveSessionPidProbeFinishedAtUtc -ge $resultLiveSessionPidProbeStartedAtUtc) -Detail 'LiveSession PID probe finish time must be at or after start time'
            }
            Add-Check -Name "${iterationName}_result_live_session_prelaunch_slay_process_count_zero" -Passed ([int](Get-JsonValue -Object $iterationResult -Name 'LiveSessionPreLaunchSlayProcessCount' -DefaultValue -1) -eq 0) -Detail 'LiveSessionPreLaunchSlayProcessCount must be 0 so prior game processes cannot contaminate shared godot.log evidence'
            Add-Check -Name "${iterationName}_result_live_session_prelaunch_slay_process_ids_empty" -Passed ((Get-ArrayCount -Value (Get-JsonValue -Object $iterationResult -Name 'LiveSessionPreLaunchSlayProcessIds' -DefaultValue @())) -eq 0) -Detail 'LiveSessionPreLaunchSlayProcessIds must be empty'
            Add-Check -Name "${iterationName}_result_live_session_selected_game_process_id_matches_result" -Passed ($resultLiveSessionSelectedGameProcessId -gt 0 -and $resultLiveSessionSelectedGameProcessId -eq $iterationGameProcessId) -Detail 'LiveSessionSelectedGameProcessId must match iteration-result GameProcessId'
            Add-Check -Name "${iterationName}_result_live_session_selected_game_process_start_time_parseable" -Passed ($null -ne $resultLiveSessionSelectedGameProcessStartTimeUtc) -Detail 'LiveSessionSelectedGameProcessStartTimeUtc must be parseable'
            Add-Check -Name "${iterationName}_result_live_session_selected_game_process_path_present" -Passed (-not [string]::IsNullOrWhiteSpace($resultLiveSessionSelectedGameProcessPathFull)) -Detail 'LiveSessionSelectedGameProcessPath must be retained'
            Add-Check -Name "${iterationName}_result_live_session_attribution_failure_reason_empty" -Passed ([string]::IsNullOrWhiteSpace([string](Get-JsonValue -Object $iterationResult -Name 'LiveSessionAttributionFailureReason' -DefaultValue ''))) -Detail 'LiveSessionAttributionFailureReason must be empty when attribution passed'
            Add-Check -Name "${iterationName}_result_game_process_start_time_parseable" -Passed ($null -ne $resultGameProcessStartTimeUtc) -Detail 'GameProcessStartTimeUtc must be parseable'
            Add-Check -Name "${iterationName}_result_game_process_path_present" -Passed (-not [string]::IsNullOrWhiteSpace($resultGameProcessPathFull)) -Detail 'GameProcessPath must be retained'
            Add-Check -Name "${iterationName}_result_game_process_id_matches_live_session" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'GameProcessIdMatchesLiveSession' -DefaultValue $false)) -Detail 'GameProcessIdMatchesLiveSession must be true'
            Add-Check -Name "${iterationName}_result_game_process_start_time_matches_live_session" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'GameProcessStartTimeMatchesLiveSession' -DefaultValue $false)) -Detail 'GameProcessStartTimeMatchesLiveSession must be true'
            Add-Check -Name "${iterationName}_result_game_process_path_matches_live_session" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'GameProcessPathMatchesLiveSession' -DefaultValue $false)) -Detail 'GameProcessPathMatchesLiveSession must be true'
            Add-Check -Name "${iterationName}_result_game_process_start_time_matches_selected_live_session" -Passed ($null -ne $resultGameProcessStartTimeUtc -and $null -ne $resultLiveSessionSelectedGameProcessStartTimeUtc -and $resultGameProcessStartTimeUtc -eq $resultLiveSessionSelectedGameProcessStartTimeUtc) -Detail 'GameProcessStartTimeUtc must match LiveSessionSelectedGameProcessStartTimeUtc'
            Add-Check -Name "${iterationName}_result_game_process_path_matches_selected_live_session" -Passed (-not [string]::IsNullOrWhiteSpace($resultGameProcessPathFull) -and [System.StringComparer]::OrdinalIgnoreCase.Equals($resultGameProcessPathFull, $resultLiveSessionSelectedGameProcessPathFull)) -Detail 'GameProcessPath must match LiveSessionSelectedGameProcessPath'
            $gameProcessStartAfterLiveSessionLaunch = $null -ne $resultGameProcessStartTimeUtc -and $null -ne $resultLiveSessionLaunchedAtUtc -and $resultGameProcessStartTimeUtc -ge $resultLiveSessionLaunchedAtUtc
            Add-Check -Name "${iterationName}_result_game_process_start_time_after_live_session_launch" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'GameProcessStartTimeAfterLiveSessionLaunch' -DefaultValue $false) -and $gameProcessStartAfterLiveSessionLaunch) -Detail 'GameProcessStartTimeUtc must be at or after LiveSessionLaunchedAt'

            if ($null -ne $prepareOutput) {
                $prepareOutputEvidenceDir = Resolve-ChildOrAbsolutePath -BaseDir $iterationDir -Path ([string](Get-JsonValue -Object $prepareOutput -Name 'EvidenceDir' -DefaultValue ''))
                $prepareLaunchFilePathFull = ConvertTo-NormalizedPathOrEmpty -Path ([string](Get-JsonValue -Object $prepareOutput -Name 'LaunchFilePath' -DefaultValue ''))
                $prepareLaunchArgumentList = @(ConvertTo-StringArray -Value (Get-JsonValue -Object $prepareOutput -Name 'LaunchArgumentList' -DefaultValue @()))
                $prepareLaunchedAtUtc = ConvertTo-DateTimeUtcOrNull -Value (Get-JsonValue -Object $prepareOutput -Name 'LaunchedAt' -DefaultValue $null)
                $prepareLaunchReturnedAtUtc = ConvertTo-DateTimeUtcOrNull -Value (Get-JsonValue -Object $prepareOutput -Name 'LaunchReturnedAt' -DefaultValue $null)
                $preparePidProbeStartedAtUtc = ConvertTo-DateTimeUtcOrNull -Value (Get-JsonValue -Object $prepareOutput -Name 'PidProbeStartedAtUtc' -DefaultValue $null)
                $preparePidProbeFinishedAtUtc = ConvertTo-DateTimeUtcOrNull -Value (Get-JsonValue -Object $prepareOutput -Name 'PidProbeFinishedAtUtc' -DefaultValue $null)
                $prepareSelectedGameProcessStartTimeUtc = ConvertTo-DateTimeUtcOrNull -Value (Get-JsonValue -Object $prepareOutput -Name 'SelectedGameProcessStartTimeUtc' -DefaultValue $null)
                $prepareSelectedGameProcessPathFull = ConvertTo-NormalizedPathOrEmpty -Path ([string](Get-JsonValue -Object $prepareOutput -Name 'SelectedGameProcessPath' -DefaultValue ''))
                Add-Check -Name "${iterationName}_prepare_output_evidence_dir_matches_iteration" -Passed ($prepareOutputEvidenceDir -and [System.StringComparer]::OrdinalIgnoreCase.Equals($prepareOutputEvidenceDir, [System.IO.Path]::GetFullPath($iterationDir))) -Detail 'prepare-output.json EvidenceDir must match the current iteration directory'
                Add-Check -Name "${iterationName}_prepare_output_launch_kind_steam_app_launch" -Passed ([string]::Equals([string](Get-JsonValue -Object $prepareOutput -Name 'LaunchKind' -DefaultValue ''), 'SteamAppLaunch', [System.StringComparison]::Ordinal)) -Detail 'prepare-output.json LaunchKind must prove Steam launch'
                Add-Check -Name "${iterationName}_prepare_output_steam_app_id_matches_sts2" -Passed ([string]::Equals([string](Get-JsonValue -Object $prepareOutput -Name 'SteamAppId' -DefaultValue ''), '2868840', [System.StringComparison]::Ordinal)) -Detail 'prepare-output.json SteamAppId must be Slay the Spire 2'
                Add-Check -Name "${iterationName}_prepare_output_launch_file_path_matches_result" -Passed (-not [string]::IsNullOrWhiteSpace($prepareLaunchFilePathFull) -and [System.StringComparer]::OrdinalIgnoreCase.Equals($prepareLaunchFilePathFull, $resultLiveSessionLaunchFilePathFull)) -Detail 'prepare-output.json LaunchFilePath must match iteration-result LiveSessionLaunchFilePath'
                Add-Check -Name "${iterationName}_prepare_output_launch_argument_list_matches_sts2" -Passed (Test-StringArrayEquals -Actual $prepareLaunchArgumentList -Expected $expectedSteamLaunchArgumentList) -Detail 'prepare-output.json LaunchArgumentList must be -applaunch 2868840'
                Add-Check -Name "${iterationName}_prepare_output_launch_argument_list_matches_result" -Passed (Test-StringArrayEquals -Actual $prepareLaunchArgumentList -Expected $resultLiveSessionLaunchArgumentList) -Detail 'prepare-output.json LaunchArgumentList must match iteration-result LiveSessionLaunchArgumentList'
                Add-Check -Name "${iterationName}_prepare_output_launched_process_id_positive" -Passed ([int](Get-JsonValue -Object $prepareOutput -Name 'LaunchedProcessId' -DefaultValue 0) -gt 0) -Detail 'prepare-output.json must retain the Steam launch process id'
                Add-Check -Name "${iterationName}_prepare_output_launched_process_id_matches_result" -Passed ([int](Get-JsonValue -Object $prepareOutput -Name 'LaunchedProcessId' -DefaultValue 0) -eq $resultLiveSessionLaunchedProcessId) -Detail 'prepare-output.json LaunchedProcessId must match iteration-result LiveSessionLaunchedProcessId'
                Add-Check -Name "${iterationName}_prepare_output_launched_at_parseable" -Passed ($null -ne $prepareLaunchedAtUtc) -Detail 'prepare-output.json LaunchedAt must be parseable'
                Add-Check -Name "${iterationName}_prepare_output_launched_at_matches_result" -Passed ($null -ne $prepareLaunchedAtUtc -and $null -ne $resultLiveSessionLaunchedAtUtc -and $prepareLaunchedAtUtc -eq $resultLiveSessionLaunchedAtUtc) -Detail 'prepare-output.json LaunchedAt must match iteration-result LiveSessionLaunchedAt'
                Add-Check -Name "${iterationName}_prepare_output_launch_returned_at_parseable" -Passed ($null -ne $prepareLaunchReturnedAtUtc) -Detail 'prepare-output.json LaunchReturnedAt must be parseable'
                Add-Check -Name "${iterationName}_prepare_output_launch_returned_at_matches_result" -Passed ($null -ne $prepareLaunchReturnedAtUtc -and $null -ne $resultLiveSessionLaunchReturnedAtUtc -and $prepareLaunchReturnedAtUtc -eq $resultLiveSessionLaunchReturnedAtUtc) -Detail 'prepare-output.json LaunchReturnedAt must match iteration-result LiveSessionLaunchReturnedAt'
                Add-Check -Name "${iterationName}_prepare_output_pid_attribution_schema_version_positive" -Passed ([int](Get-JsonValue -Object $prepareOutput -Name 'PidAttributionSchemaVersion' -DefaultValue 0) -gt 0) -Detail 'prepare-output.json PidAttributionSchemaVersion must be retained'
                Add-Check -Name "${iterationName}_prepare_output_pid_attribution_passed" -Passed ([bool](Get-JsonValue -Object $prepareOutput -Name 'PidAttributionPassed' -DefaultValue $false)) -Detail 'prepare-output.json PidAttributionPassed must be true'
                Add-Check -Name "${iterationName}_prepare_output_pid_probe_started_at_parseable" -Passed ($null -ne $preparePidProbeStartedAtUtc) -Detail 'prepare-output.json PidProbeStartedAtUtc must be parseable'
                Add-Check -Name "${iterationName}_prepare_output_pid_probe_finished_at_parseable" -Passed ($null -ne $preparePidProbeFinishedAtUtc) -Detail 'prepare-output.json PidProbeFinishedAtUtc must be parseable'
                Add-Check -Name "${iterationName}_prepare_output_pid_probe_times_match_result" -Passed ($null -ne $preparePidProbeStartedAtUtc -and $null -ne $preparePidProbeFinishedAtUtc -and $preparePidProbeStartedAtUtc -eq $resultLiveSessionPidProbeStartedAtUtc -and $preparePidProbeFinishedAtUtc -eq $resultLiveSessionPidProbeFinishedAtUtc) -Detail 'prepare-output.json PID probe timestamps must match iteration-result'
                Add-Check -Name "${iterationName}_prepare_output_prelaunch_slay_process_count_zero" -Passed ([int](Get-JsonValue -Object $prepareOutput -Name 'PreLaunchSlayProcessCount' -DefaultValue -1) -eq 0) -Detail 'prepare-output.json PreLaunchSlayProcessCount must be 0'
                Add-Check -Name "${iterationName}_prepare_output_prelaunch_slay_process_ids_empty" -Passed ((Get-ArrayCount -Value (Get-JsonValue -Object $prepareOutput -Name 'PreLaunchSlayProcessIds' -DefaultValue @())) -eq 0) -Detail 'prepare-output.json PreLaunchSlayProcessIds must be empty'
                Add-Check -Name "${iterationName}_prepare_output_selected_game_process_id_matches_result" -Passed ([int](Get-JsonValue -Object $prepareOutput -Name 'SelectedGameProcessId' -DefaultValue 0) -eq $iterationGameProcessId) -Detail 'prepare-output.json SelectedGameProcessId must match iteration-result GameProcessId'
                Add-Check -Name "${iterationName}_prepare_output_selected_game_process_start_time_parseable" -Passed ($null -ne $prepareSelectedGameProcessStartTimeUtc) -Detail 'prepare-output.json SelectedGameProcessStartTimeUtc must be parseable'
                Add-Check -Name "${iterationName}_prepare_output_selected_game_process_start_time_matches_result" -Passed ($null -ne $prepareSelectedGameProcessStartTimeUtc -and $null -ne $resultGameProcessStartTimeUtc -and $prepareSelectedGameProcessStartTimeUtc -eq $resultGameProcessStartTimeUtc) -Detail 'prepare-output.json SelectedGameProcessStartTimeUtc must match GameProcessStartTimeUtc'
                Add-Check -Name "${iterationName}_prepare_output_selected_game_process_path_matches_result" -Passed (-not [string]::IsNullOrWhiteSpace($prepareSelectedGameProcessPathFull) -and [System.StringComparer]::OrdinalIgnoreCase.Equals($prepareSelectedGameProcessPathFull, $resultGameProcessPathFull)) -Detail 'prepare-output.json SelectedGameProcessPath must match GameProcessPath'
                Add-Check -Name "${iterationName}_prepare_output_attribution_failure_reason_empty" -Passed ([string]::IsNullOrWhiteSpace([string](Get-JsonValue -Object $prepareOutput -Name 'AttributionFailureReason' -DefaultValue ''))) -Detail 'prepare-output.json AttributionFailureReason must be empty'
            }

            $sessionMovedModCount = -1
            $sessionMovedCurrentRunCount = -1
            $sessionSettingsHashBefore = ''
            $sessionSettingsBackupHashBefore = ''
            $sessionSettingsBackupExistedBefore = $false
            $sessionSettingsBackupExistedBeforeRecorded = $false
            $resultLiveSessionSettingsBackupExistedBeforeRecorded = Test-JsonProperty -Object $iterationResult -Name 'LiveSessionSettingsBackupExistedBefore'
            $resultLiveSessionSettingsBackupExistedBefore = [bool](Get-JsonValue -Object $iterationResult -Name 'LiveSessionSettingsBackupExistedBefore' -DefaultValue $false)
            if ($null -ne $sessionState) {
                $sessionStateEvidenceDir = Resolve-ChildOrAbsolutePath -BaseDir $iterationDir -Path ([string](Get-JsonValue -Object $sessionState -Name 'EvidenceDir' -DefaultValue ''))
                $sessionMovedMods = Get-JsonArrayProperty -Object $sessionState -Name 'MovedMods'
                $sessionMovedCurrentRuns = Get-JsonArrayProperty -Object $sessionState -Name 'MovedCurrentRuns'
                $sessionMovedModCount = [int]$sessionMovedMods.Count
                $sessionMovedCurrentRunCount = [int]$sessionMovedCurrentRuns.Count
                $sessionSettingsHashBefore = [string](Get-JsonValue -Object $sessionState -Name 'SettingsHashBefore' -DefaultValue '')
                $sessionSettingsBackupHashBefore = [string](Get-JsonValue -Object $sessionState -Name 'SettingsBackupHashBefore' -DefaultValue '')
                $sessionSettingsBackupExistedBeforeRecorded = Test-JsonProperty -Object $sessionState -Name 'SettingsBackupExistedBefore'
                $sessionSettingsBackupExistedBefore = [bool](Get-JsonValue -Object $sessionState -Name 'SettingsBackupExistedBefore' -DefaultValue $false)
                Add-Check -Name "${iterationName}_session_state_evidence_dir_matches_iteration" -Passed ($sessionStateEvidenceDir -and [System.StringComparer]::OrdinalIgnoreCase.Equals($sessionStateEvidenceDir, [System.IO.Path]::GetFullPath($iterationDir))) -Detail 'session-state.json EvidenceDir must match the current iteration directory'
                Add-Check -Name "${iterationName}_session_state_move_other_mods_matches_plan" -Passed ([bool](Get-JsonValue -Object $sessionState -Name 'MoveOtherMods' -DefaultValue $false) -eq [bool](Get-JsonValue -Object $plan -Name 'MoveOtherMods' -DefaultValue $false)) -Detail 'session-state.json MoveOtherMods must match monkey-plan.json'
                Add-Check -Name "${iterationName}_session_state_move_current_runs_matches_plan" -Passed ([bool](Get-JsonValue -Object $sessionState -Name 'MoveCurrentRuns' -DefaultValue $false) -eq [bool](Get-JsonValue -Object $plan -Name 'MoveCurrentRuns' -DefaultValue $false)) -Detail 'session-state.json MoveCurrentRuns must match monkey-plan.json'
                Add-Check -Name "${iterationName}_session_state_moved_mods_array" -Passed ([bool]$sessionMovedMods.IsArray) -Detail 'session-state.json MovedMods must be retained as an array'
                Add-Check -Name "${iterationName}_session_state_moved_current_runs_array" -Passed ([bool]$sessionMovedCurrentRuns.IsArray) -Detail 'session-state.json MovedCurrentRuns must be retained as an array'
                Add-Check -Name "${iterationName}_session_state_settings_hash_before_sha256" -Passed (Test-Sha256Text -Value $sessionSettingsHashBefore) -Detail 'session-state.json SettingsHashBefore must retain the pre-prepare settings SHA256'
                Add-Check -Name "${iterationName}_session_state_settings_backup_existence_recorded" -Passed $sessionSettingsBackupExistedBeforeRecorded -Detail 'session-state.json SettingsBackupExistedBefore must record whether settings.save.backup existed before prepare'
                if ($sessionSettingsBackupExistedBefore) {
                    Add-Check -Name "${iterationName}_session_state_settings_backup_hash_before_sha256" -Passed (Test-Sha256Text -Value $sessionSettingsBackupHashBefore) -Detail 'session-state.json SettingsBackupHashBefore must retain the pre-prepare backup SHA256 when settings.save.backup existed'
                } else {
                    Add-Check -Name "${iterationName}_session_state_settings_backup_absent_hash_blank" -Passed ([string]::IsNullOrWhiteSpace($sessionSettingsBackupHashBefore)) -Detail 'session-state.json SettingsBackupHashBefore must be blank when settings.save.backup did not exist before prepare'
                }
            }
            Add-Check -Name "${iterationName}_result_live_session_settings_backup_existed_before_recorded" -Passed $resultLiveSessionSettingsBackupExistedBeforeRecorded -Detail 'iteration-result.json LiveSessionSettingsBackupExistedBefore must record whether settings.save.backup existed before prepare'
            Add-Check -Name "${iterationName}_result_live_session_settings_backup_existed_before_matches_session" -Passed ($resultLiveSessionSettingsBackupExistedBeforeRecorded -and $sessionSettingsBackupExistedBeforeRecorded -and $resultLiveSessionSettingsBackupExistedBefore -eq $sessionSettingsBackupExistedBefore) -Detail 'iteration-result.json LiveSessionSettingsBackupExistedBefore must match session-state.json SettingsBackupExistedBefore'

            Add-Check -Name "${iterationName}_result_log_copied" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'LogCopied' -DefaultValue $false)) -Detail 'LogCopied must be true'
            Add-Check -Name "${iterationName}_result_current_iteration_log_copied" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'CurrentIterationLogCopied' -DefaultValue $false)) -Detail 'CurrentIterationLogCopied must be true'
            Add-Check -Name "${iterationName}_result_before_log_copied" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'GodotLogBeforeCopied' -DefaultValue $false)) -Detail 'GodotLogBeforeCopied must be true'
            $resultBeforeLogPath = Resolve-ChildOrAbsolutePath -BaseDir $iterationDir -Path ([string](Get-JsonValue -Object $iterationResult -Name 'GodotLogBeforePath' -DefaultValue ''))
            $resultAfterLaunchLogPath = Resolve-ChildOrAbsolutePath -BaseDir $iterationDir -Path ([string](Get-JsonValue -Object $iterationResult -Name 'GodotLogAfterLaunchPath' -DefaultValue ''))
            $resultGodotCurrentIterationLogPath = Resolve-ChildOrAbsolutePath -BaseDir $iterationDir -Path ([string](Get-JsonValue -Object $iterationResult -Name 'GodotLogCurrentIterationPath' -DefaultValue ''))
            Add-Check -Name "${iterationName}_godot_log_before_under_iteration_dir" -Passed ($resultBeforeLogPath -and (Test-PathUnderDirectory -Path $resultBeforeLogPath -Directory $iterationDir)) -Detail 'GodotLogBeforePath must stay inside the current iteration directory'
            Add-Check -Name "${iterationName}_godot_log_before_leaf_expected" -Passed ($resultBeforeLogPath -and ([System.IO.Path]::GetFileName($resultBeforeLogPath) -eq 'godot.log.before')) -Detail 'GodotLogBeforePath must end with godot.log.before'
            Add-Check -Name "${iterationName}_godot_log_before_path_matches_retained_file" -Passed ($resultBeforeLogPath -and ([System.StringComparer]::OrdinalIgnoreCase.Equals($resultBeforeLogPath, [System.IO.Path]::GetFullPath($beforeLogPath)))) -Detail 'GodotLogBeforePath must point to the retained godot.log.before file'
            Add-Check -Name "${iterationName}_godot_log_after_launch_under_iteration_dir" -Passed ($resultAfterLaunchLogPath -and (Test-PathUnderDirectory -Path $resultAfterLaunchLogPath -Directory $iterationDir)) -Detail 'GodotLogAfterLaunchPath must stay inside the current iteration directory'
            Add-Check -Name "${iterationName}_godot_log_after_launch_leaf_expected" -Passed ($resultAfterLaunchLogPath -and ([System.IO.Path]::GetFileName($resultAfterLaunchLogPath) -eq 'godot.log.after-launch')) -Detail 'GodotLogAfterLaunchPath must end with godot.log.after-launch'
            Add-Check -Name "${iterationName}_godot_log_after_launch_path_matches_retained_file" -Passed ($resultAfterLaunchLogPath -and ([System.StringComparer]::OrdinalIgnoreCase.Equals($resultAfterLaunchLogPath, [System.IO.Path]::GetFullPath($logPath)))) -Detail 'GodotLogAfterLaunchPath must point to the retained godot.log.after-launch file'
            Add-Check -Name "${iterationName}_godot_current_iteration_log_under_iteration_dir" -Passed ($resultGodotCurrentIterationLogPath -and (Test-PathUnderDirectory -Path $resultGodotCurrentIterationLogPath -Directory $iterationDir)) -Detail 'GodotLogCurrentIterationPath must stay inside the current iteration directory'
            Add-Check -Name "${iterationName}_godot_current_iteration_log_leaf_expected" -Passed ($resultGodotCurrentIterationLogPath -and ([System.IO.Path]::GetFileName($resultGodotCurrentIterationLogPath) -eq 'godot.log.current-iteration')) -Detail 'GodotLogCurrentIterationPath must end with godot.log.current-iteration'
            Add-Check -Name "${iterationName}_godot_current_iteration_log_path_matches_retained_file" -Passed ($resultGodotCurrentIterationLogPath -and ([System.StringComparer]::OrdinalIgnoreCase.Equals($resultGodotCurrentIterationLogPath, [System.IO.Path]::GetFullPath($currentIterationLogPath)))) -Detail 'GodotLogCurrentIterationPath must point to the retained godot.log.current-iteration file'
            $resultBeforeLogLengthBytes = [long](Get-JsonValue -Object $iterationResult -Name 'GodotLogBeforeLengthBytes' -DefaultValue -1)
            $resultBeforeLogSha256 = [string](Get-JsonValue -Object $iterationResult -Name 'GodotLogBeforeSha256' -DefaultValue '')
            $resultAfterLaunchLengthBytes = [long](Get-JsonValue -Object $iterationResult -Name 'GodotLogAfterLaunchLengthBytes' -DefaultValue -1)
            $resultAfterLaunchSha256 = [string](Get-JsonValue -Object $iterationResult -Name 'GodotLogAfterLaunchSha256' -DefaultValue '')
            $resultCurrentIterationLengthBytes = [long](Get-JsonValue -Object $iterationResult -Name 'GodotLogCurrentIterationLengthBytes' -DefaultValue -1)
            $resultCurrentIterationSha256 = [string](Get-JsonValue -Object $iterationResult -Name 'GodotLogCurrentIterationSha256' -DefaultValue '')
            Add-Check -Name "${iterationName}_godot_log_before_length_recorded" -Passed ($resultBeforeLogLengthBytes -ge 0) -Detail 'GodotLogBeforeLengthBytes must be retained and non-negative'
            Add-Check -Name "${iterationName}_godot_log_before_sha256_recorded" -Passed (-not [string]::IsNullOrWhiteSpace($resultBeforeLogSha256)) -Detail 'GodotLogBeforeSha256 must be retained'
            Add-Check -Name "${iterationName}_godot_log_after_launch_length_recorded" -Passed ($resultAfterLaunchLengthBytes -ge 0) -Detail 'GodotLogAfterLaunchLengthBytes must be retained and non-negative'
            Add-Check -Name "${iterationName}_godot_log_after_launch_sha256_recorded" -Passed (-not [string]::IsNullOrWhiteSpace($resultAfterLaunchSha256)) -Detail 'GodotLogAfterLaunchSha256 must be retained'
            Add-Check -Name "${iterationName}_godot_current_iteration_log_length_recorded" -Passed ($resultCurrentIterationLengthBytes -ge 0) -Detail 'GodotLogCurrentIterationLengthBytes must be retained and non-negative'
            Add-Check -Name "${iterationName}_godot_current_iteration_log_sha256_recorded" -Passed (-not [string]::IsNullOrWhiteSpace($resultCurrentIterationSha256)) -Detail 'GodotLogCurrentIterationSha256 must be retained'
            if ($beforeLogExists) {
                Add-Check -Name "${iterationName}_godot_log_before_length_matches_retained_file" -Passed ($resultBeforeLogLengthBytes -eq [long](Get-Item -LiteralPath $beforeLogPath).Length) -Detail 'GodotLogBeforeLengthBytes must match retained godot.log.before bytes'
                Add-Check -Name "${iterationName}_godot_log_before_sha256_matches_retained_file" -Passed ([System.StringComparer]::OrdinalIgnoreCase.Equals($resultBeforeLogSha256, (Get-FileSha256OrEmpty -Path $beforeLogPath))) -Detail 'GodotLogBeforeSha256 must match retained godot.log.before'
            }
            if ($logExists) {
                Add-Check -Name "${iterationName}_godot_log_after_launch_length_matches_retained_file" -Passed ($resultAfterLaunchLengthBytes -eq [long](Get-Item -LiteralPath $logPath).Length) -Detail 'GodotLogAfterLaunchLengthBytes must match retained godot.log.after-launch bytes'
                Add-Check -Name "${iterationName}_godot_log_after_launch_sha256_matches_retained_file" -Passed ([System.StringComparer]::OrdinalIgnoreCase.Equals($resultAfterLaunchSha256, (Get-FileSha256OrEmpty -Path $logPath))) -Detail 'GodotLogAfterLaunchSha256 must match retained godot.log.after-launch'
            }
            if ($currentIterationLogExists) {
                Add-Check -Name "${iterationName}_godot_current_iteration_log_length_matches_retained_file" -Passed ($resultCurrentIterationLengthBytes -eq [long](Get-Item -LiteralPath $currentIterationLogPath).Length) -Detail 'GodotLogCurrentIterationLengthBytes must match retained godot.log.current-iteration bytes'
                Add-Check -Name "${iterationName}_godot_current_iteration_log_sha256_matches_retained_file" -Passed ([System.StringComparer]::OrdinalIgnoreCase.Equals($resultCurrentIterationSha256, (Get-FileSha256OrEmpty -Path $currentIterationLogPath))) -Detail 'GodotLogCurrentIterationSha256 must match retained godot.log.current-iteration'
            }
            $resultCurrentIterationLogPath = Resolve-ChildOrAbsolutePath -BaseDir $iterationDir -Path ([string](Get-JsonValue -Object $iterationResult -Name 'CurrentIterationLogPath' -DefaultValue ''))
            Add-Check -Name "${iterationName}_current_iteration_log_under_iteration_dir" -Passed ($resultCurrentIterationLogPath -and (Test-PathUnderDirectory -Path $resultCurrentIterationLogPath -Directory $iterationDir)) -Detail 'CurrentIterationLogPath must stay inside the current iteration directory'
            Add-Check -Name "${iterationName}_current_iteration_log_leaf_expected" -Passed ($resultCurrentIterationLogPath -and ([System.IO.Path]::GetFileName($resultCurrentIterationLogPath) -eq 'godot.log.current-iteration')) -Detail 'CurrentIterationLogPath must end with godot.log.current-iteration'
            Add-Check -Name "${iterationName}_current_iteration_log_path_matches_retained_file" -Passed ($resultCurrentIterationLogPath -and ([System.StringComparer]::OrdinalIgnoreCase.Equals($resultCurrentIterationLogPath, [System.IO.Path]::GetFullPath($currentIterationLogPath)))) -Detail 'CurrentIterationLogPath must point to the retained godot.log.current-iteration file'
            $resultLogScanOffsetBytes = [long](Get-JsonValue -Object $iterationResult -Name 'LogScanOffsetBytes' -DefaultValue -1)
            $logScanOffsetRecorded = $resultLogScanOffsetBytes -ge 0
            Add-Check -Name "${iterationName}_log_scan_offset_recorded" -Passed $logScanOffsetRecorded -Detail 'LogScanOffsetBytes must be retained and non-negative'
            $logScanOffsetWithinFullLog = $false
            if ($logExists -and $logScanOffsetRecorded) {
                $fullLogLength = [long](Get-Item -LiteralPath $logPath).Length
                $logScanOffsetWithinFullLog = $resultLogScanOffsetBytes -le $fullLogLength
                Add-Check -Name "${iterationName}_log_scan_offset_within_full_log" -Passed $logScanOffsetWithinFullLog -Detail "LogScanOffsetBytes must be within godot.log.after-launch; offset=$resultLogScanOffsetBytes, length=$fullLogLength"
            }

            if ($beforeLogExists -and $logScanOffsetRecorded) {
                $beforeLogLength = [long](Get-Item -LiteralPath $beforeLogPath).Length
                Add-Check -Name "${iterationName}_log_scan_offset_matches_before_length" -Passed ($resultLogScanOffsetBytes -eq $beforeLogLength) -Detail "LogScanOffsetBytes must equal retained godot.log.before length; offset=$resultLogScanOffsetBytes, beforeLength=$beforeLogLength"
            }

            if ($beforeLogExists -and $logExists -and $currentIterationLogExists) {
                $sliceBinding = Test-CurrentSliceBinding -BeforePath $beforeLogPath -AfterPath $logPath -CurrentPath $currentIterationLogPath
                Add-Check -Name "${iterationName}_current_iteration_log_matches_after_launch_prefix" -Passed ([bool]$sliceBinding.PrefixMatches) -Detail $sliceBinding.Detail
                Add-Check -Name "${iterationName}_current_iteration_log_matches_after_launch_slice" -Passed ([bool]$sliceBinding.SliceMatches) -Detail $sliceBinding.Detail
            } else {
                Add-Check -Name "${iterationName}_current_iteration_log_matches_after_launch_prefix" -Passed $false -Detail 'requires retained before, after-launch, and current-iteration logs'
                Add-Check -Name "${iterationName}_current_iteration_log_matches_after_launch_slice" -Passed $false -Detail 'requires retained before, after-launch, and current-iteration logs'
            }

            if ($logExists -and $currentIterationLogExists -and $logScanOffsetRecorded -and $logScanOffsetWithinFullLog) {
                $expectedCurrentIterationLogText = Read-TextAfterByteOffset -Path $logPath -Offset $resultLogScanOffsetBytes
                $actualCurrentIterationLogText = [System.IO.File]::ReadAllText($currentIterationLogPath)
                $normalizedExpectedSlice = Normalize-LogSliceForComparison -Text $expectedCurrentIterationLogText
                $normalizedActualSlice = Normalize-LogSliceForComparison -Text $actualCurrentIterationLogText
                Add-Check -Name "${iterationName}_current_iteration_log_matches_scan_offset" -Passed ([string]::Equals($normalizedActualSlice, $normalizedExpectedSlice, [System.StringComparison]::Ordinal)) -Detail 'godot.log.current-iteration must match godot.log.after-launch from LogScanOffsetBytes, ignoring only trailing newline differences'
            }

            $resultCommandAckRequiredForLog = [bool](Get-JsonValue -Object $iterationResult -Name 'CommandAckRequired' -DefaultValue $false)
            $resultCommandAckPatternForLog = [string](Get-JsonValue -Object $iterationResult -Name 'CommandAckPattern' -DefaultValue '')
            $resultCommandAckPatternRetainedForLog = -not [string]::IsNullOrWhiteSpace($resultCommandAckPatternForLog)
            $resultCommandForAck = [string](Get-JsonValue -Object $iterationResult -Name 'Command' -DefaultValue '')
            $canonicalCommandAckPattern = Get-CanonicalCommandAckPattern -Command $resultCommandForAck
            $canonicalCommandRequiresAck = -not [string]::IsNullOrWhiteSpace($canonicalCommandAckPattern)
            $canonicalCommandOwnerArea = Get-CanonicalCommandOwnerArea -Command $resultCommandForAck
            $canonicalCommandScenarioTag = Get-CanonicalCommandScenarioTag -Command $resultCommandForAck
            if (-not [string]::IsNullOrWhiteSpace($canonicalCommandScenarioTag)) {
                $resultScenarioTagForCanonical = [string](Get-JsonValue -Object $iterationResult -Name 'ScenarioTag' -DefaultValue '')
                Add-Check -Name "${iterationName}_scenario_tag_matches_canonical_command" -Passed ([string]::Equals($resultScenarioTagForCanonical, $canonicalCommandScenarioTag, [System.StringComparison]::Ordinal)) -Detail 'ScenarioTag must match the canonical runner classification for known built-in commands'
            }

            if (-not [string]::IsNullOrWhiteSpace($canonicalCommandOwnerArea)) {
                $resultOwnerAreaForCanonical = [string](Get-JsonValue -Object $iterationResult -Name 'OwnerArea' -DefaultValue '')
                Add-Check -Name "${iterationName}_owner_area_matches_canonical_command" -Passed ([string]::Equals($resultOwnerAreaForCanonical, $canonicalCommandOwnerArea, [System.StringComparison]::Ordinal)) -Detail 'OwnerArea must match the canonical runner classification for known built-in commands'
            }

            Add-Check -Name "${iterationName}_command_ack_required_matches_pattern" -Passed ($resultCommandAckRequiredForLog -eq $resultCommandAckPatternRetainedForLog) -Detail 'CommandAckRequired must equal whether CommandAckPattern is retained'
            if ($canonicalCommandRequiresAck) {
                Add-Check -Name "${iterationName}_command_ack_required_for_canonical_command" -Passed $resultCommandAckRequiredForLog -Detail 'Known built-in commands with canonical acknowledgement patterns must require command acknowledgement'
            }

            if ($resultCommandAckRequiredForLog -or $resultCommandAckPatternRetainedForLog -or $canonicalCommandRequiresAck) {
                $commandAckPatternPresent = $resultCommandAckPatternRetainedForLog
                Add-Check -Name "${iterationName}_command_ack_pattern_present_when_required" -Passed $commandAckPatternPresent -Detail 'CommandAckRequired packets must retain the regex used to prove the command acknowledgement'

                if ($canonicalCommandRequiresAck) {
                    Add-Check -Name "${iterationName}_command_ack_pattern_matches_canonical_command" -Passed ([string]::Equals($resultCommandAckPatternForLog, $canonicalCommandAckPattern, [System.StringComparison]::Ordinal)) -Detail 'CommandAckPattern must match the canonical pattern for known built-in commands'
                }

                if ($currentIterationLogExists -and $commandAckPatternPresent) {
                    try {
                        $commandAckObservedInCurrentLog = [regex]::IsMatch([System.IO.File]::ReadAllText($currentIterationLogPath), $resultCommandAckPatternForLog)
                        Add-Check -Name "${iterationName}_command_ack_pattern_matches_current_iteration_log" -Passed $commandAckObservedInCurrentLog -Detail 'CommandAckPattern must match the retained current-iteration log slice'
                    } catch {
                        Add-Check -Name "${iterationName}_command_ack_pattern_regex_valid" -Passed $false -Detail "invalid CommandAckPattern regex: $($_.Exception.Message)"
                    }
                }
            }

            Add-Check -Name "${iterationName}_result_audit_clean" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'AuditClean' -DefaultValue $false)) -Detail 'AuditClean must be true'
            Add-Check -Name "${iterationName}_result_expectation_passed" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'ExpectationPassed' -DefaultValue $false)) -Detail 'ExpectationPassed must be true'
            Add-Check -Name "${iterationName}_result_sts1_mode_verifier_passed" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'Sts1ModeVerifierPassed' -DefaultValue $false)) -Detail 'Sts1ModeVerifierPassed must be true'
            Add-Check -Name "${iterationName}_restore_succeeded" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'RestoreSucceeded' -DefaultValue $false)) -Detail 'RestoreSucceeded must be true'
            Add-Check -Name "${iterationName}_iteration_passed" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'Passed' -DefaultValue $false)) -Detail 'Passed must be true'

            if ($null -ne $restoreState) {
                $restoreStateEvidenceDir = Resolve-ChildOrAbsolutePath -BaseDir $iterationDir -Path ([string](Get-JsonValue -Object $restoreState -Name 'EvidenceDir' -DefaultValue ''))
                $restoreStateRestoredAtUtc = ConvertTo-DateTimeUtcOrNull -Value (Get-JsonValue -Object $restoreState -Name 'RestoredAt' -DefaultValue $null)
                $restoreStateSchemaVersion = [int](Get-JsonValue -Object $restoreState -Name 'RestoreSchemaVersion' -DefaultValue 0)
                $restoreStateStoppedSelectedGameProcess = [bool](Get-JsonValue -Object $restoreState -Name 'StoppedSelectedGameProcess' -DefaultValue $false)
                $restoreStateRestoredModCount = [int](Get-JsonValue -Object $restoreState -Name 'RestoredModCount' -DefaultValue -1)
                $restoreStateRestoredCurrentRunCount = [int](Get-JsonValue -Object $restoreState -Name 'RestoredCurrentRunCount' -DefaultValue -1)
                $restoreStatePreservedNewCurrentRunCount = [int](Get-JsonValue -Object $restoreState -Name 'PreservedNewCurrentRunCount' -DefaultValue -1)
                $restoreStatePreservedManifestPath = Resolve-ChildOrAbsolutePath -BaseDir $iterationDir -Path ([string](Get-JsonValue -Object $restoreState -Name 'PreservedNewCurrentRunsManifestPath' -DefaultValue ''))
                $restoreStatePreservedManifestSha256 = [string](Get-JsonValue -Object $restoreState -Name 'PreservedNewCurrentRunsManifestSha256' -DefaultValue '')
                $restoreStatePostRestoreSlayProcessCount = [int](Get-JsonValue -Object $restoreState -Name 'PostRestoreSlayProcessCount' -DefaultValue -1)
                $restoreStatePostRestoreGodotProcessCount = [int](Get-JsonValue -Object $restoreState -Name 'PostRestoreGodotProcessCount' -DefaultValue -1)
                $restoreStateSettingsHash = [string](Get-JsonValue -Object $restoreState -Name 'SettingsHashAfterRestore' -DefaultValue '')
                $restoreStateSettingsBackupHash = [string](Get-JsonValue -Object $restoreState -Name 'SettingsBackupHashAfterRestore' -DefaultValue '')
                $restoreStateSettingsBackupExistsAfterRecorded = Test-JsonProperty -Object $restoreState -Name 'SettingsBackupExistsAfterRestore'
                $restoreStateSettingsBackupExistsAfter = [bool](Get-JsonValue -Object $restoreState -Name 'SettingsBackupExistsAfterRestore' -DefaultValue $false)
                $restoreStateStoppedProcessesProperty = @($restoreState.PSObject.Properties | Where-Object { [string]::Equals($_.Name, 'StoppedProcesses', [System.StringComparison]::Ordinal) } | Select-Object -First 1)
                $restoreStateStoppedProcessesIsArray = $restoreStateStoppedProcessesProperty.Count -eq 1 -and $restoreStateStoppedProcessesProperty[0].Value -is [System.Array]
                $restoreStatePostRestoreSlayProcessIds = Get-JsonArrayProperty -Object $restoreState -Name 'PostRestoreSlayProcessIds'
                $restoreStatePostRestoreGodotProcessIds = Get-JsonArrayProperty -Object $restoreState -Name 'PostRestoreGodotProcessIds'
                Add-Check -Name "${iterationName}_restore_state_schema_version" -Passed ($restoreStateSchemaVersion -eq 1) -Detail 'restore-state.json RestoreSchemaVersion must be 1'
                Add-Check -Name "${iterationName}_restore_state_evidence_dir_matches_iteration" -Passed ($restoreStateEvidenceDir -and [System.StringComparer]::OrdinalIgnoreCase.Equals($restoreStateEvidenceDir, [System.IO.Path]::GetFullPath($iterationDir))) -Detail 'restore-state.json EvidenceDir must match the current iteration directory'
                Add-Check -Name "${iterationName}_restore_state_restored_at_parseable" -Passed ($null -ne $restoreStateRestoredAtUtc) -Detail 'restore-state.json RestoredAt must be parseable'
                Add-Check -Name "${iterationName}_restore_state_restored_mod_count_recorded" -Passed ($restoreStateRestoredModCount -ge 0) -Detail 'restore-state.json RestoredModCount must be retained and non-negative'
                Add-Check -Name "${iterationName}_restore_state_restored_mod_count_matches_session" -Passed ($sessionMovedModCount -ge 0 -and $restoreStateRestoredModCount -eq $sessionMovedModCount) -Detail 'restore-state.json RestoredModCount must match session-state.json MovedMods count'
                Add-Check -Name "${iterationName}_restore_state_restored_current_run_count_recorded" -Passed ($restoreStateRestoredCurrentRunCount -ge 0) -Detail 'restore-state.json RestoredCurrentRunCount must be retained and non-negative'
                Add-Check -Name "${iterationName}_restore_state_restored_current_run_count_matches_session" -Passed ($sessionMovedCurrentRunCount -ge 0 -and $restoreStateRestoredCurrentRunCount -eq $sessionMovedCurrentRunCount) -Detail 'restore-state.json RestoredCurrentRunCount must match session-state.json MovedCurrentRuns count'
                $restoreItemCountsMatch = $sessionMovedModCount -ge 0 -and $sessionMovedCurrentRunCount -ge 0 -and $restoreStateRestoredModCount -eq $sessionMovedModCount -and $restoreStateRestoredCurrentRunCount -eq $sessionMovedCurrentRunCount
                Add-Check -Name "${iterationName}_result_restore_item_counts_match_flag_true" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'LiveSessionRestoreItemCountsMatch' -DefaultValue $false)) -Detail 'iteration-result.json LiveSessionRestoreItemCountsMatch must be true for a clean packet'
                Add-Check -Name "${iterationName}_result_restore_item_counts_match_flag_matches_restore_state" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'LiveSessionRestoreItemCountsMatch' -DefaultValue $false) -eq $restoreItemCountsMatch) -Detail 'iteration-result.json LiveSessionRestoreItemCountsMatch must match restore/session count parity'
                Add-Check -Name "${iterationName}_restore_state_stopped_processes_field_present" -Passed (Test-JsonProperty -Object $restoreState -Name 'StoppedProcesses') -Detail 'restore-state.json must retain StoppedProcesses from -StopGameOnRestore'
                Add-Check -Name "${iterationName}_restore_state_stopped_processes_array" -Passed $restoreStateStoppedProcessesIsArray -Detail 'restore-state.json StoppedProcesses must be an array, even when empty'
                Add-Check -Name "${iterationName}_restore_state_stopped_selected_game_process" -Passed $restoreStateStoppedSelectedGameProcess -Detail 'restore-state.json StoppedSelectedGameProcess must be true for a passed launched iteration'
                Add-Check -Name "${iterationName}_restore_state_post_restore_slay_process_count_zero" -Passed ($restoreStatePostRestoreSlayProcessCount -eq 0) -Detail 'restore-state.json PostRestoreSlayProcessCount must be 0 after restore'
                Add-Check -Name "${iterationName}_restore_state_post_restore_slay_process_ids_array" -Passed ([bool]$restoreStatePostRestoreSlayProcessIds.IsArray) -Detail 'restore-state.json PostRestoreSlayProcessIds must be retained as an array'
                Add-Check -Name "${iterationName}_restore_state_post_restore_slay_process_ids_empty" -Passed ([bool]$restoreStatePostRestoreSlayProcessIds.IsArray -and [int]$restoreStatePostRestoreSlayProcessIds.Count -eq 0) -Detail 'restore-state.json PostRestoreSlayProcessIds must be empty after restore'
                Add-Check -Name "${iterationName}_restore_state_post_restore_godot_process_count_zero" -Passed ($restoreStatePostRestoreGodotProcessCount -eq 0) -Detail 'restore-state.json PostRestoreGodotProcessCount must be 0 after restore'
                Add-Check -Name "${iterationName}_restore_state_post_restore_godot_process_ids_array" -Passed ([bool]$restoreStatePostRestoreGodotProcessIds.IsArray) -Detail 'restore-state.json PostRestoreGodotProcessIds must be retained as an array'
                Add-Check -Name "${iterationName}_restore_state_post_restore_godot_process_ids_empty" -Passed ([bool]$restoreStatePostRestoreGodotProcessIds.IsArray -and [int]$restoreStatePostRestoreGodotProcessIds.Count -eq 0) -Detail 'restore-state.json PostRestoreGodotProcessIds must be empty after restore'
                Add-Check -Name "${iterationName}_restore_state_settings_backup_exists_after_recorded" -Passed $restoreStateSettingsBackupExistsAfterRecorded -Detail 'restore-state.json SettingsBackupExistsAfterRestore must record whether settings.save.backup exists after restore'
                $settingsBackupStateClosed = if ($sessionSettingsBackupExistedBefore) {
                    (Test-Sha256Text -Value $restoreStateSettingsBackupHash) -and
                    [System.StringComparer]::OrdinalIgnoreCase.Equals($restoreStateSettingsBackupHash, $sessionSettingsBackupHashBefore) -and
                    $restoreStateSettingsBackupExistsAfter
                } else {
                    $restoreStateSettingsBackupExistsAfterRecorded -and
                    -not $restoreStateSettingsBackupExistsAfter -and
                    [string]::IsNullOrWhiteSpace($restoreStateSettingsBackupHash)
                }
                $settingsBackupHashShapeClosed = if ($sessionSettingsBackupExistedBefore) {
                    Test-Sha256Text -Value $restoreStateSettingsBackupHash
                } else {
                    [string]::IsNullOrWhiteSpace($restoreStateSettingsBackupHash)
                }
                Add-Check -Name "${iterationName}_restore_state_settings_hashes_recorded" -Passed ((-not [string]::IsNullOrWhiteSpace($restoreStateSettingsHash)) -and $settingsBackupHashShapeClosed) -Detail 'restore-state.json must retain settings hash and conditionally retain backup hash only when settings.save.backup existed before prepare'
                Add-Check -Name "${iterationName}_restore_state_settings_hashes_sha256_format" -Passed ((Test-Sha256Text -Value $restoreStateSettingsHash) -and $settingsBackupHashShapeClosed) -Detail 'restore-state.json settings hashes must use SHA256 format when files are expected to exist'
                Add-Check -Name "${iterationName}_restore_state_settings_hash_matches_session_before" -Passed ((Test-Sha256Text -Value $sessionSettingsHashBefore) -and [System.StringComparer]::OrdinalIgnoreCase.Equals($restoreStateSettingsHash, $sessionSettingsHashBefore)) -Detail 'restore-state.json SettingsHashAfterRestore must match session-state.json SettingsHashBefore'
                Add-Check -Name "${iterationName}_restore_state_settings_backup_hash_matches_session_before" -Passed $settingsBackupStateClosed -Detail 'restore-state.json SettingsBackupHashAfterRestore must match session-state.json SettingsBackupHashBefore when the backup existed, or prove absence was restored'
                Add-Check -Name "${iterationName}_restore_state_preserved_current_run_count_recorded" -Passed ($restoreStatePreservedNewCurrentRunCount -ge 0) -Detail 'restore-state.json PreservedNewCurrentRunCount must be retained and non-negative'
                if ($restoreStatePreservedNewCurrentRunCount -gt 0) {
                    Add-Check -Name "${iterationName}_restore_state_preserved_current_runs_manifest_under_iteration_dir" -Passed ($restoreStatePreservedManifestPath -and (Test-PathUnderDirectory -Path $restoreStatePreservedManifestPath -Directory $iterationDir)) -Detail 'PreservedNewCurrentRunsManifestPath must stay inside the current iteration directory'
                    Add-Check -Name "${iterationName}_restore_state_preserved_current_runs_manifest_exists" -Passed ($restoreStatePreservedManifestPath -and (Test-Path -LiteralPath $restoreStatePreservedManifestPath -PathType Leaf)) -Detail 'PreservedNewCurrentRunsManifestPath must point to a retained manifest when count is positive'
                    Add-Check -Name "${iterationName}_restore_state_preserved_current_runs_manifest_sha256_recorded" -Passed (Test-Sha256Text -Value $restoreStatePreservedManifestSha256) -Detail 'PreservedNewCurrentRunsManifestSha256 must be retained when count is positive'
                    if ($restoreStatePreservedManifestPath -and (Test-Path -LiteralPath $restoreStatePreservedManifestPath -PathType Leaf)) {
                        Add-Check -Name "${iterationName}_restore_state_preserved_current_runs_manifest_sha256_matches" -Passed ([System.StringComparer]::OrdinalIgnoreCase.Equals($restoreStatePreservedManifestSha256, (Get-FileSha256OrEmpty -Path $restoreStatePreservedManifestPath))) -Detail 'PreservedNewCurrentRunsManifestSha256 must match the retained manifest'
                    }
                    Add-Check -Name "${iterationName}_result_preserved_current_runs_manifest_bound" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'LiveSessionPreservedNewCurrentRunsManifestBound' -DefaultValue $false)) -Detail 'iteration-result.json LiveSessionPreservedNewCurrentRunsManifestBound must be true when preserved current-run count is positive'
                }
                Add-Check -Name "${iterationName}_result_restore_schema_matches_restore_state" -Passed ([int](Get-JsonValue -Object $iterationResult -Name 'LiveSessionRestoreSchemaVersion' -DefaultValue 0) -eq $restoreStateSchemaVersion) -Detail 'iteration-result.json LiveSessionRestoreSchemaVersion must match restore-state.json'
                Add-Check -Name "${iterationName}_result_restore_stopped_selected_matches_restore_state" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'LiveSessionStoppedSelectedGameProcess' -DefaultValue $false) -eq $restoreStateStoppedSelectedGameProcess) -Detail 'iteration-result.json LiveSessionStoppedSelectedGameProcess must match restore-state.json'
                Add-Check -Name "${iterationName}_result_post_restore_process_counts_match_restore_state" -Passed ([int](Get-JsonValue -Object $iterationResult -Name 'LiveSessionPostRestoreSlayProcessCount' -DefaultValue -1) -eq $restoreStatePostRestoreSlayProcessCount -and [int](Get-JsonValue -Object $iterationResult -Name 'LiveSessionPostRestoreGodotProcessCount' -DefaultValue -1) -eq $restoreStatePostRestoreGodotProcessCount) -Detail 'iteration-result.json post-restore process counts must match restore-state.json'
                Add-Check -Name "${iterationName}_result_restore_settings_hashes_match_restore_state" -Passed ([System.StringComparer]::OrdinalIgnoreCase.Equals([string](Get-JsonValue -Object $iterationResult -Name 'LiveSessionSettingsHashAfterRestore' -DefaultValue ''), $restoreStateSettingsHash) -and [System.StringComparer]::OrdinalIgnoreCase.Equals([string](Get-JsonValue -Object $iterationResult -Name 'LiveSessionSettingsBackupHashAfterRestore' -DefaultValue ''), $restoreStateSettingsBackupHash)) -Detail 'iteration-result.json restore settings hashes must match restore-state.json'
                Add-Check -Name "${iterationName}_result_restore_settings_backup_existence_matches_restore_state" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'LiveSessionSettingsBackupExistsAfterRestore' -DefaultValue $false) -eq $restoreStateSettingsBackupExistsAfter) -Detail 'iteration-result.json LiveSessionSettingsBackupExistsAfterRestore must match restore-state.json'
                Add-Check -Name "${iterationName}_result_restore_settings_restored_flags_true" -Passed ([bool](Get-JsonValue -Object $iterationResult -Name 'LiveSessionSettingsRestoredFromBackup' -DefaultValue $false) -and [bool](Get-JsonValue -Object $iterationResult -Name 'LiveSessionSettingsBackupRestoredFromBackup' -DefaultValue $false)) -Detail 'iteration-result.json restore settings restored flags must be true'
            }

            if ($null -ne $plannedForIteration) {
                $resultScenario = [string](Get-JsonValue -Object $iterationResult -Name 'Scenario' -DefaultValue '')
                $resultCommand = [string](Get-JsonValue -Object $iterationResult -Name 'Command' -DefaultValue '')
                $plannedCommand = [string](Get-JsonValue -Object $plannedForIteration -Name 'Command' -DefaultValue '')
                $resultCommandIndex = [int](Get-JsonValue -Object $iterationResult -Name 'CommandIndex' -DefaultValue -1)
                $plannedCommandIndex = [int](Get-JsonValue -Object $plannedForIteration -Name 'CommandIndex' -DefaultValue -2)
                $resultCommandSelectionMode = [string](Get-JsonValue -Object $iterationResult -Name 'CommandSelectionMode' -DefaultValue '')
                $plannedCommandSelectionMode = [string](Get-JsonValue -Object $plannedForIteration -Name 'CommandSelectionMode' -DefaultValue '')
                $resultScenarioTag = [string](Get-JsonValue -Object $iterationResult -Name 'ScenarioTag' -DefaultValue '')
                $plannedScenarioTag = [string](Get-JsonValue -Object $plannedForIteration -Name 'ScenarioTag' -DefaultValue '')
                $resultOwnerArea = [string](Get-JsonValue -Object $iterationResult -Name 'OwnerArea' -DefaultValue '')
                $plannedOwnerArea = [string](Get-JsonValue -Object $plannedForIteration -Name 'OwnerArea' -DefaultValue '')
                $resultCommandAckPattern = [string](Get-JsonValue -Object $iterationResult -Name 'CommandAckPattern' -DefaultValue '')
                $plannedCommandAckPattern = [string](Get-JsonValue -Object $plannedForIteration -Name 'CommandAckPattern' -DefaultValue '')
                Add-Check -Name "${iterationName}_scenario_matches_plan" -Passed ([string]::Equals($resultScenario, $planScenario, [System.StringComparison]::Ordinal)) -Detail 'iteration-result.json Scenario must match monkey-plan.json Scenario'
                Add-Check -Name "${iterationName}_command_matches_plan" -Passed ([string]::Equals($resultCommand, $plannedCommand, [System.StringComparison]::Ordinal)) -Detail 'iteration-result.json Command must match monkey-plan.json PlannedCommands'
                Add-Check -Name "${iterationName}_command_index_matches_plan" -Passed ($resultCommandIndex -eq $plannedCommandIndex) -Detail 'iteration-result.json CommandIndex must match monkey-plan.json PlannedCommands'
                Add-Check -Name "${iterationName}_command_selection_mode_matches_plan" -Passed ([string]::Equals($resultCommandSelectionMode, $plannedCommandSelectionMode, [System.StringComparison]::Ordinal)) -Detail 'iteration-result.json CommandSelectionMode must match monkey-plan.json PlannedCommands'
                Add-Check -Name "${iterationName}_scenario_tag_matches_plan" -Passed ([string]::Equals($resultScenarioTag, $plannedScenarioTag, [System.StringComparison]::Ordinal)) -Detail 'iteration-result.json ScenarioTag must match monkey-plan.json PlannedCommands'
                Add-Check -Name "${iterationName}_owner_area_matches_plan" -Passed ([string]::Equals($resultOwnerArea, $plannedOwnerArea, [System.StringComparison]::Ordinal)) -Detail 'iteration-result.json OwnerArea must match monkey-plan.json PlannedCommands'
                Add-Check -Name "${iterationName}_command_ack_pattern_matches_plan" -Passed ([string]::Equals($resultCommandAckPattern, $plannedCommandAckPattern, [System.StringComparison]::Ordinal)) -Detail 'iteration-result.json CommandAckPattern must match monkey-plan.json PlannedCommands'
            }

            if ($null -ne $summaryForIteration) {
                $resultScenario = [string](Get-JsonValue -Object $iterationResult -Name 'Scenario' -DefaultValue '')
                $summaryScenario = [string](Get-JsonValue -Object $summaryForIteration -Name 'Scenario' -DefaultValue '')
                $resultCommand = [string](Get-JsonValue -Object $iterationResult -Name 'Command' -DefaultValue '')
                $summaryCommand = [string](Get-JsonValue -Object $summaryForIteration -Name 'Command' -DefaultValue '')
                $resultCommandSelectionMode = [string](Get-JsonValue -Object $iterationResult -Name 'CommandSelectionMode' -DefaultValue '')
                $summaryCommandSelectionMode = [string](Get-JsonValue -Object $summaryForIteration -Name 'CommandSelectionMode' -DefaultValue '')
                $resultScenarioTag = [string](Get-JsonValue -Object $iterationResult -Name 'ScenarioTag' -DefaultValue '')
                $summaryScenarioTag = [string](Get-JsonValue -Object $summaryForIteration -Name 'ScenarioTag' -DefaultValue '')
                $resultOwnerArea = [string](Get-JsonValue -Object $iterationResult -Name 'OwnerArea' -DefaultValue '')
                $summaryOwnerArea = [string](Get-JsonValue -Object $summaryForIteration -Name 'OwnerArea' -DefaultValue '')
                $resultCommandAckPattern = [string](Get-JsonValue -Object $iterationResult -Name 'CommandAckPattern' -DefaultValue '')
                $summaryCommandAckPattern = [string](Get-JsonValue -Object $summaryForIteration -Name 'CommandAckPattern' -DefaultValue '')
                $resultCommandAckRequired = [bool](Get-JsonValue -Object $iterationResult -Name 'CommandAckRequired' -DefaultValue $false)
                $summaryCommandAckRequired = [bool](Get-JsonValue -Object $summaryForIteration -Name 'CommandAckRequired' -DefaultValue $false)
                $resultPassed = [bool](Get-JsonValue -Object $iterationResult -Name 'Passed' -DefaultValue $false)
                $summaryPassed = [bool](Get-JsonValue -Object $summaryForIteration -Name 'Passed' -DefaultValue $false)
                $resultCommandAckObserved = [bool](Get-JsonValue -Object $iterationResult -Name 'CommandAckObserved' -DefaultValue $false)
                $summaryCommandAckObserved = [bool](Get-JsonValue -Object $summaryForIteration -Name 'CommandAckObserved' -DefaultValue $false)
                Add-Check -Name "${iterationName}_summary_result_scenario_matches_iteration" -Passed ([string]::Equals($summaryScenario, $resultScenario, [System.StringComparison]::Ordinal)) -Detail 'monkey-summary.json Results Scenario must match iteration-result.json'
                Add-Check -Name "${iterationName}_summary_result_command_matches_iteration" -Passed ([string]::Equals($summaryCommand, $resultCommand, [System.StringComparison]::Ordinal)) -Detail 'monkey-summary.json Results Command must match iteration-result.json'
                Add-Check -Name "${iterationName}_summary_result_command_selection_mode_matches_iteration" -Passed ([string]::Equals($summaryCommandSelectionMode, $resultCommandSelectionMode, [System.StringComparison]::Ordinal)) -Detail 'monkey-summary.json Results CommandSelectionMode must match iteration-result.json'
                Add-Check -Name "${iterationName}_summary_result_scenario_tag_matches_iteration" -Passed ([string]::Equals($summaryScenarioTag, $resultScenarioTag, [System.StringComparison]::Ordinal)) -Detail 'monkey-summary.json Results ScenarioTag must match iteration-result.json'
                Add-Check -Name "${iterationName}_summary_result_owner_area_matches_iteration" -Passed ([string]::Equals($summaryOwnerArea, $resultOwnerArea, [System.StringComparison]::Ordinal)) -Detail 'monkey-summary.json Results OwnerArea must match iteration-result.json'
                Add-Check -Name "${iterationName}_summary_result_command_ack_pattern_matches_iteration" -Passed ([string]::Equals($summaryCommandAckPattern, $resultCommandAckPattern, [System.StringComparison]::Ordinal)) -Detail 'monkey-summary.json Results CommandAckPattern must match iteration-result.json'
                Add-Check -Name "${iterationName}_summary_result_command_ack_required_matches_iteration" -Passed ($summaryCommandAckRequired -eq $resultCommandAckRequired) -Detail 'monkey-summary.json Results CommandAckRequired must match iteration-result.json'
                Add-Check -Name "${iterationName}_summary_result_passed_matches_iteration" -Passed ($summaryPassed -eq $resultPassed) -Detail 'monkey-summary.json Results Passed must match iteration-result.json'
                Add-Check -Name "${iterationName}_summary_result_command_ack_observed_matches_iteration" -Passed ($summaryCommandAckObserved -eq $resultCommandAckObserved) -Detail 'monkey-summary.json Results CommandAckObserved must match iteration-result.json'
            }

            $probeSamplesPath = Resolve-ChildOrAbsolutePath -BaseDir $iterationDir -Path ([string](Get-JsonValue -Object $iterationResult -Name 'RuntimeProbeSamplesPath' -DefaultValue ''))
            $probeSamplesExist = $probeSamplesPath -and (Test-Path -LiteralPath $probeSamplesPath -PathType Leaf)
            $probeSamplesUnderIteration = $probeSamplesPath -and (Test-PathUnderDirectory -Path $probeSamplesPath -Directory $iterationDir)
            $probeSamplesLeafExpected = $probeSamplesPath -and ([System.IO.Path]::GetFileName($probeSamplesPath) -eq 'runtime-probe-samples.json')
            Add-Check -Name "${iterationName}_runtime_probe_samples_under_iteration_dir" -Passed $probeSamplesUnderIteration -Detail 'RuntimeProbeSamplesPath must stay inside the current iteration directory'
            Add-Check -Name "${iterationName}_runtime_probe_samples_leaf_expected" -Passed $probeSamplesLeafExpected -Detail 'RuntimeProbeSamplesPath must end with runtime-probe-samples.json'
            Add-Check -Name "${iterationName}_runtime_probe_samples_path_matches_retained_file" -Passed ($probeSamplesPath -and ([System.StringComparer]::OrdinalIgnoreCase.Equals($probeSamplesPath, [System.IO.Path]::GetFullPath($probeSamplesCandidate)))) -Detail 'RuntimeProbeSamplesPath must point to the retained runtime-probe-samples.json file'
            Add-Check -Name "${iterationName}_runtime_probe_samples_exist" -Passed $probeSamplesExist -Detail 'requires retained runtime-probe-samples.json'
            if ($probeSamplesExist) {
                try {
                    $probeSamplesJson = [System.IO.File]::ReadAllText($probeSamplesPath)
                    $probeSamplesParsed = $probeSamplesJson | ConvertFrom-Json
                    $probeSamples = @($probeSamplesParsed)
                    Add-Check -Name "${iterationName}_runtime_probe_samples_non_empty" -Passed ($probeSamples.Count -gt 0) -Detail 'runtime-probe-samples.json must contain samples'
                    Add-Check -Name "${iterationName}_runtime_probe_samples_phase_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'Phase') -Detail 'every probe sample must retain Phase'
                    $startupMainMenuProbeSamples = @($probeSamples | Where-Object {
                        [string]::Equals([string](Get-JsonValue -Object $_ -Name 'Phase' -DefaultValue ''), 'StartupMainMenu', [System.StringComparison]::Ordinal)
                    })
                    $postCommandRuntimeProbeSamples = @($probeSamples | Where-Object {
                        [string]::Equals([string](Get-JsonValue -Object $_ -Name 'Phase' -DefaultValue ''), 'PostCommandRuntime', [System.StringComparison]::Ordinal)
                    })
                    $unknownRuntimeProbePhaseSamples = @($probeSamples | Where-Object {
                        $phase = [string](Get-JsonValue -Object $_ -Name 'Phase' -DefaultValue '')
                        -not ([string]::Equals($phase, 'StartupMainMenu', [System.StringComparison]::Ordinal) -or
                            [string]::Equals($phase, 'PostCommandRuntime', [System.StringComparison]::Ordinal))
                    })
                    $mainMenuObservationForProbeSamples = Get-JsonValue -Object $iterationResult -Name 'MainMenuObservation' -DefaultValue $null
                    $runtimeObservationForProbeSamples = Get-JsonValue -Object $iterationResult -Name 'RuntimeObservation' -DefaultValue $null
                    $mainMenuObservationSampleCount = if ($null -ne $mainMenuObservationForProbeSamples) { [int](Get-JsonValue -Object $mainMenuObservationForProbeSamples -Name 'Samples' -DefaultValue -1) } else { -1 }
                    $runtimeObservationSampleCount = if ($null -ne $runtimeObservationForProbeSamples) { [int](Get-JsonValue -Object $runtimeObservationForProbeSamples -Name 'Samples' -DefaultValue -1) } else { -1 }
                    Add-Check -Name "${iterationName}_runtime_probe_samples_allowed_phase_values" -Passed ($unknownRuntimeProbePhaseSamples.Count -eq 0) -Detail "runtime-probe-samples.json phases must be StartupMainMenu or PostCommandRuntime; unknownCount=$($unknownRuntimeProbePhaseSamples.Count)"
                    Add-Check -Name "${iterationName}_runtime_probe_samples_startup_main_menu_phase_observed" -Passed ($startupMainMenuProbeSamples.Count -gt 0) -Detail 'runtime-probe-samples.json must retain at least one StartupMainMenu sample'
                    Add-Check -Name "${iterationName}_runtime_probe_samples_post_command_runtime_phase_observed" -Passed ($postCommandRuntimeProbeSamples.Count -gt 0) -Detail 'runtime-probe-samples.json must retain at least one PostCommandRuntime sample'
                    Add-Check -Name "${iterationName}_runtime_probe_samples_startup_count_matches_main_menu_observation" -Passed ($mainMenuObservationSampleCount -ge 0 -and $startupMainMenuProbeSamples.Count -eq $mainMenuObservationSampleCount) -Detail "StartupMainMenu sample count must match MainMenuObservation.Samples; expected=$mainMenuObservationSampleCount actual=$($startupMainMenuProbeSamples.Count)"
                    Add-Check -Name "${iterationName}_runtime_probe_samples_runtime_count_matches_runtime_observation" -Passed ($runtimeObservationSampleCount -ge 0 -and $postCommandRuntimeProbeSamples.Count -eq $runtimeObservationSampleCount) -Detail "PostCommandRuntime sample count must match RuntimeObservation.Samples; expected=$runtimeObservationSampleCount actual=$($postCommandRuntimeProbeSamples.Count)"
                    Add-Check -Name "${iterationName}_runtime_probe_samples_sampled_at_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'SampledAt') -Detail 'every probe sample must retain SampledAt'
                    Add-Check -Name "${iterationName}_runtime_probe_samples_log_exists_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'LogExists') -Detail 'every probe sample must retain LogExists'
                    Add-Check -Name "${iterationName}_runtime_probe_samples_log_length_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'LogLengthBytes') -Detail 'every probe sample must retain LogLengthBytes'
                    Add-Check -Name "${iterationName}_runtime_probe_samples_log_last_write_field_present" -Passed (Test-AllJsonPropertiesRetained -Items $probeSamples -Name 'LogLastWriteTimeUtc') -Detail 'every probe sample must retain LogLastWriteTimeUtc, even when the value is null before log creation'
                    $invalidSampledAtProbeSamples = @($probeSamples | Where-Object {
                        $null -eq (ConvertTo-DateTimeUtcOrNull -Value (Get-JsonValue -Object $_ -Name 'SampledAt' -DefaultValue $null))
                    })
                    $invalidLogLastWriteProbeSamples = @($probeSamples | Where-Object {
                        [bool](Get-JsonValue -Object $_ -Name 'LogExists' -DefaultValue $false) -and
                            $null -eq (ConvertTo-DateTimeUtcOrNull -Value (Get-JsonValue -Object $_ -Name 'LogLastWriteTimeUtc' -DefaultValue $null))
                    })
                    $futureLogLastWriteProbeSamples = @($probeSamples | Where-Object {
                        $logExists = [bool](Get-JsonValue -Object $_ -Name 'LogExists' -DefaultValue $false)
                        $sampledAtUtc = ConvertTo-DateTimeUtcOrNull -Value (Get-JsonValue -Object $_ -Name 'SampledAt' -DefaultValue $null)
                        $logLastWriteUtc = ConvertTo-DateTimeUtcOrNull -Value (Get-JsonValue -Object $_ -Name 'LogLastWriteTimeUtc' -DefaultValue $null)
                        $logExists -and $null -ne $sampledAtUtc -and $null -ne $logLastWriteUtc -and $logLastWriteUtc -gt $sampledAtUtc
                    })
                    Add-Check -Name "${iterationName}_runtime_probe_samples_sampled_at_parseable" -Passed ($invalidSampledAtProbeSamples.Count -eq 0) -Detail "every probe sample SampledAt must be parseable; invalidCount=$($invalidSampledAtProbeSamples.Count)"
                    Add-Check -Name "${iterationName}_runtime_probe_samples_log_last_write_parseable_when_log_exists" -Passed ($invalidLogLastWriteProbeSamples.Count -eq 0) -Detail "LogLastWriteTimeUtc must be parseable when LogExists=true; invalidCount=$($invalidLogLastWriteProbeSamples.Count)"
                    Add-Check -Name "${iterationName}_runtime_probe_samples_log_last_write_not_after_sampled_at" -Passed ($futureLogLastWriteProbeSamples.Count -eq 0) -Detail "LogLastWriteTimeUtc must not be later than SampledAt; invalidCount=$($futureLogLastWriteProbeSamples.Count)"
                    $sampledAtRegressionCount = 0
                    $phaseOrderDefectCount = 0
                    $logLengthRegressionCount = 0
                    $negativeLogLengthProbeSamples = @($probeSamples | Where-Object {
                        [bool](Get-JsonValue -Object $_ -Name 'LogExists' -DefaultValue $false) -and
                            [long](Get-JsonValue -Object $_ -Name 'LogLengthBytes' -DefaultValue -1) -lt 0
                    })
                    $previousSampledAtUtc = $null
                    $previousLogLengthBytes = $null
                    $runtimePhaseSeen = $false
                    foreach ($probeSample in $probeSamples) {
                        $sampledAtUtc = ConvertTo-DateTimeUtcOrNull -Value (Get-JsonValue -Object $probeSample -Name 'SampledAt' -DefaultValue $null)
                        if ($null -ne $sampledAtUtc) {
                            if ($null -ne $previousSampledAtUtc -and $sampledAtUtc -lt $previousSampledAtUtc) {
                                $sampledAtRegressionCount++
                            }

                            $previousSampledAtUtc = $sampledAtUtc
                        }

                        $phase = [string](Get-JsonValue -Object $probeSample -Name 'Phase' -DefaultValue '')
                        if ([string]::Equals($phase, 'PostCommandRuntime', [System.StringComparison]::Ordinal)) {
                            $runtimePhaseSeen = $true
                        } elseif ([string]::Equals($phase, 'StartupMainMenu', [System.StringComparison]::Ordinal) -and $runtimePhaseSeen) {
                            $phaseOrderDefectCount++
                        }

                        if ([bool](Get-JsonValue -Object $probeSample -Name 'LogExists' -DefaultValue $false)) {
                            $logLengthBytes = [long](Get-JsonValue -Object $probeSample -Name 'LogLengthBytes' -DefaultValue -1)
                            if ($logLengthBytes -ge 0) {
                                if ($null -ne $previousLogLengthBytes -and $logLengthBytes -lt $previousLogLengthBytes) {
                                    $logLengthRegressionCount++
                                }

                                $previousLogLengthBytes = $logLengthBytes
                            }
                        }
                    }

                    Add-Check -Name "${iterationName}_runtime_probe_samples_sampled_at_nondecreasing" -Passed ($sampledAtRegressionCount -eq 0) -Detail "runtime-probe-samples.json SampledAt values must be retained in nondecreasing order; regressionCount=$sampledAtRegressionCount"
                    Add-Check -Name "${iterationName}_runtime_probe_samples_phase_ordered" -Passed ($phaseOrderDefectCount -eq 0) -Detail "StartupMainMenu samples must not appear after PostCommandRuntime samples; defectCount=$phaseOrderDefectCount"
                    Add-Check -Name "${iterationName}_runtime_probe_samples_log_length_nonnegative_when_log_exists" -Passed ($negativeLogLengthProbeSamples.Count -eq 0) -Detail "LogLengthBytes must be non-negative when LogExists=true; invalidCount=$($negativeLogLengthProbeSamples.Count)"
                    Add-Check -Name "${iterationName}_runtime_probe_samples_log_length_nondecreasing_when_log_exists" -Passed ($logLengthRegressionCount -eq 0) -Detail "LogLengthBytes must not regress across retained LogExists=true samples; regressionCount=$logLengthRegressionCount"
                    $runtimeObservationInitialLogLengthForProbeSamples = if ($null -ne $runtimeObservationForProbeSamples) { [long](Get-JsonValue -Object $runtimeObservationForProbeSamples -Name 'LogInitialLengthBytes' -DefaultValue -1) } else { -1L }
                    $runtimeObservationLogGrewForProbeSamples = if ($null -ne $runtimeObservationForProbeSamples) { [bool](Get-JsonValue -Object $runtimeObservationForProbeSamples -Name 'LogGrew' -DefaultValue $false) } else { $false }
                    $postCommandRuntimeProbeLogLengths = @($postCommandRuntimeProbeSamples |
                        Where-Object { [bool](Get-JsonValue -Object $_ -Name 'LogExists' -DefaultValue $false) } |
                        ForEach-Object { [long](Get-JsonValue -Object $_ -Name 'LogLengthBytes' -DefaultValue -1) } |
                        Where-Object { $_ -ge 0 })
                    $postCommandRuntimeProbeMaxLogLength = if ($postCommandRuntimeProbeLogLengths.Count -gt 0) {
                        [long](@($postCommandRuntimeProbeLogLengths | Sort-Object -Descending)[0])
                    } else {
                        -1L
                    }
                    $runtimeProbeLogGrowthMatchesObservation = -not ($runtimeLogGrowthRequiredForIteration -and $runtimeObservationLogGrewForProbeSamples) -or
                        ($runtimeObservationInitialLogLengthForProbeSamples -ge 0 -and $postCommandRuntimeProbeMaxLogLength -gt $runtimeObservationInitialLogLengthForProbeSamples)
                    Add-Check -Name "${iterationName}_runtime_probe_samples_log_growth_matches_runtime_observation" -Passed $runtimeProbeLogGrowthMatchesObservation -Detail "PostCommandRuntime probe LogLengthBytes must prove RuntimeObservation.LogGrew; initial=$runtimeObservationInitialLogLengthForProbeSamples maxRuntimeSample=$postCommandRuntimeProbeMaxLogLength"
                    Add-Check -Name "${iterationName}_runtime_probe_samples_process_id_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'ProcessId') -Detail 'every probe sample must retain ProcessId'
                    Add-Check -Name "${iterationName}_runtime_probe_samples_process_start_time_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'ProcessStartTimeUtc') -Detail 'every probe sample must retain ProcessStartTimeUtc'
                    Add-Check -Name "${iterationName}_runtime_probe_samples_process_path_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'ProcessPath') -Detail 'every probe sample must retain ProcessPath'
                    Add-Check -Name "${iterationName}_runtime_probe_samples_expected_process_id_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'ExpectedGameProcessId') -Detail 'every probe sample must retain ExpectedGameProcessId'
                    Add-Check -Name "${iterationName}_runtime_probe_samples_expected_process_start_time_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'ExpectedGameProcessStartTimeUtc') -Detail 'every probe sample must retain ExpectedGameProcessStartTimeUtc'
                    Add-Check -Name "${iterationName}_runtime_probe_samples_expected_process_path_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'ExpectedGameProcessPath') -Detail 'every probe sample must retain ExpectedGameProcessPath'
                    Add-Check -Name "${iterationName}_runtime_probe_samples_process_id_match_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'ProcessIdMatchesExpected') -Detail 'every probe sample must retain ProcessIdMatchesExpected'
                    Add-Check -Name "${iterationName}_runtime_probe_samples_process_start_time_match_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'ProcessStartTimeMatchesExpected') -Detail 'every probe sample must retain ProcessStartTimeMatchesExpected'
                    Add-Check -Name "${iterationName}_runtime_probe_samples_process_path_match_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'ProcessPathMatchesExpected') -Detail 'every probe sample must retain ProcessPathMatchesExpected'
                    Add-Check -Name "${iterationName}_runtime_probe_samples_process_identity_match_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'ProcessIdentityMatchesExpected') -Detail 'every probe sample must retain ProcessIdentityMatchesExpected'
                    Add-Check -Name "${iterationName}_runtime_probe_samples_process_observed_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'ProcessObserved') -Detail 'every probe sample must retain ProcessObserved'
                    Add-Check -Name "${iterationName}_runtime_probe_samples_main_window_observed_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'MainWindowObserved') -Detail 'every probe sample must retain MainWindowObserved'
                    Add-Check -Name "${iterationName}_runtime_probe_samples_hung_window_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'HungWindow') -Detail 'every probe sample must retain HungWindow'
                    Add-Check -Name "${iterationName}_runtime_probe_samples_responding_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'Responding') -Detail 'every probe sample must retain Responding'
                    Add-Check -Name "${iterationName}_runtime_probe_samples_stale_process_count_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'StaleProcessCount') -Detail 'every probe sample must retain StaleProcessCount'
                    Add-Check -Name "${iterationName}_runtime_probe_samples_current_process_count_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'CurrentProcessCount') -Detail 'every probe sample must retain CurrentProcessCount'
                    Add-Check -Name "${iterationName}_runtime_probe_samples_unknown_start_time_count_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'UnknownStartTimeProcessCount') -Detail 'every probe sample must retain UnknownStartTimeProcessCount'
                    Add-Check -Name "${iterationName}_runtime_probe_samples_ambiguous_current_process_count_field_present" -Passed (Test-AllJsonPropertiesPresent -Items $probeSamples -Name 'AmbiguousCurrentProcessCount') -Detail 'every probe sample must retain AmbiguousCurrentProcessCount'
                    Add-Check -Name "${iterationName}_runtime_probe_samples_process_observed" -Passed (Test-AnyJsonPropertyTrue -Items $probeSamples -Name 'ProcessObserved') -Detail 'at least one probe sample must observe SlayTheSpire2'
                    Add-Check -Name "${iterationName}_runtime_probe_samples_main_window_observed" -Passed (Test-AnyJsonPropertyTrue -Items $probeSamples -Name 'MainWindowObserved') -Detail 'at least one probe sample must observe the main game window'
                    Add-Check -Name "${iterationName}_runtime_probe_samples_no_hung_window" -Passed (Test-NoJsonPropertyTrue -Items $probeSamples -Name 'HungWindow') -Detail 'probe samples must not report hung windows'
                    Add-Check -Name "${iterationName}_runtime_probe_samples_no_not_responding" -Passed (Test-NoJsonPropertyFalse -Items $probeSamples -Name 'Responding') -Detail 'probe samples must not report Responding=false'
                    $staleProcessSamples = @($probeSamples | Where-Object { [int](Get-JsonValue -Object $_ -Name 'StaleProcessCount' -DefaultValue -1) -ne 0 })
                    Add-Check -Name "${iterationName}_runtime_probe_samples_no_stale_processes" -Passed ($staleProcessSamples.Count -eq 0) -Detail 'probe samples must record StaleProcessCount=0 so shared godot.log evidence cannot come from a pre-existing process'
                    $unknownStartTimeSamples = @($probeSamples | Where-Object { [int](Get-JsonValue -Object $_ -Name 'UnknownStartTimeProcessCount' -DefaultValue -1) -ne 0 })
                    Add-Check -Name "${iterationName}_runtime_probe_samples_no_unknown_start_times" -Passed ($unknownStartTimeSamples.Count -eq 0) -Detail 'probe samples must record UnknownStartTimeProcessCount=0 so unreadable process start times cannot be treated as current'
                    $ambiguousProcessSamples = @($probeSamples | Where-Object { [int](Get-JsonValue -Object $_ -Name 'AmbiguousCurrentProcessCount' -DefaultValue -1) -ne 0 })
                    Add-Check -Name "${iterationName}_runtime_probe_samples_no_ambiguous_current_processes" -Passed ($ambiguousProcessSamples.Count -eq 0) -Detail 'probe samples must record AmbiguousCurrentProcessCount=0 so evidence binds to one launched process'
                    $nonSingleCurrentProcessSamples = @($probeSamples | Where-Object { [int](Get-JsonValue -Object $_ -Name 'CurrentProcessCount' -DefaultValue -1) -ne 1 })
                    Add-Check -Name "${iterationName}_runtime_probe_samples_single_current_process" -Passed ($nonSingleCurrentProcessSamples.Count -eq 0) -Detail 'probe samples must record CurrentProcessCount=1 so evidence binds to the launched process'
                    $observedRuntimeProbeProcessIds = @($probeSamples |
                        Where-Object { [bool](Get-JsonValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object { [int](Get-JsonValue -Object $_ -Name 'ProcessId' -DefaultValue 0) } |
                        Where-Object { $_ -gt 0 } |
                        Sort-Object -Unique)
                    Add-Check -Name "${iterationName}_runtime_probe_samples_single_positive_process_id" -Passed ($observedRuntimeProbeProcessIds.Count -eq 1) -Detail "observed probe samples must bind to one positive process id; count=$($observedRuntimeProbeProcessIds.Count) values=$($observedRuntimeProbeProcessIds -join ',')"
                    Add-Check -Name "${iterationName}_runtime_probe_samples_process_id_matches_result" -Passed ($observedRuntimeProbeProcessIds.Count -eq 1 -and $observedRuntimeProbeProcessIds[0] -eq $iterationGameProcessId) -Detail "runtime-probe-samples.json ProcessId must match iteration-result.json GameProcessId; result=$iterationGameProcessId observed=$($observedRuntimeProbeProcessIds -join ',')"
                    $observedProbeStartTimes = @($probeSamples |
                        Where-Object { [bool](Get-JsonValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object { ConvertTo-DateTimeUtcOrNull -Value (Get-JsonValue -Object $_ -Name 'ProcessStartTimeUtc' -DefaultValue $null) } |
                        Where-Object { $null -ne $_ } |
                        ForEach-Object { $_.ToString('o') } |
                        Sort-Object -Unique)
                    $observedProbePaths = @($probeSamples |
                        Where-Object { [bool](Get-JsonValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object { ConvertTo-NormalizedPathOrEmpty -Path ([string](Get-JsonValue -Object $_ -Name 'ProcessPath' -DefaultValue '')) } |
                        Where-Object { -not [string]::IsNullOrWhiteSpace($_) } |
                        Sort-Object -Unique)
                    $observedProbeExpectedProcessIds = @($probeSamples |
                        Where-Object { [bool](Get-JsonValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object { [int](Get-JsonValue -Object $_ -Name 'ExpectedGameProcessId' -DefaultValue 0) } |
                        Where-Object { $_ -gt 0 } |
                        Sort-Object -Unique)
                    $observedProbeExpectedStartTimes = @($probeSamples |
                        Where-Object { [bool](Get-JsonValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object { ConvertTo-DateTimeUtcOrNull -Value (Get-JsonValue -Object $_ -Name 'ExpectedGameProcessStartTimeUtc' -DefaultValue $null) } |
                        Where-Object { $null -ne $_ } |
                        ForEach-Object { $_.ToString('o') } |
                        Sort-Object -Unique)
                    $observedProbeExpectedPaths = @($probeSamples |
                        Where-Object { [bool](Get-JsonValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) } |
                        ForEach-Object { ConvertTo-NormalizedPathOrEmpty -Path ([string](Get-JsonValue -Object $_ -Name 'ExpectedGameProcessPath' -DefaultValue '')) } |
                        Where-Object { -not [string]::IsNullOrWhiteSpace($_) } |
                        Sort-Object -Unique)
                    $identityMismatchProbeSamples = @($probeSamples | Where-Object {
                        [bool](Get-JsonValue -Object $_ -Name 'ProcessObserved' -DefaultValue $false) -and
                        (-not [bool](Get-JsonValue -Object $_ -Name 'ProcessIdMatchesExpected' -DefaultValue $false) -or
                            -not [bool](Get-JsonValue -Object $_ -Name 'ProcessStartTimeMatchesExpected' -DefaultValue $false) -or
                            -not [bool](Get-JsonValue -Object $_ -Name 'ProcessPathMatchesExpected' -DefaultValue $false) -or
                            -not [bool](Get-JsonValue -Object $_ -Name 'ProcessIdentityMatchesExpected' -DefaultValue $false))
                    })
                    Add-Check -Name "${iterationName}_runtime_probe_samples_single_process_start_time" -Passed ($observedProbeStartTimes.Count -eq 1) -Detail "observed probe samples must bind to one process start time; count=$($observedProbeStartTimes.Count) values=$($observedProbeStartTimes -join ',')"
                    Add-Check -Name "${iterationName}_runtime_probe_samples_process_start_time_matches_result" -Passed ($observedProbeStartTimes.Count -eq 1 -and $null -ne $resultGameProcessStartTimeUtc -and [string]::Equals($observedProbeStartTimes[0], $resultGameProcessStartTimeText, [System.StringComparison]::Ordinal)) -Detail "observed probe samples must retain the same ProcessStartTimeUtc as iteration-result.json; result=$resultGameProcessStartTimeText observed=$($observedProbeStartTimes -join ',')"
                    Add-Check -Name "${iterationName}_runtime_probe_samples_single_process_path" -Passed ($observedProbePaths.Count -eq 1) -Detail "observed probe samples must bind to one process path; count=$($observedProbePaths.Count) values=$($observedProbePaths -join ',')"
                    Add-Check -Name "${iterationName}_runtime_probe_samples_process_path_matches_result" -Passed ($observedProbePaths.Count -eq 1 -and -not [string]::IsNullOrWhiteSpace($resultGameProcessPathFull) -and [System.StringComparer]::OrdinalIgnoreCase.Equals($observedProbePaths[0], $resultGameProcessPathFull)) -Detail "observed probe samples must retain the same ProcessPath as iteration-result.json; result=$resultGameProcessPathFull observed=$($observedProbePaths -join ',')"
                    Add-Check -Name "${iterationName}_runtime_probe_samples_expected_process_id_matches_live_session" -Passed ($observedProbeExpectedProcessIds.Count -eq 1 -and $observedProbeExpectedProcessIds[0] -eq $resultLiveSessionSelectedGameProcessId) -Detail "observed probe samples ExpectedGameProcessId must match LiveSessionSelectedGameProcessId; liveSession=$resultLiveSessionSelectedGameProcessId observed=$($observedProbeExpectedProcessIds -join ',')"
                    Add-Check -Name "${iterationName}_runtime_probe_samples_expected_process_start_time_matches_live_session" -Passed ($observedProbeExpectedStartTimes.Count -eq 1 -and $null -ne $resultLiveSessionSelectedGameProcessStartTimeUtc -and [string]::Equals($observedProbeExpectedStartTimes[0], $resultLiveSessionSelectedGameProcessStartTimeText, [System.StringComparison]::Ordinal)) -Detail "observed probe samples ExpectedGameProcessStartTimeUtc must match LiveSessionSelectedGameProcessStartTimeUtc; liveSession=$resultLiveSessionSelectedGameProcessStartTimeText observed=$($observedProbeExpectedStartTimes -join ',')"
                    Add-Check -Name "${iterationName}_runtime_probe_samples_expected_process_path_matches_live_session" -Passed ($observedProbeExpectedPaths.Count -eq 1 -and -not [string]::IsNullOrWhiteSpace($resultLiveSessionSelectedGameProcessPathFull) -and [System.StringComparer]::OrdinalIgnoreCase.Equals($observedProbeExpectedPaths[0], $resultLiveSessionSelectedGameProcessPathFull)) -Detail "observed probe samples ExpectedGameProcessPath must match LiveSessionSelectedGameProcessPath; liveSession=$resultLiveSessionSelectedGameProcessPathFull observed=$($observedProbeExpectedPaths -join ',')"
                    Add-Check -Name "${iterationName}_runtime_probe_samples_all_match_live_session_identity" -Passed ($identityMismatchProbeSamples.Count -eq 0) -Detail 'observed probe samples must report ProcessIdMatchesExpected, ProcessStartTimeMatchesExpected, ProcessPathMatchesExpected, and ProcessIdentityMatchesExpected as true'
                } catch {
                    Add-Check -Name "${iterationName}_runtime_probe_samples_json_valid" -Passed $false -Detail "invalid probe samples JSON in $probeSamplesPath`: $($_.Exception.Message)"
                }
            }

            $mainMenuObservation = Get-JsonValue -Object $iterationResult -Name 'MainMenuObservation' -DefaultValue $null
            Add-Check -Name "${iterationName}_main_menu_observation_exists" -Passed ($null -ne $mainMenuObservation) -Detail 'requires MainMenuObservation telemetry'
            if ($null -ne $mainMenuObservation) {
                Add-Check -Name "${iterationName}_main_menu_observation_main_menu_reached" -Passed ([bool](Get-JsonValue -Object $mainMenuObservation -Name 'MainMenuReached' -DefaultValue $false)) -Detail 'MainMenuObservation.MainMenuReached must be true'
                Add-Check -Name "${iterationName}_main_menu_observation_process_observed" -Passed ([bool](Get-JsonValue -Object $mainMenuObservation -Name 'ProcessObserved' -DefaultValue $false)) -Detail 'MainMenuObservation.ProcessObserved must be true'
                Add-Check -Name "${iterationName}_main_menu_observation_no_process_exit" -Passed (-not [bool](Get-JsonValue -Object $mainMenuObservation -Name 'ProcessExitedAfterObservation' -DefaultValue $true)) -Detail 'process must not disappear before main menu'
                Add-Check -Name "${iterationName}_main_menu_observation_no_hung_window" -Passed (-not [bool](Get-JsonValue -Object $mainMenuObservation -Name 'HungWindowDetected' -DefaultValue $true)) -Detail 'window must not be reported hung before main menu'
                Add-Check -Name "${iterationName}_main_menu_observation_no_stale_process" -Passed (-not [bool](Get-JsonValue -Object $mainMenuObservation -Name 'StaleProcessObserved' -DefaultValue $true)) -Detail 'main-menu observation must not see stale pre-existing SlayTheSpire2 processes'
                Add-Check -Name "${iterationName}_main_menu_observation_stale_process_count_zero" -Passed ([int](Get-JsonValue -Object $mainMenuObservation -Name 'MaxStaleProcessCount' -DefaultValue -1) -eq 0) -Detail 'main-menu observation MaxStaleProcessCount must be 0'
                Add-Check -Name "${iterationName}_main_menu_observation_no_log_growth_timeout" -Passed (-not [bool](Get-JsonValue -Object $mainMenuObservation -Name 'NoLogGrowthTimeoutExceeded' -DefaultValue $true)) -Detail 'godot.log must not stall before main menu'
                Add-Check -Name "${iterationName}_main_menu_observation_log_observed" -Passed ([bool](Get-JsonValue -Object $mainMenuObservation -Name 'LogObserved' -DefaultValue $false)) -Detail 'MainMenuObservation.LogObserved must be true'
            }

            $runtimeObservation = Get-JsonValue -Object $iterationResult -Name 'RuntimeObservation' -DefaultValue $null
            Add-Check -Name "${iterationName}_runtime_observation_exists" -Passed ($null -ne $runtimeObservation) -Detail 'requires RuntimeObservation telemetry'
            if ($null -ne $runtimeObservation) {
                Add-Check -Name "${iterationName}_runtime_observation_passed_detail" -Passed ([bool](Get-JsonValue -Object $runtimeObservation -Name 'Passed' -DefaultValue $false)) -Detail 'RuntimeObservation.Passed must be true'
                Add-Check -Name "${iterationName}_runtime_observation_process_observed" -Passed ([bool](Get-JsonValue -Object $runtimeObservation -Name 'ProcessObserved' -DefaultValue $false)) -Detail 'RuntimeObservation.ProcessObserved must be true'
                Add-Check -Name "${iterationName}_runtime_observation_no_process_exit" -Passed (-not [bool](Get-JsonValue -Object $runtimeObservation -Name 'ProcessExitedAfterObservation' -DefaultValue $true)) -Detail 'process must not disappear during runtime observation'
                Add-Check -Name "${iterationName}_runtime_observation_no_hung_window" -Passed (-not [bool](Get-JsonValue -Object $runtimeObservation -Name 'HungWindowDetected' -DefaultValue $true)) -Detail 'window must not be reported hung during runtime observation'
                Add-Check -Name "${iterationName}_runtime_observation_no_stale_process" -Passed (-not [bool](Get-JsonValue -Object $runtimeObservation -Name 'StaleProcessObserved' -DefaultValue $true)) -Detail 'runtime observation must not see stale pre-existing SlayTheSpire2 processes'
                Add-Check -Name "${iterationName}_runtime_observation_stale_process_count_zero" -Passed ([int](Get-JsonValue -Object $runtimeObservation -Name 'MaxStaleProcessCount' -DefaultValue -1) -eq 0) -Detail 'runtime observation MaxStaleProcessCount must be 0'
                Add-Check -Name "${iterationName}_runtime_observation_log_observed" -Passed ([bool](Get-JsonValue -Object $runtimeObservation -Name 'LogObserved' -DefaultValue $false)) -Detail 'RuntimeObservation.LogObserved must be true'
                $runtimeObservationLogGrowthRequired = [bool](Get-JsonValue -Object $runtimeObservation -Name 'RuntimeLogGrowthRequired' -DefaultValue $runtimeLogGrowthRequiredForIteration)
                $runtimeObservationInitialLogLength = [long](Get-JsonValue -Object $runtimeObservation -Name 'LogInitialLengthBytes' -DefaultValue -1)
                $runtimeObservationFinalLogLength = [long](Get-JsonValue -Object $runtimeObservation -Name 'LogFinalLengthBytes' -DefaultValue -1)
                Add-Check -Name "${iterationName}_runtime_observation_log_growth_requirement_matches_command" -Passed ($runtimeObservationLogGrowthRequired -eq $runtimeLogGrowthRequiredForIteration) -Detail 'RuntimeObservation.RuntimeLogGrowthRequired must match whether the iteration sent a runtime command'
                Add-Check -Name "${iterationName}_runtime_observation_log_initial_length_present" -Passed (Test-JsonProperty -Object $runtimeObservation -Name 'LogInitialLengthBytes') -Detail 'RuntimeObservation must retain LogInitialLengthBytes'
                Add-Check -Name "${iterationName}_runtime_observation_log_final_length_present" -Passed (Test-JsonProperty -Object $runtimeObservation -Name 'LogFinalLengthBytes') -Detail 'RuntimeObservation must retain LogFinalLengthBytes'
                if ($runtimeLogGrowthRequiredForIteration) {
                    Add-Check -Name "${iterationName}_runtime_observation_log_grew" -Passed ([bool](Get-JsonValue -Object $runtimeObservation -Name 'LogGrew' -DefaultValue $false)) -Detail 'command-bearing RuntimeObservation.LogGrew must be true so a retained static godot.log cannot satisfy runtime health'
                    Add-Check -Name "${iterationName}_runtime_observation_log_length_growth_matches_log_grew" -Passed ($runtimeObservationInitialLogLength -ge 0 -and $runtimeObservationFinalLogLength -gt $runtimeObservationInitialLogLength) -Detail "RuntimeObservation LogFinalLengthBytes must exceed LogInitialLengthBytes when LogGrew is required; initial=$runtimeObservationInitialLogLength final=$runtimeObservationFinalLogLength"
                    Add-Check -Name "${iterationName}_runtime_observation_no_log_growth_timeout" -Passed (-not [bool](Get-JsonValue -Object $runtimeObservation -Name 'NoLogGrowthTimeoutExceeded' -DefaultValue $true)) -Detail 'godot.log must keep growing during command-bearing runtime observation'
                } else {
                    Add-Check -Name "${iterationName}_runtime_observation_log_growth_not_required" -Passed (-not $runtimeObservationLogGrowthRequired) -Detail 'StartupOnly/no-command observations do not require idle main-menu log growth'
                    Add-Check -Name "${iterationName}_runtime_observation_no_log_growth_timeout" -Passed (-not [bool](Get-JsonValue -Object $runtimeObservation -Name 'NoLogGrowthTimeoutExceeded' -DefaultValue $true)) -Detail 'no-command runtime observation must not report a log-growth timeout'
                }
            }
        }
    }

    $logText = ''
    if ($logExists) {
        $logItem = Get-Item -LiteralPath $logPath
        $logText = [System.IO.File]::ReadAllText($logPath)
        Add-Check -Name "${iterationName}_godot_log_non_empty" -Passed ($logItem.Length -gt 0 -and $logText.Length -gt 0) -Detail 'godot.log.after-launch must be non-empty'
    }

    $currentIterationLogText = ''
    if ($currentIterationLogExists) {
        $currentIterationLogItem = Get-Item -LiteralPath $currentIterationLogPath
        $currentIterationLogText = [System.IO.File]::ReadAllText($currentIterationLogPath)
        Add-Check -Name "${iterationName}_current_iteration_log_non_empty" -Passed ($currentIterationLogItem.Length -gt 0 -and $currentIterationLogText.Length -gt 0) -Detail 'godot.log.current-iteration must be non-empty'
        Add-Check -Name "${iterationName}_main_menu_log_line_present" -Passed ([regex]::IsMatch($currentIterationLogText, '\[Startup\] Time to main menu')) -Detail 'expected [Startup] Time to main menu in current-iteration log slice'

        if (-not [string]::IsNullOrWhiteSpace($ExpectedPackageVersion)) {
            Add-Check -Name "${iterationName}_expected_package_version_in_log" -Passed (Contains-Text -Text $currentIterationLogText -Needle $ExpectedPackageVersion) -Detail "expected package version '$ExpectedPackageVersion' in current-iteration log slice"
        }

        if (-not [string]::IsNullOrWhiteSpace($ExpectedGameVersion)) {
            $expectedGameMarker = if ($ExpectedGameVersion.StartsWith('v', [System.StringComparison]::OrdinalIgnoreCase)) { "release = $ExpectedGameVersion" } else { "release = v$ExpectedGameVersion" }
            Add-Check -Name "${iterationName}_expected_game_version_in_log" -Passed (Contains-Text -Text $currentIterationLogText -Needle $expectedGameMarker) -Detail "expected game marker '$expectedGameMarker' in current-iteration log slice"
        }

        if (-not [string]::IsNullOrWhiteSpace($ExpectedRitsuLibVersion) -and -not [string]::IsNullOrWhiteSpace($ExpectedRitsuCompatBranch)) {
            $expectedRitsuMarker = "RitsuLib Version: $ExpectedRitsuLibVersion [compat branch: $ExpectedRitsuCompatBranch]"
            Add-Check -Name "${iterationName}_expected_ritsulib_marker_in_log" -Passed (Contains-Text -Text $currentIterationLogText -Needle $expectedRitsuMarker) -Detail "expected RitsuLib marker '$expectedRitsuMarker' in current-iteration log slice"
        }

        if ($ExpectedPatchCount -gt 0) {
            $patchHits = Get-PatchCountLineHits -Text $currentIterationLogText -ExpectedCount $ExpectedPatchCount
            Add-Check -Name "${iterationName}_expected_patch_count_in_log" -Passed ($patchHits -gt 0) -Detail "expected Spire Plus patch-count markers for $ExpectedPatchCount applied and $ExpectedPatchCount registered patches in current-iteration log slice"
        }
    }

    if ($auditExists) {
        try {
            $auditSummary = Read-AuditSummary -Path $auditPath
            Add-Check -Name "${iterationName}_audit_clean" -Passed ([bool]$auditSummary.Clean) -Detail "audit must have zero dirty items and zero signature hits; dirty=$($auditSummary.DirtyItems), hits=$($auditSummary.SignatureHitCount)"
            $auditItemPaths = @($auditSummary.ItemPaths)
            $auditItemLengths = @($auditSummary.ItemLengths)
            $auditItemSha256s = @($auditSummary.ItemSha256s)
            $expectedAuditPath = [System.IO.Path]::GetFullPath($currentIterationLogPath)
            $expectedAuditLength = if ($currentIterationLogExists) { [long](Get-Item -LiteralPath $currentIterationLogPath).Length } else { -1L }
            $expectedAuditSha256 = Get-FileSha256OrEmpty -Path $currentIterationLogPath
            Add-Check -Name "${iterationName}_audit_has_single_scanned_path" -Passed ($auditItemPaths.Count -eq 1) -Detail "audit JSON must retain exactly one scanned Path; found $($auditItemPaths.Count)"
            Add-Check -Name "${iterationName}_audit_path_matches_current_iteration_log" -Passed ($auditItemPaths.Count -eq 1 -and [System.StringComparer]::OrdinalIgnoreCase.Equals([string]$auditItemPaths[0], $expectedAuditPath)) -Detail 'godot-log-audit.json must be produced from the retained godot.log.current-iteration slice'
            Add-Check -Name "${iterationName}_audit_has_single_length" -Passed ($auditItemLengths.Count -eq 1) -Detail "audit JSON must retain exactly one Length; found $($auditItemLengths.Count)"
            Add-Check -Name "${iterationName}_audit_length_matches_current_iteration_log" -Passed ($auditItemLengths.Count -eq 1 -and $auditItemLengths[0] -eq $expectedAuditLength) -Detail 'godot-log-audit.json Length must match the retained godot.log.current-iteration bytes'
            Add-Check -Name "${iterationName}_audit_has_single_sha256" -Passed ($auditItemSha256s.Count -eq 1) -Detail "audit JSON must retain exactly one Sha256; found $($auditItemSha256s.Count)"
            Add-Check -Name "${iterationName}_audit_sha256_matches_current_iteration_log" -Passed ($auditItemSha256s.Count -eq 1 -and [System.StringComparer]::OrdinalIgnoreCase.Equals([string]$auditItemSha256s[0], $expectedAuditSha256)) -Detail 'godot-log-audit.json Sha256 must match the retained godot.log.current-iteration bytes'

            if (-not $currentIterationLogExists) {
                Add-Check -Name "${iterationName}_audit_recomputed_from_current_iteration_log" -Passed $false -Detail 'cannot recompute audit because godot.log.current-iteration is missing'
            } elseif (-not (Test-Path -LiteralPath $logAuditScript -PathType Leaf)) {
                Add-Check -Name "${iterationName}_audit_recompute_script_exists" -Passed $false -Detail "missing audit script: $logAuditScript"
            } else {
                $recomputedAuditSummary = Invoke-RecomputedAuditSummary -LogPath $currentIterationLogPath
                $recomputedPaths = @($recomputedAuditSummary.ItemPaths)
                $recomputedSha256s = @($recomputedAuditSummary.ItemSha256s)
                Add-Check -Name "${iterationName}_audit_recomputed_from_current_iteration_log" -Passed ($recomputedPaths.Count -eq 1 -and [System.StringComparer]::OrdinalIgnoreCase.Equals([string]$recomputedPaths[0], $expectedAuditPath)) -Detail 'packet checker must recompute the audit from the retained current-iteration log'
                Add-Check -Name "${iterationName}_audit_recomputed_clean" -Passed ([bool]$recomputedAuditSummary.Clean) -Detail "recomputed audit must have zero dirty items and zero signature hits; dirty=$($recomputedAuditSummary.DirtyItems), hits=$($recomputedAuditSummary.SignatureHitCount)"
                Add-Check -Name "${iterationName}_audit_signature_counts_match_recomputed" -Passed ($auditSummary.DirtyItems -eq $recomputedAuditSummary.DirtyItems -and $auditSummary.SignatureHitCount -eq $recomputedAuditSummary.SignatureHitCount) -Detail "retained audit signature counts must match recomputed counts; retained dirty=$($auditSummary.DirtyItems), retained hits=$($auditSummary.SignatureHitCount), recomputed dirty=$($recomputedAuditSummary.DirtyItems), recomputed hits=$($recomputedAuditSummary.SignatureHitCount)"
                Add-Check -Name "${iterationName}_audit_sha256_matches_recomputed" -Passed ($auditItemSha256s.Count -eq 1 -and $recomputedSha256s.Count -eq 1 -and [System.StringComparer]::OrdinalIgnoreCase.Equals([string]$auditItemSha256s[0], [string]$recomputedSha256s[0])) -Detail 'retained audit Sha256 must match the recomputed audit Sha256'
            }
        } catch {
            Add-Check -Name "${iterationName}_audit_json_valid" -Passed $false -Detail "invalid audit JSON in $auditPath`: $($_.Exception.Message)"
        }
    }

    if ($sts1ModeCheckExists) {
        $sts1ModeCheck = Read-JsonOrNull -Path $sts1ModeCheckPath -CheckName "${iterationName}_sts1_mode_log_check_json_valid"
        if ($null -ne $sts1ModeCheck) {
            Add-Check -Name "${iterationName}_sts1_mode_log_check_json_valid" -Passed $true -Detail 'sts1-mode-log-check.json parsed'
            $sts1Mismatches = @((Get-JsonValue -Object $sts1ModeCheck -Name 'Mismatches' -DefaultValue @()))
            $sts1Checks = @((Get-JsonValue -Object $sts1ModeCheck -Name 'Checks' -DefaultValue @()))
            $sts1FailedChecks = @($sts1Checks | Where-Object {
                -not [bool](Get-JsonValue -Object $_ -Name 'Passed' -DefaultValue $false)
            })
            $sts1CheckSignatures = @(Get-CheckSignatureArray -Items $sts1Checks)
            $expectedSts1Mode = [string](Get-JsonValue -Object $plan -Name 'Sts1EventMode' -DefaultValue '')
            $effectiveExpectedPackageVersion = if (-not [string]::IsNullOrWhiteSpace($ExpectedPackageVersion)) { $ExpectedPackageVersion } else { [string](Get-JsonValue -Object $plan -Name 'ExpectedPackageVersion' -DefaultValue '') }
            $effectiveExpectedGameVersion = if (-not [string]::IsNullOrWhiteSpace($ExpectedGameVersion)) { $ExpectedGameVersion } else { [string](Get-JsonValue -Object $plan -Name 'ExpectedGameVersion' -DefaultValue '') }
            $effectiveExpectedRitsuLibVersion = if (-not [string]::IsNullOrWhiteSpace($ExpectedRitsuLibVersion)) { $ExpectedRitsuLibVersion } else { [string](Get-JsonValue -Object $plan -Name 'ExpectedRitsuLibVersion' -DefaultValue '') }
            $effectiveExpectedRitsuCompatBranch = if (-not [string]::IsNullOrWhiteSpace($ExpectedRitsuCompatBranch)) { $ExpectedRitsuCompatBranch } else { [string](Get-JsonValue -Object $plan -Name 'ExpectedRitsuCompatBranch' -DefaultValue '') }
            $sts1Mode = [string](Get-JsonValue -Object $sts1ModeCheck -Name 'Mode' -DefaultValue '')
            $sts1LogPath = [string](Get-JsonValue -Object $sts1ModeCheck -Name 'LogPath' -DefaultValue '')
            $sts1LogLength = Get-JsonValue -Object $sts1ModeCheck -Name 'LogLength' -DefaultValue $null
            $sts1LogSha256 = [string](Get-JsonValue -Object $sts1ModeCheck -Name 'LogSha256' -DefaultValue '')
            $expectedSts1LogPath = ConvertTo-NormalizedPathOrEmpty -Path $currentIterationLogPath
            $expectedSts1LogLength = if ($currentIterationLogExists) { [long](Get-Item -LiteralPath $currentIterationLogPath).Length } else { -1L }
            $expectedSts1LogSha256 = Get-FileSha256OrEmpty -Path $currentIterationLogPath
            Add-Check -Name "${iterationName}_sts1_mode_log_check_mismatches_empty" -Passed ($sts1Mismatches.Count -eq 0) -Detail "sts1-mode-log-check.json must have zero mismatches; found $($sts1Mismatches.Count)"
            Add-Check -Name "${iterationName}_sts1_mode_log_check_all_checks_passed" -Passed ($sts1FailedChecks.Count -eq 0) -Detail "sts1-mode-log-check.json contains $($sts1FailedChecks.Count) failed checks"
            Add-Check -Name "${iterationName}_sts1_mode_log_check_mode_matches_plan" -Passed (-not [string]::IsNullOrWhiteSpace($expectedSts1Mode) -and $sts1Mode -eq $expectedSts1Mode) -Detail "sts1-mode-log-check.json Mode must match monkey-plan Sts1EventMode '$expectedSts1Mode'; found '$sts1Mode'"
            $normalizedSts1LogPath = ConvertTo-NormalizedPathOrEmpty -Path $sts1LogPath
            Add-Check -Name "${iterationName}_sts1_mode_log_check_log_path_matches_current_iteration_log" -Passed (-not [string]::IsNullOrWhiteSpace($normalizedSts1LogPath) -and -not [string]::IsNullOrWhiteSpace($expectedSts1LogPath) -and [System.StringComparer]::OrdinalIgnoreCase.Equals($normalizedSts1LogPath, $expectedSts1LogPath)) -Detail 'sts1-mode-log-check.json LogPath must match the retained godot.log.current-iteration slice'
            Add-Check -Name "${iterationName}_sts1_mode_log_check_log_length_matches_current_iteration_log" -Passed ($null -ne $sts1LogLength -and [long]$sts1LogLength -eq $expectedSts1LogLength) -Detail 'sts1-mode-log-check.json LogLength must match the retained godot.log.current-iteration bytes'
            Add-Check -Name "${iterationName}_sts1_mode_log_check_log_sha256_matches_current_iteration_log" -Passed (-not [string]::IsNullOrWhiteSpace($sts1LogSha256) -and [System.StringComparer]::OrdinalIgnoreCase.Equals($sts1LogSha256, $expectedSts1LogSha256)) -Detail 'sts1-mode-log-check.json LogSha256 must match the retained godot.log.current-iteration bytes'

            if (-not $currentIterationLogExists) {
                Add-Check -Name "${iterationName}_sts1_mode_log_check_recomputed_from_current_iteration_log" -Passed $false -Detail 'cannot recompute StS1 mode log check because godot.log.current-iteration is missing'
            } elseif (-not $auditExists) {
                Add-Check -Name "${iterationName}_sts1_mode_log_check_recomputed_from_current_iteration_log" -Passed $false -Detail 'cannot recompute StS1 mode log check because godot-log-audit.json is missing'
            } elseif (-not (Test-Path -LiteralPath $sts1EnabledModeLogVerifierScript -PathType Leaf)) {
                Add-Check -Name "${iterationName}_sts1_mode_log_check_recompute_script_exists" -Passed $false -Detail "missing StS1 mode log verifier: $sts1EnabledModeLogVerifierScript"
            } else {
                Add-Check -Name "${iterationName}_sts1_mode_log_check_recompute_script_exists" -Passed $true -Detail 'check-sts1-enabled-mode-runtime-log.ps1 is available'
                try {
                    $recomputedSts1ModeCheck = Invoke-RecomputedSts1ModeLogCheck `
                        -Mode $expectedSts1Mode `
                        -LogPath $currentIterationLogPath `
                        -AuditPath $auditPath `
                        -EffectiveExpectedPackageVersion $effectiveExpectedPackageVersion `
                        -EffectiveExpectedGameVersion $effectiveExpectedGameVersion `
                        -EffectiveExpectedRitsuLibVersion $effectiveExpectedRitsuLibVersion `
                        -EffectiveExpectedRitsuCompatBranch $effectiveExpectedRitsuCompatBranch
                    $recomputedSts1Mode = [string](Get-JsonValue -Object $recomputedSts1ModeCheck -Name 'Mode' -DefaultValue '')
                    $recomputedSts1LogPath = ConvertTo-NormalizedPathOrEmpty -Path ([string](Get-JsonValue -Object $recomputedSts1ModeCheck -Name 'LogPath' -DefaultValue ''))
                    $recomputedSts1LogLength = Get-JsonValue -Object $recomputedSts1ModeCheck -Name 'LogLength' -DefaultValue $null
                    $recomputedSts1LogSha256 = [string](Get-JsonValue -Object $recomputedSts1ModeCheck -Name 'LogSha256' -DefaultValue '')
                    $recomputedSts1Mismatches = @((Get-JsonValue -Object $recomputedSts1ModeCheck -Name 'Mismatches' -DefaultValue @()))
                    $recomputedSts1Checks = @((Get-JsonValue -Object $recomputedSts1ModeCheck -Name 'Checks' -DefaultValue @()))
                    $recomputedSts1FailedChecks = @($recomputedSts1Checks | Where-Object {
                        -not [bool](Get-JsonValue -Object $_ -Name 'Passed' -DefaultValue $false)
                    })
                    $recomputedSts1CheckSignatures = @(Get-CheckSignatureArray -Items $recomputedSts1Checks)

                    Add-Check -Name "${iterationName}_sts1_mode_log_check_recomputed_from_current_iteration_log" -Passed ($recomputedSts1Mode -eq $expectedSts1Mode -and -not [string]::IsNullOrWhiteSpace($recomputedSts1LogPath) -and [System.StringComparer]::OrdinalIgnoreCase.Equals($recomputedSts1LogPath, $expectedSts1LogPath) -and $null -ne $recomputedSts1LogLength -and [long]$recomputedSts1LogLength -eq $expectedSts1LogLength -and [System.StringComparer]::OrdinalIgnoreCase.Equals($recomputedSts1LogSha256, $expectedSts1LogSha256)) -Detail 'packet checker must recompute sts1-mode-log-check.json from the retained current-iteration log'
                    Add-Check -Name "${iterationName}_sts1_mode_log_check_recomputed_mismatches_empty" -Passed ($recomputedSts1Mismatches.Count -eq 0) -Detail "recomputed StS1 mode log check mismatches must be empty; found $($recomputedSts1Mismatches.Count)"
                    Add-Check -Name "${iterationName}_sts1_mode_log_check_recomputed_all_checks_passed" -Passed ($recomputedSts1FailedChecks.Count -eq 0) -Detail "recomputed StS1 mode log check contains $($recomputedSts1FailedChecks.Count) failed checks"
                    Add-Check -Name "${iterationName}_sts1_mode_log_check_mismatches_match_recomputed" -Passed (Test-StringArrayEquals -Actual $sts1Mismatches -Expected $recomputedSts1Mismatches) -Detail 'retained sts1-mode-log-check.json Mismatches must match the recomputed verifier report'
                    Add-Check -Name "${iterationName}_sts1_mode_log_check_checks_match_recomputed" -Passed (Test-StringArrayEquals -Actual $sts1CheckSignatures -Expected $recomputedSts1CheckSignatures) -Detail 'retained sts1-mode-log-check.json Checks must match the recomputed verifier report'
                } catch {
                    Add-Check -Name "${iterationName}_sts1_mode_log_check_recomputed_from_current_iteration_log" -Passed $false -Detail "failed to recompute StS1 mode log check: $($_.Exception.Message)"
                }
            }
        }
    }
}

$report = [pscustomobject]@{
    EvidenceDir = $resolvedEvidenceDir
    ExpectedIterations = $expectedIterationCount
    ExpectedPackageVersion = $ExpectedPackageVersion
    ExpectedGameVersion = $ExpectedGameVersion
    ExpectedRitsuLibVersion = $ExpectedRitsuLibVersion
    ExpectedRitsuCompatBranch = $ExpectedRitsuCompatBranch
    ExpectedPatchCount = $ExpectedPatchCount
    RequireCurrentSourceSnapshot = [bool]$RequireCurrentSourceSnapshot
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

    $report | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $resolvedOutFile -Encoding UTF8
}

if ($FailOnMismatch -and $mismatches.Count -gt 0) {
    exit 1
}
