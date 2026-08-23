using BrewUp.Sales.Facade.Acl;
using BrewUp.Sales.ReadModel.Services;
using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Sales.SharedKernel.Messages.Commands;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using BrewUp.Shared.Messages.Events.Sagas;
using BrewUp.Shared.ReadModel;
using Lena.Core;
using Microsoft.Extensions.Logging.Abstractions;
using Muflone.Messages.Commands;
using Muflone.Persistence;

namespace BrewUp.Sales.Tests.Facade;

public sealed class SagaSalesOrderAvailabilityCheckedIntegrationEventHandlerTests
{
    [Fact]
    public async Task Sends_ConfirmSalesOrder_When_Payment_Is_Authorized_And_All_Beers_Are_Available()
    {
        var salesOrderId = new SalesOrderId(Guid.CreateVersion7().ToString());
        var paymentAuthorizationId = new PaymentAuthorizationId(Guid.CreateVersion7().ToString());
        var stockReservationId = new StockReservationId(Guid.CreateVersion7().ToString());
        var serviceBus = new RecordingServiceBus();
        var handler = new SagaSalesOrderAvailabilityCheckedIntegrationEventHandler(
            new FixedAvailabilitySalesOrderService(true), serviceBus, new NullLoggerFactory());

        await handler.HandleAsync(new SagaSalesOrderAvailabilityCheckedIntegrationEvent(
            new IntegrationId(salesOrderId.Value),
            Guid.CreateVersion7(),
            salesOrderId.Value,
            paymentAuthorizationId,
            stockReservationId,
            [new ItemRequested(new BeerId(Guid.CreateVersion7().ToString()), new Quantity(2, "Bottle"), new Quantity(5, "Bottle"))]));

        var command = Assert.IsType<ConfirmSalesOrder>(serviceBus.SentCommand);
        Assert.Equal(salesOrderId.Value, command.AggregateId.Value);
        Assert.Equal(paymentAuthorizationId.Value, command.PaymentAuthorizationId.Value);
        Assert.Equal(stockReservationId.Value, command.StockReservationId.Value);
    }

    [Fact]
    public async Task Does_Not_Confirm_When_A_Beer_Is_Not_Available()
    {
        var salesOrderId = new SalesOrderId(Guid.CreateVersion7().ToString());
        var serviceBus = new RecordingServiceBus();
        var handler = new SagaSalesOrderAvailabilityCheckedIntegrationEventHandler(
            new FixedAvailabilitySalesOrderService(false), serviceBus, new NullLoggerFactory());

        await handler.HandleAsync(new SagaSalesOrderAvailabilityCheckedIntegrationEvent(
            new IntegrationId(salesOrderId.Value),
            Guid.CreateVersion7(),
            salesOrderId.Value,
            new PaymentAuthorizationId(Guid.CreateVersion7().ToString()),
            new StockReservationId(Guid.CreateVersion7().ToString()),
            [new ItemRequested(new BeerId(Guid.CreateVersion7().ToString()), new Quantity(5, "Bottle"), new Quantity(2, "Bottle"))]));

        Assert.Null(serviceBus.SentCommand);
    }

    private sealed class RecordingServiceBus : IServiceBus
    {
        public ICommand? SentCommand { get; private set; }

        public Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
            where TCommand : class, ICommand
        {
            SentCommand = command;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }

    private sealed class FixedAvailabilitySalesOrderService(bool isAvailable) : ISalesOrderService
    {
        public Task<Result<bool>> ChkAvailabilityForSagaRowsAsync(
            IEnumerable<ItemRequested> items,
            CancellationToken cancellationToken) =>
            Task.FromResult(isAvailable ? Result<bool>.Success(true) : Result<bool>.Error("Unavailable"));

        public Task<Result<string>> AddBeersToSalesOrderAsync(
            SalesOrderId salesOrderId,
            IEnumerable<SalesOrderRowJson> rows,
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

        public Task<Result<string>> ConfirmSalesOrderAsync(
            SalesOrderId salesOrderId,
            PaymentAuthorizationId paymentAuthorizationId,
            StockReservationId stockReservationId,
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
