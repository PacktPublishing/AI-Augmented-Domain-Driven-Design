using BrewUp.Orchestration.Application;
using BrewUp.Orchestration.Infrastructure;

namespace BrewUp.Orchestration.Tests;

public sealed class BrewUpOrchestrationLoopTests
{
    [Fact]
    public async Task HappyPathReachesCompletedInFourAcceptedStages()
    {
        var (loop, _) = CreateLoop();

        await RunAndAcceptAll(loop);

        Assert.Equal(ProtocolStatus.Completed, loop.State.Status);
        Assert.Equal(4, loop.State.AcceptedArtifacts.Length);
        Assert.Equal(["ES-17"], loop.State.PreservedUnresolved);
    }

    [Fact]
    public async Task ReferralRerunsOnlyTheCurrentSpecialist()
    {
        var (loop, runner) = CreateLoop();

        await loop.RunCurrentAsync("run-1");
        loop.AcceptCurrent("gate-1", TestFixtures.Accepted("accepted-facts"));
        await loop.RunCurrentAsync("run-storyteller-1");
        loop.ReferCurrent(
            "refer-storyteller",
            "Separate the observed shortage from the replenishment policy.");
        await loop.RunCurrentAsync("run-storyteller-2");

        Assert.Equal(
            ["eventstormer", "storyteller", "storyteller"],
            runner.Invocations);
        Assert.Equal(1, loop.State.StageIndex);
        Assert.Equal(2, loop.State.Candidate!.Attempt);
        Assert.DoesNotContain("command-event-writer", runner.Invocations);
    }

    [Fact]
    public async Task RejectedGateStopsTheLoop()
    {
        var (loop, runner) = CreateLoop();

        await loop.RunCurrentAsync("run-1");
        var rejected = loop.RejectCurrent("reject-1", "Evidence is unsupported.");
        var afterStop = await loop.RunCurrentAsync("run-2");

        Assert.True(rejected.Applied);
        Assert.Equal(ProtocolStatus.Stopped, loop.State.Status);
        Assert.False(afterStop.Applied);
        Assert.Single(runner.Invocations);
    }

    [Fact]
    public async Task WrongPrimaryArtifactBlocksBeforeRunnerInvocation()
    {
        var runner = new RecordingSpecialistRunner();
        var wrongInput = TestFixtures.RawEvidence with { Kind = "candidate-facts" };
        var loop = new BrewUpOrchestrationLoop(
            "brewup-stock-01",
            TestFixtures.Stages,
            wrongInput,
            runner,
            new SteppingClock());

        var result = await loop.RunCurrentAsync("run-1");

        Assert.False(result.Applied);
        Assert.Contains("requires raw-evidence", result.Message);
        Assert.Empty(runner.Invocations);
    }

    [Fact]
    public async Task DuplicateRunCommandDoesNotInvokeTheSpecialistTwice()
    {
        var (loop, runner) = CreateLoop();

        var first = await loop.RunCurrentAsync("run-1");
        var replay = await loop.RunCurrentAsync("run-1");

        Assert.True(first.Applied);
        Assert.False(replay.Applied);
        Assert.StartsWith("IGNORED", replay.Message);
        Assert.Single(runner.Invocations);
    }

    [Fact]
    public async Task CandidateArtifactCannotUnlockTheNextStage()
    {
        var (loop, runner) = CreateLoop();

        await loop.RunCurrentAsync("run-1");
        var result = loop.AcceptCurrent(
            "gate-1",
            TestFixtures.Accepted("candidate-facts"));

        Assert.False(result.Applied);
        Assert.Equal(0, loop.State.StageIndex);
        Assert.Equal(ProtocolStatus.AwaitingReview, loop.State.Status);
        Assert.DoesNotContain("storyteller", runner.Invocations);
    }

    [Fact]
    public async Task TracePreservesChronologyCausationAndUnresolvedItems()
    {
        var (loop, _) = CreateLoop();

        await RunAndAcceptAll(loop);

        Assert.Equal(8, loop.Trace.Count);
        Assert.Equal(Enumerable.Range(1, 8), loop.Trace.Select(entry => entry.Sequence));
        Assert.All(loop.Trace, entry => Assert.Contains("ES-17", entry.Unresolved));
        Assert.Equal("workflow-start", loop.Trace[0].CausationId);
        for (var index = 1; index < loop.Trace.Count; index++)
            Assert.Equal(loop.Trace[index - 1].MessageId, loop.Trace[index].CausationId);
    }

    [Fact]
    public async Task ContextMapperReceivesOriginalEvidenceAndTheAcceptedChain()
    {
        var (loop, runner) = CreateLoop();

        for (var index = 0; index < 3; index++)
        {
            await loop.RunCurrentAsync($"run-{index + 1}");
            loop.AcceptCurrent(
                $"gate-{index + 1}",
                TestFixtures.Accepted(TestFixtures.Stages[index].AcceptedKind));
        }

        await loop.RunCurrentAsync("run-4");
        var contextMapperInput = runner.Inputs[3];

        Assert.Equal("accepted-behavior", contextMapperInput.Primary.Kind);
        Assert.Equal(
            ["raw-evidence", "accepted-facts", "accepted-scenarios"],
            contextMapperInput.Context.Select(artifact => artifact.Kind));
    }

    [Fact]
    public async Task PersistedSnapshotResumesWithoutRepeatingTheRunCommand()
    {
        var directory = Path.Combine(
            Path.GetTempPath(),
            $"brewup-snapshot-{Guid.NewGuid():N}");
        try
        {
            var store = new JsonWorkflowStore(directory);
            var firstRunner = new RecordingSpecialistRunner();
            var first = new BrewUpOrchestrationLoop(
                "brewup-stock-01",
                TestFixtures.Stages,
                TestFixtures.RawEvidence,
                firstRunner,
                new SteppingClock(),
                store);

            await first.RunCurrentAsync("run-1");

            var resumedRunner = new RecordingSpecialistRunner();
            var resumed = new BrewUpOrchestrationLoop(
                "brewup-stock-01",
                TestFixtures.Stages,
                TestFixtures.RawEvidence,
                resumedRunner,
                new SteppingClock(),
                store);
            var replay = await resumed.RunCurrentAsync("run-1");

            Assert.Equal(ProtocolStatus.AwaitingReview, resumed.State.Status);
            Assert.Single(resumed.Trace);
            Assert.False(replay.Applied);
            Assert.StartsWith("IGNORED", replay.Message);
            Assert.Empty(resumedRunner.Invocations);
        }
        finally
        {
            if (Directory.Exists(directory))
                Directory.Delete(directory, recursive: true);
        }
    }

    private static (BrewUpOrchestrationLoop Loop, RecordingSpecialistRunner Runner)
        CreateLoop()
    {
        var runner = new RecordingSpecialistRunner();
        var loop = new BrewUpOrchestrationLoop(
            "brewup-stock-01",
            TestFixtures.Stages,
            TestFixtures.RawEvidence,
            runner,
            new SteppingClock());
        return (loop, runner);
    }

    private static async Task RunAndAcceptAll(BrewUpOrchestrationLoop loop)
    {
        for (var index = 0; index < TestFixtures.Stages.Length; index++)
        {
            await loop.RunCurrentAsync($"run-{index + 1}");
            loop.AcceptCurrent(
                $"gate-{index + 1}",
                TestFixtures.Accepted(TestFixtures.Stages[index].AcceptedKind));
        }
    }
}

internal static class TestFixtures
{
    internal const string CandidateHash =
        "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
    internal const string AcceptedHash =
        "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb";
    internal const string DecisionHash =
        "cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc";
    internal const string UnrelatedHash =
        "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd";

    internal static readonly LoopStage[] Stages =
    [
        new("eventstormer", "raw-evidence", "candidate-facts", "accepted-facts"),
        new("storyteller", "accepted-facts", "candidate-scenarios", "accepted-scenarios"),
        new("command-event-writer", "accepted-scenarios", "candidate-behavior", "accepted-behavior"),
        new("context-mapper", "accepted-behavior", "candidate-context-map", "accepted-context-map")
    ];

    internal static readonly ProtocolRoute[] Routes = Stages
        .Select(stage => new ProtocolRoute(
            stage.Specialist, stage.CandidateKind, stage.AcceptedKind))
        .ToArray();

    internal static readonly ArtifactEnvelope RawEvidence = new(
        "brewup-stock-01",
        "evidence-store",
        "raw-evidence",
        "docs/ch09/run-pack/stock-walkthrough.md",
        ["OBS-01", "OBS-02", "OBS-03"],
        ["ES-17"],
        0,
        CandidateHash);

    internal static ArtifactEnvelope Accepted(
        string kind,
        int attempt = 1,
        string sourceHash = CandidateHash,
        string[]? evidence = null) => new(
        "brewup-stock-01",
        "human-reviewer",
        kind,
        $"artifacts/{kind}.md",
        evidence ?? ["OBS-01"],
        ["ES-17"],
        attempt,
        AcceptedHash,
        [sourceHash],
        DecisionHash);
}

internal sealed class RecordingSpecialistRunner : ISpecialistRunner
{
    internal List<string> Invocations { get; } = [];
    internal List<SpecialistInput> Inputs { get; } = [];

    public Task<ArtifactEnvelope> RunAsync(
        LoopStage stage,
        SpecialistInput input,
        int attempt,
        CancellationToken cancellationToken = default)
    {
        Invocations.Add(stage.Specialist);
        Inputs.Add(input);
        return Task.FromResult(new ArtifactEnvelope(
            input.Primary.WorkflowId,
            stage.Specialist,
            stage.CandidateKind,
            $"artifacts/{stage.CandidateKind}-attempt-{attempt}.md",
            input.Primary.EvidenceRefs,
            input.Primary.Unresolved,
            attempt,
            TestFixtures.CandidateHash));
    }
}

internal sealed class SteppingClock : IClock
{
    private DateTimeOffset _current = new(2026, 8, 26, 8, 0, 0, TimeSpan.Zero);

    public DateTimeOffset UtcNow
    {
        get
        {
            var result = _current;
            _current = _current.AddSeconds(1);
            return result;
        }
    }
}
