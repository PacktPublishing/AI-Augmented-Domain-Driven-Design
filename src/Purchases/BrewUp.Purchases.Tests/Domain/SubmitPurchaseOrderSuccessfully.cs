using BrewUp.Purchases.SharedKernel.CustomTypes;
using BrewUp.Purchases.SharedKernel.Messages.Commands;
using BrewUp.Purchases.SharedKernel.Messages.Events;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using Muflone.SpecificationTests;

namespace BrewUp.Purchases.Tests.Domain;

public sealed class SubmitPurchaseOrderSuccessfully : CommandSpecification<SubmitPurchaseOrder>
{
    private readonly PurchaseOrderId _purchaseOrderId = new(Guid.CreateVersion7().ToString());
    private readonly SupplierId _supplierId = new(Guid.CreateVersion7().ToString());
    private readonly RagioneSociale _ragioneSociale = new("Ragione Sociale");
    private readonly PurchaseOrderNumber _purchaseOrderNumber = new("PO-12345");
    private readonly PurchaseOrderDate _purchaseOrderDate = new(DateTime.UtcNow);
    private readonly IEnumerable<BeerType> _rows = [];
    
    protected override IEnumerable<DomainEvent> Given()
    {
        yield break;
    }

    protected override SubmitPurchaseOrder When() => new(_purchaseOrderId, _purchaseOrderNumber,
        new Supplier(_supplierId, _ragioneSociale), _purchaseOrderDate, _rows);

    protected override ICommandHandlerAsync<SubmitPurchaseOrder> OnHandler()
    {
        throw new NotImplementedException();
    }

    protected override IEnumerable<DomainEvent> Expect()
    {
        yield return new PurchaseOrderSubmitted(_purchaseOrderId, _purchaseOrderNumber,
            new Supplier(_supplierId, _ragioneSociale), _purchaseOrderDate, _rows);
    }
}