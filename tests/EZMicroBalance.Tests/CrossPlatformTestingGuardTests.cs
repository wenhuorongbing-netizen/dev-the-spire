using Xunit;

namespace EZMicroBalance.Tests;

public sealed class CrossPlatformTestingGuardTests
{
    [Fact]
    public void PlatformTestingDocsCoverWindowsMacHashesLogsAndEnvironmentVariables()
    {
        var platformTesting = ReadRepoText("docs", "platform-testing.md");
        var docsIndex = ReadRepoText("docs", "README.md");
        var scriptsReadme = ReadRepoText("scripts", "README.md");
        var bashChecker = ReadRepoText("scripts", "check-installed-ezmb-package.sh");
        var powershellChecker = ReadRepoText("scripts", "check-installed-ezmb-package.ps1");

        AssertSourceContains(
            platformTesting,
            "Use `docs/private-beta-verification-handoff.md` as the current hash source.",
            "EZMicroBalance/EZMicroBalance.dll",
            "EZMicroBalance/EZMicroBalance.json",
            "EZMicroBalance/EZMicroBalance.pck",
            "EZMicroBalance/README_INSTALL.txt",
            "It must not include duplicate runtime dependency DLLs such as `BaseLib.dll`, `0Harmony.dll`, or `sts2.dll`.",
            "$env:STS2_PATH='D:\\Steam\\steamapps\\common\\Slay the Spire 2'",
            "Get-FileHash -LiteralPath .\\publish\\SpirePlus-v0.1.0-private-beta.0.zip -Algorithm SHA256",
            "Expand-Archive -LiteralPath .\\publish\\SpirePlus-v0.1.0-private-beta.0.zip -DestinationPath .\\publish\\inspect -Force",
            "$env:APPDATA\\SlayTheSpire2\\logs\\godot.log",
            "$env:EZMB_DISABLE_MORVI='1'",
            "export STS2_PATH=\"$HOME/Library/Application Support/Steam/steamapps/common/Slay the Spire 2\"",
            "scripts/check-installed-ezmb-package.sh \"$STS2_PATH/mods/EZMicroBalance\"",
            "shasum -a 256 publish/SpirePlus-v0.1.0-private-beta.0.zip",
            "unzip -q publish/SpirePlus-v0.1.0-private-beta.0.zip -d publish/inspect",
            "$HOME/Library/Application Support/SlayTheSpire2/logs/godot.log",
            "EZMB_DISABLE_MORVI=1",
            "BaseLib version and installed `mods/BaseLib` folder presence",
            "loaded mod list and any ModelDb mismatch diagnostics",
            "Passing the hash checks only proves that the same files are installed.");

        Assert.Contains("platform-testing.md", docsIndex, StringComparison.Ordinal);
        Assert.Contains("check-installed-ezmb-package.sh", scriptsReadme, StringComparison.Ordinal);
        Assert.Contains("pass `-ModDirectory` or `-GameRootZipPath` explicitly", scriptsReadme, StringComparison.Ordinal);
        Assert.Contains("-SkipGameRootZipCheck", scriptsReadme, StringComparison.Ordinal);

        AssertSourceContains(
            bashChecker,
            "#!/usr/bin/env sh",
            "set -eu",
            "shasum -a 256",
            "sha256sum",
            "docs/private-beta-verification-handoff.md",
            "expected_hash()",
            "check_file \"EZMicroBalance.dll\" \"DLL\"",
            "check_file \"EZMicroBalance.json\" \"Manifest\"",
            "check_file \"EZMicroBalance.pck\" \"PCK\"",
            "check_file \"README_INSTALL.txt\" \"README_INSTALL\"",
            "PASS: installed EZMicroBalance artifacts match handoff hashes.");

        AssertSourceContains(
            powershellChecker,
            "[string]$ModDirectory",
            "[string]$GameRootZipPath",
            "[switch]$SkipGameRootZipCheck",
            "Get-HandoffPackageFileName",
            "Get-ExpectedHash 'Zip'",
            "Game root package zip",
            "Get-FileHash -Algorithm SHA256",
            "'EZMicroBalance.dll' = 'DLL'",
            "'EZMicroBalance.json' = 'JSON'",
            "'EZMicroBalance.pck' = 'PCK'",
            "'README_INSTALL.txt' = 'README_INSTALL'");
    }
}
