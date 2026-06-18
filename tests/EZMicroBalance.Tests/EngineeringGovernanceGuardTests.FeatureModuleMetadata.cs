using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class EngineeringGovernanceGuardTests
{
    [Fact]
    public void AllFeatureModulesHaveNonEmptyDisplayName()
    {
        var moduleDisplayNames = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Ancients.Lotha"] = "Lotha Ancient",
            ["Ancients.Morvi"] = "Morvi Ancient",
            ["Ancients.Urda"] = "Urda Ancient",
            ["Ancients.VakuuFight"] = "Vakuu Fight",
            ["Ascension.A11A20"] = "Ascension 11-20",
            ["Sts1Events"] = "StS1 Event Port",
        };

        var moduleFiles = new (string Id, string Path)[]
        {
            ("Ancients.Lotha", RepoPath("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaFeatureModule.cs")),
            ("Ancients.Morvi", RepoPath("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviFeatureModule.cs")),
            ("Ancients.Urda", RepoPath("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaFeatureModule.cs")),
            ("Ancients.VakuuFight", RepoPath("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightFeatureModule.cs")),
            ("Ascension.A11A20", RepoPath("EZMicroBalanceCode", "Ascension", "Core", "AscensionFeatureModule.cs")),
            ("Sts1Events", RepoPath("EZMicroBalanceCode", "Sts1Events", "Sts1EventsFeatureModule.cs")),
        };

        foreach (var (id, filePath) in moduleFiles)
        {
            Assert.True(File.Exists(filePath), $"Feature module file not found: {filePath}");
            var source = File.ReadAllText(filePath);

            // Must declare DisplayName
            AssertSourceContains(source, "string DisplayName =>");

            // Extract the DisplayName value - must match a quoted non-empty string
            var match = System.Text.RegularExpressions.Regex.Match(
                source, @"string\s+DisplayName\s+=>\s*""(?<name>[^""]+)""");
            Assert.True(match.Success, $"Module {id}: DisplayName must return a quoted string literal. File: {filePath}");
            var displayName = match.Groups["name"].Value;
            Assert.False(string.IsNullOrWhiteSpace(displayName), $"Module {id}: DisplayName must not be empty.");
            Assert.True(
                moduleDisplayNames.TryGetValue(id, out var expected) && displayName == expected,
                $"Module {id}: expected DisplayName '{expected}' but got '{displayName}'.");
        }
    }

    [Fact]
    public void AllFeatureModulesHaveValidCategory()
    {
        var validCategories = new HashSet<string>(StringComparer.Ordinal)
        {
            "Ancients",
            "Ascension",
            "Events",
            "General",
            "Preview",
            "Diagnostics",
        };

        var moduleFiles = new (string Id, string Path)[]
        {
            ("Ancients.Lotha", RepoPath("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaFeatureModule.cs")),
            ("Ancients.Morvi", RepoPath("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviFeatureModule.cs")),
            ("Ancients.Urda", RepoPath("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaFeatureModule.cs")),
            ("Ancients.VakuuFight", RepoPath("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightFeatureModule.cs")),
            ("Ascension.A11A20", RepoPath("EZMicroBalanceCode", "Ascension", "Core", "AscensionFeatureModule.cs")),
            ("Sts1Events", RepoPath("EZMicroBalanceCode", "Sts1Events", "Sts1EventsFeatureModule.cs")),
        };

        foreach (var (id, filePath) in moduleFiles)
        {
            Assert.True(File.Exists(filePath), $"Feature module file not found: {filePath}");
            var source = File.ReadAllText(filePath);

            AssertSourceContains(source, "string Category =>");

            var match = System.Text.RegularExpressions.Regex.Match(
                source, @"string\s+Category\s+=>\s*""(?<cat>[^""]+)""");
            Assert.True(match.Success, $"Module {id}: Category must return a quoted string literal. File: {filePath}");
            var category = match.Groups["cat"].Value;
            Assert.False(string.IsNullOrWhiteSpace(category), $"Module {id}: Category must not be empty.");
            Assert.True(validCategories.Contains(category), $"Module {id}: Category '{category}' is not in the recognized set.");
        }
    }
}
