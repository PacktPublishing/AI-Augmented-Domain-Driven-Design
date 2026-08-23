using System.ComponentModel.DataAnnotations;
using BrewUp.Shared.ExternalContracts.Sales;

namespace BrewUp.Shared.ExternalContracts.Sagas;

public class PlaceSalesOrderJson
{
    [Required]
    public string OrderNumber { get; set; } = string.Empty;
    [Required]
    public DateTime OrderDate { get; set; }
    
    [Required]
    public string CustomerId { get; set; } = string.Empty;

    [Required]
    public string WarehouseId { get; set; } = string.Empty;

    public DateTime DeliveryDate { get; set; }
    
    [Required]
    public IEnumerable<SalesOrderRowJson> Rows { get; set; } = [];
}