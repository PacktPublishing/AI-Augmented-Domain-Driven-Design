using BrewUp.Sales.ReadModel.EventHandlers;
using BrewUp.Sales.ReadModel.Services;
using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using BrewUp.Shared.Messages.Events.Sagas;
using BrewUp.Shared.ReadModel;
using Lena.Core;
using Microsoft.Extensions.Logging.Abstractions;
using Muflone;
using Muflone.Messages.Events;
using SalesOrderConfirmed = BrewUp.Sales.SharedKernel.Messages.Events.SalesOrderConfirmed;

namespace BrewUp.Sales.Tests.ReadModel;

public sealed class SalesOrderConfirmedEventHandlerTests
{
    [Fact]
    public async Task Updates_Read_Model_And_Notifies_Saga()
    {
        var salesOrderId = new SalesOrderId(Guid.CreateVersion7().ToString());
        var paymentAuthorizationId = new PaymentAuthorizationId(Guid.CreateVersion7().ToString());
        var stockReservationId = new StockReservationId(Guid.CreateVersion7().ToString());
        var salesOrderService = new RecordingSalesOrderService();
        var eventBus = new RecordingEventBus();
        var handler = new SalesOrderConfirmedEventHandler(
            salesOrderService,
            eventBus,
            new NullLoggerFactory());

        await handler.HandleAsync(new SalesOrderConfirmed(
            salesOrderId,
            paymentAuthorizationId,
            stockReservationId,
            Guid.CreateVersion7()));

        Assert.Equal(salesOrderId.Value, salesOrderService.ConfirmedSalesOrderId?.Value);
        var integrationEvent = Assert.IsType<SagasSalesOrderAccepted>(Assert.Single(eventBus.PublishedEvents));
        Assert.Equal(salesOrderId.Value, integrationEvent.AggregateId.Value);
    }

    private sealed class RecordingEventBus : IEventBus
    {
        public List<IEvent> PublishedEvents { get; } = [];

        public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
            where TEvent : class, IEvent
        {
            PublishedEvents.Add(@event);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }

    private sealed class RecordingSalesOrderService : ISalesOrderService
    {
        public SalesOrderId? ConfirmedSalesOrderId { get; private set; }

        public Task<Result<string>> ConfirmSalesOrderAsync(
            SalesOrderId salesOrderId,
            PaymentAuthorizationId paymentAuthorizationId,
            StockReservationId stockReservationId,
            CancellationToken cancellationToken)
        {
            ConfirmedSalesOrderId = salesOrderId;
            return Task.FromResult(Result<string>.Success(salesOrderId.Value));
        }

        public Task<Result<string>> AddBeersToSalesOrderAsync(
            SalesOrderId salesOrderId,
            IEnumerable<SalesOrderRowJson> rows,
            CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task<Result<bool>> ChkAvailabilityForSagaRowsAsync(
            IEnumerable<ItemRequested> items,
            CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task<Result<bool>> CreateSalesOrderAsync(
            SalesOrderId salesOrderId,
            SalesOrderNumber salesOrderNumber,
            CustomerId customerId,
            CustomerName customerName,
            SalesOrderDate orderDate,
            IEnumerable<SalesOrderRowJson> rows,
            CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task<Result<CustomerTotalPurchased>> GetCustomerTotalPurchasedAsync(
            CustomerId customerId,
            CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task<Result<SalesOrderJson>> GetSalesOrderByIdAsync(
            string salesOrderId,
            CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task<Result<PagedResult<SalesOrderJson>>> GetSalesOrdersAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task<Result<PagedResult<SalesOrderTotalQuantity>>> GetSalesOrderTotalQuantitiesAsync(
            string salesOrderId,
            CancellationToken cancellationToken) =>
            throw new NotImplementedException();
    }
}
