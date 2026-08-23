using BrewUp.Sales.Domain.CommandHandlers;
using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Sales.SharedKernel.Enums;
using BrewUp.Sales.SharedKernel.Messages.Commands;
using BrewUp.Sales.SharedKernel.Messages.Events;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using Microsoft.Extensions.Logging.Abstractions;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using Muflone.SpecificationTests;

namespace BrewUp.Sales.Tests.Domain;

public sealed class ConfirmSalesOrderSuccessfully : CommandSpecification<ConfirmSalesOrder>
{
    private readonly SalesOrderId _salesOrderId = new(Guid.CreateVersion7().ToString());
    private readonly SalesOrderNumber _salesOrderNumber = new("SO-2000");
    private readonly SalesOrderDate _salesOrderDate = new(DateTime.UtcNow);
    private readonly SalesOrderDeliveryDate _salesOrderDeliveryDate = new(DateTime.UtcNow.AddDays(7));
    private readonly List<SalesOrderRowJson> _rows = [];
    private readonly Customer _customer;

    private readonly PaymentAuthorizationId _paymentAuthorizationId = new(Guid.CreateVersion7().ToString());
    private readonly StockReservationId _stockReservationId = new(Guid.CreateVersion7().ToString());
    private readonly Guid _correlationId = Guid.CreateVersion7();

    public ConfirmSalesOrderSuccessfully()
    {
        var customerId = new CustomerId(Guid.CreateVersion7().ToString());
        var customerName = new CustomerName("Jane Smith");
        _customer = new Customer(customerId, customerName, CustomerType.Bronze);
    }

    protected override IEnumerable<DomainEvent> Given()
    {
        yield return new SalesOrderCreated(_salesOrderId, _salesOrderNumber, _salesOrderDate, _customer,
            _salesOrderDeliveryDate, _rows, _correlationId);
    }

    protected override ConfirmSalesOrder When() =>
        new(_salesOrderId, _paymentAuthorizationId, _stockReservationId, _correlationId);

    protected override ICommandHandlerAsync<ConfirmSalesOrder> OnHandler() =>
        new ConfirmSalesOrderCommandHandler(Repository, new NullLoggerFactory());

    protected override IEnumerable<DomainEvent> Expect()
    {
        yield return new SalesOrderConfirmed(_salesOrderId, _paymentAuthorizationId, _stockReservationId,
            _correlationId);
    }
}
