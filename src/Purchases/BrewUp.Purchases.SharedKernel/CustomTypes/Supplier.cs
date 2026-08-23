using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.DomainIds;

namespace BrewUp.Purchases.SharedKernel.CustomTypes;

public record Supplier(SupplierId SupplierId, RagioneSociale RagioneSociale);