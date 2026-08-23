using BrewUp.Sales.Domain.CommandHandlers;
using BrewUp.Sales.SharedKernel.CustomTypes;
using BrewUp.Sales.SharedKernel.Enums;
using BrewUp.Sales.SharedKernel.Messages.Commands;
using BrewUp.Sales.SharedKernel.Messages.Events;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using Microsoft.Extensions.Logging.Abstractions;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using Muflone.SpecificationTests;

namespace BrewUp.Sales.Tests.Domain;

public sealed class AddBeersToCartSuccessfully : CommandSpecification<AddBeersToCart>
{
    private SalesOrderId _salesOrderId = new (Guid.CreateVersion7().ToString());
    private SalesOrderNumber _salesOrderNumber = new ("SO-1000");
    private CustomerId _customerId = new (Guid.CreateVersion7().ToString());
    private CustomerName _customerName = new ("John Doe");
    private readonly Customer _customer = new (
        new CustomerId(Guid.CreateVersion7().ToString()),
        new CustomerName("John Doe"),
        CustomerType.Gold
    );
    private SalesOrderDate _salesOrderDate = new (DateTime.UtcNow);
    private SalesOrderDeliveryDate _salesOrderDeliveryDate = new (DateTime.UtcNow.AddDays(7));
    private readonly List<SalesOrderRowJson> _rows = [];
    
    private readonly IEnumerable<SalesOrderRowJson> _totalRows = new List<SalesOrderRowJson>
    {
        new ()
        {
            BeerId = Guid.CreateVersion7().ToString(),
            Quantity = new Quantity(2, "Bottles")
        }
    };

    private Guid _correlationId = Guid.CreateVersion7();
   
    protected override IEnumerable<DomainEvent> Given()
    {
        yield return new SalesOrderCreated(_salesOrderId, _salesOrderNumber, _salesOrderDate, _customer,
            _salesOrderDeliveryDate, _rows, _correlationId);
    }

    protected override AddBeersToCart When() => new (_salesOrderId, _totalRows);

    protected override ICommandHandlerAsync<AddBeersToCart> OnHandler() =>
        new AddBeersToCartCommandHandler(Repository, new NullLoggerFactory());

    protected override IEnumerable<DomainEvent> Expect()
    {
        yield return new BeersAddedToCart(_salesOrderId, _totalRows);
    }
}