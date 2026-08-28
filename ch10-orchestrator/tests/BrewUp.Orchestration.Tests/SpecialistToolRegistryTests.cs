using BrewUp.Orchestration;

namespace BrewUp.Orchestration.Tests;

public sealed class SpecialistToolRegistryTests
{
    [Fact]
    public void RetryCannotAcquireToolsOutsidePolicy()
    {
        var registry =
            new SpecialistToolRegistry(
            [
                new Tool(
                    "read-accepted-artifact"),
                new Tool(
                    "read-evidence"),
                new Tool(
                    "query-operational-system")
            ]);

        var resolved =
            registry.Resolve(
                Chapter10Policies.Storyteller());

        Assert.Equal(
            [
                "read-accepted-artifact",
                "read-evidence"
            ],
            resolved
                .Select(tool => tool.Name)
                .Order()
                .ToArray());
    }

    private sealed record Tool(
        string Name)
        : ISpecialistTool;
}
