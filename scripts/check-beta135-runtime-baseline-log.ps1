param(
    [string]$EvidenceDir,
    [string]$LogPath,
    [string]$ExpectedPackageVersion = 'v0.1.0-private-beta.135',
    [string]$ExpectedGameVersion = '0.107.1',
    [string]$ExpectedRitsuLibVersion = '0.4.34',
    [string]$ExpectedRitsuCompatBranch = '0.107.1',
    [int]$ExpectedPatchCount = 168,
    [string]$GameRoot = '',
    [string]$PackageZipPath = '',
    [switch]$SelfTest,
    [switch]$FailOnMismatch
)

$ErrorActionPreference = 'Stop'
$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
$auditScript = Join-Path $PSScriptRoot 'audit-godot-log.ps1'
$canonicalEvidenceRoot = [System.IO.Path]::GetFullPath((Join-Path $repoRoot '.tools\runtime-evidence'))

function Normalize-FullPath {
    param([Parameter(Mandatory = $true)][string]$Path)

    [System.IO.Path]::GetFullPath($Path).TrimEnd([char[]]@('\', '/'))
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

function Initialize-WindowsFileIdentityType {
    if (-not [System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform([System.Runtime.InteropServices.OSPlatform]::Windows)) {
        return $false
    }

    if ('SpirePlusBeta135RuntimeBaselineFileIdentity' -as [type]) {
        return $true
    }

    try {
        Add-Type -TypeDefinition @'
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

public static class SpirePlusBeta135RuntimeBaselineFileIdentity
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
        return [SpirePlusBeta135RuntimeBaselineFileIdentity]::GetIdentity([System.IO.Path]::GetFullPath($Path))
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
        return 0
    }

    if (-not (Initialize-WindowsFileIdentityType)) {
        return 0
    }

    try {
        return [int][SpirePlusBeta135RuntimeBaselineFileIdentity]::GetLinkCount([System.IO.Path]::GetFullPath($Path))
    } catch {
        return 0
    }
}

function Assert-OutputFileDoesNotAliasProtectedEvidence {
    param(
        [Parameter(Mandatory = $true)][string]$OutputPath,
        [Parameter(Mandatory = $true)][string[]]$ProtectedPaths,
        [string]$Description = 'Output file'
    )

    if (-not (Test-Path -LiteralPath $OutputPath -PathType Leaf)) {
        return
    }

    $resolvedOutputPath = [System.IO.Path]::GetFullPath($OutputPath)
    $linkCount = Get-ExistingPathHardlinkCount -Path $resolvedOutputPath
    if ($linkCount -gt 1) {
        throw "$Description must not be a hardlink with multiple filesystem names: $resolvedOutputPath linkCount=$linkCount"
    }

    foreach ($protectedPath in @($ProtectedPaths)) {
        if ([string]::IsNullOrWhiteSpace($protectedPath) -or -not (Test-Path -LiteralPath $protectedPath -PathType Leaf)) {
            continue
        }

        $resolvedProtectedPath = [System.IO.Path]::GetFullPath($protectedPath)
        if ([System.StringComparer]::OrdinalIgnoreCase.Equals($resolvedOutputPath, $resolvedProtectedPath)) {
            continue
        }

        if (Test-SameExistingPathPhysicalIdentity -Left $resolvedOutputPath -Right $resolvedProtectedPath) {
            throw "$Description must not share physical identity with protected runtime baseline evidence: $resolvedOutputPath -> $resolvedProtectedPath"
        }
    }
}

function Assert-BaselineEvidenceDir {
    param([Parameter(Mandatory = $true)][string]$Path)

    $normalizedPath = Normalize-FullPath -Path $Path
    $normalizedRoot = Normalize-FullPath -Path $canonicalEvidenceRoot
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

function Resolve-LocalPath {
    param([Parameter(Mandatory = $true)][string]$Path)

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }

    return [System.IO.Path]::GetFullPath((Join-Path $repoRoot $Path))
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
        throw 'Pass -GameRoot or set Directory.Build.props Sts2Path before checking beta.135 runtime baseline evidence.'
    }

    return Normalize-FullPath -Path $candidate
}

function Resolve-ConfiguredPackageZipPath {
    param(
        [AllowEmptyString()][string]$Path,
        [Parameter(Mandatory = $true)][string]$PackageVersion
    )

    if (-not [string]::IsNullOrWhiteSpace($Path)) {
        return Resolve-LocalPath -Path $Path
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

function Add-Check {
    param(
        [Parameter(Mandatory = $true)][AllowEmptyCollection()][System.Collections.Generic.List[object]]$Checks,
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][bool]$Passed,
        [string]$Detail = ''
    )

    $Checks.Add([pscustomobject]@{
        Name = $Name
        Passed = $Passed
        Detail = $Detail
    }) | Out-Null
}

function Get-PatchSummary {
    param([Parameter(Mandatory = $true)][string]$Text)

    $summaryPattern = '(?im)Patch application complete:\s+(?<applied>\d+)\s+applied,\s+(?<ignored>\d+)\s+ignored,\s+(?<failed>\d+)\s+failed,\s+(?<total>\d+)\s+total'
    $registeredPattern = '(?im)ModPatcher applied\s+(?<applied>\d+)\s+patches\s+\((?<registered>\d+)\s+registered\)'
    $summaryMatch = [regex]::Match($Text, $summaryPattern)
    $registeredMatch = [regex]::Match($Text, $registeredPattern)

    [pscustomobject]@{
        SummaryFound = $summaryMatch.Success
        Applied = if ($summaryMatch.Success) { [int]$summaryMatch.Groups['applied'].Value } else { -1 }
        Ignored = if ($summaryMatch.Success) { [int]$summaryMatch.Groups['ignored'].Value } else { -1 }
        Failed = if ($summaryMatch.Success) { [int]$summaryMatch.Groups['failed'].Value } else { -1 }
        Total = if ($summaryMatch.Success) { [int]$summaryMatch.Groups['total'].Value } else { -1 }
        RegisteredFound = $registeredMatch.Success
        RegisteredApplied = if ($registeredMatch.Success) { [int]$registeredMatch.Groups['applied'].Value } else { -1 }
        Registered = if ($registeredMatch.Success) { [int]$registeredMatch.Groups['registered'].Value } else { -1 }
    }
}

function Get-ModManifestIds {
    param([Parameter(Mandatory = $true)][string]$Text)

    $ids = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
    $pattern = '(?im)Found mod manifest file\s+.*?[\\/]+mods[\\/]+(?<id>[^\\/]+)[\\/]'
    foreach ($match in [regex]::Matches($Text, $pattern)) {
        $id = $match.Groups['id'].Value
        if (-not [string]::IsNullOrWhiteSpace($id)) {
            $ids.Add($id) | Out-Null
        }
    }

    return @($ids | Sort-Object)
}

function Test-StringSetEquals {
    param(
        [string[]]$Actual,
        [string[]]$Expected
    )

    $actualSet = @($Actual | Sort-Object -Unique)
    $expectedSet = @($Expected | Sort-Object -Unique)
    if ($actualSet.Count -ne $expectedSet.Count) {
        return $false
    }

    for ($i = 0; $i -lt $expectedSet.Count; $i++) {
        if (-not [System.StringComparer]::OrdinalIgnoreCase.Equals($actualSet[$i], $expectedSet[$i])) {
            return $false
        }
    }

    return $true
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

function Get-JsonProperty {
    param(
        [Parameter(Mandatory = $true)][AllowNull()]$Object,
        [Parameter(Mandatory = $true)][string]$Name
    )

    if ($null -eq $Object -or $null -eq $Object.PSObject) {
        return @()
    }

    return @($Object.PSObject.Properties | Where-Object {
        $_.MemberType -eq [System.Management.Automation.PSMemberTypes]::NoteProperty -and
            [string]::Equals($_.Name, $Name, [System.StringComparison]::Ordinal)
    })
}

function Test-JsonProperty {
    param(
        [Parameter(Mandatory = $true)][AllowNull()]$Object,
        [Parameter(Mandatory = $true)][string]$Name
    )

    return @(Get-JsonProperty -Object $Object -Name $Name).Count -gt 0
}

function Test-NativeJsonBoolValue {
    param($Value)

    return $null -ne $Value -and $Value.GetType() -eq [bool]
}

function Test-NativeJsonIntegerValue {
    param($Value)

    if ($null -eq $Value) {
        return $false
    }

    $integerTypes = @(
        [byte],
        [sbyte],
        [int16],
        [uint16],
        [int],
        [uint32],
        [long],
        [uint64]
    )
    return $integerTypes -contains $Value.GetType()
}

function Test-NativeJsonInt32Value {
    param($Value)

    if (-not (Test-NativeJsonIntegerValue -Value $Value)) {
        return $false
    }

    try {
        $unused = [int]$Value
        return [int64]$unused -eq [int64]$Value
    } catch {
        return $false
    }
}

function Test-NativeJsonInt64Value {
    param($Value)

    if (-not (Test-NativeJsonIntegerValue -Value $Value)) {
        return $false
    }

    try {
        $unused = [long]$Value
        return [decimal]$unused -eq [decimal]$Value
    } catch {
        return $false
    }
}

function Test-RawJsonRootArray {
    param([AllowNull()][string]$Json)

    if ([string]::IsNullOrWhiteSpace($Json)) {
        return $false
    }

    return $Json.TrimStart().StartsWith('[', [System.StringComparison]::Ordinal)
}

function Test-Sha256Text {
    param([AllowEmptyString()][string]$Value)

    return -not [string]::IsNullOrWhiteSpace($Value) -and $Value -match '^[A-Fa-f0-9]{64}$'
}

$expectedAuditSignatureNames = @(
    'Creature.get_ShowsInfiniteHp',
    'DependencyFramework.Patches.UI.HealthBarForecastPatch',
    'dependency framework patch failure',
    'DamageMeter',
    'RouteSuggest',
    'Spire Plus error/exception',
    'TypeLoadException',
    'MissingMethodException',
    'Godot ERROR line'
)

function New-InvalidAuditSummary {
    param([string]$ErrorMessage = '')

    [pscustomobject]@{
        JsonValid = $false
        ItemCount = 0
        DirtyItems = 0
        SignatureHitCount = 0
        MalformedNumericValues = 0
        MalformedBoolValues = 0
        MalformedArrayValues = 0
        MalformedSchemaValues = 0
        MalformedFileBindingValues = 0
        SignatureHitNames = @()
        SignatureHitVector = @()
        Clean = $false
        Error = $ErrorMessage
    }
}

function Read-AuditSummary {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [string]$ExpectedLogPath
    )

    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        return New-InvalidAuditSummary -ErrorMessage "Audit file not found: $Path"
    }

    try {
        $raw = Get-Content -LiteralPath $Path -Raw -Encoding UTF8
        $auditJson = $raw | ConvertFrom-Json
    } catch {
        return New-InvalidAuditSummary -ErrorMessage $_.Exception.Message
    }

    $items = @($auditJson)
    $dirtyItems = 0
    $hitCount = 0
    $malformedNumericValues = 0
    $malformedBoolValues = 0
    $malformedArrayValues = 0
    $malformedSchemaValues = 0
    $malformedFileBindingValues = 0
    $auditSchemaVersions = [System.Collections.Generic.List[string]]::new()
    $signatureSetSha256s = [System.Collections.Generic.List[string]]::new()
    $signatureHitNames = [System.Collections.Generic.List[string]]::new()
    $signatureHitVector = [System.Collections.Generic.List[string]]::new()
    $expectedLogRecord = if ([string]::IsNullOrWhiteSpace($ExpectedLogPath)) { $null } else { Get-FileRecord -Path $ExpectedLogPath }
    if (-not (Test-RawJsonRootArray -Json $raw) -or [regex]::IsMatch($raw, '"SignatureHits"\s*:\s*\{')) {
        $malformedArrayValues++
    }

    foreach ($item in $items) {
        if (-not (Test-JsonProperty -Object $item -Name 'AuditSchemaVersion')) {
            $malformedSchemaValues++
        } elseif (Test-NativeJsonInt32Value -Value $item.AuditSchemaVersion) {
            $auditSchemaVersion = [int]$item.AuditSchemaVersion
            $auditSchemaVersions.Add([string]$auditSchemaVersion) | Out-Null
            if ($auditSchemaVersion -ne 2) {
                $malformedSchemaValues++
            }
        } else {
            $malformedSchemaValues++
        }

        if (-not (Test-JsonProperty -Object $item -Name 'SignatureSetSha256')) {
            $malformedSchemaValues++
        } else {
            $signatureSetSha256 = [string]$item.SignatureSetSha256
            if (Test-Sha256Text -Value $signatureSetSha256) {
                $signatureSetSha256s.Add($signatureSetSha256.ToLowerInvariant()) | Out-Null
            } else {
                $malformedSchemaValues++
            }
        }

        $clean = $false
        if ((Test-JsonProperty -Object $item -Name 'Clean') -and (Test-NativeJsonBoolValue -Value $item.Clean)) {
            $clean = [bool]$item.Clean
        } else {
            $malformedBoolValues++
        }

        if (-not $clean) {
            $dirtyItems++
        }

        if ((Test-JsonProperty -Object $item -Name 'Length') -and (Test-NativeJsonInt64Value -Value $item.Length)) {
            if ($null -ne $expectedLogRecord -and [long]$item.Length -ne [long]$expectedLogRecord.Length) {
                $malformedFileBindingValues++
            }
        } else {
            $malformedNumericValues++
        }

        if ($null -ne $expectedLogRecord) {
            if (-not (Test-JsonProperty -Object $item -Name 'Path') -or [string]::IsNullOrWhiteSpace([string]$item.Path)) {
                $malformedFileBindingValues++
            } else {
                $retainedPath = [System.IO.Path]::GetFullPath([string]$item.Path)
                if (-not [System.StringComparer]::OrdinalIgnoreCase.Equals($retainedPath, $expectedLogRecord.Path)) {
                    $malformedFileBindingValues++
                }
            }

            if (-not (Test-JsonProperty -Object $item -Name 'Sha256') -or
                -not (Test-Sha256Text -Value ([string]$item.Sha256)) -or
                -not [string]::Equals(([string]$item.Sha256).ToLowerInvariant(), [string]$expectedLogRecord.Sha256, [System.StringComparison]::Ordinal)) {
                $malformedFileBindingValues++
            }
        }

        if (-not (Test-JsonProperty -Object $item -Name 'SignatureHits')) {
            $malformedArrayValues++
            continue
        }

        $signatureHits = $item.SignatureHits
        if ($null -eq $signatureHits -or $signatureHits -is [string] -or -not ($signatureHits -is [System.Array])) {
            $malformedArrayValues++
            continue
        }

        foreach ($hit in @($signatureHits)) {
            $hitName = ''
            if (Test-JsonProperty -Object $hit -Name 'Name') {
                $hitName = [string]$hit.Name
            }

            if ([string]::IsNullOrWhiteSpace($hitName)) {
                $malformedSchemaValues++
            }

            if ((Test-JsonProperty -Object $hit -Name 'Count') -and (Test-NativeJsonInt32Value -Value $hit.Count)) {
                $hitCountValue = [int]$hit.Count
                $hitCount += $hitCountValue
                if (-not [string]::IsNullOrWhiteSpace($hitName)) {
                    $signatureHitNames.Add($hitName) | Out-Null
                    $signatureHitVector.Add("$hitName=$hitCountValue") | Out-Null
                }
            } else {
                $malformedNumericValues++
            }
        }
    }

    [pscustomobject]@{
        JsonValid = $true
        ItemCount = $items.Count
        DirtyItems = $dirtyItems
        SignatureHitCount = $hitCount
        AuditSchemaVersions = @($auditSchemaVersions.ToArray() | Sort-Object -Unique)
        SignatureSetSha256s = @($signatureSetSha256s.ToArray() | Sort-Object -Unique)
        SignatureHitNames = @($signatureHitNames.ToArray() | Sort-Object -Unique)
        SignatureHitVector = @($signatureHitVector.ToArray() | Sort-Object)
        MalformedNumericValues = $malformedNumericValues
        MalformedBoolValues = $malformedBoolValues
        MalformedArrayValues = $malformedArrayValues
        MalformedSchemaValues = $malformedSchemaValues
        MalformedFileBindingValues = $malformedFileBindingValues
        Clean = ($items.Count -gt 0 -and $dirtyItems -eq 0 -and $hitCount -eq 0 -and $signatureHitVector.Count -gt 0 -and $malformedNumericValues -eq 0 -and $malformedBoolValues -eq 0 -and $malformedArrayValues -eq 0 -and $malformedSchemaValues -eq 0 -and $malformedFileBindingValues -eq 0)
        Error = ''
    }
}

function Get-ManifestProperty {
    param(
        [Parameter(Mandatory = $true)][AllowNull()]$Object,
        [Parameter(Mandatory = $true)][string]$Name
    )

    if (Test-JsonProperty -Object $Object -Name $Name) {
        return $Object.$Name
    }

    return $null
}

function Test-ManifestBool {
    param(
        [Parameter(Mandatory = $true)][AllowNull()]$Object,
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][bool]$Expected
    )

    $value = Get-ManifestProperty -Object $Object -Name $Name
    return $null -ne $value -and $value.GetType() -eq [bool] -and [bool]$value -eq $Expected
}

function Test-ManifestString {
    param(
        [Parameter(Mandatory = $true)][AllowNull()]$Object,
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][string]$Expected
    )

    $value = Get-ManifestProperty -Object $Object -Name $Name
    return $null -ne $value -and [System.StringComparer]::OrdinalIgnoreCase.Equals([string]$value, $Expected)
}

function Test-ManifestStringExact {
    param(
        [Parameter(Mandatory = $true)][AllowNull()]$Object,
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][string]$Expected
    )

    $value = Get-ManifestProperty -Object $Object -Name $Name
    return $null -ne $value -and [string]::Equals([string]$value, $Expected, [System.StringComparison]::Ordinal)
}

function Test-StringArrayExact {
    param(
        [AllowNull()]$Actual,
        [AllowNull()]$Expected
    )

    $actualArray = @($Actual | ForEach-Object { [string]$_ })
    $expectedArray = @($Expected | ForEach-Object { [string]$_ })
    if ($actualArray.Count -ne $expectedArray.Count) {
        return $false
    }

    for ($i = 0; $i -lt $actualArray.Count; $i++) {
        if (-not [string]::Equals($actualArray[$i], $expectedArray[$i], [System.StringComparison]::Ordinal)) {
            return $false
        }
    }

    return $true
}

function Test-GitEvidenceMatches {
    param(
        [AllowNull()]$ManifestGit,
        [Parameter(Mandatory = $true)]$CurrentGit
    )

    if ($null -eq $ManifestGit -or $null -eq $CurrentGit) {
        return $false
    }

    $manifestStatus = @(Get-ManifestProperty -Object $ManifestGit -Name 'StatusShortBranch')
    $currentStatus = @($CurrentGit.StatusShortBranch)
    return (Test-ManifestBool -Object $ManifestGit -Name 'Available' -Expected $true) -and
        [bool]$CurrentGit.Available -and
        (Test-ManifestPath -Object $ManifestGit -Name 'RepoRoot' -Expected $repoRoot) -and
        (Test-ManifestStringExact -Object $ManifestGit -Name 'HeadSha' -Expected ([string]$CurrentGit.HeadSha)) -and
        (Test-ManifestStringExact -Object $ManifestGit -Name 'HeadDecorated' -Expected ([string]$CurrentGit.HeadDecorated)) -and
        (Test-ManifestStringExact -Object $ManifestGit -Name 'Branch' -Expected ([string]$CurrentGit.Branch)) -and
        (Test-ManifestStringExact -Object $ManifestGit -Name 'Upstream' -Expected ([string]$CurrentGit.Upstream)) -and
        (Test-ManifestBool -Object $ManifestGit -Name 'Dirty' -Expected ([bool]$CurrentGit.Dirty)) -and
        (Test-StringArrayExact -Actual $manifestStatus -Expected $currentStatus)
}

function Test-ManifestPath {
    param(
        [Parameter(Mandatory = $true)][AllowNull()]$Object,
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][string]$Expected
    )

    $value = Get-ManifestProperty -Object $Object -Name $Name
    if ($null -eq $value -or [string]::IsNullOrWhiteSpace([string]$value)) {
        return $false
    }

    try {
        $actualPath = Normalize-FullPath -Path ([string]$value)
        $expectedPath = Normalize-FullPath -Path $Expected
        return [System.StringComparer]::OrdinalIgnoreCase.Equals($actualPath, $expectedPath)
    } catch {
        return $false
    }
}

function Test-ManifestInteger {
    param(
        [Parameter(Mandatory = $true)][AllowNull()]$Object,
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][int]$Expected
    )

    $value = Get-ManifestProperty -Object $Object -Name $Name
    return $null -ne $value -and (Test-NativeJsonInt32Value -Value $value) -and [int]$value -eq $Expected
}

function Test-FileRecordHashBound {
    param([AllowNull()]$Record)

    if ($null -eq $Record) {
        return $false
    }

    $hasExists = Test-JsonProperty -Object $Record -Name 'Exists'
    $exists = Get-ManifestProperty -Object $Record -Name 'Exists'
    $path = Get-ManifestProperty -Object $Record -Name 'Path'
    $length = Get-ManifestProperty -Object $Record -Name 'Length'
    $sha256 = Get-ManifestProperty -Object $Record -Name 'Sha256'
    return (-not $hasExists -or ($null -ne $exists -and $exists.GetType() -eq [bool] -and [bool]$exists)) -and
        -not [string]::IsNullOrWhiteSpace([string]$path) -and
        $null -ne $length -and (Test-NativeJsonInt64Value -Value $length) -and
        (Test-Sha256Text -Value ([string]$sha256))
}

function Test-FileRecordMatchesCurrentFile {
    param(
        [AllowNull()]$Record,
        [Parameter(Mandatory = $true)][string]$ExpectedPath
    )

    if (-not (Test-FileRecordHashBound -Record $Record)) {
        return $false
    }

    $current = Get-FileRecord -Path $ExpectedPath
    if (-not [bool]$current.Exists) {
        return $false
    }

    try {
        $recordPath = [System.IO.Path]::GetFullPath([string](Get-ManifestProperty -Object $Record -Name 'Path'))
        $recordLength = [long](Get-ManifestProperty -Object $Record -Name 'Length')
        return [System.StringComparer]::OrdinalIgnoreCase.Equals($recordPath, $current.Path) -and
            $recordLength -eq [long]$current.Length -and
            [string]::Equals(([string](Get-ManifestProperty -Object $Record -Name 'Sha256')).ToLowerInvariant(), [string]$current.Sha256, [System.StringComparison]::Ordinal)
    } catch {
        return $false
    }
}

function Get-HashBoundFileRecordCount {
    param([AllowNull()]$Records)

    $count = 0
    foreach ($record in @($Records)) {
        if (Test-FileRecordHashBound -Record $record) {
            $count++
        }
    }

    return $count
}

function Test-HashBoundFileRecordPathSuffix {
    param(
        [AllowNull()]$Records,
        [Parameter(Mandatory = $true)][string]$Suffix
    )

    foreach ($record in @($Records)) {
        if (-not (Test-FileRecordHashBound -Record $record)) {
            continue
        }

        $path = [string](Get-ManifestProperty -Object $record -Name 'Path')
        if ($path.EndsWith($Suffix, [System.StringComparison]::OrdinalIgnoreCase)) {
            return $true
        }
    }

    return $false
}

function Get-HashBoundFileRecordByRelativePath {
    param(
        [AllowNull()]$Records,
        [Parameter(Mandatory = $true)][string]$RelativePath
    )

    foreach ($record in @($Records)) {
        if (-not (Test-FileRecordHashBound -Record $record)) {
            continue
        }

        $recordRelativePath = [string](Get-ManifestProperty -Object $record -Name 'RelativePath')
        if ([System.StringComparer]::OrdinalIgnoreCase.Equals($recordRelativePath, $RelativePath)) {
            return $record
        }
    }

    return $null
}

function Test-ManifestDirectoryFileRecordMatchesCurrentFile {
    param(
        [AllowNull()]$Records,
        [Parameter(Mandatory = $true)][string]$Directory,
        [Parameter(Mandatory = $true)][string]$RelativePath
    )

    if ([string]::IsNullOrWhiteSpace($Directory)) {
        return $false
    }

    $record = Get-HashBoundFileRecordByRelativePath -Records $Records -RelativePath $RelativePath
    if ($null -eq $record) {
        return $false
    }

    return Test-FileRecordMatchesCurrentFile -Record $record -ExpectedPath (Join-Path $Directory $RelativePath)
}

function Test-DirectoryPathBound {
    param(
        [AllowEmptyString()][string]$Directory,
        [Parameter(Mandatory = $true)][string]$RequiredSuffix
    )

    if ([string]::IsNullOrWhiteSpace($Directory)) {
        return $false
    }

    try {
        $fullPath = [System.IO.Path]::GetFullPath($Directory).TrimEnd([char[]]@('\', '/'))
    } catch {
        return $false
    }

    return (Test-Path -LiteralPath $fullPath -PathType Container) -and
        $fullPath.EndsWith($RequiredSuffix, [System.StringComparison]::OrdinalIgnoreCase)
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

function New-SelfTestLog {
    return @"
release = v0.107.1
[INFO] Found mod manifest file E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance\EZMicroBalance.json
[INFO] Found mod manifest file E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib\mod_manifest.json
STS2-RitsuLib loaded successfully
RitsuLib Version: 0.4.34 [compat branch: 0.107.1]
Spire Plus v0.1.0-private-beta.135 loaded
[INFO] [EZMicroBalance] [Patcher - SpirePlus] Patch application complete: 168 applied, 0 ignored, 0 failed, 168 total
[INFO] [EZMicroBalance] ModPatcher applied 168 patches (168 registered).
[Startup] Time to main menu
"@
}

if ($SelfTest) {
    $tempRoot = Join-Path ([System.IO.Path]::GetTempPath()) ('spireplus-beta135-baseline-selftest-' + [guid]::NewGuid().ToString('N'))
    New-Item -ItemType Directory -Path $tempRoot -Force | Out-Null
    try {
        $LogPath = Join-Path $tempRoot 'godot.log.after-launch'
        New-SelfTestLog | Set-Content -LiteralPath $LogPath -Encoding UTF8
        $EvidenceDir = $tempRoot
    } finally {
    }
}

if ([string]::IsNullOrWhiteSpace($LogPath)) {
    throw 'Pass -LogPath or -SelfTest.'
}

$resolvedLogPath = Resolve-LocalPath -Path $LogPath
if (-not (Test-Path -LiteralPath $resolvedLogPath -PathType Leaf)) {
    throw "Log file not found: $resolvedLogPath"
}
Assert-LeafIsNotReparsePoint -Path $resolvedLogPath -Description 'LogPath'

$resolvedEvidenceDir = if ([string]::IsNullOrWhiteSpace($EvidenceDir)) {
    Split-Path -Parent $resolvedLogPath
} else {
    Resolve-LocalPath -Path $EvidenceDir
}

if (-not $SelfTest) {
    Assert-BaselineEvidenceDir -Path $resolvedEvidenceDir
    Assert-NoReparsePointInPath -Path $resolvedEvidenceDir
    if (-not (Test-PathInsideDirectory -Path $resolvedLogPath -Directory $resolvedEvidenceDir)) {
        throw "LogPath must stay inside EvidenceDir for beta.135 runtime baseline evidence. LogPath: $resolvedLogPath EvidenceDir: $resolvedEvidenceDir"
    }
    Assert-NoReparsePointInPath -Path $resolvedLogPath
}

New-Item -ItemType Directory -Path $resolvedEvidenceDir -Force | Out-Null

$resolvedGameRoot = if ($SelfTest) { '' } else { Resolve-ConfiguredGameRoot -Path $GameRoot }
$resolvedModsRoot = if ($SelfTest) { '' } else { Join-Path $resolvedGameRoot 'mods' }
$expectedSpirePlusInstallDir = if ($SelfTest) { '' } else { Join-Path $resolvedModsRoot 'EZMicroBalance' }
$expectedRitsuLibInstallDir = if ($SelfTest) { '' } else { Join-Path $resolvedModsRoot 'STS2-RitsuLib' }
$resolvedPackageZipPath = if ($SelfTest) { '' } else { Resolve-ConfiguredPackageZipPath -Path $PackageZipPath -PackageVersion $ExpectedPackageVersion }
$gameRootAnchorMode = if ($SelfTest) { 'self-test' } else { Get-GameRootAnchorMode -ResolvedGameRoot $resolvedGameRoot }
$packageAnchorMode = if ($SelfTest) { 'self-test' } else { Get-PackageAnchorMode -ResolvedPackageZipPath $resolvedPackageZipPath -PackageVersion $ExpectedPackageVersion }
$trustAnchorMode = if ($SelfTest) { 'self-test' } else { Get-TrustAnchorMode -GameRootAnchorMode $gameRootAnchorMode -PackageAnchorMode $packageAnchorMode }

$manifestPath = Join-Path $resolvedEvidenceDir 'run-manifest.json'
$auditOutputPath = Join-Path $resolvedEvidenceDir 'godot-log-audit.json'
$reportPath = Join-Path $resolvedEvidenceDir 'runtime-baseline-log-check.json'
$baselineProtectedPaths = @(
    $resolvedLogPath,
    (Join-Path $resolvedEvidenceDir 'godot.log.after-launch'),
    (Join-Path $resolvedEvidenceDir 'godot-log-audit.json'),
    (Join-Path $resolvedEvidenceDir 'runtime-baseline-log-check.json'),
    (Join-Path $resolvedEvidenceDir 'run-manifest.json'),
    (Join-Path $resolvedEvidenceDir 'preflight.json'),
    (Join-Path $resolvedEvidenceDir 'command.txt'),
    (Join-Path $resolvedEvidenceDir 'runtime-baseline-notes.md'),
    (Join-Path $resolvedEvidenceDir 'main-menu-screenshot.png')
)
if (-not $SelfTest) {
    Assert-NoReparsePointInPath -Path $manifestPath
    Assert-OutputFileDoesNotAliasProtectedEvidence -OutputPath $manifestPath -ProtectedPaths $baselineProtectedPaths -Description 'Runtime baseline manifest'
}
$manifestFileExists = Test-Path -LiteralPath $manifestPath -PathType Leaf
$manifest = if ($manifestFileExists) {
    try {
        Get-Content -LiteralPath $manifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
    } catch {
        [pscustomobject]@{}
    }
} else {
    [pscustomobject]@{}
}
$currentGitEvidence = if ($SelfTest) { $null } else { Get-GitEvidence }

$text = Get-Content -LiteralPath $resolvedLogPath -Raw -Encoding UTF8
$checks = [System.Collections.Generic.List[object]]::new()
$patch = Get-PatchSummary -Text $text
$expectedModManifestIds = @('EZMicroBalance', 'STS2-RitsuLib')
$modManifestIds = @(Get-ModManifestIds -Text $text)
$gameVersionMarkerPattern = '(?im)\b(?:release(?:\s+version)?|host\s+version)\b\s*(?:=|:)?\s*v?' + [regex]::Escape($ExpectedGameVersion) + '\b'

if (-not $SelfTest) {
    $packageZip = Get-ManifestProperty -Object $manifest -Name 'PackageZip'
    $spirePlusFiles = @(Get-ManifestProperty -Object $manifest -Name 'SpirePlusFiles')
    $ritsuLibFiles = @(Get-ManifestProperty -Object $manifest -Name 'RitsuLibFiles')
    $manifestGit = Get-ManifestProperty -Object $manifest -Name 'Git'
    $manifestGitMatchesCurrent = Test-GitEvidenceMatches -ManifestGit $manifestGit -CurrentGit $currentGitEvidence
    $manifestGitHead = [string](Get-ManifestProperty -Object $manifestGit -Name 'HeadSha')
    $currentGitHead = [string]$currentGitEvidence.HeadSha
    $spirePlusHashBoundCount = Get-HashBoundFileRecordCount -Records $spirePlusFiles
    $ritsuLibHashBoundCount = Get-HashBoundFileRecordCount -Records $ritsuLibFiles
    $spirePlusInstallDirBound = Test-DirectoryPathBound -Directory $expectedSpirePlusInstallDir -RequiredSuffix 'mods\EZMicroBalance'
    $ritsuLibInstallDirBound = Test-DirectoryPathBound -Directory $expectedRitsuLibInstallDir -RequiredSuffix 'mods\STS2-RitsuLib'
    $packageZipMatchesCurrentFile = Test-FileRecordMatchesCurrentFile -Record $packageZip -ExpectedPath $resolvedPackageZipPath
    $spirePlusFilesMatchCurrentInstall =
        $spirePlusInstallDirBound -and
        (Test-ManifestDirectoryFileRecordMatchesCurrentFile -Records $spirePlusFiles -Directory $expectedSpirePlusInstallDir -RelativePath 'EZMicroBalance.dll') -and
        (Test-ManifestDirectoryFileRecordMatchesCurrentFile -Records $spirePlusFiles -Directory $expectedSpirePlusInstallDir -RelativePath 'EZMicroBalance.json') -and
        (Test-ManifestDirectoryFileRecordMatchesCurrentFile -Records $spirePlusFiles -Directory $expectedSpirePlusInstallDir -RelativePath 'EZMicroBalance.pck')
    $ritsuLibFilesMatchCurrentInstall =
        $ritsuLibInstallDirBound -and
        (Test-ManifestDirectoryFileRecordMatchesCurrentFile -Records $ritsuLibFiles -Directory $expectedRitsuLibInstallDir -RelativePath 'STS2-RitsuLib.dll') -and
        (Test-ManifestDirectoryFileRecordMatchesCurrentFile -Records $ritsuLibFiles -Directory $expectedRitsuLibInstallDir -RelativePath 'STS2-RitsuLib.xml')
    $manifestInstallPathsMatchConfigured =
        (Test-ManifestPath -Object $manifest -Name 'GameRoot' -Expected $resolvedGameRoot) -and
        (Test-ManifestPath -Object $manifest -Name 'ModsRoot' -Expected $resolvedModsRoot) -and
        (Test-ManifestPath -Object $manifest -Name 'SpirePlusInstallDir' -Expected $expectedSpirePlusInstallDir) -and
        (Test-ManifestPath -Object $manifest -Name 'RitsuLibInstallDir' -Expected $expectedRitsuLibInstallDir)
    $packageTargetsMatch =
        (Test-ManifestString -Object $manifest -Name 'PackageVersion' -Expected $ExpectedPackageVersion) -and
        (Test-ManifestString -Object $manifest -Name 'GameVersion' -Expected $ExpectedGameVersion) -and
        (Test-ManifestString -Object $manifest -Name 'RitsuLibVersion' -Expected $ExpectedRitsuLibVersion) -and
        (Test-ManifestString -Object $manifest -Name 'RitsuCompatBranch' -Expected $ExpectedRitsuCompatBranch) -and
        (Test-ManifestInteger -Object $manifest -Name 'ExpectedRuntimePatchCount' -Expected $ExpectedPatchCount)
    $manifestAnchorModesMatch =
        (Test-ManifestStringExact -Object $manifest -Name 'GameRootAnchorMode' -Expected $gameRootAnchorMode) -and
        (Test-ManifestStringExact -Object $manifest -Name 'PackageAnchorMode' -Expected $packageAnchorMode) -and
        (Test-ManifestStringExact -Object $manifest -Name 'TrustAnchorMode' -Expected $trustAnchorMode)

    Add-Check -Checks $checks -Name 'scaffold_manifest_present' -Passed $manifestFileExists -Detail "path=$manifestPath"
    Add-Check -Checks $checks -Name 'scaffold_manifest_owner_run_required' -Passed (Test-ManifestBool -Object $manifest -Name 'OwnerRunRequired' -Expected $true) -Detail 'run-manifest.json must come from the no-launch owner-run scaffold'
    Add-Check -Checks $checks -Name 'scaffold_manifest_does_not_launch_game' -Passed (Test-ManifestBool -Object $manifest -Name 'DoesNotLaunchGame' -Expected $true) -Detail 'scaffold command is a no-launch preparation step'
    Add-Check -Checks $checks -Name 'scaffold_manifest_no_launch' -Passed (Test-ManifestString -Object $manifest -Name 'LaunchMethod' -Expected 'owner-steam-launch-required') -Detail 'checker must not promote evidence without the owner Steam launch boundary'
    Add-Check -Checks $checks -Name 'scaffold_manifest_package_targets_match' -Passed $packageTargetsMatch -Detail "expectedPackage=$ExpectedPackageVersion expectedGame=$ExpectedGameVersion expectedRitsu=$ExpectedRitsuLibVersion expectedCompat=$ExpectedRitsuCompatBranch expectedPatchCount=$ExpectedPatchCount"
    Add-Check -Checks $checks -Name 'scaffold_manifest_git_head_bound' -Passed $manifestGitMatchesCurrent -Detail "manifestHead=$manifestGitHead currentHead=$currentGitHead currentDirty=$($currentGitEvidence.Dirty)"
    Add-Check -Checks $checks -Name 'scaffold_manifest_install_paths_match_configured_game_root' -Passed $manifestInstallPathsMatchConfigured -Detail "gameRoot=$resolvedGameRoot modsRoot=$resolvedModsRoot spirePlusInstallDir=$expectedSpirePlusInstallDir ritsuLibInstallDir=$expectedRitsuLibInstallDir"
    Add-Check -Checks $checks -Name 'scaffold_manifest_package_zip_hash_bound' -Passed $packageZipMatchesCurrentFile -Detail "run-manifest.json must match current package zip path/length/Sha256; expectedPath=$resolvedPackageZipPath"
    Add-Check -Checks $checks -Name 'scaffold_manifest_trust_anchor_mode_bound' -Passed $manifestAnchorModesMatch -Detail "gameRootAnchorMode=$gameRootAnchorMode packageAnchorMode=$packageAnchorMode trustAnchorMode=$trustAnchorMode"
    Add-Check -Checks $checks -Name 'scaffold_manifest_spire_plus_files_hash_bound' -Passed $spirePlusFilesMatchCurrentInstall -Detail "hashBoundSpirePlusFiles=$spirePlusHashBoundCount installDir=$expectedSpirePlusInstallDir requiredCurrent=EZMicroBalance.dll,EZMicroBalance.json,EZMicroBalance.pck"
    Add-Check -Checks $checks -Name 'scaffold_manifest_ritsu_lib_files_hash_bound' -Passed $ritsuLibFilesMatchCurrentInstall -Detail "hashBoundRitsuLibFiles=$ritsuLibHashBoundCount installDir=$expectedRitsuLibInstallDir requiredCurrent=STS2-RitsuLib.dll,STS2-RitsuLib.xml"
}

Add-Check -Checks $checks -Name 'game_version_marker' -Passed ([regex]::IsMatch($text, $gameVersionMarkerPattern)) -Detail "expected explicit release/host version marker for $ExpectedGameVersion"
Add-Check -Checks $checks -Name 'ritsu_lib_version_marker' -Passed ($text -match [regex]::Escape("RitsuLib Version: $ExpectedRitsuLibVersion")) -Detail "expected=$ExpectedRitsuLibVersion"
Add-Check -Checks $checks -Name 'ritsu_compat_branch_marker' -Passed ($text -match [regex]::Escape("compat branch: $ExpectedRitsuCompatBranch")) -Detail "expected=$ExpectedRitsuCompatBranch"
Add-Check -Checks $checks -Name 'spire_plus_package_marker' -Passed ($text -match [regex]::Escape($ExpectedPackageVersion)) -Detail "expected=$ExpectedPackageVersion"
Add-Check -Checks $checks -Name 'ritsu_lib_loaded_marker' -Passed ($text -match '(?i)\b(?:STS2-)?RitsuLib\b.*\bload(?:ed|ing)?\b|\bload(?:ed|ing)?\b.*\b(?:STS2-)?RitsuLib\b') -Detail 'RitsuLib load marker must be visible in runtime log'
Add-Check -Checks $checks -Name 'spire_plus_loaded_marker' -Passed ($text -match '(?i)\b(?:Spire Plus|EZMicroBalance)\b.*\bload(?:ed|ing)?\b|\bload(?:ed|ing)?\b.*\b(?:Spire Plus|EZMicroBalance)\b') -Detail 'Spire Plus load marker must be visible in runtime log'
Add-Check -Checks $checks -Name 'main_menu_marker' -Passed ($text -match '(?i)Time to main menu|main menu') -Detail 'main menu marker must appear before baseline is runtime-complete'
Add-Check -Checks $checks -Name 'mod_manifest_ids_present' -Passed ($modManifestIds.Count -gt 0) -Detail "actual=$($modManifestIds -join ',')"
Add-Check -Checks $checks -Name 'ritsu_only_mod_manifest_set_exact' -Passed (Test-StringSetEquals -Actual $modManifestIds -Expected $expectedModManifestIds) -Detail "expected=$($expectedModManifestIds -join ',') actual=$($modManifestIds -join ',')"
Add-Check -Checks $checks -Name 'no_base_lib_marker' -Passed (-not ($text -match '(?i)\bBaseLib\b')) -Detail 'beta.135 baseline evidence must remain RitsuLib-only and reject BaseLib markers'
Add-Check -Checks $checks -Name 'patch_summary_present' -Passed ([bool]$patch.SummaryFound) -Detail 'ModPatcher patch application summary must be present'
Add-Check -Checks $checks -Name 'patch_count_expected' -Passed ($patch.Applied -eq $ExpectedPatchCount -and $patch.Total -eq $ExpectedPatchCount) -Detail "expected=$ExpectedPatchCount applied=$($patch.Applied) total=$($patch.Total)"
Add-Check -Checks $checks -Name 'patch_failures_zero' -Passed ($patch.Failed -eq 0) -Detail "failed=$($patch.Failed)"
Add-Check -Checks $checks -Name 'patch_ignored_zero' -Passed ($patch.Ignored -eq 0) -Detail "ignored=$($patch.Ignored)"
Add-Check -Checks $checks -Name 'patch_registered_expected' -Passed ($patch.RegisteredFound -and $patch.RegisteredApplied -eq $ExpectedPatchCount -and $patch.Registered -eq $ExpectedPatchCount) -Detail "expected=$ExpectedPatchCount registeredApplied=$($patch.RegisteredApplied) registered=$($patch.Registered)"

$blockingPatterns = @(
    'MissingMethodException',
    'TypeLoadException',
    'TargetInvocationException',
    'loader failure',
    'initializer exception',
    '(?m)^\s*(?:\[ERROR\]|ERROR\b|\[[^\]]+\]\s*ERROR\b)'
)
foreach ($pattern in $blockingPatterns) {
    Add-Check -Checks $checks -Name ('no_' + ($pattern -replace '[^A-Za-z0-9]+', '_').Trim('_').ToLowerInvariant()) -Passed (-not [regex]::IsMatch($text, $pattern, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)) -Detail "pattern=$pattern"
}

$auditRan = $false
if (-not $SelfTest) {
    Assert-NoReparsePointInPath -Path $auditOutputPath
    Assert-OutputFileDoesNotAliasProtectedEvidence -OutputPath $auditOutputPath -ProtectedPaths $baselineProtectedPaths -Description 'Godot log audit output'
}
if (Test-Path -LiteralPath $auditScript -PathType Leaf) {
    & $auditScript -Path $resolvedLogPath -OutFile $auditOutputPath | Out-Null
    Assert-LeafIsNotReparsePoint -Path $auditOutputPath -Description 'Godot log audit output'
    $auditRan = $true
}
$auditSummary = if ($auditRan) { Read-AuditSummary -Path $auditOutputPath -ExpectedLogPath $resolvedLogPath } else { New-InvalidAuditSummary -ErrorMessage 'audit script did not run' }
Add-Check -Checks $checks -Name 'godot_log_audit_generated' -Passed $auditRan -Detail "path=$auditOutputPath"
Add-Check -Checks $checks -Name 'godot_log_audit_json_valid' -Passed ([bool]$auditSummary.JsonValid) -Detail $auditSummary.Error
Add-Check -Checks $checks -Name 'godot_log_audit_clean_bool' -Passed ([int]$auditSummary.MalformedBoolValues -eq 0) -Detail 'godot-log-audit.json Clean must be retained as a native JSON boolean'
Add-Check -Checks $checks -Name 'godot_log_audit_signature_hits_array' -Passed ([int]$auditSummary.MalformedArrayValues -eq 0) -Detail 'godot-log-audit.json root and SignatureHits must be retained as native JSON arrays'
Add-Check -Checks $checks -Name 'godot_log_audit_numeric_fields_native' -Passed ([int]$auditSummary.MalformedNumericValues -eq 0) -Detail 'godot-log-audit.json Length and SignatureHits[].Count must be native JSON integers'
Add-Check -Checks $checks -Name 'godot_log_audit_schema_fields_current' -Passed ([int]$auditSummary.MalformedSchemaValues -eq 0) -Detail "godot-log-audit.json must retain AuditSchemaVersion=2, valid SignatureSetSha256, and named SignatureHits; malformedSchema=$($auditSummary.MalformedSchemaValues)"
Add-Check -Checks $checks -Name 'godot_log_audit_file_binding_current' -Passed ([int]$auditSummary.MalformedFileBindingValues -eq 0) -Detail "godot-log-audit.json Path/Length/Sha256 must bind to LogPath; malformedFileBinding=$($auditSummary.MalformedFileBindingValues)"
Add-Check -Checks $checks -Name 'godot_log_audit_has_single_schema_version' -Passed (@($auditSummary.AuditSchemaVersions).Count -eq 1 -and [string]@($auditSummary.AuditSchemaVersions)[0] -eq '2') -Detail "schemaVersions=$(@($auditSummary.AuditSchemaVersions) -join ',')"
Add-Check -Checks $checks -Name 'godot_log_audit_has_single_signature_set_sha256' -Passed (@($auditSummary.SignatureSetSha256s).Count -eq 1) -Detail "signatureSetSha256Count=$(@($auditSummary.SignatureSetSha256s).Count)"
Add-Check -Checks $checks -Name 'godot_log_audit_signature_vector_present' -Passed (@($auditSummary.SignatureHitVector).Count -gt 0) -Detail "signatureVectorCount=$(@($auditSummary.SignatureHitVector).Count)"
Add-Check -Checks $checks -Name 'godot_log_audit_signature_names_current' -Passed (Test-StringSetEquals -Actual @($auditSummary.SignatureHitNames) -Expected $expectedAuditSignatureNames) -Detail "expected=$($expectedAuditSignatureNames -join ',') actual=$(@($auditSummary.SignatureHitNames) -join ',')"
Add-Check -Checks $checks -Name 'godot_log_audit_clean' -Passed ([bool]$auditSummary.Clean) -Detail "audit must have zero dirty items, zero signature hits, a non-empty signature vector, current file binding, and native array/numeric/bool/schema fields; dirty=$($auditSummary.DirtyItems), hits=$($auditSummary.SignatureHitCount), malformedArray=$($auditSummary.MalformedArrayValues), malformedNumeric=$($auditSummary.MalformedNumericValues), malformedBool=$($auditSummary.MalformedBoolValues), malformedSchema=$($auditSummary.MalformedSchemaValues), malformedFileBinding=$($auditSummary.MalformedFileBindingValues)"

$failedChecks = @($checks | Where-Object { -not [bool]$_.Passed })
$report = [pscustomobject]@{
    SchemaVersion = 1
    Status = if ($failedChecks.Count -eq 0) { 'pass' } else { 'fail' }
    CheckedAtUtc = [System.DateTimeOffset]::UtcNow.ToString('o')
    LogPath = $resolvedLogPath
    EvidenceDir = $resolvedEvidenceDir
    ExpectedPackageVersion = $ExpectedPackageVersion
    ExpectedGameVersion = $ExpectedGameVersion
    ExpectedRitsuLibVersion = $ExpectedRitsuLibVersion
    ExpectedRitsuCompatBranch = $ExpectedRitsuCompatBranch
    ExpectedPatchCount = $ExpectedPatchCount
    ConfiguredGameRoot = $resolvedGameRoot
    ConfiguredModsRoot = $resolvedModsRoot
    ExpectedSpirePlusInstallDir = $expectedSpirePlusInstallDir
    ExpectedRitsuLibInstallDir = $expectedRitsuLibInstallDir
    ExpectedPackageZipPath = $resolvedPackageZipPath
    GameRootAnchorMode = $gameRootAnchorMode
    PackageAnchorMode = $packageAnchorMode
    TrustAnchorMode = $trustAnchorMode
    CurrentGit = $currentGitEvidence
    ExpectedModManifestIds = @($expectedModManifestIds)
    ObservedModManifestIds = @($modManifestIds)
    PatchSummary = $patch
    AuditSummary = $auditSummary
    Checks = @($checks)
    FailedChecks = @($failedChecks)
}

if (-not $SelfTest) {
    Assert-NoReparsePointInPath -Path $reportPath
    Assert-OutputFileDoesNotAliasProtectedEvidence -OutputPath $reportPath -ProtectedPaths $baselineProtectedPaths -Description 'Runtime baseline log check report'
}
$report | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $reportPath -Encoding UTF8
Assert-LeafIsNotReparsePoint -Path $reportPath -Description 'Runtime baseline log check report'

$manifestStatus = if ($report.Status -eq 'pass') { 'runtime-baseline-log-markers-checked' } else { 'runtime-baseline-log-check-failed' }
$runtimeAuditStatus = if ($report.Status -eq 'pass') { 'checked-clean' } else { 'check-failed' }
if (-not $SelfTest) {
    Assert-OutputFileDoesNotAliasProtectedEvidence -OutputPath $manifestPath -ProtectedPaths $baselineProtectedPaths -Description 'Runtime baseline manifest'
}
Set-ObjectProperty -Object $manifest -Name 'SchemaVersion' -Value 1
Set-ObjectProperty -Object $manifest -Name 'Status' -Value $manifestStatus
Set-ObjectProperty -Object $manifest -Name 'EvidenceDir' -Value $resolvedEvidenceDir
Set-ObjectProperty -Object $manifest -Name 'GodotLogAfterLaunch' -Value $resolvedLogPath
Set-ObjectProperty -Object $manifest -Name 'GodotLogAfterLaunchRecord' -Value (Get-FileRecord -Path $resolvedLogPath)
Set-ObjectProperty -Object $manifest -Name 'GodotLogAuditRecord' -Value (Get-FileRecord -Path $auditOutputPath)
Set-ObjectProperty -Object $manifest -Name 'RuntimeBaselineLogCheckRecord' -Value (Get-FileRecord -Path $reportPath)
Set-ObjectProperty -Object $manifest -Name 'RuntimeLogStatus' -Value 'provided'
Set-ObjectProperty -Object $manifest -Name 'RuntimeAuditStatus' -Value $runtimeAuditStatus
Set-ObjectProperty -Object $manifest -Name 'BaselineLogCheckStatus' -Value $report.Status
Set-ObjectProperty -Object $manifest -Name 'BaselineLogCheckUpdatedAtUtc' -Value ([System.DateTimeOffset]::UtcNow.ToString('o'))
Set-ObjectProperty -Object $manifest -Name 'ExpectedRuntimePatchCount' -Value $ExpectedPatchCount
Set-ObjectProperty -Object $manifest -Name 'ExpectedModManifestIds' -Value @($expectedModManifestIds)
Set-ObjectProperty -Object $manifest -Name 'ObservedModManifestIds' -Value @($modManifestIds)
Set-ObjectProperty -Object $manifest -Name 'GameRootAnchorMode' -Value $gameRootAnchorMode
Set-ObjectProperty -Object $manifest -Name 'PackageAnchorMode' -Value $packageAnchorMode
Set-ObjectProperty -Object $manifest -Name 'TrustAnchorMode' -Value $trustAnchorMode
if (-not $SelfTest) {
    Set-ObjectProperty -Object $manifest -Name 'GameRoot' -Value $resolvedGameRoot
    Set-ObjectProperty -Object $manifest -Name 'ModsRoot' -Value $resolvedModsRoot
    Set-ObjectProperty -Object $manifest -Name 'SpirePlusInstallDir' -Value $expectedSpirePlusInstallDir
    Set-ObjectProperty -Object $manifest -Name 'RitsuLibInstallDir' -Value $expectedRitsuLibInstallDir
    Set-ObjectProperty -Object $manifest -Name 'PackageZipPath' -Value $resolvedPackageZipPath
    Set-ObjectProperty -Object $manifest -Name 'LastCheckedGit' -Value $currentGitEvidence
    Set-ObjectProperty -Object $manifest -Name 'LogOriginProofStatus' -Value 'marker-only-origin-not-proven-by-offline-checker'
}
$manifest | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $manifestPath -Encoding UTF8
Assert-LeafIsNotReparsePoint -Path $manifestPath -Description 'Runtime baseline manifest'

Write-Output "runtime_baseline_log_check status=$($report.Status) checks=$($checks.Count) failed=$($failedChecks.Count) report=$reportPath"
foreach ($check in $checks) {
    $status = if ([bool]$check.Passed) { 'pass' } else { 'fail' }
    Write-Output "check $($check.Name) status=$status detail='$($check.Detail)'"
}

if ($SelfTest -and (Test-Path -LiteralPath $tempRoot)) {
    Remove-Item -LiteralPath $tempRoot -Recurse -Force
}

if ($FailOnMismatch -and $failedChecks.Count -gt 0) {
    exit 1
}
