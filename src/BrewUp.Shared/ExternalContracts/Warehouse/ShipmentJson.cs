using BrewUp.Shared.ExternalContracts.Sales;

namespace BrewUp.Shared.ExternalContracts.Warehouse;

public class ShipmentJson
{
    public string Id { get; set; } =  string.Empty;
    public string SalesOrderId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public DateTime DeliveryDate { get; set; }
    public IEnumerable<OrderRowDto> Rows { get; set; } = [];
    public string ShipmentState { get; set; } = string.Empty;
}