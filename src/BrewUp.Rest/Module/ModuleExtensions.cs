namespace BrewUp.Rest.Module;

internal static class ModuleExtensions
{
    private static readonly IList<IModule> RegisteredModules = new List<IModule>();

    internal static WebApplicationBuilder RegisterModules(
        this WebApplicationBuilder builder,
        IEnumerable<IModule> modules)
    {
        // Clear & re‑add in case of multiple calls (defensive)
        RegisteredModules.Clear();

        var activeModules = modules
            .Where(m => m is not null && m.IsEnabled)
            .OrderBy(m => m.Order)
            .ToList();

        foreach (var module in activeModules)
        {
            module.Register(builder);
            RegisteredModules.Add(module);
        }

        return builder;
    }

    internal static WebApplication ConfigureModules(this WebApplication app)
    {
        foreach (var module in RegisteredModules)
        {
            module.Configure(app);
        }

        return app;
    }
}