using System.Text.RegularExpressions;

namespace BrewUp.Shared.CustomTypes;

public record CodiceFiscale
{
    public string Value { get; init; }

    private static readonly Regex Regex =
        new(
            @"^[A-Z]{6}[0-9]{2}[A-Z][0-9]{2}[A-Z][0-9]{3}[A-Z]$",
            RegexOptions.IgnoreCase);

    public CodiceFiscale(string value)
    {
        if (!Regex.IsMatch(value))
            throw new ArgumentException("Invalid Codice Fiscale format!", nameof(value));
        
        Value = value.ToUpperInvariant();
    }
}