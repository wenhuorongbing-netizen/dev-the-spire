using Xunit;

namespace EZMicroBalance.Tests;

public sealed class SaveStateContractsGuardTests
{
    [Theory]
    [MemberData(nameof(StatefulFeatureContracts))]
    public void StatefulFeaturesHaveSourceSaveHydrateClearLogAndManualRows(StatefulFeatureContract contract)
    {
        var source = contract.ReadSource();
        var manualDocs = ReadCurrentFacingDocs(
            "docs/architecture/save-state-contracts.md",
            "docs/issues.md",
            "docs/specs/release-traceability-matrix.md");

        AssertAny(source, contract.SaveFieldOrDeckMirror, $"{contract.Feature} must have a SavedSpireField-backed state key or deck mirror in source.");
        AssertAny(source, contract.HydrateOrRestorePath, $"{contract.Feature} must have a hydrate/restore path in source.");
        AssertAny(source, contract.ClearOrResetPath, $"{contract.Feature} must have a clear/reset path in source.");
        AssertAny(source, contract.ReleaseEvidenceMarker, $"{contract.Feature} must have a source log/evidence marker. A future centralized ReleaseEvidenceLog should satisfy this contract explicitly.");
        AssertAny(manualDocs, contract.ManualRows, $"{contract.Feature} must keep a manual save/load proof row open until live evidence exists.");
    }

    [Fact]
    public void SaveStateGuardIsSourceLayerNotDocsOnly()
    {
        var testSource = ReadRepoText("tests", "EZMicroBalance.Tests", "SaveStateContractsGuardTests.cs");

        Assert.Contains("[\"EZMicroBalanceCode\"", testSource, StringComparison.Ordinal);
        Assert.Contains("SavedSpireField", ReadSourceTree("EZMicroBalanceCode"), StringComparison.Ordinal);
        Assert.DoesNotContain("AssertRepoFileExists(\"docs\"", testSource, StringComparison.Ordinal);
    }

    [Fact]
    public void UrdaStateCodecKeepsWireFormatAndLegacyDecodeStable()
    {
        var state = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.State.cs");
        var schema = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.StateSchema.cs");
        var codec = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaStateCodec.cs");

        AssertSourceContains(
            state,
            "return ReadState(player).SelectedBlessing;",
            "private static UrdaProgress GetProgress(Player player)",
            "UrdaStateCodec.Encode(new UrdaStateSnapshot(blessingId, progress))",
            "UrdaStateCodec.Decode(AncientPlayerState.Get(");

        AssertSourceContains(
            schema,
            "internal sealed record UrdaProgress(",
            "bool HumusCompletionPending",
            "string RootSightMarkedCoords",
            "string SeedBankCardIds",
            "string RootSightPreviewRecords",
            "int SeedbedCombatSlots",
            "public static UrdaProgress Default => new(");

        AssertSourceContains(
            codec,
            "internal sealed record UrdaStateSnapshot(string SelectedBlessing, UrdaProgress Progress)",
            "private const char ProgressSeparator = ';'",
            "private const int LegacyMinimumPartCount = 8",
            "private const int LegacyBaseIndex = 8",
            "private const int CurrentBaseIndex = 9",
            "HumusCompletionPending before MoltingActive",
            "hasHumusPendingField && ParseBool(parts[6])",
            "ParseBool(parts[hasHumusPendingField ? 7 : 6])",
            "ParseInt(parts[hasHumusPendingField ? 8 : 7])",
            "var baseIndex = hasHumusPendingField ? CurrentBaseIndex : LegacyBaseIndex",
            "SanitizeStateField(progress.RootSightMarkedCoords)",
            "SanitizeStateField(progress.SeedBankCardIds)",
            "SanitizeStateField(progress.RootSightPreviewRecords)");
        AssertBefore(codec, "progress.HumusCompleted ? 1 : 0", "progress.HumusCompletionPending ? 1 : 0");
        AssertBefore(codec, "progress.HumusCompletionPending ? 1 : 0", "progress.MoltingActive ? 1 : 0");
        AssertBefore(codec, "SanitizeStateField(progress.RootSightMarkedCoords)", "SanitizeStateField(progress.SeedBankCardIds)");
        AssertBefore(codec, "SanitizeStateField(progress.SeedBankCardIds)", "progress.SeedBankSettled ? 1 : 0");
        AssertBefore(codec, "progress.SeedBankSettled ? 1 : 0", "SanitizeStateField(progress.RootSightPreviewRecords)");
    }

    public static TheoryData<StatefulFeatureContract> StatefulFeatureContracts() =>
    [
        new StatefulFeatureContract(
            "Urda Root Eyes",
            [
                ["EZMicroBalanceCode", "Ancients", "Common", "AncientSavedStateFields.cs"],
                ["EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.State.cs"],
                ["EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RootSightSelection.cs"],
                ["EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RootSightSelectionCommit.cs"],
                ["EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RootSightMarkers.cs"],
                ["EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.RunLifecycle.cs"]
            ],
            ["SavedSpireField<Player, string> UrdaStateKey", "SavedSpireField<CardModel, string> UrdaDeckStateKey", "RootSightPreviewRecords"],
            ["RestoreRootSightPreviewMarkers", "AfterMapGenerated", "restored one eye"],
            ["ClearStaleRootSightPreview", "ClearUnreachableRootSightPreviews", "ResetRootSightTransientState"],
            ["MainFile.Logger", "[Spire Plus] Urda Root Eyes"],
            ["Root Eyes", "Root Sight", "SAVE-LOAD"]),
        new StatefulFeatureContract(
            "Urda Seed Bank",
            [
                ["EZMicroBalanceCode", "Ancients", "Common", "AncientSavedStateFields.cs"],
                ["EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.State.cs"],
                ["EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBank.cs"],
                ["EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBankExtraction.cs"],
                ["EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBankExtractionState.cs"],
                ["EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.SeedBankStatus.cs"]
            ],
            ["SavedSpireField<Player, string> UrdaStateKey", "SavedSpireField<CardModel, string> UrdaDeckStateKey", "SeedBankCardIds"],
            ["GetSeedBankCardIds", "TryExtractSeedBankFromRelicClick", "TryGetStoredCard"],
            ["SeedBankCardIds = string.Empty", "SeedBankSettled = true", "RefreshSeedBankRelicStatus"],
            ["MainFile.Logger", "[Spire Plus] Urda Seed Bank"],
            ["Seed Bank", "SAVE-LOAD"]),
        new StatefulFeatureContract(
            "Morvi state",
            [
                ["EZMicroBalanceCode", "Ancients", "Common", "AncientSavedStateFields.cs"],
                ["EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviBlessingService.CombatState.cs"],
                ["EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviBlessingService.State.cs"],
                ["EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviBlessingService.ForbiddenLoan.cs"],
                ["EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviBlessingService.OpenBook.cs"],
                ["EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviRunHook.cs"]
            ],
            ["SavedSpireField<Player, string> MorviStateKey", "SavedSpireField<CardModel, string> MorviDeckStateKey", "MorviOpenBookSealedCard"],
            ["SyncPersistentState", "AfterCardChangedPiles", "BeforeCombatStart"],
            ["ClearState", "ClearBorrowedAncientCards", "ResetCombatState"],
            ["MainFile.Logger", "[Spire Plus] Morvi"],
            ["Morvi state", "SAVE-LOAD"]),
        new StatefulFeatureContract(
            "Lotha Death Reprieve",
            [
                ["EZMicroBalanceCode", "Ancients", "Common", "AncientSavedStateFields.cs"],
                ["EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.DeathReprieveState.cs"],
                ["EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.State.cs"],
                ["EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.DeathReprieve.cs"],
                ["EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.CombatStart.cs"],
                ["EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.CombatStateReset.cs"]
            ],
            ["SavedSpireField<Player, string> LothaStateKey", "SavedSpireField<CardModel, string> LothaDeckStateKey", "DeathReprievePhase"],
            ["HydrateDeathReprieveState", "BeforeCombatStart", "deck-mirrored blessing progress"],
            ["ResolveDeathReprieveProgress", "ResetCombatState", "DeathReprievePhase.Resolved"],
            ["MainFile.Logger", "[Spire Plus] Lotha Death Reprieve"],
            ["Lotha Death Reprieve", "SAVE-LOAD"]),
        new StatefulFeatureContract(
            "Vakuu child combat",
            [
                ["EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightPatch.cs"],
                ["EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightService.Entry.cs"],
                ["EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightService.ParentRestore.cs"],
                ["EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightService.NoRewardResume.cs"],
                ["EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightVictory.cs"]
            ],
            ["CombatRoom.ToSerializable", "PreserveParentEventForPreFinishedSave", "ParentEventId"],
            ["ArmPrefinishedParentRestoreHealSkip", "ResumeAfterVictory", "ProceedFromNoRewardVictory"],
            ["ClearEventNode", "no-reward resume found no valid parent event stack", "ClearCommandForceFightWhenBeginEventCompletes"],
            ["MainFile.Logger", "[Spire Plus] Vakuu"],
            ["Vakuu", "VAKUU-FIGHT-LIVE", "SAVE-LOAD"]),
        new StatefulFeatureContract(
            "Rootblight",
            [
                ["EZMicroBalanceCode", "Ascension", "Core", "AscensionSavedStateFields.cs"],
                ["EZMicroBalanceCode", "Ascension", "Cards", "RootFamilyCard.cs"],
                ["EZMicroBalanceCode", "Ascension", "Cards", "RootBudCard.cs"],
                ["EZMicroBalanceCode", "Ascension", "Rewards", "RootDeckService.State.cs"],
                ["EZMicroBalanceCode", "Ascension", "Rewards", "RootDeckService.Lifecycle.cs"],
                ["EZMicroBalanceCode", "Ascension", "Rewards", "RootDeckService.PendingDowngrades.cs"],
                ["EZMicroBalanceCode", "Ascension", "Combat", "RootBudCombatHook.Lifecycle.cs"]
            ],
            ["SavedSpireField<Player, int> RootblightLevel", "SavedSpireField<RootFamilyCard, bool> RootblightWasPresentAtCombatStart", "DeckVersion is RootFamilyCard"],
            ["SetDiagnosticLevelFromDeck", "MarkCombatStartRootblight", "ReadPendingCombatDowngrades"],
            ["ClearPendingCombatDowngrades", "WasPresentAtCombatStart = false", "PlantedInSeedbed = false"],
            ["MainFile.Logger", "[Spire Plus] Ascension Rootblight"],
            ["Rootblight", "SAVE-LOAD"])
    ];

    private static void AssertAny(string source, IReadOnlyList<string> snippets, string message)
    {
        Assert.True(
            snippets.Any(snippet => source.Contains(snippet, StringComparison.Ordinal)),
            message + Environment.NewLine + "Expected one of:" + Environment.NewLine + string.Join(Environment.NewLine, snippets));
    }

    public sealed record StatefulFeatureContract(
        string Feature,
        IReadOnlyList<string[]> SourceFiles,
        IReadOnlyList<string> SaveFieldOrDeckMirror,
        IReadOnlyList<string> HydrateOrRestorePath,
        IReadOnlyList<string> ClearOrResetPath,
        IReadOnlyList<string> ReleaseEvidenceMarker,
        IReadOnlyList<string> ManualRows)
    {
        public override string ToString() => Feature;

        public string ReadSource() =>
            string.Join(
                Environment.NewLine,
                SourceFiles.Select(parts => ReadRepoText(parts)));
    }
}
