using EZMicroBalance.EZMicroBalanceCode.Core.Architecture;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class ArchitectureSkeletonGuardTests
{
    // DeathProtectionService spec

    [Fact]
    public void DeathProtectionSpecDocumentsReprieveLifecycle()
    {
        var spec = TestRepo.ReadRepoText("docs", "architecture", "death-protection-spec.md");

        Assert.Contains("ShouldDie", spec, StringComparison.Ordinal);
        Assert.Contains("AfterPreventingDeath", spec, StringComparison.Ordinal);
        Assert.Contains("DeathReprieveActive", spec, StringComparison.Ordinal);
        Assert.Contains("DeathReprievePendingStart", spec, StringComparison.Ordinal);
        Assert.Contains("inReprieve", spec, StringComparison.Ordinal);
        Assert.Contains("forced unavoidable death", spec, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DeathProtectionSpecDocumentsCoopOwnerAttribution()
    {
        var spec = TestRepo.ReadRepoText("docs", "architecture", "death-protection-spec.md");

        Assert.Contains("co-op", spec, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("owner", spec, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ShouldSkipCoopCombat", spec, StringComparison.Ordinal);
        Assert.Contains("SPIREPLUS_ALLOW_UNVERIFIED_COOP_COMBAT_HOOKS", spec, StringComparison.Ordinal);
    }

    [Fact]
    public void DeathProtectionSpecDefinesFutureInterface()
    {
        var spec = TestRepo.ReadRepoText("docs", "architecture", "death-protection-spec.md");

        Assert.Contains("IDeathProtectionProvider", spec, StringComparison.Ordinal);
        Assert.Contains("ShouldPreventDeath", spec, StringComparison.Ordinal);
        Assert.Contains("IsInReprieve", spec, StringComparison.Ordinal);
        Assert.Contains("CanBypassForcedDeath", spec, StringComparison.Ordinal);
    }

    // DeathProtectionService stub

    [Fact]
    public void DeathProtectionServiceStubExists()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "DeathProtectionService.cs");

        Assert.Contains("DeathProtectionService", source, StringComparison.Ordinal);
    }

    [Fact]
    public void DeathProtectionServiceDefinesRequestRecord()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "DeathProtectionDiagnosticsContracts.cs");

        Assert.Contains("record DeathProtectionRequest", source, StringComparison.Ordinal);
        Assert.Contains("string Player", source, StringComparison.Ordinal);
        Assert.Contains("string Source", source, StringComparison.Ordinal);
        Assert.Contains("int Damage", source, StringComparison.Ordinal);
        Assert.Contains("bool IsUnavoidable", source, StringComparison.Ordinal);
    }

    [Fact]
    public void DeathProtectionServiceDefinesResultEnum()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "DeathProtectionDiagnosticsContracts.cs");

        Assert.Contains("enum DeathProtectionResult", source, StringComparison.Ordinal);
        Assert.Contains("Protected", source, StringComparison.Ordinal);
        Assert.Contains("NotProtected", source, StringComparison.Ordinal);
        Assert.Contains("ForcedDeath", source, StringComparison.Ordinal);
        Assert.Contains("record DeathProtectionCheck", source, StringComparison.Ordinal);
        Assert.Contains("IDeathProtectionProvider? Provider", source, StringComparison.Ordinal);
    }

    [Fact]
    public void DeathProtectionServiceDefinesPriorityEnum()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "DeathProtectionDiagnosticsContracts.cs");

        Assert.Contains("enum DeathProtectionPriority", source, StringComparison.Ordinal);
        Assert.Contains("Reprieve = 100", source, StringComparison.Ordinal);
        Assert.Contains("Sacrifice = 200", source, StringComparison.Ordinal);
        Assert.Contains("LastStand = 300", source, StringComparison.Ordinal);
    }

    [Fact]
    public void DeathProtectionServiceDefinesProviderInterface()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "DeathProtectionDiagnosticsContracts.cs");

        Assert.Contains("interface IDeathProtectionProvider", source, StringComparison.Ordinal);
        Assert.Contains("bool CanProtect(DeathProtectionRequest", source, StringComparison.Ordinal);
        Assert.Contains("DeathProtectionPriority Priority", source, StringComparison.Ordinal);
    }

    [Fact]
    public void DeathProtectionDiagnosticsStayOutOfLothaLethalPath()
    {
        var lothaSource = TestRepo.ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha");

        Assert.DoesNotContain("DeathProtectionService", lothaSource, StringComparison.Ordinal);
        Assert.DoesNotContain("DeathProtectionRequest", lothaSource, StringComparison.Ordinal);
        Assert.DoesNotContain("IDeathProtectionProvider", lothaSource, StringComparison.Ordinal);
    }

    [Fact]
    public void DeathProtectionServiceIsDiagnosticsOnly()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "DeathProtectionService.cs");

        Assert.Contains("Diagnostics-only", source, StringComparison.Ordinal);
        Assert.Contains("Not wired into game logic", source, StringComparison.Ordinal);
        Assert.Contains("CheckProtection", source, StringComparison.Ordinal);
        Assert.Contains("CheckProtectionDetailed", source, StringComparison.Ordinal);
    }

    [Fact]
    public void DeathProtectionServiceSortsProvidersByPriority()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "DeathProtectionService.cs");

        Assert.Contains("a.Priority.CompareTo(b.Priority)", source, StringComparison.Ordinal);
    }

    [Fact]
    public void DeathProtectionServiceHandlesUnavoidableDeath()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "DeathProtectionService.cs");

        Assert.Contains("request.IsUnavoidable", source, StringComparison.Ordinal);
        Assert.Contains("DeathProtectionResult.ForcedDeath", source, StringComparison.Ordinal);
    }

    // DeathProtectionService canary (behavioral)

    [Fact]
    public void DeathProtectionService_Canary_RegisterIncrementsProviderCount()
    {
        DeathProtectionService.ClearProviders();
        try
        {
            Assert.Equal(0, DeathProtectionService.ProviderCount);

            DeathProtectionService.Register(new StubDeathProtectionProvider(DeathProtectionPriority.Reprieve, canProtect: true));
            Assert.Equal(1, DeathProtectionService.ProviderCount);

            DeathProtectionService.Register(new StubDeathProtectionProvider(DeathProtectionPriority.Sacrifice, canProtect: false));
            Assert.Equal(2, DeathProtectionService.ProviderCount);
        }
        finally
        {
            DeathProtectionService.ClearProviders();
        }
    }

    [Fact]
    public void DeathProtectionService_Canary_ProvidersSortedByPriority()
    {
        DeathProtectionService.ClearProviders();
        try
        {
            // Register out of priority order: LastStand first, then Reprieve, then Sacrifice.
            DeathProtectionService.Register(new StubDeathProtectionProvider(DeathProtectionPriority.LastStand, canProtect: true));
            DeathProtectionService.Register(new StubDeathProtectionProvider(DeathProtectionPriority.Reprieve, canProtect: true));
            DeathProtectionService.Register(new StubDeathProtectionProvider(DeathProtectionPriority.Sacrifice, canProtect: true));

            var priorities = DeathProtectionService.RegisteredPriorities;
            Assert.Equal(3, priorities.Count);
            Assert.Equal(DeathProtectionPriority.Reprieve, priorities[0]);   // 100
            Assert.Equal(DeathProtectionPriority.Sacrifice, priorities[1]);  // 200
            Assert.Equal(DeathProtectionPriority.LastStand, priorities[2]);  // 300
        }
        finally
        {
            DeathProtectionService.ClearProviders();
        }
    }

    [Fact]
    public void DeathProtectionService_Canary_UnavoidableDeathReturnsForcedDeath()
    {
        DeathProtectionService.ClearProviders();
        try
        {
            // Register a provider that says it can protect - but unavoidable death should still force death.
            DeathProtectionService.Register(new StubDeathProtectionProvider(DeathProtectionPriority.Reprieve, canProtect: true));

            var request = new DeathProtectionRequest("Player1", "TestSource", 999, IsUnavoidable: true);
            var result = DeathProtectionService.CheckProtection(request);

            Assert.Equal(DeathProtectionResult.ForcedDeath, result);
        }
        finally
        {
            DeathProtectionService.ClearProviders();
        }
    }

    [Fact]
    public void DeathProtectionService_Canary_UnavoidableDeathDoesNotQueryProviders()
    {
        DeathProtectionService.ClearProviders();
        try
        {
            var provider = new StubDeathProtectionProvider(DeathProtectionPriority.Reprieve, canProtect: true);
            DeathProtectionService.Register(provider);

            var request = new DeathProtectionRequest("Player1", "TestSource", 999, IsUnavoidable: true);
            var result = DeathProtectionService.CheckProtectionDetailed(request);

            Assert.Equal(DeathProtectionResult.ForcedDeath, result.Result);
            Assert.Null(result.Provider);
            Assert.Equal(0, provider.CallCount);
        }
        finally
        {
            DeathProtectionService.ClearProviders();
        }
    }

    [Fact]
    public void DeathProtectionService_Canary_NoProvidersReturnsNotProtected()
    {
        DeathProtectionService.ClearProviders();
        try
        {
            var request = new DeathProtectionRequest("Player1", "TestSource", 10, IsUnavoidable: false);
            var result = DeathProtectionService.CheckProtection(request);

            Assert.Equal(DeathProtectionResult.NotProtected, result);
        }
        finally
        {
            DeathProtectionService.ClearProviders();
        }
    }

    [Fact]
    public void DeathProtectionService_Canary_MatchingProviderReturnsProtected()
    {
        DeathProtectionService.ClearProviders();
        try
        {
            DeathProtectionService.Register(new StubDeathProtectionProvider(DeathProtectionPriority.Reprieve, canProtect: true));

            var request = new DeathProtectionRequest("Player1", "TestSource", 10, IsUnavoidable: false);
            var result = DeathProtectionService.CheckProtection(request);

            Assert.Equal(DeathProtectionResult.Protected, result);
        }
        finally
        {
            DeathProtectionService.ClearProviders();
        }
    }

    [Fact]
    public void DeathProtectionService_Canary_FirstMatchingProviderWins()
    {
        DeathProtectionService.ClearProviders();
        try
        {
            // Register two providers: Reprieve (priority 100) and Sacrifice (priority 200).
            // Both can protect - but Reprieve is checked first due to lower priority.
            var reprieveProvider = new StubDeathProtectionProvider(DeathProtectionPriority.Reprieve, canProtect: true);
            var sacrificeProvider = new StubDeathProtectionProvider(DeathProtectionPriority.Sacrifice, canProtect: true);

            DeathProtectionService.Register(sacrificeProvider);  // Register higher priority first
            DeathProtectionService.Register(reprieveProvider);   // Register lower priority second

            var request = new DeathProtectionRequest("Player1", "TestSource", 10, IsUnavoidable: false);
            var result = DeathProtectionService.CheckProtectionDetailed(request);

            Assert.Equal(DeathProtectionResult.Protected, result.Result);
            Assert.Same(reprieveProvider, result.Provider);
            Assert.Equal(1, reprieveProvider.CallCount);
            Assert.Equal(0, sacrificeProvider.CallCount);
        }
        finally
        {
            DeathProtectionService.ClearProviders();
        }
    }

    [Fact]
    public void DeathProtectionService_Canary_DetailedCheckReturnsMatchingProvider()
    {
        DeathProtectionService.ClearProviders();
        try
        {
            var nonMatchingProvider = new StubDeathProtectionProvider(DeathProtectionPriority.Reprieve, canProtect: false);
            var matchingProvider = new StubDeathProtectionProvider(DeathProtectionPriority.Sacrifice, canProtect: true);
            DeathProtectionService.Register(matchingProvider);
            DeathProtectionService.Register(nonMatchingProvider);

            var request = new DeathProtectionRequest("Player1", "TestSource", 10, IsUnavoidable: false);
            var result = DeathProtectionService.CheckProtectionDetailed(request);

            Assert.Equal(DeathProtectionResult.Protected, result.Result);
            Assert.Same(matchingProvider, result.Provider);
            Assert.Equal(1, nonMatchingProvider.CallCount);
            Assert.Equal(1, matchingProvider.CallCount);
        }
        finally
        {
            DeathProtectionService.ClearProviders();
        }
    }

    [Fact]
    public void DeathProtectionService_Canary_ClearProvidersResetsState()
    {
        DeathProtectionService.ClearProviders();
        try
        {
            DeathProtectionService.Register(new StubDeathProtectionProvider(DeathProtectionPriority.Reprieve, canProtect: true));
            DeathProtectionService.Register(new StubDeathProtectionProvider(DeathProtectionPriority.Sacrifice, canProtect: false));

            Assert.Equal(2, DeathProtectionService.ProviderCount);

            DeathProtectionService.ClearProviders();
            Assert.Equal(0, DeathProtectionService.ProviderCount);
            Assert.Empty(DeathProtectionService.RegisteredPriorities);
        }
        finally
        {
            DeathProtectionService.ClearProviders();
        }
    }

    [Fact]
    public void DeathProtectionService_Canary_CheckProtectionWithNonMatchingProviderReturnsNotProtected()
    {
        DeathProtectionService.ClearProviders();
        try
        {
            // Provider says it cannot protect - should return NotProtected.
            DeathProtectionService.Register(new StubDeathProtectionProvider(DeathProtectionPriority.Reprieve, canProtect: false));

            var request = new DeathProtectionRequest("Player1", "TestSource", 10, IsUnavoidable: false);
            var result = DeathProtectionService.CheckProtection(request);

            Assert.Equal(DeathProtectionResult.NotProtected, result);
        }
        finally
        {
            DeathProtectionService.ClearProviders();
        }
    }

    /// <summary>
    /// Test provider for DeathProtectionService behavioral canary tests.
    /// </summary>
    private sealed class StubDeathProtectionProvider : IDeathProtectionProvider
    {
        private readonly bool _canProtect;

        public DeathProtectionPriority Priority { get; }
        public int CallCount { get; private set; }

        public StubDeathProtectionProvider(DeathProtectionPriority priority, bool canProtect)
        {
            Priority = priority;
            _canProtect = canProtect;
        }

        public bool CanProtect(DeathProtectionRequest request)
        {
            CallCount++;
            return _canProtect;
        }
    }
}
