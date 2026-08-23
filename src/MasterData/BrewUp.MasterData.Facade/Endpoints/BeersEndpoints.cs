using BrewUp.Shared.ExternalContracts.MasterData.Beers;
using BrewUp.Shared.Validators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BrewUp.MasterData.Facade.Endpoints;

internal static class BeersEndpoints
{
    internal static void MapBeersEndPoints(WebApplication app)
    {
        var group = app.MapGroup("/v1/masterdata/beers")
            .WithTags("MasterData");
        
        group.MapPost("/", HandlePostBeer)
            .AddEndpointFilter<ValidationFilter<RegisterBeerJson>>()
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Create a new beer")
            .WithDescription(
                "Creates a new beer. This endpoint is used to add a new beer.")
            .WithName("CreateBeer");
        
        group.MapGet("/", HandleGetBeers)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Get a list of beers")
            .WithDescription(
                "Get a list of all beers. This endpoint is used to get a list of all beers.")
            .WithName("GetBeers");
    }
    
    private static async Task<IResult> HandlePostBeer(
        IMasterDataBeerFacade beerFacade,
        RegisterBeerJson body,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var createResult = await beerFacade.RegisterBeerAsync(body, cancellationToken);

        return createResult.Match<IResult>(
            success =>
            {
                createResult.TryGetValue(out string beerId);
                return Results.Created($"/v1/masterdata/beers/{beerId}", success);
            }, 
            error => Results.Problem(error.Message, statusCode: StatusCodes.Status500InternalServerError));
    }
    
    private static async Task<IResult> HandleGetBeers(
        IMasterDataBeerFacade masterDataBeerFacade,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var queryResult = await masterDataBeerFacade.GetBeersAsync(pageNumber, pageSize, cancellationToken);

        return queryResult.Match<IResult>(
            Results.Ok,
            error => Results.Problem(error.Message, statusCode: StatusCodes.Status500InternalServerError));
    }
}