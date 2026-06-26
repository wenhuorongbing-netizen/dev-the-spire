param(
    [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
    [string[]] $Path,

    [string] $OutFile,

    [switch] $FailOnHit
)

$ErrorActionPreference = 'Stop'

$signatures = @(
    @{ Name = 'Creature.get_ShowsInfiniteHp'; Pattern = 'Creature\.get_ShowsInfiniteHp' },
    @{ Name = 'DependencyFramework.Patches.UI.HealthBarForecastPatch'; Pattern = 'DependencyFramework\.Patches\.UI\.HealthBarForecastPatch' },
    @{ Name = 'dependency framework patch failure'; Pattern = '(?i)(?:previous package|dependency framework).*(?:[1-9][0-9]*\s+failed|patch(?:es)?\s+failed|patch\s+failure|failed\s+to\s+patch|exception)' },
    @{ Name = 'DamageMeter'; Pattern = '(?i)\[ERROR\].*DamageMeter' },
    @{ Name = 'RouteSuggest'; Pattern = '(?i)\[ERROR\].*RouteSuggest' },
    @{ Name = 'Spire Plus error/exception'; Pattern = '(?i)(EZMicroBalance|Spire Plus).*(error|exception)' },
    @{ Name = 'TypeLoadException'; Pattern = 'TypeLoadException' },
    @{ Name = 'MissingMethodException'; Pattern = 'MissingMethodException' },
    @{ Name = 'Godot ERROR line'; Pattern = '(?m)^\s*(?:\[ERROR\]|ERROR\b|\[[^\]]+\]\s*ERROR\b)' }
)

$ignoredErrorPatterns = @(
    'Mod manifest .*RouteSuggestConfig\.json.*id.*field',
    'Mod manifest .*sts2-heybox-support[\\/ ]+mod_mainfest\.json.*id.*field',
    'Mod manifest .*STS2-RitsuLib[\\/ ]+ritsulib-variants\.json.*id.*field'
)

$auditSchemaVersion = 2

function Normalize-FullPath
{
    param([Parameter(Mandatory = $true)][string] $Path)

    [System.IO.Path]::GetFullPath($Path).TrimEnd([char[]]@('\', '/'))
}

function Initialize-WindowsFileIdentityType
{
    if (-not [System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform([System.Runtime.InteropServices.OSPlatform]::Windows))
    {
        return $false
    }

    if ('SpirePlusGodotLogAuditFileIdentity' -as [type])
    {
        return $true
    }

    try
    {
        Add-Type -TypeDefinition @'
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

public static class SpirePlusGodotLogAuditFileIdentity
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
    }
    catch
    {
        return $false
    }
}

function Assert-WindowsFileIdentityAvailable
{
    if ([System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform([System.Runtime.InteropServices.OSPlatform]::Windows) -and
        -not (Initialize-WindowsFileIdentityType))
    {
        throw 'Windows file identity checks are required before writing audit OutFile evidence.'
    }
}

function Get-ExistingPathPhysicalIdentity
{
    param([AllowEmptyString()][string] $Path)

    if ([string]::IsNullOrWhiteSpace($Path) -or -not (Test-Path -LiteralPath $Path -PathType Leaf))
    {
        return ''
    }

    if ([System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform([System.Runtime.InteropServices.OSPlatform]::Windows) -and
        -not (Initialize-WindowsFileIdentityType))
    {
        throw 'Windows file identity checks are required before comparing audit evidence paths.'
    }

    try
    {
        return [SpirePlusGodotLogAuditFileIdentity]::GetIdentity([System.IO.Path]::GetFullPath($Path))
    }
    catch
    {
        if ([System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform([System.Runtime.InteropServices.OSPlatform]::Windows))
        {
            throw
        }

        return ''
    }
}

function Get-ExistingPathHardlinkCount
{
    param([AllowEmptyString()][string] $Path)

    if ([string]::IsNullOrWhiteSpace($Path) -or -not (Test-Path -LiteralPath $Path -PathType Leaf))
    {
        return 0
    }

    if ([System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform([System.Runtime.InteropServices.OSPlatform]::Windows) -and
        -not (Initialize-WindowsFileIdentityType))
    {
        throw 'Windows file identity checks are required before writing audit OutFile evidence.'
    }

    try
    {
        return [int][SpirePlusGodotLogAuditFileIdentity]::GetLinkCount([System.IO.Path]::GetFullPath($Path))
    }
    catch
    {
        if ([System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform([System.Runtime.InteropServices.OSPlatform]::Windows))
        {
            throw
        }

        return 0
    }
}

function Test-SameExistingPathPhysicalIdentity
{
    param(
        [AllowEmptyString()][string] $Left,
        [AllowEmptyString()][string] $Right
    )

    $leftIdentity = Get-ExistingPathPhysicalIdentity -Path $Left
    if ([string]::IsNullOrWhiteSpace($leftIdentity))
    {
        return $false
    }

    $rightIdentity = Get-ExistingPathPhysicalIdentity -Path $Right
    return -not [string]::IsNullOrWhiteSpace($rightIdentity) -and
        [string]::Equals($leftIdentity, $rightIdentity, [System.StringComparison]::OrdinalIgnoreCase)
}

function Assert-NoReparsePointInExistingPath
{
    param(
        [Parameter(Mandatory = $true)][string] $Path,
        [string] $StopDirectory = ''
    )

    $current = Normalize-FullPath -Path $Path
    $normalizedStop = if ([string]::IsNullOrWhiteSpace($StopDirectory)) { '' } else { Normalize-FullPath -Path $StopDirectory }
    while (-not [string]::IsNullOrWhiteSpace($current))
    {
        if (Test-Path -LiteralPath $current)
        {
            $item = Get-Item -LiteralPath $current -Force
            if (($item.Attributes -band [System.IO.FileAttributes]::ReparsePoint) -ne 0)
            {
                throw "Path must not pass through a reparse point: $current"
            }
        }

        if (-not [string]::IsNullOrWhiteSpace($normalizedStop) -and
            [System.StringComparer]::OrdinalIgnoreCase.Equals((Normalize-FullPath -Path $current), $normalizedStop))
        {
            break
        }

        $parent = Split-Path -Parent $current
        if ([string]::IsNullOrWhiteSpace($parent) -or [System.StringComparer]::OrdinalIgnoreCase.Equals($parent, $current))
        {
            break
        }

        $current = $parent
    }
}

function Assert-LeafIsNotReparsePoint
{
    param(
        [Parameter(Mandatory = $true)][string] $Path,
        [string] $Description = 'File'
    )

    $item = Get-Item -LiteralPath $Path -Force
    if (($item.Attributes -band [System.IO.FileAttributes]::ReparsePoint) -ne 0)
    {
        throw "$Description must not be a reparse point: $Path"
    }
}

function Resolve-OutputPath
{
    param([Parameter(Mandatory = $true)][string] $Path)

    if ([System.IO.Path]::IsPathRooted($Path))
    {
        return [System.IO.Path]::GetFullPath($Path)
    }

    return [System.IO.Path]::GetFullPath((Join-Path (Get-Location).Path $Path))
}

function Assert-OutFileIsSafe
{
    param(
        [Parameter(Mandatory = $true)][string] $OutputPath,
        [Parameter(Mandatory = $true)][string[]] $InputPaths
    )

    $resolvedOutput = Resolve-OutputPath -Path $OutputPath
    Assert-WindowsFileIdentityAvailable
    $leaf = [System.IO.Path]::GetFileName($resolvedOutput)
    $forbiddenLeaves = @(
        'godot.log',
        'godot.log.before',
        'godot.log.after-launch',
        'godot.log.current-iteration',
        'run-manifest.json',
        'command.txt',
        'command-corpus.txt',
        'monkey-plan.json',
        'monkey-summary.json',
        'autoslay-plan.json',
        'autoslay-summary.json',
        'iteration-result.json',
        'run-result.json',
        'preflight.json',
        'prepare-output.json',
        'session-state.json',
        'restore-state.json',
        'runtime-probe-samples.json',
        'main-menu-screenshot.png',
        'window-after-command.png'
    )
    if ($forbiddenLeaves -contains $leaf)
    {
        throw "OutFile must be an audit/report artifact, not canonical runtime evidence: $resolvedOutput"
    }

    if ($leaf -notmatch '(?i)(^|[-_])audit(?:[-_].*)?\.json$')
    {
        throw "OutFile leaf must be audit-shaped JSON: $resolvedOutput"
    }

    $parent = Split-Path -Parent $resolvedOutput
    if ([string]::IsNullOrWhiteSpace($parent))
    {
        $parent = (Get-Location).Path
    }

    Assert-NoReparsePointInExistingPath -Path $parent
    if (-not (Test-Path -LiteralPath $parent -PathType Container))
    {
        New-Item -ItemType Directory -Path $parent -Force | Out-Null
        Assert-NoReparsePointInExistingPath -Path $parent
    }

    if (Test-Path -LiteralPath $resolvedOutput -PathType Leaf)
    {
        Assert-LeafIsNotReparsePoint -Path $resolvedOutput -Description 'OutFile'
        $linkCount = Get-ExistingPathHardlinkCount -Path $resolvedOutput
        if ($linkCount -gt 1)
        {
            throw "OutFile must not be a hardlink with multiple filesystem names: $resolvedOutput linkCount=$linkCount"
        }
    }

    foreach ($inputPath in @($InputPaths))
    {
        if ([string]::IsNullOrWhiteSpace($inputPath))
        {
            continue
        }

        $resolvedInput = [System.IO.Path]::GetFullPath($inputPath)
        if ([System.StringComparer]::OrdinalIgnoreCase.Equals($resolvedOutput, $resolvedInput) -or
            (Test-SameExistingPathPhysicalIdentity -Left $resolvedOutput -Right $resolvedInput))
        {
            throw "OutFile must not overwrite or alias an audited log file: $resolvedOutput"
        }
    }

    return $resolvedOutput
}

function Get-Sha256Hex
{
    param([Parameter(Mandatory = $true)][string] $Text)

    $sha256 = [System.Security.Cryptography.SHA256]::Create()
    try
    {
        $bytes = [System.Text.Encoding]::UTF8.GetBytes($Text)
        return [System.BitConverter]::ToString($sha256.ComputeHash($bytes)).Replace('-', '').ToLowerInvariant()
    }
    finally
    {
        $sha256.Dispose()
    }
}

function Get-SignatureSetSha256
{
    $lines = [System.Collections.Generic.List[string]]::new()
    foreach ($signature in $signatures)
    {
        $lines.Add("signature`t$($signature.Name)`t$($signature.Pattern)") | Out-Null
    }

    foreach ($pattern in $ignoredErrorPatterns)
    {
        $lines.Add("ignored`t$pattern") | Out-Null
    }

    $sortedLines = @($lines.ToArray() | Sort-Object)
    return Get-Sha256Hex -Text ($sortedLines -join "`n")
}

function ShouldIgnoreLine
{
    param([string] $line)

    foreach ($pattern in $ignoredErrorPatterns)
    {
        if ([regex]::IsMatch($line, $pattern, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase))
        {
            return $true
        }
    }

    return $false
}

$signatureSetSha256 = Get-SignatureSetSha256

$resolvedInputPaths = foreach ($inputPath in $Path) {
    $resolved = Resolve-Path -LiteralPath $inputPath -ErrorAction Stop
    foreach ($resolvedPath in @($resolved)) {
        $file = Get-Item -LiteralPath $resolvedPath.Path -Force
        if (-not ($file -is [System.IO.FileInfo])) {
            throw "Path must be a file: $($resolvedPath.Path)"
        }

        Assert-NoReparsePointInExistingPath -Path $file.FullName
        Assert-LeafIsNotReparsePoint -Path $file.FullName -Description 'Audited log'
        $file.FullName
    }
}

$resolvedOutFile = if ($OutFile) { Assert-OutFileIsSafe -OutputPath $OutFile -InputPaths @($resolvedInputPaths) } else { '' }

$results = foreach ($inputPath in $resolvedInputPaths) {
    $file = Get-Item -LiteralPath $inputPath -Force
    $content = Get-Content -LiteralPath $file.FullName -Raw -Encoding UTF8
    $scannableLines = $content -split "`r?`n" |
        Where-Object { -not [string]::IsNullOrWhiteSpace($_) -and -not (ShouldIgnoreLine $_) }
    $scannableContent = ($scannableLines -join "`n")

    $hits = foreach ($signature in $signatures) {
        $matches = [regex]::Matches($scannableContent, $signature.Pattern)
        [pscustomobject]@{
            Name = $signature.Name
            Count = $matches.Count
        }
    }

    [pscustomobject]@{
        AuditSchemaVersion = $auditSchemaVersion
        SignatureSetSha256 = $signatureSetSha256
        Path = $file.FullName
        LastWriteTime = $file.LastWriteTime
        Length = $file.Length
        Sha256 = (Get-FileHash -Algorithm SHA256 -LiteralPath $file.FullName).Hash.ToLowerInvariant()
        Clean = -not ($hits | Where-Object { $_.Count -gt 0 })
        SignatureHits = $hits
    }
}

$json = ConvertTo-Json -InputObject @($results) -Depth 5

if ($OutFile) {
    $json | Set-Content -LiteralPath $resolvedOutFile -Encoding UTF8
    Assert-LeafIsNotReparsePoint -Path $resolvedOutFile -Description 'OutFile'
}

$json

if ($FailOnHit -and ($results | Where-Object { -not $_.Clean })) {
    exit 1
}
