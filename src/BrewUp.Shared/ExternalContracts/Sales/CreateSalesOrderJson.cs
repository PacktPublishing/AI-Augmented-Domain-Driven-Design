using System.ComponentModel.DataAnnotations;

namespace BrewUp.Shared.ExternalContracts.Sales;

public class CreateSalesOrderJson
{
    [Required]
    public string OrderNumber { get; set; } = string.Empty;
    [Required]
    public DateTime OrderDate { get; set; }
    
    [Required]
    public string CustomerId { get; set; } = string.Empty;
    [Required]
    public string CustomerName { get; set; } = string.Empty;
    
    public DateTime DeliveryDate { get; set; }
    
    [Required]
    public IEnumerable<SalesOrderRowJson> Rows { get; set; } = [];
}