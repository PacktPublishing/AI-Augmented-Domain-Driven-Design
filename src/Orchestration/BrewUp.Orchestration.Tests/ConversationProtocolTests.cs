using BrewUp.Orchestration.Application;

namespace BrewUp.Orchestration.Tests;

public sealed class ConversationProtocolTests
{
    private readonly ConversationProtocol _protocol = new(TestFixtures.Routes);

    [Fact]
    public void AcceptedCandidateAdvancesToStoryteller()
    {
        var awaitingReview = AwaitingReview();

        var result = _protocol.Handle(
            awaitingReview,
            Accepted("gate-1", "candidate-1", ["ES-17"]));

        Assert.True(result.Applied);
        Assert.Equal(ProtocolStatus.Running, result.State.Status);
        Assert.Equal(1, result.State.StageIndex);
        Assert.Equal(["ES-17"], result.State.PreservedUnresolved);
        Assert.Contains("storyteller may run", result.Message);
    }

    [Fact]
    public void ReferralReturnsControlWithoutAdvancing()
    {
        var awaitingReview = AwaitingReview();
        var referral = new ProtocolMessage(
            "refer-1", "brewup-stock-01", "candidate-1",
            ProtocolMessageKind.GateReferred,
            "human-reviewer", "orchestrator",
            Reason: "Separate observed facts from policy.");

        var referred = _protocol.Handle(awaitingReview, referral);
        var corrected = _protocol.Handle(
            referred.State,
            Candidate("candidate-2", "refer-1", attempt: 2));

        Assert.Equal(0, corrected.State.StageIndex);
        Assert.Equal(ProtocolStatus.AwaitingReview, corrected.State.Status);
        Assert.Equal(2, corrected.State.Candidate!.Attempt);
    }

    [Fact]
    public void MissingDomainAuthorityStopsTheConversation()
    {
        var state = _protocol.Start("brewup-stock-01");
        var request = new ProtocolMessage(
            "authority-1", "brewup-stock-01", "workflow-start",
            ProtocolMessageKind.AuthorityRequired,
            "eventstormer", "orchestrator",
            Reason: "No authorized owner can decide the short-quantity policy.");

        var result = _protocol.Handle(state, request);

        Assert.True(result.Applied);
        Assert.Equal(ProtocolStatus.Stopped, result.State.Status);
        Assert.Contains("short-quantity policy", result.Message);
    }

    [Fact]
    public void DuplicateMessageIsIgnored()
    {
        var awaitingReview = AwaitingReview();

        var duplicate = _protocol.Handle(
            awaitingReview,
            Candidate("candidate-1", "workflow-start"));

        Assert.False(duplicate.Applied);
        Assert.StartsWith("IGNORED", duplicate.Message);
        Assert.Same(awaitingReview, duplicate.State);
    }

    [Fact]
    public void WrongCausationIsBlocked()
    {
        var state = _protocol.Start("brewup-stock-01");

        var result = _protocol.Handle(
            state,
            Candidate("candidate-1", "unknown-message"));

        Assert.Equal(
            "BLOCKED: expected causation workflow-start",
            result.Message);
    }

    [Fact]
    public void AcceptedArtifactCannotDropAnUnresolvedItem()
    {
        var awaitingReview = AwaitingReview();

        var result = _protocol.Handle(
            awaitingReview,
            Accepted("gate-1", "candidate-1", []));

        Assert.False(result.Applied);
        Assert.Contains("drops unresolved: ES-17", result.Message);
        Assert.Equal(0, result.State.StageIndex);
    }

    [Fact]
    public void CandidateFromTheWrongSpecialistIsBlocked()
    {
        var state = _protocol.Start("brewup-stock-01");
        var message = Candidate("candidate-1", "workflow-start") with
        {
            Sender = "storyteller",
            Artifact = Candidate("candidate-1", "workflow-start").Artifact! with
            {
                Producer = "storyteller"
            }
        };

        var result = _protocol.Handle(state, message);

        Assert.False(result.Applied);
        Assert.Equal("BLOCKED: expected sender eventstormer", result.Message);
    }

    [Fact]
    public void AcceptedArtifactMustBeLinkedToTheCandidateUnderReview()
    {
        var awaitingReview = AwaitingReview();

        var result = _protocol.Handle(
            awaitingReview,
            Accepted(
                "gate-1",
                "candidate-1",
                ["ES-17"],
                sourceHashes: [TestFixtures.UnrelatedHash]));

        Assert.False(result.Applied);
        Assert.Contains("does not cite the candidate under review", result.Message);
    }

    [Fact]
    public void AcceptedArtifactMustCarryTheHumanDecisionHash()
    {
        var awaitingReview = AwaitingReview();

        var result = _protocol.Handle(
            awaitingReview,
            Accepted(
                "gate-1",
                "candidate-1",
                ["ES-17"],
                decisionHash: null));

        Assert.False(result.Applied);
        Assert.Contains("human decision hash is missing", result.Message);
    }

    private ProtocolState AwaitingReview()
    {
        var state = _protocol.Start("brewup-stock-01");
        return _protocol.Handle(
            state,
            Candidate("candidate-1", "workflow-start")).State;
    }

    private static ProtocolMessage Candidate(
        string messageId,
        string causationId,
        int attempt = 1) => new(
            messageId,
            "brewup-stock-01",
            causationId,
            ProtocolMessageKind.CandidateProduced,
            "eventstormer",
            "orchestrator",
            new ArtifactEnvelope(
                "brewup-stock-01",
                "eventstormer",
                "candidate-facts",
                "artifacts/candidate-facts.md",
                ["OBS-01"],
                ["ES-17"],
                attempt,
                TestFixtures.CandidateHash));

    private static ProtocolMessage Accepted(
        string messageId,
        string causationId,
        string[] unresolved,
        string[]? sourceHashes = null,
        string? decisionHash = TestFixtures.DecisionHash) => new(
            messageId,
            "brewup-stock-01",
            causationId,
            ProtocolMessageKind.GateAccepted,
            "human-reviewer",
            "orchestrator",
            new ArtifactEnvelope(
                "brewup-stock-01",
                "human-reviewer",
                "accepted-facts",
                "docs/ch09/runs/9.2-eventstormer/accepted-facts.md",
                ["OBS-01"],
                unresolved,
                1,
                TestFixtures.AcceptedHash,
                sourceHashes ?? [TestFixtures.CandidateHash],
                decisionHash));
}
