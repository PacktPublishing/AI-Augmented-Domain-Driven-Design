namespace BrewUp.Orchestration.Application;

public sealed record SpecialistExecutionResult(
    ArtifactEnvelope? Candidate,
    string? StopCode,
    string? StopReason)
{
    public bool Stopped => StopCode is not null;

    public static SpecialistExecutionResult Produced(
        ArtifactEnvelope candidate) =>
        new(candidate, null, null);

    public static SpecialistExecutionResult Stop(
        string code,
        string reason) =>
        new(null, code, reason);
}

public static class BoundedSpecialistExecutor
{
    public static async Task<SpecialistExecutionResult> RunAsync(
        ISpecialistRunner runner,
        LoopStage stage,
        SpecialistInput input,
        AutonomyPolicy policy,
        int attempt,
        CancellationToken cancellationToken = default)
    {
        if (attempt > policy.MaxCorrectionAttempts)
        {
            return SpecialistExecutionResult.Stop(
                "correction-budget-exhausted",
                $"Correction budget exhausted for {policy.Specialist}.");
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
                "execution-timeout",
                $"Execution timed out for {policy.Specialist}.");
        }
    }
}
