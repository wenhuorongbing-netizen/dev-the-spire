param(
    [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
    [string[]] $Path,

    [string] $OutFile,

    [switch] $FailOnHit
)

$ErrorActionPreference = 'Stop'

$signatures = @(
    @{ Name = 'Creature.get_ShowsInfiniteHp'; Pattern = 'Creature\.get_ShowsInfiniteHp' },
    @{ Name = 'BaseLib.Patches.UI.HealthBarForecastPatch'; Pattern = 'BaseLib\.Patches\.UI\.HealthBarForecastPatch' },
    @{ Name = 'BaseLib patch failure'; Pattern = '(?i)BaseLib.*(?:[1-9][0-9]*\s+failed|patch(?:es)?\s+failed|patch\s+failure|failed\s+to\s+patch|exception)' },
    @{ Name = 'DamageMeter'; Pattern = '(?i)\[ERROR\].*DamageMeter' },
    @{ Name = 'RouteSuggest'; Pattern = '(?i)\[ERROR\].*RouteSuggest' },
    @{ Name = 'Spire Plus error/exception'; Pattern = '(?i)(EZMicroBalance|Spire Plus).*(error|exception)' },
    @{ Name = 'TypeLoadException'; Pattern = 'TypeLoadException' },
    @{ Name = 'MissingMethodException'; Pattern = 'MissingMethodException' },
    @{ Name = 'Godot ERROR line'; Pattern = '(?m)^\s*(?:\[ERROR\]|ERROR\b|\[[^\]]+\]\s*ERROR\b)' }
)

$ignoredErrorPatterns = @(
    'Mod manifest .*RouteSuggestConfig\.json.*id.*field',
    'Mod manifest .*sts2-heybox-support[\\/ ]+mod_mainfest\.json.*id.*field',
    'Mod manifest .*STS2-RitsuLib[\\/ ]+ritsulib-variants\.json.*id.*field'
)

function ShouldIgnoreLine
{
    param([string] $line)

    foreach ($pattern in $ignoredErrorPatterns)
    {
        if ([regex]::IsMatch($line, $pattern, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase))
        {
            return $true
        }
    }

    return $false
}

$results = foreach ($inputPath in $Path) {
    $resolved = Resolve-Path -LiteralPath $inputPath
    $file = Get-Item -LiteralPath $resolved.Path
    $content = Get-Content -LiteralPath $resolved.Path -Raw -Encoding UTF8
    $scannableLines = $content -split "`r?`n" |
        Where-Object { -not [string]::IsNullOrWhiteSpace($_) -and -not (ShouldIgnoreLine $_) }
    $scannableContent = ($scannableLines -join "`n")

    $hits = foreach ($signature in $signatures) {
        $matches = [regex]::Matches($scannableContent, $signature.Pattern)
        [pscustomobject]@{
            Name = $signature.Name
            Count = $matches.Count
        }
    }

    [pscustomobject]@{
        Path = $file.FullName
        LastWriteTime = $file.LastWriteTime
        Length = $file.Length
        Sha256 = (Get-FileHash -Algorithm SHA256 -LiteralPath $file.FullName).Hash.ToLowerInvariant()
        Clean = -not ($hits | Where-Object { $_.Count -gt 0 })
        SignatureHits = $hits
    }
}

$json = $results | ConvertTo-Json -Depth 5

if ($OutFile) {
    $outputPath = Resolve-Path -LiteralPath (Split-Path -Parent $OutFile) -ErrorAction SilentlyContinue
    if (-not $outputPath) {
        New-Item -ItemType Directory -Path (Split-Path -Parent $OutFile) -Force | Out-Null
    }

    $json | Set-Content -LiteralPath $OutFile -Encoding UTF8
}

$json

if ($FailOnHit -and ($results | Where-Object { -not $_.Clean })) {
    exit 1
}
