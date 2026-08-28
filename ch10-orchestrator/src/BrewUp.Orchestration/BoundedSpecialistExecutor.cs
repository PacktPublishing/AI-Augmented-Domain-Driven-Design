namespace BrewUp.Orchestration;

public sealed record SpecialistExecutionResult(
    bool Applied,
    bool Stopped,
    ArtifactEnvelope? Candidate,
    string? StopReason)
{
    public static SpecialistExecutionResult Produced(
        ArtifactEnvelope candidate) =>
        new(true, false, candidate, null);

    public static SpecialistExecutionResult Stop(
        string reason) =>
        new(false, true, null, reason);
}

public sealed class BoundedSpecialistExecutor(
    ISpecialistRunner runner)
{
    public async Task<SpecialistExecutionResult> RunAsync(
        LoopStage stage,
        SpecialistInput input,
        AutonomyPolicy policy,
        int attempt,
        CancellationToken cancellationToken = default)
    {
        if (attempt > policy.MaxCorrectionAttempts)
        {
            return SpecialistExecutionResult.Stop(
                $"Correction budget exhausted for " +
                $"{policy.Specialist}.");
        }

        using var timeout =
            CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken);

        timeout.CancelAfter(policy.Timeout);

        try
        {
            var candidate = await runner.RunAsync(
                stage,
                input,
                attempt,
                timeout.Token)
                .ConfigureAwait(false);

            return SpecialistExecutionResult.Produced(candidate);
        }
        catch (OperationCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            return SpecialistExecutionResult.Stop(
                $"Execution timed out for {policy.Specialist}.");
        }
    }
}
