using System.Text.RegularExpressions;

namespace BrewUp.Dashboards.SharedKernel.CustomTypes;

public record SalesOrderYear
{
    public string Value { get; init; }

    private static readonly Regex Regex =
        new(@"^[1-9]\d{3}$", RegexOptions.None, TimeSpan.FromMilliseconds(250));

    public SalesOrderYear(string value)
    {
        if (!Regex.IsMatch(value))
            throw new ArgumentException("Invalid Year format!", nameof(value));

        Value = value;
    }
}