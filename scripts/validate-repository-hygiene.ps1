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

$futurePeekManifestPath = Join-Path $repoRoot 'EZFuturePeek.json'
if (Test-Path -LiteralPath $futurePeekManifestPath) {
    $futurePeekManifest = Read-JsonFile $futurePeekManifestPath
    Assert-Equal $futurePeekManifest.id 'EZFuturePeek' 'Future Peek manifest id changed.'
    Assert-Equal $futurePeekManifest.affects_gameplay $false 'Future Peek must remain preview-only.'
}

Get-ChildItem -LiteralPath $repoRoot -Recurse -File -Filter '*.json' |
    Where-Object {
        $_.FullName -notmatch '\\(\.git|\.godot|\.tools|bin|obj|publish|source code)\\'
    } |
    ForEach-Object {
        [void](Read-JsonFile $_.FullName)
    }

Assert-PathMissing (Join-Path $repoRoot 'website') 'Ignored website draft returned to the active root. Restore it only through an explicit website promotion.'
Assert-PathMissing (Join-Path $repoRoot '.github\workflows\spire-plus-site.yml') 'Ignored Pages workflow returned to the active workflow path. Promote the website deliberately before restoring it.'

$futurePeekForbidden = @(
    'EZFuturePeek',
    'Future Peek',
    'FuturePeek',
    'NCrystalSphere',
    'ScryMask',
    'NTransformPreview',
    'CycleThroughCards'
)

$activeSpirePlusFiles = @()
foreach ($dir in @('EZMicroBalance', 'EZMicroBalanceCode')) {
    $path = Join-Path $repoRoot $dir
    if (Test-Path -LiteralPath $path) {
        $activeSpirePlusFiles += Get-ChildItem -LiteralPath $path -Recurse -File |
            Where-Object { $_.Extension -in '.cs', '.json', '.tscn', '.tres', '.cfg' }
    }
}

foreach ($file in $activeSpirePlusFiles) {
    $text = Get-Content -Raw -LiteralPath $file.FullName -Encoding UTF8
    foreach ($fragment in $futurePeekForbidden) {
        if ($text.IndexOf($fragment, [System.StringComparison]::Ordinal) -ge 0) {
            $relative = Get-RepoRelativePath $file.FullName
            throw "Future Peek implementation leaked into Spire Plus active surface: $relative contains '$fragment'."
        }
    }
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
