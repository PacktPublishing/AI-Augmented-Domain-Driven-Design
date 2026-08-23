using Serilog;

namespace BrewUp.Rest.Module;

/// <summary>
/// Logging Module for configuring Serilog as the logging provider in the application.
/// </summary>
public class LoggingModule : IModule
{
    /// <summary>
    /// Flag to indicate if the module is enabled.
    /// </summary>
    public bool IsEnabled => true;

    /// <summary>
    /// Order of the module during registration.
    /// </summary>
    public int Order => 0;

    /// <summary>
    /// Registers services related to this module.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public IServiceCollection Register(WebApplicationBuilder builder)
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        // Clear default logging providers and add Serilog
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(Log.Logger);

        return builder.Services;
    }

    /// <summary>
    /// Configures the application with this module's endpoints.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public WebApplication Configure(WebApplication app) => app;
}