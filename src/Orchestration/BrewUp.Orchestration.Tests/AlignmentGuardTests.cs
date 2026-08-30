using BrewUp.Orchestration.Application;

namespace BrewUp.Orchestration.Tests;

public sealed class AlignmentGuardTests
{
    private static readonly LoopStage StorytellerStage =
        TestFixtures.Stages[1];

    private static readonly SpecialistGovernance Governance =
        BrewUpGovernanceDefaults.Storyteller();

    private static readonly ArtifactEnvelope AcceptedFacts =
        TestFixtures.Accepted(
            "accepted-facts",
            evidence:
            [
                "OBS-01",
                "OBS-02",
                "OBS-03"
            ]);

    private static readonly AlignmentContext Alignment =
        Governance.Alignment with
        {
            AcceptedArtifacts =
            [
                AcceptedFacts
            ]
        };

    [Fact]
    public void ApprovedWordingPassesEvenTheStrictestPolicy()
    {
        var candidate = Candidate(
            terms:
            [
                "hold"
            ],
            decisions: []);

        var issues = AlignmentGuard.Check(
            candidate,
            candidate.AlignmentMetadata!,
            StorytellerStage,
            Governance.Policy with
            {
                MayIntroduceNewTerms = false
            },
            Alignment);

        Assert.Empty(issues);
    }

    [Fact]
    public void SpecialistCannotChangeArtifactKind()
    {
        var candidate = Candidate(
            kind: "accepted-scenarios");

        var issues = AlignmentGuard.Check(
            candidate,
            candidate.AlignmentMetadata!,
            StorytellerStage,
            Governance.Policy,
            Alignment);

        Assert.Contains(
            issues,
            issue =>
                issue.Code ==
                "wrong-artifact-kind");
    }

    [Fact]
    public void UnknownEvidenceIsRejectedMechanically()
    {
        var candidate = Candidate(
            evidence:
            [
                "OBS-01",
                "OBS-999"
            ]);

        var issues = AlignmentGuard.Check(
            candidate,
            candidate.AlignmentMetadata!,
            StorytellerStage,
            Governance.Policy,
            Alignment);

        Assert.Contains(
            issues,
            issue =>
                issue.Code ==
                "unknown-evidence");
    }

    [Fact]
    public void UnresolvedQuestionCannotDisappear()
    {
        var candidate = Candidate(
            unresolved: []);

        var issues = AlignmentGuard.Check(
            candidate,
            candidate.AlignmentMetadata!,
            StorytellerStage,
            Governance.Policy,
            Alignment);

        Assert.Contains(
            issues,
            issue =>
                issue.Code ==
                "dropped-unresolved");
    }

    [Fact]
    public void SpecialistCannotClaimAProhibitedDecision()
    {
        var candidate = Candidate(
            decisions:
            [
                "short-quantity-policy"
            ]);

        var issues = AlignmentGuard.Check(
            candidate,
            candidate.AlignmentMetadata!,
            StorytellerStage,
            Governance.Policy,
            Alignment);

        Assert.Contains(
            issues,
            issue =>
                issue.Code ==
                "prohibited-decision");
    }

    private static ArtifactEnvelope Candidate(
        string kind = "candidate-scenarios",
        string[]? evidence = null,
        string[]? unresolved = null,
        string[]? terms = null,
        string[]? decisions = null) =>
        new(
            WorkflowId:
                "brewup-stock-01",
            Producer:
                "storyteller",
            Kind:
                kind,
            Path:
                "candidate.md",
            EvidenceRefs:
                evidence ??
                [
                    "OBS-01",
                    "OBS-02"
                ],
            Unresolved:
                unresolved ??
                [
                    "ES-17"
                ],
            Attempt:
                1,
            ContentSha256:
                TestFixtures.CandidateHash,
            AlignmentMetadata:
                new CandidateAlignmentMetadata(
                    terms ?? ["hold"],
                    decisions ?? []));
}
