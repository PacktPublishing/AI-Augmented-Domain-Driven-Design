using BrewUp.Shared.ExternalContracts.Purchases;
using BrewUp.Shared.Validators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BrewUp.Purchases.Facade.Endpoints;

public static class PurchasesEndpoints
{
    public static WebApplication MapPurchasesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/v1/purchases")
            .WithTags("Purchases");
        
        group.MapPost("/", HandlePostPurchaseOrder)
            .AddEndpointFilter<ValidationFilter<CreatePurchaseOrderJson>>()
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Create a new purchase order")
            .WithDescription(
                "Creates a new purchase order. This endpoint is used to add a new purchase order.")
            .WithName("CreatePurchaseOrder");
        
        return app;
    }
    
    private static async Task<IResult> HandlePostPurchaseOrder(
        IPurchasesFacade purchasesFacade,
        CreatePurchaseOrderJson body,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var createResult = await purchasesFacade.CreatePurchaseOrderAsync(body, cancellationToken);

        return createResult.Match<IResult>(
            success =>
            {
                createResult.TryGetValue(out string orderId);
                return Results.Created($"/v1/purchases/{orderId}", success);
            }, 
            Results.BadRequest);
    }
}