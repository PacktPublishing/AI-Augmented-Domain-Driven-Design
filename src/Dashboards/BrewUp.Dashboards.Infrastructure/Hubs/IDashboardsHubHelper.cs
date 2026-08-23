namespace BrewUp.Dashboards.Infrastructure.Hubs;

public interface IDashboardsHubHelper
{
    Task TellChildrenThatCustomersDashboardWasUpdated(string customerId, CancellationToken cancellationToken);
    Task TellChildrenThatProductsDashboardWasUpdated(string productId, CancellationToken cancellationToken);
}