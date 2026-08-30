using System.Text.Json;

namespace BrewUp.Orchestration.Application;

public sealed record Specialist(
    string Consumes,
    string Produces,
    string? RequiresGate);

public sealed record WorkflowState(
    string WorkflowId,
    Dictionary<string, string> Artifacts);

public sealed record EffectiveArtifact(string Kind, string Path);

public sealed record GateRecord(
    string GateId,
    string Decision,
    EffectiveArtifact EffectiveArtifact,
    string[] PreservesUnresolved);

public sealed record EvaluationResult(
    bool Ready,
    string Message,
    IReadOnlyDictionary<string, string> AvailableArtifacts,
    string[] Unresolved);

public static class JsonDefaults
{
    public static JsonSerializerOptions Options { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };
}

public static class EligibilityEvaluator
{
    public static EvaluationResult Evaluate(
        string specialistName,
        WorkflowState state,
        IReadOnlyDictionary<string, Specialist> registry,
        string gateDirectory)
    {
        if (!registry.TryGetValue(specialistName, out var specialist))
            return Blocked(state.Artifacts, $"unknown specialist {specialistName}");

        var available = new Dictionary<string, string>(
            state.Artifacts,
            StringComparer.Ordinal);
        var unresolved = Array.Empty<string>();

        if (specialist.RequiresGate is { } requiredGate)
        {
            var path = Path.Combine(gateDirectory, $"{requiredGate}.json");
            if (!File.Exists(path))
                return Blocked(
                    available,
                    $"required gate {requiredGate} is missing");

            GateRecord? gate;
            try
            {
                gate = JsonSerializer.Deserialize<GateRecord>(
                    File.ReadAllText(path),
                    JsonDefaults.Options);
            }
            catch (JsonException exception)
            {
                return Blocked(
                    available,
                    $"gate {requiredGate} is invalid: {exception.Message}");
            }

            if (gate is null)
                return Blocked(available, $"gate {requiredGate} is empty");

            if (!string.Equals(
                    gate.GateId,
                    requiredGate,
                    StringComparison.Ordinal))
                return Blocked(
                    available,
                    $"gate id {gate.GateId} does not match {requiredGate}");

            if (!string.Equals(
                    gate.Decision,
                    "accepted",
                    StringComparison.OrdinalIgnoreCase))
                return Blocked(
                    available,
                    $"gate {requiredGate} is {gate.Decision}");

            available[gate.EffectiveArtifact.Kind] = gate.EffectiveArtifact.Path;
            unresolved = gate.PreservesUnresolved ?? [];
        }

        if (!available.TryGetValue(specialist.Consumes, out var artifactPath))
            return Blocked(
                available,
                $"required artifact {specialist.Consumes} is missing",
                unresolved);

        var message = $"READY: {specialistName} can consume " +
                      $"{specialist.Consumes} from {artifactPath}";
        if (unresolved.Length > 0)
            message += $"; unresolved preserved: {string.Join(", ", unresolved)}";

        return new EvaluationResult(true, message, available, unresolved);
    }

    private static EvaluationResult Blocked(
        IReadOnlyDictionary<string, string> available,
        string reason,
        string[]? unresolved = null) =>
        new(false, $"BLOCKED: {reason}", available, unresolved ?? []);
}
