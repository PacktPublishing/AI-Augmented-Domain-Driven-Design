using BrewUp.Sales.SharedKernel.Enums;
using BrewUp.Shared.DomainIds;

namespace BrewUp.Sales.SharedKernel.CustomTypes;

public record Customer(CustomerId CustomerId, CustomerName CustomerName, CustomerType CustomerType);