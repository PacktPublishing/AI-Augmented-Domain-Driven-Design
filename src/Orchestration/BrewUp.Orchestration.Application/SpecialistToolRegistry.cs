namespace BrewUp.Orchestration.Application;

public sealed class SpecialistToolRegistry
{
    private readonly string[]? _availableTools;

    public static SpecialistToolRegistry Unrestricted { get; } =
        new(availableTools: null);

    public SpecialistToolRegistry(
        IEnumerable<string>? availableTools)
    {
        _availableTools = availableTools?
            .Distinct(StringComparer.Ordinal)
            .ToArray();
    }

    public string[] Resolve(
        AutonomyPolicy policy)
    {
        if (_availableTools is null)
            return [.. policy.AllowedTools];

        var available = _availableTools
            .ToHashSet(StringComparer.Ordinal);

        return policy.AllowedTools
            .Where(available.Contains)
            .ToArray();
    }
}
