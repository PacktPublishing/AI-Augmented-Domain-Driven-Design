using System.Text.Json;
using BrewUp.Orchestration.Application;

namespace BrewUp.Orchestration.Tests;

public sealed class EligibilityEvaluatorTests : IDisposable
{
    private readonly string _gateDirectory = Path.Combine(
        Path.GetTempPath(),
        $"brewup-orchestration-{Guid.NewGuid():N}");

    private readonly IReadOnlyDictionary<string, Specialist> _registry =
        new Dictionary<string, Specialist>(StringComparer.Ordinal)
        {
            ["eventstormer"] = new(
                "raw-evidence", "candidate-facts", RequiresGate: null),
            ["storyteller"] = new(
                "accepted-facts", "candidate-scenarios", "eventstormer-review")
        };

    private readonly WorkflowState _state = new(
        "brewup-stock-01",
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["raw-evidence"] = "docs/ch09/run-pack/stock-walkthrough.md"
        });

    public EligibilityEvaluatorTests() => Directory.CreateDirectory(_gateDirectory);

    [Fact]
    public void EventStormerIsReadyFromRawEvidence()
    {
        var result = EligibilityEvaluator.Evaluate(
            "eventstormer", _state, _registry, _gateDirectory);

        Assert.True(result.Ready);
        Assert.Contains("raw-evidence", result.Message);
    }

    [Fact]
    public void StorytellerIsBlockedWithoutTheHumanGate()
    {
        var result = EligibilityEvaluator.Evaluate(
            "storyteller", _state, _registry, _gateDirectory);

        Assert.False(result.Ready);
        Assert.Equal(
            "BLOCKED: required gate eventstormer-review is missing",
            result.Message);
    }

    [Fact]
    public void AcceptedGateMakesStorytellerReadyAndPreservesUnresolved()
    {
        WriteGate(new GateRecord(
            "eventstormer-review",
            "accepted",
            new EffectiveArtifact(
                "accepted-facts",
                "docs/ch09/runs/9.2-eventstormer/accepted-facts.md"),
            ["ES-17"]));

        var result = EligibilityEvaluator.Evaluate(
            "storyteller", _state, _registry, _gateDirectory);

        Assert.True(result.Ready);
        Assert.Equal(["ES-17"], result.Unresolved);
        Assert.Contains("unresolved preserved: ES-17", result.Message);
    }

    [Fact]
    public void ReferredGateKeepsStorytellerBlocked()
    {
        WriteGate(new GateRecord(
            "eventstormer-review",
            "referred",
            new EffectiveArtifact(
                "accepted-facts",
                "docs/ch09/runs/9.2-eventstormer/raw-output.md"),
            ["ES-17"]));

        var result = EligibilityEvaluator.Evaluate(
            "storyteller", _state, _registry, _gateDirectory);

        Assert.False(result.Ready);
        Assert.Equal(
            "BLOCKED: gate eventstormer-review is referred",
            result.Message);
    }

    public void Dispose()
    {
        if (Directory.Exists(_gateDirectory))
            Directory.Delete(_gateDirectory, recursive: true);
    }

    private void WriteGate(GateRecord gate)
    {
        var path = Path.Combine(_gateDirectory, "eventstormer-review.json");
        File.WriteAllText(path, JsonSerializer.Serialize(gate, JsonDefaults.Options));
    }
}
