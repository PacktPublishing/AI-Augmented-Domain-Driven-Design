using BrewUp.Shared.DomainIds;
using BrewUp.Shared.ExternalContracts.Sales;
using Muflone.Messages.Events;

namespace BrewUp.Shared.Messages.Events.Mother;

public sealed class SalesOrderAssessment(
    IntegrationId aggregateId,
    string salesOrderId,
    string customerId,
    string customerName,
    decimal totalAmount,
    IReadOnlyCollection<SalesOrderRowJson> rows,
    string priority,
    string reason) : IntegrationEvent(aggregateId)
{
    public string SalesOrderId { get; init; } = salesOrderId;
    public string CustomerId { get; init; } = customerId;
    public string CustomerName { get; init; } = customerName;
    public decimal TotalAmount { get; init; } = totalAmount;
    public IReadOnlyCollection<SalesOrderRowJson> Rows { get; init; } = rows;
    public string Priority { get; init; } = priority;
    public string Reason { get; init; } = reason;
}