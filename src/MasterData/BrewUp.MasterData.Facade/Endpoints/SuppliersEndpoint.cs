using BrewUp.Shared.ExternalContracts.MasterData.Suppliers;
using BrewUp.Shared.ReadModel;
using BrewUp.Shared.Validators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BrewUp.MasterData.Facade.Endpoints;

internal static class SuppliersEndpoint
{
    internal static void MapSuppliersEndPoints(WebApplication app)
    {
        var group = app.MapGroup("/v1/masterdata/suppliers")
            .WithTags("MasterData");
        
        group.MapPost("/", HandlePostSupplier)
            .AddEndpointFilter<ValidationFilter<RegisterSupplierJson>>()
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Create a new supplier")
            .WithDescription(
                "Creates a new supplier. This endpoint is used to add a new supplier.")
            .WithName("CreateSupplier");
        
        group.MapGet("/", HandleGetSuppliers)
            .Produces<PagedResult<SupplierJson>>()
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Get a list of suppliers")
            .WithDescription(
                "Get a list of suppliers.")
            .WithName("GetSuppliers");
        
        group.MapGet("/{supplierId}", HandleGetSupplierById)
            .Produces<SupplierJson>()
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Get a supplier details")
            .WithDescription(
                "Get full details of a supplier.")
            .WithName("GetSupplierById");
    }
    
    private static async Task<IResult> HandlePostSupplier(
        IMasterDataSupplierFacade masterDataFacade,
        RegisterSupplierJson body,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var createResult = await masterDataFacade.RegisterSupplierAsync(body, cancellationToken);

        return createResult.Match<IResult>(
            success =>
            {
                createResult.TryGetValue(out string supplierId);
                return Results.Created($"/v1/masterdata/suppliers/{supplierId}", success);
            }, 
            error => Results.Problem(error.Message, statusCode: StatusCodes.Status500InternalServerError));
    }
    
    private static async Task<IResult> HandleGetSuppliers(
        IMasterDataSupplierFacade masterDataFacade,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var queryResult = await masterDataFacade.GetSuppliersAsync(pageNumber, pageSize, cancellationToken);

        return queryResult.Match<IResult>(
            Results.Ok,
            error => Results.Problem(error.Message, statusCode: StatusCodes.Status500InternalServerError));
    }
    
    private static async Task<IResult> HandleGetSupplierById(
        IMasterDataSupplierFacade masterDataFacade,
        string supplierId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var queryResult = await masterDataFacade.GetSupplierByIdAsync(supplierId, cancellationToken);

        return queryResult.Match<IResult>(
            Results.Ok,
            error => Results.Problem(error.Message, statusCode: StatusCodes.Status500InternalServerError));
    }
}