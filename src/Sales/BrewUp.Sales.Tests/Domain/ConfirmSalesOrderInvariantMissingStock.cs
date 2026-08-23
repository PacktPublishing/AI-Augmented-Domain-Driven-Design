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

public sealed class ConfirmSalesOrderInvariantMissingStock : CommandSpecification<ConfirmSalesOrder>
{
    private readonly SalesOrderId _salesOrderId = new(Guid.CreateVersion7().ToString());
    private readonly SalesOrderNumber _salesOrderNumber = new("SO-2002");
    private readonly SalesOrderDate _salesOrderDate = new(DateTime.UtcNow);
    private readonly SalesOrderDeliveryDate _salesOrderDeliveryDate = new(DateTime.UtcNow.AddDays(7));
    private readonly Customer _customer;
    private readonly List<SalesOrderRowJson> _rows = [];
    private readonly PaymentAuthorizationReference _paymentRef = new(Guid.CreateVersion7().ToString());
    private readonly Guid _correlationId = Guid.CreateVersion7();

    public ConfirmSalesOrderInvariantMissingStock()
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
    }

    // Empty StockReservationReference — invariant must block confirmation (BC-010)
    protected override ConfirmSalesOrder When() =>
        new(_salesOrderId, _correlationId, _paymentRef, new StockReservationReference(string.Empty));

    protected override ICommandHandlerAsync<ConfirmSalesOrder> OnHandler() =>
        new ConfirmSalesOrderCommandHandler(Repository, new NullLoggerFactory());

    protected override IEnumerable<DomainEvent> Expect()
    {
        // No SalesOrderConfirmed — invariant prevents confirmation without stock reference
        yield break;
    }
}
