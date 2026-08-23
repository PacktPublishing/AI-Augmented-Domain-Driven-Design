namespace BrewUp.Shared.Messages.Requests;

public abstract class AgentRequest(Guid correlationId)
{
    public Guid CorrelationId { get; init; } = correlationId;
}