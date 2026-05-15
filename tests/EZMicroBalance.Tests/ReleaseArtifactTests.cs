using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class ReleaseArtifactTests
{
    private static readonly string[] BannedSimplifiedChineseEnglishTerms =
    [
        "Swift",
        "Apotheosis",
        "Enthralled",
        "Wish",
        "Relax",
        "Folly",
        "Debt",
        "Boss",
        "Retain",
        "Ethereal",
        "Exhaust",
        "Innate",
        "Eternal",
        "Strength",
        "Power",
        "Attack",
        "Rare",
        "Cook",
        "off-color"
    ];

    private static readonly HashSet<string> SimplifiedChineseEnglishTermWhitelist = new(StringComparer.OrdinalIgnoreCase)
    {
        // Intentionally empty for this release: no raw English terms are approved in zhs player-facing text.
    };

    [Fact]
    public void ActiveManifestHasStableReleaseIdentity()
    {
        using var active = JsonDocument.Parse(File.ReadAllText(RepoPath("EZMicroBalance.json")));
        using var legacy = JsonDocument.Parse(File.ReadAllText(RepoPath("EzDailyContent.json")));

        Assert.Equal("EZMicroBalance", active.RootElement.GetProperty("id").GetString());
        Assert.Equal("Spire Plus", active.RootElement.GetProperty("name").GetString());
        Assert.True(active.RootElement.GetProperty("has_dll").GetBoolean());
        Assert.True(active.RootElement.GetProperty("has_pck").GetBoolean());
        Assert.True(active.RootElement.GetProperty("affects_gameplay").GetBoolean());
        Assert.Contains(
            active.RootElement.GetProperty("dependencies").EnumerateArray(),
            dependency => dependency.ValueKind == JsonValueKind.Object &&
                dependency.TryGetProperty("id", out var id) &&
                id.GetString() == "BaseLib" &&
                dependency.TryGetProperty("min_version", out var minVersion) &&
                minVersion.GetString() == "v3.1.2");

        Assert.Equal("EzDailyContent", legacy.RootElement.GetProperty("id").GetString());
    }

    [ReleaseArtifactFact]
    public void ActiveReleaseArtMatchesAuditedNoTextNoLogoAsset()
    {
        var activeArt = RepoPath("EZMicroBalance", "mod_image.png");
        var sourceCopy = RepoPath("publish", "EZMicroBalance-cover-source.png");
        var devEnvironment = ReadRepoText("docs", "dev-environment.md");
        var releaseChecklist = ReadRepoText("docs", "release-checklist.md");

        Assert.True(File.Exists(activeArt), $"Missing active release art: {activeArt}");
        Assert.True(File.Exists(sourceCopy), $"Missing audited source art copy: {sourceCopy}");
        var expectedArtHash = Sha256(activeArt);
        Assert.Equal(expectedArtHash, Sha256(activeArt));
        Assert.Equal(expectedArtHash, Sha256(sourceCopy));

        var (width, height) = ReadPngDimensions(activeArt);
        Assert.Equal(width, height);
        Assert.True(width >= 512, $"Release art should stay readable as an icon; actual width was {width}px.");

        Assert.Contains(expectedArtHash, devEnvironment, StringComparison.Ordinal);
        Assert.Contains("no visible text, letters, numbers, numerals, logos, or official game assets", devEnvironment, StringComparison.Ordinal);
        Assert.Contains("no text, numbers, logos, or official game assets", releaseChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("3322EB2DAFFC7807C4FC797B641994E54C66C89AA74922577DD29BB36124AD4B", devEnvironment, StringComparison.Ordinal);

        var pngBytesAsText = Encoding.Latin1.GetString(File.ReadAllBytes(activeArt));
        Assert.DoesNotContain("EZ MICRO", pngBytesAsText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("BALANCE", pngBytesAsText, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void LocalizationJsonIsValidUtf8AndKeyCompatible()
    {
        var localizationRoot = RepoPath("EZMicroBalance", "localization");
        var files = Directory.GetFiles(localizationRoot, "*.json", SearchOption.AllDirectories);
        Assert.NotEmpty(files);

        foreach (var file in files)
        {
            using var stream = File.OpenRead(file);
            using var _ = JsonDocument.Parse(stream);
        }

        AssertSameKeys("cards.json");
        AssertSameKeys("relics.json");
        AssertSameKeys("rest_site_ui.json");
    }

    [Fact]
    public void SimplifiedChinesePlayerTextContainsNoBannedEnglishLeftovers()
    {
        var zhsRoot = RepoPath("EZMicroBalance", "localization", "zhs");
        var files = Directory.GetFiles(zhsRoot, "*.json", SearchOption.AllDirectories);
        var failures = new List<string>();

        foreach (var file in files)
        {
            using var document = JsonDocument.Parse(File.ReadAllText(file, Encoding.UTF8));
            foreach (var (key, value) in JsonStringValues(document.RootElement))
            {
                var visibleValue = RemoveDynamicVarPlaceholders(value);
                foreach (var bannedTerm in BannedSimplifiedChineseEnglishTerms)
                {
                    if (SimplifiedChineseEnglishTermWhitelist.Contains(bannedTerm))
                    {
                        continue;
                    }

                    if (ContainsEnglishTerm(visibleValue, bannedTerm))
                    {
                        var relativeFile = Path.GetRelativePath(zhsRoot, file);
                        failures.Add($"{relativeFile}:{key} contains `{bannedTerm}` in `{value}`");
                    }
                }
            }
        }

        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void BeautifulBraceletUsesLocalizedSwiftTermInSimplifiedChinese()
    {
        using var relics = JsonDocument.Parse(ReadRepoText("EZMicroBalance", "localization", "zhs", "relics.json"));
        var description = relics.RootElement.GetProperty("BEAUTIFUL_BRACELET.description").GetString();

        Assert.Contains("迅速", description, StringComparison.Ordinal);
        Assert.DoesNotContain("Swift", description, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void JeweledMaskCustomEnchantmentHasSimplifiedChineseLocalization()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "JeweledMaskFreePower.cs");

        Assert.Contains("LocManager.Instance.Language == \"zhs\"", source, StringComparison.Ordinal);
        Assert.Contains("宝石面具", source, StringComparison.Ordinal);
        Assert.Contains("这张牌的费用已被宝石面具永久设为0。", source, StringComparison.Ordinal);
        Assert.Contains("来自宝石面具，费用为0。", source, StringComparison.Ordinal);
    }
    [Fact]
    public void JewelryBoxApotheosisNonInnateMarkerIsInstanceScopedAndSerializable()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "VakuRewardPatches.cs");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");

        Assert.Contains("HarmonyPatch(typeof(Apotheosis), \"get_CanonicalKeywords\")", source, StringComparison.Ordinal);
        Assert.Contains("JewelryBoxApotheosisMarker.IsMarked(__instance)", source, StringComparison.Ordinal);
        Assert.Contains("keyword => keyword != CardKeyword.Innate", source, StringComparison.Ordinal);
        Assert.Contains("JewelryBoxApotheosisMarker.Mark(card)", source, StringComparison.Ordinal);
        Assert.Contains("JewelryBoxApotheosisMarker.Mark(result.cardAdded)", source, StringComparison.Ordinal);
        Assert.Contains("SavedSpireField<CardModel, bool>", ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientSavedStateFields.cs"), StringComparison.Ordinal);
        Assert.Contains("AncientSavedStateFields.JewelryBoxNonInnateApotheosis[card] = true", source, StringComparison.Ordinal);
        Assert.Contains("AncientSavedStateFields.JewelryBoxNonInnateApotheosis[card]", source, StringComparison.Ordinal);

        Assert.Contains("enter the next combat", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("must not start in opening hand from Innate", manualMatrix, StringComparison.Ordinal);
    }

    [ReleaseArtifactFact]
    public void PublishedPckContainsOnlyActiveReleaseResources()
    {
        var pckPath = GamePath("mods", "EZMicroBalance", "EZMicroBalance.pck");
        Assert.True(File.Exists(pckPath), $"Missing published PCK: {pckPath}");

        var entries = ReadPckDirectory(pckPath);
        var activeEntries = entries
            .Where(entry => !entry.StartsWith(".godot/", StringComparison.Ordinal) &&
                            !entry.Equals("project.binary", StringComparison.Ordinal) &&
                            !entry.Equals(".import", StringComparison.Ordinal))
            .ToArray();

        var exportedEntries = ParseExportFiles(ReadRepoText("export_presets.cfg"))
            .Select(path => path["res://".Length..])
            .Where(entry => entry.StartsWith("EZMicroBalance/", StringComparison.Ordinal))
            .Concat(
                Directory.GetFiles(RepoPath("EZMicroBalance"), "*", SearchOption.AllDirectories)
                    .Select(path => ToRepoRelativePath(path))
                    .Where(IsActiveExportResource))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(entry => entry, StringComparer.Ordinal)
            .ToArray();
        var expectedPckEntries = GetExportedPckEntries(exportedEntries, activeEntries)
            .Concat(new[] { "EZMicroBalance.json" })
            .OrderBy(entry => entry, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(expectedPckEntries, activeEntries.OrderBy(entry => entry, StringComparer.Ordinal));
        Assert.DoesNotContain(activeEntries, entry =>
            entry.StartsWith("EzDailyContent", StringComparison.Ordinal) ||
            entry.StartsWith("EZMicroBalanceCode", StringComparison.Ordinal) ||
            entry.StartsWith("docs", StringComparison.Ordinal) ||
            entry.StartsWith("art_pipeline", StringComparison.Ordinal) ||
            entry.StartsWith("asset", StringComparison.Ordinal) ||
            entry.StartsWith("legacy", StringComparison.Ordinal));

        Assert.Contains("EZMicroBalance.json", entries);
        Assert.Contains("EZMicroBalance/mod_image.png", entries);
        Assert.Contains("EZMicroBalance/mod_image.png.import", entries);
        Assert.Contains("EZMicroBalance/images/ascension/firemarked_elite_indicator.png.import", entries);
        Assert.Contains("EZMicroBalance/images/ascension/banner_room_indicator.png.import", entries);
        Assert.Contains("EZMicroBalance/images/ascension/boss_seal_indicator.png.import", entries);
        Assert.Contains("EZMicroBalance/images/ascension/firemark_might_indicator.png.import", entries);
        Assert.Contains("EZMicroBalance/images/ascension/firemark_giant_indicator.png.import", entries);
        Assert.Contains("EZMicroBalance/images/ascension/firemark_forge_armor_indicator.png.import", entries);
        Assert.Contains("EZMicroBalance/images/ascension/firemark_constant_heal_indicator.png.import", entries);
        Assert.Contains("EZMicroBalance/images/ascension/banner_vanguard_indicator.png.import", entries);
        Assert.Contains("EZMicroBalance/images/ascension/banner_shield_formation_indicator.png.import", entries);
        Assert.Contains("EZMicroBalance/images/ascension/banner_bounty_indicator.png.import", entries);
        Assert.Contains("EZMicroBalance/images/ascension/fission_enchantment_icon.png.import", entries);
        Assert.Contains("EZMicroBalance/images/ascension/forge_token_status.png.import", entries);
        Assert.Contains("EZMicroBalance/images/ancients/urda/ezmb_urda_map_icon.png.import", entries);
        Assert.Contains("EZMicroBalance/images/ancients/urda/ezmb_urda_map_icon_outline.png.import", entries);
        Assert.Contains("EZMicroBalance/images/ancients/urda/ezmb_urda_run_history_icon.png.import", entries);
        Assert.Contains("EZMicroBalance/images/ancients/urda/ezmb_urda_run_history_icon_outline.png.import", entries);
        Assert.Contains("EZMicroBalance/images/ancients/urda/options/urda_seedbed.png.import", entries);
        Assert.Contains("EZMicroBalance/images/ancients/urda/options/urda_humus_pact.png.import", entries);
        Assert.Contains("EZMicroBalance/images/ancients/urda/options/urda_molting.png.import", entries);
        Assert.Contains("EZMicroBalance/images/ancients/urda/options/urda_moss_map.png.import", entries);
        Assert.Contains("EZMicroBalance/images/ancients/urda/options/urda_trial_branch.png.import", entries);
        Assert.Contains("EZMicroBalance/images/ancients/urda/options/urda_shallow_root_relic.png.import", entries);
        Assert.Contains("EZMicroBalance/images/ancients/urda/options/urda_rooted_route.png.import", entries);
        Assert.Contains("EZMicroBalance/images/ancients/urda/options/urda_after_rain.png.import", entries);
        Assert.Contains("EZMicroBalance/images/ancients/urda/options/urda_root_sight.png.import", entries);
        Assert.Contains("EZMicroBalance/images/ancients/urda/options/urda_seed_bank.png.import", entries);
        Assert.Contains("EZMicroBalance/images/events/ezmb_urda.png.import", entries);
        Assert.Contains("EZMicroBalance/scenes/events/background_scenes/ezmb_urda.tscn.remap", entries);
        Assert.Contains("EZMicroBalance/localization/eng/relics.json", entries);
        Assert.Contains("EZMicroBalance/localization/eng/ascension.json", entries);
        Assert.Contains("EZMicroBalance/localization/eng/events.json", entries);
        Assert.Contains("EZMicroBalance/localization/eng/settings_ui.json", entries);
        Assert.Contains("EZMicroBalance/localization/zhs/relics.json", entries);
        Assert.Contains("EZMicroBalance/localization/zhs/ascension.json", entries);
        Assert.Contains("EZMicroBalance/localization/zhs/events.json", entries);
        Assert.Contains("EZMicroBalance/localization/zhs/settings_ui.json", entries);
    }

    [ReleaseArtifactFact]
    public void InstalledDllMatchesABuildOutput()
    {
        var installedDll = GamePath("mods", "EZMicroBalance", "EZMicroBalance.dll");
        var buildDlls = CandidateBuildDlls().Where(File.Exists).ToArray();

        Assert.True(File.Exists(installedDll), $"Missing installed DLL: {installedDll}");
        Assert.NotEmpty(buildDlls);

        var installedHash = Sha256(installedDll);
        Assert.Contains(buildDlls, buildDll => Sha256(buildDll) == installedHash);
    }

    [ReleaseArtifactFact]
    public void InstalledManifestMatchesRepositoryManifest()
    {
        var sourceManifest = RepoPath("EZMicroBalance.json");
        var installedManifest = GamePath("mods", "EZMicroBalance", "EZMicroBalance.json");

        Assert.True(File.Exists(sourceManifest), $"Missing source manifest: {sourceManifest}");
        Assert.True(File.Exists(installedManifest), $"Missing installed manifest: {installedManifest}");
        Assert.Equal(NormalizeJson(sourceManifest), NormalizeJson(installedManifest));
    }

    [ReleaseArtifactFact]
    public void HarmonyPatchesResolveAgainstInstalledGameApi()
    {
        var dataDir = GamePath("data_sts2_windows_x86_64");
        var baseLibDir = GamePath("mods", "BaseLib");
        var installedModDir = GamePath("mods", "EZMicroBalance");
        var searchDirs = new[] { dataDir, baseLibDir, installedModDir }
            .Concat(CandidateBuildDirectories())
            .ToArray();

        ResolveEventHandler resolver = (_, args) =>
        {
            var assemblyFileName = new AssemblyName(args.Name).Name + ".dll";
            foreach (var dir in searchDirs)
            {
                var candidate = Path.Combine(dir, assemblyFileName);
                if (File.Exists(candidate))
                {
                    return Assembly.LoadFrom(candidate);
                }
            }

            return null;
        };

        AppDomain.CurrentDomain.AssemblyResolve += resolver;
        try
        {
            Assembly.LoadFrom(Path.Combine(dataDir, "0Harmony.dll"));
            Assembly.LoadFrom(Path.Combine(dataDir, "GodotSharp.dll"));
            Assembly.LoadFrom(Path.Combine(dataDir, "sts2.dll"));
            Assembly.LoadFrom(Path.Combine(baseLibDir, "BaseLib.dll"));
            var ez = Assembly.LoadFrom(Path.Combine(installedModDir, "EZMicroBalance.dll"));

            var harmonyType = Type.GetType("HarmonyLib.Harmony, 0Harmony", throwOnError: true)!;
            var harmony = Activator.CreateInstance(harmonyType, "EZMicroBalance.test")!;
            var patchAll = harmonyType.GetMethod("PatchAll", new[] { typeof(Assembly) })!;

            var exception = Record.Exception(() => patchAll.Invoke(harmony, new object[] { ez }));
            Assert.Null(Unwrap(exception));
        }
        finally
        {
            AppDomain.CurrentDomain.AssemblyResolve -= resolver;
        }
    }

    [ReleaseArtifactFact]
    public void InstalledUrdaUsesCustomAncientAssetPaths()
    {
        var dataDir = GamePath("data_sts2_windows_x86_64");
        var baseLibDir = GamePath("mods", "BaseLib");
        var installedModDir = GamePath("mods", "EZMicroBalance");
        var searchDirs = new[] { dataDir, baseLibDir, installedModDir }
            .Concat(CandidateBuildDirectories())
            .ToArray();

        ResolveEventHandler resolver = (_, args) =>
        {
            var assemblyFileName = new AssemblyName(args.Name).Name + ".dll";
            foreach (var dir in searchDirs)
            {
                var candidate = Path.Combine(dir, assemblyFileName);
                if (File.Exists(candidate))
                {
                    return Assembly.LoadFrom(candidate);
                }
            }

            return null;
        };

        AppDomain.CurrentDomain.AssemblyResolve += resolver;
        try
        {
            Assembly.LoadFrom(Path.Combine(dataDir, "GodotSharp.dll"));
            Assembly.LoadFrom(Path.Combine(dataDir, "sts2.dll"));
            Assembly.LoadFrom(Path.Combine(baseLibDir, "BaseLib.dll"));
            var ez = Assembly.LoadFrom(Path.Combine(installedModDir, "EZMicroBalance.dll"));

            var urdaType = ez.GetType(
                "EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda.EzmbUrda",
                throwOnError: true)!;
            Assert.Equal("BaseLib.Abstracts.CustomAncientModel", urdaType.BaseType?.FullName);

            var urda = Activator.CreateInstance(urdaType, nonPublic: true)!;
            Assert.Equal("res://EZMicroBalance/images/ancients/urda/ezmb_urda_map_icon.png", GetStringProperty(urdaType, urda, "CustomMapIconPath"));
            Assert.Equal("res://EZMicroBalance/images/ancients/urda/ezmb_urda_map_icon_outline.png", GetStringProperty(urdaType, urda, "CustomMapIconOutlinePath"));
            Assert.Equal("res://EZMicroBalance/images/ancients/urda/ezmb_urda_run_history_icon.png", GetStringProperty(urdaType, urda, "CustomRunHistoryIconPath"));
            Assert.Equal("res://EZMicroBalance/images/ancients/urda/ezmb_urda_run_history_icon_outline.png", GetStringProperty(urdaType, urda, "CustomRunHistoryIconOutlinePath"));
            Assert.Equal("res://EZMicroBalance/scenes/events/background_scenes/ezmb_urda.tscn", GetStringProperty(urdaType, urda, "CustomScenePath"));

            var entries = ReadPckDirectory(Path.Combine(installedModDir, "EZMicroBalance.pck"));
            Assert.Contains("EZMicroBalance/images/ancients/urda/ezmb_urda_map_icon.png.import", entries);
            Assert.Contains("EZMicroBalance/images/ancients/urda/ezmb_urda_map_icon_outline.png.import", entries);
            Assert.Contains("EZMicroBalance/images/ancients/urda/ezmb_urda_run_history_icon.png.import", entries);
            Assert.Contains("EZMicroBalance/images/ancients/urda/ezmb_urda_run_history_icon_outline.png.import", entries);
            Assert.Contains("EZMicroBalance/images/events/ezmb_urda.png.import", entries);
            Assert.Contains("EZMicroBalance/scenes/events/background_scenes/ezmb_urda.tscn.remap", entries);
        }
        finally
        {
            AppDomain.CurrentDomain.AssemblyResolve -= resolver;
        }
    }

    [Fact]
    public void PrismaticGemRerollFixHasDocumentedEvidenceAndManualCoverage()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "PrismaticGemPatches.cs");
        var apiDiscovery = ReadRepoText("docs", "features", "ancients-rework-v4", "api-discovery.md");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");

        Assert.Contains("HarmonyPatch(typeof(CardReward), nameof(CardReward.Populate))", source, StringComparison.Ordinal);
        Assert.Contains("ConditionalWeakTable<CardReward, RewardScreenState>", source, StringComparison.Ordinal);
        Assert.Contains("PrismaticGemNormalRewardCounter[prismaticGem] + 1", source, StringComparison.Ordinal);
        Assert.Contains("for (var slotIndex = 0; slotIndex < cardRewardOptions.Count; slotIndex++)", source, StringComparison.Ordinal);
        Assert.Contains("GetOffColorRewardPool(player, originalCard.Rarity, excludedIds)", source, StringComparison.Ordinal);
        Assert.Contains("creationOptions.Source == CardCreationSource.Encounter", source, StringComparison.Ordinal);
        Assert.Contains("PRISMATIC_GEM.countHint.title", source, StringComparison.Ordinal);
        Assert.Contains("PRISMATIC_GEM.rewardScreenHint", source, StringComparison.Ordinal);
        Assert.Contains("TryGetCompatibleBannerField", source, StringComparison.Ordinal);
        Assert.Contains("typeof(MegaCrit.Sts2.Core.Nodes.CommonUi.NCommonBanner).IsAssignableFrom(BannerField.FieldType)", source, StringComparison.Ordinal);
        Assert.Contains("GetNodeOrNull<MegaCrit.Sts2.Core.Nodes.CommonUi.NCommonBanner>(BannerNodePath)", source, StringComparison.Ordinal);
        Assert.Contains("WarnOnce(", source, StringComparison.Ordinal);

        Assert.Contains("CardReward.Reroll()", apiDiscovery, StringComparison.Ordinal);
        Assert.Contains("ConditionalWeakTable<CardReward, RewardScreenState>", apiDiscovery, StringComparison.Ordinal);
        Assert.Contains("increment the saved counter once", apiDiscovery, StringComparison.Ordinal);
        Assert.Contains("Rerolls reuse the same `CardReward` state", apiDiscovery, StringComparison.Ordinal);
        Assert.Contains("Trigger screens regenerate all-slot off-color replacements", apiDiscovery, StringComparison.Ordinal);
        Assert.Contains("private `_banner` field type is runtime-guarded", apiDiscovery, StringComparison.Ordinal);
        Assert.Contains("falls back to the public `UI/Banner` node lookup", apiDiscovery, StringComparison.Ordinal);

        Assert.Contains("First Normal Reward, Reroll", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("Second Normal Reward, Reroll", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("Non-Normal Rewards", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("shop, colorless-only, and other non-normal rewards", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("every visible option is off-color", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("UI/Banner fallback", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("godot.log", manualMatrix, StringComparison.Ordinal);
    }

    [ReleaseArtifactFact]
    public void PrismaticGemRewardBannerContractMatchesInstalledGameApi()
    {
        var dataDir = GamePath("data_sts2_windows_x86_64");
        var baseLibDir = GamePath("mods", "BaseLib");
        var searchDirs = new[] { dataDir, baseLibDir };

        ResolveEventHandler resolver = (_, args) =>
        {
            var assemblyFileName = new AssemblyName(args.Name).Name + ".dll";
            foreach (var dir in searchDirs)
            {
                var candidate = Path.Combine(dir, assemblyFileName);
                if (File.Exists(candidate))
                {
                    return Assembly.LoadFrom(candidate);
                }
            }

            return null;
        };

        AppDomain.CurrentDomain.AssemblyResolve += resolver;
        try
        {
            Assembly.LoadFrom(Path.Combine(dataDir, "GodotSharp.dll"));
            var sts2 = Assembly.LoadFrom(Path.Combine(dataDir, "sts2.dll"));

            var screenType = sts2.GetType(
                "MegaCrit.Sts2.Core.Nodes.Screens.CardSelection.NCardRewardSelectionScreen",
                throwOnError: true)!;
            var bannerType = sts2.GetType(
                "MegaCrit.Sts2.Core.Nodes.CommonUi.NCommonBanner",
                throwOnError: true)!;

            var bannerField = screenType.GetField("_banner", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(bannerField);
            Assert.True(
                bannerType.IsAssignableFrom(bannerField.FieldType),
                $"Expected NCardRewardSelectionScreen._banner to be assignable to {bannerType.FullName}; actual type was {bannerField.FieldType.FullName}.");

            var changeText = bannerType.GetMethod(
                "ChangeText",
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                types: [typeof(string)],
                modifiers: null);
            Assert.NotNull(changeText);
        }
        finally
        {
            AppDomain.CurrentDomain.AssemblyResolve -= resolver;
        }
    }

    [Fact]
    public void CurrentSetupDocsPointAtActiveMod()
    {
        var betaCompatibility = ReadRepoText("docs", "BETA_COMPATIBILITY.md");
        var remoteSetup = ReadRepoText("docs", "REMOTE_DEVELOPMENT_SETUP.md");
        var setupSpec = ReadRepoText("docs", "SETUP_SPEC.md");
        var manualChecklist = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-test-checklist.md");

        Assert.Contains("EZMicroBalance", betaCompatibility, StringComparison.Ordinal);
        Assert.Contains("dotnet list EZMicroBalance.csproj package --include-transitive", betaCompatibility, StringComparison.Ordinal);
        Assert.Contains("Active project: `EZMicroBalance`", remoteSetup, StringComparison.Ordinal);
        Assert.Contains(@"<GameRoot>\mods\EZMicroBalance\EZMicroBalance.dll", remoteSetup, StringComparison.Ordinal);
        Assert.Contains("manifest id `EZMicroBalance`", manualChecklist, StringComparison.Ordinal);
        Assert.Contains(@"<GameRoot>\mods\EZMicroBalance", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Confirm Spire Plus / `EZMicroBalance` appears.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Confirm legacy Easy Content / EzDailyContent is disabled or absent.", manualChecklist, StringComparison.Ordinal);

        Assert.DoesNotContain("dotnet list EzDailyContent.csproj", betaCompatibility, StringComparison.Ordinal);
        Assert.DoesNotContain("Confirm EzDailyContent appears", betaCompatibility, StringComparison.Ordinal);
        Assert.DoesNotContain("Project: `EzDailyContent`", remoteSetup, StringComparison.Ordinal);
        Assert.DoesNotContain(@"<GameRoot>\mods\EzDailyContent\EzDailyContent.dll", remoteSetup, StringComparison.Ordinal);
        Assert.DoesNotContain("current single-mod architecture", manualChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain(@"<GameRoot>\mods\EzDailyContent", manualChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("Confirm Easy Content / EzDailyContent appears.", manualChecklist, StringComparison.Ordinal);

        Assert.Contains("Historical note: this document records the original `EzDailyContent` setup baseline", setupSpec, StringComparison.Ordinal);
    }

    [Fact]
    public void ActiveProjectDoesNotCompileOrPackageLegacySources()
    {
        var project = ReadRepoText("EZMicroBalance.csproj");
        var solution = ReadRepoText("EZMicroBalance.sln");
        var exportPreset = ReadRepoText("export_presets.cfg");

        Assert.Contains("Compile Include=\"EZMicroBalanceCode/**/*.cs\"", project, StringComparison.Ordinal);
        Assert.Contains("AdditionalFiles Include=\"EZMicroBalance/localization/**/*.json\"", project, StringComparison.Ordinal);
        Assert.Contains("GodotPublishInputs Include=\"EZMicroBalance/**\"", project, StringComparison.Ordinal);
        Assert.DoesNotContain("Compile Include=\"EzDailyContentCode", project, StringComparison.Ordinal);
        Assert.DoesNotContain("AdditionalFiles Include=\"EzDailyContent", project, StringComparison.Ordinal);
        Assert.DoesNotContain("GodotPublishInputs Include=\"EzDailyContent", project, StringComparison.Ordinal);

        Assert.Contains("EZMicroBalance.csproj", solution, StringComparison.Ordinal);
        Assert.Contains("EZMicroBalance.Tests.csproj", solution, StringComparison.Ordinal);
        Assert.DoesNotContain("EzDailyContent.csproj", solution, StringComparison.Ordinal);

        Assert.Contains("export_filter=\"resources\"", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance.json", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/localization/eng/relics.json", exportPreset, StringComparison.Ordinal);
        Assert.Contains("EzDailyContent/*", exportPreset, StringComparison.Ordinal);
        Assert.Contains("EzDailyContentCode/*", exportPreset, StringComparison.Ordinal);
        Assert.Contains("EZMicroBalanceCode/*", exportPreset, StringComparison.Ordinal);
        Assert.Contains("docs/*", exportPreset, StringComparison.Ordinal);
        Assert.Contains("legacy/*", exportPreset, StringComparison.Ordinal);
    }

    private static void AssertSameKeys(string fileName)
    {
        var eng = JsonKeys(RepoPath("EZMicroBalance", "localization", "eng", fileName));
        var zhs = JsonKeys(RepoPath("EZMicroBalance", "localization", "zhs", fileName));
        Assert.Equal(eng, zhs);
    }

    private static SortedSet<string> JsonKeys(string path)
    {
        using var document = JsonDocument.Parse(File.ReadAllText(path, Encoding.UTF8));
        return new SortedSet<string>(
            document.RootElement.EnumerateObject().Select(property => property.Name),
            StringComparer.Ordinal);
    }

    private static IEnumerable<(string key, string value)> JsonStringValues(JsonElement element, string keyPrefix = "")
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in element.EnumerateObject())
            {
                var key = string.IsNullOrEmpty(keyPrefix)
                    ? property.Name
                    : $"{keyPrefix}.{property.Name}";

                foreach (var value in JsonStringValues(property.Value, key))
                {
                    yield return value;
                }
            }

            yield break;
        }

        if (element.ValueKind == JsonValueKind.Array)
        {
            var index = 0;
            foreach (var item in element.EnumerateArray())
            {
                foreach (var value in JsonStringValues(item, $"{keyPrefix}[{index}]"))
                {
                    yield return value;
                }

                index++;
            }

            yield break;
        }

        if (element.ValueKind == JsonValueKind.String)
        {
            yield return (keyPrefix, element.GetString() ?? string.Empty);
        }
    }

    private static string RemoveDynamicVarPlaceholders(string value)
    {
        return System.Text.RegularExpressions.Regex.Replace(
            value,
            @"\{[^{}]*\}",
            string.Empty,
            System.Text.RegularExpressions.RegexOptions.CultureInvariant);
    }

    private static bool ContainsEnglishTerm(string value, string term)
    {
        if (term.Contains('-', StringComparison.Ordinal))
        {
            return value.Contains(term, StringComparison.OrdinalIgnoreCase);
        }

        return System.Text.RegularExpressions.Regex.IsMatch(
            value,
            $@"(?<![A-Za-z]){System.Text.RegularExpressions.Regex.Escape(term)}(?![A-Za-z])",
            System.Text.RegularExpressions.RegexOptions.CultureInvariant |
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    }

    private static string? GetStringProperty(Type type, object instance, string propertyName)
    {
        var property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        Assert.NotNull(property);
        return (string?)property.GetValue(instance);
    }

    private static IReadOnlyList<string> ReadPckDirectory(string path)
    {
        var bytes = File.ReadAllBytes(path);
        var directoryOffset = (int)BitConverter.ToUInt64(bytes, 0x20);
        var count = (int)BitConverter.ToUInt32(bytes, directoryOffset);
        var offset = directoryOffset + 4;
        var entries = new List<string>(count);

        for (var i = 0; i < count; i++)
        {
            var length = (int)BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            entries.Add(Encoding.UTF8.GetString(bytes, offset, length).TrimEnd('\0'));
            offset += length;
            offset += 8 + 8 + 16 + 4;
        }

        return entries;
    }

    private static string[] GetExportedPckEntries(string[] exportedResources, IReadOnlyCollection<string> pckEntries)
    {
        var expected = new List<string>();

        foreach (var entry in exportedResources
                     .Where(resource => resource.EndsWith(".png", StringComparison.Ordinal) ||
                                        resource.EndsWith(".json", StringComparison.Ordinal) ||
                                        resource.EndsWith(".txt", StringComparison.Ordinal) ||
                                        resource.EndsWith(".tscn", StringComparison.Ordinal))
                     .Where(entry => IsActiveExportResource(entry)))
        {
            if (entry.EndsWith(".png", StringComparison.Ordinal))
            {
                if (pckEntries.Contains(entry))
                {
                    expected.Add(entry);
                }

                var imported = $"{entry}.import";
                if (pckEntries.Contains(imported))
                {
                    expected.Add(imported);
                }

                continue;
            }

            if (entry.EndsWith(".tscn", StringComparison.Ordinal))
            {
                if (pckEntries.Contains(entry))
                {
                    expected.Add(entry);
                }

                var remap = $"{entry}.remap";
                if (pckEntries.Contains(remap))
                {
                    expected.Add(remap);
                }

                continue;
            }

            if (pckEntries.Contains(entry))
            {
                expected.Add(entry);
            }
        }

        return expected
            .OrderBy(entry => entry, StringComparer.Ordinal)
            .ToArray();
    }

    private static bool IsActiveExportResource(string relativePath)
    {
        return IsActiveReleaseResource(relativePath) &&
            (Path.GetExtension(relativePath) is ".json" or ".png" or ".txt" or ".tscn");
    }

    private static bool IsActiveReleaseResource(string relativePath)
    {
        return relativePath.StartsWith("EZMicroBalance/", StringComparison.Ordinal) &&
            !relativePath.Equals("EZMicroBalance/mod_real.png", StringComparison.Ordinal) &&
            !relativePath.Equals("EZMicroBalance/mod_real.png.import", StringComparison.Ordinal);
    }

    private static string[] ParseExportFiles(string exportPreset)
    {
        var match = Regex.Match(exportPreset, @"export_files=PackedStringArray\((?<files>[^)]*)\)");
        Assert.True(match.Success, "Could not find export_files in export_presets.cfg.");

        return Regex.Matches(match.Groups["files"].Value, @"""(?<path>[^""]+)""")
            .Cast<Match>()
            .Select(match => match.Groups["path"].Value)
            .ToArray();
    }

    private static Exception? Unwrap(Exception? exception)
    {
        return exception is TargetInvocationException targetInvocationException
            ? targetInvocationException.InnerException ?? targetInvocationException
            : exception;
    }

    private static string Sha256(string path)
    {
        using var stream = File.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(stream));
    }

    private static string NormalizeJson(string path)
    {
        using var document = JsonDocument.Parse(File.ReadAllText(path, Encoding.UTF8));
        return JsonSerializer.Serialize(document.RootElement, new JsonSerializerOptions { WriteIndented = false });
    }

    private static (int Width, int Height) ReadPngDimensions(string path)
    {
        var bytes = File.ReadAllBytes(path);
        Assert.True(bytes.Length >= 24, $"PNG file is too short: {path}");
        ReadOnlySpan<byte> pngSignature = [0x89, (byte)'P', (byte)'N', (byte)'G', 0x0D, 0x0A, 0x1A, 0x0A];
        Assert.True(bytes.AsSpan(0, 8).SequenceEqual(pngSignature), $"Release art is not a PNG: {path}");

        return (ReadBigEndianInt32(bytes, 16), ReadBigEndianInt32(bytes, 20));
    }

    private static int ReadBigEndianInt32(byte[] bytes, int offset)
    {
        return
            (bytes[offset] << 24) |
            (bytes[offset + 1] << 16) |
            (bytes[offset + 2] << 8) |
            bytes[offset + 3];
    }

    private static string ReadRepoText(params string[] parts)
    {
        return File.ReadAllText(RepoPath(parts), Encoding.UTF8);
    }

    private static IEnumerable<string> CandidateBuildDlls()
    {
        return CandidateBuildDirectories().Select(dir => Path.Combine(dir, "EZMicroBalance.dll"));
    }

    private static IEnumerable<string> CandidateBuildDirectories()
    {
        var root = RepoPath(".godot", "mono", "temp", "bin");
        yield return Path.Combine(root, "Debug");
        yield return Path.Combine(root, "Release");
    }

    private static string RepoPath(params string[] parts)
    {
        return Path.Combine(new[] { FindRepoRoot() }.Concat(parts).ToArray());
    }

    private static string ToRepoRelativePath(string path)
    {
        return Path.GetRelativePath(FindRepoRoot(), path).Replace('\\', '/');
    }

    private static string GamePath(params string[] parts)
    {
        var root = Environment.GetEnvironmentVariable("STS2_PATH");
        if (string.IsNullOrWhiteSpace(root))
        {
            root = @"D:\Steam\steamapps\common\Slay the Spire 2";
        }

        return Path.Combine(new[] { root }.Concat(parts).ToArray());
    }

    private static string FindRepoRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "EZMicroBalance.csproj")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not find repository root from test output directory.");
    }
}
