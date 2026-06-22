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

function Assert-PathPresent {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$Message
    )

    if (-not (Test-Path -LiteralPath $Path)) {
        throw $Message
    }
}

function Assert-RetiredSharedRuntimeAbsent {
    $retiredRuntimeTokens = @(
        ('Base' + 'Lib'),
        ('base ' + 'lib'),
        ('base-' + 'lib')
    )
    $textExtensions = @(
        '.cfg',
        '.cs',
        '.csproj',
        '.css',
        '.csv',
        '.editorconfig',
        '.example',
        '.gitattributes',
        '.gitignore',
        '.html',
        '.json',
        '.md',
        '.props',
        '.ps1',
        '.sln',
        '.targets',
        '.ts',
        '.txt',
        '.xml',
        '.yaml',
        '.yml'
    )

    $trackedFiles = & git -C $repoRoot ls-files --
    if ($LASTEXITCODE -ne 0) {
        throw 'Unable to list tracked files for retired shared-runtime scan.'
    }

    $violations = New-Object System.Collections.Generic.List[string]
    foreach ($trackedFile in $trackedFiles) {
        $extension = [System.IO.Path]::GetExtension($trackedFile)
        if ($textExtensions -notcontains $extension) {
            continue
        }

        $path = Join-Path $repoRoot $trackedFile
        if (-not (Test-Path -LiteralPath $path)) {
            continue
        }

        $text = Get-Content -Raw -LiteralPath $path -Encoding UTF8
        foreach ($token in $retiredRuntimeTokens) {
            if ($text.IndexOf($token, [System.StringComparison]::OrdinalIgnoreCase) -ge 0) {
                $violations.Add($trackedFile)
                break
            }
        }
    }

    if ($violations.Count -gt 0) {
        throw "Retired shared-runtime guidance must stay out of tracked text files. Use STS2-RitsuLib docs and APIs instead:`n$($violations -join "`n")"
    }
}

$ezmbManifest = Read-JsonFile (Join-Path $repoRoot 'EZMicroBalance.json')
Assert-Equal $ezmbManifest.id 'EZMicroBalance' 'Spire Plus stable manifest id changed.'
Assert-Equal $ezmbManifest.name 'Spire Plus' 'Spire Plus player-facing manifest name changed.'
Assert-Equal $ezmbManifest.affects_gameplay $true 'Spire Plus must remain gameplay-affecting.'
Assert-RetiredSharedRuntimeAbsent

Get-ChildItem -LiteralPath $repoRoot -Recurse -File -Filter '*.json' |
    Where-Object {
        $_.FullName -notmatch '\\(\.git|\.godot|\.tools|bin|obj|publish|source code|node_modules)\\' -and
        $_.Name -ne 'package-lock.json'
    } |
    ForEach-Object {
        [void](Read-JsonFile $_.FullName)
    }

Assert-PathPresent (Join-Path $repoRoot 'website\README.md') 'Promoted website source is missing website\README.md.'
Assert-PathPresent (Join-Path $repoRoot '.github\workflows\spire-plus-site.yml') 'Promoted Pages workflow is missing.'

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
