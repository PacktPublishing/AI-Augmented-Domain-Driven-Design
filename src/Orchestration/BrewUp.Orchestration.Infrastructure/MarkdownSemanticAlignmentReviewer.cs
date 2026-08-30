using System.Text.RegularExpressions;
using BrewUp.Orchestration.Application;

namespace BrewUp.Orchestration.Infrastructure;

/// <summary>
/// Reproducible teaching adapter for the Chapter 11 controlled run.
/// A production semantic reviewer may use a model, but it must preserve the
/// same observational contract: report divergence without rewriting or accepting
/// the candidate.
/// </summary>
public sealed class MarkdownSemanticAlignmentReviewer(
    string repositoryRoot)
    : ISemanticAlignmentReviewer
{
    public Task<SemanticDivergence[]> ReviewAsync(
        ArtifactEnvelope candidate,
        AlignmentContext context,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var fullPath = Path.Combine(
            repositoryRoot,
            candidate.Path.Replace(
                '/',
                Path.DirectorySeparatorChar));

        var text = File.ReadAllText(fullPath);
        var findings = new List<SemanticDivergence>();

        var candidateTerm = Regex.Match(
            text,
            @"\breserv(?:e|es|ed|ing|ation)\b",
            RegexOptions.IgnoreCase);

        if (candidateTerm.Success &&
            !context.ApprovedTerms.Contains(
                candidateTerm.Value,
                StringComparer.OrdinalIgnoreCase))
        {
            findings.Add(
                new SemanticDivergence(
                    Kind: "candidate-term",
                    CandidateValue:
                        NormalizeReserve(candidateTerm.Value),
                    RelatedAcceptedValues:
                        context.ApprovedTerms
                            .Where(term =>
                                term is
                                    "set aside" or
                                    "hold" or
                                    "earmark")
                            .ToArray(),
                    Reason:
                        "The candidate introduces a more precise term " +
                        "that has not been accepted."));
        }

        if (context.Unresolved.Contains(
                "ES-17",
                StringComparer.Ordinal) &&
            Regex.IsMatch(
                text,
                @"\bStock\s+reserv(?:e|es|ed|ing)\b",
                RegexOptions.IgnoreCase))
        {
            findings.Add(
                new SemanticDivergence(
                    Kind:
                        "possible-policy-inference",
                    CandidateValue:
                        "short quantity is expressed as general Stock behavior",
                    RelatedAcceptedValues:
                    [
                        "ES-17"
                    ],
                    Reason:
                        "One observed short-quantity practice is expressed " +
                        "as the general Stock behavior."));
        }

        return Task.FromResult(
            findings.ToArray());
    }

    private static string NormalizeReserve(
        string value) =>
        value.StartsWith(
            "reserv",
            StringComparison.OrdinalIgnoreCase)
                ? "reserve"
                : value;
}
