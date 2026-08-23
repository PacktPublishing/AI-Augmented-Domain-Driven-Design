using System.ComponentModel.DataAnnotations;

namespace BrewUp.Shared.ExternalContracts.MasterData.Customers;

public class CustomerPropertiesJson
{
    [Required]
    public string CustomerId { get; set; } = string.Empty;
    [Required]
    public bool IsEnabled { get; set; } = true;
    [Required]
    public string ConsumerLevel { get; set; } = string.Empty;
    [Required]
    public decimal BudgetLimit { get; set; } = 0;
}