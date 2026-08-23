using BrewUp.Infrastructure;

namespace BrewUp.Rest.Module;

/// <summary>
/// Infrastructure Module for configuring the infrastructure services and dependencies in the application.
/// </summary>
public class InfrastructureModule : IModule
{
    /// <summary>
    /// Indicates whether the module is enabled and should be registered in the application.
    /// </summary>
    public bool IsEnabled => true;
    /// <summary>
    /// Set the order in which the module should be registered in the application.
    /// Modules with lower order values will be registered before those with higher values.
    /// </summary>
    public int Order => 0;
    
    /// <summary>
    /// Registers the module's services and dependencies in the application's service collection.
    /// This method is called during the application startup process to configure the module's services.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public IServiceCollection Register(WebApplicationBuilder builder)
    {
        using var serviceProvider = builder.Services.BuildServiceProvider();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        builder.Services.AddInfrastructure(loggerFactory, builder.Configuration);
        
        return builder.Services;
    }

    /// <summary>
    /// Configures the module's middleware and request pipeline in the application.
    /// This method is called during the application startup process to set up the module's middleware and request handling logic.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public WebApplication Configure(WebApplication app) => app;
}