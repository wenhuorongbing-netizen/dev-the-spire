param(
    [string]$ModDirectory,
    [string]$GameRootZipPath,
    [string]$HandoffPath = "$PSScriptRoot\..\docs\private-beta-verification-handoff.md",
    [switch]$SkipGameRootZipCheck,
    [switch]$PassVerbose
)

$ErrorActionPreference = 'Stop'

if (-not $ModDirectory) {
    $defaultRoots = @(
        (Join-Path (Get-Location) 'mods\EZMicroBalance'),
        "E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance",
        "D:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance"
    )

    foreach ($root in $defaultRoots) {
        if (Test-Path $root) {
            $ModDirectory = $root
            break
        }
    }
}

if (-not $ModDirectory -or -not (Test-Path $ModDirectory)) {
    Write-Error "Could not locate the Spire Plus compatibility install directory (EZMicroBalance). Pass -ModDirectory explicitly."
    exit 1
}

if (-not (Test-Path $HandoffPath)) {
    Write-Error "Handoff file not found: $HandoffPath"
    exit 1
}

$rawLines = Get-Content $HandoffPath

function Get-ExpectedHash {
    param([string]$Label)
    $prefix = "- $Label SHA256:"

    foreach ($line in $rawLines) {
        $trimmed = $line.Trim()
        if ($trimmed.StartsWith($prefix)) {
            $value = $trimmed.Substring($prefix.Length).Trim()
            $value = $value.Trim('`')
            $value = $value.Trim()
            if ($value -match '^[A-Fa-f0-9]{64}$') {
                return $value.ToUpperInvariant()
            }
        }
    }
    return $null
}

function Get-HandoffPackageFileName {
    foreach ($line in $rawLines) {
        if ($line -match 'SpirePlus-[^`\\/\s]+\.zip') {
            return $Matches[0]
        }
    }

    return $null
}

$expectedZipHash = Get-ExpectedHash 'Zip'
$packageFileName = Get-HandoffPackageFileName

if (-not $GameRootZipPath -and -not $SkipGameRootZipCheck -and $packageFileName) {
    $resolvedModDirectory = Resolve-Path -LiteralPath $ModDirectory
    $modDirectoryInfo = [System.IO.DirectoryInfo]$resolvedModDirectory.Path
    if ($modDirectoryInfo.Name -eq 'EZMicroBalance' -and $modDirectoryInfo.Parent -and $modDirectoryInfo.Parent.Name -eq 'mods') {
        $GameRootZipPath = Join-Path $modDirectoryInfo.Parent.Parent.FullName $packageFileName
    }
}

$expected = @{
    'EZMicroBalance.dll' = Get-ExpectedHash 'DLL'
    'EZMicroBalance.pck' = Get-ExpectedHash 'PCK'
    'README_INSTALL.txt' = Get-ExpectedHash 'README_INSTALL'
}
$expected['EZMicroBalance.json'] = Get-ExpectedHash 'Manifest'
if (-not $expected['EZMicroBalance.json']) {
    $expected['EZMicroBalance.json'] = Get-ExpectedHash 'JSON'
}

$files = [ordered]@{
    'EZMicroBalance.dll' = 'DLL'
    'EZMicroBalance.json' = 'JSON'
    'EZMicroBalance.pck' = 'PCK'
    'README_INSTALL.txt' = 'README_INSTALL'
}

$allPass = $true
Write-Host "Checking installed Spire Plus compatibility artifacts at: $ModDirectory"
Write-Host "Using expected hashes from: $HandoffPath"

$rows = @()
foreach ($kv in $files.GetEnumerator()) {
    $fileName = $kv.Key
    $expectedHash = $expected[$fileName]
    $filePath = Join-Path $ModDirectory $fileName

    if (-not (Test-Path $filePath)) {
        $expectedDisplay = if ($expectedHash) { $expectedHash } else { '<unknown>' }
        $rows += "{0} | MISSING | expected:{1} | FAIL" -f $fileName, $expectedDisplay
        $allPass = $false
        continue
    }

    $actualHash = (Get-FileHash -Algorithm SHA256 -Path $filePath).Hash.ToUpperInvariant()
    if (-not $expectedHash) {
        $rows += "{0} | expected:<missing> | actual:{1} | FAIL" -f $fileName, $actualHash
        $allPass = $false
    } elseif ($actualHash -eq $expectedHash) {
        $rows += "{0} | expected:{1} | actual:{2} | PASS" -f $fileName, $expectedHash, $actualHash
    } else {
        $rows += "{0} | expected:{1} | actual:{2} | FAIL" -f $fileName, $expectedHash, $actualHash
        $allPass = $false
    }
}

if (-not $SkipGameRootZipCheck) {
    if (-not $expectedZipHash) {
        $rows += "Game root package zip | expected:<missing> | actual:<not checked> | FAIL"
        $allPass = $false
    } elseif (-not $GameRootZipPath) {
        $rows += "Game root package zip | expected:$expectedZipHash | actual:<path not inferred> | FAIL"
        $allPass = $false
    } elseif (-not (Test-Path -LiteralPath $GameRootZipPath)) {
        $rows += "Game root package zip | MISSING | expected:$expectedZipHash | path:$GameRootZipPath | FAIL"
        $allPass = $false
    } else {
        $actualZipHash = (Get-FileHash -Algorithm SHA256 -Path $GameRootZipPath).Hash.ToUpperInvariant()
        if ($actualZipHash -eq $expectedZipHash) {
            $rows += "Game root package zip | expected:$expectedZipHash | actual:$actualZipHash | PASS"
        } else {
            $rows += "Game root package zip | expected:$expectedZipHash | actual:$actualZipHash | path:$GameRootZipPath | FAIL"
            $allPass = $false
        }
    }
}

$rows | ForEach-Object { Write-Host $_ }

$pckPath = Join-Path $ModDirectory 'EZMicroBalance.pck'
if (Test-Path -LiteralPath $pckPath) {
    $pckText = [System.Text.Encoding]::UTF8.GetString([System.IO.File]::ReadAllBytes($pckPath))

    function ConvertFrom-CodePoints {
        param([int[]]$CodePoints)
        return -join ($CodePoints | ForEach-Object { [string][char]$_ })
    }

    $sereTalonZhsTitle = ConvertFrom-CodePoints @(0x0022, 0x0053, 0x0045, 0x0052, 0x0045, 0x005F, 0x0054, 0x0041, 0x004C, 0x004F, 0x004E, 0x002E, 0x0074, 0x0069, 0x0074, 0x006C, 0x0065, 0x0022, 0x003A, 0x0020, 0x0022, 0x74E6, 0x5E93, 0x539F, 0x521D, 0x4E4B, 0x722A, 0x0022)
    $sereTalonZhsEffect = ConvertFrom-CodePoints @(0x0022, 0x0053, 0x0045, 0x0052, 0x0045, 0x005F, 0x0054, 0x0041, 0x004C, 0x004F, 0x004E, 0x002E, 0x0064, 0x0065, 0x0073, 0x0063, 0x0072, 0x0069, 0x0070, 0x0074, 0x0069, 0x006F, 0x006E, 0x0022, 0x003A, 0x0020, 0x0022, 0x62FE, 0x53D6, 0x65F6, 0xFF0C, 0x4ECE, 0x005B, 0x0062, 0x006C, 0x0075, 0x0065, 0x005D, 0x0034, 0x005B, 0x002F, 0x0062, 0x006C, 0x0075, 0x0065, 0x005D, 0x5F20, 0x8BC5, 0x5492, 0x4E2D, 0x9009, 0x62E9, 0x005B, 0x0062, 0x006C, 0x0075, 0x0065, 0x005D, 0x0031, 0x005B, 0x002F, 0x0062, 0x006C, 0x0075, 0x0065, 0x005D, 0x5F20, 0x3002, 0x5C06, 0x5B83, 0x3001, 0x005B, 0x0062, 0x006C, 0x0075, 0x0065, 0x005D, 0x0032, 0x005B, 0x002F, 0x0062, 0x006C, 0x0075, 0x0065, 0x005D, 0x5F20, 0x005B, 0x0067, 0x006F, 0x006C, 0x0064, 0x005D, 0x8BB8, 0x613F, 0x005B, 0x002F, 0x0067, 0x006F, 0x006C, 0x0064, 0x005D, 0x548C, 0x005B, 0x0062, 0x006C, 0x0075, 0x0065, 0x005D, 0x0031, 0x005B, 0x002F, 0x0062, 0x006C, 0x0075, 0x0065, 0x005D, 0x5F20, 0x005B, 0x0067, 0x006F, 0x006C, 0x0064, 0x005D, 0x8BB8, 0x613F, 0x002B, 0x005B, 0x002F, 0x0067, 0x006F, 0x006C, 0x0064, 0x005D, 0x52A0, 0x5165, 0x4F60, 0x7684, 0x724C, 0x7EC4, 0x3002, 0x0022)
    $tanxClawsZhsTitle = ConvertFrom-CodePoints @(0x0022, 0x0043, 0x004C, 0x0041, 0x0057, 0x0053, 0x002E, 0x0074, 0x0069, 0x0074, 0x006C, 0x0065, 0x0022, 0x003A, 0x0020, 0x0022, 0x5766, 0x514B, 0x65AF, 0x5229, 0x722A, 0x0022)
    $tanxClawsZhsEffect = ConvertFrom-CodePoints @(0x0022, 0x0043, 0x004C, 0x0041, 0x0057, 0x0053, 0x002E, 0x0064, 0x0065, 0x0073, 0x0063, 0x0072, 0x0069, 0x0070, 0x0074, 0x0069, 0x006F, 0x006E, 0x0022, 0x003A, 0x0020, 0x0022, 0x62FE, 0x53D6, 0x65F6, 0xFF0C, 0x5C06, 0x81F3, 0x591A, 0x005B, 0x0062, 0x006C, 0x0075, 0x0065, 0x005D, 0x007B, 0x0043, 0x0061, 0x0072, 0x0064, 0x0073, 0x007D, 0x005B, 0x002F, 0x0062, 0x006C, 0x0075, 0x0065, 0x005D, 0x5F20, 0x724C, 0x53D8, 0x5316, 0x4E3A, 0x6495, 0x54AC, 0x002B, 0x3002, 0x0022)
    $staleSereTalonBaseClawsTitle = ConvertFrom-CodePoints @(0x0022, 0x0053, 0x0045, 0x0052, 0x0045, 0x005F, 0x0054, 0x0041, 0x004C, 0x004F, 0x004E, 0x002E, 0x0074, 0x0069, 0x0074, 0x006C, 0x0065, 0x0022, 0x003A, 0x0020, 0x0022, 0x5229, 0x722A, 0x0022)
    $staleTanxBaseClawsTitle = ConvertFrom-CodePoints @(0x0022, 0x0043, 0x004C, 0x0041, 0x0057, 0x0053, 0x002E, 0x0074, 0x0069, 0x0074, 0x006C, 0x0065, 0x0022, 0x003A, 0x0020, 0x0022, 0x5229, 0x722A, 0x0022)
    $staleSereTalonAsMaul = ConvertFrom-CodePoints @(0x0022, 0x0053, 0x0045, 0x0052, 0x0045, 0x005F, 0x0054, 0x0041, 0x004C, 0x004F, 0x004E, 0x002E, 0x0064, 0x0065, 0x0073, 0x0063, 0x0072, 0x0069, 0x0070, 0x0074, 0x0069, 0x006F, 0x006E, 0x0022, 0x003A, 0x0020, 0x0022, 0x62FE, 0x53D6, 0x65F6, 0xFF0C, 0x5C06, 0x81F3, 0x591A, 0x005B, 0x0062, 0x006C, 0x0075, 0x0065, 0x005D, 0x007B, 0x0043, 0x0061, 0x0072, 0x0064, 0x0073, 0x007D, 0x005B, 0x002F, 0x0062, 0x006C, 0x0075, 0x0065, 0x005D, 0x5F20, 0x724C, 0x53D8, 0x5316, 0x4E3A, 0x6495, 0x54AC, 0x3002)
    $staleTanxAsCurseWish = ConvertFrom-CodePoints @(0x0022, 0x0043, 0x004C, 0x0041, 0x0057, 0x0053, 0x002E, 0x0064, 0x0065, 0x0073, 0x0063, 0x0072, 0x0069, 0x0070, 0x0074, 0x0069, 0x006F, 0x006E, 0x0022, 0x003A, 0x0020, 0x0022, 0x62FE, 0x53D6, 0x65F6, 0xFF0C, 0x5C06, 0x005B, 0x0062, 0x006C, 0x0075, 0x0065, 0x005D, 0x0032, 0x005B, 0x002F, 0x0062, 0x006C, 0x0075, 0x0065, 0x005D, 0x5F20, 0x968F, 0x673A, 0x8BC5, 0x5492)
    $staleSereTitleJoinedToClaws = ConvertFrom-CodePoints @(0x74E6, 0x5E93, 0x539F, 0x521D, 0x4E4B, 0x722A, 0x0022, 0x002C, 0x0020, 0x0022, 0x0043, 0x004C, 0x0041, 0x0057, 0x0053, 0x002E, 0x0064, 0x0065, 0x0073, 0x0063, 0x0072, 0x0069, 0x0070, 0x0074, 0x0069, 0x006F, 0x006E, 0x0022)

    $requiredPckFragments = [ordered]@{
        'Sere Talon EN title' = '"SERE_TALON.title": "Vakuu''s Sere Talon"'
        'Sere Talon EN effect' = '"SERE_TALON.description": "On pickup, choose [blue]1[/blue] of [blue]4[/blue] Curses. Add it, [blue]2[/blue] Wish, and [blue]1[/blue] Wish+ to your deck."'
        'Sere Talon ZHS title' = $sereTalonZhsTitle
        'Sere Talon ZHS effect' = $sereTalonZhsEffect
        'Sere Talon owned icon' = 'sere_talon_spire_plus.png'
        'Sere Talon small import' = 'EZMicroBalance/images/relics/sere_talon_spire_plus.png.import'
        'Sere Talon big import' = 'EZMicroBalance/images/relics/big/sere_talon_spire_plus.png.import'
        'Tanx Claws EN title' = '"CLAWS.title": "Tanx Claws"'
        'Tanx Claws EN effect' = '"CLAWS.description": "On pickup, transform up to [blue]{Cards}[/blue] cards into upgraded Maul."'
        'Tanx Claws ZHS title' = $tanxClawsZhsTitle
        'Tanx Claws ZHS effect' = $tanxClawsZhsEffect
    }

    foreach ($kv in $requiredPckFragments.GetEnumerator()) {
        if ($pckText.IndexOf($kv.Value, [System.StringComparison]::Ordinal) -ge 0) {
            Write-Host "PCK content $($kv.Key) | PASS"
        } else {
            Write-Host "PCK content $($kv.Key) | MISSING | FAIL"
            $allPass = $false
        }
    }

    $forbiddenPckFragments = @(
        '"SERE_TALON.description": "claws.png"',
        $staleSereTalonBaseClawsTitle,
        $staleTanxBaseClawsTitle,
        $staleSereTalonAsMaul,
        $staleTanxAsCurseWish,
        'Sere Talon", "CLAWS.description"',
        'Vakuu''s Sere Talon", "CLAWS.description"',
        $staleSereTitleJoinedToClaws,
        '"CLAWS.description": "Choose 1 of 4 Curses'
    )

    foreach ($fragment in $forbiddenPckFragments) {
        if ($pckText.IndexOf($fragment, [System.StringComparison]::Ordinal) -ge 0) {
            Write-Host "PCK forbidden stale fragment found: $fragment | FAIL"
            $allPass = $false
        }
    }

    $sereTalonImportedTextures = [System.Text.RegularExpressions.Regex]::Matches(
        $pckText,
        '\.godot/imported/sere_talon_spire_plus\.png-[A-Fa-f0-9]+\.ctex') |
        ForEach-Object { $_.Value } |
        Sort-Object -Unique

    if ($sereTalonImportedTextures.Count -ge 2) {
        Write-Host "PCK content Sere Talon imported small/big textures | PASS"
    } else {
        Write-Host "PCK content Sere Talon imported small/big textures | found:$($sereTalonImportedTextures.Count) | FAIL"
        $allPass = $false
    }
} else {
    Write-Host "PCK content Sere Talon / Tanx Claws split | PCK missing | FAIL"
    $allPass = $false
}

if ($allPass) {
    Write-Host "PASS: installed Spire Plus compatibility artifacts, package zip, Sere Talon imported textures, and Sere Talon / Tanx Claws PCK content match handoff."
    exit 0
}

Write-Host "FAIL: one or more installed Spire Plus compatibility artifact, package zip, Sere Talon imported texture, or Sere Talon / Tanx Claws PCK checks did not match handoff."
exit 1
