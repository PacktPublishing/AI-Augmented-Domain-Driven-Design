using System.Linq.Expressions;
using BrewUp.Sales.ReadModel.Dtos;
using BrewUp.Shared.ReadModel;
using Lena.Core;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace BrewUp.Sales.ReadModel.Queries;

internal sealed class SalesOrderSummaryQueries(IMongoClient mongoClient) : IQueries<SalesOrderSummary>
{
    private readonly IMongoDatabase _database = mongoClient.GetDatabase("Sales");
    
    public async Task<Result<SalesOrderSummary>> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var collection = _database.GetCollection<SalesOrderSummary>(nameof(SalesOrderSummary));
        var filter = Builders<SalesOrderSummary>.Filter.Eq("_id", id);
        
        return await collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken) > 0
            ? Result<SalesOrderSummary>.Success((await collection.FindAsync(filter, cancellationToken: cancellationToken)).First(cancellationToken: cancellationToken))
            : Result<SalesOrderSummary>.Success(ConstructAggregate<SalesOrderSummary>());
    }

    public async Task<Result<PagedResult<SalesOrderSummary>>> GetByFilterAsync(Expression<Func<SalesOrderSummary, bool>>? query, int page, int pageSize, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        if (--page < 0)
            page = 0;

        var collection = _database.GetCollection<SalesOrderSummary>(nameof(SalesOrderSummary));
        var queryable = query != null
            ? collection.AsQueryable()
                .Where(query)
            : collection.AsQueryable();

        var count = await queryable.CountAsync(cancellationToken: cancellationToken);
        var results = await queryable.Skip(page * pageSize).Take(pageSize).ToListAsync(cancellationToken: cancellationToken);

        return Result<PagedResult<SalesOrderSummary>>.Success(new PagedResult<SalesOrderSummary>(results, page, pageSize, count));
    }
    
    private static TAggregate ConstructAggregate<TAggregate>()
    {
        return (TAggregate)Activator.CreateInstance(typeof(TAggregate), true)!;
    }
}