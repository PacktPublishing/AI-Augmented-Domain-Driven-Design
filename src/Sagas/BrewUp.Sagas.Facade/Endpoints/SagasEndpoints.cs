using BrewUp.Shared.ExternalContracts.Sagas;
using BrewUp.Shared.Validators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BrewUp.Sagas.Facade.Endpoints;

public static class SagasEndpoints
{
  public static WebApplication MapSagasEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/v1/sagas")
        .WithTags("Sagas");

    group.MapPost("/", HandlePlaceSalesOrder)
        .AddEndpointFilter<ValidationFilter<PlaceSalesOrderJson>>()
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithSummary("Place a new sales order")
        .WithDescription(
            "Place a new sales order. This endpoint is used to place a new sales order.")
        .WithName("PlaceSalesOrder");
    
    return app;
  }

  private static async Task<IResult> HandlePlaceSalesOrder(
      ISagasFacade sagasFacade,
      PlaceSalesOrderJson body,
      CancellationToken cancellationToken)
  {
    cancellationToken.ThrowIfCancellationRequested();

    var createResult = await sagasFacade.PlaceSalesOrderAsync(body, cancellationToken);

    return createResult.Match<IResult>(
        success =>
        {
            createResult.TryGetValue(out string orderId);
            return Results.Created($"/v1/sagas/{orderId}", success);
        }, 
        Results.BadRequest);
  }
}