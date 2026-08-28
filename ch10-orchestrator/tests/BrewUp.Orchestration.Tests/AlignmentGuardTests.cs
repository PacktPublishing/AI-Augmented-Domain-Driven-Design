using BrewUp.Orchestration;

namespace BrewUp.Orchestration.Tests;

public sealed class AlignmentGuardTests
{
    private static readonly ArtifactEnvelope AcceptedFacts =
        TestFixtures.Accepted("accepted-facts");

    [Fact]
    public void SpecialistMayChooseWordingInsideItsContract()
    {
        var issues = AlignmentGuard.Check(
            Candidate(),
            new CandidateAlignmentMetadata(
                Terms:
                [
                    "hold"
                ],
                DecisionTypes: []),
            TestFixtures.Stages[1],
            Chapter10Policies.Storyteller(),
            Chapter10Policies.StorytellerAlignment(
                AcceptedFacts));

        Assert.Empty(issues);
    }

    [Fact]
    public void SpecialistCannotChangeItsArtifactKind()
    {
        var issues = AlignmentGuard.Check(
            Candidate(
                kind: "accepted-scenarios"),
            new CandidateAlignmentMetadata(
                ["hold"],
                []),
            TestFixtures.Stages[1],
            Chapter10Policies.Storyteller(),
            Chapter10Policies.StorytellerAlignment(
                AcceptedFacts));

        Assert.Contains(
            issues,
            issue =>
                issue.Code ==
                "wrong-artifact-kind");
    }

    [Fact]
    public void UnresolvedQuestionCannotDisappear()
    {
        var issues = AlignmentGuard.Check(
            Candidate(
                unresolved: []),
            new CandidateAlignmentMetadata(
                ["hold"],
                []),
            TestFixtures.Stages[1],
            Chapter10Policies.Storyteller(),
            Chapter10Policies.StorytellerAlignment(
                AcceptedFacts));

        Assert.Contains(
            issues,
            issue =>
                issue.Code ==
                "dropped-unresolved");
    }

    [Fact]
    public void SpecialistCannotClaimAProhibitedDecision()
    {
        var issues = AlignmentGuard.Check(
            Candidate(),
            new CandidateAlignmentMetadata(
                ["hold"],
                ["short-quantity-policy"]),
            TestFixtures.Stages[1],
            Chapter10Policies.Storyteller(),
            Chapter10Policies.StorytellerAlignment(
                AcceptedFacts));

        Assert.Contains(
            issues,
            issue =>
                issue.Code ==
                "prohibited-decision");
    }

    [Fact]
    public void UnknownEvidenceIsRejectedMechanically()
    {
        var issues = AlignmentGuard.Check(
            Candidate(
                evidence:
                [
                    "OBS-01",
                    "OBS-999"
                ]),
            new CandidateAlignmentMetadata(
                ["hold"],
                []),
            TestFixtures.Stages[1],
            Chapter10Policies.Storyteller(),
            Chapter10Policies.StorytellerAlignment(
                AcceptedFacts));

        Assert.Contains(
            issues,
            issue =>
                issue.Code ==
                "unknown-evidence");
    }

    private static ArtifactEnvelope Candidate(
        string kind = "candidate-scenarios",
        string[]? evidence = null,
        string[]? unresolved = null) => new(
            WorkflowId: "brewup-stock-01",
            Producer: "storyteller",
            Kind: kind,
            Path: "candidate.md",
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
            Attempt: 1,
            ContentSha256:
                TestFixtures.CandidateHash);
}
