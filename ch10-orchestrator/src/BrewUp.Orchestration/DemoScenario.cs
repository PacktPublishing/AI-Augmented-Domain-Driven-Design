namespace BrewUp.Orchestration;

public static class DemoScenario
{
    private const string WorkflowId = "brewup-stock-01";

    private static readonly LoopStage[] Stages =
    [
        new("eventstormer", "raw-evidence", "candidate-facts", "accepted-facts"),
        new("storyteller", "accepted-facts", "candidate-scenarios", "accepted-scenarios"),
        new("command-event-writer", "accepted-scenarios", "candidate-behavior", "accepted-behavior"),
        new("context-mapper", "accepted-behavior", "candidate-context-map", "accepted-context-map")
    ];

    public static async Task<BrewUpOrchestrationLoop> RunAsync(string repositoryRoot)
    {
        var initialInput = new ArtifactEnvelope(
            WorkflowId,
            "evidence-store",
            "raw-evidence",
            "docs/ch09/run-pack/stock-walkthrough.md",
            ["OBS-01", "OBS-02", "OBS-03"],
            ["ES-17"],
            0);
        var loop = new BrewUpOrchestrationLoop(
            WorkflowId,
            Stages,
            initialInput,
            new DeterministicSpecialistRunner(repositoryRoot),
            new SystemClock());

        await loop.RunCurrentAsync("run-1").ConfigureAwait(false);
        loop.AcceptCurrent(
            "gate-1",
            Accepted(
                "accepted-facts",
                "docs/ch09/runs/9.2-eventstormer/accepted-facts.md"));

        await loop.RunCurrentAsync("run-2").ConfigureAwait(false);
        loop.ReferCurrent(
            "refer-storyteller",
            "Separate the observed shortage from the replenishment policy.");
        await loop.RunCurrentAsync("run-2-correction").ConfigureAwait(false);
        loop.AcceptCurrent(
            "gate-2",
            Accepted(
                "accepted-scenarios",
                "docs/ch09/runs/9.3-storyteller/accepted-scenarios.md"));

        await loop.RunCurrentAsync("run-3").ConfigureAwait(false);
        loop.AcceptCurrent(
            "gate-3",
            Accepted(
                "accepted-behavior",
                "docs/ch09/runs/9.4-command-event-writer/accepted-behaviour.md"));

        await loop.RunCurrentAsync("run-4").ConfigureAwait(false);
        loop.AcceptCurrent(
            "gate-4",
            Accepted(
                "accepted-context-map",
                "docs/ch09/runs/9.5-context-mapper/accepted-context-map.md"));
        return loop;
    }

    public static void PrintTrace(BrewUpOrchestrationLoop loop, TextWriter writer)
    {
        foreach (var entry in loop.Trace)
        {
            writer.WriteLine(
                $"{entry.Sequence:00} {entry.Stage,-20} " +
                $"{entry.Event,-19} {entry.Status}");
            writer.WriteLine($"   {entry.MessageId} <- {entry.CausationId}");
        }

        writer.WriteLine(
            $"FINAL {loop.State.Status}; " +
            $"accepted={loop.State.AcceptedArtifacts.Length}; " +
            $"unresolved={string.Join(',', loop.State.PreservedUnresolved)}");
    }

    private static ArtifactEnvelope Accepted(string kind, string path) => new(
        WorkflowId,
        "human-reviewer",
        kind,
        path,
        ["OBS-01", "OBS-02", "OBS-03"],
        ["ES-17"],
        1);
}
