using System.Linq.Expressions;
using BrewUp.Shared.ReadModel;
using Lena.Core;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace BrewUp.Warehouse.ReadModel.Queries;

internal sealed class WarehouseQueries(IMongoClient mongoClient) : IQueries<Dtos.Warehouse>
{
    private readonly IMongoDatabase _database = mongoClient.GetDatabase("Warehouse");
    
    public async Task<Result<Dtos.Warehouse>> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var collection = _database.GetCollection<Dtos.Warehouse>(nameof(Warehouse));
        var filter = Builders<Dtos.Warehouse>.Filter.Eq("_id", id);

        return await collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken) > 0
            ? Result<Dtos.Warehouse>.Success((await collection.FindAsync(filter, cancellationToken: cancellationToken)).First(cancellationToken: cancellationToken))
            : Result<Dtos.Warehouse>.Success(ConstructAggregate<Dtos.Warehouse>());
    }

    public async Task<Result<PagedResult<Dtos.Warehouse>>> GetByFilterAsync(
        Expression<Func<Dtos.Warehouse, bool>>? query, int page, int pageSize, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (--page < 0)
            page = 0;

        var collection = _database.GetCollection<Dtos.Warehouse>(nameof(Warehouse));
        var queryable = query != null
            ? collection.AsQueryable()
                .Where(query)
            : collection.AsQueryable();

        var count = await queryable.CountAsync(cancellationToken: cancellationToken);
        var results = await queryable.Skip(page * pageSize).Take(pageSize)
            .ToListAsync(cancellationToken: cancellationToken);

        return Result<PagedResult<Dtos.Warehouse>>.Success(
            new PagedResult<Dtos.Warehouse>(results, page, pageSize, count));
    }
    
    private static TAggregate ConstructAggregate<TAggregate>()
    {
        return (TAggregate)Activator.CreateInstance(typeof(TAggregate), true)!;
    }
}