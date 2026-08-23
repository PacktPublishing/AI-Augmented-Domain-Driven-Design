using System.Diagnostics;
using System.Text.RegularExpressions;

namespace BrewUp.Warehouse.Tests.Architecture;

public sealed class Story13ImplementationConventionReview
{
    private const string Baseline = "8ca284c00a08bcb1a8375369f341d830941280f6";
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void ChangedStory13ProductionFilesUseApprovedAsyncAndGuidConventions()
    {
        var trackedChanges = RunGit(
                "diff",
                "--name-only",
                Baseline,
                "--",
                "src/Warehouse");
        var untrackedChanges = RunGit(
            "ls-files",
            "--others",
            "--exclude-standard",
            "--",
            "src/Warehouse");
        var changedFiles = string
            .Concat(trackedChanges, untrackedChanges)
            .Split(
                ['\r', '\n'],
                StringSplitOptions.RemoveEmptyEntries)
            .Where(path =>
                path.EndsWith(".cs", StringComparison.Ordinal) &&
                !path.Contains(
                    "BrewUp.Warehouse.Tests",
                    StringComparison.Ordinal))
            .ToArray();

        Assert.NotEmpty(changedFiles);

        foreach (var path in changedFiles)
        {
            var source = File.ReadAllText(Path.Combine(RepositoryRoot, path));
            Assert.DoesNotContain("Guid.NewGuid(", source, StringComparison.Ordinal);

            var awaitedStatements = Regex.Matches(
                source,
                @"\bawait\b[\s\S]*?;",
                RegexOptions.CultureInvariant);
            foreach (Match statement in awaitedStatements)
            {
                Assert.Contains(
                    ".ConfigureAwait(false)",
                    statement.Value,
                    StringComparison.Ordinal);
            }
        }
    }

    private static string RunGit(params string[] arguments)
    {
        var startInfo = new ProcessStartInfo("git")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            WorkingDirectory = RepositoryRoot
        };
        foreach (var argument in arguments)
            startInfo.ArgumentList.Add(argument);

        using var process = Process.Start(startInfo)!;
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();
        Assert.True(process.ExitCode == 0, error);
        return output;
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !Directory.Exists(Path.Combine(directory.FullName, ".git")))
            directory = directory.Parent;

        return directory?.FullName
            ?? throw new DirectoryNotFoundException("Git repository root not found.");
    }
}
