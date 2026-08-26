namespace BrewUp.Orchestration;

public sealed record Chapter9GateCheck(
    string Stage,
    bool Accepted,
    string Message,
    string DecisionPath,
    string ArtifactPath);

public static class Chapter9GateVerifier
{
    public static IReadOnlyList<Chapter9GateCheck> Verify(string repositoryRoot) =>
        Chapter9RepositoryCatalog.Verify(repositoryRoot)
            .Select(result => result.Record is { } record
                ? new Chapter9GateCheck(
                    result.Specialist,
                    true,
                    "complete human gate, effective artifact, and provenance hashes verified",
                    record.HumanDecisionPath,
                    record.AcceptedArtifactPath)
                : new Chapter9GateCheck(
                    result.Specialist,
                    false,
                    result.Error ?? "repository gate validation failed",
                    string.Empty,
                    string.Empty))
            .ToArray();
}
