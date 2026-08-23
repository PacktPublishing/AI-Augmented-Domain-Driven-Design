using BrewUp.Shared.DomainIds;

namespace BrewUp.Warehouse.SharedKernel.CustomTypes;

public record ReorderThreshold(BeerId BeerId, ThresholdQuantity ThresholdQuantity);