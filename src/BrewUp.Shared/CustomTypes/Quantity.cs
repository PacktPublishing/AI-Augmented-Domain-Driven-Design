using System.ComponentModel.DataAnnotations;
using BrewUp.Shared.Validators;

namespace BrewUp.Shared.CustomTypes;

public class Quantity(decimal value, string unitOfMeasure) : IEquatable<Quantity>
{
    [Required]
    [QuantityGreaterThanZero(ErrorMessage = "Quantity must be greater than 0")]
    public decimal Value { get; init; } = value;

    [Required]
    public string UnitOfMeasure { get; init; } = unitOfMeasure;

    public bool Equals(Quantity? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value && UnitOfMeasure == other.UnitOfMeasure;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Quantity) obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Value, UnitOfMeasure);
    }
}