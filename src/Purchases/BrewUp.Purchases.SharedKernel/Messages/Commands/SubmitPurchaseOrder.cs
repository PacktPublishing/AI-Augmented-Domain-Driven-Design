using BrewUp.Purchases.SharedKernel.CustomTypes;
using Muflone.Messages.Commands;

namespace BrewUp.Purchases.SharedKernel.Messages.Commands;

public sealed class SubmitPurchaseOrder(PurchaseOrderId aggregateId,
    PurchaseOrderNumber purchaseOrderNumber,
    Supplier supplier,
    PurchaseOrderDate purchaseOrderDate,
    IEnumerable<BeerType> rows) : Command(aggregateId)
{
    public PurchaseOrderNumber PurchaseOrderNumber { get; private set; } = purchaseOrderNumber;
    public Supplier Supplier { get; private set; } = supplier;
    public PurchaseOrderDate PurchaseOrderDate { get; private set; } = purchaseOrderDate;
    public IEnumerable<BeerType> Rows { get; private set; } = rows;
}