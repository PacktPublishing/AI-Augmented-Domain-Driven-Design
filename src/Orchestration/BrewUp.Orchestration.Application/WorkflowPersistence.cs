namespace BrewUp.Orchestration.Application;

public sealed record WorkflowSnapshot(
    ProtocolState State,
    ArtifactEnvelope CurrentInput,
    string[] ProcessedRunCommands,
    RunTraceEntry[] Trace,
    AlignmentTraceEntry[]? AlignmentTrace = null);

public interface IWorkflowStore
{
    WorkflowSnapshot? Load(string workflowId);
    void Save(WorkflowSnapshot snapshot);
}

public sealed class NullWorkflowStore : IWorkflowStore
{
    public static NullWorkflowStore Instance { get; } = new();

    private NullWorkflowStore()
    {
    }

    public WorkflowSnapshot? Load(string workflowId) => null;

    public void Save(WorkflowSnapshot snapshot)
    {
    }
}
