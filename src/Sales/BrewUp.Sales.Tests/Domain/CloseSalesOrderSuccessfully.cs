using BrewUp.Sales.Domain.CommandHandlers;
using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Sales.SharedKernel.Enums;
using BrewUp.Sales.SharedKernel.Messages.Commands;
using BrewUp.Sales.SharedKernel.Messages.Events;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using Microsoft.Extensions.Logging.Abstractions;
using Muflone.CustomTypes;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using Muflone.SpecificationTests;

namespace BrewUp.Sales.Tests.Domain;

public sealed class CloseSalesOrderSuccessfully : CommandSpecification<CloseSalesOrder>
{
    private SalesOrderId _salesOrderId = new (Guid.CreateVersion7().ToString());
    private SalesOrderNumber _salesOrderNumber = new ("SO-1000");
    private CustomerId _customerId = new (Guid.CreateVersion7().ToString());
    private CustomerName _customerName = new ("John Doe");
    private Customer _customer;
    private SalesOrderDate _salesOrderDate = new (DateTime.UtcNow);
    private SalesOrderDeliveryDate _salesOrderDeliveryDate = new (DateTime.UtcNow.AddDays(7));
    private readonly List<SalesOrderRowJson> _rows = [];

    private Account _account = new (Guid.CreateVersion7().ToString(), "Muflone");
    
    private Guid _correlationId = Guid.CreateVersion7();

    public CloseSalesOrderSuccessfully()
    {
        _customer = new Customer(_customerId, _customerName, CustomerType.Gold);
    }
    
    protected override IEnumerable<DomainEvent> Given()
    {
        yield return new SalesOrderCreated(_salesOrderId, _salesOrderNumber, _salesOrderDate, _customer,
            _salesOrderDeliveryDate, _rows, _correlationId);
    }

    protected override CloseSalesOrder When() => new(_salesOrderId, _salesOrderDeliveryDate, _account, _correlationId);

    protected override ICommandHandlerAsync<CloseSalesOrder> OnHandler() =>
        new CloseSalesOrderCommandHandler(Repository, new NullLoggerFactory());

    protected override IEnumerable<DomainEvent> Expect()
    {
        yield return new SalesOrderClosed(_salesOrderId, _salesOrderDeliveryDate, _correlationId);
    }
}