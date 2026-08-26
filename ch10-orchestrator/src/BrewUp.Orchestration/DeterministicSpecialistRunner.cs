using System.Text.Json;

namespace BrewUp.Orchestration;

public sealed class DeterministicSpecialistRunner(string repositoryRoot)
    : ISpecialistRunner
{
    private readonly string _repositoryRoot = repositoryRoot;

    public async Task<ArtifactEnvelope> RunAsync(
        LoopStage stage,
        SpecialistInput input,
        int attempt,
        CancellationToken cancellationToken = default)
    {
        EnsureInputExists(input.Primary);
        foreach (var artifact in input.Context)
            EnsureInputExists(artifact);

        var relativePath = Path.Combine(
            "ch10-orchestrator",
            "artifacts",
            "generated",
            $"{stage.CandidateKind}-attempt-{attempt}.json");
        var absolutePath = Path.Combine(_repositoryRoot, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);

        var candidate = new ArtifactEnvelope(
            input.Primary.WorkflowId,
            stage.Specialist,
            stage.CandidateKind,
            relativePath.Replace(Path.DirectorySeparatorChar, '/'),
            input.Primary.EvidenceRefs,
            input.Primary.Unresolved,
            attempt);

        var persisted = new
        {
            candidate,
            input = new
            {
                primary = input.Primary.Path,
                context = input.Context.Select(artifact => artifact.Path).ToArray()
            }
        };
        await File.WriteAllTextAsync(
            absolutePath,
            JsonSerializer.Serialize(persisted, JsonDefaults.Options),
            cancellationToken).ConfigureAwait(false);
        return candidate;
    }

    private void EnsureInputExists(ArtifactEnvelope artifact)
    {
        var path = Path.Combine(
            _repositoryRoot,
            artifact.Path.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(path))
            throw new FileNotFoundException(
                $"Declared artifact {artifact.Kind} does not exist.",
                path);
    }
}
