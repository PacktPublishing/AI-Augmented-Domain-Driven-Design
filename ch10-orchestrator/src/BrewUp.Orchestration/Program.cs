using System.Text.Json;

namespace BrewUp.Orchestration;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        var repositoryRoot = FindRepositoryRoot();

        if (args is ["--demo-loop"])
        {
            var checks =
                Chapter9GateVerifier.Verify(
                    repositoryRoot);

            var pending = checks
                .Where(check => !check.Accepted)
                .ToArray();

            if (pending.Length > 0)
            {
                Console.Error.WriteLine(
                    "DETERMINISTIC DEMO: the protocol mechanics " +
                    "are exercised, but the final Chapter 9 gate " +
                    "is simulated.");

                foreach (var check in pending)
                {
                    Console.Error.WriteLine(
                        $"BLOCKED repository gate " +
                        $"{check.Stage}: {check.Message}");
                }
            }

            var loop = await DemoScenario.RunAsync(
                repositoryRoot)
                .ConfigureAwait(false);

            DemoScenario.PrintTrace(
                loop,
                Console.Out);

            return loop.State.Status ==
                ProtocolStatus.Completed
                    ? 0
                    : 1;
        }

        if (args is ["--verify-ch09"])
        {
            var checks =
                Chapter9GateVerifier.Verify(
                    repositoryRoot);

            foreach (var check in checks)
            {
                var prefix =
                    check.Accepted
                        ? "READY"
                        : "BLOCKED";

                Console.WriteLine(
                    $"{prefix}: {check.Stage}: " +
                    $"{check.Message}");
            }

            return checks.All(
                check => check.Accepted)
                    ? 0
                    : 3;
        }

        if (args is ["--run-repository-loop"])
        {
            var checks =
                Chapter9GateVerifier.Verify(
                    repositoryRoot);

            var blocked = checks
                .Where(check => !check.Accepted)
                .ToArray();

            if (blocked.Length > 0)
            {
                foreach (var check in blocked)
                {
                    Console.Error.WriteLine(
                        $"BLOCKED: {check.Stage}: " +
                        $"{check.Message}; " +
                        $"decision={check.DecisionPath}");
                }

                return 3;
            }

            var store =
                new JsonWorkflowStore(
                    Path.Combine(
                        repositoryRoot,
                        "ch10-orchestrator",
                        "artifacts",
                        "runs"));

            var loop =
                await RepositoryScenario.RunAsync(
                    repositoryRoot,
                    store)
                    .ConfigureAwait(false);

            DemoScenario.PrintTrace(
                loop,
                Console.Out);

            return loop.State.Status ==
                ProtocolStatus.Completed
                    ? 0
                    : 1;
        }

        if (args is ["--verify-alignment-run"])
        {
            return await AlignmentScenario.VerifyAsync(
                repositoryRoot,
                Console.Out)
                .ConfigureAwait(false);
        }

        if (args.Length >= 1 &&
            !args[0].StartsWith('-'))
        {
            return RunEligibility(
                args,
                repositoryRoot);
        }

        PrintUsage();

        return args.Length == 0
            ? 0
            : 2;
    }

    private static int RunEligibility(
        string[] args,
        string repositoryRoot)
    {
        var gateDirectory = "gates";

        for (var index = 1;
             index < args.Length;
             index++)
        {
            if (args[index] != "--gate-dir" ||
                index + 1 >= args.Length)
            {
                PrintUsage();
                return 2;
            }

            gateDirectory = args[++index];
        }

        var laboratory =
            Path.Combine(
                repositoryRoot,
                "ch10-orchestrator");

        var registry =
            JsonSerializer.Deserialize<
                Dictionary<string, Specialist>>(
                File.ReadAllText(
                    Path.Combine(
                        laboratory,
                        "specialist-registry.json")),
                JsonDefaults.Options)!;

        var state =
            JsonSerializer.Deserialize<
                WorkflowState>(
                File.ReadAllText(
                    Path.Combine(
                        laboratory,
                        "workflow-state.json")),
                JsonDefaults.Options)!;

        var result =
            EligibilityEvaluator.Evaluate(
                args[0],
                state,
                registry,
                Path.Combine(
                    laboratory,
                    gateDirectory));

        Console.WriteLine(
            result.Message);

        return result.Ready
            ? 0
            : 1;
    }

    private static string FindRepositoryRoot()
    {
        var directory =
            new DirectoryInfo(
                Directory.GetCurrentDirectory());

        while (directory is not null)
        {
            if (Directory.Exists(
                    Path.Combine(
                        directory.FullName,
                        "docs",
                        "ch09")) &&
                Directory.Exists(
                    Path.Combine(
                        directory.FullName,
                        "ch10-orchestrator")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Run the laboratory from the repository " +
            "or one of its subdirectories.");
    }

    private static void PrintUsage() =>
        Console.WriteLine(
            "Usage:\n" +
            "  dotnet run --project " +
            "ch10-orchestrator/src/BrewUp.Orchestration -- " +
            "storyteller --gate-dir gates-empty\n" +
            "  dotnet run --project " +
            "ch10-orchestrator/src/BrewUp.Orchestration -- " +
            "storyteller --gate-dir gates\n" +
            "  dotnet run --project " +
            "ch10-orchestrator/src/BrewUp.Orchestration -- " +
            "--demo-loop\n" +
            "  dotnet run --project " +
            "ch10-orchestrator/src/BrewUp.Orchestration -- " +
            "--verify-ch09\n" +
            "  dotnet run --project " +
            "ch10-orchestrator/src/BrewUp.Orchestration -- " +
            "--run-repository-loop\n" +
            "  dotnet run --project " +
            "ch10-orchestrator/src/BrewUp.Orchestration -- " +
            "--verify-alignment-run");
}
