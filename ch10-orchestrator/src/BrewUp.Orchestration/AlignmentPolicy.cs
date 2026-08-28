namespace BrewUp.Orchestration;

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
    string[] AllowedEvidenceRefs,
    string WorkflowGoal,
    string[] ApprovedTerms,
    string[] Unresolved,
    string[] ProhibitedDecisionTypes);

public sealed record CandidateAlignmentMetadata(
    string[] Terms,
    string[] DecisionTypes);

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
    string CandidateSha256,
    SemanticDivergence[] Divergences);

public static class Chapter10Policies
{
    public static AutonomyPolicy Storyteller() => new(
        Specialist: "storyteller",
        OutputKind: "candidate-scenarios",
        AllowedEvidenceKinds:
        [
            "raw-evidence",
            "accepted-facts"
        ],
        MayIntroduceNewTerms: true,
        MaxCorrectionAttempts: 2,
        Timeout: TimeSpan.FromSeconds(30),
        AllowedTools:
        [
            "read-accepted-artifact",
            "read-evidence"
        ]);

    public static AlignmentContext StorytellerAlignment(
        params ArtifactEnvelope[] acceptedArtifacts) => new(
        AcceptedArtifacts: acceptedArtifacts,
        AllowedEvidenceRefs:
        [
            "OBS-01",
            "OBS-02",
            "OBS-03",
            "ES-17"
        ],
        WorkflowGoal:
            "Turn accepted Stock facts into concrete scenarios " +
            "without resolving unsupported policy.",
        ApprovedTerms:
        [
            "Stock",
            "Sales",
            "set aside",
            "hold"
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
        ]);
}
