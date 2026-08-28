using BrewUp.Orchestration;

namespace BrewUp.Orchestration.Tests;

public sealed class AlignmentTraceTests
{
    [Fact]
    public async Task AlignmentAssessmentIsRecordedWithoutChangingCandidate()
    {
        var runner =
            new RecordingSpecialistRunner();

        var loop =
            new BrewUpOrchestrationLoop(
                "brewup-stock-01",
                TestFixtures.Stages,
                TestFixtures.RawEvidence,
                runner,
                new SteppingClock());

        await loop.RunCurrentAsync(
            "run-1");

        var candidate =
            loop.State.Candidate;

        loop.RecordAlignmentAssessment(
            "alignment-1",
            [
                new SemanticDivergence(
                    "candidate-term",
                    "reservation",
                    ["hold"],
                    "Candidate terminology differs.")
            ]);

        Assert.Same(
            candidate,
            loop.State.Candidate);

        var trace =
            Assert.Single(
                loop.AlignmentTrace);

        Assert.Equal(
            "alignment-1",
            trace.MessageId);

        Assert.Equal(
            candidate!.ContentSha256,
            trace.CandidateSha256);
    }
}
