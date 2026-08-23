using System.ComponentModel.DataAnnotations;

namespace BrewUp.Shared.ExternalContracts.Warehouse;

public class CreateWarehouseJson
{
    [Required]
    public string Name { get; set; } = string.Empty;
}