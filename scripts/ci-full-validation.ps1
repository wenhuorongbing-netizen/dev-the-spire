param(
    [string]$Sts2Path = $env:STS2_PATH,
    [string]$GodotPath = $env:GODOT_PATH
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path

function Assert-ExistingFile {
    param(
        [Parameter(Mandatory = $true)] [string]$Path,
        [Parameter(Mandatory = $true)] [string]$Message
    )

    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        throw $Message
    }
}

function Assert-ExistingDirectory {
    param(
        [Parameter(Mandatory = $true)] [string]$Path,
        [Parameter(Mandatory = $true)] [string]$Message
    )

    if (-not (Test-Path -LiteralPath $Path -PathType Container)) {
        throw $Message
    }
}

function Invoke-Step {
    param(
        [Parameter(Mandatory = $true)] [string]$Name,
        [Parameter(Mandatory = $true)] [scriptblock]$Script
    )

    Write-Host "==> $Name"
    & $Script
}

if ([string]::IsNullOrWhiteSpace($Sts2Path)) {
    throw 'STS2_PATH or -Sts2Path is required for full validation.'
}

if ([string]::IsNullOrWhiteSpace($GodotPath)) {
    throw 'GODOT_PATH or -GodotPath is required for publish/package validation.'
}

$sts2FullPath = [System.IO.Path]::GetFullPath($Sts2Path)
$godotFullPath = [System.IO.Path]::GetFullPath($GodotPath)
$sts2DataDir = Join-Path $sts2FullPath 'data_sts2_windows_x86_64'
$baseLibDir = Join-Path $sts2FullPath 'mods\BaseLib'

Assert-ExistingDirectory $sts2FullPath "Slay the Spire 2 root not found: $sts2FullPath"
Assert-ExistingDirectory $sts2DataDir "Slay the Spire 2 Windows data dir not found: $sts2DataDir"
Assert-ExistingFile (Join-Path $sts2DataDir 'sts2.dll') "Missing sts2.dll under $sts2DataDir"
Assert-ExistingFile (Join-Path $sts2DataDir '0Harmony.dll') "Missing 0Harmony.dll under $sts2DataDir"
Assert-ExistingDirectory $baseLibDir "BaseLib runtime mod directory not found: $baseLibDir"
Assert-ExistingFile (Join-Path $baseLibDir 'BaseLib.dll') "Missing BaseLib.dll under $baseLibDir"
Assert-ExistingFile $godotFullPath "Godot executable not found: $godotFullPath"

$env:STS2_PATH = $sts2FullPath
$env:GODOT_PATH = $godotFullPath
$msbuildProps = @(
    "/p:Sts2Path=$sts2FullPath",
    "/p:GodotPath=$godotFullPath"
)
$localPropsPath = Join-Path $repoRoot 'Directory.Build.props'
$createdLocalProps = $false

if (-not (Test-Path -LiteralPath $localPropsPath)) {
    $escapedSts2Path = [System.Security.SecurityElement]::Escape($sts2FullPath)
    $escapedGodotPath = [System.Security.SecurityElement]::Escape($godotFullPath)
    @"
<Project>
    <PropertyGroup>
        <GodotPath>$escapedGodotPath</GodotPath>
        <Sts2Path>$escapedSts2Path</Sts2Path>
    </PropertyGroup>
</Project>
"@ | Set-Content -LiteralPath $localPropsPath -Encoding UTF8
    $createdLocalProps = $true
    Write-Host "Created temporary Directory.Build.props for this validation run."
}

Push-Location $repoRoot
try {
    Invoke-Step 'Repository hygiene' {
        & .\scripts\validate-repository-hygiene.ps1
    }

    Invoke-Step 'Spire Plus build' {
        dotnet build EZMicroBalance.sln @msbuildProps
    }

    Invoke-Step 'Spire Plus tests' {
        dotnet test EZMicroBalance.sln --no-build
    }

    Invoke-Step 'Spire Plus format check' {
        dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
    }

    Invoke-Step 'Diff whitespace check' {
        $previousErrorActionPreference = $ErrorActionPreference
        $ErrorActionPreference = 'Continue'
        try {
            $diffCheckOutput = & git diff --check 2>&1
            $diffCheckExitCode = $LASTEXITCODE
        }
        finally {
            $ErrorActionPreference = $previousErrorActionPreference
        }

        if ($diffCheckOutput) {
            $diffCheckOutput | ForEach-Object { Write-Host $_ }
        }

        if ($diffCheckExitCode -ne 0) {
            throw "git diff --check failed with exit code $diffCheckExitCode"
        }
    }

    Invoke-Step 'Spire Plus publish' {
        dotnet publish EZMicroBalance.sln @msbuildProps
    }

    Invoke-Step 'Spire Plus package' {
        & .\scripts\package-spire-plus.ps1 -GameRoot $sts2FullPath
    }

    Invoke-Step 'Spire Plus artifact tests' {
        $env:SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS = '1'
        try {
            dotnet test EZMicroBalance.sln --no-build
        }
        finally {
            Remove-Item Env:\SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS -ErrorAction SilentlyContinue
        }
    }
}
finally {
    Pop-Location
    if ($createdLocalProps -and (Test-Path -LiteralPath $localPropsPath)) {
        Remove-Item -LiteralPath $localPropsPath -Force
        Write-Host "Removed temporary Directory.Build.props."
    }
}
