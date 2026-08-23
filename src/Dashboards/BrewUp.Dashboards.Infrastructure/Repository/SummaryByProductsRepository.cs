using BrewUp.Dashboards.Entities.Dtos;
using BrewUp.Dashboards.ReadModel;
using BrewUp.Dashboards.SharedKernel.Persistence;
using BrewUp.Shared.Exceptions;
using Lena.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BrewUp.Dashboards.Infrastructure.Repository;

public class SummaryByProductsRepository(DashboardsContext dashboardsContext,
    ILoggerFactory loggerFactory) : IDashboardsRepository<SalesByProducts>
{
    private readonly ILogger<SummaryByProductsRepository> _logger = loggerFactory.CreateLogger<SummaryByProductsRepository>();
    
    public async Task<Result<SalesByProducts>> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var queryable = dashboardsContext.Set<SalesByProducts>()
                .Where(a => a.Id.Equals(id));
            
            var result = await queryable.FirstOrDefaultAsync(cancellationToken: cancellationToken);
            
            return result != null
                ? Result<SalesByProducts>.Success(result)
                : Result<SalesByProducts>.Error($"Entity of type {nameof(SalesByProducts)} with id {id} not found.");
        }
        catch (Exception ex)
        {
            UtilitiesService.LogError(ex, _logger);
            throw;
        }
    }

    public async Task AddAsync(SalesByProducts entity, CancellationToken cancellationToken)
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

    public async Task UpdateAsync(SalesByProducts entity, CancellationToken cancellationToken)
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

    public async Task DeleteAsync(SalesByProducts entity, CancellationToken cancellationToken)
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
    
    private async Task AddEntityAsync(SalesByProducts entity, CancellationToken cancellationToken)
    {
        var dbSet = dashboardsContext.Set<SalesByProducts>();
        await dbSet.AddAsync(entity, cancellationToken);
        await dashboardsContext.SaveChangesAsync(cancellationToken);
    }
    
    private async Task UpdateEntityAsync(SalesByProducts entity, CancellationToken cancellationToken)
    {
        var dbSet = dashboardsContext.Set<SalesByProducts>();
        dbSet.Update(entity);
        await dashboardsContext.SaveChangesAsync(cancellationToken);
    }
    
    private async Task DeleteEntityAsync(SalesByProducts entity, CancellationToken cancellationToken)
    {
        dashboardsContext.Set<SalesByProducts>().Remove(entity);
        await dashboardsContext.SaveChangesAsync(cancellationToken);
    }
}