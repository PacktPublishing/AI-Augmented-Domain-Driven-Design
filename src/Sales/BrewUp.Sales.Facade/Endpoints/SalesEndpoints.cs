using BrewUp.Shared.ExternalContracts.Sales;
using BrewUp.Shared.ReadModel;
using BrewUp.Shared.Validators;
using Lena.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BrewUp.Sales.Facade.Endpoints;

public static class SalesEndpoints
{
  public static WebApplication MapSalesEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/v1/sales")
        .WithTags("Sales");

    group.MapPost("/", HandlePostSalesOrder)
        .AddEndpointFilter<ValidationFilter<CreateSalesOrderJson>>()
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithSummary("Create a new sales order")
        .WithDescription(
            "Creates a new sales order. This endpoint is used to add a new sales order.")
        .WithName("CreateSalesOrder");
    
    group.MapPatch("/rows/{orderId}", HandleAddBeersToSalesOrder)
        .AddEndpointFilter<ValidationFilter<AddBeersToCartJson>>()
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithSummary("Add beers to an existing sales order")
        .WithDescription(
            "Add beers to an existing sales order. This endpoint is used to add beers to a sales order.")
        .WithName("AddBeersToSalesOrder");
    
    group.MapPut("/{orderId}", HandleCloseSalesOrder)
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithSummary("Close an existing sales order")
        .WithDescription(
            "Close a sales order. This endpoint is used to close a sales order.")
        .WithName("CloseSalesOrder");

    group.MapGet("/", HandleGetSalesOrder)
        .Produces<PagedResult<SalesOrderJson>>()
        .Produces(StatusCodes.Status500InternalServerError)
        .WithSummary("Get a list of sales orders")
        .WithDescription(
            "Get a list of sales orders.")
        .WithName("GetSalesOrder");
    
    group.MapGet("/{orderId}", HandleGetSalesOrderDetails)
        .Produces<PagedResult<SalesOrderJson>>()
        .Produces(StatusCodes.Status500InternalServerError)
        .WithSummary("Get sales order details")
        .WithDescription(
            "Get the full details of a sales order.")
        .WithName("GetSalesOrderDetails");

    return app;
  }

  private static async Task<IResult> HandlePostSalesOrder(
      ISalesFacade salesFacade,
      CreateSalesOrderJson body,
      CancellationToken cancellationToken)
  {
    cancellationToken.ThrowIfCancellationRequested();

    var createResult = await salesFacade.CreateSalesOrderAsync(body, cancellationToken);

    return createResult.Match<IResult>(
        success =>
        {
            createResult.TryGetValue(out string orderId);
            return Results.Created($"/v1/sales/{orderId}", success);
        }, 
        Results.BadRequest);
  }
  
  private static async Task<IResult> HandleAddBeersToSalesOrder(
      ISalesFacade salesFacade,
      string orderId,
      AddBeersToCartJson body,
      CancellationToken cancellationToken)
  {
      cancellationToken.ThrowIfCancellationRequested();

      var createResult = await salesFacade.AddBeersToSalesOrderAsync(body, cancellationToken);

      return createResult.Match<IResult>(
          success =>
          {
              createResult.TryGetValue(out string newOrderId);
              return Results.Created($"/v1/sales/{newOrderId}", success);
          }, 
          Results.BadRequest);
  }

  private static async Task<IResult> HandleCloseSalesOrder(
      ISalesFacade salesFacade,
      string orderId,
      CancellationToken cancellationToken)
  {
      cancellationToken.ThrowIfCancellationRequested();

      await salesFacade.CloseSalesOrderAsync(orderId, cancellationToken);

      return Results.NoContent();
  }

  private static async Task<IResult> HandleGetSalesOrder(
      ISalesFacade salesFacade,
      int pageNumber = 1,
      int pageSize = 10,
      CancellationToken cancellationToken = default)
  {
    cancellationToken.ThrowIfCancellationRequested();

    var queryResult =
        await salesFacade.GetSalesOrdersAsync(pageNumber, pageSize, cancellationToken);
    
    return queryResult.Match<IResult>(
        Results.Ok,
        error => Results.Problem(error.Message, statusCode: StatusCodes.Status500InternalServerError));
  }
  
  private static async Task<IResult> HandleGetSalesOrderDetails(
      ISalesFacade salesFacade,
      string orderId,
      CancellationToken cancellationToken = default)
  {
      cancellationToken.ThrowIfCancellationRequested();

      Result<SalesOrderJson> queryResult =
          await salesFacade.GetSalesOrderByIdAsync(orderId, cancellationToken);
      
      return queryResult.Match<IResult>(
          Results.Ok,
          error => Results.Problem(error.Message, statusCode: StatusCodes.Status500InternalServerError));
  }
}