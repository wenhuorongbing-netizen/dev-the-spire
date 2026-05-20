using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class AncientStateMirrorGuardTests
{
    private sealed record AncientStateMirrorSpec(
        string Name,
        string StateSourcePath,
        string HookSourcePath,
        string RuntimeField,
        string DeckField);

    private static readonly AncientStateMirrorSpec[] MirrorSpecs =
    [
        new(
            "Urda",
            "EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaBlessingService.State.cs",
            "EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaRunHook.cs",
            "UrdaStateKey",
            "UrdaDeckStateKey"),
        new(
            "Morvi",
            "EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviBlessingService.State.cs",
            "EZMicroBalanceCode/Ancients/Expansion/Morvi/MorviHooks.cs",
            "MorviStateKey",
            "MorviDeckStateKey"),
        new(
            "Lotha",
            "EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaBlessingService.State.cs",
            "EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaHooks.cs",
            "LothaStateKey",
            "LothaDeckStateKey")
    ];

    [Fact]
    public void AncientPlayerStateContractRestoresRuntimeFromOwnedNonRemovedDeckCards()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientPlayerState.cs");
        var getMethod = SliceMethod(source, "Get");
        var setMethod = SliceMethod(source, "Set");
        var syncMethod = SliceMethod(source, "SyncDeck");
        var readFromDeckMethod = SliceMethod(source, "ReadFromDeck");
        var mirrorToDeckMethod = SliceMethod(source, "MirrorToDeck");

        AssertBefore(getMethod, "var runtimeState = runtimeField[player] ?? string.Empty;", "MirrorToDeck(player, deckField, runtimeState)");
        AssertBefore(getMethod, "var deckState = ReadFromDeck(player, deckField);", "runtimeField[player] = deckState");
        AssertBefore(getMethod, "runtimeField[player] = deckState", "MirrorToDeck(player, deckField, deckState)");

        AssertSourceContains(
            setMethod,
            "runtimeField[player] = state",
            "MirrorToDeck(player, deckField, state)");
        AssertBefore(setMethod, "runtimeField[player] = state", "MirrorToDeck(player, deckField, state)");

        AssertSourceContains(
            syncMethod,
            "var state = Get(player, runtimeField, deckField)",
            "MirrorToDeck(player, deckField, state)");
        AssertBefore(syncMethod, "var state = Get(player, runtimeField, deckField)", "MirrorToDeck(player, deckField, state)");

        AssertSourceContains(
            readFromDeckMethod,
            "player.Deck.Cards",
            "card.Owner == player",
            "!card.HasBeenRemovedFromState",
            "deckField[card] ?? string.Empty",
            "FirstOrDefault(state => !string.IsNullOrWhiteSpace(state))");
        AssertSourceContains(
            mirrorToDeckMethod,
            "player.Deck.Cards.Where(card => card.Owner == player && !card.HasBeenRemovedFromState)",
            "deckField[card] = state");
    }

    [Fact]
    public void UrdaMorviAndLothaProgressStateFunnelsThroughAncientPlayerState()
    {
        foreach (var spec in MirrorSpecs)
        {
            var source = ReadRepoText(spec.StateSourcePath.Split('/'));
            var getSelectedBlessing = SliceMethod(source, "GetSelectedBlessing");
            var getProgress = SliceMethod(source, "GetProgress");
            var setProgress = SliceMethod(source, "SetProgress");
            var setState = SliceMethod(source, "SetState");
            var syncPersistentState = SliceMethod(source, "SyncPersistentState");

            AssertUsesHelperWithFields(getSelectedBlessing, "AncientPlayerState.Get(", spec);
            AssertUsesHelperWithFields(getProgress, "AncientPlayerState.Get(", spec);

            AssertSourceContains(
                setProgress,
                "var selectedBlessing = GetSelectedBlessing(player)",
                "if (!string.IsNullOrWhiteSpace(selectedBlessing))",
                "SetState(player, selectedBlessing, progress)");
            AssertBefore(setProgress, "var selectedBlessing = GetSelectedBlessing(player)", "SetState(player, selectedBlessing, progress)");
            AssertUsesHelperWithFields(setState, "AncientPlayerState.Set(", spec);
            AssertUsesHelperWithFields(syncPersistentState, "AncientPlayerState.SyncDeck(", spec);
        }
    }

    [Fact]
    public void UrdaMorviAndLothaHaveARecurrentDeckMirrorSyncHook()
    {
        foreach (var spec in MirrorSpecs)
        {
            var source = ReadRepoText(spec.HookSourcePath.Split('/'));
            var afterCardChangedPiles = SliceMethod(source, "AfterCardChangedPiles");

            AssertSourceContains(
                afterCardChangedPiles,
                "CardModel card",
                "SyncPersistentState(card.Owner)");
            Assert.True(
                afterCardChangedPiles.Contains("return Task.CompletedTask", StringComparison.Ordinal) ||
                afterCardChangedPiles.Contains("public override async Task AfterCardChangedPiles", StringComparison.Ordinal),
                $"{spec.Name} AfterCardChangedPiles should either be a synchronous mirror hook or an async hook that still performs recurrent mirror sync.");
        }
    }

    [Fact]
    public void EncodedAncientStateFieldsAreNotDirectlyIndexedOutsideTheMirrorHelper()
    {
        var forbidden = new Regex(
            @"\b(?:UrdaStateKey|UrdaDeckStateKey|MorviStateKey|MorviDeckStateKey|LothaStateKey|LothaDeckStateKey)\s*\[",
            RegexOptions.Compiled);
        var offenders = Directory
            .GetFiles(RepoPath("EZMicroBalanceCode"), "*.cs", SearchOption.AllDirectories)
            .Where(path => !path.EndsWith("AncientPlayerState.cs", StringComparison.Ordinal))
            .SelectMany(path => File.ReadLines(path, Encoding.UTF8)
                .Select((line, index) => new
                {
                    Path = ToRepoRelativePath(path),
                    LineNumber = index + 1,
                    Line = line
                }))
            .Where(entry => forbidden.IsMatch(entry.Line))
            .Select(entry => $"{entry.Path}:{entry.LineNumber}: {entry.Line.Trim()}")
            .ToArray();

        Assert.True(
            offenders.Length == 0,
            "Urda/Morvi/Lotha encoded Player/deck mirror fields must be read or written through AncientPlayerState. " +
            "Allowed separate marker fields such as LothaMirrorRebuttalCard are intentionally not part of this scan." +
            Environment.NewLine +
            string.Join(Environment.NewLine, offenders));
    }

    [Fact]
    public void DocsKeepStateMirrorCoverageSeparateFromLiveSaveLoadProof()
    {
        var riskRegister = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "risk-register.md");
        var apiResearch = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "api-research.md");
        var manualChecklist = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "manual-test-checklist.md");
        var issues = ReadRepoText("docs", "issues.md");
        var issueDetail = ReadRepoText("docs", "issues", "ancient-expansion-v2.2.md");
        var projectState = ReadRepoText("PROJECT_STATE.md");
        var currentDocs = string.Join(
            Environment.NewLine,
            riskRegister,
            apiResearch,
            manualChecklist,
            issues,
            issueDetail,
            projectState);

        AssertSourceContains(
            riskRegister,
            "`SavedSpireField<Player,string>`",
            "`SavedSpireField<CardModel,string>`",
            "`AncientPlayerState`",
            "direct field bypasses");
        AssertSourceContains(
            apiResearch,
            "AncientPlayerState.Get(...)",
            "AncientPlayerState.Set(...)",
            "AncientPlayerState.SyncDeck(...)",
            "owned, non-removed deck cards");
        AssertSourceContains(
            manualChecklist,
            "- [ ] Current Urda save/load verified",
            "- [ ] Morvi save/load after selecting each blessing",
            "- [ ] Lotha save/load after selecting each blessing");
        Assert.Contains("live", currentDocs, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("save/load", currentDocs, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("pending", currentDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("live save/load verified", currentDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("save/load ready", currentDocs, StringComparison.OrdinalIgnoreCase);
    }

    private static void AssertUsesHelperWithFields(string source, string helperCall, AncientStateMirrorSpec spec)
    {
        AssertSourceContains(
            source,
            helperCall,
            $"AncientSavedStateFields.{spec.RuntimeField}",
            $"AncientSavedStateFields.{spec.DeckField}");
    }

    private static string SliceMethod(string source, string methodName)
    {
        var signature = Regex.Match(
            source,
            $@"(?:public|private)\s+(?:override\s+)?(?:static\s+)?(?:async\s+)?[\w<>,?\[\] ]+\s+{Regex.Escape(methodName)}\s*\(");
        Assert.True(signature.Success, $"Missing method: {methodName}");

        var braceStart = source.IndexOf('{', signature.Index);
        Assert.True(braceStart >= 0, $"Missing body for method: {methodName}");

        var depth = 0;
        for (var index = braceStart; index < source.Length; index++)
        {
            if (source[index] == '{')
            {
                depth++;
            }
            else if (source[index] == '}')
            {
                depth--;
                if (depth == 0)
                {
                    return source[signature.Index..(index + 1)];
                }
            }
        }

        throw new InvalidOperationException($"Could not find end of method: {methodName}");
    }

}
