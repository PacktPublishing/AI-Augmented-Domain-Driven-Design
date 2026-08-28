namespace BrewUp.Orchestration;

public static class AlignmentScenario
{
    public static async Task<int> VerifyAsync(
        string repositoryRoot,
        TextWriter output,
        CancellationToken cancellationToken = default)
    {
        var acceptedFacts = new ArtifactEnvelope(
            WorkflowId: "brewup-stock-01",
            Producer: "human-reviewer",
            Kind: "accepted-facts",
            Path: "docs/ch09/runs/9.2-eventstormer/accepted-facts.md",
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
            Attempt: 1,
            ContentSha256:
                "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" +
                "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");

        var context =
            Chapter10Policies.StorytellerAlignment(
                acceptedFacts);

        var reviewer =
            new MarkdownSemanticAlignmentReviewer(
                repositoryRoot);

        var attempt1 = Candidate(
            repositoryRoot,
            "docs/ch10/runs/10.1-alignment/" +
            "storyteller-attempt-1.md",
            attempt: 1);

        var before =
            File.ReadAllText(
                Path.Combine(
                    repositoryRoot,
                    attempt1.Path.Replace(
                        '/',
                        Path.DirectorySeparatorChar)));

        var first = await reviewer.ReviewAsync(
            attempt1,
            context,
            cancellationToken)
            .ConfigureAwait(false);

        var after =
            File.ReadAllText(
                Path.Combine(
                    repositoryRoot,
                    attempt1.Path.Replace(
                        '/',
                        Path.DirectorySeparatorChar)));

        if (!string.Equals(
                before,
                after,
                StringComparison.Ordinal))
        {
            await output.WriteLineAsync(
                "BLOCKED: semantic review mutated attempt 1.");
            return 1;
        }

        if (!first.Any(
                item =>
                    item.Kind == "candidate-term") ||
            !first.Any(
                item =>
                    item.Kind ==
                    "possible-policy-inference"))
        {
            await output.WriteLineAsync(
                "BLOCKED: attempt 1 did not reproduce " +
                "the expected semantic divergence.");
            return 1;
        }

        var attempt2 = Candidate(
            repositoryRoot,
            "docs/ch10/runs/10.1-alignment/" +
            "storyteller-attempt-2.md",
            attempt: 2);

        var second = await reviewer.ReviewAsync(
            attempt2,
            context,
            cancellationToken)
            .ConfigureAwait(false);

        if (second.Length > 0)
        {
            await output.WriteLineAsync(
                "BLOCKED: corrected attempt still triggers " +
                "the controlled drift checks.");
            return 1;
        }

        if (!attempt2.Unresolved.Contains(
                "ES-17",
                StringComparer.Ordinal))
        {
            await output.WriteLineAsync(
                "BLOCKED: corrected attempt dropped ES-17.");
            return 1;
        }

        await output.WriteLineAsync(
            "READY: controlled alignment run verified; " +
            "attempt 1 surfaces divergence, attempt 2 " +
            "preserves ES-17 without configured drift.");

        return 0;
    }

    private static ArtifactEnvelope Candidate(
        string repositoryRoot,
        string path,
        int attempt) => new(
            WorkflowId: "brewup-stock-01",
            Producer: "storyteller",
            Kind: "candidate-scenarios",
            Path: path,
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
            Attempt: attempt,
            ContentSha256:
                ArtifactHash.Sha256(
                    repositoryRoot,
                    path));
}
