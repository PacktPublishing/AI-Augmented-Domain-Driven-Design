namespace BrewUp.Orchestration;

public static class AlignmentGuard
{
    public static AlignmentIssue[] Check(
        ArtifactEnvelope candidate,
        CandidateAlignmentMetadata metadata,
        LoopStage stage,
        AutonomyPolicy policy,
        AlignmentContext context)
    {
        var issues = new List<AlignmentIssue>();

        if (!string.Equals(
                candidate.Kind,
                stage.CandidateKind,
                StringComparison.Ordinal))
        {
            issues.Add(new(
                "wrong-artifact-kind",
                $"Expected {stage.CandidateKind}, got {candidate.Kind}."));
        }

        if (!string.Equals(
                candidate.Producer,
                policy.Specialist,
                StringComparison.Ordinal))
        {
            issues.Add(new(
                "wrong-producer",
                $"Expected {policy.Specialist}, got {candidate.Producer}."));
        }

        var unknownEvidence = candidate.EvidenceRefs
            .Except(
                context.AllowedEvidenceRefs,
                StringComparer.Ordinal)
            .ToArray();

        if (unknownEvidence.Length > 0)
        {
            issues.Add(new(
                "unknown-evidence",
                string.Join(", ", unknownEvidence)));
        }

        var lostUnresolved = context.Unresolved
            .Except(
                candidate.Unresolved,
                StringComparer.Ordinal)
            .ToArray();

        if (lostUnresolved.Length > 0)
        {
            issues.Add(new(
                "dropped-unresolved",
                string.Join(", ", lostUnresolved)));
        }

        if (!policy.MayIntroduceNewTerms)
        {
            var unknownTerms = metadata.Terms
                .Except(
                    context.ApprovedTerms,
                    StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (unknownTerms.Length > 0)
            {
                issues.Add(new(
                    "unapproved-term",
                    string.Join(", ", unknownTerms)));
            }
        }

        var forbiddenDecisions = metadata.DecisionTypes
            .Intersect(
                context.ProhibitedDecisionTypes,
                StringComparer.Ordinal)
            .ToArray();

        if (forbiddenDecisions.Length > 0)
        {
            issues.Add(new(
                "prohibited-decision",
                string.Join(", ", forbiddenDecisions)));
        }

        return [.. issues];
    }
}
