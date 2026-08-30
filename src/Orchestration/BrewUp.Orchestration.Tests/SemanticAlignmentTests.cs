using BrewUp.Orchestration.Application;
using BrewUp.Orchestration.Infrastructure;

namespace BrewUp.Orchestration.Tests;

public sealed class SemanticAlignmentTests
{
    [Fact]
    public async Task NewTermIsSurfacedButNotNormalized()
    {
        var root = FindRepositoryRoot();
        var candidate = CandidateUsing(
            root,
            "docs/ch11/runs/11.1-alignment/" +
            "storyteller-attempt-1.md",
            attempt: 1);

        var before = File.ReadAllText(
            Path.Combine(
                root,
                candidate.Artifact.Path.Replace(
                    '/',
                    Path.DirectorySeparatorChar)));

        var reviewer =
            new MarkdownSemanticAlignmentReviewer(
                root);

        var divergences =
            await reviewer.ReviewAsync(
                candidate.Artifact,
                Alignment());

        var divergence = Assert.Single(
            divergences,
            item =>
                item.Kind ==
                "candidate-term");

        Assert.Equal(
            "reserve",
            divergence.CandidateValue);

        Assert.Contains(
            "hold",
            divergence.RelatedAcceptedValues);

        var after = File.ReadAllText(
            Path.Combine(
                root,
                candidate.Artifact.Path.Replace(
                    '/',
                    Path.DirectorySeparatorChar)));

        Assert.Equal(before, after);
        Assert.Contains(
            "Stock reserves",
            candidate.Text);
    }

    [Fact]
    public async Task PolicyInferenceIsSurfacedForEs17()
    {
        var root = FindRepositoryRoot();
        var candidate = CandidateUsing(
            root,
            "docs/ch11/runs/11.1-alignment/" +
            "storyteller-attempt-1.md",
            attempt: 1);

        var reviewer =
            new MarkdownSemanticAlignmentReviewer(
                root);

        var divergences =
            await reviewer.ReviewAsync(
                candidate.Artifact,
                Alignment());

        Assert.Contains(
            divergences,
            item =>
                item.Kind ==
                "possible-policy-inference" &&
                item.RelatedAcceptedValues.Contains(
                    "ES-17",
                    StringComparer.Ordinal));
    }

    [Fact]
    public async Task CorrectedCandidateHasNoConfiguredDivergence()
    {
        var root = FindRepositoryRoot();
        var candidate = CandidateUsing(
            root,
            "docs/ch11/runs/11.1-alignment/" +
            "storyteller-attempt-2.md",
            attempt: 2);

        var reviewer =
            new MarkdownSemanticAlignmentReviewer(
                root);

        var divergences =
            await reviewer.ReviewAsync(
                candidate.Artifact,
                Alignment());

        Assert.Empty(divergences);

        Assert.Contains(
            "ES-17",
            candidate.Artifact.Unresolved);
    }

    private static AlignmentContext Alignment() =>
        BrewUpGovernanceDefaults
            .Storyteller()
            .Alignment;

    private static CandidateDocument CandidateUsing(
        string root,
        string path,
        int attempt)
    {
        var fullPath = Path.Combine(
            root,
            path.Replace(
                '/',
                Path.DirectorySeparatorChar));

        var text = File.ReadAllText(fullPath);

        return new CandidateDocument(
            new ArtifactEnvelope(
                WorkflowId:
                    "brewup-stock-01",
                Producer:
                    "storyteller",
                Kind:
                    "candidate-scenarios",
                Path:
                    path,
                EvidenceRefs:
                [
                    "OBS-01",
                    "OBS-02",
                    "OBS-03"
                ],
                Unresolved:
                [
                    "ES-17"
                ],
                Attempt:
                    attempt,
                ContentSha256:
                    ArtifactHash.Sha256(
                        root,
                        path),
                AlignmentMetadata:
                    new CandidateAlignmentMetadata(
                        attempt == 1
                            ? ["reserve"]
                            : ["set aside"],
                        [])),
            text);
    }

    private static string FindRepositoryRoot()
    {
        var directory =
            new DirectoryInfo(
                AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (Directory.Exists(
                    Path.Combine(
                        directory.FullName,
                        "docs",
                        "ch11")) &&
                Directory.Exists(
                    Path.Combine(
                        directory.FullName,
                        "src",
                        "Orchestration")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Repository root was not found.");
    }

    private sealed record CandidateDocument(
        ArtifactEnvelope Artifact,
        string Text);
}
