using System.Linq.Expressions;
using BrewUp.Dashboards.Entities.Dtos;
using BrewUp.Shared.Exceptions;
using BrewUp.Shared.ReadModel;
using Lena.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BrewUp.Dashboards.ReadModel.Queries;

public sealed class SalesByCustomersQueries(DashboardsContext dashboardsContext,
    ILoggerFactory loggerFactory) : IQueries<SalesByCustomers>
{
    private readonly ILogger<SalesByCustomersQueries> _logger = loggerFactory.CreateLogger<SalesByCustomersQueries>();
    
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

    public async Task<Result<PagedResult<SalesByCustomers>>> GetByFilterAsync(
        Expression<Func<SalesByCustomers, bool>>? query, int page, int pageSize, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        if (--page < 0)
            page = 0;

        try
        {
            var queryable = query != null
                ? dashboardsContext.Set<SalesByCustomers>()
                    .Where(query)
                : dashboardsContext.Set<SalesByCustomers>();
                    
            var count = await queryable.CountAsync(cancellationToken: cancellationToken);
            var results = await queryable.Skip(page * pageSize).Take(pageSize)
                .ToListAsync(cancellationToken: cancellationToken);
            
            return Result<PagedResult<SalesByCustomers>>.Success(new PagedResult<SalesByCustomers>(results, page, pageSize, count));
        }
        catch (Exception ex)
        {
            UtilitiesService.LogError(ex, _logger);
            throw;
        }
    }
}