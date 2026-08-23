using BrewUp.Purchases.SharedKernel.CustomTypes;
using BrewUp.Shared.ExternalContracts.Purchases;
using Muflone.Messages.Events;

namespace BrewUp.Purchases.SharedKernel.Messages.Events;

public sealed class PurchaseOrderSubmitted(PurchaseOrderId aggregateId,
    PurchaseOrderNumber purchaseOrderNumber,
    Supplier supplier,
    PurchaseOrderDate purchaseOrderDate,
    IEnumerable<BeerType> rows) : DomainEvent(aggregateId)
{
    public PurchaseOrderNumber PurchaseOrderNumber { get; private set; } = purchaseOrderNumber;
    public Supplier Supplier { get; private set; } = supplier;
    public PurchaseOrderDate PurchaseOrderDate { get; private set; } = purchaseOrderDate;
    public IEnumerable<BeerType> Rows { get; private set; } = rows;
}