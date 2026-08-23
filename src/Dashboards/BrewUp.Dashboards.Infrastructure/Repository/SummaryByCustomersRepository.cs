using BrewUp.Dashboards.Entities.Dtos;
using BrewUp.Dashboards.ReadModel;
using BrewUp.Dashboards.SharedKernel.Persistence;
using BrewUp.Shared.Exceptions;
using Lena.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BrewUp.Dashboards.Infrastructure.Repository;

public class SummaryByCustomersRepository(DashboardsContext dashboardsContext,
    ILoggerFactory loggerFactory) : IDashboardsRepository<SalesByCustomers>
{
    private readonly ILogger<SummaryByCustomersRepository> _logger = loggerFactory.CreateLogger<SummaryByCustomersRepository>();
    
    public async Task<Result<SalesByCustomers>> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var queryable = dashboardsContext.Set<SalesByCustomers>()
                .Where(a => a.Id.Equals(id));
            
            var result = await queryable.FirstOrDefaultAsync(cancellationToken: cancellationToken);
            
            return result != null
                ? Result<SalesByCustomers>.Success(result)
                : Result<SalesByCustomers>.Error($"Entity of type {nameof(SalesByCustomers)} with id {id} not found.");
        }
        catch (Exception ex)
        {
            UtilitiesService.LogError(ex, _logger);
            throw;
        }
    }

    public async Task AddAsync(SalesByCustomers entity, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            if (dashboardsContext.Database.ProviderName?.Contains("InMemory") != true)
            {
                await using var transaction = await dashboardsContext.Database.BeginTransactionAsync(cancellationToken);
                await AddEntityAsync(entity, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            else
            {
                // Skip transaction for InMemory provider
                await AddEntityAsync(entity, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            UtilitiesService.LogError(ex, _logger);
            throw;
        }
    }

    public async Task UpdateAsync(SalesByCustomers entity, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            if (dashboardsContext.Database.ProviderName?.Contains("InMemory") != true)
            {
                await using var transaction = await dashboardsContext.Database.BeginTransactionAsync(cancellationToken);
                await UpdateEntityAsync(entity, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            else
            {
                await UpdateEntityAsync(entity, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            UtilitiesService.LogError(ex, _logger);
            throw;
        }
    }

    public async Task DeleteAsync(SalesByCustomers entity, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        try
        {
            if (dashboardsContext.Database.ProviderName?.Contains("InMemory") != true)
            {
                await using var transaction = await dashboardsContext.Database.BeginTransactionAsync(cancellationToken);
                await DeleteEntityAsync(entity, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            else
            {
                await DeleteEntityAsync(entity, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            UtilitiesService.LogError(ex, _logger);
            throw;
        }
    }
    
    private async Task AddEntityAsync(SalesByCustomers entity, CancellationToken cancellationToken)
    {
        var dbSet = dashboardsContext.Set<SalesByCustomers>();
        await dbSet.AddAsync(entity, cancellationToken);
        await dashboardsContext.SaveChangesAsync(cancellationToken);
    }
    
    private async Task UpdateEntityAsync(SalesByCustomers entity, CancellationToken cancellationToken)
    {
        var dbSet = dashboardsContext.Set<SalesByCustomers>();
        dbSet.Update(entity);
        await dashboardsContext.SaveChangesAsync(cancellationToken);
    }
    
    private async Task DeleteEntityAsync(SalesByCustomers entity, CancellationToken cancellationToken)
    {
        dashboardsContext.Set<SalesByCustomers>().Remove(entity);
        await dashboardsContext.SaveChangesAsync(cancellationToken);
    }
    
    private static TAggregate ConstructAggregate<TAggregate>()
    {
        return (TAggregate)Activator.CreateInstance(typeof(TAggregate), true)!;
    }
}