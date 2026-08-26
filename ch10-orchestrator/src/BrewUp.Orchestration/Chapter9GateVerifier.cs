namespace BrewUp.Orchestration;

public sealed record Chapter9GateCheck(
    string Stage,
    bool Accepted,
    string Message,
    string DecisionPath,
    string ArtifactPath);

public static class Chapter9GateVerifier
{
    private static readonly (string Stage, string Decision, string Artifact)[] Gates =
    [
        (
            "eventstormer",
            "docs/ch09/runs/9.2-eventstormer/human-decision.md",
            "docs/ch09/runs/9.2-eventstormer/accepted-facts.md"),
        (
            "storyteller",
            "docs/ch09/runs/9.3-storyteller/human-decision.md",
            "docs/ch09/runs/9.3-storyteller/accepted-scenarios.md"),
        (
            "command-event-writer",
            "docs/ch09/runs/9.4-command-event-writer/human-decision.md",
            "docs/ch09/runs/9.4-command-event-writer/accepted-behaviour.md"),
        (
            "context-mapper",
            "docs/ch09/runs/9.5-context-mapper/human-decision.md",
            "docs/ch09/runs/9.5-context-mapper/accepted-context-map.md")
    ];

    public static IReadOnlyList<Chapter9GateCheck> Verify(string repositoryRoot) =>
        Gates.Select(gate => Verify(repositoryRoot, gate)).ToArray();

    private static Chapter9GateCheck Verify(
        string repositoryRoot,
        (string Stage, string Decision, string Artifact) gate)
    {
        var decisionPath = Path.Combine(repositoryRoot, gate.Decision);
        var artifactPath = Path.Combine(repositoryRoot, gate.Artifact);
        if (!File.Exists(decisionPath))
            return Blocked(gate, "human decision file is missing");
        if (!File.Exists(artifactPath))
            return Blocked(gate, "effective artifact is missing");

        var decision = File.ReadAllText(decisionPath);
        if (decision.Contains("pending human review", StringComparison.OrdinalIgnoreCase))
            return Blocked(gate, "human review is pending");
        if (!decision.Contains("complete", StringComparison.OrdinalIgnoreCase))
            return Blocked(gate, "no completed human gate is recorded");

        return new Chapter9GateCheck(
            gate.Stage,
            true,
            "accepted human gate and effective artifact are present",
            gate.Decision,
            gate.Artifact);
    }

    private static Chapter9GateCheck Blocked(
        (string Stage, string Decision, string Artifact) gate,
        string message) => new(
            gate.Stage,
            false,
            message,
            gate.Decision,
            gate.Artifact);
}
