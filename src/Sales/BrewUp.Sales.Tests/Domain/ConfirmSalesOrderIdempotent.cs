using BrewUp.Sales.Domain.CommandHandlers;
using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Sales.SharedKernel.Enums;
using BrewUp.Sales.SharedKernel.Messages.Commands;
using BrewUp.Sales.SharedKernel.Messages.Events;
using BrewUp.Shared.ExternalContracts.Sales;
using Microsoft.Extensions.Logging.Abstractions;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using Muflone.SpecificationTests;

namespace BrewUp.Sales.Tests.Domain;

public sealed class ConfirmSalesOrderIdempotent : CommandSpecification<ConfirmSalesOrder>
{
    private readonly SalesOrderId _salesOrderId = new(Guid.CreateVersion7().ToString());
    private readonly SalesOrderNumber _salesOrderNumber = new("SO-2003");
    private readonly SalesOrderDate _salesOrderDate = new(DateTime.UtcNow);
    private readonly SalesOrderDeliveryDate _salesOrderDeliveryDate = new(DateTime.UtcNow.AddDays(7));
    private readonly Customer _customer;
    private readonly List<SalesOrderRowJson> _rows = [];
    private readonly PaymentAuthorizationReference _paymentRef = new(Guid.CreateVersion7().ToString());
    private readonly StockReservationReference _stockRef = new(Guid.CreateVersion7().ToString());
    private readonly Guid _correlationId = Guid.CreateVersion7();

    public ConfirmSalesOrderIdempotent()
    {
        _customer = new Customer(
            new Shared.DomainIds.CustomerId(Guid.CreateVersion7().ToString()),
            new CustomerName("Acme Corp"),
            CustomerType.Gold);
    }

    protected override IEnumerable<DomainEvent> Given()
    {
        yield return new SalesOrderCreated(_salesOrderId, _salesOrderNumber, _salesOrderDate,
            _customer, _salesOrderDeliveryDate, _rows, _correlationId);
        // Order is already confirmed
        yield return new SalesOrderConfirmed(_salesOrderId, _correlationId, _paymentRef, _stockRef);
    }

    // Sending ConfirmSalesOrder again — must be idempotent (FR-009)
    protected override ConfirmSalesOrder When() =>
        new(_salesOrderId, _correlationId, _paymentRef, _stockRef);

    protected override ICommandHandlerAsync<ConfirmSalesOrder> OnHandler() =>
        new ConfirmSalesOrderCommandHandler(Repository, new NullLoggerFactory());

    protected override IEnumerable<DomainEvent> Expect()
    {
        // No second SalesOrderConfirmed — idempotency guard prevents duplicate
        yield break;
    }
}
