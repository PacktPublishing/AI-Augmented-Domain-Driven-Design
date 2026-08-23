using BrewUp.Dashboards.Infrastructure.Hubs;
using BrewUp.Shared.ExternalContracts.Dashboards;
using BrewUp.Shared.ReadModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BrewUp.Dashboards.Facade.Endpoints;

public static class DashboardsEndpoints
{
    extension(WebApplication app)
    {
        public WebApplication MapDashboardsEndpoints()
        {
            var group = app.MapGroup("/v1/dashboards")
                .WithTags("Dashboards");
        
            group.MapGet("/customers", HandleGetSalesByCustomers)
                .Produces<PagedResult<SalesByCustomerJson>>()
                .Produces(StatusCodes.Status500InternalServerError)
                .WithSummary("Get a list of sales by Customer")
                .WithDescription(
                    "Get a list of sales by Customer.")
                .WithName("GetSalesByCustomer");
        
            group.MapGet("/products", HandleGetSalesByProducts)
                .Produces<PagedResult<SalesByProductsJson>>()
                .Produces(StatusCodes.Status500InternalServerError)
                .WithSummary("Get a list of sales by Product")
                .WithDescription(
                    "Get a list of sales by Product.")
                .WithName("GetSalesByProduct");
        
            return app;
        }

        public WebApplication MapDashboardsSignalR()
        {
            app.MapHub<DashboardsHub>("/hubs/dashboards");

            return app;
        }
    }

    private static async Task<IResult> HandleGetSalesByCustomers(
        IDashboardsFacade dashboardsFacade,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var queryResult =
            await dashboardsFacade.GetSalesByCustomerAsync(pageNumber, pageSize, cancellationToken);
    
        return queryResult.Match<IResult>(
            Results.Ok,
            error => Results.Problem(error.Message, statusCode: StatusCodes.Status500InternalServerError));
    }
    
    private static async Task<IResult> HandleGetSalesByProducts(
        IDashboardsFacade dashboardsFacade,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var queryResult =
            await dashboardsFacade.GetSalesByProductAsync(pageNumber, pageSize, cancellationToken);
    
        return queryResult.Match<IResult>(
            Results.Ok,
            error => Results.Problem(error.Message, statusCode: StatusCodes.Status500InternalServerError));
    }
}