using System.Linq.Expressions;
using BrewUp.Sales.ReadModel.Dtos;
using BrewUp.Shared.ReadModel;
using Lena.Core;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace BrewUp.Sales.ReadModel.Queries;

internal sealed class SalesOrderQueries(IMongoClient mongoClient) : IQueries<SalesOrder>
{
    private readonly IMongoDatabase _database = mongoClient.GetDatabase("Sales");

    public async Task<Result<SalesOrder>> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var collection = _database.GetCollection<SalesOrder>(nameof(SalesOrder));
        var filter = Builders<SalesOrder>.Filter.Eq("_id", id);
        
        return await collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken) > 0
            ? Result<SalesOrder>.Success((await collection.FindAsync(filter, cancellationToken: cancellationToken)).First(cancellationToken: cancellationToken))
            : Result<SalesOrder>.Success(ConstructAggregate<SalesOrder>());
    }

    public async Task<Result<PagedResult<SalesOrder>>> GetByFilterAsync(Expression<Func<SalesOrder, bool>>? query, int page, int pageSize, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        if (--page < 0)
            page = 0;

        var collection = _database.GetCollection<SalesOrder>(nameof(SalesOrder));
        var queryable = query != null
            ? collection.AsQueryable()
                .Where(query)
            : collection.AsQueryable();

        var count = await queryable.CountAsync(cancellationToken: cancellationToken);
        var results = await queryable.Skip(page * pageSize).Take(pageSize).ToListAsync(cancellationToken: cancellationToken);

        return Result<PagedResult<SalesOrder>>.Success(new PagedResult<SalesOrder>(results, page, pageSize, count));
    }
    
    private static TAggregate ConstructAggregate<TAggregate>()
    {
        return (TAggregate)Activator.CreateInstance(typeof(TAggregate), true)!;
    }
}