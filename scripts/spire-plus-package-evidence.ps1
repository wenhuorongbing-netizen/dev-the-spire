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
