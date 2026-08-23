using BrewUp.MasterData.SharedKernel.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Commands;

namespace BrewUp.MasterData.SharedKernel.Messages.Commands;

public sealed class VerifyCustomerBudget(CustomerId aggregateId,
    Guid correlationId,
    Amount amountToCheck) : Command(aggregateId, correlationId)
{
    public Amount AmountToCheck { get; } = amountToCheck;
}