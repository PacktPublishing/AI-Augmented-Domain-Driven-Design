namespace BrewUp.Orchestration;

public interface ISpecialistTool
{
    string Name { get; }
}

public sealed class SpecialistToolRegistry(
    IEnumerable<ISpecialistTool> tools)
{
    private readonly IReadOnlyCollection<ISpecialistTool> _tools =
        tools.ToArray();

    public ISpecialistTool[] Resolve(
        AutonomyPolicy policy)
    {
        var allowed = policy.AllowedTools
            .ToHashSet(StringComparer.Ordinal);

        return _tools
            .Where(tool => allowed.Contains(tool.Name))
            .ToArray();
    }
}
