using BrewUp.Warehouse.Facade;
using BrewUp.Warehouse.Facade.Endpoints;

namespace BrewUp.Rest.Module;

/// <summary>
/// Warehouse module to register services and endpoints about Warehouse Bounded Context
/// </summary>
public class WarehouseModule : IModule
{
    
    /// <summary>
    /// Set module enabled or not
    /// </summary>
    public bool IsEnabled => true;
    
    /// <summary>
    /// Specify the loading order 
    /// </summary>
    public int Order => 0;
    
    
    /// <summary>
    /// Register module's services
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public IServiceCollection Register(WebApplicationBuilder builder)
    {
        builder.Services.AddWarehouse();
        
        return builder.Services;
    }

    /// <summary>
    /// Register module's endpoints
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public WebApplication Configure(WebApplication app)
    {
        app.MapWarehouseEndpoints();
        
        return app;
    }
}