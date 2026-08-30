namespace BrewUp.Orchestration.Application;

public interface ISemanticAlignmentReviewer
{
    Task<SemanticDivergence[]> ReviewAsync(
        ArtifactEnvelope candidate,
        AlignmentContext context,
        CancellationToken cancellationToken = default);
}
