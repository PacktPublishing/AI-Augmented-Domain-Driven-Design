using BrewUp.Orchestration;

namespace BrewUp.Orchestration.Tests;

public sealed class Chapter9RepositoryCatalogTests
{
    [Fact]
    public void LoadsVerifiedStageRecordsAndTheirRealUnresolvedIdentifiers()
    {
        var catalog = Chapter9RepositoryCatalog.Load(FindRepositoryRoot());

        Assert.Equal(4, catalog.Stages.Count);
        Assert.Equal(11, catalog.Stages[0].Unresolved.Length);
        Assert.Equal(10, catalog.Stages[1].Unresolved.Length);
        Assert.Equal(15, catalog.Stages[2].Unresolved.Length);
        Assert.Equal(
            ["CM-06", "CM-08", "CM-12", "CM-14"],
            catalog.Stages[3].Unresolved);
        Assert.All(catalog.Stages, stage =>
        {
            Assert.Matches("^[0-9a-f]{64}$", stage.AcceptedArtifactSha256);
            Assert.Matches("^[0-9a-f]{64}$", stage.HumanDecisionSha256);
            Assert.NotEmpty(stage.SourceArtifactSha256);
        });
    }

    [Fact]
    public async Task RepositoryScenarioReplaysCorrectionsAndPreservesAllUnresolved()
    {
        var directory = Path.Combine(
            Path.GetTempPath(),
            $"brewup-repository-snapshot-{Guid.NewGuid():N}");
        try
        {
            var store = new JsonWorkflowStore(directory);

            var loop = await RepositoryScenario.RunAsync(
                FindRepositoryRoot(),
                store);
            var resumed = await RepositoryScenario.RunAsync(
                FindRepositoryRoot(),
                store);

            Assert.Equal(ProtocolStatus.Completed, loop.State.Status);
            Assert.Equal(4, loop.State.AcceptedArtifacts.Length);
            Assert.Equal(40, loop.State.PreservedUnresolved.Length);
            Assert.Contains("ES-17", loop.State.PreservedUnresolved);
            Assert.Contains("CM-06", loop.State.PreservedUnresolved);
            Assert.Contains("CM-14", loop.State.PreservedUnresolved);
            Assert.Equal(14, loop.Trace.Count);
            Assert.Equal(
                loop.Trace.Select(entry => entry.MessageId),
                resumed.Trace.Select(entry => entry.MessageId));
            Assert.Equal(14, resumed.Trace.Count);
        }
        finally
        {
            if (Directory.Exists(directory))
                Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public void IncompleteStatusCannotPassAsComplete()
    {
        var sourceRoot = FindRepositoryRoot();
        var temporaryRoot = Path.Combine(
            Path.GetTempPath(),
            $"brewup-gate-status-{Guid.NewGuid():N}");
        try
        {
            CopyDirectory(
                Path.Combine(sourceRoot, "docs", "ch09"),
                Path.Combine(temporaryRoot, "docs", "ch09"));
            var decisionPath = Path.Combine(
                temporaryRoot,
                "docs/ch09/runs/9.2-eventstormer/human-decision.md");
            File.WriteAllText(
                decisionPath,
                File.ReadAllText(decisionPath).Replace(
                    "**Status:** complete",
                    "**Status:** incomplete",
                    StringComparison.Ordinal));

            var checks = Chapter9GateVerifier.Verify(temporaryRoot);

            var eventStormer = Assert.Single(
                checks,
                check => check.Stage == "eventstormer");
            Assert.False(eventStormer.Accepted);
            Assert.Contains("not exactly complete", eventStormer.Message);
        }
        finally
        {
            if (Directory.Exists(temporaryRoot))
                Directory.Delete(temporaryRoot, recursive: true);
        }
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, "docs", "ch09")) &&
                Directory.Exists(Path.Combine(directory.FullName, "ch10-orchestrator")))
                return directory.FullName;
            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Repository root was not found.");
    }

    private static void CopyDirectory(string source, string destination)
    {
        Directory.CreateDirectory(destination);
        foreach (var file in Directory.EnumerateFiles(source))
            File.Copy(file, Path.Combine(destination, Path.GetFileName(file)));
        foreach (var directory in Directory.EnumerateDirectories(source))
            CopyDirectory(
                directory,
                Path.Combine(destination, Path.GetFileName(directory)));
    }
}
