using Muflone.Core;

namespace BrewUp.Purchases.SharedKernel.CustomTypes;

public class PurchaseOrderId(string value) : DomainId(value);