using BrewUp.Sagas.Domain.Orchestrators;
using BrewUp.Sagas.SharedKernel.Messages.Commands;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sagas;
using Lena.Core;

namespace BrewUp.Sagas.Facade;

internal sealed class SagasFacade(ISalesOrderSagaOrchestrator salesOrderSagaOrchestrator) : ISagasFacade
{
    public async Task<Result<string>> PlaceSalesOrderAsync(PlaceSalesOrderJson body, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        StartSalesOrderSaga command = new(new IntegrationId(Guid.CreateVersion7().ToString()),
            Guid.CreateVersion7(),
            body.OrderNumber,
            body.OrderDate,
            body.CustomerId,
            body.WarehouseId,
            body.DeliveryDate,
            body.Rows);
        
        await salesOrderSagaOrchestrator.StartSagaAsync(command, cancellationToken);
        
        return Result.Success(command.AggregateId.Value);
    }
}