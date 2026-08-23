namespace BrewUp.Sagas.Infrastructure.Hubs;

public interface ISagasHubHelper
{
    Task TellChildrenThatSalesOrderSagaWasRejected(string salesOrderId, CancellationToken cancellationToken);
    Task TellChildrenThatSalesOrderSagaWasCompleted(string salesOrderId, CancellationToken cancellationToken);
}