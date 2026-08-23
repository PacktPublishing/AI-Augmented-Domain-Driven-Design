using BrewUp.Dashboards.Entities.Dtos;
using BrewUp.Dashboards.ReadModel;
using BrewUp.Dashboards.SharedKernel.Persistence;
using BrewUp.Shared.Exceptions;
using Lena.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BrewUp.Dashboards.Infrastructure.Repository;

public class MessagesReceivedRepository(DashboardsContext dashboardsContext,
    ILoggerFactory loggerFactory) : IDashboardsRepository<MessagesReceived>
{
    private readonly ILogger<MessagesReceivedRepository> _logger = loggerFactory.CreateLogger<MessagesReceivedRepository>();
    
    public async Task<Result<MessagesReceived>> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var queryable = dashboardsContext.Set<MessagesReceived>()
                .Where(a => a.Id.Equals(id));
            
            var result = await queryable.FirstOrDefaultAsync(cancellationToken: cancellationToken);
            
            return result != null
                ? Result<MessagesReceived>.Success(result)
                : Result<MessagesReceived>.Error($"Entity of type {nameof(MessagesReceived)} with id {id} not found.");
        }
        catch (Exception ex)
        {
            UtilitiesService.LogError(ex, _logger);
            throw;
        }
    }

    public async Task AddAsync(MessagesReceived entity, CancellationToken cancellationToken)
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

    public async Task UpdateAsync(MessagesReceived entity, CancellationToken cancellationToken)
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

    public async Task DeleteAsync(MessagesReceived entity, CancellationToken cancellationToken)
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
    
    private async Task AddEntityAsync(MessagesReceived entity, CancellationToken cancellationToken)
    {
        var dbSet = dashboardsContext.Set<MessagesReceived>();
        await dbSet.AddAsync(entity, cancellationToken);
        await dashboardsContext.SaveChangesAsync(cancellationToken);
    }
    
    private async Task UpdateEntityAsync(MessagesReceived entity, CancellationToken cancellationToken)
    {
        var dbSet = dashboardsContext.Set<MessagesReceived>();
        dbSet.Update(entity);
        await dashboardsContext.SaveChangesAsync(cancellationToken);
    }
    
    private async Task DeleteEntityAsync(MessagesReceived entity, CancellationToken cancellationToken)
    {
        dashboardsContext.Set<MessagesReceived>().Remove(entity);
        await dashboardsContext.SaveChangesAsync(cancellationToken);
    }

}