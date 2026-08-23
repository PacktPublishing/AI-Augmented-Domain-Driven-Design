using Microsoft.AspNetCore.Builder;

namespace BrewUp.MasterData.Facade.Endpoints;

public static class MasterDataEndpoints
{
    public static WebApplication MapMasterDataEndpoints(this WebApplication app)
    {
        CustomersEndpoint.MapCustomersEndPoints(app);
        SuppliersEndpoint.MapSuppliersEndPoints(app);
        BeersEndpoints.MapBeersEndPoints(app);
        MasterDataWarehouseEndpoints.MapWarehouseEndpoints(app);

        return app;
    }
}