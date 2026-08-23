using BrewUp.Shared.DomainIds;
using Muflone.Messages.Commands;

namespace BrewUp.Sagas.SharedKernel.Messages.Commands;

public sealed class RejectSalesOrder(IntegrationId aggregateId, 
    Guid correlationId,
    string message) : Command(aggregateId, correlationId)
{
    public string Message { get; private set; } = message;
}