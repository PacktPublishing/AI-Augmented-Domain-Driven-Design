using BrewUp.Orchestration.Application;
using BrewUp.Orchestration.Infrastructure;

namespace BrewUp.Orchestration.Tests;

public sealed class GovernedOrchestrationLoopTests
{
    [Fact]
    public async Task ControlledDriftCorrectionIsLocalAndTraceable()
    {
        var root = FindRepositoryRoot();
        var runner = new ControlledRunner(root);

        var loop = CreateLoop(
            runner,
            new GovernanceCatalog(
            [
                StorytellerGovernance()
            ]));

        await loop.RunCurrentAsync("run-1");

        loop.AcceptCurrent(
            "gate-1",
            TestFixtures.Accepted(
                "accepted-facts",
                evidence:
                [
                    "OBS-01",
                    "OBS-02",
                    "OBS-03"
                ]));

        await loop.RunCurrentAsync(
            "run-storyteller-alignment-1");

        Assert.Equal(
            ProtocolStatus.AwaitingReview,
            loop.State.Status);

        var reviewer =
            new MarkdownSemanticAlignmentReviewer(
                root);

        var assessment =
            await reviewer.ReviewAsync(
                loop.State.Candidate!,
                BrewUpGovernanceDefaults
                    .Storyteller()
                    .Alignment with
                    {
                        AcceptedArtifacts =
                            loop.State.AcceptedArtifacts
                    });

        loop.RecordAlignmentAssessment(
            "alignment-storyteller-1",
            assessment);

        loop.ReferCurrent(
            "refer-storyteller-alignment",
            "Do not turn Nadia's observed short-quantity practice " +
            "into the general Stock policy. Preserve both practices " +
            "and keep ES-17 unresolved.");

        await loop.RunCurrentAsync(
            "run-storyteller-alignment-2");

        Assert.Equal(
            [
                "eventstormer",
                "storyteller",
                "storyteller"
            ],
            runner.Invocations);

        Assert.Equal(
            2,
            loop.State.Candidate!.Attempt);

        Assert.Contains(
            "ES-17",
            loop.State.Candidate.Unresolved);

        Assert.Contains(
            loop.AlignmentTrace,
            entry =>
                entry.MessageId ==
                "alignment-storyteller-1" &&
                entry.Attempt == 1);

        Assert.Contains(
            "Do not turn Nadia's observed short-quantity practice",
            loop.Trace.Single(
                entry =>
                    entry.MessageId ==
                    "refer-storyteller-alignment")
                .Reason);

        Assert.Equal(
            1,
            loop.State.StageIndex);

        loop.AcceptCurrent(
            "gate-storyteller-alignment-2",
            TestFixtures.Accepted(
                "accepted-scenarios",
                attempt: 2,
                sourceHash:
                    loop.State.Candidate.ContentSha256,
                evidence:
                [
                    "OBS-01",
                    "OBS-02",
                    "OBS-03"
                ]));

        Assert.Equal(
            2,
            loop.State.StageIndex);
    }

    [Fact]
    public async Task CorrectionBudgetStopsThirdAttemptBeforeRunner()
    {
        var root = FindRepositoryRoot();
        var runner = new ControlledRunner(root);

        var loop = CreateLoop(
            runner,
            new GovernanceCatalog(
            [
                StorytellerGovernance()
            ]));

        await loop.RunCurrentAsync("run-1");

        loop.AcceptCurrent(
            "gate-1",
            TestFixtures.Accepted(
                "accepted-facts",
                evidence:
                [
                    "OBS-01",
                    "OBS-02",
                    "OBS-03"
                ]));

        await loop.RunCurrentAsync("story-1");
        loop.ReferCurrent(
            "refer-1",
            "First correction.");

        await loop.RunCurrentAsync("story-2");
        loop.ReferCurrent(
            "refer-2",
            "Second correction.");

        var stopped =
            await loop.RunCurrentAsync(
                "story-3");

        Assert.True(stopped.Applied);

        Assert.Equal(
            ProtocolStatus.Stopped,
            loop.State.Status);

        Assert.Contains(
            "correction-budget-exhausted",
            loop.State.StopReason);

        Assert.Equal(
            3,
            runner.Invocations.Count);

        Assert.Equal(
            2,
            runner.Invocations.Count(
                item =>
                    item == "storyteller"));
    }

    [Fact]
    public async Task TimeoutStopsWorkflowWithoutChangingAuthority()
    {
        var root = FindRepositoryRoot();
        var runner = new TimeoutRunner();

        GovernanceCatalog governance =
            new GovernanceCatalog(
            [
                StorytellerGovernance(
                    policy =>
                        policy with
                        {
                            Timeout =
                                TimeSpan.FromMilliseconds(
                                    20)
                        })
            ]);

        var loop = CreateLoop(
            runner,
            governance);

        await loop.RunCurrentAsync("run-1");

        loop.AcceptCurrent(
            "gate-1",
            TestFixtures.Accepted(
                "accepted-facts",
                evidence:
                [
                    "OBS-01",
                    "OBS-02",
                    "OBS-03"
                ]));

        var stopped =
            await loop.RunCurrentAsync(
                "story-timeout");

        Assert.True(stopped.Applied);

        Assert.Equal(
            ProtocolStatus.Stopped,
            loop.State.Status);

        Assert.Contains(
            "execution-timeout",
            loop.State.StopReason);

        Assert.Equal(
            "storyteller",
            loop.CurrentStage.Specialist);
    }

    [Fact]
    public async Task RetryKeepsTheSameToolBoundary()
    {
        var root = FindRepositoryRoot();
        var runner = new ControlledRunner(root);

        var tools =
            new SpecialistToolRegistry(
            [
                "read-accepted-artifact",
                "query-operational-system"
            ]);

        var governance =
            new GovernanceCatalog(
            [
                StorytellerGovernance()
            ],
            tools);

        var loop = CreateLoop(
            runner,
            governance);

        await loop.RunCurrentAsync("run-1");

        loop.AcceptCurrent(
            "gate-1",
            TestFixtures.Accepted(
                "accepted-facts",
                evidence:
                [
                    "OBS-01",
                    "OBS-02",
                    "OBS-03"
                ]));

        await loop.RunCurrentAsync("story-1");
        loop.ReferCurrent(
            "refer-1",
            "Keep the disagreement explicit.");
        await loop.RunCurrentAsync("story-2");

        var storytellerInputs =
            runner.Inputs
                .Where(
                    input =>
                        input.Primary.Kind ==
                        "accepted-facts")
                .ToArray();

        Assert.Equal(
            2,
            storytellerInputs.Length);

        Assert.All(
            storytellerInputs,
            input =>
                Assert.Equal(
                    [
                        "read-accepted-artifact"
                    ],
                    input.Tools));

        Assert.DoesNotContain(
            storytellerInputs
                .SelectMany(
                    input =>
                        input.Tools),
            tool =>
                tool ==
                "query-operational-system");
    }

    private static BrewUpOrchestrationLoop CreateLoop(
        ISpecialistRunner runner,
        GovernanceCatalog governance) =>
        new(
            "brewup-stock-01",
            TestFixtures.Stages,
            TestFixtures.RawEvidence,
            runner,
            new SteppingClock(),
            governance: governance);

    private static SpecialistGovernance StorytellerGovernance(
        Func<AutonomyPolicy, AutonomyPolicy>? changePolicy = null)
    {
        var specialist =
            BrewUpGovernanceDefaults
                .Storyteller();

        if (changePolicy is not null)
        {
            specialist = specialist with
            {
                Policy =
                    changePolicy(
                        specialist.Policy)
            };
        }

        return specialist;
    }

    private static string FindRepositoryRoot()
    {
        var directory =
            new DirectoryInfo(
                AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (Directory.Exists(
                    Path.Combine(
                        directory.FullName,
                        "docs",
                        "ch11")) &&
                Directory.Exists(
                    Path.Combine(
                        directory.FullName,
                        "src",
                        "Orchestration")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Repository root was not found.");
    }

    private sealed class ControlledRunner(
        string repositoryRoot)
        : ISpecialistRunner
    {
        internal List<string> Invocations { get; } = [];
        internal List<SpecialistInput> Inputs { get; } = [];

        public Task<ArtifactEnvelope> RunAsync(
            LoopStage stage,
            SpecialistInput input,
            int attempt,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Invocations.Add(stage.Specialist);
            Inputs.Add(input);

            if (stage.Specialist != "storyteller")
            {
                return Task.FromResult(
                    new ArtifactEnvelope(
                        input.Primary.WorkflowId,
                        stage.Specialist,
                        stage.CandidateKind,
                        $"artifacts/{stage.CandidateKind}-attempt-{attempt}.md",
                        input.Primary.EvidenceRefs,
                        input.Primary.Unresolved,
                        attempt,
                        TestFixtures.CandidateHash));
            }

            var path =
                "docs/ch11/runs/11.1-alignment/" +
                $"storyteller-attempt-{attempt}.md";

            var metadata =
                attempt == 1
                    ? new CandidateAlignmentMetadata(
                        ["reserve"],
                        [])
                    : new CandidateAlignmentMetadata(
                        ["set aside"],
                        []);

            return Task.FromResult(
                new ArtifactEnvelope(
                    input.Primary.WorkflowId,
                    stage.Specialist,
                    stage.CandidateKind,
                    path,
                    [
                        "OBS-01",
                        "OBS-02",
                        "OBS-03"
                    ],
                    [
                        "ES-17"
                    ],
                    attempt,
                    ArtifactHash.Sha256(
                        repositoryRoot,
                        path),
                    AlignmentMetadata:
                        metadata));
        }
    }

    private sealed class TimeoutRunner
        : ISpecialistRunner
    {
        public async Task<ArtifactEnvelope> RunAsync(
            LoopStage stage,
            SpecialistInput input,
            int attempt,
            CancellationToken cancellationToken = default)
        {
            if (stage.Specialist != "storyteller")
            {
                return new ArtifactEnvelope(
                    input.Primary.WorkflowId,
                    stage.Specialist,
                    stage.CandidateKind,
                    $"artifacts/{stage.CandidateKind}.md",
                    input.Primary.EvidenceRefs,
                    input.Primary.Unresolved,
                    attempt,
                    TestFixtures.CandidateHash);
            }

            await Task.Delay(
                TimeSpan.FromSeconds(10),
                cancellationToken);

            throw new InvalidOperationException(
                "The timeout should cancel this invocation.");
        }
    }
}
