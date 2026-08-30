using System.Text.Json;
using BrewUp.Orchestration.Application;

namespace BrewUp.Orchestration.Infrastructure;

public sealed class JsonWorkflowStore(string directory) : IWorkflowStore
{
    private readonly string _directory = directory;

    public WorkflowSnapshot? Load(string workflowId)
    {
        var path = SnapshotPath(workflowId);
        if (!File.Exists(path))
            return null;

        return JsonSerializer.Deserialize<WorkflowSnapshot>(
            File.ReadAllText(path),
            JsonDefaults.Options)
            ?? throw new InvalidDataException(
                $"Snapshot {path} is empty.");
    }

    public void Save(WorkflowSnapshot snapshot)
    {
        Directory.CreateDirectory(_directory);
        var path = SnapshotPath(snapshot.State.WorkflowId);
        var temporaryPath = $"{path}.{Guid.NewGuid():N}.tmp";

        File.WriteAllText(
            temporaryPath,
            JsonSerializer.Serialize(
                snapshot,
                JsonDefaults.Options));

        File.Move(
            temporaryPath,
            path,
            overwrite: true);
    }

    private string SnapshotPath(string workflowId) =>
        Path.Combine(
            _directory,
            $"{workflowId}.json");
}
