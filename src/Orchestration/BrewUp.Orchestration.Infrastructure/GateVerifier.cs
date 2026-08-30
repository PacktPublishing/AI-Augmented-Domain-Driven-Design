namespace BrewUp.Orchestration.Infrastructure;

public sealed record GateCheck(
    string Stage,
    bool Accepted,
    string Message,
    string DecisionPath,
    string ArtifactPath);

public static class GateVerifier
{
    public static IReadOnlyList<GateCheck> Verify(string repositoryRoot) =>
        RepositoryCatalog.Verify(repositoryRoot)
            .Select(result => result.Record is { } record
                ? new GateCheck(
                    result.Specialist,
                    true,
                    "complete human gate, effective artifact, and provenance hashes verified",
                    record.HumanDecisionPath,
                    record.AcceptedArtifactPath)
                : new GateCheck(
                    result.Specialist,
                    false,
                    result.Error ?? "repository gate validation failed",
                    string.Empty,
                    string.Empty))
            .ToArray();
}
