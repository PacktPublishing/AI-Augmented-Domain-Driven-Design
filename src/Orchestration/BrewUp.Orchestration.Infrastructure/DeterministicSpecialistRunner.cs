using System.Security.Cryptography;
using System.Text.Json;
using BrewUp.Orchestration.Application;

namespace BrewUp.Orchestration.Infrastructure;

public sealed class DeterministicSpecialistRunner(
    string repositoryRoot,
    string outputDirectory = "artifacts/ch10/generated")
    : ISpecialistRunner
{
    private readonly string _repositoryRoot = repositoryRoot;
    private readonly string _outputDirectory = outputDirectory;

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
            _outputDirectory,
            $"{stage.CandidateKind}-attempt-{attempt}.json");
        var absolutePath = Path.Combine(_repositoryRoot, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);

        var persisted = new
        {
            workflow_id = input.Primary.WorkflowId,
            producer = stage.Specialist,
            kind = stage.CandidateKind,
            evidence_refs = input.Primary.EvidenceRefs,
            unresolved = input.Primary.Unresolved,
            attempt,
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

        await using var content = File.OpenRead(absolutePath);
        var contentSha256 = Convert.ToHexString(
            await SHA256.HashDataAsync(
                content,
                cancellationToken).ConfigureAwait(false))
            .ToLowerInvariant();

        return new ArtifactEnvelope(
            input.Primary.WorkflowId,
            stage.Specialist,
            stage.CandidateKind,
            relativePath.Replace(Path.DirectorySeparatorChar, '/'),
            input.Primary.EvidenceRefs,
            input.Primary.Unresolved,
            attempt,
            contentSha256);
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
