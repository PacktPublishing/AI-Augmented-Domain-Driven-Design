using System.ComponentModel.DataAnnotations;

namespace BrewUp.Shared.ExternalContracts.Purchases;

public class CreatePurchaseOrderJson
{
    [Required]
    public string OrderNumber { get; set; } = string.Empty;
    [Required]
    public DateTime OrderDate { get; set; }
    
    [Required]
    public string SupplierId { get; set; } = string.Empty;
    
    public DateTime DeliveryDate { get; set; }
    
    [Required]
    public IEnumerable<PurchaseOrderRowJson> Rows { get; set; } = [];
}