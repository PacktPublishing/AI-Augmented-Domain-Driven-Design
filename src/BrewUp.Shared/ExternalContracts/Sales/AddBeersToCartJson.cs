using System.ComponentModel.DataAnnotations;

namespace BrewUp.Shared.ExternalContracts.Sales;

public class AddBeersToCartJson
{
    [Required]
    public string OrderId { get; set; } = string.Empty;
    
    [Required]
    public IEnumerable<SalesOrderRowJson> Rows { get; set; } = [];
}