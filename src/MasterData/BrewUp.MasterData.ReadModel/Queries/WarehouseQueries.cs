using System.Linq.Expressions;
using BrewUp.MasterData.Entities.Dtos;
using BrewUp.Shared.ReadModel;
using Lena.Core;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace BrewUp.MasterData.ReadModel.Queries;

internal sealed class WarehouseQueries(IMongoClient mongoClient) : IQueries<Warehouse>
{
    private readonly IMongoDatabase _database = mongoClient.GetDatabase("MasterData");
    
    public async Task<Result<Warehouse>> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var collection = _database.GetCollection<Warehouse>(nameof(Warehouse));
        var filter = Builders<Warehouse>.Filter.Eq("_id", id);
        
        return await collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken) > 0
            ? Result<Warehouse>.Success((await collection.FindAsync(filter, cancellationToken: cancellationToken)).First(cancellationToken: cancellationToken))
            : Result<Warehouse>.Success(ConstructAggregate<Warehouse>());
    }

    public async Task<Result<PagedResult<Warehouse>>> GetByFilterAsync(
        Expression<Func<Warehouse, bool>>? query, int page, int pageSize, CancellationToken cancellationToken)
    {
        if (--page < 0)
            page = 0;

        var collection = _database.GetCollection<Warehouse>(nameof(Warehouse));
        var queryable = query != null
            ? collection.AsQueryable()
                .Where(query)
            : collection.AsQueryable();

        var count = await queryable.CountAsync(cancellationToken: cancellationToken);
        var results = await queryable.Skip(page * pageSize).Take(pageSize).ToListAsync(cancellationToken: cancellationToken);

        return Result<PagedResult<Warehouse>>.Success(new PagedResult<Warehouse>(results, page, pageSize, count));
    }
    
    private static TAggregate ConstructAggregate<TAggregate>()
    {
        return (TAggregate)Activator.CreateInstance(typeof(TAggregate), true)!;
    }
}