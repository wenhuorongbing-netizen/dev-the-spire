param(
    [switch]$Check,
    [string]$OutputPath = 'docs\patch-inventory.md'
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
$resolvedOutputPath = if ([System.IO.Path]::IsPathRooted($OutputPath)) {
    [System.IO.Path]::GetFullPath($OutputPath)
} else {
    [System.IO.Path]::GetFullPath((Join-Path $repoRoot $OutputPath))
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

function Normalize-InventoryForCheck {
    param([Parameter(Mandatory = $true)][string]$Content)

    return (($Content -replace "`r`n", "`n") -replace '(?m)^Generated: .+$', 'Generated: <ignored>').TrimEnd()
}

function Get-OwnerFromPath {
    param([Parameter(Mandatory = $true)][string]$RelativePath)

    if ($RelativePath -like 'EZMicroBalanceCode/Ancients/Expansion/Urda/*') { return 'Urda' }
    if ($RelativePath -like 'EZMicroBalanceCode/Ancients/Expansion/Morvi/*') { return 'Morvi' }
    if ($RelativePath -like 'EZMicroBalanceCode/Ancients/Expansion/Lotha/*') { return 'Lotha' }
    if ($RelativePath -like 'EZMicroBalanceCode/Ancients/Expansion/Vakuu/*') { return 'Vakuu' }
    if ($RelativePath -like 'EZMicroBalanceCode/Ancients/Common/*') { return 'Ancient shared infrastructure' }
    if ($RelativePath -like 'EZMicroBalanceCode/Ancients/Patches/*') { return 'Ancient reward rebalance' }
    if ($RelativePath -like 'EZMicroBalanceCode/Ascension/Core/*') { return 'Ascension core' }
    if ($RelativePath -like 'EZMicroBalanceCode/Ascension/Map/*') { return 'Ascension map' }
    if ($RelativePath -like 'EZMicroBalanceCode/Ascension/Combat/*') { return 'Ascension combat' }
    if ($RelativePath -like 'EZMicroBalanceCode/Ascension/Rewards/*') { return 'Ascension rewards' }
    if ($RelativePath -like 'EZMicroBalanceCode/Ascension/Patches/*') { return 'Ascension patches' }
    if ($RelativePath -like 'EZMicroBalanceCode/Ascension/Events/*') { return 'Ascension events' }
    if ($RelativePath -like 'EZMicroBalanceCode/Map/*') { return 'Map hover composition' }
    if ($RelativePath -like 'EZMicroBalanceCode/Modding/*') { return 'Mod info localization' }
    if ($RelativePath -like 'EZMicroBalanceCode/Preview/*') { return 'Preview tools' }
    return 'Unclassified'
}

function Get-RiskFromPatch {
    param(
        [Parameter(Mandatory = $true)][string]$RelativePath,
        [Parameter(Mandatory = $true)][string]$PatchText
    )

    $combined = "$RelativePath $PatchText"
    if ($combined -match 'CombatRoom|EventRoom|RunManager|SaveManager|StartRunLobby|NGame|JoinFlow') { return 'High' }
    if ($combined -match 'NMap|NRewardsScreen|NCrystalSphere|NTransformPreview|NNormalMapPoint|NBossMapPoint|NRelicInventory|NModInfoContainer') { return 'Medium' }
    if ($combined -match 'CardModel|RelicModel|CardReward|CardPileCmd|Creature|Power|AbstractModel') { return 'Medium' }
    return 'Low'
}

$sourceRoots = @('EZMicroBalanceCode')
$patches = [System.Collections.Generic.List[object]]::new()

foreach ($root in $sourceRoots) {
    $absoluteRoot = Join-Path $repoRoot $root
    if (-not (Test-Path -LiteralPath $absoluteRoot)) {
        continue
    }

    Get-ChildItem -LiteralPath $absoluteRoot -Recurse -File -Filter '*.cs' |
        Sort-Object -Property FullName |
        ForEach-Object {
            $relativePath = Get-RepoRelativePath $_.FullName
            $lines = Get-Content -LiteralPath $_.FullName -Encoding UTF8
            for ($index = 0; $index -lt $lines.Count; $index++) {
                $line = $lines[$index].Trim()
                if ($line.StartsWith('[HarmonyPatch', [System.StringComparison]::Ordinal)) {
                    $patchText = $line
                    $cursor = $index + 1
                    while ($patchText -notmatch '\]$' -and $cursor -lt $lines.Count) {
                        $patchText += ' ' + $lines[$cursor].Trim()
                        $cursor++
                    }

                    $owner = Get-OwnerFromPath $relativePath
                    $risk = Get-RiskFromPatch $relativePath $patchText
                    [void]$patches.Add([pscustomobject]@{
                        File = $relativePath
                        Line = $index + 1
                        Owner = $owner
                        Risk = $risk
                        Patch = ($patchText -replace '\|', '\|')
                    })
                }
            }
        }
}

$generatedAt = (Get-Date).ToString('yyyy-MM-dd')
$highCount = @($patches | Where-Object Risk -eq 'High').Count
$mediumCount = @($patches | Where-Object Risk -eq 'Medium').Count
$lowCount = @($patches | Where-Object Risk -eq 'Low').Count
$unclassifiedCount = @($patches | Where-Object Owner -eq 'Unclassified').Count

$builder = [System.Text.StringBuilder]::new()
[void]$builder.AppendLine('# Harmony Patch Inventory')
[void]$builder.AppendLine()
[void]$builder.AppendLine("Generated: $generatedAt")
[void]$builder.AppendLine()
[void]$builder.AppendLine('Purpose: keep every Harmony patch visible, owned, and risk-labeled. Regenerate after adding, moving, or deleting patch declarations.')
[void]$builder.AppendLine()
[void]$builder.AppendLine('Regenerate:')
[void]$builder.AppendLine()
[void]$builder.AppendLine('```powershell')
[void]$builder.AppendLine('.\scripts\generate-patch-inventory.ps1')
[void]$builder.AppendLine('.\scripts\validate-repository-hygiene.ps1')
[void]$builder.AppendLine('```')
[void]$builder.AppendLine()
[void]$builder.AppendLine('## Summary')
[void]$builder.AppendLine()
[void]$builder.AppendLine("| Metric | Count |")
[void]$builder.AppendLine("| --- | ---: |")
[void]$builder.AppendLine("| Total patch declarations | $($patches.Count) |")
[void]$builder.AppendLine("| High risk | $highCount |")
[void]$builder.AppendLine("| Medium risk | $mediumCount |")
[void]$builder.AppendLine("| Low risk | $lowCount |")
[void]$builder.AppendLine("| Unclassified owner | $unclassifiedCount |")
[void]$builder.AppendLine()
[void]$builder.AppendLine('## Risk Meaning')
[void]$builder.AppendLine()
[void]$builder.AppendLine('- High: run, room, save, lobby, multiplayer, or game lifecycle surface.')
[void]$builder.AppendLine('- Medium: UI, card, relic, reward, combat object, or model hook surface.')
[void]$builder.AppendLine('- Low: narrow local hook with lower source-drift blast radius.')
[void]$builder.AppendLine()
[void]$builder.AppendLine('## Patches')
[void]$builder.AppendLine()
[void]$builder.AppendLine('| Owner | Risk | File | Line | Patch |')
[void]$builder.AppendLine('| --- | --- | --- | ---: | --- |')
foreach ($patch in $patches) {
    $fileCell = '`' + $patch.File + '`'
    $patchCell = '`' + $patch.Patch + '`'
    [void]$builder.AppendLine(('| {0} | {1} | {2} | {3} | {4} |' -f $patch.Owner, $patch.Risk, $fileCell, $patch.Line, $patchCell))
}

$content = $builder.ToString()
if ($Check) {
    if (-not (Test-Path -LiteralPath $resolvedOutputPath)) {
        throw "Patch inventory missing: $resolvedOutputPath"
    }

    $current = Get-Content -Raw -LiteralPath $resolvedOutputPath -Encoding UTF8
    $normalizedCurrent = Normalize-InventoryForCheck $current
    $normalizedExpected = Normalize-InventoryForCheck $content
    if ($normalizedCurrent -ne $normalizedExpected) {
        throw "Patch inventory is stale: $resolvedOutputPath"
    }

    Write-Host "Patch inventory is fresh: $resolvedOutputPath"
    return
}

$parent = Split-Path -Parent $resolvedOutputPath
if ($parent) {
    New-Item -ItemType Directory -Force -Path $parent | Out-Null
}

[System.IO.File]::WriteAllText($resolvedOutputPath, $content, [System.Text.UTF8Encoding]::new($false))
Write-Host "Wrote $resolvedOutputPath"
