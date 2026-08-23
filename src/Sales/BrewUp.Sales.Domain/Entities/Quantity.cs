namespace BrewUp.Sales.Domain.Entities;

public record Quantity
{
    public decimal Value { get; init; }
    public string UnitOfMeasure { get; init; }

    public Quantity(decimal value, string unitOfMeasure)
    {
        if (value < 0)
            throw new ArgumentException("Value cannot be negative.", nameof(value));
        if (string.IsNullOrWhiteSpace(unitOfMeasure))
            throw new ArgumentException("Unit of measure cannot be null or empty.", nameof(unitOfMeasure));

        Value = value;
        UnitOfMeasure = unitOfMeasure;
    }
}