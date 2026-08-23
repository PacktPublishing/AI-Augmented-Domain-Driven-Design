namespace BrewUp.Shared.CustomTypes;

public record AlcoholByVolume
{
    public decimal Value { get; init; }

    public AlcoholByVolume(decimal value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), value, "Value must be greater than or equal to 0");
        
        Value = value;
    }
}
