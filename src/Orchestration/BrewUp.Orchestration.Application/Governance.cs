namespace BrewUp.Orchestration.Application;

public sealed record AutonomyPolicy(
    string Specialist,
    string OutputKind,
    string[] AllowedEvidenceKinds,
    bool MayIntroduceNewTerms,
    int MaxCorrectionAttempts,
    TimeSpan Timeout,
    string[] AllowedTools);

public sealed record AlignmentContext(
    ArtifactEnvelope[] AcceptedArtifacts,
    string WorkflowGoal,
    string[] ApprovedTerms,
    string[] Unresolved,
    string[] ProhibitedDecisionTypes);

public sealed record CandidateAlignmentMetadata(
    string[] Terms,
    string[] DecisionTypes)
{
    public static CandidateAlignmentMetadata Empty { get; } = new([], []);
}

public sealed record AlignmentIssue(
    string Code,
    string Detail);

public sealed record SemanticDivergence(
    string Kind,
    string CandidateValue,
    string[] RelatedAcceptedValues,
    string Reason);

public sealed record AlignmentTraceEntry(
    string MessageId,
    int Attempt,
    string CandidateSha256,
    SemanticDivergence[] Divergences);

public sealed record SpecialistGovernance(
    AutonomyPolicy Policy,
    AlignmentContext Alignment);

public sealed class GovernanceCatalog
{
    private readonly IReadOnlyDictionary<string, SpecialistGovernance> _specialists;
    private readonly SpecialistToolRegistry _tools;

    public GovernanceCatalog(
        IEnumerable<SpecialistGovernance> specialists,
        SpecialistToolRegistry? tools = null)
    {
        _specialists = specialists.ToDictionary(
            item => item.Policy.Specialist,
            StringComparer.Ordinal);

        _tools = tools ?? SpecialistToolRegistry.Unrestricted;
    }

    public bool TryGet(
        string specialist,
        out SpecialistGovernance governance) =>
        _specialists.TryGetValue(specialist, out governance!);

    public AlignmentContext BuildAlignmentContext(
        SpecialistGovernance governance,
        ProtocolState state) =>
        governance.Alignment with
        {
            AcceptedArtifacts = [.. state.AcceptedArtifacts]
        };

    public string[] ResolveTools(
        SpecialistGovernance governance) =>
        _tools.Resolve(governance.Policy);

    public string[] FindDisallowedEvidenceKinds(
        SpecialistInput input,
        SpecialistGovernance governance)
    {
        var suppliedKinds = new[] { input.Primary }
            .Concat(input.Context)
            .Select(PolicyEvidenceKind)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        return suppliedKinds
            .Except(
                governance.Policy.AllowedEvidenceKinds,
                StringComparer.Ordinal)
            .ToArray();
    }

    private static string PolicyEvidenceKind(
        ArtifactEnvelope artifact)
    {
        // Chapter 10 records the source as the raw-evidence artifact, while
        // Chapter 11 names the permitted evidence category "walkthrough".
        // Keep the historical artifact kind unchanged and map only the
        // policy-facing category.
        if (string.Equals(
                artifact.Kind,
                "raw-evidence",
                StringComparison.Ordinal) &&
            Path.GetFileName(artifact.Path)
                .Contains(
                    "walkthrough",
                    StringComparison.OrdinalIgnoreCase))
        {
            return "walkthrough";
        }

        return artifact.Kind;
    }

    public static void ValidateStage(
        LoopStage stage,
        SpecialistGovernance governance)
    {
        if (!string.Equals(
                stage.Specialist,
                governance.Policy.Specialist,
                StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Governance for {governance.Policy.Specialist} " +
                $"cannot be applied to {stage.Specialist}.");
        }

        if (!string.Equals(
                stage.CandidateKind,
                governance.Policy.OutputKind,
                StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Policy output {governance.Policy.OutputKind} " +
                $"does not match stage output {stage.CandidateKind}.");
        }
    }
}

public static class BrewUpGovernanceDefaults
{
    public static SpecialistGovernance Storyteller() =>
        new(
            new AutonomyPolicy(
                Specialist: "storyteller",
                OutputKind: "candidate-scenarios",
                AllowedEvidenceKinds:
                [
                    "accepted-facts",
                    "walkthrough"
                ],
                MayIntroduceNewTerms: true,
                MaxCorrectionAttempts: 2,
                Timeout: TimeSpan.FromMinutes(2),
                AllowedTools:
                [
                    "read-accepted-artifact"
                ]),
            new AlignmentContext(
                AcceptedArtifacts: [],
                WorkflowGoal:
                    "Turn accepted Stock facts into concrete scenarios " +
                    "without resolving unsupported policy.",
                ApprovedTerms:
                [
                    "Stock",
                    "Sales",
                    "set aside",
                    "hold",
                    "earmark"
                ],
                Unresolved:
                [
                    "ES-17"
                ],
                ProhibitedDecisionTypes:
                [
                    "short-quantity-policy",
                    "retry-owner",
                    "hold-duration-policy"
                ]));
}
