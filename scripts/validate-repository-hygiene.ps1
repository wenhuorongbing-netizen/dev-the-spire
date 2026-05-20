param(
    [switch]$SkipPatchInventoryFreshness
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path

function Get-RepoRelativePath {
    param([Parameter(Mandatory = $true)][string]$Path)

    $root = [System.IO.Path]::GetFullPath($repoRoot).TrimEnd('\') + '\'
    $fullPath = [System.IO.Path]::GetFullPath($Path)
    if ($fullPath.StartsWith($root, [System.StringComparison]::OrdinalIgnoreCase)) {
        return $fullPath.Substring($root.Length)
    }

    return $fullPath
}

function Read-JsonFile {
    param([Parameter(Mandatory = $true)][string]$Path)

    try {
        return Get-Content -Raw -LiteralPath $Path -Encoding UTF8 | ConvertFrom-Json
    }
    catch {
        throw "Invalid JSON: $Path`n$($_.Exception.Message)"
    }
}

function Assert-Equal {
    param(
        [Parameter(Mandatory = $true)]$Actual,
        [Parameter(Mandatory = $true)]$Expected,
        [Parameter(Mandatory = $true)][string]$Message
    )

    if ($Actual -ne $Expected) {
        throw "$Message Expected '$Expected', found '$Actual'."
    }
}

function Assert-PathMissing {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$Message
    )

    if (Test-Path -LiteralPath $Path) {
        throw $Message
    }
}

$ezmbManifest = Read-JsonFile (Join-Path $repoRoot 'EZMicroBalance.json')
Assert-Equal $ezmbManifest.id 'EZMicroBalance' 'Spire Plus stable manifest id changed.'
Assert-Equal $ezmbManifest.name 'Spire Plus' 'Spire Plus player-facing manifest name changed.'
Assert-Equal $ezmbManifest.affects_gameplay $true 'Spire Plus must remain gameplay-affecting.'

Get-ChildItem -LiteralPath $repoRoot -Recurse -File -Filter '*.json' |
    Where-Object {
        $_.FullName -notmatch '\\(\.git|\.godot|\.tools|bin|obj|publish|source code)\\'
    } |
    ForEach-Object {
        [void](Read-JsonFile $_.FullName)
    }

Assert-PathMissing (Join-Path $repoRoot 'website') 'Ignored website draft returned to the active root. Restore it only through an explicit website promotion.'
Assert-PathMissing (Join-Path $repoRoot '.github\workflows\spire-plus-site.yml') 'Ignored Pages workflow returned to the active workflow path. Promote the website deliberately before restoring it.'

foreach ($removedRootSurface in @(
    'EzDailyContent',
    'EzDailyContentCode',
    'EzDailyContent.json',
    'EZFuturePeek',
    'EZFuturePeekCode',
    'EZFuturePeek.csproj',
    'EZFuturePeek.json',
    'EZFuturePeek.sln',
    'tests\EZFuturePeek.Tests',
    'scripts\export-future-peek.ps1'
)) {
    Assert-PathMissing (Join-Path $repoRoot $removedRootSurface) "Removed separate mod surface returned to the active root: $removedRootSurface"
}

if (-not $SkipPatchInventoryFreshness) {
    $inventoryPath = Join-Path $repoRoot 'docs\patch-inventory.md'
    $before = if (Test-Path -LiteralPath $inventoryPath) {
        Get-Content -Raw -LiteralPath $inventoryPath -Encoding UTF8
    } else {
        $null
    }

    & (Join-Path $repoRoot 'scripts\generate-patch-inventory.ps1') -Check | Out-Null
    $after = Get-Content -Raw -LiteralPath $inventoryPath -Encoding UTF8
    if ($before -ne $null -and $before -ne $after) {
        throw 'Patch inventory check changed the inventory file. Run scripts\generate-patch-inventory.ps1 and commit docs\patch-inventory.md.'
    }
}

Write-Host 'Repository hygiene validation passed.'
