param(
    [string[]]$Files
)

function ConvertTo-KebabCase($name) {
    $result = ''
    for ($i = 0; $i -lt $name.Length; $i++) {
        $c = $name[$i]
        if ($c -cmatch '[A-Z]' -and $i -gt 0) {
            $result += '-'
        }
        $result += $c.ToString().ToLower()
    }
    return $result
}

function Process-File($filepath) {
    $lines = Get-Content $filepath
    $needsUsing = $true
    foreach ($line in $lines) {
        if ($line -match 'STS2RitsuLib\.Patching\.Models') { $needsUsing = $false; break }
    }

    $newLines = [System.Collections.ArrayList]@()
    $i = 0
    $processed = @()

    while ($i -lt $lines.Count) {
        $line = $lines[$i]

        # Check for [HarmonyPatch] attribute
        $targetType = ''
        $methodName = ''
        $isGetter = $false

        if ($line -match 'typeof\((\w+)') {
            $targetType = $matches[1]
        }

        if ($line -match 'nameof\(\w+\.(\w+)\)') {
            $methodName = $matches[1]
        } elseif ($line -match '"([^"]+)"') {
            $methodName = $matches[1]
        }

        if ($line -match 'MethodType\.Getter' -or $methodName -match '^get_') {
            $isGetter = $true
        }

        if ($targetType -and $methodName) {
            # Look ahead for class declaration
            $j = $i + 1
            while ($j -lt $lines.Count -and $lines[$j].Trim() -eq '') { $j++ }

            if ($j -lt $lines.Count -and $lines[$j] -match 'internal static class (\w+)') {
                $className = $matches[1]
                $patchId = ConvertTo-KebabCase $className

                if ($isGetter) {
                    $targetStr = 'new ModPatchTarget(typeof(' + $targetType + '), "' + $methodName + '", HarmonyLib.MethodType.Getter)'
                } else {
                    $targetStr = 'new ModPatchTarget(typeof(' + $targetType + '), nameof(' + $targetType + '.' + $methodName + '))'
                }

                # Add IPatchMethod implementation
                [void]$newLines.Add('internal sealed class ' + $className + ' : IPatchMethod')
                [void]$newLines.Add('{')
                [void]$newLines.Add('    static string IPatchMethod.PatchId => "' + $patchId + '";')
                [void]$newLines.Add('    static bool IPatchMethod.IsCritical => false;')
                [void]$newLines.Add('    static string IPatchMethod.Description => "Patch ' + $targetType + '.' + $methodName + '";')
                [void]$newLines.Add('    static ModPatchTarget[] IPatchMethod.GetTargets() =>')
                [void]$newLines.Add('        [' + $targetStr + '];')

                $i = $j + 1
                $processed += $className
                continue
            }
        }

        [void]$newLines.Add($line)
        $i++
    }

    # Add using directive if needed
    if ($needsUsing -and $processed.Count -gt 0) {
        $result = [System.Collections.ArrayList]@()
        $added = $false
        foreach ($l in $newLines) {
            if ($l -match '^namespace ' -and -not $added) {
                [void]$result.Add('using STS2RitsuLib.Patching.Models;')
                [void]$result.Add('')
                $added = $true
            }
            [void]$result.Add($l)
        }
        $newLines = $result
    }

    if ($processed.Count -gt 0) {
        Set-Content -Path $filepath -Value ($newLines -join "`n") -Encoding UTF8
    }

    return $processed
}

$allProcessed = @()
foreach ($f in $Files) {
    $result = Process-File $f
    foreach ($r in $result) {
        $allProcessed += [System.IO.Path]::GetFileName($f) + ' : ' + $r
    }
}

Write-Host "Processed $($allProcessed.Count) classes"
foreach ($p in $allProcessed) {
    Write-Host "  $p"
}
