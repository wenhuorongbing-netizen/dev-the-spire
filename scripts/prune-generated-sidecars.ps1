param(
    [switch]$DryRun
)

$ErrorActionPreference = 'Stop'

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path

function Assert-UnderPath {
    param(
        [Parameter(Mandatory)] [string]$Candidate,
        [Parameter(Mandatory)] [string]$Parent
    )

    $parentFull = [System.IO.Path]::GetFullPath($Parent).TrimEnd('\') + '\'
    $candidateFull = [System.IO.Path]::GetFullPath($Candidate)
    if (-not $candidateFull.StartsWith($parentFull, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to prune path outside expected parent. Candidate: $candidateFull Parent: $parentFull"
    }
}

function Get-RepoRelativePath {
    param(
        [Parameter(Mandatory)] [string]$Path
    )

    $repoFull = [System.IO.Path]::GetFullPath($repoRoot).TrimEnd('\') + '\'
    $pathFull = [System.IO.Path]::GetFullPath($Path)
    if ($pathFull.StartsWith($repoFull, [System.StringComparison]::OrdinalIgnoreCase)) {
        return $pathFull.Substring($repoFull.Length)
    }

    return $pathFull
}

$targetSets = @(
    @{
        Root = Join-Path $repoRoot 'EZMicroBalanceCode'
        Pattern = '*.cs.uid'
        Label = 'source C# UID sidecars'
    },
    @{
        Root = Join-Path $repoRoot 'tests'
        Pattern = '*.cs.uid'
        Label = 'test C# UID sidecars'
    },
    @{
        Root = Join-Path $repoRoot 'website'
        Pattern = '*.import'
        Label = 'website Godot import sidecars'
    }
)

$total = 0
foreach ($targetSet in $targetSets) {
    if (-not (Test-Path -LiteralPath $targetSet.Root)) {
        continue
    }

    $files = Get-ChildItem -LiteralPath $targetSet.Root -Recurse -File -Filter $targetSet.Pattern |
        Sort-Object -Property FullName

    foreach ($file in $files) {
        Assert-UnderPath -Candidate $file.FullName -Parent $targetSet.Root
        $relativePath = Get-RepoRelativePath -Path $file.FullName
        if ($DryRun) {
            Write-Host "Would remove $relativePath"
        }
        else {
            Remove-Item -LiteralPath $file.FullName -Force
            Write-Host "Removed $relativePath"
        }

        $total++
    }

    Write-Host "$($targetSet.Label): $($files.Count)"
}

if ($DryRun) {
    Write-Host "Dry run complete: $total generated sidecar file(s) would be removed."
}
else {
    Write-Host "Pruned $total generated sidecar file(s)."
}
