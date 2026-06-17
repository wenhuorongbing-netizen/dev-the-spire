param(
    [string]$SourceRoot = 'EZMicroBalanceCode\Sts1Events\Models',
    [string]$EnglishPath = 'EZMicroBalance\localization\eng\sts1_events.json',
    [string]$ChinesePath = 'EZMicroBalance\localization\zhs\sts1_events.json',
    [string]$OutFile,
    [switch]$FailOnMissing
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path

function Resolve-RepoPath {
    param([Parameter(Mandatory = $true)][string]$Path)

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }

    return [System.IO.Path]::GetFullPath((Join-Path $repoRoot $Path))
}

function Get-RepoRelativePath {
    param([Parameter(Mandatory = $true)][string]$Path)

    $root = [System.IO.Path]::GetFullPath($repoRoot).TrimEnd('\') + '\'
    $fullPath = [System.IO.Path]::GetFullPath($Path)
    if ($fullPath.StartsWith($root, [System.StringComparison]::OrdinalIgnoreCase)) {
        return $fullPath.Substring($root.Length).Replace('\', '/')
    }

    return $fullPath.Replace('\', '/')
}

function Get-LocalizationPrefixFromClassName {
    param([Parameter(Mandatory = $true)][string]$ClassName)

    $name = $ClassName -replace '^Sts1', ''
    $parts = [System.Collections.Generic.List[string]]::new()
    $current = ''

    foreach ($character in $name.ToCharArray()) {
        $text = [string]$character
        if ($current.Length -gt 0 -and $text -cmatch '[A-Z]') {
            [void]$parts.Add($current.ToUpperInvariant())
            $current = $text
        } else {
            $current += $text
        }
    }

    if ($current.Length -gt 0) {
        [void]$parts.Add($current.ToUpperInvariant())
    }

    return 'STS1_' + ($parts -join '_')
}

function Add-ExpectedKey {
    param(
        [System.Collections.Generic.HashSet[string]]$Set,
        [Parameter(Mandatory = $true)][string]$Key
    )

    [void]$Set.Add($Key)
}

$resolvedSourceRoot = Resolve-RepoPath $SourceRoot
$resolvedEnglishPath = Resolve-RepoPath $EnglishPath
$resolvedChinesePath = Resolve-RepoPath $ChinesePath

if (-not (Test-Path -LiteralPath $resolvedSourceRoot)) {
    Write-Error "StS1 event source root not found: $resolvedSourceRoot"
    exit 1
}

if (-not (Test-Path -LiteralPath $resolvedEnglishPath)) {
    Write-Error "English StS1 localization file not found: $resolvedEnglishPath"
    exit 1
}

if (-not (Test-Path -LiteralPath $resolvedChinesePath)) {
    Write-Error "Simplified Chinese StS1 localization file not found: $resolvedChinesePath"
    exit 1
}

$english = Get-Content -Raw -Encoding UTF8 -LiteralPath $resolvedEnglishPath | ConvertFrom-Json
$chinese = Get-Content -Raw -Encoding UTF8 -LiteralPath $resolvedChinesePath | ConvertFrom-Json

$englishKeys = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
$chineseKeys = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)

foreach ($property in $english.PSObject.Properties) {
    [void]$englishKeys.Add($property.Name)
}

foreach ($property in $chinese.PSObject.Properties) {
    [void]$chineseKeys.Add($property.Name)
}

$sourceFiles = @(
    Get-ChildItem -LiteralPath $resolvedSourceRoot -Recurse -File -Filter '*.cs' |
        Sort-Object -Property FullName
)

$expectedByKey = [System.Collections.Generic.Dictionary[string, System.Collections.Generic.HashSet[string]]]::new([System.StringComparer]::Ordinal)

foreach ($file in $sourceFiles) {
    $content = Get-Content -Raw -Encoding UTF8 -LiteralPath $file.FullName
    $classMatch = [regex]::Match($content, 'class\s+(Sts1[A-Za-z0-9]+)\b')
    if (-not $classMatch.Success) {
        continue
    }

    $className = $classMatch.Groups[1].Value
    $prefix = Get-LocalizationPrefixFromClassName $className
    $fileKeys = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)

    Add-ExpectedKey $fileKeys "$prefix.title"
    Add-ExpectedKey $fileKeys "$prefix.pages.INITIAL.description"

    foreach ($match in [regex]::Matches($content, 'InitialOptionKey\("([^"]+)"\)')) {
        $option = $match.Groups[1].Value
        Add-ExpectedKey $fileKeys "$prefix.pages.INITIAL.options.$option.title"
        Add-ExpectedKey $fileKeys "$prefix.pages.INITIAL.options.$option.description"
    }

    foreach ($match in [regex]::Matches($content, 'OptionKey\("([^"]+)",\s*"([^"]+)"\)')) {
        $page = $match.Groups[1].Value
        $option = $match.Groups[2].Value
        Add-ExpectedKey $fileKeys "$prefix.pages.$page.options.$option.title"
        Add-ExpectedKey $fileKeys "$prefix.pages.$page.options.$option.description"
    }

    foreach ($match in [regex]::Matches($content, 'L10NLookup\("([^"]+)"\)')) {
        Add-ExpectedKey $fileKeys $match.Groups[1].Value
    }

    foreach ($key in $fileKeys) {
        if (-not $expectedByKey.ContainsKey($key)) {
            $expectedByKey[$key] = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
        }

        [void]$expectedByKey[$key].Add((Get-RepoRelativePath $file.FullName))
    }
}

$results = [System.Collections.Generic.List[object]]::new()

foreach ($key in ($expectedByKey.Keys | Sort-Object)) {
    $missingEnglish = -not $englishKeys.Contains($key)
    $missingChinese = -not $chineseKeys.Contains($key)
    if ($missingEnglish -or $missingChinese) {
        [void]$results.Add([pscustomobject]@{
            Key = $key
            MissingEnglish = $missingEnglish
            MissingChinese = $missingChinese
            SourceFiles = @($expectedByKey[$key] | Sort-Object)
        })
    }
}

$englishOnly = @($englishKeys | Where-Object { -not $chineseKeys.Contains($_) } | Sort-Object)
$chineseOnly = @($chineseKeys | Where-Object { -not $englishKeys.Contains($_) } | Sort-Object)

$report = [pscustomobject]@{
    SourceRoot = Get-RepoRelativePath $resolvedSourceRoot
    EnglishPath = Get-RepoRelativePath $resolvedEnglishPath
    ChinesePath = Get-RepoRelativePath $resolvedChinesePath
    SourceFileCount = $sourceFiles.Count
    ExpectedKeyCount = $expectedByKey.Keys.Count
    EnglishKeyCount = $englishKeys.Count
    ChineseKeyCount = $chineseKeys.Count
    EnglishOnlyKeys = $englishOnly
    ChineseOnlyKeys = $chineseOnly
    MissingSourceReferencedKeys = @($results)
}

$json = $report | ConvertTo-Json -Depth 8
if ($OutFile) {
    $resolvedOutFile = Resolve-RepoPath $OutFile
    $outDir = Split-Path -Parent $resolvedOutFile
    if ($outDir -and -not (Test-Path -LiteralPath $outDir)) {
        New-Item -ItemType Directory -Path $outDir | Out-Null
    }

    Set-Content -LiteralPath $resolvedOutFile -Encoding UTF8 -Value $json
}

Write-Output "source_files=$($report.SourceFileCount)"
Write-Output "expected_source_keys=$($report.ExpectedKeyCount)"
Write-Output "english_keys=$($report.EnglishKeyCount)"
Write-Output "chinese_keys=$($report.ChineseKeyCount)"
Write-Output "english_only_keys=$($report.EnglishOnlyKeys.Count)"
Write-Output "chinese_only_keys=$($report.ChineseOnlyKeys.Count)"
Write-Output "missing_source_referenced_keys=$($report.MissingSourceReferencedKeys.Count)"

foreach ($result in $report.MissingSourceReferencedKeys) {
    $languages = @()
    if ($result.MissingEnglish) { $languages += 'eng' }
    if ($result.MissingChinese) { $languages += 'zhs' }
    Write-Output ("missing {0} [{1}] ({2})" -f $result.Key, ($languages -join ','), ($result.SourceFiles -join ', '))
}

if ($FailOnMissing -and (
        $report.MissingSourceReferencedKeys.Count -gt 0 -or
        $report.EnglishOnlyKeys.Count -gt 0 -or
        $report.ChineseOnlyKeys.Count -gt 0)) {
    exit 1
}
