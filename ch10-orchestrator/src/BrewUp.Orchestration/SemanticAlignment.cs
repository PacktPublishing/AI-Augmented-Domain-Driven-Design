using System.Text.RegularExpressions;

namespace BrewUp.Orchestration;

public interface ISemanticAlignmentReviewer
{
    Task<SemanticDivergence[]> ReviewAsync(
        ArtifactEnvelope candidate,
        AlignmentContext context,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Deterministic reviewer for the controlled drift experiment in Chapter 10.
/// It reads the candidate artifact and reports configured divergence patterns.
/// It never rewrites the candidate and never changes workflow state.
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
            @"\breserv(?:e|es|ed|ation)\b",
            RegexOptions.IgnoreCase);

        if (candidateTerm.Success &&
            !context.ApprovedTerms.Contains(
                candidateTerm.Value,
                StringComparer.OrdinalIgnoreCase))
        {
            findings.Add(new(
                Kind: "candidate-term",
                CandidateValue: candidateTerm.Value,
                RelatedAcceptedValues:
                [
                    "set aside",
                    "hold"
                ],
                Reason:
                    "The candidate introduces a more precise term " +
                    "that has not been accepted."));
        }

        if (context.Unresolved.Contains(
                "ES-17",
                StringComparer.Ordinal) &&
            Regex.IsMatch(
                text,
                @"Stock\s+reserv(?:e|es|ed)",
                RegexOptions.IgnoreCase))
        {
            findings.Add(new(
                Kind: "possible-policy-inference",
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

        return Task.FromResult(findings.ToArray());
    }
}
