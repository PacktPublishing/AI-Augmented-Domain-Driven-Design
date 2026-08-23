using BrewUp.Shared.ExternalContracts.Warehouse;
using BrewUp.Shared.ReadModel;
using BrewUp.Shared.Validators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BrewUp.MasterData.Facade.Endpoints;

internal static class MasterDataWarehouseEndpoints
{
    internal static WebApplication MapWarehouseEndpoints(WebApplication app)
    {
        var group = app.MapGroup("/v1/masterdata/warehouse")
            .WithTags("MasterData");
        
        group.MapPost("/", HandlePostWarehouse)
            .AddEndpointFilter<ValidationFilter<CreateWarehouseJson>>()
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Create a new warehouse")
            .WithDescription(
                "Creates a new warehouse. This endpoint is used to add a new warehouse.")
            .WithName("CreateWarehouse");
        
        group.MapGet("/", HandleGetWarehouses)
            .Produces<PagedResult<WarehouseJson>>()
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Get a list of warehouses")
            .WithDescription(
                "Get a list of warehouses.")
            .WithName("GetWarehouses");

        return app;
    }
    
    private static async Task<IResult> HandlePostWarehouse(
        IMasterDataWarehouseFacade warehouseFacade,
        CreateWarehouseJson body,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var createResult = await warehouseFacade.CreateWarehouseAsync(body, cancellationToken);

        return createResult.Match<IResult>(
            success =>
            {
                createResult.TryGetValue(out string warehouseId);
                return Results.Created($"/v1/warehouse/{warehouseId}", success);
            }, 
            Results.BadRequest);
    }
    
    private static async Task<IResult> HandleGetWarehouses(
        IMasterDataWarehouseFacade warehouseFacade,
        CancellationToken cancellationToken,
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 10)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var warehousesResult = await warehouseFacade.GetWarehousesAsync(page, pageSize, cancellationToken);

        return warehousesResult.Match(
            Results.Ok, 
            Results.BadRequest);
    }
}