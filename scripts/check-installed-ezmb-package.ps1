param(
    [string]$ModDirectory,
    [string]$GameRootZipPath,
    [string]$HandoffPath = "$PSScriptRoot\..\docs\private-beta-verification-handoff.md",
    [switch]$SkipGameRootZipCheck,
    [switch]$PassVerbose
)

$preferredChecker = Join-Path $PSScriptRoot 'check-installed-spire-plus-package.ps1'
& $preferredChecker @PSBoundParameters
exit $LASTEXITCODE
