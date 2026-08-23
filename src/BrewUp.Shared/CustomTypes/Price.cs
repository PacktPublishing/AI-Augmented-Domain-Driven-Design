using System.ComponentModel.DataAnnotations;
using BrewUp.Shared.Validators;

namespace BrewUp.Shared.CustomTypes;

public class Price(decimal value, string currency) : IEquatable<Price>
{
    [Required]
    [PriceGreaterThanZero(ErrorMessage = "Price must be greater than 0")]
    public decimal Value { get; init; } = value;

    [Required]
    public string Currency { get; init; } = currency;

    public bool Equals(Price? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value && Currency == other.Currency;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Price) obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Value, Currency);
    }
}