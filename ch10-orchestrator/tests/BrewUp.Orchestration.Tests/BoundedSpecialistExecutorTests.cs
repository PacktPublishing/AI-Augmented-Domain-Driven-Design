using BrewUp.Orchestration;

namespace BrewUp.Orchestration.Tests;

public sealed class BoundedSpecialistExecutorTests
{
    [Fact]
    public async Task RetryExhaustionStopsWithSpecificReason()
    {
        var runner =
            new NeverCalledRunner();

        var executor =
            new BoundedSpecialistExecutor(
                runner);

        var result =
            await executor.RunAsync(
                TestFixtures.Stages[1],
                new SpecialistInput(
                    TestFixtures.Accepted(
                        "accepted-facts"),
                    []),
                Chapter10Policies.Storyteller(),
                attempt: 3);

        Assert.True(
            result.Stopped);

        Assert.Contains(
            "Correction budget exhausted",
            result.StopReason);

        Assert.Equal(
            0,
            runner.Invocations);
    }

    private sealed class NeverCalledRunner
        : ISpecialistRunner
    {
        internal int Invocations { get; private set; }

        public Task<ArtifactEnvelope> RunAsync(
            LoopStage stage,
            SpecialistInput input,
            int attempt,
            CancellationToken cancellationToken = default)
        {
            Invocations++;

            throw new InvalidOperationException(
                "Runner should not be invoked " +
                "after retry exhaustion.");
        }
    }
}
