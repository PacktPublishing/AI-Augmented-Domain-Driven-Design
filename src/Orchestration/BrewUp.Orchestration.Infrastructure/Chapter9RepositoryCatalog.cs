using System.Text.RegularExpressions;

namespace BrewUp.Orchestration.Infrastructure;

public sealed record Chapter9StageRecord(
    string Specialist,
    string CandidateKind,
    string AcceptedKind,
    string HumanDecisionPath,
    string AcceptedArtifactPath,
    string[] SourceArtifactPaths,
    string[] SourceArtifactSha256,
    string[] DecisionArtifactSha256,
    string AcceptedArtifactSha256,
    string HumanDecisionSha256,
    string[] EvidenceRefs,
    string[] Unresolved);

public sealed class Chapter9RepositoryCatalog
{
    private static readonly StageDefinition[] Definitions =
    [
        new(
            "eventstormer",
            "candidate-facts",
            "accepted-facts",
            "docs/ch09/runs/9.2-eventstormer/human-decision.md",
            "docs/ch09/runs/9.2-eventstormer/accepted-facts.md",
            [
                new("Primary output", "docs/ch09/runs/9.2-eventstormer/raw-output.md"),
                new("Correction output", "docs/ch09/runs/9.2-eventstormer/correction-01/raw-output.md")
            ],
            [
                new("Primary decision", "docs/ch09/runs/9.2-eventstormer/human-decision.md"),
                new("Correction decision", "docs/ch09/runs/9.2-eventstormer/correction-01/human-decision.md")
            ]),
        new(
            "storyteller",
            "candidate-scenarios",
            "accepted-scenarios",
            "docs/ch09/runs/9.3-storyteller/human-decision.md",
            "docs/ch09/runs/9.3-storyteller/accepted-scenarios.md",
            [
                new("Primary output", "docs/ch09/runs/9.3-storyteller/raw-output.md"),
                new("Correction output", "docs/ch09/runs/9.3-storyteller/correction-01/raw-output.md")
            ],
            [
                new("Primary decision", "docs/ch09/runs/9.3-storyteller/human-decision.md"),
                new("Correction decision", "docs/ch09/runs/9.3-storyteller/correction-01/human-decision.md")
            ]),
        new(
            "command-event-writer",
            "candidate-behavior",
            "accepted-behavior",
            "docs/ch09/runs/9.4-command-event-writer/human-decision.md",
            "docs/ch09/runs/9.4-command-event-writer/accepted-behaviour.md",
            [
                new("Primary output", "docs/ch09/runs/9.4-command-event-writer/raw-output.md")
            ],
            [
                new("Human decision", "docs/ch09/runs/9.4-command-event-writer/human-decision.md")
            ]),
        new(
            "context-mapper",
            "candidate-context-map",
            "accepted-context-map",
            "docs/ch09/runs/9.5-context-mapper/human-decision.md",
            "docs/ch09/runs/9.5-context-mapper/accepted-context-map.md",
            [
                new("Primary output", "docs/ch09/runs/9.5-context-mapper/raw-output.md"),
                new("Correction output", "docs/ch09/runs/9.5-context-mapper/correction-01/raw-output.md")
            ],
            [
                new("Human decision", "docs/ch09/runs/9.5-context-mapper/human-decision.md")
            ])
    ];

    private Chapter9RepositoryCatalog(IReadOnlyList<Chapter9StageRecord> stages)
    {
        Stages = stages;
    }

    public IReadOnlyList<Chapter9StageRecord> Stages { get; }

    public static Chapter9RepositoryCatalog Load(string repositoryRoot) =>
        new(Definitions.Select(definition => Load(repositoryRoot, definition)).ToArray());

    internal static IReadOnlyList<(string Specialist, Chapter9StageRecord? Record, string? Error)>
        Verify(string repositoryRoot) => Definitions
            .Select(definition =>
            {
                try
                {
                    return (definition.Specialist, Load(repositoryRoot, definition), (string?)null);
                }
                catch (Exception exception) when (
                    exception is IOException or InvalidDataException)
                {
                    return (definition.Specialist, (Chapter9StageRecord?)null, exception.Message);
                }
            })
            .ToArray();

    private static Chapter9StageRecord Load(
        string repositoryRoot,
        StageDefinition definition)
    {
        var decision = ReadRequired(repositoryRoot, definition.HumanDecisionPath);
        if (!Regex.IsMatch(
                decision,
                @"(?im)^\*\*Status:\*\*\s*complete(?:\s|—|-|$)"))
            throw new InvalidDataException(
                $"{definition.Specialist} human decision status is not exactly complete");

        var handoff = ReadRequired(repositoryRoot, definition.AcceptedArtifactPath);
        var sourceHashes = definition.Sources
            .Select(source => VerifyDeclaredHash(
                repositoryRoot,
                handoff,
                source.Label,
                source.Path))
            .ToArray();
        var decisionHashes = definition.Decisions
            .Select(decisionDefinition => VerifyDeclaredHash(
                repositoryRoot,
                handoff,
                decisionDefinition.Label,
                decisionDefinition.Path))
            .ToArray();

        var unresolved = ExtractUnresolved(handoff);
        var evidence = Regex.Matches(
                handoff,
                @"\b(?:OBS|QUOTE|ES|ST|CEW|CM)-\d+\b")
            .Select(match => match.Value)
            .Distinct(StringComparer.Ordinal)
            .ToArray();
        if (evidence.Length == 0)
            throw new InvalidDataException(
                $"{definition.Specialist} accepted artifact has no evidence references");

        return new Chapter9StageRecord(
            definition.Specialist,
            definition.CandidateKind,
            definition.AcceptedKind,
            definition.HumanDecisionPath,
            definition.AcceptedArtifactPath,
            definition.Sources.Select(source => source.Path).ToArray(),
            sourceHashes,
            decisionHashes,
            ArtifactHash.Sha256(repositoryRoot, definition.AcceptedArtifactPath),
            decisionHashes[0],
            evidence,
            unresolved);
    }

    private static string ReadRequired(string repositoryRoot, string relativePath)
    {
        var path = Path.Combine(
            repositoryRoot,
            relativePath.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(path))
            throw new FileNotFoundException(
                $"Required repository record is missing: {relativePath}",
                path);
        return File.ReadAllText(path);
    }

    private static string VerifyDeclaredHash(
        string repositoryRoot,
        string handoff,
        string label,
        string relativePath)
    {
        var match = Regex.Match(
            handoff,
            $@"(?im)^- {Regex.Escape(label)} SHA-256:\s*`(?<hash>[0-9a-f]{{64}})`\s*$");
        if (!match.Success)
            throw new InvalidDataException(
                $"{relativePath}: {label} SHA-256 is missing");

        var declared = match.Groups["hash"].Value;
        var actual = ArtifactHash.Sha256(repositoryRoot, relativePath);
        if (!string.Equals(declared, actual, StringComparison.Ordinal))
            throw new InvalidDataException(
                $"{relativePath}: {label} SHA-256 mismatch; declared={declared}; actual={actual}");
        return actual;
    }

    private static string[] ExtractUnresolved(string handoff)
    {
        var matches = Regex.Matches(
            handoff,
            @"(?ms)^\s*- id:\s*(?<id>\S+)(?<body>.*?)(?=^\s*- id:|\z)");
        return matches
            .Where(match => Regex.IsMatch(
                match.Groups["body"].Value,
                @"(?m)^\s+status:\s*unresolved\s*$"))
            .Select(match => match.Groups["id"].Value)
            .ToArray();
    }

    private sealed record StageDefinition(
        string Specialist,
        string CandidateKind,
        string AcceptedKind,
        string HumanDecisionPath,
        string AcceptedArtifactPath,
        SourceDefinition[] Sources,
        DecisionDefinition[] Decisions);

    private sealed record SourceDefinition(string Label, string Path);
    private sealed record DecisionDefinition(string Label, string Path);
}
