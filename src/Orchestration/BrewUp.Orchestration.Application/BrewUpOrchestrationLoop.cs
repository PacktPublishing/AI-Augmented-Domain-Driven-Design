namespace BrewUp.Orchestration.Application;

public sealed record LoopStage(
    string Specialist,
    string InputKind,
    string CandidateKind,
    string AcceptedKind);

public sealed record SpecialistInput(
    ArtifactEnvelope Primary,
    ArtifactEnvelope[] Context,
    string[]? AllowedTools = null)
{
    public IReadOnlyList<string> Tools =>
        AllowedTools ?? [];
}

public interface ISpecialistRunner
{
    Task<ArtifactEnvelope> RunAsync(
        LoopStage stage,
        SpecialistInput input,
        int attempt,
        CancellationToken cancellationToken = default);
}

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}

public sealed record RunTraceEntry(
    int Sequence,
    DateTimeOffset RecordedAt,
    string Stage,
    string MessageId,
    string CausationId,
    string Event,
    ProtocolStatus Status,
    string? ArtifactPath,
    string[] Unresolved,
    string? Reason = null);

public sealed class BrewUpOrchestrationLoop
{
    private readonly LoopStage[] _stages;
    private readonly ArtifactEnvelope _initialInput;
    private readonly ISpecialistRunner _runner;
    private readonly IClock _clock;
    private readonly ConversationProtocol _protocol;
    private readonly IWorkflowStore _store;
    private readonly GovernanceCatalog? _governance;
    private readonly HashSet<string> _processedRunCommands;
    private readonly List<RunTraceEntry> _trace;
    private readonly List<AlignmentTraceEntry> _alignmentTrace;

    private ArtifactEnvelope _currentInput;

    public BrewUpOrchestrationLoop(
        string workflowId,
        IEnumerable<LoopStage> stages,
        ArtifactEnvelope initialInput,
        ISpecialistRunner runner,
        IClock clock,
        IWorkflowStore? store = null,
        GovernanceCatalog? governance = null)
    {
        _stages = stages.ToArray();

        if (_stages.Length == 0)
        {
            throw new ArgumentException(
                "At least one stage is required.",
                nameof(stages));
        }

        if (!string.Equals(
                workflowId,
                initialInput.WorkflowId,
                StringComparison.Ordinal))
        {
            throw new ArgumentException(
                "The initial input must belong to the workflow.",
                nameof(initialInput));
        }

        _initialInput = initialInput;
        _currentInput = initialInput;
        _runner = runner;
        _clock = clock;
        _store = store ?? NullWorkflowStore.Instance;
        _governance = governance;

        _protocol = new ConversationProtocol(
            _stages.Select(
                stage => new ProtocolRoute(
                    stage.Specialist,
                    stage.CandidateKind,
                    stage.AcceptedKind)));

        var snapshot = _store.Load(workflowId);

        if (snapshot is null)
        {
            State = _protocol.Start(workflowId);
            _processedRunCommands =
                new(StringComparer.Ordinal);
            _trace = [];
            _alignmentTrace = [];
            Persist();
        }
        else
        {
            if (!string.Equals(
                    snapshot.State.WorkflowId,
                    workflowId,
                    StringComparison.Ordinal))
            {
                throw new InvalidDataException(
                    "Snapshot workflow id does not match.");
            }

            State = snapshot.State;
            _currentInput = snapshot.CurrentInput;

            _processedRunCommands = new(
                snapshot.ProcessedRunCommands,
                StringComparer.Ordinal);

            _trace = [.. snapshot.Trace];

            _alignmentTrace =
                snapshot.AlignmentTrace is null
                    ? []
                    : [.. snapshot.AlignmentTrace];
        }
    }

    public ProtocolState State { get; private set; }

    public IReadOnlyList<RunTraceEntry> Trace => _trace;

    public IReadOnlyList<AlignmentTraceEntry>
        AlignmentTrace => _alignmentTrace;

    public LoopStage CurrentStage =>
        _stages[State.StageIndex];

    public async Task<ProtocolResult> RunCurrentAsync(
        string commandId,
        CancellationToken cancellationToken = default)
    {
        if (_processedRunCommands.Contains(commandId))
        {
            return Ignored(
                $"run command {commandId} was already applied");
        }

        if (State.Status is not (
                ProtocolStatus.Running or
                ProtocolStatus.CorrectionRequired))
        {
            return Blocked(
                $"cannot run a specialist while {State.Status}");
        }

        var stage = CurrentStage;

        if (!string.Equals(
                _currentInput.Kind,
                stage.InputKind,
                StringComparison.Ordinal))
        {
            return Blocked(
                $"{stage.Specialist} requires " +
                $"{stage.InputKind}, not {_currentInput.Kind}");
        }

        var attempt =
            State.Status == ProtocolStatus.CorrectionRequired
                ? (State.Candidate?.Attempt ?? 0) + 1
                : 1;

        var context =
            State.StageIndex == 0
                ? []
                : new[] { _initialInput }
                    .Concat(State.AcceptedArtifacts[..^1])
                    .ToArray();

        SpecialistGovernance? governedStage = null;
        string[] allowedTools = [];

        if (_governance is not null &&
            _governance.TryGet(
                stage.Specialist,
                out var configuredGovernance))
        {
            GovernanceCatalog.ValidateStage(
                stage,
                configuredGovernance);

            governedStage = configuredGovernance;

            allowedTools =
                _governance.ResolveTools(
                    configuredGovernance);
        }

        var input = new SpecialistInput(
            _currentInput,
            context,
            allowedTools);

        if (governedStage is not null)
        {
            var disallowedEvidenceKinds =
                _governance!.FindDisallowedEvidenceKinds(
                    input,
                    governedStage);

            if (disallowedEvidenceKinds.Length > 0)
            {
                return Blocked(
                    "autonomy policy forbids evidence kinds: " +
                    string.Join(
                        ", ",
                        disallowedEvidenceKinds));
            }
        }

        ArtifactEnvelope candidate;

        if (governedStage is null)
        {
            candidate = await _runner.RunAsync(
                stage,
                input,
                attempt,
                cancellationToken)
                .ConfigureAwait(false);
        }
        else
        {
            var execution =
                await BoundedSpecialistExecutor.RunAsync(
                    _runner,
                    stage,
                    input,
                    governedStage.Policy,
                    attempt,
                    cancellationToken)
                    .ConfigureAwait(false);

            if (execution.Stopped)
            {
                _processedRunCommands.Add(commandId);

                return ApplyExecutionStop(
                    commandId,
                    stage,
                    execution);
            }

            candidate = execution.Candidate
                ?? throw new InvalidDataException(
                    "A successful specialist execution " +
                    "did not return a candidate.");

            var alignment =
                _governance!.BuildAlignmentContext(
                    governedStage,
                    State);

            var issues = AlignmentGuard.Check(
                candidate,
                candidate.AlignmentMetadata
                    ?? CandidateAlignmentMetadata.Empty,
                stage,
                governedStage.Policy,
                alignment);

            if (issues.Length > 0)
            {
                _processedRunCommands.Add(commandId);

                return ApplyMechanicalViolation(
                    commandId,
                    stage,
                    candidate,
                    issues);
            }
        }

        var message = new ProtocolMessage(
            MessageId: $"{commandId}-candidate",
            WorkflowId: State.WorkflowId,
            CausationId: State.LastMessageId,
            Kind: ProtocolMessageKind.CandidateProduced,
            Sender: stage.Specialist,
            Recipient: "orchestrator",
            Artifact: candidate);

        var result = _protocol.Handle(
            State,
            message);

        if (!result.Applied)
            return result;

        _processedRunCommands.Add(commandId);

        Apply(
            result,
            message,
            stage.Specialist,
            "candidate-produced");

        return result;
    }

    public ProtocolResult AcceptCurrent(
        string messageId,
        ArtifactEnvelope acceptedArtifact)
    {
        var stage = CurrentStage;

        var message = HumanGate(
            messageId,
            ProtocolMessageKind.GateAccepted,
            acceptedArtifact);

        var result = _protocol.Handle(
            State,
            message);

        if (result.Applied)
        {
            _currentInput = acceptedArtifact;

            Apply(
                result,
                message,
                stage.Specialist,
                "gate-accepted");
        }

        return result;
    }

    public ProtocolResult ReferCurrent(
        string messageId,
        string reason)
    {
        var stage = CurrentStage;

        var message = HumanGate(
            messageId,
            ProtocolMessageKind.GateReferred,
            reason: reason);

        var result = _protocol.Handle(
            State,
            message);

        if (result.Applied)
        {
            Apply(
                result,
                message,
                stage.Specialist,
                "gate-referred");
        }

        return result;
    }

    public ProtocolResult RejectCurrent(
        string messageId,
        string reason)
    {
        var stage = CurrentStage;

        var message = HumanGate(
            messageId,
            ProtocolMessageKind.GateRejected,
            reason: reason);

        var result = _protocol.Handle(
            State,
            message);

        if (result.Applied)
        {
            Apply(
                result,
                message,
                stage.Specialist,
                "gate-rejected");
        }

        return result;
    }

    public void RecordAlignmentAssessment(
        string messageId,
        SemanticDivergence[] divergences)
    {
        if (State.Status != ProtocolStatus.AwaitingReview ||
            State.Candidate is null)
        {
            throw new InvalidOperationException(
                "Alignment assessment requires " +
                "a candidate awaiting review.");
        }

        if (_alignmentTrace.Any(
                entry =>
                    string.Equals(
                        entry.MessageId,
                        messageId,
                        StringComparison.Ordinal)))
        {
            return;
        }

        _alignmentTrace.Add(
            new AlignmentTraceEntry(
                MessageId: messageId,
                Attempt: State.Candidate.Attempt,
                CandidateSha256:
                    State.Candidate.ContentSha256,
                Divergences: divergences));

        Persist();
    }

    private ProtocolResult ApplyMechanicalViolation(
        string commandId,
        LoopStage stage,
        ArtifactEnvelope candidate,
        AlignmentIssue[] issues)
    {
        var reason = string.Join(
            "; ",
            issues.Select(
                issue =>
                    $"{issue.Code}: {issue.Detail}"));

        var message = new ProtocolMessage(
            MessageId:
                $"{commandId}-mechanical-violation",
            WorkflowId:
                State.WorkflowId,
            CausationId:
                State.LastMessageId,
            Kind:
                ProtocolMessageKind.MechanicalViolationDetected,
            Sender:
                "alignment-guard",
            Recipient:
                "orchestrator",
            Artifact:
                candidate,
            Reason:
                reason);

        var result = _protocol.Handle(
            State,
            message);

        if (result.Applied)
        {
            Apply(
                result,
                message,
                stage.Specialist,
                "mechanical-violation");
        }

        return result;
    }

    private ProtocolResult ApplyExecutionStop(
        string commandId,
        LoopStage stage,
        SpecialistExecutionResult execution)
    {
        var reason =
            $"{execution.StopCode}: {execution.StopReason}";

        var message = new ProtocolMessage(
            MessageId:
                $"{commandId}-execution-limit",
            WorkflowId:
                State.WorkflowId,
            CausationId:
                State.LastMessageId,
            Kind:
                ProtocolMessageKind.ExecutionLimitReached,
            Sender:
                "execution-policy",
            Recipient:
                "orchestrator",
            Reason:
                reason);

        var result = _protocol.Handle(
            State,
            message);

        if (result.Applied)
        {
            Apply(
                result,
                message,
                stage.Specialist,
                "execution-limit-reached");
        }

        return result;
    }

    private ProtocolMessage HumanGate(
        string messageId,
        ProtocolMessageKind kind,
        ArtifactEnvelope? artifact = null,
        string? reason = null) => new(
            messageId,
            State.WorkflowId,
            State.LastMessageId,
            kind,
            "human-reviewer",
            "orchestrator",
            artifact,
            reason);

    private void Apply(
        ProtocolResult result,
        ProtocolMessage message,
        string stage,
        string eventName)
    {
        State = result.State;

        var unresolved =
            message.Artifact?.Unresolved
            ?? result.State.Candidate?.Unresolved
            ?? result.State.PreservedUnresolved;

        var artifactPath =
            message.Artifact?.Path
            ?? result.State.Candidate?.Path;

        _trace.Add(
            new RunTraceEntry(
                _trace.Count + 1,
                _clock.UtcNow,
                stage,
                message.MessageId,
                message.CausationId,
                eventName,
                result.State.Status,
                artifactPath,
                unresolved,
                message.Reason));

        Persist();
    }

    private void Persist() =>
        _store.Save(
            new WorkflowSnapshot(
                State,
                _currentInput,
                [.. _processedRunCommands],
                [.. _trace],
                [.. _alignmentTrace]));

    private ProtocolResult Blocked(
        string message) =>
        new(
            State,
            false,
            $"BLOCKED: {message}");

    private ProtocolResult Ignored(
        string message) =>
        new(
            State,
            false,
            $"IGNORED: {message}");
}
