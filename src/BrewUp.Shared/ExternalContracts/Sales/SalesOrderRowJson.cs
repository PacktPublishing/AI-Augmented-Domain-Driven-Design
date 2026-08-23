using System.ComponentModel.DataAnnotations;
using BrewUp.Shared.CustomTypes;
using BrewUp.Shared.Validators;

namespace BrewUp.Shared.ExternalContracts.Sales;

public class SalesOrderRowJson : IEquatable<SalesOrderRowJson>
{
    [Required]
    public string BeerId { get; init; } = string.Empty;
    [Required]
    public string BeerName { get; init; } = string.Empty;
    
    [Required]
    [QuantityGreaterThanZero(ErrorMessage = "Quantity must be greater than 0")]
    [AttributeIsMandatory("UnitOfMeasure", ErrorMessage = "Unit of Measure is mandatory")]
    public Quantity Quantity { get; init; } = new (0, string.Empty);

    [Required]
    [PriceGreaterThanZero(ErrorMessage = "Price must be greater than 0")]
    [AttributeIsMandatory("Currency", ErrorMessage = "Currency is mandatory")]
    public Price Price { get; init; } = new(0, string.Empty);

    public bool Equals(SalesOrderRowJson? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return BeerId == other.BeerId && BeerName == other.BeerName && Quantity.Equals(other.Quantity) && Price.Equals(other.Price);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((SalesOrderRowJson) obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(BeerId, BeerName, Quantity, Price);
    }
}