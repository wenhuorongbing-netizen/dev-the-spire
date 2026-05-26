Set-StrictMode -Version 3.0

function Get-SpirePlusManifestVersion {
    param([Parameter(Mandatory = $true)][string]$RepoRoot)

    $manifestPath = Join-Path $RepoRoot 'EZMicroBalance.json'
    $manifest = Get-Content -Raw -LiteralPath $manifestPath -Encoding UTF8 | ConvertFrom-Json
    return [string]$manifest.version
}

function Get-SpirePlusPackageName {
    param([Parameter(Mandatory = $true)][string]$RepoRoot)

    return "SpirePlus-$(Get-SpirePlusManifestVersion -RepoRoot $RepoRoot)"
}

function Get-SpirePlusPackageRelativePath {
    param([Parameter(Mandatory = $true)][string]$RepoRoot)

    return "publish\$(Get-SpirePlusPackageName -RepoRoot $RepoRoot).zip"
}

function Resolve-SpirePlusPackagePath {
    param(
        [Parameter(Mandatory = $true)][string]$RepoRoot,
        [string]$PackagePath
    )

    $path = if ([string]::IsNullOrWhiteSpace($PackagePath)) {
        Get-SpirePlusPackageRelativePath -RepoRoot $RepoRoot
    } else {
        $PackagePath
    }

    if ([System.IO.Path]::IsPathRooted($path)) {
        return [System.IO.Path]::GetFullPath($path)
    }

    return [System.IO.Path]::GetFullPath((Join-Path $RepoRoot $path))
}

function Get-SpirePlusFileSha256 {
    param([Parameter(Mandatory = $true)][string]$Path)

    for ($attempt = 1; $attempt -le 5; $attempt++) {
        try {
            return (Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash.ToUpperInvariant()
        } catch {
            if ($attempt -eq 5) {
                throw
            }

            Start-Sleep -Milliseconds (100 * $attempt)
        }
    }
}

function Get-SpirePlusPackageSha256 {
    param(
        [Parameter(Mandatory = $true)][string]$RepoRoot,
        [string]$PackagePath
    )

    $packageFullPath = Resolve-SpirePlusPackagePath -RepoRoot $RepoRoot -PackagePath $PackagePath
    if (-not (Test-Path -LiteralPath $packageFullPath -PathType Leaf)) {
        throw "Spire Plus package zip not found: $packageFullPath"
    }

    return Get-SpirePlusFileSha256 -Path $packageFullPath
}

function Get-SpirePlusPackageArtifactRelativePaths {
    param(
        [Parameter(Mandatory = $true)][string]$RepoRoot,
        [string]$PackagePath
    )

    $packageName = Get-SpirePlusPackageName -RepoRoot $RepoRoot
    $zipPath = if ([string]::IsNullOrWhiteSpace($PackagePath)) {
        "publish\$packageName.zip"
    } else {
        $PackagePath
    }

    return @(
        $zipPath
        "publish\$packageName\EZMicroBalance\EZMicroBalance.dll"
        "publish\$packageName\EZMicroBalance\EZMicroBalance.pck"
        "publish\$packageName\EZMicroBalance\EZMicroBalance.json"
        "publish\$packageName\EZMicroBalance\README_INSTALL.txt"
    )
}
