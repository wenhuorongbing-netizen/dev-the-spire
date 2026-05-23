param(
    [string]$EvidenceRoot = ".tools\runtime-evidence\release-ready-manual",

    [string]$ManifestPath,

    [string]$PackageSha256 = "124AF7C77B33CE5EAC5A7369519D90AD66EC4CFCDC887DD1E352CF4F24E7968C",

    [string]$PackagePath = "publish\SpirePlus-v0.1.0-private-beta.0.zip",

    [int]$MinScreenshotWidth = 800,

    [int]$MinScreenshotHeight = 450,

    [switch]$WriteTemplate,

    [switch]$AllowDeferred
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path

$requiredReleaseRows = @(
    @{ Id = 'ancient-ui-urda'; Kind = 'clicked-ui'; Label = 'Urda clicked Ancient UI' },
    @{ Id = 'ancient-ui-morvi'; Kind = 'clicked-ui'; Label = 'Morvi clicked Ancient UI' },
    @{ Id = 'ancient-ui-lotha'; Kind = 'clicked-ui'; Label = 'Lotha clicked Ancient UI' },
    @{ Id = 'ancient-ui-vakuu-normal'; Kind = 'clicked-ui'; Label = 'Vakuu normal clicked Ancient UI' },
    @{ Id = 'ancient-ui-vakuu-fight'; Kind = 'clicked-ui'; Label = 'Vakuu force-fight clicked Ancient UI' },
    @{ Id = 'ancient-reward-visible-relics'; Kind = 'gameplay'; Label = 'Selected Ancient rewards visible as relics with readable hover tips' },
    @{ Id = 'player-text-tooltip-readability'; Kind = 'gameplay'; Label = 'Player-facing text, tooltip, and hover readability' },
    @{ Id = 'art-resource-routing-live-preview'; Kind = 'clicked-ui'; Label = 'Live UI preview proves event art, map icons, run-history icons, relic art, card art, and power art are not misrouted' },
    @{ Id = 'vakuu-victory-no-black-screen'; Kind = 'gameplay'; Label = 'Vakuu victory returns to the event without a black screen' },
    @{ Id = 'vakuu-failure-death-path'; Kind = 'gameplay'; Label = 'Vakuu failure and death path does not softlock' },
    @{ Id = 'vakuu-active-fight-save-load'; Kind = 'save-load'; Label = 'Vakuu active child-combat save/load' },
    @{ Id = 'ancient-state-save-load'; Kind = 'save-load'; Label = 'Urda, Morvi, Lotha, and Ancient reward state save/load' },
    @{ Id = 'rootblight-visual-behavior'; Kind = 'gameplay'; Label = 'Rootblight and Blight Sprout visual/gameplay behavior' },
    @{ Id = 'a11-natural-route-traversal'; Kind = 'gameplay'; Label = 'Natural A11 route click traversal' },
    @{ Id = 'disable-mod-gameplay'; Kind = 'gameplay'; Label = 'BaseLib-only disabled Spire Plus gameplay comparison' },
    @{ Id = 'coop-disposition'; Kind = 'coop'; Label = 'Two-client co-op disposition or explicit release-note deferral' }
)

$invalidEvidenceNotePattern = '(?i)\b(not counted|invalid|main menu|wrong surface|covered by|not gameplay evidence|do not satisfy|does not satisfy|loader health only)\b'

function Resolve-WorkspacePath {
    param([Parameter(Mandatory = $true)][string]$Path)

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }

    return [System.IO.Path]::GetFullPath((Join-Path $repoRoot $Path))
}

function Test-PathWithin {
    param(
        [Parameter(Mandatory = $true)][string]$BasePath,
        [Parameter(Mandatory = $true)][string]$ChildPath
    )

    $trimChars = [char[]]@([System.IO.Path]::DirectorySeparatorChar, [System.IO.Path]::AltDirectorySeparatorChar)
    $baseFull = [System.IO.Path]::GetFullPath($BasePath).TrimEnd($trimChars)
    $childFull = [System.IO.Path]::GetFullPath($ChildPath).TrimEnd($trimChars)
    $comparison = [System.StringComparison]::OrdinalIgnoreCase

    return $childFull.Equals($baseFull, $comparison) -or
        $childFull.StartsWith($baseFull + [System.IO.Path]::DirectorySeparatorChar, $comparison)
}

function Resolve-EvidenceFilePath {
    param(
        [Parameter(Mandatory = $true)][string]$EvidenceDir,
        [Parameter(Mandatory = $true)][string]$Path
    )

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }

    return [System.IO.Path]::GetFullPath((Join-Path $EvidenceDir $Path))
}

function Get-PropertyValue {
    param(
        [Parameter(Mandatory = $true)]$Object,
        [Parameter(Mandatory = $true)][string]$Name,
        $Default = $null
    )

    if ($Object.PSObject.Properties.Name -contains $Name) {
        return $Object.$Name
    }

    return $Default
}

function Get-DefaultRequiredFiles {
    param([Parameter(Mandatory = $true)][string]$Kind)

    switch ($Kind) {
        'clicked-ui' {
            return @('command.txt', 'window-preflight.json', 'godot.log', 'godot-log-audit.json', 'route-note.md')
        }
        'save-load' {
            return @('command.txt', 'godot.log', 'godot-log-audit.json', 'save-load-note.md')
        }
        'coop' {
            return @('command.txt', 'host-godot.log', 'host-godot-log-audit.json', 'client-godot.log', 'client-godot-log-audit.json', 'result-note.md')
        }
        default {
            return @('command.txt', 'godot.log', 'godot-log-audit.json', 'result-note.md')
        }
    }
}

function Merge-RequiredEvidenceFiles {
    param(
        [Parameter(Mandatory = $true)][string[]]$DefaultFiles,
        [object[]]$RowFiles = @()
    )

    $merged = [System.Collections.Generic.List[string]]::new()
    foreach ($file in $DefaultFiles) {
        if (-not [string]::IsNullOrWhiteSpace($file) -and -not $merged.Contains($file)) {
            [void]$merged.Add($file)
        }
    }

    foreach ($file in $RowFiles) {
        $fileString = [string]$file
        if (-not [string]::IsNullOrWhiteSpace($fileString) -and -not $merged.Contains($fileString)) {
            [void]$merged.Add($fileString)
        }
    }

    return @($merged)
}

function Read-CleanLogAudit {
    param([Parameter(Mandatory = $true)][string]$AuditPath)

    $audit = Get-Content -LiteralPath $AuditPath -Raw -Encoding UTF8 | ConvertFrom-Json
    $items = @()
    if ($audit -is [System.Array]) {
        $items = @($audit)
    } else {
        $items = @($audit)
    }

    if ($items.Count -eq 0) {
        return $false
    }

    foreach ($item in $items) {
        if (-not [bool](Get-PropertyValue -Object $item -Name 'Clean' -Default $false)) {
            return $false
        }
    }

    return $true
}

function Test-PreflightForeground {
    param([Parameter(Mandatory = $true)][string]$PreflightPath)

    $preflight = Get-Content -LiteralPath $PreflightPath -Raw -Encoding UTF8 | ConvertFrom-Json
    return [bool](Get-PropertyValue -Object $preflight -Name 'SpireForeground' -Default $false)
}

function Test-PngSignature {
    param([Parameter(Mandatory = $true)][string]$Path)

    return $null -ne (Get-PngDimensions -Path $Path)
}

function Get-PngDimensions {
    param([Parameter(Mandatory = $true)][string]$Path)

    $stream = [System.IO.File]::OpenRead($Path)
    try {
        if ($stream.Length -lt 24) {
            return $null
        }

        $bytes = [byte[]]::new(24)
        [void]$stream.Read($bytes, 0, 24)
        $signature = [byte[]]@(137, 80, 78, 71, 13, 10, 26, 10)

        for ($index = 0; $index -lt $signature.Length; $index++) {
            if ($bytes[$index] -ne $signature[$index]) {
                return $null
            }
        }

        if ([char]$bytes[12] -ne 'I' -or [char]$bytes[13] -ne 'H' -or [char]$bytes[14] -ne 'D' -or [char]$bytes[15] -ne 'R') {
            return $null
        }

        $width =
            ([int]$bytes[16] -shl 24) -bor
            ([int]$bytes[17] -shl 16) -bor
            ([int]$bytes[18] -shl 8) -bor
            [int]$bytes[19]
        $height =
            ([int]$bytes[20] -shl 24) -bor
            ([int]$bytes[21] -shl 16) -bor
            ([int]$bytes[22] -shl 8) -bor
            [int]$bytes[23]

        return [pscustomobject]@{
            Width = $width
            Height = $height
        }
    } finally {
        $stream.Dispose()
    }
}

function Test-PngMinimumDimensions {
    param([Parameter(Mandatory = $true)][string]$Path)

    $dimensions = Get-PngDimensions -Path $Path
    if ($null -eq $dimensions) {
        return $false
    }

    return $dimensions.Width -ge $MinScreenshotWidth -and $dimensions.Height -ge $MinScreenshotHeight
}

function New-TemplateManifest {
    param([Parameter(Mandatory = $true)][string]$OutputPath)

    $rows = foreach ($required in $requiredReleaseRows) {
        [ordered]@{
            Id = $required.Id
            Label = $required.Label
            Kind = $required.Kind
            Status = 'pending'
            EvidenceDir = ''
            RequiredFiles = @(Get-DefaultRequiredFiles -Kind $required.Kind)
            ScreenshotFile = if ($required.Kind -eq 'clicked-ui') { '' } else { $null }
            ResultNote = ''
            ReleaseNote = ''
            ExplicitOwnerDecision = $false
        }
    }

    $template = [ordered]@{
        PackageSha256 = $PackageSha256
        PackagePath = $PackagePath
        CreatedAt = (Get-Date).ToString('o')
        Rows = @($rows)
    }

    $parent = Split-Path -Parent $OutputPath
    if ($parent) {
        New-Item -ItemType Directory -Path $parent -Force | Out-Null
    }

    $template | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $OutputPath -Encoding UTF8
}

function Add-Failure {
    param(
        [System.Collections.Generic.List[string]]$Failures,
        [Parameter(Mandatory = $true)][string]$Message
    )

    [void]$Failures.Add($Message)
}

function Add-Warning {
    param(
        [System.Collections.Generic.List[string]]$Warnings,
        [Parameter(Mandatory = $true)][string]$Message
    )

    [void]$Warnings.Add($Message)
}

$evidenceRootFull = Resolve-WorkspacePath -Path $EvidenceRoot
if ([string]::IsNullOrWhiteSpace($ManifestPath)) {
    $ManifestPath = Join-Path $evidenceRootFull 'release-evidence-manifest.json'
}

$manifestFull = Resolve-WorkspacePath -Path $ManifestPath
if (-not (Test-PathWithin -BasePath $evidenceRootFull -ChildPath $manifestFull)) {
    Write-Error "ManifestPath is outside EvidenceRoot: $manifestFull."
    exit 1
}

if ($WriteTemplate) {
    New-TemplateManifest -OutputPath $manifestFull
    Write-Output "Wrote release evidence template: $manifestFull"
    exit 0
}

if (-not (Test-Path -LiteralPath $manifestFull)) {
    Write-Error "Missing release evidence manifest: $manifestFull. Run this script with -WriteTemplate first, then fill the evidence rows."
    exit 1
}

$manifest = Get-Content -LiteralPath $manifestFull -Raw -Encoding UTF8 | ConvertFrom-Json
$failures = [System.Collections.Generic.List[string]]::new()
$warnings = [System.Collections.Generic.List[string]]::new()

$manifestPackageSha256 = Get-PropertyValue -Object $manifest -Name 'PackageSha256' -Default ''
if ($manifestPackageSha256 -ne $PackageSha256) {
    Add-Failure -Failures $failures -Message "Manifest PackageSha256 '$manifestPackageSha256' does not match current package '$PackageSha256'."
}

$manifestPackagePath = [string](Get-PropertyValue -Object $manifest -Name 'PackagePath' -Default $PackagePath)
if ([string]::IsNullOrWhiteSpace($manifestPackagePath)) {
    $manifestPackagePath = $PackagePath
}

$packageFull = Resolve-WorkspacePath -Path $manifestPackagePath
$actualPackageSha256 = ''
if (-not (Test-Path -LiteralPath $packageFull -PathType Leaf)) {
    Add-Failure -Failures $failures -Message "Package under test does not exist: $packageFull."
} else {
    $actualPackageSha256 = (Get-FileHash -LiteralPath $packageFull -Algorithm SHA256).Hash.ToUpperInvariant()
    if ($actualPackageSha256 -ne $PackageSha256) {
        Add-Failure -Failures $failures -Message "Actual package SHA256 '$actualPackageSha256' for '$packageFull' does not match current package '$PackageSha256'."
    }
}

$rows = @(Get-PropertyValue -Object $manifest -Name 'Rows' -Default @())
$rowMap = @{}
$requiredRowIds = @{}
foreach ($required in $requiredReleaseRows) {
    $requiredRowIds[$required.Id] = $true
}

foreach ($row in $rows) {
    $id = [string](Get-PropertyValue -Object $row -Name 'Id' -Default '')
    if ([string]::IsNullOrWhiteSpace($id)) {
        Add-Warning -Warnings $warnings -Message 'Release evidence manifest contains a row with no Id; it is ignored.'
        continue
    }

    if ($rowMap.ContainsKey($id)) {
        Add-Failure -Failures $failures -Message "Duplicate release evidence row id: $id."
        continue
    }

    if (-not $requiredRowIds.ContainsKey($id)) {
        Add-Warning -Warnings $warnings -Message "Unknown release evidence row id ignored: $id."
    }

    $rowMap[$id] = $row
}

foreach ($required in $requiredReleaseRows) {
    if (-not $rowMap.ContainsKey($required.Id)) {
        Add-Failure -Failures $failures -Message "Missing required release evidence row: $($required.Id) ($($required.Label))."
        continue
    }

    $row = $rowMap[$required.Id]
    $status = ([string](Get-PropertyValue -Object $row -Name 'Status' -Default '')).ToLowerInvariant()
    $rowKind = [string](Get-PropertyValue -Object $row -Name 'Kind' -Default $required.Kind)
    if (-not [string]::Equals($rowKind, $required.Kind, [System.StringComparison]::OrdinalIgnoreCase)) {
        Add-Failure -Failures $failures -Message "Row $($required.Id) Kind '$rowKind' does not match required kind '$($required.Kind)'."
    }

    if ($status -eq 'deferred') {
        $explicitOwnerDecision = [bool](Get-PropertyValue -Object $row -Name 'ExplicitOwnerDecision' -Default $false)
        $releaseNote = [string](Get-PropertyValue -Object $row -Name 'ReleaseNote' -Default '')
        if (-not $AllowDeferred) {
            Add-Failure -Failures $failures -Message "Row $($required.Id) is deferred. Re-run with -AllowDeferred only after the owner explicitly accepts a release-note deferral."
            continue
        }

        if (-not $explicitOwnerDecision -or [string]::IsNullOrWhiteSpace($releaseNote)) {
            Add-Failure -Failures $failures -Message "Row $($required.Id) is deferred without ExplicitOwnerDecision=true and a ReleaseNote."
        }
        continue
    }

    if ($status -ne 'pass') {
        Add-Failure -Failures $failures -Message "Row $($required.Id) is not pass or accepted deferred. Current status: '$status'."
        continue
    }

    $evidenceDirRaw = [string](Get-PropertyValue -Object $row -Name 'EvidenceDir' -Default '')
    if ([string]::IsNullOrWhiteSpace($evidenceDirRaw)) {
        Add-Failure -Failures $failures -Message "Row $($required.Id) has pass status but no EvidenceDir."
        continue
    }

    $evidenceDir = Resolve-WorkspacePath -Path $evidenceDirRaw
    if (-not (Test-PathWithin -BasePath $evidenceRootFull -ChildPath $evidenceDir)) {
        Add-Failure -Failures $failures -Message "Row $($required.Id) EvidenceDir is outside EvidenceRoot: $evidenceDir."
        continue
    }

    if (-not (Test-Path -LiteralPath $evidenceDir -PathType Container)) {
        Add-Failure -Failures $failures -Message "Row $($required.Id) EvidenceDir does not exist: $evidenceDir."
        continue
    }

    $defaultRequiredFiles = @(Get-DefaultRequiredFiles -Kind $required.Kind)
    $rowRequiredFiles = @(Get-PropertyValue -Object $row -Name 'RequiredFiles' -Default @())
    $requiredFiles = @(Merge-RequiredEvidenceFiles -DefaultFiles $defaultRequiredFiles -RowFiles $rowRequiredFiles)
    foreach ($requiredFile in $requiredFiles) {
        $requiredFileString = [string]$requiredFile
        if ([string]::IsNullOrWhiteSpace($requiredFileString)) {
            continue
        }

        $filePath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path $requiredFileString
        if (-not (Test-PathWithin -BasePath $evidenceDir -ChildPath $filePath)) {
            Add-Failure -Failures $failures -Message "Row $($required.Id) required evidence file path escapes EvidenceDir: $requiredFileString."
            continue
        }

        if (-not (Test-Path -LiteralPath $filePath -PathType Leaf)) {
            Add-Failure -Failures $failures -Message "Row $($required.Id) missing required evidence file: $filePath."
            continue
        }

        if ((Get-Item -LiteralPath $filePath).Length -eq 0) {
            Add-Failure -Failures $failures -Message "Row $($required.Id) required evidence file is empty: $filePath."
        }

        if ($requiredFileString.EndsWith('.md', [System.StringComparison]::OrdinalIgnoreCase)) {
            $note = Get-Content -LiteralPath $filePath -Raw -Encoding UTF8
            if ([string]::IsNullOrWhiteSpace($note)) {
                Add-Failure -Failures $failures -Message "Row $($required.Id) evidence note '$requiredFileString' is empty."
            }

            if ($note -match $invalidEvidenceNotePattern) {
                Add-Failure -Failures $failures -Message "Row $($required.Id) evidence note '$requiredFileString' describes invalid or non-counting evidence."
            }
        }
    }

    $logAuditFiles = @($requiredFiles | Where-Object { ([string]$_).EndsWith('godot-log-audit.json', [System.StringComparison]::OrdinalIgnoreCase) })
    foreach ($logAuditFile in $logAuditFiles) {
        $auditPath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path ([string]$logAuditFile)
        if (-not (Test-PathWithin -BasePath $evidenceDir -ChildPath $auditPath)) {
            continue
        }

        if ((Test-Path -LiteralPath $auditPath -PathType Leaf) -and -not (Read-CleanLogAudit -AuditPath $auditPath)) {
            Add-Failure -Failures $failures -Message "Row $($required.Id) log audit is not clean: $auditPath."
        }
    }

    if ($required.Kind -eq 'clicked-ui') {
        $preflightPath = Join-Path $evidenceDir 'window-preflight.json'
        if ((Test-Path -LiteralPath $preflightPath -PathType Leaf) -and -not (Test-PreflightForeground -PreflightPath $preflightPath)) {
            Add-Failure -Failures $failures -Message "Row $($required.Id) window-preflight.json does not prove Slay the Spire 2 was foreground."
        }

        $screenshotFile = [string](Get-PropertyValue -Object $row -Name 'ScreenshotFile' -Default '')
        if (-not [string]::IsNullOrWhiteSpace($screenshotFile)) {
            $screenshotPath = Resolve-EvidenceFilePath -EvidenceDir $evidenceDir -Path $screenshotFile
            if (-not (Test-PathWithin -BasePath $evidenceDir -ChildPath $screenshotPath)) {
                Add-Failure -Failures $failures -Message "Row $($required.Id) screenshot path escapes EvidenceDir: $screenshotFile."
            } elseif (-not (Test-Path -LiteralPath $screenshotPath -PathType Leaf)) {
                Add-Failure -Failures $failures -Message "Row $($required.Id) screenshot file is missing: $screenshotPath."
            } elseif ((Get-Item -LiteralPath $screenshotPath).Length -eq 0) {
                Add-Failure -Failures $failures -Message "Row $($required.Id) screenshot file is empty: $screenshotPath."
            } elseif (-not (Test-PngSignature -Path $screenshotPath)) {
                Add-Failure -Failures $failures -Message "Row $($required.Id) screenshot file is not a valid PNG: $screenshotPath."
            } elseif (-not (Test-PngMinimumDimensions -Path $screenshotPath)) {
                Add-Failure -Failures $failures -Message "Row $($required.Id) screenshot file is too small: $screenshotPath. Minimum is ${MinScreenshotWidth}x${MinScreenshotHeight}."
            }
        } else {
            $screenshots = @(Get-ChildItem -LiteralPath $evidenceDir -Filter '*.png' -File -ErrorAction SilentlyContinue)
            if ($screenshots.Count -eq 0) {
                Add-Failure -Failures $failures -Message "Row $($required.Id) needs a PNG screenshot or ScreenshotFile."
            } elseif (-not ($screenshots | Where-Object { $_.Length -gt 0 } | Select-Object -First 1)) {
                Add-Failure -Failures $failures -Message "Row $($required.Id) has only empty PNG screenshots in $evidenceDir."
            } elseif (-not ($screenshots | Where-Object { $_.Length -gt 0 -and (Test-PngSignature -Path $_.FullName) } | Select-Object -First 1)) {
                Add-Failure -Failures $failures -Message "Row $($required.Id) has no valid non-empty PNG screenshots in $evidenceDir."
            } elseif (-not ($screenshots | Where-Object { $_.Length -gt 0 -and (Test-PngMinimumDimensions -Path $_.FullName) } | Select-Object -First 1)) {
                Add-Failure -Failures $failures -Message "Row $($required.Id) has no valid PNG screenshots at least ${MinScreenshotWidth}x${MinScreenshotHeight} in $evidenceDir."
            }
        }

    }

    $resultNote = [string](Get-PropertyValue -Object $row -Name 'ResultNote' -Default '')
    if ([string]::IsNullOrWhiteSpace($resultNote)) {
        Add-Failure -Failures $failures -Message "Row $($required.Id) is pass but has no ResultNote."
    } elseif ($resultNote -match $invalidEvidenceNotePattern) {
        Add-Failure -Failures $failures -Message "Row $($required.Id) ResultNote describes invalid or non-counting evidence."
    }
}

$summary = [ordered]@{
    ManifestPath = $manifestFull
    EvidenceRoot = $evidenceRootFull
    CheckedAt = (Get-Date).ToString('o')
    PackageSha256 = $PackageSha256
    PackagePath = $packageFull
    ActualPackageSha256 = $actualPackageSha256
    MinScreenshotWidth = $MinScreenshotWidth
    MinScreenshotHeight = $MinScreenshotHeight
    AllowDeferred = [bool]$AllowDeferred
    RequiredRowCount = $requiredReleaseRows.Count
    FailureCount = $failures.Count
    WarningCount = $warnings.Count
    Failures = @($failures)
    Warnings = @($warnings)
}

$summary | ConvertTo-Json -Depth 20

if ($failures.Count -gt 0) {
    exit 1
}
