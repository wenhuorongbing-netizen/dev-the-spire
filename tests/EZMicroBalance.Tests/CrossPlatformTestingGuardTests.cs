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
        var bashPreferredChecker = ReadRepoText("scripts", "check-installed-spire-plus-package.sh");
        var powershellPreferredChecker = ReadRepoText("scripts", "check-installed-spire-plus-package.ps1");

        AssertSourceContains(
            platformTesting,
            "Use `docs/private-beta-verification-handoff.md` as the current hash source.",
            "EZMicroBalance/EZMicroBalance.dll",
            "EZMicroBalance/EZMicroBalance.json",
            "EZMicroBalance/EZMicroBalance.pck",
            "EZMicroBalance/README_INSTALL.txt",
            "It must not include duplicate runtime dependency DLLs such as `BaseLib.dll`, `0Harmony.dll`, or `sts2.dll`.",
            "$env:STS2_PATH='D:\\Steam\\steamapps\\common\\Slay the Spire 2'",
            "Get-FileHash -LiteralPath .\\publish\\SpirePlus-v0.1.0-private-beta.14.zip -Algorithm SHA256",
            "Expand-Archive -LiteralPath .\\publish\\SpirePlus-v0.1.0-private-beta.14.zip -DestinationPath .\\publish\\inspect -Force",
            "$env:APPDATA\\SlayTheSpire2\\logs\\godot.log",
            "$env:SPIREPLUS_DISABLE_MORVI='1'",
            "export STS2_PATH=\"$HOME/Library/Application Support/Steam/steamapps/common/Slay the Spire 2\"",
            "scripts/check-installed-spire-plus-package.sh \"$STS2_PATH/mods/EZMicroBalance\"",
            "shasum -a 256 publish/SpirePlus-v0.1.0-private-beta.14.zip",
            "unzip -q publish/SpirePlus-v0.1.0-private-beta.14.zip -d publish/inspect",
            "$HOME/Library/Application Support/SlayTheSpire2/logs/godot.log",
            "SPIREPLUS_DISABLE_MORVI=1",
            "BaseLib version and installed `mods/BaseLib` folder presence",
            "loaded mod list and any ModelDb mismatch diagnostics",
            "Sere Talon imported texture entries are present in the installed PCK",
            "loader parity on another machine or OS");

        Assert.Contains("platform-testing.md", docsIndex, StringComparison.Ordinal);
        Assert.Contains("check-installed-spire-plus-package.sh", scriptsReadme, StringComparison.Ordinal);
        Assert.Contains("check-installed-spire-plus-package.ps1", scriptsReadme, StringComparison.Ordinal);
        Assert.Contains("pass `-ModDirectory` or `-GameRootZipPath` explicitly", scriptsReadme, StringComparison.Ordinal);
        Assert.Contains("-SkipGameRootZipCheck", scriptsReadme, StringComparison.Ordinal);

        AssertSourceContains(
            bashPreferredChecker,
            "#!/usr/bin/env sh",
            "set -eu",
            "shasum -a 256",
            "sha256sum",
            "check_pck_contains",
            "check_pck_absent",
            "check_sere_talon_imported_textures",
            "docs/private-beta-verification-handoff.md",
            "expected_hash()",
            "check_file \"EZMicroBalance.dll\" \"DLL\"",
            "check_file \"EZMicroBalance.json\" \"Manifest\"",
            "check_file \"EZMicroBalance.pck\" \"PCK\"",
            "check_file \"README_INSTALL.txt\" \"README_INSTALL\"",
            "\\\"SERE_TALON.title\\\": \\\"Vakuu's Sere Talon\\\"",
            "\\\"SERE_TALON.title\\\": \\\"\u74e6\u5e93\u539f\u521d\u4e4b\u722a\\\"",
            "\\\"SERE_TALON.description\\\": \\\"\u62fe\u53d6\u65f6\uff0c\u4ece[blue]4[/blue]\u5f20\u8bc5\u5492\u4e2d\u9009\u62e9[blue]1[/blue]\u5f20\u3002\u5c06\u5b83\u3001[blue]2[/blue]\u5f20[gold]\u8bb8\u613f[/gold]\u548c[blue]1[/blue]\u5f20[gold]\u8bb8\u613f+[/gold]\u52a0\u5165\u4f60\u7684\u724c\u7ec4\u3002\\\"",
            "\\\"CLAWS.title\\\": \\\"Tanx Claws\\\"",
            "\\\"CLAWS.title\\\": \\\"\u5766\u514b\u65af\u5229\u722a\\\"",
            "cards into upgraded Maul",
            "\u6495\u54ac+",
            "\\\"CLAWS.title\\\": \\\"\u5229\u722a\\\"",
            "sere_talon_spire_plus.png",
            "EZMicroBalance/images/relics/sere_talon_spire_plus.png.import",
            "EZMicroBalance/images/relics/big/sere_talon_spire_plus.png.import",
            "Sere Talon imported small/big textures",
            "Sere Talon imported textures, and Sere Talon / Tanx Claws PCK content match handoff.");

        AssertSourceContains(
            powershellPreferredChecker,
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
            "'README_INSTALL.txt' = 'README_INSTALL'",
            "requiredPckFragments",
            "forbiddenPckFragments",
            "\"SERE_TALON.title\": \"Vakuu''s Sere Talon\"",
            "\"CLAWS.title\": \"Tanx Claws\"",
            "ConvertFrom-CodePoints",
            "$sereTalonZhsTitle",
            "$sereTalonZhsEffect",
            "$tanxClawsZhsTitle",
            "$tanxClawsZhsEffect",
            "$staleSereTalonBaseClawsTitle",
            "$staleTanxBaseClawsTitle",
            "0x74E6, 0x5E93, 0x539F, 0x521D, 0x4E4B, 0x722A",
            "0x5766, 0x514B, 0x65AF, 0x5229, 0x722A",
            "sere_talon_spire_plus.png",
            "EZMicroBalance/images/relics/sere_talon_spire_plus.png.import",
            "EZMicroBalance/images/relics/big/sere_talon_spire_plus.png.import",
            "Sere Talon imported small/big textures",
            "Sere Talon imported textures, and Sere Talon / Tanx Claws PCK content match handoff");

        AssertSourceContains(
            bashChecker,
            "#!/usr/bin/env sh",
            "set -eu",
            "check-installed-spire-plus-package.sh",
            "\"$@\"");

        AssertSourceContains(
            powershellChecker,
            "check-installed-spire-plus-package.ps1",
            "& $preferredChecker @PSBoundParameters",
            "exit $LASTEXITCODE");
    }
}
