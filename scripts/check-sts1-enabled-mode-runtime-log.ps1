param(
    [ValidateSet('Off', 'CanaryOnly', 'AdditiveBatch1')]
    [string]$Mode = 'Off',

    [string]$LogPath,
    [string]$AuditPath,
    [string]$ExpectedPackageVersion,
    [string]$ExpectedRitsuCompatBranch,
    [string]$ExpectedRitsuLibVersion,
    [string]$ExpectedGameVersion,
    [string]$RegistrationServicePath = 'EZMicroBalanceCode\Sts1Events\Runtime\Sts1EventRegistrationService.cs',
    [string]$OutFile,
    [switch]$PrintExpected,
    [switch]$FailOnMismatch
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
$logAuditScript = Join-Path $PSScriptRoot 'audit-godot-log.ps1'
$checks = [System.Collections.Generic.List[object]]::new()
$mismatches = [System.Collections.Generic.List[string]]::new()

function Resolve-RepoPath {
    param([Parameter(Mandatory = $true)][string]$Path)

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }

    return [System.IO.Path]::GetFullPath((Join-Path $repoRoot $Path))
}

function Initialize-WindowsFileIdentityType {
    if ([System.Environment]::OSVersion.Platform -ne [System.PlatformID]::Win32NT) {
        return $false
    }

    if ('SpirePlusSts1RuntimeFileIdentity' -as [type]) {
        return $true
    }

    try {
        Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

public static class SpirePlusSts1RuntimeFileIdentity
{
    private const uint FileReadAttributes = 0x00000080;
    private const uint FileShareRead = 0x00000001;
    private const uint FileShareWrite = 0x00000002;
    private const uint FileShareDelete = 0x00000004;
    private const uint OpenExisting = 3;
    private const uint FileFlagBackupSemantics = 0x02000000;

    [StructLayout(LayoutKind.Sequential)]
    private struct FileTime
    {
        public uint LowDateTime;
        public uint HighDateTime;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct ByHandleFileInformation
    {
        public uint FileAttributes;
        public FileTime CreationTime;
        public FileTime LastAccessTime;
        public FileTime LastWriteTime;
        public uint VolumeSerialNumber;
        public uint FileSizeHigh;
        public uint FileSizeLow;
        public uint NumberOfLinks;
        public uint FileIndexHigh;
        public uint FileIndexLow;
    }

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern SafeFileHandle CreateFileW(
        string fileName,
        uint desiredAccess,
        uint shareMode,
        IntPtr securityAttributes,
        uint creationDisposition,
        uint flagsAndAttributes,
        IntPtr templateFile);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetFileInformationByHandle(
        SafeFileHandle fileHandle,
        out ByHandleFileInformation fileInformation);

    public static string GetIdentity(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }

        using (SafeFileHandle handle = CreateFileW(
            path,
            FileReadAttributes,
            FileShareRead | FileShareWrite | FileShareDelete,
            IntPtr.Zero,
            OpenExisting,
            FileFlagBackupSemantics,
            IntPtr.Zero))
        {
            if (handle.IsInvalid)
            {
                return string.Empty;
            }

            ByHandleFileInformation information;
            if (!GetFileInformationByHandle(handle, out information))
            {
                return string.Empty;
            }

            return string.Format(
                "{0:x8}:{1:x8}:{2:x8}",
                information.VolumeSerialNumber,
                information.FileIndexHigh,
                information.FileIndexLow);
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

    if ([string]::IsNullOrWhiteSpace($Path) -or -not (Test-Path -LiteralPath $Path)) {
        return ''
    }

    if (-not (Initialize-WindowsFileIdentityType)) {
        return ''
    }

    try {
        return [SpirePlusSts1RuntimeFileIdentity]::GetIdentity([System.IO.Path]::GetFullPath($Path))
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

function Test-PathUnderPhysicalDirectory {
    param(
        [AllowEmptyString()][string]$Path,
        [AllowEmptyString()][string]$Directory
    )

    if ([string]::IsNullOrWhiteSpace($Path) -or [string]::IsNullOrWhiteSpace($Directory)) {
        return $false
    }

    try {
        $candidate = [System.IO.Path]::GetDirectoryName([System.IO.Path]::GetFullPath($Path))
        while (-not [string]::IsNullOrWhiteSpace($candidate)) {
            if (Test-SameExistingPathPhysicalIdentity -Left $candidate -Right $Directory) {
                return $true
            }

            $parent = [System.IO.Path]::GetDirectoryName($candidate)
            if ([string]::Equals($parent, $candidate, [System.StringComparison]::OrdinalIgnoreCase)) {
                break
            }

            $candidate = $parent
        }
    } catch {
    }

    return $false
}

function Test-PathInsideDirectory {
    param(
        [AllowEmptyString()][string]$Path,
        [AllowEmptyString()][string]$Directory
    )

    if ([string]::IsNullOrWhiteSpace($Path) -or [string]::IsNullOrWhiteSpace($Directory)) {
        return $false
    }

    try {
        $pathFull = [System.IO.Path]::GetFullPath($Path)
        $directoryFull = [System.IO.Path]::GetFullPath($Directory)
        if (-not $directoryFull.EndsWith([System.IO.Path]::DirectorySeparatorChar)) {
            $directoryFull = $directoryFull + [System.IO.Path]::DirectorySeparatorChar
        }

        return $pathFull.StartsWith($directoryFull, [System.StringComparison]::OrdinalIgnoreCase)
    } catch {
        return $false
    }
}

function Add-ProtectedOutputPath {
    param(
        [Parameter(Mandatory = $true)]$ProtectedPaths,
        [AllowEmptyString()][string]$Path
    )

    if ([string]::IsNullOrWhiteSpace($Path)) {
        return
    }

    try {
        $ProtectedPaths.Add([System.IO.Path]::GetFullPath($Path)) | Out-Null
    } catch {
    }
}

function Add-ProtectedOutputRootsForEvidenceFile {
    param(
        [Parameter(Mandatory = $true)]$ProtectedRoots,
        [AllowEmptyString()][string]$Path
    )

    if ([string]::IsNullOrWhiteSpace($Path)) {
        return
    }

    try {
        $resolvedPath = [System.IO.Path]::GetFullPath($Path)
        $directory = [System.IO.Path]::GetDirectoryName($resolvedPath)
        if (-not [string]::IsNullOrWhiteSpace($directory)) {
            $ProtectedRoots.Add($directory) | Out-Null
            $parent = [System.IO.Path]::GetDirectoryName($directory)
            if (-not [string]::IsNullOrWhiteSpace($parent)) {
                $ProtectedRoots.Add($parent) | Out-Null
            }
        }
    } catch {
    }
}

function Assert-OutFileDoesNotOverwriteExplicitEvidence {
    param(
        [Parameter(Mandatory = $true)][string]$ResolvedOutFile,
        [Parameter(Mandatory = $true)]$ProtectedPaths,
        [Parameter(Mandatory = $true)][string]$Message
    )

    $resolvedFullPath = [System.IO.Path]::GetFullPath($ResolvedOutFile)
    foreach ($path in @($ProtectedPaths)) {
        if ([string]::IsNullOrWhiteSpace([string]$path)) {
            continue
        }

        if ([string]::Equals($resolvedFullPath, [System.IO.Path]::GetFullPath([string]$path), [System.StringComparison]::OrdinalIgnoreCase)) {
            throw "$Message $ResolvedOutFile"
        }

        if (Test-SameExistingPathPhysicalIdentity -Left $resolvedFullPath -Right ([string]$path)) {
            throw "$Message $ResolvedOutFile"
        }
    }
}

function Assert-OutFileDoesNotOverwriteCanonicalEvidence {
    param(
        [Parameter(Mandatory = $true)][string]$ResolvedOutFile,
        [Parameter(Mandatory = $true)]$ProtectedRoots
    )

    $canonicalNames = @(
        'godot.log',
        'godot.log.before',
        'godot.log.after-launch',
        'godot.log.current-iteration',
        'godot-log-audit.json',
        'godot-log-current-iteration-audit.json',
        'godot-log-after-launch-audit.json'
    )
    foreach ($root in @($ProtectedRoots)) {
        if ([string]::IsNullOrWhiteSpace([string]$root)) {
            continue
        }

        foreach ($canonicalName in $canonicalNames) {
            $canonicalPath = Join-Path ([string]$root) $canonicalName
            if (Test-SameExistingPathPhysicalIdentity -Left $ResolvedOutFile -Right $canonicalPath) {
                throw "Refusing to write OutFile over canonical StS1 runtime evidence: $ResolvedOutFile"
            }
        }
    }

    $leafName = [System.IO.Path]::GetFileName($ResolvedOutFile)
    if ($canonicalNames -notcontains $leafName) {
        return
    }

    foreach ($root in @($ProtectedRoots)) {
        if ([string]::IsNullOrWhiteSpace([string]$root)) {
            continue
        }

        if ((Test-PathInsideDirectory -Path $ResolvedOutFile -Directory ([string]$root)) -or
            (Test-PathUnderPhysicalDirectory -Path $ResolvedOutFile -Directory ([string]$root))) {
            throw "Refusing to write OutFile over canonical StS1 runtime evidence: $ResolvedOutFile"
        }
    }
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

function Read-RegistrationServiceText {
    param([Parameter(Mandatory = $true)][string]$Path)

    $resolved = Resolve-RepoPath $Path
    if (-not (Test-Path -LiteralPath $resolved -PathType Leaf)) {
        Write-Error "Registration service file not found: $resolved"
        exit 1
    }

    if ([System.IO.Path]::GetFileNameWithoutExtension($resolved) -ne 'Sts1EventRegistrationService') {
        return [System.IO.File]::ReadAllText($resolved)
    }

    $directory = [System.IO.Path]::GetDirectoryName($resolved)
    $files = @(Get-ChildItem -LiteralPath $directory -Filter 'Sts1EventRegistrationService*.cs' -File | Sort-Object FullName)
    if ($files.Count -eq 0) {
        Write-Error "No Sts1EventRegistrationService partial files found under: $directory"
        exit 1
    }

    return ($files | ForEach-Object { [System.IO.File]::ReadAllText($_.FullName) }) -join [System.Environment]::NewLine
}

function Get-RegistrationServiceInputPaths {
    param([Parameter(Mandatory = $true)][string]$Path)

    $resolved = Resolve-RepoPath $Path
    if ([System.IO.Path]::GetFileNameWithoutExtension($resolved) -ne 'Sts1EventRegistrationService') {
        return @($resolved)
    }

    $directory = [System.IO.Path]::GetDirectoryName($resolved)
    return @(Get-ChildItem -LiteralPath $directory -Filter 'Sts1EventRegistrationService*.cs' -File | Sort-Object FullName | ForEach-Object { $_.FullName })
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
    param([Parameter(Mandatory = $true)][string]$Block)

    $items = [System.Collections.Generic.List[object]]::new()

    foreach ($match in [regex]::Matches($Block, 'content\.ActEvent<\s*([A-Za-z0-9_]+)\s*,\s*([A-Za-z0-9_]+)\s*>\s*\(')) {
        $items.Add([pscustomobject]@{
            Kind = 'ActEvent'
            Act = $match.Groups[1].Value
            Event = $match.Groups[2].Value
        }) | Out-Null
    }

    foreach ($match in [regex]::Matches($Block, 'content\.SharedEvent<\s*([A-Za-z0-9_]+)\s*>\s*\(')) {
        $items.Add([pscustomobject]@{
            Kind = 'SharedEvent'
            Act = 'Shared'
            Event = $match.Groups[1].Value
        }) | Out-Null
    }

    return @($items)
}

function Get-ExpectedModeShape {
    param(
        [Parameter(Mandatory = $true)][string]$Text,
        [Parameter(Mandatory = $true)][string]$ModeName
    )

    if ($ModeName -eq 'Off') {
        return [pscustomobject]@{
            Mode = $ModeName
            MethodName = ''
            ExpectedRegistrationCalls = 0
            ExpectedEventClasses = @()
            ExpectedRegistrationTuples = @()
            ExpectedEventTypes = 0
            ReasonNeedle = 'StS1 events default Off; set SPIREPLUS_STS1_EVENT_MODE to enable.'
            StartNeedle = ''
            SuccessNeedle = ''
        }
    }

    $methodName = if ($ModeName -eq 'CanaryOnly') { 'RegisterCanaryOnly' } else { 'RegisterAdditiveBatch1' }
    $slice = Get-MethodSlice -Text $Text -MethodName $methodName
    $registrations = @(Get-Registrations -Block $slice)
    $classes = @($registrations | Select-Object -ExpandProperty Event | Sort-Object -Unique)
    $tuples = @($registrations | ForEach-Object { "$($_.Kind):$($_.Act):$($_.Event)" })

    $reasonNeedle = if ($ModeName -eq 'CanaryOnly') {
        'StS1 events CanaryOnly mode: registering 4 canary events.'
    } else {
        'StS1 events AdditiveBatch1 mode: registering 10 verified-scope events.'
    }

    $startNeedle = if ($ModeName -eq 'CanaryOnly') {
        '[StS1 Events] Registering canary events'
    } else {
        '[StS1 Events] Registering AdditiveBatch1 events'
    }

    $successNeedle = if ($ModeName -eq 'CanaryOnly') {
        '[StS1 Events] Canary events registered successfully.'
    } else {
        '[StS1 Events] AdditiveBatch1 events registered successfully.'
    }

    return [pscustomobject]@{
        Mode = $ModeName
        MethodName = $methodName
        ExpectedRegistrationCalls = $registrations.Count
        ExpectedEventClasses = $classes
        ExpectedRegistrationTuples = $tuples
        ExpectedEventTypes = $classes.Count
        ReasonNeedle = $reasonNeedle
        StartNeedle = $startNeedle
        SuccessNeedle = $successNeedle
    }
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

function Get-JsonProperty {
    param(
        [AllowNull()]$Object,
        [Parameter(Mandatory = $true)][string]$Name
    )

    if ($null -eq $Object) {
        return @()
    }

    return @($Object.PSObject.Properties | Where-Object {
        $_.MemberType -eq [System.Management.Automation.PSMemberTypes]::NoteProperty -and
            [string]::Equals($_.Name, $Name, [System.StringComparison]::Ordinal)
    })
}

function Test-JsonProperty {
    param(
        [AllowNull()]$Object,
        [Parameter(Mandatory = $true)][string]$Name
    )

    return @(Get-JsonProperty -Object $Object -Name $Name).Count -gt 0
}

function Test-RawJsonRootArray {
    param([AllowNull()][string]$Json)

    if ([string]::IsNullOrWhiteSpace($Json)) {
        return $false
    }

    return $Json.TrimStart().StartsWith('[', [System.StringComparison]::Ordinal)
}

function Test-NativeJsonIntegerValue {
    param([AllowNull()]$Value)

    if ($null -eq $Value -or $Value -is [bool] -or $Value -is [string]) {
        return $false
    }

    return (
        $Value -is [byte] -or
        $Value -is [sbyte] -or
        $Value -is [int16] -or
        $Value -is [uint16] -or
        $Value -is [int] -or
        $Value -is [uint32] -or
        $Value -is [long] -or
        $Value -is [uint64]
    )
}

function Test-NativeJsonInt32Value {
    param([AllowNull()]$Value)

    if (-not (Test-NativeJsonIntegerValue -Value $Value)) {
        return $false
    }

    try {
        $decimalValue = [decimal]$Value
        return $decimalValue -ge [int]::MinValue -and $decimalValue -le [int]::MaxValue
    } catch {
        return $false
    }
}

function Test-JsonBoolProperty {
    param(
        [AllowNull()]$Object,
        [Parameter(Mandatory = $true)][string]$Name
    )

    return (Test-JsonProperty -Object $Object -Name $Name) -and $Object.$Name -is [bool]
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

function Test-Sha256Text {
    param([AllowEmptyString()][string]$Value)

    return -not [string]::IsNullOrWhiteSpace($Value) -and $Value -match '^[A-Fa-f0-9]{64}$'
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

function Get-FileSha256OrEmpty {
    param([AllowEmptyString()][string]$Path)

    if ([string]::IsNullOrWhiteSpace($Path) -or -not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        return ''
    }

    return (Get-FileHash -Algorithm SHA256 -LiteralPath $Path).Hash.ToLowerInvariant()
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

function Get-GameVersionLineHits {
    param(
        [AllowEmptyString()][string]$Text,
        [Parameter(Mandatory = $true)][string]$ExpectedVersion
    )

    if ([string]::IsNullOrWhiteSpace($ExpectedVersion)) {
        return 0
    }

    $numericVersion = $ExpectedVersion.Trim()
    if ($numericVersion.StartsWith('v', [System.StringComparison]::OrdinalIgnoreCase)) {
        $numericVersion = $numericVersion.Substring(1)
    }

    $labelVersion = "v$numericVersion"
    $numericEscaped = [regex]::Escape($numericVersion)
    $labelEscaped = [regex]::Escape($labelVersion)
    $patterns = @(
        "(?im)\brelease\s*=\s*$labelEscaped\b",
        "(?im)\bHost Version:\s*$labelEscaped\b",
        "(?im)\bRelease Version:\s*$labelEscaped\b",
        "(?im)\bHost version label\s*=\s*$labelEscaped\s+numeric\s*=\s*$numericEscaped\b"
    )

    $hits = 0
    foreach ($pattern in $patterns) {
        $hits += [regex]::Matches($Text, $pattern).Count
    }

    return $hits
}

function Get-RitsuCompatBranchLineHits {
    param(
        [AllowEmptyString()][string]$Text,
        [Parameter(Mandatory = $true)][string]$ExpectedBranch
    )

    if ([string]::IsNullOrWhiteSpace($ExpectedBranch)) {
        return 0
    }

    $branchEscaped = [regex]::Escape($ExpectedBranch.Trim())
    $patterns = @(
        "(?im)\[compat branch:\s*$branchEscaped\]",
        "(?im)\bpicked variant\s+$branchEscaped\b"
    )

    $hits = 0
    foreach ($pattern in $patterns) {
        $hits += [regex]::Matches($Text, $pattern).Count
    }

    return $hits
}

function Get-RitsuLibVersionLineHits {
    param(
        [AllowEmptyString()][string]$Text,
        [Parameter(Mandatory = $true)][string]$ExpectedVersion
    )

    if ([string]::IsNullOrWhiteSpace($ExpectedVersion)) {
        return 0
    }

    $version = $ExpectedVersion.Trim()
    if ($version.StartsWith('v', [System.StringComparison]::OrdinalIgnoreCase)) {
        $version = $version.Substring(1)
    }

    $versionEscaped = [regex]::Escape($version)
    $patterns = @(
        "(?im)\bRitsuLib Version:\s*$versionEscaped\s+\[compat branch:",
        "(?im)\[com\.ritsukage\.sts2-RitsuLib\]\s+Version:\s*$versionEscaped\s+\[compat branch:",
        "(?im)\bRitsuLib\s+$versionEscaped\s+bootstrap starting\b",
        "(?im)\*\s+RitsuLib\s+\[STS2-RitsuLib\]\s+\($versionEscaped\)"
    )

    $hits = 0
    foreach ($pattern in $patterns) {
        $hits += [regex]::Matches($Text, $pattern).Count
    }

    return $hits
}

function Get-RegisteredEventClassesFromLog {
    param([AllowEmptyString()][string[]]$Lines)

    $registeredMatches = [System.Collections.Generic.List[object]]::new()
    $actPattern = 'Registered\s+act\s+event:\s+(Sts1[A-Za-z0-9_]+)\b.*?->\s*([A-Za-z0-9_]+)\b'
    $sharedPattern = 'Registered\s+shared\s+event:\s+(Sts1[A-Za-z0-9_]+)\b'
    $genericPattern = 'Registered\s+.*\bevent:\s+(Sts1[A-Za-z0-9_]+)\b'

    foreach ($line in $Lines) {
        $matched = $false

        foreach ($match in [regex]::Matches($line, $actPattern, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)) {
            $eventClass = $match.Groups[1].Value
            $actName = $match.Groups[2].Value
            $registeredMatches.Add([pscustomobject]@{
                ClassName = $eventClass
                Kind = 'ActEvent'
                Act = $actName
                Tuple = "ActEvent:${actName}:${eventClass}"
                Line = $line
            }) | Out-Null
            $matched = $true
        }

        if ($matched) {
            continue
        }

        foreach ($match in [regex]::Matches($line, $sharedPattern, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)) {
            $eventClass = $match.Groups[1].Value
            $registeredMatches.Add([pscustomobject]@{
                ClassName = $eventClass
                Kind = 'SharedEvent'
                Act = 'Shared'
                Tuple = "SharedEvent:Shared:${eventClass}"
                Line = $line
            }) | Out-Null
            $matched = $true
        }

        if ($matched) {
            continue
        }

        foreach ($match in [regex]::Matches($line, $genericPattern, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)) {
            $eventClass = $match.Groups[1].Value
            $registeredMatches.Add([pscustomobject]@{
                ClassName = $eventClass
                Kind = 'UnknownEvent'
                Act = 'Unknown'
                Tuple = "UnknownEvent:Unknown:${eventClass}"
                Line = $line
            }) | Out-Null
        }
    }

    return @($registeredMatches)
}

function ConvertTo-AuditSummary {
    param(
        [Parameter(Mandatory = $true)][string]$Json,
        [Parameter(Mandatory = $true)][string]$Path
    )

    $rootIsArray = Test-RawJsonRootArray -Json $Json
    try {
        $convertedJson = $Json | ConvertFrom-Json
    } catch {
        return [pscustomobject]@{
            Path = $Path
            Items = 0
            ItemPaths = @()
            ItemLengths = @()
            ItemSha256s = @()
            AuditSchemaVersions = @()
            SignatureSetSha256s = @()
            SignatureHitVector = @()
            DirtyItems = 0
            SignatureHitCount = 0
            MalformedNumericValues = 1
            MalformedBoolValues = 1
            MalformedArrayValues = 1
            MalformedSchemaValues = 1
            Clean = $false
        }
    }

    [object[]]$items = @(
        if ($rootIsArray -and $convertedJson -is [System.Array]) {
            foreach ($entry in $convertedJson) {
                $entry
            }
        } else {
            $convertedJson
        }
    )
    $hitCount = 0
    $dirtyItems = 0
    $malformedNumericValues = 0
    $malformedBoolValues = 0
    $malformedArrayValues = 0
    $malformedSchemaValues = 0
    $itemPaths = [System.Collections.Generic.List[string]]::new()
    $itemLengths = [System.Collections.Generic.List[long]]::new()
    $itemSha256s = [System.Collections.Generic.List[string]]::new()
    $auditSchemaVersions = [System.Collections.Generic.List[string]]::new()
    $signatureSetSha256s = [System.Collections.Generic.List[string]]::new()
    $signatureHitVector = [System.Collections.Generic.List[string]]::new()
    if (-not $rootIsArray) {
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

        if (-not (Test-JsonBoolProperty -Object $item -Name 'Clean')) {
            $malformedBoolValues++
        }

        if (-not ((Test-JsonBoolProperty -Object $item -Name 'Clean') -and [bool]$item.Clean)) {
            $dirtyItems++
        }

        if ((Test-JsonProperty -Object $item -Name 'Path') -and -not [string]::IsNullOrWhiteSpace([string]$item.Path)) {
            $normalizedItemPath = ConvertTo-NormalizedPathOrEmpty -Path ([string]$item.Path)
            if (-not [string]::IsNullOrWhiteSpace($normalizedItemPath)) {
                $itemPaths.Add($normalizedItemPath) | Out-Null
            }
        }

        if (-not (Test-JsonProperty -Object $item -Name 'Length')) {
            $malformedNumericValues++
        } elseif (Test-NativeJsonIntegerValue -Value $item.Length) {
            $itemLengths.Add([long]$item.Length) | Out-Null
        } else {
            $malformedNumericValues++
        }

        if ((Test-JsonProperty -Object $item -Name 'Sha256') -and -not [string]::IsNullOrWhiteSpace([string]$item.Sha256)) {
            $itemSha256s.Add(([string]$item.Sha256).ToLowerInvariant()) | Out-Null
        }

        $signatureHitsProperty = @(Get-JsonProperty -Object $item -Name 'SignatureHits' | Select-Object -First 1)
        if ($signatureHitsProperty.Count -ne 1 -or $null -eq $signatureHitsProperty[0].Value -or -not ($signatureHitsProperty[0].Value -is [System.Array])) {
            $malformedArrayValues++
            continue
        }

        foreach ($hit in @($signatureHitsProperty[0].Value)) {
            $hitName = ''
            if (Test-JsonProperty -Object $hit -Name 'Name') {
                $hitName = [string]$hit.Name
            }

            if ([string]::IsNullOrWhiteSpace($hitName)) {
                $malformedSchemaValues++
            }

            if (-not (Test-JsonProperty -Object $hit -Name 'Count')) {
                $malformedNumericValues++
                continue
            }

            if (Test-NativeJsonInt32Value -Value $hit.Count) {
                $hitCountValue = [int]$hit.Count
                $hitCount += $hitCountValue
                if (-not [string]::IsNullOrWhiteSpace($hitName)) {
                    $signatureHitVector.Add("$hitName=$hitCountValue") | Out-Null
                }
            } else {
                $malformedNumericValues++
            }
        }
    }

    return [pscustomobject]@{
        Path = $Path
        Items = $items.Count
        ItemPaths = @($itemPaths)
        ItemLengths = @($itemLengths)
        ItemSha256s = @($itemSha256s)
        AuditSchemaVersions = @($auditSchemaVersions.ToArray() | Sort-Object -Unique)
        SignatureSetSha256s = @($signatureSetSha256s.ToArray() | Sort-Object -Unique)
        SignatureHitVector = @($signatureHitVector.ToArray() | Sort-Object)
        DirtyItems = $dirtyItems
        SignatureHitCount = $hitCount
        MalformedNumericValues = $malformedNumericValues
        MalformedBoolValues = $malformedBoolValues
        MalformedArrayValues = $malformedArrayValues
        MalformedSchemaValues = $malformedSchemaValues
        Clean = ($items.Count -gt 0 -and $dirtyItems -eq 0 -and $hitCount -eq 0 -and $malformedNumericValues -eq 0 -and $malformedBoolValues -eq 0 -and $malformedArrayValues -eq 0 -and $malformedSchemaValues -eq 0)
    }
}

function Read-AuditSummary {
    param([Parameter(Mandatory = $true)][string]$Path)

    $resolved = Resolve-RepoPath $Path
    if (-not (Test-Path -LiteralPath $resolved)) {
        Write-Error "Audit file not found: $resolved"
        exit 1
    }

    $json = [System.IO.File]::ReadAllText($resolved)
    return ConvertTo-AuditSummary -Json $json -Path $resolved
}

function Invoke-RecomputedAuditSummary {
    param([Parameter(Mandatory = $true)][string]$LogPath)

    $auditJson = (& $logAuditScript -Path $LogPath | Out-String)
    if ([string]::IsNullOrWhiteSpace($auditJson)) {
        throw "audit-godot-log.ps1 returned empty output for $LogPath"
    }

    return ConvertTo-AuditSummary -Json $auditJson -Path '<recomputed>'
}

$registrationService = Read-RegistrationServiceText $RegistrationServicePath
$expected = Get-ExpectedModeShape -Text $registrationService -ModeName $Mode

Write-Output "mode=$($expected.Mode)"
Write-Output "expected_registration_calls=$($expected.ExpectedRegistrationCalls)"
Write-Output "expected_event_types=$($expected.ExpectedEventTypes)"
Write-Output "expected_event_classes=$((@($expected.ExpectedEventClasses) | Sort-Object) -join ',')"
Write-Output "expected_registration_tuples=$((@($expected.ExpectedRegistrationTuples) | Sort-Object) -join ',')"

$report = [ordered]@{
    Mode = $Mode
    ExpectedRegistrationCalls = $expected.ExpectedRegistrationCalls
    ExpectedEventTypes = $expected.ExpectedEventTypes
    ExpectedEventClasses = @($expected.ExpectedEventClasses)
    ExpectedRegistrationTuples = @($expected.ExpectedRegistrationTuples)
    RuntimeLogStatus = 'not-validated'
    Checks = $checks
    Mismatches = $mismatches
}

if ($PrintExpected -and -not $LogPath) {
    Write-Output 'runtime_log_status=not-validated-print-expected-only'

    if ($OutFile) {
        $resolvedOutFile = Resolve-RepoPath $OutFile
        $protectedVerifierInputs = [System.Collections.Generic.List[string]]::new()
        foreach ($path in @(Get-RegistrationServiceInputPaths -Path $RegistrationServicePath)) {
            Add-ProtectedOutputPath -ProtectedPaths $protectedVerifierInputs -Path ([string]$path)
        }

        Assert-OutFileDoesNotOverwriteExplicitEvidence -ResolvedOutFile $resolvedOutFile -ProtectedPaths $protectedVerifierInputs -Message 'Refusing to write OutFile over verifier input file:'
        $outDir = [System.IO.Path]::GetDirectoryName($resolvedOutFile)
        if ($outDir -and -not (Test-Path -LiteralPath $outDir)) {
            [void][System.IO.Directory]::CreateDirectory($outDir)
        }

        [pscustomobject]$report | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $resolvedOutFile -Encoding UTF8
    }

    Write-Output 'checks=0'
    Write-Output 'mismatches=0'
    exit 0
}

if (-not $LogPath) {
    Write-Error 'LogPath is required unless -PrintExpected is used without a log.'
    exit 1
}

$resolvedLogPath = Resolve-RepoPath $LogPath
if (-not (Test-Path -LiteralPath $resolvedLogPath)) {
    Write-Error "Log file not found: $resolvedLogPath"
    exit 1
}

$logText = [System.IO.File]::ReadAllText($resolvedLogPath)
$lines = if ($logText.Length -eq 0) { @() } else { $logText -split '\r?\n' }
$registeredEventMatches = @(Get-RegisteredEventClassesFromLog -Lines $lines)
$observedClasses = @($registeredEventMatches | Select-Object -ExpandProperty ClassName | Sort-Object -Unique)
$expectedClasses = @($expected.ExpectedEventClasses | Sort-Object -Unique)
$missingClasses = @($expectedClasses | Where-Object { $observedClasses -notcontains $_ })
$unexpectedClasses = @($observedClasses | Where-Object { $expectedClasses -notcontains $_ })
$observedTuples = @($registeredEventMatches | Select-Object -ExpandProperty Tuple | Sort-Object)
$expectedTuples = @($expected.ExpectedRegistrationTuples | Sort-Object)
$observedTupleCounts = [System.Collections.Generic.Dictionary[string, int]]::new([System.StringComparer]::Ordinal)
foreach ($tuple in $observedTuples) {
    if ($observedTupleCounts.ContainsKey($tuple)) {
        $observedTupleCounts[$tuple]++
    } else {
        $observedTupleCounts[$tuple] = 1
    }
}

$expectedTupleCounts = [System.Collections.Generic.Dictionary[string, int]]::new([System.StringComparer]::Ordinal)
foreach ($tuple in $expectedTuples) {
    if ($expectedTupleCounts.ContainsKey($tuple)) {
        $expectedTupleCounts[$tuple]++
    } else {
        $expectedTupleCounts[$tuple] = 1
    }
}

$tupleKeys = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
foreach ($tuple in $observedTuples) {
    $tupleKeys.Add($tuple) | Out-Null
}
foreach ($tuple in $expectedTuples) {
    $tupleKeys.Add($tuple) | Out-Null
}

$missingTupleDetails = [System.Collections.Generic.List[string]]::new()
$unexpectedTupleDetails = [System.Collections.Generic.List[string]]::new()
foreach ($tuple in @($tupleKeys | Sort-Object)) {
    $expectedTupleCount = if ($expectedTupleCounts.ContainsKey($tuple)) { $expectedTupleCounts[$tuple] } else { 0 }
    $observedTupleCount = if ($observedTupleCounts.ContainsKey($tuple)) { $observedTupleCounts[$tuple] } else { 0 }

    if ($observedTupleCount -lt $expectedTupleCount) {
        $missingTupleDetails.Add("${tuple} expected=$expectedTupleCount observed=$observedTupleCount") | Out-Null
    } elseif ($observedTupleCount -gt $expectedTupleCount) {
        $unexpectedTupleDetails.Add("${tuple} expected=$expectedTupleCount observed=$observedTupleCount") | Out-Null
    }
}

$missingTuples = @($missingTupleDetails)
$unexpectedTuples = @($unexpectedTupleDetails)

$reasonHits = [regex]::Matches($logText, [regex]::Escape($expected.ReasonNeedle)).Count
$enabledFeatureLineHits = [regex]::Matches($logText, 'Feature Sts1Events .*bootstrap=enabled, live=Enabled').Count
$disabledFeatureLineHits = [regex]::Matches($logText, 'Feature Sts1Events .*bootstrap=disabled, live=Disabled').Count
$startHits = if ([string]::IsNullOrWhiteSpace($expected.StartNeedle)) { 0 } else { [regex]::Matches($logText, [regex]::Escape($expected.StartNeedle)).Count }
$successHits = if ([string]::IsNullOrWhiteSpace($expected.SuccessNeedle)) { 0 } else { [regex]::Matches($logText, [regex]::Escape($expected.SuccessNeedle)).Count }
$ritsuInactiveHits = [regex]::Matches($logText, 'RitsuLib not active; skipping .*event registration').Count
$unsafeModeHits = [regex]::Matches($logText, 'AdditiveAllDraft|ReplaceUnknownEventsPrototype').Count
$hasExpectedPackageVersion = -not [string]::IsNullOrWhiteSpace($ExpectedPackageVersion)
$hasExpectedRitsuCompatBranch = -not [string]::IsNullOrWhiteSpace($ExpectedRitsuCompatBranch)
$hasExpectedRitsuLibVersion = -not [string]::IsNullOrWhiteSpace($ExpectedRitsuLibVersion)
$hasExpectedGameVersion = -not [string]::IsNullOrWhiteSpace($ExpectedGameVersion)
$expectedRitsuCompatBranchLineHits = if ($hasExpectedRitsuCompatBranch) { Get-RitsuCompatBranchLineHits -Text $logText -ExpectedBranch $ExpectedRitsuCompatBranch } else { 0 }
$expectedRitsuLibVersionLineHits = if ($hasExpectedRitsuLibVersion) { Get-RitsuLibVersionLineHits -Text $logText -ExpectedVersion $ExpectedRitsuLibVersion } else { 0 }
$expectedGameVersionLineHits = if ($hasExpectedGameVersion) { Get-GameVersionLineHits -Text $logText -ExpectedVersion $ExpectedGameVersion } else { 0 }

Write-Output "log_path=$resolvedLogPath"
Write-Output "observed_registered_event_lines=$($registeredEventMatches.Count)"
Write-Output "observed_event_types=$($observedClasses.Count)"
Write-Output "observed_event_classes=$(($observedClasses | Sort-Object) -join ',')"
Write-Output "observed_registration_tuples=$(($observedTuples | Sort-Object) -join ',')"
Write-Output "missing_registration_tuples=$(($missingTuples | Sort-Object) -join ',')"
Write-Output "unexpected_registration_tuples=$(($unexpectedTuples | Sort-Object) -join ',')"
Write-Output "mode_reason_hits=$reasonHits"
Write-Output "enabled_feature_line_hits=$enabledFeatureLineHits"
Write-Output "disabled_feature_line_hits=$disabledFeatureLineHits"
Write-Output "registration_start_hits=$startHits"
Write-Output "registration_success_hits=$successHits"
if (-not [string]::IsNullOrWhiteSpace($ExpectedPackageVersion)) {
    Write-Output "expected_package_version=$ExpectedPackageVersion"
}

if (-not [string]::IsNullOrWhiteSpace($ExpectedRitsuCompatBranch)) {
    Write-Output "expected_ritsu_compat_branch=$ExpectedRitsuCompatBranch"
    Write-Output "expected_ritsu_compat_branch_line_hits=$expectedRitsuCompatBranchLineHits"
}

if (-not [string]::IsNullOrWhiteSpace($ExpectedRitsuLibVersion)) {
    Write-Output "expected_ritsu_lib_version=$ExpectedRitsuLibVersion"
    Write-Output "expected_ritsu_lib_version_line_hits=$expectedRitsuLibVersionLineHits"
}

if (-not [string]::IsNullOrWhiteSpace($ExpectedGameVersion)) {
    Write-Output "expected_game_version=$ExpectedGameVersion"
    Write-Output "expected_game_version_line_hits=$expectedGameVersionLineHits"
}

Add-Check -Name 'mode_reason_present' -Passed ($reasonHits -gt 0) -Detail "expected log reason '$($expected.ReasonNeedle)'"
Add-Check -Name 'ritsulib_active_for_mode' -Passed ($ritsuInactiveHits -eq 0) -Detail 'RitsuLib inactive registration warning must be absent'

if ($hasExpectedPackageVersion) {
    Add-Check -Name 'expected_package_version_in_log' -Passed (Contains-Text -Text $logText -Needle $ExpectedPackageVersion) -Detail "expected package version '$ExpectedPackageVersion' in log"
}

if ($hasExpectedRitsuCompatBranch) {
    Add-Check -Name 'expected_ritsu_compat_branch_in_log' -Passed ($expectedRitsuCompatBranchLineHits -gt 0) -Detail "expected explicit RitsuLib compat branch line for '$ExpectedRitsuCompatBranch' in log"
}

if ($hasExpectedRitsuLibVersion) {
    Add-Check -Name 'expected_ritsu_lib_version_in_log' -Passed ($expectedRitsuLibVersionLineHits -gt 0) -Detail "expected explicit RitsuLib package version '$ExpectedRitsuLibVersion' in log"
}

if ($hasExpectedGameVersion) {
    Add-Check -Name 'expected_game_version_in_log' -Passed ($expectedGameVersionLineHits -gt 0) -Detail "expected explicit game version line for '$ExpectedGameVersion' in log"
}

if ($Mode -eq 'Off') {
    Add-Check -Name 'off_feature_line_disabled' -Passed ($disabledFeatureLineHits -gt 0) -Detail 'expected Feature Sts1Events bootstrap=disabled, live=Disabled'
    Add-Check -Name 'off_no_registered_sts1_event_lines' -Passed ($registeredEventMatches.Count -eq 0) -Detail 'Off mode must have zero registered StS1 event lines'
    Add-Check -Name 'off_no_registration_start' -Passed (-not [regex]::IsMatch($logText, '\[StS1 Events\]\s+Registering')) -Detail 'Off mode must not start StS1 registration'
    Add-Check -Name 'off_no_registration_success' -Passed (-not [regex]::IsMatch($logText, '\[StS1 Events\].*registered successfully')) -Detail 'Off mode must not complete StS1 registration'
} else {
    Add-Check -Name 'enabled_expected_package_version_parameter_provided' -Passed $hasExpectedPackageVersion -Detail 'Enabled-mode copied logs must be checked with -ExpectedPackageVersion'
    Add-Check -Name 'enabled_expected_ritsu_compat_branch_parameter_provided' -Passed $hasExpectedRitsuCompatBranch -Detail 'Enabled-mode copied logs must be checked with -ExpectedRitsuCompatBranch'
    Add-Check -Name 'enabled_expected_ritsu_lib_version_parameter_provided' -Passed $hasExpectedRitsuLibVersion -Detail 'Enabled-mode copied logs must be checked with -ExpectedRitsuLibVersion'
    Add-Check -Name 'enabled_expected_game_version_parameter_provided' -Passed $hasExpectedGameVersion -Detail 'Enabled-mode copied logs must be checked with -ExpectedGameVersion'
    Add-Check -Name 'enabled_audit_path_parameter_provided' -Passed (-not [string]::IsNullOrWhiteSpace($AuditPath)) -Detail 'Enabled-mode copied-log checks must include -AuditPath for godot-log-audit.json'
    Add-Check -Name 'enabled_outfile_parameter_provided' -Passed (-not [string]::IsNullOrWhiteSpace($OutFile)) -Detail 'Enabled-mode copied-log checks must be run with -OutFile so enabled-mode-log-check.json is retained'

    Add-Check -Name 'enabled_feature_line_present' -Passed ($enabledFeatureLineHits -gt 0) -Detail 'expected Feature Sts1Events bootstrap=enabled, live=Enabled'
    Add-Check -Name 'registration_start_present' -Passed ($startHits -gt 0) -Detail "expected '$($expected.StartNeedle)'"
    Add-Check -Name 'registration_success_present' -Passed ($successHits -gt 0) -Detail "expected '$($expected.SuccessNeedle)'"
    Add-Check -Name 'observed_registration_call_count' -Passed ($registeredEventMatches.Count -eq $expected.ExpectedRegistrationCalls) -Detail "expected $($expected.ExpectedRegistrationCalls) registered event lines, observed $($registeredEventMatches.Count)"
    Add-Check -Name 'observed_event_type_count' -Passed ($observedClasses.Count -eq $expected.ExpectedEventTypes) -Detail "expected $($expected.ExpectedEventTypes), observed $($observedClasses.Count)"
    Add-Check -Name 'observed_event_classes_match_expected' -Passed ($missingClasses.Count -eq 0 -and $unexpectedClasses.Count -eq 0) -Detail "missing=$($missingClasses -join ','); unexpected=$($unexpectedClasses -join ',')"
    Add-Check -Name 'observed_registration_tuples_match_expected' -Passed ($missingTuples.Count -eq 0 -and $unexpectedTuples.Count -eq 0) -Detail "missing=$($missingTuples -join ','); unexpected=$($unexpectedTuples -join ',')"
    Add-Check -Name 'no_unsafe_mode_runtime_lines' -Passed ($unsafeModeHits -eq 0) -Detail 'CanaryOnly/AdditiveBatch1 proof logs must not use unsafe StS1 modes'
}

$auditSummary = $null
$resolvedAuditPath = ''
if ($AuditPath) {
    $resolvedAuditPath = Resolve-RepoPath $AuditPath
    $auditSummary = Read-AuditSummary -Path $AuditPath
    Write-Output "audit_path=$($auditSummary.Path)"
    Write-Output "audit_items=$($auditSummary.Items)"
    Write-Output "audit_signature_hits=$($auditSummary.SignatureHitCount)"
    Write-Output "audit_dirty_items=$($auditSummary.DirtyItems)"
    Add-Check -Name 'audit_clean_bool' -Passed ([int]$auditSummary.MalformedBoolValues -eq 0) -Detail 'godot-log-audit.json Clean must be retained as a native JSON boolean'
    Add-Check -Name 'audit_array_fields_native' -Passed ([int]$auditSummary.MalformedArrayValues -eq 0) -Detail 'godot-log-audit.json root and SignatureHits must be retained as native JSON arrays'
    Add-Check -Name 'audit_numeric_fields_native' -Passed ([int]$auditSummary.MalformedNumericValues -eq 0) -Detail 'godot-log-audit.json Length and SignatureHits[].Count must be native JSON integers'
    Add-Check -Name 'audit_schema_fields_current' -Passed ([int]$auditSummary.MalformedSchemaValues -eq 0) -Detail "godot-log-audit.json must retain AuditSchemaVersion=2, valid SignatureSetSha256, and named SignatureHits; malformedSchema=$($auditSummary.MalformedSchemaValues)"
    Add-Check -Name 'audit_clean' -Passed ([bool]$auditSummary.Clean) -Detail "expected audit JSON to have zero dirty items, zero signature hits, and native array/numeric/bool/schema fields; dirty=$($auditSummary.DirtyItems), hits=$($auditSummary.SignatureHitCount), malformedArray=$($auditSummary.MalformedArrayValues), malformedNumeric=$($auditSummary.MalformedNumericValues), malformedBool=$($auditSummary.MalformedBoolValues), malformedSchema=$($auditSummary.MalformedSchemaValues)"

    $auditItemPaths = @($auditSummary.ItemPaths)
    $auditItemLengths = @($auditSummary.ItemLengths)
    $auditItemSha256s = @($auditSummary.ItemSha256s)
    $auditSchemaVersions = @($auditSummary.AuditSchemaVersions)
    $auditSignatureSetSha256s = @($auditSummary.SignatureSetSha256s)
    $auditSignatureHitVector = @($auditSummary.SignatureHitVector)
    $expectedAuditPath = [System.IO.Path]::GetFullPath($resolvedLogPath)
    $expectedAuditLength = [long](Get-Item -LiteralPath $resolvedLogPath).Length
    $expectedAuditSha256 = Get-FileSha256OrEmpty -Path $resolvedLogPath
    Add-Check -Name 'audit_has_single_schema_version' -Passed ($auditSchemaVersions.Count -eq 1 -and [string]$auditSchemaVersions[0] -eq '2') -Detail "audit JSON must retain exactly one current AuditSchemaVersion=2; found $($auditSchemaVersions -join ',')"
    Add-Check -Name 'audit_has_single_signature_set_sha256' -Passed ($auditSignatureSetSha256s.Count -eq 1) -Detail "audit JSON must retain exactly one SignatureSetSha256; found $($auditSignatureSetSha256s.Count)"
    Add-Check -Name 'audit_has_single_scanned_path' -Passed ($auditItemPaths.Count -eq 1) -Detail "audit JSON must retain exactly one scanned Path; found $($auditItemPaths.Count)"
    Add-Check -Name 'audit_path_matches_log_path' -Passed ($auditItemPaths.Count -eq 1 -and [System.StringComparer]::OrdinalIgnoreCase.Equals([string]$auditItemPaths[0], $expectedAuditPath)) -Detail 'godot-log-audit.json must be produced from the copied log passed as -LogPath'
    Add-Check -Name 'audit_has_single_length' -Passed ($auditItemLengths.Count -eq 1) -Detail "audit JSON must retain exactly one Length; found $($auditItemLengths.Count)"
    Add-Check -Name 'audit_length_matches_log_path' -Passed ($auditItemLengths.Count -eq 1 -and $auditItemLengths[0] -eq $expectedAuditLength) -Detail 'godot-log-audit.json Length must match the copied log bytes'
    Add-Check -Name 'audit_has_single_sha256' -Passed ($auditItemSha256s.Count -eq 1) -Detail "audit JSON must retain exactly one Sha256; found $($auditItemSha256s.Count)"
    Add-Check -Name 'audit_sha256_matches_log_path' -Passed ($auditItemSha256s.Count -eq 1 -and [System.StringComparer]::OrdinalIgnoreCase.Equals([string]$auditItemSha256s[0], $expectedAuditSha256)) -Detail 'godot-log-audit.json Sha256 must match the copied log bytes'

    if (-not (Test-Path -LiteralPath $logAuditScript -PathType Leaf)) {
        Add-Check -Name 'audit_recompute_script_exists' -Passed $false -Detail "missing audit script: $logAuditScript"
    } else {
        $recomputedAuditSummary = Invoke-RecomputedAuditSummary -LogPath $resolvedLogPath
        $recomputedPaths = @($recomputedAuditSummary.ItemPaths)
        $recomputedSha256s = @($recomputedAuditSummary.ItemSha256s)
        $recomputedSchemaVersions = @($recomputedAuditSummary.AuditSchemaVersions)
        $recomputedSignatureSetSha256s = @($recomputedAuditSummary.SignatureSetSha256s)
        $recomputedSignatureHitVector = @($recomputedAuditSummary.SignatureHitVector)
        Add-Check -Name 'audit_recomputed_from_log_path' -Passed ($recomputedPaths.Count -eq 1 -and [System.StringComparer]::OrdinalIgnoreCase.Equals([string]$recomputedPaths[0], $expectedAuditPath)) -Detail 'verifier must recompute the audit from the copied log passed as -LogPath'
        Add-Check -Name 'audit_recomputed_clean' -Passed ([bool]$recomputedAuditSummary.Clean) -Detail "recomputed audit must have zero dirty items and zero signature hits; dirty=$($recomputedAuditSummary.DirtyItems), hits=$($recomputedAuditSummary.SignatureHitCount)"
        Add-Check -Name 'audit_schema_version_matches_recomputed' -Passed (Test-StringArrayEquals -Actual $auditSchemaVersions -Expected $recomputedSchemaVersions) -Detail "retained audit schema versions must match recomputed versions; retained=$($auditSchemaVersions -join ',') recomputed=$($recomputedSchemaVersions -join ',')"
        Add-Check -Name 'audit_signature_set_matches_recomputed' -Passed (Test-StringArrayEquals -Actual $auditSignatureSetSha256s -Expected $recomputedSignatureSetSha256s) -Detail 'retained audit SignatureSetSha256 must match the recomputed audit rule set hash'
        Add-Check -Name 'audit_signature_counts_match_recomputed' -Passed ($auditSummary.DirtyItems -eq $recomputedAuditSummary.DirtyItems -and $auditSummary.SignatureHitCount -eq $recomputedAuditSummary.SignatureHitCount -and (Test-StringArrayEquals -Actual $auditSignatureHitVector -Expected $recomputedSignatureHitVector)) -Detail "retained audit signature vector must match recomputed Name/Count pairs; retained dirty=$($auditSummary.DirtyItems), retained hits=$($auditSummary.SignatureHitCount), recomputed dirty=$($recomputedAuditSummary.DirtyItems), recomputed hits=$($recomputedAuditSummary.SignatureHitCount)"
        Add-Check -Name 'audit_sha256_matches_recomputed' -Passed ($auditItemSha256s.Count -eq 1 -and $recomputedSha256s.Count -eq 1 -and [System.StringComparer]::OrdinalIgnoreCase.Equals([string]$auditItemSha256s[0], [string]$recomputedSha256s[0])) -Detail 'retained audit Sha256 must match the recomputed audit Sha256'
    }
} else {
    Write-Output 'audit_status=not-provided'
}

$report.RuntimeLogStatus = 'validated'
$report['LogPath'] = $resolvedLogPath
$report['LogLength'] = [long](Get-Item -LiteralPath $resolvedLogPath).Length
$report['LogSha256'] = Get-FileSha256OrEmpty -Path $resolvedLogPath
$report['ObservedRegisteredEventLines'] = $registeredEventMatches.Count
$report['ObservedEventTypes'] = $observedClasses.Count
$report['ObservedEventClasses'] = $observedClasses
$report['ObservedRegistrationTuples'] = $observedTuples
$report['MissingRegistrationTuples'] = $missingTuples
$report['UnexpectedRegistrationTuples'] = $unexpectedTuples
$report['ModeReasonHits'] = $reasonHits
$report['EnabledFeatureLineHits'] = $enabledFeatureLineHits
$report['DisabledFeatureLineHits'] = $disabledFeatureLineHits
$report['RegistrationStartHits'] = $startHits
$report['RegistrationSuccessHits'] = $successHits
$report['ExpectedPackageVersion'] = $ExpectedPackageVersion
$report['ExpectedRitsuCompatBranch'] = $ExpectedRitsuCompatBranch
$report['ExpectedRitsuLibVersion'] = $ExpectedRitsuLibVersion
$report['ExpectedGameVersion'] = $ExpectedGameVersion
$report['Audit'] = $auditSummary

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
    $protectedRuntimeEvidence = [System.Collections.Generic.List[string]]::new()
    $protectedVerifierInputs = [System.Collections.Generic.List[string]]::new()
    $protectedCanonicalRoots = [System.Collections.Generic.List[string]]::new()
    Add-ProtectedOutputPath -ProtectedPaths $protectedRuntimeEvidence -Path $resolvedLogPath
    Add-ProtectedOutputRootsForEvidenceFile -ProtectedRoots $protectedCanonicalRoots -Path $resolvedLogPath
    if (-not [string]::IsNullOrWhiteSpace($resolvedAuditPath)) {
        Add-ProtectedOutputPath -ProtectedPaths $protectedRuntimeEvidence -Path $resolvedAuditPath
        Add-ProtectedOutputRootsForEvidenceFile -ProtectedRoots $protectedCanonicalRoots -Path $resolvedAuditPath
    }

    foreach ($path in @(Get-RegistrationServiceInputPaths -Path $RegistrationServicePath)) {
        Add-ProtectedOutputPath -ProtectedPaths $protectedVerifierInputs -Path ([string]$path)
    }

    Assert-OutFileDoesNotOverwriteExplicitEvidence -ResolvedOutFile $resolvedOutFile -ProtectedPaths $protectedRuntimeEvidence -Message 'Refusing to write OutFile over input StS1 runtime evidence:'
    Assert-OutFileDoesNotOverwriteExplicitEvidence -ResolvedOutFile $resolvedOutFile -ProtectedPaths $protectedVerifierInputs -Message 'Refusing to write OutFile over verifier input file:'
    Assert-OutFileDoesNotOverwriteCanonicalEvidence -ResolvedOutFile $resolvedOutFile -ProtectedRoots $protectedCanonicalRoots
    $outDir = [System.IO.Path]::GetDirectoryName($resolvedOutFile)
    if ($outDir -and -not (Test-Path -LiteralPath $outDir)) {
        [void][System.IO.Directory]::CreateDirectory($outDir)
    }

    [pscustomobject]$report | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $resolvedOutFile -Encoding UTF8
}

if ($FailOnMismatch -and $mismatches.Count -gt 0) {
    exit 1
}
