using BrewUp.Orchestration.Application;

namespace BrewUp.Orchestration.Infrastructure;

public static class RepositoryScenario
{
    private const string WorkflowId = "brewup-stock-repository-01";

    public static async Task<BrewUpOrchestrationLoop> RunAsync(
        string repositoryRoot,
        IWorkflowStore store,
        CancellationToken cancellationToken = default)
    {
        var catalog = Chapter9RepositoryCatalog.Load(repositoryRoot);
        var stages = catalog.Stages
            .Select((record, index) => new LoopStage(
                record.Specialist,
                index == 0
                    ? "raw-evidence"
                    : catalog.Stages[index - 1].AcceptedKind,
                record.CandidateKind,
                record.AcceptedKind))
            .ToArray();

        const string evidencePath = "docs/ch09/run-pack/stock-walkthrough.md";
        var initialInput = new ArtifactEnvelope(
            WorkflowId,
            "evidence-store",
            "raw-evidence",
            evidencePath,
            ["OBS-01", "OBS-02", "OBS-03"],
            [],
            0,
            ArtifactHash.Sha256(repositoryRoot, evidencePath));

        var loop = new BrewUpOrchestrationLoop(
            WorkflowId,
            stages,
            initialInput,
            new RecordedSpecialistRunner(catalog.Stages),
            new SystemClock(),
            store);

        while (loop.State.Status is not (
                   ProtocolStatus.Completed or ProtocolStatus.Stopped))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var record = catalog.Stages[loop.State.StageIndex];

            if (loop.State.Status is ProtocolStatus.Running or
                ProtocolStatus.CorrectionRequired)
            {
                var attempt = loop.State.Status == ProtocolStatus.CorrectionRequired
                    ? (loop.State.Candidate?.Attempt ?? 0) + 1
                    : 1;
                var run = await loop.RunCurrentAsync(
                    $"repository-{record.Specialist}-attempt-{attempt}",
                    cancellationToken).ConfigureAwait(false);
                if (!run.Applied)
                    throw new InvalidOperationException(run.Message);
                continue;
            }

            var candidate = loop.State.Candidate
                ?? throw new InvalidDataException("AwaitingReview has no candidate.");

            if (candidate.Attempt < record.SourceArtifactPaths.Length)
            {
                var referred = loop.ReferCurrent(
                    $"repository-{record.Specialist}-referral-{candidate.Attempt}",
                    "Recorded Chapter 9 gate requires the corrected attempt.");
                if (!referred.Applied)
                    throw new InvalidOperationException(referred.Message);
                continue;
            }

            var accepted = new ArtifactEnvelope(
                WorkflowId,
                "human-reviewer",
                record.AcceptedKind,
                record.AcceptedArtifactPath,
                record.EvidenceRefs,
                candidate.Unresolved,
                candidate.Attempt,
                record.AcceptedArtifactSha256,
                record.SourceArtifactSha256,
                record.HumanDecisionSha256);
            var gate = loop.AcceptCurrent(
                $"repository-{record.Specialist}-accepted",
                accepted);
            if (!gate.Applied)
                throw new InvalidOperationException(gate.Message);
        }

        return loop;
    }
}
