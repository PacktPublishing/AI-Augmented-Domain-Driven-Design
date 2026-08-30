using BrewUp.Orchestration.Application;

namespace BrewUp.Orchestration.Infrastructure;

public sealed class RecordedSpecialistRunner(
    IReadOnlyList<StageRecord> records)
    : ISpecialistRunner
{
    private readonly IReadOnlyDictionary<string, StageRecord> _records =
        records.ToDictionary(record => record.Specialist, StringComparer.Ordinal);

    public Task<ArtifactEnvelope> RunAsync(
        LoopStage stage,
        SpecialistInput input,
        int attempt,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_records.TryGetValue(stage.Specialist, out var record))
            throw new InvalidOperationException(
                $"No Chapter 9 record exists for {stage.Specialist}.");

        if (attempt < 1 || attempt > record.SourceArtifactPaths.Length)
            throw new InvalidOperationException(
                $"No recorded attempt {attempt} exists for {stage.Specialist}.");

        var index = attempt - 1;
        var evidence = input.Primary.EvidenceRefs
            .Concat(record.EvidenceRefs)
            .Distinct(StringComparer.Ordinal)
            .ToArray();
        var unresolved = input.Primary.Unresolved
            .Concat(record.Unresolved)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        return Task.FromResult(new ArtifactEnvelope(
            input.Primary.WorkflowId,
            stage.Specialist,
            stage.CandidateKind,
            record.SourceArtifactPaths[index],
            evidence,
            unresolved,
            attempt,
            record.SourceArtifactSha256[index]));
    }
}
