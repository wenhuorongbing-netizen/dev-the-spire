param(
    [string]$ModDirectory,
    [string]$HandoffPath = "$PSScriptRoot\..\docs\private-beta-verification-handoff.md",
    [switch]$PassVerbose
)

$ErrorActionPreference = 'Stop'

if (-not $ModDirectory) {
    $defaultRoots = @(
        (Join-Path (Get-Location) 'mods\EZMicroBalance'),
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
    Write-Error "Could not locate EZMicroBalance install directory. Pass -ModDirectory explicitly."
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
Write-Host "Checking installed EZMicroBalance artifacts at: $ModDirectory"
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

$rows | ForEach-Object { Write-Host $_ }

if ($allPass) {
    Write-Host "PASS: installed EZMicroBalance artifacts match handoff hashes."
    exit 0
}

Write-Host "FAIL: one or more installed artifact hashes do not match handoff."
exit 1

