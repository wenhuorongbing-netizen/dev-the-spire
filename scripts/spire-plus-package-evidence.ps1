Set-StrictMode -Version 3.0

function Get-SpirePlusGitValue {
    param(
        [Parameter(Mandatory = $true)][string]$RepoRoot,
        [Parameter(Mandatory = $true)][string[]]$Arguments
    )

    try {
        $value = & git -C $RepoRoot @Arguments 2>$null
        if ($LASTEXITCODE -eq 0) {
            return ($value -join "`n").Trim()
        }
    } catch {
    }

    return $null
}

function Get-SpirePlusGitEvidence {
    param([Parameter(Mandatory = $true)][string]$RepoRoot)

    $head = Get-SpirePlusGitValue -RepoRoot $RepoRoot -Arguments @('rev-parse', 'HEAD')
    $upstream = Get-SpirePlusGitValue -RepoRoot $RepoRoot -Arguments @('rev-parse', '--abbrev-ref', '--symbolic-full-name', '@{u}')
    $upstreamHead = if ([string]::IsNullOrWhiteSpace($upstream)) {
        $null
    } else {
        Get-SpirePlusGitValue -RepoRoot $RepoRoot -Arguments @('rev-parse', $upstream)
    }

    $headMatchesUpstream = -not [string]::IsNullOrWhiteSpace($head) -and
        -not [string]::IsNullOrWhiteSpace($upstreamHead) -and
        [System.StringComparer]::OrdinalIgnoreCase.Equals($head, $upstreamHead)

    return [ordered]@{
        Head = $head
        HeadShort = Get-SpirePlusGitValue -RepoRoot $RepoRoot -Arguments @('rev-parse', '--short', 'HEAD')
        LatestCommit = Get-SpirePlusGitValue -RepoRoot $RepoRoot -Arguments @('log', '-1', '--oneline', '--decorate')
        Branch = Get-SpirePlusGitValue -RepoRoot $RepoRoot -Arguments @('branch', '--show-current')
        StatusShort = Get-SpirePlusGitValue -RepoRoot $RepoRoot -Arguments @('status', '--short')
        BranchStatus = Get-SpirePlusGitValue -RepoRoot $RepoRoot -Arguments @('status', '--short', '--branch')
        Upstream = $upstream
        UpstreamHead = $upstreamHead
        PushedHead = $upstreamHead
        HeadMatchesUpstream = $headMatchesUpstream
    }
}

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
