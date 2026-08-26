namespace BrewUp.Orchestration;

public sealed record ArtifactEnvelope(
    string WorkflowId,
    string Producer,
    string Kind,
    string Path,
    string[] EvidenceRefs,
    string[] Unresolved,
    int Attempt);

public enum ProtocolMessageKind
{
    CandidateProduced,
    GateAccepted,
    GateReferred,
    GateRejected,
    AuthorityRequired
}

public sealed record ProtocolMessage(
    string MessageId,
    string WorkflowId,
    string CausationId,
    ProtocolMessageKind Kind,
    string Sender,
    string Recipient,
    ArtifactEnvelope? Artifact = null,
    string? Reason = null);

public enum ProtocolStatus
{
    Running,
    AwaitingReview,
    CorrectionRequired,
    Stopped,
    Completed
}

public sealed record ProtocolState(
    string WorkflowId,
    int StageIndex,
    ProtocolStatus Status,
    string LastMessageId,
    ArtifactEnvelope? Candidate,
    ArtifactEnvelope[] AcceptedArtifacts,
    string[] PreservedUnresolved,
    string[] ProcessedMessageIds,
    string? StopReason = null);

public sealed record ProtocolRoute(
    string Specialist,
    string CandidateKind,
    string AcceptedKind);

public sealed record ProtocolResult(
    ProtocolState State,
    bool Applied,
    string Message);

public sealed class ConversationProtocol
{
    private readonly ProtocolRoute[] _routes;

    public ConversationProtocol(IEnumerable<ProtocolRoute> routes)
    {
        _routes = routes.ToArray();
        if (_routes.Length == 0)
            throw new ArgumentException("At least one route is required.", nameof(routes));
    }

    public ProtocolState Start(string workflowId) => new(
        workflowId,
        StageIndex: 0,
        ProtocolStatus.Running,
        LastMessageId: "workflow-start",
        Candidate: null,
        AcceptedArtifacts: [],
        PreservedUnresolved: [],
        ProcessedMessageIds: []);

    public ProtocolResult Handle(
        ProtocolState state,
        ProtocolMessage message)
    {
        if (state.ProcessedMessageIds.Contains(
                message.MessageId,
                StringComparer.Ordinal))
            return Ignored(
                state,
                $"message {message.MessageId} was already applied");

        if (!string.Equals(
                message.WorkflowId,
                state.WorkflowId,
                StringComparison.Ordinal))
            return Blocked(state, "workflow id does not match");

        if (!string.Equals(
                message.Recipient,
                "orchestrator",
                StringComparison.Ordinal))
            return Blocked(
                state,
                "protocol messages must target the orchestrator");

        if (!string.Equals(
                message.CausationId,
                state.LastMessageId,
                StringComparison.Ordinal))
            return Blocked(
                state,
                $"expected causation {state.LastMessageId}");

        if (state.Status is ProtocolStatus.Stopped or ProtocolStatus.Completed)
            return Blocked(state, $"workflow is {state.Status}");

        return message.Kind switch
        {
            ProtocolMessageKind.CandidateProduced => RecordCandidate(state, message),
            ProtocolMessageKind.GateAccepted => AcceptCandidate(state, message),
            ProtocolMessageKind.GateReferred => ReferCandidate(state, message),
            ProtocolMessageKind.GateRejected => StopFromGate(state, message),
            ProtocolMessageKind.AuthorityRequired => StopForAuthority(state, message),
            _ => Blocked(state, "unsupported message kind")
        };
    }

    private ProtocolResult RecordCandidate(
        ProtocolState state,
        ProtocolMessage message)
    {
        if (state.Status is not (
                ProtocolStatus.Running or ProtocolStatus.CorrectionRequired))
            return Blocked(
                state,
                $"CandidateProduced is not valid while {state.Status}");

        var route = CurrentRoute(state);
        if (!string.Equals(message.Sender, route.Specialist, StringComparison.Ordinal))
            return Blocked(state, $"expected sender {route.Specialist}");

        if (message.Artifact is not { } artifact)
            return Blocked(state, "candidate artifact is missing");

        if (!string.Equals(artifact.WorkflowId, state.WorkflowId, StringComparison.Ordinal))
            return Blocked(state, "candidate workflow id does not match");

        if (!string.Equals(artifact.Producer, message.Sender, StringComparison.Ordinal))
            return Blocked(state, "candidate producer does not match sender");

        if (!string.Equals(artifact.Kind, route.CandidateKind, StringComparison.Ordinal))
            return Blocked(state, $"expected artifact kind {route.CandidateKind}");

        if (artifact.EvidenceRefs.Length == 0)
            return Blocked(state, "candidate has no evidence references");

        var expectedAttempt = state.Status == ProtocolStatus.CorrectionRequired
            ? (state.Candidate?.Attempt ?? 0) + 1
            : 1;
        if (artifact.Attempt != expectedAttempt)
            return Blocked(state, $"expected attempt {expectedAttempt}");

        var lost = state.PreservedUnresolved
            .Except(artifact.Unresolved, StringComparer.Ordinal)
            .ToArray();
        if (lost.Length > 0)
            return Blocked(
                state,
                $"candidate drops unresolved: {string.Join(", ", lost)}");

        var next = AppliedState(
            state with
            {
                Status = ProtocolStatus.AwaitingReview,
                Candidate = artifact
            },
            message);
        return Applied(
            next,
            $"awaiting review of {artifact.Kind}");
    }

    private ProtocolResult AcceptCandidate(
        ProtocolState state,
        ProtocolMessage message)
    {
        if (state.Status != ProtocolStatus.AwaitingReview)
            return Blocked(
                state,
                $"GateAccepted is not valid while {state.Status}");

        if (!string.Equals(
                message.Sender,
                "human-reviewer",
                StringComparison.Ordinal))
            return Blocked(state, "only human-reviewer may accept a candidate");

        if (state.Candidate is null)
            return Blocked(state, "there is no candidate to accept");

        if (message.Artifact is not { } accepted)
            return Blocked(state, "accepted artifact is missing");

        var route = CurrentRoute(state);
        if (!string.Equals(accepted.Kind, route.AcceptedKind, StringComparison.Ordinal))
            return Blocked(state, $"expected accepted kind {route.AcceptedKind}");

        if (!string.Equals(accepted.WorkflowId, state.WorkflowId, StringComparison.Ordinal))
            return Blocked(state, "accepted artifact workflow id does not match");

        if (accepted.EvidenceRefs.Length == 0)
            return Blocked(state, "accepted artifact has no evidence references");

        var lost = state.Candidate.Unresolved
            .Except(accepted.Unresolved, StringComparer.Ordinal)
            .ToArray();
        if (lost.Length > 0)
            return Blocked(
                state,
                $"accepted artifact drops unresolved: {string.Join(", ", lost)}");

        var isLastStage = state.StageIndex == _routes.Length - 1;
        var next = AppliedState(
            state with
            {
                StageIndex = isLastStage
                    ? state.StageIndex
                    : state.StageIndex + 1,
                Status = isLastStage
                    ? ProtocolStatus.Completed
                    : ProtocolStatus.Running,
                Candidate = null,
                AcceptedArtifacts = [.. state.AcceptedArtifacts, accepted],
                PreservedUnresolved = accepted.Unresolved
            },
            message);

        return isLastStage
            ? Applied(next, "workflow completed")
            : Applied(
                next,
                $"accepted {accepted.Kind}; {CurrentRoute(next).Specialist} may run");
    }

    private ProtocolResult ReferCandidate(
        ProtocolState state,
        ProtocolMessage message)
    {
        if (state.Status != ProtocolStatus.AwaitingReview)
            return Blocked(
                state,
                $"GateReferred is not valid while {state.Status}");

        if (!string.Equals(
                message.Sender,
                "human-reviewer",
                StringComparison.Ordinal))
            return Blocked(state, "only human-reviewer may refer a candidate");

        if (state.Candidate is null)
            return Blocked(state, "there is no candidate to refer");

        if (string.IsNullOrWhiteSpace(message.Reason))
            return Blocked(state, "referral reason is required");

        var next = AppliedState(
            state with { Status = ProtocolStatus.CorrectionRequired },
            message);
        return Applied(next, $"correction required: {message.Reason}");
    }

    private ProtocolResult StopFromGate(
        ProtocolState state,
        ProtocolMessage message)
    {
        if (state.Status != ProtocolStatus.AwaitingReview)
            return Blocked(
                state,
                $"GateRejected is not valid while {state.Status}");

        if (!string.Equals(
                message.Sender,
                "human-reviewer",
                StringComparison.Ordinal))
            return Blocked(state, "only human-reviewer may reject a candidate");

        if (string.IsNullOrWhiteSpace(message.Reason))
            return Blocked(state, "rejection reason is required");

        var next = AppliedState(
            state with
            {
                Status = ProtocolStatus.Stopped,
                StopReason = message.Reason
            },
            message);
        return Applied(next, $"stopped: {message.Reason}");
    }

    private ProtocolResult StopForAuthority(
        ProtocolState state,
        ProtocolMessage message)
    {
        if (state.Status is not (
                ProtocolStatus.Running or ProtocolStatus.CorrectionRequired))
            return Blocked(
                state,
                $"AuthorityRequired is not valid while {state.Status}");

        var route = CurrentRoute(state);
        if (!string.Equals(message.Sender, route.Specialist, StringComparison.Ordinal))
            return Blocked(state, $"expected sender {route.Specialist}");

        if (string.IsNullOrWhiteSpace(message.Reason))
            return Blocked(state, "authority reason is required");

        var next = AppliedState(
            state with
            {
                Status = ProtocolStatus.Stopped,
                StopReason = message.Reason
            },
            message);
        return Applied(next, $"stopped: {message.Reason}");
    }

    private ProtocolRoute CurrentRoute(ProtocolState state) =>
        _routes[state.StageIndex];

    private static ProtocolState AppliedState(
        ProtocolState state,
        ProtocolMessage message) => state with
        {
            LastMessageId = message.MessageId,
            ProcessedMessageIds = [.. state.ProcessedMessageIds, message.MessageId]
        };

    private static ProtocolResult Applied(ProtocolState state, string message) =>
        new(state, true, $"APPLIED: {message}");

    private static ProtocolResult Blocked(ProtocolState state, string message) =>
        new(state, false, $"BLOCKED: {message}");

    private static ProtocolResult Ignored(ProtocolState state, string message) =>
        new(state, false, $"IGNORED: {message}");
}
