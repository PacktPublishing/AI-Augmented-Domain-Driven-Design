using System.Linq.Expressions;
using BrewUp.Dashboards.Entities.Dtos;
using BrewUp.Shared.Exceptions;
using BrewUp.Shared.ReadModel;
using Lena.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BrewUp.Dashboards.ReadModel.Queries;

public sealed class SalesByProductsQueries(DashboardsContext dashboardsContext,
    ILoggerFactory loggerFactory) : IQueries<SalesByProducts>
{
    private readonly ILogger<SalesByCustomersQueries> _logger = loggerFactory.CreateLogger<SalesByCustomersQueries>();
    
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

    public async Task<Result<PagedResult<SalesByProducts>>> GetByFilterAsync(
        Expression<Func<SalesByProducts, bool>>? query, int page, int pageSize, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        if (--page < 0)
            page = 0;

        try
        {
            var queryable = query != null
                ? dashboardsContext.Set<SalesByProducts>()
                    .Where(query)
                : dashboardsContext.Set<SalesByProducts>();
                    
            var count = await queryable.CountAsync(cancellationToken: cancellationToken);
            var results = await queryable.Skip(page * pageSize).Take(pageSize)
                .ToListAsync(cancellationToken: cancellationToken);
            
            return Result<PagedResult<SalesByProducts>>.Success(new PagedResult<SalesByProducts>(results, page, pageSize, count));
        }
        catch (Exception ex)
        {
            UtilitiesService.LogError(ex, _logger);
            throw;
        }
    }
}